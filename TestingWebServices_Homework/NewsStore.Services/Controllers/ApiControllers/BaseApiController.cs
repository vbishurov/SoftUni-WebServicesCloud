namespace NewsStore.Services.Controllers.ApiControllers
{
    using System.Web.Http;
    using Data;
    using Repositories;
    using Repositories.Contracts;

    public class BaseApiController : ApiController
    {
        protected INewsData Data { get; set; }

        public BaseApiController()
            : this(new NewsData(new NewsContext()))
        {
        }

        public BaseApiController(INewsData data)
        {
            this.Data = data;
        }
    }
}