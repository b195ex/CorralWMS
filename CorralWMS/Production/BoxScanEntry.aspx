<%@ Page Title="Ingresar Cajas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BoxScanEntry.aspx.cs" Inherits="CorralWMS.Production.BoxScanEntry" %>
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
    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
            <span aria-hidden="true">&times;</span>
        </button>
        <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
    </div>
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Production/ScanLocationEntry.aspx">Cambiar Ubicación</asp:HyperLink>
    <div class="row">
        <div class="col-md-4">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="BoxTxtBox" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="StartBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClientClick="myApp.showPleaseWait();" OnClick="StartBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="BoxGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="BoxDataSrc" GridLines="None" OnPreRender="BoxGrid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Id" HeaderText="#Caja" />
                <asp:BoundField DataField="Weight" DataFormatString="{0:N2}" HeaderText="Peso" />
                <asp:BoundField DataField="EntryLocation.BinCode" HeaderText="Ubicación" />
            </Columns>
        </asp:GridView>
    </div>
    <ef:EntityDataSource ID="BoxDataSrc" runat="server" AutoGenerateWhereClause="true" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Boxes" Include="EntryLocation">
        <WhereParameters>
            <asp:Parameter DbType="Int32" Name="ProdEntryID" />
        </WhereParameters>
    </ef:EntityDataSource>
    <asp:Button ID="EndBtn" runat="server" Text="Cerrar Ingreso" CssClass="btn btn-primary" OnClientClick="myApp.showPleaseWait();" OnClick="EndBtn_Click" />
</asp:Content>
