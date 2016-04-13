using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAPbobsCOM;

namespace CorralWMS.AppAdministration
{
    public partial class ManageSAP : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 14).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void SAPDetails_Load(object sender, EventArgs e)
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

        protected void SrvrTypeDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var cell = dropdn.Parent;
            var text = (TextBox)cell.FindControl("SrvrTypeTxt");
            text.Text = dropdn.SelectedItem.Text;
        }

        protected void SrvrTypeDropDn_DataBinding(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var values = System.Enum.GetValues(typeof(BoDataServerTypes));
            var names = System.Enum.GetNames(typeof(BoDataServerTypes));
            for (int i = 0; i < names.Length; i++)
            {
                var item = new ListItem(names[i], values.GetValue(i).ToString());
                dropdn.Items.Add(item);
            }
            var cell = dropdn.Parent;
            var text = (TextBox)cell.FindControl("SrvrTypeTxt");
            dropdn.SelectedItem.Selected = false;
            dropdn.Items.FindByText(text.Text).Selected=true;
        }

        protected void langDropDn_DataBinding(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var values = System.Enum.GetValues(typeof(BoSuppLangs));
            var names = System.Enum.GetNames(typeof(BoSuppLangs));
            for (int i = 0; i < names.Length; i++)
            {
                var item = new ListItem(names[i], values.GetValue(i).ToString());
                dropdn.Items.Add(item);
            }
            var cell = dropdn.Parent;
            var text = (TextBox)cell.FindControl("langTxtBox");
            dropdn.SelectedItem.Selected = false;
            dropdn.Items.FindByText(text.Text).Selected = true;
        }

        protected void langDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdn = (DropDownList)sender;
            var cell = dropdn.Parent;
            var text = (TextBox)cell.FindControl("langTxtBox");
            text.Text = dropdn.SelectedItem.Text;
        }
    }
}