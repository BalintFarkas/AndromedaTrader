<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.master" AutoEventWireup="true"
    Inherits="Andromeda.WebPages.RegisterComplete" CodeBehind="~/RegisterComplete.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        Andromeda Trader</h1>
    <h2>
        Registration Complete
    </h2>
    <span style="font-size: 150%">Congratulations! Your ship is ready and waiting for you
        in the spaceport!</span><br />
    <br />
    Your identifier: <b>
        <asp:Literal runat="server" ID="PlayerGuidLiteral"></asp:Literal></b>
    <br />
    <br />
    Now download the <a href="AndromedaScaffold.zip">Andromeda Trader Visual Studio template</a>.
    To run the template, you'll need the <a href="http://go.microsoft.com/fwlink/?LinkID=234939&clcid=0x409">
        Azure SDK</a> but <b>no Azure knowledge</b>.
    <br />
    Copy the identifier above to the template's <em>app.config</em> file, then open
    the <em>SpaceshipController</em> code file and modify the <em>ShipLanded()</em>
    method.
    <br />
    <br />
    You can test the code locally. When you feel you are ready, <strong><a href="/andromeda-upload">
        deploy your code to Azure</a> free of charge, and you are in the competition for
        the prizes!</strong><br />
    <br />
    Good luck! If you have any questions, do not hesitate to <a href="/andromeda-forum">
        ask them on the forum</a>!
    <br />
    <br />
    <a href="Map.aspx">Back to the Starmap</a>
</asp:Content>
