using CorralWMS.Entities;
using CorralWMS.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ClearAlerts()
        {
            if (!LoginException.Attributes["class"].Contains("collapse"))
                LoginException.Attributes["class"] += "collapse";
            if (!SignInFailed.Attributes["class"].Contains("collapse"))
                SignInFailed.Attributes["class"] += "collapse";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ClearAlerts();
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var usuario = ctx.Users.Where(u => u.UserName == inputEmail.Text && u.Password == inputPassword.Text).FirstOrDefault();
                    if (usuario == null)
                    {
                        SignInFailed.Attributes["class"] = SignInFailed.Attributes["class"].Replace("collapse", "");
                    }
                    else
                    {
                        var permissions = SecurityTools.GetPermissions(usuario, ctx);
                        Session.Add("LoggedInUser", usuario);
                        Session.Add("Permissions", permissions);
                        Response.Redirect("~/Default.aspx");
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ErrorLabel.Text = ex.Message;
                LoginException.Attributes["class"] = LoginException.Attributes["class"].Replace("collapse", "");
            }
        }
    }
}