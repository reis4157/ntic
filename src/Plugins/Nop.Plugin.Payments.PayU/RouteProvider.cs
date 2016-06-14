// Decompiled with JetBrains decompiler
// Type: Nop.Plugin.Payments.PayU.RouteProvider
// Assembly: Nop.Plugin.Payments.PayU, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C21DCEB9-A37E-4087-977B-1759CF8D8225
// Assembly location: A:\PERSONAL\Projects\62945_72412_PayU_Plugin\Nop.Plugin.Payments.PayU.dll

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
      RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.PayU.Configure", "Plugins/PaymentPayU/Configure", (object) new
      {
        controller = "PaymentPayU",
        action = "Configure"
      }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
      RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.PayU.PaymentInfo", "Plugins/PaymentPayU/PaymentInfo", (object) new
      {
        controller = "PaymentPayU",
        action = "PaymentInfo"
      }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
      RouteCollectionExtensions.MapRoute(routes, "Plugin.Payments.PayU.PaymentHandler", "Plugins/PaymentPayU/PaymentHandler/{ORDER}", (object) new
      {
        controller = "PaymentPayU",
        action = "PaymentHandler",
        ORDER = UrlParameter.Optional,
        err = UrlParameter.Optional
      }, new string[1]
      {
        "Nop.Plugin.Payments.PayU.Controllers"
      });
    }
  }
}
