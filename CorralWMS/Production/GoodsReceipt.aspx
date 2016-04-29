<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoodsReceipt.aspx.cs" Inherits="CorralWMS.Production.GoodsReceipt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddPermissionAlert" runat="server">
                <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=AddPermissionAlert.ClientID %>)">
                    <span aria-hidden="true">&times;</span>
                </button>
                <asp:Label ID="AddPermissionExceptionLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="row">
        <div class="form-group col-md-8">
            <label class="control-label">Ordenes de Producción Abiertas:</label>
            <asp:ListBox ID="ProductionOrdersLstBox" runat="server" CssClass="form-control" OnDataBinding="ProductionOrdersLstBox_DataBinding"></asp:ListBox>
        </div>
    </div>
    <div class="row">

    </div>
</asp:Content>
