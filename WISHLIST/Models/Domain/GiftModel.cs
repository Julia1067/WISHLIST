using System.ComponentModel.DataAnnotations.Schema;

namespace WISHLIST.Models.Domain
{
    public class GiftModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageFilePath { get; set; }

        public string GiftUrl { get; set; }

        public int Priority { get; set; }

        [Column(TypeName = "bit")]
        public bool IsFullfilled { get; set; }

        public ModificatorType ModificatorType { get; set; }

        public string WishlistId { get; set; }

        public string AuthorId { get; set; }

        [Column(TypeName ="date")]
        public DateTime LastUpdateDate { get; set; }

        public WishlistModel Wishlist{ get; set; }

        public ApplicationUser Author { get; set; }
        
        public List<OwnerGiftModel> Owner {  get; set; }
    }
}
