using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class RoomWardEffect : WardSpellEffectBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("roomward", (root, spell) => new RoomWardEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("roomward", BuilderFactory,
			"Applies a room ward that can block or reflect matching magic schools",
			HelpText,
			false,
			true,
			SpellTriggerFactory.MagicTriggerTypes
			                   .Where(x => CompatibleTypes.Contains(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                   .ToArray());
	}

	private static readonly string[] CompatibleTypes = ["room"];

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands, IMagicSpell spell)
	{
		return (new RoomWardEffect(new XElement("Effect",
			new XAttribute("type", "roomward"),
			new XElement("School", spell.School.Id),
			new XElement("Mode", MagicInterdictionMode.Fail),
			new XElement("Coverage", MagicInterdictionCoverage.Both),
			new XElement("IncludesSubschools", true),
			new XElement("Prog", 0L)
		), spell), string.Empty);
	}

	protected RoomWardEffect(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected override string EffectType => "roomward";
	protected override string EffectName => "Room Ward";
	protected override string[] CompatibleTargetTypes => CompatibleTypes;

	protected override IMagicSpellEffect CreateWardEffect(IPerceivable target, IMagicSpellEffectParent parent)
	{
		return target is ICell
			? new SpellRoomWardEffect(target, parent, School, Mode, Coverage, IncludesSubschools, Prog)
			: null;
	}

	protected override IMagicSpellEffectTemplate CloneEffect(XElement root, IMagicSpell spell)
	{
		return new RoomWardEffect(root, spell);
	}
}
