using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.CycleCounts
{
    public partial class DoCycleCount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.FirstOrDefault(p => p.Id == 24);
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    if (IsPostBack) return;
                    try
                    {
                        using (var ctx = new LWMS_Context())
                        {
                            var currcount = ctx.CycleCounts.FirstOrDefault(c => c.Closed == false && c.UserId == usuario.Id && c.Date==DateTime.Today);
                            if (currcount == null)
                            {
                                var counts = ctx.CycleCounts.Where(c => c.Date == DateTime.Today).Select(c => c.BinEntry);
                                currcount = new CycleCount()
                                {
                                    Closed = false,
                                    Date = DateTime.Today,
                                    UserId = usuario.Id
                                };
                                if (ctx.CriticLocations.Any(c => !counts.Contains(c.BinEntry)))
                                {
                                    var cl = ctx.CriticLocations.OrderBy(l => l.Priority).FirstOrDefault(c => !counts.Contains(c.BinEntry));
                                    currcount.BinEntry = cl.BinEntry;
                                    currcount.BinCode = cl.BinCode;
                                }
                                else
                                {
                                    string query = "SELECT TOP 1 AbsEntry,BinCode FROM OBIN WHERE SysBin='N'ORDER BY NEWID()";
                                    using (var cmd = new SqlCommand(query,new SqlConnection(ctx.SapSettings.Find(1).ConnectionString)))
                                    {
                                        cmd.Connection.Open();
                                        var rdr = cmd.ExecuteReader();
                                        rdr.Read();
                                        while (counts.Contains(rdr.GetInt32(0)))
                                        {
                                            rdr = cmd.ExecuteReader();
                                            rdr.Read();
                                        }
                                        currcount.BinEntry = rdr.GetInt32(0);
                                        currcount.BinCode = rdr.GetString(1);
                                    }
                                }
                                ctx.CycleCounts.Add(currcount);
                                ctx.SaveChanges();
                            }
                            ctx.Entry(currcount).State = System.Data.Entity.EntityState.Detached;
                            BinCodeLabel.Text = currcount.BinCode;
                            Session.Add("CurrCount", currcount);
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
            }
        }

        protected void ScanButton_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrEmpty(ScanTextBox.Text))
                return;
            try
            {
                if (!(ScanTextBox.Text.IndexOf('-') + 7 == ScanTextBox.Text.IndexOf('.') && ScanTextBox.Text.Length >= 30))
                {
                    throw new Exception("El código parece inválido");
                }
                string itmcod = ScanTextBox.Text.Substring(0, 8);
                double wt = double.Parse(ScanTextBox.Text.Substring(8, 5));
                string batch = ScanTextBox.Text.Substring(13, 9);
                int boxid = int.Parse(ScanTextBox.Text.Substring(22));
                var currcount = (CycleCount)Session["CurrCount"];
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currcount).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(currcount).Collection("Boxes").Load();
                    var box = ctx.Boxes.FirstOrDefault(b => b.Batch == batch && b.Id == boxid && b.ItemCode == itmcod);
                    if (box == null)
                    {
                        var itm = ctx.Items.FirstOrDefault(i => i.ItemCode == itmcod);
                        if (itm == null)
                        {
                            itm = new Item();
                            itm.ItemCode = itmcod;
                            ctx.Items.Add(itm);
                        }
                        box = new Box();
                        box.Batch = batch;
                        box.Id = boxid;
                        box.ItemCode = itmcod;
                        box.Weight = wt;
                    }
                    currcount.Boxes.Add(box);
                    ctx.SaveChanges();
                    ctx.Entry(currcount).State = System.Data.Entity.EntityState.Detached;
                    BoxesGrid.DataBind();
                    ScanTextBox.Text = "";
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

        protected void BoxesDataSrc_Load(object sender, EventArgs e)
        {
            BoxesDataSrc.SelectParameters["Date"].DefaultValue = ((CycleCount)Session["CurrCount"]).Date.ToString();
            BoxesDataSrc.SelectParameters["BinEntry"].DefaultValue = ((CycleCount)Session["CurrCount"]).BinEntry.ToString();
            BoxesDataSrc.DataBind();
        }

        protected void BoxesGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void EndBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            try
            {
                var currcount = (CycleCount)Session["CurrCount"];
                string sql = "SELECT WhsCode FROM OBIN WHERE AbsEntry=@AbsEntry";
                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                {
                    var param = new SqlParameter("AbsEntry", currcount.BinEntry);
                    param.DbType = System.Data.DbType.Int32;
                    cmd.Parameters.Add(param);
                    cmd.Connection.Open();
                    sql=cmd.ExecuteScalar().ToString();
                }
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currcount).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(currcount).Collection("Boxes").Load();
                    ctx.Entry(currcount).Reference("User").Load();
                    var oCompany = Tools.SAPSingleton.oCompany;
                    InventoryCountingsService oICS = oCompany.GetCompanyService().GetBusinessService(ServiceTypes.InventoryCountingsService);
                    InventoryCounting oIC = oICS.GetDataInterface(InventoryCountingsServiceDataInterfaces.icsInventoryCounting);
                    oIC.CountDate = currcount.Date;
                    oIC.CountingType = CountingTypeEnum.ctSingleCounter;
                    oIC.Remarks = string.Format("Inventario cíclico de ubicación {0}, realizado por {1}", currcount.BinCode, currcount.User.Name);
                    ctx.Entry(currcount).Collection("Boxes").Load();
                    foreach (var box in currcount.Boxes.OrderBy(b => b.ItemCode))
                    {
                        InventoryCountingLine oICL = null;
                        if (oIC.InventoryCountingLines.Count == 0)
                        {
                            string uom;
                            using (var cmd = new SqlCommand("SELECT UomCode FROM OITM LEFT JOIN OUOM ON IUoMEntry=UomEntry WHERE ItemCode=@ItemCode", new SqlConnection(Tools.Util.GetSapConnStr())))
                            {
                                cmd.Parameters.Add(new SqlParameter("Itemcode", box.ItemCode));
                                cmd.Connection.Open();
                                uom = (string)cmd.ExecuteScalar();
                            }
                            oICL = oIC.InventoryCountingLines.Add();
                            oICL.BinEntry = currcount.BinEntry;
                            oICL.Counted = BoYesNoEnum.tYES;
                            oICL.ItemCode = box.ItemCode;
                            oICL.UoMCode = uom;
                            oICL.WarehouseCode = sql;
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
                                string uom;
                                using (var cmd = new SqlCommand("SELECT UomCode FROM OITM LEFT JOIN OUOM ON IUoMEntry=UomEntry WHERE ItemCode=@ItemCode", new SqlConnection(Tools.Util.GetSapConnStr())))
                                {
                                    cmd.Parameters.Add(new SqlParameter("Itemcode", box.ItemCode));
                                    cmd.Connection.Open();
                                    uom = (string)cmd.ExecuteScalar();
                                }
                                oICL = oIC.InventoryCountingLines.Add();
                                oICL.BinEntry = currcount.BinEntry;
                                oICL.Counted = BoYesNoEnum.tYES;
                                oICL.ItemCode = box.ItemCode;
                                oICL.UoMCode = uom;
                                oICL.WarehouseCode = sql;
                            }
                        }
                        oICL.CountedQuantity += box.Weight;
                        var oICBN = oICL.InventoryCountingBatchNumbers.Add();
                        oICBN.BatchNumber = box.SAPBatch;
                        oICBN.Quantity = box.Weight;
                    }
                    var oICP = oICS.Add(oIC);
                    //oICS.Close(oICP);
                    currcount.Closed = true;
                    ctx.SaveChanges();
                }
                Session.Remove("CurrCount");
                Response.Redirect("~/Default.aspx");
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