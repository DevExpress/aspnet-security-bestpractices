<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingBinaryImages.BinaryImage" Codebehind="BinaryImage.aspx.cs" %>

<%@ Register assembly="DevExpress.Web.v17.2" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxBinaryImage ID="ASPxBinaryImage1" runat="server">
            <EditingSettings Enabled="True">
            </EditingSettings>
        </dx:ASPxBinaryImage>
        <dx:ASPxButton ID="ASPxButton1" runat="server" OnClick="ASPxButton1_Click" Text="Save">
        </dx:ASPxButton>
    </form>
</body>
</html>
