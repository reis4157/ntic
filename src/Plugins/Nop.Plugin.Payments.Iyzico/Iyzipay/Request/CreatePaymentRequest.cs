using Nop.Plugin.Payments.Iyzico.Iyzipay.Model;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class IyziCreatePaymentRequest : BaseRequest
    {
        public String Price { get; set; }
        public String PaidPrice { get; set; }
        public int? Installment { get; set; }
        public String PaymentChannel { get; set; }
        public String BasketId { get; set; }
        public String PaymentGroup { get; set; }
        public IyziPaymentCard PaymentCard { get; set; }
        public IyziBuyer Buyer { get; set; }
        public IyziAddress ShippingAddress { get; set; }
        public IyziAddress BillingAddress { get; set; }
        public List<IyziBasketItem> BasketItems { get; set; }
        public String PaymentSource { get; set; }
        public String CallbackUrl { get; set; }
        public String PosOrderId { get; set; }
        public String ConnectorName { get; set; }
        public String Currency { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .AppendPrice("price", Price)
                .AppendPrice("paidPrice", PaidPrice)
                .Append("installment", Installment)
                .Append("paymentChannel", PaymentChannel)
                .Append("basketId", BasketId)
                .Append("paymentGroup", PaymentGroup)
                .Append("paymentCard", PaymentCard)               
                .Append("buyer", Buyer)
                .Append("shippingAddress", ShippingAddress)
                .Append("billingAddress", BillingAddress)
                .AppendList("basketItems", BasketItems)
                .Append("paymentSource", PaymentSource)
                .Append("currency", Currency)
                .Append("posOrderId", PosOrderId)
                .Append("connectorName", ConnectorName)
                .Append("callbackUrl", CallbackUrl)
                .GetRequestString();
        }
    }
}
