using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WISHLIST.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {
        public string Surname { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime Birthday { get; set; }

        public string ImageFilePath {  get; set; }

        public List<WishlistModel> Wishlists { get; set; }
    }
}
