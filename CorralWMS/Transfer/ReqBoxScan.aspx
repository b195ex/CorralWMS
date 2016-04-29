<%@ Page Title="Cajas para traslado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReqBoxScan.aspx.cs" Inherits="CorralWMS.Transfer.ReqBoxScan" %>
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
    <asp:HyperLink ID="BackLink" runat="server" CssClass="btn btn-primary" NavigateUrl="~/Transfer/ScanFromLocation.aspx">Cambiar Ubicación...</asp:HyperLink>
    <asp:Button ID="EndBtn" runat="server" Text="Guardar Traslado" CssClass="btn btn-default pull-right" OnClick="EndBtn_Click" UseSubmitBehavior="false" />
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
        <asp:GridView ID="BoxGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" GridLines="None" OnPreRender="Grid_PreRender" OnLoad="BoxGrid_Load">
            <Columns>
                <asp:BoundField DataField="BinCode" HeaderText="Ubicación" />
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Box_Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Box_Id" HeaderText="#Caja" />
                <asp:BoundField DataField="Weight" HeaderText="Peso" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
