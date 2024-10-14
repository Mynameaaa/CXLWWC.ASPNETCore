namespace WWC._240711.ASPNETCore.FileServer.Model
{
    public class BaseOperateResult
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public int Code { get; set; } = 400;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = false;

    }
}
