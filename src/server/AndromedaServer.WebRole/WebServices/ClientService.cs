using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Andromeda;
using System.ServiceModel.Activation;
using Andromeda.Data;
using Andromeda.ClientEntities;
using Andromeda.ServerEntities;
using Andromeda.Common;
using Andromeda.EventLog;
using Andromeda.EventLogEntries;
using System.Web;
using System.ServiceModel.Channels;
using AndromedaServer.WebRole;

namespace Andromeda.WebServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerCall)]
    public class ClientService : IClientService
    {
        public List<MapObject> GetStars()
        {
            var db = DataContextFactory.GetAndromedaDataContext();
            return db.Stars.ToList().Select(i => new MapObject()
            {
                Name = i.Name,
                Color = "#999999",
                X = (int)i.X.Value / 2,
                Y = (int)i.Y.Value / 2
            })
                .ToList();
        }

        public List<MapObject> GetShips()
        {
            var db = DataContextFactory.GetAndromedaDataContext();
            var ships = db.Spaceships.Where(i => i.Deleted == false).ToList();

            List<MapObject> mapObjects = new List<MapObject>();
            foreach (var i in ships)
            {
                var coordinates = i.Coordinates;

                mapObjects.Add(
                    new MapObject()
                    {
                        Name = Localization.Map_Spaceship + i.Player.PlayerName,
                        Color = "red",
                        X = (int)coordinates.Item1 / 2,
                        Y = (int)coordinates.Item2 / 2
                    }
                );
            }
            return mapObjects;
        }
    }
}