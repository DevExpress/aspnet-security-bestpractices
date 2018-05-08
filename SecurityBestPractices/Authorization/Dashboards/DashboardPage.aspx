<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DashboardPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.Dashboards.Dashboard" %>
<%@ Register Assembly="DevExpress.Dashboard.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.DashboardWeb" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxDashboard ID="ASPxDashboard2" runat="server" UseDashboardConfigurator="True"
        ClientInstanceName="dashboard">
    </dx:ASPxDashboard>
</asp:Content>