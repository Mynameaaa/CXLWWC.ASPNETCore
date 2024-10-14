using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using WWC._240711.ASPNETCore.Auth;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.FileServer.Services
{
    public class DownLoadFileService : IDownLoadFileService
    {

        /// <summary>
        /// 获取 Token 密钥
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public List<(Stream FileStream, string FileName)> DownloadTokenFile()
        {
            try
            {
                var files = new List<(Stream, string)>();

                string privateKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PrivateKeyPath") ?? "Keys/private.pem");
                string publicKeyPath = Path.Combine(Directory.GetCurrentDirectory(), Appsettings.app("TokenKey:PublicKeyPath") ?? "Keys/public.pem");

                string privateKey = Path.Combine(Directory.GetCurrentDirectory(), privateKeyPath);
                string publicKey = Path.Combine(Directory.GetCurrentDirectory(), publicKeyPath);

                if (!System.IO.File.Exists(privateKey) || !System.IO.File.Exists(publicKey))
                {
                    KeyHelper keys = new KeyHelper();
                    keys.GenerateKeys(privateKeyPath, publicKeyPath);
                }

                if (File.Exists(privateKeyPath))
                {
                    files.Add((new FileStream(privateKeyPath, FileMode.Open, FileAccess.Read), "privateKey.pem"));
                }

                if (File.Exists(publicKeyPath))
                {
                    files.Add((new FileStream(publicKeyPath, FileMode.Open, FileAccess.Read), "publicKey.pem"));
                }

                return files;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
