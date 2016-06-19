<%@ Page Title="Despachar Cajas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FillOrder.aspx.cs" Inherits="CorralWMS.Dispatching.FillOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var myApp;
        myApp = myApp || (function () {
            var pleaseWaitDiv = $('<div class="modal hide" id="pleaseWaitDialog" data-backdrop="static" data-keyboard="false"><div class="modal-header"><h1>Processing...</h1></div><div class="modal-body"><div class="progress progress-striped active"><div class="bar" style="width: 100%;"></div></div></div></div>');
            return {
                showPleaseWait: function () {
                    pleaseWaitDiv.modal();
                },
                hidePleaseWait: function () {
                    pleaseWaitDiv.modal('hide');
                },
            };
        })();
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <div class="page-header">
        <label>Pedido: </label>
        <asp:Label ID="OrdrLabel" runat="server" Text="Label"></asp:Label>
        <br />
        <label>Ubicación: </label>
        <asp:Label ID="BinLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
            <span aria-hidden="true">&times;</span>
        </button>
        <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <div class="row">
        <asp:LinkButton ID="BackLinkBtn" runat="server" CssClass="pull-right" PostBackUrl="~/Dispatching/ScanLoc.aspx">Cambiar Ubicación...</asp:LinkButton>
    </div>
        <div class="row">
        <div class="col-md-4">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="ScanTextBox" CssClass="form-control" runat="server" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="ScanButton" runat="server" Text="Go!" CssClass="btn btn-default" OnClientClick="myApp.showPleaseWait();" OnClick="ScanButton_Click" />
                </span>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="DtlsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="DtlsDataSrc" GridLines="None">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Cód. Artículo" />
                <asp:BoundField DataField="Item.ItemName" HeaderText="Articulo" />
                <asp:BoundField DataField="Quantity" HeaderText="Libras Pedido" />
                <asp:TemplateField HeaderText="Libras Factura">
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# ((ICollection<CorralWMS.Entities.Box>)Eval("Boxes")).Sum(b => b.Weight) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="DtlsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableFlattening="false" EntitySetName="OrdrDtls" OnContextCreating="DtlsDataSrc_ContextCreating" Where="it.DocEntry=@DocEntry"></ef:EntityDataSource>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <div class="btn-group" role="group">
                <asp:Button ID="InvBtn" runat="server" Text="Factura" CssClass="btn btn-primary" OnClick="InvBtn_Click" />
                <asp:Button ID="DlvrBtn" runat="server" Text="Entrega" CssClass="btn btn-primary" OnClick="DlvrBtn_Click" />
            </div>
        </div>
    </div>
</asp:Content>

