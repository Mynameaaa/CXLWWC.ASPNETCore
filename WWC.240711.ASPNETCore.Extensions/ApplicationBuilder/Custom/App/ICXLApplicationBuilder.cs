using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Http.Custom.App;

namespace WWC._240711.ASPNETCore.Extensions.ApplicationBuilder.Custom.App
{
    public delegate Task CustomRequestDelegate(CustomHttpContext context);

    public interface ICXLApplicationBuilder
    {
        CustomRequestDelegate Build();
        ICXLApplicationBuilder Use(Func<CustomRequestDelegate, CustomRequestDelegate> middleware);

    }
}
