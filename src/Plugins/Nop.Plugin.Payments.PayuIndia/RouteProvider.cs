using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Payments.PayU
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.Payu.Configure", "Plugins/PaymentPayu/Configure", (object)new
            {
                controller = "PaymentPayu",
                action = "Configure"
            }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
            RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.Payu.PaymentInfo", "Plugins/PaymentPayu/PaymentInfo", (object)new
            {
                controller = "PaymentPayu",
                action = "PaymentInfo"
            }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
            RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.Payu.Return", "Plugins/PaymentPayu/Return", (object)new
            {
                controller = "PaymentPayu",
                action = "Return"
            }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
        }
    }
}