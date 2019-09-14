/* The game was developed by: Bálint Farkas (balint.farkas@windowslive.com), MatroIT Systems Kft.
 * Commissioned by Microsoft Hungary. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndromedaScaffold.WorkerRole
{
    public static class SpaceshipController
    {
        /// <summary>
        /// This method is called automatically when your ship lands on a planet.
        /// You can use this method to purchase commodities, equip your ship and launch it towards another star system.
        /// All you have to do is implement this method and the ShipFlying() method - you don't
        /// need to touch the other files in the solution.
        /// 
        /// Don't forget: post any issues you have on the forum! {FORUM URL HERE}
        /// The starmap and all other information can be found on: {GAME URL HERE}
        /// </summary>
        public static void ShipLanded(Guid currentShip)
        {
            //Get the status of our ship
            var ship = NavigationComputer.GetShipStatus();
            //Get our current star
            var currentStar = NavigationComputer.GetCurrentStar();
            //Get the stars we can see
            var stars = NavigationComputer.GetVisibleStars();

            //Please note that you can own multiple ships. You can differentiate between ships using the
            //currentShip parameter and provide separate logic for each ship. This is not checked in this example,
            //so all ships behave in the same way after landing. See more details in the ShipFlying() method at the 
            //bottom!

            //If we can, buy a larger ship model!
            //This costs a lot of credits, but increases storage space.
            if (ship.Money > 1000000 && ship.TotalCapacity == 100)
            {
                NavigationComputer.UpgradeShipCapacityTo200();
            }

            //If the ship has no extra sensor yet, then purchase one, to see further.
            //(This increases sensor range to 70 light years from 50 light years, but decreases available
            //cargo space from 100 to 80 units - this is probably worth it.)
            if (ship.SensorCount == 0)
            {
                //These method calls do not throw exceptions, to avoid disrupting program flow.
                //Instead they return any error as a string.
                NavigationComputer.AddSensor();

                //Refresh ship status and visible stars right away (we'll quite possible see more stars, since
                //our sensor range has increased).
                ship = NavigationComputer.GetShipStatus();
                stars = NavigationComputer.GetVisibleStars();

                //You can use the same logic to add or remove drives, sensors, cannons and shields.
            }

            //Sell everything we've brought.
            foreach (var cargoItem in ship.Cargo)
            {
                //These method calls do not throw exceptions, to avoid disrupting program flow.
                //Instead they return any error as a string.
                NavigationComputer.Sell(cargoItem.Name, cargoItem.Stock);
            }
            
            //Let's trade water!
            //Check if any of the nearby stars have a higher price level for water than this star system.
            //If we find such a star we can make a profit by buying water here and selling it there.
            int localPrice = currentStar.Commodities.Single(i => i.Name == "Water").Price;
            int maxPrice = stars.Max(i => i.Commodities.Single(j => j.Name == "Water").Price);

            //Profit can be made by trading water - buy as much as we can, and go to that star!
            if (maxPrice > localPrice)
            {
                NavigationComputer.BuyMaximum("Water");
                var targetStar = stars.First(i => i.Commodities.Single(j => j.Name == "Water").Price == maxPrice);
                NavigationComputer.LaunchSpaceship(targetStar);
            }
            //There is no trade opportunity in water - go to a random star and hope for better luck.
            else
            {
                Random rnd = new Random((int)DateTime.Now.Ticks);
                var randomStar = stars[rnd.Next(0, stars.Count)];
                NavigationComputer.LaunchSpaceship(randomStar);
            }
        }

        /// <summary>
        /// This method is called automatically every few seconds while your ship is flying.
        /// Trading, equipment modification etc. can be done only when landed on a planet,
        /// but you can use this method to attack others!
        /// If you wish to be a peaceful trader, then you can ignore this method, since only pirates
        /// can make any use of it.
        /// 
        /// All you have to do is implement this method and the ShipLanded() method - you don't
        /// need to touch the other files in the solution.
        /// Don't forget: post any issues you have on the forum! {FORUM URL HERE}
        /// The starmap and all other information can be found on: {GAME URL HERE}
        /// </summary>
        public static void ShipFlying(Guid currentShip)
        {
            //Let's try and buy another ship, if we have the money! The first additional ship costs 10 million credits.
            var ship = NavigationComputer.GetShipStatus();
            if (ship.Money > 10000000)
            {
                NavigationComputer.BuyNewShip();
            }

            //We can have multiple ships. By checking the value of the currentShip parameter, you can differentiate
            //between your ships and give each of the separate orders.
            //In this example, if this is not the first ship, we'll use it for piracy.
            var ownedShips = NavigationComputer.GetOwnedShips();
            if (currentShip != ownedShips.First())
            {
                //Let's attack the first ship that gets in our sight!
                //Caution: this only makes sense when you have some cannons equipped.
                var raidableShips = NavigationComputer.GetRaidableShips();

                if (raidableShips.Count > 0)
                {
                    NavigationComputer.Raid(raidableShips.First());
                }
            }
        }
    }
}
