using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Inventories
{
    public partial class StartInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 32).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else if (!IsPostBack)
                {

                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var inv = ctx.WhsInvs.FirstOrDefault(w => w.WhsCode == WhsDropDn.SelectedValue && w.EndDate==null);
                    if (inv != null)
                        throw new Exception("Ya se está realizando inventario en ese almacén");
                    else
                    {
                        inv = new WhsInv();
                        inv.Locations = new HashSet<LocInv>();
                        inv.StartDate = DateTime.Now;
                        inv.UserId = ((User)Session["LoggedInUser"]).Id;
                        inv.WhsCode = WhsDropDn.SelectedValue;
                        inv.WhsName = WhsDropDn.SelectedItem.Text;
                        string sql = "SELECT AbsEntry, Bincode FROM OBIN WHERE SysBin='N' AND WhsCode=@WhsCode";
                        var param = new SqlParameter("WhsCode", WhsDropDn.SelectedValue);
                        using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                        {
                            cmd.Parameters.Add(param);
                            cmd.Connection.Open();
                            var rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                var loc = new LocInv();
                                loc.BinAbs = rdr.GetInt32(0);
                                loc.BinCode = rdr.GetString(1);
                                inv.Locations.Add(loc);
                            }
                        }
                        ctx.WhsInvs.Add(inv);
                        ctx.SaveChanges();
                        CurrInvGrid.DataBind();
                    }
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

        protected void CurrInvGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void InvDataSrc_Load(object sender, EventArgs e)
        {
            InvDataSrc.DataBind();
        }

        protected void WhsDataSrc_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                WhsDataSrc.DataBind();
        }

        protected void CreatePanel_Load(object sender, EventArgs e)
        {
            var permisos = (HashSet<Permission>)Session["Permissions"];
            if (permisos.FirstOrDefault(p => p.Id == int.Parse(CreatePanel.Attributes["reqperm"])) == null)
                CreatePanel.Visible = false;
        }

        protected void CurrInvGrid_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var usuario = (User)Session["LoggedInUser"];
                    var invid=(int)CurrInvGrid.DataKeys[e.NewSelectedIndex].Value;
                    var inv = ctx.WhsInvs.FirstOrDefault(w => w.Id == invid);
                    if (inv.UserId == usuario.Id)
                    {
                        ctx.Entry(inv).Collection("Locations").Load();
                        if (inv.Locations.Any(l => l.EndDate == null))
                            throw new Exception("Aún hay ubicaciones sin inventariar en el almacén");
                        else
                        {
                            var oCompany = Tools.SAPSingleton.oCompany;
                            InventoryCountingsService oICS = oCompany.GetCompanyService().GetBusinessService(ServiceTypes.InventoryCountingsService);
                            InventoryCounting oIC = oICS.GetDataInterface(InventoryCountingsServiceDataInterfaces.icsInventoryCounting);
                            oIC.CountDate = inv.StartDate;
                            oIC.CountingType = CountingTypeEnum.ctSingleCounter;
                            oIC.Remarks = string.Format("Inventario de almacén {0}, a cargo de {1}", inv.WhsName, inv.User.Name);
                            
                            ctx.Entry(inv).Collection("Locations").Load();
                            foreach (var loc in inv.Locations)
                            {
                                ctx.Entry(loc).Collection("Boxes").Load();
                                foreach (var box in loc.Boxes)
                                {
                                    InventoryCountingLine oICL = null;
                                    if (oIC.InventoryCountingLines.Count == 0)
                                    {
                                        oICL = oIC.InventoryCountingLines.Add();
                                        oICL.BinEntry = loc.BinAbs;
                                        oICL.Counted = BoYesNoEnum.tYES;
                                        oICL.ItemCode = box.ItemCode;
                                        oICL.WarehouseCode = inv.WhsCode;
                                    }
                                    else
                                    {
                                        int i;
                                        for (i = 0; i < oIC.InventoryCountingLines.Count; i++)
                                        {
                                            oICL = oIC.InventoryCountingLines.Item(i);
                                            if (oICL.BinEntry == loc.BinAbs && oICL.ItemCode == box.ItemCode)
                                                break;
                                        }
                                        if (i == oIC.InventoryCountingLines.Count)
                                        {
                                            oICL = oIC.InventoryCountingLines.Add();
                                            oICL.BinEntry = loc.BinAbs;
                                            oICL.Counted = BoYesNoEnum.tYES;
                                            oICL.ItemCode = box.ItemCode;
                                            oICL.WarehouseCode = inv.WhsCode;
                                        }
                                    }
                                    oICL.CountedQuantity += box.Weight;
                                    var oICBN = oICL.InventoryCountingBatchNumbers.Add();
                                    oICBN.BatchNumber = box.SAPBatch;
                                    oICBN.Quantity = box.Weight;
                                }
                                var oICP = oICS.Add(oIC);
                                inv.DocEntry = oICP.DocumentEntry;
                                oICS.Close(oICP);
                            }
                            inv.EndDate = DateTime.Now;
                            ctx.SaveChanges();
                            CurrInvGrid.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            } 
            e.Cancel = true;
        }
    }
}