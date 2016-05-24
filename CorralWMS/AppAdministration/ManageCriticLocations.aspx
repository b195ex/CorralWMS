<%@ Page Title="Gestión de Ubicaciones Críticas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCriticLocations.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageCriticLocations" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <%--<script>
        $(document).ready(function () {
            $("#<%=txtSearch.ClientID%>").autocomplete('BinCodSrch.ashx');
        });
    </script>--%>
    <asp:Panel ID="AddPanel" runat="server" CssClass="form-horizontal" GroupingText="Creación de Ubicación Crítica" OnLoad="AddPanel_Load" reqperm="26">
        <asp:UpdatePanel ID="AddLocationPanel" runat="server">
            <ContentTemplate>
                <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddAlert" runat="server">
                    <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=AddAlert.ClientID %>)">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="AddExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Ubicación:</label>
                    <div class="col-md-4">
                        <asp:DropDownList ID="BinCodDropDn" runat="server" CssClass="form-control" DataSourceID="BinDataSrc" DataTextField="BinCode" DataValueField="AbsEntry"></asp:DropDownList>
                        <asp:SqlDataSource ID="BinDataSrc" runat="server" ConnectionString="<%# CorralWMS.Tools.Util.GetSapConnStr() %>" SelectCommand="SELECT AbsEntry,BinCode FROM OBIN WHERE SysBin='N' ORDER BY BinCode" OnLoad="WhsDataSrc_Load">
                        </asp:SqlDataSource>
                        <%--<asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>--%>
                    </div>
                    <%--<div class="col-md-4">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSearch" ForeColor="Red">*Ingrese la Ubicación</asp:RequiredFieldValidator>
                    </div>--%>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Prioridad:</label>
                    <div class="col-md-4">
                        <asp:TextBox ID="PriorityTxtBox" runat="server" TextMode="Number" CssClass="form-control" ValidationGroup="add"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="PriorityTxtBox" Display="Dynamic" ForeColor="Red" ValidationExpression="[1-9][0-9]*" ValidationGroup="add">*Ingrese un número entero positivo.</asp:RegularExpressionValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="PriorityTxtBox" ForeColor="Red" ValidationGroup="add">*Ingrese la prioridad</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-4 col-md-offset-3">
                        <asp:Button ID="AddBtn" runat="server" Text="Agregar" CssClass="btn btn-primary" OnClick="AddBtn_Click" ValidationGroup="add" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="table-responsive">
                <asp:GridView ID="CritLocationsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataKeyNames="BinEntry" DataSourceID="CritLocDataSrc" GridLines="None" OnLoad="CritLocationsGrid_Load" OnPreRender="CritLocationsGrid_PreRender" delperm="28" reqperm="27">
                    <Columns>
                        <asp:BoundField DataField="BinCode" HeaderText="Ubicación" />
                        <asp:BoundField DataField="Priority" HeaderText="Prioridad" />
                    </Columns>
                </asp:GridView>
                <ef:EntityDataSource ID="CritLocDataSrc" runat="server" EnableUpdate="true" EnableDelete="true" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="CriticLocations" OrderBy="it.Priority"></ef:EntityDataSource>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AddBtn" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
