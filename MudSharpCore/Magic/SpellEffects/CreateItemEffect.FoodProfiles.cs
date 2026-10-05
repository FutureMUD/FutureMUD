#nullable enable
using System.Xml.Linq;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
namespace MudSharp.Magic.SpellEffects;

public partial class CreateItemEffect : IMagicSpellEffectPreparedSelection
{
	private sealed record FoodProfile(long Predicate, long[] Prototypes);
	private readonly SortedDictionary<int, FoodProfile> _foodProfiles = new();
	private XElement? _unreadableFoodProfiles;
	private string? _foodProfileLoadError;
	private IGameItemProto[]? _preparedFoodOutputs;
	private ProvisionProfilePolicy.ActorFrame? _foodProfileFrame;
	private string? _foodProfilePreparedStamp;
	private int? _preparedFoodProfileOrder;
	/// <summary>Per-effect random source; revalidation of an admitted choice never calls it.</summary>
	protected virtual Random FoodProfileRandom => Random.Shared;
	private IPerceivable? _preparedFoodRecipient;
	private sealed record FoodSelection(ProvisionProfilePolicy.ActorFrame Frame, string Stamp, int Order,
		System.Collections.ObjectModel.ReadOnlyCollection<IGameItemProto> Outputs, IPerceivable Recipient) : IMagicSpellEffectPreparedSelectionToken;
	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_preparedFoodOutputs is null) return null;
		if (_preparedFoodRecipient is not null && !ReferenceEquals(_preparedFoodRecipient, recipient))
			throw new InvalidOperationException("Food selections require a single unchanged recipient.");
		_preparedFoodRecipient = recipient;
		return new FoodSelection(_foodProfileFrame!, _foodProfilePreparedStamp!, _preparedFoodProfileOrder!.Value,
			Array.AsReadOnly((IGameItemProto[])_preparedFoodOutputs.Clone()), recipient);
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = FoodProfileError();
		if (error is not null) return false;
		if (selection is not FoodSelection token || !ReferenceEquals(token.Recipient, recipient) || !token.Frame.Matches(caster) ||
			token.Stamp != FoodProfileStamp() || Spell is not MagicSpell { InvocationGrade: { } grade } || token.Outputs.Count != grade) {
			error = "Food selection identity, recipient, grade or configuration changed."; return false;
		}
		_preparedFoodOutputs = token.Outputs.ToArray(); _foodProfileFrame = token.Frame; _foodProfilePreparedStamp = token.Stamp;
		_preparedFoodProfileOrder = token.Order; _preparedFoodRecipient = recipient; return true;
	}
	private string FoodProfileStamp() => SaveToXml() + ProvisionProfilePolicy.Stamp(Gameworld, _foodProfiles.Values.Select(x => x.Predicate).Concat([_eligibilityProgId, _lifetimeMultiplierProgId])) +
		string.Join(";", _foodProfiles.Values.SelectMany(x => x.Prototypes).Select(id => {
			var item = Gameworld.ItemProtos.Get(id);
			var food = item?.GetItemType<FoodGameItemComponentProto>(); return item is null ? $"{id}:missing" : $"{id}:{System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(item)}:{item.RevisionNumber}:{item.Status}:{item.BaseItemQuality}:{item.Weight}:{item.Size}:{food?.Bites}:{food?.SatiationPoints}:{food?.WaterLitres}:{food?.ThirstPoints}:{food?.AlcoholLitres}:{food?.TasteString}:{food?.Decorator}";
		}));
	private void LoadFoodProfiles(XElement? root)
	{
		if (root is null) return;
		try {
			if ((string?)root.Attribute("version") != "1") throw new FormatException("Unsupported food profile version.");
			if (!root.Elements("Profile").Any()) throw new FormatException("Configured food profiles cannot be empty.");
			foreach (var row in root.Elements("Profile")) {
				var order = (int)row.Attribute("order")!; var predicate = (long)row.Attribute("predicate")!;
				var ids = row.Elements("Prototype").Select(x => (long)x).ToArray();
				if (order is < 1 or > 32 || predicate < 0 || ids.Length is < 1 or > 32 || ids.Any(x => x <= 0) ||
					ids.Distinct().Count() != ids.Length || !_foodProfiles.TryAdd(order, new(predicate, ids)))
					throw new FormatException("Food profiles need unique orders 1-32, a predicate and distinct positive prototype IDs.");
			}
		} catch (Exception error) when (error is FormatException or OverflowException or ArgumentNullException) {
			_foodProfileLoadError = error.Message; _unreadableFoodProfiles = new(root); _foodProfiles.Clear();
		}
	}
	private XElement? SaveFoodProfiles() => _unreadableFoodProfiles is not null ? new(_unreadableFoodProfiles) : _foodProfiles.Count == 0 ? null :
		new("FoodProfiles", new XAttribute("version", 1), _foodProfiles.Select(x => new XElement("Profile",
			new XAttribute("order", x.Key), new XAttribute("predicate", x.Value.Predicate), x.Value.Prototypes.Select(id => new XElement("Prototype", id)))));
	private string? FoodProfileError() => _foodProfileLoadError ?? (_foodProfiles.Count == 0 ? null :
		!CountByGrade || WornLight || PrimaryHand || PermanentGrade is not null || _gradeOutputs.Count != 0 ? "Food profiles require grade-count plain food without other output overrides." :
		_foodProfiles.Values.Last().Predicate != 0 || _foodProfiles.Values.SkipLast(1).Any(x => x.Predicate == 0) ? "Food profiles require an explicit always fallback last." :
		_foodProfiles.Values.Select(x => ProvisionProfilePolicy.Error(Gameworld, x.Predicate)).FirstOrDefault(x => x is not null) ??
		_foodProfiles.Values.SelectMany(x => x.Prototypes).Select(id => Gameworld.ItemProtos.Get(id)).Select(x =>
			NativeItemCreationEligibility.Error(x, Gameworld) ?? (x!.GetItemType<FoodGameItemComponentProto>() is null ? "Every food pool member must be plain edible food." : null)).FirstOrDefault(x => x is not null));
	internal bool TryPrepareFoodOutputs(ICharacter caster, int grade, Random random, out IGameItemProto[]? outputs, out string? error)
	{
		outputs = null; error = FoodProfileError();
		if (error is not null || _foodProfiles.Count == 0) return error is null;
		if (_preparedFoodOutputs is not null) {
			if (!_foodProfileFrame!.Matches(caster) || _foodProfilePreparedStamp != FoodProfileStamp()) { error = "The food profile context changed after admission."; return false; }
			if (_preparedFoodOutputs.Length != grade) { error = "The food grade changed after admission."; return false; }
		}
		var frame = ProvisionProfilePolicy.ActorFrame.Capture(caster); var definition = FoodProfileStamp();
		foreach (var pair in _foodProfiles.ToArray()) {
			var profile = pair.Value;
			var predicate = Gameworld.FutureProgs.Get(profile.Predicate);
			if (!ProvisionProfilePolicy.TryMatch(Gameworld, profile.Predicate, caster, out var matches, out error)) return false;
			if (!frame.Matches(caster) || FoodProfileStamp() != definition ||
				profile.Predicate != 0 && !ReferenceEquals(predicate, Gameworld.FutureProgs.Get(profile.Predicate)) || FoodProfileError() is not null) {
				error = "Food profile callback changed actor or authored configuration during admission."; return false;
			}
			if (!matches) continue;
			if (_preparedFoodOutputs is not null) {
				if (_preparedFoodProfileOrder != pair.Key) { error = "The first matching food profile changed after admission."; return false; }
				outputs = _preparedFoodOutputs; return true;
			}
			_preparedFoodProfileOrder = pair.Key;
			var pool = profile.Prototypes.Select(id => Gameworld.ItemProtos.Get(id)).ToArray();
			_preparedFoodOutputs = Enumerable.Range(0, grade).Select(_ => pool[random.Next(pool.Length)]).ToArray();
			_foodProfileFrame = frame; _foodProfilePreparedStamp = definition; outputs = _preparedFoodOutputs; return true;
		}
		error = "No food profile matched."; return false;
	}
	private bool BuildingCommandFoodProfile(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		if (!int.TryParse(command.PopSpeech(), out var order) || order is < 1 or > 32) { actor.OutputHandler.Send("Use foodprofile <order 1-32> <boolean(character) prog|always> <food prototypes...>, or <order> remove."); return false; }
		var text = command.PopSpeech();
		if (text.EqualTo("remove")) { if (!command.IsFinished) return false; _foodProfiles.Remove(order); }
		else {
			var prog = text.EqualTo("always") ? null : Gameworld.FutureProgs.GetByIdOrName(text);
			if (!text.EqualTo("always") && (prog is null || ProvisionProfilePolicy.Error(Gameworld, prog.Id) is not null || !prog.Compile())) { actor.OutputHandler.Send("Select a compiling boolean(character) predicate or always."); return false; }
			var ids = new List<long>();
			while (!command.IsFinished) {
				var item = Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
				if (NativeItemCreationEligibility.Error(item, Gameworld) is not null || item!.GetItemType<FoodGameItemComponentProto>() is null) { actor.OutputHandler.Send("Every member must be approved plain food."); return false; }
				ids.Add(item.Id);
			}
			if (ids.Count is < 1 or > 32 || ids.Distinct().Count() != ids.Count) return false;
			_foodProfiles[order] = new(prog?.Id ?? 0, ids.ToArray());
		}
		_foodProfileLoadError = null; _unreadableFoodProfiles = null; _preparedFoodOutputs = null; _foodProfileFrame = null; _preparedFoodProfileOrder = null; _preparedFoodRecipient = null;
		Spell.Changed = true; actor.OutputHandler.Send("Food profile updated. First true predicate wins; configure an always fallback last. Validation: " + (FoodProfileError() ?? "Valid")); return true;
	}
}
