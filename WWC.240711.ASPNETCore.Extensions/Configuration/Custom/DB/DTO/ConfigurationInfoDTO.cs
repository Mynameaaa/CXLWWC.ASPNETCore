using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.DTO
{
    public class ConfigurationInfoDTO
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string ParentID { get; set; }

        public object ConfigurationInfo { get; set; } = null;
    }
}
