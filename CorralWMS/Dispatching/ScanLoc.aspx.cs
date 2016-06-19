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
    public partial class ScanLoc : System.Web.UI.Page
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
                        OrdrLabel.Text = currOrdr.DocNum.ToString();
                    }
                }
            }
        }

        protected void StartBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text))
                return;
            string sql="SELECT AbsEntry FROM OBIN WHERE SysBin='N' AND BinCode=@BinCode";
            try
            {
                int absentry;
                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                {
                    cmd.Parameters.Add(new SqlParameter("BinCode", BinCodeTxtBox.Text));
                    cmd.Connection.Open();
                    var rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                        throw new Exception("Esa ubicación no existe.");
                    rdr.Read();
                    absentry = rdr.GetInt32(0);
                    cmd.Connection.Close();
                }
                Response.Redirect(string.Format("~/Dispatching/FillOrder.aspx?BinAbs={0}",absentry), true);
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
            var oCompany = Tools.SAPSingleton.oCompany;
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
    }
}