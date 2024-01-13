using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {
        public Task<StatusModel> RegistrationAsync(RegistrationModel model);

        public Task<StatusModel> LoginAsync(LoginModel model);

        public Task LogoutAsync();

        public Task<StatusModel> ChangePasswordAsync(ChangePasswordModel model, string username);

        public Task<StatusModel> DeleteAccountAsync(string username);
    }
}
