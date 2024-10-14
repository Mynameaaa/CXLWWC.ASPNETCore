using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;

namespace WWC._240711.ASPNETCore.AuthPlatform.Servcies
{
    public class DownLoadFileService : IDownLoadFileService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DownLoadFileService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 读取 TokenKey 文件
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<byte[]>> GetTokenKeyFile()
        {
            List<byte[]> result = new List<byte[]>();
            var _httpClient = _httpClientFactory.CreateClient("FileServer");
            string url = "api/DownLoadFile/downloadTokenKey";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var boundary = HeaderUtilities.RemoveQuotes(
                    response.Content.Headers.ContentType.Parameters
                        .FirstOrDefault(p => p.Name == "boundary")?.Value
                ).Value;

                if (boundary == null)
                {
                    throw new Exception("Invalid multipart response.");
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var reader = new MultipartReader(boundary, stream);

                MultipartSection section;
                while ((section = await reader.ReadNextSectionAsync()) != null)
                {
                    var contentDisposition = section.ContentDisposition;

                    if (contentDisposition != null)
                    {
                        var contentDispositionHeader = ContentDispositionHeaderValue.Parse(contentDisposition);

                        // 检查是否是文件部分，并提取文件名
                        if (contentDispositionHeader.DispositionType.Equals("attachment") &&
                            !string.IsNullOrEmpty(contentDispositionHeader.FileName.Value))
                        {
                            var fileName = contentDispositionHeader.FileName.Value.Trim('"'); // 去除引号

                            // 读取文件内容到内存中
                            using (var memoryStream = new MemoryStream())
                            {
                                await section.Body.CopyToAsync(memoryStream);
                                var fileContent = memoryStream.ToArray(); // 将内容读为 byte[]
                                result.Add(fileContent);    // 添加到 List 中
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception($"Error downloading files: {response.StatusCode}");
            }

            return result;
        }
    }
}
