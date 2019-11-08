<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EncodeHtml.aspx.cs" Inherits="SecurityBestPractices.HtmlEncoding.EncodeHtml" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Html Encoding in Controls</h2>
        <p>Data values are encoded automatically when EncodeHtml = true (for example, the "Produt Name" cell values). UI strings are not encoded (for example, GridView.Caption).</p>
        <dx:ASPxGridView ID="GridView" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" KeyFieldName="ProductID" Caption="<b>Caption</b> text" OnHeaderFilterFillItems="GridView_HeaderFilterFillItems">
            <Settings ShowHeaderFilterButton="true" ShowFooter="true" ShowFilterBar="Visible" />
            <SettingsAdaptivity>
                <AdaptiveDetailLayoutProperties ColCount="1">
                </AdaptiveDetailLayoutProperties>
            </SettingsAdaptivity>
            <SettingsPager PageSize="5">
            </SettingsPager>
            <EditFormLayoutProperties ColCount="1">
            </EditFormLayoutProperties>
            <Columns>
                <dx:GridViewCommandColumn ShowEditButton="True" ShowDeleteButton="true" VisibleIndex="0">
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="ProductID" ReadOnly="True" VisibleIndex="1">
                    <EditFormSettings Visible="False" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ProductName" VisibleIndex="2">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="UnitPrice" VisibleIndex="3">
                </dx:GridViewDataTextColumn>
            </Columns>
            <ClientSideEvents Init="function(s,e){ s.StartEditRow(0); }" />
        </dx:ASPxGridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nwindConnection %>" DeleteCommand="DELETE FROM [Products] WHERE [ProductID] = ?" InsertCommand="INSERT INTO [Products] ([ProductID], [ProductName], [UnitPrice]) VALUES (?, ?, ?)" ProviderName="<%$ ConnectionStrings:nwindConnection.ProviderName %>" SelectCommand="SELECT [ProductID], [ProductName], [UnitPrice] FROM [Products]" UpdateCommand="UPDATE [Products] SET [ProductName] = ?, [UnitPrice] = ? WHERE [ProductID] = ?">
            <DeleteParameters>
                <asp:Parameter Name="ProductID" Type="Int32" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="ProductID" Type="Int32" />
                <asp:Parameter Name="ProductName" Type="String" />
                <asp:Parameter Name="UnitPrice" Type="Decimal" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="ProductName" Type="String" />
                <asp:Parameter Name="UnitPrice" Type="Decimal" />
                <asp:Parameter Name="ProductID" Type="Int32" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </form>
</body>
</html>
