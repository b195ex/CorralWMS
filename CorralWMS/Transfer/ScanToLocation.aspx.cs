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
    public partial class ScanToLocation : System.Web.UI.Page
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
                else
                {
                    var currtrans = (Entities.Transfer)Session["CurrTrans"];
                    if (currtrans == null)
                        Response.Redirect("~/Transfer/ReceiveTrans.aspx");
                    else
                    {
                        using (var ctx = new LWMS_Context())
                        {
                            ctx.Entry(currtrans).State = System.Data.Entity.EntityState.Unchanged;
                            ctx.Entry(currtrans).Reference("TransReq").Load();
                            FromLabel.Text = currtrans.TransReq.FromWhs;
                            ToLabel.Text = currtrans.TransReq.ToWhs;
                            ctx.Entry(currtrans).State = System.Data.Entity.EntityState.Detached;
                        }
                    }
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text))
                return;
            var trans = (Entities.Transfer)Session["CurrTrans"];
            try
            {
                int AbsEntry;
                string BinWhsCode;
                string query = "SELECT AbsEntry, WhsCode FROM OBIN WHERE BinCode=@BinCode";
                SapSetting SapSett;
                var currTrans = (Entities.Transfer)Session["CurrTrans"];
                TransReq currReq;
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currTrans).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(currTrans).Reference("TransReq").Load();
                    currReq = currTrans.TransReq;
                    ctx.Entry(currTrans).State = System.Data.Entity.EntityState.Detached;
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
                if (currReq.ToWhs == BinWhsCode)
                {
                    using (var ctx = new LWMS_Context())
                    {
                        ctx.Entry(currTrans).State = System.Data.Entity.EntityState.Unchanged;
                        ctx.Entry(currTrans).Collection("ToLocations").Load();
                        var toLoc = currTrans.ToLocations.Where(fl => fl.AbsEntry == AbsEntry).FirstOrDefault();
                        if (toLoc == null)
                        {
                            toLoc = new ToLocation();
                            toLoc.AbsEntry = AbsEntry;
                            toLoc.BinCode = BinCodeTxtBox.Text;
                            toLoc.TransferId = currTrans.Id;
                            toLoc.WhsCode = BinWhsCode;
                            currTrans.ToLocations.Add(toLoc);
                            ctx.SaveChanges();
                        }
                        ctx.Entry(toLoc).State = System.Data.Entity.EntityState.Detached;
                        Session.Add("CurrToLoc", toLoc);
                        ctx.Entry(currTrans).State = System.Data.Entity.EntityState.Detached;
                        Response.Redirect("~/Transfer/TransBoxScan.aspx");
                    }
                }
                else
                    throw new Exception("La ubicación seleccionada no pertenece a la bodega de destino.");
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
                var oCompany = Tools.SAPSingleton.oCompany;
                var currTrans = (Entities.Transfer)Session["CurrTrans"];
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currTrans).State = System.Data.Entity.EntityState.Unchanged;
                    StockTransfer oTrans = oCompany.GetBusinessObject(BoObjectTypes.oStockTransferDraft);
                    oTrans.GetByKey(currTrans.TransReq.DocEntry.Value);
                    var currReq = currTrans.TransReq;
                    foreach (var toLoc in currTrans.ToLocations)
                    {
                        foreach (var box in toLoc.Boxes)
                        {
                            //find the line (through the itemcode)
                            int i;
                            for (i = 0; i < oTrans.Lines.Count; i++)
                            {
                                oTrans.Lines.SetCurrentLine(i);
                                if (box.ItemCode == oTrans.Lines.ItemCode)
                                {
                                    //line found, find batch
                                    int bat;
                                    for (bat = 0; bat < oTrans.Lines.BatchNumbers.Count; bat++)
                                    {
                                        oTrans.Lines.BatchNumbers.SetCurrentLine(bat);
                                        if (box.SAPBatch == oTrans.Lines.BatchNumbers.BatchNumber)
                                        {
                                            //batch found, find bin
                                            int loc;
                                            for (loc = 0; loc < oTrans.Lines.BinAllocations.Count; loc++)
                                            {
                                                oTrans.Lines.BinAllocations.SetCurrentLine(loc);
                                                if (oTrans.Lines.BinAllocations.BinAbsEntry == toLoc.AbsEntry && oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine==bat)
                                                {
                                                    //bin found, add weight
                                                    oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = bat;
                                                    oTrans.Lines.BinAllocations.Quantity += box.Weight;
                                                    break;
                                                }
                                            }
                                            if (loc == oTrans.Lines.BinAllocations.Count)
                                            {
                                                //bin not found, add bin
                                                oTrans.Lines.BinAllocations.Add();
                                                oTrans.Lines.BinAllocations.BinAbsEntry = toLoc.AbsEntry;
                                                oTrans.Lines.BinAllocations.BinActionType = BinActionTypeEnum.batToWarehouse;
                                                oTrans.Lines.BinAllocations.Quantity = box.Weight;
                                                oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = bat;
                                            }
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (oTrans.SaveDraftToDocument() != 0)
                    {
                        int errcod;
                        string errmess;
                        oCompany.GetLastError(out errcod, out errmess);
                        throw new Exception(string.Format("Error al Crear Doc, {0}:{1}", errcod, errmess));
                    }
                    else
                    {
                        string asdf = oCompany.GetNewObjectKey();
                        currTrans.DocEntry = int.Parse(asdf);
                        oCompany.GetNewObjectCode(out asdf);
                        currTrans.DocNum = int.Parse(asdf);
                        currTrans.EndDate = DateTime.Now;
                        ctx.SaveChanges();
                        Session.Remove("CurrTrans");
                        Session.Remove("CurrToLoc");
                        Response.Redirect("~/Transfer/ReceiveTrans.aspx");
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