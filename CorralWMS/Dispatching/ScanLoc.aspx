<%@ Page Title="Seleccione Ubicación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScanLoc.aspx.cs" Inherits="CorralWMS.Dispatching.ScanLoc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <div class="page-header">
        <label>Pedido: </label>
        <asp:Label ID="OrdrLabel" runat="server" Text="Label"></asp:Label>
        <br />
    </div>
    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
            <span aria-hidden="true">&times;</span>
        </button>
        <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <div class="row">
        <div class="col-md-4">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="BinCodeTxtBox" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="StartBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="StartBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <div class="btn-group" role="group">
                <asp:Button ID="InvBtn" runat="server" Text="Factura" CssClass="btn btn-primary" OnClick="InvBtn_Click" />
                <asp:Button ID="DlvrBtn" runat="server" Text="Entrega" CssClass="btn btn-primary" OnClick="DlvrBtn_Click" />
            </div>
        </div>
    </div>
</asp:Content>
