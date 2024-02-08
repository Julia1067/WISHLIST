using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WISHLIST.Models.Domain
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<WishlistModel> Wishlists { get; set; }

        public DbSet<GiftModel> Gifts { get; set; }

        public DbSet<InteractionModel> Interactions { get; set; }

        public DbSet<OwnerWishlistModel> OwnerWishlists { get; set; }

        public DbSet<OwnerGiftModel> OwnerGifts { get; set; }
    }
}
