namespace WISHLIST.Models.Domain
{
    public class GiftModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageFilePath { get; set; }

        public string GiftUrl { get; set; }

        public string WishlistId { get; set; }

        public WishlistModel Wishlist { get; set; }
    }
}
