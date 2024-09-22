using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.ApplicationBuilder.Custom.App;

namespace WWC._240711.ASPNETCore.Extensions
{
    public interface ICustomServer
    {
        Task StartAsync(CustomRequestDelegate handler);
    }
}
