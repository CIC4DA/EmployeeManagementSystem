using System.Text.Json.Serialization;

namespace BaseLibrary.Entities.OtherEntities.Vacation
{
    public class VacationType : BaseEntity
    {
        // one to many relationships with vacations
        [JsonIgnore]
        public List<Vacation>? Vacations { get; set; }
    }
}
