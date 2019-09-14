using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Andromeda.EventLogEntries;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Andromeda.ServerEntities;
using Andromeda.Data;
using System.Reflection;
using Andromeda.Common;

namespace Andromeda.EventLog
{
    /// <summary>
    /// Logs events into the database and loads them back.
    /// </summary>
    /// <remarks>
    /// The chief complexity is the mapping between a number of classes neatly derived from EventLogEntryBase
    /// and the relational DB. The class contains two methods that abstract this complexity away.
    /// </remarks>
    public static class Logger
    {
        private static AndromedaDataContext db = DataContextFactory.GetAndromedaDataContext();

        /// <summary>
        /// Logs the given object into the DB.
        /// </summary>
        /// <param name="logEntry"></param>
        public static void Log(EventLogEntryBase logEntry)
        {
            //Serialize the object into a string
            XmlSerializer serializer = new XmlSerializer(logEntry.GetType());
            StringBuilder outputStringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(outputStringBuilder);
            serializer.Serialize(stringWriter, logEntry);

            //Create a database record around the string and write it to the DB
            //The record is a hybrid: the common fields are stored as SQL columns for quick access,
            //the log entry specific columns are stored in XML for flexibility.
            EventLogEntry dbEntry = new EventLogEntry()
            {
                Error = logEntry.Error,
                EventType = logEntry.GetType().Name,
                Guid = logEntry.Guid,
                Player = logEntry.Player,
                Timestamp = logEntry.Timestamp,
                Xml = outputStringBuilder.ToString()
            };
            db.EventLogEntries.InsertOnSubmit(dbEntry);
            db.SubmitChanges();
        }

        /// <summary>
        /// Converts XML stored in an event log entry back into a C# class.
        /// Suggested usage: use the DB columns to efficiently filter/sort to interesting rows, then convert those
        /// rows back into objects using this method.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static EventLogEntryBase ConvertEventXmlToObject(string eventType, string xml)
        {
            //Get the type from the type name
            //EventLogEntryBase has no significance here, it's simply a type that is certainly in the assembly that 
            //stores the other event log types
            XmlSerializer serializer = new XmlSerializer(typeof(EventLogEntryBase).Assembly.GetType("Andromeda.EventLogEntries." + eventType));
            //Deserialize the XML into an instance of the given type
            return (EventLogEntryBase)serializer.Deserialize(new StringReader(xml));
        }
    }
}