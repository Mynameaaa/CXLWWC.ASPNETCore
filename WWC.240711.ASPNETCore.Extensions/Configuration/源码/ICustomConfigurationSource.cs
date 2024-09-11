using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.源码
{

    public interface ICustomConfigurationSource
    {

        ICustomConfigurationProvider Build(ICustomConfigurationBuilder builder);
    }
}
