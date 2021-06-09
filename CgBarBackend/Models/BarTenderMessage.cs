using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CgBarBackend.Models
{
    public class BarTenderMessage
    {
        public string Event { get; set; }
        public string Template { get; set; }
        public string Target { get; set; }

        public BarTenderMessage(){}

        public BarTenderMessage(string eventType, string target, string template)
        {
            Event = eventType;
            Target = target;
            Template = template;
        }
    }
}
