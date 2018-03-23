using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Messages
{
    public class FetchMessage
    {
        public string OpenId { get; set; }
        public string TemplateId { get; set; }
        public string First { get; set; }
        public string Operator { get; set; }
        public string OrderId { get; set; }
    }
}