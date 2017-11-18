using Nop.Plugin.Payments.Iyzico.Iyzipay.Model;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class CreateBkmInitializeRequest : BaseRequest
    {
        public String Price { get; set; }
        public String BasketId { get; set; }
        public String PaymentGroup { get; set; }
        public String PaymentSource { get; set; }
        public IyziBuyer Buyer { get; set; }
        public IyziAddress ShippingAddress { get; set; }
        public IyziAddress BillingAddress { get; set; }
        public List<IyziBasketItem> BasketItems { get; set; }
        public String CallbackUrl { get; set; }

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
                .GetRequestString();
        }
    }
}
