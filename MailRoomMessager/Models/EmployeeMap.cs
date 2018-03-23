using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Models
{
    public class EmployeeMap
    {
        public string OpenId { get; set; }
        public string Alias { get; set; }
        public string Code { get; set; }
        public string Captcha { get; set; }
        public DateTime CaptchaExpiresTime { get; set; }
    }
}