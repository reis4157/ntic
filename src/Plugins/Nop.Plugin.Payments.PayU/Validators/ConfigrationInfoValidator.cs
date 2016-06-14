//using FluentValidation;
using Nop.Plugin.Payments.PayU.Models;
using Nop.Services.Localization;
using System;
using System.Collections;
using System.Linq.Expressions;

namespace Nop.Plugin.Payments.PayU.Validators
{
  public class ConfigrationInfoValidator : ConfigurationModel // AbstractValidator<ConfigurationModel>
  {
    public ConfigrationInfoValidator(ILocalizationService localizationService)
    { 
        
      //DefaultValidatorExtensions.NotNull<ConfigurationModel, string>((IRuleBuilder<M0, M1>) this.RuleFor<string>((Expression<Func<ConfigurationModel, M0>>) (x => x.Anahtar)));
      //DefaultValidatorExtensions.NotNull<ConfigurationModel, string>((IRuleBuilder<M0, M1>) this.RuleFor<string>((Expression<Func<ConfigurationModel, M0>>) (x => x.IsyeriKodu)));
    }
  }
}
