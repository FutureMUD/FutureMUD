#nullable enable

using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Form.Shape;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language;

public interface ISignedLanguage : ICommunicationLanguage
{
	IEnumerable<ISignedLanguageVariety> Varieties { get; }
	IEnumerable<ISignedLanguageArticulationProfile> ArticulationProfiles { get; }
	Difficulty MutualIntelligability(ISignedLanguage otherLanguage);
	SignedLanguageArticulationResult EvaluateArticulation(IBody body);
}

public interface ISignedLanguageVariety : MudSharp.Framework.IFrameworkItem
{
	ISignedLanguage Language { get; }
	string Description { get; }
	string Suffix { get; }
	string VagueSuffix { get; }
	Difficulty RecognitionDifficulty { get; }
}

public interface ISignedLanguageArticulationProfile : MudSharp.Framework.IFrameworkItem
{
	IBodyPrototype BodyPrototype { get; }
	IEnumerable<ISignedLanguageArticulationRequirement> Requirements { get; }
	SignedLanguageArticulationResult Evaluate(IBody body);
}

public interface ISignedLanguageArticulationRequirement
{
	IBodypartShape BodypartShape { get; }
	int MinimumCount { get; }
	int PreferredCount { get; }
}

public readonly record struct SignedLanguageArticulationResult(
	bool CanSign,
	int MissingPreferredParts,
	string Error)
{
	public static SignedLanguageArticulationResult Impossible(string error) => new(false, 0, error);
	public static SignedLanguageArticulationResult Success(int missingPreferredParts = 0) =>
		new(true, missingPreferredParts, string.Empty);
}
