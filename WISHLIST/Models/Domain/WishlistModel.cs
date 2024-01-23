namespace WISHLIST.Models.Domain
{
    public class WishlistModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AuthorId { get; set; }

        public List<GiftModel> Gifts { get; set; }

        public ApplicationUser Author { get; set; }
    }
}
