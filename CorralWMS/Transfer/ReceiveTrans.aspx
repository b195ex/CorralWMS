<%@ Page Title="Recepción de Envío" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReceiveTrans.aspx.cs" Inherits="CorralWMS.Transfer.ReceiveTrans" %>
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
    <div class="table-responsive">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" DataKeyNames="Id" DataSourceID="ReqDataSrc" GridLines="None" OnRowDataBound="GridView1_RowDataBound" OnPreRender="Grid_PreRender" OnSelectedIndexChanging="GridView1_SelectedIndexChanging">
            <Columns>
                <asp:BoundField DataField="DocEntry" HeaderText="Documento" SortExpression="DocEntry" />
                <asp:BoundField DataField="FromWhs" HeaderText="Origen" SortExpression="FromWhs" />
                <asp:BoundField DataField="ToWhs" HeaderText="Destino" SortExpression="ToWhs" />
                <asp:TemplateField HeaderText="Detalle">
                    <ItemTemplate>
                        <asp:GridView ID="DtlGrid" runat="server" CssClass="table table-condensed table-striped" AutoGenerateColumns="False" DataSourceID="DtlDataSrc" GridLines="None" OnPreRender="Grid_PreRender">
                            <Columns>
                                <asp:BoundField DataField="ItemCode" HeaderText="Producto" SortExpression="ItemCode" />
                                <asp:BoundField DataField="WT" HeaderText="Libras" ReadOnly="True" SortExpression="WT" />
                            </Columns>
                        </asp:GridView>
                        <asp:SqlDataSource ID="DtlDataSrc" runat="server" 
                            ConnectionString="Data Source=(localdb)\mssqllocaldb;Initial Catalog=CorralWMS.Entities.LWMS_Context;Integrated Security=True" ProviderName="System.Data.SqlClient" 
                            SelectCommand="SELECT T4.ItemCode, SUM(T4.Weight) WT FROM FromLocations T2
                                LEFT JOIN FromLocationBoxes T3 ON T2.AbsEntry=T3.FromLocation_AbsEntry AND T2.TransReqId=T3.FromLocation_TransReqId
                                LEFT JOIN Boxes T4 ON T4.Batch=T3.Box_Batch AND T3.Box_Id=T4.Id AND T4.ItemCode=t3.Box_ItemCode WHERE T2.TransReqId=@TransReqID GROUP BY T4.ItemCode">
                            <SelectParameters>
                                <asp:Parameter DbType="Int32" Name="TransReqID" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowSelectButton="true" SelectText="Recibir" ControlStyle-CssClass="btn btn-default"></asp:CommandField>
            </Columns>
        </asp:GridView>
        <asp:SqlDataSource ID="ReqDataSrc" runat="server" ConnectionString="Data Source=(localdb)\mssqllocaldb;Initial Catalog=CorralWMS.Entities.LWMS_Context;Integrated Security=True" 
            ProviderName="System.Data.SqlClient" SelectCommand="SELECT T0.Id, T0.DocEntry, T0.FromWhs, T0.ToWhs FROM TransReqs T0 LEFT JOIN Transfers T1 ON T0.Id=T1.Id 
                WHERE T1.Id IS NULL AND T0.DocEntry IS NOT NULL">
        </asp:SqlDataSource>
    </div>
</asp:Content>
