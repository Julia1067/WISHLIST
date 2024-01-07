using System.ComponentModel.DataAnnotations;

namespace WISHLIST.Models.DTO
{
    public class UserInfoChangeModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public DateTime Birthday { get; set; }


    }
}
