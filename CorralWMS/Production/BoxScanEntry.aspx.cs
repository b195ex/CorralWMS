using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Production
{
    public partial class BoxScanEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 23).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else if (Session["CurrLoc"] == null)
                {
                    Response.Redirect("~/Production/ScanLocationEntry.aspx");
                }
                else
                {
                    var currloc=(EntryLocation)Session["CurrLoc"];
                    BoxDataSrc.WhereParameters["ProdEntryID"].DefaultValue = currloc.ProdEntryID.ToString();
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoxTxtBox.Text)) 
                return;
            try
            {
                if (!(BoxTxtBox.Text.IndexOf('-') + 7 == BoxTxtBox.Text.IndexOf('.') && BoxTxtBox.Text.Length >= 30))
                    throw new Exception("El código parece inválido");
                string itmcod = BoxTxtBox.Text.Substring(0, 8);
                double wt = double.Parse(BoxTxtBox.Text.Substring(8, 5));
                string batch = BoxTxtBox.Text.Substring(13, 9);
                int boxid = int.Parse(BoxTxtBox.Text.Substring(22));
                var currEntry = (ProdEntry)Session["CurrEntry"];

                if (currEntry.BaseEntry != null)
                {
                    if (currEntry.ItemCode != itmcod)
                    {
                        throw new Exception(string.Format("La orden de producción es para {0}, esta caja es de {1}", currEntry.ItemCode, itmcod));
                    }
                }
                using (var ctx = new LWMS_Context())
                {
                    var box = ctx.Boxes.FirstOrDefault(b => b.Batch == batch && b.Id == boxid && b.ItemCode == itmcod);
                    if (box != null) 
                        throw new Exception("Esa caja ya existe.");
                    box = new Box()
                    {
                        Batch = batch,
                        Id = boxid,
                        ItemCode=itmcod,
                        ManufDate=DateTime.Today,
                        Weight=wt
                    };
                    var loc = (EntryLocation)Session["CurrLoc"];
                    ctx.Entry(loc).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(loc).Collection("Boxes").Load();
                    loc.Boxes.Add(box);
                    ctx.SaveChanges();
                    ctx.Entry(loc).State = System.Data.Entity.EntityState.Detached;
                }
                BoxTxtBox.Text = "";
                BoxGrid.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void EndBtn_Click(object sender, EventArgs e)
        {
            var entry = (ProdEntry)Session["CurrEntry"];
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(entry).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(entry).Collection("EntryLocations").Load();
                    var oCompany = Tools.SAPSingleton.oCompany;
                    Documents oInvEntry = oCompany.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);
                    oInvEntry.Comments = "Ingresado por: " + ((User)Session["LoggedInUser"]).Name;
                    var boxes = entry.EntryLocations.SelectMany(el => el.Boxes);
                    int test = oInvEntry.Lines.BaseEntry;
                    foreach (var box in boxes)
                    {
                        int linenum;
                        for (linenum = 0; linenum < oInvEntry.Lines.Count; linenum++)
                        {
                            oInvEntry.Lines.SetCurrentLine(linenum);
                            if (oInvEntry.Lines.ItemCode == box.ItemCode && oInvEntry.Lines.BaseEntry == (box.EntryLocation.ProdEntry.BaseEntry == null ? 0 : box.EntryLocation.ProdEntry.BaseEntry.Value) && oInvEntry.Lines.WarehouseCode == box.EntryLocation.WhsCode)
                                break;
                        }
                        if (linenum == oInvEntry.Lines.Count)
                        {
                            if (oInvEntry.Lines.ItemCode != "")
                                oInvEntry.Lines.Add();
                            if (box.EntryLocation.ProdEntry.BaseEntry != null)
                                oInvEntry.Lines.BaseEntry = box.EntryLocation.ProdEntry.BaseEntry.Value;
                            oInvEntry.Lines.ItemCode = box.ItemCode;
                            oInvEntry.Lines.WarehouseCode = box.EntryLocation.WhsCode;
                        }
                        oInvEntry.Lines.Quantity += box.Weight;
                        if (oInvEntry.Lines.BatchNumbers.BatchNumber != "")
                            oInvEntry.Lines.BatchNumbers.Add();
                        oInvEntry.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                        oInvEntry.Lines.BatchNumbers.ExpiryDate = box.ExpDate;
                        oInvEntry.Lines.BatchNumbers.ManufacturingDate = box.ManufDate.Value;
                        oInvEntry.Lines.BatchNumbers.Quantity = box.Weight;
                        if(oInvEntry.Lines.BinAllocations.BinAbsEntry!=0)
                            oInvEntry.Lines.BinAllocations.Add();
                        oInvEntry.Lines.BinAllocations.BinAbsEntry = box.EntryLocation.AbsEntry;
                        oInvEntry.Lines.BinAllocations.Quantity = box.Weight;
                        oInvEntry.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = oInvEntry.Lines.BatchNumbers.Count - 1;
                    }
                    if (oInvEntry.Add() != 0)
                    {
                        int errCode;
                        string errMsg;
                        oCompany.GetLastError(out errCode, out errMsg);
                        throw new Exception(string.Format("Error al Crear Ingreso en SAP {0}:{1}", errCode, errMsg));
                    }
                    else
                    {
                        entry.DocEntry = int.Parse(oCompany.GetNewObjectKey());
                        ctx.SaveChanges();
                        Session.Remove("CurrEntry");
                        Session.Remove("CurrLoc");
                        Response.Redirect("~/Production/GoodsReceipt.aspx");
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

        protected void BoxGrid_PreRender(object sender, EventArgs e)
        {
            if (BoxGrid.HeaderRow != null)
                BoxGrid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}