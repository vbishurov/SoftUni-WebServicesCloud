namespace NewsStore.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.ModelBinding;
    using DTOs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using Repositories.Contracts;
    using Services.Controllers.ApiControllers;
    using Services.Models.BindingModels;

    [TestClass]
    public class NewsStoriesControllerTests
    {
        private MockContainer mocks;

        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }

        [TestMethod]
        public void GetNews_ShouldReturnAllNewsSortedByPublishDate()
        {
            var fakeNews = this.mocks.NewsStoriesRepositoryMock.Object.All();

            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var response = newsStoriesController.GetNews().ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var newsStoriesIdsExpected = fakeNews.OrderBy(n => n.PublishDate).Select(n => n.Id).ToList();
            var newsStoriesIds = response.Content.ReadAsAsync<IList<NewsStoryDTO>>().Result.OrderBy(n => n.PublishDate).Select(n => n.Id).ToList();

            CollectionAssert.AreEqual(newsStoriesIdsExpected, newsStoriesIds);
        }

        [TestMethod]
        public void CreateNews_CorrectData_ShouldCreateAndReturnNewsStory()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            this.mocks.NewsStoriesRepositoryMock.Setup(m => m.Add(It.IsAny<NewsStory>()))
                .Callback((NewsStory newsStory) => this.mocks.newsStories.Add(newsStory));

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var expectedNewsStory = new NewsStoryDTO()
            {
                Title = "TestNewsStory20",
                Content = "TestNewsStory20",
                PublishDate = DateTime.Now
            };

            var createNewsStoryResponse = newsStoriesController.CreateNewsStory(new NewsStoryBindingModel()
            {
                Title = "TestNewsStory20",
                Content = "TestNewsStory20",
                PublishDate = DateTime.Now
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.Created, createNewsStoryResponse.StatusCode);

            var createdNewsStory = createNewsStoryResponse.Content.ReadAsAsync<NewsStoryDTO>().Result;

            Assert.AreEqual(expectedNewsStory, createdNewsStory);
        }

        [TestMethod]
        public void CreateNews_IncorrectData_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var createNewsStoryResponse = newsStoriesController.CreateNewsStory(new NewsStoryBindingModel()
            {
                Title = "",
                Content = "TestNewsStory20",
                PublishDate = DateTime.Now
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, createNewsStoryResponse.StatusCode);
        }

        [TestMethod]
        public void ModifyNewsStory_CorrectData_ShouldModifyNewsStory()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var publishDate = DateTime.Now.AddDays(-40);

            var editNewsStoryResponse = newsStoriesController.EditNewsStory(1, new NewsStoryBindingModel()
            {
                Title = "TestNewsStory1Modified",
                Content = "TestNewsStory1Modified",
                PublishDate = publishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, editNewsStoryResponse.StatusCode);

            var newsStoryModifiedExpected = new NewsStory()
            {
                Title = "TestNewsStory1Modified",
                Content = "TestNewsStory1Modified",
                PublishDate = publishDate
            };

            var newsStoryModified = this.mocks.NewsStoriesRepositoryMock.Object.Find(1);

            Assert.AreEqual(newsStoryModifiedExpected, newsStoryModified);
        }

        [TestMethod]
        public void ModifyNewsStory_IncorrectData_ShouldReturnBadRequest()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var publishDate = DateTime.Now.AddDays(-40);

            var editNewsStoryResponse = newsStoriesController.EditNewsStory(1, new NewsStoryBindingModel()
            {
                Title = "",
                Content = "TestNewsStory1Modified",
                PublishDate = publishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, editNewsStoryResponse.StatusCode);
        }

        [TestMethod]
        public void ModifyNewsStory_NonExistant_ShouldReturnNotFound()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            var publishDate = DateTime.Now.AddDays(-40);

            var editNewsStoryResponse = newsStoriesController.EditNewsStory(10, new NewsStoryBindingModel()
            {
                Title = "TestNewsStory10Modified",
                Content = "TestNewsStory10Modified",
                PublishDate = publishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, editNewsStoryResponse.StatusCode);
        }

        [TestMethod]
        public void DeleteNewsStory_Existant_ShouldDeleteTheNewsStory()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            this.mocks.NewsStoriesRepositoryMock.Setup(m => m.Delete(It.IsAny<NewsStory>()))
                .Callback((NewsStory newsStory) => this.mocks.newsStories.Remove(newsStory));

            var newsStoryDeleteResult = newsStoriesController.DeleteNewsStory(1).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, newsStoryDeleteResult.StatusCode);

            var newsStoryDeleted = this.mocks.NewsStoriesRepositoryMock.Object.Find(1);

            Assert.AreEqual(null, newsStoryDeleted);
        }

        [TestMethod]
        public void DeleteNewsStory_NonExistant_ShouldReturnNotFound()
        {
            var mockContext = new Mock<INewsData>();
            mockContext.Setup(m => m.News)
                .Returns(this.mocks.NewsStoriesRepositoryMock.Object);

            var newsStoriesController = new NewsStoriesController(mockContext.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            this.mocks.NewsStoriesRepositoryMock.Setup(m => m.Delete(It.IsAny<NewsStory>()))
                .Callback((NewsStory newsStory) => this.mocks.newsStories.Remove(newsStory));

            var newsStoryDeleteResult = newsStoriesController.DeleteNewsStory(10).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.NotFound, newsStoryDeleteResult.StatusCode);
        }
    }
}
