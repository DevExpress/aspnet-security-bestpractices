<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportDesignerPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.Reports.ReportDesignerPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxReportDesigner ID="ASPxReportDesigner1" runat="server">
    </dx:ASPxReportDesigner>
</asp:Content>
