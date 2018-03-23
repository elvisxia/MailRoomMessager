using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailRoomMessager.Models
{
    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public static Error Auth = new Error { ErrorCode = "40001", ErrorMessage = "Authorize failed." };
        public static Error LackInfo = new Error { ErrorCode = "40002", ErrorMessage = "Not enough information provided." };
        public static Error NotBound = new Error { ErrorCode = "40003", ErrorMessage = "Action need: Bind." };
        public static Error NotFound = new Error { ErrorCode = "40004", ErrorMessage = "No record found." };
        public static Error Server = new Error { ErrorCode = "40005", ErrorMessage = "Internal Server Error." };
    }
}