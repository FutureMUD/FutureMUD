#nullable enable

using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.RPG.Checks;

namespace FutureMUD.FuryCalmStockNativeHarness;

internal static class FuryCalmStockEntry
{
	private static int Main(string[] args)
	{
		if (args.Length != 2 || (args[0] != "profile-preflight" && args[0] != "paid-native"))
		{
			Console.Error.WriteLine("Use profile-preflight|paid-native <new output file>."); return 2;
		}
		if (File.Exists(args[1])) { Console.Error.WriteLine("Refusing to overwrite an existing receipt."); return 2; }
		var operation = Guid.NewGuid();
		if (args[0] == "paid-native")
		{
			Write(args[1], new { OperationIdentity = operation, Mode = args[0], Status = "blocked",
				Reason = "Main-owned casting/admitted-attack/pair-cessation hooks are not allocated or integrated.",
				RuntimeQualified = false, DatabaseStarted = false, ProcessStarted = false,
				AcceptancePlan = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "AcceptancePlan.json"))).RootElement.Clone() });
			return 2;
		}
		try
		{
			// Explicit fixture identities; these do not identify a seeded or production world.
			var fury = ArmageddonRousedFuryStock.Profile(10, 0.5, 5, 20, new(101, 102, 103, 104, 105, 106, 107));
			var calm = ArmageddonStillAngerStock.Profile(11, 11, 20,
				Enumerable.Range(1, 7).Select(x => new KeyValuePair<int, Difficulty>(x, Difficulty.Normal)));
			var profiles = new[] { fury, calm }.Select(x => EmotionalStockProfile.Read(XElement.Parse(x.SaveToXml().ToString()))).ToArray();
			Require(XNode.DeepEquals(fury.SaveToXml(), profiles[0].SaveToXml()), "Fury profile persistence");
			Require(XNode.DeepEquals(calm.SaveToXml(), profiles[1].SaveToXml()), "Calm profile persistence");
			var low = profiles[0].ResolveLifetime(1, 107, SpellPower.ExtremelyWeak, (_, _) => throw new InvalidOperationException("Grade1 must not draw."));
			var high = profiles[0].ResolveLifetime(7, 107, SpellPower.ExtremelyStrong, (_, upper) => upper);
			Require(low.Duration == TimeSpan.FromSeconds(2400) && low.State.EndurancePoints == 2, "Earth grade1");
			Require(high.Duration == TimeSpan.FromSeconds(16800) && high.State.EndurancePoints == 31, "Earth grade7");
			var furyRefresh = fury.ResolveLifetime(7, 107, SpellPower.ExtremelyStrong, (_, upper) => upper, TimeSpan.FromSeconds(21600), low.State);
			Require(furyRefresh.State == low.State && furyRefresh.Duration == TimeSpan.FromSeconds(21600), "Fury retained state/cap");
			var calmHigh = calm.ResolveLifetime(7, 999, SpellPower.ExtremelyWeak, (_, _) => throw new InvalidOperationException("Calm must not draw."));
			var calmRefresh = calm.ResolveLifetime(1, 999, SpellPower.ExtremelyStrong, (_, _) => 0, TimeSpan.FromSeconds(14400), calmHigh.State);
			Require(calmRefresh.State == calmHigh.State && calmRefresh.Duration == TimeSpan.FromSeconds(14400), "Calm strongest state/cap");
			Require(EmotionalSpellPolicy.Counter(7, 1).RemainingIncomingGrade == 6, "source counter");
			Require(ArmageddonStillAngerStock.NextRawCap == 60 && ArmageddonRousedFuryStock.PrerequisiteRaw == 80, "canonical acquisition metadata");
			var furyDefinition = ArmageddonRousedFuryStock.Definition(1, 2, profiles[0]);
			var calmDefinition = ArmageddonStillAngerStock.Definition(1, 2, profiles[1]);
			Require(furyDefinition.Element("StockIdentity")!.Value == ArmageddonRousedFuryStock.Key &&
				calmDefinition.Element("StockIdentity")!.Value == ArmageddonStillAngerStock.Key, "stock definitions");
			Write(args[1], new { OperationIdentity = operation, Mode = args[0], Status = "passed",
				Checks = 9, Kind = "profile_preparation_only", RuntimeQualified = false, DatabaseStarted = false,
				IndependentDatabaseReloadProved = false, NativeCombatProved = false,
				Profiles = profiles.Select(x => x.SaveToXml().ToString()).ToArray(),
				Assemblies = new[] { typeof(ArmageddonRousedFuryStock).Assembly, typeof(SpellPower).Assembly, Assembly.GetExecutingAssembly() }
					.Distinct().Select(x => new { x.Location, Sha256 = Hash(x.Location) }).ToArray() });
			return 0;
		}
		catch (Exception error)
		{
			Write(args[1], new { OperationIdentity = operation, Mode = args[0], Status = "failed", Error = error.Message, RuntimeQualified = false });
			return 1;
		}
	}
	private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
	private static void Require(bool value, string assertion) { if (!value) throw new InvalidOperationException(assertion); }
	private static void Write(string path, object receipt)
	{
		using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
		JsonSerializer.Serialize(stream, receipt, new JsonSerializerOptions { WriteIndented = true });
	}
}
