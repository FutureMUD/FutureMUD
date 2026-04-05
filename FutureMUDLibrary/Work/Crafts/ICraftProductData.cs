using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts;

public interface ICraftProductData
{
    IPerceivable Perceivable { get; }
    XElement SaveToXml();
    void FinaliseLoadTimeTasks();
    void ReleaseProducts(ICell location, RoomLayer layer);
    void Delete();
    void Quit();
}

public interface ICraftProductDataWithItems : ICraftProductData
{
    IEnumerable<IGameItem> Products { get; }
}
