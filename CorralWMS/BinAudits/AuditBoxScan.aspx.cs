using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.BinAudits
{
    public partial class AuditBoxScan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.FirstOrDefault(p => p.Id == 29);
                if (perm == null)
                    Response.Redirect("~/Default.aspx");
                else
                {
                    if (Session["CurrAudit"] == null)
                        Response.Redirect("~/BinAudits/AuditBoxScan.aspx");
                }
            }
        }

        protected void ScanBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(ScanTxt.Text))
                return;
            try
            {
                if (!(ScanTxt.Text.IndexOf('-') + 7 == ScanTxt.Text.IndexOf('.') && ScanTxt.Text.Length >= 30))
                {
                    throw new Exception("El código parece inválido");
                }
                string itmcod = ScanTxt.Text.Substring(0, 8);
                double wt = double.Parse(ScanTxt.Text.Substring(8, 5));
                string batch = ScanTxt.Text.Substring(13, 9);
                int boxid = int.Parse(ScanTxt.Text.Substring(22));
                using (var ctx = new LWMS_Context())
                {
                    var box = ctx.Boxes.Find(new object[] { batch, boxid, itmcod });
                    if (box == null)
                    {
                        var itm = ctx.Items.Find(itmcod);
                        if (itm == null)
                        {
                            itm = new Item()
                            {
                                ItemCode=itmcod
                            };
                            ctx.Items.Add(itm);
                        }
                        box = new Box()
                        {
                            Batch = batch,
                            Id = boxid,
                            ItemCode=itmcod,
                            Weight=wt
                        };
                        ctx.Boxes.Add(box);
                    }
                    var curraudit = (BinAudit)Session["CurrAudit"];
                    ctx.Entry(curraudit).State = System.Data.Entity.EntityState.Unchanged;
                    curraudit.Boxes.Add(box);
                    ctx.SaveChanges();
                    BoxesGrid.DataSource = curraudit.Boxes.ToList();
                    BoxesGrid.DataBind();
                    ctx.Entry(curraudit).State = System.Data.Entity.EntityState.Detached;
                    ScanTxt.Text = "";
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

        protected void BoxesGrid_Load(object sender, EventArgs e)
        {
            using (var ctx = new LWMS_Context())
            {
                var curraudit = (BinAudit)Session["CurrAudit"];
                ctx.Entry(curraudit).State = System.Data.Entity.EntityState.Unchanged;
                ctx.Entry(curraudit).Collection("Boxes").Load();
                BoxesGrid.DataSource = curraudit.Boxes.ToList();
                BoxesGrid.DataBind();
            }
        }

        protected void EndBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            try
            {
                var curraudit = (BinAudit)Session["CurrAudit"];
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(curraudit).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(curraudit).Collection("Boxes").Load();
                    var oCompany = Tools.SAPSingleton.oCompany;
                    InventoryCountingsService oICS = oCompany.GetCompanyService().GetBusinessService(ServiceTypes.InventoryCountingsService);
                    InventoryCounting oIC = oICS.GetDataInterface(InventoryCountingsServiceDataInterfaces.icsInventoryCounting);
                    oIC.CountDate = DateTime.Today;
                    oIC.CountingType = CountingTypeEnum.ctSingleCounter;
                    ctx.Entry(curraudit).Reference("User").Load();
                    oIC.Remarks = string.Format("Auditoría de ubicación {0}, realizado por {1}", curraudit.BinCode, curraudit.User.Name);
                    ctx.Entry(curraudit).Collection("Boxes").Load();
                    foreach (var box in curraudit.Boxes.OrderBy(b => b.ItemCode))
                    {
                        InventoryCountingLine oICL = null;
                        if (oIC.InventoryCountingLines.Count == 0)
                        {
                            oICL = oIC.InventoryCountingLines.Add();
                            oICL.BinEntry = curraudit.BinEntry;
                            oICL.Counted = BoYesNoEnum.tYES;
                            oICL.ItemCode = box.ItemCode;
                            oICL.WarehouseCode = curraudit.WhsCode;
                            oICL.CountedQuantity = 0;
                        }
                        else
                        {
                            int i;
                            for (i = 0; i < oIC.InventoryCountingLines.Count; i++)
                            {
                                oICL = oIC.InventoryCountingLines.Item(i);
                                if (oICL.ItemCode == box.ItemCode)
                                    break;
                            }
                            if (i == oIC.InventoryCountingLines.Count)
                            {
                                oICL = oIC.InventoryCountingLines.Add();
                                oICL.BinEntry = curraudit.BinEntry;
                                oICL.Counted = BoYesNoEnum.tYES;
                                oICL.ItemCode = box.ItemCode;
                                oICL.WarehouseCode = curraudit.WhsCode;
                            }
                        }
                        oICL.CountedQuantity += box.Weight;
                        var oICBN = oICL.InventoryCountingBatchNumbers.Add();
                        oICBN.BatchNumber = box.SAPBatch;
                        oICBN.Quantity = box.Weight;
                    }
                    var oICP = oICS.Add(oIC);
                    oICS.Close(oICP);
                    curraudit.EndDate = DateTime.Now;
                    ctx.SaveChanges();
                }
                Session.Remove("CurrAudit");
                Response.Redirect("~/BinAudits/ScanAuditBin.aspx");
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