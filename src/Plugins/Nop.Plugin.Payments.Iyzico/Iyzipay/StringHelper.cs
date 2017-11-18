using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay
{
    class StringHelper
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
