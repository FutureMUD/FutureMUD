#nullable enable
using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	private const string UtilityStockNames = "#3sense-enchantment#0, #3unravel-enchantment#0, #3mend-flesh#0, #3draw-water#0 and #3hovering-light#0";
	private static bool TryCreateUtilityStockSpell(ICharacter actor, StringStack command)
	{
		var stock=command.PeekSpeech().ToLowerInvariant();
		if(stock is not ("sense-enchantment" or "unravel-enchantment" or "mend-flesh" or "draw-water" or "hovering-light")) return false;
		command.PopSpeech(); var world=actor.Gameworld;
		var school=world.MagicSchools.GetByIdOrName(command.PopSpeech());
		var trait=world.Traits.GetByIdOrName(command.PopSpeech());
		var resource=world.MagicResources.GetByIdOrName(command.PopSpeech());
		var eligibility=stock=="mend-flesh" ? world.FutureProgs.GetByIdOrName(command.PopSpeech()) : null;
		var water=stock=="draw-water" ? world.Liquids.GetByIdOrName(command.PopSpeech()) : null;
		var planeText=stock=="draw-water" ? command.PopSpeech() : "none";
		var plane=planeText.EqualTo("none") ? null : world.Planes.GetByIdOrName(planeText);
		var light=stock=="hovering-light" ? world.ItemProtos.GetByIdOrName(command.PopSpeech()) : null;
		if(school is null || trait is null || resource is null || !command.IsFinished || stock=="mend-flesh" && eligibility is null ||
			stock=="draw-water" && (water is null || !planeText.EqualTo("none") && plane is null) || stock=="hovering-light" && light is null) {
			actor.OutputHandler.Send(("Use #3magic spell edit new stock "+stock+" <school> <casting skill> <resource>"+
				(stock=="mend-flesh" ? " <boolean (target, caster) eligibility prog>" : stock=="draw-water" ? " <clean water liquid> <bonus plane|none>" : stock=="hovering-light" ? " <approved wearable prog-light prototype>" : "")+
				"#0. Quote names containing spaces. Mend Flesh's eligibility prog must map your undead and Nilaz/defiler policy.").SubstituteANSIColour()); return true;
		}
		try {
			var spell=stock switch {
				"sense-enchantment"=>ArmageddonSenseEnchantmentStock.Create(world,school,trait,resource),
				"unravel-enchantment"=>ArmageddonUnravelEnchantmentStock.Create(world,school,trait,resource),
				"mend-flesh"=>ArmageddonMendFleshStock.Create(world,school,trait,resource,eligibility!),
				"draw-water"=>ArmageddonDrawWaterStock.Create(world,school,trait,resource,water!,plane),
				_=>ArmageddonHoveringLightStock.Create(world,school,trait,resource,light!) };
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(); actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor){EditingItem=spell});
			actor.OutputHandler.Send($"Created {spell.Name.ColourName()}; you are now editing it. Add the intended casting-capability entry with opening "+
				(stock is "sense-enchantment" or "unravel-enchantment" ? "60 and cap 90" : stock=="mend-flesh" ? "30 and cap 60" : "30 and cap 90")+
				", relative grades. Acquisition is separate. See the editable description for recovered rules and native adaptations.");
		} catch(InvalidOperationException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
		return true;
	}
}
