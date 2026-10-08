#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private const string SeeTheUnbodiedStockName = "#3see-the-unbodied#0";

	private static bool TryCreateSeeTheUnbodiedStockSpell(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("see-the-unbodied")) return false;
		command.PopSpeech();
		var world = actor.Gameworld;
		var school = world.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait = world.Traits.GetByIdOrName(command.PopSpeech());
		var resource = world.MagicResources.GetByIdOrName(command.PopSpeech());
		var silt = world.Terrains.GetByIdOrName(command.PopSpeech());
		var shadow = world.Terrains.GetByIdOrName(command.PopSpeech());
		var ranks = new List<ITag>();
		for (var rank = 0; rank < 5; rank++)
		{
			var tag = world.Tags.GetByIdOrName(command.PopSpeech());
			if (tag is not null) ranks.Add(tag);
		}
		if (school is null || trait is null || resource is null || silt is null || shadow is null || ranks.Count != 5 || !command.IsFinished)
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock see-the-unbodied <school> <casting skill> <resource> <Silt terrain> <Shadow terrain> <Divination rank0 tag> <rank1> <rank2> <rank3> <rank4>#0. Select explicit native terrain definitions and an ascending rank hierarchy; quote names containing spaces.".SubstituteANSIColour());
			return true;
		}
		try
		{
			var spell = ArmageddonSeeTheUnbodiedStock.Create(world, school, trait, resource, silt, shadow, ranks);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add capability admission with opening30/cap90 and Pierce Concealment raw80 as its prerequisite. Acquisition is separate. Its self-only minimum-Standing filter, terrain IDs, component ranks and lifetime remain editable.");
		}
		catch (InvalidOperationException error) { actor.OutputHandler.Send(error.Message.ColourError()); }
		return true;
	}
}
