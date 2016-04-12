<%@ Page Title="Gestión de Permisos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagePermissions.aspx.cs" Inherits="CorralWMS.AppAdministration.ManagePermissions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="AddPermissionPanel" runat="server" GroupingText="Creación de Permisos" OnLoad="Control_Load" reqperm="2">
        <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddPermissionAlert" runat="server">
            <%--<button type="button" class="close" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>--%>
            <asp:Label ID="AddPermissionExceptionLabel" runat="server" Text="Label"></asp:Label>
        </div>
        <div class="row">
            <div class="form-group col-md-6">
                <label class="control-label" for="<%=PermissionTxt.ClientID %>">Permiso:</label>
                <asp:TextBox ID="PermissionTxt" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
        <asp:Button ID="AddBtn" runat="server" Text="Crear" CssClass="btn btn-primary" OnClick="AddBtn_Click" />
    </asp:Panel>
    <br />
    <asp:Panel ID="EditPemissionPanel" runat="server" GroupingText="Edición de Permisos">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="table-responsive">
                    <asp:GridView ID="PermissionGrid" runat="server" CssClass="table table-striped" DataKeyNames="Id" DataSourceID="PermissionsDataSrc" GridLines="None" OnPreRender="Grid_PreRender" OnLoad="Grid_Load" reqperm="6"></asp:GridView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <ef:EntityDataSource ID="PermissionsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="Permissions"></ef:EntityDataSource>
    </asp:Panel>
</asp:Content>
