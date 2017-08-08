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
                // Id property will not be changed when calling the Patch method.
                cfg.ExcludeProperties<Person>(x => x.Id);
            });
        }
    }
}
