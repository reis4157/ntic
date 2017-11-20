using System;
using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Iyzico.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;
using Nop.Plugin.Payments.Iyzico.Iyzipay.Model;
using Nop.Plugin.Payments.Iyzico.Iyzipay;
using Nop.Services.Directory;
using Nop.Services.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core;
using Nop.Core.Domain.Customers;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Payments.Iyzico
{
    /// <summary>
    /// Iyzico payment processor
    /// </summary>
    public class IyzicoPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields
        private readonly IyzicoPaymentSettings _iyzicoPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly CurrencySettings _currencySettings;
        private readonly IWebHelper _webHelper;



        private readonly Options _options;
        #endregion

        #region Ctor


        public IyzicoPaymentProcessor(IyzicoPaymentSettings payInStorePaymentSettings,
            ISettingService settingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            CurrencySettings currencySettings,
            IWebHelper webHelper)
        {
            this._iyzicoPaymentSettings = payInStorePaymentSettings;
            this._settingService = settingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;

            this._options = new Options()
            {
                ApiKey = _iyzicoPaymentSettings.ApiKey,
                BaseUrl = _iyzicoPaymentSettings.BaseUrl,
                SecretKey = _iyzicoPaymentSettings.SecretKey
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("Customer cannot be loaded");

            IyziCreatePaymentRequest request = GetPaymentInfo(processPaymentRequest);
            request.PaymentCard = GetPaymentCard(processPaymentRequest);
            request.Buyer = GetBuyer(customer);
            request.ShippingAddress = GetShippingAdress(customer);
            request.BillingAddress = GetBillingAddress(customer);
            request.BasketItems = GetBasketItems(customer);
            //kargo haric urunlerin fiyatý.
            request.Price = request.BasketItems.Sum(f => f.PriceAmount).ToString(CultureInfo.GetCultureInfo("en-US"));
            IyziPayment payment = IyziPayment.Create(request, _options);
            if (payment.Status == Status.SUCCESS.ToString())
            {
                var result = new ProcessPaymentResult();
                result.NewPaymentStatus = PaymentStatus.Paid;
                result.AuthorizationTransactionCode = payment.AuthCode;
                result.AuthorizationTransactionId = payment.PaymentId;
                result.AuthorizationTransactionResult = payment.Status;
                return result;
            }
            else
            {
                var result = new ProcessPaymentResult();
                result.AuthorizationTransactionCode = payment.AuthCode;
                result.AuthorizationTransactionId = payment.PaymentId;
                result.AuthorizationTransactionResult = payment.Status;
                result.NewPaymentStatus = PaymentStatus.Voided;
                result.AuthorizationTransactionResult = payment.ErrorMessage;
                result.AddError(payment.ErrorMessage);
                return result;
            }
        }

        private List<IyziBasketItem> GetBasketItems(Customer customer)
        {
            List<IyziBasketItem> basketItems = new List<IyziBasketItem>();
            foreach (var item in customer.ShoppingCartItems)
            {
                IyziBasketItem firstBasketItem = new IyziBasketItem();
                firstBasketItem.Id = item.Id.ToString();
                firstBasketItem.Name = item.Product.Name;
                int i = 0;
                foreach (var category in item.Product.ProductCategories)
                {
                    if (i == 0)
                        firstBasketItem.Category1 = category.Category.Name;

                    if (i == 1)
                        firstBasketItem.Category2 = category.Category.Name;
                    i++;
                }
                firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                firstBasketItem.PriceAmount = item.Product.Price;
                firstBasketItem.Price = item.Product.Price.ToString(CultureInfo.GetCultureInfo("en-US"));
                basketItems.Add(firstBasketItem);
            }
            return basketItems;
        }

        private IyziAddress GetBillingAddress(Customer customer)
        {
            //shipping
            if (customer.BillingAddress != null)
            {
                IyziAddress shippingAddress = new IyziAddress();
                shippingAddress.ContactName = customer.BillingAddress.FirstName + " " + customer.BillingAddress.LastName;
                shippingAddress.City = customer.BillingAddress.City;
                shippingAddress.Country = customer.BillingAddress.Country.Name;
                shippingAddress.Description = customer.BillingAddress.Address1 + customer.ShippingAddress.Address2;
                shippingAddress.ZipCode = customer.BillingAddress.ZipPostalCode;
                return shippingAddress;
            }
            return null;
        }

        private IyziBuyer GetBuyer(Customer customer)
        {
            IyziBuyer buyer = new IyziBuyer();
            buyer.Id = customer.Id.ToString();
            buyer.Name = customer.BillingAddress.FirstName;
            buyer.Surname = customer.BillingAddress.LastName;
            buyer.GsmNumber = customer.BillingAddress.PhoneNumber;
            buyer.Email = customer.BillingAddress.Email;
            //todo: tc kimlik no eklenecek.
            buyer.IdentityNumber = "1234567890";
            buyer.LastLoginDate = (customer.LastLoginDateUtc != null ?
                                                                        customer.LastLoginDateUtc.Value.ToString("yyyy-MM-dd HH:mm:ss") :
                                                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            buyer.RegistrationDate = customer.CreatedOnUtc.ToString("yyyy-MM-dd HH:mm:ss");
            buyer.RegistrationAddress = customer.BillingAddress.Address1 + customer.BillingAddress.Address2;
            buyer.Ip = customer.LastIpAddress;
            buyer.City = customer.BillingAddress.City;
            buyer.Country = customer.BillingAddress.Country.Name;
            buyer.ZipCode = customer.BillingAddress.ZipPostalCode;
            return buyer;
        }

        private IyziCreatePaymentRequest GetPaymentInfo(ProcessPaymentRequest processPaymentRequest)
        {
            var request = new IyziCreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = processPaymentRequest.OrderGuid.ToString();
            request.Price = processPaymentRequest.OrderTotal.ToString(CultureInfo.GetCultureInfo("en-US"));
            request.PaidPrice = processPaymentRequest.OrderTotal.ToString(CultureInfo.GetCultureInfo("en-US"));
            request.Currency = IyziCurrency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = processPaymentRequest.OrderGuid.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            return request;
        }

        private IyziPaymentCard GetPaymentCard(ProcessPaymentRequest processPaymentRequest)
        {
            IyziPaymentCard paymentCard = new IyziPaymentCard();
            paymentCard.CardHolderName = processPaymentRequest.CreditCardName;
            paymentCard.CardNumber = processPaymentRequest.CreditCardNumber;
            paymentCard.ExpireMonth = processPaymentRequest.CreditCardExpireMonth.ToString().PadLeft(2, '0');
            paymentCard.ExpireYear = processPaymentRequest.CreditCardExpireYear.ToString();
            paymentCard.Cvc = processPaymentRequest.CreditCardCvv2;
            paymentCard.RegisterCard = 0;
            return paymentCard;
        }



        private IyziAddress GetShippingAdress(Customer customer)
        {
            //shipping
            if (customer.ShippingAddress != null)
            {
                IyziAddress shippingAddress = new IyziAddress();
                shippingAddress.ContactName = customer.ShippingAddress.FirstName + " " + customer.ShippingAddress.LastName;
                shippingAddress.City = customer.ShippingAddress.City;
                shippingAddress.Country = customer.ShippingAddress.Country.Name;
                shippingAddress.Description = customer.ShippingAddress.Address1 + customer.ShippingAddress.Address2;
                shippingAddress.ZipCode = customer.ShippingAddress.ZipPostalCode;
                return shippingAddress;
            }
            return null;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            //nothing
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                //_payInStorePaymentSettings.AdditionalFee, _payInStorePaymentSettings.AdditionalFeePercentage);
                0, false);
            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "IyzicoStore";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Iyzico.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "IyzicoStore";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Iyzico.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(IyzicoStoreController);
        }

        public override void Install()
        {
            var settings = new IyzicoPaymentSettings()
            {
                DescriptionText = "Tüm kredi kartlarýna kolayca online alýþveriþ yap!",
                ApiKey = "SandBox.API Anahtarý",
                SecretKey = "SandBox.Güvenlik Anahtarý"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.DescriptionText.Hint", "Enter info that will be shown to customers during checkout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.ApiKey", "Api Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.ApiKey.Hint", "API Anahtarý");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.SecretKey", "Secret Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.SecretKey.Hint", "Güvenlik Anahtarý");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.BaseUrl", "Iyzico Payment Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.Iyzico.BaseUrl.Hint", "Iyzico Payment Url");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<IyzicoPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.ApiKey");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.ApiKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.SecretKey");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.SecretKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.BaseUrl");
            this.DeletePluginLocaleResource("Plugins.Payment.Iyzico.BaseUrl.Hint");
            base.Uninstall();
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Standard;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get { return false; }
        }

        #endregion

    }
}
