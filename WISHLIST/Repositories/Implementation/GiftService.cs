using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Repositories.Implementation
{
    public class GiftService(DatabaseContext dbContext,
                            IWebHostEnvironment environment,
                            UserManager<ApplicationUser> userManager,
                            IUserService userService) : IGiftService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly IWebHostEnvironment _environment = environment;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUserService _userService = userService;

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

                if (model.IsFulfilled == true)
                    gift.IsFullfilled = true;
                else
                    gift.IsFullfilled = false;
                gift.Id = Id;
                gift.Name = model.Name;
                gift.Description = model.Description;
                gift.ImageFilePath = "подарунок.png";
                gift.GiftUrl = model.GiftUrl;
                gift.AuthorId = user.Id;
                gift.ModificatorType = model.ModificatorType;
                gift.WishlistId = model.WishlistId;
                gift.LastUpdateDate = DateTime.Now;
                gift.Priority = model.Priority;

                await _dbContext.Gifts.AddAsync(gift);

                OwnerGiftModel ownerGift = new()
                {
                    Id = Id2,
                    GiftId = Id,
                    WishlistId = model.WishlistId,
                    OwnerId = (await _userManager.FindByNameAsync(username)).Id
                };

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

        public async Task<StatusModel> DeleteGiftAsync(string giftId, string wishlistId, string username)
        {
            StatusModel status = new();

            try
            {
                var user = await _userManager.FindByNameAsync(username);

                var ownerGift = await _dbContext.OwnerGifts.FirstOrDefaultAsync(o => o.OwnerId == user.Id && o.WishlistId == wishlistId && o.GiftId == giftId);

                if (ownerGift != null)
                {
                    var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == giftId);

                    if (gift.AuthorId == user.Id)
                    {
                        if (gift.ImageFilePath != "подарунок.png")
                        {
                            var wwwrootPath = _environment.WebRootPath;
                            string path = Path.Combine(wwwrootPath, "images", gift.ImageFilePath);

                            File.Delete(path);
                        }

                        var ownerGifts = await _dbContext.OwnerGifts.Where(o => o.GiftId == giftId).ToListAsync();

                        foreach (var item in ownerGifts)
                        {
                            _dbContext.OwnerGifts.Remove(item);
                        }

                        _dbContext.Gifts.Remove(gift);
                    }
                    else
                    {
                        _dbContext.OwnerGifts.Remove(ownerGift);
                    }
                    await _dbContext.SaveChangesAsync();
                    status.StatusValue = true;
                    return status;
                }

                status.StatusValue = false;
                return status;
            }
            catch (Exception ex)
            {
                status.StatusValue = false;
                status.StatusMessage = ex.Message;
                return status;
            }
        }

        public async Task<List<GiftModel>> GetAllWishlistGiftsAsync(string wishlidtId,string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var ownerGifts = await _dbContext.OwnerGifts.Where(o => o.WishlistId == wishlidtId && o.OwnerId == user.Id).ToListAsync();

            var gifts = new List<GiftModel>();

            foreach (var ownerGift in ownerGifts)
            {
                var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == ownerGift.GiftId);

                if (gift != null)
                {
                    gifts.Add(gift);
                }
            }
            return gifts;
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

        public async Task<List<GiftModel>> GetAllOwnerGifts(string attendedUsername, string username, string wishlistId)
        {
            var attendedUser = await _userManager.FindByNameAsync(attendedUsername);
            var user = await _userManager.FindByNameAsync(username);

            var ownerGifts = await _dbContext.OwnerGifts.Where(o => o.OwnerId == user.Id && o.WishlistId == wishlistId).ToListAsync();

            var gifts = new List<GiftModel>();

            if (attendedUser.Id == user.Id)
            {
                foreach (var ownerGift in ownerGifts)
                {
                    var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == ownerGift.Id);

                    gifts.Add(gift);
                }
            }
            else
            {
                foreach (var ownerGift in ownerGifts)
                {
                    var gift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.Id == ownerGift.GiftId);

                    var author = await _userManager.FindByIdAsync(gift.AuthorId);

                    if(attendedUsername == author.UserName)
                    {
                        gifts.Add(gift);
                    }
                    else
                    {
                        var interaction = await _userService.GetInteractionTypeAsync(attendedUsername, author.UserName);

                        if (interaction != null)
                        {
                            var type = (InteractionType)interaction;

                            if (type == InteractionType.Block2 || type == InteractionType.Request1 || type == InteractionType.Request2)
                            {
                                if (gift.ModificatorType == ModificatorType.Public)
                                {
                                    gifts.Add(gift);
                                }
                            }
                            else if (type == InteractionType.Friendship)
                            {
                                if (gift.ModificatorType == ModificatorType.Public || gift.ModificatorType == ModificatorType.FriendsOnly)
                                {
                                    gifts.Add(gift);
                                }
                            }
                        }
                        else
                        {
                            if (gift.ModificatorType == ModificatorType.Public)
                            {
                                gifts.Add(gift);
                            }
                        }
                    }
                }
            }

            return gifts;
        }

        public async Task<StatusModel> AddExistingGift(string wishlistId, string username, string giftId)
        {
            var status = new StatusModel();

            try
            {
                var user = await _userManager.FindByNameAsync(username);

                var ownerGift = await _dbContext.OwnerGifts.FirstOrDefaultAsync(o => o.WishlistId == wishlistId && o.OwnerId == user.Id && o.GiftId == giftId);

                if (ownerGift == null)
                {
                    ownerGift = new OwnerGiftModel();

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

                    ownerGift.Id = Id;
                    ownerGift.OwnerId = user.Id;
                    ownerGift.WishlistId = wishlistId;
                    ownerGift.GiftId = giftId;

                    await _dbContext.OwnerGifts.AddAsync(ownerGift);

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

        public async Task<bool> IsGiftOwned(string username, string wishlistId, string giftId)
        {
            var user = await _userManager.FindByNameAsync(username);

            var ownerGift = await _dbContext.OwnerGifts.FirstOrDefaultAsync(o => o.OwnerId == user.Id && o.WishlistId == wishlistId && o.GiftId == giftId);

            if (ownerGift == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsGiftAuthored(string username, string wishlistId, string giftId)
        {
            var user = await _userManager.FindByNameAsync(username);

            var ownerGift = await _dbContext.Gifts.FirstOrDefaultAsync(g => g.AuthorId == user.Id && g.WishlistId == wishlistId && g.Id == giftId);

            if (ownerGift == null)
            {
                return false;
            }

            return true;
        }
    }
}


