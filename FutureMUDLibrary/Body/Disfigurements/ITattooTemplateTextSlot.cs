using System.Collections.Generic;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.GameItems;

namespace MudSharp.Body.Disfigurements
{
	public interface ITattooTemplateTextSlot
	{
		string Name { get; }
		int MaximumLength { get; }
		bool RequiredCustomText { get; }
		ILanguage DefaultLanguage { get; }
		IScript DefaultScript { get; }
		WritingStyleDescriptors DefaultStyle { get; }
		IColour DefaultColour { get; }
		double DefaultMinimumSkill { get; }
		string DefaultText { get; }
		string DefaultAlternateText { get; }
	}

	public interface ITattooTextValue
	{
		string Name { get; }
		ILanguage Language { get; }
		IScript Script { get; }
		WritingStyleDescriptors Style { get; }
		IColour Colour { get; }
		double MinimumSkill { get; }
		string Text { get; }
		string AlternateText { get; }
		bool IsCopiedFromSource { get; }
		bool WasCopiedWithoutUnderstanding { get; }
	}

	public interface ISelectedTattoo
	{
		ITattooTemplate Tattoo { get; }
		IBodypart Bodypart { get; }
		IReadOnlyDictionary<string, ITattooTextValue> TextValues { get; }
	}
}
