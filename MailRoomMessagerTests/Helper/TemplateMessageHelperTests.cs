using Microsoft.VisualStudio.TestTools.UnitTesting;
using MailRoomMessager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRoomMessager.Messages;

namespace MailRoomMessager.Helper.Tests
{
    [TestClass()]
    public class TemplateMessageHelperTests
    {
        [TestMethod()]
        public void SendAsyncTest()
        {
            FetchMessage message = new FetchMessage
            {
                OpenId = "o5dhF1EKpj54qs6-GNoYtLPUSUOA",
                TemplateId = "gGmlcdqNyZ87QuljWZrggVtPc9RU7eZxrJJ9eomzbs4",
                First = "您的包裹已到",
                Operator = "EMS",
                OrderId = "111"
            };

            TemplateMessageHelper.SendAsync(message).Wait();

            Assert.Fail();
        }
    }
}