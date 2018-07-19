using SimplePatch.Examples.FullNET.WebAPI.Domain;
using System.Web.Http;

namespace SimplePatch.Examples.FullNET.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            DeltaConfig.Init(cfg => {
                cfg.AddEntity<Person>()
                    .Property(x => x.Id).Exclude();
            });
        }
    }
}
