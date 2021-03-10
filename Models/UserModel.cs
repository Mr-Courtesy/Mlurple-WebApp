using System;
using System.ComponentModel.DataAnnotations;

namespace Mlurple_WebApp.Models
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
