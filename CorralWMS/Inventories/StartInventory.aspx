<%@ Page Title="Inicio de Inventario General" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StartInventory.aspx.cs" Inherits="CorralWMS.Inventories.StartInventory" %>
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
    <asp:Panel ID="CreatePanel" runat="server" GroupingText="Crear Inventario de Almacén" reqperm="30" OnLoad="CreatePanel_Load">
        <div class="row">
        <div class="col-md-4">
            <div class="form-group">
                <label class="control-label">Seleccione Almacén:</label>
                <asp:DropDownList ID="WhsDropDn" runat="server" CssClass="form-control" DataSourceID="WhsDataSrc" DataTextField="WhsName" DataValueField="WhsCode"></asp:DropDownList>
                <asp:SqlDataSource ID="WhsDataSrc" runat="server" ConnectionString="<%# CorralWMS.Tools.Util.GetSapConnStr() %>" ProviderName="System.Data.SqlClient" SelectCommand="SELECT WhsCode,WhsCode+' - '+WhsName WhsName FROM OWHS" OnLoad="WhsDataSrc_Load"></asp:SqlDataSource>
            </div>
            <asp:Button ID="AddBtn" runat="server" Text="Agregar" CssClass="btn btn-primary" OnClick="Button1_Click" />
        </div>
    </div>
    </asp:Panel>
    <div class="table-responsive">
        <asp:GridView ID="CurrInvGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="InvDataSrc" DataKeyNames="Id" GridLines="None" OnPreRender="CurrInvGrid_PreRender" OnSelectedIndexChanging="CurrInvGrid_SelectedIndexChanging">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="Id" DataNavigateUrlFormatString="~/Inventories/LocInvReport.aspx?Id={0}" DataTextField="WhsName" HeaderText="Almacén" />
                <asp:BoundField DataField="StartDate" DataFormatString="{0:d}" HeaderText="Fecha de Inicio" />
                <asp:BoundField DataField="User.Name" HeaderText="Coordinador" />
                <asp:CommandField ButtonType="Button" SelectText="Cerrar" ShowSelectButton="True">
                    <ControlStyle CssClass="btn btn-primary" />
                </asp:CommandField>
            </Columns>
        </asp:GridView>
        <ef:EntityDataSource ID="InvDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="WhsInvs" Include="User" Where="it.EndDate IS NULL" OnLoad="InvDataSrc_Load"></ef:EntityDataSource>
    </div>
</asp:Content>
