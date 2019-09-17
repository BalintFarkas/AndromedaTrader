/* The game was developed by: Bálint Farkas (balint.farkas@windowslive.com), MatroIT Systems Kft.
 * Commissioned by Microsoft Hungary. */
 
using AndromedaScaffold.WorkerRole.AndromedaServiceReference;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AndromedaScaffold.FunctionApp
{
    public static class Function
    {
        public static Guid GetPlayerGuid()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            //Check if the player entered his GUID in the app.config file
            if (!Guid.TryParse(configuration.GetSection("Values:PlayerGuid")?.Value, out var playerGuid))
            {
                throw new Exception("Invalid player identifier! Please don't forget to register yourself on the Andromeda homepage and to enter your identifier into the app.config file!");
            }

            return playerGuid;
        }


        /// <summary>
        /// https://codehollow.com/2017/02/azure-functions-time-trigger-cron-cheat-sheet/
        /// /*The basic format of the CRON expressions in Azure is:{second} {minute} {hour} {day} {month} {day of the week} e.g. 0 * * * * * (=every minute)*/
        /// </summary>
        [FunctionName("Function")]
        public static async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //Create the service reference
            var client = new AndromedaServiceClient();

            var playerGuid = GetPlayerGuid();

            await Run(client, playerGuid);
        }

        /// <summary>
        /// This method runs in an endless cycle in the Azure Worker Role.
        /// Its task is to periodically poll the player's status and call the ShipLanded() and ShipFlying()
        /// methods to execute the player's code.
        /// </summary>
        private static async Task Run(AndromedaServiceClient client, Guid playerGuid)
        {
            //This call tells the Andromeda server whether the player is running his code from the 
            //emulator or from Azure.
            //As stated in the game's rules, only code running from Azure is eligible for the competition's prizes.
            //Naturally this call can be faked very easily, but we'll manually check the winners by their IP 
            //addresses, so this is not worth doing. :)
            await client.RegisterRunLocationAsync(playerGuid, false); //TODO: RoleEnvironment.IsEmulated);

            //Load all the ships of the player
            var spaceshipGuids = await client.GetOwnedShipsAsync(playerGuid);

            //Check the status of each ship and call the appropriate control method
            foreach (var spaceshipGuid in spaceshipGuids)
            {
                //We try to download the list of the player's ships (the server will throw an exception
                //if the player ID is invalid
                var spaceship = await client.GetSpaceshipStatusAsync(spaceshipGuid);

                //Ship list successfully downloaded. If the ship is on a planet, call ShipLanded(); otherwise
                //call ShipFlying().
                NavigationComputer.CommandedShip = spaceshipGuid; //Point the NavigationComputer to this ship
                if (!spaceship.IsInTransit)
                {
                    await SpaceshipController.ShipLandedAsync(spaceshipGuid);
                }
                else
                {
                    await SpaceshipController.ShipFlying(spaceshipGuid);
                }
            }
        }
    }
}
