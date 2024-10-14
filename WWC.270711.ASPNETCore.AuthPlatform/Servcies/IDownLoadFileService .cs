namespace WWC._240711.ASPNETCore.AuthPlatform.Servcies
{
    public interface IDownLoadFileService
    {

        /// <summary>
        /// 读取 TokenKey 文件
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<List<byte[]>> GetTokenKeyFile();

    }
}
