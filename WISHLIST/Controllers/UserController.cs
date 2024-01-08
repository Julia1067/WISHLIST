using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService,
                              UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            //var user = await _userManager.FindByNameAsync(User.Identity.Name);

            //var correctedPath = $"/images/{user.ImageFilePath}";

            //var model = new UserInfoChangeModel();

            //model.Surname = user.Surname;
            //model.Name = user.Name;
            //model.ImageFilePath = correctedPath;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserInfoChange(string username)
        {
            
            var user = await _userManager.FindByNameAsync(username);

            var correctedPath = $"/images/{user.ImageFilePath}";

            var model = new UserInfoChangeModel();

            model.Surname = user.Surname;
            model.Name = user.Name;
            model.Birthday = user.Birthday;
            model.Id = user.Id;
            model.ImageFilePath = correctedPath;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UserInfoChange(UserInfoChangeModel model)
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
            var result = await _userService.SaveFile(User.Identity.Name, file);

            TempData["msg"] = result.StatusMessage;

            return RedirectToAction("UserInfoChange", new {username = User.Identity.Name });
        }

    }
}
