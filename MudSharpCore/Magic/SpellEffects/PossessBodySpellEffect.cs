#nullable enable

using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class PossessBodySpellEffect : IMagicSpellEffectTemplate
{
	public const string DefaultPossessionEcho = "Your focus pours into a borrowed shell.";
	public const string DefaultRoomEcho = "@ twitch|twitches once, moving with borrowed intent.";
	public const string DefaultCollapseEcho = "The possessed shell unravels and your focus returns to your primary body.";

	private int? _sortOrder;
	private string _possessionEcho = DefaultPossessionEcho;
	private string _targetEcho = string.Empty;
	private string _roomEcho = DefaultRoomEcho;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _backlashEcho = string.Empty;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("possessbody", (root, spell) => new PossessBodySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("possessbody", BuilderFactory,
			"Creates a player-focusable possessed shell from a non-player character target.",
			HelpText,
			false,
			true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new PossessBodySpellEffect(new XElement("Effect",
			new XAttribute("type", "possessbody"),
			new XElement("FormKey", "possessed-body"),
			new XElement("SortOrder", string.Empty),
			new XElement("PossessionEcho", new XCData(DefaultPossessionEcho)),
			new XElement("TargetEcho", new XCData(string.Empty)),
			new XElement("RoomEcho", new XCData(DefaultRoomEcho)),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty))
		), spell), string.Empty);
	}

	private PossessBodySpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "possessed-body";
		if (int.TryParse(root.Element("SortOrder")?.Value, out var sortOrder))
		{
			_sortOrder = sortOrder;
		}

		_possessionEcho = root.Element("PossessionEcho")?.Value ?? DefaultPossessionEcho;
		_targetEcho = root.Element("TargetEcho")?.Value ?? string.Empty;
		_roomEcho = root.Element("RoomEcho")?.Value ?? DefaultRoomEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicSpell Spell { get; }
	public string FormKey { get; private set; }

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "possessbody"),
			new XElement("FormKey", FormKey),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("PossessionEcho", new XCData(_possessionEcho)),
			new XElement("TargetEcho", new XCData(_targetEcho)),
			new XElement("RoomEcho", new XCData(_roomEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho))
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return StandaloneSpellEffectTemplateHelper.IsCharacterTarget(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter targetCharacter)
		{
			return null;
		}

		var anchor = caster.Identity.PrimaryInstance as ICharacter ?? caster;
		if (anchor.InstanceId != caster.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on your primary body to possess another body.");
			return null;
		}

		if (!anchor.IsPlayerCharacter || anchor.IsGuest)
		{
			caster.OutputHandler.Send("Only player characters can focus a possessed body shell in this implementation.");
			return null;
		}

		if (CharacterInstanceIdentityComparer.SameIdentity(anchor, targetCharacter))
		{
			caster.OutputHandler.Send("You cannot possess your own identity with this effect.");
			return null;
		}

		if (targetCharacter.IsPlayerCharacter || targetCharacter.IsGuest)
		{
			caster.OutputHandler.Send("This possession effect can only target non-player characters in this first slice.");
			return null;
		}

		if (targetCharacter.State.IsDead())
		{
			caster.OutputHandler.Send("Corpse possession still needs the next ownership and corpse-body slice.");
			return null;
		}

		if (targetCharacter.Location is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not create a possessed shell from character #{targetCharacter.Id.ToString("N0")}: the target has no location.",
				true
			);
			return null;
		}

		if (PossessionControlService.AnyPossessionEffectsForAnchor(anchor))
		{
			caster.OutputHandler.Send("You are already sustaining a possession effect.");
			return null;
		}

		var sourceFormKey = SourceFormKey(targetCharacter);
		if (!anchor.EnsureForm(FormSpecificationForTarget(targetCharacter, anchor),
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, sourceFormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure possessed body form '{sourceFormKey}' for character #{anchor.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		ApplyTargetDescriptionToForm(form, targetCharacter);
		var result = CharacterInstanceService.SpawnSecondaryInstance(
			CharacterInstanceService.CreatePossessedBodySpawnOptions(
				anchor,
				form,
				targetCharacter.Location,
				targetCharacter.RoomLayer,
				CharacterInstanceIdentityComparer.IdentityId(targetCharacter),
				targetCharacter.InstanceId,
				Spell.Id,
				sourceFormKey));
		if (!result.Success || result.Instance is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn possessed body shell for character #{anchor.Id.ToString("N0")}: {result.Message}",
				true
			);
			caster.OutputHandler.Send("The possessed shell fails to take shape.");
			return null;
		}

		return new SpellPossessedBodyEffect(
			targetCharacter,
			parent,
			sourceFormKey,
			anchor.Id,
			anchor.InstanceId,
			result.Instance.InstanceId,
			result.Instance.Body.Id,
			CharacterInstanceIdentityComparer.IdentityId(targetCharacter),
			targetCharacter.InstanceId,
			Spell.Id,
			CharacterInstancePersistencePolicy.DespawnOnReboot,
			_possessionEcho,
			_targetEcho,
			_roomEcho,
			_collapseEcho,
			_backlashEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new PossessBodySpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable form-key prefix used for target-derived possessed shells
	#3sort <number>|clear#0 - sets or clears the initial sort order for provisioned shell forms
	#3possessionecho <text>|default|none#0 - sets private text shown after focus enters the shell
	#3targetecho <text>|default|none#0 - sets private text shown to the targeted NPC
	#3roomecho <text>|default|none#0 - sets the room echo when the shell starts moving
	#3collapseecho <text>|default|none#0 - sets the focus-return echo on collapse
	#3backlashecho <text>|default|none#0 - sets optional private backlash text";

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Possess Body",
			("Form Key", FormKey.ColourCommand()),
			("Sort Order", _sortOrder?.ToString("N0", actor).ColourValue() ?? "Auto".ColourValue()),
			("Persistence", CharacterInstancePersistencePolicy.DespawnOnReboot.DescribeEnum().ColourValue()),
			("Possession Echo", DescribeEcho(_possessionEcho, DefaultPossessionEcho)),
			("Target Echo", DescribeEcho(_targetEcho, string.Empty)),
			("Room Echo", DescribeEcho(_roomEcho, DefaultRoomEcho)),
			("Collapse Echo", DescribeEcho(_collapseEcho, DefaultCollapseEcho)),
			("Backlash", DescribeEcho(_backlashEcho, string.Empty))
		);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "formkey":
				return BuildingCommandFormKey(actor, command);
			case "sort":
			case "sortorder":
				return BuildingCommandSortOrder(actor, command);
			case "possessionecho":
			case "possessecho":
			case "possess":
				return BuildingCommandEcho(actor, command, "possession", DefaultPossessionEcho,
					value => _possessionEcho = value);
			case "targetecho":
			case "target":
				return BuildingCommandEcho(actor, command, "target", string.Empty, value => _targetEcho = value);
			case "roomecho":
			case "room":
				return BuildingCommandEcho(actor, command, "room", DefaultRoomEcho, value => _roomEcho = value);
			case "collapseecho":
			case "collapse":
				return BuildingCommandEcho(actor, command, "collapse", DefaultCollapseEcho,
					value => _collapseEcho = value);
			case "backlashecho":
			case "backlash":
				return BuildingCommandEcho(actor, command, "backlash", string.Empty, value => _backlashEcho = value);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private ICharacterFormSpecification FormSpecificationForTarget(ICharacter target, ICharacter anchor)
	{
		return new CharacterFormSpecification
		{
			Race = target.Body.Race,
			Ethnicity = target.Body.Ethnicity,
			Gender = target.Body.Gender.Enum,
			Alias = TargetAlias(target, anchor),
			SortOrder = _sortOrder,
			AllowVoluntarySwitch = false
		};
	}

	private string SourceFormKey(ICharacter target)
	{
		return $"{FormKey}:{CharacterInstanceIdentityComparer.IdentityId(target)}";
	}

	private static string TargetAlias(ICharacter target, ICharacter anchor)
	{
		return target.HowSeen(anchor, colour: false,
			flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf |
			       PerceiveIgnoreFlags.IgnoreNamesSetting);
	}

	private static void ApplyTargetDescriptionToForm(ICharacterForm form, ICharacter target)
	{
		var descriptions = target.Body.GetRawDescriptions;
		form.Body.SetShortDescription(descriptions.ShortDescription.IfNullOrWhiteSpace(
			target.HowSeen(target, colour: false,
				flags: PerceiveIgnoreFlags.TrueDescription)));
		form.Body.SetFullDescription(descriptions.FullDescription.IfNullOrWhiteSpace(
			"You cannot tell anything special or unique about it."));
	}

	private bool BuildingCommandFormKey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What stable form-key prefix should this effect use?");
			return false;
		}

		FormKey = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the stable form-key prefix {FormKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial sort order should this spell use for possessed shell forms, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_sortOrder = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now append possessed shell forms after the character's existing forms.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var sortOrder))
		{
			actor.OutputHandler.Send("That is not a valid sort order.");
			return false;
		}

		_sortOrder = sortOrder;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use sort order {_sortOrder.Value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string label, string defaultEcho,
		Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {label} echo should this effect use, or should it be defaulted or suppressed?");
			return false;
		}

		var echo = BodyBackupEffect.NormaliseEcho(command.SafeRemainingArgument, defaultEcho);
		setter(echo);
		Spell.Changed = true;
		actor.OutputHandler.Send(echo switch
		{
			"" => $"This effect will now suppress {label} echoes.",
			_ when echo == defaultEcho => $"This effect will now use the default {label} echo.",
			_ => $"This effect will now use {echo.ColourCommand()} as its {label} echo."
		});
		return true;
	}

	private static string DescribeEcho(string echo, string defaultEcho)
	{
		return echo switch
		{
			"" => "Suppressed".ColourError(),
			_ when echo == defaultEcho => "Default".ColourValue(),
			_ => echo.ColourCommand()
		};
	}
}
