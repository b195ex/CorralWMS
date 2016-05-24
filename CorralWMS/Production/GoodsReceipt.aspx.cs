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
                else using (var ctx = new LWMS_Context())
                {
                    try
                    {
                        var sett = ctx.SapSettings.Find(1);
                        ProdOrdrDataSrc.ConnectionString = sett.ConnectionString;
                        var entry = ctx.ProdEntries.Where(p => p.DocEntry == null && p.UserId == usuario.Id).FirstOrDefault();
                        if (entry != null)
                        {
                            Session.Add("CurrEntry", entry);
                            Response.Redirect("~/Production/ScanLocationEntry.aspx");
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
            }
        }

        protected void ProdOrdrGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void ProdOrdrGrid_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            var grid = (GridView)sender;
            var ProdOrdrKey= (int)grid.DataKeys[e.NewSelectedIndex].Value;
            var Entry = new ProdEntry();
            Entry.BaseEntry = ProdOrdrKey;
            Entry.ItemCode = grid.Rows[e.NewSelectedIndex].Cells[1].Text;
            Entry.UserId = ((User)Session["LoggedInUser"]).Id;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.ProdEntries.Add(Entry);
                    ctx.SaveChanges();
                    ctx.Entry(Entry).State = System.Data.Entity.EntityState.Detached;
                }
                Session.Add("CurrEntry", Entry);
                Response.Redirect("~/Production/ScanLocationEntry.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
            e.Cancel = true;
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            var Entry = new ProdEntry();
            Entry.UserId = ((User)Session["LoggedInUser"]).Id;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.ProdEntries.Add(Entry);
                    ctx.SaveChanges();
                    ctx.Entry(Entry).State = System.Data.Entity.EntityState.Detached;
                }
                Session.Add("CurrEntry", Entry);
                Response.Redirect("~/Production/ScanLocationEntry.aspx");
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