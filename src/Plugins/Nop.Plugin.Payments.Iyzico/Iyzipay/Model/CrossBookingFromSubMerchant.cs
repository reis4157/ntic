using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class CrossBookingFromSubMerchant : IyzipayResource
    {
        public static CrossBookingFromSubMerchant Create(CreateCrossBookingRequest request, Options options)
        {
            return RestHttpClient.Create().Post<CrossBookingFromSubMerchant>(options.BaseUrl + "/crossbooking/receive", GetHttpHeaders(request, options), request);
        }
    }
}
