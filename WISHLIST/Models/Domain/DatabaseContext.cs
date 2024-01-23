using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WISHLIST.Models.Domain
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<WishlistModel> Wishlists { get; set; }

        public DbSet<GiftModel> Gifts { get; set; }
    }
}
