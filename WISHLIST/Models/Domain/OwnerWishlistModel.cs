namespace WISHLIST.Models.Domain
{
    public class OwnerWishlistModel
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }

        public string WishlistId { get; set; }

        public ApplicationUser Owner {  get; set; }

        public WishlistModel Wishlist { get; set; }
    }
}
