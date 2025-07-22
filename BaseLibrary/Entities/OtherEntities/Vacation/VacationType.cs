namespace BaseLibrary.Entities.OtherEntities.Vacation
{
    public class VacationType : BaseEntity
    {
        // one to many relationships with vacations
        public List<Vacation>? Vacations { get; set; }
    }
}
