<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpreadsheetReadingModeOnly.aspx.cs" Inherits="SecurityBestPractices.ClientSideApi.SpreadsheetReadingModeOnly" %>

<%@ Register assembly="DevExpress.Web.ASPxSpreadsheet.v19.2, Version=19.2.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxSpreadsheet" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxSpreadsheet ID="spreadsheet" ClientIDMode="Static" runat="server" WorkDirectory="~/App_Data/WorkDirectory" ReadOnly="true">
            <SettingsView Mode="Reading" />
            <Settings>
                <Behavior SwitchViewModes="Hidden" />
            </Settings>
        </dx:ASPxSpreadsheet>
    </form>
</body>
</html>
