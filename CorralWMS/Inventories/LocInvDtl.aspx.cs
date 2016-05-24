using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Inventories
{
    public partial class LocInvDtl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["InvId"]) || string.IsNullOrWhiteSpace(Request.QueryString["BinAbs"]))
                Response.Redirect("~/Inventories/StartInventory.aspx", true);
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.FirstOrDefault(p => p.Id == 32);
                if (perm == null)
                    Response.Redirect("~/Inventories/StartInventory.aspx", true);
            }
        }

        protected void BoxesGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}