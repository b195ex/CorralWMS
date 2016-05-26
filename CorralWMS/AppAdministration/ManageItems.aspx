<%@ Page Title="Gestión de Artículos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageItems.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageItems" %>
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
    <asp:Button ID="Button1" runat="server" Text="Sincronizar Lista de Artículos" CssClass="btn btn-default" OnClick="Button1_Click" />
    <asp:Panel ID="Panel1" runat="server" GroupingText="Duración de Producto">
        <div class="table-responsive">
            <asp:UpdatePanel ID="DurationUpdtPnl" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="ItemsGrid" runat="server" AutoGenerateEditButton="true" CssClass="table table-striped" DataKeyNames="ItemCode" DataSourceID="ItemsDataSrc" GridLines="None" OnPreRender="ItemsGrid_PreRender" OnRowDataBound="ItemsGrid_RowDataBound"></asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
            <ef:EntityDataSource ID="ItemsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="Items"></ef:EntityDataSource>
        </div>
    </asp:Panel>
</asp:Content>
