using System.Xml.Linq;

namespace MudSharp.Framework {
    public interface IXmlLoadable {
        /// <summary>
        ///     This function causes the object to be loaded from a specified XML filepath
        /// </summary>
        /// <param name="filepath">The full or relative filepath of the XML file to load</param>
        /// <returns>Returns true if the load was completed properly</returns>
        void LoadFromXml(XElement root);
    }
}