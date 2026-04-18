using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MudSharp.TimeAndDate.Date
{
    public class IntercalaryRule : IXmlSavable, IXmlLoadable
    {
        public void LoadFromXml(XElement root)
        {
            if (root?.HasElements != true)
            {
                throw new XmlException("Missing or empty Intercalary Rule in LoadFromXml.");
            }

            // Offset
            XElement element = root.Element("offset");
            if ((element == null) || (element.Value.Length == 0))
            {
                throw new XmlException("Missing nominalorder value in IntercalaryRule LoadFromXML.");
            }

            try
            {
                Offset = int.Parse(element.Value);
            }
            catch
            {
                throw new XmlException("Value for nominalorder in IntercalaryRule LoadFromXML is not a valid Integer");
            }

            // Divisor
            element = root.Element("divisor");
            if ((element == null) || (element.Value.Length == 0))
            {
                throw new XmlException("Missing divisor value in IntercalaryRule LoadFromXML.");
            }

            try
            {
                DivisibleBy = int.Parse(element.Value);
            }
            catch
            {
                throw new XmlException("Value for divisor in IntercalaryRule LoadFromXML is not a valid Integer");
            }

            // Exceptions
            element = root.Element("exceptions");
            if (element?.HasElements == true)
            {
                foreach (XElement subElement in element.Elements("intercalaryrule"))
                {
                    IntercalaryRule rule = new();
                    rule.LoadFromXml(subElement);
                    _exceptions.Add(rule);
                }
            }

            // And conditions
            element = root.Element("ands");
            if (element?.HasElements == true)
            {
                foreach (XElement subElement in element.Elements("intercalaryrule"))
                {
                    IntercalaryRule rule = new();
                    rule.LoadFromXml(subElement);
                    _andConditions.Add(rule);
                }
            }

            // Or conditions
            element = root.Element("ors");
            if (element?.HasElements == true)
            {
                foreach (XElement subElement in element.Elements("intercalaryrule"))
                {
                    IntercalaryRule rule = new();
                    rule.LoadFromXml(subElement);
                    _orConditions.Add(rule);
                }
            }
        }

        public XElement SaveToXml()
        {
            return new XElement
            (
                "intercalaryrule", new XElement("offset", Offset), new XElement("divisor", DivisibleBy),
                new XElement("exceptions",
                    new object[] {
                        from ex in _exceptions
                        select ex.SaveToXml()
                    }
                ), new XElement("ands",
                    new object[] {
                        from an in _andConditions
                        select an.SaveToXml()
                    }
                ), new XElement("ors",
                    new object[] {
                        from or in _orConditions
                        select or.SaveToXml()
                    }
                ));
        }

        #region Methods

        /// <summary>
        ///     This method calculates whether this intercalary rule is true for the specified year value, considering all
        ///     exceptions and other logical conditions
        /// </summary>
        /// <param name="whichYear">The year to be compared</param>
        /// <returns>Returns true is the intercalary year should be applied for year whichYear</returns>
        public bool IsIntercalaryYear(int whichYear)
        {
            return (((whichYear - Offset).Modulus(DivisibleBy) == 0) &&
                    !_exceptions.Any(x => x.IsIntercalaryYear(whichYear)) &&
                    (!_andConditions.Any() || _andConditions.All(x => x.IsIntercalaryYear(whichYear)))) ||
                   _orConditions.Any(x => x.IsIntercalaryYear(whichYear));
        }

        #endregion

        #region Overrides of Object

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append($"divisible by {DivisibleBy}");
            foreach (IntercalaryRule condition in AndConditions)
            {
                sb.Append($" and {condition}");
            }
            foreach (IntercalaryRule condition in OrConditions)
            {
                sb.Append($" or {condition}");
            }
            foreach (IntercalaryRule condition in Exceptions)
            {
                sb.Append($" unless {condition}");
            }
            return sb.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Apply intercalary rule on years divisible by this number
        /// </summary>
        protected int _divisibleBy;

        public int DivisibleBy
        {
            get => _divisibleBy; set => _divisibleBy = value;
        }

        /// <summary>
        ///     Offset in whole years subtracted from number compared by divisible rule, e.g. with an offset of 3, the year 2003
        ///     would become a "divisible by 2000" year
        /// </summary>
        protected int _offset;

        public int Offset
        {
            get => _offset; set => _offset = value;
        }

        #endregion

        #region Vars

        /// <summary>
        ///     If these exceptions are true, this rule is false
        /// </summary>
        protected List<IntercalaryRule> _exceptions = new();

        public List<IntercalaryRule> Exceptions
        {
            get => _exceptions; protected set => _exceptions = value;
        }

        /// <summary>
        ///     Conditions which must also be true for this rule to be true
        /// </summary>
        protected List<IntercalaryRule> _andConditions = new();

        public List<IntercalaryRule> AndConditions
        {
            get => _andConditions; protected set => _andConditions = value;
        }

        /// <summary>
        ///     Conditions which are also checked if this condition is false
        /// </summary>
        protected List<IntercalaryRule> _orConditions = new();

        public List<IntercalaryRule> OrConditions
        {
            get => _orConditions; protected set => _orConditions = value;
        }

        #endregion

        #region Constructor

        public IntercalaryRule()
        {
        }

        public IntercalaryRule(int divisor, int offset)
        {
            _offset = offset;
            _divisibleBy = divisor;
        }

        #endregion
    }
}