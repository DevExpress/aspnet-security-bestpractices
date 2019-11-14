<%@ Page Title="Validate Input - General" Language="C#" MasterPageFile="~/UsingAntiForgeryToken/MasterPageWithAntiForgeryToken.Master" AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="SecurityBestPractices.UsingAntiForgeryToken.GeneralEditForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>General</h2>
    <dx:ASPxSpinEdit ID="someEdit" runat="server" Caption="SomeEdit" MaxLength="1">
        <ValidationSettings RequiredField-IsRequired="true" />
    </dx:ASPxSpinEdit>

    <dx:ASPxSpinEdit ID="spinEdit" runat="server" Caption="SpinEdit" MinValue="1" MaxValue="10">
    </dx:ASPxSpinEdit>
    <br />
    <asp:Button runat="server" Text="Standard Button" OnClick="StandardButton_Click" />
    <br />
    <dx:ASPxLabel runat="server" ID="UpdateStatusLabel" />
</asp:Content>
