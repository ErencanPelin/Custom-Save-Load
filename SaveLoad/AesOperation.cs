using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CustomSaveLoad.Utils
{
    /// <summary>
    /// Handles encryption and decryption
    /// </summary>
    internal static class AesOperation
    {
        /// <summary>
        /// Encrypts the given text using the private key using Symmetric encryption.
        /// The same private key used to encrypt the text MUST be used to decrypt the text
        /// </summary>
        /// <param name="key">private key</param>
        /// <param name="plainText">text to encrypt</param>
        /// <returns>Given plain text in AES symmetric encryption format</returns>
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Decrypts the given cipher text using the provided private key. You must use
        /// the same key used to encrypt the text in the first place
        /// </summary>
        /// <param name="key">private key</param>
        /// <param name="cipherText">cipher text to be decrypted</param>
        /// <returns>Decrypted plain text of the given cipher</returns>
        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream =
                           new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}