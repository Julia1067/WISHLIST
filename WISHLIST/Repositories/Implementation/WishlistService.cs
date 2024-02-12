using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Repositories.Implementation
{
    public class WishlistService(DatabaseContext dbContext,
                                 UserManager<ApplicationUser> userManager,
                                 IUserService userService
        ) : IWishlistService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUserService _userService = userService;

        public async Task<StatusModel> CreateWishlistAsync(CreateWishlistModel model, string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            StatusModel status = new();

            string Id = string.Empty;

            while (true)
            {
                Id = RandomnId();

                var dbWishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == Id);

                if (dbWishlist == null)
                {
                    break;
                }
            }
            string Id2 = string.Empty;

            while (true)
            {
                Id2 = RandomnId();

                var dbWishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == Id2);

                if (dbWishlist == null)
                {
                    break;
                }
            }

            try
            {
                WishlistModel wishlist = new()
                {
                    Id = Id,
                    Name = model.Name,
                    Description = model.Description,
                    AuthorId = user.Id,
                    ModificatorType = model.ModificatorType
                };

                await _dbContext.Wishlists.AddAsync(wishlist);

                OwnerWishlistModel ownerWishlist = new()
                {
                    Id = Id2,
                    WishlistId = Id,
                    OwnerId = user.Id
                };

                await _dbContext.OwnerWishlists.AddAsync(ownerWishlist);

                await _dbContext.SaveChangesAsync();
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

        public async Task<StatusModel> DeleteCurrentWishlistAsync(string WishlistId, string username)
        {
            StatusModel status = new();
            try
            {
                var user = await _userManager.FindByNameAsync(username);

                var wishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == WishlistId);

                if (wishlist.AuthorId == user.Id)
                {
                    var ownerWishlist = await _dbContext.OwnerWishlists.Where(o => o.WishlistId == WishlistId).ToListAsync();

                    var giftsArr = await _dbContext.Gifts.Where(g => g.WishlistId == WishlistId).ToArrayAsync();


                    foreach (var gift in giftsArr)
                    {
                        var ownerGifts = await _dbContext.OwnerGifts.Where(g => g.GiftId == gift.Id).ToArrayAsync();

                        foreach (var ownerGift in ownerGifts)
                        {
                            _dbContext.OwnerGifts.Remove(ownerGift);
                        }

                        _dbContext.Gifts.Remove(gift);
                    }

                    foreach (var ownerW in ownerWishlist)
                    {
                        _dbContext.OwnerWishlists.Remove(ownerW);
                    }

                    _dbContext.Wishlists.Remove(wishlist);
                }
                else
                {
                    var ownerWishlist = await _dbContext.OwnerWishlists.FirstOrDefaultAsync(o => o.WishlistId == wishlist.Id && o.OwnerId == user.Id);

                    _dbContext.OwnerWishlists.Remove(ownerWishlist);
                }

                await _dbContext.SaveChangesAsync();

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

        public async Task<List<WishlistModel>> GetAllOwnWishlistListAsync(string attendedUsername, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var attendeduser = await _userManager.FindByNameAsync(attendedUsername);

            var ownerWishlists = await _dbContext.OwnerWishlists.Where(w => w.OwnerId == user.Id).ToListAsync();

            var wishlists = new List<WishlistModel>();  

            foreach(var ownerWishlist in ownerWishlists)
            {
                var wishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == ownerWishlist.WishlistId);

                if(username == attendedUsername)
                {
                    wishlists.Add(wishlist);
                }

                else
                {
                    var author = await _userManager.FindByIdAsync(wishlist.AuthorId);

                    if (author.UserName == attendedUsername)
                    {
                        wishlists.Add(wishlist);
                    }
                    else
                    {
                        var interaction = await _userService.GetInteractionTypeAsync(attendedUsername, author.UserName);


                        if (interaction != null)
                        {
                            var type = (InteractionType)interaction;

                            if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                            {
                                if (wishlist.ModificatorType == ModificatorType.Public)
                                {
                                    wishlists.Add(wishlist);
                                }
                            }
                            else if (type == InteractionType.Friendship)
                            {
                                if (wishlist.ModificatorType == ModificatorType.Public || wishlist.ModificatorType == ModificatorType.FriendsOnly)
                                {
                                    wishlists.Add(wishlist);
                                }
                            }
                        }
                        else
                        {
                            if (wishlist.ModificatorType == ModificatorType.Public)
                            {
                                wishlists.Add(wishlist);
                            }
                        }
                    }
                }
            }

            return wishlists;
        }

        public async Task<WishlistModel> GetCurrentWishListAsync(string WishlistId)
        {
            return await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == WishlistId);
        }

        public async Task<StatusModel> UpdateCurrentWishlistAsync(CreateWishlistModel model)
        {
            StatusModel status = new();
            try
            {
                var wishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == model.Id);

                if (wishlist != null)
                {
                    wishlist.Name = model.Name;
                    wishlist.Description = model.Description;
                    wishlist.ModificatorType = model.ModificatorType;

                    _dbContext.Update(wishlist);

                    await _dbContext.SaveChangesAsync();
                }


                await _dbContext.SaveChangesAsync();

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

        public async Task<List<WishlistModel>> GetAllWishlists(string username)
        {
            var wishlists = await _dbContext.Wishlists.ToListAsync();

            List<WishlistModel> correctedList = [];

            foreach(var wishlist in wishlists)
            {
                var user = await _userManager.FindByIdAsync(wishlist.AuthorId);

                var interaction = await _userService.GetInteractionTypeAsync(username, user.UserName);

                if(user.UserName == username)
                {
                    correctedList.Add(wishlist);
                }
                else
                {
                    if (interaction == null)
                    {
                        var interaction2 = await _userService.GetInteractionTypeAsync(user.UserName, username);

                        if (interaction2 == null)
                        {
                            if (wishlist.ModificatorType == ModificatorType.Public)
                            {
                                correctedList.Add(wishlist);
                            }
                        }
                        else
                        {
                            var type = (InteractionType)interaction2;

                            if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                            {
                                if (wishlist.ModificatorType == ModificatorType.Public)
                                {
                                    correctedList.Add(wishlist);
                                }
                            }
                            else if (type == InteractionType.Friendship)
                            {
                                if (wishlist.ModificatorType == ModificatorType.Public || wishlist.ModificatorType == ModificatorType.FriendsOnly)
                                {
                                    correctedList.Add(wishlist);
                                }
                            }
                        }
                    }
                    else
                    {
                        var type = (InteractionType)interaction;

                        if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                        {
                            if (wishlist.ModificatorType == ModificatorType.Public)
                            {
                                correctedList.Add(wishlist);
                            }
                        }
                        else if (type == InteractionType.Friendship)
                        {
                            if (wishlist.ModificatorType == ModificatorType.Public || wishlist.ModificatorType == ModificatorType.FriendsOnly)
                            {
                                correctedList.Add(wishlist);
                            }
                        }
                    }
                }
            }

            return correctedList;
        }

        public async Task<List<WishlistModel>> GetWishlistByRequest(string request, string username)
        {
            var nameContain = await _dbContext.Wishlists.Where(w => w.Name.Contains(request)).ToListAsync();

            var correctedList = new List<WishlistModel>();

            foreach(var list in nameContain)
            {
                var user = await _userManager.FindByIdAsync(list.AuthorId);

                if(user.UserName == username)
                {
                    correctedList.Add(list);
                }
                else
                {
                    var interaction = await _userService.GetInteractionTypeAsync(username, user.UserName);

                    if (interaction == null)
                    {
                        var interaction2 = await _userService.GetInteractionTypeAsync(user.UserName, username);

                        if (interaction2 == null)
                        {
                            if (list.ModificatorType == ModificatorType.Public)
                            {
                                correctedList.Add(list);
                            }
                        }
                        else
                        {
                            var type = (InteractionType)interaction2;

                            if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                            {
                                if (list.ModificatorType == ModificatorType.Public)
                                {
                                    correctedList.Add(list);
                                }
                            }
                            else if (type == InteractionType.Friendship)
                            {
                                if (list.ModificatorType == ModificatorType.Public || list.ModificatorType == ModificatorType.FriendsOnly)
                                {
                                    correctedList.Add(list);
                                }
                            }
                        }
                    }
                    else
                    {
                        var type = (InteractionType)interaction;

                        if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                        {
                            if (list.ModificatorType == ModificatorType.Public)
                            {
                                correctedList.Add(list);
                            }
                        }
                        else if (type == InteractionType.Friendship)
                        {
                            if (list.ModificatorType == ModificatorType.Public || list.ModificatorType == ModificatorType.FriendsOnly)
                            {
                                correctedList.Add(list);
                            }
                        }
                    }
                }

            }

            return correctedList;
        }

        public async Task<StatusModel> AddExistingWishlist(string username, string wishlistId)
        {
            StatusModel status = new();

            try
            {
                var user = await _userManager.FindByNameAsync(username);

                var wishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == wishlistId);

                if (wishlist != null)
                {
                    string Id = string.Empty;

                    while (true)
                    {
                        Id = RandomnId();

                        var dbWishlist = await _dbContext.OwnerWishlists.FirstOrDefaultAsync(w => w.Id == Id);

                        if (dbWishlist == null)
                        {
                            break;
                        }
                    }

                    OwnerWishlistModel ownerWishlist = new()
                    {
                        Id = Id,
                        OwnerId = user.Id,
                        WishlistId = wishlistId
                    };

                    await _dbContext.OwnerWishlists.AddAsync(ownerWishlist);

                    await _dbContext.SaveChangesAsync();

                    status.StatusValue = true;
                    return status;
                }
                status.StatusValue = false;
                return status;
            }
            catch
            {
                status.StatusValue = false;
                return status;
            }
        }

        public async Task<bool> IsWishlistOwned(string wishlistId, string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var ownWishlist = _dbContext.OwnerWishlists.FirstOrDefault(o => o.WishlistId == wishlistId && o.OwnerId == user.Id);

            if (ownWishlist != null)
            {
                return true;
            }

            return false;
        }

        public async Task<int> GetWishlistCountByUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var ownerWishlists = await _dbContext.OwnerWishlists.Where(o => o.OwnerId == user.Id).ToListAsync();

            return ownerWishlists.Count;
        }

        public async Task<List<WishlistModel>> GetAllAuthoredWishlists(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var wishlists = await _dbContext.Wishlists.Where(w => w.AuthorId == user.Id).ToListAsync();

            return wishlists;
        }
    }
}
