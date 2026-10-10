#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static bool TryCreateShelterStockSpell(ICharacter actor, StringStack command)
	{
		var name = command.PeekSpeech().ToLowerInvariant();
		if (name is not ("spring-haven" or "burrow-refuge" or "sand-shelter")) return false;
		command.PopSpeech();
		var schoolText = command.PopSpeech(); var traitText = command.PopSpeech(); var resourceText = command.PopSpeech();
		var templateText = command.PopSpeech(); var terrainText = command.PopSpeech(); var fallbackText = command.PopSpeech();
		var secondsText = command.PopSpeech(); var capacityText = command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText); var trait = actor.Gameworld.Traits.GetByIdOrName(traitText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText); var template = actor.Gameworld.Rooms.GetByIdOrName(templateText);
		var terrain = actor.Gameworld.Terrains.GetByIdOrName(terrainText); var fallback = actor.Gameworld.Rooms.GetByIdOrName(fallbackText);
		var kind = name == "spring-haven" ? SpellShelterKind.SpringHaven : name == "burrow-refuge" ? SpellShelterKind.BurrowRefuge : SpellShelterKind.SandShelter;
		var waterText = kind == SpellShelterKind.SpringHaven ? command.PopSpeech() : "";
		var liquidText = kind == SpellShelterKind.SpringHaven ? command.PopSpeech() : "";
		var litresText = kind == SpellShelterKind.SpringHaven ? command.PopSpeech() : "0";
		var water = kind == SpellShelterKind.SpringHaven ? actor.Gameworld.ItemProtos.GetByIdOrName(waterText) : null;
		var liquid = kind == SpellShelterKind.SpringHaven ? actor.Gameworld.Liquids.GetByIdOrName(liquidText) : null;
		if (school is null || trait is null || resource is null || template is null || terrain is null || fallback is null ||
			!double.TryParse(secondsText, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) || !double.IsFinite(seconds) || seconds <= 0 ||
			!int.TryParse(capacityText, out var capacity) || capacity is < 1 or > 128 ||
			!double.TryParse(litresText, NumberStyles.Float, CultureInfo.InvariantCulture, out var litres) || !double.IsFinite(litres) ||
			kind == SpellShelterKind.SpringHaven && (water is null || liquid is null || litres <= 0) || !command.IsFinished)
		{
			actor.OutputHandler.Send(($"Use #3magic spell edit new stock {name} <school> <casting skill> <resource> <indoor template room> <source terrain> <fallback room> <seconds per grade> <capacity>#0" +
				(kind == SpellShelterKind.SpringHaven ? " #3<immovable liquid-container prototype> <liquid> <litres per grade>#0" : "") +
				". Quote names containing spaces. All bindings and timings are explicit native policy.").SubstituteANSIColour());
			return true;
		}
		try
		{
			var spell = ArmageddonShelterStock.Create(actor.Gameworld, school, trait, resource,
				new(kind, template.Id, [terrain.Id], fallback.Id, seconds, capacity, water?.Id ?? 0, liquid?.Id ?? 0, litres));
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add it to the intended casting capability. " +
				"Edit the createshelter effect to change source terrains, template, fallback, capacity or lifetime. Native admission is checked before payment.");
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
		return true;
	}
}
