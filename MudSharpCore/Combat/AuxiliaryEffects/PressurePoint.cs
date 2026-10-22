#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.AuxiliaryEffects;

public static class PressurePointAnatomy
{
	public static IReadOnlyDictionary<string, IReadOnlyCollection<string>> StockAliases { get; } =
		new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.InvariantCultureIgnoreCase)
		{
			["neck"] = new[] { "neck", "bneck" },
			["throat"] = new[] { "throat", "neck" },
			["spine"] = new[] { "spine", "uback", "lback", "abdomen", "belly" },
			["stomach"] = new[] { "abdomen", "belly" },
			["leftarm"] = new[] { "lupperarm", "lforearm" },
			["rightarm"] = new[] { "rupperarm", "rforearm" },
			["leftleg"] = new[] { "lhip", "lthigh", "lshin" },
			["rightleg"] = new[] { "rhip", "rthigh", "rshin" }
		};

	public static bool Matches(string location, IEnumerable<string> bodyPartNames,
		IReadOnlyDictionary<string, IReadOnlyCollection<string>> aliases)
	{
		return aliases.TryGetValue(location, out var parts) && bodyPartNames.Any(name =>
			parts.Contains(name, StringComparer.InvariantCultureIgnoreCase));
	}

	public static bool TryParseSelection(StringStack arguments, out string selection)
	{
		selection = arguments.PopSpeech().ToLowerInvariant().Replace(" ", "");
		if (selection is "left" or "right") selection += arguments.PopSpeech().ToLowerInvariant();
		return arguments.IsFinished && StockAliases.ContainsKey(selection);
	}

	public static ILimb? ResolveLimb(IBody body, string location,
		IReadOnlyCollection<string> aliases)
	{
		var type = location.EndsWith("arm", StringComparison.Ordinal) ? LimbType.Arm : LimbType.Leg;
		var side = location.StartsWith("left", StringComparison.Ordinal) ? Alignment.Left : Alignment.Right;
		var limbs = body.Bodyparts
			.Where(part => new[] { part.Name }.Concat(part.Keywords)
				.Any(name => aliases.Contains(name, StringComparer.InvariantCultureIgnoreCase)))
			.Select(body.GetLimbFor)
			.Where(limb => limb is not null && body.Limbs.Contains(limb) && limb.LimbType == type &&
				limb.RootBodypart.Alignment.LeftRightOnly() == side)
			.Distinct()
			.Take(2)
			.ToArray();
		return limbs.Length == 1 ? limbs[0] : null;
	}
}

/// <summary>
/// Contextual manual auxiliary effect for the Jihaen poke. Stock values are
/// encoded here as defaults; trait and duration remain builder-configurable.
/// </summary>
internal sealed class PressurePoint : OpposedAuxiliaryEffectBase, IContextualAuxiliaryEffect
{
	private static readonly string[] Locations = { "neck", "spine", "throat", "stomach", "leftarm", "rightarm", "leftleg", "rightleg" };
	private readonly Dictionary<string, HashSet<string>> _anatomyAliases = new(StringComparer.InvariantCultureIgnoreCase);

	public PressurePoint(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		var traitId = long.Parse(root.Attribute("agilitytrait")!.Value, CultureInfo.InvariantCulture);
		AgilityTrait = gameworld.Traits.Get(traitId) ?? throw new ApplicationException("Missing agility trait for pressure point effect.");
		AgilityPenalty = Math.Clamp(ReadSigned(root, "agilitypenalty", -3.0), -100.0, 0.0);
		NeckDurationSeconds = Read(root, "neckseconds", 20.0);
		SpineDurationSeconds = Read(root, "spineseconds", 5.0);
		ThroatDurationSeconds = Read(root, "throatseconds", 20.0);
		StomachDurationSeconds = Read(root, "stomachseconds", 45.0);
		NauseaIntensity = Read(root, "nauseaintensity", 4.0);
		foreach (var location in Locations.Where(IsLimbLocation))
			_limbDurations[location] = Read(root, location + "seconds", 5.0);
		foreach (var location in Locations)
		{
			var defaults = PressurePointAnatomy.StockAliases[location];
			var configured = root.Element("Anatomy")?.Elements("Location")
				.FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), location, StringComparison.InvariantCultureIgnoreCase))?
				.Attribute("parts")?.Value;
			_anatomyAliases[location] = new HashSet<string>(
				(configured ?? string.Join(' ', defaults)).Split(' ', StringSplitOptions.RemoveEmptyEntries),
				StringComparer.InvariantCultureIgnoreCase);
		}
	}

	private PressurePoint(IFuturemud gameworld, ITraitDefinition agilityTrait, ITraitDefinition defenseTrait) :
		this(new XElement("Effect", new XAttribute("agilitytrait", agilityTrait.Id),
			new XAttribute("defensetrait", defenseTrait.Id),
			new XAttribute("defensedifficulty", (int)Difficulty.Normal)), gameworld) { }

	private static double Read(XElement root, string name, double fallback) =>
		root.Attribute(name) is XAttribute value && double.TryParse(value.Value, NumberStyles.Float,
			CultureInfo.InvariantCulture, out var parsed) && double.IsFinite(parsed) && parsed >= 0.0
			? Math.Min(parsed, 86400.0) : fallback;

	private static double ReadSigned(XElement root, string name, double fallback) =>
		root.Attribute(name) is XAttribute value && double.TryParse(value.Value, NumberStyles.Float,
			CultureInfo.InvariantCulture, out var parsed) && double.IsFinite(parsed)
			? parsed : fallback;

	public ITraitDefinition AgilityTrait { get; private set; }
	public double AgilityPenalty { get; private set; }
	public double NeckDurationSeconds { get; private set; }
	public double SpineDurationSeconds { get; private set; }
	public double ThroatDurationSeconds { get; private set; }
	public double StomachDurationSeconds { get; private set; }
	public double NauseaIntensity { get; private set; }
	private readonly Dictionary<string, double> _limbDurations = new(StringComparer.InvariantCultureIgnoreCase);
	private static bool IsLimbLocation(string location) => location is "leftarm" or "rightarm" or "leftleg" or "rightleg";

	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("pressurepoint", (action, actor, command) =>
		{
			var agilityTrait = command.IsFinished
				? actor.Gameworld.Traits.GetByName("Agility")
				: actor.Gameworld.Traits.GetByIdOrName(command.PopSpeech());
			var defenseTrait = command.IsFinished
				? actor.Gameworld.Traits.GetByName("Dodge")
				: actor.Gameworld.Traits.GetByIdOrName(command.PopSpeech());
			if (agilityTrait is null || defenseTrait is null || !command.IsFinished)
			{
				actor.OutputHandler.Send("Specify the agility and defense traits, or omit them to use Agility and Dodge.");
				return null;
			}
			return new PressurePoint(actor.Gameworld, agilityTrait, defenseTrait);
		}, "A contextual pressure-point strike opposed by the configured defense trait. Syntax: pressurepoint [agility trait] [defense trait].", true);
	}

	protected override string TypeName => "pressurepoint";
	protected override string EffectName => "Pressure Point";
	protected override string AmountName => "Effect";
	protected override double DefaultFlatAmount => 0.0;
	protected override double DefaultPerDegreeAmount => 0.0;
	protected override double DefaultMaximumAmount => 0.0;

	public override XElement Save()
	{
		var root = new XElement("Effect");
		SaveCommonAttributes(root);
		root.Add(new XAttribute("agilitytrait", AgilityTrait.Id.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("agilitypenalty", AgilityPenalty.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("neckseconds", NeckDurationSeconds.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("spineseconds", SpineDurationSeconds.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("throatseconds", ThroatDurationSeconds.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("stomachseconds", StomachDurationSeconds.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("nauseaintensity", NauseaIntensity.ToString(CultureInfo.InvariantCulture)));
		foreach (var (location, seconds) in _limbDurations)
			root.Add(new XAttribute(location + "seconds", seconds.ToString(CultureInfo.InvariantCulture)));
		root.Add(new XElement("Anatomy", Locations.Select(location => new XElement("Location",
			new XAttribute("name", location),
			new XAttribute("parts", string.Join(' ', _anatomyAliases[location].OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)))))));
		return root;
	}

	public override string DescribeForShow(ICharacter actor) =>
		$"Pressure Point | agility {AgilityPenalty.ToString("N1", actor)} for {NeckDurationSeconds.ToString("N0", actor)}s (neck), paralysis {SpineDurationSeconds.ToString("N0", actor)}s (spine), silence {ThroatDurationSeconds.ToString("N0", actor)}s (throat), nausea intensity {NauseaIntensity.ToString("N2", actor)} for {StomachDurationSeconds.ToString("N0", actor)}s (stomach), limb numbness: {_limbDurations.Select(x => $"{x.Key} {x.Value.ToString("N0", actor)}s").ListToString()}";
	public override string Show(ICharacter actor) => base.Show(actor);
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopForSwitch();
		if (field is "agility" or "agilitytrait")
		{
			var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
			if (trait is null)
			{
				actor.OutputHandler.Send("There is no such agility trait.");
				return false;
			}
			AgilityTrait = trait;
			actor.OutputHandler.Send($"The neck penalty will now affect {trait.Name.ColourValue()}.");
			return true;
		}
		var durationLocation = field.EndsWith("seconds", StringComparison.Ordinal) ? field[..^7] : field;
		if (field is "penalty" or "agilitypenalty" or "nauseaintensity" || Locations.Contains(durationLocation))
		{
			if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ||
				!double.IsFinite(value) || (field is "penalty" or "agilitypenalty" ? value > 0.0 || value < -100.0 : value < 0.0 || value > 86400.0))
			{
				actor.OutputHandler.Send("Use a finite agility penalty from -100 to 0, or a duration from 0 to 86400 seconds.");
				return false;
			}
			switch (field)
			{
				case "penalty": case "agilitypenalty": AgilityPenalty = value; break;
				case "neck": case "neckseconds": NeckDurationSeconds = value; break;
				case "spine": case "spineseconds": SpineDurationSeconds = value; break;
				case "throat": case "throatseconds": ThroatDurationSeconds = value; break;
				case "stomach": case "stomachseconds": StomachDurationSeconds = value; break;
				case "nauseaintensity": NauseaIntensity = value; break;
				default: _limbDurations[durationLocation] = value; break;
			}
			actor.OutputHandler.Send("The pressure point effect has been updated.");
			return true;
		}
		if (field is "trait" or "skill" or "defensetrait" or "difficulty" or "diff" or "minimum" or "minimumdegree" or "mindegree" or "min")
			return base.BuildingCommand(actor, new StringStack($"{field} {command.SafeRemainingArgument}"));
		if (field is "anatomy" or "parts")
		{
			var location = command.PopForSwitch();
			var parts = command.SafeRemainingArgument.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (!Locations.Contains(location, StringComparer.InvariantCultureIgnoreCase) || parts.Length == 0)
			{
				actor.OutputHandler.Send($"Use anatomy <{string.Join('|', Locations)}> <one or more bodypart aliases>.");
				return false;
			}
			_anatomyAliases[location] = new HashSet<string>(parts, StringComparer.InvariantCultureIgnoreCase);
			actor.OutputHandler.Send($"The {location.ColourValue()} pressure point will match: {string.Join(", ", parts).ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send($"Use agility <trait>, penalty <-100..0>, <{string.Join('|', Locations)}> <seconds>, nauseaintensity <value>, anatomy <location> <bodypart aliases>, or a defense setting such as trait, difficulty, or minimum.");
		return false;
	}
	public override void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome) { }

	public bool TryParseContext(ICharacter actor, ICharacter target, StringStack arguments,
		out ManualCombatActionContext? context, out string error)
	{
		context = null;
		if (!PressurePointAnatomy.TryParseSelection(arguments, out var selection))
		{
			error = $"Choose exactly one target location: {Locations.ListToString()}.";
			return false;
		}

		var limb = IsLimbLocation(selection) ? PressurePointAnatomy.ResolveLimb(target.Body, selection, _anatomyAliases[selection]) : null;
		context = new ManualCombatActionContext(selection, target.Body.Id, limb?.Id);
		return IsValidContext(actor, target, context, out error);
	}

	public bool IsValidContext(ICharacter actor, ICharacter target, ManualCombatActionContext context,
		out string error)
	{
		var selection = context.Selection.ToLowerInvariant();
		if (!Locations.Contains(selection, StringComparer.InvariantCultureIgnoreCase))
		{
			error = "That is not a valid pressure point location.";
			return false;
		}
		if (target.State.IsDead())
		{
			error = "You cannot use a pressure point strike against a dead target.";
			return false;
		}
		if (context.TargetBodyId.HasValue && context.TargetBodyId != target.Body.Id)
		{
			error = "The target's body has changed since you selected the pressure point.";
			return false;
		}
		var hasFreeHand = actor.Body.HoldLocs.Any(part =>
			actor.Body.CanUseBodypart(part) == CanUseBodypartResult.CanUse &&
			!actor.Body.HeldItemsFor(part).Any() && !actor.Body.WieldedItemsFor(part).Any());
		if (!hasFreeHand)
		{
			error = "You need one free, functioning hand to use a pressure point strike.";
			return false;
		}

		var aliases = _anatomyAliases.ToDictionary(x => x.Key, x => (IReadOnlyCollection<string>)x.Value,
			StringComparer.InvariantCultureIgnoreCase);
		var hasAnatomy = PressurePointAnatomy.Matches(selection,
			target.Body.Bodyparts.SelectMany(part => new[] { part.Name }.Concat(part.Keywords)), aliases);
		if (selection == "stomach")
			hasAnatomy &= target.Body.Organs.Any(x => x.BodypartType == BodypartTypeEnum.Stomach);
		if (IsLimbLocation(selection))
		{
			var limb = PressurePointAnatomy.ResolveLimb(target.Body, selection, _anatomyAliases[selection]);
			hasAnatomy &= limb is not null && context.TargetLimbId == limb.Id;
		}
		if (target.Race.BloodLiquid is null || !hasAnatomy)
		{
			error = $"{target.HowSeen(actor, true)} has no compatible organic {selection} anatomy.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome,
		ManualCombatActionContext context, bool opposedSuccess = false)
	{
		var selection = context.Selection.ToLowerInvariant();
		if (target is not ICharacter character ||
			!IsValidContext(attacker, character, context, out _) ||
			!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return;
		if (!opposedSuccess && !TryGetOpposedSuccess(attacker, character, outcome, out _))
		{
			SendEcho(FailureEcho, attacker, character);
			return;
		}

		var (seconds, message) = selection switch
		{
			"neck" => (NeckDurationSeconds, "Your agility is impaired by a pressure point strike."),
			"spine" => (SpineDurationSeconds, "Your body locks in paralysis from a pressure point strike."),
			"throat" => (ThroatDurationSeconds, "Your throat is muted by a pressure point strike."),
			"stomach" => (StomachDurationSeconds, "Your stomach churns with severe nausea from a pressure point strike."),
			var location when IsLimbLocation(location) => (_limbDurations[location], string.Empty),
			_ => (0.0, string.Empty)
		};
		if (seconds <= 0.0) return;

		var duplicate = selection switch
		{
			"neck" => character.EffectsOfType<MundaneTraitPenaltyEffect>().Any(x => x.Trait == AgilityTrait),
			"spine" => character.EffectsOfType<MundaneParalysisEffect>().Any(),
			"throat" => character.EffectsOfType<MundaneSilenceEffect>().Any(),
			"stomach" => character.Body.EffectsOfType<MundaneNauseaEffect>().Any(),
			var location when IsLimbLocation(location) => character.Body.EffectsOfType<LimbNumbnessEffect>().Any(x => x.LimbId == context.TargetLimbId),
			_ => true
		};
		if (duplicate) return;

		var owner = IsLimbLocation(selection) || selection == "stomach" ? (IPerceivable)character.Body : character;
		IEffect effect = selection switch
		{
			"neck" => new MundaneTraitPenaltyEffect(character, AgilityTrait, AgilityPenalty),
			"spine" => new MundaneParalysisEffect(character),
			"throat" => new MundaneSilenceEffect(character),
			"stomach" => new MundaneNauseaEffect(character.Body, NauseaIntensity),
			_ => new LimbNumbnessEffect(character.Body, character.Body.Limbs.Single(x => x.Id == context.TargetLimbId))
		};
		if (!MudSharp.NPC.AI.CommandExecutionScope.TryContinue()) return;
		MudSharp.NPC.AI.CommandExecutionScope.MarkCommitted();
		owner.AddEffect(effect, TimeSpan.FromSeconds(seconds));
		if (!string.IsNullOrEmpty(message)) character.OutputHandler.Send(message);
	}
}
