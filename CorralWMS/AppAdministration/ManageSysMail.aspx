<%@ Page Title="Gestión de Correo del Sistema" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageSysMail.aspx.cs" Inherits="CorralWMS.AppAdministration.ManageSysMail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <asp:Panel ID="NewMailPanel" runat="server" CssClass="form-horizontal" GroupingText="Agregar Nuevo Correo de Sistema" OnLoad="Control_Load" reqperm="17">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="alert alert-danger alert-dismissable collapse" role="alert" id="AddAlert" runat="server">
                    <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=AddAlert.ClientID %>)">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <asp:Label ID="AddExceptionLabel" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Dirección de Correo:</label>
                    <div class="col-md-5">
                        <asp:TextBox ID="FromAddressTxt" runat="server" CssClass="form-control" ValidationGroup="AddMail"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RegularExpressionValidator ID="FromRegExValidator" runat="server" ControlToValidate="FromAddressTxt" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="AddMail">*Ingrese una dirección válida</asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Contraseña:</label>
                    <div class="col-md-5">
                        <asp:TextBox ID="PwdTxt" runat="server" CssClass="form-control" ValidationGroup="AddMail"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RequiredFieldValidator ID="PwdRqrdValidator" runat="server" ControlToValidate="PwdTxt" ForeColor="Red" ValidationGroup="AddMail">*Debe ingresar una contraseña</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Host de Correo SMTP:</label>
                    <div class="col-md-5">
                        <asp:TextBox ID="HostTxtBox" runat="server" CssClass="form-control" ValidationGroup="AddMail"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RequiredFieldValidator ID="HostRqrdValidator" runat="server" ControlToValidate="HostTxtBox" ForeColor="Red" ValidationGroup="AddMail">*Se requiere un servidor SMTP</asp:RequiredFieldValidator>
                    </div>
                </div>
                <div class="form-group">
                    <label class="control-label col-md-3">Puerto SMTP:</label>
                    <div class="col-md-5">
                        <asp:TextBox ID="PortTxtBox" runat="server" CssClass="form-control" TextMode="Number" ValidationGroup="AddMail"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <asp:RegularExpressionValidator ID="PortRegExValidator" runat="server" ControlToValidate="PortTxtBox" ForeColor="Red" ValidationExpression="[0-9]*[1-9][0-9]*" ValidationGroup="AddMail">*Se requiere un puerto válido</asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-5">
                        <asp:Button ID="AddBtn" runat="server" Text="Agregar" CssClass="btn btn-primary" OnClick="AddBtn_Click" ValidationGroup="AddMail" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="table-responsive">
                <asp:DetailsView ID="MailSettingView" runat="server" AllowPaging="True" AutoGenerateRows="False" CssClass="table table-striped" DataKeyNames="Id" DataSourceID="MailSettingDataSrc" GridLines="None" OnLoad="MailSettingView_Load" reqperm="18">
                    <Fields>
                        <asp:TemplateField HeaderText="Dirección">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("FromAddress") %>' ValidationGroup="Edit"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TextBox1" ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="Edit">*Ingrese una dirección válida</asp:RegularExpressionValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("FromAddress") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contraseña">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("FromPass") %>' ValidationGroup="Edit"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox2" ForeColor="Red">*Se requiere una Contraseña</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("FromPass") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Dirección de Host">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("MailHost") %>' ValidationGroup="Edit"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox3" ForeColor="Red" ValidationGroup="Edit">*Se requiere una dirección de host</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("MailHost") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Puerto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("MailPort") %>' ValidationGroup="Edit"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox4" ForeColor="Red" ValidationGroup="Edit">*Ingrese un numero de puerto válido</asp:RegularExpressionValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("MailPort") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <ef:EntityDataSource ID="MailSettingDataSrc" runat="server" ContextTypeName="CorralWMS.Entities.LWMS_Context" EnableUpdate="true" EntitySetName="MailSettings"></ef:EntityDataSource>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AddBtn" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
