<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Templates.aspx.cs" Inherits="SecurityBestPractices.HtmlEncoding.EncodeHtmlInTemplate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Html Encoding in Tempaltes</h2>
        <dx:ASPxDataView ID="ASPxDataView1" runat="server" DataSourceID="SqlDataSource1">
<PagerSettings ShowNumericButtons="False"></PagerSettings>
            <ItemTemplate>
                <b>ProductID</b>: <asp:Label ID="ProductIDLabel" runat="server" Text='<%# Eval("ProductID") %>' /> <br/>
                <b>ProductName</b>: 
                <asp:Label ID="ProductNameLabel" runat="server" Text='<%# System.Web.HttpUtility.HtmlEncode(Eval("ProductName")) %>' />
                <%--<asp:Label ID="Label1" runat="server" Text='<%# Eval("ProductName") %>' />--%>
            </ItemTemplate>
        </dx:ASPxDataView>
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
