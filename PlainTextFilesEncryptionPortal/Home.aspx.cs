using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO.Compression;
using System.ServiceModel;

namespace PlainTextFilesEncryptionPortal
{
    public partial class Home : System.Web.UI.Page
    {
        // Reference for the Host
        FileServiceReference.FileServiceClient client = new FileServiceReference.FileServiceClient("BasicHttpBinding_IFileService");

        private static string Key = "dofkrfaosrdedofkrfaosrdedofkrfao";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_upload_Click(object sender, EventArgs e)
        {
            if (FileUpload.HasFile)
            {
                byte[] file = new byte[FileUpload.PostedFile.ContentLength];
                FileUpload.PostedFile.InputStream.Read(file, 0, FileUpload.PostedFile.ContentLength);
                // the File Name
                string fileName = FileUpload.PostedFile.FileName;


                // Digitally sign the file using RSA algorithm, the key must be taken from a certificate hosted in windows store
                byte[] signedFile = Sign(file);

                // Hash the File (MD5 algo.)
                string hashedFile = CalculateMD5(file);

                // Compress the file (GZip algo.)
                byte[] compressedFile = compress(file);

                // Encrypt By Hybird technique

                /*
                 * I have a one miss her
                 * 
                 * The miss is encrypt the key by RSA and then sent it
                 * 
                 * But I fail after to day search
                 * 
                 */

                // Encrypt the Compress File by (AES algo.)
                byte[] AESEncryptFile = AESEncrypt(compressedFile);


                // Send the file to WCF Service using http
                client.SendData(fileName, AESEncryptFile, hashedFile, signedFile, Key);

            }
        }

        protected byte[] compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }
        protected string CalculateMD5(byte[] file)
        {
            byte[] byteHashed;
            using (MD5 md5 = MD5.Create())
            {
                byteHashed = md5.ComputeHash(file);
            }
            return BitConverter.ToString(byteHashed).Replace("-", "").ToLowerInvariant();         
         }

        protected byte[] AESEncrypt(byte[] data)
        {
            string IV = "zxcvbnmdfrasdfgh";
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(Key);
            aes.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV);
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            ICryptoTransform crypto = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encrypted = crypto.TransformFinalBlock(data, 0, data.Length);
            crypto.Dispose();
            return encrypted;
            //return Convert.ToBase64String(encrypted);
        }
 
        protected byte[] Sign(byte[] data)
        {
            string certificateSubject = "E=support@bec.net, CN=www.BEC.net, OU=Software Enginnering, O=BEC.net, L=Cairo, S=DC, C=EG";

            // Access Personal (MY) certificate store of current user
            X509Store my = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            my.Open(OpenFlags.ReadOnly);

            // Find the certificate we'll use to sign
            RSACryptoServiceProvider csp = null;

            foreach (X509Certificate2 cert in my.Certificates)
            {

                if (cert.Subject.Contains(certificateSubject))
                {
                    // We found it.
                    // Get its associated CSP and private key
                    csp = (RSACryptoServiceProvider)cert.PrivateKey;
                }
            }
            my.Close();

            if (csp == null)
            {

                throw new Exception("No valid cert was found");

            }


            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();
 
            //UnicodeEncoding encoding = new UnicodeEncoding();
            //byte[] data = encoding.GetBytes(text);

            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            return csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }
    }
}