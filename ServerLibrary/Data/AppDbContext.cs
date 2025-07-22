using BaseLibrary.Entities;
using BaseLibrary.Entities.OtherEntities.Doctor;
using BaseLibrary.Entities.OtherEntities.Overtime;
using BaseLibrary.Entities.OtherEntities.Sanction;
using BaseLibrary.Entities.OtherEntities.Vacation;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }

        // General Departments / Departments / Branches
        public DbSet<GeneralDepartment> GeneralDepartments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }

        // Country / Cities / Towns
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Town> Towns { get; set; }


        // Authentication 
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }  
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

        // Other Tables - Overtime / Vacations / Sanctions / Doctor
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<OvertimeType> OvertimeTypes { get; set; }

        public DbSet<Sanction> Sanctions { get; set; }
        public DbSet<SanctionType> SanctionTypes { get; set; }

        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

    }
}
