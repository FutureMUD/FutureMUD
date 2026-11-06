#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private sealed record NpcCombatTraits(TraitDefinition Profile, TraitDefinition Offence, TraitDefinition Defence);
	internal sealed record NpcCombatRating(double Offence, double Defence);
	private NpcCombatTraits? _npcCombatTraits;
	private readonly Dictionary<(int Race, int Guild, int Age), (double Strength, double Agility, double Reaction)> _npcCombatMeans = new();
	internal const double NpcCombatRatingCap = 1_000_000;

	internal static string? NpcCombatCalibrationError(ArmageddonWorldfilePackage source)
	{
		var tables = source.CombatCalibration;
		if (tables is null || tables.AgilityReaction.Length != 101 || tables.StrengthAgePermille.Length != 21 ||
			tables.AgilityAgePermille.Length != 21 || tables.StrengthAgePermille.Any(x => x < 0) || tables.AgilityAgePermille.Any(x => x < 0))
			return "Regenerate the private worldfile package with scripts/armageddon-worldfile-package.py: qualified combat age/reaction tables are required.";
		if (source.Races.Any(x => x.Index == 0) || source.Guilds.Any(x => x.Index == 0))
			return "Race/guild zero are source sentinels and must not be supplied as file-backed combat profiles.";
		return null;
	}

	private void InstallNpcCombatRatings()
	{
		if (NpcCombatCalibrationError(_source) is { } error) throw new InvalidOperationException(error);
		var names = new[] { "Worldfile Combat Profile", "Worldfile Offence", "Worldfile Defence" };
		if (_db.TraitDefinitions.Any(x => names.Contains(x.Name)))
			throw new InvalidOperationException("Worldfile combat rating traits already exist; this import is one-shot.");
		var checks = new[] { CheckType.MeleeWeaponCheck, CheckType.NaturalWeaponAttack, CheckType.DodgeCheck, CheckType.ParryCheck, CheckType.BlockCheck }
			.Select(type => _db.Checks.Include(x => x.CheckTemplate).Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
				.SingleOrDefault(x => x.Type == (int)type) ?? throw new InvalidOperationException($"Native {type} check is required for combat calibration."))
			.ToArray();
		foreach (var check in checks)
			if (check.CheckTemplate.CheckMethod != "Standard" || check.CheckTemplate.FailIfTraitMissingMode != (int)FailIfTraitMissingType.DoNotFail)
				throw new InvalidOperationException($"{(CheckType)check.Type} requires the native Standard, non-abject-on-missing skill check foundation for optional combat profiles.");
		var decorator = _db.TraitDecorators.OrderBy(x => x.Id).FirstOrDefault(x => x.Type == "SimpleNumeric");
		if (decorator is null)
		{
			decorator = new TraitDecorator { Id = Next<TraitDecorator>(), Name = "Worldfile Combat Ratings", Type = "SimpleNumeric", Contents = "" };
			_db.TraitDecorators.Add(decorator);
		}
		var improver = _db.Improvers.OrderBy(x => x.Id).FirstOrDefault(x => x.Type == "non-improving");
		if (improver is null)
		{
			improver = new Improver { Id = Next<Improver>(), Name = "Worldfile Fixed Combat Ratings", Type = "non-improving", Definition = "<Definition/>" };
			_db.Improvers.Add(improver);
		}
		TraitDefinition Rating(string name, string key, double cap)
		{
			var expression = new TraitExpression { Id = Next<TraitExpression>(), Name = name + " Cap", Expression = cap.ToString("R", CultureInfo.InvariantCulture) };
			_db.TraitExpressions.Add(expression);
			var trait = new TraitDefinition
			{
				Id = Next<TraitDefinition>(), Name = name, Alias = key, Type = 0, OwnerScope = 1, DerivedType = 0,
				TraitGroup = "Worldfile Combat", Hidden = true, ShowInScoreCommand = false, ShowInAttributeCommand = false, DisplayAsSubAttribute = false,
				ExpressionId = expression.Id, DecoratorId = decorator.Id, ImproverId = improver.Id, BranchMultiplier = 0,
				AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				LearnableProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				TeachableProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
				ValueExpression = "", ChargenBlurb = "Imported combat baseline; not a trainable skill.", TeachDifficulty = 7, LearnDifficulty = 7
			};
			_db.TraitDefinitions.Add(trait);
			Record("combat.rating." + key, nameof(TraitDefinition), trait.Id, null, new { name, Maximum = cap, Trainable = false });
			return trait;
		}
		_npcCombatTraits = new(Rating(names[0], "profile", 1), Rating(names[1], "offence", NpcCombatRatingCap), Rating(names[2], "defence", NpcCombatRatingCap));
		foreach (var check in checks)
		{
			var original = check.TraitExpression;
			if (original.Expression.Contains("armcombat_", StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("An existing check expression already reserves worldfile combat parameters.");
			var type = (CheckType)check.Type;
			// Reuse the exact native skill expression, including its parameter labels, so a
			// fallback branch cannot introduce duplicate learning bindings for the same skill.
			var active = original.Expression;
			var expression = new TraitExpression
			{
				Id = Next<TraitExpression>(), Name = "Worldfile " + type + " Rating Formula",
				Expression = NpcCombatCheckFormula(type, _npcCombatTraits.Profile.Id, _npcCombatTraits.Offence.Id, _npcCombatTraits.Defence.Id, active, original.Expression)
			};
			foreach (var parameter in original.TraitExpressionParameters)
				expression.TraitExpressionParameters.Add(new TraitExpressionParameters { Parameter = parameter.Parameter,
					TraitDefinitionId = parameter.TraitDefinitionId, CanImprove = parameter.CanImprove, CanBranch = parameter.CanBranch });
			_db.TraitExpressions.Add(expression);
			check.TraitExpressionId = expression.Id;
			check.TraitExpression = expression;
			Record("combat.check." + type, nameof(Check), check.Type, null, new
			{
				OriginalExpressionId = original.Id, OriginalFormula = original.Expression, ExpressionId = expression.Id,
				Formula = expression.Expression, NativeCheckTemplate = check.CheckTemplateId,
				Scope = "Profiled actors only; original native formula for other actors. Dodge is shared with ranged defence."
			});
		}
		Flush();
	}

	// Both sides receive half the baseline difference, but each side contributes its own active skill once.
	internal static string NpcCombatCheckFormula(CheckType type, long profile, long offence, long defence, string activeSkill, string nativeFormula)
	{
		var selfProfile = $"armcombat_self_profile:{profile}{{raw,noimprove,nobranch}}";
		var targetProfile = $"armcombat_target_profile:!{profile}{{raw}}";
		var attack = type is CheckType.MeleeWeaponCheck or CheckType.NaturalWeaponAttack;
		var own = $"armcombat_self_rating:{(attack ? offence : defence)}{{raw,noimprove,nobranch}}";
		var other = $"armcombat_target_rating:!{(attack ? defence : offence)}{{raw}}";
		var delta = $"if({targetProfile}>0,({own}-{other})/2,{own}-68)";
		string Response(string value) => $"(5+90/(1+exp(max(-40,min(40,-({value})/21)))))";
		var basis = Response(attack ? $"({delta})+({activeSkill})" : delta);
		var target = attack ? basis : $"({basis}+(100-{basis})*max(0,min(100,({activeSkill})))/100)";
		return $"if({selfProfile}>0,{target},({nativeFormula}))";
	}

	private void ApplyNpcCombatRatings(ArmWorldNpc source, IDictionary<long, double> skills)
	{
		var traits = _npcCombatTraits ?? throw new InvalidOperationException("Install NPC combat rating definitions before NPC templates.");
		var race = source.Identity[6] == 0 ? null : _source.Races.SingleOrDefault(x => x.Index == source.Identity[6]);
		var guild = source.Identity[3] == 0 ? null : _source.Guilds.SingleOrDefault(x => x.Index == source.Identity[3]);
		var key = (source.Identity[6], source.Identity[3], source.Age);
		if (!_npcCombatMeans.TryGetValue(key, out var means))
		{
			means = SourceNpcCombatMeans(source.Age, race, guild, _source.CombatCalibration!);
			_npcCombatMeans.Add(key, means);
		}
		var rating = SourceNpcCombatRating(source, race, guild, means);
		if (!double.IsFinite(rating.Offence) || !double.IsFinite(rating.Defence) || Math.Abs(rating.Offence) >= NpcCombatRatingCap || Math.Abs(rating.Defence) >= NpcCombatRatingCap)
			throw new InvalidOperationException($"NPC {source.Vnum} exceeds the qualified combat rating range.");
		skills[traits.Profile.Id] = 1;
		skills[traits.Offence.Id] = rating.Offence;
		skills[traits.Defence.Id] = rating.Defence;
	}

	internal static NpcCombatRating SourceNpcCombatRating(ArmWorldNpc npc, ArmWorldRace? race, ArmWorldGuild? guild,
		(double Strength, double Agility, double Reaction) means) => new(
		npc.Combat[1] + (race?.Combat[0] ?? 0) + (guild?.Combat[2] ?? 0) + means.Strength + means.Agility + means.Reaction,
		npc.Combat[2] + (race?.Combat[1] ?? 0) + (guild?.Combat[3] ?? 0) + 2 * means.Agility + means.Reaction);

	internal static (double Strength, double Agility, double Reaction) SourceNpcCombatMeans(int age, ArmWorldRace? race, ArmWorldGuild? guild, ArmWorldCombatCalibration tables)
	{
		var strength = SourceNpcStatDistribution(age, race, guild, 0, tables.StrengthAgePermille);
		var agility = SourceNpcStatDistribution(age, race, guild, 1, tables.AgilityAgePermille);
		return (strength.Sum(x => x.Key * x.Value), agility.Sum(x => x.Key * x.Value), agility.Sum(x => tables.AgilityReaction[x.Key] * x.Value));
	}

	internal static Dictionary<int, double> SourceNpcStatDistribution(int age, ArmWorldRace? race, ArmWorldGuild? guild, int index, int[] multipliers)
	{
		var maximumAge = race?.Identity.ElementAtOrDefault(1) ?? 0;
		var bucket = maximumAge > 0 ? (int)Math.Clamp(20L * (age == -1 ? maximumAge / 2 : age) / maximumAge, 0, 20) : -1;
		var dice = race?.Stats[index] ?? "10";
		var match = Regex.Match(dice, @"^\s*(\d+)\s*d\s*(\d+)(?:\s*([+-])\s*(\d+))?\s*$", RegexOptions.IgnoreCase);
		var distribution = new Dictionary<int, double> { [0] = 1 };
		var bonus = guild?.Stats[index] ?? 0;
		if (match.Success)
		{
			var count = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
			var sides = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
			if (count <= 0 || count > 100 || sides <= 0 || sides > 1000) throw new InvalidOperationException("Unsupported source combat dice.");
			if (match.Groups[4].Success) bonus += int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture) * (match.Groups[3].Value == "-" ? -1 : 1);
			for (var die = 0; die < count; die++)
			{
				var next = new Dictionary<int, double>();
				foreach (var (sum, probability) in distribution)
					for (var face = 1; face <= sides; face++) next[sum + face] = next.GetValueOrDefault(sum + face) + probability / sides;
				distribution = next;
			}
		}
		else if (int.TryParse(dice, NumberStyles.Integer, CultureInfo.InvariantCulture, out var constant)) bonus += constant;
		else throw new InvalidOperationException($"Unsupported source combat stat generator: {dice}.");
		var result = new Dictionary<int, double>();
		foreach (var (value, probability) in distribution)
		{
			var adjusted = value + bonus;
			if (bucket >= 0) adjusted = (int)((long)adjusted * multipliers[bucket] / 1000);
			adjusted = Math.Clamp(adjusted, 1, 100);
			result[adjusted] = result.GetValueOrDefault(adjusted) + probability;
		}
		return result;
	}
}
