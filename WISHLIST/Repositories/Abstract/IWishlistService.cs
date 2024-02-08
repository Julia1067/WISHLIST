using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IWishlistService
    {
        public Task<List<OwnerWishlistModel>> GetAllOwnWishlistListAsync(string Username);

        public Task<WishlistModel> GetCurrentWishListAsync(string WishlistId);

        public Task<StatusModel> CreateWishlistAsync(CreateWishlistModel model, string username);

        public Task<StatusModel> UpdateCurrentWishlistAsync(CreateWishlistModel model);

        public Task<StatusModel> DeleteCurrentWishlistAsync(string WishlistId, string username);

        public Task<List<WishlistModel>> GetAllWishlists(string username);

        public Task<List<WishlistModel>> GetWishlistByRequest(string request, string username);

        public Task<StatusModel> AddExistingWishlist(string username, string wishlistId);

        public Task<bool> IsWishlistOwned(string wishlistId, string username);
    }
}
