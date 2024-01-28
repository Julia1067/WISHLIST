using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    [Authorize(Roles = "user")]
    public class WishlistController(IWishlistService wishlistService,
                                    IGiftService giftService) : Controller
    {
        private readonly IWishlistService _wishlistService = wishlistService;
        private readonly IGiftService _giftService = giftService;

        [HttpGet]
        public IActionResult CreateWishlist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateWishlist(CreateWishlistModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var status = await _wishlistService.CreateWishlistAsync(model, User.Identity.Name);

            if(status.StatusValue == false)
            {
                return View(model);
            }

            return RedirectToAction("Dashboard", "User", new {username = User.Identity.Name});
        }

        [HttpDelete]
        public async Task DeleteWishlist(string wishlistId)
        {
            await _wishlistService.DeleteCurrentWishlistAsync(wishlistId);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateWishlist(string wishlistId)
        {
            var wishlist = await _wishlistService.GetCurrentWishListAsync(wishlistId);

            CombineWishlistGiftModel model = new();

            model.CreateWishlistModel = new CreateWishlistModel();
            
            model.CreateWishlistModel.Id = wishlistId;
            model.CreateWishlistModel.Name = wishlist.Name;
            model.CreateWishlistModel.Description = wishlist.Description;

            model.GiftsModel = await _giftService.GetAllWishlistGiftsAsync(wishlistId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWishlist(CreateWishlistModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var status = await _wishlistService.UpdateCurrentWishlistAsync(model);

            if(status.StatusValue == false)
            {
                return View(model);
            }

            return RedirectToAction("Dashboard", "User", new {username = User.Identity.Name});
        }
    }
}
