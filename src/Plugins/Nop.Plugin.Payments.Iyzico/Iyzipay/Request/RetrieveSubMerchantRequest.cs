﻿using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Request
{
    public class RetrieveSubMerchantRequest : BaseRequest
    {
        public String SubMerchantExternalId { get; set; }

        public override String ToPKIRequestString()
        {
            return ToStringRequestBuilder.NewInstance()
                .AppendSuper(base.ToPKIRequestString())
                .Append("subMerchantExternalId", SubMerchantExternalId)
                .GetRequestString();
        }
    }
}
