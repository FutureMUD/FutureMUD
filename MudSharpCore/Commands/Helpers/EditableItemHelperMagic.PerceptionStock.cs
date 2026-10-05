#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private const string PerceptionStockNames = "#3pierce-concealment#0 and " + WaterBreathingStockName;

	private static bool TryCreatePerceptionStockSpell(ICharacter actor, StringStack command)
	{
		if (TryCreateWaterBreathingStockSpell(actor, command)) return true;
		if (!command.PeekSpeech().EqualTo("pierce-concealment")) return false;
		command.PopSpeech();
		var world = actor.Gameworld;
		var school = world.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait = world.Traits.GetByIdOrName(command.PopSpeech());
		var resource = world.MagicResources.GetByIdOrName(command.PopSpeech());
		if (school is null || trait is null || resource is null || !command.IsFinished)
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock pierce-concealment <school> <casting skill> <resource>#0. Quote names containing spaces.".SubstituteANSIColour());
			return true;
		}

		try
		{
			var spell = ArmageddonPierceConcealmentStock.Create(world, school, trait, resource);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add its casting-capability entry with opening 30, cap 90 and relative grades. Acquisition is separate. The description records recovered rules and native adaptations.");
		}
		catch (InvalidOperationException error)
		{
			actor.OutputHandler.Send(error.Message.ColourError());
		}

		return true;
	}
}
