<%@ Page Title="Seleccione Ubicación para Inventariar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScanBin.aspx.cs" Inherits="CorralWMS.Inventories.ScanBin" %>
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
    <div class="row">
        <div class="col-md-4">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="BinCodeTxtBox" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="ScanBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="ScanBtn_Click" />
                </span>
            </div>
        </div>
    </div>
</asp:Content>
