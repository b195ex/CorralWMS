using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Production
{
    public partial class GoodsReceipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ProductionOrdersLstBox_DataBinding(object sender, EventArgs e)
        {
            try
            {
                SapSetting SAPSett;
                using (var ctx = new LWMS_Context())
                {
                    SAPSett = ctx.SapSettings.Find(1);
                }
                string cstr = string.Format("data source={0};initial catalog={1};persist security info=True;user id={2};password={4}", SAPSett.Server, SAPSett.CompanyDB, SAPSett.DbUserName, SAPSett.DbPassword);
                string cmdText = "SELECT * FROM OWOR WHERE Status='R' --AND ItemCode = @ItemCode";
                using (var cmd = new SqlCommand(cmdText, new SqlConnection(cstr)))
                {
                    SqlParameter param = new SqlParameter("ItemCode", "");
                    cmd.Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ProductionOrdersLstBox.DataTextField = "DocNum";
                    ProductionOrdersLstBox.DataValueField = "DocEntry";
                    ProductionOrdersLstBox.DataSource = dt;
                    ProductionOrdersLstBox.DataBind();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                //show the error message
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            AddPermissionAlert.Attributes["class"] = AddPermissionAlert.Attributes["class"].Replace("collapse", "");
        }
    }
}