using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageItems : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                if (!IsPostBack)
                {
                    var permisos = (HashSet<Permission>)Session["Permissions"];
                    var ReqPerm = permisos.Where(p => p.Id == 34).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            string sql = "SELECT ItemCode, ItemName FROM OITM WHERE SellItem='Y' AND ItmsGrpCod NOT IN (116,117,120,121,122,123,124,125,126,127,129)";
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var itms = ctx.Items.Select(i => i.ItemCode);
                    var bldr = new StringBuilder(" AND ItemCode NOT IN (");
                    foreach(var itm in itms)
                    {
                        bldr.Append("'").Append(itm).Append("'").Append(",");
                    }
                    bldr.Remove(bldr.Length - 1, 1);
                    bldr.Append(")");
                    if (bldr.Length > 22)
                    {
                        sql += bldr.ToString();
                    }
                    using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                    {
                        cmd.Connection.Open();
                        var rdr = cmd.ExecuteReader();
                        Item itm;
                        while (rdr.Read())
                        {
                            itm = new Item();
                            itm.ItemCode = rdr.GetString(0);
                            itm.ItemName = rdr.GetString(1);
                            itm.Duration = 0;
                            ctx.Items.Add(itm);
                        }
                    }
                    if (ctx.SaveChanges() > 0)
                    {
                        ExceptionLabel.Text = "Se agregaron Artículos, por favor ingrese la duración de ese producto.";
                        Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
                    }
                    ItemsGrid.DataBind();
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

        protected void ItemsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "Duration").ToString() == "0")
                {
                    e.Row.BackColor = System.Drawing.Color.Yellow;
                    e.Row.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void ItemsGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}