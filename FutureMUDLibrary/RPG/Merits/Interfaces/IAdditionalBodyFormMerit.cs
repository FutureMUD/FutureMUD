#nullable enable
using MudSharp.Character;
using MudSharp.RPG.Merits;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IAdditionalBodyFormMerit : ICharacterMerit
{
	ICharacterFormSpecification FormSpecification { get; }
	bool AutoTransformWhenApplicable { get; }
	ForcedTransformationPriorityBand ForcedTransformationPriorityBand { get; }
	int ForcedTransformationPriorityOffset { get; }
	ForcedTransformationRecheckCadence ApplicabilityRecheckCadence { get; }
}
