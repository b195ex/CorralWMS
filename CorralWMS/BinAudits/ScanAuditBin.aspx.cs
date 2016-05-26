using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.BinAudits
{
    public partial class ScanAuditBin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.FirstOrDefault(p => p.Id == 29);
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    using (var ctx = new LWMS_Context())
                    {
                        var currAudit = ctx.BinAudits.FirstOrDefault(a => a.UserId == usuario.Id && a.EndDate == null);
                        if (currAudit != null)
                        {
                            ctx.Entry(currAudit).State = System.Data.Entity.EntityState.Detached;
                            Session.Add("CurrAudit", currAudit);
                            Response.Redirect("~/BinAudits/AuditBoxScan.aspx");
                        }
                    }
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text))
                return;
            var param = new SqlParameter("Bincode", BinCodeTxtBox.Text);
            string sql="SELECT AbsEntry,WhsCode FROM OBIN WHERE BinCode=@BinCode";
            string whscode;
            int binabs;
            try
            {
                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                {
                    cmd.Parameters.Add(param);
                    cmd.Connection.Open();
                    var rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        binabs = rdr.GetInt32(0);
                        whscode = rdr.GetString(1);
                    }
                    else
                        throw new Exception("Esa ubicación no existe");
                }
                BinAudit curraudit = new BinAudit();
                curraudit.UserId = ((User)Session["LoggedInUser"]).Id;
                curraudit.BinCode = BinCodeTxtBox.Text;
                curraudit.BinEntry = binabs;
                curraudit.StartDate = DateTime.Now;
                curraudit.WhsCode = whscode;
                using (var ctx = new LWMS_Context())
                {
                    ctx.BinAudits.Add(curraudit);
                    ctx.SaveChanges();
                    ctx.Entry(curraudit).State = System.Data.Entity.EntityState.Detached;
                }
                Session.Add("CurrAudit", curraudit);
                Response.Redirect("~/BinAudits/AuditBoxScan.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }
    }
}