using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayU.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Routing;

namespace Nop.Plugin.Payments.PayU
{
    public class PayuPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        private readonly PayuPaymentSettings _PayuPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return (RecurringPaymentType)0;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return (PaymentMethodType)15;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        public PayuPaymentProcessor(PayuPaymentSettings PayuPaymentSettings, ISettingService settingService, ICurrencyService currencyService, CurrencySettings currencySettings, IWebHelper webHelper)
        {
            this._PayuPaymentSettings = PayuPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.NewPaymentStatus = PaymentStatus.Pending;
            //todo:currency set ile alakalı geliştirme yapılcak.
            //if (_currencyService.GetCurrencyByCode("INR").CurrencyCode)
            //if (!BaseEntity.Equals(this._currencyService.GetCurrencyByCode("INR"), (BaseEntity)null) || !this._currencyService.GetCurrencyByCode("INR").get_Published())
            processPaymentResult.AddError("You need to enable TR currency from nopcommerce admin. Go to Settings=> Currency and add INR Currencies Or Contact the admin.");
            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            PayuHelper payuHelper = new PayuHelper();
            int id;
            string str;
            if (!(this._PayuPaymentSettings.PayUri == "https://test.payu.in/_payment"))
            {
                id = ((BaseEntity)postProcessPaymentRequest.Order).Id;
                str = id.ToString();
            }
            else
                str = ((BaseEntity)postProcessPaymentRequest.Order).Id.ToString() + DateTime.Now.Ticks.ToString().Substring(0, 5);
            string txnid = str;
            id = ((BaseEntity)postProcessPaymentRequest.Order).Id;
            id.ToString();
            postProcessPaymentRequest.Order.OrderTotal = this._currencyService.ConvertFromPrimaryExchangeRateCurrency(postProcessPaymentRequest.Order.OrderTotal, _currencyService.GetCurrencyByCode("INR"));
            RemotePost remotePost = new RemotePost();
            remotePost.FormName = "PayuForm";
            remotePost.Url = this._PayuPaymentSettings.PayUri;
            remotePost.Add("key", this._PayuPaymentSettings.MerchantId.ToString());
            remotePost.Add("amount", postProcessPaymentRequest.Order.OrderTotal.ToString((IFormatProvider)new CultureInfo("en-US", false).NumberFormat));
            remotePost.Add("productinfo", "productinfo");
            remotePost.Add("Currency", this._currencyService.GetCurrencyById(this._currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
            remotePost.Add("Order_Id", txnid);
            remotePost.Add("txnid", txnid);
            remotePost.Add("surl", this._webHelper.GetStoreLocation(false) + "Plugins/PaymentPayu/Return");
            remotePost.Add("furl", this._webHelper.GetStoreLocation(false) + "Plugins/PaymentPayu/Return");
            remotePost.Add("hash", payuHelper.getchecksum(this._PayuPaymentSettings.MerchantId.ToString(), txnid, postProcessPaymentRequest.Order.OrderTotal.ToString((IFormatProvider)new CultureInfo("en-US", false).NumberFormat), "productinfo", postProcessPaymentRequest.Order.BillingAddress.FirstName.ToString(), postProcessPaymentRequest.Order.BillingAddress.Email.ToString(), this._PayuPaymentSettings.Key));
            remotePost.Add("firstname", postProcessPaymentRequest.Order.BillingAddress.FirstName.ToString());
            remotePost.Add("billing_cust_address", postProcessPaymentRequest.Order.BillingAddress.Address1);
            remotePost.Add("phone", postProcessPaymentRequest.Order.BillingAddress.PhoneNumber);
            remotePost.Add("email", postProcessPaymentRequest.Order.BillingAddress.Email.ToString());
            remotePost.Add("billing_cust_city", postProcessPaymentRequest.Order.BillingAddress.City);
            StateProvince stateProvince1 = postProcessPaymentRequest.Order.BillingAddress.StateProvince;
            if (stateProvince1 != null)
                remotePost.Add("billing_cust_state", stateProvince1.Abbreviation);
            else
                remotePost.Add("billing_cust_state", "");
            remotePost.Add("billing_zip_code", postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode);
            Country country1 = postProcessPaymentRequest.Order.BillingAddress.Country;
            if (country1 != null)
                remotePost.Add("billing_cust_country", country1.ThreeLetterIsoCode);
            else
                remotePost.Add("billing_cust_country", "");


            if (postProcessPaymentRequest.Order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                remotePost.Add("delivery_cust_name", postProcessPaymentRequest.Order.ShippingAddress.FirstName);
                remotePost.Add("delivery_cust_address", postProcessPaymentRequest.Order.ShippingAddress.Address1);
                remotePost.Add("delivery_cust_notes", string.Empty);
                remotePost.Add("delivery_cust_tel", postProcessPaymentRequest.Order.ShippingAddress.PhoneNumber);
                remotePost.Add("delivery_cust_city", postProcessPaymentRequest.Order.ShippingAddress.City);
                StateProvince stateProvince2 = postProcessPaymentRequest.Order.ShippingAddress.StateProvince;
                if (stateProvince2 != null)
                    remotePost.Add("delivery_cust_state", stateProvince2.Abbreviation);
                else
                    remotePost.Add("delivery_cust_state", "");
                remotePost.Add("delivery_zip_code", postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode);
                Country country2 = postProcessPaymentRequest.Order.ShippingAddress.Country;
                if (country2 != null)
                    remotePost.Add("delivery_cust_country", country2.ThreeLetterIsoCode);
                else
                    remotePost.Add("delivery_cust_country", "");
            }
            remotePost.Post();
        }

        public Decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return this._PayuPaymentSettings.AdditionalFee;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            CapturePaymentResult capturePaymentResult = new CapturePaymentResult();
            capturePaymentResult.AddError("Capture method not supported");
            return capturePaymentResult;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult refundPaymentResult = new RefundPaymentResult();
            refundPaymentResult.AddError("Refund method not supported");
            return refundPaymentResult;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            VoidPaymentResult voidPaymentResult = new VoidPaymentResult();
            voidPaymentResult.AddError("Void method not supported");
            return voidPaymentResult;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.AddError("Recurring payment not supported");
            return processPaymentResult;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            CancelRecurringPaymentResult recurringPaymentResult = new CancelRecurringPaymentResult();
            recurringPaymentResult.AddError("Recurring payment not supported");
            return recurringPaymentResult;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            return order.PaymentStatus == PaymentStatus.Pending && (DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes >= 1.0;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayu";
            routeValues = new RouteValueDictionary()
      {
        {
          "Namespaces",
          (object) "Nop.Plugin.Payments.PayU.Controllers"
        },
        {
          "area",
          (object) null
        }
      };
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPayu";
            routeValues = new RouteValueDictionary()
      {
        {
          "Namespaces",
          (object) "Nop.Plugin.Payments.PayU.Controllers"
        },
        {
          "area",
          (object) null
        }
      };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayuController);
        }

        public virtual void Install()
        {
            this._settingService.SaveSetting<PayuPaymentSettings>(new PayuPaymentSettings()
            {
                MerchantId = "",
                Key = "",
                MerchantParam = "",
                PayUri = "https://test.payu.in/_payment",
                AdditionalFee = new Decimal(0)
            }, 0);
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.RedirectionTip", "You will be redirected to Payu site to complete the order.");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantId", "Merchant ID");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantId.Hint", "Enter merchant ID.");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.Key", "Working Key");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.Key.Hint", "Enter working key.");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantParam", "Merchant Param");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantParam.Hint", "Enter merchant param.");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.PayUri", "Pay URI");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.PayUri.Hint", "Enter Pay URI.");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.AdditionalFee", "Additional fee");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            base.Install();
        }

        public virtual void Uninstall()
        {
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.RedirectionTip");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantId");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantId.Hint");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.Key");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.Key.Hint");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantParam");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.MerchantParam.Hint");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.PayUri");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.PayUri.Hint");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.AdditionalFee");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.Payu.AdditionalFee.Hint");
            base.Uninstall();
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }
    }
}