using System;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.GameItems.Inventory.Size;

public class WearableSizeUniversal : FrameworkItem, IWearableSize, ISaveable
{
	protected IBodyPrototype _body;

	public WearableSizeUniversal(MudSharp.Models.WearableSize size, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = size.Id;
		_body = gameworld.BodyPrototypes.Get(size.BodyPrototypeId);
	}

	public WearableSizeUniversal(IFuturemud gameworld, IBody sizefor, bool temporary = false)
	{
		Gameworld = gameworld;
		if (sizefor != null)
		{
			_body = sizefor.Prototype;
		}

		if (!temporary)
		{
			using (new FMDB())
			{
				var dbsize = new Models.WearableSize
				{
					BodyPrototypeId = _body?.Id ?? 0,
					OneSizeFitsAll = true
				};
				FMDB.Context.WearableSizes.Add(dbsize);
				FMDB.Context.SaveChanges();
				_id = dbsize.Id;
			}

			Changed = false;
		}
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	public override string FrameworkItemType => "WearableSizeUniversal";

	#region IWearableSize Members

	public IWearableSize SetSize(IBody body)
	{
		_body = body?.Prototype;
		Changed = true;
		return this;
	}

	public IWearableSize SetSize(IWearableSize size)
	{
		return size.Clone();
	}

	public IWearableSize Clone()
	{
		using (new FMDB())
		{
			var dbsize = new Models.WearableSize
			{
				BodyPrototypeId = _body?.Id ?? 0,
				OneSizeFitsAll = true
			};
			FMDB.Context.WearableSizes.Add(dbsize);
			FMDB.Context.SaveChanges();
			return new WearableSizeUniversal(dbsize, Gameworld);
		}
	}

	public bool CanWear(IBody body)
	{
		return _body == null || _body == body.Prototype;
	}

	public Tuple<ItemVolumeFitDescription, ItemLinearFitDescription> DescribeFit(IBody body)
	{
		return Tuple.Create(ItemVolumeFitDescription.Normal, ItemLinearFitDescription.Normal);
	}

	public double Height => 0.0;

	public double Weight => 0.0;

	public IBodyPrototype Body => _body;

	public double TraitValue => 0.0;

	#endregion

	#region ISaveable Members

	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value && !_changed)
			{
				Gameworld.SaveManager.Add(this);
			}

			_changed = value;
		}
	}

	public void Save()
	{
		using (new FMDB())
		{
			var dbsize = FMDB.Context.WearableSizes.Find(Id);
			dbsize.BodyPrototypeId = _body?.Id ?? 0;
			dbsize.OneSizeFitsAll = true;
			FMDB.Context.SaveChanges();
		}
	}

	#endregion
}