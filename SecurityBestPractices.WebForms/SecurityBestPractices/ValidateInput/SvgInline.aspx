<%@ Page Title="Validate Input - General" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="SvgInline.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.SvgInline" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>Inline Svg images are not secure! You must trust the source!</p>
    <p>For example: Svg bytes uploaded by end user, stored in the data source and embedded inline. JavaScript is excecuted:</p>
    <div id="svgInlineImageContainer" runat="server"></div>
</asp:Content>
