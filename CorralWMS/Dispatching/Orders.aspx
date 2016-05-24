<%@ Page Title="Seleccione Pedido" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="CorralWMS.Dispatching.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="table-responsive">
        <asp:GridView ID="OrdersGrid" runat="server"></asp:GridView>
    </div>
</asp:Content>
