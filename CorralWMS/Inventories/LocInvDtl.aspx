<%@ Page Title="Resultado de Inventario de Ubicación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LocInvDtl.aspx.cs" Inherits="CorralWMS.Inventories.LocInvDtl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="table-responsive">
        <asp:GridView ID="BoxesGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="BoxesDataSrc" GridLines="None" OnPreRender="BoxesGrid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Id" HeaderText="#Caja" />
                <asp:BoundField DataField="Weight" DataFormatString="{0:N2}" HeaderText="Peso" />
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="BoxesDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Boxes" 
            Where="EXISTS(SELECT VALUE p FROM it.LocInvs AS p WHERE p.WhsinvId==@WhsinvID && p.BinAbs==@BinAbs)" OrderBy="it.ItemCode">
            <WhereParameters>
                <asp:QueryStringParameter DbType="Int32" Name="WhsinvId" QueryStringField="InvId" />
                <asp:QueryStringParameter DbType="Int32" Name="BinAbs" QueryStringField="BinAbs" />
            </WhereParameters>
        </ef:EntityDataSource>
    </div>
</asp:Content>
