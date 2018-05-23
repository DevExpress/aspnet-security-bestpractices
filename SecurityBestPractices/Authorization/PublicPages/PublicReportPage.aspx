<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PublicReportPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.PublicPages.PublicReportPage" %>

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
