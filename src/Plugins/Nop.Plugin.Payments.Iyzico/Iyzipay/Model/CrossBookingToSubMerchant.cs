using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class CrossBookingToSubMerchant : IyzipayResource
    {
        public static CrossBookingToSubMerchant Create(CreateCrossBookingRequest request, Options options)
        {
            return RestHttpClient.Create().Post<CrossBookingToSubMerchant>(options.BaseUrl + "/crossbooking/send", GetHttpHeaders(request, options), request);
        }
    }
}
