using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Production
{
    public partial class ScanLocationEntry : System.Web.UI.Page
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
                else if (Session["CurrEntry"] == null)
                {
                    Response.Redirect("~/Production/GoodsReceipt.aspx");
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text)) return;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var SapSett = ctx.SapSettings.Find(1);
                    using (var cmd = new SqlCommand("SELECT AbsEntry,BinCode,WhsCode FROM OBIN WHERE BinCode=@BinCode", new SqlConnection(SapSett.ConnectionString)))
                    {
                        cmd.Parameters.Add(new SqlParameter("BinCode", BinCodeTxtBox.Text));
                        cmd.Connection.Open();
                        SqlDataReader lect = cmd.ExecuteReader();
                        if (!lect.Read())
                        {
                            throw new Exception("Esa ubicación no existe.");
                        }
                        
                        var entry = (ProdEntry)Session["CurrEntry"];
                        ctx.Entry(entry).State = System.Data.Entity.EntityState.Unchanged;
                        ctx.Entry(entry).Collection("EntryLocations").Load();
                        int binEntry = lect.GetInt32(0);
                        var loc = entry.EntryLocations.FirstOrDefault(l => l.AbsEntry == binEntry);
                        if (loc == null)
                        {
                            loc = new EntryLocation();
                            loc.AbsEntry = lect.GetInt32(0);
                            loc.BinCode = lect.GetString(1);
                            loc.WhsCode = lect.GetString(2);
                            entry.EntryLocations.Add(loc);
                            ctx.SaveChanges();
                        }
                        ctx.Entry(loc).State = System.Data.Entity.EntityState.Detached;
                        ctx.Entry(entry).State = System.Data.Entity.EntityState.Detached;
                        Session.Add("CurrLoc", loc);
                    }
                }
                Response.Redirect("~/Production/BoxScanEntry.aspx");
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
                        if (oInvEntry.Lines.BinAllocations.BinAbsEntry != 0)
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
    }
}