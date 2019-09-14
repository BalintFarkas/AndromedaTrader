using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndromedaScaffold.WorkerRole.AndromedaServiceReference;
using System.Configuration;

namespace AndromedaScaffold.WorkerRole
{
    /// <summary>
    /// This class represents the ship's onboard computer.
    /// Using this class, you can query the ship's various sensors and give commands to ship systems.
    /// </summary>
    /// <remarks>
    /// Command methods (such as Buy or Sell) will throw exceptions when called incorrectly (for example
    /// if the player tried to purchase more products than he can afford). We don't want such an exception to 
    /// accidentally derail the user's code, so these methods swallow exceptions and return their message as a string.
    /// This way, the raw exceptions can be seen in the debugger and their error messages are returned to user code,
    /// but they won't disrupt program flow, sparing novice programmers from a frustrating experience.
    /// </remarks>
    public static class NavigationComputer
    {
        /// <summary>
        /// Sets the ship on which the NavigationComputer acts.
        /// The player may have multiple ships; this is how you can set which ship to send orders to.
        /// </summary>
        public static Guid CommandedShip;

        /// <summary>
        /// Service reference to the Andromeda server.
        /// </summary>
        private static AndromedaServiceClient client = new AndromedaServiceClient();

        /// <summary>
        /// Queries the status of the ship.
        /// </summary>
        /// <returns></returns>
        public static Spaceship GetShipStatus()
        {
            return client.GetSpaceshipStatus(CommandedShip);
        }

        /// <summary>
        /// Queries the star the ship is on.
        /// </summary>
        /// <returns></returns>
        public static Star GetCurrentStar()
        {
            return client.GetCurrentStar(CommandedShip);
        }

        /// <summary>
        /// Queries the stars that the ship can see on its sensors.
        /// </summary>
        /// <returns></returns>
        public static List<Star> GetVisibleStars()
        {
            return client.GetVisibleStars(CommandedShip);
        }

        /// <summary>
        /// Launches the ship towards a star.
        /// If the operation is unsuccessful (for example if you chose a star
        /// that is too distant), it returns the detailed description of the error.
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static string LaunchSpaceship(Star destination)
        {
            try
            {
                client.LaunchSpaceship(CommandedShip, destination.StarGuid);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Purchases the given quantity of the given commodity in the current star system.
        /// If the operation is unsuccessful (for example if you don't have enough money), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <param name="commodityName"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static string Buy(string commodityName, int quantity)
        {
            try
            {
                client.Buy(CommandedShip, commodityName, quantity);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Purchases the maximum possible amount of the given commodity (taking into account your available money,
        /// the star system's stocks and the ship's free cargo hold capacity).
        /// If the operation is unsuccessful (for example if you specified an invalid product name), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <param name="commodityName"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static string BuyMaximum(string commodityName)
        {
            try
            {
                client.BuyMaximum(CommandedShip, commodityName);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Sells the given quantity of the given commodity in the current star system.
        /// If the operation is unsuccessful (for example if you have less than the amount you tried to sell), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// <param name="commodityName"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static string Sell(string commodityName, int quantity)
        {
            try
            {
                client.Sell(CommandedShip, commodityName, quantity);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Adds an extra drive to the ship, which makes it faster, but decreases cargo hold space.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't have enough space for the component), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string AddDrive()
        {
            try
            {
                client.AddDrive(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Removes an extra drive from the ship, which makes it slower, but frees up cargo space. 
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't actually have a drive to remove), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string RemoveDrive()
        {
            try
            {
                client.RemoveDrive(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Adds an extra sensor to the ship, which increases its sensor range, but decreases free cargo space.
        /// The larger the sensor range, the further the ship can get information from stars, navigate to them, or
        /// raid others as a pirate.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't have enough space for the component), it 
        /// returns the detailed description of the error.
        /// Don't forget to call GetShipStatus() and GetVisibleStars() to update the ship's status and check
        /// the new stars you can see after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string AddSensor()
        {
            try
            {
                client.AddSensor(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Removes an extra sensor from the ship, which decreases its sensor range, but increases free cargo space.
        /// The larger the sensor range, the further the ship can get information from stars, navigate to them, or
        /// raid others as a pirate.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't have enough space for the component), it 
        /// returns the detailed description of the error.
        /// Don't forget to call GetShipStatus() and GetVisibleStars() to update the ship's status and the list of 
        /// stars you can see after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string RemoveSensor()
        {
            try
            {
                client.RemoveSensor(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Upgrades your current ship hull to the Nebula model which has a cargo hold capacity of 200.
        /// This costs one million credits.
        /// You cannot downgrade from a larger ship to a smaller one.
        /// All other parameters of the ship remain unaffected, so all existing cargo and components
        /// (such as sensors, drives and so on) are untouched.
        /// </summary>
        /// <returns></returns>
        public static string UpgradeShipCapacityTo200()
        {
            try
            {
                client.UpgradeShip(CommandedShip, 1);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Upgrades your current ship hull to the Aquila model which has a cargo hold capacity of 300.
        /// This costs ten million credits.
        /// You cannot downgrade from a larger ship to a smaller one.
        /// All other parameters of the ship remain unaffected, so all existing cargo and components
        /// (such as sensors, drives and so on) are untouched.
        /// </summary>
        /// <returns></returns>
        public static string UpgradeShipCapacityTo300()
        {
            try
            {
                client.UpgradeShip(CommandedShip, 2);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Installs a plasma cannon on your ship. Using your cannons, you can raid others as a pirate for their cargo,
        /// but cannons take up cargo hold space.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't have enough space for the component), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string AddCannon()
        {
            try
            {
                client.AddCannon(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Removes a plasma cannon from your ship. Using your cannons, you can raid others as a pirate for their cargo,
        /// but cannons take up cargo hold space.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example you don't actually have a cannon to remove), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string RemoveCannon()
        {
            try
            {
                client.RemoveCannon(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Installs a gravity shield on your ship. Using your shields, you can defend yourself from pirate attacks,
        /// but shields take up cargo space.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't have enough space for the component), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string AddShield()
        {
            try
            {
                client.AddShield(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Removes a gravity shield from your ship. Using your shields, you can defend yourself from pirate attacks,
        /// but shields take up cargo space.
        /// The modification does not cost money, but the ship can be modified only once per landing,
        /// so you have to visit another star system to add or remove further components.
        /// If the operation is unsuccessful (for example if you don't actually have a shield to remove), it 
        /// returns the detailed description of the error.
        /// Don't forget to update the ship's status using the GetShipStatus() method after the transaction!
        /// </summary>
        /// <returns></returns>
        public static string RemoveShield()
        {
            try
            {
                client.RemoveShield(CommandedShip);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Gets the list of ships you can attack. A ship can be attacked if it is within your sensor range,
        /// was not robbed in a while and has been in play for long enough.
        /// </summary>
        /// <remarks>
        /// A ship's transponder code is different from the GUIDs players are identified with, so a ship's
        /// transponder code cannot be used to impersonate a player.
        /// </remarks>
        /// <returns></returns>
        public static List<MerchantShip> GetRaidableShips()
        {
            return client.GetRaidableShips(CommandedShip);
        }

        /// <summary>
        /// Attacks the target ship. The attack is instantaneous, because you don't actually have to get near
        /// the target, your cannons have a very long range.
        /// The success of your attack depends the number of cannons you have, the number of shields the target has,
        /// and the distance between you (cannons become less effective over long distances).
        /// If your attack succeeds, your crew uses shuttles to bring in as much loot from the target ship as they
        /// can fit in your cargo hold. You cannot take money from the target and their ship can continue its journey.
        /// If your attack fails, then the target cannot shoot back (because his shields are overloaded and interfere
        /// with his targeting computer), so you get away unharmed, but you lost time while others were busy making profit!
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string Raid(MerchantShip target)
        {
            try
            {
                client.Raid(CommandedShip, target.TransponderCode);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Returns the identifiers of all the ships that you own.
        /// </summary>
        /// <returns></returns>
        public static List<Guid> GetOwnedShips()
        {
            return client.GetOwnedShips(new Guid(ConfigurationManager.AppSettings["PlayerGuid"]));
        }

        /// <summary>
        /// Purchases a new ship for you. The maximum number of ships one can own is limited and the price of
        /// subsequent ships gets higher.
        /// The ShipLanded() and ShipFlying() methods are called for each ship you own; you can see which ship's turn
        /// is up from the parameter passed in to these methods.
        /// You can get a list of all your ships using the GetOwnedShips() method.
        /// </summary>
        /// <returns></returns>
        public static string BuyNewShip()
        {
            try
            {
                client.BuyNewShip(new Guid(ConfigurationManager.AppSettings["PlayerGuid"]));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
