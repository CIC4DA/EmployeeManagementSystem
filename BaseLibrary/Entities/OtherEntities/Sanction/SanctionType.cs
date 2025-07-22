namespace BaseLibrary.Entities.OtherEntities.Sanction
{
    public class SanctionType : BaseEntity
    {
        // one to many relationships with Sanctions
        public List<Sanction>? Sanctions { get; set; }
    }
}
