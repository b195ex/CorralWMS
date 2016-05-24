<%@ Page Title="Seleccione Ubicación de Ingreso" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ScanLocationEntry.aspx.cs" Inherits="CorralWMS.Production.ScanLocationEntry" %>
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
                    <asp:Button ID="StartBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="StartBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <asp:Button ID="EndBtn" runat="server" Text="Cerrar Ingreso" CssClass="btn btn-primary" OnClick="EndBtn_Click" />
</asp:Content>
