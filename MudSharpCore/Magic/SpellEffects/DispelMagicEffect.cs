#nullable enable

using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public enum DispelMagicMode
{
	Remove = 0,
	Shorten = 1
}

public enum DispelCasterPolicy
{
	OwnOnly = 0,
	AnyCaster = 1,
	OthersOnly = 2
}

public class DispelMagicEffect : IMagicSpellEffectTemplate
{
	private static readonly Dictionary<string, Func<IMagicSpellEffect, bool>> EffectKeyMatchers =
		new(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "any", _ => true },
			{ "spell", _ => true },
			{ "magictag", x => x is IMagicTagEffect },
			{ "itemenchant", x => x is SpellItemEnchantmentEffect },
			{ "portal", x => x is SpellPortalEffect },
			{ "planarstate", x => x is SpellPlanarStateEffect },
			{ "roomward", x => x is SpellRoomWardEffect },
			{ "personalward", x => x is SpellPersonalWardEffect },
			{ "exitbarrier", x => x is SpellExitBarrierEffect },
			{ "subjectivedesc", x => x is SpellSubjectiveDescriptionEffect },
			{ "transformform", x => x is SpellTransformFormEffect },
			{ "projectile", x => x is IMagicProjectilePayloadEffect },
			{ "crafttool", x => x is IMagicCraftToolEnhancementEffect },
			{ "powerfuel", x => x is IMagicPowerOrFuelEnhancementEffect },
			{ "itemevent", x => x is IMagicItemEventEffect }
		};

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("dispelmagic", (root, spell) => new DispelMagicEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("dispelmagic", BuilderFactory,
			"Removes or shortens matching magical spell effects",
			HelpText,
			true,
			true,
			SpellTriggerFactory.MagicTriggerTypes
				.Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
				.ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new DispelMagicEffect(new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Mode", (int)DispelMagicMode.Remove),
			new XElement("CasterPolicy", (int)DispelCasterPolicy.OwnOnly),
			new XElement("AllowHostile", false),
			new XElement("IncludeSubschools", true),
			new XElement("ShortenSeconds", 60.0),
			new XElement("SpellId", 0L),
			new XElement("SchoolId", 0L),
			new XElement("Tag", new XCData(string.Empty)),
			new XElement("TagValue", new XCData(string.Empty)),
			new XElement("MatchTagValue", false),
			new XElement("EffectKey", new XCData("any"))
		), spell), string.Empty);
	}

	private DispelMagicEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		Mode = (DispelMagicMode)int.Parse(root.Element("Mode")?.Value ?? "0");
		CasterPolicy = (DispelCasterPolicy)int.Parse(root.Element("CasterPolicy")?.Value ?? "0");
		AllowHostile = bool.Parse(root.Element("AllowHostile")?.Value ?? "false");
		IncludeSubschools = bool.Parse(root.Element("IncludeSubschools")?.Value ?? "true");
		ShortenDuration = TimeSpan.FromSeconds(double.Parse(root.Element("ShortenSeconds")?.Value ?? "60"));
		SpellId = long.Parse(root.Element("SpellId")?.Value ?? "0");
		SchoolId = long.Parse(root.Element("SchoolId")?.Value ?? "0");
		Tag = root.Element("Tag")?.Value ?? string.Empty;
		TagValue = root.Element("TagValue")?.Value ?? string.Empty;
		MatchTagValue = bool.Parse(root.Element("MatchTagValue")?.Value ?? "false");
		EffectKey = root.Element("EffectKey")?.Value ?? "any";
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public DispelMagicMode Mode { get; private set; }
	public DispelCasterPolicy CasterPolicy { get; private set; }
	public bool AllowHostile { get; private set; }
	public bool IncludeSubschools { get; private set; }
	public TimeSpan ShortenDuration { get; private set; }
	public long SpellId { get; private set; }
	public long SchoolId { get; private set; }
	public string Tag { get; private set; }
	public string TagValue { get; private set; }
	public bool MatchTagValue { get; private set; }
	public string EffectKey { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "dispelmagic"),
			new XElement("Mode", (int)Mode),
			new XElement("CasterPolicy", (int)CasterPolicy),
			new XElement("AllowHostile", AllowHostile),
			new XElement("IncludeSubschools", IncludeSubschools),
			new XElement("ShortenSeconds", ShortenDuration.TotalSeconds),
			new XElement("SpellId", SpellId),
			new XElement("SchoolId", SchoolId),
			new XElement("Tag", new XCData(Tag)),
			new XElement("TagValue", new XCData(TagValue)),
			new XElement("MatchTagValue", MatchTagValue),
			new XElement("EffectKey", new XCData(EffectKey))
		);
	}

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string targetTypes)
	{
		return targetTypes is "character" or "characters" or "item" or "items" or "room" or "rooms" or
			"perceivable" or "perceivables" or "character&room";
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is null)
		{
			return null;
		}

		var parents = target.EffectsOfType<MagicSpellParent>(x => MatchesParent(caster, x)).ToList();
		foreach (var effect in parents)
		{
			if (Mode == DispelMagicMode.Remove)
			{
				target.RemoveEffect(effect, true);
				continue;
			}

			target.RemoveDuration(effect, ShortenDuration, true);
		}

		return null;
	}

	private bool MatchesParent(ICharacter caster, MagicSpellParent parent)
	{
		if (!MatchesCaster(caster, parent))
		{
			return false;
		}

		if (SpellId > 0L && parent.Spell.Id != SpellId)
		{
			return false;
		}

		if (SchoolId > 0L)
		{
			var school = Gameworld.MagicSchools.Get(SchoolId);
			if (school is null)
			{
				return false;
			}

			if (parent.Spell.School != school && (!IncludeSubschools || !parent.Spell.School.IsChildSchool(school)))
			{
				return false;
			}
		}

		if (!string.IsNullOrWhiteSpace(Tag) && !parent.SpellEffects.OfType<IMagicTagEffect>().Any(MatchesTag))
		{
			return false;
		}

		if (!string.IsNullOrWhiteSpace(EffectKey) && !EffectKey.EqualTo("any"))
		{
			if (!EffectKeyMatchers.TryGetValue(EffectKey, out var matcher))
			{
				return false;
			}

			if (!parent.SpellEffects.Any(matcher))
			{
				return false;
			}
		}

		return true;
	}

	private bool MatchesCaster(ICharacter caster, MagicSpellParent parent)
	{
		var sameCaster = parent.Caster?.Id == caster.Id;
		if (!sameCaster && !AllowHostile)
		{
			return false;
		}

		return CasterPolicy switch
		{
			DispelCasterPolicy.OwnOnly => sameCaster,
			DispelCasterPolicy.AnyCaster => true,
			DispelCasterPolicy.OthersOnly => !sameCaster,
			_ => false
		};
	}

	private bool MatchesTag(IMagicTagEffect tag)
	{
		if (!tag.Tag.EqualTo(Tag))
		{
			return false;
		}

		return !MatchTagValue || tag.Value.EqualTo(TagValue);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new DispelMagicEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3mode remove|shorten#0 - removes matching spells or shortens their duration
	#3shorten <seconds>#0 - sets the duration removed by shorten mode
	#3caster own|any|others#0 - sets caster matching policy
	#3hostile#0 - toggles whether non-caster spells can be affected
	#3subschools#0 - toggles whether school matching includes child schools
	#3spell <id|none>#0 - restricts matching to a specific spell
	#3school <id|name|none>#0 - restricts matching to a magic school
	#3tag <tag> [value]#0 - restricts matching to a magic tag, optionally including value
	#3tag none#0 - clears tag matching
	#3effect <key>#0 - restricts matching to an approved key: any, magictag, itemenchant, portal, planarstate, roomward, personalward, exitbarrier, subjectivedesc, transformform, projectile, crafttool, powerfuel, itemevent";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "mode":
				return BuildingCommandMode(actor, command);
			case "shorten":
				return BuildingCommandShorten(actor, command);
			case "caster":
				return BuildingCommandCaster(actor, command);
			case "hostile":
				AllowHostile = !AllowHostile;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This dispel will {AllowHostile.NowNoLonger()} affect spells from other casters.");
				return true;
			case "subschools":
			case "subschool":
				IncludeSubschools = !IncludeSubschools;
				Spell.Changed = true;
				actor.OutputHandler.Send($"This dispel will {IncludeSubschools.NowNoLonger()} include child schools.");
				return true;
			case "spell":
				return BuildingCommandSpell(actor, command);
			case "school":
				return BuildingCommandSchool(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			case "effect":
			case "key":
				return BuildingCommandEffect(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out DispelMagicMode value))
		{
			actor.OutputHandler.Send($"You must specify either {nameof(DispelMagicMode.Remove).ColourCommand()} or {nameof(DispelMagicMode.Shorten).ColourCommand()}.");
			return false;
		}

		Mode = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This dispel now uses {value.DescribeEnum().ColourValue()} mode.");
		return true;
	}

	private bool BuildingCommandShorten(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var seconds) || seconds <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive number of seconds.");
			return false;
		}

		ShortenDuration = TimeSpan.FromSeconds(seconds);
		Spell.Changed = true;
		actor.OutputHandler.Send($"Shorten mode will remove {ShortenDuration.Describe(actor).ColourValue()} from matching spells.");
		return true;
	}

	private bool BuildingCommandCaster(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out DispelCasterPolicy value))
		{
			actor.OutputHandler.Send($"Valid caster policies are {Enum.GetValues<DispelCasterPolicy>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		CasterPolicy = value;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This dispel now uses the {value.DescribeEnum().ColourValue()} caster policy.");
		return true;
	}

	private bool BuildingCommandSpell(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "0"))
		{
			SpellId = 0L;
			Spell.Changed = true;
			actor.OutputHandler.Send("This dispel no longer restricts by spell.");
			return true;
		}

		var spell = Gameworld.MagicSpells.GetByIdOrName(command.SafeRemainingArgument);
		if (spell is null)
		{
			actor.OutputHandler.Send("There is no such spell.");
			return false;
		}

		SpellId = spell.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This dispel now only affects the {spell.Name.Colour(spell.School.PowerListColour)} spell.");
		return true;
	}

	private bool BuildingCommandSchool(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "0"))
		{
			SchoolId = 0L;
			Spell.Changed = true;
			actor.OutputHandler.Send("This dispel no longer restricts by school.");
			return true;
		}

		var school = Gameworld.MagicSchools.GetByIdOrName(command.SafeRemainingArgument);
		if (school is null)
		{
			actor.OutputHandler.Send("There is no such magic school.");
			return false;
		}

		SchoolId = school.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This dispel now only affects {school.Name.Colour(school.PowerListColour)} spells.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear"))
		{
			Tag = string.Empty;
			TagValue = string.Empty;
			MatchTagValue = false;
			Spell.Changed = true;
			actor.OutputHandler.Send("This dispel no longer restricts by magic tag.");
			return true;
		}

		Tag = command.PopSpeech();
		TagValue = command.SafeRemainingArgument;
		MatchTagValue = !string.IsNullOrWhiteSpace(TagValue);
		Spell.Changed = true;
		actor.OutputHandler.Send(MatchTagValue
			? $"This dispel now matches magic tag {Tag.ColourName()} with value {TagValue.ColourValue()}."
			: $"This dispel now matches any magic tag {Tag.ColourName()}.");
		return true;
	}

	private bool BuildingCommandEffect(ICharacter actor, StringStack command)
	{
		var key = command.SafeRemainingArgument;
		if (!EffectKeyMatchers.ContainsKey(key))
		{
			actor.OutputHandler.Send($"Valid effect keys are {EffectKeyMatchers.Keys.ListToColouredString()}.");
			return false;
		}

		EffectKey = key.ToLowerInvariant();
		Spell.Changed = true;
		actor.OutputHandler.Send($"This dispel now matches effect key {EffectKey.ColourCommand()}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return $"DispelMagic - {Mode.DescribeEnum().ColourValue()} - Caster {CasterPolicy.DescribeEnum().ColourValue()} - Hostile {AllowHostile.ToColouredString()} - School #{SchoolId.ToString("N0", actor).ColourValue()} - Spell #{SpellId.ToString("N0", actor).ColourValue()} - Tag {(string.IsNullOrWhiteSpace(Tag) ? "any".ColourValue() : $"{Tag}={TagValue}".ColourName())} - Effect {EffectKey.ColourCommand()}";
	}
}
