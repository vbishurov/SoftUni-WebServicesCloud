namespace NewsStore.Services.Controllers.ApiControllers
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Web.Http;
    using Models.BindingModels;
    using NewsStore.Models;
    using Repositories.Contracts;

    [RoutePrefix("api/news")]
    public class NewsStoriesController : BaseApiController
    {
        public NewsStoriesController(INewsData newsData)
            : base(newsData)
        {
        }

        public NewsStoriesController()
            : base()
        {
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetNews()
        {
            var news = this.Data.News.All().OrderBy(n => n.PublishDate);

            if (!news.Any())
            {
                return this.NotFound();
            }

            return this.Ok(news);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateNewsStory([FromBody]NewsStoryBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var newsStory = new NewsStory()
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = model.PublishDate
            };

            this.Data.News.Add(newsStory);

            try
            {
                this.Data.SaveChanges();

                var newNewsStory = this.Data.News.All().FirstOrDefault(n => n.Title == model.Title);
                if (newNewsStory == null)
                {
                    return this.BadRequest("There was an error creating news story");
                }

                return this.Created(string.Format("{0}", newNewsStory.Id), newNewsStory);
            }
            catch (DbUpdateException ex)
            {
                return this.BadRequest(string.Format("A news story with title {0} already exists.", model.Title));
            }
            catch (Exception ex)
            {
                return this.BadRequest("There was a problem creating news story. Please try again");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditNewsStory([FromUri]int id, [FromBody]NewsStoryBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (string.IsNullOrEmpty(model.Title))
            {
                return this.BadRequest("Title is required");
            }

            NewsStory newsStory;

            if (!this.FindNewsStoryByID(id, out newsStory))
            {
                return this.NotFound();
            }

            newsStory.Title = model.Title;
            newsStory.Content = model.Content;
            newsStory.PublishDate = model.PublishDate;

            try
            {
                this.Data.SaveChanges();

                return this.Ok(string.Format("News story {0} modified successfully", id));
            }
            catch (Exception)
            {
                return this.BadRequest(string.Format("There was a problem modifying news story {0}", id));
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteNewsStory([FromUri]int id)
        {
            NewsStory newsStory;

            if (!this.FindNewsStoryByID(id, out newsStory))
            {
                return this.NotFound();
            }

            this.Data.News.Delete(newsStory);

            try
            {
                this.Data.SaveChanges();

                return this.Ok(string.Format("News story {0} deleted successfully", id));
            }
            catch (Exception)
            {
                return this.BadRequest(string.Format("There was a problem deleting news story {0}", id));
            }
        }

        private bool FindNewsStoryByID(int id, out NewsStory newsStory)
        {
            newsStory = this.Data.News.Find(id);

            if (newsStory == null)
            {
                return false;
            }

            return true;
        }
    }
}