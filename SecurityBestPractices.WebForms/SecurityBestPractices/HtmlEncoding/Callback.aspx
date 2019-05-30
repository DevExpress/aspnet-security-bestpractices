<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Callback.aspx.cs" Inherits="SecurityBestPractices.HtmlEncoding.Callback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Html Encoding Callback Result</title>
</head>
<body>
    <form id="form1" runat="server">
        <b>Name:</b> <div id="namePlaceHodler"></div>
        <b>Some Info:</b> <div id="someInfo"></div>
        <hr />
        <dx:ASPxCallback runat="server" ID="CallbackControl" OnCallback="Callback_Callback" ClientInstanceName="callbackControl" >
            <ClientSideEvents CallbackComplete="function(s, e) { 
                document.getElementById('namePlaceHodler').innerHTML = e.result; 
                    if(callbackControl.cpSomeInfo)
                        document.getElementById('someInfo').innerHTML = callbackControl.cpSomeInfo;
                }" />
        </dx:ASPxCallback>
        <dx:ASPxButton ID="NextButton" runat="server" Text="Next >" AutoPostBack="false">
            <ClientSideEvents Click="function(){ callbackControl.PerformCallback('next'); }" />
        </dx:ASPxButton>
    </form>
</body>
</html>
