using Dapper;
using MailRoomMessager.Helper;
using MailRoomMessager.Messages;
using MailRoomMessager.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MailRoomMessager.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrderController : ApiController
    {
        [Route("wx")]
        [HttpPost]
        public HttpResponseMessage SavePacket([FromBody]Order order)
        {
            order.InputTime = DateTime.UtcNow;
            string openid = "";
            string sql = @"insert Orders(OrderId,Operator,Name,Call,InputTime) 
                                  values(@OrderId,@Operator,@Name,@Call,@InputTime)";
            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    connection.Execute(sql, order);
                    sql = $"select OpenId from Orders left join Contact on Orders.Call = Contact.Call where Orders.Call = '{order.Call}'";
                    openid = connection.QueryFirst<string>(sql);
                }

                if (!string.IsNullOrEmpty(openid))
                {
                    FetchMessage message = new FetchMessage
                    {
                        OpenId = openid,
                        TemplateId = "GicaQGB7iOQWHBVs_0riiEYPSo-LyOMVa1jLIXPWRBA",
                        First = "您的包裹已到",
                        Operator = order.Operator,
                        OrderId = order.OrderId
                    };

                    TemplateMessageHelper.SendAsync(message);
                }                

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        [Route("wx")]
        [HttpGet]
        public HttpResponseMessage DeletePacket([FromUri]string code)
        {
            throw new NotImplementedException("To-Do Item");
        }

        [Route("wx")]
        [HttpPut]
        public HttpResponseMessage UpdatePacket([FromBody]Order order)
        {
            throw new NotImplementedException("To-Do Item");
        }

        [Route("wx")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPacket([FromUri]string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }
            string openid = await OpenIdHelper.GetOpenIdAsync(code);
            //string openid = "o5dhF1EKpj54qs6-GNoYtLPUSUOA";
            if (string.IsNullOrEmpty(openid))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }
            string sql = $"select * from Employee where OpenId='{openid}'";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employees = connection.Query<Employee>(sql);
                    if (employees == null || employees.Count() == 0)
                    {
                        sql = $"insert EmployeeMap(OpenId, Code) values('{openid}', '{code}')";
                        connection.Execute(sql);
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotBound);
                    }

                    sql = $"select Orders.* from Orders left join Contact on Orders.Call =Contact.Call where Contact.openid= '{openid}' order by Orders.InputTime desc";
                    var orders = connection.Query<Order>(sql);
                    return Request.CreateResponse(HttpStatusCode.OK, orders);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        #region port

        #region Query Orders
        [Route("port")]
        [HttpGet]
        public async Task<JArray> QueryPackets()
        {
            //1. Get the input date range;
            string fromDate = HttpContext.Current.Request["from"];
            string toDate = HttpContext.Current.Request["to"];
            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.ToString();
            }
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Parse(toDate).AddDays(-7.0).ToString();
            }

            string sql = string.Format(
                @"select
                OrderId,
                e.Name,
                o.Call,
                Status,
                InputTime,
                Operator,
                c.Alias,
                e.Site,
                e.Building,
                e.Floor,
                e.Seat
                from dbo.Orders as o
                inner join dbo.Contact as c
                on o.Call = c.Call
                inner join dbo.Employee as e
                on c.empid = e.SysId where inputTime between '{0}' and '{1}'",fromDate,toDate);

            IList<object> list = DBHelper.QueryList<object>(sql);
            JArray data = JArray.Parse(JsonConvert.SerializeObject(list).ToLower());
            return data;
        }
        #endregion

        


        #endregion
    }
}
