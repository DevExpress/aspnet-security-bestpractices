<%@ Page Language="C#" AutoEventWireup="true" Inherits="SecurityBestPractices.UploadingBinaryImages.BinaryImage" Codebehind="BinaryImage.aspx.cs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxBinaryImage ID="ASPxBinaryImage1" runat="server">
            <EditingSettings Enabled="True" UploadSettings-UploadValidationSettings-MaxFileSize="4194304">
            </EditingSettings>
            <ClientSideEvents ValueChanged = "function () { SubmitButton.SetEnabled(true); }" />
        </dx:ASPxBinaryImage>
        <br />
        <dx:ASPxButton ID="SubmitButton" ClientInstanceName="SubmitButton" ClientEnabled="false" runat="server" OnClick="ASPxButton1_Click" Text="Save">
        </dx:ASPxButton>
    </form>
</body>
</html>
