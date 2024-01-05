using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            this._signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public Task<StatusModel> ChangePassword(ChangePasswordModel model)
        {
            throw new NotImplementedException();
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
            status.StatusMessage = "User registered successfully";
            status.StatusValue = true;
            return status;
        }

        public Task<StatusModel> Logout(string Id)
        {
            throw new NotImplementedException();
        }

        public async Task<StatusModel> RegistrationAsync(RegistrationModel model)
        {
            var status = new StatusModel();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == model.Username);

            if(user != null)
            {
                status.StatusMessage = "User already exists";
                status.StatusValue = false;

                return status;
            }

            user = new()
            {
                Name = model.Name,
                Email = model.Email,
                Surname = model.Surname,
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            
            await _userManager.AddToRoleAsync(user, model.Role);

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
    }
}
