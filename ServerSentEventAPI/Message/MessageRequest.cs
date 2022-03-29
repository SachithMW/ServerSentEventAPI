using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerSentEventAPI.Message
{
    public class MessageRequest
    {
        public string cif { get; set; }
        public string Message { get; set; }
    }
}
