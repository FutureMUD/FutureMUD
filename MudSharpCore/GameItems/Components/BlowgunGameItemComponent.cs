using MudSharp.Body;
using MudSharp.Body.PartProtos;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Components;

public class BlowgunGameItemComponent : GameItemComponent, IRangedWeapon
{
	protected BlowgunGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BlowgunGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BlowgunGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Loaded", LoadedAmmo?.Parent.Id ?? 0),
			new XElement("Readied", IsReadied)
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
				$"This is a blowgun of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.\nIt can be fired from hiding without automatically revealing the firer.",
			_ => description
		};
	}

	public override void Quit()
	{
		base.Quit();
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
	public bool CanFireWhileHidden => true;
	public string FireVerbForEchoes => "blow|blows";
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

	private (bool Success, string Reason) CanUseBreathToFire(ICharacter actor)
	{
		var mouth = actor.Body.Bodyparts.OfType<MouthProto>().FirstOrDefault();
		if (mouth == null)
		{
			return (false, "You need a mouth to use a blowgun.");
		}

		if (!actor.Body.IsBreathing)
		{
			return (false, "You must be able to breathe freely to use a blowgun.");
		}

		var coveredMouthItem = actor.Body.WornItemsFor(mouth).FirstOrDefault();
		if (coveredMouthItem != null)
		{
			return (false,
				$"You cannot use a blowgun while {coveredMouthItem.HowSeen(actor)} is covering your {mouth.FullDescription()}.");
		}

		return (true, string.Empty);
	}

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

		if (!CanUseBreathToFire(readier).Success)
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
			return $"You must first load a dart before you can ready {Parent.HowSeen(readier)}.";
		}

		if (IsReadied)
		{
			return $"{Parent.HowSeen(readier, true)} is already ready to fire.";
		}

		if (!readier.Body.WieldedItems.Contains(Parent))
		{
			return $"{Parent.HowSeen(readier, true)} must first be wielded before it can be readied.";
		}

		var breathCheck = CanUseBreathToFire(readier);
		if (!breathCheck.Success)
		{
			return breathCheck.Reason;
		}

		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
			readier.Body.WieldedHandCount(Parent) < 2)
		{
			return $"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown reason in BlowgunGameItemComponent.WhyCannotReady.");
	}

	public bool Ready(ICharacter readier)
	{
		if (!CanReady(readier))
		{
			readier.Send(WhyCannotReady(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ raise|raises $0 and ready|readies $1 in the mouthpiece.", readier, Parent, LoadedAmmo.Parent),
			flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
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
			return $"{Parent.HowSeen(readier, true)} is not ready to fire and has no need of being unreadied.";
		}

		throw new ApplicationException("Unknown reason in BlowgunGameItemComponent.WhyCannotUnready.");
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
			new Emote("@ lower|lowers $0 from a ready position.", readier, Parent),
			flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
		IsReadied = false;
		Changed = true;
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
			return $"You must lower {Parent.HowSeen(loader)} before you can unload it.";
		}

		throw new NotImplementedException("Unknown WhyCannotUnload reason in BlowgunGameItemComponent.WhyCannotLoad.");
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
			Console.WriteLine("Error: Unfeasible plan made it to blowgun load.");
			return;
		}

		plan.ExecuteWholePlan();
		IGameItem ammo = loader.Body.HeldItems.FirstOrDefault(x =>
			(x.GetItemType<IAmmo>()?.AmmoType.SpecificType.Equals(WeaponType.SpecificAmmunitionGrade,
				StringComparison.InvariantCultureIgnoreCase) ?? false) &&
			(x.GetItemType<IAmmo>()?.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.Blowgun) ?? false));
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
				new Emote("@ insert|inserts $1 into $0.", loader, Parent, newammo),
				flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
			plan.FinalisePlan();
		}
		else
		{
			loader.Body.Take(ammo);
			LoadedAmmo = ammo.GetItemType<IAmmo>();
			loader.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ insert|inserts $1 into $0.", loader, Parent, ammo),
				flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
			plan.FinalisePlanNoRestore();
		}

		Changed = true;
	}

	public bool CanFire(ICharacter actor, IPerceivable target)
	{
		return LoadedAmmo != null && IsReadied && CanUseBreathToFire(actor).Success;
	}

	public string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		if (LoadedAmmo == null)
		{
			return $"You cannot fire {Parent.HowSeen(actor)} because it is not loaded.";
		}

		if (!IsReadied)
		{
			return $"You have not readied {Parent.HowSeen(actor)}. You must do so before firing.";
		}

		var breathCheck = CanUseBreathToFire(actor);
		if (!breathCheck.Success)
		{
			return breathCheck.Reason;
		}

		throw new ApplicationException("Unknown WhyCannotFire reason in BlowgunGameItemComponent.WhyCannotFire");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		var breathCheck = CanUseBreathToFire(actor);
		if (!breathCheck.Success)
		{
			actor.Send(breathCheck.Reason);
			IsReadied = false;
			Changed = true;
			return;
		}

		OutputFlags flags = OutputFlags.InnerWrap | OutputFlags.SuppressObscured;
		if (LoadedAmmo == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ puff|puffs into $1, but nothing was loaded and nothing happens.", actor, target, Parent),
				style: OutputStyle.CombatMessage, flags: flags));
			IsReadied = false;
			Changed = true;
			return;
		}

		if (target == null)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ puff|puffs into $1 and send|sends $2 darting straight upwards.", actor,
					target, Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage, flags: flags));
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ puff|puffs into $1 and send|sends $2 darting towards $0.", actor, target,
					Parent, LoadedAmmo.Parent), style: OutputStyle.CombatMessage, flags: flags));
		}

		IAmmo ammo = LoadedAmmo;
		LoadedAmmo = null;
		IsReadied = false;
		Changed = true;
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo.Parent, WeaponType,
			defenseEmote);
	}

	public BlowgunGameItemComponent(BlowgunGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BlowgunGameItemComponent(MudSharp.Models.GameItemComponent component, BlowgunGameItemComponentProto proto,
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
		IsReadied = bool.TryParse(root.Element("Readied")?.Value, out bool readied) && readied;
	}

	public BlowgunGameItemComponent(BlowgunGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		LoadedAmmo = rhs.LoadedAmmo;
		PrimaryWieldedLocation = rhs.PrimaryWieldedLocation;
		IsReadied = rhs.IsReadied;
	}

	public override void FinaliseLoad()
	{
		LoadedAmmo?.Parent.FinaliseLoadTimeTasks();
	}
}
