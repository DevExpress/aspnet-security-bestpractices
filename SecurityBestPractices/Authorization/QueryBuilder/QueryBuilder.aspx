<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryBuilder.aspx.cs" Inherits="SecurityBestPractices.Authorization.QueryBuilder.WebForm1" %>

<%@ Register assembly="DevExpress.XtraReports.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraReports.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
            <dx:ASPxQueryBuilder ID="ASPxQueryBuilder1"  runat="server"></dx:ASPxQueryBuilder>
    </form>
</body>
</html>
