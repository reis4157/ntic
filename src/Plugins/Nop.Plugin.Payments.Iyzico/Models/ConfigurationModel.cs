using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Iyzico.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.Iyzico.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Iyzico.ApiKey")]
        public string ApiKey { get; set; }

        [NopResourceDisplayName("Plugins.Payment.Iyzico.SecretKey")]
        public string SecretKey { get; set; } 

        [NopResourceDisplayName("Plugins.Payment.Iyzico.BaseUrl")]
        public string BaseUrl { get; set; }
    }
}