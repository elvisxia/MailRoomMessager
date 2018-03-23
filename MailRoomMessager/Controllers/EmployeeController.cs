using Dapper;
using MailRoomMessager.Helper;
using MailRoomMessager.Models;
using MailRoomMessager.Models.Filters;
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
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [Route("wx")]
        [HttpPut]
        public HttpResponseMessage SaveUsers([FromBody]Employee emp)
        {
            if (emp == null || string.IsNullOrEmpty(emp.OpenId))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            string sql = @"select * from Employee where OpenId = @OpenId";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employees = connection.Query<Employee>(sql, emp);
                    if (employees != null && employees.Count() > 0)
                    {
                        sql = @"update Employee set Name=@Name,Site=@Site,Building=@Building,Floor=@Floor,Seat=@Seat where OpenId=@OpenId";
                        connection.Execute(sql, emp);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        [Route("wx")]
        [HttpDelete]
        public HttpResponseMessage DeleteUsers([FromUri]string openid)
        {
            if (string.IsNullOrEmpty(openid))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            string sql = $"delete from Contact where OpenId = '{openid}'";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    sql = $"delete from Employee where OpenId = '{openid}'";
                    connection.Execute(sql);
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
        public async Task<HttpResponseMessage> GetUsers([FromUri]string code)
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

            string sql = $"select * from Employee where OpenId = '{openid}'";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var employees = connection.Query<Employee>(sql);
                    if (employees != null && employees.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, employees.ToList()[0]);
                    }
                    else
                    {
                        sql = $"insert EmployeeMap(OpenId, Code) values('{openid}', '{code}')";
                        connection.Execute(sql);
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotBound);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        public HttpResponseMessage QueryUsers([FromBody]UserFilter filter)
        {

        }
    }
}
