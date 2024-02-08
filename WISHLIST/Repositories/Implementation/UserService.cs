using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;
using static System.Net.Mime.MediaTypeNames;

namespace WISHLIST.Repositories.Implementation
{
    public class UserService(UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment,
        DatabaseContext databaseContext) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly DatabaseContext _databaseContext = databaseContext;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<StatusModel> ChangeInfoAsync(UpdateUserInfoModel model)
        {
            var status = new StatusModel();
            var user = await _userManager.FindByIdAsync(model.Id);

            user.Surname = model.Surname;
            user.Name = model.Name;
            user.Birthday = model.Birthday;

            var result = await _userManager.UpdateAsync(user);

            _databaseContext.SaveChanges();

            if (!result.Succeeded)
            {
                status.StatusValue = false;
                status.StatusMessage = "Updating user info failed";
                return status;
            }

            status.StatusValue = true;
            status.StatusMessage = "Updating user info succeded";
            return status;

        }

        public async Task<string> GetCurrentUserImage(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            return user.ImageFilePath;
        }

        public async Task<StatusModel> SaveUserFile(string username, IFormFile file)
        {
            var status = new StatusModel();
            if (file == null || file.Length == 0)
            {
                status.StatusValue = false;
                status.StatusMessage = "Please choose one image file";
                return status;
            }
            string extension = Path.GetExtension(file.FileName);

            string[] extensions = [".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png"];

            bool result = false;
            foreach (var item in extensions)
            {
                if (extension == item)
                {
                    result = true;
                    break;
                }
            }

            if (result)
            {
                if (file != null)
                {
                    Random rand = new();

                    string originalName = file.FileName; //original name initialization
                    string FileName = $"{DateTime.Now:yyyyMMddHHmmss}_{rand.Next(1000000, 999999999)}{extension}";
                    //unique file name creation

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FileName);
                    //photo path creation

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream); //adding file to folder
                    }

                    var user = await _userManager.FindByNameAsync(username); //user searching 
                                                                             //in database in AspNetUsers table

                    if (user.ImageFilePath != "завантаження.png")
                    {
                        var wwwrootPath = _environment.WebRootPath;
                        path = Path.Combine(wwwrootPath, "images", user.ImageFilePath);
                        File.Delete(path);
                    }

                    user.ImageFilePath = FileName; //adding new path to the database

                    var queryResult = await _userManager.UpdateAsync(user);

                    await _databaseContext.SaveChangesAsync();

                    if (!queryResult.Succeeded)
                    {
                        status.StatusValue = false;
                        status.StatusMessage = "Updating user info failed";
                        return status;
                    }

                    status.StatusValue = true;
                    status.StatusMessage = "Updating user info succeded";
                    return status;
                }
                status.StatusValue = false;
                status.StatusMessage = "Please choose one image file from your folder";
                return status;
            }

            status.StatusValue = false;
            status.StatusMessage = "File extension must be .jpg, .jpeg, .jfif, .pjpeg, .pjp, .png";
            return status;

        }

        public async Task<List<ApplicationUser>> FindUserAsync(string request, string username)
        {
            if (request == null)
            {
                return null;
            }
            var users = await _databaseContext.Users.ToListAsync();

            var requestedUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                bool requestedUsername = user.UserName.Contains(request);
                bool requestedName = user.Name.Contains(request);
                bool requestedSurname = user.Surname.Contains(request);

                var interaction = _databaseContext.Interactions
                    .FirstOrDefault(i => i.FirstUsername == user.UserName && i.SecondUsername == username);

                if (interaction == null)
                {
                    var interaction2 = await _databaseContext.Interactions
                        .FirstOrDefaultAsync(i => i.FirstUsername == username && i.SecondUsername == user.UserName);

                    if (interaction2 == null)
                    {
                        if (requestedUsername || requestedName || requestedSurname)
                            requestedUsers.Add(user);
                    }
                    else
                    {
                        if ((requestedUsername || requestedName || requestedSurname) && interaction2.InteractionType != InteractionType.Block)
                            requestedUsers.Add(user);
                    }
                }
                else
                {
                    if (requestedUsername || requestedName || requestedSurname)
                        requestedUsers.Add(user);
                }
            }
            return requestedUsers;
        }

        public async Task<StatusModel> MakeFriendRequest(string user1name, string user2name)
        {
            StatusModel status = new();
            try
            {
                var interaction = await _databaseContext.Interactions.FirstOrDefaultAsync(i => i.FirstUsername == user1name && i.SecondUsername == user2name);
                var interaction2 = await _databaseContext.Interactions.FirstOrDefaultAsync(i => i.FirstUsername == user2name && i.SecondUsername == user1name);

                if (interaction != null || interaction2 != null)
                {
                    status.StatusValue = false;
                    return status;
                }

                string Id = string.Empty;

                while (true)
                {
                    Id = RandomnId();

                    var dbGift = await _databaseContext.Interactions.FirstOrDefaultAsync(w => w.Id == Id);

                    if (dbGift == null)
                    {
                        break;
                    }
                }

                interaction = new InteractionModel
                {
                    Id = Id,

                    FirstUsername = user1name,
                    SecondUsername = user2name,

                    InteractionType = 0
                };

                await _databaseContext.Interactions.AddAsync(interaction);

                await _databaseContext.SaveChangesAsync();
                status.StatusValue = true;
                return status;
            }
            catch (Exception ex)
            {
                status.StatusValue = false;
                status.StatusMessage = ex.Message;
                return status;
            }
        }

        public async Task<int?> GetInteractionTypeAsync(string user1name, string user2name)
        {
            var interaction = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == user1name && i.SecondUsername == user2name);

            if (interaction == null)
            {
                var interaction2 = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == user2name && i.SecondUsername == user1name);
                if (interaction2 == null)
                {
                    return null;
                }

                if(interaction2.InteractionType == InteractionType.Block)
                {
                    return (int)InteractionType.Block2;
                }
                else if(interaction2.InteractionType == InteractionType.Request)
                {
                    return (int)InteractionType.Request2;
                }

                int type2 = (int)interaction2.InteractionType;
                return type2;
            }

            if (interaction.InteractionType == InteractionType.Block)
            {
                return (int)InteractionType.Block1;
            }
            else if (interaction.InteractionType == InteractionType.Request)
            {
                return (int)InteractionType.Request1;
            }

            int type = (int)interaction.InteractionType;

            return type;
        }

        public async Task<StatusModel> ConfirmFriend(string user1name, string user2name)
        {
            StatusModel status = new();
            try
            {
                var interaction = await _databaseContext.Interactions.FirstOrDefaultAsync(i => i.FirstUsername == user1name && i.SecondUsername == user2name);

                if (interaction != null && interaction.InteractionType == InteractionType.Request)
                {
                    interaction.InteractionType = InteractionType.Friendship;
                    _databaseContext.Interactions.Update(interaction);
                }

                await _databaseContext.SaveChangesAsync();

                status.StatusValue = true;
                return status;
            }
            catch
            {
                status.StatusValue = false; 
                return status;
            }
        }

        public async Task<StatusModel> DeleteFromFriends(string user1name, string user2name)
        {
            StatusModel status = new();
            try
            {
                var interaction = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == user1name && i.SecondUsername == user2name);

                if (interaction == null)
                {
                    var interaction2 = await _databaseContext.Interactions
                        .FirstOrDefaultAsync(i => i.FirstUsername == user2name && i.SecondUsername == user1name);

                    if (interaction2 == null || interaction2.InteractionType != InteractionType.Friendship)
                    {
                        status.StatusValue = false; 
                        return status;
                    }

                    interaction2.InteractionType = InteractionType.Request;

                    _databaseContext.Interactions.Update(interaction2);
                }
                else
                {
                    if (interaction.InteractionType != InteractionType.Friendship)
                    {
                        status.StatusValue = false;
                        return status;
                    }

                    interaction.InteractionType = InteractionType.Request;

                    _databaseContext.Interactions.Update(interaction);
                }

                await _databaseContext.SaveChangesAsync();

                status.StatusValue = true;
                return status;
            }
            catch
            {
                status.StatusValue = false; 
                return status;
            }
        }

        public async Task<StatusModel> DeleteFromRequests(string user1name, string user2name)
        {
            StatusModel status = new();
            try
            {
                var interaction = await _databaseContext.Interactions.FirstOrDefaultAsync(i => i.FirstUsername == user1name && i.SecondUsername == user2name);

                if(interaction != null)
                {
                    _databaseContext.Interactions.Remove(interaction);

                    await _databaseContext.SaveChangesAsync();
                }

                status.StatusValue = true;
                return status;
            }
            catch
            {
                status.StatusValue= false;
                return status;
            }
        }

        public async Task<List<InteractionModel>> GetAllRequestsByUser2(string user2name)
        {
            var requestsList = await _databaseContext.Interactions
                .Where(i => i.SecondUsername == user2name && i.InteractionType == InteractionType.Request)
                .ToListAsync();
            return requestsList;
        }

        public async Task<int> GetAllRequestsByUser2Count(string user2name)
        {
            var count = await _databaseContext.Interactions
                .Where(i => i.SecondUsername == user2name && i.InteractionType == InteractionType.Request)
                .CountAsync();
            return count;
        }

        public async Task BlockUser(string blockedUser, string userWhoBlocks)
        {
            var interaction = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == blockedUser && i.SecondUsername == userWhoBlocks);

            if (interaction == null)
            {
                var interaction2 = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == userWhoBlocks && i.SecondUsername == blockedUser);
                if (interaction2 == null)
                {
                    interaction2 = new InteractionModel();

                    string Id = string.Empty;

                    while (true)
                    {
                        Id = RandomnId();

                        var dbGift = await _databaseContext.Interactions.FirstOrDefaultAsync(w => w.Id == Id);

                        if (dbGift == null)
                        {
                            break;
                        }
                    }

                    interaction2.Id = Id;
                    interaction2.FirstUsername = blockedUser;
                    interaction2.SecondUsername = userWhoBlocks;
                    interaction2.InteractionType = InteractionType.Block;

                    await _databaseContext.Interactions.AddAsync(interaction2);
                }
                else
                {
                    interaction2.InteractionType = InteractionType.Block;

                    _databaseContext.Interactions.Update(interaction2);
                }
            }
            else
            {
                interaction.InteractionType = InteractionType.Block;

                _databaseContext.Interactions.Update(interaction);
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UnblockUser(string blockedUser, string userWhoBlocks)
        {
            var interaction = await _databaseContext.Interactions
                .FirstOrDefaultAsync(i => i.FirstUsername == blockedUser && i.SecondUsername == userWhoBlocks);

            if(interaction != null && interaction.InteractionType == InteractionType.Block)
            {
                _databaseContext.Interactions.Remove(interaction);
                
                await _databaseContext.SaveChangesAsync();
            }
        }

        private static string RandomnId()
        {
            const string elements = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            string wishlistId = string.Empty;

            Random random = new();

            for (int i = 0; i < 6; i++)
            {
                wishlistId += elements[random.Next(0, elements.Length - 1)];
            }

            return wishlistId;
        }

        public async Task<int> FriendsCount(string username)
        {
            var user1pos = await _databaseContext.Interactions
                .Where(i => i.FirstUsername == username && i.InteractionType == InteractionType.Friendship)
                .CountAsync();
            var user2pos = await _databaseContext.Interactions
                .Where(i => i.SecondUsername == username && i.InteractionType == InteractionType.Friendship)
                .CountAsync();

            return user1pos + user2pos;
        }

        public async Task<int> BlockedUsersCount(string username)
        {
            var blocked = await _databaseContext.Interactions
                .Where(i => i.SecondUsername == username && i.InteractionType == InteractionType.Block)
                .CountAsync();

            return blocked;
        }

        public async Task<ApplicationUser> GetUserByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
    }
}