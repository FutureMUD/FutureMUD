#nullable enable
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Economy;
public interface IConsumerGroup : ISaveable, IEditableItem
{
	string Description { get; }
}

public interface IConsumerGroupNeed : ISaveable, IEditableItem
{
	string Description { get; }
	IConsumerGroup ConsumerGroup { get; }
	IMarketCategory MarketCategory { get; }
}