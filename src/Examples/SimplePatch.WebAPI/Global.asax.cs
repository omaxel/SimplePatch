using SimplePatch.WebAPI.Models;
using System.Web.Http;

namespace SimplePatch.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            DeltaConfig.Init((cfg) =>
            {
                cfg.ExcludeProperties<Person>(x => x.Id);
            });
        }
    }
}
