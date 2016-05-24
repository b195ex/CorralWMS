<%@ Page Title="Ingreso de Producto Terminado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GoodsReceipt.aspx.cs" Inherits="CorralWMS.Production.GoodsReceipt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function CloseAlert(elem) {
            elem.classList.add("collapse");
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="alert alert-danger alert-dismissable collapse" role="alert" id="Alert" runat="server">
                <button type="button" class="close" aria-label="Close" onclick="CloseAlert(<%=Alert.ClientID %>)">
                    <span aria-hidden="true">&times;</span>
                </button>
                <asp:Label ID="ExceptionLabel" runat="server" Text="Label"></asp:Label>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="ProdOrdrGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped" DataKeyNames="DocNum" DataSourceID="ProdOrdrDataSrc" GridLines="None" OnPreRender="ProdOrdrGrid_PreRender" OnSelectedIndexChanging="ProdOrdrGrid_SelectedIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="DocNum" HeaderText="# Orden" />
                        <asp:BoundField DataField="ItemCode" HeaderText="Cod. Artículo" />
                        <asp:BoundField DataField="ItemName" HeaderText="Artículo" />
                        <asp:BoundField DataField="RlsDate" DataFormatString="{0:d}" HeaderText="Fecha Liberación" />
                        <asp:BoundField DataField="DueDate" DataFormatString="{0:d}" HeaderText="Fecha Terminación" />
                        <asp:BoundField DataField="Plannedqty" DataFormatString="{0:N2}" HeaderText="Cant. Planeada" />
                        <asp:BoundField DataField="CmpltQty" DataFormatString="{0:N2}" HeaderText="Cant. Completada" />
                        <asp:BoundField DataField="RjctQty" DataFormatString="{0:N2}" HeaderText="Cant. Rechazada" />
                        <asp:BoundField DataField="Comments" HeaderText="Comentario" />
                        <asp:CommandField ShowSelectButton="True">
                        <ControlStyle CssClass="btn btn-default" />
                        </asp:CommandField>
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="ProdOrdrDataSrc" runat="server" 
                    SelectCommand="SELECT OWOR.DocEntry,DocNum,OWOR.ItemCode,ItemName,PlannedQty,CmpltQty,RjctQty,DueDate,Comments,RlsDate FROM OWOR LEFT JOIN OITM ON OITM.ItemCode=OWOR.ItemCode WHERE Status='R'">
                </asp:SqlDataSource>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="StartBtn" runat="server" Text="Ingreso sin OP" CssClass="btn btn-primary" OnClick="StartBtn_Click" />
</asp:Content>
