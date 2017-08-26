using SimplePatch.Examples.FullNET.DAL;
using System.Web;
using System.Web.Http;

namespace SimplePatch.Examples.FullNET.WebAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            DeltaConfig.Init(x => x.ExcludeProperties<Person>(y => y.Id));
        }
    }
}
