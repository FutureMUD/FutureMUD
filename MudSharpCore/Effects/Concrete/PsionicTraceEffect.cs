#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public sealed class PsionicTraceEffect : Effect, IPsionicTraceEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PsionicTrace", (effect, owner) => new PsionicTraceEffect(effect, owner));
	}

	public PsionicTraceEffect(IPerceivable owner, ICharacter source, ICharacter? target, ICell? sourceCell,
		IMagicPower power, PsionicActivityKind activityKind, string activityDescription,
		string unknownIdentityDescription, Difficulty readDifficulty, int concealmentDifficultyStages,
		Guid traceId, DateTime createdUtc, TimeSpan traceDuration) : base(owner)
	{
		TraceId = traceId;
		SourceCharacterId = source.Id;
		TargetCharacterId = target?.Id;
		SourceCellId = sourceCell?.Id;
		PowerId = power.Id;
		SchoolId = power.School.Id;
		ActivityKind = activityKind;
		ActivityDescription = activityDescription;
		UnknownIdentityDescription = unknownIdentityDescription;
		ReadDifficulty = readDifficulty;
		ConcealmentDifficultyStages = concealmentDifficultyStages;
		CreatedUtc = createdUtc;
		TraceDuration = traceDuration;
	}

	private PsionicTraceEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		TraceId = Guid.Parse(trueRoot?.Element("TraceId")?.Value ?? Guid.NewGuid().ToString());
		SourceCharacterId = long.Parse(trueRoot?.Element("SourceCharacterId")?.Value ?? "0");
		TargetCharacterId = long.TryParse(trueRoot?.Element("TargetCharacterId")?.Value, out var targetId) &&
		                    targetId > 0
			? targetId
			: null;
		SourceCellId = long.TryParse(trueRoot?.Element("SourceCellId")?.Value, out var cellId) && cellId > 0
			? cellId
			: null;
		PowerId = long.Parse(trueRoot?.Element("PowerId")?.Value ?? "0");
		SchoolId = long.Parse(trueRoot?.Element("SchoolId")?.Value ?? "0");
		ActivityKind = Enum.Parse<PsionicActivityKind>(trueRoot?.Element("ActivityKind")?.Value ??
		                                                nameof(PsionicActivityKind.Psychic), true);
		ActivityDescription = trueRoot?.Element("ActivityDescription")?.Value ?? "psychic activity";
		UnknownIdentityDescription = trueRoot?.Element("UnknownIdentityDescription")?.Value ?? "an unknown mind";
		ReadDifficulty = (Difficulty)int.Parse(trueRoot?.Element("ReadDifficulty")?.Value ??
		                                       ((int)Difficulty.Normal).ToString());
		ConcealmentDifficultyStages = int.Parse(trueRoot?.Element("ConcealmentDifficultyStages")?.Value ?? "0");
		CreatedUtc = DateTime.Parse(trueRoot?.Element("CreatedUtc")?.Value ?? DateTime.UtcNow.ToString("O"),
			CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		TraceDuration = TimeSpan.FromSeconds(double.Parse(trueRoot?.Element("TraceDurationSeconds")?.Value ?? "0",
			CultureInfo.InvariantCulture));
	}

	public Guid TraceId { get; }
	public long SourceCharacterId { get; }
	public long? TargetCharacterId { get; }
	public long? SourceCellId { get; }
	public long PowerId { get; }
	public long SchoolId { get; }
	public PsionicActivityKind ActivityKind { get; }
	public string ActivityDescription { get; }
	public string UnknownIdentityDescription { get; }
	public DateTime CreatedUtc { get; }
	public TimeSpan TraceDuration { get; }
	public Difficulty ReadDifficulty { get; }
	public int ConcealmentDifficultyStages { get; }
	public override bool SavingEffect => true;
	public IMagicSchool School => Gameworld.MagicSchools.Get(SchoolId)!;
	public IMagicPower PowerOrigin => Gameworld.MagicPowers.Get(PowerId)!;
	public Difficulty DetectMagicDifficulty => ReadDifficulty;
	public ICharacter? SourceCharacter => SourceCharacterId > 0 ? Gameworld.Actors.Get(SourceCharacterId) : null;
	public ICharacter? TargetCharacter => TargetCharacterId is > 0 ? Gameworld.Actors.Get(TargetCharacterId.Value) : null;
	public ICell? SourceCell => SourceCellId is > 0 ? Gameworld.Cells.Get(SourceCellId.Value) : null;

	public bool Involves(ICharacter character)
	{
		return character.Id == SourceCharacterId || character.Id == TargetCharacterId;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"{ActivityKind.DescribeEnum()} trace of {ActivityDescription.ColourValue()} from {PowerOrigin?.Name.ColourName() ?? "an unknown power".ColourError()}.";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("TraceId", TraceId),
			new XElement("SourceCharacterId", SourceCharacterId),
			new XElement("TargetCharacterId", TargetCharacterId ?? 0L),
			new XElement("SourceCellId", SourceCellId ?? 0L),
			new XElement("PowerId", PowerId),
			new XElement("SchoolId", SchoolId),
			new XElement("ActivityKind", ActivityKind),
			new XElement("ActivityDescription", new XCData(ActivityDescription)),
			new XElement("UnknownIdentityDescription", new XCData(UnknownIdentityDescription)),
			new XElement("ReadDifficulty", (int)ReadDifficulty),
			new XElement("ConcealmentDifficultyStages", ConcealmentDifficultyStages),
			new XElement("CreatedUtc", CreatedUtc.ToString("O", CultureInfo.InvariantCulture)),
			new XElement("TraceDurationSeconds", TraceDuration.TotalSeconds)
		);
	}

	protected override string SpecificEffectType => "PsionicTrace";
}
