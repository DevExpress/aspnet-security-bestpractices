<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingFiles.UploadControlTempFileName" Codebehind="UploadControlTempFileName.aspx.cs" %>

<%@ Register assembly="DevExpress.Web.v17.2" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxUploadControl ID="uploadControl" runat="server" OnFilesUploadComplete="uploadControl_FilesUploadComplete" ShowProgressPanel="True" ShowUploadButton="True" UploadMode="Auto" Width="280px">
            </dx:ASPxUploadControl>
        </div>
    </form>
</body>
</html>
