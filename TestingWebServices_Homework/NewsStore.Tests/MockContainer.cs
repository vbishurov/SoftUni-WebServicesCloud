namespace NewsStore.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Moq;
    using Repositories.Contracts;

    public class MockContainer
    {
        public Mock<IRepository<NewsStory>> NewsStoriesRepositoryMock { get; set; }

        public List<NewsStory> newsStories;

        public MockContainer()
        {
            this.newsStories = new List<NewsStory>()
            {
                new NewsStory()
                {
                    Title = "TestNewsStory1",
                    Content = "TestNewsStory1",
                    PublishDate = DateTime.Now,
                    Id = 1
                },
                new NewsStory()
                {
                    Title = "TestNewsStory2",
                    Content = "TestNewsStory2",
                    PublishDate = DateTime.Now.AddDays(-6),
                    Id = 2
                },
                new NewsStory()
                {
                    Title = "TestNewsStory3",
                    Content = "TestNewsStory3",
                    PublishDate = DateTime.Now.AddDays(-2),
                    Id = 3
                }
            };
        }

        public void PrepareMocks()
        {
            this.SetupFakeNewsStories();
        }

        private void SetupFakeNewsStories()
        {
            this.NewsStoriesRepositoryMock = new Mock<IRepository<NewsStory>>();
            this.NewsStoriesRepositoryMock.Setup(m => m.All())
                .Returns(this.newsStories.AsQueryable());

            this.NewsStoriesRepositoryMock.Setup(m => m.Find(It.IsAny<int>()))
                .Returns((int id) => this.newsStories.FirstOrDefault(n => n.Id == id));
        }
    }
}
