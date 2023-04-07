using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.TimeAndDate.Date {
    public class Month {
        #region Constructor

        public Month(MonthDefinition definition, int whichYear) {
            // Property copies
            _days = definition.NormalDays;
            _alias = definition.Alias;
            _fullName = definition.FullName;
            _shortName = definition.ShortName;
            _nominalOrder = definition.NominalOrder;

            // Copy the day names across from the base month definition
            _dayNames = definition.SpecialDayNames.ToDictionary(e => e.Key, e => e.Value);

            _nonWeekdays = definition.NonWeekdays.ToList();

            // Go through each of the intercalary rules and process them
            definition.Intercalaries.FindAll(x => x.Rule.IsIntercalaryYear(whichYear)).ForEach(i => {
                i.RemoveSpecialDayNames.ForEach(x => _dayNames.Remove(x));
                i.SpecialDayNames.ToList().ForEach(x => _dayNames.Add(x.Key, x.Value));
                _days += i.InsertNumnewDays;
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     How many days are in this month
        /// </summary>
        protected int _days;

        public int Days => _days;

        /// <summary>
        ///     Alias of the Month, e.g. july
        /// </summary>
        protected string _alias;

        public string Alias => _alias;

        /// <summary>
        ///     Short Name of the month, e.g. Jul - used in abbreviated date output
        /// </summary>
        protected string _shortName;

        public string ShortName => _shortName;

        /// <summary>
        ///     Full name of the month, e.g. July
        /// </summary>
        protected string _fullName;

        public string FullName => _fullName;

        /// <summary>
        ///     The nominal order in which the month appears in the year, for example, July would be 7. Intercalaries often do not
        ///     appear in this nominal order and can have a nominal order of -1
        /// </summary>
        protected int _nominalOrder;

        public int NominalOrder => _nominalOrder;

        /// <summary>
        ///     The true order of this month in the year - this includes intercalaries, so may not correspond to the nominal order
        /// </summary>
        protected int _trueOrder;

        public int TrueOrder
        {
            get { return _trueOrder; }
            set { _trueOrder = value; }
        }

        protected List<int> _nonWeekdays;

        public List<int> NonWeekdays => _nonWeekdays;

        #endregion

        #region Vars

        protected Dictionary<int, DayName> _dayNames;

        public IReadOnlyDictionary<int, DayName> DayNames => _dayNames;

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the correct display for the short version of the day/month combination for the specified day of the month,
        ///     e.g. "ides of march", "march 15th", "15th of march"
        /// </summary>
        /// <param name="whichDay">which numerical day of the month to display</param>
        /// <returns>returns a string with the short version of the day/month</returns>
        public string GetDayName(MudDate whichDay) {
            //return ((_dayNames.ContainsKey(whichDay)) ? _dayNames[whichDay].ShortName : ((MonthBeforeDay == true) ? (Alias + " " + Utilities.ToOrdinal(whichDay)) : (Utilities.ToOrdinal(whichDay) + " of " + Alias)));
            //return ((_dayNames.ContainsKey(whichDay.Day)) ? _dayNames[whichDay.Day].ShortName + ", " : "") + (NonWeekdays.Contains(whichDay.Day) ? "" : whichDay.Weekday + ", ") + ((MonthBeforeDay == true) ? (FullName + " " + Utilities.ToOrdinal(whichDay.Day)) : (Utilities.ToOrdinal(whichDay.Day) + " " + FullName));
            return _dayNames.ContainsKey(whichDay.Day) ? _dayNames[whichDay.Day].ShortName : "";
        }

        /// <summary>
        ///     Returns the correct display for the long version of the day/month combination for the specified day of the month,
        ///     e.g. "The Ides of March", "
        /// </summary>
        /// <param name="whichDay"></param>
        /// <returns></returns>
        public string GetFullDayName(MudDate whichDay) {
            //return ((_dayNames.ContainsKey(whichDay.Day)) ? _dayNames[whichDay.Day].FullName + ", " : "") + (NonWeekdays.Contains(whichDay.Day) ? "" : whichDay.Weekday + ", ") + ((MonthBeforeDay == true) ? (FullName + " " + Utilities.ToOrdinal(whichDay.Day)) : (Utilities.ToOrdinal(whichDay.Day) + " of " + FullName));
            return _dayNames.ContainsKey(whichDay.Day) ? _dayNames[whichDay.Day].FullName : "";
        }

        public int CountWeekdays() {
            return Days - NonWeekdays.Count;
        }

        public override bool Equals(object obj) {
            if (obj is not Month objAsMonth) {
                return false;
            }

            return Alias.Equals(objAsMonth.Alias, StringComparison.InvariantCultureIgnoreCase) &&
                   (Days == objAsMonth.Days);
        }

        public override int GetHashCode() {
            return Alias.GetHashCode()*Days.GetHashCode();
        }

        #endregion
    }
}