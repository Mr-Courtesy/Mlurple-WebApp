using System;
using System.ComponentModel.DataAnnotations;

namespace Mlurple_WebApp.Classes
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
