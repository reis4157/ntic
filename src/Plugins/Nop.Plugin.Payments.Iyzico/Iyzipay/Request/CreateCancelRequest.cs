﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class CreateCancelRequest : BaseRequest
    {
        public String PaymentId { get; set; }
        public String Ip { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("paymentId", PaymentId)
                .Append("ip", Ip)
                .GetRequestString();
        }
    }
}
