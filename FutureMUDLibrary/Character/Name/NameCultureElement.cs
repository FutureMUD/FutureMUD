using System.Xml.Linq;

namespace MudSharp.Character.Name {
    public record NameCultureElement {
        public NameCultureElement(XElement definition) {
            Usage = (NameUsage) int.Parse(definition.Attribute("Usage").Value);
            MinimumCount = int.Parse(definition.Attribute("MinimumCount").Value);
            MaximumCount = int.Parse(definition.Attribute("MaximumCount").Value);
            Name = definition.Attribute("Name").Value;
            ChargenBlurb = definition.Value;
        }

        public NameCultureElement()
        {
        }

        public XElement SaveToXml()
        {
            return new XElement("Element", 
                new XAttribute("Usage", (int)Usage),
                new XAttribute("MinimumCount", MinimumCount),
                new XAttribute("MaximumCount", MaximumCount),
                new XAttribute("Name", Name),
                new XCData(ChargenBlurb)
            );
        }

        /// <summary>
        ///     The way in which this name element is used in name patterns
        /// </summary>
        public NameUsage Usage { get; init; }

        /// <summary>
        ///     The minimum number of these possessed by an individual
        /// </summary>
        public int MinimumCount { get; init; }

        /// <summary>
        ///     The maximum number of these possessed by an individual
        /// </summary>
        public int MaximumCount { get; init; }

        /// <summary>
        ///     The name of this element, e.g. "Given Name"
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        ///     A short text blurb shown at character creation when selecting values for this NameCultureElement
        /// </summary>
        public string ChargenBlurb { get; init; }
    }
}