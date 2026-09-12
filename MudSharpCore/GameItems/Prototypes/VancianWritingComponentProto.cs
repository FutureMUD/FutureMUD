using ExpressionEngine;
using MudSharp.Accounts;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

#nullable enable
namespace MudSharp.GameItems.Prototypes;

/// <summary>Shared production authoring. Formulae and charges are never prototype fields.</summary>
public abstract class VancianWritingComponentProto : GameItemComponentProto
{
	private string? _definitionError;
	protected VancianWritingComponentProto(IFuturemud game, IAccount account, string type) : base(game, account, type) { }
	protected VancianWritingComponentProto(Models.GameItemComponentProto model, IFuturemud game) : base(model, game) { }
	public int Capacity { get; private set; } = 100;
	public long EligibilityProgId { get; private set; }
	public long UseProgId { get; private set; }
	public bool RequireReadable { get; private set; }
	public string DurationFormula { get; private set; } = "600";
	private IInventoryPlanTemplate? _plan;
	public IInventoryPlanTemplate ProductionPlan => _plan ??= new InventoryPlanTemplate(Gameworld, [new InventoryPlanPhaseTemplate(1, [])]);
	public string StartEmote { get; private set; } = "$0 begin|begins carefully writing a magical formula on $1.";
	public string CompleteEmote { get; private set; } = "$0 finish|finishes writing the magical formula on $1.";
	public string CancelEmote { get; private set; } = "$0 stop|stops working on $1.";
	public string ReleaseEmote { get; private set; } = "$0 release|releases the stored magic from $1, consuming it.";
	public string FizzleEmote { get; private set; } = "$1 crumble|crumbles away as $0 fail|fails to control its stored magic.";
	public TimeSpan Duration(int spellLevel, int castingLevel, int casterLevel)
	{
		var seconds = new Expression(DurationFormula).EvaluateDoubleWith(("spelllevel", spellLevel), ("castinglevel", castingLevel), ("casterlevel", casterLevel));
		if (!double.IsFinite(seconds) || seconds <= 0 || seconds > TimeSpan.MaxValue.TotalSeconds) throw new InvalidOperationException("Production duration must be finite and positive.");
		return TimeSpan.FromSeconds(seconds);
	}
	protected override void LoadFromXml(XElement root)
	{
		try
		{
		if ((int?)root.Attribute("version") != 1) throw new FormatException("Unsupported magical writing prototype version.");
		Capacity = (int?)root.Element("Capacity") ?? 100;
		EligibilityProgId = (long?)root.Element("Eligibility") ?? 0;
		UseProgId = (long?)root.Element("Usable") ?? 0;
		RequireReadable = (bool?)root.Element("RequireReadable") ?? false;
		DurationFormula = (string?)root.Element("Duration") ?? "600";
		_plan = root.Element("Plan") is { } plan ? new InventoryPlanTemplate(plan, Gameworld) : null;
		StartEmote = (string?)root.Element("Start") ?? StartEmote;
		CompleteEmote = (string?)root.Element("Complete") ?? CompleteEmote;
		CancelEmote = (string?)root.Element("Cancel") ?? CancelEmote;
		ReleaseEmote = (string?)root.Element("Release") ?? ReleaseEmote;
		FizzleEmote = (string?)root.Element("Fizzle") ?? FizzleEmote;
		}
		catch (Exception ex) { _definitionError = ex.Message; }
	}
	public IReadOnlyList<string> ConfigurationErrors()
	{
		var errors = new List<string>();
		if (_definitionError is not null) errors.Add(_definitionError);
		if (Capacity is < 0 or > 100_000) errors.Add("Formula capacity must be 0-100,000.");
		if (new Expression(DurationFormula).HasErrors()) errors.Add("The production duration expression is invalid.");
		foreach (var (id, name) in new[] { (UseProgId, "usable"), (EligibilityProgId, "eligibility") })
		{
			if (id == 0) continue;
			var prog = Gameworld.FutureProgs.Get(id);
			ProgVariableTypes[] signature = name == "usable" ? [ProgVariableTypes.Character, ProgVariableTypes.Item]
				: this is SpellbookGameItemComponentProto ? [ProgVariableTypes.Item, ProgVariableTypes.MagicSpell]
				: [ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Number];
			if (prog is null || prog.AcceptsAnyParameters || prog.ReturnType != ProgVariableTypes.Boolean || !prog.Parameters.SequenceEqual(signature) || !string.IsNullOrEmpty(prog.CompileError))
				errors.Add($"The {name} prog is missing, invalid or has an incompatible signature.");
		}
		return errors.AsReadOnly();
	}
	public override bool CanSubmit() => ConfigurationErrors().Count == 0;
	public override string WhyCannotSubmit() => string.Join("\n", ConfigurationErrors());
	protected override string SaveToXml() => _definitionError is not null ? throw new InvalidOperationException(_definitionError) : new XElement("Definition", new XAttribute("version", 1), new XElement("Capacity", Capacity),
		new XElement("Eligibility", EligibilityProgId), new XElement("Usable", UseProgId), new XElement("RequireReadable", RequireReadable),
		new XElement("Duration", DurationFormula), ProductionPlan.SaveToXml(), new XElement("Start", StartEmote),
		new XElement("Complete", CompleteEmote), new XElement("Cancel", CancelEmote), new XElement("Release", ReleaseEmote), new XElement("Fizzle", FizzleEmote)).ToString();
	public const string ProductionHelp = @"Settings:
	check - validate component configuration and referenced policies
	capacity <non-negative integer> - spellbook formula capacity; zero holds no formulae
	eligibility <prog|none> - book: boolean(item,spell); scroll: boolean(actor,spell,storedLevel)
	usable <prog|none> - boolean(actor,item) physical/readability policy
	readable <true|false> - require a readable component and comprehensible writing
	duration <expression> - elapsed seconds; spelllevel, castinglevel and casterlevel variables
	plan add held|wielded|inroom|consumed|consumedliquid ... - a normal inventory-plan action
	plan remove <ordinal> - remove an action
	start|complete|cancel|release|fizzle <emote> - $0 actor, $1 destination/scroll, $2 source when copying

Spellbook instances start empty. Scroll instances start blank. A charged scroll has one use.
Production plans add to the spell's own costs; inscription never executes spell effects.
Name and description use the standard component editor commands.";
	public override string ShowBuildingHelp => ProductionHelp;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var setting = command.PopForSwitch();
		try
		{
			if (setting == "check") { actor.OutputHandler.Send(string.Join("\n", ConfigurationErrors().DefaultIfEmpty("This component is ready."))); return true; }
			if (_definitionError is not null) throw new InvalidOperationException(_definitionError);
			switch (setting)
			{
				case "capacity":
					if (!int.TryParse(command.SafeRemainingArgument, out var count) || count < 0 || count > 100_000) throw new InvalidOperationException("Formula capacity must be 0-100,000.");
					Capacity = count; break;
				case "duration":
					var expression = new Expression(command.SafeRemainingArgument);
					if (expression.HasErrors()) throw new InvalidOperationException(expression.Error);
					DurationFormula = command.SafeRemainingArgument; break;
				case "readable": RequireReadable = bool.Parse(command.SafeRemainingArgument); break;
				case "eligibility": case "usable":
					var text = command.SafeRemainingArgument;
					var id = 0L;
					if (!text.EqualTo("none"))
					{
						var prog = Gameworld.FutureProgs.GetByIdOrName(text);
						ProgVariableTypes[] signature = setting == "usable" ? [ProgVariableTypes.Character, ProgVariableTypes.Item]
							: this is SpellbookGameItemComponentProto ? [ProgVariableTypes.Item, ProgVariableTypes.MagicSpell]
							: [ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Number];
						if (prog is null || prog.AcceptsAnyParameters || prog.ReturnType != ProgVariableTypes.Boolean || !prog.Parameters.SequenceEqual(signature) || !string.IsNullOrEmpty(prog.CompileError)) throw new InvalidOperationException($"Requires a compiled boolean prog ({string.Join(", ", signature.Select(x => x.Describe()))}).");
						id = prog.Id;
					}
					if (setting == "usable") UseProgId = id; else EligibilityProgId = id; break;
				case "plan":
					var op = command.PopForSwitch();
					if (op == "add")
					{
						var action = InventoryPlanTemplate.ParseActionFromBuilderInput(actor, command);
						if (action is null) return false;
						ProductionPlan.Phases.First().AddAction(action);
					}
					else if (op is "remove" or "delete" && int.TryParse(command.PopSpeech(), out var index) && index > 0 && index <= ProductionPlan.Phases.First().Actions.Count())
						ProductionPlan.Phases.First().RemoveAction(ProductionPlan.Phases.First().Actions.ElementAt(index - 1));
					else throw new InvalidOperationException("Use plan add <normal action> or plan remove <ordinal>.");
					break;
				case "start": case "complete": case "cancel": case "release": case "fizzle":
					var emoteText = command.SafeRemainingArgument;
					if (string.IsNullOrWhiteSpace(emoteText)) throw new InvalidOperationException("The emote must not be empty.");
					var emote = new Emote(emoteText, actor, actor, actor, actor);
					if (!emote.Valid) throw new InvalidOperationException(emote.ErrorMessage);
					switch (setting) { case "start": StartEmote = emoteText; break; case "complete": CompleteEmote = emoteText; break; case "cancel": CancelEmote = emoteText; break; case "release": ReleaseEmote = emoteText; break; case "fizzle": FizzleEmote = emoteText; break; }
					break;
				default: return base.BuildingCommand(actor, command.GetUndo());
			}
			Changed = true; actor.OutputHandler.Send("Magical writing component configuration updated."); return true;
		}
		catch (Exception ex) { actor.OutputHandler.Send(ex.Message); return false; }
	}
	public override string ComponentDescriptionOLC(ICharacter actor) =>
		string.Join("\n", ConfigurationErrors().Select(x => $"Disabled: {x}")) + "\n" +
		$"{TypeDescription} #{Id.ToString("N0", actor)}: capacity {Capacity.ToString("N0", actor)}, duration {DurationFormula}, eligibility #{EligibilityProgId}, usability #{UseProgId}, readable required {RequireReadable.ToColouredString()}\n" +
		string.Join("\n", ProductionPlan.Phases.SelectMany(x => x.Actions).Select((a, i) => $"{i + 1}. {a.Describe(actor)}")) +
		$"\nStart: {StartEmote}\nComplete: {CompleteEmote}\nCancel: {CancelEmote}\nRelease: {ReleaseEmote}\nFizzle: {FizzleEmote}";
}
