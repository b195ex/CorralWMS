using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Production
{
    public partial class ScanLocationEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 23).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else if (Session["CurrEntry"] == null)
                {
                    Response.Redirect("~/Production/GoodsReceipt.aspx");
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text)) return;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var SapSett = ctx.SapSettings.Find(1);
                    using (var cmd = new SqlCommand("SELECT AbsEntry,BinCode,WhsCode FROM OBIN WHERE BinCode=@BinCode", new SqlConnection(SapSett.ConnectionString)))
                    {
                        cmd.Parameters.Add(new SqlParameter("BinCode", BinCodeTxtBox.Text));
                        cmd.Connection.Open();
                        SqlDataReader lect = cmd.ExecuteReader();
                        if (!lect.Read())
                        {
                            throw new Exception("Esa ubicación no existe.");
                        }
                        
                        var entry = (ProdEntry)Session["CurrEntry"];
                        ctx.Entry(entry).State = System.Data.Entity.EntityState.Unchanged;
                        ctx.Entry(entry).Collection("EntryLocations").Load();
                        int binEntry = lect.GetInt32(0);
                        string bincode = lect.GetString(1);
                        var loc = entry.EntryLocations.FirstOrDefault(l => l.AbsEntry == binEntry);
                        if (loc == null)
                        {
                            loc = new EntryLocation();
                            loc.AbsEntry = lect.GetInt32(0);
                            loc.BinCode = bincode;
                            entry.EntryLocations.Add(loc);
                            ctx.SaveChanges();
                        }
                        ctx.Entry(loc).State = System.Data.Entity.EntityState.Detached;
                        ctx.Entry(entry).State = System.Data.Entity.EntityState.Detached;
                        Session.Add("CurrLoc", loc);
                    }
                }
                Response.Redirect("~/Production/BoxScanEntry.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void EndBtn_Click(object sender, EventArgs e)
        {

        }
    }
}