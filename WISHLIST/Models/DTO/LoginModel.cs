using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace WISHLIST.Models.DTO
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public List<AuthenticationScheme> ExternalLogins { get; set; }  
    }
}
