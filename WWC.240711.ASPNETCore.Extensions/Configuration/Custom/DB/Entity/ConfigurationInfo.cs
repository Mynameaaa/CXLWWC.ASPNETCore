using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Configuration.Custom.DB.Entity
{
    public class ConfigurationInfo : RootKey
    {
        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(30)]
        public string Key { get; set; }

        [Required]
        [Column(TypeName = "varchar")]
        [MaxLength(3000)]
        public string Value { get; set; }

        //[Required]
        public string? ParentID { get; set; }

    }
}
