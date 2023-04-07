using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Work.Butchering;

public class ButcheryProductItem : IButcheryProductItem
{
	public IFuturemud Gameworld { get; set; }

	public ButcheryProductItem(Models.ButcheryProductItems item, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_normalProtoId = item.NormalProtoId;
		_damagedProtoId = item.DamagedProtoId;
		NormalQuantity = item.NormalQuantity;
		DamagedQuantity = item.DamagedQuantity;
		DamagedThreshold = item.DamageThreshold;
	}

	#region Implementation of IButcheryProductItem

	private readonly long _normalProtoId;

	/// <summary>
	/// The prototype to load in normal cases for this item
	/// </summary>
	public IGameItemProto NormalProto => Gameworld.ItemProtos.Get(_normalProtoId);

	private readonly long? _damagedProtoId;

	/// <summary>
	/// The prototype to load in cases where the damage threshold is exceeded. If null, load nothing.
	/// </summary>
	public IGameItemProto DamagedProto => Gameworld.ItemProtos.Get(_damagedProtoId ?? 0L);

	/// <summary>
	/// The quantity of items to load in the normal case
	/// </summary>
	public int NormalQuantity { get; set; }

	/// <summary>
	/// The quantity of items to load in the damaged case
	/// </summary>
	public int DamagedQuantity { get; set; }

	/// <summary>
	/// The threshold in terms of damage percentage to parts beyond which this item is considered "damaged"
	/// </summary>
	public double DamagedThreshold { get; set; }

	#endregion
}