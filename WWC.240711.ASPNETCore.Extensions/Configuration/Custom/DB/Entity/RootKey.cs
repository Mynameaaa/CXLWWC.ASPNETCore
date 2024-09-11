using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.Entity
{
    public class RootKey
    {

        [Key]
        public Guid ConfigKey { get; set; }

    }
}
