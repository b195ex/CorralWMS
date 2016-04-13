<%@ Page Title="Parámetros de Conexión con SAP B1" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageSAP.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageSAP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:DetailsView ID="SAPDetails" runat="server" AutoGenerateRows="False" CssClass="table table-striped" DataSourceID="SAPDataSrc" DataKeyNames="id" HeaderStyle-Font-Bold="true" GridLines="None" OnLoad="SAPDetails_Load" reqperm="13">
        <Fields>
            <asp:BoundField DataField="CompanyDB" HeaderText="CompanyDB" />
            <asp:BoundField DataField="DbPassword" HeaderText="DbPassword" />
            <asp:TemplateField HeaderText="DbServerType">
                <EditItemTemplate>
                    <asp:TextBox ID="SrvrTypeTxt" runat="server" Text='<%# Bind("DbServerType") %>' Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="SrvrTypeDropDn" runat="server" OnDataBinding="SrvrTypeDropDn_DataBinding" OnSelectedIndexChanged="SrvrTypeDropDn_SelectedIndexChanged"></asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("DbServerType") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DbUserName" HeaderText="DbUserName" />
            <asp:TemplateField HeaderText="language">
                <EditItemTemplate>
                    <asp:TextBox ID="langTxtBox" runat="server" Text='<%# Bind("language") %>' Visible="false"></asp:TextBox>
                    <asp:DropDownList ID="langDropDn" runat="server" OnDataBinding="langDropDn_DataBinding" OnSelectedIndexChanged="langDropDn_SelectedIndexChanged"></asp:DropDownList>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("language") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Password" HeaderText="Password" />
            <asp:BoundField DataField="Server" HeaderText="Server" />
            <asp:BoundField DataField="UserName" HeaderText="UserName" />
            <asp:CheckBoxField DataField="UseTrusted" HeaderText="UseTrusted" />
        </Fields>
        <HeaderStyle Font-Bold="True"></HeaderStyle>
    </asp:DetailsView>
    <ef:EntityDataSource ID="SAPDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="SapSettings" Where="it.id=1"></ef:EntityDataSource>
</asp:Content>
