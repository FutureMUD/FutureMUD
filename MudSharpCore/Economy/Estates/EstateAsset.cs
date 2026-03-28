using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Estates;

public class EstateAsset : SaveableItem, IEstateAsset
{
	public EstateAsset(MudSharp.Models.EstateAsset asset, IEstate estate)
	{
		Gameworld = estate.Gameworld;
		_id = asset.Id;
		Estate = estate;
		_assetReference = new FrameworkItemReference(asset.FrameworkItemId, asset.FrameworkItemType, Gameworld);
		IsPresumedOwnership = asset.IsPresumedOwnership;
		_isTransferred = asset.IsTransferred;
		_isLiquidated = asset.IsLiquidated;
		_liquidatedValue = asset.LiquidatedValue;
	}

	public EstateAsset(IEstate estate, IFrameworkItem asset, bool presumedOwnership)
	{
		Gameworld = estate.Gameworld;
		Estate = estate;
		_assetReference = new FrameworkItemReference(asset.Id, asset.FrameworkItemType, Gameworld);
		IsPresumedOwnership = presumedOwnership;
		_isTransferred = false;
		_isLiquidated = false;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.EstateAsset
			{
				EstateId = estate.Id,
				FrameworkItemId = asset.Id,
				FrameworkItemType = asset.FrameworkItemType,
				IsPresumedOwnership = presumedOwnership,
				IsTransferred = false,
				IsLiquidated = false
			};
			FMDB.Context.EstateAssets.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "EstateAsset";

	public override void Save()
	{
		var dbitem = FMDB.Context.EstateAssets.Find(Id);
		dbitem.IsTransferred = IsTransferred;
		dbitem.IsLiquidated = IsLiquidated;
		dbitem.LiquidatedValue = LiquidatedValue;
		Changed = false;
	}

	private readonly FrameworkItemReference _assetReference;
	private IFrameworkItem _asset;
	public IEstate Estate { get; }
	public IFrameworkItem Asset => _asset ??= _assetReference.GetItem;
	public bool IsPresumedOwnership { get; }
	private bool _isTransferred;
	public bool IsTransferred
	{
		get => _isTransferred;
		set
		{
			_isTransferred = value;
			Changed = true;
		}
	}

	private bool _isLiquidated;
	public bool IsLiquidated
	{
		get => _isLiquidated;
		set
		{
			_isLiquidated = value;
			Changed = true;
		}
	}

	private decimal? _liquidatedValue;
	public decimal? LiquidatedValue
	{
		get => _liquidatedValue;
		set
		{
			_liquidatedValue = value;
			Changed = true;
		}
	}
}
