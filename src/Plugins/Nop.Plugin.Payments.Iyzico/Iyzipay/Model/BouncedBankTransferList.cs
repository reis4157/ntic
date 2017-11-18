using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class BouncedBankTransferList : IyzipayResource
    {
        [JsonProperty(PropertyName = "bouncedRows")]
        public List<BankTransfer> BankTransfers { get; set; }

        public static BouncedBankTransferList Retrieve(RetrieveTransactionsRequest request, Options options)
        {
            return RestHttpClient.Create().Post<BouncedBankTransferList>(options.BaseUrl + "/reporting/settlement/bounced", GetHttpHeaders(request, options), request);
        }
    }
}
