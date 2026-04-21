using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerProgCharacter : CastingTriggerProgTargetBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("progcharacter", DoBuilderLoad,
			"Targets a character supplied by a prog",
			"character",
			new CastingTriggerProgCharacter().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("progcharacter", (root, spell) => new CastingTriggerProgCharacter(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerProgCharacter(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerProgCharacter(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected CastingTriggerProgCharacter()
	{
	}

	protected override string TriggerKeyword => "progcharacter";
	protected override string TriggerDisplayName => "Cast@ProgCharacter";
	protected override ProgVariableTypes TargetProgReturnType => ProgVariableTypes.Character;
	protected override string TargetTypeName => "character";

	public override XElement SaveToXml()
	{
		return SaveBaseXml();
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerProgCharacter(SaveToXml(), Spell);
	}

	public override string TargetTypes => "character";
}
