using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Repositories.Implementation
{
    public class GiftService(DatabaseContext dbContext,
                            IWebHostEnvironment environment,
                            UserManager<ApplicationUser> userManager) : IGiftService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<string> CreateGiftAsync(CreateGiftModel model, string username)
        {
            var status = new StatusModel();

            string Id = string.Empty;

            while (true)
            {
                Id = RandomnId();

                var dbGift = await _dbContext.Gifts.FirstOrDefaultAsync(w => w.Id == Id);

                if (dbGift == null)
                {
                    break;
                }
            }
            string Id2 = string.Empty;
            while (true)
            {
                Id2 = RandomnId();

                var dbGift = await _dbContext.OwnerGifts.FirstOrDefaultAsync(w => w.Id == Id2);

                if (dbGift == null)
                {
                    break;
                }
            }

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                GiftModel gift = new();

                gift.Id = Id;
                gift.Name = model.Name;
                gift.Description = model.Description;
                gift.ImageFilePath = "подарунок.png";
                gift.GiftUrl = model.GiftUrl;
                gift.AuthorId = user.Id;
                if (model.IsFulfilled == true)
                    gift.IsFullfilled = true;
                else
                    gift.IsFullfilled = false;
                gift.ModificatorType = model.ModificatorType;
                gift.WishlistId = model.WishlistId;
                gift.LastUpdateDate = DateTime.Now;
                gift.Priority = model.Priority;

                await _dbContext.Gifts.AddAsync(gift);

                OwnerGiftModel ownerGift = new();

                ownerGift.Id = Id2;
                ownerGift.GiftId = Id;
                ownerGift.OwnerId = (await _userManager.FindByNameAsync(username)).Id;

                await _dbContext.OwnerGifts.AddAsync(ownerGift);

                await _dbContext.SaveChangesAsync();

                return Id;
            }
            catch (Exception ex)
            {
                status.StatusValue = false;
                status.StatusMessage = ex.Message;
                return Id;
            }
        }

        public async Task<StatusModel> DeleteGiftAsync(string giftId)
        {
            StatusModel status = new();

            try
            {
                var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == giftId);

                if (gift.ImageFilePath != "подарунок.png")
                {
                    var wwwrootPath = _environment.WebRootPath;
                    string path = Path.Combine(wwwrootPath, "images", gift.ImageFilePath);

                    File.Delete(path);
                }

                _dbContext.Gifts.Remove(gift);

                await _dbContext.SaveChangesAsync();
                status.StatusValue = true;
                return status;
            }
            catch(Exception ex) 
            { 
                status.StatusValue = false;
                status.StatusMessage = ex.Message;
                return status;
            }
        }

        public async Task<List<GiftModel>> GetAllWishlistGiftsAsync(string wishlidtId)
        {
            return await _dbContext.Gifts.Where(g => g.WishlistId == wishlidtId).ToListAsync();
        }

        public async Task<GiftModel> GetGiftAsync(string giftid)
        {
            return await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == giftid);
        }

        public async Task<StatusModel> UpdateGiftAsync(CreateGiftModel model)
        {
            StatusModel status = new();
            try
            {
                var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == model.Id);

                gift.Priority = model.Priority;
                gift.Description = model.Description;
                gift.Name = model.Name;
                gift.LastUpdateDate = DateTime.Now;
                gift.GiftUrl = model.GiftUrl;
                gift.IsFullfilled = model.IsFulfilled;
                gift.ModificatorType = model.ModificatorType;

                _dbContext.Gifts.Update(gift);

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

        public async Task<string> GetCurrentGiftImage(string giftId)
        {
            var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == giftId);

            return gift.ImageFilePath;
        }

        public async Task<StatusModel> SaveGiftFile(string giftId, IFormFile file)
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

                    var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == giftId);//user searching 
                                                                             //in database in AspNetUsers table

                    if (gift.ImageFilePath != "подарунок.png")
                    {
                        string wwwrootPath = _environment.WebRootPath;
                        path = Path.Combine(wwwrootPath, "images", gift.ImageFilePath);
                        File.Delete(path);
                    }

                    gift.ImageFilePath = FileName; //adding new path to the database

                    _dbContext.Gifts.Update(gift);

                    await _dbContext.SaveChangesAsync();


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

        public async Task<int> UserGiftsNumber(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var giftList = await _dbContext.OwnerGifts.Where(o => o.OwnerId == user.Id).ToListAsync();

            return giftList.Count;
        }
    }
}

