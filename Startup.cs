using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAPI_FormsAuth.Startup))]
namespace WebAPI_FormsAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
