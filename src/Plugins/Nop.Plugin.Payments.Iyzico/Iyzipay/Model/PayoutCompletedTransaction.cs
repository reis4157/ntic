using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public class PayoutCompletedTransaction
    {
        public String PaymentTransactionId { get; set; }
        public String PayoutAmount { get; set; }
        public String PayoutType { get; set; }
        public String SubMerchantKey { get; set; }
        public String Currency { get; set; }
    }
}
