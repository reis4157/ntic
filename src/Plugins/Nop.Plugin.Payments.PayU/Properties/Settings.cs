// Decompiled with JetBrains decompiler
// Type: Nop.Plugin.Payments.PayU.Properties.Settings
// Assembly: Nop.Plugin.Payments.PayU, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C21DCEB9-A37E-4087-977B-1759CF8D8225
// Assembly location: A:\PERSONAL\Projects\62945_72412_PayU_Plugin\Nop.Plugin.Payments.PayU.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Nop.Plugin.Payments.PayU.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings settings = Settings.defaultInstance;
        return settings;
      }
    }

    [ApplicationScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("https://apitest.authorize.net/soap/v1/Service.asmx")]
    [SpecialSetting(SpecialSetting.WebServiceUrl)]
    public string Nop_Plugin_Payments_PayU_net_authorize_api_Service
    {
      get
      {
        return (string) this["Nop_Plugin_Payments_PayU_net_authorize_api_Service"];
      }
    }
  }
}
