using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManagePermissions : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 7).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PermissionTxt.Text))
            {
                var perm = new Permission() { Description = PermissionTxt.Text };
                try
                {
                    using (var ctx = new LWMS_Context())
                    {
                        ctx.Permissions.Add(perm);
                        ctx.SaveChanges();
                    }
                    PermissionGrid.DataBind();
                    PermissionTxt.Text = "";
                    if (!AddPermissionAlert.Attributes["class"].Contains("collapse"))
                        AddPermissionAlert.Attributes["class"] += "collapse";
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    AddPermissionExceptionLabel.Text = ex.Message;
                    AddPermissionAlert.Attributes["class"] = AddPermissionAlert.Attributes["class"].Replace("collapse", "");
                }
            }
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            int ReqPerm;
            if (!int.TryParse(control.Attributes["reqperm"], out ReqPerm))
            {
                control.Visible = false;
                return;
            }
            HashSet<Permission> permisos = (HashSet<Permission>)Session["Permissions"];
            var perm_req = permisos.Where(p => p.Id == ReqPerm).FirstOrDefault();
            if (perm_req == null)
                control.Visible = false;
        }

        protected void Grid_Load(object sender, EventArgs e)
        {
            var control = (GridView)sender;
            int ReqPerm;
            if (!int.TryParse(control.Attributes["reqperm"], out ReqPerm))
                return;
            HashSet<Permission> permisos = (HashSet<Permission>)Session["Permissions"];
            var perm_req = permisos.Where(p => p.Id == ReqPerm).FirstOrDefault();
            if (perm_req != null)
                control.AutoGenerateEditButton = true;
        }
    }
}