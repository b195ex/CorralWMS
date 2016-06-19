<%@ Page Title="Gestión de Roles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageRoles.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageRoles" %>
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
    <asp:Panel ID="AddRolePanel" runat="server" CssClass="form-horizontal" GroupingText="Creación de Rol" OnLoad="Control_Load" reqperm="3">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddRoleAlert" runat="server">
                    <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=AddRoleAlert.ClientID %>)">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="AddRoleExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="form-group">
                    <label for="<%= RoleNameTxt.ClientID %>" class="col-md-2 control-label">Nombre:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="RoleNameTxt" runat="server" CssClass="form-control" placeholder="Nombre" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*Campo requerido" ControlToValidate="RoleNameTxt" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%= DescTxt.ClientID %>" class="col-md-2 control-label">Descripción:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="DescTxt" runat="server" CssClass="form-control" placeholder="Descripción" Rows="4" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="CreateBtn" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-6">
                <asp:Button ID="CreateBtn" runat="server" Text="Crear" CssClass="btn btn-primary" OnClick="CreateBtn_Click" OnClientClick="myApp.showPleaseWait();" />
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="Panel1" runat="server" GroupingText="Edición de Roles">
        <div class="table-responsive">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="alert alert-danger alert-dismissable collapse" role="alert" id="EditRoleAlert" runat="server">
                    <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=EditRoleAlert.ClientID %>)">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="EditRoleExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                    <asp:GridView ID="RolesGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped" GridLines="None" DataKeyNames="Id" DataSourceID="RolesDataSrc" OnLoad="Grid_Load" OnPreRender="Grid_PreRender" reqperm="5">
                        <Columns>
                            <asp:TemplateField HeaderText="Nombre">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RoleName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*Campo requerido" ControlToValidate="TextBox1" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("RoleName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Description" HeaderText="Descripción" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="CreateBtn" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <ef:EntityDataSource ID="RolesDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Roles" EnableUpdate="true" OrderBy="it.RoleName"></ef:EntityDataSource>
        </div>
    </asp:Panel>
    <asp:Panel ID="AssignPermissionsPanel" runat="server" GroupingText="Asignación de Permisos" OnLoad="Control_Load" reqperm="5">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div class="row">
                    <div class="form-group col-md-6">
                        <label class="control-label">Rol:</label>
                        <asp:DropDownList ID="RolesDropDn" runat="server" AutoPostBack="true" CssClass="form-control" DataSourceID="RolesDataSrc" DataTextField="RoleName" DataValueField="Id" OnSelectedIndexChanged="RolesDropDn_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="AssignPermissionsGrid" runat="server" CssClass="table table-striped" DataKeyNames="Id" DataSourceID="PermissionsDataSrc" GridLines="None" OnPreRender="Grid_PreRender" AutoGenerateColumns="False" OnRowDataBound="AssignPermissionsGrid_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox ID="AssignCheck" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Id" HeaderText="Id" />
                            <asp:BoundField DataField="Description" HeaderText="Descripción" />
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Button ID="AssignBtn" runat="server" Text="Guardar Cambios" CssClass="btn btn-default" OnClick="AssignBtn_Click" OnClientClick="myApp.showPleaseWait();" UseSubmitBehavior="false" CausesValidation="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <ef:EntityDataSource ID="PermissionsDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Permissions"></ef:EntityDataSource>
    </asp:Panel>
</asp:Content>
