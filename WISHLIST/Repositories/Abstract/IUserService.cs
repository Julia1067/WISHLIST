using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IUserService
    {
        public Task<StatusModel> ChangeInfoAsync(UserInfoChangeModel model);

        public Task<StatusModel> SaveFile(string username, IFormFile File);

    }
}
