using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom
{
    public class DbLoggeroOptions
    {
        public bool UseDefaultFilter { get; set; } = true;

        public string[] CategoryNames { get; set; }

        public LogLevel MinLogLevel { get; set; }

    }
}
