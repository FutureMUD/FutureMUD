using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Date {
    public class IntercalaryDay : IXmlSavable, IXmlLoadable {
        /// <summary>
        ///     Number of new calendar days to insert for this month
        /// </summary>
        protected int _insertNumNewDays;

        /// <summary>
        ///     Add these days to the month's list of days which do not count as weekdays
        /// </summary>
        protected List<int> _nonWeekdays = new();

        /// <summary>
        ///     Remove these days from the month's list of days which do not count as weekdays
        /// </summary>
        protected List<int> _removeNonWeekdays = new();

        /// <summary>
        ///     Original special day names to remove
        /// </summary>
        protected List<int> _removeSpecialDayNames = new();

        /// <summary>
        ///     The rules by which this Intercalary is evaluated
        /// </summary>
        protected IntercalaryRule _rule = new();

        /// <summary>
        ///     New special day names to insert
        /// </summary>
        protected Dictionary<int, DayName> _specialDayNames = new();

        public IntercalaryRule Rule
        {
            get { return _rule; }
            set { _rule = value; }
        }

        public Dictionary<int, DayName> SpecialDayNames => _specialDayNames;

        public List<int> NonWeekdays => _nonWeekdays;

        public List<int> RemoveNonWeekdays => _removeNonWeekdays;

        public List<int> RemoveSpecialDayNames => _removeSpecialDayNames;

        public int InsertNumnewDays
        {
            get { return _insertNumNewDays; }
            set { _insertNumNewDays = value; }
        }

        public void LoadFromXml(XElement root) {
            if (root?.HasElements != true) {
                throw new XmlException("Empty IntercalaryDay in LoadFromXML.");
            }

            // Normal Days
            var element = root.Element("insertdays");
            if ((element == null) || (element.Value.Length == 0)) {
                throw new XmlException("Missing insertdays value in IntercalaryDay LoadFromXML.");
            }

            try {
                InsertNumnewDays = int.Parse(element.Value);
            }
            catch {
                throw new XmlException("Value for normaldays in IntercalaryDay LoadFromXML is not a valid Integer");
            }

            // Special Day Names
            if (root.Element("specialdays").HasElements) {
                (from sd in root.Element("specialdays").Elements("specialday")
                 where (sd.Attribute("day") != null) &&
                       (sd.Attribute("short") != null) &&
                       (sd.Attribute("long") != null)
                 select sd)
                    .ToList()
                    .ForEach(x => SpecialDayNames
                                 .Add(int.Parse(x.Attribute("day").Value.ToString()),
                                      new DayName(x.Attribute("short").Value, x.Attribute("long").Value))
                    );
            }

            // Remove Special Day Names
            if (root.Element("removespecialdays").HasElements) {
                (from nwd in root.Element("removespecialdays").Elements("removespecialday")
                 select nwd)
                    .ToList()
                    .ForEach(x => RemoveSpecialDayNames.Add(int.Parse(x.Value)));
            }

            // Non Weekdays
            if (root.Element("nonweekdays").HasElements) {
                (from nwd in root.Element("nonweekdays").Elements("nonweekday")
                 select nwd)
                    .ToList()
                    .ForEach(x => NonWeekdays.Add(int.Parse(x.Value)));
            }

            // Remove Non Weekdays
            if (root.Element("removenonweekdays").HasElements) {
                (from nwd in root.Element("removenonweekdays").Elements("removenonweekday")
                 select nwd)
                    .ToList()
                    .ForEach(x => RemoveNonWeekdays.Add(int.Parse(x.Value)));
            }

            // Rule
            element = root.Element("intercalaryrule");
            if (element?.HasElements != true) {
                throw new XmlException("Missing or empty rule in IntercalaryDay LoadFromXml.");
            }

            var rule = new IntercalaryRule();
            rule.LoadFromXml(element);
            Rule = rule;
        }

        public XElement SaveToXml() {
            return new XElement
            (
                "intercalaryday", new XElement("insertdays", InsertNumnewDays), new XElement("nonweekdays",
                    new object[] {
                        from nwd in NonWeekdays
                        select new XElement("nonweekday", nwd)
                    }
                ), new XElement("removenonweekdays",
                    new object[] {
                        from rnwd in RemoveNonWeekdays
                        select new XElement("removenonweekday", rnwd)
                    }
                ), new XElement("specialdays",
                    new object[] {
                        from sd in SpecialDayNames
                        select
                        new XElement("specialday", new XAttribute("day", sd.Key),
                            new XAttribute("short", sd.Value.ShortName), new XAttribute("long", sd.Value.FullName))
                    }
                ), new XElement("removespecialdays",
                    new object[] {
                        from rsd in RemoveSpecialDayNames
                        select new XElement("removespecialday", rsd)
                    }
                ), Rule.SaveToXml());
        }
    }
}