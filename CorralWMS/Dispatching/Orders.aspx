<%@ Page Title="Seleccione Pedido" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="CorralWMS.Dispatching.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
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
    <div class="table-responsive">
        <asp:GridView ID="OrdersGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped" DataKeyNames="DocEntry" DataSourceID="OrdrsDataSrc" GridLines="None" OnPreRender="OrdersGrid_PreRender" OnSelectedIndexChanging="OrdersGrid_SelectedIndexChanging">
            <Columns>
                <asp:HyperLinkField DataTextField="DocNum" HeaderText="#Pedido" DataNavigateUrlFields="DocNum" DataNavigateUrlFormatString="~/Dispatching/OrderPreview.aspx?DocEntry={0}" />
                <asp:BoundField DataField="Client.CardName" HeaderText="Cliente" />
                <asp:BoundField DataField="DocDate" DataFormatString="{0:d}" HeaderText="Fecha Pedido" />
                <asp:BoundField DataField="DueDate" DataFormatString="{0:d}" HeaderText="Fecha Entrega" />
                <asp:BoundField DataField="Comment" HeaderText="Comentario" />
                <asp:BoundField DataField="Client.Route.RouteName" HeaderText="Ruta" />
                <asp:CommandField ButtonType="Button" ShowSelectButton="True">
                <ControlStyle CssClass="btn btn-primary" />
                </asp:CommandField>
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="OrdrsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Orders" Include="Client, Client.Route" Where="it.UserId IS NULL"></ef:EntityDataSource>
    </div>
</asp:Content>
