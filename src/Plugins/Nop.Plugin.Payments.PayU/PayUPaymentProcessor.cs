using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.PayU.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Payments.PayU
{
    public class PayUPaymentProcessor : BasePlugin, IPaymentMethod, IPlugin
    {
        private readonly PayUPaymentSettings _PayUPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly HttpContextBase _httpContext;

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
                return RecurringPaymentType.NotSupported;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        public PayUPaymentProcessor(PayUPaymentSettings PayUPaymentSettings, ISettingService settingService, ICurrencyService currencyService, ICustomerService customerService, CurrencySettings currencySettings, IWebHelper webHelper, IOrderTotalCalculationService orderTotalCalculationService, StoreInformationSettings storeInformationSettings, HttpContextBase httpContext)
        {
            this._PayUPaymentSettings = PayUPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._storeInformationSettings = storeInformationSettings;
            this._httpContext = httpContext;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            ProcessPaymentResult processPaymentResult = new ProcessPaymentResult();
            processPaymentResult.NewPaymentStatus = PaymentStatus.Pending;
            return processPaymentResult;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            string str1 = "https://secure.payu.com.tr/order/lu.php";
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.Append("<html>");
            stringBuilder2.Append("<head>");
            stringBuilder2.Append("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            stringBuilder2.Append("</head>");
            stringBuilder2.AppendFormat("<body onload='document.forms[\"form\"].submit()'>");
            stringBuilder2.AppendFormat("<form name='form' action='{0}' method='post' accept-charset='UTF-8'>", str1);
            stringBuilder1.Append(_PayUPaymentSettings.IsyeriKodu.Length + _PayUPaymentSettings.IsyeriKodu);
            StringBuilder stringBuilder3 = stringBuilder1;
            int id = ((BaseEntity)postProcessPaymentRequest.Order).Id;
            string OrderId = id.ToString().Length.ToString() + id.ToString();
            stringBuilder3.Append(OrderId);
            stringBuilder1.Append(string.Format("{0:yyyy-MM-dd HH:mm:ss}", postProcessPaymentRequest.Order.CreatedOnUtc).Length + string.Format("{0:yyyy-MM-dd HH:mm:ss}", postProcessPaymentRequest.Order.CreatedOnUtc));
            stringBuilder2.AppendFormat("<input type='hidden' name='MERCHANT' value='{0}'>", _PayUPaymentSettings.IsyeriKodu);
            stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_REF' value='{0}'>", postProcessPaymentRequest.Order.Id.ToString());
            stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_DATE' value='{0}'>", string.Format("{0:yyyy-MM-dd HH:mm:ss}", postProcessPaymentRequest.Order.CreatedOnUtc));

            ICollection<OrderItem> orderProductVariants = postProcessPaymentRequest.Order.OrderItems;
            int num1 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OrderItem current = enumerator.Current;
                    string productName = current.Product.Name.Length > 155 ? current.Product.Name.Substring(0, 155) : current.Product.Name;
                    stringBuilder1.Append(productName.Length + productName);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_PNAME[" + num1 + "]' value='{0}'>", productName);
                    ++num1;
                }
            }

            int num2 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OrderItem current = enumerator.Current;
                    string productId = current.Product.Sku == null ? current.ProductId.ToString() : current.Product.Sku;
                    productId = productId.Length > 50 ? productId.Substring(0, 50) : productId;
                    stringBuilder1.Append(productId.Length + productId);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_PCODE[" + num2 + "]' value='{0}'>", productId);
                    ++num2;
                }
            }

            int num3 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OrderItem current = enumerator.Current;
                    stringBuilder1.Append("0");
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_PINFO[" + num3 + "]' value='{0}'>", "");
                    ++num3;
                }
            }

            int num4 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    string UnitPriceExclTax = Math.Round(enumerator.Current.UnitPriceExclTax, 2).ToString();
                    stringBuilder1.Append(UnitPriceExclTax.Length + UnitPriceExclTax);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_PRICE[" + num4 + "]' value='{0}'>", UnitPriceExclTax);
                    ++num4;
                }
            }

            int num5 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    string quantity = enumerator.Current.Quantity.ToString();
                    stringBuilder1.Append(quantity.Length + quantity);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_QTY[" + num5 + "]' value='{0}'>", quantity);
                    ++num5;
                }
            }
            int num6 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OrderItem current = enumerator.Current;
                    string str4 = "18";
                    stringBuilder1.Append(str4.Length + str4);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_VAT[" + num6 + "]' value='{0}'>", str4);
                    ++num6;
                }
            }

            string orderShippingInclTax = Math.Round(postProcessPaymentRequest.Order.OrderShippingInclTax, 2).ToString();
            stringBuilder1.Append(orderShippingInclTax.Length + orderShippingInclTax);
            stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_SHIPPING' value='{0}'>", orderShippingInclTax);
            postProcessPaymentRequest.Order.CustomerCurrencyCode.ToString();
            stringBuilder1.Append("3TRY");
            stringBuilder2.AppendFormat("<input type='hidden' name='PRICES_CURRENCY' value='{0}'>", "TRY");
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_FNAME' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.FirstName);
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_LNAME' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.LastName);
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_EMAIL' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.Email);
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_ADDRESS' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.Address1);
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_ADDRESS2' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.Address2);
            stringBuilder2.AppendFormat("<input type='hidden' name='DELIVERY_ADDRESS' value='{0}'>", postProcessPaymentRequest.Order.ShippingAddress.Address1);
            stringBuilder2.AppendFormat("<input type='hidden' name='DELIVERY_ADDRESS2' value='{0}'>", postProcessPaymentRequest.Order.ShippingAddress.Address2);
            stringBuilder2.AppendFormat("<input type='hidden' name='BILL_CITY' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.City);
            if (postProcessPaymentRequest.Order.BillingAddress.StateProvince != null)
                stringBuilder2.AppendFormat("<input type='hidden' name='BILL_STATE' value='{0}'>", postProcessPaymentRequest.Order.BillingAddress.StateProvince.Name);
            stringBuilder2.AppendFormat("<input type='hidden' name='DELIVERY_CITY' value='{0}'>", postProcessPaymentRequest.Order.ShippingAddress.City);
            if (postProcessPaymentRequest.Order.ShippingAddress.StateProvince != null)
                stringBuilder2.AppendFormat("<input type='hidden' name='DELIVERY_STATE' value='{0}'>", postProcessPaymentRequest.Order.ShippingAddress.StateProvince.Name);
            string orderDiscount = Math.Round(postProcessPaymentRequest.Order.OrderDiscount, 2).ToString();
            stringBuilder1.Append(orderDiscount.Length + orderDiscount);
            stringBuilder2.AppendFormat("<input type='hidden' name='DISCOUNT' value='{0}'>", orderDiscount);
            string city = postProcessPaymentRequest.Order.ShippingAddress.City;
            stringBuilder1.Append(city.Length + city);
            stringBuilder2.AppendFormat("<input type='hidden' name='DESTINATION_CITY' value='{0}'>", city);
            string name = postProcessPaymentRequest.Order.ShippingAddress.StateProvince.Name;
            stringBuilder1.Append(name.Length + name);
            stringBuilder2.AppendFormat("<input type='hidden' name='DESTINATION_STATE' value='{0}'>", name);
            string twoLetterIsoCode = postProcessPaymentRequest.Order.ShippingAddress.Country.TwoLetterIsoCode;
            stringBuilder1.Append(twoLetterIsoCode.Length + twoLetterIsoCode);
            stringBuilder2.AppendFormat("<input type='hidden' name='DESTINATION_COUNTRY' value='{0}'>", twoLetterIsoCode);
            string payMethod = "CCVISAMC";
            stringBuilder1.Append(payMethod.Length + payMethod);
            stringBuilder2.AppendFormat("<input type='hidden' name='PAY_METHOD' value='{0}'>", payMethod);
            int num7 = 0;
            using (IEnumerator<OrderItem> enumerator = ((IEnumerable<OrderItem>)orderProductVariants).GetEnumerator())
            {
                while (((IEnumerator)enumerator).MoveNext())
                {
                    OrderItem current = enumerator.Current;
                    string orderPriceType = "GROSS";
                    stringBuilder1.Append(orderPriceType.Length + orderPriceType);
                    stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_PRICE_TYPE[" + num7 + "]' value='{0}'>", orderPriceType);
                    ++num7;
                }
            }
            string installmentOption = "2,3,7,10,12";
            stringBuilder1.Append(installmentOption.Length + installmentOption);
            stringBuilder2.AppendFormat("<input type='hidden' name='INSTALLMENT_OPTIONS' value='{0}'>", installmentOption);
            stringBuilder2.AppendFormat("<input type='hidden' name='TESTORDER' value='{0}'>", "0");
            stringBuilder2.AppendFormat("<input type='hidden' name='LANGUAGE' value='{0}'>", "TR");
            string str10 = this._webHelper.GetStoreLocation(false) + "Plugins/PaymentPayU/PaymentHandler?ORDER=" + ((BaseEntity)postProcessPaymentRequest.Order).Id.ToString();
            stringBuilder2.AppendFormat("<input type='hidden' name='BACK_REF' value='{0}'>", str10);
            Encoding utF8 = Encoding.UTF8;
            byte[] hash = new HMACMD5(utF8.GetBytes(this._PayUPaymentSettings.Anahtar)).ComputeHash(utF8.GetBytes(stringBuilder1.ToString()));
            StringBuilder stringBuilder4 = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder4.Append(hash[index].ToString("x2"));
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            string s = stringBuilder4.ToString();
            byte[] bytes = utF8Encoding.GetBytes(s);
            stringBuilder2.AppendFormat("<input type='hidden' name='ORDER_HASH' value='{0}'>", bytes.ToString());
            this._httpContext.Response.Clear();
            stringBuilder2.Append("</form>");
            stringBuilder2.Append("</body>");
            stringBuilder2.Append("</html>");
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            object obj = DateTime.ParseExact("20130416", "yyyyMMdd", (IFormatProvider)null);
            this._httpContext.Response.Write(utF8Encoding.GetBytes(stringBuilder2.ToString()));
            this._httpContext.Response.End();
        }

        public Decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return new Decimal(0);
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
            return (DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes >= 1.0;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPayU";
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
            controllerName = "PaymentPayU";
            routeValues = new RouteValueDictionary(){
                                                        {
                                                          "Namespaces",
                                                           "Nop.Plugin.Payments.PayU.Controllers"
                                                        },
                                                        {
                                                          "area",
                                                           null
                                                        }
                                                      };



        }

        public Type GetControllerType()
        {
            return typeof(PaymentPayUController);
        }

        public virtual void Install()
        {
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.IsyeriKodu", "1234");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.IsyeriKodu.Hint", "İşyeri kodunuzu girin");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.Anahtar", "1234");
            LocalizationExtensions.AddOrUpdatePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.Anahtar.Hint", "Anahtarı girin");  
            base.Install();
        }

        public virtual void Uninstall()
        {
            this._settingService.DeleteSetting<PayUPaymentSettings>();
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.IsyeriKodu");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.IsyeriKodu.Hint");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.Anahtar");
            LocalizationExtensions.DeletePluginLocaleResource((BasePlugin)this, "Plugins.Payments.PayU.Fields.Anahtar.Hint"); 
            base.Uninstall();
        }



    }
}
