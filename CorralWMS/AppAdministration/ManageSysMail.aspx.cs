using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageSysMail : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 19).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
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

        protected void MailSettingView_Load(object sender, EventArgs e)
        {
            var control = (DetailsView)sender;
            int ReqPerm;
            if (!int.TryParse(control.Attributes["reqperm"], out ReqPerm))
                return;
            HashSet<Permission> permisos = (HashSet<Permission>)Session["Permissions"];
            var perm_req = permisos.Where(p => p.Id == ReqPerm).FirstOrDefault();
            if (perm_req != null)
                control.AutoGenerateEditButton = true;
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            if (!AddAlert.Attributes["class"].Contains("collapse"))
                AddAlert.Attributes["class"] += "collapse";
            try
            {
                var mailSett = new MailSetting();
                mailSett.FromAddress = FromAddressTxt.Text;
                mailSett.FromPass = PwdTxt.Text;
                mailSett.MailHost = HostTxtBox.Text;
                mailSett.MailPort = int.Parse(PortTxtBox.Text);
                using (var ctx = new LWMS_Context())
                {
                    ctx.MailSettings.Add(mailSett);
                    ctx.SaveChanges();
                }
                MailSettingView.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                AddExceptionLabel.Text = ex.Message;
                AddAlert.Attributes["class"] = AddAlert.Attributes["class"].Replace("collapse", "");
            }
        }
    }
}