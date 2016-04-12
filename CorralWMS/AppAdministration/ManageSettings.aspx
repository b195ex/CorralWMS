<%@ Page Title="Configuración del Sistema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageSettings.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="AddSettingPanel" runat="server" GroupingText="Agregar Configuración" OnLoad="Control_Load" reqperm="11">
        <div class="form-horizontal">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="form-group">
                        <label class="col-md-2 control-label">Key:</label>
                        <div class="col-md-4">
                            <asp:TextBox ID="KeyTxt" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-md-2 control-label">Value:</label>
                        <div class="col-md-4">
                            <asp:TextBox ID="ValueTxt" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AddBtn" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-4">
                    <asp:Button ID="AddBtn" runat="server" Text="Agregar" CssClass="btn btn-primary" OnClick="AddBtn_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="EditSettingsPanel" runat="server" GroupingText="Editar Configuración">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="SettingsGrid" runat="server" CssClass="table table-striped" DataKeyNames="Key" DataSourceID="SettingsDataSrc" GridLines="None" OnLoad="Grid_Load" OnPreRender="Grid_PreRender" reqperm="12"></asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        <ef:EntityDataSource ID="SettingsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="AppSettings"></ef:EntityDataSource>
    </asp:Panel>
</asp:Content>
