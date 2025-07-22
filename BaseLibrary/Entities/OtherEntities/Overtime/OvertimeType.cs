namespace BaseLibrary.Entities.OtherEntities.Overtime
{
    public class OvertimeType : BaseEntity
    {
        // one to many relationships with overtime
        public List<Overtime>? Overtimes { get; set; }
    }
}
