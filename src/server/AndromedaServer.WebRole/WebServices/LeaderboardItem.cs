using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Andromeda.WebServices
{
    /// <summary>
    /// This object is a local class used when communicating with the AJAX map in the user's browser.
    /// </summary>
    public class LeaderboardItem
    {
        public string PlayerName { get; set; }
        public long NetWorth { get; set; }
        public string NetWorthAsString
        {
            get
            {
                return NetWorth.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = 0, NumberGroupSeparator = " " });
            }
            set
            {
                //Just here to enable serialization on this field
                throw new NotImplementedException();
            }
        }
    }
}