using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class BasicPayment : BasicPaymentResource
    {
        public static BasicPayment Create(CreateBasicPaymentRequest request, Options options)
        {
            return RestHttpClient.Create().Post<BasicPayment>(options.BaseUrl + "/payment/auth/basic", GetHttpHeaders(request, options), request);
        }
    }
}
