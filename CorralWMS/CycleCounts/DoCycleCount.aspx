<%@ Page Title="Inventario Cíclico" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DoCycleCount.aspx.cs" Inherits="CorralWMS.CycleCounts.DoCycleCount" %>
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
    <div class="page-header">
        <h2>Por favor proceda a la ubicación <asp:Label ID="BinCodeLabel" runat="server" Text="[Ubicación]"></asp:Label></h2>
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
        <asp:GridView ID="BoxesGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataKeyNames="Batch,Id,ItemCode" GridLines="None" DataSourceID="BoxesDataSrc" OnPreRender="BoxesGrid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Id" HeaderText="#Caja" />
                <asp:BoundField DataField="Weight" DataFormatString="{0:N2}" HeaderText="Peso" />
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="BoxesDataSrc" runat="server" ConnectionString="<%#CorralWMS.Tools.Util.GetCtxConnStr() %>" OnLoad="BoxesDataSrc_Load" 
            SelectCommand="SELECT B.* FROM Boxes B INNER JOIN CycleCountBoxes CCB ON B.Batch=CCB.Box_Batch AND B.Id=CCB.Box_Id AND B.ItemCode=CCB.Box_ItemCode INNER JOIN CycleCounts CC ON CCB.CycleCount_BinEntry=CC.BinEntry AND CCB.CycleCount_Date=CC.Date WHERE CC.BinEntry=@BinEntry and CC.Date=@Date">
            <SelectParameters>
                <asp:Parameter Name="BinEntry" Type="Int32" />
                <asp:Parameter Name="Date" Type="DateTime" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
    <asp:Button ID="EndBtn" runat="server" Text="Cerrar Ubicación" CssClass="btn btn-primary" OnClick="EndBtn_Click" />
</asp:Content>
