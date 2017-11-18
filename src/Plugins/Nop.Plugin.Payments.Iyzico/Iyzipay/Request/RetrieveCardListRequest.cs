﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class RetrieveCardListRequest : BaseRequest
    {
        public String CardUserKey { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("cardUserKey", CardUserKey)
                .GetRequestString();
        }
    }
}
