using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using WWC._240711.ASPNETCore.FileServer.Services;
using WWC._240711.ASPNETCore.Infrastructure;

namespace WWC._240711.ASPNETCore.FileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownLoadFileController : ControllerBase
    {
        private readonly IDownLoadFileService _downLoadFileService;

        public DownLoadFileController(IDownLoadFileService downLoadFileService)
        {
            _downLoadFileService = downLoadFileService;
        }

        /// <summary>
        /// 下载公私钥信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("downloadTokenKey")]
        public async Task<IActionResult> downloadTokenKey()
        {
            try
            {
                var pemFiles = _downLoadFileService.DownloadTokenFile();

                if (pemFiles.Count == 0)
                {
                    return NotFound("不包含任何 Token 文件");
                }

                var boundary = $"----WebKitFormBoundary{Guid.NewGuid().ToString("N")}";
                var multipartContent = new MultipartContent("mixed", boundary);

                foreach (var (fileStream, fileName) in pemFiles)
                {
                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-pem-file");
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    multipartContent.Add(streamContent);
                }

                // 将 multipart 内容转换为字节数组
                var memoryStream = new MemoryStream();
                await multipartContent.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // 重置流位置

                return new FileStreamResult(memoryStream, $"multipart/form-data; boundary={boundary}")
                {
                    FileDownloadName = "pem_files.zip" // 可根据需要修改文件名
                };
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误 {ex.Message}");
            }
        }

    }
}
