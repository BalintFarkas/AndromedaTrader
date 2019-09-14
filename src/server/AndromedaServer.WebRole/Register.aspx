<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.Master" AutoEventWireup="true"
    Inherits="Andromeda.WebPages.Register" CodeBehind="~/Register.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="Server">
    <link rel="Stylesheet" href="Style/Style.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="Server">
    <h1>
        Andromeda Trader</h1>
    <h2>
        Game Rules
    </h2>
    <b>We have summed up the rules of the game below. Go through it once, and you&#39;ll 
    know everything you need to play immediately!<br />
    </b>
    <br />
    <strong>The Galaxy<br />
    </strong>The Andromeda Trader galaxy is 1000×1000 light-years in size. It has 
    star systems; each star system has a single spaceport, where spaceships can dock 
    and trade. The aim of the game is to buy low, sell high (or rob others), and 
    gain maximum profit.<br />
    <br />
    Travel through the galaxy is done with a faster-than-light Cherenkov drive. This 
    uses fusion, so its energy source is virtually inexhaustible - you don&#39;t need to 
    care about refueling. A ship without extra drives can travel 50 light years per 
    minute.<br />
    There is one restriction, however: your ship needs to have precise information 
    about the star it is trying to travel to, so you can travel only to stars that 
    are within the ship&#39;s sensor range. A ship without extra sensors sees in a 
    radius of 50 light years.<br />
    <br />
    This means that if there is a star 25 light-years from you, 
    your ship can see it, 
    and can travel to it in about half a minute. If, however, there is another star 
    75 light-years from you, your ship cannot see it and thus cannot navigate to it.<br />
    <br />
    <strong>Commerce</strong><br />
    Each star systems knows the same goods (such as water, crude oil, heavy 
    machinery etc.), so if you bought a product somewhere, you can sell it everywhere 
    else.<br />
    Buying, however, has limits: each product type is only manufactured on some of 
    the star systems (for example, only a small percentage of star systems have the 
    industry necessary to manufacture robots), and the supply is limited even in 
    these star systems. So if for example all robots have been bought in a given 
    star system, you have to wait for more to be made before you can buy.<br />
    The cargo space on your ship is also limited; depending on the ship model, 
    you can fit in 100, 200 or 300 units of products.<br />
    <br />
    The price level of a given star system is constant. For example, if 
    semiconductors cost 500 credits per unit, then they can be sold and bought at 
    this same price, and the price does not change over time.<br />
    The price levels of different star systems, however, vary greatly. This is how 
    you make a profit - you can see the price levels of star systems that are within 
    your sensor envelope, and you can find possible profit opportunities.<br />
    <br />
    <strong>Upgrading your Fleet<br />
    </strong>You start with a basic Cobra-class spaceship that has 100 units of 
    cargo space, can travel 50 light years/minute and has a sensor range of 50 
    light-years. There are various upgrades available for ship systems, the ship 
    itself, and more ships.<br />
    <br />
    <em><strong>1. Ship Systems<br />
    </strong></em>You can upgrade your ship with additional components to expand its capabilities. 
    Equipping these additional components is free, but you can equip (or detach) a component only once per spaceport. You have to travel to another spaceport 
    to equip (or detach) more components.<br />
    Also, each component takes up 20 units from your cargo space, so the more 
    components you have, the less cargo you can carry!<br />
    <br />
    The available ship components are:<br />
    Extra Drives: Each additional Drive increases your speed by 20 light 
    years/minute.<br />
    Extra Sensors: Each additional Sensor increases your sensor range by 20 light 
    years.<br />
    Plasma Cannons: Used in combat (see Piracy below).<br />
    Gravity Shields: Used in combat (see Piracy below).
    <br />
    <br />
    <em><strong>2. Ship Types<br />
    </strong></em>You can also upgrade the ship itself. The basic Cobra model has 100 units of 
    cargo space. You can upgrade to larger ship types with more cargo space. When 
    you do an upgrade, only the cargo space of the ship changes, nothing else; all 
    your ship components and cargo remain in place. Also, you cannot downgrade ship 
    models.<br />
    <br />
    The following ship types are available for purchase:<br />
    Nebula: has 200 units of cargo space; costs 1 million credits<br />
    Aquila: has 300 units of cargo space; costs 10 million credits<br />
    <br />
    <strong><em>3. Additional Ships<br />
    </em></strong>And finally, you start with only one ship, but you can purchase more ships to 
    expand your fleet and bring in profit quicker. The additional ships you purchase 
    have exactly the same capabilities as your starting ship: you can write a 
    different control program for them (or use the one you already have), you can 
    give them additional components, upgrade their model etc. The newly purchased 
    ships start as empty Cobra-class ships, so they have 100 units of cargo space, 
    and have no components or cargo.<br />
    <br />
    You start with 1 ship.<br />
    The second ship costs 10 million credits.<br />
    The third ship costs 50 million credits.<br />
    The fourth ship costs 150 million credits.<br />
    The fifth ship costs 400 million credits.<br />
    You cannot buy a sixth ship.<br />
    <br />
    <strong>Piracy<br />
    </strong>Not everyone is cut out to be a trader. If you wish, you can paint 
    the skull and crossbones on your ship hull and start raiding others for profit. 
    (Or you can appear to be a peaceful trader to the galaxy, and do some sneak 
    attacks when nobody is looking!)<br />
    To raid someone, first you need to equip plasma cannons on your ships. Then 
    start looking; when someone gets in your sensor range, you can attack him and 
    force him to surrender his goods to you.<br />
    The efficiency of your plasma cannons depends on distance. You can use one 
    cannon less per 10 light years.<br />
    If, for example, you 4 plasma cannons, and somebody is within 10 light years, 
    you can use all 4 cannons to attack him. If the target is farther than 10 light 
    years, but within 20 light years, you can use 3 cannons. If the target is 
    is farther than 20 light years, but within 30 light years, you can only use 2 
    cannons. And so on; if the target is farther than 40 light years, then even 
    though you have him on your sensors, you cannot effectively attack him.<br />
    <br />
    When raiding someone, you&#39;ll try to hit him with cannons and his ship computer 
    uses his ship&#39;s shields to defend him. If you have more cannons than he has 
    shields, than you win - you manage to disable his Cherenkov drives and your crew 
    takes as much cargo from his holds as they can fit in your ship. (So even if 
    you&#39;re a pirate, you need some empty cargo space.)<br />
    If you have equal or less cannons than he has shields, then your attack is 
    blocked and the target gets away.<br />
    <br />
    Attacked targets do not fire back. If an attack fails, the attacker does not get 
    any punishment (apart from the time he lost while others were busy making 
    profit!). Raided ships lose only their cargo; their ship systems are not damaged 
    and they also do not lose money.<br />
    After a ship has been successfully raided, it cannot be attacked again for 10 
    minutes (since a cruiser of the Galactic Navy escorts him for a while). Ships of 
    new players who just started the game also cannot be attacked for 24 hours.<br />
    <br />
    <asp:Panel runat="server" ID="RegisterPanel">
        <b>And that&#39;s it.</b> Click the button below to create your first own spaceship 
        - then download the Visual Studio template and start developing its control 
        software!
        <br />
        <br />
        <asp:Button runat="server" ID="CreatePilotButton" Text="Launch Spaceship" OnClick="CreatePilotButton_Click" />
    </asp:Panel>
</asp:Content>
