using WISHLIST.Models.Domain;

namespace WISHLIST.Models.DTO
{
    public class CombineWishlistGiftViewModel
    {
        public WishlistModel Wishlist { get; set; }

        public List<WishlistModel> AuthoredWishlists { get; set; }

        public List<GiftModel> Gifts { get; set; }
    }
}
