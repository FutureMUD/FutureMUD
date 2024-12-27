using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class ThrownWeaponGameItemComponent : GameItemComponent, IRangedWeapon, IMeleeWeapon
{
	protected ThrownWeaponGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ThrownWeaponGameItemComponent(this, newParent, temporary);
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
		_prototype = (ThrownWeaponGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Wielded", PrimaryWieldedLocation?.Id ?? 0)).ToString();
	}

	#region Implementation of IRangedWeapon

	public bool CanBeAimedAtSelf => false;
	public string FireVerbForEchoes => "throw|throws";
	IRangedWeaponType IRangedWeapon.WeaponType => _prototype.RangedWeaponType;
	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;
	WeaponClassification IRangedWeapon.Classification => _prototype.RangedWeaponType.Classification;
	WeaponClassification IMeleeWeapon.Classification => _prototype.MeleeWeaponType.Classification;
	public Difficulty AimDifficulty => _prototype.RangedWeaponType.BaseAimDifficulty;
	public Difficulty BaseBlockDifficulty => Difficulty.Easy;
	public Difficulty BaseDodgeDifficulty => Difficulty.Hard;

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

	public bool AlwaysRequiresTwoHandsToWield => _prototype.RangedWeaponType.AlwaysRequiresTwoHandsToWield;
	public bool ReadyToFire => true;

	public int LoadStage => 0;

	public IEnumerable<IGameItem> MagazineContents => Enumerable.Empty<IGameItem>();

	public bool IsLoaded => true;

	public bool IsReadied => true;

	public bool CanUnload(ICharacter loader)
	{
		return false;
	}

	public string WhyCannotUnload(ICharacter loader)
	{
		return "That is not something that can be unloaded. What would you do, just drop it?";
	}

	public IEnumerable<IGameItem> Unload(ICharacter loader)
	{
		return Enumerable.Empty<IGameItem>();
	}

	public bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		return false;
	}

	public string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		return "That is not something that needs to be loaded! Just throw the thing!";
	}

	public void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)
	{
		// Do nothing
	}

	public bool CanFire(ICharacter actor, IPerceivable target)
	{
		return actor.Body.CanUnwield(Parent);
	}

	public string WhyCannotFire(ICharacter actor, IPerceivable target)
	{
		if (!actor.Body.CanUnwield(Parent))
		{
			return actor.Body.WhyCannotUnwield(Parent);
		}

		throw new NotImplementedException(
			"Unknown WhyCannotFire reason in ThrownWeaponGameItemComponent.WhyCannotFire");
	}

	public void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
	{
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ hurl|hurls $1 at $0.", actor, target, Parent),
			style: OutputStyle.CombatMessage));
		if (defenseEmote != null)
		{
			originalTarget.OutputHandler.Handle(defenseEmote);
		}

		actor.Aim = null;
		actor.Body.Take(Parent);
		var path = actor.PathBetween(target, 10, false, false, true);
		var dirDesc = path.Select(x => x.OutboundDirection).DescribeDirection();
		var oppDirDesc = path.Select(x => x.OutboundDirection).DescribeOppositeDirection();
		foreach (var cell in actor.CellsUnderneathFlight(target, 10))
		{
			cell.Handle(
				cell.OutdoorsType(null) == CellOutdoorsType.Outdoors
					? new EmoteOutput(new Emote(
						$"@ fly|flies overhead from the {oppDirDesc} towards the {dirDesc}", Parent))
					: new EmoteOutput(
						new Emote($"@ fly|flies through the area from the {oppDirDesc} towards the {dirDesc}",
							Parent)));
		}

		if (actor.Location != target.Location)
		{
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote($"$1 $1|fly|flies in from the {oppDirDesc}.", target, target, Parent)));
		}
		else if (actor.RoomLayer != target.RoomLayer)
		{
			target.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					$"$1 $1|fly|flies in from {(target.RoomLayer.IsHigherThan(actor.RoomLayer) ? "below" : "above")}.",
					target, target, Parent), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

		_prototype.RangedWeaponType.DamageBonusExpression.Formula.Parameters["quality"] = (int)Parent.Quality;
		_prototype.RangedWeaponType.DamageBonusExpression.Formula.Parameters["degrees"] =
			(int)defenseOutcome.Degree;
		_prototype.RangedWeaponType.DamageBonusExpression.Formula.Parameters["range"] = actor.DistanceBetween(
			target, 10);
		var damage = new Damage
		{
			ActorOrigin = actor,
			ToolOrigin = Parent,
			Bodypart = bodypart,
			DamageAmount =
				_prototype.RangedWeaponType.DamageBonusExpression.Evaluate(actor,
					_prototype.RangedWeaponType.FireTrait),
			DamageType = _prototype.MeleeWeaponType.Attacks.FirstMax(x => x.Weighting).Profile.DamageType,
			LodgableItem = Parent
		};

		var wounds = new List<IWound>();
		if (shotOutcome.IsPass() && coverOutcome.IsFail() && target.Cover != null)
		{
			// Shot would've hit if it wasn't for cover
			var strikeCover = target.Cover.Cover.CoverType == CoverType.Hard || shotOutcome == Outcome.MajorPass ||
			                  coverOutcome == Outcome.MinorFail;
			if (strikeCover)
			{
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote($"The {Parent.Name} strikes $?1|$1, ||$$0's cover!", target, target,
						target.Cover.CoverItem?.Parent)));
				actor.Send("You hit your target's cover instead.".Colour(Telnet.Yellow));
				wounds.AddRange(target.Cover?.CoverItem?.Parent.PassiveSufferDamage(damage) ??
				                Enumerable.Empty<IWound>());
				wounds.ProcessPassiveWounds();
				defenseOutcome = new OpposedOutcome(OpposedOutcomeDirection.Opponent, OpposedOutcomeDegree.Total);
			}
		}

		if (defenseOutcome.Outcome == OpposedOutcomeDirection.Opponent)
		{
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote(
					$"The {Parent.Name} {(shotOutcome.IsPass() ? "narrowly misses @!" : "misses @ by a wide margin.")}",
					target, Parent)));
			if (!actor.ColocatedWith(target))
			{
				actor.Send("You missed your target.".Colour(Telnet.Red));
			}

			if (wounds.All(x => x.Lodged != Parent))
			{
				Parent.RoomLayer = target.RoomLayer;
				target.Location.Insert(Parent);
				if (actor.Combat != null)
				{
					Parent.AddEffect(new CombatNoGetEffect(Parent, actor.Combat), TimeSpan.FromSeconds(20));
				}
			}

			return;
		}

		if (!target.ColocatedWith(actor))
		{
			actor.Send("You hit your target.".Colour(Telnet.BoldGreen));
		}

		if (target is ICharacter targetChar)
		{
			wounds.AddRange(targetChar.Body.PassiveSufferDamage(damage));
			if (!wounds.Any())
			{
				target.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							$"$1 hit|hits $0 on &0's {bodypart.FullDescription()} but bounces right off without causing any damage!",
							target, target, Parent)));
				Parent.RoomLayer = target.RoomLayer;
				target.Location.Insert(Parent);
				if (actor.Combat != null)
				{
					Parent.AddEffect(new CombatNoGetEffect(Parent, actor.Combat), TimeSpan.FromSeconds(20));
				}

				return;
			}

			if (wounds.Any(x => x.Lodged == Parent))
			{
				var lodgedWound = wounds.First(x => x.Lodged == Parent);
				if (lodgedWound.Parent == targetChar)
				{
					target.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"$0 lodges in $1's {lodgedWound.Bodypart.FullDescription()}!",
								target, Parent, target)));
				}
				else
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"$0 lodges in $1's !2!", target, Parent, target,
							lodgedWound.Parent)));
				}

				wounds.ProcessPassiveWounds();
				return;
			}

			Parent.RoomLayer = target.RoomLayer;
			target.Location.Insert(Parent);
			target.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"$0 strikes $1's {bodypart.FullDescription()}, and then falls to the ground!", target,
						Parent, target)));
			wounds.ProcessPassiveWounds();
			return;
		}

		if (target is IGameItem targetItem)
		{
			wounds.AddRange(targetItem.PassiveSufferDamage(damage));
			if (!wounds.Any())
			{
				target.OutputHandler.Handle(
					new EmoteOutput(new Emote("$1 hit|hits $0 but bounces right off without causing any damage!",
						targetItem, targetItem, Parent)));
				Parent.RoomLayer = target.RoomLayer;
				target.Location.Insert(Parent);
				if (actor.Combat != null)
				{
					Parent.AddEffect(new CombatNoGetEffect(Parent, actor.Combat), TimeSpan.FromSeconds(20));
				}

				return;
			}

			if (wounds.Any(x => x.Lodged == Parent))
			{
				target.OutputHandler.Handle(new EmoteOutput(new Emote($"$0 lodges in $1!", actor, Parent, target)));
				wounds.ProcessPassiveWounds();
				return;
			}

			Parent.RoomLayer = target.RoomLayer;
			target.Location.Insert(Parent); //Put the thrown weapon on the ground
			target.OutputHandler.Handle(
				new EmoteOutput(new Emote($"$0 strikes $1, and then falls to the ground!", actor, Parent, target)));
			wounds.ProcessPassiveWounds();
			return;
		}

		throw new NotImplementedException("Unknown target type in Fire.");
	}

	public bool CanReady(ICharacter readier)
	{
		return false;
	}

	public string WhyCannotReady(ICharacter readier)
	{
		return $"{Parent.HowSeen(readier, true)} is not something that needs to be readied. Just throw the thing!";
	}

	public bool Ready(ICharacter readier)
	{
		readier.Send(WhyCannotReady(readier));
		return false;
	}

	public bool CanUnready(ICharacter readier)
	{
		return false;
	}

	public string WhyCannotUnready(ICharacter readier)
	{
		return $"{Parent.HowSeen(readier, true)} is not something that needs to be unreadied.";
	}

	public bool Unready(ICharacter readier)
	{
		readier.Send(WhyCannotUnready(readier));
		return false;
	}

	#endregion

	#region Constructors

	public ThrownWeaponGameItemComponent(ThrownWeaponGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ThrownWeaponGameItemComponent(MudSharp.Models.GameItemComponent component,
		ThrownWeaponGameItemComponentProto proto,
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
	}

	public ThrownWeaponGameItemComponent(ThrownWeaponGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion

	#region Overrides of GameItemComponent

	/// <summary>
	///     This property indicates whether this IGameItemComponent acts as a decorator for the IGameItem's description
	/// </summary>
	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Evaluate)
		{
			var mw = (IMeleeWeapon)this;
			var rw = (IRangedWeapon)this;
			return
				$"This is a throwing weapon of type {rw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {rw.WeaponType.FireTrait.Name.Colour(Telnet.Green)} skill when thrown.\nThis is also a melee weapon of type {mw.WeaponType.Name.Colour(Telnet.Cyan)}.\nIt uses the {mw.WeaponType.AttackTrait.Name.Colour(Telnet.Green)} skill for attack and {(mw.WeaponType.ParryTrait == mw.WeaponType.AttackTrait ? "defense" : $"the {mw.WeaponType.ParryTrait.Name.Colour(Telnet.Green)} skill for defense")}.\nIt is classified as {rw.WeaponType.Classification.Describe().Colour(Telnet.Green)}.";
		}

		return description;
	}

	#endregion
}