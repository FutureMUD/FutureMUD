#nullable enable

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp;
using MudSharp.Body;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record LegacyRemainsReader(string Database, FixtureIds Fixture, long BodyId, long ItemId,
		bool Corpse, bool MissingBody, bool CurrentBody, bool FinalDeath, bool MissingOwner = false, long? WoundId = null);

	private static string LegacyRemainsDefinition(FixtureIds fixture, long bodyId, bool corpse, bool finalDeath) => corpse
		? $"<Definition><OriginalCharacter>{fixture.CharacterId}</OriginalCharacter><OriginalBody>{bodyId}</OriginalBody><RemainsContext>{(finalDeath ? 0 : 2)}</RemainsContext><Model>1</Model><DecayPoints>0</DecayPoints><DecayState>0</DecayState><TimeOfDeath>2026-10-02T12:00:00Z</TimeOfDeath></Definition>"
		: $"<Definition><OriginalCharacterId>{fixture.CharacterId}</OriginalCharacterId><OriginalBodyId>{bodyId}</OriginalBodyId><Model>1</Model><DecayPoints>0</DecayPoints><DecayState>0</DecayState><Parts><Part>{fixture.BodypartId}</Part></Parts><Contents/><Wounds/></Definition>";

	private static int RunLegacyRemainsAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		using (var schema = NewIndependentContext(database.ConnectionString)) schema.Database.Migrate();
		var fixture = FixtureSeed.Create(database, "arm03_legacy_remains", true);
		var inputs = new List<LegacyRemainsReader>();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var oldBody = AddLifecycleBody(db, fixture.BodyId); oldBody.Weight = 60000; db.SaveChanges();
			foreach (var (corpse, missing, current, finalDeath, missingOwner) in new[]
			{
				(true, false, false, false, false), (false, false, false, false, false),
				(true, true, false, false, false), (false, true, false, false, false),
				(true, false, true, false, false), (false, false, true, false, false), (true, false, true, true, false),
				(true, true, false, true, false), (true, false, false, true, true), (false, false, false, false, true),
				(true, false, false, true, false)
			})
			{
				var item = NewLifecycleItem(); db.GameItems.Add(item); db.SaveChanges();
				var bodyId = current ? fixture.BodyId : missing ? oldBody.Id + 1000000 : oldBody.Id;
				long? woundId = null;
				if (!corpse)
				{
					var wound = new Db.Wound { GameItemId = item.Id, BodypartProtoId = fixture.BodypartId,
						OriginalDamage = 7, CurrentDamage = 7, CurrentPain = 11, CurrentStun = 13,
						DamageType = (int)DamageType.Slashing, ActorOriginId = fixture.CharacterId,
						WoundType = "SimpleOrganic", RealTimeOfWound = DateTime.UtcNow,
						ExtraInformation = $"<Definition><DamageDescription>cut</DamageDescription><BleedStatus>{(int)BleedStatus.Bleeding}</BleedStatus></Definition>" };
					db.Wounds.Add(wound); db.SaveChanges(); woundId = wound.Id;
				}
				var definition = LegacyRemainsDefinition(fixture, bodyId, corpse, finalDeath);
				if (woundId.HasValue) definition = definition.Replace("<Wounds/>", $"<Wounds><Wound>{woundId}</Wound></Wounds>");
				db.GameItemComponents.Add(new() { GameItemId = item.Id, GameItemComponentProtoId = 0, Definition = definition });
				inputs.Add(new(database.Name, fixture, bodyId, item.Id, corpse, missing, current, finalDeath, missingOwner, woundId));
			}
			db.SaveChanges();
			Require(!db.CharacterBodyRetirements.Any() && !db.MagicSpellOwnedEntities.Any() &&
				!db.CharacterBodies.Any(x => x.BodyId == oldBody.Id) && !db.CharacterBodySources.Any(x => x.BodyId == oldBody.Id),
				"Legacy fixture unexpectedly supplied retirement or form ownership.");
		}
		Console.WriteLine("ARM03-legacy-seed=passed persisted-positive-body-IDs existing-missing-and-current-body cases no-old-body-form-source-retirement-or-spell-ownership");
		var allPassed = true;
		foreach (var input in inputs)
		{
			var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
			start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--legacy-remains-reader");
			start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
			using var process = Process.Start(start)!;
			var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
			if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Legacy remains reader exceeded 60 seconds."); }
			Console.Write(output.GetAwaiter().GetResult());
			if (process.ExitCode == 0) continue;
			allPassed = false;
			Console.Error.WriteLine($"legacy-reader-failed corpse:{input.Corpse} missing-body:{input.MissingBody} current-body:{input.CurrentBody} final-death:{input.FinalDeath}: {error.GetAwaiter().GetResult()}");
		}
		Require(allPassed, "Legacy remains reload/presentation/release acceptance failed; all reader failures are retained above.");
		Console.WriteLine("ARM03-legacy-acceptance=passed eleven-separate-process-readers exact-read-without-cleanup-authority safe-unresolved-body context-specific-current-body-validation runtime-consumer-boundaries");
		return 0;
	}

	private static int RunLegacyRemainsReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<LegacyRemainsReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, false); PrepareLifecycleRuntime(native); LoadRetirementForms(database, native);
		var bodies = new All<IBody>(); bodies.Add(native.Actor.Body); native.WorldMock.SetupGet(x => x.Bodies).Returns(bodies);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IBody>())).Callback<IBody>(body => bodies.Add(body));
		native.WorldMock.Setup(x => x.TryGetCharacter(input.Fixture.CharacterId, It.IsAny<bool>())).Returns(input.MissingOwner ? null! : native.Actor);
		var model = new Mock<ICorpseModel>(MockBehavior.Strict);
		model.SetupGet(x => x.Id).Returns(1); model.SetupGet(x => x.EdiblePercentage).Returns(0.5);
		model.Setup(x => x.GetDecayState(It.IsAny<double>())).Returns(DecayState.Fresh);
		model.Setup(x => x.Describe(It.IsAny<DescriptionType>(), It.IsAny<DecayState>(), native.Actor,
			It.Is<IBody>(x => x.Id == input.BodyId), It.IsAny<IPerceiver>(), It.IsAny<double>())).Returns("legacy corpse");
		model.Setup(x => x.DescribeSevered(It.IsAny<DescriptionType>(), It.IsAny<DecayState>(), native.Actor,
			It.Is<IBody>(x => x.Id == input.BodyId), It.IsAny<IPerceiver>(), It.IsAny<ISeveredBodypart>(), It.IsAny<double>())).Returns("legacy severed part");
		var models = new All<ICorpseModel>(); models.Add(model.Object); native.WorldMock.SetupGet(x => x.CorpseModels).Returns(models);
		Db.GameItemComponent stored;
		using (var db = NewIndependentContext(database.ConnectionString)) stored = db.GameItemComponents.AsNoTracking().Single(x => x.GameItemId == input.ItemId);
		var parent = RetirementItem(native, input.ItemId);
		GameItemComponent component;
		if (input.Corpse) component = AttachRetirementCorpse(native, parent, stored);
		else
		{
			var prototype = (BodypartGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(BodypartGameItemComponentProto));
			var part = new BodypartGameItemComponent(stored, prototype, parent.Object); component = part;
			parent.Setup(x => x.GetItemType<IButcherable>()).Returns(part);
			parent.Setup(x => x.GetItemType<ISeveredBodypart>()).Returns(part);
			parent.Setup(x => x.GetItemType<ICorpse>()).Returns((ICorpse)null!);
			parent.SetupGet(x => x.Components).Returns([part]);
		}
		var items = new All<IGameItem>(); items.Add(parent.Object); native.WorldMock.SetupGet(x => x.Items).Returns(items);
		Require(input.CurrentBody ? ReferenceEquals(bodies.Get(input.BodyId), native.Actor.CurrentBody) : bodies.Get(input.BodyId) is null,
			"Legacy reader did not begin with only the current body cached.");
		var remains = (IButcherable)component;
		var description = component.Decorate(native.Actor, "", "", DescriptionType.Full, false, PerceiveIgnoreFlags.None);
		var weight = component.ComponentWeight;
		var buoyancy = component.ComponentBuoyancy(1.0);
		_ = remains.OriginalRace;
		Require(remains.OriginalBodyId == input.BodyId && double.IsFinite(weight) && weight >= 0 && double.IsFinite(buoyancy) &&
			(input.CurrentBody || !ReferenceEquals(remains.OriginalBody, native.Actor.CurrentBody)),
			"Legacy remains changed identity, redirected to current body or produced invalid physical values.");
		var unresolved = input.MissingOwner || input.MissingBody || input.Corpse && input.CurrentBody && !input.FinalDeath;
		if (unresolved)
		{
			Require(remains.OriginalBody is null && remains.OriginalRace is null && weight == 0 && !string.IsNullOrWhiteSpace(description),
				"Unresolved legacy body was fabricated or its presentation was unsafe.");
		}
		else
		{
			Require(remains.OriginalBody.Id == input.BodyId && ReferenceEquals(bodies.Get(input.BodyId), remains.OriginalBody) && weight > 0 &&
				description == (input.Corpse ? "legacy corpse" : "legacy severed part"), "Existing legacy body did not resolve exactly for presentation.");
			Require(!native.Actor.TryCleanupRetiredBody(remains.OriginalBody, parent.Object), "Read-only legacy reference fabricated cleanup authority.");
		}
		if (input.Corpse)
		{
			var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
			typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, new List<IGameItemComponent> { component });
			typeof(GameItem).GetField("_overridingWoundBehaviourComponent", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, component);
			typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.EffectHandler))!.SetValue(item, new EffectHandler(item));
			Require(double.IsFinite(item.IlluminationProvided), "Legacy corpse item illumination was unsafe.");
			if (unresolved) Require(!item.PassiveSufferDamage(Mock.Of<IDamage>()).Any(), "Unresolved corpse forwarded damage to a survivor.");
		}
		VerifyLegacyRemainsRuntimeBoundaries(input, native, parent, component, unresolved);
		component.Delete();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.Characters.Single(x => x.Id == input.Fixture.CharacterId).BodyId == input.Fixture.BodyId &&
				db.Bodies.Any(x => x.Id == input.Fixture.BodyId) && db.Wounds.Any(x => x.Id == input.Fixture.ExistingWoundId && x.ActorOriginId == input.Fixture.CharacterId) &&
				(input.MissingBody || db.Bodies.Any(x => x.Id == input.BodyId)) && !db.CharacterBodyRetirements.Any() && !db.MagicSpellOwnedEntities.Any(),
				"Legacy release deleted an unowned body or changed canonical identity, history or ownership metadata.");
			Require(!input.WoundId.HasValue || !db.Wounds.Any(x => x.Id == input.WoundId.Value), "Part release did not delete its own persisted wound.");
			db.GameItems.Remove(db.GameItems.Find(input.ItemId)!); db.SaveChanges();
		}
		var marker = input.Corpse ? "corpse" : "part";
		var scenario = LegacyRemainsScenario(input);
		Console.WriteLine($"ARM03-legacy-{scenario}{marker}=passed separate-process-reload description-weight-buoyancy-race-and-Delete-callback safe corpse-item-illumination-and-unresolved-damage exact-positive-ID-kept canonical-current-body-and-history-preserved no-cleanup-authority-inferred");
		return 0;
	}
}
