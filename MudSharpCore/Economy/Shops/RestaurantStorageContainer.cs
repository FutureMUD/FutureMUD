using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using DbRestaurantStorageContainer = MudSharp.Models.RestaurantStorageContainer;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// A physical kitchen container with one or more restaurant storage roles. The item itself is
/// deliberately referenced rather than copied so the normal item permissions and capacity rules
/// remain authoritative.
/// </summary>
public sealed class RestaurantStorageContainer : SaveableItem, IRestaurantStorageContainer
{
	private RestaurantStorageRole _roles;

	public RestaurantStorageContainer(Restaurant restaurant, IGameItem item, RestaurantStorageRole roles)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		GameItemId = item.Id;
		_roles = roles;
		_id = item.Id;
	}

	public RestaurantStorageContainer(DbRestaurantStorageContainer item, Restaurant restaurant)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		GameItemId = item.GameItemId;
		_roles = (RestaurantStorageRole)item.Roles;
		_id = item.GameItemId;
	}

	public override string FrameworkItemType => "RestaurantStorageContainer";
	public IRestaurant Restaurant { get; }
	public long GameItemId { get; }
	public RestaurantStorageRole Roles => _roles;

	internal void SetRoles(RestaurantStorageRole roles)
	{
		_roles = roles;
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantStorageContainers.Find(Restaurant.Id, GameItemId);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Roles = (int)_roles;
		Changed = false;
	}
}
