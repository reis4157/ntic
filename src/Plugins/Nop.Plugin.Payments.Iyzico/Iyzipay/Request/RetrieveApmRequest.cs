﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
   public class RetrieveApmRequest : BaseRequest
    {
        public String PaymentId { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("paymentId", PaymentId)           
                .GetRequestString();
        }
    }
}
