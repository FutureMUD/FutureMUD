#nullable enable
using MudSharp.Effects.Concrete;
using MudSharp.Magic;
namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private const string ProvisionStockNames = "#3sustain-meal#0 and #3draw-wine#0";
	private static bool TryCreateProvisionStockSpell(ICharacter actor, StringStack command)
	{
		var stock = command.PeekSpeech().ToLowerInvariant();
		if (stock is not ("sustain-meal" or "draw-wine")) return false;
		command.PopSpeech();
		var world = actor.Gameworld;
		var school = world.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait = world.Traits.GetByIdOrName(command.PopSpeech());
		var resource = world.MagicResources.GetByIdOrName(command.PopSpeech());
		var foods = new List<MudSharp.GameItems.IGameItemProto?>();
		if (stock == "sustain-meal") {
			foods.Add(world.ItemProtos.GetByIdOrName(command.PopSpeech()));
			foods.Add(world.ItemProtos.GetByIdOrName(command.PopSpeech()));
			foods.Add(world.ItemProtos.GetByIdOrName(command.PopSpeech()));
		}
		var wine = stock == "draw-wine" ? world.Liquids.GetByIdOrName(command.PopSpeech()) : null;
		var planeText = stock == "draw-wine" ? command.PopSpeech() : "none";
		var plane = planeText.EqualTo("none") ? null : world.Planes.GetByIdOrName(planeText);
		if (school is null || trait is null || resource is null || !command.IsFinished ||
			stock == "sustain-meal" && foods.Any(x => x is null) || stock == "draw-wine" && (wine is null || !planeText.EqualTo("none") && plane is null))
		{
			actor.OutputHandler.Send(("Use #3magic spell edit new stock " + stock + " <school> <casting skill> <resource> " +
				(stock == "sustain-meal" ? "<three approved plain food prototypes>" : "<wine liquid> <bonus plane|none>") +
				"#0. Quote names containing spaces.").SubstituteANSIColour());
			return true;
		}
		try
		{
			var spell = stock == "sustain-meal" ? ArmageddonSustainMealStock.Create(world, school, trait, resource, foods.Select(x => x!).ToArray()) :
				ArmageddonDrawWineStock.Create(world, school, trait, resource, wine!, plane);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add the intended casting-capability entry with opening 30, cap 90 and relative grades. Acquisition is separate. The description records recovered rules and native adaptations.");
		}
		catch (InvalidOperationException error) { actor.OutputHandler.Send(error.Message.ColourError()); }
		return true;
	}
}
