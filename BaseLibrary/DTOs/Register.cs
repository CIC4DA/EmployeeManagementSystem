using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLibrary.DTOs
{
    public class Register : AccountBase
    {
        [Required(ErrorMessage = "Full Name is required")]
        [MinLength(5)]
        [MaxLength(100)]
        public string? FullName { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        public string? ConfirmPassword { get; set; }
    }
}
