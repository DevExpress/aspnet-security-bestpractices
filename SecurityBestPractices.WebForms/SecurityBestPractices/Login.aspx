<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SecurityBestPractices.Login" %>
<%@ Register Assembly="DevExpress.Web.v18.2" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <dx:ASPxComboBox runat="server" ID="UserName" Caption="UserName" SelectedIndex="0" DropDownStyle="DropDownList" IncrementalFilteringMode="None" >
            <Items>
                <dx:ListEditItem Text="Admin" Value="Admin"></dx:ListEditItem>
                <dx:ListEditItem Text="John" Value="John"></dx:ListEditItem>
            </Items>
        </dx:ASPxComboBox>
        <br/>
        <dx:ASPxButton ID="ASPxButton1" runat="server" OnClick="ASPxButton1_Click" Text="Login">
        </dx:ASPxButton>
    </div>
</asp:Content>

