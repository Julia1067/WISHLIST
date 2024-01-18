using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WISHLIST.Models.Domain;
using WISHLIST.Repositories.Abstract;
using WISHLIST.Repositories.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
        googleOptions.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
    });

builder.Services.AddAuthentication()
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.ClientId = builder.Configuration.GetSection("FacebookKeys:ClientId").Value;
        facebookOptions.ClientSecret = builder.Configuration.GetSection("FacebookKeys:ClientSecret").Value;
    });

builder.Services.AddAuthentication().
    AddTwitter(twitterOptions =>
    {
        twitterOptions.ConsumerKey = builder.Configuration.GetSection("TwitterKeys:ApiId").Value;
        twitterOptions.ConsumerSecret = builder.Configuration.GetSection("TwitterKeys:ApiSecret").Value;
        twitterOptions.RetrieveUserDetails = true;
    });

builder.Services
    .AddDbContext<DatabaseContext>(options => options
        .UseSqlServer(builder.Configuration
            .GetConnectionString("conn")));


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserAuthentication}/{action=Login}/{id?}");

app.Run();
