using System.Diagnostics.Contracts;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IGiftService
    {
        public Task<GiftModel> GetGiftAsync(string giftId);

        public Task<List<GiftModel>> GetAllWishlistGiftsAsync(string wishlidtId, string username);

        public Task<string> CreateGiftAsync(CreateGiftModel model, string username);

        public Task<StatusModel> UpdateGiftAsync(CreateGiftModel model);

        public Task<StatusModel> DeleteGiftAsync(string giftId, string wishlistId, string username);

        public Task<string> GetCurrentGiftImage(string giftId);

        public Task<StatusModel> SaveGiftFile(string giftId, IFormFile file);

        public Task<int> UserGiftsNumber(string username);

        public Task<List<GiftModel>> GetAllOwnerGifts(string attendedUsername, string username, string wishlistId);

        public Task<StatusModel> AddExistingGift(string wishlistId, string username, string giftId);

        public Task<bool> IsGiftOwned(string username, string wishlistId, string giftId);
        
        public Task<bool> IsGiftAuthored(string username, string wishlistId, string giftId);
    }
}
