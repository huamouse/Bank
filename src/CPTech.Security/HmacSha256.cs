using System;
using System.Security.Cryptography;
using System.Text;

namespace CPTech.Security
{
    public class HmacSha256
    {
        public static string Compute(string data, string key)
        {
            if (data is not { Length: > 0 }) throw new ArgumentNullException(nameof(data));
            if (key is not { Length: > 0 }) throw new ArgumentNullException(nameof(key));

            using var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hsah = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            return BitConverter.ToString(hsah).Replace("-", "");
        }
    }
}
