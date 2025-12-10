using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
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

namespace MudSharp.GameItems.Components;

public abstract class FirearmBaseGameItemComponent : GameItemComponent, IRangedWeapon, ISwitchable, IMeleeWeapon
{
	private FirearmBaseGameItemComponentProto _prototype;
	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (FirearmBaseGameItemComponentProto)newProto;
	}

	#region Constructors

	public FirearmBaseGameItemComponent(FirearmBaseGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public FirearmBaseGameItemComponent(MudSharp.Models.GameItemComponent component,
		FirearmBaseGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public FirearmBaseGameItemComponent(FirearmBaseGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(
			rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected virtual void LoadFromXml(XElement root)
	{
		ChamberedRound = Gameworld.TryGetItem(long.Parse(root.Element("ChamberedRound").Value), true)
		                          ?.GetItemType<IAmmo>();
		PrimaryWieldedLocation = Gameworld.BodypartPrototypes.Get(long.Parse(root.Element("Wielded")?.Value ?? "0")) as IWield;


		var element = root.Element("Safety");
		if (element != null)
		{
			Safety = element.Value == "true";
		}
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
		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ switch|switches the safety on $0 {(Safety ? "on" : "off")}.",
				actor, Parent)));
		return true;
	}

	public IEnumerable<string> SwitchSettings => new[] { "safe", "unsafe" };

	#endregion

	#region Implementation of IMeleeWeapon

	IWeaponType IMeleeWeapon.WeaponType => _prototype.MeleeWeaponType;

	#endregion

	#region IRangedWeapon Implementation
	public virtual string FireVerbForEchoes => "fire|fires";
	public virtual bool CanBeAimedAtSelf => true;
	public IRangedWeaponType WeaponType => _prototype.RangedWeaponType;

	public virtual bool ReadyToFire => ChamberedRound != null && !Safety;

	public int LoadStage => 0;

	/// <inheritdoc />
	public abstract bool IsLoaded { get; }

	/// <inheritdoc />
	public bool IsReadied => ChamberedRound != null;

	/// <inheritdoc />
	public abstract IEnumerable<IGameItem> MagazineContents { get; }

	/// <inheritdoc />
	public abstract IEnumerable<IGameItem> AllContainedItems { get; }

	public IAmmo ChamberedRound { get; set; }
	public string SpecificAmmoGrade => _prototype.RangedWeaponType.SpecificAmmunitionGrade;

	public Difficulty AimDifficulty => WeaponType.BaseAimDifficulty;

	public Difficulty BaseBlockDifficulty
		=> ChamberedRound?.AmmoType.DamageProfile.BaseBlockDifficulty ?? Difficulty.Automatic;

	public Difficulty BaseDodgeDifficulty
		=> ChamberedRound?.AmmoType.DamageProfile.BaseDodgeDifficulty ?? Difficulty.Automatic;

	/// <inheritdoc />
	public abstract bool CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

	/// <inheritdoc />
	public abstract string WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

	/// <inheritdoc />
	public abstract void Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal);

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

	public string WhyCannotReady(ICharacter readier)
	{
		if (WeaponType.RequiresFreeHandToReady && !readier.Body.FunctioningFreeHands.Any() &&
		    readier.Body.WieldedHandCount(Parent) < 2)
		{
			return
				$"You need at least one free {readier.Body.WielderDescriptionSingular} to ready {Parent.HowSeen(readier)}.";
		}

		throw new ApplicationException("Unknown WhyCannotReady reason in ready BoltActionGameItemComponent.");
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

	protected abstract void ChamberRound(ICharacter readier);

	protected abstract bool SemiAutomaticCycleOnFire { get; }

	/// <inheritdoc />
	public bool CanUnready(ICharacter readier)
	{
		return true;
	}

	public string WhyCannotUnready(ICharacter readier)
	{
		throw new ApplicationException($"Should always be able to unready a {GetType().FullName}.");
	}

	public virtual bool Unready(ICharacter readier)
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

	/// <inheritdoc />
	public abstract bool CanUnload(ICharacter loader);

	/// <inheritdoc />
	public abstract string WhyCannotUnload(ICharacter loader);

	/// <inheritdoc />
	public abstract IEnumerable<IGameItem> Unload(ICharacter loader);

	/// <inheritdoc />
	public abstract bool CanFire(ICharacter actor, IPerceivable target);

	/// <inheritdoc />
	public abstract string WhyCannotFire(ICharacter actor, IPerceivable target);

	/// <inheritdoc />
	public virtual void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)
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
		if (SemiAutomaticCycleOnFire)
		{
			ChamberRound(actor);
		}
		
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
		
		HandleShellCasingOnFire(actor, originalLocation, shell);

		if (ammo.AmmoType.Loudness > AudioVolume.Silent)
		{
			actor.Location.HandleAudioEcho(Gameworld.GetStaticString("GunshotHeardEcho"), ammo.AmmoType.Loudness, Parent, actor.RoomLayer);
		}
	}

	protected virtual void HandleShellCasingOnFire(ICharacter actor, ICell originalLocation, IGameItem shell)
	{
		if (shell != null)
		{
			originalLocation.Handle(new EmoteOutput(new Emote("@ tumble|tumbles to the ground.", shell), flags: OutputFlags.Insigificant));
			shell.RoomLayer = actor.RoomLayer;
			originalLocation.Insert(shell);
		}
	}

	protected IWield _primaryWieldedLocation;

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

	#endregion
}