using System;
using System.ComponentModel.DataAnnotations;

namespace Mlurple_WebApp.Models
{
    public class ProjectSpace
    {
        [Required]
        [MaxLength(16, ErrorMessage = "Name cannot be more than 16 characters")]
        public string Name { get; set; }
    }
}