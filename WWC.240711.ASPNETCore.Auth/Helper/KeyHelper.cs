using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WWC._240711.ASPNETCore.Auth;

public class KeyHelper : IKeyHelper
{

    /// <summary>
    /// 生成公私钥对并保存到文件
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <param name="publicKeyPath"></param>
    public void GenerateKeys(string privateKeyPath, string publicKeyPath)
    {
        // 检查并创建私钥文件夹
        string privateKeyDirectory = Path.GetDirectoryName(privateKeyPath);
        if (!Directory.Exists(privateKeyDirectory))
        {
            Directory.CreateDirectory(privateKeyDirectory);
        }

        // 检查并创建公钥文件夹
        string publicKeyDirectory = Path.GetDirectoryName(publicKeyPath);
        if (!Directory.Exists(publicKeyDirectory))
        {
            Directory.CreateDirectory(publicKeyDirectory);
        }

        using (RSA rsa = RSA.Create(2048))  // 创建 RSA 密钥
        {
            // 导出私钥（PKCS#8 格式）和公钥（SubjectPublicKeyInfo 格式）
            var privateKey = rsa.ExportPkcs8PrivateKey();
            var publicKey = rsa.ExportSubjectPublicKeyInfo();

            bool isGenerate = false;
            if (!File.Exists(privateKeyPath) || !File.Exists(publicKeyPath))
            {
                isGenerate = true;
                // 将私钥转换为 PEM 格式并写入文件
                File.WriteAllText(privateKeyPath, ExportPrivateKeyToPEM(privateKey));
            }

            if (isGenerate)
            {
                // 将公钥转换为 PEM 格式并写入文件
                File.WriteAllText(publicKeyPath, ExportPublicKeyToPEM(publicKey));
            }
        }
    }

    /// <summary>
    /// 将私钥转换为 PEM 格式（PKCS#8）
    /// </summary>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    private string ExportPrivateKeyToPEM(byte[] privateKey)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("-----BEGIN PRIVATE KEY-----");
        builder.AppendLine(Convert.ToBase64String(privateKey, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END PRIVATE KEY-----");
        return builder.ToString();
    }

    // 将公钥转换为 PEM 格式（SubjectPublicKeyInfo）
    private string ExportPublicKeyToPEM(byte[] publicKey)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("-----BEGIN PUBLIC KEY-----");
        builder.AppendLine(Convert.ToBase64String(publicKey, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END PUBLIC KEY-----");
        return builder.ToString();
    }

    /// <summary>
    /// 加载公钥（SubjectPublicKeyInfo 格式）
    /// </summary>
    /// <param name="publicKeyPath"></param>
    /// <returns></returns>
    public RSA LoadPublicKeyFromPEM(string publicKeyPath)
    {
        // 读取 PEM 文件内容
        string publicKeyPEM = File.ReadAllText(publicKeyPath);

        // 使用正则表达式删除 "-----BEGIN PUBLIC KEY-----" 和 "-----END PUBLIC KEY-----" 部分
        string base64Key = Regex.Replace(publicKeyPEM, "-----.*?-----", string.Empty).Trim();

        // 将 Base64 字符串转换为字节数组
        byte[] publicKeyBytes = Convert.FromBase64String(base64Key);

        // 创建 RSA 实例并导入公钥（SubjectPublicKeyInfo 格式）
        RSA rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return rsa;
    }

    /// <summary>
    /// 加载私钥（PKCS#8 格式）
    /// </summary>
    /// <param name="privateKeyPath"></param>
    /// <returns></returns>
    public async Task<RSA> LoadPrivateKeyFromPEM(string privateKeyPath)
    {
        // 读取 PEM 文件内容
        string privateKeyPEM = await File.ReadAllTextAsync(privateKeyPath);

        // 使用正则表达式删除 "-----BEGIN PRIVATE KEY-----" 和 "-----END PRIVATE KEY-----" 部分
        string base64Key = Regex.Replace(privateKeyPEM, "-----.*?-----", string.Empty).Trim();

        // 将 Base64 字符串转换为字节数组
        byte[] privateKeyBytes = Convert.FromBase64String(base64Key);

        // 创建 RSA 实例并导入私钥（PKCS#8 格式）
        RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        return rsa;
    }

    /// <summary>
    /// 加载私钥（PKCS#8 格式）
    /// </summary>
    /// <param name="privateKeyValue"></param>
    /// <returns></returns>
    public RSA LoadPrivateKeyFromPEM(byte[] privateKeyValue)
    {
        // 读取 PEM 文件内容
        string privateKeyPEM = Encoding.UTF8.GetString(privateKeyValue);

        // 使用正则表达式删除 "-----BEGIN PRIVATE KEY-----" 和 "-----END PRIVATE KEY-----" 部分
        string base64Key = Regex.Replace(privateKeyPEM, "-----.*?-----", string.Empty).Trim();

        // 将 Base64 字符串转换为字节数组
        byte[] privateKeyBytes = Convert.FromBase64String(base64Key);

        // 创建 RSA 实例并导入私钥（PKCS#8 格式）
        RSA rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        return rsa;
    }

    /// <summary>
    /// 加载公钥（PKCS#8 格式）
    /// </summary>
    /// <param name="publicKeyValue"></param>
    /// <returns></returns>
    public RSA LoadPublicKeyFromPEM(byte[] publicKeyValue)
    {
        // 读取 PEM 文件内容
        string publicKeyPEM = Encoding.UTF8.GetString(publicKeyValue);

        // 使用正则表达式删除 "-----BEGIN PUBLIC KEY-----" 和 "-----END PUBLIC KEY-----" 部分
        string base64Key = Regex.Replace(publicKeyPEM, "-----.*?-----", string.Empty).Trim();

        // 将 Base64 字符串转换为字节数组
        byte[] publicKeyBytes = Convert.FromBase64String(base64Key);

        // 创建 RSA 实例并导入公钥（SubjectPublicKeyInfo 格式）
        RSA rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return rsa;
    }
}
