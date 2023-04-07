using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class CrossbowGameItemComponent : GameItemComponent, IRangedWeapon, IMeleeWeapon
{
	protected CrossbowGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CrossbowGameItemComponentProto)newProto;
	}

	#region Constructors

	public CrossbowGameItemComponent(CrossbowGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public CrossbowGameItemComponent(MudSharp.Models.GameItemComponent component, CrossbowGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public CrossbowGameItemComponent(CrossbowGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;

		LoadedAmmo = rhs.LoadedAmmo;
		PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
	}

	protected void LoadFromXml(XElement root)
	{
		PrimaryWieldedLocation =
			Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		LoadedAmmo = Gameworld.TryGetItem(long.Parse(root.Element("Loaded").Value), true)?.GetItemType<IAmmo>();
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CrossbowGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Loaded", LoadedAmmo?.Parent.Id ?? 0)
		).ToString();
	}

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		switch (type)
		{
			case DescriptionType.Full:
				return true;
			case DescriptionType.Evaluate:
				return true;
			default:
				return false;
		}
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Full:
				return
					$"{description}\n\n{(LoadedAmmo != null ? $"It is loaded with {LoadedAmmo.Parent.HowSeen(voyeur)}" : "")}";
			case DescriptionType.Evaluate:
				return
					$"This is a crossbow of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt takes bolts of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nThis is also a melee weapon of type {((IMeleeWeapon)this).WeaponType.Name.Colour(Telnet.Cyan)}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
			default:
				return description;
		}
	}

	#region Implementation of IRangedWeapon

	public bool CanBeAimedAtSelf => true;
	public string FireVerbForEchoes => "fire|fires";
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;
	public Difficulty AimDifficulty => _prototype.RangedWeaponType.BaseAimDifficulty;
	WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
	WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;

	public Difficulty BaseBlockDifficulty
		=> LoadedAmmo?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

	public Difficulty BaseDodgeDifficulty
		=> LoadedAmmo?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

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

	public bool ReadyToFire => IsLoaded && IsReadied;

	public bool IsLoaded => LoadedAmmo != null;
	public bool IsReadied { get; set; }

	public bool CanReady(ICharacter readier)
	{
		if (!IsLoaded)
		{
			return false;
		}

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
		if (!IsLoaded)
		{
			return $"You must first load a quarrel before you can ready {Parent.HowSeen(readier)}.";
		}

		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already ready to be fired, simply aim and fire.";
		}

		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return
				$"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown reason in CrossbowGameItemComponent.WhyCannotReady.");
	}

	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(new Emote("@ wind|winds up $0 until it is ready to fire $1.",
			readier, Parent, LoadedAmmo.Parent)));
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
		if (!IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is not ready to fire, and has no need of being unreadied.";
		}

		throw new ApplicationException("Unknown reason in CrossbowGameItemComponent.WhyCannotUnready.");
	}

	public bool Unready(ICharacter readier)
	{
		if (readier == null)
		{
			IsReadied = false;
			return true;
		}

		if (!CanUnready(readier))
		{
			readier.Send(WhyCannotUnready(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ unwind|unwinds $0 until the bow returns to its neutral position.", readier, Parent)));
		IsReadied = false;
		Changed = true;
		return true;
	}

	public int LoadStage => 0;

	public IAmmo LoadedAmmo { get; set; }

	public IEnumerable<IGameItem> MagazineContents => new[] { LoadedAmmo.Parent };

	public bool CanUnload(ICharacter loader)
	{
		return LoadedAmmo != null && !IsReadied;
	}

	public string WhyCannotUnload(ICharacter loader)
	{
		if (LoadedAmmo == null)
		{
			return $"You cannot unload {Parent.HowSeen(loader)} because it is not loaded.";
		}

		if (!IsReadied)
		{
			return
				$"You cannot unload {Parent.HowSeen(loader)} until you have unwound the tension and unreadied it for fire.";
		}

		throw new NotImplementedException("Unknown WhyCannotUnload reason in BowGameItemComponent.WhyCannotLoad.");
	}

	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		var ammo = LoadedAmmo;
		LoadedAmmo = null;
		loader.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ unload|unloads $0 from $1.", loader, ammo.Parent, Parent)));
		if (!loader.Body.CanGet(ammo.Parent, 0))
		{
			loader.Location.Insert(ammo.Parent);
		}
		else
		{
			loader.Body.Get(ammo.Parent, silent: true);
		}

		Changed = true;
		return new[] { ammo.Parent };
	}

	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false)
	{
		if (LoadedAmmo != null)
		{
			return false;
		}

		var plan = _prototype.LoadTemplate.CreatePlan(loader);
		return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false)
	{
		if (LoadedAmmo != null)
		{
			return $"You cannot load {Parent.HowSeen(loader)} because it is already loaded!";
		}

		var plan = _prototype.LoadTemplate.CreatePlan(loader);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				return $"You cannot load {Parent.HowSeen(loader)} because you don't have any suitable ammunition";
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				return
					$"You cannot load {Parent.HowSeen(loader)} because you don't have enough working {loader.Body.WielderDescriptionPlural}.";
		}

		throw new NotImplementedException();
	}

	public void Load(ICharacter loader, bool ignoreEmpty = false)
	{
		var plan = _prototype.LoadTemplate.CreatePlan(loader);
		plan.ExecuteWholePlan();
		var ammo =
			loader.Body.HeldItems.FirstOrDefault(
				x =>
					(x.GetItemType<IAmmo>()?.AmmoType.SpecificType.Equals(WeaponType.SpecificAmmunitionGrade) ??
					 false) &&
					(x.GetItemType<IAmmo>()?.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.Crossbow) ?? false));
		if (ammo == null)
		{
			return;
		}

		var ammoStack = ammo.GetItemType<IStackable>();
		if (ammoStack != null && ammoStack.Quantity > 1)
		{
			var newammo = ammoStack.Split(1);
			LoadedAmmo = newammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $1 in $0.", loader, Parent, newammo)));
			plan.FinalisePlan();
		}
		else
		{
			loader.Body.Take(ammo);
			LoadedAmmo = ammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ load|loads $1 in $0.", loader, Parent, ammo)));
			plan.FinalisePlanWithExemptions(new List<IGameItem> { ammo });
		}

		Changed = true;
	}

	public bool CanFire(ICharacter actor, IPerceivable target)
	{
		return LoadedAmmo != null && IsReadied;
	}

	public string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		if (LoadedAmmo == null)
		{
			return $"You cannot fire {Parent.HowSeen(actor)} because it is not loaded.";
		}

		if (!IsReadied)
		{
			return $"You have not wound up {Parent.HowSeen(actor)}. You must do so before firing.";
		}

		throw new ApplicationException(
			"Unknown WhyCannotFire reason in CrossbowGameItemComponent.WhyCannotFire");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ pull|pulls the trigger on $1 and send|sends $2 whizzing off towards $0.", actor,
				target ?? (IPerceivable)new DummyPerceivable("the sky"), Parent, LoadedAmmo.Parent),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		var ammo = LoadedAmmo;
		LoadedAmmo = null;
		IsReadied = false;
		Changed = true;
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo.Parent, WeaponType,
			defenseEmote);
	}

	#endregion

	#region Implementation of IDamageSource

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Implementation of IUseTrait

	public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

	#endregion

	public override void FinaliseLoad()
	{
		LoadedAmmo?.Parent.FinaliseLoadTimeTasks();
	}


	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	#endregion

	public override void Quit()
	{
		base.Quit();
		LoadedAmmo?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		LoadedAmmo?.Parent.Delete();
	}

	public override void Login()
	{
		LoadedAmmo?.Login();
	}
}