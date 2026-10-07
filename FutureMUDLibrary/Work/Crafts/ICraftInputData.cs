using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Form.Shape;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts;

public interface ICraftInputData
{
    XElement SaveToXml();
    IPerceivable Perceivable { get; }
    ItemQuality InputQuality { get; }
    void FinaliseLoadTimeTasks();
    void Delete();
    void Quit();
}

public interface ICraftInputDataWithItems : ICraftInputData
{
    IEnumerable<IGameItem> ConsumedItems { get; }
    void ReleaseItemsAtCraftCompletion(IRoom location, RoomLayer layer)
    {
        // Do nothing
    }

    void ReleaseItemsAtCraftCompletion(ILocateable source, IRoom location, RoomLayer layer)
    {
        ReleaseItemsAtCraftCompletion(location, layer);
    }
}
