using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWC._240711.ASPNETCore.Extensions.Logging.Custom.Db
{

    public class CustomLogEntry
    {
        [Key]
        public long LogID { get; set; }

        public string? Message { get; set; }
        public DateTime Time { get; set; }
        public int? EventId { get; set; }

        public string? EventName { get; set; }
        public string? Data { get; set; }
    }
}
