using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Auth;

public interface IKeyHelper
{
    /// <summary>
    /// 生成公私钥对并保存到文件
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="publicKeyPath"></param>
    void GenerateKeys(string privateKeyPath, string publicKeyPath);

    /// <summary>
    /// 加载公钥（SubjectPublicKeyInfo 格式）
    /// </summary>
    /// <param name="publicKeyPath"></param>
    /// <returns></returns>
    RSA LoadPublicKeyFromPEM(string publicKeyPath);

    /// <summary>
    /// 加载私钥（PKCS#8 格式）
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <returns></returns>
    Task<RSA> LoadPrivateKeyFromPEM(string privateKeyPath);


    /// <summary>
    /// 加载私钥（PKCS#8 格式）
    /// </summary>
    /// <param name="privateKeyValue"></param>
    /// <returns></returns>
    RSA LoadPrivateKeyFromPEM(byte[] privateKeyValue);

    /// <summary>
    /// 加载公钥（PKCS#8 格式）
    /// </summary>
    /// <param name="publicKeyValue"></param>
    /// <returns></returns>
    RSA LoadPublicKeyFromPEM(byte[] publicKeyValue);

}
