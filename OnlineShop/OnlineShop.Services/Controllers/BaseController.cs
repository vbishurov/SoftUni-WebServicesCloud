namespace OnlineShop.Services.Controllers
{
    using System.Web.Http;
    using Data;

    public class BaseController : ApiController
    {
        protected readonly OnlineShopContext Context;

        public BaseController()
        {
            this.Context = OnlineShopContext.Create();
        }
    }
}