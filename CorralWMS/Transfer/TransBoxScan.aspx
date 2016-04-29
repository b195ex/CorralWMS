<%@ Page Title="Recibir Cajas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TransBoxScan.aspx.cs" Inherits="CorralWMS.Transfer.TransBoxScan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
            <span aria-hidden="true">&times;</span>
        </button>
        <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <asp:HyperLink ID="BackLink" runat="server" CssClass="btn btn-primary" NavigateUrl="~/Transfer/ScanToLocation.aspx">Cambiar Ubicación...</asp:HyperLink>
    <br /><br />
    <div class="row">
        <div class="col-md-6">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="ScanTxt" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="ScanBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="ScanBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <br />
    <div class="table-responsive">
        <asp:GridView ID="BoxGrid" runat="server" CssClass="table table-striped" GridLines="None" OnPreRender="Grid_PreRender" AutoGenerateColumns="False" DataKeyNames="Lote,Caja" DataSourceID="BoxDataSrc">
            <Columns>
                <asp:BoundField DataField="Producto" HeaderText="Producto" SortExpression="Producto" />
                <asp:BoundField DataField="Lote" HeaderText="Lote" ReadOnly="True" SortExpression="Lote" />
                <asp:BoundField DataField="Caja" HeaderText="Caja" ReadOnly="True" SortExpression="Caja" />
                <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" />
                <asp:BoundField DataField="Destino" HeaderText="Destino" SortExpression="Destino" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="BoxDataSrc" runat="server" ConnectionString="Data Source=(localdb)\mssqllocaldb;Initial Catalog=CorralWMS.Entities.LWMS_Context;Integrated Security=True" ProviderName="System.Data.SqlClient" 
            SelectCommand="SELECT T0.ItemCode Producto, T0.Batch Lote, T0.Id Caja, T0.Weight Peso, T2.BinCode Destino
FROM Boxes T0
LEFT JOIN FromLocationBoxes FLB ON FLB.Box_Batch=T0.Batch AND FLB.Box_Id=T0.ID
LEFT JOIN FromLocations T1 
ON FLB.FromLocation_AbsEntry=T1.AbsEntry AND FLB.FromLocation_TransReqId=T1.TransReqId
LEFT JOIN ToLocationBoxes TLB ON TLB.Box_Batch=T0.Batch AND TLB.Box_Id=T0.ID AND TLB.ToLocation_TransferId=FLB.FromLocation_TransReqId
LEFT JOIN ToLocations T2 ON TLB.ToLocation_AbsEntry=T2.AbsEntry AND TLB.ToLocation_TransferId=T2.TransferId AND T1.TransReqId=T2.TransferId
WHERE T1.TransReqId=@ID">
            <SelectParameters>
                <asp:Parameter Name="ID" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
