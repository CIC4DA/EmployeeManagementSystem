using System.Text.Json.Serialization;

namespace BaseLibrary.Entities.OtherEntities.Overtime
{
    public class OvertimeType : BaseEntity
    {
        // one to many relationships with overtime
        [JsonIgnore]
        public List<Overtime>? Overtimes { get; set; }
    }
}
