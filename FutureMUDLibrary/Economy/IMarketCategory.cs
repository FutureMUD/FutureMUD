using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Economy;
#nullable enable
public interface IMarketCategory : ISaveable, IEditableItem
{
	string Description { get; }
	bool BelongsToCategory(IGameItem item);
	bool BelongsToCategory(IGameItemProto proto);
	double ElasticityFactorAbove { get; }
	double ElasticityFactorBelow { get; }
	IMarketCategory Clone(string newName);
}