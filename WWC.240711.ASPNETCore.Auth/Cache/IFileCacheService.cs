namespace WWC._240711.ASPNETCore.Auth.Cache;

public interface IFileCacheService
{

    /// <summary>
    /// 缓存文件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    bool CacheFile(string key, byte[] content);

    /// <summary>
    /// 获取文件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    byte[] GetFile(string key);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool RemoveFile(string key);

    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool HasKey(string key);

}
