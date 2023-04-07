using System;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.GameItems.Inventory.Size;

public class WearableSize : FrameworkItem, IWearableSize, ISaveable
{
	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	public override string FrameworkItemType => "WearableSize";

	#region IWearableSize Members

	public double Height { get; protected set; }

	public double Weight { get; protected set; }

	public IBodyPrototype Body { get; protected set; }

	public double TraitValue { get; protected set; }

	public IWearableSize SetSize(IBody body)
	{
		Height = body.Height;
		Weight = body.Weight;
		Body = body.Prototype;
		TraitValue = body.SizeRules.IgnoreAttribute ? 0 : body.TraitValue(body.SizeRules.WhichTrait);
		Changed = true;
		return this;
	}

	public IWearableSize SetSize(IWearableSize size)
	{
		Height = size.Height;
		Weight = size.Weight;
		Body = size.Body;
		TraitValue = size.TraitValue;
		Changed = true;
		return this;
	}

	public IWearableSize Clone()
	{
		using (new FMDB())
		{
			var dbsize = new Models.WearableSize
			{
				Height = Height,
				Weight = Weight,
				TraitValue = TraitValue,
				BodyPrototypeId = Body.Id,
				OneSizeFitsAll = false
			};
			FMDB.Context.WearableSizes.Add(dbsize);
			FMDB.Context.SaveChanges();
			var size = new WearableSize(dbsize, Gameworld);
			Gameworld.Add(size);
			return size;
		}
	}

	public bool CanWear(IBody body)
	{
		var rules = body.SizeRules;
		return
			body.Prototype.CountsAs(Body) &&
			body.Height * rules.MinHeightFactor >= Height &&
			body.Height * rules.MaxHeightFactor <= Height &&
			body.Weight * rules.MinWeightFactor >= Weight &&
			body.Weight * rules.MaxWeightFactor <= Weight &&
			(
				rules.IgnoreAttribute ||
				(body.TraitValue(rules.WhichTrait) * rules.MinTraitFactor >= TraitValue &&
				 body.TraitValue(rules.WhichTrait) * rules.MaxTraitFactor <= TraitValue)
			);
	}

	public Tuple<ItemVolumeFitDescription, ItemLinearFitDescription> DescribeFit(IBody body)
	{
		return Tuple.Create(
			body.SizeRules.IgnoreAttribute
				? body.SizeRules.WeightVolumeRatios.Find(body.Weight / Weight)
				: new[]
					{
						body.SizeRules.WeightVolumeRatios.Find(body.Weight / Weight),
						body.SizeRules.TraitVolumeRatios.Find(body.TraitValue(body.SizeRules.WhichTrait) / TraitValue)
					}
					.FirstMax(x => x.Score()),
			body.SizeRules.HeightLinearRatios.Find(body.Height / Height)
		);
	}

	#endregion

	#region Constructors

	public static IWearableSize LoadSize(MudSharp.Models.WearableSize size, IFuturemud gameworld)
	{
		return !size.OneSizeFitsAll
			? new WearableSize(size, gameworld)
			: (IWearableSize)new WearableSizeUniversal(size, gameworld);
	}

	public WearableSize(MudSharp.Models.WearableSize size, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = size.Id;
		Body = gameworld.BodyPrototypes.Get(size.BodyPrototypeId);
		Height = size.Height ?? 0;
		Weight = size.Weight ?? 0;
		TraitValue = size.TraitValue ?? 0;
	}

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
			dbsize.Height = Height;
			dbsize.Weight = Weight;
			dbsize.TraitValue = TraitValue;
			dbsize.BodyPrototypeId = Body.Id;
			dbsize.OneSizeFitsAll = false;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion
}