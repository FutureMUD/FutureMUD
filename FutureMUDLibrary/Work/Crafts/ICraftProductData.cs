using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Work.Crafts
{
    public interface ICraftProductData
    {
        IPerceivable Perceivable { get; }
        XElement SaveToXml();
        void FinaliseLoadTimeTasks();
        void ReleaseProducts(ICell location, RoomLayer layer);
        void Delete();
        void Quit();
    }
}
