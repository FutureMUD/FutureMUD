namespace MudSharp.TimeAndDate.Date {
    public class DayName {
        #region Constructor

        public DayName(string shortname, string longname) {
            _shortName = shortname;
            _fullName = longname;
        }

        #endregion

        #region Properties

        // Short name, e.g. Ides of March, or Holadanis
        protected string _shortName;

        public string ShortName => _shortName;

        // Full name, e.g. The Ides of March, or The Most Sacred Feastday of Holadanis
        protected string _fullName;

        public string FullName => _fullName;

        #endregion
    }
}