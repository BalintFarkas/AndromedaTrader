using System;
using Andromeda.Data;
using System.Data.Linq;
using Andromeda.ServerEntities;

namespace Andromeda.Common
{
    public static class DataContextFactory
    {
        /// <summary>
        /// Instantiates an AndromedaDataContext with appropriate settings
        /// </summary>
        /// <returns></returns>
        public static AndromedaDataContext GetAndromedaDataContext()
        {
            //Get base objects
            var db = new AndromedaDataContext();

            //Make sure loading spaceships also loads players (to prevents lots of unnecessary transactions)
            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<Spaceship>(i => i.Player);
            db.LoadOptions = options;

            //Return
            return db;
        }
    }
}