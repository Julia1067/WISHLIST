using System.ComponentModel.DataAnnotations;

namespace WISHLIST.Models.DTO
{
    public class ConfirmInfoModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime Birthday { get; set; }
    }
}
