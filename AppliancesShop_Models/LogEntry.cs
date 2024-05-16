using AppliancesShop_Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliancesShop_Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
