using Microsoft.AspNetCore.Mvc;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    public class GiftController(IGiftService giftService) : Controller
    {
        private readonly IGiftService _giftService = giftService;

        [HttpPost]
        public async Task<IActionResult> CreateGift(CombineWishlistGiftModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UpdateWishlist", "Wishlist", new {wishlistId = model.CreateGiftModel.WishlistId});
            }
            var giftId = await _giftService.CreateGiftAsync(model.CreateGiftModel, User.Identity.Name);

            if (model.CreateGiftModel.ImageFile != null)
                await _giftService.SaveGiftFile(giftId, model.CreateGiftModel.ImageFile);

            return RedirectToAction("UpdateWishlist", "Wishlist", new { wishlistId = model.CreateGiftModel.WishlistId });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateGift(string giftId)
        {
            var gift = await _giftService.GetGiftAsync(giftId);

            if (gift != null)
            {
                CreateGiftModel model = new();

                model.Priority = gift.Priority;
                model.ModificatorType = gift.ModificatorType;
                model.GiftUrl = gift.GiftUrl;
                model.ImagePath = @$"/images/{gift.ImageFilePath}";
                model.Description = gift.Description;
                model.Name = gift.Name;
                model.IsFulfilled = gift.IsFullfilled;
                model.Id = gift.Id;
                model.WishlistId = gift.WishlistId;
                
                return View(model);
            }
            return RedirectToAction("UpdateWishlist", "Wishlist", new { wishlistId = gift.WishlistId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGift(CreateGiftModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var status = await _giftService.UpdateGiftAsync(model);

            if (model.ImageFile != null)
                await _giftService.SaveGiftFile(model.Id, model.ImageFile);

            if (status.StatusValue == false)
            {
                return View(model);
            }

            return RedirectToAction("UpdateWishlist", "Wishlist", new { wishlistId = model.WishlistId });
        }

        [HttpDelete]
        public async Task DeleteGift(string giftId, string wishlistId)
        {
            await _giftService?.DeleteGiftAsync(giftId, wishlistId, User.Identity.Name);
        }

        [HttpGet]
        public async Task<IActionResult> AddExistingGift(string wishlistId, string giftId)
        {
            await _giftService.AddExistingGift(wishlistId, User.Identity.Name, giftId);

            return RedirectToAction("UpdateWishlist", "Wishlist", new { wishlistId = wishlistId });
        }
    }
}
