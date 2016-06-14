// Decompiled with JetBrains decompiler
// Type: Nop.Plugin.Payments.PayU.Models.PaymentInfoModel
// Assembly: Nop.Plugin.Payments.PayU, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C21DCEB9-A37E-4087-977B-1759CF8D8225
// Assembly location: A:\PERSONAL\Projects\62945_72412_PayU_Plugin\Nop.Plugin.Payments.PayU.dll

using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.PayU.Models
{
  public class PaymentInfoModel : BaseNopModel
  {
    [AllowHtml]
    [NopResourceDisplayName("Payment.SelectCreditCard")]
    public string CreditCardType { get; set; }

    [NopResourceDisplayName("Payment.SelectCreditCard")]
    public IList<SelectListItem> CreditCardTypes { get; set; }

    [AllowHtml]
    [NopResourceDisplayName("Payment.CardholderName")]
    public string CardholderName { get; set; }

    [AllowHtml]
    [NopResourceDisplayName("Payment.CardNumber")]
    public string CardNumber { get; set; }

    [AllowHtml]
    [NopResourceDisplayName("Payment.ExpirationDate")]
    public string ExpireMonth { get; set; }

    [NopResourceDisplayName("Payment.ExpirationDate")]
    [AllowHtml]
    public string ExpireYear { get; set; }

    public IList<SelectListItem> ExpireMonths { get; set; }

    public IList<SelectListItem> ExpireYears { get; set; }

    [NopResourceDisplayName("Payment.CardCode")]
    [AllowHtml]
    public string CardCode { get; set; }

    public PaymentInfoModel()
    {
      this.CreditCardTypes = (IList<SelectListItem>) new List<SelectListItem>();
      this.ExpireMonths = (IList<SelectListItem>) new List<SelectListItem>();
      this.ExpireYears = (IList<SelectListItem>) new List<SelectListItem>();
    }
  }
}
