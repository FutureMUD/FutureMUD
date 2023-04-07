using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Date {
    public class MonthDefinition : IXmlSavable, IXmlLoadable {
        public void LoadFromXml(XElement root) {
            if (!root.HasElements) {
                throw new XmlException("Root without any elements in MonthDefinition LoadFromXML.");
            }

            // Alias
            var element = root.Element("alias");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing alias value in MonthDefinition LoadFromXML.");
            }

            Alias = element.Value;

            // Full Name
            element = root.Element("fullname");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing fullname value in MonthDefinition LoadFromXML.");
            }

            FullName = element.Value;

            // ShortName
            element = root.Element("shortname");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing shortname value in MonthDefinition LoadFromXML.");
            }

            ShortName = element.Value;

            // Normal Days
            element = root.Element("normaldays");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing normaldays value in MonthDefinition LoadFromXML.");
            }

            try {
                NormalDays = int.Parse(element.Value);
            }
            catch {
                throw new XmlException("Value for normaldays in MonthDefinition LoadFromXML is not a valid Integer");
            }

            // Nominal Order
            element = root.Element("nominalorder");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing nominalorder value in MonthDefinition LoadFromXML.");
            }

            try {
                NominalOrder = int.Parse(element.Value);
            }
            catch {
                throw new XmlException("Value for nominalorder in MonthDefinition LoadFromXML is not a valid Integer");
            }

            // Special Day Names
            (from sd in root.Element("specialdays").Elements("specialday")
                    where (sd.Attribute("day") != null) &&
                          (sd.Attribute("short") != null) &&
                          (sd.Attribute("long") != null)
                    select sd)
                .ToList()
                .ForEach(x => SpecialDayNames
                        .Add(int.Parse(x.Attribute("day").Value),
                            new DayName(x.Attribute("short").Value, 
	                            x.Attribute("long").Value))
                );

            // Non Weekdays
            (from nwd in root.Element("nonweekdays").Elements("nonweekday")
                    select nwd)
                .ToList()
                .ForEach(x => NonWeekdays.Add(int.Parse(x.Value)));

            // Intercalaries
            element = root.Element("intercalarydays");
            if (element?.HasElements == true) {
                foreach (var subElement in element.Elements()) {
                    var intercalary = new IntercalaryDay();
                    intercalary.LoadFromXml(subElement);
                    _intercalaries.Add(intercalary);
                }
            }
        }

        public XElement SaveToXml() {
            return new XElement
            (
                "month", new XElement("alias", Alias), new XElement("shortname", ShortName),
                new XElement("fullname", FullName), new XElement("nominalorder", NominalOrder),
                new XElement("normaldays", NormalDays), new XElement("intercalarydays",
                    new object[] {
                        from ic in Intercalaries
                        select ic.SaveToXml()
                    }
                ), new XElement("specialdays",
                    new object[] {
                        from sdn in SpecialDayNames
                        select
                        new XElement("specialday", new XAttribute("day", sdn.Key),
                            new XAttribute("short", sdn.Value.ShortName), new XAttribute("long", sdn.Value.FullName))
                    }
                ), new XElement("nonweekdays",
                    new object[] {
                        from nwd in NonWeekdays
                        select new XElement("nonweekday", nwd)
                    }
                ));
        }

        #region Properties

        // Alias of the Month, e.g. july
        protected string _alias;

        public string Alias
        {
            get { return _alias; }
            protected set { _alias = value; }
        }

        // Short Name of the month, e.g. Jul - used in abbreviated date output
        protected string _shortName;

        public string ShortName
        {
            get { return _shortName; }
            protected set { _shortName = value; }
        }

        // Full name of the month, e.g. July
        protected string _fullName;

        public string FullName
        {
            get { return _fullName; }
            protected set { _fullName = value; }
        }

        /// <summary>
        ///     The nominal order in which the month appears in the year, for example, July would be 7. Intercalaries often do not
        ///     appear in this nominal order and can have a nominal order of -1
        /// </summary>
        protected int _nominalOrder;

        public int NominalOrder
        {
            get { return _nominalOrder; }
            protected set { _nominalOrder = value; }
        }

        // Ordinary days in the month
        protected int _normalDays;

        public int NormalDays
        {
            get { return _normalDays; }
            protected set { _normalDays = value; }
        }

        #endregion

        #region Vars

        /// <summary>
        ///     Specifies which days have special names in this month
        /// </summary>
        protected Dictionary<int, DayName> _specialDayNames = new();

        public Dictionary<int, DayName> SpecialDayNames => _specialDayNames;

        /// <summary>
        ///     All intercalary days that apply to this month
        /// </summary>
        protected List<IntercalaryDay> _intercalaries = new();

        public List<IntercalaryDay> Intercalaries => _intercalaries;

        protected List<int> _nonWeekdays = new();

        public List<int> NonWeekdays => _nonWeekdays;

        #endregion

        #region Methods

        #endregion

        #region Constructor

        public MonthDefinition() {
        }

        public MonthDefinition(string shortname, string alias, string fullname, int nominalorder, int normaldays,
            Dictionary<int, DayName> daynames, List<IntercalaryDay> intercalaries) {
            _alias = alias;
            _shortName = shortname;
            _fullName = fullname;
            _nominalOrder = nominalorder;
            _normalDays = normaldays;
            _specialDayNames = daynames;
            _intercalaries = intercalaries;
        }

        #endregion
    }
}