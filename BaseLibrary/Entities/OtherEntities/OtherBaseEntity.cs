using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities.OtherEntities
{
    public class OtherBaseEntity
    {
        public int Id { get; set; }

        // One to many relationship with vacationtype
        public string? EmployeeName { get; set; }

        public int EmployeeId { get; set; } 

    }
}
