using Nop.Plugin.Payments.PayU;
using Nop.Plugin.Payments.PayU.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.PayU.Controllers
{
    public class PaymentPayUController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly PayUPaymentSettings _PayUPaymentSettings;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;

        public PaymentPayUController(ISettingService settingService, ILocalizationService localizationService, PayUPaymentSettings PayUPaymentSettings, IOrderService orderService, IOrderProcessingService orderProcessingService)
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._PayUPaymentSettings = PayUPaymentSettings;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
        }

        [ChildActionOnly]
        [AdminAuthorize]
        public ActionResult Configure()
        {
            int storeScope = 0;//default store id.
            var payUPaymentSettings = _settingService.LoadSetting<PayUPaymentSettings>(storeScope);
            ConfigurationModel model = new ConfigurationModel();
            model.Anahtar = payUPaymentSettings.Anahtar;
            model.IsyeriKodu = payUPaymentSettings.IsyeriKodu;
            return View("~/Plugins/Payments.PayU/Views/PaymentPayU/Configure.cshtml", model);
        }

        [AdminAuthorize]
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!((Controller)this).ModelState.IsValid)
                return this.Configure();
            this._PayUPaymentSettings.IsyeriKodu = model.IsyeriKodu;
            this._PayUPaymentSettings.Anahtar = model.Anahtar;

            this._settingService.SaveSetting<PayUPaymentSettings>(this._PayUPaymentSettings); 
            return View("~/Plugins/Payments.PayU/Views/PaymentPayU/Configure.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        { 
            return View("~/Plugins/Payments.PayU/Views/PaymentPayU/PaymentInfo.cshtml");
        }



        public ActionResult PaymentHandler(int ORDER, string err)
        {
            if (err == null)
            {
                this._orderProcessingService.MarkOrderAsPaid(_orderService.GetOrderById(ORDER));
                return RedirectToRoute("CheckoutCompleted", new
                {
                    orderId = ORDER
                });
            }
            

            this._orderProcessingService.CancelOrder(_orderService.GetOrderById(ORDER), true);
            return RedirectToAction("Index", "Home", new
            {
                area = ""
            });
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            return new List<string>();
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            return new ProcessPaymentRequest();
        } 
    }
}
