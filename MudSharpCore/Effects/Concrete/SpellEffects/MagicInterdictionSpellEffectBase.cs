using MudSharp.Magic;

#nullable enable
#nullable disable warnings

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract partial class MagicInterdictionSpellEffectBase : MagicSpellEffectBase, IMagicInterdictionUseEffect
{
	protected MagicInterdictionSpellEffectBase(IPerceivable owner, IMagicSpellEffectParent parent, IMagicSchool school,
		MagicInterdictionMode mode, MagicInterdictionCoverage coverage, bool includesSubschools, IFutureProg? prog, bool oneUse = false)
		: base(owner, parent, prog)
	{
		School = school;
		Mode = mode;
		Coverage = coverage;
		IncludesSubschools = includesSubschools;
		OneUse = oneUse;
		if (OneUse && (Mode != MagicInterdictionMode.Reflect || Coverage != MagicInterdictionCoverage.Incoming))
			throw new ArgumentException("One-use wards require incoming reflection.");
	}

	protected MagicInterdictionSpellEffectBase(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement trueRoot = root.Element("Effect");
		School = Gameworld.MagicSchools.Get(long.Parse(trueRoot.Element("School").Value));
		Mode = Enum.Parse<MagicInterdictionMode>(trueRoot.Element("Mode").Value, true);
		Coverage = Enum.Parse<MagicInterdictionCoverage>(trueRoot.Element("Coverage").Value, true);
		IncludesSubschools = bool.Parse(trueRoot.Element("IncludesSubschools").Value);
		OneUse = bool.Parse(trueRoot.Element("OneUse")?.Value ?? "false");
		Used = bool.Parse(trueRoot.Element("Used")?.Value ?? "false");
		if (Used && !OneUse) throw new FormatException("An unlimited ward cannot have spent bounded-use state.");
		if (OneUse && (Mode != MagicInterdictionMode.Reflect || Coverage != MagicInterdictionCoverage.Incoming))
			throw new FormatException("One-use wards require incoming reflection.");
	}

	public IMagicSchool School { get; protected set; }
	public MagicInterdictionCoverage Coverage { get; protected set; }
	public MagicInterdictionMode Mode { get; protected set; }
	public bool IncludesSubschools { get; protected set; }

	public bool ShouldInterdict(ICharacter source, IMagicSchool school)
	{
		if (OneUse && (Used || _removed)) return false;
		if (school != School && (!IncludesSubschools || !school.IsChildSchool(School)))
		{
			return false;
		}

		if (ApplicabilityProg is null)
		{
			return true;
		}

		if (ApplicabilityProg.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.MagicSchool]))
		{
			return ApplicabilityProg.ExecuteBool(source, Owner, school);
		}

		if (ApplicabilityProg.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]))
		{
			return ApplicabilityProg.ExecuteBool(source, Owner);
		}

		return false;
	}

	public override bool Applies()
	{
		return true;
	}

	protected XElement SaveInterdictionDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0L),
			new XElement("School", School.Id),
			new XElement("Mode", Mode),
			new XElement("Coverage", Coverage),
			new XElement("IncludesSubschools", IncludesSubschools),
			OneUse ? new XElement("OneUse", true) : null,
			OneUse ? new XElement("Used", Used) : null
		);
	}

	protected string InterdictionDescription(IPerceiver voyeur, string kind)
	{
		return
			$"{kind} Ward - {School.Name.Colour(School.PowerListColour)} - {Coverage.DescribeEnum().ColourValue()} - {Mode.DescribeEnum().ColourValue()}{(IncludesSubschools ? " incl. subschools" : "")}";
	}
}
