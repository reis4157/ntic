using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Iyzico
{
    public class IyzicoPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        /// <summary>
        /// ApiKey
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// SecretKey
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// BaseUrl
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
