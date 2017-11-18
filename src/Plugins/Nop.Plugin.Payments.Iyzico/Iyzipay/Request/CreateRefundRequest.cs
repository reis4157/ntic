﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class CreateRefundRequest : BaseRequest
    {
        public String PaymentTransactionId { get; set; }
        public String Price { get; set; }
        public String Ip { get; set; }
        public String Currency { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("paymentTransactionId", PaymentTransactionId)
                .AppendPrice("price", Price)
                .Append("ip", Ip)
                .Append("currency", Currency)
                .GetRequestString();
        }
    }
}
