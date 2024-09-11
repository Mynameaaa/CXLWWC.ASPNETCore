using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWC._240711.ASPNETCore.Extensions.Configuration.Custom.ConfigurationProvider;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom.ConfigurationSource
{
    public class CXLWebconfigConfigurationSource : IConfigurationSource
    {
        private readonly string _filePath;

        public CXLWebconfigConfigurationSource(string filePath)
        {
            _filePath = filePath;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CXLWebconfigConfigurationProvider(_filePath);
        }
    }
}
