using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Effects.Interfaces;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRoomWardEffect : MagicInterdictionSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomWard", (effect, owner) => new SpellRoomWardEffect(effect, owner));
	}

	public SpellRoomWardEffect(IPerceivable owner, IMagicSpellEffectParent parent, IMagicSchool school,
		MagicInterdictionMode mode, MagicInterdictionCoverage coverage, bool includesSubschools, IFutureProg? prog)
		: base(owner, parent, school, mode, coverage, includesSubschools, prog)
	{
	}

	protected SpellRoomWardEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return InterdictionDescription(voyeur, "Room");
	}

	protected override XElement SaveDefinition()
	{
		return SaveInterdictionDefinition();
	}

	protected override string SpecificEffectType => "SpellRoomWard";
}
