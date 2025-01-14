using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
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
public class MusketGameItemComponent : GameItemComponent, IJammableWeapon, IBelt, IMeleeWeapon
{
	protected MusketGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MusketGameItemComponentProto)newProto;
	}

	#region Constructors
	public MusketGameItemComponent(MusketGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public MusketGameItemComponent(Models.GameItemComponent component, MusketGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MusketGameItemComponent(MusketGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		IsReadied = bool.Parse(root.Element("IsReadied").Value);
		NeedsCleaning = bool.Parse(root.Element("NeedsCleaning").Value);
		IsJammed = bool.Parse(root.Element("IsJammed").Value);
		LoadStage = int.Parse(root.Element("LoadStage").Value);
		PrimaryWieldedLocation = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;
		foreach (var element in root.Element("Magazine").Elements("Item"))
		{
			var item = Gameworld.TryGetItem(long.Parse(element.Value), true);
			if (item is null)
			{
				continue;
			}

			_magazineContents.Add(item);
		}

		var connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Bayonet")?.Value ?? "0"), true);
		if (connectedItem?.IsItemType<IBeltable>() == true)
		{
			_bayonet = connectedItem.GetItemType<IBeltable>();
			connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
		}

		connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Sights")?.Value ?? "0"), true);
		if (connectedItem?.IsItemType<IBeltable>() == true)
		{
			_sights = connectedItem.GetItemType<IBeltable>();
			connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
		}

		connectedItem = Gameworld.TryGetItem(long.Parse(root.Element("Ramrod")?.Value ?? "0"), true);
		if (connectedItem?.IsItemType<IBeltable>() == true)
		{
			_ramrod = connectedItem.GetItemType<IBeltable>();
			connectedItem.GetItemType<IBeltable>().ConnectedTo = this;
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MusketGameItemComponent(this, newParent, temporary);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IsReadied", IsReadied),
			new XElement("LoadStage", LoadStage),
			new XElement("NeedsCleaning", NeedsCleaning),
			new XElement("IsJammed", IsJammed),
			new XElement("TapLoaded", TapLoaded),
			new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0),
			new XElement("Bayonet", _bayonet?.Parent.Id ?? 0),
			new XElement("Sights", _sights?.Parent.Id ?? 0),
			new XElement("Ramrod", _ramrod?.Parent.Id ?? 0),
			new XElement("Magazine", 
				from item in _magazineContents
				select new XElement("Item", item.Id)
			)
		).ToString();
	}
	#endregion

	public override double ComponentWeight
	{
		get
		{
			return MagazineContents.Sum(x => x.Weight);
		}
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return MagazineContents.Sum(x => x.Buoyancy(fluidDensity));
	}

	public override void Quit()
	{
		base.Quit();
		foreach (var item in MagazineContents)
		{
			item.Quit();
		}
		_bayonet?.Parent.Quit();
		_ramrod?.Parent.Quit();
		_sights?.Parent.Quit();
	}

	public override void Delete()
	{
		base.Delete();
		foreach (var item in MagazineContents.ToList())
		{
			item.Delete();
		}
		_bayonet?.Parent.Delete();
		_ramrod?.Parent.Delete();
		_sights?.Parent.Delete();
	}

	public override void Login()
	{
		foreach (var item in MagazineContents)
		{
			item.Login();
		}

		_bayonet?.Parent.Login();
		_ramrod?.Parent.Login();
		_sights?.Parent.Login();
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
			switch (LoadStage)
			{
				case 0:
					sb.AppendLine($"It is not loaded.");
					break;
				case 1:
					sb.AppendLine($"It has been cleaned but not yet loaded with any shot or powder.");
					break;
				case 2:
					sb.AppendLine($"It has been loaded with powder but requires a musket ball to be ready to shoot.");
					break;
				case 3:
					sb.AppendLine($"It has been loaded with shot but requires tamping before it is ready to shoot.");
					break;
				case 4:
					sb.AppendLine($"It is fully loaded.");
					break;
			}
			sb.AppendLine($"The firing hammer {(IsReadied ? "is" : "is not")} in a position to fire.");
			if (IsJammed)
			{
				sb.AppendLine($"It is jammed and needs to be unjammed before being used.");
			}
			return sb.ToString();
		}

		if (type == DescriptionType.Evaluate)
		{
			var mw = (IMeleeWeapon)this;
			return
				$@"This is a muzzle-loading firearm of type {WeaponType.Name.Colour(Telnet.Cyan)}.
It uses the {WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill for firing and the {WeaponType.OperateTrait.Name.ColourValue()} skill for operation.
It takes musket balls of a bore equal or less than {$"{_prototype.BarrelBore.ToString("N2", voyeur)}in".ColourValue()}.
This is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.
It uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.
It is classified as {WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	#region Implementation of IWieldable

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

	/// <inheritdoc />
	public bool AlwaysRequiresTwoHandsToWield => WeaponType.AlwaysRequiresTwoHandsToWield;

	#endregion

	#region Implementation of IDamageSource

	/// <inheritdoc />
	public IDamage GetDamage(IPerceiver perceiverSource, OpposedOutcome opposedOutcome)
	{
		throw new NotImplementedException();
	}

	#endregion

	#region Implementation of IUseTrait

	/// <inheritdoc />
	public ITraitDefinition Trait => _prototype.RangedWeaponType.FireTrait;

	#endregion

	#region Implementation of IRangedWeapon

	/// <inheritdoc />
	public string FireVerbForEchoes => "fire|fires";

	/// <inheritdoc />
	public bool CanBeAimedAtSelf => true;

	/// <inheritdoc />
	IWeaponType IMeleeWeapon.WeaponType => _weaponType;

	/// <inheritdoc />
	public WeaponClassification Classification => _prototype.RangedWeaponType.Classification;

	/// <inheritdoc />
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

	/// <inheritdoc />
	public bool ReadyToFire => IsLoaded && IsReadied;

	/// <inheritdoc />
	/// 0 - not loaded, 1 - clean, 2 - powder, 3 - musketball, 4 - ramrod
	public int LoadStage { get; set; }

	/// <inheritdoc />
	public bool IsLoaded => LoadStage >= 4;

	/// <inheritdoc />
	public bool IsReadied { get; set; }

	private bool _isJammed;

	public bool IsJammed
	{
		get => _isJammed;
		set
		{
			_isJammed = value;
			Changed = true;
		}
	}

	/// <inheritdoc />
	public bool CanUnjam(ICharacter actor)
	{
		if (!IsJammed)
		{
			return false;
		}

		var plan = _prototype.LoadTemplateClean.CreatePlan(actor);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public string WhyCannotUnjam(ICharacter actor)
	{
		if (!IsJammed)
		{
			return $"You cannot unjam {Parent.HowSeen(actor)} because it is not jammed.";
		}

		var plan = _prototype.LoadTemplateClean.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				return $"You cannot unjam {Parent.HowSeen(actor)} because you do not have a suitable tool.";
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				return $"You cannot unjam {Parent.HowSeen(actor)} because you don't have enough free, working {actor.Body.WielderDescriptionPlural}.";
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				return $"You cannot unjam {Parent.HowSeen(actor)} because you don't have enough free, working {actor.Body.WielderDescriptionPlural}.";
		}

		throw new ApplicationException("Unknown WhyCannotUnjam reason in MusketGameItemComponent.WhyCannotUnjam");
	}

	/// <inheritdoc />
	public bool Unjam(ICharacter actor)
	{
		if (!CanUnjam(actor))
		{
			actor.OutputHandler.Send(WhyCannotUnjam(actor));
			return false;
		}

		var plan = _prototype.LoadTemplateClean.CreatePlan(actor);
		var results = plan.ExecuteWholePlan();
		var ramrod = results.FirstOrDefault(x => x.OriginalReference == "ramrod").PrimaryTarget;
		actor.AddEffect(new UnjammingGun(actor, this, ramrod, plan.AssociatedEffects.FirstOrDefault(x => x.TargetItem == ramrod).DesiredState), UnjammingGun.EffectDuration(actor, this, ramrod));
		return true;
	}

	public string StartUnjamEmote => _prototype.StartUnjamEmote;
	public string FinishUnjamEmote => _prototype.FinishUnjamEmote;
	public string FailUnjamEmote => _prototype.FailUnjamEmote;

	public bool NeedsCleaning { get; set; }
	public bool TapLoaded { get; set; }

	private readonly List<IGameItem> _magazineContents = new();

	/// <inheritdoc />
	public IEnumerable<IGameItem> MagazineContents => _magazineContents;

	public IEnumerable<IGameItem> AllContainedItems => MagazineContents;

	/// <inheritdoc />
	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

	/// <inheritdoc />
	public Difficulty BaseBlockDifficulty => Difficulty.Insane;

	/// <inheritdoc />
	public Difficulty BaseDodgeDifficulty => Difficulty.Impossible;

	/// <inheritdoc />
	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		switch (LoadStage)
		{
			case 0:
				switch (mode)
				{
					case LoadMode.Blank:
					case LoadMode.Normal:
					case LoadMode.Tap:
						return _prototype.LoadTemplateClean.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
				}

				goto case 1;
			case 1:
				// Try Cartridges first
				if (_prototype.LoadTemplateLoadCartridge.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
				{
					return true;
				}

				if (_prototype.LoadTemplateLoadPowder.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
				{
					return true;
				}

				return false;
			case 2:
				return _prototype.LoadTemplateLoadBall.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
			case 3:
				return _prototype.LoadTemplateLoadRamrod.CreatePlan(loader).PlanIsFeasible() == InventoryPlanFeasibility.Feasible;
			default:
				return false;
		}
	}

	/// <inheritdoc />
	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		switch (LoadStage)
		{
			case 0:
				switch (mode)
				{
					case LoadMode.Blank:
					case LoadMode.Normal:
					case LoadMode.Tap:
						switch (_prototype.LoadTemplateClean.CreatePlan(loader).PlanIsFeasible())
						{
							case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
							case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
								return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
							case InventoryPlanFeasibility.NotFeasibleMissingItems:
								return $"You don't have a ramrod that you can use to clean your weapon.";
							default:
								throw new ArgumentOutOfRangeException();
						}
				}

				goto case 1;
			case 1:
				// Try Cartridges first
				switch (_prototype.LoadTemplateLoadCartridge.CreatePlan(loader).PlanIsFeasible())
				{
					case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
						return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
					case InventoryPlanFeasibility.NotFeasibleMissingItems:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				switch (_prototype.LoadTemplateLoadPowder.CreatePlan(loader).PlanIsFeasible())
				{
					case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
						return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
					case InventoryPlanFeasibility.NotFeasibleMissingItems:
						return $"You don't have either gunpowder or a cartridge that you could use to load your weapon.";
					default:
						throw new ArgumentOutOfRangeException();
				}
			case 2:
				switch (_prototype.LoadTemplateLoadBall.CreatePlan(loader).PlanIsFeasible())
				{
					case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
						return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
					case InventoryPlanFeasibility.NotFeasibleMissingItems:
						return $"You don't have a musket ball that you could use to load your weapon.";
					default:
						throw new ArgumentOutOfRangeException();
				}
			case 3:
				switch (_prototype.LoadTemplateLoadRamrod.CreatePlan(loader).PlanIsFeasible())
				{
					case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
						return $"You don't have enough {loader.Body.WielderDescriptionPlural} to carry out that action.";
					case InventoryPlanFeasibility.NotFeasibleMissingItems:
						return $"You don't have a ramrod that you can use to tamp the ammunition into your weapon.";
					default:
						throw new ArgumentOutOfRangeException();
				}
			default:
				throw new ApplicationException("Unknown WhyCannotLoad reason in MusketGameItemComponent.WhyCannotLoad");
		}
	}

	/// <inheritdoc />
	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		void AddEffect()
		{
			var check = Gameworld.GetCheck(CheckType.LoadMusket);
			var difficulty = Difficulty.Normal;
			if (loader.Combat is not null)
			{
				difficulty = difficulty.StageUp(2);
			}

			if (loader.IsEngagedInMelee)
			{
				difficulty = difficulty.StageUp(2);
			}

			var result = check.Check(loader, difficulty, Parent, this);
			loader.AddEffect(new LoadingMusket(loader, this, mode), TimeSpan.FromSeconds(
				LoadStage switch
				{
					1 => 10.0 + (result.Outcome.CheckDegrees() * -2.0),
					2 => 10.0 + (result.Outcome.CheckDegrees() * -2.0),
					3 => mode.In(LoadMode.Tap, LoadMode.TapNoClean) ?
						4.0 + (result.Outcome.CheckDegrees() * -1.0) :
						10.0 + (result.Outcome.CheckDegrees() * -1.5),
					_ => 1.0
				}
			));
		}

		if (!CanLoad(loader))
		{
			loader.Send(WhyCannotLoad(loader));
			return;
		}

		IInventoryPlan plan;
		IEnumerable<InventoryPlanActionResult> results;
		IGameItem ramrod;
		switch (LoadStage)
		{
			case 0:
				if (!NeedsCleaning)
				{
					goto case 1;
				}

				switch (mode)
				{
					case LoadMode.Blank:
					case LoadMode.Normal:
					case LoadMode.Tap:
						plan = _prototype.LoadTemplateClean.CreatePlan(loader);
						results = plan.ExecuteWholePlan();
						ramrod = results.FirstOrDefault(x => x.OriginalReference == "ramrod").PrimaryTarget;
						loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteClean, loader, loader, Parent, ramrod), flags: OutputFlags.InnerWrap));
						plan.FinalisePlanWithExemptions([Parent]);
						LoadStage = 1;
						NeedsCleaning = false;
						Changed = true;
						AddEffect();
						return;
				}

				goto case 1;
			case 1:
				// Try Cartridges first
				plan = _prototype.LoadTemplateLoadCartridge.CreatePlan(loader);
				if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
				{
					results = plan.ExecuteWholePlan();
					var cartridge = results.First(x => x.OriginalReference == "cartridge").PrimaryTarget;
					loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteCartridge, loader, loader, Parent, cartridge), flags: OutputFlags.InnerWrap));
					loader.Body.Take(cartridge);
					cartridge.ContainedIn = Parent;
					_magazineContents.Add(cartridge);
					plan.FinalisePlanWithExemptions([cartridge, Parent]);
					LoadStage = 3;
					Changed = true;
					AddEffect();
					return;
				}

				plan = _prototype.LoadTemplateLoadPowder.CreatePlan(loader);
				plan.ExecuteWholePlan();
				var powder = CommodityGameItemComponentProto.CreateNewCommodity(MusketGameItemComponentProto.GunpowderMaterial, _prototype.PowderVolumePerShot, null, false);
				powder.ContainedIn = Parent;
				_magazineContents.Add(powder);
				loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmotePowder, loader, loader, Parent, powder), flags: OutputFlags.InnerWrap));
				plan.FinalisePlanWithExemptions([Parent]);
				LoadStage = 2;
				Changed = true;
				AddEffect();
				return;
			case 2:
				plan = _prototype.LoadTemplateLoadBall.CreatePlan(loader);
				results = plan.ExecuteWholePlan();
				var ball = results.First(x => x.OriginalReference == "ball").PrimaryTarget;
				loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteBall, loader, loader, Parent, ball, null), flags: OutputFlags.InnerWrap));
				loader.Body.Take(ball);
				ball.ContainedIn = Parent;
				_magazineContents.Add(ball);
				plan.FinalisePlanWithExemptions([ball, Parent]);
				LoadStage = 3;
				Changed = true;
				AddEffect();
				return;
			case 3:
				if (mode.In(LoadMode.Tap, LoadMode.TapNoClean))
				{
					plan = _prototype.LoadTemplateFinishLoading.CreatePlan(loader);
					results = plan.ExecuteWholePlan();
					loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteTap, loader, loader, Parent), flags: OutputFlags.InnerWrap));
					plan.FinalisePlanWithExemptions([Parent]);
					LoadStage = 4;
					TapLoaded = true;
					Changed = true;
					return;
				}
				plan = _prototype.LoadTemplateLoadRamrod.CreatePlan(loader);
				results = plan.ExecuteWholePlan();
				ramrod = results.FirstOrDefault(x => x.OriginalReference == "ramrod").PrimaryTarget;
				loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.LoadEmoteRamrod, loader, loader, Parent, ramrod), flags: OutputFlags.InnerWrap));
				plan.FinalisePlanWithExemptions([Parent]);
				plan = _prototype.LoadTemplateFinishLoading.CreatePlan(loader);
				results = plan.ExecuteWholePlan();
				plan.FinalisePlanNoRestore();
				LoadStage = 4;
				TapLoaded = false;
				Changed = true;
				return;
			default:
				return;
		}
	}

	/// <inheritdoc />
	public bool CanReady(ICharacter readier)
	{
		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public string WhyCannotReady(ICharacter readier)
	{
		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return
				$"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown WhyCannotReady reason in ready MusketGameItemComponent.");
	}

	/// <inheritdoc />
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

	/// <inheritdoc />
	public bool CanUnready(ICharacter readier)
	{
		return true;
	}

	/// <inheritdoc />
	public string WhyCannotUnready(ICharacter readier)
	{
		throw new ApplicationException("Should always be able to unready MusketGameItemComponent");
	}

	/// <inheritdoc />
	public bool Unready(ICharacter readier)
	{
		if (!CanUnready(readier))
		{
			readier.Send(WhyCannotUnready(readier));
			return false;
		}

		readier.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnreadyEmote, readier, readier, Parent),
			flags: OutputFlags.InnerWrap));
		IsReadied = false;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public bool CanUnload(ICharacter loader)
	{
		if (IsJammed)
		{
			return false;
		}

		if (_magazineContents.Count == 0 && LoadStage <= 1)
		{
			return false;
		}

		return true;
	}

	/// <inheritdoc />
	public string WhyCannotUnload(ICharacter loader)
	{
		if (IsJammed)
		{
			return $"{Parent.HowSeen(loader, true)} is jammed and the jam must be cleared first.";
		}

		if (_magazineContents.Count == 0 && LoadStage <= 1)
		{
			return $"{Parent.HowSeen(loader, true)} is already unloaded.";
		}

		throw new ApplicationException("Unknown reason in MusketGameItemComponent.WhyCannotUnload");
	}

	/// <inheritdoc />
	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		if (!CanUnload(loader))
		{
			loader.Send(WhyCannotUnload(loader));
			return Enumerable.Empty<IGameItem>();
		}

		IGameItem removed = null;
		IPerceivable removedPerceivable = null;
		if (_magazineContents.Count > 1)
		{
			var contents = _magazineContents.ToList();
			_magazineContents.Clear();
			foreach (var item in contents)
			{
				item.ContainedIn = null;
			}

			removed = PileGameItemComponentProto.CreateNewBundle(contents);
			removedPerceivable = removed;
		}
		else if (_magazineContents.Count == 1)
		{
			removed = _magazineContents[0];
			_magazineContents.Clear();
			removed.ContainedIn = null;
			removedPerceivable = removed;
		}
		else
		{
			removedPerceivable = new DummyPerceivable("nothing");
		}
		
		Changed = true;
		loader.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.UnloadEmote, loader, loader, Parent, removedPerceivable)));
		LoadStage = 0;
		if (removed is not null)
		{
			loader.Location.Insert(removed);
			removed.RoomLayer = loader.RoomLayer;
			removed.SetPosition(PositionUndefined.Instance, PositionModifier.Behind, loader, null);
		}

		return removed is not null ? [removed] : [];
	}

	/// <inheritdoc />
	public bool CanFire(ICharacter actor, IPerceivable target)
	{
		return ReadyToFire;
	}

	/// <inheritdoc />
	public string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		if (IsJammed)
		{
			return $"You cannot fire {Parent.HowSeen(actor)} because it is jammed.";
		}

		if (!IsLoaded)
		{
			return $"You cannot fire {Parent.HowSeen(actor)} because it is not loaded.";
		}

		if (!IsReadied)
		{
			return $"You cannot fire {Parent.HowSeen(actor)} because it has not been readied.";
		}

		throw new ApplicationException(
			"Unknown WhyCannotFire reason in MusketGameItemComponent.WhyCannotFire");
	}

	/// <inheritdoc />
	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		IAmmunitionType type = null;
		IGameItemProto bulletProto = null;
		IGameItem ball = null;
		
		// Do we misfire?
		var misfire = false;
		var catastrophy = false;
		var cartridge = _magazineContents.FirstOrDefault()?.GetItemType<MusketCartridgeGameItemComponent>();
		var wadused = false;
		IAmmo ammo = null;

		// If there isn't exactly what we expect in the magazine, we always misfire
		switch (_magazineContents.Count)
		{
			case 1:
				if (cartridge is null)
				{
					misfire = true;
					Gameworld.DebugMessage("Musket misfired due to unexpected item in barrel (count 1)");
				}
				else
				{
					type = cartridge.AmmoType;
					bulletProto = cartridge.BulletProto;
					ammo = cartridge;
					wadused = true;
				}

				break;
			case 2:
			case 3:
				var powder = _magazineContents.FirstOrDefault(x => x.GetItemType<CommodityGameItemComponent>() is {} commodity && commodity.Material == MusketGameItemComponentProto.GunpowderMaterial);
				ball = _magazineContents.FirstOrDefault(x => x.IsItemType<MusketBallGameItemComponent>());
				var wad = _magazineContents.FirstOrDefault(x => x.IsA(MusketGameItemComponentProto.WadItemTag));
				if (powder is null || ball is null || (wad is null && _magazineContents.Count == 3))
				{
					misfire = true;
					Gameworld.DebugMessage("Musket misfired due to unexpected or missing item in barrel (count 2 or 3)");
				}
				else
				{
					var ballComp = ball.GetItemType<MusketBallGameItemComponent>();
					type = ballComp.AmmoType;
					bulletProto = ballComp.BulletProto;
					ammo = ballComp;
				}
				

				if (wad is not null)
				{
					wadused = true;
				}
				break;
			default:
				misfire = true;
				Gameworld.DebugMessage("Musket misfired due to unexpected item in barrel (count >3)");
				break;
		}

		catastrophy = _magazineContents.Count(x => x.IsItemType<MusketBallGameItemComponent>() || x.IsItemType<MusketCartridgeGameItemComponent>() || x.IsItemType<CommodityGameItemComponent>()) > 2;

		// Otherwise use the expression
		if (!misfire)
		{
			var misExpression = _prototype.MisfireChance;
			misExpression.Formula.Parameters["operate"] = actor.TraitValue(WeaponType.OperateTrait);
			misExpression.Formula.Parameters["skipclean"] = NeedsCleaning ? 1.0 : 0.0;
			misExpression.Formula.Parameters["precipitation"] = actor.Location.CurrentWeather(actor) is not null ? (actor.Location.CurrentWeather(actor)?.Precipitation ?? PrecipitationLevel.Parched).PrecipitationIntensityForGunpowder() : 0.0;
			misExpression.Formula.Parameters["gunquality"] = (int)Parent.Quality;
			misExpression.Formula.Parameters["cartridgeused"] = _magazineContents.Count == 1 && cartridge is not null ? 1.0 : 0.0;
			misExpression.Formula.Parameters["cartridgequality"] = _magazineContents.Count == 1 && cartridge is not null ? (int)cartridge.Parent.Quality : (int)ItemQuality.Standard;
			misExpression.Formula.Parameters["condition"] = Parent.Condition;
			misExpression.Formula.Parameters["wadused"] = wadused ? 1.0 : 0.0;
			misExpression.Formula.Parameters["wetpowder"] = _magazineContents.Any(x => x.AffectedBy<ILiquidContaminationEffect>()) ? 1.0 : 0.0;
			misExpression.Formula.Parameters["taploaded"] = TapLoaded ? 1.0 : 0.0;
			var chance = misExpression.Evaluate(actor, WeaponType.FireTrait, TraitBonusContext.MusketMisfire);
			var roll = RandomUtilities.DoubleRandom(0.0, 1.0);
			Gameworld.DebugMessage($"Musket misfire chance #2{chance:P3}#0 rolled {roll:P3}");
			if (roll < chance)
			{
				misfire = true;
			}
		}

		var vicinity = actor.CellsInVicinity((uint)AudioVolume.ExtremelyLoud, false, false)
		                    .Except(actor.Location);

		// Handle all the unloading etc
		Changed = true;
		LoadStage = 0;
		NeedsCleaning = true;
		IsReadied = false;

		// If we misfired, work out if it's a catastrophy
		if (misfire && catastrophy)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteCatastrophy, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			var contents = _magazineContents.ToList();
			_magazineContents.Clear();
			foreach (var item in contents)
			{
				item.Delete();
			}

			foreach (var location in vicinity)
			{
				if (location.Characters.Any() || location.GameItems.Any())
				{
					var directions = location.ExitsBetween(actor.Location, 10).ToList();
					location.Handle(new EmoteOutput(
						new Emote($"An explosion can be heard {directions.DescribeDirectionsToFrom()}.", Parent),
						flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
				}
			}

			var damages = new List<IDamage>();
			foreach (var damageType in new[] { DamageType.Shrapnel, DamageType.Shockwave, DamageType.Burning })
			{
				var damageAmount = _prototype.CatastrophyDamageFormula.EvaluateDouble();
				damages.Add(new Damage
				{
					DamageType = damageType,
					DamageAmount = damageAmount,
					PainAmount = damageAmount,
					StunAmount = damageAmount
				});
			}

			var explosion = new ExplosiveDamage(damages, 0.0, Parent.Size, Proximity.Immediate, false, null);
			var wounds = new List<IWound>();
			wounds.AddRange(actor.Body.InventoryExploded(Parent, explosion));
			wounds.AddRange(Parent.PassiveSufferDamage(explosion, Proximity.Intimate, Facing.Front));
			wounds.ProcessPassiveWounds();
			return;
		}

		// If we misfired, work out if it's a jam
		var isJam = false;
		if (misfire)
		{
			var jamExpression = _prototype.JamChance;
			jamExpression.Formula.Parameters["operate"] = actor.TraitValue(WeaponType.OperateTrait);
			jamExpression.Formula.Parameters["skipclean"] = NeedsCleaning ? 1.0 : 0.0;
			jamExpression.Formula.Parameters["precipitation"] = actor.Location.CurrentWeather(actor) is not null ? (actor.Location.CurrentWeather(actor)?.Precipitation ?? PrecipitationLevel.Parched).PrecipitationIntensityForGunpowder() : 0.0;
			jamExpression.Formula.Parameters["gunquality"] = (int)Parent.Quality;
			jamExpression.Formula.Parameters["cartridgeused"] = _magazineContents.Count == 1 && cartridge is not null ? 1.0 : 0.0;
			jamExpression.Formula.Parameters["cartridgequality"] = _magazineContents.Count == 1 && cartridge is not null ? (int)cartridge.Parent.Quality : (int)ItemQuality.Standard;
			jamExpression.Formula.Parameters["condition"] = Parent.Condition;
			jamExpression.Formula.Parameters["wadused"] = wadused ? 1.0 : 0.0;
			jamExpression.Formula.Parameters["wetpowder"] = _magazineContents.Any(x => x.AffectedBy<ILiquidContaminationEffect>()) ? 1.0 : 0.0;
			jamExpression.Formula.Parameters["taploaded"] = TapLoaded ? 1.0 : 0.0;
			var chance = jamExpression.Evaluate(actor, WeaponType.FireTrait, TraitBonusContext.MusketMisfire);
			var roll = RandomUtilities.DoubleRandom(0.0, 1.0);
			Gameworld.DebugMessage($"Musket jam chance #2{chance:P3}#0 rolled {roll:P3}");
			if (roll < chance)
			{
				isJam = true;
			}
		}

		// Handle echoing the gunshot
		foreach (var location in vicinity)
		{
			if (location.Characters.Any() || location.GameItems.Any())
			{
				var directions = location.ExitsBetween(actor.Location, 10).ToList();
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

		var magContents = _magazineContents.ToList();
		_magazineContents.Clear();
		foreach (var item in magContents)
		{
			item.Delete();
		}

		if (isJam)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteJam, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			IsJammed = true;
			return;
		}

		if (misfire)
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmoteMisfire, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.FireEmote, actor, actor, target ?? (IPerceivable)new DummyPerceivable("the air"), Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		var bullet = bulletProto?.CreateNew() ?? ball;
		ammo.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, bullet, WeaponType, defenseEmote);
	}

	#endregion

	#region Implementation of IBelt

	/// <inheritdoc />
	public SizeCategory MaximumSize => Parent.Size;

	/// <inheritdoc />
	public int MaximumNumberOfBeltedItems => 3;

	/// <inheritdoc />
	private IBeltable _bayonet;

	private IBeltable _ramrod;
	private IBeltable _sights;
	private IWeaponType _weaponType;

	public IEnumerable<IBeltable> ConnectedItems
	{
		get
		{
			if (_bayonet is not null)
			{
				yield return _bayonet;
			}

			if (_ramrod is not null)
			{
				yield return _ramrod;
			}

			if (_sights is not null)
			{
				yield return _sights;
			}
		}
	}

	public void AddConnectedItem(IBeltable item)
	{
		// TODO - bayonets
		// TODO - sights
		item.ConnectedTo?.RemoveConnectedItem(item);
		_ramrod = item;
		item.ConnectedTo = this;
		Changed = true;
	}

	public void RemoveConnectedItem(IBeltable item)
	{
		// TODO - bayonets
		// TODO - sights
		if (_ramrod == item)
		{
			_ramrod = null;
		}
		item.ConnectedTo = null;
		Changed = true;
	}

	public IBeltCanAttachBeltableResult CanAttachBeltable(IBeltable beltable)
	{
		if (beltable.Parent.Size > MaximumSize)
		{
			return IBeltCanAttachBeltableResult.FailureTooLarge;
		}

		// TODO - bayonets
		// TODO - sights
		if (beltable.Parent.IsA(MusketGameItemComponentProto.RamrodTag))
		{
			if (_ramrod is not null)
			{
				return IBeltCanAttachBeltableResult.FailureExceedMaximumNumber;
			}

			return IBeltCanAttachBeltableResult.Success;
		}
		
		return IBeltCanAttachBeltableResult.NotValidType;
	}

	#endregion
}
