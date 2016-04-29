using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Transfer
{
    public partial class StartTransferReq : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id ==15).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else using (var ctx = new LWMS_Context())
                {
                    try
                    {
                        var req = ctx.TransReqs.Where(tr => tr.DocEntry == null && tr.UserId == usuario.Id).FirstOrDefault();
                        if (req != null)
                        {
                            Session.Add("CurrReq", req);
                            Response.Redirect("~/Transfer/ScanFromLocation.aspx");
                        }
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException!=null)
                            ex = ex.InnerException;
                        ExceptionLabel.Text = ex.Message;
                        Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
                    }
                }
            }
        }

        protected void DropDn_Load(object sender, EventArgs e)
        {
            
            if(!IsPostBack)try
            {
                SapSetting SAPSett;
                using (var ctx = new LWMS_Context())
                {
                    SAPSett = ctx.SapSettings.Find(1);
                    ctx.Entry(SAPSett).State = System.Data.Entity.EntityState.Detached;
                }
                string cmdText = "SELECT WhsCode, WhsCode+' - '+WhsName WhsName FROM OWHS WHERE BinActivat='Y'";
                using (var cmd = new SqlCommand(cmdText, new SqlConnection(SAPSett.ConnectionString)))
                {
                    cmd.Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    var dropdn = (DropDownList)sender;
                    dropdn.DataSource = dt;
                    dropdn.DataBind();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (FromWhsDropDn.SelectedValue != ToWhsDropDn.SelectedValue)
            {
                try
                {
                    var req = new TransReq();
                    req.FromWhs = FromWhsDropDn.SelectedValue;
                    req.StartDate = DateTime.Now;
                    req.ToWhs = ToWhsDropDn.SelectedValue;
                    var usr = (User)Session["LoggedInUser"];
                    req.UserId = usr.Id;
                    using (var ctx = new LWMS_Context())
                    {
                        ctx.TransReqs.Add(req);
                        ctx.SaveChanges();
                        ctx.Entry(req).State = System.Data.Entity.EntityState.Detached;
                    }
                    Session.Add("CurrReq", req);
                    Response.Redirect("~/Transfer/ScanFromLocation.aspx");
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
}