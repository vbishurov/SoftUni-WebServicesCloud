namespace NewsStore.Tests
{
    using System.Data.Entity;
    using Migrations;
    using Models;

    public class TestDBContext : DbContext
    {
        public TestDBContext()
            : base("TestDBContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TestDBContext, Configuration>());
        }

        public virtual DbSet<NewsStory> News { get; set; }

    }
}