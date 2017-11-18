﻿using System;
using System.Text;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay
{
    public sealed class DigestHelper
    {
        private DigestHelper()
        {
        }

        public static String DecodeString(String content)
        {
            return (!String.IsNullOrEmpty(content)) ? Encoding.UTF8.GetString(Convert.FromBase64String(content)) : null;
        }
    }
}
