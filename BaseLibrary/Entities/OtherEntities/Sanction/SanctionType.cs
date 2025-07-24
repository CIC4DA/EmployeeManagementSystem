using System.Text.Json.Serialization;

namespace BaseLibrary.Entities.OtherEntities.Sanction
{
    public class SanctionType : BaseEntity
    {
        // one to many relationships with Sanctions
        [JsonIgnore]
        public List<Sanction>? Sanctions { get; set; }
    }
}
