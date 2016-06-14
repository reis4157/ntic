using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayU.Models
{
  public class ConfigurationModel : BaseNopModel
  {
    [NopResourceDisplayName("Plugins.Payments.PayU.Fields.IsyeriKodu")]
    public string IsyeriKodu { get; set; } 

    [NopResourceDisplayName("Plugins.Payments.PayU.Fields.Anahtar")]
    public string Anahtar { get; set; } 

    public ConfigurationModel()
    {

    }
  }
}
