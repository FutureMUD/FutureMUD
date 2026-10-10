#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static void CreateStockSpell(ICharacter actor, StringStack command)
	{
		if (TryCreateShelterStockSpell(actor, command)) return;
		if (TryCreatePerceptionStockSpell(actor, command)) return;
		if (TryCreateUtilityStockSpell(actor, command)) return;
		if (TryCreateProvisionStockSpell(actor, command)) return;
		var stock = command.PopSpeech().ToLowerInvariant();
		if (stock is not ("air-guardian" or "raise-servitor" or "storm-spear" or "flame-knife" or "sand-knife"))
		{
			actor.OutputHandler.Send(("Available stock spells: #3spring-haven#0, #3burrow-refuge#0, #3sand-shelter#0, #3air-guardian#0, #3raise-servitor#0, #3storm-spear#0, #3flame-knife#0, #3sand-knife#0, " + UtilityStockNames + ", " + ProvisionStockNames + ", " + PerceptionStockNames + ".").SubstituteANSIColour());
			return;
		}
		var schoolText = command.PopSpeech();
		var traitText = command.PopSpeech();
		var resourceText = command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText);
		var trait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText);
		var guardian = stock == "air-guardian" ? actor.Gameworld.NpcTemplates.GetByIdOrName(command.PopSpeech()) : null;
		var guardianLifetime = stock == "air-guardian" ? command.SafeRemainingArgument : "";
		var weapon = stock == "storm-spear" ? actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech()) : null;
		var ranks = new List<ITag>();
		var tierPrototypes = new List<GameItems.IGameItemProto>();
		ITag? monComponent = null;
		ITag? sandstormTag = null;
		Climate.IWeatherEvent? sandstormWeather = null;
		if (stock is "flame-knife" or "sand-knife")
		{
			for (var index = 0; index < 14; index++)
			{
				var prototype = actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
				if (prototype is not null) tierPrototypes.Add(prototype);
			}
			monComponent = actor.Gameworld.Tags.GetByIdOrName(command.PopSpeech());
		}
		if (stock == "sand-knife")
		{
			sandstormTag = actor.Gameworld.Tags.GetByIdOrName(command.PopSpeech());
			sandstormWeather = actor.Gameworld.WeatherEvents.GetByIdOrName(command.PopSpeech());
		}
		if (stock == "storm-spear")
			for (var rank = 0; rank < 5; rank++)
			{
				var tag = actor.Gameworld.Tags.GetByIdOrName(command.PopSpeech());
				if (tag is not null) ranks.Add(tag);
			}
		if (school is null || trait is null || resource is null || stock != "air-guardian" && !command.IsFinished ||
			stock == "air-guardian" && (guardian is null || string.IsNullOrWhiteSpace(guardianLifetime)) ||
			stock == "storm-spear" && (weapon is null || ranks.Count != 5) ||
			stock is ("flame-knife" or "sand-knife") && (tierPrototypes.Count != 14 || monComponent is null) ||
			stock == "sand-knife" && (sandstormTag is null || sandstormWeather is null))
		{
			actor.OutputHandler.Send((stock == "air-guardian"
				? "Use #3magic spell edit new stock air-guardian <school> <casting trait> <resource> <approved NPC template> <lifetime expression in real seconds>#0. Quote names containing spaces. The template and timing are explicit native mappings."
				: stock == "sand-knife"
				? "Use #3magic spell edit new stock sand-knife <school> <casting trait> <resource> <grade 1 prototype> ... <grade 6 prototype> <staff 1 prototype> ... <staff 8 prototype> <Creation rank-six tag> <sandstorm room tag> <sandstorm weather event>#0. Select fourteen distinct approved physical melee prototypes and explicit storm mappings. Quote names containing spaces."
				: stock == "flame-knife"
				? "Use #3magic spell edit new stock flame-knife <school> <casting trait> <resource> <grade 1 prototype> ... <grade 6 prototype> <staff 1 prototype> ... <staff 8 prototype> <Conjuration rank-six tag>#0. Select fourteen distinct approved burning melee prototypes. Quote names containing spaces."
				: stock == "raise-servitor"
				? "Use #3magic spell edit new stock raise-servitor <school> <casting trait> <resource>#0, quoting names containing spaces."
				: "Use #3magic spell edit new stock storm-spear <school> <casting trait> <resource> <weapon prototype> <Creation rank 0 tag> <rank 1 tag> <rank 2 tag> <rank 3 tag> <rank 4 tag>#0, quoting names containing spaces. The weapon needs an electrical melee profile and the rank tags must form an ascending hierarchy.").SubstituteANSIColour());
			return;
		}
		try
		{
			var spell = stock switch
			{
				"air-guardian" => ArmageddonAirGuardianStock.Create(actor.Gameworld, school, trait, resource, guardian!, guardianLifetime),
				"raise-servitor" => ArmageddonRaiseServitorStock.Create(actor.Gameworld, school, trait, resource),
				"flame-knife" => ArmageddonFlameKnifeStock.Create(actor.Gameworld, school, trait, resource, tierPrototypes.Take(6).ToArray(), tierPrototypes.Skip(6).ToArray(), monComponent!),
				"sand-knife" => ArmageddonSandKnifeStock.Create(actor.Gameworld, school, trait, resource, tierPrototypes.Take(6).ToArray(), tierPrototypes.Skip(6).ToArray(), monComponent!, sandstormTag!, sandstormWeather!),
				_ => ArmageddonStormSpearStock.Create(actor.Gameworld, school, trait, resource, weapon!, ranks)
			};
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()} in {school.Name.ColourName()}; you are now editing it. Add it to the intended casting capability. " +
				(stock == "air-guardian" ? "One creature is created per requested grade. The selected template, nine-energy-per-grade cost and real-second lifetime are authored native mappings; historical duration units remain unverified. Edit its creation effect to change count, creator protection or lifecycle policy."
					: stock == "sand-knife" ? "Only grade seven consumes a Creation component and creates a permanent staff. Edit output pools, material grade and the sufficient-sand prog. The five-energy lower bound, physical profiles and terrain/tag/weather mappings are authored native policy; no shadow lifetime reduction applies."
					: stock == "flame-knife" ? "Only grade seven consumes a component and creates a permanent staff. Edit output pools, material grade and environment progs; profiles are authored native policy and the printed energy minimum is seven."
					: stock == "raise-servitor" ? "Its control eligibility prog excludes terrain named Silt and Shallows; edit that prog if your world uses other names."
					: "All grades are temporary. The selected electrical profile is an authored native adaptation. Use plan carried and plan ranks to edit component scope and rank tags."));
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
	}
}
