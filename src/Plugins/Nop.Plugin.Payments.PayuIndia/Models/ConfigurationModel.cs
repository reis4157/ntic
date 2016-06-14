using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Payments.PayU.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.Payu.MerchantId")]
        public string MerchantId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payu.Key")]
        public string Key { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payu.MerchantParam")]
        public string MerchantParam { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payu.PayUri")]
        public string PayUri { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payu.AdditionalFee")]
        public Decimal AdditionalFee { get; set; }

        public ConfigurationModel()
        {

        }
    }
}
