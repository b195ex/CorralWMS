using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Transfer
{
    public partial class ScanFromLocation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usr = (User)Session["LoggedInUser"];
            if (usr == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 15).FirstOrDefault();
                if (perm == null)
                    Response.Redirect("~/Default.aspx");
                else if (Session["CurrReq"] == null)
                    Response.Redirect("~/Transfer/StartTransferReq.aspx");
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text))
                return;
            try
            {
                string query = "SELECT AbsEntry, WhsCode FROM OBIN WHERE BinCode=@BinCode";
                int AbsEntry;
                string BinWhsCode;
                SapSetting SapSett;
                using (var ctx = new LWMS_Context())
                {
                    SapSett = ctx.SapSettings.Find(1);
                    ctx.Entry(SapSett).State = System.Data.Entity.EntityState.Detached;
                }
                using (var cmd = new SqlCommand(query, new SqlConnection(SapSett.ConnectionString)))
                {
                    cmd.Parameters.Add("BinCode", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["BinCode"].Value = BinCodeTxtBox.Text;
                    cmd.Connection.Open();
                    SqlDataReader lect = cmd.ExecuteReader();
                    lect.Read();
                    AbsEntry = (int)lect["AbsEntry"];
                    BinWhsCode = (string)lect["WhsCode"];
                }
                var currReq = (TransReq)Session["CurrReq"];
                if (currReq.FromWhs == BinWhsCode)
                {
                    using (var ctx = new LWMS_Context())
                    {
                        ctx.Entry(currReq).State = System.Data.Entity.EntityState.Unchanged;
                        ctx.Entry(currReq).Collection("FromLocations").Load();
                        var fromLoc = currReq.FromLocations.Where(fl => fl.AbsEntry==AbsEntry).FirstOrDefault();
                        if (fromLoc == null)
                        {
                            fromLoc = new FromLocation();
                            fromLoc.AbsEntry = AbsEntry;
                            fromLoc.BinCode = BinCodeTxtBox.Text;
                            fromLoc.TransReqId = currReq.Id;
                            fromLoc.WhsCode = BinWhsCode;
                            currReq.FromLocations.Add(fromLoc);
                            ctx.SaveChanges();
                        }
                        ctx.Entry(fromLoc).State = System.Data.Entity.EntityState.Detached;
                        Session.Add("CurrFromLoc", fromLoc);
                        ctx.Entry(currReq).State = System.Data.Entity.EntityState.Detached;
                        Response.Redirect("~/Transfer/ReqBoxScan.aspx");
                    }
                }
                else
                    throw new Exception("La ubicación seleccionada no pertenece a la bodega de origen.");
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
                var transReq = (TransReq)Session["CurrReq"];
                var oCompany = Tools.SAPSingleton.oCompany;
                StockTransfer oTransReq = oCompany.GetBusinessObject(BoObjectTypes.oStockTransferDraft);
                oTransReq.DocObjectCode = BoObjectTypes.oStockTransfer;
                oTransReq.FromWarehouse = transReq.FromWhs;
                oTransReq.ToWarehouse = transReq.ToWhs;
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(transReq).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(transReq).Collection("FromLocations").Load();
                    foreach (var location in transReq.FromLocations)
                    {
                        ctx.Entry(location).Collection("Boxes").Load();
                        foreach (var box in location.Boxes)
                        {
                            if (oTransReq.Lines.Count == 1 && oTransReq.Lines.ItemCode == "")
                            {
                                oTransReq.FromWarehouse = transReq.FromWhs;
                                oTransReq.ToWarehouse = transReq.ToWhs;
                                oTransReq.Lines.ItemCode = box.ItemCode;
                                oTransReq.Lines.Quantity = box.Weight;
                                oTransReq.Lines.WarehouseCode = transReq.ToWhs;
                                oTransReq.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                                oTransReq.Lines.BatchNumbers.Quantity = box.Weight;
                                oTransReq.Lines.BinAllocations.BinAbsEntry = location.AbsEntry;
                                oTransReq.Lines.BinAllocations.Quantity = box.Weight;
                                oTransReq.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                            }
                            else
                            {
                                //more than one line found, get correct line
                                int i;
                                for (i = 0; i < oTransReq.Lines.Count; i++)
                                {
                                    oTransReq.Lines.SetCurrentLine(i);
                                    if (oTransReq.Lines.ItemCode == box.ItemCode)
                                        break;//correct line found exit for
                                }
                                if (i == oTransReq.Lines.Count)
                                {
                                    //correct line not found, add line, initialize
                                    oTransReq.Lines.Add();
                                    oTransReq.FromWarehouse = transReq.FromWhs;
                                    oTransReq.ToWarehouse = transReq.ToWhs;
                                    oTransReq.Lines.ItemCode = box.ItemCode;
                                    oTransReq.Lines.Quantity = box.Weight;
                                    oTransReq.Lines.WarehouseCode = transReq.ToWhs;
                                    oTransReq.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                                    oTransReq.Lines.BatchNumbers.Quantity = box.Weight;
                                    oTransReq.Lines.BinAllocations.BinAbsEntry = location.AbsEntry;
                                    oTransReq.Lines.BinAllocations.Quantity = box.Weight;
                                    oTransReq.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                                }
                                else
                                {
                                    oTransReq.Lines.Quantity += box.Weight;
                                    int y;
                                    for (y = 0; y < oTransReq.Lines.BatchNumbers.Count; y++)
                                    {
                                        oTransReq.Lines.BatchNumbers.SetCurrentLine(y);
                                        if (oTransReq.Lines.BatchNumbers.BatchNumber == box.SAPBatch)
                                            break;//correct batch found exit for
                                    }
                                    if (y == oTransReq.Lines.BatchNumbers.Count)
                                    {
                                        oTransReq.Lines.BatchNumbers.Add();
                                        oTransReq.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                                        oTransReq.Lines.BatchNumbers.Quantity = box.Weight;
                                    }
                                    else
                                    {
                                        oTransReq.Lines.BatchNumbers.Quantity += box.Weight;
                                    }
                                    for (i = 0; i < oTransReq.Lines.BinAllocations.Count; i++)
                                    {
                                        oTransReq.Lines.BinAllocations.SetCurrentLine(i);
                                        if (oTransReq.Lines.BinAllocations.BinAbsEntry == location.AbsEntry)
                                            break;//bin allocation found, exit for
                                    }
                                    if (i == oTransReq.Lines.BinAllocations.Count)
                                    {
                                        oTransReq.Lines.BinAllocations.Add();
                                        oTransReq.Lines.BinAllocations.BinAbsEntry = location.AbsEntry;
                                        oTransReq.Lines.BinAllocations.BinActionType = BinActionTypeEnum.batFromWarehouse;
                                        oTransReq.Lines.BinAllocations.Quantity = box.Weight;
                                        oTransReq.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = y;
                                    }
                                    else
                                    {
                                        oTransReq.Lines.BinAllocations.Quantity += box.Weight;
                                    }
                                }
                            }
                        }
                    }
                    if (oTransReq.Add() != 0)
                    {
                        int code;
                        string mess;
                        oCompany.GetLastError(out code, out mess);
                        throw new Exception(string.Format("Error al crear draft; {0}:{1}", code, mess));
                    }
                    else
                    {
                        string asdf = oCompany.GetNewObjectKey();
                        transReq.DocEntry = int.Parse(asdf);
                        transReq.EndDate = DateTime.Now;
                        ctx.SaveChanges();
                        Session.Remove("CurrReq");
                        Session.Remove("CurrFromLoc");
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