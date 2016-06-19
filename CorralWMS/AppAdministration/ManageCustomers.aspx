<%@ Page Title="Gestión de Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCustomers.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageCustomers" %>
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
    <asp:Button ID="SyncBtn" runat="server" Text="Sincronizar Lista de Clientes" CssClass="btn btn-default" OnClick="SyncBtn_Click" />
    <asp:Panel ID="Panel1" runat="server">
        <div class="table-responsive">
            <asp:GridView ID="ClientsGrid" runat="server" CssClass="table table-striped" DataSourceID="ClientsDataSrc" GridLines="None" AutoGenerateEditButton="true" AutoGenerateColumns="False" OnPreRender="ClientsGrid_PreRender">
                <Columns>
                    <asp:BoundField DataField="CardCode" HeaderText="Código" ReadOnly="true" />
                    <asp:BoundField DataField="CardName" HeaderText="Nombre" ReadOnly="true" />
                    <asp:TemplateField HeaderText="Ruta">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RouteID") %>' Visible="false"></asp:TextBox>
                            <asp:DropDownList ID="RouteDropDn" runat="server" AutoPostBack="true" CssClass="form-control" DataSourceID="RouteDataSrc" DataTextField="RouteName" DataValueField="Id" OnSelectedIndexChanged="RouteDropDn_SelectedIndexChanged" OnDataBound="RouteDropDn_DataBound"></asp:DropDownList>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Route.RouteName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <ef:EntityDataSource ID="RouteDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Routes"></ef:EntityDataSource>
        <ef:EntityDataSource ID="ClientsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Clients" EnableUpdate="true" Include="Route"></ef:EntityDataSource>
    </asp:Panel>
</asp:Content>
