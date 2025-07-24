using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Town : BaseEntity
    {
        // Many to one relationship with city
        public City? City { get; set; }
        public int CityId { get; set; }

        // One to many relationship with employee
        [JsonIgnore]
        public List<Employee>? Employees { get; set; }

    }
}
