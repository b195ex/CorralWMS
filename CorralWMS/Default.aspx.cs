using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
                Response.Redirect("~/SignIn.aspx");
            using (var ctx = new LWMS_Context())
            {
                //Label1.Text = ctx.Database.Connection.ConnectionString;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var comp = Tools.SAPSingleton.oCompany;
            }
            catch (Exception ex)
            {
                Label1.Text += ex.Message;
            }
        }
    }
}