#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static bool TryCreateBaneStockSpell(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("apex-bane")) return false;
		command.PopSpeech();
		var schoolText = command.PopSpeech(); var traitText = command.PopSpeech(); var resourceText = command.PopSpeech();
		var filterText = command.PopSpeech(); var resistanceText = command.PopSpeech(); var difficultyText = command.PopSpeech();
		var amountText = command.PopSpeech(); var maximumText = command.PopSpeech(); var typeText = command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText); var trait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText); var filter = actor.Gameworld.FutureProgs.GetByIdOrName(filterText);
		var resistance = actor.Gameworld.Traits.GetByIdOrName(resistanceText);
		if (school is null || trait is null || resource is null || filter is null || resistance is null || !command.IsFinished ||
			!difficultyText.TryParseEnum<Difficulty>(out var difficulty) || !Enum.IsDefined(difficulty) ||
			!typeText.TryParseEnum<DamageType>(out var type) || !Enum.IsDefined(type) ||
			!double.TryParse(amountText, NumberStyles.Float, CultureInfo.InvariantCulture, out var amount) ||
			!double.TryParse(maximumText, NumberStyles.Float, CultureInfo.InvariantCulture, out var maximum))
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock apex-bane <school> <casting skill> <resource> <Boolean eligibility prog (target, caster)> <resistance trait> <resistance difficulty> <damage per grade> <maximum damage> <damage type>#0. Quote names with spaces.".SubstituteANSIColour());
			return true;
		}
		try
		{
			var spell = ArmageddonBaneStock.Create(actor.Gameworld, school, trait, resource, filter, resistance, difficulty, amount, maximum, type);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add it to the intended casting capability. " +
				"The eligibility prog, native resistance and capped damage formula remain editable authored policy.");
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
		return true;
	}
}
