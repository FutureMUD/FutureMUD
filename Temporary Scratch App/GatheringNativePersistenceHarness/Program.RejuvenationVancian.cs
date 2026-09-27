#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Powers;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using CompiledFutureProg = MudSharp.FutureProg.FutureProg;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void RunRejuvenationVancianProbe(NativeRuntime runtime, string connectionString, Cell cell,
		MagicSpell spell, HarnessClock clock, EnvironmentalMagicCoordinator coordinator)
	{
		var actor = runtime.Actor;
		using var context = NewIndependentContext(connectionString);
		long Policy(string kind, string result)
		{
			var signature = kind == "power" ? (Return: ProgVariableTypes.Boolean, Parameters: new[] { ProgVariableTypes.Character }) : VancianPolicy.Signatures[kind];
			var model = new Db.FutureProg { FunctionName = $"rejuvenation_{kind}", FunctionComment = "Owned native acceptance policy.",
				FunctionText = $"return {result}", ReturnTypeDefinition = signature.Return.ToStorageString(), Category = "Harness",
				Subcategory = "Rejuvenation", StaticType = (int)FutureProgStaticType.NotStatic };
			for (var i = 0; i < signature.Parameters.Length; i++) model.FutureProgsParameters.Add(new()
			{ ParameterIndex = i, ParameterName = $"p{i}", ParameterTypeDefinition = signature.Parameters[i].ToStorageString() });
			context.FutureProgs.Add(model); context.SaveChanges();
			var prog = new CompiledFutureProg(runtime.World, model.FunctionName, signature.Return,
				signature.Parameters.Select((type, index) => Tuple.Create(type, $"p{index}")), model.FunctionText)
			{ Id = model.Id, StaticType = FutureProgStaticType.NotStatic };
			Require(prog.Compile(), prog.CompileError);
			((All<IFutureProg>)runtime.World.FutureProgs).Add(prog);
			return prog.Id;
		}
		var level = Policy("casterlevel", "3"); var change = Policy("canchangeknown", "true");
		var candidate = Policy("candidates", "true"); var limit = Policy("limit", "2"); var count = Policy("count", "2");
		var rule = Guid.NewGuid(); var allowance = Guid.NewGuid();
		var definition = XElement.Parse(((SkillLevelBasedMagicCapability)runtime.Capability).SaveToXml());
		definition.Element("Gathering")?.Remove();
		definition.Add(new XElement("Vancian", new XAttribute("version", 1), new XAttribute("basePower", (int)SpellPower.Standard),
			new XAttribute("step", 1), new XAttribute("outcome", (int)Outcome.Pass), new XAttribute("loadouts", 10),
			new XAttribute("recovery", "PreparationAction"), new XAttribute("prepareSeconds", 1), new XAttribute("sleepSeconds", 1),
			new XAttribute("intervalSeconds", 0), new XElement("Policy", new XAttribute("name", "casterlevel"), new XAttribute("id", level)),
			new XElement("Policy", new XAttribute("name", "canchangeknown"), new XAttribute("id", change)),
			new XElement("Repertoire", new XAttribute("key", rule), new XAttribute("alias", "known"), new XAttribute("name", "Known"),
				new XAttribute("order", 0), new XAttribute("source", "Selected"), new XAttribute("min", 0), new XAttribute("max", 3),
				new XAttribute("candidate", candidate), new XAttribute("limit", limit), new XAttribute("bookPolicy", "EveryRefresh")),
			new XElement("Allowance", new XAttribute("key", allowance), new XAttribute("alias", "first"), new XAttribute("name", "First"),
				new XAttribute("order", 0), new XAttribute("mode", "Memorised"), new XAttribute("level", 1), new XAttribute("version", 1),
				new XAttribute("min", 0), new XAttribute("max", 1), new XAttribute("count", count), new XAttribute("eligibility", 0), new XElement("Rule", rule))));
		var model = new Db.MagicCapability { Name = "Restorers", MagicSchoolId = spell.School.Id, CapabilityModel = "vancian",
			PowerLevel = 1, Definition = definition.ToString() };
		context.MagicCapabilities.Add(model); context.SaveChanges();
		var capability = new VancianMagicCapability(model, runtime.World);
		Require(capability.ConfigurationErrors().Count == 0, string.Join("; ", capability.ConfigurationErrors()));
		((All<IMagicCapability>)runtime.World.MagicCapabilities).Add(capability);
		var merit = new Mock<IMagicCapabilityMerit>();
		merit.SetupGet(x => x.Capabilities).Returns(new IMagicCapability[] { runtime.Capability, capability });
		merit.Setup(x => x.Applies(It.IsAny<IHaveMerits>())).Returns(true);
		actor.SetMerits([merit.Object]);
		var store = new VancianStateStore();
		var service = new VancianMagicService(runtime.World, store, clock);
		var services = (ConditionalWeakTable<IFuturemud, VancianMagicService>)typeof(VancianMagicService)
			.GetField("Services", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		services.Remove(runtime.World); services.Add(runtime.World, service);
		var selection = service.CommitKnown(actor, capability, service.State(actor, capability).Version,
			new Dictionary<Guid, IReadOnlyList<long>> { [rule] = [spell.Id] });
		Require(selection.Success, selection.Message);
		Require(service.EditLoadout(actor, capability, "new", "restoration").Success, "R-T12 loadout creation failed.");
		for (var i = 1; i <= 2; i++)
		{
			var assigned = service.EditLoadout(actor, capability, "assign", "restoration", assignment:
				new(allowance, 1, i, rule, spell.Id, 1, spell.SpellLevel, VancianPolicy.Power(capability, spell.SpellLevel, 1)));
			Require(assigned.Success, assigned.Message);
		}
		Require(service.SelectLoadout(actor, capability, "restoration").Success, "R-T12 loadout selection failed.");
		var refresh = service.RequestRefresh(actor, capability); Require(refresh.Success, refresh.Message);
		clock.Advance(TimeSpan.FromSeconds(1));
		actor.EffectsOfType<VancianTimedAction>().Single().ExpireEffect();
		Require(store.Read(actor.Id, capability.Id).Slots.Count(x => x.Status == VancianSlotStatus.Prepared) == 2, "R-T12 preparation did not persist two slots.");
		var template = (RejuvenateLandEffect)spell.SpellEffects.Single();
		Require(template.BuildingCommand(actor, new StringStack("budget 9 + spelllevel + castinglevel + casterlevel")) &&
			template.BuildingCommand(actor, new StringStack("rate castinglevel")), "R-T12 numerical context configuration failed.");
		var scar = cell.EnvironmentState.ScarDamage; var mana = actor.MagicResourceAmounts[runtime.Resource];
		MagicModule.MagicGeneric(actor, $"{spell.School.SchoolVerb} vancian {capability.Id} cast known first \"{spell.Name}\" 1");
		var child = cell.Effects.OfType<SpellRejuvenateLandEffect>().Single();
		var treatment = coordinator.InspectTreatments(cell).Single(x => x.Id == child.TreatmentId);
		Require(treatment.InitialBudget == Math.Min(scar, 13) && treatment.Rate == 1 && actor.MagicResourceAmounts[runtime.Resource] == mana - 0.25,
			"R-T12 direct Vancian invocation did not capture its levels/rate or paid cost exactly once.");
		Require(store.Read(actor.Id, capability.Id).Slots.Single(x => x.Ordinal == 1).Status == VancianSlotStatus.Spent &&
			store.Read(actor.Id, capability.Id).Slots.Single(x => x.Ordinal == 2).Status == VancianSlotStatus.Prepared, "R-T12 Vancian cost changed the wrong slots.");
		clock.Advance(TimeSpan.FromSeconds(60)); coordinator.Pump();
		Require(cell.EnvironmentState.ScarDamage == scar - 1, "R-T12 Vancian treatment did not advance through the real coordinator.");
		cell.RemoveEffect(child.ParentEffect, true);
		Require(template.BuildingCommand(actor, new StringStack("budget 12")) && template.BuildingCommand(actor, new StringStack("rate 2")), "Unable to restore plain direct spell expressions.");
		// The real learned-power route is independent of the remaining prepared slot.
		var powerModel = new Db.MagicPower { Name = "Restore Through Power", MagicSchoolId = spell.School.Id, PowerModel = "spellbacked",
			Blurb = "Harness independently granted spell", ShowHelp = "invoke standard", Definition = new XElement("Definition",
				new XElement("Spell", spell.Id), new XElement("Verb", "invoke"), new XElement("IsPsionic", false),
				new XElement("CanInvokePowerProg", Policy("power", "true")), new XElement("WhyCantInvokePowerProg", 0)).ToString() };
		context.MagicPowers.Add(powerModel); context.SaveChanges();
		var power = MagicPowerFactory.LoadPower(powerModel, runtime.World);
		((All<IMagicPower>)runtime.World.MagicPowers).Add(power);
		actor.LearnPower(power);
		var version = store.Read(actor.Id, capability.Id).Version; mana = actor.MagicResourceAmounts[runtime.Resource];
		MagicModule.MagicGeneric(actor, $"{spell.School.SchoolVerb} invoke standard");
		child = cell.Effects.OfType<SpellRejuvenateLandEffect>().Single();
		Require(actor.MagicResourceAmounts[runtime.Resource] == mana - 0.25 && store.Read(actor.Id, capability.Id).Version == version &&
			store.Read(actor.Id, capability.Id).Slots.Single(x => x.Ordinal == 2).Status == VancianSlotStatus.Prepared,
			"R-T12 independently granted power incorrectly spent a Vancian slot or missed ordinary costs.");
		cell.RemoveEffect(child.ParentEffect, true);
		Console.WriteLine($"R-T12-native=passed capability:{capability.Id} direct-Vancian-slot:1-spent slot:2-prepared captured-budget:{treatment.InitialBudget} captured-rate:1 paid:0.25 independent-power-paid:0.25 independent-power-slot-debit:0");
	}
}
