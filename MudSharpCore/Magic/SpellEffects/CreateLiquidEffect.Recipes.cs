#nullable enable
using System.Xml.Linq;
using MudSharp.Form.Material;
namespace MudSharp.Magic.SpellEffects;

public partial class CreateLiquidEffect : IMagicSpellEffectPreparedSelection
{
	private sealed record LiquidRecipe(long Predicate, long Liquid);
	private readonly SortedDictionary<int, LiquidRecipe> _recipes = new();
	private XElement? _unreadableRecipes;
	private string? _recipeLoadError;
	private ILiquid? _preparedRecipe;
	private ProvisionProfilePolicy.ActorFrame? _recipeFrame;
	private string? _recipePreparedStamp;
	private int? _preparedRecipeOrder;
	private IPerceivable? _preparedRecipeRecipient;
	private sealed record RecipeSelection(ProvisionProfilePolicy.ActorFrame Frame, string Stamp, int Order,
		ILiquid Liquid, IPerceivable Recipient) : IMagicSpellEffectPreparedSelectionToken;
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_preparedRecipe is null) return null;
		if (_preparedRecipeRecipient is not null && !ReferenceEquals(_preparedRecipeRecipient, recipient))
			throw new InvalidOperationException("Recipes require a single unchanged recipient.");
		_preparedRecipeRecipient = recipient;
		return new RecipeSelection(_recipeFrame!, _recipePreparedStamp!, _preparedRecipeOrder!.Value, _preparedRecipe, recipient);
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = RecipeError();
		if (error is not null) return false;
		if (selection is not RecipeSelection token || !ReferenceEquals(token.Recipient, recipient) || !token.Frame.Matches(caster) || token.Stamp != RecipeStamp()) {
			error = "Recipe selection identity, recipient or configuration changed."; return false;
		}
		_preparedRecipe = token.Liquid; _recipeFrame = token.Frame; _recipePreparedStamp = token.Stamp;
		_preparedRecipeOrder = token.Order; _preparedRecipeRecipient = recipient; return true;
	}
	private string RecipeStamp() => SaveToXml() + ProvisionProfilePolicy.Stamp(Gameworld, _recipes.Values.Select(x => x.Predicate)) +
		string.Join(";", _recipes.Values.Select(x => x.Liquid).Select(id => {
			var liquid = Gameworld.Liquids.Get(id);
			return liquid is null ? $"{id}:missing" : $"{id}:{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(liquid)}:{liquid.WaterLitresPerLitre}:{liquid.AlcoholLitresPerLitre}:{liquid.FoodSatiatedHoursPerLitre}:{liquid.DrinkSatiatedHoursPerLitre}:{liquid.DraughtProg}:{liquid.CountsAsLiquid}:{liquid.FreshnessConfiguration}";
		}));
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The liquid recipe was not prepared for this recipient.";
		return _preparedRecipe is not null && ReferenceEquals(_preparedRecipeRecipient, recipient) && TryPrepareRecipe(caster, out error);
	}
	private ILiquid SelectedLiquid => _preparedRecipe ?? Liquid;
	private void LoadRecipes(XElement? root)
	{
		if (root is null) return;
		try {
			if ((string?)root.Attribute("version") != "1") throw new FormatException("Unsupported liquid recipe version.");
			if (!root.Elements("Recipe").Any()) throw new FormatException("Configured recipes cannot be empty.");
			foreach (var row in root.Elements("Recipe")) {
				var order = (int)row.Attribute("order")!; var predicate = (long)row.Attribute("predicate")!; var liquid = (long)row.Attribute("liquid")!;
				if (order is < 1 or > 32 || predicate < 0 || liquid <= 0 || !_recipes.TryAdd(order, new(predicate, liquid))) throw new FormatException("Recipes need unique orders 1-32, a predicate and a positive liquid ID.");
			}
		} catch (Exception error) when (error is FormatException or OverflowException or ArgumentNullException) {
			_recipeLoadError = error.Message; _unreadableRecipes = new(root); _recipes.Clear();
		}
	}
	private XElement? SaveRecipes() => _unreadableRecipes is not null ? new(_unreadableRecipes) : _recipes.Count == 0 ? null :
		new("Recipes", new XAttribute("version", 1), _recipes.Select(x => new XElement("Recipe", new XAttribute("order", x.Key),
			new XAttribute("predicate", x.Value.Predicate), new XAttribute("liquid", x.Value.Liquid))));
	private string? RecipeError() => _recipeLoadError ?? (_recipes.Count == 0 ? null : !ContainerOnly ? "Recipes require native container filling." :
		_recipes.Values.Last().Predicate != 0 || _recipes.Values.SkipLast(1).Any(x => x.Predicate == 0) ? "Recipes require an explicit always fallback last." :
		_recipes.Values.Select(x => ProvisionProfilePolicy.Error(Gameworld, x.Predicate)).FirstOrDefault(x => x is not null) ??
		(_recipes.Values.Any(x => Gameworld.Liquids.Get(x.Liquid) is null) ? "A configured recipe liquid is missing." : null));
	internal bool TryPrepareRecipe(ICharacter caster, out string? error)
	{
		error = RecipeError(); if (error is not null || _recipes.Count == 0) return error is null;
		if (_preparedRecipe is not null) {
			if (!_recipeFrame!.Matches(caster) || _recipePreparedStamp != RecipeStamp()) { error = "The liquid recipe context changed after admission."; return false; }
		}
		var frame = ProvisionProfilePolicy.ActorFrame.Capture(caster); var definition = RecipeStamp();
		foreach (var pair in _recipes.ToArray()) {
			var recipe = pair.Value;
			var predicate = Gameworld.FutureProgs.Get(recipe.Predicate);
			if (!ProvisionProfilePolicy.TryMatch(Gameworld, recipe.Predicate, caster, out var matches, out error)) return false;
			if (!frame.Matches(caster) || RecipeStamp() != definition || recipe.Predicate != 0 && !ReferenceEquals(predicate, Gameworld.FutureProgs.Get(recipe.Predicate)) || RecipeError() is not null) {
				error = "Liquid recipe callback changed actor or authored configuration during admission."; return false;
			}
			if (!matches) continue;
			if (_preparedRecipe is not null) {
				if (_preparedRecipeOrder != pair.Key) { error = "The first matching liquid recipe changed after admission."; return false; }
				return true;
			}
			_preparedRecipeOrder = pair.Key;
			_preparedRecipe = Gameworld.Liquids.Get(recipe.Liquid); _recipeFrame = frame; _recipePreparedStamp = definition; return true;
		}
		error = "No liquid recipe matched."; return false;
	}
	private bool BuildingCommandRecipe(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (!int.TryParse(command.PopSpeech(), out var order) || order is < 1 or > 32) { actor.OutputHandler.Send("Use recipe <order 1-32> <boolean(character) prog|always> <liquid>, or <order> remove."); return false; }
		var text = command.PopSpeech();
		if (text.EqualTo("remove")) { if (!command.IsFinished) return false; _recipes.Remove(order); }
		else {
			var prog = text.EqualTo("always") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
			var liquid = Gameworld.Liquids.GetByIdOrName(command.PopSpeech());
			if (!command.IsFinished || liquid is null || !text.EqualTo("always") && (prog is null || ProvisionProfilePolicy.Error(Gameworld, prog.Id) is not null || !prog.Compile())) { actor.OutputHandler.Send("Select a compiling boolean(character) predicate or always, and an existing liquid."); return false; }
			_recipes[order] = new(prog?.Id ?? 0, liquid.Id);
		}
		_recipeLoadError = null; _unreadableRecipes = null; _preparedRecipe = null; _recipeFrame = null; _preparedRecipeOrder = null; _preparedRecipeRecipient = null;
		Spell.Changed = true; actor.OutputHandler.Send("Recipe updated. First true predicate wins; configure an always fallback last. Validation: " + (RecipeError() ?? "Valid")); return true;
	}
}
