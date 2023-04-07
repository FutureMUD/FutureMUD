using System.Xml.Linq;

namespace MudSharp.Framework {
    public interface IXmlSavable {
        /// <summary>
        ///     This function causes the object to be saved to a specified XML filepath
        /// </summary>
        /// <param name="filepath">The full or relative filepath of the XML file to save</param>
        /// <returns>Returns true if the save was completed properly</returns>
        XElement SaveToXml();
    }
}