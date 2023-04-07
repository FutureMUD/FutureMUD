using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Property;

public class PropertyOwner : SaveableItem, IPropertyOwner, ILazyLoadDuringIdleTime
{
	public override void Save()
	{
		var dbitem = FMDB.Context.PropertyOwners.Find(Id);
		dbitem.RevenueAccountId = RevenueAccount?.Id;
		dbitem.ShareOfOwnership = ShareOfOwnership;
		Changed = false;
	}

	public void DoLoad()
	{
		_owner ??= _ownerReference.GetItem;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.PropertyOwners.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.PropertyOwners.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public string Describe(IPerceiver voyeur)
	{
		if (voyeur is ICharacter vch && vch.IsAdministrator())
		{
			switch (Owner)
			{
				case ICharacter ch:
					return
						$"{ch.HowSeen(ch, flags: PerceiveIgnoreFlags.IgnoreCanSee)} ({ch.PersonalName.GetName(NameStyle.FullName)}) (#{ch.Id.ToString("N0", voyeur)})";
				case IClan clan:
					return $"{clan.FullName.ColourValue()} (#{clan.Id.ToString("N0", voyeur)})";
				default:
					return $"{_ownerReference.FrameworkItemType} #{_ownerReference.Id}";
			}
		}

		switch (Owner)
		{
			case ICharacter ch:
				return $"{ch.PersonalName.GetName(NameStyle.FullName)}";
			case IClan clan:
				return $"{clan.FullName.ColourValue()}";
			default:
				return $"{_ownerReference.FrameworkItemType} #{_ownerReference.Id}";
		}
	}

	public PropertyOwner(Models.PropertyOwner owner, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = owner.Id;
		_ownerReference = new FrameworkItemReference
		{
			Gameworld = gameworld,
			Id = owner.FrameworkItemId,
			FrameworkItemType = owner.FrameworkItemType
		};
		_shareOfOwnership = owner.ShareOfOwnership;
		_revenueAccount = Gameworld.BankAccounts.Get(owner.RevenueAccountId ?? 0);
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public PropertyOwner(IFrameworkItem owner, decimal share, IBankAccount account, IProperty property)
	{
		Gameworld = property.Gameworld;
		_ownerReference = new FrameworkItemReference
		{
			Gameworld = Gameworld,
			Id = owner.Id,
			FrameworkItemType = owner.FrameworkItemType
		};
		_shareOfOwnership = share;
		_revenueAccount = account;
		using (new FMDB())
		{
			var dbitem = new Models.PropertyOwner
			{
				FrameworkItemId = owner.Id,
				FrameworkItemType = owner.FrameworkItemType,
				RevenueAccountId = RevenueAccount?.Id,
				ShareOfOwnership = ShareOfOwnership,
				PropertyId = property.Id
			};
			FMDB.Context.PropertyOwners.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	#region Implementation of IPropertyOwner

	private readonly FrameworkItemReference _ownerReference;
	private IFrameworkItem _owner;

	public IFrameworkItem Owner
	{
		get
		{
			if (_owner == null)
			{
				_owner = _ownerReference.GetItem;
			}

			return _owner;
		}
	}

	private decimal _shareOfOwnership;

	public decimal ShareOfOwnership
	{
		get => _shareOfOwnership;
		set
		{
			_shareOfOwnership = value;
			Changed = true;
		}
	}

	private IBankAccount _revenueAccount;

	public IBankAccount RevenueAccount
	{
		get => _revenueAccount;
		set
		{
			_revenueAccount = value;
			Changed = true;
		}
	}

	#endregion

	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "PropertyOwner";

	#endregion
}