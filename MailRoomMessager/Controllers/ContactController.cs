using Dapper;
using MailRoomMessager.Helper;
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
    [RoutePrefix("api/contacts")]
    public class ContactController : ApiController
    {
        [Route("wx")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetContacts([FromUri]string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }
            string openid = await OpenIdHelper.GetOpenIdAsync(code);
            //string openid = "o5dhF1EKpj54qs6-GNoYtLPUSUOA";

            if (string.IsNullOrEmpty(code))
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
                    sql = $"select * from Contact where OpenId='{openid}'";
                    var contacts = connection.Query<Contact>(sql);
                    if (contacts != null && contacts.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, contacts);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, employees.ToList()[0].OpenId);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        [Route("wx")]
        [HttpPut]
        public HttpResponseMessage SaveContacts([FromBody]JObject jObject)
        {
            List<Contact> contacts = JsonConvert.DeserializeObject<List<Contact>>(jObject["contacts"].ToString());
            string openid = jObject["openid"].ToString();
            if (string.IsNullOrEmpty(openid))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            foreach (var contact in contacts)
            {
                contact.OpenId = openid;
            }
            string sql = $"delete from Contact where OpenId='{contacts[0].OpenId}'";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    connection.Execute(sql);
                    sql = @"insert Contact(OpenId, Call)
                                               values(@OpenId, @Call)";
                    connection.Execute(sql, contacts);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }

        [Route("wx/sg")]
        [HttpGet]
        public HttpResponseMessage GetSuggestedContacts([FromUri]string call)
        {
            if (string.IsNullOrEmpty(call))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, Error.LackInfo);
            }

            string sql = $"select top 3 Call, Name from Contact left join Employee on Contact.OpenId = Employee.OpenId where Call like '{call}%' order by call";

            try
            {
                using (IDbConnection connection = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    var contacts = connection.Query<SuggestedContact>(sql);
                    if (contacts != null && contacts.Count() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, contacts);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error.NotFound);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error.Server);
            }
        }
    }
}
