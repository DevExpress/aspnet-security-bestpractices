<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DashboardPage.aspx.cs" Inherits="SecurityBestPractices.Authorization.Dashboards.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxDashboard ID="ASPxDashboard2" runat="server" UseDashboardConfigurator="True"
        ClientInstanceName="dashboard">
    </dx:ASPxDashboard>
</asp:Content>