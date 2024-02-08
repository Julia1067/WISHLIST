using WISHLIST.Models.Domain;

namespace WISHLIST.Models.DTO
{
    public class CreateWishlistModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AuthorId { get; set; }

        public ModificatorType ModificatorType { get; set; }
    }
}
