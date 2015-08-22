using Microsoft.Owin;
using OnlineShop.Services;

[assembly: OwinStartup(typeof(Startup))]

namespace OnlineShop.Services
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
