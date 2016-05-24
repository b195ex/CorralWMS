using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Inventories
{
    public partial class LocInvBoxScan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var usr = (User)Session["LoggedInUser"];
                if (usr == null)
                    Response.Redirect("~/SignIn.aspx");
                else
                {
                    var permisos = (HashSet<Permission>)Session["Permissions"];
                    var perm = permisos.Where(p => p.Id == 33).FirstOrDefault();
                    if (perm == null)
                        Response.Redirect("~/Default.aspx");
                    else
                    {
                        var currlocinv = (LocInv)Session["CurrLocInv"];
                        if (currlocinv == null)
                            Response.Redirect("~/Inventories/ScanBin.aspx");
                        else
                        {
                            BinLbl.Text = currlocinv.BinCode;
                            BoxesDataSrc.WhereParameters["InvId"].DefaultValue = currlocinv.WhsinvId.ToString();
                            BoxesDataSrc.WhereParameters["BinAbs"].DefaultValue = currlocinv.BinAbs.ToString();
                        }
                    }
                }
            }
        }

        protected void BoxesGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void ScanBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(ScanTxtBox.Text))
                return;
            try
            {
                if (!(ScanTxtBox.Text.IndexOf('-') + 7 == ScanTxtBox.Text.IndexOf('.') && ScanTxtBox.Text.Length >= 30))
                {
                    throw new Exception("El código parece inválido");
                }
                string itmcod = ScanTxtBox.Text.Substring(0, 8);
                double wt = double.Parse(ScanTxtBox.Text.Substring(8, 5));
                string batch = ScanTxtBox.Text.Substring(13, 9);
                int boxid = int.Parse(ScanTxtBox.Text.Substring(22));
                using (var ctx = new LWMS_Context())
                {
                    var vox = ctx.Boxes.FirstOrDefault(b => b.Batch == batch && b.Id == boxid && b.ItemCode == itmcod);
                    if (vox == null)
                    {
                        vox = new Box();
                        vox.Batch = batch;
                        vox.Id = boxid;
                        vox.ItemCode = itmcod;
                        vox.Weight = wt;
                        var itm = ctx.Items.FirstOrDefault(i => i.ItemCode == itmcod);
                        if (itm == null)
                        {
                            itm = new Item();
                            itm.ItemCode = itmcod;
                            ctx.Items.Add(itm);
                        }
                        ctx.Boxes.Add(vox);
                    }
                    var currloc = (LocInv)Session["CurrLocInv"];
                    ctx.Entry(currloc).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(currloc).Collection("Boxes").Load();
                    if (currloc.Boxes.Contains(vox))
                        throw new Exception("Esa caja ya se ingresó al conteo.");
                    currloc.Boxes.Add(vox);
                    ctx.SaveChanges();
                    ctx.Entry(currloc).State = System.Data.Entity.EntityState.Detached;
                    ScanTxtBox.Text = "";
                    BoxesGrid.DataBind();
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void EndBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var currloc = (LocInv)Session["CurrLocInv"];
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currloc).State = System.Data.Entity.EntityState.Unchanged;
                    currloc.EndDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                Session.Remove("CurrLocInv");
                Response.Redirect("~/Inventories/ScanBin.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }
    }
}