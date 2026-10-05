#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private const string WaterBreathingStockName = "#3water-breathing#0";

	private static bool TryCreateWaterBreathingStockSpell(ICharacter actor, StringStack command)
	{
		if (!command.PeekSpeech().EqualTo("water-breathing")) return false;
		command.PopSpeech();
		var world = actor.Gameworld;
		var school = world.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait = world.Traits.GetByIdOrName(command.PopSpeech());
		var resource = world.MagicResources.GetByIdOrName(command.PopSpeech());
		var water = new List<ILiquid>();
		var invalid = false;
		while (!command.IsFinished)
		{
			var text = command.PopSpeech();
			var liquid = world.Liquids.GetByIdOrName(text);
			if (liquid is null || water.Count >= 128) invalid = true;
			else water.Add(liquid);
		}
		if (school is null || trait is null || resource is null || invalid || water.Count == 0)
		{
			actor.OutputHandler.Send("Use #3magic spell edit new stock water-breathing <school> <casting skill> <resource> <water liquid> [other water liquids]#0. Explicitly select every compatible native water definition; quote names containing spaces.".SubstituteANSIColour());
			return true;
		}
		try
		{
			var spell = ArmageddonWaterBreathingStock.Create(world, school, trait, resource, water);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add capability admission with opening 30, cap 90 and relative grades, and Draw Wine raw80 as its prerequisite. Acquisition is separate. Its water mappings, minimum-Standing filter and lifetime remain editable.");
		}
		catch (InvalidOperationException error) { actor.OutputHandler.Send(error.Message.ColourError()); }
		return true;
	}
}
