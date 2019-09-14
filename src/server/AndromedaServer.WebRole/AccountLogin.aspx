<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.Master" AutoEventWireup="true"
    CodeBehind="AccountLogin.aspx.cs" Inherits="AndromedaServer.WebSites.AccountLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Andromeda Trader</h1>
    <h2>
        Log In</h2>
    <p>
        If you don&#39;t yet have an account, <a href="/AccountRegister.aspx<%= GetRedirectQueryString() %>">click here to
            register!</a></p>
    <asp:Login ID="Login1" MembershipProvider="AspNetSqlMembershipProvider" runat="server">
    </asp:Login>
</asp:Content>
