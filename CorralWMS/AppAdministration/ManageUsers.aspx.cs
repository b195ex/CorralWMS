using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageUsers : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 8).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void CreateBtn_Click(object sender, EventArgs e)
        {
            var nuevo = new User()
            {
                Active = true,
                Email = EmailTxt.Text,
                FirstName = FirstNameTxt.Text,
                LastName=LastNameTxt.Text,
                Password=PasswordTxt.Text,
                UserName=UserNameTxt.Text
            };
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.Users.Add(nuevo);
                    ctx.SaveChanges();
                }
                EmailTxt.Text = FirstNameTxt.Text = LastNameTxt.Text = PasswordTxt.Text = UserNameTxt.Text = "";
                EditUsersGrid.DataBind();
                UserDropDn.DataBind();
                if (!AddUserAlert.Attributes["class"].Contains("collapse"))
                    AddUserAlert.Attributes["class"] += "collapse";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                AdduserExceptionLabel.Text = ex.Message;
                AddUserAlert.Attributes["class"] = AddUserAlert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void RoleAssignGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var grid = (GridView)sender;
                using (var ctx = new LWMS_Context())
                {
                    var rol = ctx.Roles.Find(grid.DataKeys[e.Row.RowIndex].Value);
                    var usr = ctx.Users.Find(int.Parse(UserDropDn.SelectedValue));
                    ctx.Entry(usr).Collection("Roles").Load();
                    if (usr.Roles.Contains(rol))
                    {
                        var check = (CheckBox)e.Row.Cells[0].FindControl("AssignCheck");
                        check.Checked = true;
                    }
                }
            }
        }

        protected void UserDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            RoleAssignGrid.DataBind();
        }

        protected void AssignBtn_Click(object sender, EventArgs e)
        {
            using (var ctx = new LWMS_Context())
            {
                var usr = ctx.Users.Find(int.Parse(UserDropDn.SelectedValue));
                ctx.Entry(usr).Collection("Roles").Load();
                foreach (GridViewRow row in RoleAssignGrid.Rows)
                {
                    var rol = ctx.Roles.Find(RoleAssignGrid.DataKeys[row.RowIndex].Value);
                    if (((CheckBox)row.Cells[0].FindControl("AssignCheck")).Checked)
                    {
                        if (!usr.Roles.Contains(rol))
                            usr.Roles.Add(rol);
                    }
                    else
                    {
                        if (usr.Roles.Contains(rol))
                            usr.Roles.Remove(rol);
                    }
                }
                ctx.SaveChanges();
            }
            RoleAssignGrid.DataBind();
        }

        protected void showAlert(string id)
        {
            StringBuilder sb = new StringBuilder("<script type=\"text/javascript\">$(document).ready(function(){$(\"#");
            sb.Append(id).Append("\").show();});</script>");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", sb.ToString(), false);
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if args.Value
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