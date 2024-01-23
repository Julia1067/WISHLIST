using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Repositories.Implementation
{
    public class WishlistService(DatabaseContext dbContext,
                                 UserManager<ApplicationUser> userManager) : IWishlistService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<StatusModel> CreateWishlistAsync(WishlistCreateModel model, string username)
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

            try
            {
                WishlistModel wishlist = new();

                wishlist.Id = Id;
                wishlist.Name = model.Name;
                wishlist.Description = model.Description;
                wishlist.AuthorId = user.Id;

                await _dbContext.Wishlists.AddAsync(wishlist);

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

        public async Task<StatusModel> DeleteCurrentWishlistAsync(string WishlistId)
        {
            StatusModel status = new();
            try
            {
                await _dbContext.Wishlists.Where(w => w.Id == WishlistId).ExecuteDeleteAsync();

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

        public async Task<List<WishlistModel>> GetAuthorWishlistListAsync(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);

            return await _dbContext.Wishlists.Where(w => w.AuthorId == user.Id).ToListAsync();
        }

        public async Task<WishlistModel> GetCurrentWishListAsync(string WishlistId)
        {
            return await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == WishlistId);
        }

        public async Task<StatusModel> UpdateCurrentWishlistAsync(WishlistCreateModel model)
        {
            StatusModel status = new();
            try
            {
                var wishlist = await _dbContext.Wishlists.FirstOrDefaultAsync(w => w.Id == model.Id);

                if (wishlist != null)
                {
                    wishlist.Name = model.Name;
                    wishlist.Description = model.Description;

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
    }
}
