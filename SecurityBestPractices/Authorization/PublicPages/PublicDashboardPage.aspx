<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicDashboardPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.PublicPages.PublicDashboardPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Public Dashboard</title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxDashboard ID="PublicDashboard" runat="server" WorkingMode="ViewerOnly" ClientInstanceName="publicDashboard">
        </dx:ASPxDashboard>
    </form>
</body>
</html>
