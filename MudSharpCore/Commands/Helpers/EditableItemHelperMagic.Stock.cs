#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static void CreateStockSpell(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("raise-servitor"))
		{
			actor.OutputHandler.Send("Available stock spells: #3raise-servitor#0.".SubstituteANSIColour());
			return;
		}
		var schoolText = command.PopSpeech();
		var traitText = command.PopSpeech();
		var resourceText = command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText);
		var trait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText);
		if (school is null || trait is null || resource is null || !command.IsFinished)
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock raise-servitor <school> <casting trait> <resource>#0, quoting names containing spaces.".SubstituteANSIColour());
			return;
		}
		try
		{
			var spell = ArmageddonRaiseServitorStock.Create(actor.Gameworld, school, trait, resource);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()} in {school.Name.ColourName()}; you are now editing it. Add it to the intended casting capability. Its control eligibility prog excludes terrain named Silt and Shallows; edit that prog if your world uses other names.");
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
	}
}
