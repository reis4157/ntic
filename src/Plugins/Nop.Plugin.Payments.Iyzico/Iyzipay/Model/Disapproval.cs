using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;
using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class Disapproval : IyzipayResource
    {
        public String PaymentTransactionId { get; set; }

        public static Disapproval Create(CreateApprovalRequest request, Options options)
        {
            return RestHttpClient.Create().Post<Disapproval>(options.BaseUrl + "/payment/iyzipos/item/disapprove", GetHttpHeaders(request, options), request);
        }
    }
}
