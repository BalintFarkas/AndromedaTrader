using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a trade transaction.
    /// </summary>
    public class Trade : EventLogEntryBase
    {
        public string Star { get; set; }
        public int StarId { get; set; }
        public string Commodity { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public bool IsPurchase { get; set; }
        public long NewPlayerMoney { get; set; }
    }
}