using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Configuration;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq; 
using Nop.Plugin.Payments.Iyzico.Validators;
using Nop.Services.Localization;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Core.Domain.Payments;


namespace Nop.Plugin.Payments.Iyzico.Controllers
{
    public class IyzicoStoreController : BasePaymentController
    {   
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly ILocalizationService _localizationService;

        public IyzicoStoreController(IWorkContext workContext,
            IStoreService storeService, 
            ISettingService settingService, 
            IPaymentService paymentService, 
            IOrderService orderService, 
            IOrderProcessingService orderProcessingService, 
            ILogger logger,
            IyzicoPaymentSettings paymentSettings, 
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._logger = logger;
            this._iyzicoPaymentSettings = paymentSettings;
            this._localizationService = localizationService;
        }


        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DescriptionText = _iyzicoPaymentSettings.DescriptionText;
            model.ApiKey = _iyzicoPaymentSettings.ApiKey;
            model.SecretKey = _iyzicoPaymentSettings.SecretKey;
            model.BaseUrl = _iyzicoPaymentSettings.BaseUrl;

            return View("~/Plugins/Payments.Iyzico/Views/Iyzico/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _iyzicoPaymentSettings.DescriptionText = model.DescriptionText;
            _iyzicoPaymentSettings.ApiKey = model.ApiKey;
            _iyzicoPaymentSettings.SecretKey = model.SecretKey;
            _iyzicoPaymentSettings.BaseUrl = model.BaseUrl;
            _settingService.SaveSetting(_iyzicoPaymentSettings);

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            //years
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                model.ExpireYears.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            //months
            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                model.ExpireMonths.Add(new SelectListItem
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            //set postback values
            var form = this.Request.Form;
            model.CardholderName = form["CardholderName"];
            model.CardNumber = form["CardNumber"];
            model.CardCode = form["CardCode"];
            var selectedCcType = model.CreditCardTypes.FirstOrDefault(x => x.Value.Equals(form["CreditCardType"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedCcType != null)
                selectedCcType.Selected = true;
            var selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals(form["ExpireMonth"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            var selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals(form["ExpireYear"], StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            model.CardholderName = "John Doe";
            model.CardNumber = "5528790000000008";
            model.CardCode = "123";
            selectedMonth = model.ExpireMonths.FirstOrDefault(x => x.Value.Equals("12", StringComparison.InvariantCultureIgnoreCase));
            if (selectedMonth != null)
                selectedMonth.Selected = true;
            selectedYear = model.ExpireYears.FirstOrDefault(x => x.Value.Equals("2030", StringComparison.InvariantCultureIgnoreCase));
            if (selectedYear != null)
                selectedYear.Selected = true;

            return View("~/Plugins/Payments.Iyzico/Views/Iyzico/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            //validate 
            var model = new PaymentInfoModel
            {
                CardholderName = form["CardholderName"],
                CardNumber = form["CardNumber"],
                CardCode = form["CardCode"],
                ExpireMonth = form["ExpireMonth"],
                ExpireYear = form["ExpireYear"]
            };
            warnings = PaymentInfoValidator.GetWarning(_localizationService, model); 
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo.CreditCardType = form["CreditCardType"];
            paymentInfo.CreditCardName = form["CardholderName"];
            paymentInfo.CreditCardNumber = form["CardNumber"];
            paymentInfo.CreditCardExpireMonth = int.Parse(form["ExpireMonth"]);
            paymentInfo.CreditCardExpireYear = int.Parse(form["ExpireYear"]);
            paymentInfo.CreditCardCvv2 = form["CardCode"];
            return paymentInfo;
        }
    }
}