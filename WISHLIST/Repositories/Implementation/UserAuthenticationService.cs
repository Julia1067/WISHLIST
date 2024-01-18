using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;

namespace WISHLIST.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserAuthenticationService(SignInManager<ApplicationUser> signInManager, 
                                         UserManager<ApplicationUser> userManager, 
                                         DatabaseContext dbContext, 
                                         RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<StatusModel> ChangePasswordAsync(ChangePasswordModel model, string username)
        {
            var status = new StatusModel();
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                status.StatusMessage = "Password changed successfully";
                status.StatusValue = true;
                return status;
            }

            status.StatusMessage = "Something went wrong";
            status.StatusValue = false;
            return status;
        }

        public async Task<StatusModel> DeleteAccountAsync(string username)
        {
            var status = new StatusModel();

            var user = await _userManager.FindByNameAsync(username);

            var reault = await _userManager.DeleteAsync(user);

            if(reault.Succeeded)
            {
                status.StatusValue = true;
                return status;
            }

            status.StatusValue = false;
            return status;
        }

        public async  Task<StatusModel> LoginAsync(LoginModel model)
        {
            var status = new StatusModel();

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                status.StatusMessage = "User doesn't exist";
                status.StatusValue = false;
                return status;
            }

            if(!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusMessage = "Invalid password";
                status.StatusValue = false;
                return status;
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

            if (result.IsLockedOut)
            {
                status.StatusMessage = "User locked out";
                status.StatusValue = false;
                return status;
            }
            else if(!result.Succeeded)
            {
                status.StatusMessage = "Error on loggin in";
                status.StatusValue = false;
                return status;
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            status.StatusValue = true;
            return status;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<StatusModel> RegistrationAsync(RegistrationModel model)
        {
            var status = new StatusModel();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.Username);

            var userEmail = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                status.StatusMessage = "User with this username already exists";
                status.StatusValue = false;

                return status;
            }

            if (userEmail != null)
            {
                status.StatusMessage = "User with this email already exists";
                status.StatusValue = false;

                return status;
            }


            user = new()
            {
                Name = model.Name,
                Email = model.Email,
                Surname = model.Surname,
                UserName = model.Username,
                Birthday = DateTime.Now,
                ImageFilePath = "завантаження.png"
            };

            var role = "user";

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
            
            await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                status.StatusMessage = "User creation failed";
                status.StatusValue = false;
            }

            status.StatusMessage = "User was created successfully";
            status.StatusValue = true;

            await _dbContext.SaveChangesAsync();

            return status;
        }

        public async Task<object> ExternlLoginCallbackMethod()
        {
            var status = new StatusModel();
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                status.StatusValue = false;
                status.StatusMessage = "Error loading external login information";
                return status;
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if(signInResult.Succeeded)
            {
                status.StatusValue = true;
                return status;
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email == null)
                {
                    status.StatusValue = false;
                    status.StatusMessage = "Your email doesn`t exist";
                    
                    return status;
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user != null)
                    {
                        status.StatusValue = false;
                        status.StatusMessage = "We consider your personal information was damaged\n" +
                            "Try to register new account\n" +
                            "We are sorry for your discomfort";
                        return status;
                    }
                    else
                    {
                        user = new();

                        user.Name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                        user.Surname = info.Principal.FindFirstValue(ClaimTypes.Surname);
                        user.Email = email;
                        user.Birthday = DateTime.Now;

                        return user;
                    }
                }
            }
        }

        public async Task<StatusModel> InfoConfirmAsync(InfoConfirm model)
        {
            var status = new StatusModel();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                status.StatusValue = false;
                status.StatusMessage = "User with this email already exists";
                return status;
            }

            user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                status.StatusValue = false;
                status.StatusMessage = "User with this username already exists";
                return status;
            }

            user = new ApplicationUser();

            user.Surname = model.Surname;
            user.Email = model.Email;
            user.Name = model.Name;
            user.Birthday = model.Birthday;
            user.UserName = model.Username;

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                status.StatusValue = false;
                status.StatusMessage = "Error existed on user creation way";
                return status;
            }

            string role = "user";

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            var info = await _signInManager.GetExternalLoginInfoAsync();

            await _userManager.AddLoginAsync(user, info);

            await _signInManager.SignInAsync(user, isPersistent: false);

            status.StatusValue = true;
            return status;
        }
    }
}
