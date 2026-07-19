#nullable enable

using MudSharp.Construction;

namespace MudSharp.Effects.Concrete;

public class BodyBackupEffect : Effect, IBodyBackupEffect
{
	public const string DefaultOldLocationEcho = "@ go|goes suddenly still as $1 is left behind.";
	public const string DefaultNoRemainsOldLocationEcho = "@ go|goes suddenly still.";
	public const string DefaultNewLocationEcho = "@ convulse|convulses and draw|draws a sudden breath.";
	public const string DefaultSelfEcho = "You awaken in your backup body.";

	public static bool TryParseRemainsContext(string text, out BodyRemainsContext context)
	{
		if (int.TryParse(text, out var value) && System.Enum.IsDefined(typeof(BodyRemainsContext), value))
		{
			context = (BodyRemainsContext)value;
			return true;
		}

		switch ((text ?? string.Empty).ToLowerInvariant().CollapseString())
		{
			case "final":
			case "finaldeath":
			case "finalcharacterdeath":
				context = BodyRemainsContext.FinalCharacterDeath;
				return true;
			case "abandoned":
			case "abandonedbody":
				context = BodyRemainsContext.AbandonedBody;
				return true;
			case "sleeve":
			case "sleevedeath":
				context = BodyRemainsContext.SleeveDeath;
				return true;
			case "clone":
			case "spentclone":
				context = BodyRemainsContext.SpentClone;
				return true;
			case "other":
				context = BodyRemainsContext.Other;
				return true;
			default:
				context = BodyRemainsContext.AbandonedBody;
				return false;
		}
	}

	public static BodyRemainsContext NormaliseBackupRemainsContext(BodyRemainsContext context)
	{
		return context == BodyRemainsContext.FinalCharacterDeath ? BodyRemainsContext.SleeveDeath : context;
	}

	public static bool TryParseBackupRemainsContext(string text, out BodyRemainsContext context)
	{
		if (!TryParseRemainsContext(text, out context) ||
		    context == BodyRemainsContext.FinalCharacterDeath)
		{
			context = BodyRemainsContext.SleeveDeath;
			return false;
		}

		return true;
	}

	public static string NormaliseEcho(string text, string defaultEcho)
	{
		text ??= string.Empty;
		if (text.EqualToAny("default", "clear"))
		{
			return defaultEcho;
		}

		return text.EqualToAny("none", "suppress", "blank") ? string.Empty : text;
	}

	public static string OldLocationEchoForRemains(string echo, bool hasRemains)
	{
		return !hasRemains && echo.EqualTo(DefaultOldLocationEcho)
			? DefaultNoRemainsOldLocationEcho
			: echo;
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("BodyBackup", (effect, owner) => new BodyBackupEffect(effect, owner));
	}

	public BodyBackupEffect(IPerceivable owner, long backupBodyId, ICell destinationCell, RoomLayer destinationLayer,
		int priority, BodyRemainsContext remainsContext, string sourceDescription, string oldLocationEcho,
		string newLocationEcho, string selfEcho, bool consumeOnUse, IFutureProg? applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		BackupBodyId = backupBodyId;
		DestinationCellId = destinationCell.Id;
		DestinationLayer = destinationLayer;
		Priority = priority;
		RemainsContext = NormaliseBackupRemainsContext(remainsContext);
		SourceDescription = sourceDescription;
		OldLocationEcho = oldLocationEcho;
		NewLocationEcho = newLocationEcho;
		SelfEcho = selfEcho;
		ConsumeOnUse = consumeOnUse;
	}

	private BodyBackupEffect(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Effect");
		BackupBodyId = long.Parse(root?.Element("BackupBodyId")?.Value ?? "0");
		DestinationCellId = long.Parse(root?.Element("DestinationCellId")?.Value ?? "0");
		DestinationLayer = (RoomLayer)int.Parse(root?.Element("DestinationLayer")?.Value ?? "0");
		Priority = int.Parse(root?.Element("Priority")?.Value ?? "0");
		RemainsContext =
			NormaliseBackupRemainsContext((BodyRemainsContext)int.Parse(root?.Element("RemainsContext")?.Value ?? "1"));
		SourceDescription = root?.Element("SourceDescription")?.Value ?? "prog";
		OldLocationEcho = root?.Element("OldLocationEcho")?.Value ?? DefaultOldLocationEcho;
		NewLocationEcho = root?.Element("NewLocationEcho")?.Value ?? DefaultNewLocationEcho;
		SelfEcho = root?.Element("SelfEcho")?.Value ?? DefaultSelfEcho;
		ConsumeOnUse = bool.Parse(root?.Element("ConsumeOnUse")?.Value ?? "true");
	}

	public long BackupBodyId { get; }
	public long DestinationCellId { get; }
	public ICell? DestinationCell => Gameworld.Cells.Get(DestinationCellId);
	public RoomLayer DestinationLayer { get; }
	public int Priority { get; }
	public BodyRemainsContext RemainsContext { get; }
	public bool ConsumeOnUse { get; }
	public string SourceDescription { get; }
	public string OldLocationEcho { get; }
	public string NewLocationEcho { get; }
	public string SelfEcho { get; }

	public void ConsumeBackup(ICharacter character)
	{
		if (ConsumeOnUse)
		{
			character.RemoveEffect(this, true);
		}
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("BackupBodyId", BackupBodyId),
			new XElement("DestinationCellId", DestinationCellId),
			new XElement("DestinationLayer", (int)DestinationLayer),
			new XElement("Priority", Priority),
			new XElement("RemainsContext", (int)RemainsContext),
			new XElement("SourceDescription", new XCData(SourceDescription)),
			new XElement("OldLocationEcho", new XCData(OldLocationEcho)),
			new XElement("NewLocationEcho", new XCData(NewLocationEcho)),
			new XElement("SelfEcho", new XCData(SelfEcho)),
			new XElement("ConsumeOnUse", ConsumeOnUse)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Ready to transfer into body #{BackupBodyId.ToString("N0", voyeur)} on death via {SourceDescription}.";
	}

	public override bool SavingEffect => true;

	protected override string SpecificEffectType => "BodyBackup";
}
