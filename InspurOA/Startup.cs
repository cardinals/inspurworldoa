using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InspurOA.Startup))]
namespace InspurOA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
