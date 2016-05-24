<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LocInvBoxScan.aspx.cs" Inherits="CorralWMS.Inventories.LocInvBoxScan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <div class="page-header">
        <h1>Inventario de Ubicación <asp:Label ID="BinLbl" runat="server" Text="Label"></asp:Label></h1>
    </div>
    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
            <span aria-hidden="true">&times;</span>
        </button>
        <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <div class="row">
        <div class="col-md-6">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="ScanTxtBox" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="ScanBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="ScanBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="BoxesGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" GridLines="None" DataSourceID="BoxesDataSrc" OnPreRender="BoxesGrid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Id" HeaderText="#Caja" />
                <asp:BoundField DataField="Weight" DataFormatString="{0:N2}" HeaderText="Peso" />
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="BoxesDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Boxes" Include="LocInvs" Where="EXISTS(SELECT VALUE p FROM it.LocInvs AS p WHERE p.WhsinvId=@InvId AND p.BinAbs=@BinAbs)">
            <WhereParameters>
                <asp:Parameter Name="InvId" Type="Int32" />
                <asp:Parameter Name="BinAbs" Type="Int32" />
            </WhereParameters>
        </ef:EntityDataSource>
    </div>
    <asp:Button ID="EndBtn" runat="server" Text="Cerrar Ubicación" CssClass="btn btn-primary" OnClick="EndBtn_Click" />
</asp:Content>
