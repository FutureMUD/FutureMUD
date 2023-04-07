using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Work.Crafts
{
    public interface ICraftInputData {
        XElement SaveToXml();
        IPerceivable Perceivable { get; }
        ItemQuality InputQuality { get; }
        void FinaliseLoadTimeTasks();
        void Delete();
        void Quit();
    }
}
