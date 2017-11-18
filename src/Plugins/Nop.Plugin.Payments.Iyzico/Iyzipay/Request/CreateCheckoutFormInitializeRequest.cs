using Nop.Plugin.Payments.Iyzico.Iyzipay.Model;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class CreateCheckoutFormInitializeRequest : BaseRequest
    {
        public String Price { get; set; }
        public String PaidPrice { get; set; }
        public String BasketId { get; set; }
        public String PaymentGroup { get; set; }
        public String PaymentSource { get; set; }
        public String Currency { get; set; }
        public IyziBuyer Buyer { get; set; }
        public IyziAddress ShippingAddress { get; set; }
        public IyziAddress BillingAddress { get; set; }
        public List<IyziBasketItem> BasketItems { get; set; }
        public String CallbackUrl { get; set; }
        public int? ForceThreeDS { get; set; }
        public String CardUserKey { get; set; }
        public String PosOrderId { get; set; }
        public List<int> EnabledInstallments { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .AppendPrice("price", Price)
                .Append("basketId", BasketId)
                .Append("paymentGroup", PaymentGroup)
                .Append("buyer", Buyer)
                .Append("shippingAddress", ShippingAddress)
                .Append("billingAddress", BillingAddress)
                .AppendList("basketItems", BasketItems)
                .Append("callbackUrl", CallbackUrl)
                .Append("paymentSource", PaymentSource)
                .Append("currency", Currency)
                .Append("posOrderId", PosOrderId)
                .AppendPrice("paidPrice", PaidPrice)
                .Append("forceThreeDS", ForceThreeDS)
                .Append("cardUserKey", CardUserKey)
                .AppendList("enabledInstallments", EnabledInstallments)
                .GetRequestString();
        }
    }
}
