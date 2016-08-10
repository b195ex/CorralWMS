using CorralWMS.Entities;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Dispatching
{
    public partial class FillOrder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.Where(p => p.Id == 34).FirstOrDefault();
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    var currOrdr = (Order)Session["CurrOrder"];
                    if (currOrdr == null)
                    {
                        Response.Redirect("~/Dispatching/Orders.aspx");
                    }
                    else
                    {
                        
                        int entry;
                        if(!int.TryParse(Request.QueryString["BinAbs"], out entry))
                            Response.Redirect("~/Dispatching/ScanLoc.aspx");
                        else if (!IsPostBack)
                        {
                            using (var cmd = new SqlCommand(string.Format("SELECT BinCode FROM OBIN WHERE AbsEntry={0}", entry), new SqlConnection(Tools.Util.GetSapConnStr())))
                            {
                                cmd.Connection.Open();
                                var rdr = cmd.ExecuteReader();
                                if(!rdr.HasRows)
                                    Response.Redirect("~/Dispatching/ScanLoc.aspx");
                                else
                                {
                                    rdr.Read();
                                    BinLabel.Text = rdr.GetString(0);
                                    OrdrLabel.Text = currOrdr.DocNum.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void ScanButton_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(ScanTextBox.Text))
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
                var ordr = (Order)Session["CurrOrder"];
                string whscod;
                string sql = "SELECT WhsCode FROM OBBQ T0 LEFT JOIN OBTN T1 ON T0.SnBMDAbs=T1.AbsEntry WHERE T0.Itemcode=@ItemCode AND DistNumber=@BatchNum AND BinAbs=@BinAbs AND OnHandQty>0";
                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                {
                    cmd.Parameters.Add(new SqlParameter("ItemCode", itmcod));
                    cmd.Parameters.Add(new SqlParameter("BatchNum", batch + boxid.ToString().PadLeft(8, '0')));
                    cmd.Parameters.Add(new SqlParameter("Binabs", Request.QueryString["BinAbs"]));
                    cmd.Connection.Open();
                    var rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                        throw new Exception("Esa caja no se encuentra en la ubicación actual");
                    rdr.Read();
                    whscod = rdr.GetString(0);
                }
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(ordr).State = System.Data.Entity.EntityState.Unchanged;
                    ctx.Entry(ordr).Collection("OrdrDtls").Load();
                    var dtl = ordr.OrdrDtls.FirstOrDefault(d => d.ItemCode == itmcod);
                    if (dtl == null)
                        throw new Exception("El pedido no contiene ese artículo");
                    if (dtl.Boxes.Sum(b => b.Weight)+wt > dtl.Quantity)
                        throw new Exception("Ya se sobrepasó la cantidad requerida de este producto.");
                    var xob = ctx.Boxes.FirstOrDefault(b => b.Batch == batch && b.Id == boxid && b.ItemCode == itmcod);
                    if (xob == null)
                    {
                        xob = new Box();
                        xob.Batch = batch;
                        xob.Id = boxid;
                        xob.ItemCode = itmcod;
                        xob.Weight = wt;
                        ctx.Boxes.Add(xob);
                    }
                    dtl.WhsCode = whscod;
                    dtl.Boxes.Add(xob);
                    ctx.SaveChanges();
                }
                ScanTextBox.Text = "";
                DtlsGrid.DataBind();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void InvBtn_Click(object sender, EventArgs e)
        {
            try
            {
                CreateDocument(true);
                Session.Remove("CurrOrder");
                Response.Redirect("~/Dispatching/Orders.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void DlvrBtn_Click(object sender, EventArgs e)
        {
            try
            {
                CreateDocument(false);
                Session.Remove("CurrOrder");
                Response.Redirect("~/Dispatching/Orders.aspx");
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                ExceptionLabel.Text = ex.Message;
                Alert.Attributes["class"] = Alert.Attributes["class"].Replace("collapse", "");
            }
        }

        protected void CreateDocument(bool inv)
        {
            var oCompany=Tools.SAPSingleton.oCompany;
            Documents oDoc;
            if (inv)
                oDoc = oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
            else 
                oDoc = oCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
            var currOrdr = (Order)Session["CurrOrder"];
            oDoc.CardCode = currOrdr.CardCode;
            using (var ctx = new LWMS_Context())
            {
                ctx.Entry(currOrdr).State = System.Data.Entity.EntityState.Unchanged;
                bool firstline = true;
                foreach (var dtl in currOrdr.OrdrDtls)
                {
                    if (firstline)
                        firstline = false;
                    else
                        oDoc.Lines.Add();
                    oDoc.Lines.BaseType = (int)SAPbobsCOM.BoObjectTypes.oOrders;
                    oDoc.Lines.BaseEntry = currOrdr.DocEntry;
                    oDoc.Lines.BaseLine = dtl.LineNum;
                    oDoc.Lines.ItemCode = dtl.ItemCode;
                    oDoc.Lines.WarehouseCode = dtl.WhsCode;
                    bool firstbatch = true;
                    foreach (var box in dtl.Boxes)
                    {
                        if (firstbatch)
                            firstbatch = false;
                        else
                            oDoc.Lines.BatchNumbers.Add();
                        oDoc.Lines.BatchNumbers.BatchNumber = box.SAPBatch;
                        oDoc.Lines.BatchNumbers.Quantity = box.Weight;
                        oDoc.Lines.Quantity += box.Weight;
                    }
                }
            }
            if (oDoc.Add() != 0)
            {
                int errcod;
                string errmsg;
                oCompany.GetLastError(out errcod, out errmsg);
                throw new Exception(string.Format("Error al crear documento en SAP; {0}:{1}", errcod, errmsg));
            }
            else
            {
                int key = int.Parse(oCompany.GetNewObjectKey());
                Documents doc = oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                doc.GetByKey(key);
                using (var ctx = new LWMS_Context())
                {
                    ctx.Entry(currOrdr).State = System.Data.Entity.EntityState.Unchanged;
                    currOrdr.TargetEntry = doc.DocEntry;
                    currOrdr.TargetRef = doc.DocNum;
                    //currOrdr.TargetType = doc.DocObjectCode;
                    ctx.SaveChanges();
                }
                
            }
        }

        protected void DtlsDataSrc_ContextCreating(object sender, Microsoft.AspNet.EntityDataSource.EntityDataSourceContextCreatingEventArgs e)
        {
            if (IsPostBack)
                return;
            var param = new Parameter();
            param.DbType = System.Data.DbType.Int32;
            param.Name = "DocEntry";
            param.DefaultValue = ((Order)Session["CurrOrder"]).DocEntry.ToString();
            DtlsDataSrc.WhereParameters.Add(param);
        }
    }
}