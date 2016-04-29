<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageUsers" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="AddUserPanel" runat="server" CssClass="form-horizontal" GroupingText="Creación de Usuario" OnLoad="Control_Load" reqperm="1">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddUserAlert" runat="server">
                    <%--<button type="button" class="close" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>--%>
                    <asp:Label ID="AdduserExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="form-group">
                    <label for="<%= UserNameTxt.ClientID %>" class="col-md-2 control-label">Usuario:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="UserNameTxt" runat="server" CssClass="form-control" placeholder="Usuario" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserNameTxt" Display="Dynamic" ForeColor="Red" Text="*Campo requerido."></asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%= PasswordTxt.ClientID %>" class="col-md-2 control-label">Contraseña:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="PasswordTxt" runat="server" CssClass="form-control" pattern=".{8,}" placeholder="Password" required="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="PasswordTxt" Display="Dynamic" ForeColor="Red" Text="*Mínimo 8 caracteres." ValidationExpression="^[\s\S]{8,}$"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=FirstNameTxt.ClientID %>" class="col-md-2 control-label">Nombre:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="FirstNameTxt" runat="server" CssClass="form-control" placeholder="Nombre"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=LastNameTxt.ClientID %>" class="col-md-2 control-label">Apellido:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="LastNameTxt" runat="server" CssClass="form-control" placeholder="Apellido"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <label for="<%=EmailTxt %>" class="col-md-2 control-label">Correo:</label>
                    <div class="col-md-6">
                        <asp:TextBox ID="EmailTxt" runat="server" CssClass="form-control" TextMode="Email" placeholder="correo@ejemplo.com"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="EmailTxt" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*Ingrese una dirección válida</asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-6">
                        <asp:Button ID="CreateBtn" runat="server" Text="Crear" CssClass="btn btn-primary" OnClick="CreateBtn_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:Panel ID="EditUsersPanel" runat="server" GroupingText="Edición de Usuarios">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="table-responsive">
                    <asp:GridView ID="EditUsersGrid" runat="server" CssClass="table table-striped" DataKeyNames="Id" DataSourceID="UsersDataSrc" GridLines="None" OnPreRender="Grid_PreRender" AutoGenerateColumns="False" OnLoad="Grid_Load" reqperm="4">
                        <Columns>
                            <asp:TemplateField HeaderText="Usuario">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("UserName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ForeColor="Red" ErrorMessage="*Campo Requerido" ControlToValidate="TextBox2"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("UserName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Password">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Password") %>' pattern=".{8,}" required="true"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox3" Display="Dynamic" ForeColor="Red" Text="*Mínimo 8 caracteres." ValidationExpression="^[\s\S]{8,}$"></asp:RegularExpressionValidator>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ForeColor="Red" ErrorMessage="*Campo Requerido" ControlToValidate="TextBox3"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Password") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="FirstName" HeaderText="Nombre" />
                            <asp:BoundField DataField="LastName" HeaderText="Apellido" />
                            <asp:TemplateField HeaderText="Correo">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Email") %>' TextMode="Email"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ErrorMessage="*Ingrese un correo válido." ControlToValidate="TextBox1" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:CheckBoxField DataField="Active" HeaderText="Activo" />
                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="CreateBtn" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </asp:Panel>
    <ef:EntityDataSource ID="UsersDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="Users" OrderBy="it.FirstName"></ef:EntityDataSource>
    <asp:Panel ID="AssignRolesPanel" runat="server" GroupingText="Asignación de Roles">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div class="form-group">
                    <label class="control-label" for="<%=UserDropDn.ClientID %>">Usuario</label>
                    <asp:DropDownList ID="UserDropDn" runat="server" AutoPostBack="true" CssClass="form-control" DataSourceID="UsersDataSrc" DataTextField="UserName" DataValueField="Id" OnSelectedIndexChanged="UserDropDn_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="RoleAssignGrid" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" DataKeyNames="Id" DataSourceID="RolesDataSrc" GridLines="None" OnPreRender="Grid_PreRender" OnRowDataBound="RoleAssignGrid_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Asignado">
                                <ItemTemplate>
                                    <asp:CheckBox ID="AssignCheck" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RoleName" HeaderText="Rol" />
                            <asp:BoundField DataField="Description" HeaderText="Descripción" />
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Button ID="AssignBtn" runat="server" Text="Guardar Cambios" CssClass="btn btn-default" OnClick="AssignBtn_Click" UseSubmitBehavior="false" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="CreateBtn" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <ef:EntityDataSource ID="RolesDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EntitySetName="Roles" OrderBy="it.Rolename"></ef:EntityDataSource>
    </asp:Panel>
</asp:Content>
