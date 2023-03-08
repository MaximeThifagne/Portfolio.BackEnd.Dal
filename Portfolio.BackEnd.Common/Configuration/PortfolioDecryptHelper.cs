using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.BackEnd.Common.Configuration
{
    public class PortfolioDecryptHelper

    {
        private readonly RijndaelManaged _rij;

        public PortfolioDecryptHelper(string secretKey)
        {
            _rij = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                FeedbackSize = 128,
                Key = Encoding.UTF8.GetBytes(secretKey.Substring(0, 16)),
                IV = Encoding.UTF8.GetBytes(secretKey.Substring(0, 16))
            };
        }

        /// <summary>
        /// function to decrypt a string with an encryption Key
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Check arguments.  
                if (cipherBytes == null || cipherBytes.Length <= 0)
                {
                    throw new ArgumentNullException("cipherBytes");
                }

                // Declare the string used to hold  
                // the decrypted text.  
                string plaintext = null;

                // Create the streams used for decryption.  
                var decryptor = _rij.CreateDecryptor(_rij.Key, _rij.IV);

                // Create the streams used for decryption.  
                using (var msDecrypt = new MemoryStream(cipherBytes))
                {
                    using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

                    using var srDecrypt = new StreamReader(csDecrypt);
                    // Read the decrypted bytes from the decrypting stream  
                    // and place them in a string.  
                    plaintext = srDecrypt.ReadToEnd();
                }

                return plaintext;
            }
            catch (Exception e)
            {
                Log.Error(e, "Decrypt Error while retreiving JWT claims value, details:  {Message}", e.Message);
                throw;
            }
        }
    }

}
