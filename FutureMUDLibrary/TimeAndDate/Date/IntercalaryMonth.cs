using MudSharp.Framework;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.TimeAndDate.Date
{
    public class IntercalaryMonth : IXmlLoadable, IXmlSavable
    {
        public void LoadFromXml(XElement root)
        {
            if (root?.HasElements != true)
            {
                throw new XmlException("Empty or Missing Intercalary Month in LoadFromXml.");
            }

            // Normal Days
            XElement element = root.Element("position");
            if ((element == null) || (element.Value.Length == 0))
            {
                throw new XmlException("Missing position value in IntercalaryMonth LoadFromXML.");
            }

            InsertPosition = element.Value;

            // Rule
            element = root.Element("intercalaryrule");
            if (element?.HasElements != true)
            {
                throw new XmlException("Missing or empty rule in IntercalaryMonth LoadFromXml.");
            }

            IntercalaryRule rule = new();
            rule.LoadFromXml(element);
            Rule = rule;

            // Month
            element = root.Element("month");
            if (element?.HasElements != true)
            {
                throw new XmlException("Missing or empty month in IntercalaryMonth LoadFromXml.");
            }

            MonthDefinition month = new();
            month.LoadFromXml(element);
            Month = month;
        }

        public XElement SaveToXml()
        {
            return new XElement
            (
                "intercalarymonth", new XElement("position", InsertPosition), Month.SaveToXml(), Rule.SaveToXml());
        }

        #region Properties

        /// <summary>
        ///     The rules by which this Intercalary is evaluated
        /// </summary>
        protected IntercalaryRule _rule;

        public IntercalaryRule Rule
        {
            get => _rule; protected set => _rule = value;
        }

        /// <summary>
        ///     The month definition inserted by the intercalary rule
        /// </summary>
        protected MonthDefinition _month;

        public MonthDefinition Month
        {
            get => _month; protected set => _month = value;
        }

        /// <summary>
        ///     The alias of the month before which this intercalary should be inserted
        /// </summary>
        protected string _insertPosition;

        public string InsertPosition
        {
            get => _insertPosition; protected set => _insertPosition = value;
        }

        #endregion

        #region Constructor

        public IntercalaryMonth()
        {
        }

        public IntercalaryMonth(IntercalaryRule rule, MonthDefinition month, string insertposition)
        {
            _rule = rule;
            _month = month;
            _insertPosition = insertposition;
        }

        #endregion
    }
}