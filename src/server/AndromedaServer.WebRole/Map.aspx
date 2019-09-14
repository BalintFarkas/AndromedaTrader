<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.master" AutoEventWireup="true"
    Inherits="Andromeda.WebPages.Map" CodeBehind="~/Map.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="Server">
    <script src="Scripts/kinetic-v3.9.0.min.js" type="text/javascript"></script>
    <link rel="Stylesheet" href="Style/Style.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        //This script block is responsible for downloading and updating textual status via AJAX and a timed loop
        var playerGuid = "<%= GetPlayerGuid() %>";
        var ticker;
        var pilotStatus;
        var leaderboard;
        var temp;
        $(function () {
            downloadStatus();
        });

        function downloadStatus() {
            $.ajax({
                type: "POST",
                url: "/InformationService.svc/GetStatus",
                data: '{"playerGuid": "' + playerGuid + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    temp = msg;
                    ticker = temp.GetStatusResult[0];
                    leaderboard = temp.GetStatusResult[1];
                    pilotStatus = temp.GetStatusResult[2];
                    displayStatus();
                }
            });
        }

        function displayStatus() {
            $("#tickerSpan").html(ticker);
            $("#pilotStatusSpan").html(pilotStatus);
            $("#leaderboardSpan").html(leaderboard);

            setTimeout(downloadStatus, 5000);
        }
    </script>
    <script type="text/javascript">
        //This script block is responsible for downloading and rendering star map via AJAX and a timed loop

        //Constants
        var playerGuid = "<%= GetPlayerGuid() %>";
        var starRadius = 3;
        var shipRadius = 5;

        //Data objects
        var stars;
        var ships;

        //Interface objects
        var stage;
        var starsLayer;
        var shipsLayer;

        $(function () {
            //Perform initial download of stars and initial object setup - needs to be done only once
            //This will download stars, perform initial render and then set up a loop to perform regular
            //refresh and re-render of ships
            downloadStars();
        });

        function downloadStars() {
            $.ajax({
                type: "POST",
                url: "/InformationService.svc/GetStars",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    stars = msg;
                    initializeMap();
                }
            });
        }

        function initializeMap() {
            stage = new Kinetic.Stage({ container: "mapViewport", width: 500, height: 500 });
            starsLayer = new Kinetic.Layer();
            shipsLayer = new Kinetic.Layer();

            var backgroundImage = new Kinetic.Rect({
                x: 0,
                y: 0,
                width: 500,
                height: 500,
                fill: "#000000"
            });

            starsLayer.add(backgroundImage);

            for (var pubI = 0; pubI < stars.GetStarsResult.length; pubI++) {
                (function () { //Function call to create a new variable scope
                    var i = pubI; //Local copy of the iterator in this scope
                    var starImage = new Kinetic.Circle({
                        x: stars.GetStarsResult[i].X,
                        y: stars.GetStarsResult[i].Y,
                        radius: starRadius,
                        fill: stars.GetStarsResult[i].Color
                    });

                    starImage.on("mouseover", function () {
                        $("#planetNameSpan").text(stars.GetStarsResult[i].Name);
                    });
                    starImage.on("mouseout", function () {
                        $("#planetNameSpan").text('<%= AndromedaServer.WebRole.Localization.Map_HoverMouse %>');
                    });

                    starsLayer.add(starImage);
                } ());
            };

            stage.add(starsLayer);
            stage.add(shipsLayer);

            //Fetch ships - this will download ships, perform first render for ships and enter a loop to do this periodically
            downloadShips();
        }

        function downloadShips() {
            $.ajax({
                type: "POST",
                url: "/InformationService.svc/GetShips",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{"playerGuid": "' + playerGuid + '"}',
                success: function (msg) {
                    ships = msg;
                    renderShips();
                }
            });
        }

        function renderShips() {
            //Clear stale data from ships layer
            //The clear() method clears only the drawn area but doesn't actually delete children objects - do this manually
            var count = shipsLayer.children.length;
            for (var i = 0; i < count; i++) shipsLayer.remove(shipsLayer.children[0]);

            //Load fresh data
            for (var pubI = 0; pubI < ships.GetShipsResult.length; pubI++) {
                (function () { //Function call to create a new variable scope
                    var i = pubI; //Local copy of the iterator in this scope
                    var shipImage = new Kinetic.Circle({
                        x: ships.GetShipsResult[i].X,
                        y: ships.GetShipsResult[i].Y,
                        radius: shipRadius,
                        fill: ships.GetShipsResult[i].Color
                    });

                    shipImage.on("mouseover", function () {
                        $("#planetNameSpan").text(ships.GetShipsResult[i].Name);
                    });
                    shipImage.on("mouseout", function () {
                        $("#planetNameSpan").text("Vidd rá valamire az egeret!");
                    });

                    shipsLayer.add(shipImage);
                } ());
            };

            //Perform render
            shipsLayer.draw();

            //Refresh ships every 5 seconds
            setTimeout(downloadShips, 5000);
        }
    </script>
    <h1>
        Andromeda Trader</h1>
    <asp:Panel runat="server" ID="NoPilotPanel">
        <div class="note">
            Andromeda Trader is set in a simulated galaxy, where the spaceships of our players
            cruise amongst the stars and try to bring in maximum profit. The players use C#
            to create the controller software of the spaceships. It only takes 10 minutes to
            get in - you only need to implement a single method -, but then do your best to
            sell, purchase, pirate and travel, for the possibilities for profit are endless!
            <br />
            <br />
            {ENTER SHORT SUMMARY OF PRIZES AND ROUND LENGTHS HERE}
            <br />
            <br />
            <a href="Register.aspx">Hop in and see how fast your programming knowledge, trading
                savvy and light-years of travel can earn you a fortune!</a>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="FromEmulatorPanel">
        <div class="note red">
            You are currently running the Andromeda client from an emulator (local machine).
            <b>This is not an issue</b>, this works very well for testing, but if you want to
            compete for prizes, you'll need to upload the application into Azure. This is free
            and takes about 10 minutes. <a href="/andromeda-upload">You'll find the steps here.</a>
        </div>
    </asp:Panel>
    <div style="float: left; width: 500px;">
        <h2>
            Starmap</h2>
        Selected item: <span id="planetNameSpan" style="font-weight: bold; margin-bottom: 5px;
            display: inline-block">Hover the mouse over something!</span>
        <br />
        <div id="mapViewport" style="width: 500px; height: 500px">
        </div>
        <i>You need a HTML5-capable browser to display the map.</i>
    </div>
    <div style="float: left; margin-left: 15px; width: 435px">
        <h2>
            Happening Now</h2>
        <div style="height: 350px; overflow: scroll">
            <span id="tickerSpan"></span>
        </div>
        <br />
        <h2>
            Results</h2>
        <span id="leaderboardSpan"></span><span id="leaderboardWarningSpan" runat="server"
            style="font-weight: bold">Only ships <a href="/andromeda-upload">running from Azure</a>
            get on the leaderboard.<br />
        </span><a href="FullLeaderboard.aspx">Click here</a> to view the full leaderboard.<br />
    </div>
    <div style="clear: both; height: 0">
        &nbsp;</div>
    <asp:Panel runat="server" ID="PilotPanel">
        <div style="padding: 10px 0 20px 0">
            <h2>
                Fleet Status</h2>
            <span id="pilotStatusSpan"></span>
            <br />
            <br />
            <h2>
                Developer Tools</h2>
            If you'd like to re-read the rules, <a href="Register.aspx">click here.</a><br />
            <br />
            Your Unique ID: <b>
                <asp:Literal runat="server" ID="PilotGuidLiteral"></asp:Literal></b><br />
            <br />
            <a href="AndromedaScaffold.zip">Click here to download the Visual Studio template.</a>
            You'll need the <a href="http://go.microsoft.com/fwlink/?LinkID=234939&clcid=0x409">
                Azure SDK</a> to run it, but <b>no Azure skills are necessary</b>.<br />
            Enter the identifier above in the <em>app.config </em>file of the template, then
            open the <em>SpaceshipController</em> code file and edit the <em>ShipLanded()</em>
            method.
            <br />
            <br />
            You can test your code fully by running it from your local computer. And when you
            feel you are ready to go live, upload it in about 10 minutes, <strong>free of charge,
                <a href="/andromeda-upload">into Azure</a>, and you are competing for the prizes!</strong><br />
            <br />
            If you have any questions, <a href="/andromeda-forum">ask them in the forum!</a><br />
            <br />
            <hr />
            <br />
            <strong>Spaceship Operations<br />
            </strong>
            <br />
            If you click this button, all your spaceships will be randomly teleported to other
            star systems. All your money, ships and wares remain unaffected, only the locations
            of your ships change. Use this feature if you are &quot;stuck&quot; in a star system.
            This can only be done once in 48 hours.
            <br />
            <asp:Button runat="server" ID="TeleportPilotButton" Text="Teleport Spaceships" OnClientClick="return confirm('Are you sure you wish to teleport your spaceships? Only the locations of your ships will change, but you can do this only once in 48 hours!')"
                OnClick="TeleportPilotButton_Click" Width="300px" />
            <br />
            <br />
            You can click this button to erase your results so far end restart the game. Use
            it if you are in a hopeless situation for some reason.
            <br />
            <asp:Button runat="server" ID="ResetPilotButton" Text="Reset Game" OnClick="ResetPilotButton_Click"
                OnClientClick="return confirm('Are you sure you wish to reset the game? This will cause you to lose all your ships, wares and money and restart the game from scratch!')"
                Width="300px" />
        </div>
    </asp:Panel>
</asp:Content>
