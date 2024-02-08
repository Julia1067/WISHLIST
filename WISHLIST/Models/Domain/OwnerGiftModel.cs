namespace WISHLIST.Models.Domain
{
    public class OwnerGiftModel
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }

        public string GiftId { get; set; }

        public string WishlistId { get; set; }

        public ApplicationUser Owner { get; set; }

        public GiftModel Gift { get; set; }

        public WishlistModel Wishlist { get; set; }
    }
}
