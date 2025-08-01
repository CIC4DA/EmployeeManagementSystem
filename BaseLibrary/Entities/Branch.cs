﻿using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Branch : BaseEntity
    {
        // Many to one relationship with Department
        public Department? Department { get; set; }

        public int DepartmentId { get; set; }


        // One to Many relationship with employees
        [JsonIgnore]
        public List<Employee>? Employees { get; set; }
    }
}
