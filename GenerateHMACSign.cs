using System;
using System.Security.Cryptography;
using System.Text;

namespace GrlReportSign
{
    public class HMACSignature : ISignatureProvider
    {
        private string GenerateKey(string content)
        {
            return content.Length.ToString();
        }

        public string GenerateSignature(string content)
        {
            string key = GenerateKey(content);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifySignature(string content, string signature)
        {
            string computedSignature = GenerateSignature(content);
            return computedSignature.Equals(signature, StringComparison.Ordinal);
        }
    }
}
