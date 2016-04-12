using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageRoles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["LoggedInUser"]==null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                if (!IsPostBack)
                {
                    var permisos = (HashSet<Permission>)Session["Permissions"];
                    var ReqPerm = permisos.Where(p => p.Id == 9).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void CreateBtn_Click(object sender, EventArgs e)
        {
            var nuevo = new Role() 
            { 
                Description = string.IsNullOrWhiteSpace(DescTxt.Text) ? null : DescTxt.Text, 
                RoleName = RoleNameTxt.Text 
            };
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.Roles.Add(nuevo);
                    ctx.SaveChanges();
                }
                DescTxt.Text = RoleNameTxt.Text = "";
                RolesGrid.DataBind();
                RolesDropDn.DataBind();
                if(!AddRoleAlert.Attributes["class"].Contains("collapse"))
                    AddRoleAlert.Attributes["class"]+="collapse";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                AddRoleExceptionLabel.Text = ex.Message;
                AddRoleAlert.Attributes["class"] = AddRoleAlert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void RolesDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            AssignPermissionsGrid.DataBind();
        }

        protected void AssignBtn_Click(object sender, EventArgs e)
        {
            using (var ctx = new LWMS_Context())
            {
                var rol = ctx.Roles.Find(int.Parse(RolesDropDn.SelectedValue));
                ctx.Entry(rol).Collection("Permissions").Load();
                foreach (GridViewRow row in AssignPermissionsGrid.Rows)
                {
                    var perm = ctx.Permissions.Find(AssignPermissionsGrid.DataKeys[row.RowIndex].Value);
                    var check=(CheckBox)row.Cells[0].FindControl("AssignCheck");
                    if (check.Checked)
                    {
                        if (!rol.Permissions.Contains(perm))
                            rol.Permissions.Add(perm);
                    }
                    else
                    {
                        if (rol.Permissions.Contains(perm))
                            rol.Permissions.Remove(perm);
                    }
                }
                ctx.SaveChanges();
            }
            AssignPermissionsGrid.DataBind();
        }

        protected void AssignPermissionsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType==DataControlRowType.DataRow)
            using (var ctx = new LWMS_Context())
            {
                var rol = ctx.Roles.Find(int.Parse(RolesDropDn.SelectedValue));
                ctx.Entry(rol).Collection("Permissions").Load();
                var perm = ctx.Permissions.Find(AssignPermissionsGrid.DataKeys[e.Row.RowIndex].Value);
                if (rol.Permissions.Contains(perm))
                    ((CheckBox)e.Row.Cells[0].FindControl("AssignCheck")).Checked = true;
            }
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