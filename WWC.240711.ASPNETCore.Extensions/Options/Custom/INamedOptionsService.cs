namespace WWC._240711.ASPNETCore.Extensions
{
    public interface INamedOptionsService
    {

        /// <summary>
        /// 获取 HttpClient 配置
        /// </summary>
        /// <returns></returns>
        Task<List<NamedHttpClientOptions>?> GetHttpClientOptions();

    }
}
