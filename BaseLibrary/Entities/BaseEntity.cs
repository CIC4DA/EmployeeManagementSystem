using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class BaseEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; } = string.Empty;
    }
}
