using Microsoft.AspNetCore.Mvc;
using WWC._240711.ASPNETCore.FileServer.Model;

namespace WWC._240711.ASPNETCore.FileServer.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IWebHostEnvironment _env;

        public UploadFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<BaseOperateResult> UploadFiles(string folderName, List<IFormFile> files)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                return new BaseOperateResult
                {
                    Message = "文件夹名称是必须的"
                };
            }

            if (files == null || files.Count == 0)
            {
                return new BaseOperateResult
                {
                    Message = "至少需要一个文件"
                };
            }

            // 使用WebRootPath获取项目的根目录
            var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads", folderName);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uploadResults = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        // 确保文件名安全
                        var fileName = Path.GetFileName(file.FileName);

                        // 目标文件路径
                        var filePath = Path.Combine(uploadPath, fileName);

                        // 使用异步流的方式将文件保存到磁盘
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        uploadResults.Add($"File {fileName} uploaded successfully to {filePath}");
                    }
                }


                return new BaseOperateResult
                {
                    Code = 200,
                    Message = "文上传成功",
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                // 捕获任何错误，返回错误信息
                return new BaseOperateResult
                {
                    Code = 500,
                    Message = "文件上传失败"
                };
            }
        }
    }
}
