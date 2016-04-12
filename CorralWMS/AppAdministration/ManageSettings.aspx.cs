using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageSettings : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 10).FirstOrDefault();
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

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(KeyTxt.Text) && !string.IsNullOrWhiteSpace(ValueTxt.Text))
            {
                var sett = new AppSetting() { Key = KeyTxt.Text, Value = ValueTxt.Text };
                using (var ctx = new LWMS_Context())
                {
                    ctx.AppSettings.Add(sett);
                    ctx.SaveChanges();
                }
                SettingsGrid.DataBind();
                KeyTxt.Text = ValueTxt.Text = "";
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