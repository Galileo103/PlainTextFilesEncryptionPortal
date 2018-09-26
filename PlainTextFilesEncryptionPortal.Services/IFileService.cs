using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PlainTextFilesEncryptionPortal.Services
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        void SendData(string fileName, byte[] data, string hash, byte[] sign, string key);

        byte[] AESDecrypt(byte[] encrypted, string key);

        byte[] decompress(byte[] raw);

        string CalculateMD5(byte[] file);

        bool Verify(byte[] data, byte[] signature);
    }

}
