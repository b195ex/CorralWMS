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
    public partial class ManageCustomers : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 36).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void SyncBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            string sql = "SELECT CardCode, CardName FROM OCRD WHERE CardType='C'";
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var ClientCodes = ctx.Clients.Select(i => i.CardCode);
                    var bldr = new StringBuilder(" AND CardCode NOT IN (");
                    foreach (var code in ClientCodes)
                    {
                        bldr.Append("'").Append(code).Append("'").Append(",");
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
                        Client cust;
                        while (rdr.Read())
                        {
                            cust = new Client();
                            cust.CardCode = rdr.GetString(0);
                            cust.CardName = rdr.GetString(1);
                            cust.RouteID = null;
                            ctx.Clients.Add(cust);
                        }
                        if (ctx.SaveChanges() > 0)
                        {
                            ExceptionLabel.Text = "Se agregaron Clientes, por favor asigne una Ruta a cada nuevo cliente.";
                            Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
                        }
                    }
                }
                ClientsGrid.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void RouteDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var cell = dropdn.Parent;
            var tb = (TextBox)cell.FindControl("TextBox1");
            tb.Text = dropdn.SelectedValue;
        }

        protected void ClientsGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void RouteDropDn_DataBound(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var cell = dropdn.Parent;
            var tb = (TextBox)cell.FindControl("TextBox1");
            if (dropdn.SelectedItem != null)
                dropdn.SelectedItem.Selected = false;
            foreach (ListItem itm in dropdn.Items)
            {
                if (itm.Value == tb.Text)
                {
                    itm.Selected = true;
                    break;
                }
            }
        }
    }
}