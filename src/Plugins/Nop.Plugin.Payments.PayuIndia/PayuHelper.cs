using System.Security.Cryptography;
using System.Text;

namespace Nop.Plugin.Payments.PayU
{
    public class PayuHelper
    {
        public string getchecksum(string key, string txnid, string amount, string productinfo, string firstname, string email, string salt)
        {
            return this.Generatehash512(key + "|" + txnid + "|" + amount + "|" + productinfo + "|" + firstname + "|" + email + "|||||||||||" + salt);
        }

        public string verifychecksum(string MerchantId, string OrderId, string Amount, string productinfo, string firstname, string email, string status, string salt)
        {
            return this.Generatehash512(salt + "|" + status + "|||||||||||" + email + "|" + firstname + "|" + productinfo + "|" + Amount + "|" + OrderId + "|" + MerchantId);
        }

        public string Generatehash512(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            SHA512Managed shA512Managed = new SHA512Managed();
            string str = "";
            foreach (byte num in shA512Managed.ComputeHash(bytes))
                str += string.Format("{0:x2}", (object)num);
            return str;
        }
    }
}
