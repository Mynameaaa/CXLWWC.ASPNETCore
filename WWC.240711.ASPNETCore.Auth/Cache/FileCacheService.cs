namespace WWC._240711.ASPNETCore.Auth.Cache;

public class FileCacheService : IFileCacheService
{
    private static Dictionary<string, byte[]> _BytesCacheDic = new Dictionary<string, byte[]>();

    /// <summary>
    /// 缓存文件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public bool CacheFile(string key, byte[] content)
    {
        _BytesCacheDic[key] = content;
        return true;
    }

    /// <summary>
    /// 获取文件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public byte[] GetFile(string key)
    {
        var hasFile = _BytesCacheDic.TryGetValue(key, out var value);
        if (hasFile)
        {
            return value;
        }
        else
        {
            return [];
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool RemoveFile(string key)
    {
        if (!_BytesCacheDic.ContainsKey(key))
            return false;

        return _BytesCacheDic.Remove(key);
    }

    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool HasKey(string key)
    {
        return _BytesCacheDic.ContainsKey(key);
    }

}
