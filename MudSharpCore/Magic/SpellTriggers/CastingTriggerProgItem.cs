using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellTriggers;

public class CastingTriggerProgItem : CastingTriggerProgTargetBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("progitem", DoBuilderLoad,
			"Targets an item supplied by a prog",
			"item",
			new CastingTriggerProgItem().BuildingCommandHelp
		);
		SpellTriggerFactory.RegisterLoadTimeFactory("progitem", (root, spell) => new CastingTriggerProgItem(root, spell));
	}

	private static (IMagicTrigger Trigger, string Error) DoBuilderLoad(StringStack command, IMagicSpell spell)
	{
		return (
			new CastingTriggerProgItem(
				new XElement("Trigger",
					new XElement("MinimumPower", (int)SpellPower.Insignificant),
					new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
					new XElement("TargetProg", 0L)
				), spell), string.Empty);
	}

	protected CastingTriggerProgItem(XElement root, IMagicSpell spell)
		: base(root, spell)
	{
	}

	protected CastingTriggerProgItem()
	{
	}

	protected override string TriggerKeyword => "progitem";
	protected override string TriggerDisplayName => "Cast@ProgItem";
	protected override ProgVariableTypes TargetProgReturnType => ProgVariableTypes.Item;
	protected override string TargetTypeName => "item";

	public override XElement SaveToXml()
	{
		return SaveBaseXml();
	}

	public override IMagicTrigger Clone()
	{
		return new CastingTriggerProgItem(SaveToXml(), Spell);
	}

	public override string TargetTypes => "item";
}
