namespace WWC._240711.ASPNETCore.FileServer.Services
{
    public interface IDownLoadFileService
    {

        /// <summary>
        /// 获取 Token 密钥
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        List<(Stream FileStream, string FileName)> DownloadTokenFile();

    }
}
