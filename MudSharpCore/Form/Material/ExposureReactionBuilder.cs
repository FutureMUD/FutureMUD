using MudSharp.Framework.Units;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Form.Material;

public static class ExposureReactionBuilder
{
	public static bool IsDiagnostic(StringStack command)
	{
		var copy = new StringStack(command.SafeRemainingArgument);
		return copy.PopSpeech().EqualTo("reaction") && copy.PopSpeech().EqualTo("test");
	}
	public const string Help = @"
	#3reaction add <tag>#0 - adds an inactive v2 rule; then author its rates
	#3reaction <number> name|channel|category <text>#0 - sets identity and deliberate stacking channel
	#3reaction <number> material <solid>|none#0 - selects an exact material
	#3reaction <number> tag <tag>#0 - toggles a hierarchical material tag
	#3reaction <number> routes <flags>#0 - LiquidContact, GasContact, Inhalation
	#3reaction <number> priority <integer>#0 - selects the winning rule within its channel
	#3reaction <number> exclude#0 - toggles an explicit no-reaction exclusion
	#3reaction <number> type <damage type>#0 - sets the wound type
	#3reaction <number> damage|pain|stun <rate>#0 - output per reference exposure second
	#3reaction <number> consume none|<volume>#0 - volume consumed per reference exposure second
	#3reaction <number> spent <liquid>|none#0 - optional inert 1:1 carrier replacement
	#3reaction <number> mintemp|maxtemp <temperature>|none#0 - limits source temperature
	#3reaction <number> applicability|intensity|notification <prog>|none#0 - validated exposure hooks
	#3reaction <number> message <text>|none#0 - overrides the continuing-exposure message
	#3reaction <number> convert <damage> <pain> <stun>#0 - preserves v1 payload and explicitly supplies v2 rates
	#3reaction <number> delete#0 - removes a rule
	#3reaction test <material>#0 - read-only effective-rule and consumption diagnostic
	#3reaction test actor|item <target> <route|pool|interior> <seconds> [volume]#0 - target/layer/finite-source prediction";

	public static string Show(IEnumerable<ILiquidSurfaceReaction> definitions, ICharacter actor, bool gas)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Environmental Exposure".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		var index = 0;
		foreach (var rule in definitions.OfType<LiquidSurfaceReaction>())
		{
			sb.AppendLine($"{(++index).ToString("N0", actor)}. {rule.Name.ColourName()} [{rule.Id}] v{rule.Version}; legacy payload: {rule.HasLegacy.ToColouredString()}");
			sb.AppendLine($"   Routes: {rule.Routes.ToString().ColourValue()}; category: {rule.Category.ColourName()}; channel: {rule.Channel.ColourName()}; priority: {rule.Priority.ToString(actor).ColourValue()}; exclusion: {rule.NoReaction.ToColouredString()}");
			sb.AppendLine($"   Material: {rule.TargetMaterial?.Name.ColourName() ?? "tag selection"}; tags: {rule.TargetTags.Select(x => x.FullName).ListToString()}");
			sb.AppendLine($"   {rule.DamageType.DescribeEnum().ColourName()}: damage {rule.DamageRate.ToString("N3", actor)}, pain {rule.PainRate.ToString("N3", actor)}, stun {rule.StunRate.ToString("N3", actor)} / reference second");
			sb.AppendLine($"   Consumption: {rule.Consumption}; {_Volume(rule.ConsumptionRate)} / reference second; spent: {rule.SpentLiquid?.Name ?? "none"}");
			sb.AppendLine($"   Source temperature: {rule.MinimumTemperature?.ToString(actor) ?? "unbounded"} to {rule.MaximumTemperature?.ToString(actor) ?? "unbounded"}; applicability/intensity/notification progs: {rule.ApplicabilityProg?.Id.ToString(actor) ?? "none"}/{rule.IntensityProg?.Id.ToString(actor) ?? "none"}/{rule.NotificationProg?.Id.ToString(actor) ?? "none"}; message: {rule.Message ?? "static default"}");
			if (rule.Version == 1) sb.AppendLine($"   Legacy finite contact only: {rule.DamagePerTick.ToString("N3", actor)} damage per base volume; no continuous rate.");
			foreach (var error in rule.ValidationErrors(gas)) sb.AppendLine($"   INACTIVE: {error}".ColourError());
		}
		if (index == 0) sb.AppendLine("No exposure reactions.");
		return sb.ToString();
		string _Volume(double value) => actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor);
	}

	public static bool Edit(ICharacter actor, StringStack command, IList<ILiquidSurfaceReaction> definitions, bool gas, IFluid source)
	{
		var world = actor.Gameworld;
		var token = command.PopSpeech();
		if (token.EqualTo("test"))
		{
			var selection = new StringStack(command.SafeRemainingArgument);
			var targetKind = selection.PopSpeech();
			if (targetKind.EqualToAny("actor", "item"))
			{
				var name = selection.PopSpeech();
				IPerceivable? target = targetKind.EqualTo("actor") ? name.EqualTo("self") ? actor : actor.TargetActor(name) : actor.TargetItem(name);
				var routeText = selection.PopSpeech();
				var kind = routeText.EqualTo("pool") ? ExposureSourceKind.Immersion : routeText.EqualTo("interior") ? ExposureSourceKind.ContainerInterior : gas ? ExposureSourceKind.Atmosphere : ExposureSourceKind.Retained;
				var route = ExposureRoute.LiquidContact;
				if (target is null || !routeText.EqualToAny("pool", "interior") && !Enum.TryParse(routeText, true, out route) ||
					!double.TryParse(selection.PopSpeech(), actor, out var seconds) || !ExposureArithmetic.Valid(seconds) || seconds <= 0 || seconds > 600 ||
					route is not (ExposureRoute.LiquidContact or ExposureRoute.GasContact or ExposureRoute.Inhalation or ExposureRoute.AmbientHeat))
				{ actor.OutputHandler.Send("Select a visible actor/item, a named route (or pool/interior), and 0-600 positive seconds."); return false; }
				var volume = EnvironmentalExposureOptions.Read(world).SplashReferenceVolume;
				if (!selection.IsFinished && (!world.UnitManager.TryGetBaseUnits(selection.SafeRemainingArgument, UnitType.FluidVolume, actor, out volume) || !ExposureArithmetic.Valid(volume) || volume <= 0))
				{ actor.OutputHandler.Send("Supply a positive finite liquid volume with units."); return false; }
				actor.OutputHandler.Send(ExposureDiagnostic.Describe(actor, target, source, route, kind, seconds, volume)); return false;
			}
			var material = world.Materials.GetByIdOrName(command.SafeRemainingArgument);
			if (material is null) { actor.OutputHandler.Send("Select an existing solid material."); return false; }
			var text = new StringBuilder($"Mode: {EnvironmentalExposureOptions.Read(world).Mode}; dry run, full reference surface, one second.\n");
			foreach (var route in Enum.GetValues<ExposureRoute>().Where(x => x != ExposureRoute.None))
			{
				var selected = EnvironmentalExposureResolver.SelectRules(source, material, route, actor.Location.CurrentTemperature(null), out var diagnostics);
				foreach (var rule in selected) text.AppendLine($"{route}: {rule.Name}, {rule.Channel}, priority {rule.Priority}, {(rule.NoReaction ? "excluded" : $"raw {rule.DamageRate} damage/s; {rule.ConsumptionRate} base volume/s consumption")}; applicability/intensity progs require a live target context.");
				foreach (var error in diagnostics) text.AppendLine(error);
			}
			text.AppendLine($"Transmission liquid/gas/heat: {material.ExposureProperties.LiquidTransmission:P0}/{material.ExposureProperties.GasTransmission:P0}/{material.ExposureProperties.ThermalTransmission:P0}; heat threshold: {material.HeatDamagePoint?.ToString(actor) ?? "not authored"} base temperature.");
			actor.OutputHandler.Send(text.ToString()); return false;
		}
		EnvironmentalExposureService.For(world).Settle(actor);
		if (token.EqualTo("add"))
		{
			var tag = world.Tags.GetByIdOrName(command.SafeRemainingArgument);
			if (tag is null) { actor.OutputHandler.Send("Select an existing target tag."); return false; }
			var added = new LiquidSurfaceReaction(world) { Name = tag.Name + " reaction", DamageType = DamageType.Chemical, Routes = gas ? ExposureRoute.GasContact : ExposureRoute.LiquidContact };
			added.Convert(0, 0, 0, false); added.ToggleTargetTag(tag); definitions.Add(added);
			actor.OutputHandler.Send($"Added reaction {definitions.Count.ToString(actor).ColourValue()}. Author its rates before use."); return true;
		}
		if (!int.TryParse(token, out var index) || index < 1 || index > definitions.Count || definitions[index - 1] is not LiquidSurfaceReaction old)
		{ actor.OutputHandler.Send(Help.SubstituteANSIColour()); return false; }
		var ruleEdit = new LiquidSurfaceReaction(old, world);
		var field = command.PopForSwitch();
		var value = command.SafeRemainingArgument;
		if (field is "delete" or "remove") { definitions.RemoveAt(index - 1); actor.OutputHandler.Send("Reaction removed."); return true; }
		if (field == "convert")
		{
			if (!double.TryParse(command.PopSpeech(), actor, out var damage) || !double.TryParse(command.PopSpeech(), actor, out var pain) || !double.TryParse(command.PopSpeech(), actor, out var stun) || !ExposureArithmetic.Valid(damage) || !ExposureArithmetic.Valid(pain) || !ExposureArithmetic.Valid(stun))
			{ actor.OutputHandler.Send("Supply finite non-negative damage, pain and stun per reference second."); return false; }
			ruleEdit.Convert(damage, pain, stun);
		}
		else switch (field)
		{
			case "name": ruleEdit.Name = value; break;
			case "channel": ruleEdit.Channel = value; break;
			case "category": ruleEdit.Category = value; break;
			case "material":
				var material = world.Materials.GetByIdOrName(value);
				if (material is null && !value.EqualTo("none")) return Error("No such material.");
				ruleEdit.TargetMaterial = material; break;
			case "tag":
				var tag = world.Tags.GetByIdOrName(value);
				if (tag is null) return Error("No such tag.");
				ruleEdit.ToggleTargetTag(tag); break;
			case "routes":
				if (!Enum.TryParse<ExposureRoute>(value, true, out var routes) || routes == ExposureRoute.None || ((int)routes & ~7) != 0)
					return Error("Use LiquidContact, GasContact or Inhalation, separated by commas. Ambient heat uses material thresholds; ingestion/injection use reagent delivery.");
				ruleEdit.Routes = routes; break;
			case "priority":
				if (!int.TryParse(value, out var priority)) return Error("Supply an integer priority.");
				ruleEdit.Priority = priority; break;
			case "exclude": ruleEdit.NoReaction = !ruleEdit.NoReaction; break;
			case "type":
				if (!value.TryParseEnum<DamageType>(out var type)) return Error("No such damage type.");
				ruleEdit.DamageType = type; break;
			case "damage": case "pain": case "stun":
				if (!double.TryParse(value, actor, out var rate) || !ExposureArithmetic.Valid(rate)) return Error("Supply a finite non-negative rate.");
				if (ruleEdit.Version == 1)
				{
					if (field == "damage") ruleEdit.DamagePerTick = rate;
					if (field == "pain") ruleEdit.PainPerTick = rate;
					if (field == "stun") ruleEdit.StunPerTick = rate;
				}
				else
				{
					if (field == "damage") ruleEdit.DamageRate = rate;
					if (field == "pain") ruleEdit.PainRate = rate;
					if (field == "stun") ruleEdit.StunRate = rate;
				}
				break;
			case "consume":
				if (gas) return Error("Gas reactions do not consume liquids.");
				if (value.EqualTo("none")) { ruleEdit.Consumption = ReactionConsumption.None; ruleEdit.ConsumptionRate = 0; break; }
				if (!world.UnitManager.TryGetBaseUnits(value, UnitType.FluidVolume, actor, out var volume) || !ExposureArithmetic.Valid(volume) || volume <= 0) return Error("Supply a positive volume per reference second, such as 2mL.");
				ruleEdit.Consumption = ReactionConsumption.PerExposure; ruleEdit.ConsumptionRate = volume; break;
			case "spent":
				if (gas) return Error("Gas reactions cannot create spent liquids.");
				var liquid = world.Liquids.GetByIdOrName(value);
				if (liquid is null && !value.EqualTo("none")) return Error("No such liquid.");
				ruleEdit.SpentLiquid = liquid; break;
			case "mintemp": case "maxtemp":
				double? temperature = null;
				if (!value.EqualTo("none"))
				{
					if (!world.UnitManager.TryGetBaseUnits(value, UnitType.Temperature, actor, out var parsed) || !double.IsFinite(parsed)) return Error("Supply an absolute temperature with units.");
					temperature = parsed;
				}
				if (field == "mintemp") ruleEdit.MinimumTemperature = temperature; else ruleEdit.MaximumTemperature = temperature;
				break;
			case "applicability": case "intensity": case "notification":
				var prog = world.FutureProgs.GetByIdOrName(value);
				if (prog is null && !value.EqualTo("none")) return Error("No such FutureProg.");
				if (prog is not null && !ExposureProgContract.Valid(prog, field)) return Error(ExposureProgContract.Description);
				if (field == "applicability") ruleEdit.ApplicabilityProg = prog;
				if (field == "intensity") ruleEdit.IntensityProg = prog;
				if (field == "notification") ruleEdit.NotificationProg = prog;
				break;
			case "message": ruleEdit.Message = value.EqualTo("none") ? null : value; break;
			default: return Error(Help.SubstituteANSIColour());
		}
		var errors = ruleEdit.ValidationErrors(gas).ToArray();
		if (errors.Length > 0) return Error(string.Join("\n", errors));
		definitions[index - 1] = ruleEdit;
		EnvironmentalExposureService.For(world).Refresh();
		actor.OutputHandler.Send("Reaction updated."); return true;
		bool Error(string text) { actor.OutputHandler.Send(text); return false; }
	}
}

public static class ExposureProgContract
{
	public const string Description = "Exposure hooks take perceivable target, number body ID, number part ID, number material ID, text source, text category, text route, location room, number layer, number strength, number seconds, number quantity. Notifications append number damage, pain, stun, consumption. Applicability returns boolean; intensity returns number; notification returns void.";
	public static readonly ProgVariableTypes[] Parameters = { ProgVariableTypes.Perceivable, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Location, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number };
	public static bool Valid(IFutureProg prog, string kind) => prog.ReturnType == (kind == "applicability" ? ProgVariableTypes.Boolean : kind == "intensity" ? ProgVariableTypes.Number : ProgVariableTypes.Void) &&
		prog.MatchesParameters(kind == "notification" ? Parameters.Concat(Enumerable.Repeat(ProgVariableTypes.Number, 4)).ToArray() : Parameters);
}
