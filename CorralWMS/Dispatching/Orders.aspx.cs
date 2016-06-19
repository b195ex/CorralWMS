using CorralWMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CorralWMS.Dispatching
{
    public partial class Orders : System.Web.UI.Page
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
                    try
                    {
                        using (var ctx = new LWMS_Context())
                        {
                            var currOrdr = ctx.Orders.FirstOrDefault(o => o.UserId == usuario.Id && o.TargetEntry == null);
                            if (currOrdr != null)
                            {
                                Session.Add("CurrOrder", currOrdr);
                                ctx.Entry(currOrdr).State = System.Data.Entity.EntityState.Detached;
                                Response.Redirect("~/Dispatching/ScanLoc.aspx", true);
                            }
                            else
                            {
                                string sql = "SELECT DocEntry,DocNum,CardCode,ISNULL(Comments,''),DocDate,DocDueDate FROM ORDR WHERE DocStatus='O' AND DocEntry NOT IN (SELECT DISTINCT DocEntry FROM RDR1 WHERE TrgetEntry IS NOT NULL)";
                                StringBuilder bldr = new StringBuilder(" AND DocEntry NOT IN(");
                                foreach (var ordr in ctx.Orders)
                                {
                                    bldr.Append(ordr.DocEntry).Append(",");
                                }
                                bldr.Replace(',', ')', bldr.Length - 1, 1);
                                if (bldr.Length > 22)
                                    sql += bldr.ToString();
                                using (var cmd = new SqlCommand(sql, new SqlConnection(Tools.Util.GetSapConnStr())))
                                {
                                    cmd.Connection.Open();
                                    var rdr = cmd.ExecuteReader();
                                    Order neuOrder;
                                    while (rdr.Read())
                                    {
                                        neuOrder = new Order();
                                        neuOrder.CardCode = rdr.GetString(2);
                                        neuOrder.Comment = rdr.GetString(3);
                                        neuOrder.DocDate = rdr.GetDateTime(4);
                                        neuOrder.DocEntry = rdr.GetInt32(0);
                                        neuOrder.DocNum = rdr.GetInt32(1);
                                        neuOrder.DueDate = rdr.GetDateTime(5);
                                        neuOrder.OrdrDtls = new HashSet<OrdrDtl>();
                                        ctx.Orders.Add(neuOrder);
                                        string dtlSql = "SELECT LineNum,ItemCode,Quantity FROM RDR1 WHERE DocEntry=" + neuOrder.DocEntry;
                                        using (var dtlCmd = new SqlCommand(dtlSql, new SqlConnection(Tools.Util.GetSapConnStr())))
                                        {
                                            dtlCmd.Connection.Open();
                                            var dtlRdr = dtlCmd.ExecuteReader();
                                            OrdrDtl neuDtl;
                                            while (dtlRdr.Read())
                                            {
                                                neuDtl = new OrdrDtl();
                                                neuDtl.ItemCode = dtlRdr.GetString(1);
                                                neuDtl.LineNum = dtlRdr.GetInt32(0);
                                                neuDtl.Quantity = Convert.ToDouble(dtlRdr.GetDecimal(2));
                                                neuOrder.OrdrDtls.Add(neuDtl);
                                            }
                                        }
                                    }
                                    ctx.SaveChanges();
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

        protected void OrdersGrid_PreRender(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
            if (grid.HeaderRow != null)
                grid.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void OrdersGrid_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            var grid = (GridView)sender;
            var key = (int)grid.DataKeys[e.NewSelectedIndex].Value;
            try
            {
                using (var ctx = new LWMS_Context())
                {
                    var ordr = ctx.Orders.Find(key);
                    if (ordr.UserId == null)
                        ordr.UserId = ((User)Session["LoggedInUser"]).Id;
                    else
                    {
                        grid.DataBind();
                        throw new Exception("Ese pedido está siendo surtido por otro usuario, por favor selecione otro");
                    }
                    ctx.SaveChanges();
                    ctx.Entry(ordr).State = System.Data.Entity.EntityState.Detached;
                    Session.Add("CurrOrder", ordr);
                }
                e.Cancel = true;
                Response.Redirect("~/Dispatching/ScanLoc.aspx");
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