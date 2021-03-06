﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class CreatePeccoPaymentRequest : BaseRequest
    {
        public String Token { set; get; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("token", Token)
                .GetRequestString();
        }
    }
}
