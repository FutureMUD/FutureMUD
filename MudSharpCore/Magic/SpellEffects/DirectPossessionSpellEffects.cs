#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

internal static class DirectPossessionSecurity
{
	internal static bool HasProtectedStaffAuthority(ICharacter character)
	{
		return character.PermissionLevel >= PermissionLevel.JuniorAdmin;
	}
}

public sealed class SeizeBodySpellEffect : IMagicSpellEffectTemplate
{
	public const string DefaultPossessionEcho = "Your will seizes another body.";
	public const string DefaultVictimEcho = "Your body is seized by another will. You can only watch until the possession ends.";
	public const string DefaultVictimEndEcho = "The alien will releases your body.";
	public const string DefaultTargetEcho = "A hostile will pours into your body.";
	public const string DefaultRoomEcho = "@ shudder|shudders as another will takes command.";
	public const string DefaultCollapseEcho = "Your focus snaps back to your own body.";

	private bool _allowPcs = true;
	private bool _allowNpcs = true;
	private bool _allowAdmins;
	private string _possessionEcho = DefaultPossessionEcho;
	private string _victimEcho = DefaultVictimEcho;
	private string _victimEndEcho = DefaultVictimEndEcho;
	private string _targetEcho = DefaultTargetEcho;
	private string _roomEcho = DefaultRoomEcho;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _backlashEcho = string.Empty;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("seizebody", (root, spell) => new SeizeBodySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("seizebody", BuilderFactory,
			"Seizes hostile control of a living character body.",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new SeizeBodySpellEffect(new XElement("Effect",
			new XAttribute("type", "seizebody"),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", true),
			new XElement("AllowAdmins", false),
			new XElement("PossessionEcho", new XCData(DefaultPossessionEcho)),
			new XElement("VictimEcho", new XCData(DefaultVictimEcho)),
			new XElement("VictimEndEcho", new XCData(DefaultVictimEndEcho)),
			new XElement("TargetEcho", new XCData(DefaultTargetEcho)),
			new XElement("RoomEcho", new XCData(DefaultRoomEcho)),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty))
		), spell), string.Empty);
	}

	private SeizeBodySpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_allowPcs = bool.Parse(root.Element("AllowPCs")?.Value ?? "true");
		_allowNpcs = bool.Parse(root.Element("AllowNPCs")?.Value ?? "true");
		_allowAdmins = bool.Parse(root.Element("AllowAdmins")?.Value ?? "false");
		_possessionEcho = root.Element("PossessionEcho")?.Value ?? DefaultPossessionEcho;
		_victimEcho = root.Element("VictimEcho")?.Value ?? DefaultVictimEcho;
		_victimEndEcho = root.Element("VictimEndEcho")?.Value ?? DefaultVictimEndEcho;
		_targetEcho = root.Element("TargetEcho")?.Value ?? DefaultTargetEcho;
		_roomEcho = root.Element("RoomEcho")?.Value ?? DefaultRoomEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "seizebody"),
			new XElement("AllowPCs", _allowPcs),
			new XElement("AllowNPCs", _allowNpcs),
			new XElement("AllowAdmins", _allowAdmins),
			new XElement("PossessionEcho", new XCData(_possessionEcho)),
			new XElement("VictimEcho", new XCData(_victimEcho)),
			new XElement("VictimEndEcho", new XCData(_victimEndEcho)),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("RoomEcho", new XCData(_roomEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho))
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return StandaloneSpellEffectTemplateHelper.IsCharacterTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter targetCharacter || !TryGetAnchor(caster, out var anchor))
		{
			return null;
		}

		if (CharacterInstanceIdentityComparer.SameIdentity(anchor, targetCharacter))
		{
			caster.OutputHandler.Send("You cannot seize control of your own identity.");
			return null;
		}

		if (!targetCharacter.IsEmbodied || targetCharacter.Location is null)
		{
			caster.OutputHandler.Send("That target does not have an embodied presence to seize.");
			return null;
		}

		if (targetCharacter.State.IsDead())
		{
			caster.OutputHandler.Send("That body is dead. Use a corpse possession effect instead.");
			return null;
		}

		if (targetCharacter.IsPlayerCharacter && (!_allowPcs || targetCharacter.IsGuest))
		{
			caster.OutputHandler.Send("This spell is not configured to seize player-character bodies.");
			return null;
		}

		if (!targetCharacter.IsPlayerCharacter && !_allowNpcs)
		{
			caster.OutputHandler.Send("This spell is not configured to seize non-player bodies.");
			return null;
		}

		if (!_allowAdmins && HasProtectedStaffAuthority(targetCharacter))
		{
			caster.OutputHandler.Send("This spell is not configured to seize administrator avatars.");
			return null;
		}

		if (PossessionControlService.AnyPossessionEffectsForAnchor(anchor))
		{
			caster.OutputHandler.Send("You are already sustaining a possession effect.");
			return null;
		}

		if (targetCharacter.AffectedBy<ILiveBodyPossessionEffect>() ||
		    targetCharacter.AffectedBy<IPossessedBodyEffect>() ||
		    targetCharacter.AffectedBy<IDispelMagicProxyEffect>())
		{
			caster.OutputHandler.Send("That body is already under possession magic.");
			return null;
		}

		SendIfNotSuppressed(targetCharacter, _targetEcho);
		var result = PossessionControlService.BeginLivePossession(anchor, targetCharacter, _victimEcho, _victimEndEcho);
		if (!result.Success || result.RuntimeState is null)
		{
			caster.OutputHandler.Send(result.Message);
			return null;
		}

		return new SpellLiveBodyPossessionEffect(
			targetCharacter,
			parent,
			CharacterInstanceIdentityComparer.IdentityId(anchor),
			anchor.InstanceId,
			CharacterInstanceIdentityComparer.IdentityId(targetCharacter),
			targetCharacter.InstanceId,
			targetCharacter.Body.Id,
			Spell.Id,
			CharacterInstancePersistencePolicy.TemporaryEffectBound,
			result.RuntimeState,
			_possessionEcho,
			_roomEcho,
			_collapseEcho,
			_backlashEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new SeizeBodySpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3possessionecho <text>|default|none#0 - sets private text shown to the possessor in the seized body
	#3victimecho <text>|default|none#0 - sets private text shown to a PC victim when bound as a spectator
	#3victimend <text>|default|none#0 - sets private text shown to a PC victim when possession ends
	#3targetecho <text>|default|none#0 - sets private text shown to the target body before control is seized
	#3roomecho <text>|default|none#0 - sets the room echo when possession begins
	#3collapseecho <text>|default|none#0 - sets the focus-return echo on cleanup
	#3backlashecho <text>|default|none#0 - sets optional private backlash text
	#3allowpcs#0 - toggles whether player-character targets are allowed
	#3allownpcs#0 - toggles whether non-player targets are allowed
	#3allowadmins#0 - toggles whether administrator avatars are allowed";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "possessionecho":
			case "possessecho":
			case "possess":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "possession",
					DefaultPossessionEcho, value => _possessionEcho = value);
			case "victimecho":
			case "victim":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "victim",
					DefaultVictimEcho, value => _victimEcho = value);
			case "victimend":
			case "victimendecho":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "victim end",
					DefaultVictimEndEcho, value => _victimEndEcho = value);
			case "targetecho":
			case "target":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "target",
					DefaultTargetEcho, value => _targetEcho = value);
			case "roomecho":
			case "room":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "room",
					DefaultRoomEcho, value => _roomEcho = value);
			case "collapseecho":
			case "collapse":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "collapse",
					DefaultCollapseEcho, value => _collapseEcho = value);
			case "backlashecho":
			case "backlash":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "backlash",
					string.Empty, value => _backlashEcho = value);
			case "allowpcs":
			case "pcs":
				_allowPcs = !_allowPcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowPcs.NowNoLonger()} allow player-character targets.");
				return true;
			case "allownpcs":
			case "npcs":
				_allowNpcs = !_allowNpcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowNpcs.NowNoLonger()} allow non-player targets.");
				return true;
			case "allowadmins":
			case "admins":
				_allowAdmins = !_allowAdmins;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowAdmins.NowNoLonger()} allow administrator targets.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Seize Body",
			("Persistence", CharacterInstancePersistencePolicy.TemporaryEffectBound.DescribeEnum().ColourValue()),
			("Allow PCs", _allowPcs.ToColouredString()),
			("Allow NPCs", _allowNpcs.ToColouredString()),
			("Allow Admins", _allowAdmins.ToColouredString()),
			("Possession Echo", DirectPossessionBuilderHelpers.DescribeEcho(_possessionEcho, DefaultPossessionEcho)),
			("Victim Echo", DirectPossessionBuilderHelpers.DescribeEcho(_victimEcho, DefaultVictimEcho)),
			("Victim End", DirectPossessionBuilderHelpers.DescribeEcho(_victimEndEcho, DefaultVictimEndEcho)),
			("Target Echo", DirectPossessionBuilderHelpers.DescribeEcho(_targetEcho, DefaultTargetEcho)),
			("Room Echo", DirectPossessionBuilderHelpers.DescribeEcho(_roomEcho, DefaultRoomEcho)),
			("Collapse Echo", DirectPossessionBuilderHelpers.DescribeEcho(_collapseEcho, DefaultCollapseEcho)),
			("Backlash", DirectPossessionBuilderHelpers.DescribeEcho(_backlashEcho, string.Empty))
		);
	}

	internal static bool HasProtectedStaffAuthority(ICharacter character)
	{
		return DirectPossessionSecurity.HasProtectedStaffAuthority(character);
	}

	private bool TryGetAnchor(ICharacter caster, out ICharacter anchor)
	{
		anchor = caster.Identity.PrimaryInstance as ICharacter ?? caster;
		if (anchor.InstanceId != caster.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on your primary body to seize another body.");
			return false;
		}

		if (!anchor.IsPlayerCharacter || anchor.IsGuest)
		{
			caster.OutputHandler.Send("Only non-guest player characters can sustain direct body possession.");
			return false;
		}

		return true;
	}

	private static void SendIfNotSuppressed(ICharacter character, string echo)
	{
		if (!string.IsNullOrWhiteSpace(echo))
		{
			character.OutputHandler?.Send(echo.SubstituteANSIColour());
		}
	}
}

public sealed class PossessCorpseSpellEffect : IMagicSpellEffectTemplate
{
	public const string DefaultPossessionEcho = "Your will hauls the dead body into motion.";
	public const string DefaultTargetEcho = "";
	public const string DefaultRoomEcho = "@ jerk|jerks upright with a borrowed will.";
	public const string DefaultCollapseEcho = "The corpse falls still and your focus returns to your body.";
	public const string DefaultRestoreEcho = "@ collapse|collapses back into $1.";

	private bool _allowPcs = true;
	private bool _allowNpcs = true;
	private bool _allowAdmins;
	private bool _allowFinal = true;
	private bool _allowSkeletal;
	private string _possessionEcho = DefaultPossessionEcho;
	private string _targetEcho = DefaultTargetEcho;
	private string _roomEcho = DefaultRoomEcho;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _backlashEcho = string.Empty;
	private string _restoreEcho = DefaultRestoreEcho;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("possesscorpse",
			(root, spell) => new PossessCorpseSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("possesscorpse", BuilderFactory,
			"Animates and controls a corpse's original body while hiding the corpse item.",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new PossessCorpseSpellEffect(new XElement("Effect",
			new XAttribute("type", "possesscorpse"),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", true),
			new XElement("AllowAdmins", false),
			new XElement("AllowFinal", true),
			new XElement("AllowSkeletal", false),
			new XElement("PossessionEcho", new XCData(DefaultPossessionEcho)),
			new XElement("TargetEcho", new XCData(DefaultTargetEcho)),
			new XElement("RoomEcho", new XCData(DefaultRoomEcho)),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty)),
			new XElement("RestoreEcho", new XCData(DefaultRestoreEcho))
		), spell), string.Empty);
	}

	private PossessCorpseSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_allowPcs = bool.Parse(root.Element("AllowPCs")?.Value ?? "true");
		_allowNpcs = bool.Parse(root.Element("AllowNPCs")?.Value ?? "true");
		_allowAdmins = bool.Parse(root.Element("AllowAdmins")?.Value ?? "false");
		_allowFinal = bool.Parse(root.Element("AllowFinal")?.Value ?? "true");
		_allowSkeletal = bool.Parse(root.Element("AllowSkeletal")?.Value ?? "false");
		_possessionEcho = root.Element("PossessionEcho")?.Value ?? DefaultPossessionEcho;
		_targetEcho = root.Element("TargetEcho")?.Value ?? DefaultTargetEcho;
		_roomEcho = root.Element("RoomEcho")?.Value ?? DefaultRoomEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
		_restoreEcho = root.Element("RestoreEcho")?.Value ?? DefaultRestoreEcho;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "possesscorpse"),
			new XElement("AllowPCs", _allowPcs),
			new XElement("AllowNPCs", _allowNpcs),
			new XElement("AllowAdmins", _allowAdmins),
			new XElement("AllowFinal", _allowFinal),
			new XElement("AllowSkeletal", _allowSkeletal),
			new XElement("PossessionEcho", new XCData(_possessionEcho)),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("RoomEcho", new XCData(_roomEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho)),
			new XElement("RestoreEcho", new XCData(_restoreEcho))
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return trigger.TargetTypes == "item";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem corpseItem || corpseItem.GetItemType<ICorpse>() is not { } corpse ||
		    !TryGetAnchor(caster, out var anchor))
		{
			return null;
		}

		if (corpse.OriginalBody is null)
		{
			caster.OutputHandler.Send("That corpse has no original body to animate.");
			return null;
		}

		if (corpseItem.ContainedIn is not null || corpseItem.InInventoryOf is not null ||
		    corpseItem.TrueLocations.FirstOrDefault() is not { } location)
		{
			caster.OutputHandler.Send("That corpse must be visibly present in a room to possess.");
			return null;
		}

		if (corpseItem.AffectedBy<ICorpsePossessionEffect>())
		{
			caster.OutputHandler.Send("That corpse is already animated by possession magic.");
			return null;
		}

		if (!_allowFinal && corpse.RepresentsFinalCharacterDeath)
		{
			caster.OutputHandler.Send("This spell is not configured to animate final-death corpses.");
			return null;
		}

		if (!_allowSkeletal && corpse.Decay == DecayState.Skeletal)
		{
			caster.OutputHandler.Send("This spell is not configured to animate skeletal remains.");
			return null;
		}

		if (corpse.OriginalCharacter.IsPlayerCharacter && (!_allowPcs || corpse.OriginalCharacter.IsGuest))
		{
			caster.OutputHandler.Send("This spell is not configured to animate player-character corpses.");
			return null;
		}

		if (!corpse.OriginalCharacter.IsPlayerCharacter && !_allowNpcs)
		{
			caster.OutputHandler.Send("This spell is not configured to animate non-player corpses.");
			return null;
		}

		if (!_allowAdmins && DirectPossessionSecurity.HasProtectedStaffAuthority(corpse.OriginalCharacter))
		{
			caster.OutputHandler.Send("This spell is not configured to animate administrator avatars.");
			return null;
		}

		if (PossessionControlService.AnyPossessionEffectsForAnchor(anchor))
		{
			caster.OutputHandler.Send("You are already sustaining a possession effect.");
			return null;
		}

		if (!string.IsNullOrWhiteSpace(_targetEcho))
		{
			corpseItem.Handle(_targetEcho.SubstituteANSIColour());
		}

		var originalLayer = corpseItem.RoomLayer;
		var spawn = CharacterInstanceService.SpawnPossessedCorpseInstance(
			corpse.OriginalCharacter,
			corpse.OriginalBody,
			location,
			originalLayer,
			CharacterInstanceIdentityComparer.IdentityId(anchor),
			anchor.InstanceId,
			corpseItem.Id,
			Spell.Id);
		if (!spawn.Success || spawn.Instance is not ICharacter animated)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn possessed corpse actor for corpse #{corpseItem.Id.ToString("N0")}: {spawn.Message}",
				true);
			caster.OutputHandler.Send("The corpse fails to rise.");
			return null;
		}

		location.Extract(corpseItem);
		var control = PossessionControlService.BeginLivePossession(anchor, animated, string.Empty, string.Empty);
		if (!control.Success || control.RuntimeState is null)
		{
			location.Insert(corpseItem, true);
			if (spawn.Instance is ICharacterInstance failedInstance)
			{
				CharacterInstanceService.Retire(failedInstance, out _, true, false, false);
			}

			caster.OutputHandler.Send(control.Message);
			return null;
		}

		animated.AddEffect(new CorpsePossessionDispelProxyEffect(animated, corpseItem.Id));
		return new SpellCorpsePossessionEffect(
			corpseItem,
			parent,
			CharacterInstanceIdentityComparer.IdentityId(anchor),
			anchor.InstanceId,
			corpseItem.Id,
			CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter),
			corpse.OriginalBody.Id,
			animated.InstanceId,
			location.Id,
			(int)originalLayer,
			Spell.Id,
			CharacterInstancePersistencePolicy.TemporaryEffectBound,
			control.RuntimeState,
			_possessionEcho,
			_roomEcho,
			_collapseEcho,
			_backlashEcho,
			_restoreEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new PossessCorpseSpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3possessionecho <text>|default|none#0 - sets private text shown to the possessor in the animated corpse
	#3targetecho <text>|default|none#0 - sets optional text emitted through the corpse item before animation
	#3roomecho <text>|default|none#0 - sets the room echo when the corpse rises
	#3collapseecho <text>|default|none#0 - sets the focus-return echo on cleanup
	#3backlashecho <text>|default|none#0 - sets optional private backlash text
	#3restoreecho <text>|default|none#0 - sets the room echo when the corpse item is restored
	#3allowpcs#0 - toggles whether player-character corpses are allowed
	#3allownpcs#0 - toggles whether non-player corpses are allowed
	#3allowadmins#0 - toggles whether administrator avatars are allowed
	#3allowfinal#0 - toggles whether final-death corpses are allowed
	#3allowskeletal#0 - toggles whether skeletal corpses are allowed";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "possessionecho":
			case "possessecho":
			case "possess":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "possession",
					DefaultPossessionEcho, value => _possessionEcho = value);
			case "targetecho":
			case "target":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "target",
					DefaultTargetEcho, value => _targetEcho = value);
			case "roomecho":
			case "room":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "room",
					DefaultRoomEcho, value => _roomEcho = value);
			case "collapseecho":
			case "collapse":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "collapse",
					DefaultCollapseEcho, value => _collapseEcho = value);
			case "backlashecho":
			case "backlash":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "backlash",
					string.Empty, value => _backlashEcho = value);
			case "restoreecho":
			case "restore":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "restore",
					DefaultRestoreEcho, value => _restoreEcho = value);
			case "allowpcs":
			case "pcs":
				_allowPcs = !_allowPcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowPcs.NowNoLonger()} allow player-character corpses.");
				return true;
			case "allownpcs":
			case "npcs":
				_allowNpcs = !_allowNpcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowNpcs.NowNoLonger()} allow non-player corpses.");
				return true;
			case "allowadmins":
			case "admins":
				_allowAdmins = !_allowAdmins;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowAdmins.NowNoLonger()} allow administrator corpses.");
				return true;
			case "allowfinal":
			case "final":
				_allowFinal = !_allowFinal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowFinal.NowNoLonger()} allow final-death corpses.");
				return true;
			case "allowskeletal":
			case "skeletal":
				_allowSkeletal = !_allowSkeletal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowSkeletal.NowNoLonger()} allow skeletal corpses.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Possess Corpse",
			("Persistence", CharacterInstancePersistencePolicy.TemporaryEffectBound.DescribeEnum().ColourValue()),
			("Allow PCs", _allowPcs.ToColouredString()),
			("Allow NPCs", _allowNpcs.ToColouredString()),
			("Allow Admins", _allowAdmins.ToColouredString()),
			("Allow Final", _allowFinal.ToColouredString()),
			("Allow Skeletal", _allowSkeletal.ToColouredString()),
			("Possession Echo", DirectPossessionBuilderHelpers.DescribeEcho(_possessionEcho, DefaultPossessionEcho)),
			("Target Echo", DirectPossessionBuilderHelpers.DescribeEcho(_targetEcho, DefaultTargetEcho)),
			("Room Echo", DirectPossessionBuilderHelpers.DescribeEcho(_roomEcho, DefaultRoomEcho)),
			("Collapse Echo", DirectPossessionBuilderHelpers.DescribeEcho(_collapseEcho, DefaultCollapseEcho)),
			("Backlash", DirectPossessionBuilderHelpers.DescribeEcho(_backlashEcho, string.Empty)),
			("Restore Echo", DirectPossessionBuilderHelpers.DescribeEcho(_restoreEcho, DefaultRestoreEcho))
		);
	}

	private bool TryGetAnchor(ICharacter caster, out ICharacter anchor)
	{
		anchor = caster.Identity.PrimaryInstance as ICharacter ?? caster;
		if (anchor.InstanceId != caster.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on your primary body to possess a corpse.");
			return false;
		}

		if (!anchor.IsPlayerCharacter || anchor.IsGuest)
		{
			caster.OutputHandler.Send("Only non-guest player characters can sustain direct corpse possession.");
			return false;
		}

		return true;
	}
}

public sealed class AnimateCorpseSpellEffect : IMagicSpellEffectTemplate
{
	public const string DefaultTargetEcho = "";
	public const string DefaultRoomEcho = "@ jerk|jerks upright under necromantic command.";
	public const string DefaultCollapseEcho = "@ collapse|collapses as the animating magic fails.";
	public const string DefaultRestoreEcho = "@ settle|settles back into $1.";

	private readonly List<long> _aiIds = new();
	private bool _allowPcs = true;
	private bool _allowNpcs = true;
	private bool _allowAdmins;
	private bool _allowFinal = true;
	private bool _allowSkeletal;
	private string _targetEcho = DefaultTargetEcho;
	private string _roomEcho = DefaultRoomEcho;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _restoreEcho = DefaultRestoreEcho;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("animatecorpse",
			(root, spell) => new AnimateCorpseSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("animatecorpse", BuilderFactory,
			"Animates a corpse's original body as a temporary AI-controlled NPC.",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes == "item")
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new AnimateCorpseSpellEffect(new XElement("Effect",
			new XAttribute("type", "animatecorpse"),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", true),
			new XElement("AllowAdmins", false),
			new XElement("AllowFinal", true),
			new XElement("AllowSkeletal", false),
			new XElement("ArtificialIntelligences"),
			new XElement("TargetEcho", new XCData(DefaultTargetEcho)),
			new XElement("RoomEcho", new XCData(DefaultRoomEcho)),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("RestoreEcho", new XCData(DefaultRestoreEcho))
		), spell), string.Empty);
	}

	private AnimateCorpseSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		_allowPcs = bool.Parse(root.Element("AllowPCs")?.Value ?? "true");
		_allowNpcs = bool.Parse(root.Element("AllowNPCs")?.Value ?? "true");
		_allowAdmins = bool.Parse(root.Element("AllowAdmins")?.Value ?? "false");
		_allowFinal = bool.Parse(root.Element("AllowFinal")?.Value ?? "true");
		_allowSkeletal = bool.Parse(root.Element("AllowSkeletal")?.Value ?? "false");
		foreach (var element in root.Element("ArtificialIntelligences")?.Elements("AI") ?? Enumerable.Empty<XElement>())
		{
			if (long.TryParse(element.Attribute("id")?.Value ?? element.Value, out var value) &&
			    value > 0 && !_aiIds.Contains(value))
			{
				_aiIds.Add(value);
			}
		}

		_targetEcho = root.Element("TargetEcho")?.Value ?? DefaultTargetEcho;
		_roomEcho = root.Element("RoomEcho")?.Value ?? DefaultRoomEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_restoreEcho = root.Element("RestoreEcho")?.Value ?? DefaultRestoreEcho;
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "animatecorpse"),
			new XElement("AllowPCs", _allowPcs),
			new XElement("AllowNPCs", _allowNpcs),
			new XElement("AllowAdmins", _allowAdmins),
			new XElement("AllowFinal", _allowFinal),
			new XElement("AllowSkeletal", _allowSkeletal),
			new XElement("ArtificialIntelligences",
				_aiIds.Distinct().Select(x => new XElement("AI", new XAttribute("id", x)))),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("RoomEcho", new XCData(_roomEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("RestoreEcho", new XCData(_restoreEcho))
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return trigger.TargetTypes == "item";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem corpseItem || corpseItem.GetItemType<ICorpse>() is not { } corpse)
		{
			return null;
		}

		if (corpse.OriginalBody is null)
		{
			caster.OutputHandler.Send("That corpse has no original body to animate.");
			return null;
		}

		if (corpseItem.ContainedIn is not null || corpseItem.InInventoryOf is not null ||
		    corpseItem.TrueLocations.FirstOrDefault() is not { } location)
		{
			caster.OutputHandler.Send("That corpse must be visibly present in a room to animate.");
			return null;
		}

		if (corpseItem.AffectedBy<ICorpsePossessionEffect>() || corpseItem.AffectedBy<IAnimatedCorpseEffect>())
		{
			caster.OutputHandler.Send("That corpse is already animated by magic.");
			return null;
		}

		if (!_allowFinal && corpse.RepresentsFinalCharacterDeath)
		{
			caster.OutputHandler.Send("This spell is not configured to animate final-death corpses.");
			return null;
		}

		if (!_allowSkeletal && corpse.Decay == DecayState.Skeletal)
		{
			caster.OutputHandler.Send("This spell is not configured to animate skeletal remains.");
			return null;
		}

		if (corpse.OriginalCharacter.IsPlayerCharacter && (!_allowPcs || corpse.OriginalCharacter.IsGuest))
		{
			caster.OutputHandler.Send("This spell is not configured to animate player-character corpses.");
			return null;
		}

		if (!corpse.OriginalCharacter.IsPlayerCharacter && !_allowNpcs)
		{
			caster.OutputHandler.Send("This spell is not configured to animate non-player corpses.");
			return null;
		}

		if (!_allowAdmins && DirectPossessionSecurity.HasProtectedStaffAuthority(corpse.OriginalCharacter))
		{
			caster.OutputHandler.Send("This spell is not configured to animate administrator avatars.");
			return null;
		}

		if (!TryResolveArtificialIntelligences(caster, out var ais))
		{
			return null;
		}

		if (!string.IsNullOrWhiteSpace(_targetEcho))
		{
			corpseItem.Handle(_targetEcho.SubstituteANSIColour());
		}

		var originalLayer = corpseItem.RoomLayer;
		var spawn = CharacterInstanceService.SpawnAnimatedCorpseInstance(
			corpse.OriginalCharacter,
			corpse.OriginalBody,
			location,
			originalLayer,
			CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId,
			corpseItem.Id,
			Spell.Id,
			ais);
		if (!spawn.Success || spawn.Instance is not ICharacter animated)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn animated corpse actor for corpse #{corpseItem.Id.ToString("N0")}: {spawn.Message}",
				true);
			caster.OutputHandler.Send("The corpse fails to rise.");
			return null;
		}

		location.Extract(corpseItem);
		animated.AddEffect(new CorpseAnimationDispelProxyEffect(animated, corpseItem.Id));
		return new SpellAnimatedCorpseEffect(
			corpseItem,
			parent,
			CharacterInstanceIdentityComparer.IdentityId(caster),
			caster.InstanceId,
			corpseItem.Id,
			CharacterInstanceIdentityComparer.IdentityId(corpse.OriginalCharacter),
			corpse.OriginalBody.Id,
			animated.InstanceId,
			location.Id,
			(int)originalLayer,
			Spell.Id,
			ais.Select(x => x.Id),
			CharacterInstancePersistencePolicy.TemporaryEffectBound,
			_roomEcho,
			_collapseEcho,
			_restoreEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new AnimateCorpseSpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3ai add <which>#0 - adds an AI to attach to the animated corpse
	#3ai remove <which>#0 - removes an AI from this effect
	#3ai clear#0 - clears all configured AIs
	#3targetecho <text>|default|none#0 - sets optional text emitted through the corpse item before animation
	#3roomecho <text>|default|none#0 - sets the room echo when the corpse rises
	#3collapseecho <text>|default|none#0 - sets the room echo when the animation collapses
	#3restoreecho <text>|default|none#0 - sets the room echo when the corpse item is restored
	#3allowpcs#0 - toggles whether player-character corpses are allowed
	#3allownpcs#0 - toggles whether non-player corpses are allowed
	#3allowadmins#0 - toggles whether administrator avatars are allowed
	#3allowfinal#0 - toggles whether final-death corpses are allowed
	#3allowskeletal#0 - toggles whether skeletal corpses are allowed";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ai":
			case "ais":
				return BuildingCommandAI(actor, command);
			case "targetecho":
			case "target":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "target",
					DefaultTargetEcho, value => _targetEcho = value);
			case "roomecho":
			case "room":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "room",
					DefaultRoomEcho, value => _roomEcho = value);
			case "collapseecho":
			case "collapse":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "collapse",
					DefaultCollapseEcho, value => _collapseEcho = value);
			case "restoreecho":
			case "restore":
				return DirectPossessionBuilderHelpers.BuildingCommandEcho(actor, Spell, command, "restore",
					DefaultRestoreEcho, value => _restoreEcho = value);
			case "allowpcs":
			case "pcs":
				_allowPcs = !_allowPcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowPcs.NowNoLonger()} allow player-character corpses.");
				return true;
			case "allownpcs":
			case "npcs":
				_allowNpcs = !_allowNpcs;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowNpcs.NowNoLonger()} allow non-player corpses.");
				return true;
			case "allowadmins":
			case "admins":
				_allowAdmins = !_allowAdmins;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowAdmins.NowNoLonger()} allow administrator corpses.");
				return true;
			case "allowfinal":
			case "final":
				_allowFinal = !_allowFinal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowFinal.NowNoLonger()} allow final-death corpses.");
				return true;
			case "allowskeletal":
			case "skeletal":
				_allowSkeletal = !_allowSkeletal;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This effect will {_allowSkeletal.NowNoLonger()} allow skeletal corpses.");
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Animate Corpse",
			("Persistence", CharacterInstancePersistencePolicy.TemporaryEffectBound.DescribeEnum().ColourValue()),
			("AIs", DescribeArtificialIntelligences(actor)),
			("Allow PCs", _allowPcs.ToColouredString()),
			("Allow NPCs", _allowNpcs.ToColouredString()),
			("Allow Admins", _allowAdmins.ToColouredString()),
			("Allow Final", _allowFinal.ToColouredString()),
			("Allow Skeletal", _allowSkeletal.ToColouredString()),
			("Target Echo", DirectPossessionBuilderHelpers.DescribeEcho(_targetEcho, DefaultTargetEcho)),
			("Room Echo", DirectPossessionBuilderHelpers.DescribeEcho(_roomEcho, DefaultRoomEcho)),
			("Collapse Echo", DirectPossessionBuilderHelpers.DescribeEcho(_collapseEcho, DefaultCollapseEcho)),
			("Restore Echo", DirectPossessionBuilderHelpers.DescribeEcho(_restoreEcho, DefaultRestoreEcho))
		);
	}

	private bool BuildingCommandAI(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "":
			case "list":
				actor.OutputHandler.Send($"This effect will attach these AIs: {DescribeArtificialIntelligences(actor)}.");
				return true;
			case "clear":
			case "none":
				_aiIds.Clear();
				Spell.Changed = true;
				actor.OutputHandler.Send("This effect will no longer attach any AIs.");
				return true;
			case "remove":
			case "delete":
			case "del":
				return BuildingCommandRemoveAI(actor, command);
			case "add":
				return BuildingCommandAddAI(actor, command);
			default:
				return BuildingCommandAddAI(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandAddAI(ICharacter actor, StringStack command)
	{
		if (!TryGetArtificialIntelligence(actor, command, out var ai))
		{
			return false;
		}

		if (!ai.IsReadyToBeUsed)
		{
			actor.OutputHandler.Send("That AI is not ready to be used.");
			return false;
		}

		if (_aiIds.Contains(ai.Id))
		{
			actor.OutputHandler.Send("This effect already attaches that AI.");
			return false;
		}

		_aiIds.Add(ai.Id);
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now attach the {ai.Name.ColourName()} AI.");
		return true;
	}

	private bool BuildingCommandRemoveAI(ICharacter actor, StringStack command)
	{
		if (!TryGetArtificialIntelligence(actor, command, out var ai))
		{
			return false;
		}

		if (!_aiIds.Remove(ai.Id))
		{
			actor.OutputHandler.Send("This effect is not currently attaching that AI.");
			return false;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will no longer attach the {ai.Name.ColourName()} AI.");
		return true;
	}

	private bool TryGetArtificialIntelligence(ICharacter actor, StringStack command, out IArtificialIntelligence ai)
	{
		ai = null!;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which AI do you want to use?");
			return false;
		}

		var resolved = Gameworld.AIs.GetByIdOrName(command.SafeRemainingArgument);
		if (resolved is null)
		{
			actor.OutputHandler.Send("There is no such AI.");
			return false;
		}

		ai = resolved;
		return true;
	}

	private bool TryResolveArtificialIntelligences(ICharacter caster, out List<IArtificialIntelligence> ais)
	{
		ais = _aiIds.Select(x => Gameworld.AIs.Get(x))
		            .OfType<IArtificialIntelligence>()
		            .Distinct()
		            .ToList();
		if (ais.Count == 0)
		{
			caster.OutputHandler.Send("This spell effect has no AIs configured for the animated corpse.");
			return false;
		}

		if (ais.Count != _aiIds.Distinct().Count())
		{
			caster.OutputHandler.Send("This spell effect references an AI that no longer exists.");
			return false;
		}

		if (ais.FirstOrDefault(x => !x.IsReadyToBeUsed) is { } notReady)
		{
			caster.OutputHandler.Send($"The {notReady.Name.ColourName()} AI is not ready to be used.");
			return false;
		}

		return true;
	}

	private string DescribeArtificialIntelligences(ICharacter actor)
	{
		if (_aiIds.Count == 0)
		{
			return "none".ColourError();
		}

		return _aiIds.Select(x => Gameworld.AIs.Get(x) is { } ai
			             ? $"{ai.Name.ColourName()} (#{ai.Id.ToString("N0", actor).ColourValue()})"
			             : $"missing #{x.ToString("N0", actor).ColourError()}")
		             .ListToString();
	}
}

internal static class DirectPossessionBuilderHelpers
{
	public static bool BuildingCommandEcho(ICharacter actor, IMagicSpell spell, StringStack command, string label,
		string defaultEcho, Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {label} echo should this effect use, or should it be defaulted or suppressed?");
			return false;
		}

		var echo = BodyBackupEffect.NormaliseEcho(command.SafeRemainingArgument, defaultEcho);
		setter(echo);
		spell.Changed = true;
		actor.OutputHandler.Send(echo switch
		{
			"" => $"This effect will now suppress {label} echoes.",
			_ when echo == defaultEcho => $"This effect will now use the default {label} echo.",
			_ => $"This effect will now use {echo.ColourCommand()} as its {label} echo."
		});
		return true;
	}

	public static string DescribeEcho(string echo, string defaultEcho)
	{
		return echo switch
		{
			"" => "Suppressed".ColourError(),
			_ when echo == defaultEcho => "Default".ColourValue(),
			_ => echo.ColourCommand()
		};
	}
}
