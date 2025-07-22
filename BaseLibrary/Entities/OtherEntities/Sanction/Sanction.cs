using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities.OtherEntities.Sanction
{
    public class Sanction : OtherBaseEntity
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Punishment { get; set; }  = string.Empty;

        [Required]
        public DateTime? PunishmentDate { get; set; }

        // Many to one relationship with sanction type
        public SanctionType? SanctionType { get; set; }

        [Required]
        public int SanctionId { get; set; }
    }
}
