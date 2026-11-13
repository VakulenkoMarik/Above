using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Encrypting
{
    public static class CryptoHelper
    {
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private const int DerivationIterations = 1000;
        
        public static string GenerateKeyFromUid(string uid) {
            byte[] uidBytes = Encoding.UTF8.GetBytes(uid);
            using HMACSHA256 hmac = new HMACSHA256(uidBytes);
            return Convert.ToBase64String(hmac.ComputeHash(uidBytes));
        }

        public static void Encrypt(object jsonData, string password) {
            string filePath = Path.Combine(Application.persistentDataPath, "data.json");
            string jsonString = JsonUtility.ToJson(jsonData, true);
            EncryptToFile(filePath, jsonString, password);
        }

        public static T LoadAndDecrypt<T>(string filePath, string password) {
            string decryptedJson = DecryptFromFile(filePath, password);
            return JsonUtility.FromJson<T>(decryptedJson);
        }
        
        private static void EncryptToFile(string filePath, string json, string password) {
            using Aes aes = CreateAesWithPassword(password, out byte[] iv);
            byte[] encryptedData = EncryptStringToBytes(json, aes.Key, iv);
            WriteEncryptedFile(filePath, iv, encryptedData);
        }

        private static string DecryptFromFile(string filePath, string password) {
            try {
                using FileStream fileStream = File.OpenRead(filePath);
                byte[] iv = ReadIvFromFile(fileStream);

                using Aes aes = CreateAesWithPassword(password, iv);
                byte[] decryptedData = DecryptStreamToBytes(fileStream, aes.Key, aes.IV);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch {
                return null;
            }
        }
        
        private static Aes CreateAesWithPassword(string password, out byte[] iv) {
            Aes aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Key = DeriveKey(password);
            aes.GenerateIV();
            iv = aes.IV;
            return aes;
        }

        private static Aes CreateAesWithPassword(string password, byte[] iv) {
            Aes aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Key = DeriveKey(password);
            aes.IV = iv;
            return aes;
        }

        private static byte[] DeriveKey(string password) {
            using var keyDerivation = new Rfc2898DeriveBytes(password, GenerateSalt(password), DerivationIterations);
            return keyDerivation.GetBytes(KeySize / 8);
        }
        
        private static void WriteEncryptedFile(string filePath, byte[] iv, byte[] encryptedData) {
            using FileStream fileStream = File.Create(filePath);
            fileStream.Write(iv, 0, iv.Length);
            fileStream.Write(encryptedData, 0, encryptedData.Length);
        }

        private static byte[] ReadIvFromFile(FileStream fileStream) {
            byte[] iv = new byte[BlockSize / 8];
            fileStream.Read(iv, 0, iv.Length);
            return iv;
        }
        
        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv) {
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = KeySize;
            aesAlg.BlockSize = BlockSize;
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using MemoryStream msEncrypt = new MemoryStream();
            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                swEncrypt.Write(plainText);
            }
            
            return msEncrypt.ToArray();
        }

        private static byte[] DecryptStreamToBytes(Stream encryptedStream, byte[] key, byte[] iv) {
            using Aes aesAlg = Aes.Create();
            aesAlg.KeySize = KeySize;
            aesAlg.BlockSize = BlockSize;
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using MemoryStream msDecrypt = new MemoryStream();
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Write)) {
                encryptedStream.CopyTo(csDecrypt);
            }
            
            return msDecrypt.ToArray();
        }
        
        private static byte[] GenerateSalt(string pass) {
            if (pass == null) {
                throw new ArgumentNullException(nameof(pass));
            }

            byte[] salt = Encoding.UTF8.GetBytes(pass);
            using HMACSHA256 hmac = new HMACSHA256(salt);
            
            return hmac.ComputeHash(salt);
        }
    }
}
