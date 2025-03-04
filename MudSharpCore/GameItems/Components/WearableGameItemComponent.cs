using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MoreLinq.Extensions;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class WearableGameItemComponent : GameItemComponent, IWearable
{
	protected WearableGameItemComponentProto _prototype;
	private IWearProfile _currentProfile;

	public WearableGameItemComponent(WearableGameItemComponentProto proto, IGameItem parent, IBody sizefor,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		CurrentProfile = null;
		WornBy = null;
	}

	public WearableGameItemComponent(MudSharp.Models.GameItemComponent component, WearableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public WearableGameItemComponent(WearableGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		WornBy?.RemoveItem(Parent);
	}

	public override bool AffectsLocationOnDestruction => true;

	public override int ComponentDieOrder => 5;

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (WornBy == null)
		{
			return false;
		}

		var held = Parent.GetItemType<IHoldable>();
		if (held != null)
			// This is necessary so that the holdable component, which evalutes later, does not do its thing.
		{
			held.HeldBy = null;
		}

		var newItemWearable = newItem?.GetItemType<WearableGameItemComponent>();
		if (newItemWearable?.Profiles.Any(x => x.CompatibleWith(CurrentProfile)) == true)
		{
			WornBy.SwapInPlace(Parent, newItem);
		}
		else if (newItem != null)
		{
			if (WornBy.CanGet(newItem, 0))
			{
				WornBy.Get(newItem, silent: true);
			}
			else
			{
				WornBy.Location.Insert(newItem);
			}
		}

		WornBy?.Take(Parent);
		WornBy = null;
		CurrentProfile = null;
		return true;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new WearableGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return $"<Definition/>";
	}

	protected void LoadFromXml(XElement root)
	{
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (WearableGameItemComponentProto)newProto;
	}

	#region IWearable Members

	public bool GloballyTransparent
	{
		get
		{
			var destroyable = Parent.GetItemType<IDestroyable>();
			if (destroyable == null)
			{
				return false;
			}

			// TODO - effects that make clothes transparent
			var ratio = 1.0 - Parent.Wounds.Sum(x => x.CurrentDamage) / destroyable.MaximumDamage;
			return ratio <= _prototype.SeeThroughDamageRatio;
		}
	}

	public bool Waterproof
	{
		get
		{
			if (_prototype.Waterproof)
			{
				var destroyable = Parent.GetItemType<IDestroyable>();
				if (destroyable == null)
				{
					return true;
				}

				var ratio = 1.0 - Parent.Wounds.Sum(x => x.CurrentDamage) / destroyable.MaximumDamage;
				return ratio >= _prototype.WaterproofDamageRatio;
			}

			// TODO - effects that make items waterproof

			return false;
		}
	}

	public IWearProfile CurrentProfile
	{
		get => _currentProfile ?? DefaultProfile;
		protected set => _currentProfile = value;
	}

	public IBody WornBy { get; protected set; }

	public bool DisplayInventoryWhenWorn => _prototype.DisplayInventoryWhenWorn;

	public double LayerWeightConsumption => _prototype.LayerWeightConsumption;

	public IEnumerable<IWearProfile> Profiles => _prototype.Profiles;

	public IWearProfile DefaultProfile => _prototype.DefaultProfile;

	public virtual void UpdateWear(IBody body, IWearProfile profile)
	{
		WornBy = body;
		CurrentProfile = profile;
	}

	public bool CanWear(IBody wearer, IWearProfile profile)
	{
		if (Bulky)
		{
			foreach (var location in profile.Profile(wearer))
			{
				if (!location.Value.Mandatory)
				{
					continue;
				}

				foreach (var item in wearer.WornItemsFor(location.Key))
				{
					if (item.GetItemType<IWearable>()?.Bulky != true)
					{
						continue;
					}

					var local = wearer.WornItemsFullInfo.First(x => x.Item == item && x.Wearloc == location.Key);
					if (!local.Profile.Mandatory)
					{
						continue;
					}

					return false;
				}
			}
		}

		return true;
	}

	public WhyCannotDrapeReason WhyCannotWear(IBody wearer, IWearProfile profile)
	{
		if (Bulky)
		{
			foreach (var location in profile.Profile(wearer))
			{
				foreach (var item in wearer.WornItemsFor(location.Key))
				{
					if (item.GetItemType<IWearable>()?.Bulky != true)
					{
						continue;
					}

					var local = wearer.WornItemsFullInfo.First(x => x.Item == item && x.Wearloc == location.Key);
					if (!local.Profile.Mandatory)
					{
						continue;
					}

					return WhyCannotDrapeReason.TooBulky;
				}
			}
		}

		return WhyCannotDrapeReason.Unknown;
	}

	public bool CanWear(IBody wearer)
	{
		return _prototype.WearableProg?.ExecuteBool(wearer?.Actor, Parent) ?? true;
	}

	public WhyCannotDrapeReason WhyCannotWear(IBody wearer)
	{
		if (_prototype.WearableProg?.ExecuteBool(wearer?.Actor) == false)
		{
			return WhyCannotDrapeReason.ProgFailed;
		}

		return WhyCannotDrapeReason.Unknown;
	}

	public bool Bulky => _prototype.Bulky;

	public string WhyCannotWearProgText(IBody wearer)
	{
		return _prototype.WhyCannotWearProg?.Execute(wearer.Actor, Parent)?.ToString() ?? "You cannot wear that";
	}

	#endregion

	#region Overrides of GameItemComponent

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var destroyable = Parent.GetItemType<IDestroyable>();
		var ratio = 1.0 - Parent.Wounds.Sum(x => x.CurrentDamage) / destroyable?.MaximumDamage ?? 1.0;
		var sb = new StringBuilder();
		switch (type)
		{
			case DescriptionType.Short:
				if (destroyable != null && ratio <= _prototype.SeeThroughDamageRatio)
				{
					return description + " (tattered)".FluentColour(Telnet.BoldMagenta, colour);
				}

				return base.Decorate(voyeur, name, description, type, colour, flags);
			case DescriptionType.Full:
				sb.AppendLine(description);
				sb.AppendLine();
				if (voyeur is ICharacter ch)
				{
					if (Profiles.All(x => !ch.Body.Prototype.CountsAs(x.DesignedBody)))
					{
						sb.AppendLine("This item can be worn, but it is not designed for your body type.");
					}
					else
					{
						sb.AppendLine(
							$"This item can be worn in the following ways: {Profiles.Where(x => ch.Body.Prototype.CountsAs(x.DesignedBody)).Select(x => $"{x.Name.ToLowerInvariant().Colour(Telnet.Cyan)}{(DefaultProfile == x ? " (default)" : "")}").ListToString()}");
						if (Bulky)
						{
							sb.AppendLine("It is bulky, and cannot be worn with other bulky items.");
						}
					}
				}

				if (destroyable != null)
				{
					if (ratio <= _prototype.SeeThroughDamageRatio)
					{
						sb.AppendLine("It has been so damaged that it no longer hides what is worn below!");
					}
				}

				return sb.ToString();
			case DescriptionType.Evaluate:
				sb.AppendLine($"This is something that can be worn by those with a body type of {Profiles.Select(x => x.DesignedBody.Name.ColourValue()).ListToString(conjunction: "or ")}.");
				if (Bulky)
				{
					sb.AppendLine($"It is considered bulky, and cannot be worn over other bulky items.");
				}

				sb.AppendLine($"It consumes {LayerWeightConsumption.ToString("P2", voyeur).ColourValue()} of a layer for maximum layering purposes.");
				if (_prototype.Waterproof)
				{
					sb.AppendLine($"It is waterproof when less than {(1.0 - _prototype.WaterproofDamageRatio).ToString("P2", voyeur).ColourValue()} damaged.");
					sb.AppendLine($"It {(Waterproof ? "is" : "is not")} currently waterproof.");
				}
				else
				{
					sb.AppendLine($"It is not waterproof and will let fluids through.");
				}

				if (destroyable is not null)
				{
					sb.AppendLine(
						$"It will stop hiding items layered underneath at {(1.0 - _prototype.SeeThroughDamageRatio).ToString("P2", voyeur).ColourValue()} damage.");
				}

				sb.AppendLine();
				sb.AppendLine($"It will be worn in the {DefaultProfile.Name.ColourValue()} profile by default.");
				sb.AppendLine();
				sb.AppendLine("It has the following profiles:");
				foreach (var profile in Profiles.OrderByDescending(x => x == DefaultProfile).ThenBy(x => x.Name))
				{
					sb.AppendLine();
					sb.AppendLine(
						$"{profile.Name.TitleCase().ColourName()} for the {profile.DesignedBody.Name.TitleCase().ColourValue()} body");
					foreach (var loc in profile.AllProfiles)
					{
						var tags = new StringBuilder();
						if (!loc.Value.Mandatory)
						{
							tags.Append(" (Optional)".Colour(Telnet.BoldGreen));
						}

						if (loc.Value.Transparent)
						{
							tags.Append(" (Transparent)".Colour(Telnet.BoldCyan));
						}

						if (!loc.Value.PreventsRemoval)
						{
							tags.Append(" (AllowRemoveUnder)".Colour(Telnet.Yellow));
						}

						if (loc.Value.HidesSeveredBodyparts)
						{
							tags.Append(" (HidesSevers)".Colour(Telnet.Magenta));
						}

						if (loc.Value.NoArmour)
						{
							tags.Append(" (NoArmour)".Colour(Telnet.Red));
						}

						sb.AppendLine($"\tCovers {loc.Key.FullDescription().ColourValue()}{tags}");
					}
				}

				return sb.ToString();
			default:
				return base.Decorate(voyeur, name, description, type, colour, flags);
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Short || type == DescriptionType.Evaluate;
	}

	/// <summary>
	///     Items with multiple decorating components are sorted in DecorationPriority order. Positive values of
	///     DecorationPriority are decorated BEFORE any colour is applied, Negative values are decorated AFTER colour is
	///     applied in descending order
	/// </summary>
	public override int DecorationPriority => -1;

	#endregion
}