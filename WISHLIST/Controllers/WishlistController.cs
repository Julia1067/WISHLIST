using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
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
            await _wishlistService.DeleteCurrentWishlistAsync(wishlistId, User.Identity.Name);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateWishlist(string wishlistId)
        {
            var wishlist = await _wishlistService.GetCurrentWishListAsync(wishlistId);

            CombineWishlistGiftModel model = new()
            {
                CreateWishlistModel = new CreateWishlistModel
                {
                    Id = wishlistId,
                    Name = wishlist.Name,
                    Description = wishlist.Description,
                    ModificatorType = wishlist.ModificatorType
                },

                GiftsModel = await _giftService.GetAllWishlistGiftsAsync(wishlistId)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWishlist(CombineWishlistGiftModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model.CreateWishlistModel);
            }

            var status = await _wishlistService.UpdateCurrentWishlistAsync(model.CreateWishlistModel);

            if(status.StatusValue == false)
            {
                return View(model.CreateWishlistModel);
            }

            return RedirectToAction("Dashboard", "User", new {username = User.Identity.Name});
        }

        [HttpGet]
        public async Task<IActionResult> GlobalWishlistPage(string request)
        {
            if(request == null)
            {
                var list = await _wishlistService.GetAllWishlists(User.Identity.Name);

                return View(list);
            }
            else
            {
                var list = await _wishlistService.GetWishlistByRequest(request, User.Identity.Name);

                return View(list);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Wishlist(string wishlistId)
        {
            CombineWishlistGiftViewModel model = new()
            {
                Wishlist = await _wishlistService.GetCurrentWishListAsync(wishlistId),
                Gifts = await _giftService.GetAllWishlistGiftsAsync(wishlistId)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddExistingWishlist(string wishlistId)
        {
            await _wishlistService.AddExistingWishlist(User.Identity.Name, wishlistId);

            return RedirectToAction("GlobalWishlistPage");
        }
    }
}
