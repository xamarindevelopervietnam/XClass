﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
using XTemplateV2.iOS.SourceCode.DependencyServices;
using XTemplateV2.SourceCode.DependencyServices;

[assembly: Dependency(typeof(XEncryptionService))]
namespace XTemplateV2.iOS.SourceCode.DependencyServices
{
    public class XEncryptionService : IXEncryptionService
    {
        private const string EncryptionSalt = "YourEncryptionSalt";
        private const string EncryptionPassword = "YourEncryptionPassword";

        public string Encrypt(string plainValue)
        {
            var salt = Encoding.ASCII.GetBytes(EncryptionSalt);
            var key = new Rfc2898DeriveBytes(EncryptionPassword, salt);
            
            var algorithm = new RijndaelManaged();
            var bytesForKey = algorithm.KeySize / 8;
            var bytesForIv = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIv);

            byte[] encryptedBytes;
            using (var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                var bytesToEncrypt = Encoding.UTF8.GetBytes(plainValue);
                encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            var memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            return memory.ToArray();
        }

        public string Decrypt(string encryptedValue)
        {
            var salt = Encoding.ASCII.GetBytes(EncryptionSalt);
            var key = new Rfc2898DeriveBytes(EncryptionPassword, salt);
            
            var algorithm = new RijndaelManaged();
            var bytesForKey = algorithm.KeySize / 8;
            var bytesForIv = algorithm.BlockSize / 8;
            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIv);

            byte[] descryptedBytes;
            using (var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                var encryptedBytes = Convert.FromBase64String(encryptedValue);
                descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            }

            return Encoding.UTF8.GetString(descryptedBytes);
        }
    }
}
