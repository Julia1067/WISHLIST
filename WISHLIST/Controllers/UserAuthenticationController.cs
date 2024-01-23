using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using WISHLIST.Models;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WISHLIST.Controllers
{
    public class UserAuthenticationController(IUserAuthenticationService userAuthenticationService,
                                              SignInManager<ApplicationUser> signInManager) : Controller
    {
        private readonly IUserAuthenticationService _userAuthenticationService = userAuthenticationService;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LoginModel model = new();
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            TempData["msg"] = string.Empty;
            if (!ModelState.IsValid)
                return View(model);

            var status = await _userAuthenticationService.LoginAsync(model);

            if (status.StatusValue == false)
            {
                TempData["msg"] = (status.StatusMessage);
                return View(model);  
            }

            TempData["msg"] = (status.StatusMessage);

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
 
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);   
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var temp = await _userAuthenticationService.ExternlLoginCallbackMethod();

            var type = temp.GetType();

            if (type.Equals(typeof(StatusModel)))
            {
                StatusModel status = (StatusModel)temp;

                if(status.StatusValue == false)
                {
                    TempData["msg"] = status.StatusMessage;
                    return RedirectToAction("Login");
                }
                return RedirectToAction("Dashboard", "User", new { username = User.Identity.Name });
            }
            ApplicationUser user = (ApplicationUser)temp;

            TempData["User"] = JsonConvert.SerializeObject(user);
            return RedirectToAction("InfoConfirm", "UserAuthentication");
        }

        [HttpGet]
        public IActionResult InfoConfirm()
        {
            var json = TempData["User"] as string;

            ApplicationUser user = JsonConvert.DeserializeObject<ApplicationUser>(json);
            InfoConfirm model = new();

            model.Email = user.Email;
            model.Surname = user.Surname;
            model.Name = user.Name;
            model.Birthday = user.Birthday;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InfoConfirm(InfoConfirm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var status = await _userAuthenticationService.InfoConfirmAsync(model);
            if (status.StatusValue == false)
            {
                TempData["msg"] = status.StatusMessage;
                return RedirectToAction("Login");
            }

            return RedirectToAction("Dashboard", "User", new { username = User.Identity.Name });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
