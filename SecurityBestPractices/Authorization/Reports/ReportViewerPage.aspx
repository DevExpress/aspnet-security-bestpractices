<%@ Page Language="C#"  MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportViewerPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.Reports.ReportViewerPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxComboBox runat="server" DataSecurityMode="Strict" ID="ReportNames" 
                     Caption="Report" SelectedIndex="0" DropDownStyle="DropDownList" IncrementalFilteringMode="None" 
                     OnSelectedIndexChanged="ReportName_OnSelectedIndexChanged" AutoPostBack="True">
    </dx:ASPxComboBox>  
    
    <dx:ASPxButton runat="server" id="EditButton" Text="Edit" OnClick="EditButton_OnClick"></dx:ASPxButton>

    <dx:ASPxWebDocumentViewer ID="documentViewer" runat="server" ClientInstanceName="documentViewer">
    </dx:ASPxWebDocumentViewer>
</asp:Content>
