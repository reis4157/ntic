using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayU
{
    public class PayUPaymentSettings : ISettings
    {
        public string IsyeriKodu { get; set; }

        public string Anahtar { get; set; }
    }
}
