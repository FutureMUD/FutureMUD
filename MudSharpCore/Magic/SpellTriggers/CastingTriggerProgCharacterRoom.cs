using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerProgCharacterRoom : CastingTriggerProgTargetWithRoomBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("progcharacterroom", DoBuilderLoad,
			"Targets a prog-supplied character and room",
			"character&room",
			new CastingTriggerProgCharacterRoom().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("progcharacterroom", (root, spell) => new CastingTriggerProgCharacterRoom(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerProgCharacterRoom(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetProg", 0L),
					new XElement("TargetRoomProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerProgCharacterRoom(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected CastingTriggerProgCharacterRoom()
	{
	}

	protected override string TriggerKeyword => "progcharacterroom";
	protected override string TriggerDisplayName => "Cast@ProgCharacter&Room";
	protected override ProgVariableTypes TargetProgReturnType => ProgVariableTypes.Character;
	protected override string TargetTypeName => "character";

	public override XElement SaveToXml()
	{
		return SaveBaseXml();
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerProgCharacterRoom(SaveToXml(), Spell);
	}

	public override string TargetTypes => "character&room";
}
