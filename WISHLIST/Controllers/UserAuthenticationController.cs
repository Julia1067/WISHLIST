using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using WISHLIST.Models;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _userAuthenticationService;

        public UserAuthenticationController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var status = await _userAuthenticationService.LoginAsync(model);

            if (status.StatusValue == false)
            {
                TempData["msg"] = status.StatusMessage;
                return View(model);  
            }

            TempData["msg"] = status.StatusMessage;

            return RedirectToAction("Dashboard", "User", new { username = User.Identity.Name });
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _userAuthenticationService.RegistrationAsync(model);

            if (result.StatusValue == false)
            {
                TempData["msg"] = result.StatusMessage;
                return View(model);
            }

            TempData["result"] = result.StatusMessage;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userAuthenticationService.LogoutAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            StatusModel status = await _userAuthenticationService.ChangePasswordAsync(model, User.Identity.Name);

            if(status.StatusValue == false)
            {
                return View(model);
            }
            return RedirectToAction("Dashboard", "User", new { username= User.Identity.Name });
        }

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            return View();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            StatusModel status = await _userAuthenticationService.DeleteAccountAsync(User.Identity.Name);

            if (status.StatusValue == false)
            {
                return RedirectToAction("User", "Dashboard", new { username = User.Identity.Name });
            }

            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
