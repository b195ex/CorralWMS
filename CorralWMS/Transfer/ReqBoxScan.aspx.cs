using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Transfer
{
    public partial class ReqBoxScan : System.Web.UI.Page
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
                else if (Session["CurrFromLoc"] == null)
                    Response.Redirect("~/Transfer/ScanFromLocation.aspx");
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
                string query = "SELECT*FROM OBBQ T0 LEFT JOIN OBTN T1 ON T0.SnBMDAbs=T1.AbsEntry WHERE T0.Itemcode=@ItemCode AND DistNumber=@BatchNum AND BinAbs=@BinAbs AND OnHandQty>0";
                var currFromLoc = (FromLocation)Session["CurrFromLoc"];
                SapSetting sapsett;
                using (var ctx = new LWMS_Context())
                {
                    sapsett = ctx.SapSettings.Find(1);
                    ctx.Entry(sapsett).State = System.Data.Entity.EntityState.Detached;
                }
                using (var cmd = new SqlCommand(query, new SqlConnection(sapsett.ConnectionString)))
                {
                    cmd.Parameters.Add(new SqlParameter("ItemCode", itmcod));
                    cmd.Parameters.Add(new SqlParameter("BatchNum", batch+boxid.ToString().PadLeft(8,'0')));
                    cmd.Parameters.Add(new SqlParameter("BinAbs", currFromLoc.AbsEntry));
                    cmd.Connection.Open();
                    var lect = cmd.ExecuteReader();
                    if (!lect.HasRows)
                    {
                        throw new Exception("Esa Caja no está en esta ubicación");
                    }
                }
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currFromLoc).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(currFromLoc).Collection("Boxes").Load();
                    var CurrBox = currFromLoc.Boxes.Where(b => b.Batch == batch && b.Id == boxid && b.ItemCode==itmcod).FirstOrDefault();
                    if (CurrBox != null)
                        throw new Exception("La caja ya se había agregado al traslado.");
                    else
                    {
                        CurrBox = ctx.Boxes.Find(new object[] { batch, boxid, itmcod });
                        if (CurrBox == null)
                        {
                            var itm = ctx.Items.Find(itmcod);
                            if (itm == null)
                            {
                                itm = new Item() { ItemCode = itmcod };
                                ctx.Items.Add(itm);
                            }
                            CurrBox = new Box() { Batch=batch, Id=boxid, ItemCode=itmcod, Weight=wt };
                            ctx.Boxes.Add(CurrBox);
                        }
                        currFromLoc.Boxes.Add(CurrBox);
                        ctx.SaveChanges();
                        ctx.Entry(currFromLoc).State = System.Data.Entity.EntityState.Detached;
                        BoxGrid_Load(BoxGrid, e);
                        ScanTxt.Text = "";
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

        protected void Grid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void BoxGrid_Load(object sender, EventArgs e)
        {
            string connStr, sql = @"SELECT BinCode,ItemCode,Box_Batch,Box_Id,Weight 
                FROM FromLocations T0 
                LEFT JOIN FromLocationBoxes T1 ON T0.TransReqId=T1.FromLocation_TransReqId AND T0.AbsEntry=T1.FromLocation_AbsEntry 
                LEFT JOIN Boxes T2 ON T1.Box_Batch=T2.Batch AND T1.Box_Id=T2.Id AND T1.Box_ItemCode=T2.ItemCode
                WHERE T0.TransReqId=@TransReqId";
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    connStr = ctx.Database.Connection.ConnectionString;
                }
                using (var cmd = new SqlCommand(sql, new SqlConnection(connStr)))
                {
                    cmd.Parameters.Add(new SqlParameter("TransReqId", ((FromLocation)Session["CurrFromLoc"]).TransReqId));
                    cmd.Connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    BoxGrid.DataSource = dt;
                    BoxGrid.DataBind();
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
                            int uom;
                            using (var cmd = new SqlCommand("SELECT IUoMEntry FROM OITM WHERE ItemCode=@ItemCode", new SqlConnection(Tools.Util.GetSapConnStr())))
                            {
                                cmd.Parameters.Add(new SqlParameter("ItemCode", box.ItemCode));
                                cmd.Connection.Open();
                                uom=(int)cmd.ExecuteScalar();
                            }
                            //find the item line in the document
                            int l;
                            for (l = 0; l < oTransReq.Lines.Count; l++)
                            {
                                oTransReq.Lines.SetCurrentLine(l);
                                if (oTransReq.Lines.ItemCode == box.ItemCode)
                                    break;//item line found
                            }
                            if (l == oTransReq.Lines.Count)
                            {
                                //line not found 
                                if (oTransReq.Lines.ItemCode != "")
                                    oTransReq.Lines.Add();//non empty line found, add new
                                //now working on empty line
                                oTransReq.Lines.FromWarehouseCode = transReq.FromWhs;
                                oTransReq.Lines.ItemCode = box.ItemCode;
                                oTransReq.Lines.UoMEntry = uom;
                                oTransReq.Lines.WarehouseCode = transReq.ToWhs;
                            }
                            //now working on initialized line
                            oTransReq.Lines.Quantity += box.Weight;
                            oTransReq.Lines.BatchNumbers.SetCurrentLine(0);
                            if (oTransReq.Lines.BatchNumbers.BatchNumber != "")
                                oTransReq.Lines.BatchNumbers.Add();
                            oTransReq.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                            oTransReq.Lines.BatchNumbers.Quantity = box.Weight;
                            oTransReq.Lines.BinAllocations.SetCurrentLine(0);
                            if (oTransReq.Lines.BinAllocations.BinAbsEntry != 0)
                                oTransReq.Lines.BinAllocations.Add();
                            oTransReq.Lines.BinAllocations.BinAbsEntry = location.AbsEntry;
                            oTransReq.Lines.BinAllocations.BinActionType = BinActionTypeEnum.batFromWarehouse;
                            oTransReq.Lines.BinAllocations.Quantity += box.Weight;
                            oTransReq.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = oTransReq.Lines.BatchNumbers.Count - 1;
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
                        Response.Redirect("~/Transfer/StartTransferReq.aspx");
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