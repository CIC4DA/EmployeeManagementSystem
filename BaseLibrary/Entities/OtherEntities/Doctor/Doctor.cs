using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities.OtherEntities.Doctor
{
    public class Doctor : OtherBaseEntity
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string MedicalDiagnosis { get; set; } = string.Empty;

        public string MedicalRecommendation {  get; set; } = string.Empty;
    }
}
