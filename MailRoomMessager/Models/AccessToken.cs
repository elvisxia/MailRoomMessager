using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Models
{
    public class AccessToken
    {
        public string Token { get; set; }
        public string Ticket { get; set; }
        public DateTime RefreshTime { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}