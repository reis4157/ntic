using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayU;
using Nop.Plugin.Payments.PayU.Models;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.PayU.Controllers
{
    public class PaymentPayuController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly PayuPaymentSettings _PayuPaymentSettings;
        private readonly PaymentSettings _paymentSettings;

        public PaymentPayuController(ISettingService settingService, IPaymentService paymentService, IOrderService orderService, IOrderProcessingService orderProcessingService, PayuPaymentSettings PayuPaymentSettings, PaymentSettings paymentSettings)
        {
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._PayuPaymentSettings = PayuPaymentSettings;
            this._paymentSettings = paymentSettings;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.MerchantId = this._PayuPaymentSettings.MerchantId;
            model.Key = this._PayuPaymentSettings.Key;
            model.MerchantParam = this._PayuPaymentSettings.MerchantParam;
            model.PayUri = this._PayuPaymentSettings.PayUri;
            model.AdditionalFee = this._PayuPaymentSettings.AdditionalFee;
            return View("~/Plugins/Payments.Payu/Views/PaymentPayu/Configure.cshtml", model);

        }

        [ChildActionOnly]
        [HttpPost]
        [AdminAuthorize]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!((Controller)this).ModelState.IsValid)
                return this.Configure();
            this._PayuPaymentSettings.MerchantId = model.MerchantId;
            this._PayuPaymentSettings.Key = model.Key;
            this._PayuPaymentSettings.MerchantParam = model.MerchantParam;
            this._PayuPaymentSettings.PayUri = model.PayUri;
            this._PayuPaymentSettings.AdditionalFee = model.AdditionalFee;
            this._settingService.SaveSetting<PayuPaymentSettings>((PayuPaymentSettings)this._PayuPaymentSettings, 0);
            return View("~/Plugins/Payments.Payu/Views/PaymentPayu/Configure.cshtml", model);
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.Payu/Views/PaymentPayu/PaymentInfo.cshtml", new PaymentInfoModel());
        }



        [ValidateInput(false)]
        public ActionResult Return(FormCollection form)
        {
            PayuPaymentProcessor paymentProcessor = this._paymentService.LoadPaymentMethodBySystemName("Payments.Payu") as PayuPaymentProcessor;
            if (paymentProcessor == null || !PaymentExtensions.IsPaymentMethodActive((IPaymentMethod)paymentProcessor, this._paymentSettings) || !paymentProcessor.PluginDescriptor.Installed)
                throw new NopException("Payu module cannot be loaded");
            PayuHelper payuHelper = new PayuHelper();
            if (string.IsNullOrWhiteSpace(this._PayuPaymentSettings.Key))
                throw new NopException("Payu key is not set");
            string MerchantId = this._PayuPaymentSettings.MerchantId.ToString();
            string OrderId = form["txnid"];
            string Amount = form["amount"];
            string productinfo = form["productinfo"];
            string firstname = form["firstname"];
            string email = form["email"];
            string str = form["hash"];
            string status = form["status"];
            if (!(payuHelper.verifychecksum(MerchantId, OrderId, Amount, productinfo, firstname, email, status, this._PayuPaymentSettings.Key) == str))
                return Content("Security Error. Illegal access detected");
            if (!(status == "success"))
                return RedirectToRoute("HomePage");
            Order orderById = this._orderService.GetOrderById(Convert.ToInt32(OrderId));
            if (this._orderProcessingService.CanMarkOrderAsPaid(orderById))
                this._orderProcessingService.MarkOrderAsPaid(orderById);
            return RedirectToRoute("CheckoutCompleted", new
            {
                orderId = ((BaseEntity)orderById).Id
            });
        }


        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }
    }
}
