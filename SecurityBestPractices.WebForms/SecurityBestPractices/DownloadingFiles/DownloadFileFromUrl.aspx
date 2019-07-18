<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.DownloadingFiles.DownloadFile" Codebehind="DownloadFileFromUrl.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxBinaryImage ID="BinaryImage" runat="server"></dx:ASPxBinaryImage>
        <dx:ASPxTextBox runat="server" ID="edUrl" Width="400px" Text="https://demos.devexpress.com/ASPxImageAndDataNavigationDemos/Content/Images/widescreen/woman-using-laptop.jpg"></dx:ASPxTextBox>
        <dx:ASPxButton runat="server" Text="Download From Url" OnClick="Download_Click"></dx:ASPxButton>
        <hr />
        <dx:ASPxButton runat="server" Text="Download Confedential Image" OnClick="DownloadConfedentialImage_Click"></dx:ASPxButton>
    </form>
</body>
</html>
