using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;
using System.IO;

namespace Encryption_algo
{
    class Encrypt
    {
        #region Encrypt File
        public static void EncryptFile(string input, string output, string password, string salt)
        {
            string file = input;
            string pw = password;

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes, salt);

            string fileEncrypted = output;

            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
        }
        #endregion

        #region Decrypt File
        public static void DecryptFile(string input, string output, string password, string salt)
        {
            string fileEncrypted = input;
            string pw = password;

            byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pw);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes, salt);

            string fileDecrypted = output;

            File.WriteAllBytes(fileDecrypted, bytesDecrypted);
        }
        #endregion

        #region Core Encryption Algorithm
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, string salt)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, string salt)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        #endregion

        #region Old Encryption Algorithm
        /*public static void EncryptFile(string inputfile, string outputfile, int key)
        {

            var file = new FileInfo(inputfile);
            byte[] Bytes;


            var stream = file.OpenRead();
            

            Bytes = new byte[stream.Length];
            stream.Read(Bytes, 0, (int)stream.Length);
            stream.Close();


            for (int i = 0; i < Bytes.Length; i++)
            {
                Bytes[i] = (byte)(Bytes[i] + key);
            }

            File.WriteAllBytes(outputfile, Bytes);
        }

        public static void DecryptFile(string input, string output, int key)
        {

            byte[] Bytes;
          
                Bytes = File.ReadAllBytes(input);

                for (int i = 0; i < Bytes.Length; i++)
                {
                Bytes[i] = (byte)(Bytes[i] - key);
                }

                File.WriteAllBytes(output, Bytes);
            
        }*/
        #endregion
    }
}
