using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.FutureProg.Variables;

#nullable enable
namespace MudSharp.FutureProg.Functions.Magic;

/// <summary>Typed adapters use the same guarded service as commands. No ledger is created by a query.</summary>
internal sealed class VancianFunction : BuiltInFunction
{
	public sealed record Contract(string Name, ProgVariableTypes ReturnType, ProgVariableTypes[] Parameters, string Help);
	private readonly IFuturemud _gameworld;
	private readonly Contract _contract;
	public VancianFunction(IList<IFunction> parameters, IFuturemud gameworld, Contract contract) : base(parameters) { _gameworld = gameworld; _contract = contract; }
	public override ProgVariableTypes ReturnType { get => _contract.ReturnType; protected set { } }
	private static readonly ProgVariableTypes C = ProgVariableTypes.Character, K = ProgVariableTypes.MagicCapability,
		S = ProgVariableTypes.MagicSpell, N = ProgVariableTypes.Number, B = ProgVariableTypes.Boolean, T = ProgVariableTypes.Text,
		I = ProgVariableTypes.Item, SC = ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection;
	public static IReadOnlyList<Contract> Contracts { get; } = new Contract[]
	{
		new("vanciancasterlevel", N, [C,K], "Effective caster level from the canonical identity's configured pure progression policy."),
		new("vancianknownspells", SC, [C,K], "Distinct committed selections, including suspended selections; does not grant access or capacity."),
		new("vancianknownspells", SC, [C,K,T], "Distinct committed selections in one repertoire alias or stable key, including suspended selections."),
		new("vanciancandidates", SC, [C,K,T], "Permitted ordinary-cast candidates in one repertoire. Candidate policies must be pure and must not call this query recursively."),
		new("vancianpreparedspells", SC, [C,K], "One spell per usable unspent memorised copy. Duplicate spells are intentional; reserved, spent and suspended copies are omitted."),
		new("vancianslotsremaining", N, [C,K,T], "Usable unspent finite copies/slots. AtWill returns the sentinel zero: use vancianisatwill to distinguish it."),
		new("vancianslotcapacity", N, [C,K,T], "Current finite configured capacity, regardless of spent entries. It does not initialise new slots. AtWill returns zero."),
		new("vancianisatwill", B, [C,K,T], "Whether the named allowance is AtWill; no fictitious finite capacity is returned."),
		new("vancianselectedloadout", T, [C,K], "Selected next saved plan name, or empty text. This is independent of the last committed pattern."),
		new("vanciancanrefresh", B, [C,K], "Pure validation of the selected plan, current book access, recovery and permission conditions."),
		new("vanciancancast", B, [C,K,S,T,T], "Whether an eligible current route has a usable prepared/spontaneous/at-will casting. Does not resolve targets or pay costs."),
		new("spellbookspells", SC, [I], "Structured formulae on this item instance; no prose parsing or prototype defaults."),
		new("scrollspell", S, [I], "Logical source spell for an intact usable charge, or null. Numeric potency belongs to the stored snapshot."),
		new("scrollcastinglevel", N, [I], "Stored casting level; zero for blank/spent/invalid items. Check scrollischarged to distinguish a real level-zero charge."),
		new("scrollpower", N, [I], "Stored SpellPower numeric value; zero for blank/spent/invalid items."),
		new("scrollischarged", B, [I], "True only for an intact, compatible, unconsumed stored charge."),
		new("setvancianknownspells", B, [C,K,S | ProgVariableTypes.CollectionDictionary], "Commit the whole Selected repertoire, keyed by rule alias/stable key. Canonical-owner permission and post-change hooks run once; no automatic capacity grant."),
		new("selectvancianloadout", B, [C,K,T], "Select an existing saved plan for the next refresh; does not refill slots."),
		new("refreshvancian", B, [C,K], "Request the configured refresh. True means accepted/started, not necessarily completed. All normal recovery and policy rules apply."),
		new("copyspellformula", B, [C,K,I,S,I], "Begin timed copying. True means started; destination owns time/material costs. Scroll sources are consumed at commitment, with no effects or activation check."),
		new("inscribespellscroll", B, [C,K,T,T,S,N,I], "Begin timed finite inscription using a positive slot ordinal; true means started. Actual actor pays and supplies potency; identity owns the reserved slot."),
		new("inscribeatwillspellscroll", B, [C,K,T,T,S,I], "Begin timed inscription through an explicit AtWill allowance, with normal production/spell costs and no finite slot debit."),
		new("begininscribespellscroll", T, [C,K,T,T,S,N,I], "Begin timed finite inscription and return the engine-issued reservation token, or empty text on refusal."),
		new("vancianlastoperation", T, [C,K], "Most recent operation token for this canonical owner/capability, or empty text."),
		new("vancianoperationstatus", T, [C,T], "Status of an operation owned by the actor's identity, or empty text. Pending/Invoking/NeedsReview are never automatically replayed."),
		new("completevancianwriting", B, [C,T], "Trusted crafting completion using a live engine-issued reservation token. Revalidates elapsed time, actor, state, item and all debits; no prepaid boolean bypass exists."),
		new("cancelvancianwriting", B, [C,T], "Cancel a live precommit writing reservation owned by this actor. Committed costs cannot be refunded.")
	};
	public static void RegisterFunctionCompiler()
	{
		foreach (var contract in Contracts)
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(contract.Name, contract.Parameters,
				(parameters, game) => new VancianFunction(parameters, game, contract), contract.Parameters.Select((_, i) => $"argument{i + 1}").ToArray(),
				contract.Parameters.Select(x => x.Describe()).ToArray(), contract.Help, "Vancian Magic", contract.ReturnType));
	}
	private object? Value(int index) => ParameterFunctions[index].Result?.GetObject;
	private string Text(int index) => Value(index)?.ToString() ?? "";
	private TArg Required<TArg>(int index) where TArg : class => Value(index) as TArg ?? throw new InvalidOperationException($"Argument {index + 1} must be a non-null {typeof(TArg).Name}.");
	private static Guid Rule(IVancianMagicCapability capability, string alias) => capability.Repertoires.FirstOrDefault(x => x.Alias.EqualTo(alias) || x.Key.ToString().EqualTo(alias))?.Key ?? throw new InvalidOperationException("Unknown repertoire alias/key.");
	private static VancianCastingAllowanceDefinition Allowance(IVancianMagicCapability capability, string alias) => capability.Allowances.FirstOrDefault(x => x.Alias.EqualTo(alias) || x.Key.ToString().EqualTo(alias)) ?? throw new InvalidOperationException("Unknown allowance alias/key.");
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		try
		{
			var service = VancianMagicService.For(_gameworld); var name = _contract.Name;
			if (_contract.Parameters[0] == I) { ExecuteItem(service, name, Required<IGameItem>(0)); return StatementResult.Normal; }
			var actor = Required<ICharacter>(0);
			if (name is "completevancianwriting" or "cancelvancianwriting" or "vancianoperationstatus")
			{
				if (!Guid.TryParse(Text(1), out var token)) throw new InvalidOperationException("Invalid operation token.");
				if (name == "vancianoperationstatus") { var operation = service.Store.Operation(token); Result = new TextVariable(operation?.OwnerId == VancianPolicy.Owner(actor).Id ? operation.Status : ""); }
				else Result = new BooleanVariable((name == "completevancianwriting" ? service.CompleteWriting(actor, token) : service.CancelWriting(actor, token)).Success);
				return StatementResult.Normal;
			}
			var capability = Required<IVancianMagicCapability>(1);
			if (service.AccessError(actor, capability) is { } access) throw new InvalidOperationException(access);
			switch (name)
			{
				case "vanciancasterlevel": Result = new NumberVariable(service.CasterLevel(actor, capability)); break;
				case "vancianknownspells": Result = new CollectionVariable(service.KnownSpells(actor, capability, ParameterFunctions.Count == 3 ? Rule(capability, Text(2)) : null).ToList(), S); break;
				case "vanciancandidates": Result = new CollectionVariable(service.Candidates(actor, capability, Rule(capability, Text(2))).ToList(), S); break;
				case "vancianpreparedspells": Result = new CollectionVariable(service.Slots(actor, capability).Where(x => x.Status == VancianSlotStatus.Prepared && x.Preparation is not null).Select(x => _gameworld.MagicSpells.Get(x.Preparation!.SpellId)).Where(x => x is not null).ToList(), S); break;
				case "vancianslotcapacity": Result = new NumberVariable(service.Capacity(actor, capability, Allowance(capability, Text(2)))); break;
				case "vancianslotsremaining":
					var allowance = Allowance(capability, Text(2)); Result = new NumberVariable(allowance.Mode == VancianAllowanceMode.AtWill ? 0 : service.Slots(actor, capability).Count(x => x.AllowanceKey == allowance.Key && x.Status is VancianSlotStatus.Prepared or VancianSlotStatus.AvailableSpontaneous)); break;
				case "vancianisatwill": Result = new BooleanVariable(Allowance(capability, Text(2)).Mode == VancianAllowanceMode.AtWill); break;
				case "vancianselectedloadout": var state = service.State(actor, capability); Result = new TextVariable(state.Loadouts.FirstOrDefault(x => x.Id == state.SelectedLoadout)?.Name ?? ""); break;
				case "vanciancanrefresh": Result = new BooleanVariable(service.CanRefresh(actor, capability)); break;
				case "vanciancancast": Result = new BooleanVariable(service.CanCast(actor, capability, Rule(capability, Text(3)), Allowance(capability, Text(4)).Key, Required<IMagicSpell>(2)).Available); break;
				case "setvancianknownspells":
					var values = Required<CollectionDictionary<string, IProgVariable>>(2);
					var selections = values.ToDictionary(x => Rule(capability, x.Key), x => (IReadOnlyList<long>)x.Value.Select(v => (v.GetObject as IMagicSpell ?? throw new InvalidOperationException("Selections must contain non-null spells.")).Id).ToArray());
					Result = new BooleanVariable(service.CommitKnown(actor, capability, service.State(actor, capability).Version, selections).Success); break;
				case "selectvancianloadout": Result = new BooleanVariable(service.SelectLoadout(actor, capability, Text(2)).Success); break;
				case "refreshvancian": Result = new BooleanVariable(service.RequestRefresh(actor, capability).Success); break;
				case "copyspellformula": Result = new BooleanVariable(service.BeginTranscription(actor, capability, Required<IGameItem>(2), Required<IMagicSpell>(3), Required<IGameItem>(4)).Success); break;
				case "inscribespellscroll": case "inscribeatwillspellscroll": case "begininscribespellscroll":
					var castingAllowance = Allowance(capability, Text(3)); var atWill = name == "inscribeatwillspellscroll";
					if (atWill != (castingAllowance.Mode == VancianAllowanceMode.AtWill)) throw new InvalidOperationException("Use the explicit at-will overload only for an AtWill allowance.");
					int? ordinal = null;
					if (!atWill) { var number = Convert.ToDecimal(Value(5)); if (number != decimal.Floor(number) || number < 1 || number > int.MaxValue) throw new InvalidOperationException("A positive integral finite slot ordinal is required."); ordinal = (int)number; }
					var result = service.BeginInscription(actor, capability, Rule(capability, Text(2)), castingAllowance.Key, Required<IMagicSpell>(4), ordinal, Required<IGameItem>(atWill ? 5 : 6));
					Result = name == "begininscribespellscroll" ? new TextVariable(result.Success ? result.OperationId?.ToString() ?? "" : "") : new BooleanVariable(result.Success); break;
				case "vancianlastoperation": Result = new TextVariable(service.Store.Operations(VancianPolicy.Owner(actor).Id, capability.Id).FirstOrDefault()?.Id.ToString() ?? ""); break;
				default: throw new InvalidOperationException("Unknown Vancian function contract.");
			}
			return StatementResult.Normal;
		}
		catch (Exception ex) { ErrorMessage = ex.Message; return StatementResult.Error; }
	}
	private void ExecuteItem(VancianMagicService service, string name, IGameItem item)
	{
		if (name == "spellbookspells") { Result = new CollectionVariable(item.GetItemType<ISpellbook>() is { DataError: null } book ? book.Formulae.Select(x => _gameworld.MagicSpells.Get(x.SpellId)).Where(x => x is not null).ToList() : [], S); return; }
		var scroll = item.GetItemType<ISpellScroll>() as SpellScrollGameItemComponent;
		var usable = scroll?.IsCharged == true && scroll.Reservation is null && !service.Store.ItemConsumed(item.Id, scroll.ChargeId!.Value);
		if (usable) try { _ = scroll!.Snapshot!.CreateSpell(_gameworld); } catch { usable = false; }
		Result = name switch
		{
			"scrollischarged" => new BooleanVariable(usable),
			"scrollcastinglevel" => new NumberVariable(usable ? scroll!.CastingLevel!.Value : 0),
			"scrollpower" => new NumberVariable(usable ? (int)scroll!.StoredPower!.Value : 0),
			"scrollspell" => usable ? _gameworld.MagicSpells.Get(scroll!.SpellId!.Value) : new NullVariable(S),
			_ => throw new InvalidOperationException("Unknown item query.")
		};
	}
}
