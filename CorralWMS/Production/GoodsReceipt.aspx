<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoodsReceipt.aspx.cs" Inherits="CorralWMS.Production.GoodsReceipt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="form-group col-md-8">
            <label class="control-label">Ordenes de Producción Abiertas:</label>
            <asp:ListBox ID="ProductionOrdersLstBox" runat="server" CssClass="form-control"></asp:ListBox>
        </div>
    </div>
    <div class="row">

    </div>
</asp:Content>
