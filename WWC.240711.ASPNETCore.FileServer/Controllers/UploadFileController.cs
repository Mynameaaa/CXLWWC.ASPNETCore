using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.FileServer.Model;
using WWC._240711.ASPNETCore.FileServer.Services;

namespace WWC._240711.ASPNETCore.FileServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IUploadFileService _uploadFileService;

        public UploadFileController(IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<BaseOperateResult> UploadFiles([FromForm] string folderName, [FromForm] List<IFormFile> files)
        {
            return await _uploadFileService.UploadFiles(folderName, files);
        }
    }

}
