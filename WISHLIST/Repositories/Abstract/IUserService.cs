using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetUserByUsername(string username);

        public Task<StatusModel> ChangeInfoAsync(UpdateUserInfoModel model);

        public Task<StatusModel> SaveUserFile(string username, IFormFile File);

        public Task<string> GetCurrentUserImage(string username);

        public Task<List<ApplicationUser>> FindUserAsync(string request, string username);

        public Task<StatusModel> MakeFriendRequest(string user1name, string user2name);

        public Task<int?> GetInteractionTypeAsync(string user1name, string user2name);

        public Task<StatusModel> ConfirmFriend(string user1name, string user2name);

        public Task<StatusModel> DeleteFromFriends(string user1name, string user2name);

        public Task<StatusModel> DeleteFromRequests(string user1name, string user2name);

        public Task<List<InteractionModel>> GetAllRequestsByUser2(string user2name);

        public Task<int> GetAllRequestsByUser2Count(string user2name);

        public Task BlockUser(string user1name, string user2name);

        public Task UnblockUser(string blockedUser, string userWhoBlocks);

        public Task<int> FriendsCount(string username);

        public Task<int> BlockedUsersCount(string username);

        public Task<ApplicationUser> GetUserById(string userId);
    }
}
