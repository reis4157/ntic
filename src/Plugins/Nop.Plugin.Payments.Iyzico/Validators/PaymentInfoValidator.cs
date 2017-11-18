using FluentValidation;
using Nop.Plugin.Payments.Iyzico.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Nop.Plugin.Payments.Iyzico.Validators
{
    public class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        public static List<string> GetWarning(ILocalizationService localizationService, PaymentInfoModel model)
        {
            List<String> messages = new List<string>();
            if (String.IsNullOrWhiteSpace(model.CardholderName))
                messages.Add(localizationService.GetResource("Payment.CardholderName.Required"));

            if (!ValidateCreditCard(model.CardNumber) ||
                GetMod10Digit(model.CardNumber) != 0)
                messages.Add(localizationService.GetResource("Payment.CardNumber.Wrong"));

            if (!Regex.IsMatch(model.CardCode, @"^[0-9]{3,4}$"))
                messages.Add(localizationService.GetResource("Payment.CardCode.Wrong"));

            if (String.IsNullOrWhiteSpace(model.ExpireMonth))
                messages.Add(localizationService.GetResource("Payment.ExpireMonth.Required"));

            if (String.IsNullOrWhiteSpace(model.ExpireYear))
                messages.Add(localizationService.GetResource("Payment.ExpireYear.Required"));

            return messages;
        }
        /// <summary>
        /// Luhn algorithm,if correct card number return 0.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetMod10Digit(string data)
        {
            int sumOfDigits = data.Where((e) => e >= '0' && e <= '9')
            .Reverse()
            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
            .Sum((e) => e / 10 + e % 10);
            //// If the final sum is divisible by 10, then the credit card number is valid. If it is not divisible by 10, the number is invalid.
            return sumOfDigits % 10;
        }

        public static bool ValidateCreditCard(string creditCardNumber)
        {
            //Amex Card: ^3[47][0-9]{13}$
            //BCGlobal: ^(6541|6556)[0-9]{12}$ 
            //Diners Club Card: ^3(?:0[0-5]|[68][0-9])[0-9]{11}$
            //Discover Card: ^65[4-9][0-9]{13}|64[4-9][0-9]{13}|6011[0-9]{12}|(622(?:12[6-9]|1[3-9][0-9]|[2-8][0-9][0-9]|9[01][0-9]|92[0-5])[0-9]{10})$
            //Insta Payment Card: ^63[7-9][0-9]{13}$
            //JCB Card: ^(?:2131|1800|35\d{3})\d{11}$
            //KoreanLocalCard: ^9[0-9]{15}$
            //Laser Card: ^(6304|6706|6709|6771)[0-9]{12,15}$
            //Maestro Card: ^(5018|5020|5038|6304|6759|6761|6763)[0-9]{8,15}$
            //Mastercard: ^5[1-5][0-9]{14}$
            //Solo Card: ^(6334|6767)[0-9]{12}|(6334|6767)[0-9]{14}|(6334|6767)[0-9]{15}$
            //Switch Card: ^(4903|4905|4911|4936|6333|6759)[0-9]{12}|(4903|4905|4911|4936|6333|6759)[0-9]{14}|(4903|4905|4911|4936|6333|6759)[0-9]{15}|564182[0-9]{10}|564182[0-9]{12}|564182[0-9]{13}|633110[0-9]{10}|633110[0-9]{12}|633110[0-9]{13}$
            //Union Pay Card: ^(62[0-9]{14,17})$
            //Visa Card: ^4[0-9]{12}(?:[0-9]{3})?$
            //Visa Master Card: ^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$
            //Rupay Debit Card: ^6[0-9]{15}$

            Dictionary<String, String> regularExpressionList = new Dictionary<string, string>();
            regularExpressionList.Add("AMEX", "^3[47][0-9]{13}$");
            //regularExpressionList.Add("BCGLOBAL", "^(6541|6556)[0-9]{12}$");
            //regularExpressionList.Add("DINNERS", "^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            regularExpressionList.Add("MAESTRO", "^(5018|5020|5038|6304|6759|6761|6763)[0-9]{8,15}$");
            regularExpressionList.Add("MASTER", "^5[1-5][0-9]{14}$");
            //regularExpressionList.Add("UPC", "^(62[0-9]{14,17})$");
            regularExpressionList.Add("VISA", "^4[0-9]{12}(?:[0-9]{3})?$");
            regularExpressionList.Add("VISAMASTERCARD", "^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14})$");
            //Build your Regular Expression
            foreach (var item in regularExpressionList)
            {
                Regex expression = new Regex(item.Value);
                if (expression.IsMatch(creditCardNumber))
                    return true;
            }
            //Return if it was a match or not
            return false;
        }

    }
}