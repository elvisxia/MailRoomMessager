using Dapper;
using MailRoomMessager.Helper;
using MailRoomMessager.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    [RoutePrefix("api/tokens")]
    public class TokenController : ApiController
    {
        [Route("wx")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTicket([FromUri]string code)
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

            EmployeeMap employeeMap = new EmployeeMap();
            employeeMap.OpenId = openid;
            employeeMap.Code = code;

            string sql = @"select * from Employee where OpenId = @OpenId";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employeeMaps = connection.Query<EmployeeMap>(sql, employeeMap);
                    if (employeeMaps == null || employeeMaps.Count() <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotFound);
                    }

                    sql = @"select top 1 * from AccessToken order by ExpireTime desc";
                    var tickets = connection.Query<AccessToken>(sql);
                    if (tickets == null || tickets.Count() <= 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotFound);
                    }

                    string noncestr = ConfigurationManager.AppSettings["noncestr"].ToString();
                    string timestamp = ConfigurationManager.AppSettings["timestamp"].ToString();
                    string url = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                    string ticket = tickets.ToList()[0].Ticket;

                    string signature = Sha1Helper.SHA1Encrypt($"jsapi_ticket={ticket}&noncestr={noncestr}&timestamp={timestamp}&url={url}");
                    JObject result = new JObject();
                    result["sig"] = signature;
                    result["openid"] = openid;
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                    //HttpContext.Current.Response.Write(result);
                    //HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }
    }
}
