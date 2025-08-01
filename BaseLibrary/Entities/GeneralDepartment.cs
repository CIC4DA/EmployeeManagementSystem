﻿using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        // One to many relationship with Departments
        // to prevent circular references during serialization
        [JsonIgnore]
        public List<Department>? Departments { get; set; }
    }
}
