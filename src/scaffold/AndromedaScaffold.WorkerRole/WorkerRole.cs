using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;
using AndromedaScaffold.WorkerRole.AndromedaServiceReference;

namespace AndromedaScaffold.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        /// <summary>
        /// This method runs in an endless cycle in the Azure Worker Role.
        /// Its task is to periodically poll the player's status and call the ShipLanded() and ShipFlying()
        /// methods to execute the player's code.
        /// </summary>
        public override void Run()
        {
            //Create the service reference
            AndromedaServiceClient client = new AndromedaServiceClient();

            while (true)
            {
                //Check if the player entered his GUID in the app.config file
                Guid playerGuid;
                if (!Guid.TryParse(ConfigurationManager.AppSettings["PlayerGuid"], out playerGuid))
                {
                    throw new Exception("Invalid player identifier! Please don't forget to register yourself on the Andromeda homepage and to enter your identifier into the app.config file!");
                }

                //This call tells the Andromeda server whether the player is running his code from the 
                //emulator or from Azure.
                //As stated in the game's rules, only code running from Azure is eligible for the competition's prizes.
                //Naturally this call can be faked very easily, but we'll manually check the winners by their IP 
                //addresses, so this is not worth doing. :)
                client.RegisterRunLocation(playerGuid, RoleEnvironment.IsEmulated);

                //Load all the ships of the player
                var spaceshipGuids = client.GetOwnedShips(playerGuid);

                //Check the status of each ship and call the appropriate control method
                foreach (var spaceshipGuid in spaceshipGuids)
                {
                    //We try to download the list of the player's ships (the server will throw an exception
                    //if the player ID is invalid
                    var spaceship = client.GetSpaceshipStatus(spaceshipGuid);

                    //Ship list successfully downloaded. If the ship is on a planet, call ShipLanded(); otherwise
                    //call ShipFlying().
                    NavigationComputer.CommandedShip = spaceshipGuid; //Point the NavigationComputer to this ship
                    if (!spaceship.IsInTransit)
                    {
                        SpaceshipController.ShipLanded(spaceshipGuid);
                    }
                    else
                    {
                        SpaceshipController.ShipFlying(spaceshipGuid);
                    }
                }

                //We wait a bit to avoid overloading the server.
                //It is against the rules to modify this value.
                Thread.Sleep(10000);
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            return base.OnStart();
        }
    }
}
