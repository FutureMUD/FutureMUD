using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class LaserGameItemComponent : GameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
	protected LaserGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LaserGameItemComponentProto)newProto;
	}

	#region Constructors

	public LaserGameItemComponent(LaserGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
	}

	public LaserGameItemComponent(MudSharp.Models.GameItemComponent component, LaserGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public LaserGameItemComponent(LaserGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		Safety = bool.Parse(root.Element("Safety")?.Value ?? "false");
		IsReadied = bool.Parse(root.Element("Readied")?.Value ?? "false");
		PowerPack = Gameworld.TryGetItem(long.Parse(root.Element("PowerPack")?.Value ?? "0"), true)
		                     ?.GetItemType<ILaserPowerPack>();
		var element = root.Element("Safety");
		if (element != null)
		{
			Safety = element.Value == "true";
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LaserGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PowerPack", PowerPack?.Parent.Id ?? 0),
			new XElement("Safety", Safety),
			new XElement("Readied", IsReadied),
			new XElement("Safety", Safety ? "true" : "false")
		).ToString();
	}

	#endregion

	#region IRangedWeapon Implementation

	public bool CanBeAimedAtSelf => true;
	public string FireVerbForEchoes => "fire|fires";
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

	public bool ReadyToFire => IsReadied && !Safety && IsLoaded && PowerPack.CanDraw(_prototype.WattsPerShot);

	public int LoadStage => 0;

	public ILaserPowerPack PowerPack { get; set; }

	public IEnumerable<IGameItem> MagazineContents => Enumerable.Empty<IGameItem>();
	public IEnumerable<IGameItem> AllContainedItems => new List<IGameItem> { PowerPack?.Parent }.SelectNotNull(x => x);

	public string SpecificAmmoGrade => _prototype.RangedWeaponType.SpecificAmmunitionGrade;

	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

	public Difficulty BaseBlockDifficulty => Difficulty.Hard;

	public Difficulty BaseDodgeDifficulty => Difficulty.ExtremelyHard;

	private IWield _primaryWieldedLocation;

	public IWield PrimaryWieldedLocation
	{
		get => _primaryWieldedLocation;
		set
		{
			_primaryWieldedLocation = value;
			Changed = true;
		}
	}

	public bool AlwaysRequiresTwoHandsToWield => WeaponType.AlwaysRequiresTwoHandsToWield;

	/// <inheritdoc />
	public bool CanWield(ICharacter actor)
	{
		return _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
	}

	/// <inheritdoc />
	public string WhyCannotWield(ICharacter actor)
	{
		return _prototype.WhyCannotWieldProg?.ExecuteString(actor, Parent) ?? "You can't wield that for an unknown reason.";
	}

	public ITraitDefinition Trait => WeaponType.FireTrait;

	WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
	WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;

	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (PowerPack != null)
		{
			return false;
		}

		var plan = ignoreEmpty
			? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
			: _prototype.LoadTemplate.CreatePlan(loader);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (PowerPack != null)
		{
			return $"There is already a clip in the magazine of {Parent.HowSeen(loader)}, you should eject that first.";
		}

		var plan = ignoreEmpty
			? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
			: _prototype.LoadTemplate.CreatePlan(loader);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			switch (plan.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					return $"You don't have a suitable magazine of ammunition to load {Parent.HowSeen(loader)}.";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		throw new ApplicationException("Unknown WhyCannotLoad reason in PistolGameItemComponent.WhyCannotLoad");
	}

	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (!CanLoad(loader))
		{
			loader.Send(WhyCannotLoad(loader));
			return;
		}

		var plan = ignoreEmpty
			? _prototype.LoadTemplateIgnoreEmpty.CreatePlan(loader)
			: _prototype.LoadTemplate.CreatePlan(loader);
		var results = plan.ExecuteWholePlan();
		var ammo = results.Where(x => (string)x.OriginalReference == "loaditem")
		                  .SelectNotNull(x => x.PrimaryTarget.GetItemType<ILaserPowerPack>()).First();
		plan.FinalisePlanWithExemptions(new List<IGameItem> { ammo.Parent });
		loader.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent), flags: OutputFlags.InnerWrap));
		loader.Body.Take(ammo.Parent);
		PowerPack = ammo;
		ammo.Parent.ContainedIn = Parent;
		Changed = true;
	}

	public bool CanUnload(ICharacter loader)
	{
		return PowerPack != null;
	}

	public string WhyCannotUnload(ICharacter loader)
	{
		if (PowerPack == null)
		{
			return $"{Parent.HowSeen(loader, true)} is already unloaded.";
		}

		throw new ApplicationException("Unknown reason in GunGameItemComponent.WhyCannotUnload");
	}

	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		if (!CanUnload(loader))
		{
			loader.Send(WhyCannotUnload(loader));
			return Enumerable.Empty<IGameItem>();
		}

		loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent,
			PowerPack.Parent)));
		var mag = PowerPack;
		if (loader.Body.CanGet(PowerPack.Parent, 0))
		{
			loader.Body.Get(PowerPack.Parent, silent: true);
		}
		else
		{
			loader.Location.Insert(PowerPack.Parent);
		}

		PowerPack = null;
		IsReadied = false;
		Changed = true;
		return new[] { mag.Parent };
	}

	public bool CanFire(ICharacter actor, IPerceivable target)
	{
		return true;
	}

	public string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		throw new ApplicationException("Guns should always be able to fire.");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		if (!ReadyToFire)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote(_prototype.FireEmoteNoAmmo, actor, actor,
					target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
			actor.HandleEvent(EventType.FireGunEmpty, actor, target, Parent);
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.FireEmote, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"),
				Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

		Changed = true;
		PowerPack.Draw(_prototype.WattsPerShot);
		PowerPack.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, WeaponType,
			_prototype.PainMultiplier, _prototype.StunMultiplier, defenseEmote);

		if (PowerPack.PowerLevel <= 0.0)
		{
			IsReadied = false;
		}

		if (_prototype.FireVolume > AudioVolume.Silent)
		{
			var vicinity = actor.CellsInVicinity((uint)_prototype.FireVolume, false, false)
			                    .Except(actor.Location);
			foreach (var location in vicinity)
			{
				if (location.Characters.Any() || location.GameItems.Any())
				{
					var directions = location.ExitsBetween(actor.Location, 10).ToList();
					location.Handle(new AudioOutput(
						new Emote($"A laser blast can be heard {directions.DescribeDirectionsToFrom()}.", Parent),
						_prototype.FireVolume.StageDown((uint)Math.Max(0, directions.Count - 1)),
						flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
				}
			}

			foreach (var layer in actor.Location.Terrain(null).TerrainLayers.Except(actor.RoomLayer))
			{
				if (layer.IsLowerThan(actor.RoomLayer))
				{
					actor.Location.Handle(layer,
						new AudioOutput(new Emote($"A laser blast can be heard from above.", Parent),
							_prototype.FireVolume,
							flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
				}
				else
				{
					actor.Location.Handle(layer,
						new AudioOutput(new Emote($"A laser blast can be heard from below.", Parent),
							_prototype.FireVolume,
							flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
				}
			}
		}
	}

	public bool IsLoaded => PowerPack != null;
	public bool IsReadied { get; protected set; }

	public bool CanReady(ICharacter readier)
	{
		if (IsReadied)
		{
			return false;
		}

		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotReady(ICharacter readier)
	{
		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already ready to fire, and does not need further readying.";
		}

		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return
				$"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown WhyCannotReady reason in LaserGameItemComponent");
	}

	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.ReadyEmote, readier, readier, Parent),
			flags: OutputFlags.InnerWrap));
		IsReadied = true;
		Changed = true;
		return true;
	}

	public bool CanUnready(ICharacter readier)
	{
		return IsReadied;
	}

	public string WhyCannotUnready(ICharacter readier)
	{
		return $"{Parent.HowSeen(readier, true)} is already in storage mode, and does not need further unreadying.";
	}

	public bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier))
		{
			readier.Send(WhyCannotUnready(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent)));
		IsReadied = false;
		Changed = true;
		return true;
	}

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region ISwitchable Implementation

	public bool Safety { get; set; }

	public bool CanSwitch(ICharacter actor, string setting)
	{
		switch (setting.ToLowerInvariant())
		{
			case "safe":
			case "safety":
				return !Safety;
			case "fire":
			case "unsafe":
				return Safety;
		}

		return false;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		switch (setting.ToLowerInvariant())
		{
			case "safe":
			case "safety":
				return $"{Parent.HowSeen(actor, true)} already has its safety switched on.";
			case "fire":
			case "unsafe":
				return $"{Parent.HowSeen(actor, true)} is already in fire mode.";
		}

		return
			$"That is not a valid option for switching in {Parent.HowSeen(actor)}. Valid options are safe, or unsafe.";
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			actor.Send(WhyCannotSwitch(actor, setting));
			return false;
		}

		if (setting.EqualTo("fire") || setting.EqualTo("unsafe"))
		{
			Safety = false;
		}
		else
		{
			Safety = true;
		}

		Changed = true;
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote($"@ switch|switches the safety on $0 {(Safety ? "on" : "off")}.", actor,
				Parent)));
		return true;
	}

	public IEnumerable<string> SwitchSettings => new[] { "safe", "unsafe" };

	#endregion

	#region IGameItemComponent Overrides

	public override double ComponentWeight => PowerPack?.Parent.Weight ?? 0.0;

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return PowerPack?.Parent.Buoyancy(fluidDensity) ?? 0.0;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine();
			sb.AppendLine(PowerPack != null
				? $"It has {PowerPack.Parent.HowSeen(voyeur)} in the magazine.\nThe ammunition level indicator is displaying {PowerPack.PowerLevel:P1}."
				: "It does not currently have any clip in the magazine.");
			if (IsReadied)
			{
				sb.AppendLine("It is primed for firing.");
			}

			sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
			return sb.ToString();
		}

		if (type == DescriptionType.Evaluate)
		{
			return
				$"This is a laser gun of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt accepts power packs of type {_prototype.ClipType.Colour(Telnet.Green)}.\n This is also a melee weapon of type {((IMeleeWeapon)this).WeaponType.Name.Colour(Telnet.Cyan)}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override void Quit()
	{
		base.Quit();
		PowerPack?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		if (PowerPack is not null)
		{
			PowerPack.Parent.ContainedIn = null;
		}
		PowerPack?.Parent.Delete();
	}

	public override void Login()
	{
		PowerPack?.Parent.Login();
	}

	#endregion

	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	#endregion
}