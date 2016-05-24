using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Inventories
{
    public partial class ScanBin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var usuario = (User)Session["LoggedInUser"];
            if (usuario == null)
                Response.Redirect("~/SignIn.aspx");
            else
            {
                var permisos = (HashSet<Permission>)Session["Permissions"];
                var perm = permisos.FirstOrDefault(p => p.Id == 33);
                if (perm == null)
                {
                    Response.Redirect("~/Default.aspx");
                }
                else
                {
                    using (var ctx = new LWMS_Context())
                    {
                        var currloc = ctx.LocInvs.FirstOrDefault(l => l.EndDate == null && l.UserId == usuario.Id);
                        if (currloc != null)
                        {
                            ctx.Entry(currloc).State = System.Data.Entity.EntityState.Detached;
                            Session.Add("CurrLocInv", currloc);
                            Response.Redirect("~/Inventories/LocInvBoxScan.aspx");
                        }
                    }
                }
            }
        }

        protected void ScanBtn_Click(object sender, EventArgs e)
        {
            if (!Alert.Attributes["class"].Contains("collapse"))
                Alert.Attributes["class"] += "collapse";
            if (string.IsNullOrWhiteSpace(BinCodeTxtBox.Text))
                return;
            var sql="SELECT AbsEntry,WhsCode FROM OBIN WHERE BinCode=@BinCode";
            var param = new SqlParameter("BinCode", BinCodeTxtBox.Text);
            try
            {
                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                {
                    cmd.Parameters.Add(param);
                    cmd.Connection.Open();
                    var rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                    {
                        var msg = string.Format("La ubicación {0} no existe.", BinCodeTxtBox.Text);
                        BinCodeTxtBox.Text = "";
                        BinCodeTxtBox.Focus();
                        throw new Exception(msg);
                    }
                    rdr.Read();
                    using (var ctx = new LWMS_Context())
                    {
                        string tmp=rdr.GetString(1);
                        var currinv = ctx.WhsInvs.FirstOrDefault(i => i.EndDate == null && i.WhsCode == tmp);
                        if (currinv == null)
                        {
                            var msg = string.Format("El Almacén al que pertenece la ubicación {0} no se está inventariando actualmente", BinCodeTxtBox.Text);
                            BinCodeTxtBox.Text = "";
                            BinCodeTxtBox.Focus();
                            throw new Exception(msg);
                        }
                        else
                        {
                            ctx.Entry(currinv).Collection("Locations").Load();
                            var currloc = currinv.Locations.FirstOrDefault(l => l.BinAbs == rdr.GetInt32(0));
                            if (currloc.UserId != null)
                            {
                                if (currloc.EndDate != null)
                                {
                                    var msg = string.Format("La ubicación {0} está siendo inventariada por otro usuario", BinCodeTxtBox.Text);
                                    BinCodeTxtBox.Text = "";
                                    BinCodeTxtBox.Focus();
                                    throw new Exception(msg);
                                }
                                else
                                {
                                    var msg = string.Format("La ubicación {0} ya se inventarió", BinCodeTxtBox.Text);
                                    BinCodeTxtBox.Text = "";
                                    BinCodeTxtBox.Focus();
                                    throw new Exception(msg);
                                }
                            }
                            else
                            {
                                currloc.StartDate = DateTime.Now;
                                currloc.UserId = ((User)Session["LoggedInUser"]).Id;
                                ctx.SaveChanges();
                                ctx.Entry(currloc).State = System.Data.Entity.EntityState.Detached;
                                Session.Add("CurrLocInv", currloc);
                                Response.Redirect("~/Inventories/LocInvBoxScan.aspx", true);
                            }
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
        }
    }
}