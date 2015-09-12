namespace NewsStore.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Repositories;
    using Repositories.Contracts;

    [TestClass]
    public class NewsStoriesRepositoryTests
    {
        private TestDBContext TestNewsContext { get; set; }

        private List<NewsStory> testNewsStories;

        [TestInitialize]
        public void InitTest()
        {
            this.TestNewsContext = new TestDBContext();

            this.testNewsStories = new List<NewsStory>()
            {
                new NewsStory() {Title = "TestNewsStory1", Content = "TestNewsStory1", PublishDate = DateTime.Now},
                new NewsStory()
                {
                    Title = "TestNewsStory2",
                    Content = "TestNewsStory2",
                    PublishDate = DateTime.Now.AddDays(-6)
                },
                new NewsStory()
                {
                    Title = "TestNewsStory3",
                    Content = "TestNewsStory3",
                    PublishDate = DateTime.Now.AddDays(-2)
                }
            };

            foreach (NewsStory newsStory in this.testNewsStories)
            {
                this.TestNewsContext.News.Add(newsStory);
            }

            this.TestNewsContext.SaveChanges();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            this.TestNewsContext.Database.ExecuteSqlCommand("TRUNCATE TABLE NewsStories");

            this.TestNewsContext.SaveChanges();

            this.TestNewsContext = null;

            this.testNewsStories = null;
        }

        [TestMethod]
        public void GetNews_ShouldReturnAllNewsOrderByPublishDate()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStories = repository.All().OrderBy(n => n.PublishDate).ToList();

            CollectionAssert.AreEqual(this.testNewsStories.OrderBy(n => n.PublishDate).ToList(), newsStories);
        }

        [TestMethod]
        public void CreateNewsStory_CorrectData_ShouldCreateAndReturnNewsStory()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStoryExpected = new NewsStory()
            {
                Title = "TestNewsStory4",
                Content = "TestNewsStory4",
                PublishDate = DateTime.Now
            };

            repository.Add(newsStoryExpected);

            this.TestNewsContext.SaveChanges();

            var newsStoryCreated = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory4");

            Assert.AreEqual(newsStoryExpected, newsStoryCreated);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void CreateNewsStory_IncorrectData_ShouldThrowException()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStoryExpected = new NewsStory()
            {
                Title = "",
                Content = "TestNewsStory4",
                PublishDate = DateTime.Now.AddDays(-20)
            };

            repository.Add(newsStoryExpected);

            try
            {
                this.TestNewsContext.SaveChanges();
            }
            catch (DbEntityValidationException)
            {

                var entry = this.TestNewsContext.Entry(newsStoryExpected);
                entry.State = EntityState.Detached;
                throw;
            }
        }

        [TestMethod]
        public void ModifyNewsStory_CorrectData_ShouldModifyTheNewsStoryCorrectly()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStoryExpected = new NewsStory()
            {
                Title = "TestNewsStory1Modified",
                Content = "TestNewsStory1Modified",
                PublishDate = DateTime.Now,
            };

            var newsStoryEntity = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory1");

            newsStoryEntity.Title = newsStoryExpected.Title;
            newsStoryEntity.Content = newsStoryExpected.Content;
            newsStoryEntity.PublishDate = newsStoryExpected.PublishDate;

            this.TestNewsContext.SaveChanges();

            var newsStoryEntityModified = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory1Modified");

            Assert.AreEqual(newsStoryExpected, newsStoryEntityModified);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void ModifyNewsStory_IncorrectData_ShouldThrowException()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStoryExpected = new NewsStory()
            {
                Title = "",
                Content = "TestNewsStory1Modified",
                PublishDate = DateTime.Now,
            };

            var newsStoryEntity = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory1");

            newsStoryEntity.Title = newsStoryExpected.Title;
            newsStoryEntity.Content = newsStoryExpected.Content;
            newsStoryEntity.PublishDate = newsStoryExpected.PublishDate;

            try
            {
                this.TestNewsContext.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                var entry = this.TestNewsContext.Entry(newsStoryEntity);
                entry.State = EntityState.Detached;
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ModifyNewsStory_NonExistingNewsStory_ShouldThrowException()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStoryExpected = new NewsStory()
            {
                Title = "",
                Content = "TestNewsStory1Modified",
                PublishDate = DateTime.Now,
            };

            var newsStoryEntity = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory10");

            newsStoryEntity.Title = newsStoryExpected.Title;
            newsStoryEntity.Content = newsStoryExpected.Content;
            newsStoryEntity.PublishDate = newsStoryExpected.PublishDate;
        }

        [TestMethod]
        public void DeleteNewsStory_Existant_ShouldDeleteTheNewsStory()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStory = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory1");

            repository.Delete(newsStory);

            this.TestNewsContext.SaveChanges();

            newsStory = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory1");

            Assert.AreEqual(null, newsStory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteNewsStory_NonExistant_ShouldThrowException()
        {
            var typeOfRepository = typeof(GenericRepository<NewsStory>);
            var repository = Activator.CreateInstance(
                typeOfRepository, this.TestNewsContext) as IRepository<NewsStory>;

            var newsStory = repository.All().FirstOrDefault(n => n.Title == "TestNewsStory10");

            repository.Delete(newsStory);

            this.TestNewsContext.SaveChanges();
        }
    }
}
