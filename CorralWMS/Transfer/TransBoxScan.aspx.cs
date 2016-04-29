using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Transfer
{
    public partial class TransBoxScan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usr = (User)Session["LoggedInUser"];
            if (usr == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 16).FirstOrDefault();
                if (perm == null)
                    Response.Redirect("~/Default.aspx");
                else if (Session["CurrToLoc"] == null)
                    Response.Redirect("~/Transfer/ScanToLocation.aspx");
                else
                {
                    var trans = (Entities.Transfer)Session["CurrTrans"];
                    if (trans != null)
                    {
                        BoxDataSrc.SelectParameters["ID"].DefaultValue = trans.Id.ToString();
                    }
                }
            }
        }

        protected void ScanBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(ScanTxt.Text))
                return;
            if (!(ScanTxt.Text.IndexOf('-') + 7 == ScanTxt.Text.IndexOf('.') && ScanTxt.Text.Length >= 30))
            {
                ExceptionLabel.Text = "El código parece inválido";
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
                return;
            }
            string itmcod = ScanTxt.Text.Substring(0, 8);
            double wt = double.Parse(ScanTxt.Text.Substring(8, 5));
            string batch = ScanTxt.Text.Substring(13, 9);
            int boxid = int.Parse(ScanTxt.Text.Substring(22));
            var trans = (Entities.Transfer)Session["CurrTrans"];
            var toLoc = (ToLocation)Session["CurrToLoc"];
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(trans).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(toLoc).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(toLoc).Collection("Boxes").Load();
                    ctx.Entry(trans).Reference("TransReq").Load();
                    ctx.Entry(trans.TransReq).Collection("FromLocations").Load();
                    foreach (var FLoc in trans.TransReq.FromLocations)
                    {
                        ctx.Entry(FLoc).Collection("Boxes").Load();
                        foreach (var box in FLoc.Boxes)
                        {
                            if (box.Id == boxid && box.Batch == batch && box.ItemCode == itmcod)
                            {
                                if (box.ToLocations.Count(t=>t.TransferId==trans.Id) > 0)
                                {
                                    throw new Exception("Esa caja ya se recibió");
                                }
                                toLoc.Boxes.Add(box);
                                ctx.SaveChanges();
                                ScanTxt.Text = "";
                                BoxGrid.DataBind();
                                return;
                            }
                        }
                    }
                    throw new Exception("Esa caja no es parte del envío");
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

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}