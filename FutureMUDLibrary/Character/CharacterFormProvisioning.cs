#nullable enable
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;

namespace MudSharp.Character;

public enum CharacterFormSourceType
{
	Merit = 0,
	SpellEffect = 1,
	Prog = 2
}

public interface ICharacterFormSource
{
	CharacterFormSourceType SourceType { get; }
	long SourceId { get; }
	string SourceKey { get; }
}

public class CharacterFormSource : ICharacterFormSource
{
	public CharacterFormSource(CharacterFormSourceType sourceType, long sourceId = 0, string? sourceKey = null)
	{
		SourceType = sourceType;
		SourceId = sourceId;
		SourceKey = sourceKey ?? string.Empty;
	}

	public CharacterFormSourceType SourceType { get; init; }
	public long SourceId { get; init; }
	public string SourceKey { get; init; }
}

public interface ICharacterFormSpecification
{
	IRace Race { get; }
	IEthnicity? Ethnicity { get; }
	Gender? Gender { get; }
	string? Alias { get; }
	int? SortOrder { get; }
	BodySwitchTraumaMode TraumaMode { get; }
	string? TransformationEcho { get; }
	bool AllowVoluntarySwitch { get; }
	IFutureProg? CanVoluntarilySwitchProg { get; }
	IFutureProg? WhyCannotVoluntarilySwitchProg { get; }
	IFutureProg? CanSeeFormProg { get; }
	IEntityDescriptionPattern? ShortDescriptionPattern { get; }
	IEntityDescriptionPattern? FullDescriptionPattern { get; }
}

public class CharacterFormSpecification : ICharacterFormSpecification
{
	public IRace Race { get; init; } = null!;
	public IEthnicity? Ethnicity { get; init; }
	public Gender? Gender { get; init; }
	public string? Alias { get; init; }
	public int? SortOrder { get; init; }
	public BodySwitchTraumaMode TraumaMode { get; init; } = BodySwitchTraumaMode.Automatic;
	public string? TransformationEcho { get; init; }
	public bool AllowVoluntarySwitch { get; init; }
	public IFutureProg? CanVoluntarilySwitchProg { get; init; }
	public IFutureProg? WhyCannotVoluntarilySwitchProg { get; init; }
	public IFutureProg? CanSeeFormProg { get; init; }
	public IEntityDescriptionPattern? ShortDescriptionPattern { get; init; }
	public IEntityDescriptionPattern? FullDescriptionPattern { get; init; }
}
