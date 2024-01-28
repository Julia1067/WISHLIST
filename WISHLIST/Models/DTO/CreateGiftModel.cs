using System.ComponentModel.DataAnnotations;

namespace WISHLIST.Models.DTO
{
    public class CreateGiftModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile ImageFile { get; set; }

        public string ImagePath { get; set; }

        public string GiftUrl { get; set; }

        public bool IsFulfilled { get; set; }

        public int Priority { get; set; }

        public int ModificatorId { get; set; }

        public string WishlistId { get; set;}
    }
}
