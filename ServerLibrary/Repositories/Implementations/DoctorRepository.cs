using BaseLibrary.Entities;
using BaseLibrary.Entities.OtherEntities.Doctor;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DoctorRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Doctor>
    {
        // HERE THE ID WEARE GETTING IS AN EMPLOYEEID
        public async Task<List<Doctor>> GetAll() => await appDbContext.Doctors.AsNoTracking()
                                                                                .ToListAsync();
        // getting doctor for an employee ID
        public async Task<Doctor> GetById(int id) => await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Doctor item)
        {
            if (item.EmployeeId <= 0)
            {
                return new GeneralResponse(false, "A valid EmployeeId is required.");
            }
            appDbContext.Doctors.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Doctor item)
        {
            var obj = await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == item.EmployeeId);
            if (obj is null) return NotFound();

            obj.MedicalRecommendation = item.MedicalRecommendation;
            obj.MedicalDiagnosis = item.MedicalDiagnosis;
            obj.Date = item.Date;
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Doctors.Remove(item);
            await Commit();
            return Success();
        }

        // -------------------- HELPERS --------------------
        private static GeneralResponse NotFound() => new(false, "Sorry Doctor not found");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

    }
}
