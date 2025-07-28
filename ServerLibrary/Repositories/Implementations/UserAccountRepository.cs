using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly string _connectionString;
        private readonly ILogger<UserAccountRepository> _logger;
        private readonly AppDbContext _appDbContext;
        private readonly JwtSection _jwtConfig;

        public UserAccountRepository(
            IOptions<JwtSection> config,
            AppDbContext appDbContext,
            ILogger<UserAccountRepository> logger)
        {
            _appDbContext = appDbContext;
            _connectionString = appDbContext.Database.GetConnectionString();
            _logger = logger;
            _jwtConfig = config.Value;
        }

        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            const string methodName = nameof(CreateAsync);
            _logger.LogInformation("{Method} initiated for email: {Email}", methodName, user.Email);

            if (user == null)
            {
                _logger.LogWarning("{Method}: Model is empty", methodName);
                return new GeneralResponse(false, "Model is empty");
            }

            try
            {
                // Check if user exists
                if (await UserExists(user.Email!))
                {
                    _logger.LogWarning("{Method}: User with email {Email} already exists", methodName, user.Email);
                    return new GeneralResponse(false, "User already exists");
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    // Insert user
                    var insertUserSql = @"
                        INSERT INTO ApplicationUsers (Fullname, Email, Password)
                        VALUES (@Fullname, @Email, @Password);
                        SELECT SCOPE_IDENTITY();";

                    using var userCommand = new SqlCommand(insertUserSql, connection, transaction);
                    userCommand.Parameters.AddWithValue("@Fullname", user.Fullname);
                    userCommand.Parameters.AddWithValue("@Email", user.Email);
                    userCommand.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(user.Password!));

                    var userId = Convert.ToInt32(await userCommand.ExecuteScalarAsync());

                    // Check and assign role
                    var adminRoleId = await GetRoleId(Constants.Admin, connection, transaction);
                    var userRoleId = await GetRoleId(Constants.User, connection, transaction);

                    if (adminRoleId == 0) // No admin role exists
                    {
                        adminRoleId = await CreateRole(Constants.Admin, connection, transaction);
                        await AssignRole(userId, adminRoleId, connection, transaction);
                        _logger.LogInformation("{Method}: First user created and assigned as Admin", methodName);
                        await transaction.CommitAsync();
                        return new GeneralResponse(true, "User created successfully and assigned as Admin");
                    }

                    // No user role exist
                    if (userRoleId == 0)
                    {
                        userRoleId = await CreateRole(Constants.User, connection, transaction);
                        _logger.LogInformation("{Method}: User role created with ID {RoleId}", methodName, userRoleId);

                    }

                    // Assign user role
                    await AssignRole(userId, userRoleId, connection, transaction);
                    await transaction.CommitAsync();

                    _logger.LogInformation("{Method}: User created successfully with ID {UserId}", methodName, userId);
                    return new GeneralResponse(true, "User created successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for email {Email}", methodName, user.Email);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error for email {Email}", methodName, user.Email);
                return new GeneralResponse(false, "Database error while creating user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error for email {Email}", methodName, user.Email);
                return new GeneralResponse(false, "Unexpected error while creating user");
            }
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            const string methodName = nameof(SignInAsync);
            _logger.LogInformation("{Method} initiated for email: {Email}", methodName, user.Email);

            if (user == null)
            {
                _logger.LogWarning("{Method}: Model is empty", methodName);
                return new LoginResponse(false, "Model is empty");
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Get user by email
                var getUserSql = @"
                    SELECT Id, Fullname, Email, Password 
                    FROM ApplicationUsers 
                    WHERE Email = @Email";

                using var userCommand = new SqlCommand(getUserSql, connection);
                userCommand.Parameters.AddWithValue("@Email", user.Email);

                using var userReader = await userCommand.ExecuteReaderAsync();

                if (!await userReader.ReadAsync())
                {
                    _logger.LogWarning("{Method}: User with email {Email} not found", methodName, user.Email);
                    return new LoginResponse(false, "User not found");
                }

                var applicationUser = new ApplicationUser
                {
                    Id = userReader.GetInt32(userReader.GetOrdinal("Id")),
                    Fullname = userReader.GetString(userReader.GetOrdinal("Fullname")),
                    Email = userReader.GetString(userReader.GetOrdinal("Email")),
                    Password = userReader.GetString(userReader.GetOrdinal("Password"))
                };

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(user.Password!, applicationUser.Password))
                {
                    _logger.LogWarning("{Method}: Invalid password for email {Email}", methodName, user.Email);
                    return new LoginResponse(false, "Email/Password is invalid");
                }

                // Get user role
                var roleName = await GetUserRoleName(applicationUser.Id);
                if (string.IsNullOrEmpty(roleName))
                {
                    _logger.LogWarning("{Method}: No role found for user ID {UserId}", methodName, applicationUser.Id);
                    return new LoginResponse(false, "User role not found");
                }

                // Generate tokens
                var jwtToken = GenerateToken(applicationUser, roleName);
                var refreshToken = GenerateRefreshToken();

                // Save refresh token
                await SaveRefreshToken(applicationUser.Id, refreshToken);

                _logger.LogInformation("{Method}: User {Email} signed in successfully", methodName, user.Email);
                return new LoginResponse(true, "Login successful", jwtToken, refreshToken);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error for email {Email}", methodName, user.Email);
                return new LoginResponse(false, "Database error during sign in");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error for email {Email}", methodName, user.Email);
                return new LoginResponse(false, "Unexpected error during sign in");
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            const string methodName = nameof(RefreshTokenAsync);
            _logger.LogInformation("{Method} initiated", methodName);

            if (token == null)
            {
                _logger.LogWarning("{Method}: Refresh token model is empty", methodName);
                return new LoginResponse(false, "Model is empty");
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Find refresh token
                var findTokenSql = @"
                    SELECT UserId 
                    FROM RefreshTokenInfos 
                    WHERE Token = @Token";

                using var tokenCommand = new SqlCommand(findTokenSql, connection);
                tokenCommand.Parameters.AddWithValue("@Token", token.Token);

                var userId = await tokenCommand.ExecuteScalarAsync() as int?;
                if (!userId.HasValue)
                {
                    _logger.LogWarning("{Method}: Refresh token not found", methodName);
                    return new LoginResponse(false, "Refresh token not found");
                }

                // Get user details
                var getUserSql = "SELECT Id, Fullname, Email FROM ApplicationUsers WHERE Id = @UserId";
                using var userCommand = new SqlCommand(getUserSql, connection);
                userCommand.Parameters.AddWithValue("@UserId", userId.Value);

                using var userReader = await userCommand.ExecuteReaderAsync();
                if (!await userReader.ReadAsync())
                {
                    _logger.LogWarning("{Method}: User with ID {UserId} not found", methodName, userId);
                    return new LoginResponse(false, "User not found");
                }

                var user = new ApplicationUser
                {
                    Id = userReader.GetInt32(userReader.GetOrdinal("Id")),
                    Fullname = userReader.GetString(userReader.GetOrdinal("Fullname")),
                    Email = userReader.GetString(userReader.GetOrdinal("Email"))
                };

                // Get user role
                var roleName = await GetUserRoleName(user.Id);
                if (string.IsNullOrEmpty(roleName))
                {
                    _logger.LogWarning("{Method}: No role found for user ID {UserId}", methodName, user.Id);
                    return new LoginResponse(false, "User role not found");
                }

                // Generate new tokens
                var newJwtToken = GenerateToken(user, roleName);
                var newRefreshToken = GenerateRefreshToken();

                // Update refresh token
                await UpdateRefreshToken(user.Id, newRefreshToken);

                _logger.LogInformation("{Method}: Tokens refreshed successfully for user ID {UserId}", methodName, user.Id);
                return new LoginResponse(true, "New Token generated successfully", newJwtToken, newRefreshToken);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error", methodName);
                return new LoginResponse(false, "Database error during token refresh");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error", methodName);
                return new LoginResponse(false, "Unexpected error during token refresh");
            }
        }

        public async Task<List<ManageUser>> GetUsers()
        {
            const string methodName = nameof(GetUsers);
            _logger.LogInformation("{Method} initiated", methodName);

            var users = new List<ManageUser>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"
                    SELECT u.Id as UserId, u.Fullname as Name, u.Email, r.Name as Role
                    FROM ApplicationUsers u
                    JOIN UserRoles ur ON u.Id = ur.UserId
                    JOIN SystemRoles r ON ur.RoleId = r.Id";

                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new ManageUser
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Role = reader.GetString(reader.GetOrdinal("Role"))
                    });
                }

                _logger.LogInformation("{Method} completed. Retrieved {Count} users", methodName, users.Count);
                return users;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error", methodName);
                throw new RepositoryException("Database error while fetching users", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error", methodName);
                throw new RepositoryException("Unexpected error while fetching users", ex);
            }
        }

        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            const string methodName = nameof(UpdateUser);
            _logger.LogInformation("{Method} initiated for user ID {UserId}", methodName, user.UserId);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    // Get role ID
                    var getRoleIdSql = "SELECT Id FROM SystemRoles WHERE Name = @RoleName";
                    using var roleCommand = new SqlCommand(getRoleIdSql, connection, transaction);
                    roleCommand.Parameters.AddWithValue("@RoleName", user.Role);

                    var roleId = await roleCommand.ExecuteScalarAsync() as int?;
                    if (!roleId.HasValue)
                    {
                        _logger.LogWarning("{Method}: Role {RoleName} not found", methodName, user.Role);
                        return new GeneralResponse(false, "Role not found");
                    }

                    // Update user role
                    var updateRoleSql = @"
                        UPDATE UserRoles 
                        SET RoleId = @RoleId 
                        WHERE UserId = @UserId";

                    using var updateCommand = new SqlCommand(updateRoleSql, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@RoleId", roleId.Value);
                    updateCommand.Parameters.AddWithValue("@UserId", user.UserId);

                    var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning("{Method}: User with ID {UserId} not found", methodName, user.UserId);
                        return new GeneralResponse(false, "User not found");
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("{Method}: User role updated successfully for ID {UserId}", methodName, user.UserId);
                    return new GeneralResponse(true, "User role updated successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for user ID {UserId}", methodName, user.UserId);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error for user ID {UserId}", methodName, user.UserId);
                return new GeneralResponse(false, "Database error while updating user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error for user ID {UserId}", methodName, user.UserId);
                return new GeneralResponse(false, "Unexpected error while updating user");
            }
        }

        public async Task<List<SystemRole>> GetRoles()
        {
            const string methodName = nameof(GetRoles);
            _logger.LogInformation("{Method} initiated", methodName);

            var roles = new List<SystemRole>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "SELECT Id, Name FROM SystemRoles";
                using var command = new SqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new SystemRole
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    });
                }

                _logger.LogInformation("{Method} completed. Retrieved {Count} roles", methodName, roles.Count);
                return roles;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error", methodName);
                throw new RepositoryException("Database error while fetching roles", sqlEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error", methodName);
                throw new RepositoryException("Unexpected error while fetching roles", ex);
            }
        }

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            const string methodName = nameof(DeleteUser);
            _logger.LogInformation("{Method} initiated for user ID {Id}", methodName, id);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    // Delete refresh tokens first
                    var deleteTokensSql = "DELETE FROM RefreshTokenInfos WHERE UserId = @UserId";
                    using var tokensCommand = new SqlCommand(deleteTokensSql, connection, transaction);
                    tokensCommand.Parameters.AddWithValue("@UserId", id);
                    await tokensCommand.ExecuteNonQueryAsync();

                    // Delete user roles
                    var deleteRolesSql = "DELETE FROM UserRoles WHERE UserId = @UserId";
                    using var rolesCommand = new SqlCommand(deleteRolesSql, connection, transaction);
                    rolesCommand.Parameters.AddWithValue("@UserId", id);
                    await rolesCommand.ExecuteNonQueryAsync();

                    // Delete user
                    var deleteUserSql = "DELETE FROM ApplicationUsers WHERE Id = @UserId";
                    using var userCommand = new SqlCommand(deleteUserSql, connection, transaction);
                    userCommand.Parameters.AddWithValue("@UserId", id);

                    var rowsAffected = await userCommand.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning("{Method}: User with ID {Id} not found", methodName, id);
                        return new GeneralResponse(false, "User not found");
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("{Method}: User with ID {Id} deleted successfully", methodName, id);
                    return new GeneralResponse(true, "User deleted successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for user ID {Id}", methodName, id);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "{Method} SQL error for user ID {Id}", methodName, id);
                return new GeneralResponse(false, "Database error while deleting user");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Method} unexpected error for user ID {Id}", methodName, id);
                return new GeneralResponse(false, "Unexpected error while deleting user");
            }
        }

       // Private Helper Methods

        private async Task<bool> UserExists(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT COUNT(*) 
                    FROM ApplicationUsers 
                    WHERE Email = @Email";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);

                return (int)await command.ExecuteScalarAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User existence check failed for email {Email}", email);
                throw;
            }
        }

        private async Task<int> GetRoleId(string roleName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = "SELECT Id FROM SystemRoles WHERE Name = @RoleName";
            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@RoleName", roleName);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        private async Task<int> CreateRole(string roleName, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"
                INSERT INTO SystemRoles (Name)
                VALUES (@RoleName);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@RoleName", roleName);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        private async Task AssignRole(int userId, int roleId, SqlConnection connection, SqlTransaction transaction)
        {
            const string sql = @"
                INSERT INTO UserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId)";

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleId", roleId);

            await command.ExecuteNonQueryAsync();
        }

        private async Task<string> GetUserRoleName(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT r.Name
                    FROM UserRoles ur
                    JOIN SystemRoles r ON ur.RoleId = r.Id
                    WHERE ur.UserId = @UserId";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                return (await command.ExecuteScalarAsync())?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get role name for user ID {UserId}", userId);
                return null;
            }
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!),
            };
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task SaveRefreshToken(int userId, string refreshToken)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    // Check if token exists
                    var checkSql = "SELECT COUNT(*) FROM RefreshTokenInfos WHERE UserId = @UserId";
                    using var checkCommand = new SqlCommand(checkSql, connection, transaction);
                    checkCommand.Parameters.AddWithValue("@UserId", userId);

                    var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;

                    if (exists)
                    {
                        // Update existing token
                        var updateSql = "UPDATE RefreshTokenInfos SET Token = @Token WHERE UserId = @UserId";
                        using var updateCommand = new SqlCommand(updateSql, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@Token", refreshToken);
                        updateCommand.Parameters.AddWithValue("@UserId", userId);
                        await updateCommand.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        // Insert new token
                        var insertSql = "INSERT INTO RefreshTokenInfos (UserId, Token) VALUES (@UserId, @Token)";
                        using var insertCommand = new SqlCommand(insertSql, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@UserId", userId);
                        insertCommand.Parameters.AddWithValue("@Token", refreshToken);
                        await insertCommand.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save refresh token for user ID {UserId}", userId);
                throw;
            }
        }

        private async Task UpdateRefreshToken(int userId, string newRefreshToken)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = "UPDATE RefreshTokenInfos SET Token = @Token WHERE UserId = @UserId";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Token", newRefreshToken);
                command.Parameters.AddWithValue("@UserId", userId);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No refresh token found to update for user ID {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update refresh token for user ID {UserId}", userId);
                throw;
            }
        }

    }
}