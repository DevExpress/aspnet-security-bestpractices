<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicReportPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.PublicPages.PublicReportPage" %>

<%@ Register Assembly="DevExpress.XtraReports.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Public Report</title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxWebDocumentViewer ID="documentViewer" runat="server" ClientInstanceName="documentViewer">
        </dx:ASPxWebDocumentViewer>
    </form>
</body>
</html>
