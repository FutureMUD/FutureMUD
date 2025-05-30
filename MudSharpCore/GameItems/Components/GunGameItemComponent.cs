﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Events;
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
using MudSharp.Construction;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.GameItems.Components;

public class GunGameItemComponent : GameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
	protected GunGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (GunGameItemComponentProto)newProto;
	}

	#region Constructors

	public GunGameItemComponent(GunGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
	}

	public GunGameItemComponent(MudSharp.Models.GameItemComponent component, GunGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public GunGameItemComponent(GunGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		Magazine = Gameworld.TryGetItem(long.Parse(root.Element("Magazine").Value), true)?.GetItemType<IContainer>();
		ChamberedRound = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedRound").Value), true)
		                          ?.GetItemType<IAmmo>();
		PrimaryWieldedLocation =
			Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		var element = root.Element("Safety");
		if (element != null)
		{
			Safety = element.Value == "true";
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new GunGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Magazine", Magazine?.Parent.Id ?? 0),
			new XElement("ChamberedRound", ChamberedRound?.Parent.Id ?? 0),
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Safety", Safety ? "true" : "false")
		).ToString();
	}

	#endregion

	#region IRangedWeapon Implementation

	public bool CanBeAimedAtSelf => true;
	public string FireVerbForEchoes => "fire|fires";
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

	public bool ReadyToFire => ChamberedRound != null && !Safety;

	public int LoadStage => 0;

	public IAmmo ChamberedRound { get; set; }

	public IContainer Magazine { get; set; }

	public IEnumerable<IGameItem> MagazineContents => Magazine?.Contents ?? Enumerable.Empty<IGameItem>();
	public IEnumerable<IGameItem> AllContainedItems => MagazineContents.Concat([ChamberedRound?.Parent, Magazine?.Parent]).SelectNotNull(x => x);

	public string SpecificAmmoGrade => _prototype.RangedWeaponType.SpecificAmmunitionGrade;

	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

	public Difficulty BaseBlockDifficulty
		=> ChamberedRound?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

	public Difficulty BaseDodgeDifficulty
		=> ChamberedRound?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

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
		if (Magazine != null)
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
		if (Magazine != null)
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

	private void ChamberRound(ICharacter loader)
	{
		if (ChamberedRound != null)
		{
			loader.OutputHandler.Handle(new EmoteOutput(new Emote("$1 is ejected from $0 by the action.", loader,
				Parent, ChamberedRound.Parent)));
			ChamberedRound.Parent.RoomLayer = loader.RoomLayer;
			loader.Location.Insert(ChamberedRound.Parent);
			ChamberedRound.Parent.ContainedIn = null;
		}

		var newRound = MagazineContents.SelectNotNull(x => x.GetItemType<IAmmo>()).FirstOrDefault(x =>
			x.AmmoType.SpecificType == SpecificAmmoGrade &&
			x.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm));
		if (newRound != null)
		{
			ChamberedRound = Magazine.Take(null, newRound.Parent, 1)?.GetItemType<IAmmo>();
		}
		else
		{
			ChamberedRound = null;
			loader.HandleEvent(EventType.ReadyGunEmpty, loader, this);
		}

		Changed = true;
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
		                  .SelectNotNull(x => x.PrimaryTarget.GetItemType<IAmmoClip>()).First();
		loader.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent), flags: OutputFlags.InnerWrap));
		loader.Body.Take(ammo.Parent);
		Magazine = ammo;
		ammo.Parent.ContainedIn = Parent;

		var exemptions = new List<IGameItem> { ammo.Parent };
		plan.FinalisePlanWithExemptions(exemptions);
		Changed = true;
	}

	public bool CanUnload(ICharacter loader)
	{
		return Magazine != null;
	}

	public string WhyCannotUnload(ICharacter loader)
	{
		if (Magazine == null)
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
			Magazine.Parent)));
		var mag = Magazine;
		if (loader.Body.CanGet(Magazine.Parent, 0))
		{
			loader.Body.Get(Magazine.Parent, silent: true);
		}
		else
		{
			Magazine.Parent.RoomLayer = loader.RoomLayer;
			loader.Location.Insert(Magazine.Parent);
		}

		Magazine = null;
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
				new Emote(_prototype.FireEmoteNoChamberedRound, actor, actor,
					target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap));
			actor.HandleEvent(EventType.FireGunEmpty, actor, target, Parent);
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.FireEmote, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"),
				Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

		var ammo = ChamberedRound;
		ChamberedRound = null;
		ChamberRound(actor);

		var bullet = ammo.GetFiredItem ?? ammo.Parent;
		var shell = ammo.GetFiredWasteItem;

		if (bullet != ammo.Parent)
		{
			ammo.Parent.Delete();
		}

		Changed = true;

		var originalLocation =
			actor.Location; // If the character is firing at themselves, their location can be changed by the ammo.Fire call.
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, bullet, WeaponType, defenseEmote);
		if (shell != null)
		{
			originalLocation.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground.", shell),
				flags: OutputFlags.Insigificant));
			shell.RoomLayer = actor.RoomLayer;
			originalLocation.Insert(shell);
		}

		var vicinity = originalLocation.CellsInVicinity((uint)ammo.AmmoType.Loudness, false, false)
		                               .Except(originalLocation);
		foreach (var location in vicinity)
		{
			if (location.Characters.Any() || location.GameItems.Any())
			{
				var directions = location.ExitsBetween(originalLocation, 10).ToList();
				location.Handle(new EmoteOutput(
					new Emote($"A gun shot can be heard {directions.DescribeDirectionsToFrom()}.", Parent),
					flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
			}
		}

		foreach (var layer in actor.Location.Terrain(null).TerrainLayers.Except(actor.RoomLayer))
		{
			if (layer.IsLowerThan(actor.RoomLayer))
			{
				actor.Location.Handle(layer,
					new EmoteOutput(new Emote($"A gun shot can be heard from above.", Parent),
						flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
			}
			else
			{
				actor.Location.Handle(layer,
					new EmoteOutput(new Emote($"A gun shot can be heard from below.", Parent),
						flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
			}
		}
	}

	public bool IsLoaded => Magazine != null;
	public bool IsReadied => ChamberedRound != null;

	public bool CanReady(ICharacter readier)
	{
		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotReady(ICharacter readier)
	{
		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return
				$"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown WhyCannotReady reason in ready GunGameItemComponent.");
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
		ChamberRound(readier);
		return true;
	}

	public bool CanUnready(ICharacter readier)
	{
		return true;
	}

	public string WhyCannotUnready(ICharacter readier)
	{
		throw new ApplicationException("Should always be able to unready GunGameItemComponent");
	}

	public bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier))
		{
			readier.Send(WhyCannotUnready(readier));
			return false;
		}

		if (ChamberedRound != null)
		{
			readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent,
				ChamberedRound.Parent)));
			ChamberedRound.Parent.ContainedIn = null;
			if (readier.Body.CanGet(ChamberedRound.Parent, 0))
			{
				readier.Body.Get(ChamberedRound.Parent, silent: true);
			}
			else
			{
				readier.Location.Insert(ChamberedRound.Parent);
			}

			ChamberedRound = null;
		}
		else
		{
			readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmoteNoChamberedRound, readier,
				readier, Parent)));
		}

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

	public override double ComponentWeight
	{
		get { return MagazineContents.Sum(x => x.Weight) + ChamberedRound?.Parent.Weight ?? 0.0; }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return MagazineContents.Sum(x => x.Buoyancy(fluidDensity)) + ChamberedRound?.Parent.Buoyancy(fluidDensity) ??
		       0.0;
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
			sb.AppendLine(Magazine != null
				? $"It has {Magazine.Parent.HowSeen(voyeur)} in the magazine."
				: "It does not currently have any clip in the magazine.");
			sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
			return sb.ToString();
		}

		if (type == DescriptionType.Evaluate)
		{
			var mw = (IMeleeWeapon)this;
			return
				$"This is a semi-automatic firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)} and accepts clips of type {_prototype.ClipType.Colour(Telnet.Green)}.\n This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override void Quit()
	{
		base.Quit();
		ChamberedRound?.Parent.Quit();
		Magazine?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		if (ChamberedRound is not null)
		{
			ChamberedRound.Parent.ContainedIn = null;
		}
		ChamberedRound?.Parent.Delete();

		if (Magazine is not null)
		{
			Magazine.Parent.ContainedIn = null;
		}
		Magazine?.Parent.Delete();
	}

	public override void Login()
	{
		ChamberedRound?.Parent.Login();
		Magazine?.Parent.Login();
	}

	#endregion


	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	#endregion
}