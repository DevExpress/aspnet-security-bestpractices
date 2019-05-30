<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OfficeControlsFileOperations.aspx.cs" Inherits="SecurityBestPractices.ClientSideApi.OfficeControlsFileOperations" %>

<%@ Register Assembly="DevExpress.Web.ASPxRichEdit.v18.2, Version=18.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRichEdit" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.ASPxSpreadsheet.v18.2, Version=18.2.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxSpreadsheet" TagPrefix="dx" %>

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
