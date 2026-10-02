using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

#nullable enable
namespace MudSharp_Unit_Tests;

internal sealed class MagicCastingFixture
{
	public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICharacter> Staff { get; } = new();
	public Mock<IBody> Body { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICheck> Check { get; } = new();
	public Mock<ITrait> NativeSkill { get; } = new();
	public List<IMagicCapability> Capabilities { get; } = [];
	public List<IMagicCapability> ActiveCapabilities { get; } = [];
	public List<IMagicSpell> Spells { get; } = [];
	public List<ITraitDefinition> Traits { get; } = [];
	public List<ITraitExpression> Expressions { get; } = [];
	public List<IMagicResource> Resources { get; } = [];
	public Dictionary<IMagicResource, double> Balances { get; } = [];
	public Dictionary<long, double> Skills { get; } = new() { [1] = 42, [2] = 62, [3] = 10 };
	public List<string> Messages { get; } = [];
	public CastingMemoryStore Store { get; } = new();
	public MagicCastingService Service { get; private set; } = null!;
	public MagicSpell Spell { get; }
	public SkillLevelBasedMagicCapability Earth { get; }
	public SkillLevelBasedMagicCapability Sorcerer { get; }
	public IMagicSchool School { get; }
	public DateTime Now { get; set; } = new(2026, 9, 27, 1, 0, 0, DateTimeKind.Utc);
	public int SkillUses { get; private set; }
	public int Rolls { get; private set; }
	public int Flushes { get; private set; }
	public double RandomValue { get; set; } = 0.1;
	public int Samples { get; private set; }
	public Action<string>? Checkpoint { get; set; }
	public Outcome Outcome { get; set; } = Outcome.Pass;

	public MagicCastingFixture()
	{
		var school = new Mock<IMagicSchool>();
		school.SetupGet(x => x.Id).Returns(1); school.SetupGet(x => x.Name).Returns("Earth");
		school.SetupGet(x => x.SchoolVerb).Returns("earth"); school.SetupGet(x => x.SchoolAdjective).Returns("earthen");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.Cyan);
		School = school.Object;
		for (var i = 1; i <= 3; ++i)
		{
			var trait = new Mock<ITraitDefinition>(); var id = i;
			trait.SetupGet(x => x.Id).Returns(id); trait.SetupGet(x => x.Name).Returns($"Proficiency {id}");
			trait.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			trait.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			Traits.Add(trait.Object);
		}
		foreach (var id in new[] { 10, 11, 12 })
		{
			var resource = new Mock<IMagicResource>(); resource.SetupGet(x => x.Id).Returns(id);
			resource.SetupGet(x => x.Name).Returns($"reserve {id}");
			resource.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
			Resources.Add(resource.Object); Balances[resource.Object] = 100;
		}
		World.SetupGet(x => x.MagicSchools).Returns(Collection(() => new[] { School }));
		World.SetupGet(x => x.Traits).Returns(Collection(() => Traits));
		World.SetupGet(x => x.TraitExpressions).Returns(Collection(() => Expressions));
		World.SetupGet(x => x.MagicResources).Returns(Collection(() => Resources));
		World.SetupGet(x => x.MagicSpells).Returns(Collection(() => Spells));
		World.SetupGet(x => x.MagicCapabilities).Returns(Collection(() => Capabilities));
		World.SetupGet(x => x.Characters).Returns(Collection<ICharacter>(() => []));
		World.SetupGet(x => x.LegalAuthorities).Returns(Collection<ILegalAuthority>(() => []));
		World.SetupGet(x => x.DefaultPlane).Returns((IPlane)null!);
		var known = new Mock<IFutureProg>(); known.SetupGet(x => x.Id).Returns(1);
		known.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
		World.SetupGet(x => x.FutureProgs).Returns(Collection(() => new[] { known.Object }));
		World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(Check.Object);
		World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(Check.Object);
		Check.SetupGet(x => x.MaximumDifficultyForImprovement).Returns(Difficulty.Impossible);
		Check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(() => { Rolls++; return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck, Outcome)); });
		Actor.SetupGet(x => x.Id).Returns(100); Actor.SetupGet(x => x.InstanceId).Returns(100);
		Actor.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
		Actor.SetupGet(x => x.GetObject).Returns(() => Actor.Object);
		Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		Actor.SetupGet(x => x.Capabilities).Returns(() => ActiveCapabilities); Actor.SetupGet(x => x.Body).Returns(Body.Object);
		var cell = new Mock<ICell>() { DefaultValue = DefaultValue.Mock };
		Actor.SetupGet(x => x.Location).Returns(cell.Object);
		Body.SetupGet(x => x.Id).Returns(200); Body.SetupGet(x => x.Gameworld).Returns(World.Object);
		Body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		Body.SetupGet(x => x.FunctioningFreeHands).Returns(new[] { new Mock<IGrab>().Object });
		Body.Setup(x => x.Communications.CanVocalise(Body.Object)).Returns(true);
		Body.Setup(x => x.Communications.CanVocalise(Body.Object, It.IsAny<MudSharp.Form.Audio.AudioVolume>()))
			.Returns(() => Body.Object.Communications.CanVocalise(Body.Object));
		Actor.Setup(x => x.CombinedEffectsOfType<MagicSpellLockout>()).Returns([]);
		Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
		Actor.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => Skills.GetValueOrDefault(x.Id));
		Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>()))
			.Returns<ITraitDefinition, TraitBonusContext>((x, _) => Skills.GetValueOrDefault(x.Id));
		Actor.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => Skills.ContainsKey(x.Id));
		Actor.Setup(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>())).Returns<ITraitDefinition, double>((t, n) => { Skills[t.Id] = n; return true; });
		Actor.Setup(x => x.GetTrait(It.IsAny<ITraitDefinition>())).Returns(NativeSkill.Object);
		NativeSkill.Setup(x => x.TraitUsed(Actor.Object, It.IsAny<Outcome>(), It.IsAny<Difficulty>(), It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()))
			.Returns(() => { SkillUses++; return true; });
		Actor.SetupGet(x => x.MagicResourceAmounts).Returns(Balances);
		Actor.Setup(x => x.CanUseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns<IMagicResource, double>((r, n) => Balances.GetValueOrDefault(r) >= n);
		Actor.Setup(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns<IMagicResource, double>((r, n) => { if (Balances[r] < n) return false; Balances[r] -= n; return true; });
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((s, _, _) => Messages.Add(s));
		Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		Staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true); Staff.SetupGet(x => x.Id).Returns(999);
		Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 1, Name = "Cost", Expression = "5*grade" }, World.Object));
		Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 2, Name = "Duration", Expression = "variable+grade*10" }, World.Object));
		Spell = NewSpell(1, "Stone Skin", "<Effect type='boost' trait='3' bonus='0' context='0' />");
		Assert.IsTrue(Spell.BuildingCommand(Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(Spell.BuildingCommand(Actor.Object, new StringStack("grades scalar add target 0 boost Bonus -grade")));
		Earth = NewCapability(1, 1, 11, true); Sorcerer = NewCapability(2, 2, 12, false);
		ActiveCapabilities.Add(Earth); ActiveCapabilities.Add(Sorcerer);
		Restart();
	}

	public static IUneditableAll<T> Collection<T>(Func<System.Collections.Generic.IEnumerable<T>> values) where T : class, IFrameworkItem
	{
		var result = new Mock<IUneditableAll<T>>();
		result.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => values().FirstOrDefault(x => x.Id == id)!);
		result.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((s, _) => values().FirstOrDefault(x => x.Id.ToString() == s || x.Name.EqualTo(s))!);
		result.Setup(x => x.GetEnumerator()).Returns(() => values().GetEnumerator());
		result.As<System.Collections.IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => values().GetEnumerator());
		return result.Object;
	}
	public MagicSpell NewSpell(long id, string name, string effects)
	{
		var spell = new MagicSpell(new MudSharp.Models.MagicSpell
		{
			Id = id, Name = name, MagicSchoolId = 1, SpellKnownProgId = 1, CastingTraitDefinitionId = 1,
			CastingDifficulty = (int)Difficulty.Easy, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			EffectDurationExpressionId = 2, CastingEmote = "casting", FailCastingEmote = "failing", ExclusiveDelay = 5,
			Definition = $"<Spell><Trigger type='self'><MinimumPower>0</MinimumPower><MaximumPower>10</MaximumPower></Trigger><Costs><Cost resource='10' expression='1' /></Costs><Effects>{effects}</Effects><CasterEffects/><Plan><Phase/></Plan></Spell>"
		}, World.Object);
		Spells.Add(spell); return spell;
	}
	public SkillLevelBasedMagicCapability NewCapability(long id, long trait, long reserve, bool passive, XElement? casting = null)
	{
		var root = XElement.Parse("<Definition><ConcentrationTrait>1</ConcentrationTrait><ConcentrationCapabilityExpression>1</ConcentrationCapabilityExpression><ConcentrationDifficultyExpression>5</ConcentrationDifficultyExpression><Regenerators/></Definition>");
		root.Add(casting ?? new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()),
			new XAttribute("enabled", true), new XAttribute("trait", trait), new XAttribute("source", 10), new XAttribute("reserve", reserve),
			new XAttribute("passive", passive), new XAttribute("startingVersion", 1), Admission(1)));
		var capability = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(new MudSharp.Models.MagicCapability
		{ Id = id, Name = id == 1 ? "Earth" : "Sorcerer", MagicSchoolId = 1, PowerLevel = 1, CapabilityModel = "skilllevel", Definition = root.ToString() }, World.Object);
		Capabilities.Add(capability); return capability;
	}
	public static XElement Admission(long spell) => new("Admission", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell), new XAttribute("starting", true), new XAttribute("min", 1), new XAttribute("max", 7));
	public void Restart() { Service = new(World.Object, Store, () => Now, () => { Samples++; return RandomValue; }, x => Checkpoint?.Invoke(x), () => Flushes++); World.SetupGet(x => x.MagicCasting).Returns(Service); }
	public void Acquire(int grade = 2) => Store.Write(acquired: new(100, 1, grade, 1, Now, "test acquisition", DateTime.UnixEpoch, Store.Acquisition(100, 1)?.Version ?? 0));
	public MagicCastingIntent Intent(int grade = 3, bool overreach = true, long capability = 1) => new(Actor.Object, capability, 1, grade, overreach, "self");
}

internal sealed class CastingMemoryStore : IMagicCastingStateStore
{
	public Dictionary<(long Owner, long Spell), AcquiredSpell> Acquired { get; } = [];
	public Dictionary<(long Owner, long Trait), CastingSkillOpportunity> Opportunities { get; } = [];
	public Dictionary<(long Owner, Guid Capability), CastingEnrolment> Enrolments { get; } = [];
	public Dictionary<Guid, CastingOperation> Operations { get; } = [];
	public int Writes { get; private set; }
	public Action<CastingOperation?>? BeforeWrite { get; set; }
	public IReadOnlySet<long> CappedTraits(long characterId) => Operations.Values
		.Where(x => x.CharacterId == characterId && x.Stage is MagicCastingStateStore.SkillCapRecorded or MagicCastingStateStore.CappedSupportGranted).Select(x => x.TraitId).ToHashSet();
	public CastingSupportAcquisition? SupportGrant(long characterId, Guid identity, Guid key) => Operations.Values
		.Where(x => x.CharacterId == characterId && MagicCastingStateStore.IsSupportRecord(x.Stage)).Select(MagicCastingStateStore.ReadSupportGrant)
		.SingleOrDefault(x => x.CapabilityIdentity == identity && x.GrantKey == key);
	public AcquiredSpell? Acquisition(long characterId, long spellId) => Acquired.GetValueOrDefault((characterId, spellId));
	public CastingSkillOpportunity? Opportunity(long characterId, long traitId) => Opportunities.GetValueOrDefault((characterId, traitId));
	public CastingEnrolment? Enrolment(long characterId, Guid capabilityIdentity) => Enrolments.GetValueOrDefault((characterId, capabilityIdentity));
	public IReadOnlyList<CastingOperation> Unresolved(long? characterId = null) => Operations.Values.Where(x => (!characterId.HasValue || x.CharacterId == characterId) && !MagicCastingStateStore.TerminalStages.Contains(x.Stage)).ToArray();
	public CastingOperation? Operation(Guid id) => Operations.GetValueOrDefault(id);
	public void Write(CastingOperation? operation = null, AcquiredSpell? acquired = null, CastingSkillOpportunity? opportunity = null, CastingEnrolment? enrolment = null)
	{
		BeforeWrite?.Invoke(operation); Writes++;
		if (acquired is not null) { Assert.AreEqual(Acquisition(acquired.CharacterId, acquired.SpellId)?.Version ?? 0, acquired.Version); Acquired[(acquired.CharacterId, acquired.SpellId)] = acquired with { Version = acquired.Version + 1 }; }
		if (opportunity is not null) { Assert.AreEqual(Opportunity(opportunity.CharacterId, opportunity.TraitId)?.Version ?? 0, opportunity.Version); Opportunities[(opportunity.CharacterId, opportunity.TraitId)] = opportunity with { Version = opportunity.Version + 1 }; }
		if (enrolment is not null) Enrolments[(enrolment.CharacterId, enrolment.CapabilityIdentity)] = enrolment;
		if (operation is not null) Operations[operation.Id] = operation;
	}
}
