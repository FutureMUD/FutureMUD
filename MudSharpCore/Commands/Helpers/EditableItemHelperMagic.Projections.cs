#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static bool TryCreateProjectionStockSpell(ICharacter actor, StringStack commands)
	{
		var name = commands.PeekSpeech().ToLowerInvariant(); if (name is not ("sand-effigy" or "walking-shadow")) return false;
		commands.PopSpeech();
		var schoolText = commands.PopSpeech(); var skillText = commands.PopSpeech(); var resourceText = commands.PopSpeech(); var planeText = commands.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(schoolText); var skill = actor.Gameworld.Traits.GetByIdOrName(skillText);
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(resourceText); var plane = actor.Gameworld.Planes.GetByIdOrName(planeText);
		var kind = name == "sand-effigy" ? SpellProjectionKind.SandEffigy : SpellProjectionKind.WalkingShadow;
		var itemText = kind == SpellProjectionKind.SandEffigy ? commands.PopSpeech() : "";
		var item = kind == SpellProjectionKind.SandEffigy ? actor.Gameworld.ItemProtos.GetByIdOrName(itemText) : null;
		var secondsText = commands.PopSpeech(); var rangeText = kind == SpellProjectionKind.WalkingShadow ? commands.PopSpeech() : "0";
		var backlashText = commands.PopSpeech(); var energyText = commands.PopSpeech(); var doorsText = kind == SpellProjectionKind.WalkingShadow ? commands.PopSpeech() : "false";
		if (school is null || skill is null || resource is null || plane is null || kind == SpellProjectionKind.SandEffigy && item is null ||
			!double.TryParse(secondsText, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) || !int.TryParse(rangeText, out var range) ||
			!double.TryParse(backlashText, NumberStyles.Float, CultureInfo.InvariantCulture, out var backlash) || !double.TryParse(energyText, NumberStyles.Float, CultureInfo.InvariantCulture, out var energy) ||
			!bool.TryParse(doorsText, out var doors) || !commands.IsFinished)
		{
			actor.OutputHandler.Send(($"Use #3magic spell edit new stock {name} <school> <casting skill> <resource> <plane> " +
				(kind == SpellProjectionKind.SandEffigy ? "<plain holdable effigy prototype> <seconds per grade> " : "<seconds per grade> <range in room edges> ") +
				"<backlash damage> <energy per grade>" + (kind == SpellProjectionKind.WalkingShadow ? " <cross closed doors true|false>" : "") + "#0. Quote names with spaces; all policy values are explicit native authorship.").SubstituteANSIColour()); return true;
		}
		try
		{
			var spell = ArmageddonProjectionStock.Create(actor.Gameworld, school, skill, resource, new(kind, plane.Id, item?.Id ?? 0, seconds, range, backlash, doors), energy);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(); actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add it to the intended casting capability; edit createprojection for native policy.");
		}
		catch (InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
		return true;
	}
}
