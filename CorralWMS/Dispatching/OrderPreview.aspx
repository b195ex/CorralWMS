<%@ Page Title="Detalle de Pedido" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderPreview.aspx.cs" Inherits="CorralWMS.Dispatching.OrderPreview" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="table-responsive">
        <asp:DetailsView ID="OrdrView" runat="server" AutoGenerateRows="false" CssClass="table table-bordered table-condensed table-striped" DataSourceID="OrdrDataSrc" GridLines="None">
            <Fields>
                <asp:BoundField DataField="DocNum" HeaderText="#Pedido" />
                <asp:BoundField DataField="Client.CardName" HeaderText="Cliente" />
                <asp:BoundField DataField="Comment" HeaderText="Comentario" />
                <asp:BoundField DataField="DocDate" DataFormatString="{0:d}" HeaderText="Fecha Creación" />
                <asp:BoundField DataField="DueDate" DataFormatString="{0:d}" HeaderText="Fecha Entrega" />
            </Fields>
        </asp:DetailsView>
        <ef:EntityDataSource ID="OrdrDataSrc" runat="server" AutoGenerateWhereClause="true" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Orders" Include="Client">
            <WhereParameters>
                <asp:QueryStringParameter DbType="Int32" Name="DocEntry" QueryStringField="DocEntry" />
            </WhereParameters>
        </ef:EntityDataSource>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="DtlGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="DtlDataSrc" GridLines="None" OnPreRender="DtlGrid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Cód. Artículo" />
                <asp:BoundField DataField="Item.ItemName" HeaderText="Nom. Artículo" />
                <asp:BoundField DataField="Quantity" HeaderText="Cant. Pedido" />
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="DtlDataSrc" runat="server" AutoGenerateWhereClause="true" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="OrdrDtls" Include="Item">
            <WhereParameters>
                <asp:QueryStringParameter DbType="Int32" Name="DocEntry" QueryStringField="DocEntry" />
            </WhereParameters>
        </ef:EntityDataSource>
    </div>
</asp:Content>
