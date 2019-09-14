<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.Master" AutoEventWireup="True"
    CodeBehind="~/Admin.aspx.cs" Inherits="Andromeda.WebPages.Admin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        Andromeda Trader Administration</h1>
    <p>
        <asp:Label ID="FeedbackLabel" Font-Size="Large" ForeColor="Blue" Visible="false"
            Text="text" runat="server" /></p>
    <h2>
        Starfield Generation</h2>
    <p>
        This button will scrap all stars on the map (including commodities, if you have
        generated them in the next step) and generate a new starfield.</p>
    <p>
        Starfield width (light years):<br />
        <asp:TextBox ID="StarfieldWidthTextBox" runat="server" Width="50px">1000</asp:TextBox>
        <br />
        Starfield height (light years):<br />
        <asp:TextBox ID="StarfieldHeightTextBox" runat="server" Width="50px">1000</asp:TextBox>
        <br />
        Number of star clusters:<br />
        <asp:TextBox ID="StarClusterCountTextBox" runat="server" Width="50px">20</asp:TextBox>
        <br />
        Star cluster radius (light years - clusters may not overlap with the map edge or
        each other):<br />
        <asp:TextBox ID="StarClusterRadiusTextBox" runat="server" Width="50px">75</asp:TextBox>
        <br />
        Number of stars in clusters:<br />
        <asp:TextBox ID="ClusteredStarCountTextBox" runat="server" Width="50px">360</asp:TextBox>
        <br />
        Number of stars in the void between clusters:<br />
        <asp:TextBox ID="VoidStarCountTextBox" runat="server" Width="50px">160</asp:TextBox>
        <br />
        Minimum distance between two stars (light years):<br />
        <asp:TextBox ID="MinimumDistanceTextBox" runat="server" Width="50px">10</asp:TextBox>
    </p>
    <p>
        <strong>Warning: clusters and stars need to have enough space, or the algorithm will
            never complete.</strong></p>
    <asp:Button ID="GenerateStarfieldButton" Text="Generate Starfield" runat="server"
        OnClick="GenerateStarfieldButton_Click" Width="250px" />
    <br />
    <br />
    <h2>
        Commodity Generation
    </h2>
    <p>
        This button will scrap all commodity information on the map and regenerate it.<br />
        Prices, production rates and max capacities have a Gaussian distribution with the
        specified mean and deviation.<br />
        Chance of Manufacture is the chance in percents that a planet produces the commodity.
        Production rate is units produced per time unit; max capacity is the maximum a planet
        can hold of a resource.<br />
        <i>This version of the generator assigns the same buy/sell price to each commodity,
            and each commodity can be sold everywhere. These are not game engine limitations,
            but generator limitations.</i>
    </p>
    <p>
        Please enter desired products in the following format:<br />
        Product Name|Price Mean|Price Deviation|Chance of Manufacture|Production Rate Mean|Production
        Rate Deviation|Max Capacity Mean|Max Capacity Deviation<br />
        Example:<br />
        Robots|1000|200|20|10|2|100|40</p>
    <p>
        <asp:TextBox ID="CommoditiesInputTextBox" runat="server" Height="200px" TextMode="MultiLine"
            Width="700px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="GenerateCommoditiesButton" Text="Generate Commodities" runat="server"
            OnClick="GenerateCommoditiesButton_Click" Width="250px" />
    </p>
</asp:Content>
