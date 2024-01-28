using WISHLIST.Models.Domain;

namespace WISHLIST.Models.DTO
{
    public class CombineWishlistGiftModel
    {
        public CreateWishlistModel CreateWishlistModel { get; set; }

        public CreateGiftModel CreateGiftModel { get; set; }

        public List<GiftModel> GiftsModel { get; set; }
    }
}
