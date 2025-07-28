using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;
using System.Reflection;

namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository : IGenericRepositoryInterface<Country>
    {
        private readonly string _connectionString;
        private readonly ILogger<CountryRepository> _logger;
        private readonly AppDbContext appDbContext;

        // Once we Injected these in the server, the DI takes care of providing these in the constructors
        public CountryRepository(AppDbContext context, ILogger<CountryRepository> logger)
        {
            appDbContext = context;
            _connectionString = context.Database.GetConnectionString();
            _logger = logger;
        }

        // ------------------------ GET ALL ----------------------
        public async Task<List<Country>> GetAll()
        {
            const string methodName = nameof(GetAll);
            _logger.LogInformation("{Method} initiated", methodName);

            var countries = new List<Country>();

            try
            {
                // Using `using` is important, as it helps in Disposing, it Automatically disposes the object when the block ends
                /* Database connections are limited resources (connection pool exhaustion risk) 
                 * Without using, you might leak connections, leading to: TimeoutException
                 * Commands and readers also hold database resources
                    They must be disposed to release:

                    Server-side cursors

                    Network buffers

                    Locked resources
                 */
                _logger.LogInformation("Starting to fetch all countries");
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT Id, Name FROM Countries";

                using var command = new SqlCommand(sql, connection);

                using var reader = await command.ExecuteReaderAsync();
                // while reader reads through the results
                while (await reader.ReadAsync())
                {
                    countries.Add(new Country
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    });
                }

                _logger.LogInformation("{Method} completed. Retrieved {Count} countries",
                    methodName, countries.Count);

                return countries;

            }
            catch (SqlException sqlEx)
            {
                // Specific handling for SQL errors
                _logger.LogError($"Database error in GetAll: {sqlEx.Message}");
                throw new RepositoryException("Database error while fetching countries", sqlEx);
            }
            catch (InvalidOperationException ioEx)
            {
                // Connection string or connection problems
                _logger.LogError($"Connection error in GetAll: {ioEx.Message}");
                throw new RepositoryException("Connection problem while fetching countries", ioEx);
            }
            catch (Exception ex)
            {
                // Catch-all for other exceptions
                _logger.LogError($"Unexpected error in GetAll: {ex.Message}");
                throw new RepositoryException("Unexpected error while fetching countries", ex);
            }

            return countries;
        }

        // --------------- GET BY ID -------------------------
        public async Task<Country> GetById(int id)
        {
            const string methodName = nameof(GetById);
            _logger.LogInformation("{Method} initiated for Id {id}", methodName, id);

            try
            {
                // connected to the database
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sqlQ = "SELECT Id, Name From Countries WHERE Id = @Id";

                // writing command to database
                using var command = new SqlCommand(sqlQ, connection);
                // setting the variable value in the query
                command.Parameters.AddWithValue("@Id", id);

                using var reader = await command.ExecuteReaderAsync();

                if(await reader.ReadAsync())
                {
                    return new Country
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                    };
                }

                _logger.LogWarning("{Method}: Country with ID {Id} not found", methodName, id);
                return null;
            }
            catch (SqlException sqlEx)
            {
                // Specific handling for SQL errors
                _logger.LogError($"Database error in GetAll: {sqlEx.Message}");
                throw new RepositoryException("Database error while fetching countries", sqlEx);
            }
            catch (InvalidOperationException ioEx)
            {
                // Connection string or connection problems
                _logger.LogError($"Connection error in GetAll: {ioEx.Message}");
                throw new RepositoryException("Connection problem while fetching countries", ioEx);
            }
            catch (Exception ex)
            {
                // Catch-all for other exceptions
                _logger.LogError($"Unexpected error in GetAll: {ex.Message}");
                throw new RepositoryException("Unexpected error while fetching countries", ex);
            }
        }


        // ---------------- INSERT ---------------
        public async Task<GeneralResponse> Insert(Country item)
        {
            const string methodName = nameof(Insert);
            _logger.LogInformation("{Method} initiated for {CountryName}",
                methodName, item.Name);

            try
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    _logger.LogWarning("{Method}: Empty country name provided", methodName);
                    return new GeneralResponse(false, "Country name cannot be empty");
                }

                if (await CountryExists(item.Name))
                {
                    _logger.LogWarning("{Method}: Country {CountryName} already exists",
                        methodName, item.Name);
                    return new GeneralResponse(false, "Country already exists");
                }

                // connection started
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // starting a transaction
                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    const string sqlQ = @"INSERT INTO Countries (Name)
                                          VALUES (@Name);
                                          SELECT SCOPE_IDENTITY();";

                    using var command = new SqlCommand(sqlQ, connection, transaction);
                    command.Parameters.AddWithValue("@Name", item.Name.Trim());

                    var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    await transaction.CommitAsync();

                    _logger.LogInformation("{Method}: Country {CountryName} created with ID {Id}",
                        methodName, item.Name, newId);

                    return new GeneralResponse(true, "Country created successfully");

                }
                catch (Exception ex)
                {
                    // Rolling back the changes
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for {CountryName}",
                        methodName, item.Name);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                // Specific handling for SQL errors
                _logger.LogError($"Database error in GetAll: {sqlEx.Message}");
                throw new RepositoryException("Database error while fetching countries", sqlEx);
            }
            catch (InvalidOperationException ioEx)
            {
                // Connection string or connection problems
                _logger.LogError($"Connection error in GetAll: {ioEx.Message}");
                throw new RepositoryException("Connection problem while fetching countries", ioEx);
            }
            catch (Exception ex)
            {
                // Catch-all for other exceptions
                _logger.LogError($"Unexpected error in GetAll: {ex.Message}");
                throw new RepositoryException("Unexpected error while fetching countries", ex);
            }

        }

        // --------------- UPDATE -----------------------
        public async Task<GeneralResponse> Update(Country item)
        {
            const string methodName = nameof(Update);
            _logger.LogInformation("{Method} initiated for ID {Id}", methodName, item.Id);

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    _logger.LogWarning("{Method}: Empty country name provided for ID {Id}",
                        methodName, item.Id);
                    return new GeneralResponse(false, "Country name cannot be empty");
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    // Verify country exists
                    const string checkSql = "SELECT COUNT(*) FROM Countries WHERE Id = @Id";

                    using var checkCommand = new SqlCommand(checkSql, connection, transaction);
                    checkCommand.Parameters.AddWithValue("@Id", item.Id);

                    // Returns: Number of rows affected (as int) for single values best for this
                    // not for update aand delete operations
                    var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;
                    if (!exists)
                    {
                        _logger.LogWarning("{Method}: Country ID {Id} not found",
                            methodName, item.Id);
                        return new GeneralResponse(false, "Country not found");
                    }

                    // Update country
                    const string updateSql = @"
                        UPDATE Countries 
                        SET Name = @Name 
                        WHERE Id = @Id";

                    using var updateCommand = new SqlCommand(updateSql, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@Id", item.Id);
                    updateCommand.Parameters.AddWithValue("@Name", item.Name.Trim());
                    updateCommand.CommandTimeout = 30;

                    // This is used because --> Returns: Number of rows affected (as int)
                    await updateCommand.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("{Method}: Country ID {Id} updated successfully",
                        methodName, item.Id);

                    return new GeneralResponse(true, "Country updated successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for ID {Id}",
                        methodName, item.Id);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                // Specific handling for SQL errors
                _logger.LogError($"Database error in GetAll: {sqlEx.Message}");
                throw new RepositoryException("Database error while fetching countries", sqlEx);
            }
            catch (InvalidOperationException ioEx)
            {
                // Connection string or connection problems
                _logger.LogError($"Connection error in GetAll: {ioEx.Message}");
                throw new RepositoryException("Connection problem while fetching countries", ioEx);
            }
            catch (Exception ex)
            {
                // Catch-all for other exceptions
                _logger.LogError($"Unexpected error in GetAll: {ex.Message}");
                throw new RepositoryException("Unexpected error while fetching countries", ex);
            }
        }

        // ---------------------- DELETE ------------------------
        public async Task<GeneralResponse> DeleteById(int id)
        {
            const string methodName = nameof(DeleteById);
            _logger.LogInformation("{Method} initiated for ID {Id}", methodName, id);

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

                try
                {
                    const string checkCitiesSql = @"
                        SELECT COUNT(*) 
                        FROM Cities 
                        WHERE CountryId = @CountryId";

                    using var checkCommand = new SqlCommand(checkCitiesSql, connection, transaction);
                    checkCommand.Parameters.AddWithValue("@CountryId", id);

                    var hasCities = (int)await checkCommand.ExecuteScalarAsync() > 0;
                    if (hasCities)
                    {
                        _logger.LogWarning("{Method}: Country ID {Id} has dependent cities",
                            methodName, id);
                        return new GeneralResponse(
                            false,
                            "Cannot delete country with associated cities");
                    }

                    // Delete country
                    const string deleteSql = "DELETE FROM Countries WHERE Id = @Id";

                    using var deleteCommand = new SqlCommand(deleteSql, connection, transaction);
                    deleteCommand.Parameters.AddWithValue("@Id", id);

                    // This is used because --> Returns: Number of rows affected (as int)
                    var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning("{Method}: Country ID {Id} not found", methodName, id);
                        return new GeneralResponse(false, "Country not found");
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("{Method}: Country ID {Id} deleted successfully",
                        methodName, id);

                    return new GeneralResponse(true, "Country deleted successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "{Method} transaction failed for ID {Id}",
                        methodName, id);
                    throw;
                }
            }
            catch (SqlException sqlEx)
            {
                // Specific handling for SQL errors
                _logger.LogError($"Database error in GetAll: {sqlEx.Message}");
                throw new RepositoryException("Database error while fetching countries", sqlEx);
            }
            catch (InvalidOperationException ioEx)
            {
                // Connection string or connection problems
                _logger.LogError($"Connection error in GetAll: {ioEx.Message}");
                throw new RepositoryException("Connection problem while fetching countries", ioEx);
            }
            catch (Exception ex)
            {
                // Catch-all for other exceptions
                _logger.LogError($"Unexpected error in GetAll: {ex.Message}");
                throw new RepositoryException("Unexpected error while fetching countries", ex);
            }
        }

        // -------------------- HELPERS --------------------
        private static GeneralResponse NotFound() => new(false, "Sorry Country not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Countries.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }

        private async Task<bool> CountryExists(string name)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    SELECT COUNT(*) 
                    FROM Countries 
                    WHERE LOWER(Name) = LOWER(@Name)";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Name", name.Trim());

                return (int)await command.ExecuteScalarAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Country existence check failed for {CountryName}", name);
                throw;
            }
        }
    }
}
