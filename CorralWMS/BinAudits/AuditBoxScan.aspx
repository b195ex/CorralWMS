<%@ Page Title="Auditoria de Ubicación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AuditBoxScan.aspx.cs" Inherits="CorralWMS.BinAudits.AuditBoxScan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
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
        <div class="col-md-6">
            <div class="input-group">
                <div class="input-group-addon"><span class="glyphicon glyphicon-barcode"></span></div>
                <asp:TextBox ID="ScanTxt" runat="server" CssClass="form-control" autofocus></asp:TextBox>
                <span class="input-group-btn">
                    <asp:Button ID="ScanBtn" runat="server" Text="Go!" CssClass="btn btn-default" OnClick="ScanBtn_Click" />
                </span>
            </div>
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="BoxesGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" GridLines="None" OnLoad="BoxesGrid_Load" OnPreRender="Grid_PreRender">
            <Columns>
                <asp:BoundField DataField="ItemCode" HeaderText="Artículo" />
                <asp:BoundField DataField="Batch" HeaderText="Lote" />
                <asp:BoundField DataField="Id" HeaderText="# Caja" />
                <asp:BoundField DataField="Weight" DataFormatString="{0:N2}" HeaderText="Peso" />
            </Columns>
        </asp:GridView>
    </div>
    <asp:Button ID="EndBtn" runat="server" Text="Cerrar Auditoría" CssClass="btn btn-primary" OnClick="EndBtn_Click" />
</asp:Content>
