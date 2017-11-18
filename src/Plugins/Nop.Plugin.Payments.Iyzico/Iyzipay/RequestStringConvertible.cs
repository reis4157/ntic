using System;

namespace Nop.Plugin.Payments.Iyzico.Iyzipay
{
    public interface RequestStringConvertible
    {
        String ToPKIRequestString();
    }
}
