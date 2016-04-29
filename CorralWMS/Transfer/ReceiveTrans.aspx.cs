using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Transfer
{
    public partial class ReceiveTrans : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 16).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else using (var ctx = new LWMS_Context())
                {
                    try
                    {
                        var req = ctx.Transfers.Where(tr => tr.DocEntry == null && tr.UserId == usuario.Id).FirstOrDefault();
                        if (req != null)
                        {
                            Session.Add("CurrTrans", req);
                            Response.Redirect("~/Transfer/ScanToLocation.aspx");
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

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var grid = (GridView)sender;
                var datasrc = (SqlDataSource)e.Row.FindControl("DtlDataSrc");
                var dtlgrd = (GridView)e.Row.FindControl("DtlGrid");
                datasrc.SelectParameters["TransReqID"].DefaultValue = grid.DataKeys[e.Row.RowIndex].Value.ToString();
                dtlgrd.DataBind();
            }
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void GridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            var grid = (GridView)sender;
            var user = (User)Session["LoggedInuser"];
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var req = ctx.TransReqs.Find(grid.DataKeys[e.NewSelectedIndex].Value);
                    var trans = new Entities.Transfer();
                    trans.Id = req.Id;
                    trans.StartDate = DateTime.Now;
                    trans.UserId = user.Id;
                    ctx.Transfers.Add(trans);
                    ctx.SaveChanges();
                    Session.Add("CurrTrans", trans);
                    Response.Redirect("~/Transfer/ScanToLocation.aspx");
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