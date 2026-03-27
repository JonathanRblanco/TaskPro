using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPro.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Logger { get; set; }
        public string? Url { get; set; }
        public string? Exception { get; set; }
        public string? InnerException { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Method { get; set; }
        public string? User { get; set; }
        public string? Ip { get; set; }
        public string? SourceURL { get; set; }
    }
}
