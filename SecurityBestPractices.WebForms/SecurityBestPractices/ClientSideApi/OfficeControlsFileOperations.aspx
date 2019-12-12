<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficeControlsFileOperations.aspx.cs" Inherits="SecurityBestPractices.ClientSideApi.OfficeControlsFileOperations" %>

<%@ Register assembly="DevExpress.Web.ASPxRichEdit.v19.2, Version=19.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxRichEdit" tagprefix="dx" %>
<%@ Register assembly="DevExpress.Web.ASPxSpreadsheet.v19.2, Version=19.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxSpreadsheet" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxSpreadsheet ID="spreadsheet" runat="server" WorkDirectory="~/App_Data/WorkDirectory">
            <Settings>
                <Behavior CreateNew="Hidden" Open="Hidden" Save="Hidden" SaveAs="Hidden" SwitchViewModes="Hidden"/>
            </Settings>
        </dx:ASPxSpreadsheet>
        <hr />
        <dx:ASPxRichEdit ID="richEdit" runat="server" WorkDirectory="~\App_Data\WorkDirectory">
            <Settings>
                <Behavior CreateNew="Hidden" Open="Hidden" Save="Hidden" SaveAs="Hidden" />
            </Settings>
        </dx:ASPxRichEdit>
    </form>
</body>
</html>
