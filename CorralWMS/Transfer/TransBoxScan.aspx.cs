using CorralWMS.Entities;
using SAPbobsCOM;
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

        protected void Button1_Click(object sender, EventArgs e)
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
                    //ARMAR
                    var boxes = currTrans.ToLocations.SelectMany(l => l.Boxes).OrderBy(b=>b.ItemCode);
                    foreach (var box in boxes)
                    {
                        int linenum;
                        for (linenum = 0; linenum < oTrans.Lines.Count; linenum++)
                        {
                            oTrans.Lines.SetCurrentLine(linenum);
                            if (oTrans.Lines.ItemCode == box.ItemCode)
                                break;
                        }
                        int batchlinenum;
                        for (batchlinenum = 0; batchlinenum < oTrans.Lines.BatchNumbers.Count; batchlinenum++)
                        {
                            oTrans.Lines.BatchNumbers.SetCurrentLine(batchlinenum);
                            if (oTrans.Lines.BatchNumbers.BatchNumber == box.SAPBatch)
                                break;
                        }
                        int i;
                        for (i = 0; i < oTrans.Lines.BinAllocations.Count; i++)
                        {
                            oTrans.Lines.BinAllocations.SetCurrentLine(i);
                            if (oTrans.Lines.BinAllocations.BinAbsEntry == box.ToLocations.First(tl => tl.TransferId == currReq.Id).AbsEntry && oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine == batchlinenum)
                                break;
                        }
                        if (i == oTrans.Lines.BinAllocations.Count)
                            oTrans.Lines.BinAllocations.Add();
                        oTrans.Lines.BinAllocations.BinAbsEntry = box.ToLocations.First(tl => tl.TransferId == currTrans.Id).AbsEntry;
                        oTrans.Lines.BinAllocations.BinActionType = BinActionTypeEnum.batToWarehouse;
                        oTrans.Lines.BinAllocations.Quantity = box.Weight;
                        oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = batchlinenum;
                    }
                    //FIN ARMAR
                    //for (int l = 0; l < oTrans.Lines.Count; l++)
                    //{
                    //    oTrans.Lines.SetCurrentLine(l);
                    //    string item = oTrans.Lines.ItemCode;
                    //    string from = oTrans.Lines.FromWarehouseCode;
                    //    string to = oTrans.Lines.WarehouseCode;
                    //    double amnt = oTrans.Lines.Quantity;
                    //    for (int b = 0; b < oTrans.Lines.BatchNumbers.Count; b++)
                    //    {
                    //        oTrans.Lines.BatchNumbers.SetCurrentLine(b);
                    //        string bat = oTrans.Lines.BatchNumbers.BatchNumber;
                    //    }
                    //    for (int c = 0; c < oTrans.Lines.BinAllocations.Count; c++)
                    //    {
                    //        oTrans.Lines.BatchNumbers.SetCurrentLine(c);
                    //        int binabs = oTrans.Lines.BinAllocations.BinAbsEntry;
                    //        BinActionTypeEnum binact = oTrans.Lines.BinAllocations.BinActionType;
                    //        double bnamnt = oTrans.Lines.BinAllocations.Quantity;
                    //        int basebatch = oTrans.Lines.BinAllocations.SerialAndBatchNumbersBaseLine;
                    //    }
                    //}
                    if (oTrans.Update() != 0)
                    {
                        int errcod;
                        string errmess;
                        oCompany.GetLastError(out errcod, out errmess);
                        throw new Exception(string.Format("Error al Actualizar Draft, {0}:{1}", errcod, errmess));
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