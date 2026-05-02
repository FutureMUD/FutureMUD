using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Concrete;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class SlingGameItemComponent : GameItemComponent, IRangedWeaponWithUnreadyEvent,
	IReadiedRangedWeaponStaminaSource
{
	protected SlingGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SlingGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SlingGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Loaded", LoadedAmmo?.Parent.Id ?? 0)
		).ToString();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type switch
		{
			DescriptionType.Full => true,
			DescriptionType.Evaluate => true,
			_ => false
		};
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Full =>
				$"{description}{(LoadedAmmo != null ? $"\n\nIt is loaded with {LoadedAmmo.Parent.HowSeen(voyeur)}" : "")}",
			DescriptionType.Evaluate =>
				$"This is a sling of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.",
			_ => description
		};
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
		LoadedAmmo?.Parent.ContainedIn = null;
		LoadedAmmo?.Parent.Delete();
	}

	public override void Login()
	{
		LoadedAmmo?.Login();
	}

	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

	public bool CanBeAimedAtSelf => false;
	public string FireVerbForEchoes => "sling|slings";
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;
	public Difficulty AimDifficulty => _prototype.RangedWeaponType.BaseAimDifficulty;
	WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;

	public Difficulty BaseBlockDifficulty => LoadedAmmo?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;
	public Difficulty BaseDodgeDifficulty => LoadedAmmo?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

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

	public bool CanWield(ICharacter actor)
	{
		return _prototype.CanWieldProg?.ExecuteBool(false, actor, Parent) ?? true;
	}

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
			return $"You must first load a sling bullet before you can ready {Parent.HowSeen(readier)}.";
		}

		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already being whirled and is ready to be fired.";
		}

		if (!readier.Body.WieldedItems.Contains(Parent))
		{
			return $"{Parent.HowSeen(readier, true)} must first be wielded before it can be readied.";
		}

		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
			readier.Body.WieldedHandCount(Parent) < 2)
		{
			return $"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown reason in SlingGameItemComponent.WhyCannotReady.");
	}

	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ begin|begins whirling $0 with $1 seated in its pouch.", readier, Parent, LoadedAmmo.Parent)));
		IsReadied = true;
		Changed = true;
		readier.AddEffect(new ReadiedRangedWeaponDrainStamina(readier, this), TimeSpan.FromSeconds(5));
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
			return $"{Parent.HowSeen(readier, true)} is not being whirled and has no need of being unreadied.";
		}

		throw new ApplicationException("Unknown reason in SlingGameItemComponent.WhyCannotUnready.");
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
			new Emote("@ slow|slows and stop|stops whirling $0.", readier, Parent)));
		IsReadied = false;
		Changed = true;
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
			return $"You must stop whirling {Parent.HowSeen(loader)} before you can unload it.";
		}

		throw new NotImplementedException("Unknown WhyCannotUnload reason in SlingGameItemComponent.WhyCannotLoad.");
	}

	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		IAmmo ammo = LoadedAmmo;
		LoadedAmmo = null;
		loader.OutputHandler.Handle(new EmoteOutput(new Emote("@ unload|unloads $0 from $1.", loader, ammo.Parent, Parent)));
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

		IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
		return plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (LoadedAmmo != null)
		{
			return $"You cannot load {Parent.HowSeen(loader)} because it is already loaded!";
		}

		IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
		return plan.PlanIsFeasible() switch
		{
			InventoryPlanFeasibility.NotFeasibleMissingItems =>
				$"You cannot load {Parent.HowSeen(loader)} because you don't have any suitable ammunition.",
			InventoryPlanFeasibility.NotFeasibleNotEnoughHands or InventoryPlanFeasibility.NotFeasibleNotEnoughWielders =>
				$"You cannot load {Parent.HowSeen(loader)} because you don't have enough working {loader.Body.WielderDescriptionPlural}.",
			_ => throw new NotImplementedException()
		};
	}

	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		IInventoryPlan plan = _prototype.LoadTemplate.CreatePlan(loader);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			Console.WriteLine("Error: Unfeasible plan made it to sling load.");
			return;
		}

		plan.ExecuteWholePlan();
		IGameItem ammo = loader.Body.HeldItems.FirstOrDefault(x =>
			(x.GetItemType<IAmmo>()?.AmmoType.SpecificType.Equals(WeaponType.SpecificAmmunitionGrade,
				StringComparison.InvariantCultureIgnoreCase) ?? false) &&
			(x.GetItemType<IAmmo>()?.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.Sling) ?? false));
		if (ammo == null)
		{
			return;
		}

		IStackable ammoStack = ammo.GetItemType<IStackable>();
		if (ammoStack != null && ammoStack.Quantity > 1)
		{
			IGameItem newammo = ammoStack.Split(1);
			LoadedAmmo = newammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ place|places $1 into the pouch of $0.", loader, Parent, newammo)));
			plan.FinalisePlan();
		}
		else
		{
			loader.Body.Take(ammo);
			LoadedAmmo = ammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ place|places $1 into the pouch of $0.", loader, Parent, ammo)));
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
			return $"You have not started whirling {Parent.HowSeen(actor)}. You must do so before firing.";
		}

		throw new ApplicationException("Unknown WhyCannotFire reason in SlingGameItemComponent.WhyCannotFire");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		if (LoadedAmmo == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ snap|snaps $1 forward, but nothing was loaded and nothing flies free.",
					actor, target, Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			IsReadied = false;
			Changed = true;
			return;
		}

		if (target == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ snap|snaps $1 forward and send|sends $2 hurtling straight upwards.", actor,
					target, Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ snap|snaps $1 forward and send|sends $2 hurtling towards $0.", actor, target,
					Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

		IAmmo ammo = LoadedAmmo;
		LoadedAmmo = null;
		IsReadied = false;
		Changed = true;
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo.Parent, WeaponType,
			defenseEmote);
		OnFire?.Invoke(Parent);
	}

	public SlingGameItemComponent(SlingGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public SlingGameItemComponent(MudSharp.Models.GameItemComponent component, SlingGameItemComponentProto proto,
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
		LoadedAmmo = Gameworld.TryGetItem(long.Parse(root.Element("Loaded")?.Value ?? "0"), true)?.GetItemType<IAmmo>();
	}

	public SlingGameItemComponent(SlingGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		LoadedAmmo = rhs.LoadedAmmo;
		PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
	}

	public override void FinaliseLoad()
	{
		LoadedAmmo?.Parent.FinaliseLoadTimeTasks();
	}

	public event PerceivableEvent OnFire;
	public event PerceivableEvent OnUnready;
	public double StaminaPerTick => _prototype.StaminaPerTick;
	public bool ReadiedUseRequiresFreeHand => false;
	public string ReadiedStaminaReleaseEmote => "@ stop|stops whirling $0 and let|lets it fall still.";
	public string ReadiedStaminaNoFreeHandEmote => "@ lose|loses control of the motion of $0 and let|lets it fall still.";
	public string ReadiedStaminaExhaustedEmote =>
		"@ stop|stops whirling $0 because #1 are|is too exhausted to keep it moving.";
	public string ReadiedStaminaEffectDescription => "A readied sling is draining the stamina of its wielder.";
}
