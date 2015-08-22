namespace BookShop.Data
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    using Models;

    public class BookShopContext : IdentityDbContext<ApplicationUser>
    {
        public BookShopContext()
            : base("BookShopContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookShopContext, Configuration>());
        }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Author> Authors { get; set; }

        public virtual DbSet<Purchase> Purchases { get; set; }

        public static BookShopContext Create()
        {
            return new BookShopContext();
        }
    }
}