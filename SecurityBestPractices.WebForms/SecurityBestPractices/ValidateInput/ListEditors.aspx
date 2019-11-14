<%@ Page Title="Validate Input - List Editors" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="ListEditors.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.ListEditors" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>List Editors</h2>
    <p>DataSecurityMode</p>
    <dx:ASPxComboBox runat="server" ID="ComboBoxInStrictMode" ValueField="CategoryID" TextField="CategoryName" DataSecurityMode="Strict">
    </dx:ASPxComboBox>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:nwindConnection %>' ProviderName='<%$ ConnectionStrings:nwindConnection.ProviderName %>' SelectCommand="SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]"></asp:SqlDataSource>

    <br />
    <dx:ASPxButton ID="UpdateButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
    <br />
    <dx:ASPxLabel runat="server" ID="UpdateStatusLabel" />
</asp:Content>
