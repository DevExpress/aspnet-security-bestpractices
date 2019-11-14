<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EncodePageTitle.aspx.cs" Inherits="SecurityBestPractices.HtmlEncoding.EncodePageTitle" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Html Encoding in Page.Title</h1>
        <p>The <b>Page.Title</b> property's value is not encoded by default. If you assign a value from a database or user input to this property, you should escape/encode it.</p>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:nwindConnection %>" ProviderName="<%$ ConnectionStrings:nwindConnection.ProviderName %>" SelectCommand="SELECT [ProductID], [ProductName], [UnitPrice] FROM [Products] WHERE [ProductID] = 2">
        </asp:SqlDataSource>
    </form>
</body>
</html>
