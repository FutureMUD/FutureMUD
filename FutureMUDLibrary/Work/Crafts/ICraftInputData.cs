using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

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
	void ReleaseItemsAtCraftCompletion(ICell location, RoomLayer layer)
	{
		// Do nothing
	}
}