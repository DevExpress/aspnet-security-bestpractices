<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SecurityBestPractices._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>DevExpress ASP.NET Security Best Practices (WebForms)</h1>
        <dx:ASPxSiteMapControl runat="server" DataSourceID="SiteMapDataSource" Border-BorderStyle="None">
        </dx:ASPxSiteMapControl>
        <dx:ASPxSiteMapDataSource ShowStartingNode="false" runat="server" ID="SiteMapDataSource" SiteMapFileName="~/web.sitemap"></dx:ASPxSiteMapDataSource>
    </form>
</body>
</html>
