namespace OnlineShop.Data
{
    using System.Data.Entity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Migrations;
    using Models;

    public class OnlineShopContext : IdentityDbContext
    {

        public OnlineShopContext()
            : base("OnlineShopContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<OnlineShopContext, Configuration>());
        }

        public virtual DbSet<Ad> Ads { get; set; }

        public virtual DbSet<AdType> AdTypes { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public static OnlineShopContext Create()
        {
            return new OnlineShopContext();
        }
    }
}