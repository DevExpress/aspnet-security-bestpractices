<%@ Page Title="Validate Input - General" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="SvgInline.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.SvgInline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>Inline SVG images are potentionaly unsecure, display such images only from trusted sources!</p>
    <p>In this example, SVG bytes uploaded by an end user are stored in the data source and embedded inline. As the result, JavaScript is excecuted in the client browser:</p>
    <div id="svgInlineImageContainer" runat="server"></div>
</asp:Content>
