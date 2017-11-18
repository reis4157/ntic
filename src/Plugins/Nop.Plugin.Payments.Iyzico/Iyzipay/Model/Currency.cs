using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay.Model
{
    public sealed class IyziCurrency
    {
        private readonly String value;

        public static readonly IyziCurrency TRY = new IyziCurrency("TRY");
        public static readonly IyziCurrency EUR = new IyziCurrency("EUR");
        public static readonly IyziCurrency USD = new IyziCurrency("USD");
        public static readonly IyziCurrency GBP = new IyziCurrency("GBP");
        public static readonly IyziCurrency IRR = new IyziCurrency("IRR");
        public static readonly IyziCurrency NOK = new IyziCurrency("NOK");
        public static readonly IyziCurrency RUB = new IyziCurrency("RUB");
        public static readonly IyziCurrency CHF = new IyziCurrency("CHF");

        private IyziCurrency(String value)
        {
            this.value = value;
        }

        public override String ToString()
        {
            return value;
        }
    }
}
