using WWC._240711.ASPNETCore.FileServer.Model;

namespace WWC._240711.ASPNETCore.FileServer.Services
{
    public interface IUploadFileService
    {
        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        Task<BaseOperateResult> UploadFiles(string folderName, List<IFormFile> files);

    }
}
