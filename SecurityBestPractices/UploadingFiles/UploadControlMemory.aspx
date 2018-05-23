<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingFiles.UploadControlMemory" Codebehind="UploadControlMemory.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxUploadControl ID="uploadControl" runat="server" 
                OnFilesUploadComplete="uploadControl_FilesUploadComplete" ShowProgressPanel="True" ShowUploadButton="True" UploadMode="Auto" Width="280px">
                <AdvancedModeSettings PacketSize="1000000" ></AdvancedModeSettings>
            </dx:ASPxUploadControl>
        </div>
    </form>
</body>
</html>
