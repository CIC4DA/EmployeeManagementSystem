namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        // One to many relationship with Departments
        public List<Department>? Departments { get; set; }
    }
}
