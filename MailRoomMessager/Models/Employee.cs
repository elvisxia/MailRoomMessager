using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Models
{
    public class Employee
    {
        public string Code { get; set; }
        public string OpenId { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Building { get; set; }
        public int Floor { get; set; }
        public string Seat { get; set; }
    }
}