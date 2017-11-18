using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;
using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class ThreedsInitialize : IyzipayResource
    {
        [JsonProperty(PropertyName = "threeDSHtmlContent")]
        public String HtmlContent { get; set; }

        public static ThreedsInitialize Create(IyziCreatePaymentRequest request, Options options)
        {
            ThreedsInitialize response = RestHttpClient.Create().Post<ThreedsInitialize>(options.BaseUrl + "/payment/3dsecure/initialize", GetHttpHeaders(request, options), request);

            if (response != null)
            {
                response.HtmlContent = DigestHelper.DecodeString(response.HtmlContent);
            }
            return response;
        }
    }
}
