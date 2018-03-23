using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;

namespace MailRoomMessager.Helper
{
    public class DBHelper
    {
        private static string ConnectionString;

        public static string GetConnectionString()
        {
            if (ConnectionString == null)
            {
                ConnectionString = ConfigurationManager.AppSettings["DBConnectionString"].ToString();
            }
            return ConnectionString;
        }

        public static IList<T> QueryList<T>(string cmd)
        {
            IList<T> list = null;
            string connStr = GetConnectionString();
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                list = conn.Query<T>(cmd).ToList<T>();
                return list;
            }
        }

        public static T GetObject<T>(string cmd)
        {
            T obj = default(T);
            string connStr = GetConnectionString();
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                obj = conn.QueryFirst<T>(cmd);
                return obj;
            }
        }

        public static int Execute(string cmd)
        {
            int result;
            string connStr = GetConnectionString();
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                result = conn.Execute(cmd);
                return result;
            }
        }
        
    }
}