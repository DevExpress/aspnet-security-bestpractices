<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgegyToken.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <%= System.Web.Helpers.AntiForgery.GetHtml() %>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Delete" />
    </form>
</body>
</html>
