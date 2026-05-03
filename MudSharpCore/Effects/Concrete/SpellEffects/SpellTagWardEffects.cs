#nullable enable

using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract class TagMagicInterdictionSpellEffectBase : MagicSpellEffectBase, IMagicInterdictionEffect, IMagicContextualInterdictionEffect
{
	protected TagMagicInterdictionSpellEffectBase(IPerceivable owner, IMagicSpellEffectParent parent, string tag,
		string value, bool matchValue, MagicInterdictionMode mode, MagicInterdictionCoverage coverage, IFutureProg? prog)
		: base(owner, parent, prog)
	{
		Tag = tag;
		Value = value;
		MatchValue = matchValue;
		Mode = mode;
		Coverage = coverage;
	}

	protected TagMagicInterdictionSpellEffectBase(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Tag = trueRoot?.Element("Tag")?.Value ?? string.Empty;
		Value = trueRoot?.Element("Value")?.Value ?? string.Empty;
		MatchValue = bool.Parse(trueRoot?.Element("MatchValue")?.Value ?? "false");
		Mode = System.Enum.Parse<MagicInterdictionMode>(trueRoot?.Element("Mode")?.Value ?? nameof(MagicInterdictionMode.Fail), true);
		Coverage = System.Enum.Parse<MagicInterdictionCoverage>(trueRoot?.Element("Coverage")?.Value ?? nameof(MagicInterdictionCoverage.Both), true);
	}

	public string Tag { get; }
	public string Value { get; }
	public bool MatchValue { get; }
	public MagicInterdictionCoverage Coverage { get; }
	public MagicInterdictionMode Mode { get; }
	public IMagicSchool School => Spell.School;
	public bool IncludesSubschools => true;

	public bool ShouldInterdict(ICharacter source, IMagicSchool school)
	{
		return false;
	}

	public bool ShouldInterdict(MagicInterdictionContext context)
	{
		if (string.IsNullOrWhiteSpace(Tag))
		{
			return false;
		}

		if (!context.Tags.Any(x => x.Tag.EqualTo(Tag) && (!MatchValue || x.Value.EqualTo(Value))))
		{
			return false;
		}

		if (ApplicabilityProg is null)
		{
			return true;
		}

		if (ApplicabilityProg.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text]))
		{
			return ApplicabilityProg.ExecuteBool(context.Source, Owner, Tag, Value);
		}

		if (ApplicabilityProg.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]))
		{
			return ApplicabilityProg.ExecuteBool(context.Source, Owner);
		}

		return false;
	}

	public override bool Applies()
	{
		return true;
	}

	protected XElement SaveTagInterdictionDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0L),
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value)),
			new XElement("MatchValue", MatchValue),
			new XElement("Mode", Mode),
			new XElement("Coverage", Coverage)
		);
	}

	protected string TagInterdictionDescription(IPerceiver voyeur, string kind)
	{
		return
			$"{kind} Tag Ward - {Tag.ColourValue()}{(MatchValue ? $"={Value.ColourValue()}" : "")} - {Coverage.DescribeEnum().ColourValue()} - {Mode.DescribeEnum().ColourValue()}";
	}
}

public sealed class SpellRoomTagWardEffect : TagMagicInterdictionSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomTagWard", (effect, owner) => new SpellRoomTagWardEffect(effect, owner));
	}

	public SpellRoomTagWardEffect(IPerceivable owner, IMagicSpellEffectParent parent, string tag, string value,
		bool matchValue, MagicInterdictionMode mode, MagicInterdictionCoverage coverage, IFutureProg? prog)
		: base(owner, parent, tag, value, matchValue, mode, coverage, prog)
	{
	}

	private SpellRoomTagWardEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return TagInterdictionDescription(voyeur, "Room");
	}

	protected override string SpecificEffectType => "SpellRoomTagWard";

	protected override XElement SaveDefinition()
	{
		return SaveTagInterdictionDefinition();
	}
}

public sealed class SpellPersonalTagWardEffect : TagMagicInterdictionSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPersonalTagWard", (effect, owner) => new SpellPersonalTagWardEffect(effect, owner));
	}

	public SpellPersonalTagWardEffect(IPerceivable owner, IMagicSpellEffectParent parent, string tag, string value,
		bool matchValue, MagicInterdictionMode mode, MagicInterdictionCoverage coverage, IFutureProg? prog)
		: base(owner, parent, tag, value, matchValue, mode, coverage, prog)
	{
	}

	private SpellPersonalTagWardEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return TagInterdictionDescription(voyeur, "Personal");
	}

	protected override string SpecificEffectType => "SpellPersonalTagWard";

	protected override XElement SaveDefinition()
	{
		return SaveTagInterdictionDefinition();
	}
}
