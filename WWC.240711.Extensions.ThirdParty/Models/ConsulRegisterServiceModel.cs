using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.Extensions.ThirdParty;

public class ConsulRegisterServiceModel
{

    /// <summary>
    /// 服务编号
    /// </summary>
    public string ServiceID { get; set; }

    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; }

    /// <summary>
    /// 结束时是否释放
    /// </summary>
    public bool IsDisposed { get; set; }

    /// <summary>
    /// 健康检查地址
    /// </summary>
    public string HealthRoute { get; set; }

    /// <summary>
    /// 健康检查时间间隔
    /// </summary>
    public TimeSpan HealthTimeSpan { get; set; }

    /// <summary>
    /// 服务主机
    /// </summary>
    public string ServiceHost { get; set; }

    /// <summary>
    /// 服务地址
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 服务标签
    /// </summary>
    public string[] Tags { get; set; }

}
