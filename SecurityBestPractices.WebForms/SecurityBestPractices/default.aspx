<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SecurityBestPractices._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxSiteMapControl runat="server" DataSourceID="SiteMapDataSource" Font-Size="115%" Theme="" Border-BorderStyle="None" ForeColor="Black">
        </dx:ASPxSiteMapControl>
        <dx:ASPxSiteMapDataSource ShowStartingNode="true" runat="server" ID="SiteMapDataSource" SiteMapFileName="~/web.sitemap"></dx:ASPxSiteMapDataSource>
    </form>
</body>
</html>
