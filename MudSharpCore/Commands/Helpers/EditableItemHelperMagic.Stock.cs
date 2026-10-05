#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static void CreateStockSpell(ICharacter actor, StringStack command)
	{
		var stock = command.PopSpeech().ToLowerInvariant();
		if (stock is not ("raise-servitor" or "storm-spear"))
		{
			actor.OutputHandler.Send("Available stock spells: #3raise-servitor#0 and #3storm-spear#0.".SubstituteANSIColour());
			return;
		}
		var schoolText = command.PopSpeech();
		var traitText = command.PopSpeech();
		var resourceText = command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText);
		var trait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText);
		var weapon = stock == "storm-spear" ? actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech()) : null;
		var ranks = new List<ITag>();
		if (stock == "storm-spear")
			for (var rank = 0; rank < 5; rank++)
			{
				var tag = actor.Gameworld.Tags.GetByIdOrName(command.PopSpeech());
				if (tag is not null) ranks.Add(tag);
			}
		if (school is null || trait is null || resource is null || !command.IsFinished ||
			stock == "storm-spear" && (weapon is null || ranks.Count != 5))
		{
			actor.OutputHandler.Send((stock == "raise-servitor"
				? "Use #3magic spell edit new stock raise-servitor <school> <casting trait> <resource>#0, quoting names containing spaces."
				: "Use #3magic spell edit new stock storm-spear <school> <casting trait> <resource> <weapon prototype> <Creation rank 0 tag> <rank 1 tag> <rank 2 tag> <rank 3 tag> <rank 4 tag>#0, quoting names containing spaces. The weapon needs an electrical melee profile and the rank tags must form an ascending hierarchy.").SubstituteANSIColour());
			return;
		}
		try
		{
			var spell = stock == "raise-servitor"
				? ArmageddonRaiseServitorStock.Create(actor.Gameworld, school, trait, resource)
				: ArmageddonStormSpearStock.Create(actor.Gameworld, school, trait, resource, weapon!, ranks);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()} in {school.Name.ColourName()}; you are now editing it. Add it to the intended casting capability. " +
				(stock == "raise-servitor" ? "Its control eligibility prog excludes terrain named Silt and Shallows; edit that prog if your world uses other names."
					: "All grades are temporary. The selected electrical profile is an authored native adaptation. Use plan carried and plan ranks to edit component scope and rank tags."));
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
	}
}
