#if DEBUG
#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal static class SeederReplayRunner
{
	internal static ProductReplayRunner Runner { get; } = new([typeof(SkillSeeder)], context =>
		context.Accounts.Any() || context.SeederChoices.Any()
			? "Replay profiles require a freshly migrated, unseeded database. This database already has bootstrap or seeder-answer data."
			: null);

	internal static SeederReplayValidationResult Validate(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders) =>
		Runner.Validate(profile, seeders);

	internal static SeederReplayRunResult Run(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory, Version version, Action<string>? progress = null) =>
		Runner.Run(profile, seeders, contextFactory, version, progress);

	internal static SeederReplayRunResult RunPrefixBefore(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		Type stopBeforeSeederType, Func<FuturemudDatabaseContext> contextFactory, Version version, Action<string>? progress = null) =>
		Runner.RunPrefixBefore(profile, seeders, stopBeforeSeederType, contextFactory, version, progress);
}

internal static class DebugSeederReplayProfiles
{
	private const string DebugPassword = "DebugReplayOnly!2026";

	internal static IReadOnlyList<SeederReplayProfile> All { get; } =
	[
		CreateProfile(
			"medieval-standard",
			"Medieval Standard",
			"Full Debug replay with medieval time, health and culture.",
			"Debug Medieval",
			"1300",
			"Medieval Age",
			"medieval",
			"medieval"),
		CreateProfile(
			"renaissance-standard",
			"Renaissance Standard",
			"Full Debug replay with Renaissance health and culture.",
			"Debug Renaissance",
			"1500",
			"Medieval Age",
			"renaissance",
			"renaissance"),
		CreateProfile(
			"early-modern-standard",
			"Early Modern Standard",
			"Full Debug replay based on the Europe/1703 DemoMUD-style configuration.",
			"Debug Early Modern",
			"1703",
			"Early Modern Age",
			"earlymodern",
			"earthrenaissanceeurope"),
		CreateProfile(
			"industrial-neutral",
			"Industrial Neutral",
			"Full Debug replay of the foundations for an Industrial Neutral item replay.",
			"Debug Industrial",
			"1897",
			"Early Modern Age",
			"earlymodern",
			"earthrenaissanceeurope"),
		CreateProfile(
			"industrial-custom",
			"Industrial Custom",
			"Full Debug replay of the foundations for an Industrial Custom item replay.",
			"Debug Industrial Custom",
			"1897",
			"Early Modern Age",
			"earlymodern",
			"earthrenaissanceeurope"),
		CreateProfile(
			"armageddon-native",
			"Armageddon Native World",
			"Full Debug replay with Armageddon time, celestial, skill and setting packages, followed by the native worldfile import.",
			"Debug Armageddon",
			"1",
			"Medieval Age",
			"medieval",
			"earthmedievaleurope",
			armageddon: true)
	];

	private static SeederReplayProfile CreateProfile(
		string id,
		string name,
		string description,
		string gameName,
		string year,
		string economyEra,
		string healthTechLevel,
		string culturePack,
		bool armageddon = false)
	{
		var epoch = $"1 January {year}";
		var moonEpoch = $"21 January {year}";
		return new SeederReplayProfile(id, name, description,
		[
			Step<CoreDataSeeder>(
				("gamename", gameName),
				("environmentalexposure", "no"),
				("account", "Admin"),
				("password", DebugPassword),
				("email", "debug-replay@futuremud.com")),
			Step<TimeSeeder>(
				("secondsmultiplier", armageddon ? "6" : "2"),
				("mode", armageddon ? "armageddon" : "gregorian-uk"),
				("startyear", year),
				("ardaage", "3")),
			Step<CelestialSeeder>(
				("packages", armageddon ? "Armageddon" : "EarthSun EarthMoon Jupiter Rails Scripts MorningStar MorningGlow"),
				("suncalendar", "1"),
				("sunepoch", epoch),
				("mooncalendar", "1"),
				("moonepoch", moonEpoch),
				("gasgiantcalendar", "1"),
				("gasgiantsunepoch", epoch),
				("gasgiantmoonepoch", epoch),
				("authoredcalendar", "1")),
			Step<AttributeSeeder>(
				("choice", "labmud"),
				("decorator", "labmud")),
			Step<CurrencySeeder>(("currency", "pounds")),
			Step<AIStorytellerSeeder>(("install", "yes")),
			Step<SkillPackageSeeder>(
				("skillpreset", armageddon ? "Armageddon" : "Complex-NonGerund-Modern"),
				("branching", armageddon ? "no" : "yes"),
				("skillcapmodel", armageddon ? "class" : "rpi"),
				("skillgainmodel", armageddon ? "armageddon" : "labmud"),
				("complexity", armageddon ? "armageddon" : "complex"),
				("gerund", "no"),
				("modern", armageddon ? "no" : "yes")),
			Step<EconomySeeder>(
				("era", economyEra),
				("currency", "1"),
				("zone", "1"),
				("shopper-scale", "standard")),
			Step<ClanSeeder>(),
			Step<HumanSeeder>(
				("balance", "combat-rebalance"),
				("model", "full"),
				("inventory", "hands"),
				("sever", "yes"),
				("bones", "full"),
				("distinctive", "yes"),
				("nonbinary", "no"),
				("includeextraperson", "no")),
        Step<WeatherSeeder>(("operation", "install"), ("weathercelestial", "1"), ("rain", "full")),
			Step<LawSeeder>(
				("name", "Debug Authority"),
				("currency", "1"),
				("createai", "yes"),
				("separatepowers", "yes"),
				("punishmentlevel", "western"),
				("classes", "immune sovereign noble officer soldier enforcer citizen non-citizen slave pet felon criminal"),
				("religiouslaws", "yes"),
				("penaltyunits", "4")),
			Step<ChargenSeeder>(
				("rpp", "yes"),
				("rppname", "Roleplay Point/RPP"),
				("bp", "yes"),
				("class", armageddon ? "yes" : "no"),
				("subclass", armageddon ? "yes" : "no"),
				("role-first", "race"),
				("attributemode", "points"),
				("skillmode", "boosts"),
				("merits", "merit"),
				("customdescs", "no")),
			Step<ArenaSeeder>(
				("arena-name", $"{gameName} Arena"),
				("arena-zone", "1")),
			Step<UsefulSeeder>(
				("ai", "yes"),
				("covers", "yes"),
				("items", "yes"),
				("modernitems", "no"),
				("autobuilder", "yes"),
				("hints", "yes"),
				("dreams", "yes"),
				("dream-eras", "old")),
			Step<TrapSeeder>(),
			Step<AgricultureSeeder>(),
			Step<HealthSeeder>(("techlevel", healthTechLevel)),
			Step<CookingSeeder>(),
			Step<CombatSeeder>(
				("installmuskets", armageddon ? "no" : "yes"),
				("installguns", "no"),
				("random", "static"),
				("parryoption", "yes"),
				("skilloption", armageddon ? "armageddon" : "weapons"),
				("messagestyle", "sparse")),
			Step<CultureSeeder>(
				("culturepacks", culturePack),
				("seednames", "yes"),
				("seedlanguages", "yes"),
				("seedsignedlanguages", "no"),
				("seedheritage", "yes")),
			Step<StockMeritsSeeder>(),
			Step<AnimalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse")),
			Step<ArmageddonSettingSeeder>((ArmageddonSettingSeeder.InstallQuestion, "no"),
				(ArmageddonSettingSeeder.FullInstallQuestion, armageddon ? "yes" : "no"), (ArmageddonSettingSeeder.WeatherQuestion, "no"), (ArmageddonSettingSeeder.ModeQuestion, "Classic"),
				(ArmageddonSettingSeeder.CapacityQuestion, armageddon ? "4" : "0"), (ArmageddonSettingSeeder.NativeBindingsQuestion, "{}"),
				(ArmageddonSettingSeeder.BindingsQuestion, "none")),
			Step<MythicalAnimalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse")),
			Step<WildlifeCatalogueSeeder>(),
			Step<RobotSeeder>(),
			(armageddon
				? Step<ArmageddonWorldfileSeeder>()
				: NotApplicableStep<ArmageddonWorldfileSeeder>(
					"This profile explicitly declines the Armageddon Setting; the native worldfile applies only when Armageddon Installation settings exist.",
					db => ArmageddonInstallation.Read(db) is null)),
			Step<AnimalButcherySeeder>(),
			Step<SupernaturalSeeder>(
				("model", "full"),
				("random", "static"),
				("messagestyle", "sparse")),
			Step<PsionicsSeeder>(("install-psionics", "yes")),
			Step<EnvironmentalExposureSeeder>(("natural", "yes"), ("fantasy", "yes"))
		]);
	}

	private static SeederReplayStep Step<T>(params (string Id, string Answer)[] answers)
		where T : IDatabaseSeeder
	{
		return new SeederReplayStep(typeof(T), answers
			.Select(x => new SeederReplayAnswer(x.Id, x.Answer))
			.ToList());
	}

	private static SeederReplayStep NotApplicableStep<T>(string reason,
		Func<FuturemudDatabaseContext, bool> predicate, params (string Id, string Answer)[] answers)
		where T : IDatabaseSeeder => new(typeof(T), answers
		.Select(x => new SeederReplayAnswer(x.Id, x.Answer)).ToList(),
			SeederReplayDisposition.NotApplicable, reason, predicate);
}
#endif
