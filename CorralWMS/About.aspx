<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="CorralWMS.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Lynx WMS.</h3>
    <div class="row">
        <div class="col-md-6">
            <asp:Image ImageUrl="~/Images/lynxLogo.png" runat="server" CssClass="img-responsive" />
        </div>
        <div class="col-md-6">
            <asp:Image ImageUrl="~/Images/DelCorralLogo.png" runat="server" CssClass="img-responsive" />
        </div>
    </div>
    
    <p>Este producto está licenciado para su uso por Agroindustrias "El Corral".</p>
    <p>Está prohibida la distribucuón total o parcial de este software a terceros sin autorización expresa de Lynx Consulting</p>
</asp:Content>
