<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingFiles.LimitDirectorySize" Codebehind="LimitDirectorySize.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxUploadControl ID="uploadControl" runat="server" OnFilesUploadComplete="uploadControl_FilesUploadComplete" 
                                  ShowProgressPanel="True" ShowUploadButton="True" AutoStartUpload="true" UploadMode="Auto" Width="280px" >
				<%-- The code below prevents uploading executable(aspx) files--%>
				<ValidationSettings AllowedFileExtensions =".jpg,.png">
                </ValidationSettings>
            </dx:ASPxUploadControl>
        </div>
    </form>
</body>
</html>
