using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Property;

public class PropertyKey : SaveableItem, IPropertyKey
{
	public override string FrameworkItemType => "PropertyKey";

	public PropertyKey(Models.PropertyKey key, IProperty property, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_property = property;
		_id = key.Id;
		_name = key.Name;
		_isReturned = key.IsReturned;
		_addedToPropertyOnDate = new MudDateTime(key.AddedToPropertyOnDate, gameworld);
		_costToReplace = key.CostToReplace;
		_gameItem = Gameworld.TryGetItem(key.GameItemId, true);
	}

	public PropertyKey(IFuturemud gameworld, IProperty property, string name, IGameItem item, MudDateTime addedDate,
		decimal costToReplace)
	{
		Gameworld = gameworld;
		_property = property;
		_name = name;
		_gameItem = item;
		_addedToPropertyOnDate = addedDate;
		_costToReplace = costToReplace;
		_isReturned = true;
		using (new FMDB())
		{
			var dbitem = new Models.PropertyKey
			{
				Name = Name,
				PropertyId = property.Id,
				GameItemId = GameItem.Id,
				IsReturned = IsReturned,
				AddedToPropertyOnDate = AddedToPropertyOnDate.GetDateTimeString(),
				CostToReplace = CostToReplace
			};
			FMDB.Context.PropertyKeys.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.PropertyKeys.Find(Id);
		dbitem.Name = Name;
		dbitem.GameItemId = GameItem.Id;
		dbitem.IsReturned = IsReturned;
		dbitem.AddedToPropertyOnDate = AddedToPropertyOnDate.GetDateTimeString();
		dbitem.CostToReplace = CostToReplace;
		Changed = false;
	}

	#endregion

	#region Implementation of IPropertyKey

	private IProperty _property;

	public IProperty Property
	{
		get => _property;
		set => _property = value;
	}

	private IGameItem _gameItem;

	public IGameItem GameItem
	{
		get => _gameItem;
		set => _gameItem = value;
	}

	private MudDateTime _addedToPropertyOnDate;

	public MudDateTime AddedToPropertyOnDate
	{
		get => _addedToPropertyOnDate;
		set => _addedToPropertyOnDate = value;
	}

	private decimal _costToReplace;

	public decimal CostToReplace
	{
		get => _costToReplace;
		set => _costToReplace = value;
	}

	private bool _isReturned;

	public bool IsReturned
	{
		get => _isReturned;
		set => _isReturned = value;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.PropertyKeys.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.PropertyKeys.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	#endregion
}