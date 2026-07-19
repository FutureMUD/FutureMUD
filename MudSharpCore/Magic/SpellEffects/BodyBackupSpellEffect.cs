#nullable enable

using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class BodyBackupSpellEffect : IMagicSpellEffectTemplate
{
	private IRace _race = null!;
	private IEthnicity? _ethnicity;
	private Gender? _gender;
	private string? _alias;
	private int? _sortOrder;
	private int _priority;
	private BodyRemainsContext _remainsContext = BodyRemainsContext.SleeveDeath;
	private bool _consumeOnUse = true;
	private string _oldLocationEcho = BodyBackupEffect.DefaultOldLocationEcho;
	private string _newLocationEcho = BodyBackupEffect.DefaultNewLocationEcho;
	private string _selfEcho = BodyBackupEffect.DefaultSelfEcho;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("bodybackup", (root, spell) => new BodyBackupSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("bodybackup", BuilderFactory,
			"Ensures or reuses a keyed alternate form and readies it as a death backup.",
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
		return (new BodyBackupSpellEffect(new XElement("Effect",
			new XAttribute("type", "bodybackup"),
			new XElement("FormKey", "default"),
			new XElement("Race", defaultRace.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", $"{defaultRace.Name} backup"),
			new XElement("SortOrder", string.Empty),
			new XElement("Priority", 0),
			new XElement("RemainsContext", (int)BodyRemainsContext.SleeveDeath),
			new XElement("ConsumeOnUse", true),
			new XElement("OldLocationEcho", new XCData(BodyBackupEffect.DefaultOldLocationEcho)),
			new XElement("NewLocationEcho", new XCData(BodyBackupEffect.DefaultNewLocationEcho)),
			new XElement("SelfEcho", new XCData(BodyBackupEffect.DefaultSelfEcho))
		), spell), string.Empty);
	}

	private BodyBackupSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "default";
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

		_ = int.TryParse(root.Element("Priority")?.Value, out _priority);
		if (int.TryParse(root.Element("RemainsContext")?.Value, out var contextValue) &&
		    Enum.IsDefined(typeof(BodyRemainsContext), contextValue))
		{
			_remainsContext = BodyBackupEffect.NormaliseBackupRemainsContext((BodyRemainsContext)contextValue);
		}

		_consumeOnUse = bool.TryParse(root.Element("ConsumeOnUse")?.Value, out var consume) ? consume : true;
		_oldLocationEcho = root.Element("OldLocationEcho")?.Value ?? BodyBackupEffect.DefaultOldLocationEcho;
		_newLocationEcho = root.Element("NewLocationEcho")?.Value ?? BodyBackupEffect.DefaultNewLocationEcho;
		_selfEcho = root.Element("SelfEcho")?.Value ?? BodyBackupEffect.DefaultSelfEcho;
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
			new XAttribute("type", "bodybackup"),
			new XElement("FormKey", FormKey),
			new XElement("Race", _race.Id),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("Priority", _priority),
			new XElement("RemainsContext", (int)_remainsContext),
			new XElement("ConsumeOnUse", _consumeOnUse),
			new XElement("OldLocationEcho", new XCData(_oldLocationEcho)),
			new XElement("NewLocationEcho", new XCData(_newLocationEcho)),
			new XElement("SelfEcho", new XCData(_selfEcho))
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
		if (!recipient.EnsureForm(FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, FormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure body backup form '{FormKey}' for character #{recipient.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		if (recipient.Location is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ready body backup form '{FormKey}' for character #{recipient.Id.ToString("N0")}: the character has no location.",
				true
			);
			return null;
		}

		return new SpellBodyBackupEffect(recipient, parent, FormKey, form.Body.Id, recipient.Location.Id,
			recipient.RoomLayer, _priority, _remainsContext, _oldLocationEcho, _newLocationEcho, _selfEcho,
			_consumeOnUse);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new BodyBackupSpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable key used to reuse the same provisioned backup form
	#3race <which>#0 - sets the race for the provisioned backup form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override
	#3gender <which>|clear#0 - sets or clears the gender override
	#3alias <text>|clear#0 - sets or clears the initial alias
	#3sort <number>|clear#0 - sets or clears the initial sort order
	#3priority <number>#0 - sets the backup priority when multiple backups are ready
	#3remains <abandoned|sleeve|clone|other>#0 - sets the old-body remains context after transfer
	#3consume [true|false]#0 - toggles or sets whether the backup is consumed after transfer
	#3oldecho <text>|default|none#0 - sets, defaults or suppresses the echo at the old body
	#3newecho <text>|default|none#0 - sets, defaults or suppresses the echo at the backup body
	#3selfecho <text>|default|none#0 - sets, defaults or suppresses the private transfer echo";

	public string Show(ICharacter actor)
	{
		return SpellEffectPresentation.Describe(actor, "Body Backup",
			("Form Key", FormKey.ColourCommand()),
			("Race", _race.Name.ColourName()),
			("Ethnicity", _ethnicity?.Name.ColourName() ?? "Auto".ColourValue()),
			("Gender", _gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()),
			("Alias", (_alias ?? "auto").ColourCommand()),
			("Priority", _priority.ToString("N0", actor).ColourValue()),
			("Remains", _remainsContext.DescribeEnum().ColourValue()),
			("Consume On Use", _consumeOnUse.ToColouredString()),
			("Old Body Echo", DescribeEcho(_oldLocationEcho, BodyBackupEffect.DefaultOldLocationEcho)),
			("Backup Body Echo", DescribeEcho(_newLocationEcho, BodyBackupEffect.DefaultNewLocationEcho)),
			("Self Echo", DescribeEcho(_selfEcho, BodyBackupEffect.DefaultSelfEcho))
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
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "remains":
			case "context":
			case "remainscontext":
				return BuildingCommandRemains(actor, command);
			case "consume":
			case "consumeonuse":
				return BuildingCommandConsume(actor, command);
			case "oldecho":
			case "oldechoes":
			case "oldecholocation":
				return BuildingCommandEcho(actor, command, "old-body", BodyBackupEffect.DefaultOldLocationEcho,
					value => _oldLocationEcho = value);
			case "newecho":
			case "newechoes":
			case "newecholocation":
				return BuildingCommandEcho(actor, command, "backup-body", BodyBackupEffect.DefaultNewLocationEcho,
					value => _newLocationEcho = value);
			case "selfecho":
				return BuildingCommandEcho(actor, command, "private", BodyBackupEffect.DefaultSelfEcho,
					value => _selfEcho = value);
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
		actor.OutputHandler.Send($"This effect will now provision a {_race.Name.ColourName()} backup body.");
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

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What priority should this backup have?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var priority))
		{
			actor.OutputHandler.Send("That is not a valid priority.");
			return false;
		}

		_priority = priority;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This backup will now have priority {_priority.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRemains(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify abandoned, sleeve, clone or other.");
			return false;
		}

		if (!BodyBackupEffect.TryParseBackupRemainsContext(command.SafeRemainingArgument, out var context))
		{
			actor.OutputHandler.Send("You must specify abandoned, sleeve, clone or other. Backup transfers cannot create final-death corpses.");
			return false;
		}

		_remainsContext = context;
		Spell.Changed = true;
		actor.OutputHandler.Send(
			$"The old body will now become {_remainsContext.DescribeEnum().ColourValue()} remains when the backup transfer happens.");
		return true;
	}

	private bool BuildingCommandConsume(ICharacter actor, StringStack command)
	{
		var consume = !_consumeOnUse;
		if (!command.IsFinished)
		{
			switch (command.SafeRemainingArgument.ToLowerInvariant())
			{
				case "true":
				case "yes":
				case "on":
				case "consume":
				case "enabled":
					consume = true;
					break;
				case "false":
				case "no":
				case "off":
				case "keep":
				case "disabled":
					consume = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify true or false if you provide an explicit value.");
					return false;
			}
		}

		_consumeOnUse = consume;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This backup will {_consumeOnUse.NowNoLonger()} be consumed after a death transfer.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string label, string defaultEcho,
		Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {label} transfer echo should this effect use, or should it be defaulted or suppressed?");
			return false;
		}

		var echo = BodyBackupEffect.NormaliseEcho(command.SafeRemainingArgument, defaultEcho);
		setter(echo);
		Spell.Changed = true;
		actor.OutputHandler.Send(echo switch
		{
			"" => $"This effect will now suppress {label} transfer echoes.",
			_ when echo == defaultEcho => $"This effect will now use the default {label} transfer echo.",
			_ => $"This effect will now use {echo.ColourCommand()} as its {label} transfer echo."
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
