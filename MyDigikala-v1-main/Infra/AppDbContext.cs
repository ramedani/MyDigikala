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




    }
}
