#nullable enable

using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position;
using MudSharp.Body.PartProtos;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Models;
using System.Xml.Linq;

namespace MudSharp.Commands.Modules;

internal partial class BuilderModule
{
	private const string BodyPrototypeHelp = @"Syntax:
	#3bodyproto list#0
	#3bodyproto show <prototype>#0
	#3bodyproto edit <prototype>#0
	#3bodyproto edit#0 - show the prototype currently being edited
	#3bodyproto close#0
	#3bodyproto create <name> size <wear-rule-id> [from <prototype>]#0
	#3bodyproto clone <source> as <new-name>#0 - creates a derived prototype that inherits the source anatomy
	#3bodyproto set <property> <value>#0
	#3bodyproto set part role <part> core|male|female|extra#0
	#3bodyproto set part remove|restore <part>#0
	#3bodyproto part add <template-part> <new-name>#0 - makes a new owned part from template properties; graph references are not copied
	#3bodyproto set position add|remove <position>#0
	#3bodyproto set speed add <position> <alias> <rate> <stamina-rate> <1st-person-verb> <3rd-person-verb> <participle>#0
	#3bodyproto set speed edit <speed-id> <alias> <rate> <stamina-rate> <1st-person-verb> <3rd-person-verb> <participle>#0
	#3bodyproto set limb add <name> type <limb-type> root <part> parts <id,id,...> [spine <id,id,...>] [damage <multiplier>] [pain <multiplier>]#0

Properties: #6parent#0, #6tracking#0, #6consider#0, #6wielder singular|plural#0, #6leg singular|plural#0, #6legs#0, #6wings#0, #6smashing#0, and #6wearsize#0.

Parts belong to exactly one body prototype. Create/edit owned parts with the existing #3bodypart#0 builder; inherited parts keep their source IDs and must be changed on their owner. A structural change is refused when this prototype or a descendant is referenced by a race or instantiated body. Movement defaults are derived from the first speed loaded for each position; this command does not persist a separate default choice.";

	[PlayerCommand("BodyPrototype", "bodyproto")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("bodyproto", BodyPrototypeHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void BodyPrototypeCommand(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list": BodyPrototypeList(actor); return;
			case "show": BodyPrototypeShow(actor, ss); return;
			case "edit": BodyPrototypeEdit(actor, ss); return;
			case "close": BodyPrototypeClose(actor); return;
			case "create": BodyPrototypeCreate(actor, ss); return;
			case "clone": BodyPrototypeClone(actor, ss); return;
			case "part": BodyPrototypePart(actor, ss); return;
			case "set": BodyPrototypeSet(actor, ss); return;
			default: actor.OutputHandler.Send(BodyPrototypeHelp.SubstituteANSIColour()); return;
		}
	}

	private static BuilderEditingEffect<IBodyPrototype>? EditingBodyPrototype(ICharacter actor) =>
		actor.EffectsOfType<BuilderEditingEffect<IBodyPrototype>>().FirstOrDefault();

	private static void BodyPrototypeList(ICharacter actor)
	{
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from body in actor.Gameworld.BodyPrototypes.OrderBy(x => x.Id)
			select new List<string> { body.Id.ToString("N0", actor), body.Name, body.Parent?.Name ?? "", body.AllBodypartsBonesAndOrgans.Count().ToString("N0", actor), body.Limbs.Count().ToString("N0", actor) },
			new List<string> { "Id", "Name", "Parent", "Parts", "Limbs" }, actor, Telnet.Green));
	}

	private static IBodyPrototype? ResolveBodyPrototype(ICharacter actor, string text)
	{
		var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(text);
		if (body is null) actor.OutputHandler.Send($"There is no body prototype identified by {text.ColourCommand()}.");
		return body;
	}

	private static void BodyPrototypeShow(ICharacter actor, StringStack ss)
	{
		var body = ss.IsFinished ? EditingBodyPrototype(actor)?.EditingItem : ResolveBodyPrototype(actor, ss.SafeRemainingArgument);
		if (body is null)
		{
			if (ss.IsFinished && EditingBodyPrototype(actor) is null) actor.OutputHandler.Send("Which body prototype do you want to show?");
			return;
		}
		actor.OutputHandler.Send(body.Show(actor));
	}

	private static void BodyPrototypeEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var current = EditingBodyPrototype(actor);
			if (current is null) actor.OutputHandler.Send("Which body prototype do you want to edit?");
			else actor.OutputHandler.Send(current.EditingItem.Show(actor));
			return;
		}
		var body = ResolveBodyPrototype(actor, ss.SafeRemainingArgument);
		if (body is null) return;
		actor.RemoveAllEffects<BuilderEditingEffect<IBodyPrototype>>();
		actor.AddEffect(new BuilderEditingEffect<IBodyPrototype>(actor) { EditingItem = body });
		actor.OutputHandler.Send($"You are now editing body prototype {body.Name.ColourName()}.");
	}

	private static void BodyPrototypeClose(ICharacter actor)
	{
		if (EditingBodyPrototype(actor) is null)
		{
			actor.OutputHandler.Send("You are not editing a body prototype.");
			return;
		}
		actor.RemoveAllEffects<BuilderEditingEffect<IBodyPrototype>>();
		actor.OutputHandler.Send("You are no longer editing a body prototype.");
	}

	private static void BodyPrototypeCreate(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Use bodyproto create <name> size <wear-rule-id> [from <prototype>].");
			return;
		}
		var name = ss.PopSpeech().Trim();
		if (string.IsNullOrWhiteSpace(name) || actor.Gameworld.BodyPrototypes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("Enter a non-empty, unique body prototype name.");
			return;
		}

		long? wearRuleId = null;
		IBodyPrototype? parent = null;
		while (!ss.IsFinished)
		{
			var option = ss.PopSpeech().ToLowerInvariant();
			switch (option)
			{
				case "size":
					if (!long.TryParse(ss.PopSpeech(), out var sizeId)) { actor.OutputHandler.Send("The size rule must be a numeric ID."); return; }
					wearRuleId = sizeId;
					break;
				case "from":
					var parentText = ss.PopSpeech();
					parent = actor.Gameworld.BodyPrototypes.GetByIdOrName(parentText);
					if (parent is null) { actor.OutputHandler.Send($"There is no body prototype identified by {parentText.ColourCommand()}."); return; }
					break;
				default: actor.OutputHandler.Send("Expected size <wear-rule-id> or from <prototype>."); return;
			}
		}

		Models.WearableSizeParameterRule wearRule;
		using (new FMDB())
		{
			wearRule = wearRuleId.HasValue ? FMDB.Context.WearableSizeParameterRule.Find(wearRuleId.Value) : null;
			if (wearRule is null) { actor.OutputHandler.Send("Choose a valid wearable-size rule with size <id>."); return; }
			if (!IsUsableWearableSizeRule(wearRule, out var ruleError))
			{
				actor.OutputHandler.Send($"That wearable-size rule cannot be used: {ruleError.ColourError()}");
				return;
			}
			var row = new BodyProto
			{
				Name = name,
				CountsAsId = parent?.Id,
				WearSizeParameterId = wearRule.Id,
				WielderDescriptionSingle = "hand",
				WielderDescriptionPlural = "hands",
				LegDescriptionSingular = "leg",
				LegDescriptionPlural = "legs",
				NameForTracking = name,
				ConsiderString = ""
			};
			FMDB.Context.BodyProtos.Add(row);
			FMDB.Context.SaveChanges();
			row.WearSizeParameter = wearRule;
			var prototype = new BodyPrototype(row, actor.Gameworld);
			prototype.FinaliseBodyparts(row);
			actor.Gameworld.Add(prototype);
			actor.RemoveAllEffects<BuilderEditingEffect<IBodyPrototype>>();
			actor.AddEffect(new BuilderEditingEffect<IBodyPrototype>(actor) { EditingItem = prototype });
			actor.OutputHandler.Send($"Created and opened body prototype {prototype.Name.ColourName()} (#{prototype.Id.ToString("N0", actor)}). It {(parent is null ? "starts with no owned anatomy" : $"inherits anatomy from {parent.Name.ColourName()}")}; create bodyparts and limbs before using it for a race.");
		}
	}

	private static bool IsUsableWearableSizeRule(WearableSizeParameterRule rule, out string error)
	{
		try
		{
			foreach (var xml in new[] { rule.WeightVolumeRatios, rule.TraitVolumeRatios, rule.HeightLinearRatios })
			{
				var root = XElement.Parse(xml);
				var ranges = new RankedRange<int>();
				if (!root.Elements("Ratio").Any()) { error = "Each sizing dimension needs at least one ratio."; return false; }
				foreach (var ratio in root.Elements("Ratio"))
				{
					var item = ratio.Attribute("Item")?.Value;
					var min = ratio.Attribute("Min")?.Value;
					var max = ratio.Attribute("Max")?.Value;
					if (!int.TryParse(item, out _) || !double.TryParse(min, out var minimum) ||
					    !double.TryParse(max, out var maximum) || !double.IsFinite(minimum) ||
					    !double.IsFinite(maximum) || minimum > maximum)
					{
						error = "A ratio entry has missing or invalid Item, Min, or Max values.";
						return false;
					}
					ranges.Add(int.Parse(item!), minimum, maximum);
				}
			}
		}
		catch (Exception ex) when (ex is System.Xml.XmlException or FormatException or OverflowException or ArgumentNullException or ApplicationException)
		{
			error = ex.Message;
			return false;
		}

		error = string.Empty;
		return true;
	}

	private static void BodyPrototypeClone(ICharacter actor, StringStack ss)
	{
		var sourceText = ss.PopSpeech();
		var source = actor.Gameworld.BodyPrototypes.GetByIdOrName(sourceText);
		if (source is null) { actor.OutputHandler.Send($"There is no body prototype identified by {sourceText.ColourCommand()}."); return; }
		if (!ss.PopSpeech().EqualTo("as") || ss.IsFinished) { actor.OutputHandler.Send("Use bodyproto clone <source> as <new-name>. This creates an inheriting prototype and preserves inherited source part IDs."); return; }
		var name = ss.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(name) || actor.Gameworld.BodyPrototypes.Any(x => x.Name.EqualTo(name))) { actor.OutputHandler.Send("Enter a non-empty, unique body prototype name."); return; }
		using (new FMDB())
		{
			var sourceRow = FMDB.Context.BodyProtos.Find(source.Id);
			var row = new BodyProto
			{
				Name = name, CountsAsId = source.Id, WearSizeParameterId = sourceRow.WearSizeParameterId,
				WielderDescriptionSingle = sourceRow.WielderDescriptionSingle, WielderDescriptionPlural = sourceRow.WielderDescriptionPlural,
				LegDescriptionSingular = sourceRow.LegDescriptionSingular, LegDescriptionPlural = sourceRow.LegDescriptionPlural,
				ConsiderString = sourceRow.ConsiderString, StaminaRecoveryProgId = sourceRow.StaminaRecoveryProgId,
				MinimumLegsToStand = sourceRow.MinimumLegsToStand, MinimumWingsToFly = sourceRow.MinimumWingsToFly,
				DefaultSmashingBodypartId = sourceRow.DefaultSmashingBodypartId, NameForTracking = name,
				PlanarData = sourceRow.PlanarData
			};
			foreach (var position in source.ValidPositions)
			{
				row.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = row, Position = (int)position.Id });
			}
			FMDB.Context.BodyProtos.Add(row);
			FMDB.Context.SaveChanges();
			row.WearSizeParameter = FMDB.Context.WearableSizeParameterRule.Find(row.WearSizeParameterId);
			var prototype = new BodyPrototype(row, actor.Gameworld);
			prototype.FinaliseBodyparts(row);
			actor.Gameworld.Add(prototype);
			actor.RemoveAllEffects<BuilderEditingEffect<IBodyPrototype>>();
			actor.AddEffect(new BuilderEditingEffect<IBodyPrototype>(actor) { EditingItem = prototype });
			actor.OutputHandler.Send($"Created and opened derived body prototype {prototype.Name.ColourName()}. It inherits source anatomy IDs from {source.Name.ColourName()}; use bodyproto create ... size <id> for a standalone prototype.");
		}
	}

	private static void BodyPrototypeSet(ICharacter actor, StringStack ss)
	{
		var effect = EditingBodyPrototype(actor);
		if (effect is null) { actor.OutputHandler.Send("Edit a body prototype first."); return; }
		if (ss.IsFinished) { actor.OutputHandler.Send(BodyPrototypeHelp.SubstituteANSIColour()); return; }
		var body = effect.EditingItem as BodyPrototype;
		var property = ss.PopSpeech().ToLowerInvariant();
		string error = string.Empty;
		switch (property)
		{
			case "parent":
				var parentText = ss.SafeRemainingArgument;
				var parent = parentText.EqualToAny("none", "clear") ? null : actor.Gameworld.BodyPrototypes.GetByIdOrName(parentText);
				if (!parentText.EqualToAny("none", "clear") && parent is null) { actor.OutputHandler.Send($"There is no body prototype identified by {parentText.ColourCommand()}."); return; }
				if (!body.SetParent(parent, out error)) { actor.OutputHandler.Send(error.ColourError()); return; }
				break;
			case "tracking": case "consider":
				if (!body.SetMetadata(property, ss.SafeRemainingArgument, out error)) { actor.OutputHandler.Send(error.ColourError()); return; }
				break;
			case "wielder": case "leg":
				var form = ss.PopSpeech().ToLowerInvariant();
				if (!form.EqualToAny("singular", "plural")) { actor.OutputHandler.Send("Choose singular or plural."); return; }
				if (!body.SetMetadata($"{property} {form}", ss.SafeRemainingArgument, out error)) { actor.OutputHandler.Send(error.ColourError()); return; }
				break;
			case "legs": case "wings":
				if (!int.TryParse(ss.PopSpeech(), out var count) || !body.SetThreshold(property, count, out error)) { actor.OutputHandler.Send(string.IsNullOrEmpty(error) ? "Enter a whole-number threshold." : error.ColourError()); return; }
				break;
			case "wearsize":
				if (!long.TryParse(ss.PopSpeech(), out var id) || !body.SetWearSize(id, out error)) { actor.OutputHandler.Send("Choose a valid wearable-size rule ID."); return; }
				break;
			case "smashing":
				var smashText = ss.SafeRemainingArgument;
				var smashPart = body.AllExternalBodyparts.FirstOrDefault(x => x.Name.EqualTo(smashText) || x.Id.ToString() == smashText);
				if (!body.SetDefaultSmashingPart(smashPart, out error)) { actor.OutputHandler.Send(error.ColourError()); return; }
				break;
			case "part": BodyPrototypeSetPart(actor, body, ss); return;
			case "position": BodyPrototypeSetPosition(actor, body, ss); return;
			case "speed": BodyPrototypeSetSpeed(actor, body, ss); return;
			case "limb": BodyPrototypeSetLimb(actor, body, ss); return;
			default: actor.OutputHandler.Send(BodyPrototypeHelp.SubstituteANSIColour()); return;
		}
		actor.OutputHandler.Send($"Body prototype {body.Name.ColourName()} has been updated.");
	}

	private static void BodyPrototypeSetPart(ICharacter actor, BodyPrototype body, StringStack ss)
	{
		var action = ss.PopSpeech().ToLowerInvariant();
		var partText = ss.PopSpeech();
		var candidates = action == "restore" ? actor.Gameworld.BodypartPrototypes : body.AllBodypartsBonesAndOrgans;
		var part = candidates.FirstOrDefault(x => x.Name.EqualTo(partText) || x.Id.ToString() == partText);
		if (part is null) { actor.OutputHandler.Send($"That part is not part of {body.Name.ColourName()}."); return; }
		if (action == "role")
		{
			var roleText = ss.PopSpeech().ToLowerInvariant() switch { "male" => "MaleAddition", "female" => "FemaleAddition", var text => text };
			if (!Enum.TryParse(roleText, true, out BodypartRole role) || !Enum.IsDefined(role)) { actor.OutputHandler.Send("Choose core, male, female or extra."); return; }
			if (!body.SetPartRole(part, role, out var error)) { actor.OutputHandler.Send(error.ColourError()); return; }
		}
		else if (action is "remove" or "restore")
		{
			if (!body.SetPartRemoved(part, action == "remove", out var error)) { actor.OutputHandler.Send(error.ColourError()); return; }
		}
		else { actor.OutputHandler.Send("Use part role <part> core|male|female|extra, or part remove|restore <part>."); return; }
		actor.OutputHandler.Send($"Updated bodypart {part.FullDescription().ColourName()}.");
	}

	private static void BodyPrototypePart(ICharacter actor, StringStack ss)
	{
		var effect = EditingBodyPrototype(actor);
		if (effect?.EditingItem is not BodyPrototype body) { actor.OutputHandler.Send("Edit a body prototype first."); return; }
		if (!ss.PopSpeech().EqualTo("add")) { actor.OutputHandler.Send("Use bodyproto part add <template-part> <new-name>."); return; }
		var templateText = ss.PopSpeech();
		var template = actor.Gameworld.BodypartPrototypes.GetByIdOrName(templateText);
		if (template is null) { actor.OutputHandler.Send($"There is no bodypart identified by {templateText.ColourCommand()}."); return; }
		if (ss.IsFinished) { actor.OutputHandler.Send("What name should the new owned part have?"); return; }
		var name = ss.SafeRemainingArgument;
		var part = body.AddPartFromTemplate(template, name, out var error);
		if (part is null) { actor.OutputHandler.Send(error.ColourError()); return; }
		actor.OutputHandler.Send($"Created bodypart {part.FullDescription().ColourName()} (#{part.Id.ToString("N0", actor)}) owned by {body.Name.ColourName()}. Upstream, internal-organ, bone-coverage and limb links were not copied; set those references to parts owned by this prototype.");
	}

	private static void BodyPrototypeSetPosition(ICharacter actor, BodyPrototype body, StringStack ss)
	{
		var action = ss.PopSpeech().ToLowerInvariant();
		if (!action.EqualToAny("add", "remove")) { actor.OutputHandler.Send("Use position add or position remove."); return; }
		var positionText = ss.SafeRemainingArgument;
		var position = PositionState.GetState(positionText);
		if (position is null || positionText.Length == 0) { actor.OutputHandler.Send("Which position state?"); return; }
		if (!body.SetPosition(position, action == "add", out var error)) { actor.OutputHandler.Send(error.ColourError()); return; }
		actor.OutputHandler.Send($"Position {position.Name.ColourName()} is now {(action == "add" ? "allowed" : "disallowed")} for {body.Name.ColourName()}.");
	}

	private static void BodyPrototypeSetSpeed(ICharacter actor, BodyPrototype body, StringStack ss)
	{
		var action = ss.PopSpeech().ToLowerInvariant();
		if (action == "edit")
		{
			if (!long.TryParse(ss.PopSpeech(), out var speedId)) { actor.OutputHandler.Send("Enter the speed ID shown by bodyproto show."); return; }
			var speed = body.Speeds.Get(speedId);
			if (speed is null) { actor.OutputHandler.Send("That speed is not part of this body prototype."); return; }
			var editedAlias = ss.PopSpeech();
			if (!double.TryParse(ss.PopSpeech(), out var editedRate) || !double.TryParse(ss.PopSpeech(), out var editedStamina)) { actor.OutputHandler.Send("Movement and stamina rates must be numbers."); return; }
			var editedFirst = ss.PopSpeech(); var editedThird = ss.PopSpeech(); var editedParticiple = ss.SafeRemainingArgument;
			if (!body.EditSpeed(speed, editedAlias, editedFirst, editedThird, editedParticiple, editedRate, editedStamina, out var editError)) { actor.OutputHandler.Send(editError.ColourError()); return; }
			actor.OutputHandler.Send($"Updated movement speed {editedAlias.ColourName()} while preserving its speed ID.");
			return;
		}
		if (action != "add") { actor.OutputHandler.Send("Use speed add or speed edit."); return; }
		var positionText = ss.PopSpeech();
		var position = PositionState.GetState(positionText);
		var alias = ss.PopSpeech();
		if (!double.TryParse(ss.PopSpeech(), out var rate) || !double.TryParse(ss.PopSpeech(), out var stamina)) { actor.OutputHandler.Send("Movement and stamina rates must be numbers."); return; }
		var first = ss.PopSpeech(); var third = ss.PopSpeech(); var participle = ss.SafeRemainingArgument;
		if (!body.AddSpeed(position, alias, first, third, participle, rate, stamina, out var error)) { actor.OutputHandler.Send(error.ColourError()); return; }
		actor.OutputHandler.Send($"Added {alias.ColourName()} movement for {position.Name.ColourName()}.");
	}

	private static void BodyPrototypeSetLimb(ICharacter actor, BodyPrototype body, StringStack ss)
	{
		if (!ss.PopSpeech().EqualTo("add")) { actor.OutputHandler.Send("Use limb add <name> type <limb-type> root <part> parts <id,id,...> [spine <id,id,...>] [damage <multiplier>] [pain <multiplier>]."); return; }
		var name = ss.PopSpeech();
		if (!ss.PopSpeech().EqualTo("type") || !Enum.TryParse(ss.PopSpeech(), true, out LimbType type) || !Enum.IsDefined(type) || !ss.PopSpeech().EqualTo("root")) { actor.OutputHandler.Send("Specify a limb type and root bodypart."); return; }
		var rootText = ss.PopSpeech();
		var root = body.AllBodyparts.OfType<IExternalBodypart>().FirstOrDefault(x => x.Name.EqualTo(rootText) || x.Id.ToString() == rootText);
		if (root is null) { actor.OutputHandler.Send("The limb root must be an external bodypart in this prototype's anatomy."); return; }
		if (!ss.PopSpeech().EqualTo("parts")) { actor.OutputHandler.Send("Specify parts <id,id,...>."); return; }
		if (!TryParseIds(ss.PopSpeech(), out var partIds)) { actor.OutputHandler.Send("The parts list must be a comma-separated list of bodypart IDs."); return; }
		var spinalIds = new List<long>(); var damage = 1.0; var pain = 1.0;
		while (!ss.IsFinished)
		{
			var option = ss.PopSpeech().ToLowerInvariant();
			if (option == "spine")
			{
				if (!TryParseIds(ss.PopSpeech(), out spinalIds)) { actor.OutputHandler.Send("The spine list must be a comma-separated list of bodypart IDs."); return; }
			}
			else if (option == "damage") double.TryParse(ss.PopSpeech(), out damage);
			else if (option == "pain") double.TryParse(ss.PopSpeech(), out pain);
			else { actor.OutputHandler.Send("Expected spine, damage, or pain."); return; }
		}
		var parts = partIds.Select(id => actor.Gameworld.BodypartPrototypes.Get(id)).Where(x => x is not null).ToList();
		var spinal = spinalIds.Select(id => actor.Gameworld.BodypartPrototypes.Get(id)).Where(x => x is not null).ToList();
		if (parts.Count != partIds.Count || spinal.Count != spinalIds.Count) { actor.OutputHandler.Send("Every limb part ID must identify an existing bodypart."); return; }
		var limb = body.CreateLimb(name, type, root, parts, spinal, damage, pain, out var error);
		if (limb is null) { actor.OutputHandler.Send(error.ColourError()); return; }
		actor.OutputHandler.Send($"Created limb {limb.Name.ColourName()} on {body.Name.ColourName()} with {limb.Parts.Count()} parts.");
	}

	private static bool TryParseIds(string text, out List<long> ids)
	{
		var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		ids = new List<long>();
		foreach (var part in parts)
		{
			if (!long.TryParse(part, out var id) || id <= 0) return false;
			ids.Add(id);
		}
		return parts.Length > 0;
	}
}
