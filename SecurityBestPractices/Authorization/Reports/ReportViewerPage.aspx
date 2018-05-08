<%@ Page Language="C#"  MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportViewerPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.Reports.ReportViewerPage" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.XtraReports.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraReports.Web" tagprefix="dx" %>

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
