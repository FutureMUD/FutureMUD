#nullable enable

using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Revision;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class CopySpellEffect : IMagicSpellEffectTemplate
{
	public const string MagicalCopyTransitionProfile = "magicalcopy";
	public const string DefaultCollapseEcho = "Your magical copy collapses and your focus returns to your primary body.";

	private IRace _race = null!;
	private IEthnicity? _ethnicity;
	private Gender? _gender;
	private string? _alias;
	private int? _sortOrder;
	private bool _playerFocusable;
	private bool _intangible = true;
	private long _planeId;
	private CharacterInstancePersistencePolicy _persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _backlashEcho = string.Empty;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createcopy", (root, spell) => new CopySpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createcopy", BuilderFactory,
			"Creates a temporary magical copy or mirror-image body instance.",
			HelpText,
			false,
			false,
			SpellTriggerFactory.MagicTriggerTypes
			                  .Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                  .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		var defaultRace = spell.Gameworld.Races.First();
		var plane = DefaultCopyPlane(spell.Gameworld);
		return (new CopySpellEffect(new XElement("Effect",
			new XAttribute("type", "createcopy"),
			new XElement("FormKey", "copy"),
			new XElement("Race", defaultRace.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", $"{defaultRace.Name} magical copy"),
			new XElement("SortOrder", string.Empty),
			new XElement("PlayerFocusable", false),
			new XElement("Intangible", true),
			new XElement("Plane", plane?.Id ?? 0L),
			new XElement("PersistencePolicy", CharacterInstancePersistencePolicy.DespawnOnReboot),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty))
		), spell), string.Empty);
	}

	private CopySpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "copy";
		_race = Gameworld.Races.Get(long.Parse(root.Element("Race")?.Value ?? "0")) ?? Gameworld.Races.First();
		_ethnicity = Gameworld.Ethnicities.Get(long.Parse(root.Element("Ethnicity")?.Value ?? "0"));
		if (int.TryParse(root.Element("Gender")?.Value, out var genderValue) && genderValue >= 0)
		{
			_gender = (Gender)genderValue;
		}

		_alias = root.Element("Alias")?.Value;
		if (int.TryParse(root.Element("SortOrder")?.Value, out var sortOrder))
		{
			_sortOrder = sortOrder;
		}

		_playerFocusable = bool.Parse(root.Element("PlayerFocusable")?.Value ?? "false");
		_intangible = bool.Parse(root.Element("Intangible")?.Value ?? "true");
		_planeId = long.Parse(root.Element("Plane")?.Value ?? "0");
		if (_planeId == 0)
		{
			_planeId = DefaultCopyPlane(Gameworld)?.Id ?? 0;
		}

		_persistencePolicy = (root.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.DespawnOnReboot;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicSpell Spell { get; }
	public string FormKey { get; private set; }

	private ICharacterFormSpecification FormSpecification => new CharacterFormSpecification
	{
		Race = _race,
		Ethnicity = _ethnicity,
		Gender = _gender,
		Alias = _alias,
		SortOrder = _sortOrder
	};

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createcopy"),
			new XElement("FormKey", FormKey),
			new XElement("Race", _race.Id),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("PlayerFocusable", _playerFocusable),
			new XElement("Intangible", _intangible),
			new XElement("Plane", _planeId),
			new XElement("PersistencePolicy", _persistencePolicy),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho))
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => false;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return true;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var recipient = target as ICharacter ?? caster;
		var anchor = recipient.Identity.PrimaryInstance as ICharacter ?? recipient;
		if (anchor.InstanceId != recipient.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on a primary body to create a magical copy.");
			return null;
		}

		if (anchor.Location is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not create a magical copy for character #{anchor.Id.ToString("N0")}: the character has no location.",
				true
			);
			return null;
		}

		if (!anchor.EnsureForm(FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, FormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure magical copy form '{FormKey}' for character #{anchor.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		var result = CharacterInstanceService.SpawnSecondaryInstance(
			CharacterInstanceService.CreateMagicalCopySpawnOptions(
				anchor,
				form,
				anchor.Location,
				anchor.RoomLayer,
				EffectivePlaneId(),
				Spell.Id,
				FormKey,
				_playerFocusable,
				_intangible,
				_persistencePolicy));
		if (!result.Success || result.Instance is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn magical copy for character #{anchor.Id.ToString("N0")}: {result.Message}",
				true
			);
			caster.OutputHandler.Send("The magical copy fails to take shape.");
			return null;
		}

		return new SpellMagicalCopyEffect(
			anchor,
			parent,
			FormKey,
			result.Instance.InstanceId,
			result.Instance.Body.Id,
			anchor.InstanceId,
			Spell.Id,
			_playerFocusable,
			_intangible,
			EffectivePlaneId(),
			_persistencePolicy,
			_collapseEcho,
			_backlashEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CopySpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable key used to reuse the same provisioned copy form
	#3race <which>#0 - sets the race for the provisioned copy form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override
	#3gender <which>|clear#0 - sets or clears the gender override
	#3alias <text>|clear#0 - sets or clears the initial alias
	#3sort <number>|clear#0 - sets or clears the initial sort order
	#3focusable [true|false]#0 - toggles or sets whether PCs can focus this copy
	#3persistent|temporary#0 - sets whether the copy survives reboot or despawns on reboot
	#3persistence <persistent|temporary|logout|effect>#0 - sets the persistence policy
	#3intangible [true|false]#0 - toggles or sets planar intangibility
	#3plane <which>#0 - sets the plane for intangible copies
	#3collapseecho <text>|default|none#0 - sets the focus-return echo on collapse
	#3backlashecho <text>|default|none#0 - sets optional private backlash text";

	public string Show(ICharacter actor)
	{
		var plane = Gameworld.Planes.Get(EffectivePlaneId());
		return SpellEffectPresentation.Describe(actor, "Create Copy",
			("Form Key", FormKey.ColourCommand()),
			("Race", _race.Name.ColourName()),
			("Ethnicity", _ethnicity?.Name.ColourName() ?? "Auto".ColourValue()),
			("Gender", _gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()),
			("Alias", (_alias ?? "auto").ColourCommand()),
			("Focusable", _playerFocusable.ToColouredString()),
			("Persistence", _persistencePolicy.DescribeEnum().ColourValue()),
			("Intangible", _intangible.ToColouredString()),
			("Plane", (plane?.Name ?? $"#{EffectivePlaneId().ToString("N0", actor)}").ColourName()),
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
			case "race":
				return BuildingCommandRace(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "gender":
				return BuildingCommandGender(actor, command);
			case "alias":
				return BuildingCommandAlias(actor, command);
			case "sort":
			case "sortorder":
				return BuildingCommandSortOrder(actor, command);
			case "focusable":
				return BuildingCommandToggle(actor, command, "player focusability", _playerFocusable,
					value => _playerFocusable = value);
			case "persistent":
				return BuildingCommandSetPersistence(actor, CharacterInstancePersistencePolicy.Persistent);
			case "temporary":
				return BuildingCommandSetPersistence(actor, CharacterInstancePersistencePolicy.DespawnOnReboot);
			case "persistence":
			case "persist":
				return BuildingCommandPersistence(actor, command);
			case "intangible":
			case "incorporeal":
				return BuildingCommandToggle(actor, command, "intangibility", _intangible,
					value => _intangible = value);
			case "plane":
				return BuildingCommandPlane(actor, command);
			case "collapseecho":
			case "collapse":
				return BuildingCommandEcho(actor, command, "collapse", DefaultCollapseEcho,
					value => _collapseEcho = value);
			case "backlashecho":
			case "backlash":
				return BuildingCommandEcho(actor, command, "backlash", string.Empty,
					value => _backlashEcho = value);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public static PlanarPresenceDefinition CreateMagicalCopyPlanarPresence(IFuturemud gameworld, long planeId)
	{
		var copyPlaneId = planeId > 0
			? planeId
			: DefaultCopyPlane(gameworld)?.Id ?? gameworld.DefaultPlane?.Id ?? 1L;
		var defaultPlaneId = gameworld.DefaultPlane?.Id ?? copyPlaneId;
		var visibleTo = new[] { copyPlaneId, defaultPlaneId }.Distinct().ToArray();
		var perceives = new[] { copyPlaneId, defaultPlaneId }.Distinct().ToArray();
		var copyOnly = new[] { copyPlaneId };
		var interactions = Enum.GetValues<PlanarInteractionKind>()
		                       .ToDictionary(
			                       x => x,
			                       x => x is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear or
				                       PlanarInteractionKind.Speak or PlanarInteractionKind.Magic
				                       ? (IEnumerable<long>)copyOnly
				                       : Array.Empty<long>());

		return new PlanarPresenceDefinition(
			copyOnly,
			visibleTo,
			perceives,
			interactions,
			true,
			false,
			false,
			false,
			true,
			false,
			false,
			MagicalCopyTransitionProfile);
	}

	private long EffectivePlaneId()
	{
		return _planeId > 0 ? _planeId : DefaultCopyPlane(Gameworld)?.Id ?? Gameworld.DefaultPlane?.Id ?? 1L;
	}

	private static IPlane? DefaultCopyPlane(IFuturemud gameworld)
	{
		return gameworld.Planes.GetByIdOrName("Astral Plane") ??
		       gameworld.DefaultPlane ??
		       gameworld.Planes.FirstOrDefault();
	}

	private bool BuildingCommandFormKey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What stable form key should this effect use?");
			return false;
		}

		FormKey = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the stable form key {FormKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race should this spell provision?");
			return false;
		}

		var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race is null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return false;
		}

		_race = race;
		if (_ethnicity is not null && !_race.SameRace(_ethnicity.ParentRace))
		{
			_ethnicity = null;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision a {_race.Name.ColourName()} magical copy form.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_ethnicity = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible ethnicity.");
			return true;
		}

		var ethnicity = Gameworld.Ethnicities.GetByIdOrName(command.SafeRemainingArgument);
		if (ethnicity is null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return false;
		}

		if (!_race.SameRace(ethnicity.ParentRace))
		{
			actor.OutputHandler.Send("That ethnicity is not compatible with the currently selected race.");
			return false;
		}

		_ethnicity = ethnicity;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_ethnicity.Name.ColourName()} ethnicity.");
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gender should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_gender = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible gender.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		_gender = gender;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_gender.Value.DescribeEnum().ColourValue()} gender.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial alias should this spell use for its provisioned form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_alias = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select its initial alias from the race name.");
			return true;
		}

		_alias = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now initially name its provisioned form {_alias.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial sort order should this spell use for its form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_sortOrder = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now append its form after the character's existing forms.");
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

	private bool BuildingCommandToggle(ICharacter actor, StringStack command, string label, bool current,
		Action<bool> setter)
	{
		var value = !current;
		if (!command.IsFinished && !TryParseBoolean(command.SafeRemainingArgument, out value))
		{
			actor.OutputHandler.Send("You must specify true or false if you provide an explicit value.");
			return false;
		}

		setter(value);
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect's {label} is now {value.ToColouredString()}.");
		return true;
	}

	private bool BuildingCommandPersistence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify persistent, temporary, logout or effect.");
			return false;
		}

		if (!TryParsePersistence(command.SafeRemainingArgument, out var policy))
		{
			actor.OutputHandler.Send("You must specify persistent, temporary, logout or effect.");
			return false;
		}

		return BuildingCommandSetPersistence(actor, policy);
	}

	private bool BuildingCommandSetPersistence(ICharacter actor, CharacterInstancePersistencePolicy policy)
	{
		_persistencePolicy = policy;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use {_persistencePolicy.DescribeEnum().ColourValue()} persistence.");
		return true;
	}

	private bool BuildingCommandPlane(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which plane should intangible copies inhabit?");
			return false;
		}

		var plane = Gameworld.Planes.GetByIdOrName(command.SafeRemainingArgument);
		if (plane is null)
		{
			actor.OutputHandler.Send("There is no such plane.");
			return false;
		}

		_planeId = plane.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Intangible copies will now inhabit {plane.Name.ColourName()}.");
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

	private static bool TryParseBoolean(string text, out bool value)
	{
		switch ((text ?? string.Empty).ToLowerInvariant())
		{
			case "true":
			case "yes":
			case "on":
			case "enabled":
				value = true;
				return true;
			case "false":
			case "no":
			case "off":
			case "disabled":
				value = false;
				return true;
			default:
				value = false;
				return false;
		}
	}

	private static bool TryParsePersistence(string text, out CharacterInstancePersistencePolicy policy)
	{
		switch ((text ?? string.Empty).ToLowerInvariant().CollapseString())
		{
			case "persistent":
			case "permanent":
				policy = CharacterInstancePersistencePolicy.Persistent;
				return true;
			case "temporary":
			case "temp":
			case "reboot":
			case "despawnonreboot":
				policy = CharacterInstancePersistencePolicy.DespawnOnReboot;
				return true;
			case "logout":
			case "despawnonlogout":
				policy = CharacterInstancePersistencePolicy.DespawnOnLogout;
				return true;
			case "effect":
			case "effectbound":
			case "temporaryeffectbound":
				policy = CharacterInstancePersistencePolicy.TemporaryEffectBound;
				return true;
			default:
				return text.TryParseEnum(out policy);
		}
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

public class CloneSpellEffect : IMagicSpellEffectTemplate
{
	public const string DefaultDeathEcho = "Your physical clone collapses and your focus returns to your primary body.";

	private IRace _race = null!;
	private IEthnicity? _ethnicity;
	private Gender? _gender;
	private string? _alias;
	private int? _sortOrder;
	private bool _playerFocusable;
	private CharacterInstancePersistencePolicy _persistencePolicy = CharacterInstancePersistencePolicy.DespawnOnReboot;
	private string _deathEcho = DefaultDeathEcho;
	private string _backlashEcho = string.Empty;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("createclone", (root, spell) => new CloneSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("createclone", BuilderFactory,
			"Creates a tangible physical clone body instance.",
			HelpText,
			false,
			false,
			SpellTriggerFactory.MagicTriggerTypes
			                  .Where(x => CopySpellEffect.IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                  .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		var defaultRace = spell.Gameworld.Races.First();
		return (new CloneSpellEffect(new XElement("Effect",
			new XAttribute("type", "createclone"),
			new XElement("FormKey", "clone"),
			new XElement("Race", defaultRace.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", $"{defaultRace.Name} physical clone"),
			new XElement("SortOrder", string.Empty),
			new XElement("PlayerFocusable", false),
			new XElement("PersistencePolicy", CharacterInstancePersistencePolicy.DespawnOnReboot),
			new XElement("DeathEcho", new XCData(DefaultDeathEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty))
		), spell), string.Empty);
	}

	private CloneSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "clone";
		_race = Gameworld.Races.Get(long.Parse(root.Element("Race")?.Value ?? "0")) ?? Gameworld.Races.First();
		_ethnicity = Gameworld.Ethnicities.Get(long.Parse(root.Element("Ethnicity")?.Value ?? "0"));
		if (int.TryParse(root.Element("Gender")?.Value, out var genderValue) && genderValue >= 0)
		{
			_gender = (Gender)genderValue;
		}

		_alias = root.Element("Alias")?.Value;
		if (int.TryParse(root.Element("SortOrder")?.Value, out var sortOrder))
		{
			_sortOrder = sortOrder;
		}

		_playerFocusable = bool.Parse(root.Element("PlayerFocusable")?.Value ?? "false");
		_persistencePolicy = (root.Element("PersistencePolicy")?.Value ?? string.Empty)
			.TryParseEnum<CharacterInstancePersistencePolicy>(out var persistence)
			? persistence
			: CharacterInstancePersistencePolicy.DespawnOnReboot;
		_deathEcho = root.Element("DeathEcho")?.Value ?? DefaultDeathEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
	}

	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicSpell Spell { get; }
	public string FormKey { get; private set; }

	private ICharacterFormSpecification FormSpecification => new CharacterFormSpecification
	{
		Race = _race,
		Ethnicity = _ethnicity,
		Gender = _gender,
		Alias = _alias,
		SortOrder = _sortOrder
	};

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "createclone"),
			new XElement("FormKey", FormKey),
			new XElement("Race", _race.Id),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("PlayerFocusable", _playerFocusable),
			new XElement("PersistencePolicy", _persistencePolicy),
			new XElement("DeathEcho", new XCData(_deathEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho))
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => false;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return CopySpellEffect.IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var recipient = target as ICharacter ?? caster;
		var anchor = recipient.Identity.PrimaryInstance as ICharacter ?? recipient;
		if (anchor.InstanceId != recipient.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on a primary body to create a physical clone.");
			return null;
		}

		if (anchor.Location is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not create a physical clone for character #{anchor.Id.ToString("N0")}: the character has no location.",
				true
			);
			return null;
		}

		if (!anchor.EnsureForm(FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, FormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure physical clone form '{FormKey}' for character #{anchor.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		var result = CharacterInstanceService.SpawnSecondaryInstance(
			CharacterInstanceService.CreatePhysicalCloneSpawnOptions(
				anchor,
				form,
				anchor.Location,
				anchor.RoomLayer,
				Spell.Id,
				FormKey,
				_playerFocusable,
				_persistencePolicy));
		if (!result.Success || result.Instance is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn physical clone for character #{anchor.Id.ToString("N0")}: {result.Message}",
				true
			);
			caster.OutputHandler.Send("The physical clone fails to take shape.");
			return null;
		}

		return new SpellPhysicalCloneEffect(
			anchor,
			parent,
			FormKey,
			result.Instance.InstanceId,
			result.Instance.Body.Id,
			anchor.InstanceId,
			Spell.Id,
			_playerFocusable,
			_persistencePolicy,
			_deathEcho,
			_backlashEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new CloneSpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable key used to reuse the same provisioned clone form
	#3race <which>#0 - sets the race for the provisioned clone form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override
	#3gender <which>|clear#0 - sets or clears the gender override
	#3alias <text>|clear#0 - sets or clears the initial alias
	#3sort <number>|clear#0 - sets or clears the initial sort order
	#3focusable [true|false]#0 - toggles or sets whether PCs can focus this clone
	#3persistent|temporary#0 - sets whether the clone survives reboot or despawns on reboot
	#3persistence <persistent|temporary|logout|effect>#0 - sets the persistence policy
	#3deathecho <text>|default|none#0 - sets the focus-return echo on clone death/retire
	#3backlashecho <text>|default|none#0 - sets optional private backlash text";

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Create Clone",
			("Form Key", FormKey.ColourCommand()),
			("Race", _race.Name.ColourName()),
			("Ethnicity", _ethnicity?.Name.ColourName() ?? "Auto".ColourValue()),
			("Gender", _gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()),
			("Alias", (_alias ?? "auto").ColourCommand()),
			("Focusable", _playerFocusable.ToColouredString()),
			("Persistence", _persistencePolicy.DescribeEnum().ColourValue()),
			("Death Echo", CopySpellEffect.DescribeEchoForClone(_deathEcho, DefaultDeathEcho)),
			("Backlash", CopySpellEffect.DescribeEchoForClone(_backlashEcho, string.Empty))
		);
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "formkey":
				return BuildingCommandFormKey(actor, command);
			case "race":
				return BuildingCommandRace(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "gender":
				return BuildingCommandGender(actor, command);
			case "alias":
				return BuildingCommandAlias(actor, command);
			case "sort":
			case "sortorder":
				return BuildingCommandSortOrder(actor, command);
			case "focusable":
				return BuildingCommandToggle(actor, command);
			case "persistent":
				return BuildingCommandSetPersistence(actor, CharacterInstancePersistencePolicy.Persistent);
			case "temporary":
				return BuildingCommandSetPersistence(actor, CharacterInstancePersistencePolicy.DespawnOnReboot);
			case "persistence":
			case "persist":
				return BuildingCommandPersistence(actor, command);
			case "deathecho":
			case "death":
				return BuildingCommandEcho(actor, command, "death", DefaultDeathEcho, value => _deathEcho = value);
			case "backlashecho":
			case "backlash":
				return BuildingCommandEcho(actor, command, "backlash", string.Empty, value => _backlashEcho = value);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandFormKey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What stable form key should this effect use?");
			return false;
		}

		FormKey = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the stable form key {FormKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race should this spell provision?");
			return false;
		}

		var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race is null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return false;
		}

		_race = race;
		if (_ethnicity is not null && !_race.SameRace(_ethnicity.ParentRace))
		{
			_ethnicity = null;
		}

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision a {_race.Name.ColourName()} physical clone form.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_ethnicity = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible ethnicity.");
			return true;
		}

		var ethnicity = Gameworld.Ethnicities.GetByIdOrName(command.SafeRemainingArgument);
		if (ethnicity is null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return false;
		}

		if (!_race.SameRace(ethnicity.ParentRace))
		{
			actor.OutputHandler.Send("That ethnicity is not compatible with the currently selected race.");
			return false;
		}

		_ethnicity = ethnicity;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_ethnicity.Name.ColourName()} ethnicity.");
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gender should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_gender = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible gender.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		_gender = gender;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_gender.Value.DescribeEnum().ColourValue()} gender.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial alias should this spell use for its provisioned form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_alias = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select its initial alias from the race name.");
			return true;
		}

		_alias = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now initially name its provisioned form {_alias.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial sort order should this spell use for its form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_sortOrder = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now append its form after the character's existing forms.");
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

	private bool BuildingCommandToggle(ICharacter actor, StringStack command)
	{
		var value = !_playerFocusable;
		if (!command.IsFinished && !CopySpellEffect.TryParseBooleanForClone(command.SafeRemainingArgument, out value))
		{
			actor.OutputHandler.Send("You must specify true or false if you provide an explicit value.");
			return false;
		}

		_playerFocusable = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect's player focusability is now {_playerFocusable.ToColouredString()}.");
		return true;
	}

	private bool BuildingCommandPersistence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify persistent, temporary, logout or effect.");
			return false;
		}

		if (!CopySpellEffect.TryParsePersistenceForClone(command.SafeRemainingArgument, out var policy))
		{
			actor.OutputHandler.Send("You must specify persistent, temporary, logout or effect.");
			return false;
		}

		return BuildingCommandSetPersistence(actor, policy);
	}

	private bool BuildingCommandSetPersistence(ICharacter actor, CharacterInstancePersistencePolicy policy)
	{
		_persistencePolicy = policy;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use {_persistencePolicy.DescribeEnum().ColourValue()} persistence.");
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
}

public partial class CopySpellEffect
{
	public static bool TryParseBooleanForClone(string text, out bool value)
	{
		return TryParseBoolean(text, out value);
	}

	public static bool TryParsePersistenceForClone(string text, out CharacterInstancePersistencePolicy policy)
	{
		return TryParsePersistence(text, out policy);
	}

	public static string DescribeEchoForClone(string echo, string defaultEcho)
	{
		return DescribeEcho(echo, defaultEcho);
	}
}
