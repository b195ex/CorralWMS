<%@ Page Title="Iniciar Transferencia" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StartTransferReq.aspx.cs" Inherits="CorralWMS.Transfer.StartTransferReq" %>
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
        <div class="form-group col-md-4">
            <label class="control-label">Bodega Origen:</label>
            <asp:DropDownList ID="FromWhsDropDn" runat="server" CssClass="form-control" DataTextField="WhsName" DataValueField="WhsCode" OnLoad="DropDn_Load"></asp:DropDownList>
        </div>
    </div>
    <div class="row">
        <div class="form-group col-md-4">
            <label class="control-label">Bodega Destino:</label>
            <asp:DropDownList ID="ToWhsDropDn" runat="server" CssClass="form-control" DataTextField="WhsName" DataValueField="WhsCode" OnLoad="DropDn_Load"></asp:DropDownList>
        </div>
    </div>
    <asp:Button ID="Button1" runat="server" Text="Crear" CssClass="btn btn-primary" OnClick="Button1_Click" />
</asp:Content>
