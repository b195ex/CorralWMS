<%@ Page Title="Avance de Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LocInvReport.aspx.cs" Inherits="CorralWMS.Inventories.LocInvReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="table-responsive">
        <asp:GridView ID="LocInvGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataKeyNames="WhsinvId, BinAbs" DataSourceID="LocInvDataSrc" GridLines="None" OnPreRender="LocInvGrid_PreRender">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="WhsinvId, BinAbs" DataNavigateUrlFormatString="~/Inventories/LocInvDtl.aspx?InvId={0}&BinAbs={1}" DataTextField="BinCode" HeaderText="Ubicación" />
                <asp:BoundField DataField="User.Name" HeaderText="Usuario" />
                <asp:BoundField DataField="StartDate" HeaderText="Inicio" />
                <asp:BoundField DataField="EndDate" HeaderText="Final" />
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="LocInvDataSrc" runat="server" AutoGenerateWhereClause="true" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="LocInvs" Include="User">
            <WhereParameters>
                <asp:QueryStringParameter DbType="Int32" Name="WhsinvId" QueryStringField="Id" />
            </WhereParameters>
        </ef:EntityDataSource>
    </div>
</asp:Content>
