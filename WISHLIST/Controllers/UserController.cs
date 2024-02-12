using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController(IUserService userService,
                                UserManager<ApplicationUser> userManager,
                                IWishlistService wishlistService,
                                IGiftService giftService
                                ) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IWishlistService _wishlistService = wishlistService;
        private readonly IGiftService _giftService = giftService;

        [HttpGet]
        public async Task<object> Dashboard(string username)
        {
            var interaction = await _userService.GetInteractionTypeAsync(User.Identity.Name, username);

            if (interaction != null)
            {
                InteractionType type = (InteractionType)interaction;

                if (type == InteractionType.Block1)
                {
                    return HttpStatusCode.NotFound;
                }
            }

            CombineDashboardViewModel model = new();

            model.Wishlists = await _wishlistService.GetAllOwnWishlistListAsync(User.Identity.Name, username);

            model.GiftsCount = await _giftService.UserGiftsNumber(username);

            model.User = await _userManager.FindByNameAsync(username);

            model.WishlistsCount = await _wishlistService.GetWishlistCountByUser(model.User.UserName);

            model.FriendsCount = await _userService.FriendsCount(username);

            model.BlockedUsersCount = await _userService.BlockedUsersCount(username);

            return View(model);
        }

        [Route("404")]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserInfoChange(string username)
        {

            var user = await _userManager.FindByNameAsync(username);

            var correctedPath = $"/images/{user.ImageFilePath}";

            var model = new UpdateUserInfoModel
            {
                Surname = user.Surname,
                Name = user.Name,
                Birthday = user.Birthday,
                Id = user.Id,
                ImageFilePath = correctedPath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserInfoChange(UpdateUserInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var status = await _userService.ChangeInfoAsync(model);

            if (status.StatusValue == true)
            {
                TempData["msg"] = status.StatusMessage;
                return RedirectToAction("Dashboard", "User", new { username = User.Identity.Name });
            }

            TempData["msg"] = status.StatusMessage;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFile(IFormFile file)
        {
            var result = await _userService.SaveUserFile(User.Identity.Name, file);

            TempData["msg"] = result.StatusMessage;

            return RedirectToAction("UserInfoChange", new { username = User.Identity.Name });
        }

        [HttpGet]
        public async Task<IActionResult> FindUser(string request)
        {
            var users = await _userService.FindUserAsync(request, User.Identity.Name);

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> MakeFriendRequest(string user2name)
        {
            try
            {
                var status = await _userService.MakeFriendRequest(User.Identity.Name, user2name);
                return RedirectToAction("FindUser");
            }
            catch
            {
                return RedirectToAction("FindUser");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            var requestsList = await _userService.GetAllRequestsByUser2(User.Identity.Name);
            return View(requestsList);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmFriend(string user1name)
        {
            await _userService.ConfirmFriend(user1name, User.Identity.Name);
            return RedirectToAction("Requests");
        }

        [HttpGet]
        public async Task<IActionResult> BlockUser(string user1name)
        {
            await _userService.BlockUser(user1name, User.Identity.Name);
            return RedirectToAction("Requests");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUserFromRequests(string user1name)
        {
            await _userService.DeleteFromRequests(user1name, User.Identity.Name);
            return RedirectToAction("Requests");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFromFriends(string username)
        {
            await _userService.DeleteFromFriends(User.Identity.Name, username);
            return RedirectToAction("FindUser");
        }

        [HttpGet]
        public async Task<IActionResult> UnblockUser(string username)
        {
            await _userService.UnblockUser(username, User.Identity.Name);
            return RedirectToAction("FindUser");
        }
    }
}
