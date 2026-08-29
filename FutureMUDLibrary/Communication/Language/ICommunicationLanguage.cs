#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Communication.Language;

/// <summary>
/// Common contract for natural-language communication modalities.
/// </summary>
public interface ICommunicationLanguage : IEditableItem, IProgVariable
{
	ILanguageDifficultyModel Model { get; }
	ITraitDefinition LinkedTrait { get; }
	string UnknownLanguageDescription { get; }
	double LanguageObfuscationFactor { get; }
}
