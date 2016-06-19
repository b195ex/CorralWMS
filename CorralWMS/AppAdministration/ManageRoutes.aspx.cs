using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageRoutes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
                Response.Redirect("~/SignIn.aspx");
            else if (!IsPostBack)
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var ReqPerm = permisos.FirstOrDefault(p => p.Id == 35);
                if (ReqPerm == null)
                    Response.Redirect("~/Default.aspx");
            }
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RtTxtBox.Text))
                return;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    Route nueva = new Route();
                    nueva.RouteName = RtTxtBox.Text;
                    ctx.Routes.Add(nueva);
                    ctx.SaveChanges();
                }
                RtGrid.DataBind();
                RtTxtBox.Text = "";
                RtTxtBox.Focus();
            }
            catch (Exception ex)
            {

            }
        }

        protected void RtGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}