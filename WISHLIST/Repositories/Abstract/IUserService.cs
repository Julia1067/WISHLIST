using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IUserService
    {
        public Task<StatusModel> ChangeInfoAsync(UpdateUserInfoModel model);

        public Task<StatusModel> SaveUserFile(string username, IFormFile File);

        public Task<string> GetCurrentUserImage(string username);
    }
}
