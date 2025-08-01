﻿using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Department>
    {
        // We are using AsNoTracking, to say to dotnet to dont trrack the request, as we only need to get the data
        // while tracking helps in updating data, it is not needed here.
        // Hence it will improve the performance
        public async Task<List<Department>> GetAll() => await appDbContext.Departments.AsNoTracking()
                                                                                        .Include(gd => gd.GeneralDepartment)
                                                                                        .ToListAsync();

        public async Task<Department> GetById(int id) => await appDbContext.Departments.FindAsync(id);

        public async Task<GeneralResponse> Insert(Department item)
        {
            if (!await CheckName(item.Name!)) return new GeneralResponse(false, "Department already added");
            appDbContext.Departments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await appDbContext.Departments.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            dep.GeneralDepartmentId = item.GeneralDepartmentId;
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var department = await appDbContext.Departments
                .Include(d => d.Branches)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department is null) return NotFound();

            // Check if department has any branches
            if (department.Branches?.Count > 0)
            {
                return new GeneralResponse(false, "Cannot delete department because it has branches assigned");
            }

            appDbContext.Departments.Remove(department);
            await Commit();
            return Success();
        }

        // -------------------- HELPERS --------------------
        private static GeneralResponse NotFound() => new(false, "Sorry Department not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

        private async Task<bool> CheckName(string name)
        {
            var item = await appDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return item is null;
        }
    }
}
