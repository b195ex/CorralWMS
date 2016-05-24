using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace CorralWMS
{
    /// <summary>
    /// This Handler returns data for autocompleting bincode textbox
    /// </summary>
    public class BinCodSrch : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string prefixText = context.Request.QueryString["q"];
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = Tools.Util.GetSapConnStr();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT BinCode FROM OBIN L LEFT JOIN OWHS R ON L.WhsCode=R.WhsCode WHERE SysBin='N' AND Inactive='N' AND BinCode LIKE '%'+@SearchText+'%'";
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    cmd.Connection = conn;
                    StringBuilder sb = new StringBuilder();
                    conn.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            sb.Append(sdr["ContactName"])
                                .Append(Environment.NewLine);
                        }
                    }
                    conn.Close();
                    context.Response.Write(sb.ToString());
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}