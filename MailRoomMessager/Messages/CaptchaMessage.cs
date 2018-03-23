using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Messages
{
    public class CaptchaMessage
    {
        public string Alias { get; set; }
        public string Captcha { get; set; }
    }
}