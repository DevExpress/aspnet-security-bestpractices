<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingBinaryImages.UploadControl" Codebehind="UploadControl.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
   
        <dx:ASPxUploadControl AutoStartUpload="True" ID="ASPxUploadControl1" runat="server" UploadMode="Auto" Width="280px" OnFileUploadComplete="ASPxUploadControl1_FileUploadComplete" ShowUploadButton="True">
            <ValidationSettings AllowedFileExtensions=".jpg, .jpeg">
            </ValidationSettings>
            <AdvancedModeSettings EnableDragAndDrop="True">
            </AdvancedModeSettings>
        </dx:ASPxUploadControl>
    </form>
</body>
</html>
