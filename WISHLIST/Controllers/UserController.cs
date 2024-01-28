using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController(IUserService userService,
                                UserManager<ApplicationUser> userManager,
                                IWishlistService wishlistService) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IWishlistService _wishlistService = wishlistService;

        [HttpGet]
        public async Task<IActionResult> Dashboard(string username)
        {
            var list = await _wishlistService.GetAuthorWishlistListAsync(username);

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> UserInfoChange(string username)
        {
            
            var user = await _userManager.FindByNameAsync(username);

            var correctedPath = $"/images/{user.ImageFilePath}";

            var model = new UpdateUserInfoModel();

            model.Surname = user.Surname;
            model.Name = user.Name;
            model.Birthday = user.Birthday;
            model.Id = user.Id;
            model.ImageFilePath = correctedPath;

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
                return RedirectToAction("Dashboard");
            }

            TempData["msg"] = status.StatusMessage;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveFile(IFormFile file)
        {
            var result = await _userService.SaveUserFile(User.Identity.Name, file);

            TempData["msg"] = result.StatusMessage;

            return RedirectToAction("UserInfoChange", new {username = User.Identity.Name });
        }

    }
}
