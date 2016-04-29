<%@ Page Title="Listas de Correo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageMailingLists.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageMailingLists" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <asp:Panel ID="AddListPanel" runat="server" GroupingText="Creación de lista" CssClass="form-horizontal" reqperm="21" OnLoad="Control_Load">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddAlert" runat="server">
                    <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=AddAlert.ClientID %>)">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="AddExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-2">Nombre:</label>
                    <div class="col-md-4">
                        <asp:TextBox ID="NameTxtBox" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-4">
                        <asp:Button ID="AddBtn" runat="server" Text="Agregar" CssClass="btn btn-primary" OnClick="AddBtn_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <div>
        <ul class="nav nav-tabs">
            <li role="presentation" class="active"><a href="#ver_listas" aria-controls="ver_listas" role="tab" data-toggle="tab">Ver Listas</a></li>
            <li role="presentation" id="EditTab" runat="server" reqperm="22" onload="HtmlControl_Load"><a href="#editar_lista" aria-controls="editar_lista" role="tab" data-toggle="tab">Editar Listas</a></li>
        </ul>
    </div>
    <div class="tab-content">
        <div class="tab-pane active" id="ver_listas" role="tabpanel">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:GridView ID="ListGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataSourceID="ListsDataSrc" GridLines="None" OnPreRender="Grid_PreRender" OnRowDataBound="ListGrid_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Nombre" />
                                <asp:TemplateField HeaderText="Destinatarios">
                                    <ItemTemplate>
                                        <div class="table-responsive">
                                            <asp:GridView ID="RecipientsGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-striped" GridLines="None" OnPreRender="Grid_PreRender">
                                                <Columns>
                                                    <asp:BoundField DataField="Name" HeaderText="Nombre" />
                                                    <asp:BoundField DataField="Email" HeaderText="Correo" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <ef:EntityDataSource ID="ListsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="MailingLists" Include="Recipients"></ef:EntityDataSource>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="SavBtn" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="editar_lista" role="tabpanel">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <div class="form-group col-md-4">
                            <label class="control-label">Lista:</label>
                            <asp:DropDownList ID="ListDropDn" runat="server" CssClass="form-control" DataSourceID="ListsDataSrc" AutoPostBack="True" DataTextField="Name" DataValueField="Id" OnSelectedIndexChanged="ListDropDn_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="SetRecipientsGrid" runat="server" CssClass="table table-striped" DataKeyNames="Id" GridLines="None" OnPreRender="Grid_PreRender" AutoGenerateColumns="False" OnRowDataBound="SetRecipientsGrid_RowDataBound" DataSourceID="UsrsDataSrc">
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="AssignCheck" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FirstName" HeaderText="Nombre" />
                                <asp:BoundField DataField="LastName" HeaderText="Apellido" />
                                <asp:BoundField DataField="Email" HeaderText="Correo" />
                            </Columns>
                        </asp:GridView>
                        <ef:EntityDataSource ID="UsrsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Users" Include="MailingLists"></ef:EntityDataSource>
                    </div>
                    <asp:Button ID="SavBtn" runat="server" Text="Guardar" CssClass="btn btn-default" OnClick="SavBtn_Click" />
                    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="EditAlert" runat="server">
                        <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=EditAlert.ClientID %>)">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <asp:Label ID="EditExceptionLabel" runat="server" Text="Label"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
