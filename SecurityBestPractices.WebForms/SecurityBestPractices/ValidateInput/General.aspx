<%@ Page Title="Validate Input - General" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.GeneralEditForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>General</h2>
    <dx:ASPxSpinEdit ID="spinEdit" runat="server" Caption="SpinEdit" MinValue="1" MaxValue="10" OnValidation="spinEdit_CustomValidation">
        <ValidationSettings RequiredField-IsRequired="true" /> 
    </dx:ASPxSpinEdit>

    <br />
    <dx:ASPxButton ID="UpdateButton" runat="server" Text="Submit" OnClick="UpdateButton_Click" />
    <br />
    <asp:Button runat="server" Text="Submit - Standard Button" OnClick="StandardButton_Click" />
    <br />
    <dx:ASPxLabel runat="server" ID="UpdateStatusLabel" />
</asp:Content>
