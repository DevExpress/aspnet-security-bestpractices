<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="SecurityBestPractices.HtmlEncoding.General" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <dx:ASPxTextBox ID="SearchBox" runat="server" Width="170px" NullText="Search" HelpText="Type text and press Enter"></dx:ASPxTextBox>

        <p><asp:Literal runat="server" ID="SearchResultLiteral"></asp:Literal></p>

        <p><dx:ASPxLabel runat="server" ID="SearchResultLabel"></dx:ASPxLabel></p>

        <hr />
        <p>Go <a runat="server" id="urlLink">back</a></p>
    </form>
    <script>
        var s = "<%= HttpUtility.JavaScriptStringEncode(SearchBox.Text) %>"; 
        // DoSomething(s);
    </script>
</body>
</html>
