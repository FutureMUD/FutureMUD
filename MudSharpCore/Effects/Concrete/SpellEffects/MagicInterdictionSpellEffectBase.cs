using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using System;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract class MagicInterdictionSpellEffectBase : MagicSpellEffectBase, IMagicInterdictionEffect
{
	protected MagicInterdictionSpellEffectBase(IPerceivable owner, IMagicSpellEffectParent parent, IMagicSchool school,
		MagicInterdictionMode mode, MagicInterdictionCoverage coverage, bool includesSubschools, IFutureProg? prog)
		: base(owner, parent, prog)
	{
		School = school;
		Mode = mode;
		Coverage = coverage;
		IncludesSubschools = includesSubschools;
	}

	protected MagicInterdictionSpellEffectBase(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement trueRoot = root.Element("Effect");
		School = Gameworld.MagicSchools.Get(long.Parse(trueRoot.Element("School").Value));
		Mode = Enum.Parse<MagicInterdictionMode>(trueRoot.Element("Mode").Value, true);
		Coverage = Enum.Parse<MagicInterdictionCoverage>(trueRoot.Element("Coverage").Value, true);
		IncludesSubschools = bool.Parse(trueRoot.Element("IncludesSubschools").Value);
	}

	public IMagicSchool School { get; protected set; }
	public MagicInterdictionCoverage Coverage { get; protected set; }
	public MagicInterdictionMode Mode { get; protected set; }
	public bool IncludesSubschools { get; protected set; }

	public bool ShouldInterdict(ICharacter source, IMagicSchool school)
	{
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
			new XElement("IncludesSubschools", IncludesSubschools)
		);
	}

	protected string InterdictionDescription(IPerceiver voyeur, string kind)
	{
		return
			$"{kind} Ward - {School.Name.Colour(School.PowerListColour)} - {Coverage.DescribeEnum().ColourValue()} - {Mode.DescribeEnum().ColourValue()}{(IncludesSubschools ? " incl. subschools" : "")}";
	}
}
