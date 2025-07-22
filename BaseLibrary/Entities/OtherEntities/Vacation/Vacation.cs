using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities.OtherEntities.Vacation
{
    public class Vacation : OtherBaseEntity
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public int NumberOfDays { get; set; }

        public DateTime EndDate => StartDate.AddDays(NumberOfDays);

        // One to many relationship with vacationtype
        public VacationType? VacationType { get; set; }

        public int VacationTypeId { get; set; }

    }
}
