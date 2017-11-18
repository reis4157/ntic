using Nop.Plugin.Payments.Iyzico.Iyzipay.Request;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class PaymentPostAuth : PaymentResource
    {        
        public static PaymentPostAuth Create(CreatePaymentPostAuthRequest request, Options options)
        {
            return RestHttpClient.Create().Post<PaymentPostAuth>(options.BaseUrl + "/payment/postauth", GetHttpHeaders(request, options), request);
        }
    }
}
