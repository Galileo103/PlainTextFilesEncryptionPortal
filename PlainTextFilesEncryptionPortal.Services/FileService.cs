using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace PlainTextFilesEncryptionPortal.Services
{
    public class FileService : IFileService
    {

        public void SendData(string fileName, byte[] encryptCompressData, string hash, byte[] sign, string key)
        {

            byte[] AESDecryptFile = AESDecrypt(encryptCompressData, key);

            byte[] decompressedFile = decompress(AESDecryptFile);

            string hashedCompressedFile = CalculateMD5(decompressedFile);

            if(hashedCompressedFile.Equals(hash))
            {
                if (Verify(decompressedFile, sign))
                {
                    string finalPath = Path.Combine(@"C:\Users\Galileo\Documents\Visual Studio 2013\Projects\PlainTextFilesEncryptionPortal\PlainTextFilesEncryptionPortal\UploadedFiles", fileName);

                    File.WriteAllBytes(finalPath, decompressedFile);
                }
            }
        }


        public byte[] AESDecrypt(byte[] encrypted, string Key)
        {
            string IV = "zxcvbnmdfrasdfgh";
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
            aes.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] data = crypto.TransformFinalBlock(encrypted, 0, encrypted.Length);
            crypto.Dispose();
            return data;
            //return System.Text.ASCIIEncoding.ASCII.GetString(secret);
        }

        public byte[] decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public string CalculateMD5(byte[] file)
        {
            byte[] byteHashed;
            using (MD5 md5 = MD5.Create())
            {
                byteHashed = md5.ComputeHash(file);
            }
            return BitConverter.ToString(byteHashed).Replace("-", "").ToLowerInvariant();
        }

        public bool Verify(byte[] data, byte[] signature)
        {
            string certificatePath = @"C:\Users\Galileo\Documents\Visual Studio 2013\Projects\PlainTextFilesEncryptionPortal\PlainTextFilesEncryptionPortal\keys\task-ebc-public.pem";
            // Load the certificate we'll use to verify the signature from a file
            X509Certificate2 cert = new X509Certificate2(certificatePath);

            // Note:
            // If we want to use the client cert in an ASP.NET app, we may use something like this instead:
            // X509Certificate2 cert = new X509Certificate2(Request.ClientCertificate.Certificate);

            // Get its associated CSP and public key
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;

            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();

            //UnicodeEncoding encoding = new UnicodeEncoding();
            //byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);

            // Verify the signature with the hash
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);
        }


    }
}
