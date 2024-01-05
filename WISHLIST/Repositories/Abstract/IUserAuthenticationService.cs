using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        public Task<StatusModel> RegistrationAsync(RegistrationModel model);

        public Task<StatusModel> LoginAsync(LoginModel model);

        public Task<StatusModel> Logout(string Id);

        public Task<StatusModel> ChangePassword(ChangePasswordModel model);
    }
}
