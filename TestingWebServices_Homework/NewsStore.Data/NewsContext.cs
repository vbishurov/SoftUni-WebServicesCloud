namespace NewsStore.Data
{
    using System.Data.Entity;
    using Migrations;
    using Models;

    public class NewsContext : DbContext
    {
        public NewsContext()
            : base("NewsContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsContext, Configuration>());
        }

        public virtual DbSet<NewsStory> News { get; set; }
    }
}