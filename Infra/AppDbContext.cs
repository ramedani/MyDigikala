using Microsoft.EntityFrameworkCore;
using Domain;
namespace Infra
{
    public class AppDbContext: DbContext
    {
      public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<SlideShow> SlideShows { get; set; }
        public DbSet<ProductComment> productComments { get; set; }

        public DbSet<SiteSetting> SiteSettings { get; set; }


        public DbSet<register> registers { get; set; }
        public DbSet<RegisterUser> User { get; set; }

        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<NewsBlocks> NewsBlocks { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
    }
}
