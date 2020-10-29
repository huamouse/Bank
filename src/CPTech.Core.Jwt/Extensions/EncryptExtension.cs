using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace CPTech.Extensions
{
    public static class EncryptExtension
    {
        public static string AESEncrypt(this string input, string key)
        {
            var encryptKey = Encoding.UTF8.GetBytes(key);

            using var aesAlg = Aes.Create();

            using var encryptor = aesAlg.CreateEncryptor(encryptKey, aesAlg.IV);

            using var msEncrypt = new MemoryStream();

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor,
                CryptoStreamMode.Write))

            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(input);
            }

            var iv = aesAlg.IV;

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result,
                iv.Length, decryptedContent.Length);

            string base64String = Convert.ToBase64String(result);
            return base64String;

        }

        public static string AESDecrypt(this string input, string key)
        {
            var fullCipher = Convert.FromBase64String(input);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
            var decryptKey = Encoding.UTF8.GetBytes(key);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(decryptKey, iv);

            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                using var srDecrypt = new StreamReader(csDecrypt);

                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        public static string MD5Encrypt(this string txt)
        {
            using MD5 mi = MD5.Create();

                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
        }
    }
}
