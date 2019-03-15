<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorMessage.aspx.cs" Inherits="SecurityBestPractices.InformationExposure.ErrorMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Information Exposure Through an Error Message</h2>
        <dx:ASPxTextBox ID="EmailTextBox" runat="server" Caption="Email">
            <ValidationSettings RequiredField-IsRequired="true"></ValidationSettings>
        </dx:ASPxTextBox>
        <br />
        <br />
        <dx:ASPxButton ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" />
        <br />
        <dx:ASPxLabel runat="server" ID="UpdateStatusLabel" Visible="true" />
    </form>
</body>
</html>
