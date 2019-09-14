<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.Master" AutoEventWireup="true"
    CodeBehind="AccountRegister.aspx.cs" Inherits="AndromedaServer.WebPages.AccountRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Account Registration</h1>
    <p>
        You'll need to register an user account to participate in the Andromeda Trader competition.
        This is very quick and free.</p>
    <p>
        <asp:CreateUserWizard ID="CreateUserWizard1" MembershipProvider="AspNetSqlMembershipProvider"
            runat="server" oncreateduser="CreateUserWizard1_CreatedUser">
            <WizardSteps>
                <asp:CreateUserWizardStep runat="server" />
                <asp:CompleteWizardStep runat="server" />
            </WizardSteps>
        </asp:CreateUserWizard>
    </p>
</asp:Content>
