using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Operator { get; set; }
        public string Name { get; set; }
        public string Call { get; set; }
        public DateTime InputTime { get; set; }
    }
}