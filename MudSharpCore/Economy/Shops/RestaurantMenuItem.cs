using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Crafts;
using DbRestaurantMenuItem = MudSharp.Models.RestaurantMenuItem;

#nullable enable

namespace MudSharp.Economy.Shops;

public sealed class RestaurantMenuItem : SaveableItem, IRestaurantMenuItem
{
	private string _description;
	private RestaurantFulfilmentMode _fulfilmentMode;
	private bool _isActive;
	private bool _dineInAvailable;
	private bool _takeawayAvailable;
	private TimeSpan _preparationTime;
	private long? _craftId;
	private int? _craftRevisionNumber;
	private ICraft? _craft;
	private long? _servingContainerPrototypeId;
	private int? _servingContainerPrototypeRevisionNumber;
	private IGameItemProto? _servingContainerPrototype;
	private long? _takeawayContainerPrototypeId;
	private int? _takeawayContainerPrototypeRevisionNumber;
	private IGameItemProto? _takeawayContainerPrototype;
	private long? _takeawayBagPrototypeId;
	private int? _takeawayBagPrototypeRevisionNumber;
	private IGameItemProto? _takeawayBagPrototype;

	public RestaurantMenuItem(Restaurant restaurant, IMerchandise merchandise)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		Merchandise = merchandise;
		_name = merchandise.Name;
		_description = merchandise.ListDescription;
		_fulfilmentMode = RestaurantFulfilmentMode.BringUnaltered;
		_isActive = true;
		_dineInAvailable = true;
		_takeawayAvailable = true;
		_preparationTime = TimeSpan.Zero;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantMenuItem
			{
				RestaurantShopId = restaurant.Id,
				MerchandiseId = merchandise.Id,
				Description = _description,
				FulfilmentMode = (int)_fulfilmentMode,
				IsActive = true,
				DineInAvailable = true,
				TakeawayAvailable = true,
				PreparationSeconds = 0,
				SortOrder = restaurant.MenuItems.Any() ? restaurant.MenuItems.Max(x => (x as RestaurantMenuItem)?.SortOrder ?? 0) + 1 : 0
			};
			FMDB.Context.RestaurantMenuItems.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			SortOrder = dbitem.SortOrder;
		}
	}

	public RestaurantMenuItem(DbRestaurantMenuItem item, Restaurant restaurant)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		_id = item.Id;
		Merchandise = restaurant.Merchandises.FirstOrDefault(x => x.Id == item.MerchandiseId)!;
		_name = Merchandise?.Name ?? $"Missing merchandise #{item.MerchandiseId:N0}";
		_description = item.Description;
		_fulfilmentMode = Enum.IsDefined(typeof(RestaurantFulfilmentMode), item.FulfilmentMode)
			? (RestaurantFulfilmentMode)item.FulfilmentMode
			: RestaurantFulfilmentMode.BringUnaltered;
		_isActive = item.IsActive;
		_dineInAvailable = item.DineInAvailable;
		_takeawayAvailable = item.TakeawayAvailable;
		_preparationTime = TimeSpan.FromSeconds(Math.Max(0, item.PreparationSeconds));
		_craftId = item.CraftId;
		_craftRevisionNumber = item.CraftRevisionNumber;
		_servingContainerPrototypeId = item.ServingContainerPrototypeId;
		_servingContainerPrototypeRevisionNumber = item.ServingContainerPrototypeRevisionNumber;
		_takeawayContainerPrototypeId = item.TakeawayContainerPrototypeId;
		_takeawayContainerPrototypeRevisionNumber = item.TakeawayContainerPrototypeRevisionNumber;
		_takeawayBagPrototypeId = item.TakeawayBagPrototypeId;
		_takeawayBagPrototypeRevisionNumber = item.TakeawayBagPrototypeRevisionNumber;
		SortOrder = item.SortOrder;
	}

	public override string FrameworkItemType => "RestaurantMenuItem";
	public IRestaurant Restaurant { get; }
	public IMerchandise Merchandise { get; } = null!;
	public int SortOrder { get; set; }

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Changed = true;
		}
	}

	public RestaurantFulfilmentMode FulfilmentMode
	{
		get => _fulfilmentMode;
		set
		{
			_fulfilmentMode = value;
			Changed = true;
		}
	}

	public bool IsActive
	{
		get => _isActive;
		set
		{
			_isActive = value;
			Changed = true;
		}
	}

	public bool DineInAvailable
	{
		get => _dineInAvailable;
		set
		{
			_dineInAvailable = value;
			Changed = true;
		}
	}

	public bool TakeawayAvailable
	{
		get => _takeawayAvailable;
		set
		{
			_takeawayAvailable = value;
			Changed = true;
		}
	}

	public TimeSpan PreparationTime
	{
		get => _preparationTime;
		set
		{
			_preparationTime = value < TimeSpan.Zero ? TimeSpan.Zero : value;
			Changed = true;
		}
	}

	public ICraft? Craft
	{
		get => _craft ??= _craftId.HasValue
			? Gameworld.Crafts.Get(_craftId.Value, _craftRevisionNumber ?? 0)
			: null;
		set
		{
			_craft = value;
			_craftId = value?.Id;
			_craftRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}

	public IGameItemProto? ServingContainerPrototype
	{
		get => _servingContainerPrototype ??= _servingContainerPrototypeId.HasValue
			? Gameworld.ItemProtos.Get(_servingContainerPrototypeId.Value, _servingContainerPrototypeRevisionNumber ?? 0)
			: null;
		set
		{
			_servingContainerPrototype = value;
			_servingContainerPrototypeId = value?.Id;
			_servingContainerPrototypeRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}

	public IGameItemProto? TakeawayContainerPrototype
	{
		get => _takeawayContainerPrototype ??= _takeawayContainerPrototypeId.HasValue
			? Gameworld.ItemProtos.Get(_takeawayContainerPrototypeId.Value, _takeawayContainerPrototypeRevisionNumber ?? 0)
			: null;
		set
		{
			_takeawayContainerPrototype = value;
			_takeawayContainerPrototypeId = value?.Id;
			_takeawayContainerPrototypeRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}

	public IGameItemProto? TakeawayBagPrototype
	{
		get => _takeawayBagPrototype ??= _takeawayBagPrototypeId.HasValue
			? Gameworld.ItemProtos.Get(_takeawayBagPrototypeId.Value, _takeawayBagPrototypeRevisionNumber ?? 0)
			: null;
		set
		{
			_takeawayBagPrototype = value;
			_takeawayBagPrototypeId = value?.Id;
			_takeawayBagPrototypeRevisionNumber = value?.RevisionNumber;
			Changed = true;
		}
	}

	public bool IsValid(out string reason)
	{
		if (Merchandise is null)
		{
			reason = "its stock merchandise no longer exists";
			return false;
		}

		var craft = Craft;
		var servingContainer = ServingContainerPrototype;
		var takeawayContainer = TakeawayContainerPrototype;
		var takeawayBag = TakeawayBagPrototype;
		return RestaurantServiceRules.ValidateFulfilmentConfiguration(
			FulfilmentMode,
			DineInAvailable,
			TakeawayAvailable,
			craft is not null,
			craft is not null && craft.CraftIsValid && craft.Products.Any(x => x.RefersToItemProto(Merchandise.Item.Id)),
			Merchandise.Item.IsItemType<IOpenablePrototype>(),
			servingContainer is not null,
			servingContainer is null || servingContainer.IsItemType<IContainerPrototype>(),
			takeawayContainer is not null,
			takeawayContainer is null || takeawayContainer.IsItemType<IContainerPrototype>(),
			takeawayBag is not null,
			takeawayBag is null || takeawayBag.IsItemType<IContainerPrototype>(),
			out reason);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantMenuItems.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Description = Description;
		dbitem.FulfilmentMode = (int)FulfilmentMode;
		dbitem.IsActive = IsActive;
		dbitem.DineInAvailable = DineInAvailable;
		dbitem.TakeawayAvailable = TakeawayAvailable;
		dbitem.PreparationSeconds = (int)Math.Ceiling(PreparationTime.TotalSeconds);
		dbitem.CraftId = Craft?.Id ?? _craftId;
		dbitem.CraftRevisionNumber = Craft?.RevisionNumber ?? _craftRevisionNumber;
		dbitem.ServingContainerPrototypeId = ServingContainerPrototype?.Id ?? _servingContainerPrototypeId;
		dbitem.ServingContainerPrototypeRevisionNumber = ServingContainerPrototype?.RevisionNumber ?? _servingContainerPrototypeRevisionNumber;
		dbitem.TakeawayContainerPrototypeId = TakeawayContainerPrototype?.Id ?? _takeawayContainerPrototypeId;
		dbitem.TakeawayContainerPrototypeRevisionNumber = TakeawayContainerPrototype?.RevisionNumber ?? _takeawayContainerPrototypeRevisionNumber;
		dbitem.TakeawayBagPrototypeId = TakeawayBagPrototype?.Id ?? _takeawayBagPrototypeId;
		dbitem.TakeawayBagPrototypeRevisionNumber = TakeawayBagPrototype?.RevisionNumber ?? _takeawayBagPrototypeRevisionNumber;
		dbitem.SortOrder = SortOrder;
		Changed = false;
	}
}
