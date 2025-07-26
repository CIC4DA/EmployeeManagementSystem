using BaseLibrary.Entities;
using BaseLibrary.Entities.OtherEntities.Doctor;
using BaseLibrary.Entities.OtherEntities.Overtime;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class OvertimeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Overtime>
    {

        public async Task<List<Overtime>> GetAll() => await appDbContext.Overtimes.AsNoTracking()
                                                                               .Include(_ => _.OvertimeType)
                                                                               .ToListAsync();

        public async Task<Overtime> GetById(int id) => await appDbContext.Overtimes.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
        
        public async Task<GeneralResponse> Insert(Overtime item)
        {
            if (item.EmployeeId <= 0)
            {
                return new GeneralResponse(false, "A valid EmployeeId is required.");
            }

            appDbContext.Overtimes.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Overtime item)
        {
            var obj = await appDbContext.Overtimes
                .FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();

            obj.StartDate = item.StartDate;
            obj.EndDate = item.EndDate;
            obj.OvertimeTypeId = item.OvertimeTypeId;
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Overtimes.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Overtimes.Remove(item);
            await Commit();
            return Success();
        }

        // -------------------- HELPERS --------------------
        private static GeneralResponse NotFound() => new(false, "Sorry Overtime not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
