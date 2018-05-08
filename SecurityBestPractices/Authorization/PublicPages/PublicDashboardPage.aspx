<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicDashboardPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.PublicPages.PublicDashboardPage" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.DashboardWeb" Assembly="DevExpress.Dashboard.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>

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
