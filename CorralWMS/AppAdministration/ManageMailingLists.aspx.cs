using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageMailingLists : System.Web.UI.Page
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
                    var ReqPerm = permisos.Where(p => p.Id == 20).FirstOrDefault();
                    if (ReqPerm == null)
                        Response.Redirect("~/Default.aspx");
                }
            }
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            if (!AddAlert.Attributes["class"].Contains("collapse"))
                AddAlert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(NameTxtBox.Text))
                return;
            try
            {
                var list = new MailingList();
                list.Name = NameTxtBox.Text;
                using (var ctx = new LWMS_Context())
                {
                    ctx.MailingLists.Add(list);
                    ctx.SaveChanges();
                }
                NameTxtBox.Text = "";
                ListGrid.DataBind();
                ListDropDn.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                AddExceptionLabel.Text = ex.Message;
                AddAlert.Attributes["class"] = AddAlert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void ListGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
            var grid = (GridView)sender;
            var descriptor = (ICustomTypeDescriptor)e.Row.DataItem;
            var property = descriptor.GetProperties().Cast<PropertyDescriptor>().First();
            var list = (MailingList)descriptor.GetPropertyOwner(property);
            var childgrid = (GridView)e.Row.FindControl("RecipientsGrid");
            childgrid.DataSource = list.Recipients.ToList();
            childgrid.DataBind();
        }

        protected void SetRecipientsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
            var grid = (GridView)sender;
            var childgrid = (GridView)e.Row.FindControl("RecipientsGrid");
            var descriptor = (ICustomTypeDescriptor)e.Row.DataItem;
            var property = descriptor.GetProperties().Cast<PropertyDescriptor>().First();
            var usr = (User)descriptor.GetPropertyOwner(property);
            using (var ctx = new LWMS_Context())
            {
                var mList = ctx.MailingLists.Find(int.Parse(ListDropDn.SelectedValue));
                var check = (CheckBox)e.Row.FindControl("AssignCheck");
                foreach (var ml in usr.MailingLists)
                {
                    if (ml.Id == mList.Id)
                    {
                        check.Checked = true;
                        break;
                    }
                }
            }
        }

        protected void SavBtn_Click(object sender, EventArgs e)
        {
            if (!EditAlert.Attributes["class"].Contains("collapse"))
                EditAlert.Attributes["class"] += "collapse";
            MailingList ml;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ml = ctx.MailingLists.Find(int.Parse(ListDropDn.SelectedValue));
                    foreach (GridViewRow row in SetRecipientsGrid.Rows)
                    {
                        var assigncheck = (CheckBox)row.FindControl("AssignCheck");
                        var usr = ctx.Users.Find(SetRecipientsGrid.DataKeys[row.RowIndex].Value);
                        if (assigncheck.Checked)
                        {
                            if (!usr.MailingLists.Contains(ml))
                                usr.MailingLists.Add(ml);
                        }
                        else
                        {
                            if (usr.MailingLists.Contains(ml))
                                usr.MailingLists.Remove(ml);
                        }
                    }
                    if (ctx.SaveChanges() > 0)
                        ListGrid.DataBind();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                EditExceptionLabel.Text = ex.Message;
                EditAlert.Attributes["class"] = EditAlert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void ListDropDn_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetRecipientsGrid.DataBind();
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

        protected void HtmlControl_Load(object sender, EventArgs e)
        {
            var control = (HtmlGenericControl)sender;
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
    }
}