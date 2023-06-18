using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Work.Butchering;

#nullable enable
public class ButcheryProductItem : SaveableItem, IButcheryProductItem
{
	public ButcheryProductItem(IButcheryProduct product, IGameItemProto proto, int quantity, IGameItemProto? failProto = null, int failQuantity = 0, double threshold = 1.0)
	{
		Gameworld = product.Gameworld;
		_normalProtoId = proto.Id;
		NormalQuantity = quantity;
		_damagedProtoId = failProto?.Id;
		DamagedQuantity = failQuantity;
		DamagedThreshold = threshold;
		using (new FMDB())
		{
			var dbitem = new Models.ButcheryProductItems
			{
				ButcheryProductId = product.Id,
				NormalProtoId = _normalProtoId,
				DamagedProtoId = _damagedProtoId,
				NormalQuantity = NormalQuantity,
				DamagedQuantity = DamagedQuantity,
				DamageThreshold = DamagedThreshold,
			};
			FMDB.Context.ButcheryProductItems.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public ButcheryProductItem(Models.ButcheryProductItems item, IFuturemud gameworld)
	{
		_id = item.Id;
		Gameworld = gameworld;
		_normalProtoId = item.NormalProtoId;
		_damagedProtoId = item.DamagedProtoId;
		NormalQuantity = item.NormalQuantity;
		DamagedQuantity = item.DamagedQuantity;
		DamagedThreshold = item.DamageThreshold;
	}

	#region Implementation of IButcheryProductItem

	private long _normalProtoId;

	/// <summary>
	/// The prototype to load in normal cases for this item
	/// </summary>
	public IGameItemProto NormalProto
	{
		get => Gameworld.ItemProtos.Get(_normalProtoId);
		set
		{
			_normalProtoId = value.Id;
		}
	}

	private long? _damagedProtoId;

	/// <summary>
	/// The prototype to load in cases where the damage threshold is exceeded. If null, load nothing.
	/// </summary>
	public IGameItemProto? DamagedProto
	{
		get => Gameworld.ItemProtos.Get(_damagedProtoId ?? 0L);
		set
		{
			_damagedProtoId = value?.Id;
		}
	}

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

	#region Overrides of FrameworkItem

	/// <inheritdoc />
	public override string FrameworkItemType => "ButcheryProductItem";

	#endregion

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.ButcheryProductItems.Find(Id);
		dbitem.NormalQuantity = NormalQuantity;
		dbitem.DamagedQuantity = DamagedQuantity;
		dbitem.NormalProtoId = _normalProtoId;
		dbitem.DamagedProtoId = _damagedProtoId;
		dbitem.DamageThreshold = DamagedThreshold;
		Changed = false;
	}

	#endregion

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ButcheryProductItems.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ButcheryProductItems.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}
}