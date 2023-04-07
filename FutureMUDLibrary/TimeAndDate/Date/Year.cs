using System.Collections.Generic;

namespace MudSharp.TimeAndDate.Date {
    public class Year {
        #region Properties

        protected int _year;

        public int YearName => _year;

        protected ICalendar _calendar;

        public ICalendar Calendar => _calendar;

        /// <summary>
        ///     The weekday index of the first day of the year that is a weekday.
        /// </summary>
        protected int _firstWeekdayIndex;

        public int FirstWeekdayIndex => _firstWeekdayIndex;

        #endregion

        #region Vars

        protected List<Month> _months;

        public List<Month> Months => _months;

        #endregion

        #region Methods

        #endregion

        #region Constructors

        public Year(List<Month> months, int year, ICalendar calendar) {
            _months = months;
            _year = year;
            _calendar = calendar;
            var trueOrder = 1;
            _months.ForEach(x => x.TrueOrder = trueOrder++);
            _firstWeekdayIndex = Calendar.GetFirstWeekday(_year);
        }

        public Year(Year copyYear) {
            _year = copyYear._year;
            _months = new List<Month>(copyYear._months);
            _calendar = copyYear._calendar;
        }

        #endregion
    }
}