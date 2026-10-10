#nullable enable

using System.Globalization;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private static bool TryCreatePocketStockSpell(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("folded-pocket")) return false;
		command.PopSpeech();
		var school = actor.Gameworld.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait = actor.Gameworld.Traits.GetByIdOrName(command.PopSpeech());
		var resource = actor.Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
		var prototype = actor.Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
		var capacityText = command.PopSpeech(); var sizeText = command.PopSpeech(); var secondsText = command.PopSpeech();
		var accessText = command.PopSpeech(); var fallback = actor.Gameworld.Rooms.GetByIdOrName(command.PopSpeech());
		if (school is null || trait is null || resource is null || prototype is null || fallback is null || !command.IsFinished ||
			!actor.Gameworld.UnitManager.TryGetBaseUnits(capacityText, UnitType.Mass, actor, out var capacity) ||
			!sizeText.TryParseEnum<SizeCategory>(out var size) || !accessText.TryParseEnum<SpellPocketAccess>(out var access) ||
			!double.TryParse(secondsText, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock folded-pocket <school> <casting skill> <resource> <approved pocket prototype> <mass per grade> <maximum size> <seconds per grade> Bearer|Creator <fallback room>#0. Quote multi-word values.".SubstituteANSIColour()); return true;
		}
		try
		{
			var c = new SpellPocketConfiguration(prototype.Id, capacity, size, seconds, access, fallback.Id);
			if (SpellOwnedPocketService.PrototypeError(prototype, actor.Gameworld) is { } error) throw new InvalidOperationException(error);
			var spell = ArmageddonUtilityStock.Create(actor.Gameworld, school, trait, resource, ArmageddonPocketContent.Create(c));
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(); actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Configure capability membership separately. Cast on an ordinary held item; it remains borrowed while a new finite carrier is created.");
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { actor.OutputHandler.Send(ex.Message.ColourError()); }
		return true;
	}
}
