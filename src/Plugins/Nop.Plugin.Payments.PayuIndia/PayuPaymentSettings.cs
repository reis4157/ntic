using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Payments.PayU
{
    public class PayuPaymentSettings : ISettings
    {
        public string MerchantId { get; set; }

        public string Key { get; set; }

        public string MerchantParam { get; set; }

        public string PayUri { get; set; }

        public Decimal AdditionalFee { get; set; }
    }
}
