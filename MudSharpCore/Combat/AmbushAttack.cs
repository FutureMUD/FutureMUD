#nullable enable

using MudSharp.Construction;
using MudSharp.Body.Position.PositionStates;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat;

public sealed class AmbushAttack : WeaponAttack, IAmbushAttack
{
	private HashSet<RoomLayer> _sourceLayers = [];
	private HashSet<RoomLayer> _destinationLayers = [];
	public IReadOnlyCollection<RoomLayer> SourceLayers => _sourceLayers;
	public IReadOnlyCollection<RoomLayer> DestinationLayers => _destinationLayers;
	public bool AttemptSeize { get; private set; }
	public Difficulty SeizeDifficulty { get; private set; }

	public AmbushAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld) { }
	public AmbushAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type) => LoadAmbush(null);

	protected override void LoadFromDatabase(Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		LoadAmbush(string.IsNullOrWhiteSpace(attack.AdditionalInfo) ? null : XElement.Parse(attack.AdditionalInfo));
	}

	private void LoadAmbush(XElement? xml)
	{
		_sourceLayers = ReadLayers(xml?.Element("Sources"), [RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.HighInTrees, RoomLayer.Underwater]);
		_destinationLayers = ReadLayers(xml?.Element("Destinations"), [RoomLayer.GroundLevel]);
		AttemptSeize = !bool.TryParse(xml?.Element("Seize")?.Value, out var seize) || seize;
		SeizeDifficulty = Enum.TryParse<Difficulty>(xml?.Element("Resist")?.Value, out var difficulty) && Enum.IsDefined(difficulty)
			? difficulty : Difficulty.Normal;
	}

	private static HashSet<RoomLayer> ReadLayers(XElement? xml, RoomLayer[] defaults) => xml is null ? [.. defaults] :
		xml.Elements("Layer").Select(x => Enum.TryParse<RoomLayer>(x.Value, out var layer) && Enum.IsDefined(layer) ? (RoomLayer?)layer : null)
			.Where(x => x.HasValue).Select(x => x!.Value).ToHashSet();

	private XElement Data() => new("Data", new XElement("Sources", SourceLayers.Select(x => new XElement("Layer", x))),
		new XElement("Destinations", DestinationLayers.Select(x => new XElement("Layer", x))),
		new XElement("Seize", AttemptSeize), new XElement("Resist", SeizeDifficulty));
	protected override void SeedInitialData(Models.WeaponAttack attack)
	{
		LoadAmbush(null);
		attack.AdditionalInfo = Data().ToString();
		attack.RequiredPositionStateIds += $" {PositionClimbing.Instance.Id}";
	}
	protected override void AddAttackSpecificCloneData(Models.WeaponAttack attack) => attack.AdditionalInfo = Data().ToString();
	protected override void SaveAttackSpecificData(Models.WeaponAttack attack) => attack.AdditionalInfo = Data().ToString();
	public override string SpecialListText => $"Ambush {SourceLayers.Select(x => x.DescribeEnum()).ListToString()} -> {DestinationLayers.Select(x => x.DescribeEnum()).ListToString()}, seize {AttemptSeize}";
	protected override string ShowBuilderInternal(ICharacter actor) => $"{SpecialListText}\nSeize resistance: {SeizeDifficulty.Describe().ColourValue()}";
	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor) => sb.Append(ShowBuilderInternal(actor));
	public override string HelpText => $@"{base.HelpText}
	#3source <layer>#0 - toggles a permitted ambush source layer
	#3destination <layer>#0 - toggles a permitted prey layer
	#3seize#0 - toggles an opposed initial grapple after a successful hit
	#3resist <difficulty>#0 - sets the difficulty of resisting that seize";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopForSwitch();
		switch (option)
		{
			case "source": case "destination":
				if (!command.SafeRemainingArgument.TryParseEnum<RoomLayer>(out var layer))
				{ actor.OutputHandler.Send("Specify a valid room layer."); return false; }
				var set = option == "source" ? _sourceLayers : _destinationLayers;
				if (!set.Remove(layer)) set.Add(layer);
				break;
			case "seize": AttemptSeize = !AttemptSeize; break;
			case "resist":
				if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
				{ actor.OutputHandler.Send("Specify a valid resistance difficulty."); return false; }
				SeizeDifficulty = difficulty; break;
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
		Changed = true;
		actor.OutputHandler.Send(ShowBuilderInternal(actor));
		return true;
	}
}
