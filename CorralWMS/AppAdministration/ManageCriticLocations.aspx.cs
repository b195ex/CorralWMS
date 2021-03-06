﻿using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.AppAdministration
{
    public partial class ManageCriticLocations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedInUser"] == null)
                Response.Redirect("~/SignIn.aspx");
            else if (!IsPostBack)
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var ReqPerm = permisos.Where(p => p.Id == 25).FirstOrDefault();
                if (ReqPerm == null)
                    Response.Redirect("~/Default.aspx");
            }
        }

        protected void WhsDataSrc_Load(object sender, EventArgs e)
        {
            var datasrc = (SqlDataSource)sender;
            datasrc.DataBind();
        }

        protected void AddBtn_Click(object sender, EventArgs e)
        {
            var critLoc = new CriticLocation();
                critLoc.BinCode = BinCodDropDn.SelectedItem.Text;
                critLoc.BinEntry = int.Parse(BinCodDropDn.SelectedValue);
                critLoc.Priority = int.Parse(PriorityTxtBox.Text);
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.CriticLocations.Add(critLoc);
                    ctx.SaveChanges();
                }
                PriorityTxtBox.Text = "";
                CritLocationsGrid.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                AddExceptionLabel.Text = ex.Message;
                AddAlert.Attributes["class"] = AddAlert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void CritLocationsGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void CritLocationsGrid_Load(object sender, EventArgs e)
        {
            var control = (GridView)sender;
            int ReqPerm, DelPerm;
            if (!int.TryParse(control.Attributes["reqperm"], out ReqPerm))
                ReqPerm=-1;
            if (!int.TryParse(control.Attributes["delperm"], out DelPerm))
                DelPerm = -1;
            var permisos = (HashSet<Permission>)Session["Permissions"];
            var perm_req = permisos.FirstOrDefault(p => p.Id == ReqPerm);
            var del_perm = permisos.FirstOrDefault(p => p.Id == DelPerm);
            if (perm_req != null)
                control.AutoGenerateEditButton = true;
            if (del_perm != null)
                control.AutoGenerateDeleteButton = true;
        }

        protected void AddPanel_Load(object sender, EventArgs e)
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
    }
}