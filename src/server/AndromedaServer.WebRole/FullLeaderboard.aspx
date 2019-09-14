<%@ Page Title="" Language="C#" MasterPageFile="~/Andromeda.Master" AutoEventWireup="true"
    Inherits="Andromeda.WebPages.FullLeaderboard" CodeBehind="~/FullLeaderboard.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        var leaderboard;

        $(function () {
            downloadStatus();
        });

        function downloadStatus() {
            $.ajax({
                type: "POST",
                url: "/InformationService.svc/GetFullLeaderboard",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    leaderboard = msg.GetFullLeaderboardResult;
                    displayStatus();
                }
            });
        }

        function displayStatus() {
            $("#leaderboardSpan").html(leaderboard);
            setTimeout(downloadStatus, 5000);
        }
    </script>
    <h1>
        Andromeda Trader</h1>
    <h2>
        Results Table
    </h2>
    <a href="Map.aspx">Back to the Starmap</a><br />
    <br />
    <b>This is the current full results table of the Andromeda Trader competition.<br />
        Don't forget:
        <br />
        1. Only spaceships <a href="/andromeda-upload">running from Azure</a> get on
        the results table (and thus compete for prizes)!
        <br />
        2. The competition has multiple rounds, and everyone starts fresh at the beginning
        of each!</b><br />
    <br />
    <span id="leaderboardSpan"></span>
</asp:Content>
