using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerProgItemRoom : CastingTriggerProgTargetWithRoomBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("progitemroom", DoBuilderLoad,
			"Targets a prog-supplied item and room",
			"item&room",
			new CastingTriggerProgItemRoom().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("progitemroom", (root, spell) => new CastingTriggerProgItemRoom(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerProgItemRoom(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetProg", 0L),
					new XElement("TargetRoomProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerProgItemRoom(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected CastingTriggerProgItemRoom()
	{
	}

	protected override string TriggerKeyword => "progitemroom";
	protected override string TriggerDisplayName => "Cast@ProgItem&Room";
	protected override ProgVariableTypes TargetProgReturnType => ProgVariableTypes.Item;
	protected override string TargetTypeName => "item";

	public override XElement SaveToXml()
	{
		return SaveBaseXml();
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerProgItemRoom(SaveToXml(), Spell);
	}

	public override string TargetTypes => "item&room";
}
