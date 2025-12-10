using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Construction;
using MudSharp.Form.Audio;

namespace MudSharp.GameItems.Components;

public class BoltActionGameItemComponent : FirearmBaseGameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
	protected BoltActionGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BoltActionGameItemComponentProto)newProto;
	}

	#region Constructors

	public BoltActionGameItemComponent(BoltActionGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public BoltActionGameItemComponent(MudSharp.Models.GameItemComponent component,
		BoltActionGameItemComponentProto proto,
		IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BoltActionGameItemComponent(BoltActionGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(
			rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Magazine = Gameworld.TryGetItem(long.Parse(root.Element("Magazine").Value), true)
		                    ?.GetItemType<IContainer>();

		ChamberedCasing = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedCasing")?.Value ?? "0"), true);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BoltActionGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Magazine", Magazine?.Parent.Id ?? 0),
			new XElement("ChamberedRound", ChamberedRound?.Parent.Id ?? 0),
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Safety", Safety ? "true" : "false"),
			new XElement("ChamberedCasing", ChamberedCasing?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region IRangedWeapon Implementation

	public IGameItem ChamberedCasing { get; set; } //The casing waiting to be ejected for when you ready

	public IContainer Magazine { get; set; }

	public override IEnumerable<IGameItem> MagazineContents => Magazine?.Contents ?? Enumerable.Empty<IGameItem>();

	public override IEnumerable<IGameItem> AllContainedItems => MagazineContents.Concat([ChamberedCasing, ChamberedRound?.Parent, Magazine?.Parent]).SelectNotNull(x => x);

	WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
	WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;

	public override bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
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

	public override string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		if (Magazine != null)
		{
			return
				$"There is already a clip in the magazine of {Parent.HowSeen(loader)}, you should eject that first.";
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
					return $"You don't have a suitable magazine of ammunition to load {Parent.HowSeen(loader)}.";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		throw new ApplicationException("Unknown WhyCannotLoad reason in PistolGameItemComponent.WhyCannotLoad");
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

		var newRound = MagazineContents.SelectNotNull(x => x.GetItemType<IAmmo>())
		                               .FirstOrDefault(x => x.AmmoType.SpecificType == SpecificAmmoGrade &&
		                                                    x.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType
			                                                    .ModernFirearm));
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

	protected override bool SemiAutomaticCycleOnFire => false;

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
		                  .SelectNotNull(x => x.PrimaryTarget.GetItemType<IAmmoClip>()).First();


		loader.OutputHandler.Handle(new EmoteOutput(
			new Emote(_prototype.LoadEmote, loader, loader, Parent, ammo.Parent),
			flags: OutputFlags.InnerWrap));

		loader.Body.Take(ammo.Parent);
		Magazine = ammo;
		ammo.Parent.ContainedIn = Parent;

		var exemptions = new List<IGameItem> { ammo.Parent };
		plan.FinalisePlanWithExemptions(exemptions);
		Changed = true;
	}

	public override bool CanUnload(ICharacter loader)
	{
		return Magazine != null;
	}

	public override string WhyCannotUnload(ICharacter loader)
	{
		if (Magazine == null)
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

		loader.OutputHandler.Handle(
			new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, Magazine.Parent)));
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

	public override bool IsLoaded => Magazine != null;

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
				$"This is a single-shot firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing.\nIt takes ammunition of type {WeaponType.SpecificAmmunitionGrade.Colour(Telnet.Green)} and accepts clips of type {_prototype.ClipType.Colour(Telnet.Green)}.\n This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public override void Quit()
	{
		base.Quit();
		ChamberedRound?.Parent.Quit();
		ChamberedCasing?.Quit();
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

		if (ChamberedCasing is not null)
		{
			ChamberedCasing.ContainedIn = null;
		}
		ChamberedCasing?.Delete();
		

		if (Magazine is not null)
		{
			Magazine.Parent.ContainedIn = null;
		}
		Magazine?.Parent.Delete();
		
	}

	public override void Login()
	{
		ChamberedRound?.Parent.Login();
		ChamberedCasing?.Login();
		Magazine?.Parent.Login();
	}

	#endregion
}