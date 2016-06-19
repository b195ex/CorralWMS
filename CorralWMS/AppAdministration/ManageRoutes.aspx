<%@ Page Title="Gestión de Rutas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageRoutes.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageRoutes" %>
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
    <div class="row">
        <div class="col-md-4">
            <div class="form-group">
                <label class="control-label" for="RtTxtBox">Ruta:</label>
                <asp:TextBox ID="RtTxtBox" runat="server" CssClass="form-control" autofocus></asp:TextBox>
            </div>
        </div>
    </div>
    <asp:Button ID="AddBtn" runat="server" Text="Crear" CssClass="btn btn-primary" OnClick="AddBtn_Click" OnClientClick="myApp.showPleaseWait();" />
    <div class="table-responsive">
        <asp:GridView ID="RtGrid" runat="server" CssClass="table table-striped" DataSourceID="RtDataSrc" GridLines="None" OnPreRender="RtGrid_PreRender"></asp:GridView>
        <ef:EntityDataSource ID="RtDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Routes"></ef:EntityDataSource>
    </div>
</asp:Content>
