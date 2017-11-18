using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class IyziPayment : PaymentResource
    {
        public static IyziPayment Create(IyziCreatePaymentRequest request, Options options)
        {
            return RestHttpClient.Create().Post<IyziPayment>(options.BaseUrl + "/payment/auth", GetHttpHeaders(request, options), request);
        }

        public static IyziPayment Retrieve(RetrievePaymentRequest request, Options options)
        {
            return RestHttpClient.Create().Post<IyziPayment>(options.BaseUrl + "/payment/detail", GetHttpHeaders(request, options), request);
        }
    }
}
