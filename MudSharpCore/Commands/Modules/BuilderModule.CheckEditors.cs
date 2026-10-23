#nullable enable
using System.Globalization;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Decorators;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using Microsoft.EntityFrameworkCore;
using CheckTemplateModel = MudSharp.Models.CheckTemplate;
using StackDecoratorRuntime = MudSharp.GameItems.Decorators.StackDecorator;
using CultureInfo = System.Globalization.CultureInfo;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.Commands.Modules;

internal partial class BuilderModule
{
	private sealed class CheckTemplateDraft
	{
		public long? Id { get; set; }
		public string Name { get; set; } = "";
		public string CheckMethod { get; set; } = "Standard";
		public string Definition { get; set; } = "<Definition />";
		public bool ImproveTraits { get; set; } = true;
		public short FailIfTraitMissingMode { get; set; }
		public bool CanBranchIfTraitMissing { get; set; }
		public Dictionary<int, double> Difficulties { get; } = new();
		public CheckTemplateDraft Copy()
		{
			var copy = new CheckTemplateDraft { Id = Id, Name = Name, CheckMethod = CheckMethod, Definition = Definition,
				ImproveTraits = ImproveTraits, FailIfTraitMissingMode = FailIfTraitMissingMode, CanBranchIfTraitMissing = CanBranchIfTraitMissing };
			foreach (var pair in Difficulties) copy.Difficulties[pair.Key] = pair.Value;
			return copy;
		}
	}

	private sealed class TraitDecoratorDraft
	{
		public long? Id { get; set; }
		public string Name { get; set; } = "";
		public string Type { get; set; } = "SimpleNumeric";
		public string Contents { get; set; } = "";
		public TraitDecoratorDraft Copy() => new() { Id = Id, Name = Name, Type = Type, Contents = Contents };
	}

	private sealed class StackDecoratorDraft
	{
		public long? Id { get; set; }
		public string Name { get; set; } = "";
		public string Type { get; set; } = "Suffix";
		public string Description { get; set; } = "";
		public string Definition { get; set; } = "<Definition />";
		public StackDecoratorDraft Copy() => new() { Id = Id, Name = Name, Type = Type, Description = Description, Definition = Definition };
	}

	private const string CheckTemplateEditorHelp = @"Syntax:
	#3checktemplate list [ +name ]#0
	#3checktemplate show <id|name>#0
	#3checktemplate new <name> [method]#0
	#3checktemplate clone <id|name> <new name>#0
	#3checktemplate edit <id|name>#0
	#3checktemplate set name <name> | method <method> | improve <true|false> | branch <true|false> | missing <policy>#0
	#3checktemplate set difficulty <difficulty> <modifier>#0
	#3checktemplate set minimum_time <seconds> | maximum_time <seconds>#0 (Timebound)
	#3checktemplate set core_expression <formula> | passive_fuzziness <0..1>#0 (PassivePerception)
	#3checktemplate set limb list|add <type> <minimum>|set <index> <type> <minimum>|remove <index>#0 (Limb)
	#3checktemplate set definition <xml>#0 (advanced override)
	#3checktemplate close#0
	#3checktemplate assign <template> <existing check type>#0

Edits are validated and saved immediately. Method-specific values are authored with typed fields. The advanced definition override accepts the engine XML shape. Timebound seconds must be non-negative and maximum_time must be at least minimum_time. PassivePerception requires a valid trait formula and fuzziness below 1. Limb entries use LimbType and a non-negative required count. Check definitions load into runtime caches at startup, so restart the server to activate changes. Check types are the existing CheckType enum values; this command does not create new check types.";

	[PlayerCommand("CheckTemplate", "checktemplate")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("checktemplate", CheckTemplateEditorHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void CheckTemplate(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var action = ss.PopForSwitch();
		switch (action)
		{
			case "list": CheckTemplateList(actor, ss); return;
			case "show": CheckTemplateShow(actor, ss); return;
			case "new": CheckTemplateNew(actor, ss); return;
			case "clone": CheckTemplateClone(actor, ss); return;
			case "edit": CheckTemplateEdit(actor, ss); return;
			case "set": CheckTemplateSet(actor, ss); return;
			case "assign": CheckTemplateAssign(actor, ss); return;
			case "close": actor.SetEditingItem<CheckTemplateDraft>(null); actor.OutputHandler.Send("You close the check template editor."); return;
			default: actor.OutputHandler.Send(CheckTemplateEditorHelp.SubstituteANSIColour()); return;
		}
	}

	private static List<CheckTemplateModel> GetCheckTemplates()
	{
		using var db = new FMDB();
		return FMDB.Context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).OrderBy(x => x.Id).ToList();
	}

	private static CheckTemplateModel? FindCheckTemplate(string selector)
	{
		var templates = GetCheckTemplates();
		return long.TryParse(selector, out var id)
			? templates.FirstOrDefault(x => x.Id == id)
			: templates.FirstOrDefault(x => x.Name.EqualTo(selector));
	}

	private static void CheckTemplateList(ICharacter actor, StringStack ss)
	{
		var templates = GetCheckTemplates();
		while (!ss.IsFinished)
		{
			var filter = ss.PopSpeech();
			if (!filter.StartsWith('+') || filter.Length < 2)
			{
				actor.OutputHandler.Send($"Invalid filter {filter.ColourCommand()}. Use +keyword.");
				return;
			}
			var keyword = filter[1..];
			templates = templates.Where(x => x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
				x.CheckMethod.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
		}
		actor.OutputHandler.Send(StringUtilities.GetTextTable(templates.Select(x => new[]
			{ x.Id.ToString("N0", actor), x.Name, x.CheckMethod, x.CheckTemplateDifficulties.Count.ToString(actor) }),
			new[] { "Id", "Name", "Method", "Difficulties" }, actor.LineFormatLength, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void CheckTemplateShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which check template?"); return; }
		var template = FindCheckTemplate(ss.SafeRemainingArgument);
		if (template is null) { actor.OutputHandler.Send("There is no such check template."); return; }
		var uses = GetCheckTemplateUses(template.Id);
		var output = new System.Text.StringBuilder();
		output.AppendLine($"Check Template #{template.Id.ToString("N0", actor)}: {template.Name.ColourName()}");
		output.AppendLine($"Method: {template.CheckMethod.ColourValue()}");
		output.AppendLine($"Improve traits: {template.ImproveTraits.ToColouredString()}");
		output.AppendLine($"Missing trait policy: {((FailIfTraitMissingType)template.FailIfTraitMissingMode).DescribeEnum()}");
		output.AppendLine($"Branch when missing: {template.CanBranchIfTraitMissing.ToColouredString()}");
		output.AppendLine("Difficulty modifiers:");
		foreach (var difficulty in template.CheckTemplateDifficulties.OrderBy(x => x.Difficulty))
			output.AppendLine($"  {((Difficulty)difficulty.Difficulty).DescribeEnum()}: {difficulty.Modifier.ToString("N2", actor)}");
		output.AppendLine("Definition:");
		output.AppendLine(template.Definition);
		output.AppendLine($"Assigned check types: {(uses.Count == 0 ? "none" : uses.ListToCommaSeparatedValues())}");
		output.AppendLine("Runtime activation: server restart required.");
		actor.OutputHandler.Send(output.ToString());
	}

	private static List<string> GetCheckTemplateUses(long templateId)
	{
		using var db = new FMDB();
		return FMDB.Context.Checks.Where(x => x.CheckTemplateId == templateId).Select(x => x.Type)
			.AsEnumerable().Select(x => ((CheckType)x).DescribeEnum()).OrderBy(x => x).ToList();
	}

	private static CheckTemplateDraft DraftFrom(CheckTemplateModel template)
	{
		var draft = new CheckTemplateDraft { Id = template.Id, Name = template.Name, CheckMethod = template.CheckMethod,
			Definition = template.Definition, ImproveTraits = template.ImproveTraits,
			FailIfTraitMissingMode = template.FailIfTraitMissingMode, CanBranchIfTraitMissing = template.CanBranchIfTraitMissing };
		foreach (var item in template.CheckTemplateDifficulties) draft.Difficulties[item.Difficulty] = item.Modifier;
		return draft;
	}

	private static void CheckTemplateNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a new template name and optional method."); return; }
		var name = ss.PopSpeech();
		var method = ss.IsFinished ? "Standard" : ss.PopSpeech();
		if (GetCheckTemplates().Any(x => x.Name.EqualTo(name))) { actor.OutputHandler.Send("A template with that name already exists."); return; }
		var draft = new CheckTemplateDraft { Name = name, CheckMethod = method, Definition = DefaultCheckTemplateDefinition(method) };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) draft.Difficulties[(int)difficulty] = 0.0;
		try { ValidateCheckTemplateDraft(draft, actor.Gameworld); SaveCheckTemplateDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or OverflowException)
		{ actor.OutputHandler.Send($"Template was not created: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft);
		actor.OutputHandler.Send($"Created check template #{draft.Id!.Value.ToString("N0", actor)}. Configure it with checktemplate set.");
	}

	private static void CheckTemplateClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a template and new name."); return; }
		var source = FindCheckTemplate(ss.PopSpeech());
		var name = ss.SafeRemainingArgument;
		if (source is null || string.IsNullOrWhiteSpace(name)) { actor.OutputHandler.Send("Specify an existing template and a new name."); return; }
		var draft = DraftFrom(source); draft.Id = null; draft.Name = name;
		try { ValidateCheckTemplateDraft(draft, actor.Gameworld); SaveCheckTemplateDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or OverflowException)
		{ actor.OutputHandler.Send($"Template was not cloned: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft);
		actor.OutputHandler.Send($"Cloned {source.Name.ColourName()} as template #{draft.Id!.Value.ToString("N0", actor)}.");
	}

	private static void CheckTemplateEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which check template would you like to edit?"); return; }
		var template = FindCheckTemplate(ss.SafeRemainingArgument);
		if (template is null) { actor.OutputHandler.Send("There is no such check template."); return; }
		actor.SetEditingItem(DraftFrom(template));
		actor.OutputHandler.Send($"You are editing check template #{template.Id.ToString("N0", actor)} {template.Name.ColourName()}.");
	}

	private static void CheckTemplateSet(ICharacter actor, StringStack ss)
	{
		var draft = actor.EditingItem<CheckTemplateDraft>();
		if (draft is null) { actor.OutputHandler.Send("Open a check template with checktemplate new, clone or edit first."); return; }
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a field and value."); return; }
		var field = ss.PopSpeech().ToLowerInvariant();
		var value = field == "definition" ? ss.RemainingArgument : ss.SafeRemainingArgument;
		if (field == "limb" && value.Equals("list", StringComparison.OrdinalIgnoreCase))
		{
			try { RequireCheckMethod(draft, "limb"); actor.OutputHandler.Send(FormatLimbEntries(actor, draft)); }
			catch (FormatException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
			return;
		}
		var candidate = draft.Copy();
		try
		{
			SetCheckTemplateValue(candidate, field, value, actor.Gameworld);
			ValidateCheckTemplateDraft(candidate, actor.Gameworld);
			SaveCheckTemplateDraft(candidate);
			actor.SetEditingItem(candidate);
			actor.OutputHandler.Send($"Saved check template {candidate.Name.ColourName()}. Restart the server to activate the runtime change.");
		}
		catch (Exception ex) when (ex is FormatException or ArgumentException or InvalidOperationException or System.Xml.XmlException or OverflowException)
		{
			actor.OutputHandler.Send($"Check template was not saved: {ex.Message.ColourError()}");
		}
	}

	private static void SetCheckTemplateValue(CheckTemplateDraft draft, string field, string value, IFuturemud gameworld)
	{
		switch (field)
		{
			case "name": draft.Name = value; break;
			case "method": draft.CheckMethod = value; draft.Definition = DefaultCheckTemplateDefinition(value); break;
			case "definition": draft.Definition = value; break;
			case "improve": draft.ImproveTraits = ParseBoolean(value); break;
			case "branch": draft.CanBranchIfTraitMissing = ParseBoolean(value); break;
			case "missing": draft.FailIfTraitMissingMode = (short)ParseEnum<FailIfTraitMissingType>(value); break;
			case "difficulty":
				var parts = value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 2 || !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var modifier) || !double.IsFinite(modifier))
					throw new FormatException("Use difficulty <Difficulty> <finite modifier>.");
				draft.Difficulties[(int)ParseEnum<Difficulty>(parts[0])] = modifier; break;
			case "minimum_time": RequireCheckMethod(draft, "timebound"); SetCheckTime(draft, minimum: true, value); break;
			case "maximum_time": RequireCheckMethod(draft, "timebound"); SetCheckTime(draft, minimum: false, value); break;
			case "core_expression": RequireCheckMethod(draft, "passiveperception"); SetCheckElement(draft, "CoreTraitExpression", value); break;
			case "passive_fuzziness":
				RequireCheckMethod(draft, "passiveperception");
				if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var fuzz) || !double.IsFinite(fuzz)) throw new FormatException("Fuzziness must be a finite number.");
				SetCheckElement(draft, "PassiveFuzziness", fuzz.ToString("R", CultureInfo.InvariantCulture)); break;
			case "limb": RequireCheckMethod(draft, "limb"); ApplyLimbOperation(draft, value, gameworld); break;
			default: throw new FormatException("Fields: name, method, improve, branch, missing, difficulty, minimum_time, maximum_time, core_expression, passive_fuzziness, limb, definition (advanced override).");
		}
	}

	private static void RequireCheckMethod(CheckTemplateDraft draft, string expected)
	{
		if (expected == "timebound" && draft.CheckMethod.EqualTo("PassivePerception")) return;
		if (!draft.CheckMethod.Replace(" ", "", StringComparison.Ordinal).Equals(expected, StringComparison.OrdinalIgnoreCase))
			throw new FormatException($"This setting applies only to the {expected} check method.");
	}

	private static void SetCheckTime(CheckTemplateDraft draft, bool minimum, string value)
	{
		var seconds = ParseRequiredNonnegative(value, minimum ? "minimum_time" : "maximum_time");
		var root = XElement.Parse(draft.Definition);
		root.SetAttributeValue(minimum ? "minimum_time" : "maximum_time", seconds.ToString(CultureInfo.InvariantCulture));
		draft.Definition = root.ToString(SaveOptions.DisableFormatting);
	}

	private static void SetCheckElement(CheckTemplateDraft draft, string elementName, string value)
	{
		var root = XElement.Parse(draft.Definition);
		var element = root.Element(elementName);
		if (element is null) root.Add(element = new XElement(elementName));
		element.Value = value;
		draft.Definition = root.ToString(SaveOptions.DisableFormatting);
	}

	private static void ApplyLimbOperation(CheckTemplateDraft draft, string value, IFuturemud gameworld)
	{
		var args = new StringStack(value);
		var operation = args.PopForSwitch();
		var root = XElement.Parse(draft.Definition);
		var entries = root.Elements("Limbs").ToList();
		if (operation == "add")
		{
			var limb = ParseEnum<LimbType>(args.PopSpeech());
			var minimum = ParseRequiredNonnegative(args.PopSpeech(), "minimum");
			root.Add(new XElement("Limbs", new XElement("Limb", (int)limb), new XElement("Minimum", minimum)));
		}
		else if (operation == "remove")
		{
			if (!int.TryParse(args.PopSpeech(), out var index) || index < 1 || index > entries.Count) throw new FormatException("Specify a valid one-based limb entry number.");
			entries[index - 1].Remove();
		}
		else if (operation == "set")
		{
			if (!int.TryParse(args.PopSpeech(), out var index) || index < 1 || index > entries.Count) throw new FormatException("Specify a valid one-based limb entry number.");
			var limb = ParseEnum<LimbType>(args.PopSpeech());
			var minimum = ParseRequiredNonnegative(args.PopSpeech(), "minimum");
			var limbElement = entries[index - 1].Element("Limb") ?? throw new FormatException("Selected limb entry is missing its Limb value.");
			var minimumElement = entries[index - 1].Element("Minimum") ?? throw new FormatException("Selected limb entry is missing its Minimum value.");
			limbElement.Value = ((int)limb).ToString(CultureInfo.InvariantCulture);
			minimumElement.Value = minimum.ToString(CultureInfo.InvariantCulture);
		}
		else throw new FormatException("Use limb list, limb add <LimbType> <minimum>, limb set <index> <LimbType> <minimum>, or limb remove <index>.");
		if (!args.IsFinished) throw new FormatException("Unexpected trailing limb arguments.");
		draft.Definition = root.ToString(SaveOptions.DisableFormatting);
		ValidateCheckTemplateDraft(draft, gameworld);
	}

	private static string FormatLimbEntries(ICharacter actor, CheckTemplateDraft draft)
	{
		var root = XElement.Parse(draft.Definition);
		var entries = root.Elements("Limbs").ToList();
		if (entries.Count == 0) return "This template has no limb requirements.";
		return StringUtilities.GetTextTable(entries.Select((x, i) => new[]
			{ (i + 1).ToString(CultureInfo.InvariantCulture), x.Element("Limb")?.Value ?? "?", x.Element("Minimum")?.Value ?? "?" }),
			new[] { "Entry", "LimbType value", "Minimum" }, actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode);
	}

	private static void ValidateCheckTemplateDraft(CheckTemplateDraft draft, IFuturemud gameworld)
	{
		if (string.IsNullOrWhiteSpace(draft.Name)) throw new FormatException("Name is required.");
		if (!new[] { "Standard", "OGL", "Timebound", "Limb", "Static", "Branch", "PassivePerception", "BonusAbsent" }.Contains(draft.CheckMethod, StringComparer.OrdinalIgnoreCase))
			throw new FormatException("Supported methods: Standard, OGL, Timebound, Limb, Static, Branch, PassivePerception and BonusAbsent.");
		if (!Enum.IsDefined((FailIfTraitMissingType)draft.FailIfTraitMissingMode)) throw new FormatException("Invalid missing-trait policy.");
		var root = XElement.Parse(draft.Definition);
		var method = draft.CheckMethod.Replace(" ", "", StringComparison.Ordinal).ToLowerInvariant();
		if (method is "timebound" or "passiveperception")
		{
			var min = ParseRequiredNonnegative(root.Attribute("minimum_time")?.Value, "minimum_time");
			var max = ParseRequiredNonnegative(root.Attribute("maximum_time")?.Value, "maximum_time");
			if (max < min || max == int.MaxValue) throw new FormatException("maximum_time must be at least minimum_time and less than 2147483647 seconds.");
		}
		if (method == "passiveperception")
		{
			var expression = root.Element("CoreTraitExpression")?.Value;
			if (string.IsNullOrWhiteSpace(expression)) throw new FormatException("PassivePerception requires CoreTraitExpression.");
			var formula = new TraitExpression(expression, gameworld);
			if (formula.Formula.HasErrors()) throw new FormatException("CoreTraitExpression is not a valid formula.");
			if (!double.TryParse(root.Element("PassiveFuzziness")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var fuzz) || !double.IsFinite(fuzz) || fuzz < 0 || fuzz >= 1)
				throw new FormatException("PassiveFuzziness must be a finite number from 0 up to (but not including) 1.");
		}
		else if (method == "limb")
		{
			foreach (var item in root.Elements("Limbs"))
			{
				if (!int.TryParse(item.Element("Limb")?.Value, out var limb) || !Enum.IsDefined((LimbType)limb)) throw new FormatException("Each Limbs element needs a valid Limb value.");
				ParseRequiredNonnegative(item.Element("Minimum")?.Value, "Minimum");
			}
		}
		foreach (var modifier in draft.Difficulties.Values)
			if (!double.IsFinite(modifier)) throw new FormatException("Difficulty modifiers must be finite numbers.");
		if (Enum.GetValues<Difficulty>().Any(x => !draft.Difficulties.ContainsKey((int)x))) throw new FormatException("Provide a modifier for every difficulty.");
	}

	private static void SaveCheckTemplateDraft(CheckTemplateDraft draft)
	{
		using var db = new FMDB();
		var model = draft.Id.HasValue ? FMDB.Context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).FirstOrDefault(x => x.Id == draft.Id.Value) : null;
		if (draft.Id.HasValue && model is null) throw new InvalidOperationException("The selected template was removed while you were editing it.");
		if (FMDB.Context.CheckTemplates.Any(x => x.Name == draft.Name && (!draft.Id.HasValue || x.Id != draft.Id.Value))) throw new FormatException("A check template with that name already exists.");
		model ??= new CheckTemplateModel { CheckTemplateDifficulties = new HashSet<CheckTemplateDifficulty>(), Checks = new HashSet<Check>() };
		draft.CheckMethod = new[] { "Standard", "OGL", "Timebound", "Limb", "Static", "Branch", "PassivePerception", "BonusAbsent" }.Single(x => x.EqualTo(draft.CheckMethod));
		model.Name = draft.Name; model.CheckMethod = draft.CheckMethod; model.Definition = draft.Definition;
		model.ImproveTraits = draft.ImproveTraits; model.FailIfTraitMissingMode = draft.FailIfTraitMissingMode;
		model.CanBranchIfTraitMissing = draft.CanBranchIfTraitMissing;
		foreach (var pair in draft.Difficulties)
		{
			var difficulty = model.CheckTemplateDifficulties.SingleOrDefault(x => x.Difficulty == pair.Key);
			if (difficulty is null)
				model.CheckTemplateDifficulties.Add(new CheckTemplateDifficulty { CheckTemplateId = model.Id, Difficulty = pair.Key, Modifier = pair.Value });
			else difficulty.Modifier = pair.Value;
		}
		if (!draft.Id.HasValue) FMDB.Context.CheckTemplates.Add(model);
		FMDB.Context.SaveChanges();
		if (!draft.Id.HasValue) draft.Id = model.Id;
	}

	private static string DefaultCheckTemplateDefinition(string method) => method.Replace(" ", "", StringComparison.Ordinal).ToLowerInvariant() switch
	{
		"timebound" => "<Definition minimum_time=\"1\" maximum_time=\"1\" />",
		"passiveperception" => "<Definition minimum_time=\"1\" maximum_time=\"1\"><CoreTraitExpression>0</CoreTraitExpression><PassiveFuzziness>0</PassiveFuzziness></Definition>",
		_ => "<Definition />"
	};

	private static void CheckTemplateAssign(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a template and existing CheckType enum name."); return; }
		var template = FindCheckTemplate(ss.PopSpeech());
		if (template is null || ss.IsFinished) { actor.OutputHandler.Send("Specify an existing template and check type."); return; }
		var name = ss.PopSpeech();
		if (!Enum.TryParse<CheckType>(name, true, out var checkType) || !Enum.IsDefined(checkType)) { actor.OutputHandler.Send("That is not an existing CheckType."); return; }
		using var db = new FMDB();
		var check = FMDB.Context.Checks.FirstOrDefault(x => x.Type == (int)checkType);
		if (check is null) { actor.OutputHandler.Send("That check type has no existing runtime Check row to assign."); return; }
		check.CheckTemplateId = template.Id;
		FMDB.Context.SaveChanges();
		actor.OutputHandler.Send($"Assigned {template.Name.ColourName()} to {checkType.DescribeEnum()}. Restart the server to activate the runtime change.");
	}

	private static bool ParseBoolean(string value) => value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase) || value == "1" ? true : value.Equals("false", StringComparison.OrdinalIgnoreCase) || value.Equals("no", StringComparison.OrdinalIgnoreCase) || value == "0" ? false : throw new FormatException("Use true or false.");
	private static T ParseEnum<T>(string value) where T : struct, Enum => Enum.TryParse<T>(value, true, out var result) && Enum.IsDefined(result) ? result : throw new FormatException($"Invalid {typeof(T).Name} value.");
	private static int ParseRequiredNonnegative(string? value, string field) { if (!int.TryParse(value, out var result) || result < 0) throw new FormatException($"{field} must be a non-negative integer."); return result; }

	private const string TraitDecoratorHelp = @"Syntax:
	#3traitdecorator list [ +name ]#0
	#3traitdecorator show <id|name>#0
	#3traitdecorator new <name> <SimpleNumeric|CurrentMax|Percentage|Range>#0
	#3traitdecorator clone <id|name> <new name>#0
	#3traitdecorator edit <id|name>#0
	#3traitdecorator set name <name> | type <type> | prefix <text> | suffix <text>#0 (Range)
	#3traitdecorator set colour_capped|colour_buffed|colour_default <true|false>#0 (Range)
	#3traitdecorator set ranges list|add <low> <high> <descriptor>|set <index> <low> <high> <descriptor>|remove <index>#0 (Range)
	#3traitdecorator set definition <xml>#0 (advanced override)
	#3traitdecorator close#0

Definition edits are factory-validated and saved immediately. Runtime activation requires a server restart. Range descriptors use integer bounds and must be ordered and contiguous. The advanced XML override uses a Definition root with name/prefix/suffix/colour_capped/colour_buffed/colour_default attributes and lowercase range elements with text/low/high attributes.";

	[PlayerCommand("TraitDecorator", "traitdecorator")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("traitdecorator", TraitDecoratorHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void TraitDecorator(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord()); var action = ss.PopForSwitch();
		switch (action)
		{
			case "list": TraitDecoratorList(actor, ss); return;
			case "show": TraitDecoratorShow(actor, ss); return;
			case "new": TraitDecoratorNew(actor, ss); return;
			case "clone": TraitDecoratorClone(actor, ss); return;
			case "edit": TraitDecoratorEdit(actor, ss); return;
			case "set": TraitDecoratorSet(actor, ss); return;
			case "close": actor.SetEditingItem<TraitDecoratorDraft>(null); actor.OutputHandler.Send("You close the trait decorator editor."); return;
			default: actor.OutputHandler.Send(TraitDecoratorHelp.SubstituteANSIColour()); return;
		}
	}

	private static List<MudSharp.Models.TraitDecorator> GetTraitDecoratorModels()
	{
		using var db = new FMDB(); return FMDB.Context.TraitDecorators.OrderBy(x => x.Id).ToList();
	}

	private static MudSharp.Models.TraitDecorator? FindTraitDecorator(string selector)
	{
		var all = GetTraitDecoratorModels();
		return long.TryParse(selector, out var id) ? all.FirstOrDefault(x => x.Id == id) : all.FirstOrDefault(x => x.Name.EqualTo(selector));
	}

	private static void TraitDecoratorList(ICharacter actor, StringStack ss)
	{
		var all = GetTraitDecoratorModels();
		while (!ss.IsFinished) { var f = ss.PopSpeech(); if (!f.StartsWith('+') || f.Length < 2) { actor.OutputHandler.Send("Use +keyword filters."); return; } var k = f[1..]; all = all.Where(x => x.Name.Contains(k, StringComparison.OrdinalIgnoreCase) || x.Type.Contains(k, StringComparison.OrdinalIgnoreCase)).ToList(); }
		actor.OutputHandler.Send(StringUtilities.GetTextTable(all.Select(x => new[] { x.Id.ToString("N0", actor), x.Name, x.Type }), new[] { "Id", "Name", "Type" }, actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
	}

	private static void TraitDecoratorShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which trait decorator?"); return; }
		var model = FindTraitDecorator(ss.SafeRemainingArgument); if (model is null) { actor.OutputHandler.Send("There is no such trait decorator."); return; }
		ITraitValueDecorator loaded;
		try { loaded = DecoratorBase.GetDecorator(model, setAsCurrentMaxSingleton: false); }
		catch (Exception ex) { actor.OutputHandler.Send($"Stored decorator definition is invalid: {ex.Message.ColourError()}\n{model.Contents}"); return; }
		if (loaded is null) { actor.OutputHandler.Send($"Unknown trait decorator type {model.Type.ColourError()}."); return; }
		var uses = GetTraitDecoratorUses(model.Id);
		actor.OutputHandler.Send(loaded.Show(actor) + $"Stored type: {model.Type}\nStored definition:\n{model.Contents}\nUsed by traits: {(uses.Count == 0 ? "none" : uses.ListToCommaSeparatedValues())}\nRuntime activation: server restart required.");
	}

	private static List<string> GetTraitDecoratorUses(long id)
	{
		using var db = new FMDB(); return FMDB.Context.TraitDefinitions.Where(x => x.DecoratorId == id).Select(x => x.Name).OrderBy(x => x).ToList();
	}

	private static TraitDecoratorDraft TraitDraftFrom(MudSharp.Models.TraitDecorator model) => new() { Id = model.Id, Name = model.Name, Type = model.Type, Contents = model.Contents };
	private static void TraitDecoratorNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a name and decorator type."); return; }
		var name = ss.PopSpeech(); var type = ss.SafeRemainingArgument;
		var draft = new TraitDecoratorDraft { Name = name, Type = type, Contents = DefaultTraitDecoratorContents(type, name) };
		try { ValidateTraitDecoratorDraft(draft); SaveTraitDecoratorDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException) { actor.OutputHandler.Send($"Decorator was not created: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft); actor.OutputHandler.Send($"Created trait decorator #{draft.Id!.Value.ToString("N0", actor)}.");
	}
	private static void TraitDecoratorClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify an existing decorator and new name."); return; }
		var source = FindTraitDecorator(ss.PopSpeech()); var name = ss.SafeRemainingArgument;
		if (source is null || string.IsNullOrWhiteSpace(name)) { actor.OutputHandler.Send("Specify an existing decorator and new name."); return; }
		var draft = TraitDraftFrom(source); draft.Id = null;
		try { SetTraitDecoratorValue(draft, "name", name); ValidateTraitDecoratorDraft(draft); SaveTraitDecoratorDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException) { actor.OutputHandler.Send($"Decorator was not cloned: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft); actor.OutputHandler.Send($"Cloned decorator as #{draft.Id!.Value.ToString("N0", actor)}.");
	}
	private static void TraitDecoratorEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which trait decorator?"); return; }
		var model = FindTraitDecorator(ss.SafeRemainingArgument); if (model is null) { actor.OutputHandler.Send("There is no such trait decorator."); return; }
		actor.SetEditingItem(TraitDraftFrom(model)); actor.OutputHandler.Send($"You are editing {model.Name.ColourName()}.");
	}
	private static void TraitDecoratorSet(ICharacter actor, StringStack ss)
	{
		var draft = actor.EditingItem<TraitDecoratorDraft>(); if (draft is null) { actor.OutputHandler.Send("Open a trait decorator first."); return; }
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a field and value."); return; }
		var field = ss.PopSpeech().ToLowerInvariant(); var value = field == "definition" ? ss.RemainingArgument : ss.SafeRemainingArgument;
		if (field == "ranges" && value.Equals("list", StringComparison.OrdinalIgnoreCase))
		{
			try { actor.OutputHandler.Send(FormatTraitRanges(actor, draft)); }
			catch (FormatException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
			return;
		}
		var candidate = draft.Copy();
		try
		{
			SetTraitDecoratorValue(candidate, field, value);
			ValidateTraitDecoratorDraft(candidate); SaveTraitDecoratorDraft(candidate); actor.SetEditingItem(candidate);
			actor.OutputHandler.Send($"Saved {candidate.Name.ColourName()}. Restart the server to activate the runtime change.");
		}
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException or OverflowException) { actor.OutputHandler.Send($"Decorator was not saved: {ex.Message.ColourError()}"); }
	}
	private static void SetTraitDecoratorValue(TraitDecoratorDraft draft, string field, string value)
	{
		if (field == "name")
		{
			draft.Name = value;
			if (CanonicalTraitDecoratorType(draft.Type) == "Range")
			{
				var renamedDefinition = XElement.Parse(draft.Contents);
				renamedDefinition.SetAttributeValue("name", value);
				draft.Contents = renamedDefinition.ToString(SaveOptions.DisableFormatting);
			}
			return;
		}
		if (field == "type")
		{
			draft.Type = CanonicalTraitDecoratorType(value);
			if (draft.Type == "Range") draft.Contents = DefaultTraitDecoratorContents(draft.Type, draft.Name);
			return;
		}
		if (field == "definition") { draft.Contents = value; return; }
		if (CanonicalTraitDecoratorType(draft.Type) != "Range") throw new FormatException("Prefix, suffix, colour flags and ranges are available only for Range decorators.");
		var root = XElement.Parse(draft.Contents);
		switch (field)
		{
			case "prefix": root.SetAttributeValue("prefix", value); break;
			case "suffix": root.SetAttributeValue("suffix", value); break;
			case "colour_capped": case "colour_buffed": case "colour_default": root.SetAttributeValue(field, ParseBoolean(value).ToString().ToLowerInvariant()); break;
			case "ranges": ApplyTraitRangeOperation(root, value); break;
			default: throw new FormatException("Use name, type, prefix, suffix, colour_capped, colour_buffed, colour_default, ranges, or definition (advanced override).");
		}
		draft.Contents = root.ToString(SaveOptions.DisableFormatting);
	}
	private static void ApplyTraitRangeOperation(XElement root, string value)
	{
		var args = new StringStack(value); var operation = args.PopForSwitch(); var ranges = root.Elements("range").ToList();
		if (operation == "add")
		{
			var low = ParseRequiredInteger(args.PopSpeech(), "low"); var high = ParseRequiredInteger(args.PopSpeech(), "high"); var text = args.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(text)) throw new FormatException("Provide descriptor text.");
			root.Add(new XElement("range", new XAttribute("text", text), new XAttribute("low", low), new XAttribute("high", high)));
		}
		else if (operation == "set")
		{
			var index = ParseRangeIndex(args.PopSpeech(), ranges.Count); var low = ParseRequiredInteger(args.PopSpeech(), "low"); var high = ParseRequiredInteger(args.PopSpeech(), "high"); var text = args.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(text)) throw new FormatException("Provide descriptor text.");
			ranges[index].SetAttributeValue("text", text); ranges[index].SetAttributeValue("low", low); ranges[index].SetAttributeValue("high", high);
		}
		else if (operation == "remove") ranges[ParseRangeIndex(args.PopSpeech(), ranges.Count)].Remove();
		else throw new FormatException("Use ranges add <low> <high> <descriptor>, set <index> <low> <high> <descriptor>, remove <index>, or list.");
		if (operation == "remove" && !args.IsFinished) throw new FormatException("Unexpected trailing range arguments.");
	}
	private static string FormatTraitRanges(ICharacter actor, TraitDecoratorDraft draft)
	{
		if (CanonicalTraitDecoratorType(draft.Type) != "Range") throw new FormatException("Only Range trait decorators have descriptors.");
		var ranges = XElement.Parse(draft.Contents).Elements("range").ToList();
		if (ranges.Count == 0) return "No trait descriptor ranges are configured.";
		return StringUtilities.GetTextTable(ranges.Select((x, i) => new[] { (i + 1).ToString(actor), x.Attribute("low")?.Value ?? "?", x.Attribute("high")?.Value ?? "?", x.Attribute("text")?.Value ?? "?" }), new[] { "Index", "Low", "High", "Descriptor" }, actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode);
	}
	private static int ParseRequiredInteger(string value, string field)
	{
		if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)) throw new FormatException($"{field} must be an integer.");
		return result;
	}
	private static int ParseRangeIndex(string value, int count)
	{
		if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var oneBased) || oneBased < 1 || oneBased > count) throw new FormatException("Specify a valid one-based range index.");
		return oneBased - 1;
	}
	private static string DefaultTraitDecoratorContents(string type, string name) => type.ToLowerInvariant() == "range"
		? new XElement("Definition", new XAttribute("name", name), new XAttribute("prefix", ""), new XAttribute("suffix", ""), new XAttribute("colour_capped", "true"), new XAttribute("colour_buffed", "true"), new XAttribute("colour_default", "true"), new XElement("range", new XAttribute("text", "unrated"), new XAttribute("low", "0"), new XAttribute("high", "100"))).ToString(SaveOptions.DisableFormatting)
		: "";
	private static void ValidateTraitDecoratorDraft(TraitDecoratorDraft draft)
	{
		if (string.IsNullOrWhiteSpace(draft.Name)) throw new FormatException("Name is required.");
		var type = draft.Type.ToLowerInvariant();
		if (type is not ("simplenumeric" or "currentmax" or "percentage" or "range")) throw new FormatException("Supported trait types: SimpleNumeric, CurrentMax, Percentage, Range.");
		if (type == "range")
		{
			var root = XElement.Parse(draft.Contents);
			foreach (var attribute in new[] { "name", "prefix", "suffix", "colour_capped" })
				if (root.Attribute(attribute) is null) throw new FormatException($"Range definition requires the {attribute} attribute.");
			if (string.IsNullOrWhiteSpace(root.Attribute("name")?.Value)) throw new FormatException("Range definition name cannot be empty.");
			if (!root.Attribute("name")!.Value.EqualTo(draft.Name)) throw new FormatException("Range definition name must match the saved decorator name.");
			foreach (var attribute in new[] { "colour_capped", "colour_buffed", "colour_default" })
				if (root.Attribute(attribute) is { } colourAttribute && !bool.TryParse(colourAttribute.Value, out _)) throw new FormatException($"{attribute} must be true or false.");
			var ranges = root.Elements("range").ToList();
			if (ranges.Count == 0) throw new FormatException("Range decorator needs at least one range element.");
			var bounds = new List<(int Low, int High)>();
			foreach (var range in ranges)
			{
				if (string.IsNullOrWhiteSpace(range.Attribute("text")?.Value) || !int.TryParse(range.Attribute("low")?.Value, out var low) || !int.TryParse(range.Attribute("high")?.Value, out var high) || low > high)
					throw new FormatException("Each range needs text and integer low/high bounds with low <= high.");
				bounds.Add((low, high));
			}
			for (var i = 1; i < bounds.Count; i++) if (bounds[i].Low != bounds[i - 1].High) throw new FormatException("Trait ranges must be ordered and meet exactly at adjacent boundaries without gaps or overlap.");
		}
		var model = new MudSharp.Models.TraitDecorator { Id = draft.Id ?? 0, Name = draft.Name, Type = CanonicalTraitDecoratorType(draft.Type), Contents = draft.Contents };
		if (DecoratorBase.GetDecorator(model, setAsCurrentMaxSingleton: false) is null) throw new FormatException("The trait decorator factory rejected this type.");
	}
	private static string CanonicalTraitDecoratorType(string type) => type.ToLowerInvariant() switch { "simplenumeric" => "SimpleNumeric", "currentmax" => "CurrentMax", "percentage" => "Percentage", "range" => "Range", _ => type };
	private static void SaveTraitDecoratorDraft(TraitDecoratorDraft draft)
	{
		using var db = new FMDB(); var model = draft.Id.HasValue ? FMDB.Context.TraitDecorators.FirstOrDefault(x => x.Id == draft.Id.Value) : null;
		if (draft.Id.HasValue && model is null) throw new InvalidOperationException("The decorator was removed while editing.");
		if (FMDB.Context.TraitDecorators.Any(x => x.Name == draft.Name && (!draft.Id.HasValue || x.Id != draft.Id.Value))) throw new FormatException("A decorator with that name already exists.");
		model ??= new MudSharp.Models.TraitDecorator(); model.Name = draft.Name; model.Type = CanonicalTraitDecoratorType(draft.Type); model.Contents = draft.Contents;
		if (!draft.Id.HasValue) FMDB.Context.TraitDecorators.Add(model); FMDB.Context.SaveChanges(); if (!draft.Id.HasValue) draft.Id = model.Id;
	}

	private const string StackDecoratorHelp = @"Syntax:
	#3stackdecorator list [ +name ]#0
	#3stackdecorator show <id|name>#0
	#3stackdecorator new <name> <Suffix|Pile|Bites|Range>#0
	#3stackdecorator clone <id|name> <new name>#0
	#3stackdecorator edit <id|name>#0
	#3stackdecorator set name <name> | type <type> | description <text>#0
	#3stackdecorator set ranges list|add <min> <max> <item text>|set <index> <min> <max> <item text>|remove <index>#0 (Pile/Bites/Range)
	#3stackdecorator set definition <xml>#0 (advanced override)
	#3stackdecorator close#0

Definitions are factory-validated and saved immediately. Runtime activation requires a server restart. Pile/Bites use integer quantities; Range uses finite numeric quantities. Bites item text is a format string that accepts the item description as {0}. Ranges must be ordered and contiguous.";

	[PlayerCommand("StackDecorator", "stackdecorator")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("stackdecorator", StackDecoratorHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void StackDecorator(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord()); var action = ss.PopForSwitch();
		switch (action)
		{
			case "list": StackDecoratorList(actor, ss); return;
			case "show": StackDecoratorShow(actor, ss); return;
			case "new": StackDecoratorNew(actor, ss); return;
			case "clone": StackDecoratorClone(actor, ss); return;
			case "edit": StackDecoratorEdit(actor, ss); return;
			case "set": StackDecoratorSet(actor, ss); return;
			case "close": actor.SetEditingItem<StackDecoratorDraft>(null); actor.OutputHandler.Send("You close the stack decorator editor."); return;
			default: actor.OutputHandler.Send(StackDecoratorHelp.SubstituteANSIColour()); return;
		}
	}

	private static List<MudSharp.Models.StackDecorator> GetStackDecoratorModels()
	{
		using var db = new FMDB(); return FMDB.Context.StackDecorators.OrderBy(x => x.Id).ToList();
	}
	private static MudSharp.Models.StackDecorator? FindStackDecorator(string selector)
	{
		var all = GetStackDecoratorModels(); return long.TryParse(selector, out var id) ? all.FirstOrDefault(x => x.Id == id) : all.FirstOrDefault(x => x.Name.EqualTo(selector));
	}
	private static void StackDecoratorList(ICharacter actor, StringStack ss)
	{
		var all = GetStackDecoratorModels();
		while (!ss.IsFinished) { var f = ss.PopSpeech(); if (!f.StartsWith('+') || f.Length < 2) { actor.OutputHandler.Send("Use +keyword filters."); return; } var k = f[1..]; all = all.Where(x => x.Name.Contains(k, StringComparison.OrdinalIgnoreCase) || x.Type.Contains(k, StringComparison.OrdinalIgnoreCase)).ToList(); }
		actor.OutputHandler.Send(StringUtilities.GetTextTable(all.Select(x => new[] { x.Id.ToString("N0", actor), x.Name, x.Type, x.Description ?? "" }), new[] { "Id", "Name", "Type", "Description" }, actor.LineFormatLength, truncatableColumnIndex: 3, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
	}
	private static void StackDecoratorShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which stack decorator?"); return; }
		var model = FindStackDecorator(ss.SafeRemainingArgument); if (model is null) { actor.OutputHandler.Send("There is no such stack decorator."); return; }
		try { _ = StackDecoratorRuntime.LoadStackDecorator(model); }
		catch (Exception ex) { actor.OutputHandler.Send($"The stored definition is invalid: {ex.Message.ColourError()}\n{model.Definition}"); return; }
		var uses = GetStackDecoratorUses(model.Id);
		actor.OutputHandler.Send($"Stack Decorator #{model.Id.ToString("N0", actor)}: {model.Name.ColourName()}\nType: {model.Type.ColourValue()}\nDescription: {model.Description}\nDefinition:\n{model.Definition}\nKnown configured uses: {(uses.Count == 0 ? "none found in component definitions" : uses.ListToCommaSeparatedValues())}\nRuntime activation: server restart required.");
	}
	private static List<string> GetStackDecoratorUses(long id)
	{
		using var db = new FMDB();
		var uses = new List<string>();
		foreach (var proto in FMDB.Context.GameItemComponentProtos.AsNoTracking().Select(x => new { x.Id, x.Type, x.Definition }).ToList())
		{
			try
			{
				var root = XElement.Parse(proto.Definition);
				if (root.DescendantsAndSelf().Any(x => x.Attributes().Any(a => a.Name.LocalName.Contains("decorator", StringComparison.OrdinalIgnoreCase) && a.Value == id.ToString(CultureInfo.InvariantCulture)) ||
					x.Name.LocalName.Contains("decorator", StringComparison.OrdinalIgnoreCase) && x.Value == id.ToString(CultureInfo.InvariantCulture)))
					uses.Add($"{proto.Type} #{proto.Id}");
			}
			catch (Exception ex) when (ex is System.Xml.XmlException or ArgumentNullException) { }
		}
	var token = id.ToString(CultureInfo.InvariantCulture);
	uses.AddRange(FMDB.Context.StaticConfigurations.AsNoTracking()
		.Where(x => x.SettingName.Contains("StackDecorator") && x.Definition == token)
		.Select(x => "static configuration " + x.SettingName).ToList());
		return uses.OrderBy(x => x).ToList();
	}
	private static StackDecoratorDraft StackDraftFrom(MudSharp.Models.StackDecorator model) => new() { Id = model.Id, Name = model.Name, Type = model.Type, Description = model.Description, Definition = model.Definition };
	private static void StackDecoratorNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a name and decorator type."); return; }
		var name = ss.PopSpeech(); var type = ss.SafeRemainingArgument;
		var draft = new StackDecoratorDraft { Name = name, Type = type, Definition = DefaultStackDecoratorDefinition(type) };
		try { ValidateStackDecoratorDraft(draft); SaveStackDecoratorDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException or NotImplementedException or OverflowException) { actor.OutputHandler.Send($"Decorator was not created: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft); actor.OutputHandler.Send($"Created stack decorator #{draft.Id!.Value.ToString("N0", actor)}.");
	}
	private static void StackDecoratorClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify an existing decorator and new name."); return; }
		var source = FindStackDecorator(ss.PopSpeech()); var name = ss.SafeRemainingArgument;
		if (source is null || string.IsNullOrWhiteSpace(name)) { actor.OutputHandler.Send("Specify an existing decorator and new name."); return; }
		var draft = StackDraftFrom(source); draft.Id = null; draft.Name = name;
		try { ValidateStackDecoratorDraft(draft); SaveStackDecoratorDraft(draft); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException or NotImplementedException or OverflowException) { actor.OutputHandler.Send($"Decorator was not cloned: {ex.Message.ColourError()}"); return; }
		actor.SetEditingItem(draft); actor.OutputHandler.Send($"Cloned stack decorator as #{draft.Id!.Value.ToString("N0", actor)}.");
	}
	private static void StackDecoratorEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished) { actor.OutputHandler.Send("Which stack decorator?"); return; }
		var model = FindStackDecorator(ss.SafeRemainingArgument); if (model is null) { actor.OutputHandler.Send("There is no such stack decorator."); return; }
		actor.SetEditingItem(StackDraftFrom(model)); actor.OutputHandler.Send($"You are editing {model.Name.ColourName()}.");
	}
	private static void StackDecoratorSet(ICharacter actor, StringStack ss)
	{
		var draft = actor.EditingItem<StackDecoratorDraft>(); if (draft is null) { actor.OutputHandler.Send("Open a stack decorator first."); return; }
		if (ss.IsFinished) { actor.OutputHandler.Send("Specify a field and value."); return; }
		var field = ss.PopSpeech().ToLowerInvariant(); var value = field == "definition" ? ss.RemainingArgument : ss.SafeRemainingArgument;
		if (field == "ranges" && value.Equals("list", StringComparison.OrdinalIgnoreCase))
		{
			try { actor.OutputHandler.Send(FormatStackRanges(actor, draft)); }
			catch (FormatException ex) { actor.OutputHandler.Send(ex.Message.ColourError()); }
			return;
		}
		var candidate = draft.Copy();
		try { SetStackDecoratorValue(candidate, field, value); ValidateStackDecoratorDraft(candidate); SaveStackDecoratorDraft(candidate); actor.SetEditingItem(candidate); actor.OutputHandler.Send($"Saved {candidate.Name.ColourName()}. Restart the server to activate the runtime change."); }
		catch (Exception ex) when (ex is FormatException or ArgumentException or System.Xml.XmlException or InvalidOperationException or NotImplementedException or OverflowException) { actor.OutputHandler.Send($"Decorator was not saved: {ex.Message.ColourError()}"); }
	}
	private static void SetStackDecoratorValue(StackDecoratorDraft draft, string field, string value)
	{
		if (field == "name") { draft.Name = value; return; }
		if (field == "type")
		{
			draft.Type = CanonicalStackDecoratorType(value);
			draft.Definition = DefaultStackDecoratorDefinition(draft.Type);
			return;
		}
		if (field == "description") { draft.Description = value; return; }
		if (field == "definition") { draft.Definition = value; return; }
		if (field != "ranges") throw new FormatException("Fields: name, type, description, ranges, definition (advanced override).");
		if (CanonicalStackDecoratorType(draft.Type) == "Suffix") throw new FormatException("Suffix decorators do not use quantity ranges.");
		var root = XElement.Parse(draft.Definition);
		ApplyStackRangeOperation(root, value);
		draft.Definition = root.ToString(SaveOptions.DisableFormatting);
	}
	private static void ApplyStackRangeOperation(XElement root, string value)
	{
		var args = new StringStack(value); var operation = args.PopForSwitch(); var ranges = root.Elements("Range").ToList();
		if (operation == "add")
		{
			var min = args.PopSpeech(); var max = args.PopSpeech(); var item = args.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(item)) throw new FormatException("Provide Item text (for Bites, include the {0} placeholder).");
			root.Add(new XElement("Range", new XAttribute("Item", item), new XAttribute("Min", min), new XAttribute("Max", max)));
		}
		else if (operation == "set")
		{
			var index = ParseRangeIndex(args.PopSpeech(), ranges.Count); var min = args.PopSpeech(); var max = args.PopSpeech(); var item = args.SafeRemainingArgument;
			if (string.IsNullOrWhiteSpace(item)) throw new FormatException("Provide Item text.");
			ranges[index].SetAttributeValue("Min", min); ranges[index].SetAttributeValue("Max", max); ranges[index].SetAttributeValue("Item", item);
		}
		else if (operation == "remove") ranges[ParseRangeIndex(args.PopSpeech(), ranges.Count)].Remove();
		else throw new FormatException("Use ranges add <min> <max> <item text>, set <index> <min> <max> <item text>, remove <index>, or list.");
		if (operation == "remove" && !args.IsFinished) throw new FormatException("Unexpected trailing range arguments.");
	}
	private static string FormatStackRanges(ICharacter actor, StackDecoratorDraft draft)
	{
		if (CanonicalStackDecoratorType(draft.Type) == "Suffix") throw new FormatException("Suffix decorators do not have quantity ranges.");
		var ranges = XElement.Parse(draft.Definition).Elements("Range").ToList();
		if (ranges.Count == 0) return "No stack quantity ranges are configured.";
		return StringUtilities.GetTextTable(ranges.Select((x, i) => new[] { (i + 1).ToString(actor), x.Attribute("Min")?.Value ?? "?", x.Attribute("Max")?.Value ?? "?", x.Attribute("Item")?.Value ?? "?" }), new[] { "Index", "Minimum", "Maximum", "Item/format" }, actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode);
	}
	private static string CanonicalStackDecoratorType(string type) => type.ToLowerInvariant() switch { "suffix" => "Suffix", "pile" => "Pile", "bites" => "Bites", "range" => "Range", _ => type };
	private static string DefaultStackDecoratorDefinition(string type) => CanonicalStackDecoratorType(type) switch
	{
		"Suffix" => "<Definition />",
		"Bites" => "<Definition><Range Item=\"{0}\" Min=\"0\" Max=\"2147483647\" /></Definition>",
		"Pile" => "<Definition><Range Item=\"item\" Min=\"0\" Max=\"2147483647\" /></Definition>",
		_ => "<Definition><Range Item=\"item\" Min=\"0\" Max=\"1.7976931348623157E+308\" /></Definition>"
	};
	private static void ValidateStackDecoratorDraft(StackDecoratorDraft draft)
	{
		if (string.IsNullOrWhiteSpace(draft.Name)) throw new FormatException("Name is required.");
		var canonical = CanonicalStackDecoratorType(draft.Type);
		if (canonical is not ("Suffix" or "Pile" or "Bites" or "Range")) throw new FormatException("Supported stack types: Suffix, Pile, Bites, Range.");
		if (canonical == "Suffix")
		{
			_ = StackDecoratorRuntime.LoadStackDecorator(new MudSharp.Models.StackDecorator { Id = draft.Id ?? 0, Name = draft.Name, Type = canonical, Description = draft.Description, Definition = draft.Definition });
			return;
		}
		var root = XElement.Parse(draft.Definition); var ranges = root.Elements("Range").ToList(); if (ranges.Count == 0) throw new FormatException("This decorator needs at least one Range element.");
		var parsed = new List<(double Min, double Max)>();
		foreach (var range in ranges)
		{
			if (string.IsNullOrWhiteSpace(range.Attribute("Item")?.Value)) throw new FormatException("Every Range needs an Item description.");
			double min;
			double max;
			if (canonical is "Pile" or "Bites")
			{
				if (!int.TryParse(range.Attribute("Min")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intMin) || !int.TryParse(range.Attribute("Max")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intMax))
					throw new FormatException("Pile and Bites ranges need integer Min and Max values.");
				min = intMin; max = intMax;
			}
			else if (!double.TryParse(range.Attribute("Min")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out min) || !double.IsFinite(min) || !double.TryParse(range.Attribute("Max")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out max) || !double.IsFinite(max))
				throw new FormatException("Range Min and Max must be finite numbers.");
			if (!double.IsFinite(min) || !double.IsFinite(max) || min > max)
				throw new FormatException("Range Min and Max must be finite numbers with Min <= Max.");
			if (canonical == "Bites")
			{
				try { _ = string.Format(CultureInfo.InvariantCulture, range.Attribute("Item")!.Value, "item"); }
				catch (FormatException) { throw new FormatException("Bites Item text must be a valid format string with a placeholder for the item description."); }
			}
			parsed.Add((min, max));
		}
		for (var i = 1; i < parsed.Count; i++) if (parsed[i].Min != parsed[i - 1].Max) throw new FormatException("Quantity ranges must be ordered and meet exactly at adjacent boundaries without gaps or overlap.");
		var model = new MudSharp.Models.StackDecorator { Id = draft.Id ?? 0, Name = draft.Name, Type = canonical, Description = draft.Description, Definition = draft.Definition };
		_ = StackDecoratorRuntime.LoadStackDecorator(model);
	}
	private static void SaveStackDecoratorDraft(StackDecoratorDraft draft)
	{
		using var db = new FMDB(); var model = draft.Id.HasValue ? FMDB.Context.StackDecorators.FirstOrDefault(x => x.Id == draft.Id.Value) : null;
		if (draft.Id.HasValue && model is null) throw new InvalidOperationException("The decorator was removed while editing.");
		if (FMDB.Context.StackDecorators.Any(x => x.Name == draft.Name && (!draft.Id.HasValue || x.Id != draft.Id.Value))) throw new FormatException("A decorator with that name already exists.");
		model ??= new MudSharp.Models.StackDecorator(); model.Name = draft.Name; model.Type = CanonicalStackDecoratorType(draft.Type); model.Description = draft.Description; model.Definition = draft.Definition;
		if (!draft.Id.HasValue) FMDB.Context.StackDecorators.Add(model); FMDB.Context.SaveChanges(); if (!draft.Id.HasValue) draft.Id = model.Id;
	}
}
