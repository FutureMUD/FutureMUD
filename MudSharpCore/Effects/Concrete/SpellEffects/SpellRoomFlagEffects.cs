#nullable enable
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellRoomPeacefulEffect : SimpleSpellStatusEffectBase, IPeacefulEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomPeaceful", (effect, owner) => new SpellRoomPeacefulEffect(effect, owner));
	}

	public SpellRoomPeacefulEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellRoomPeacefulEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "This location is warded against violence.";
	}

	protected override string SpecificEffectType => "SpellRoomPeaceful";
}

public class SpellRoomNoDreamEffect : SimpleSpellStatusEffectBase, INoDreamEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomNoDream", (effect, owner) => new SpellRoomNoDreamEffect(effect, owner));
	}

	public SpellRoomNoDreamEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellRoomNoDreamEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Dreams are magically barred in this location.";
	}

	protected override string SpecificEffectType => "SpellRoomNoDream";
}

public class SpellRoomDarknessEffect : SimpleSpellStatusEffectBase, IAreaLightEffect, IDescriptionAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomDarkness", (effect, owner) => new SpellRoomDarknessEffect(effect, owner));
	}

	public SpellRoomDarknessEffect(IPerceivable owner, IMagicSpellEffectParent parent, double darknessLux,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		DarknessLux = darknessLux;
	}

	private SpellRoomDarknessEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		DarknessLux = double.Parse(root.Element("Effect")?.Element("DarknessLux")?.Value ?? "0");
	}

	public double DarknessLux { get; }
	public double AddedLight => -DarknessLux;

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(new XElement("DarknessLux", DarknessLux));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Blanketing the room in darkness by {DarknessLux.ToString("N2", voyeur).ColourValue()} lux.";
	}

	protected override string SpecificEffectType => "SpellRoomDarkness";

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return "Magical darkness shrouds the area.";
	}

	public bool PlayerSet => false;
}

public class SpellRoomAlarmEffect : SimpleSpellStatusEffectBase, IHandleEventsEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomAlarm", (effect, owner) => new SpellRoomAlarmEffect(effect, owner));
	}

	public SpellRoomAlarmEffect(IPerceivable owner, IMagicSpellEffectParent parent, string alarmEcho,
		IFutureProg? alarmProg = null, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		AlarmEcho = alarmEcho;
		AlarmProg = alarmProg;
	}

	private SpellRoomAlarmEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		AlarmEcho = effect.Element("AlarmEcho")?.Value ?? string.Empty;
		AlarmProg = Gameworld.FutureProgs.Get(long.Parse(effect.Element("AlarmProg")?.Value ?? "0"));
	}

	public string AlarmEcho { get; }
	public IFutureProg? AlarmProg { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("AlarmEcho", new XCData(AlarmEcho)),
			new XElement("AlarmProg", AlarmProg?.Id ?? 0L)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "A magical alarm will sound when someone enters this location.";
	}

	protected override string SpecificEffectType => "SpellRoomAlarm";

	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.CharacterEnterCellFinish || Owner is not ICell cell)
		{
			return false;
		}

		ICharacter mover = (ICharacter)arguments[0];
		ICell enteredCell = (ICell)arguments[1];
		if (!ReferenceEquals(cell, enteredCell))
		{
			return false;
		}

		if (!string.IsNullOrWhiteSpace(AlarmEcho))
		{
			cell.HandleRoomEcho(new EmoteOutput(new Emote(AlarmEcho, mover, mover)));
		}

		if (AlarmProg is not null)
		{
			if (AlarmProg.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Character, ProgVariableTypes.Location }))
			{
				AlarmProg.Execute(mover, cell);
			}
			else if (AlarmProg.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }))
			{
				AlarmProg.Execute(mover);
			}
		}

		return false;
	}

	public bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.CharacterEnterCellFinish);
	}
}

public class SpellRoomWardTagEffect : SimpleSpellStatusEffectBase, IRoomWardTagEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellRoomWardTag", (effect, owner) => new SpellRoomWardTagEffect(effect, owner));
	}

	public SpellRoomWardTagEffect(IPerceivable owner, IMagicSpellEffectParent parent, string wardTag,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		WardTag = wardTag;
	}

	private SpellRoomWardTagEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		WardTag = root.Element("Effect")?.Element("WardTag")?.Value ?? string.Empty;
	}

	public string WardTag { get; }

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(new XElement("WardTag", new XCData(WardTag)));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Bearing the magical ward tag {WardTag.ColourValue()}.";
	}

	protected override string SpecificEffectType => "SpellRoomWardTag";
}
