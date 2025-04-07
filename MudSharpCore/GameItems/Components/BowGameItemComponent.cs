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
using MudSharp.Effects.Concrete;

namespace MudSharp.GameItems.Components;

public class BowGameItemComponent : GameItemComponent, IRangedWeaponWithUnreadyEvent, IMeleeWeapon
{
	protected BowGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BowGameItemComponent(this, newParent, temporary);
	}

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
					$"{description}{(LoadedAmmo != null ? $"\n\nIt is loaded with {LoadedAmmo.Parent.HowSeen(voyeur)}" : "")}";
			case DescriptionType.Evaluate:
				var mw = (IMeleeWeapon)this;
				return
					$"This is a bow of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes arrows of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nThis is also a melee weapon of type {((IMeleeWeapon)this).WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";

			default:
				return description;
		}
	}

	public override void Quit()
	{
		base.Quit();
		OnFire = null;
		OnUnready = null;
		LoadedAmmo?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		if (LoadedAmmo is not null)
		{
			LoadedAmmo.Parent.ContainedIn = null;
		}
		LoadedAmmo?.Parent.Delete();
	}

	public override void Login()
	{
		LoadedAmmo?.Login();
	}

	#region Implementation of IDamageSource

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Implementation of IUseTrait

	public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

	#endregion

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BowGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Loaded", LoadedAmmo?.Parent.Id ?? 0)
		).ToString();
	}

	#region Implementation of IRangedWeapon

	public bool CanBeAimedAtSelf => false;
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

	public bool AlwaysRequiresTwoHandsToWield => false;

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

	public bool ReadyToFire => IsLoaded && IsReadied;

	public int LoadStage => 0;

	public IAmmo LoadedAmmo { get; set; }

	public IEnumerable<IGameItem> MagazineContents => new[] { LoadedAmmo?.Parent }.SelectNotNull(x => x);
	public IEnumerable<IGameItem> AllContainedItems => MagazineContents;

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

		if (!readier.Body.WieldedItems.Contains(Parent))
		{
			return false;
		}

		if (!readier.Body.FunctioningFreeHands.Any())
		{
			return false;
		}

		return true;
	}

	public string WhyCannotReady(ICharacter readier)
	{
		if (!IsLoaded)
		{
			return $"You must first load an arrow before you can ready {Parent.HowSeen(readier)}.";
		}

		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already ready to be fired, simply aim and fire.";
		}

		if (!readier.Body.WieldedItems.Contains(Parent))
		{
			return $"{Parent.HowSeen(readier, true)} must first be wielded before it can be readied.";
		}

		if (!readier.Body.FunctioningFreeHands.Any())
		{
			return $"You need at least one functioning free {readier.Body.WielderDescriptionSingular} to ready a bow.";
		}

		throw new ApplicationException("Unknown reason in BowGameItemComponent.WhyCannotReady.");
	}

	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(new Emote("@ draw|draws back $0 with $1 ready to fire.", readier,
			Parent, LoadedAmmo.Parent)));
		IsReadied = true;
		readier.AddEffect(new BowDrainStamina(readier, this), TimeSpan.FromSeconds(5));
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

		throw new ApplicationException("Unknown reason in BowGameItemComponent.WhyCannotUnready.");
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

		readier.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ slowly release|releases the draw on $0 and returns it to an undrawn position.", readier, Parent)));
		IsReadied = false;
		OnUnready?.Invoke(Parent);
		return true;
	}

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

		if (IsReadied)
		{
			return $"You must let go of your draw on {Parent.HowSeen(loader)} before you can unload it.";
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

	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (LoadedAmmo != null)
		{
			return false;
		}

		var plan = _prototype.LoadTemplate.CreatePlan(loader);
		return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
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

	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		var plan = _prototype.LoadTemplate.CreatePlan(loader);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			Console.WriteLine("Error: Unfeasible plan made it to bow load.");
			return;
		}

		plan.ExecuteWholePlan();
		var ammo =
			loader.Body.HeldItems.FirstOrDefault(
				x =>
					(x.GetItemType<IAmmo>()?.AmmoType.SpecificType.Equals(WeaponType.SpecificAmmunitionGrade) ??
					 false) &&
					(x.GetItemType<IAmmo>()?.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.Bow) ?? false));
		if (ammo == null)
		{
			return;
		}

		var ammoStack = ammo.GetItemType<IStackable>();
		if (ammoStack != null && ammoStack.Quantity > 1)
		{
			var newammo = ammoStack.Split(1);
			LoadedAmmo = newammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ nock|nocks $1 in $0.", loader, Parent, newammo)));
			plan.FinalisePlan();
		}
		else
		{
			loader.Body.Take(ammo);
			LoadedAmmo = ammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ nock|nocks $1 in $0.", loader, Parent, ammo)));
			plan.FinalisePlanNoRestore();
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
			return $"You have not drawn back {Parent.HowSeen(actor)}. You must do so before firing.";
		}

		throw new ApplicationException(
			"Unknown WhyCannotFire reason in BowGameItemComponent.WhyCannotFire");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		if (LoadedAmmo == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					"@ release|releases the string on $1, but nothing was loaded and so it just twangs back to normal.",
					actor, target, Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			IsReadied = false;
			Changed = true;
			return;
		}

		if (target == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases the string on $1 and send|sends $2 hurtling straight upwards.", actor,
					target, Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ release|releases the string on $1 and send|sends $2 hurtling towards $0.", actor, target,
					Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

		var ammo = LoadedAmmo;
		LoadedAmmo = null;
		IsReadied = false;
		Changed = true;
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo.Parent, WeaponType,
			defenseEmote);
		OnFire?.Invoke(Parent);
	}

	#endregion

	#region Constructors

	public BowGameItemComponent(BowGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BowGameItemComponent(MudSharp.Models.GameItemComponent component, BowGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		PrimaryWieldedLocation =
			Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		LoadedAmmo = Gameworld.TryGetItem(long.Parse(root.Element("Loaded").Value), true)?.GetItemType<IAmmo>();
	}

	public BowGameItemComponent(BowGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		LoadedAmmo = rhs.LoadedAmmo;
		PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
	}

	public override void FinaliseLoad()
	{
		LoadedAmmo?.Parent.FinaliseLoadTimeTasks();
	}

	#endregion

	public event PerceivableEvent OnFire;
	public event PerceivableEvent OnUnready;
	public double StaminaPerTick => _prototype.StaminaPerTick;

	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	#endregion
}