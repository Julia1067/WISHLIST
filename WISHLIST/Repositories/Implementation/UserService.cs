using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;
using WISHLIST.Repositories.Abstract;
using static System.Net.Mime.MediaTypeNames;

namespace WISHLIST.Repositories.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _databaseContext;
        private readonly IWebHostEnvironment _environment;

        public UserService(UserManager<ApplicationUser> userManager, 
            IWebHostEnvironment environment, 
            DatabaseContext databaseContext)
        {
            _userManager = userManager;
            _databaseContext = databaseContext;
            _environment = environment;
        }

        public async Task<StatusModel> ChangeInfoAsync(UserInfoChangeModel model)
        {
            var status = new StatusModel();
            var user = await _userManager.FindByIdAsync(model.Id);

            user.Surname = model.Surname;
            user.Name = model.Name;
            user.Birthday = model.Birthday;

            var result = await _userManager.UpdateAsync(user);

            _databaseContext.SaveChanges();

            if (!result.Succeeded)
            {
                status.StatusValue = false;
                status.StatusMessage = "Updating user info failed";
                return status;
            }

            status.StatusValue = true;
            status.StatusMessage = "Updating user info succeded";
            return status;

        }

        public async Task<string> GetCurrentImage(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            return user.ImageFilePath; 
        }

        public async Task<StatusModel> SaveFile(string username, IFormFile file)
        {
            var status = new StatusModel();
            if (file == null || file.Length == 0)
            {
                status.StatusValue = false;
                status.StatusMessage = "Please choose one image file";
                return status;
            }
            string extension = Path.GetExtension(file.FileName);
            
            string[] extensions = [".jpg", ".jpeg", ".jfif", ".pjpeg", ".pjp", ".png"];

            

            bool result = false;
            foreach ( var item in extensions)
            {
                if (extension == item)
                {
                    result = true;
                    break;
                }
            }

            if (result)
            {
                if (file != null)
                {
                    Random rand = new Random();

                    string originalName = file.FileName; //original name initialization
                    string FileName = $"{DateTime.Now:yyyyMMddHHmmss}_{rand.Next(1000000, 999999999)}{extension}";
                    //unique file name creation

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", FileName);
                    //photo path creation

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream); //adding file to folder
                    }

                    var user = await _userManager.FindByNameAsync(username); //user searching 
                                                                             //in database in AspNetUsers table

                    if (user.ImageFilePath != "завантаження.png")
                    {
                        string wwwrootPath = _environment.WebRootPath;
                        path = Path.Combine(wwwrootPath, "images", user.ImageFilePath);
                        File.Delete(path);
                    }

                    user.ImageFilePath = FileName; //adding new path to the database

                    var queryResult = await _userManager.UpdateAsync(user);

                    await _databaseContext.SaveChangesAsync();

                    if (!queryResult.Succeeded)
                    {
                        status.StatusValue = false;
                        status.StatusMessage = "Updating user info failed";
                        return status;
                    }

                    status.StatusValue = true;
                    status.StatusMessage = "Updating user info succeded";
                    return status;
                }
                status.StatusValue = false;
                status.StatusMessage = "Please choose one image file from your folder";
                return status;
            }

            status.StatusValue = false;
            status.StatusMessage = "File extension must be .jpg, .jpeg, .jfif, .pjpeg, .pjp, .png";
            return status;

        }


    }
}
