using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class InternalMagazineGunGameItemComponent : FirearmBaseGameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
	protected InternalMagazineGunGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (InternalMagazineGunGameItemComponentProto)newProto;
	}

	#region Constructors

	public InternalMagazineGunGameItemComponent(InternalMagazineGunGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public InternalMagazineGunGameItemComponent(Models.GameItemComponent component,
		InternalMagazineGunGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public InternalMagazineGunGameItemComponent(InternalMagazineGunGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		foreach (var sub in root.Element("RoundsInMagazine").Elements())
		{
			var item = Gameworld.TryGetItem(long.Parse(sub.Value), true);
			if (item != null)
			{
				_roundsInMagazine.Add(item);
			}
		}

		ChamberedCasing = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedCasing")?.Value ?? "0"), true);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new InternalMagazineGunGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("RoundsInMagazine",
				from item in _roundsInMagazine
				select new XElement("Round", item.Id)
			),
			new XElement("ChamberedRound", ChamberedRound?.Parent.Id ?? 0),
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Safety", Safety ? "true" : "false"),
			new XElement("ChamberedCasing", ChamberedCasing?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region IRangedWeapon Implementation

	public IGameItem ChamberedCasing { get; set; } //The casing waiting to be ejected for when you ready

	private readonly List<IGameItem> _roundsInMagazine = new();

	public override IEnumerable<IGameItem> MagazineContents => _roundsInMagazine;
	public override IEnumerable<IGameItem> AllContainedItems => MagazineContents.Concat([ChamberedCasing, ChamberedRound?.Parent]).SelectNotNull(x => x);

	public override bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (_roundsInMagazine.Sum(x => x.Quantity) >= _prototype.InternalMagazineCapacity)
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

	public override string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (_roundsInMagazine.Sum(x => x.Quantity) >= _prototype.InternalMagazineCapacity)
		{
			return
				$"There is no more space in the magazine of {Parent.HowSeen(loader)}.";
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
					return
						$"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					return $"You don't have any suitable rounds of ammunition to load {Parent.HowSeen(loader)}.";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		throw new ApplicationException(
			"Unknown WhyCannotLoad reason in InternalMagazineGunGameItemComponent.WhyCannotLoad");
	}

	protected override void ChamberRound(ICharacter loader)
	{
		if (ChamberedRound != null)
		{
			loader.OutputHandler.Handle(new EmoteOutput(
				new Emote("$1 is ejected from $0 by the action.", loader, Parent,
					ChamberedRound.Parent), flags: OutputFlags.Insigificant));
			ChamberedRound.Parent.RoomLayer = loader.RoomLayer;
			loader.Location.Insert(ChamberedRound.Parent);
			ChamberedRound.Parent.ContainedIn = null;
		}

		if (ChamberedCasing != null)
		{
			loader.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ tumble|tumbles to the ground.", ChamberedCasing), flags: OutputFlags.Insigificant));
			ChamberedCasing.RoomLayer = loader.RoomLayer;
			loader.Location.Insert(ChamberedCasing);
			ChamberedCasing.ContainedIn = null;
			ChamberedCasing = null;
		}

		if (_roundsInMagazine.Any())
		{
			var first = _roundsInMagazine.First();
			if (first.Quantity > 1)
			{
				ChamberedRound = first.Get(null, 1).GetItemType<IAmmo>();
			}
			else
			{
				ChamberedRound = first.GetItemType<IAmmo>();
				_roundsInMagazine.Remove(first);
			}
		}
		else
		{
			ChamberedRound = null;
			loader.HandleEvent(EventType.ReadyGunEmpty, loader, this);
		}

		Changed = true;
	}

	public override void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
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
		                  .SelectNotNull(x => x.PrimaryTarget.GetItemType<IAmmo>()).First();
		var exemptions = new List<IGameItem>();
		if (ammo.Parent.Quantity > _prototype.InternalMagazineCapacity - _roundsInMagazine.Sum(x => x.Quantity))
		{
			ammo = ammo.Parent.Get(null, _prototype.InternalMagazineCapacity - _roundsInMagazine.Sum(x => x.Quantity))
			           .GetItemType<IAmmo>();
		}
		else
		{
			exemptions.Add(ammo.Parent);
			loader.Body.Take(ammo.Parent);
		}

		loader.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent),
			flags: OutputFlags.InnerWrap));
		var mergeTarget = _roundsInMagazine.FirstOrDefault(x => x.CanMerge(ammo.Parent));
		if (mergeTarget != null)
		{
			mergeTarget.Merge(ammo.Parent);
			ammo.Parent.Delete();
		}
		else
		{
			_roundsInMagazine.Add(ammo.Parent);
			ammo.Parent.ContainedIn = Parent;
		}

		plan.FinalisePlanWithExemptions(exemptions);
		Changed = true;
	}

	public override bool CanUnload(ICharacter loader)
	{
		return _roundsInMagazine.Any();
	}

	public override string WhyCannotUnload(ICharacter loader)
	{
		if (!_roundsInMagazine.Any())
		{
			return $"{Parent.HowSeen(loader, true)} is already unloaded.";
		}

		throw new ApplicationException("Unknown reason in GunGameItemComponent.WhyCannotUnload");
	}

	public override IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		if (!CanUnload(loader))
		{
			loader.Send(WhyCannotUnload(loader));
			return Enumerable.Empty<IGameItem>();
		}

		var results = new List<IGameItem>();
		if (_roundsInMagazine.Count > 1)
		{
			loader.OutputHandler.Handle(
				new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent,
					new PerceivableGroup(_roundsInMagazine))));
		}
		else
		{
			loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent,
				_roundsInMagazine.First())));
		}

		foreach (var item in _roundsInMagazine.ToList())
		{
			results.Add(item);
			if (loader.Body.CanGet(item, 0))
			{
				loader.Body.Get(item, silent: true);
			}
			else
			{
				item.RoomLayer = loader.RoomLayer;
				loader.Location.Insert(item);
			}
		}

		_roundsInMagazine.Clear();
		Changed = true;
		return results;
	}

	public override bool CanFire(ICharacter actor, IPerceivable target)
	{
		return true;
	}

	public override string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		throw new ApplicationException("Guns should always be able to fire.");
	}

	#region Overrides of FirearmBaseGameItemComponent

	/// <inheritdoc />
	protected override void HandleShellCasingOnFire(ICharacter actor, ICell originalLocation, IGameItem casing)
	{
		if (casing != null)
		{
			if (_prototype.EjectOnFire)
			{
				originalLocation.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground.", casing),
					flags: OutputFlags.Insigificant));
				casing.RoomLayer = actor.RoomLayer;
				originalLocation.Insert(casing);
			}
			else
			{
				ChamberedCasing = casing;
				casing.ContainedIn = Parent;
			}
		}
	}

	#endregion

	public override bool IsLoaded => _roundsInMagazine.Any();

	public override bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier))
		{
			readier.Send(WhyCannotUnready(readier));
			return false;
		}

		if (ChamberedRound != null)
		{
			readier.OutputHandler.Handle(
				new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent,
					ChamberedRound.Parent)));
			ChamberedRound.Parent.ContainedIn = null;
			if (readier.Body.CanGet(ChamberedRound.Parent, 0))
			{
				readier.Body.Get(ChamberedRound.Parent, silent: true);
			}
			else
			{
				ChamberedRound.Parent.RoomLayer = readier.RoomLayer;
				readier.Location.Insert(ChamberedRound.Parent);
			}

			ChamberedRound = null;
		}
		else if (ChamberedCasing != null)
		{
			readier.OutputHandler.Handle(
				new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent,
					ChamberedCasing)));
			ChamberedCasing.ContainedIn = null;
			if (readier.Body.CanGet(ChamberedCasing, 0))
			{
				readier.Body.Get(ChamberedCasing, silent: true);
			}
			else
			{
				ChamberedCasing.RoomLayer = readier.RoomLayer;
				readier.Location.Insert(ChamberedCasing);
			}

			ChamberedCasing = null;
		}
		else
		{
			readier.OutputHandler.Handle(
				new EmoteOutput(new Emote(_prototype.UnreadyEmoteNoChamberedRound, readier, readier, Parent)));
		}

		return true;
	}

	#endregion

	#region IGameItemComponent Overrides

	public override double ComponentWeight
	{
		get
		{
			return MagazineContents.Sum(x => x.Weight) + ChamberedRound?.Parent.Weight ??
			       0.0 + ChamberedCasing?.Weight ?? 0.0;
		}
	}


	public override double ComponentBuoyancy(double fluidDensity)
	{
		return MagazineContents.Sum(x => x.Buoyancy(fluidDensity)) +
		       (ChamberedRound?.Parent.Buoyancy(fluidDensity) ?? 0.0) +
		       (ChamberedCasing?.Buoyancy(fluidDensity) ?? 0.0);
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
			sb.AppendLine(_roundsInMagazine.Any()
				? $"It has {_roundsInMagazine.Select(x => x.HowSeen(voyeur)).ListToString()} in the magazine."
				: "It does not currently have any ammunition in the magazine.");
			sb.AppendLine($"The safety is currently {(Safety ? "on" : "off")}.");
			return sb.ToString();
		}

		if (type == DescriptionType.Evaluate)
		{
			var mw = (IMeleeWeapon)this;
			return
				$"This is a single-shot firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)}.\n This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override void Quit()
	{
		base.Quit();
		ChamberedRound?.Parent.Quit();
		ChamberedCasing?.Quit();
		foreach (var item in _roundsInMagazine.ToList())
		{
			item.Quit();
		}
	}

	public override void Delete()
	{
		base.Delete();
		if (ChamberedRound is not null)
		{
			ChamberedRound.Parent.ContainedIn = null;
		}
		ChamberedRound?.Parent.Delete();

		if (ChamberedCasing is not null)
		{
			ChamberedCasing.ContainedIn = null;
		}
		ChamberedCasing?.Delete();


		foreach (var item in _roundsInMagazine.ToList())
		{
			item.ContainedIn = null;
			item.Delete();
		}
	}

	public override void Login()
	{
		ChamberedRound?.Parent.Login();
		ChamberedCasing?.Login();
		foreach (var item in _roundsInMagazine.ToList())
		{
			item.Login();
		}
	}

	#endregion

	#region Overrides of FirearmBaseGameItemComponent

	/// <inheritdoc />
	protected override bool SemiAutomaticCycleOnFire => false;

	#endregion
}