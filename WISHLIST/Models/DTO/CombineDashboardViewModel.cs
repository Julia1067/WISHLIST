using WISHLIST.Models.Domain;

namespace WISHLIST.Models.DTO
{
    public class CombineDashboardViewModel
    {
        public List<WishlistModel> Wishlists { get; set; }

        public ApplicationUser User { get; set; }

        public int FriendsCount { get; set; }

        public int BlockedUsersCount { get; set; }

        public int WishlistsCount { get; set; }

        public int GiftsCount { get; set; }
    }
}
