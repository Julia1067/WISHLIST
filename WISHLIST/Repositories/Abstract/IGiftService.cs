using System.Diagnostics.Contracts;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IGiftService
    {
        public Task<GiftModel> GetGiftAsync(string giftId);

        public Task<List<GiftModel>> GetAllWishlistGiftsAsync(string wishlidtId);

        public Task<string> CreateGiftAsync(CreateGiftModel model);

        public Task<StatusModel> UpdateGiftAsync(CreateGiftModel model);

        public Task<StatusModel> DeleteGiftAsync(string giftid);

        public Task<string> GetCurrentGiftImage(string giftId);

        public Task<StatusModel> SaveGiftFile(string giftId, IFormFile file);

        public Task<List<ModificatorModel>> GetModificatorListAsync();

        public Task<ModificatorModel> GetCurrentModificator(int modificatorId);
    }
}
