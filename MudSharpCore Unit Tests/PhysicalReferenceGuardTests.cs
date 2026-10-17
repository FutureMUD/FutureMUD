#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Body;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Framework;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhysicalReferenceGuardTests
{
	private static PhysicalReferenceTargets Targets(PhysicalEntityKind kind, long id = 17) => new([new(kind, id, "candidate")]);
	private static string Effect(string type, string fields) => $"<Effects><Effect><Type>{type}</Type><Effect>{fields}</Effect></Effect></Effects>";

	[DataTestMethod]
	[DataRow("BodyBackup", "<BackupBodyId>17</BackupBodyId>")]
	[DataRow("SpellBodyBackup", "<BackupBodyId>&#49;&#55;</BackupBodyId>")]
	[DataRow("ForcedTransformationBaseline", "<BaselineBodyId>17</BaselineBodyId>")]
	[DataRow("SpellTransformForm", "<FormBodyId>17</FormBodyId><PriorBodyId>23</PriorBodyId>")]
	[DataRow("SpellLiveBodyPossession", "<AnchorCharacterId>10</AnchorCharacterId><TargetCharacterId>11</TargetCharacterId><TargetBodyId>17</TargetBodyId>")]
	public void Effects_KnownBodyFields_RetainOnlyTheirTypedBody(string type, string fields)
	{
		var refs = PhysicalReferenceCodecs.Effects(Effect(type, fields)).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[DataTestMethod]
	[DataRow("HasLegalCounsel", "<Lawyer>17</Lawyer>")]
	[DataRow("InCustodyOfEnforcer", "<Enforcer>17</Enforcer>")]
	[DataRow("Lawyering", "<EngagedBy>17</EngagedBy>")]
	[DataRow("OnTrial", "<Prosecutor>17</Prosecutor>")]
	[DataRow("MagicClairaudienceConcentration", "<Target>17</Target>")]
	[DataRow("AnimalHunt", "<Target>&#49;7</Target><Ai>17</Ai>")]
	[DataRow("MonsterIntent", "<Target>17</Target><LastRace>17</LastRace>")]
	[DataRow("MonsterState", "<Provoker>17</Provoker><WarningTarget>4</WarningTarget><Corpse>17</Corpse>")]
	[DataRow("SpellSubjectiveDescription", "<FixedPerceiver>17</FixedPerceiver><TargetId>23</TargetId>")]
	[DataRow("PsionicTrace", "<SourceCharacterId>17</SourceCharacterId><SourceCellId>17</SourceCellId>")]
	[DataRow("Trap", "<CreatorId>17</CreatorId>")]
	[DataRow("TrapPayloadSchedule", "<CreatorId>4</CreatorId><TargetCharacterId>17</TargetCharacterId>")]
	public void Effects_KnownCharacterFields_DoNotMixBodyOrWoundIds(string type, string fields)
	{
		var refs = PhysicalReferenceCodecs.Effects(Effect(type, fields)).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Wound).Includes));
	}

	[TestMethod]
	public void Effects_NestedSpellChildren_InspectTheirCodecWithoutReadingStoredSpellText()
	{
		var xml = Effect("MagicSpellParent", "<Caster>7</Caster><CasterInstance>17</CasterInstance>" +
			"<StoredSpell><Definition><Body>17</Body><Formula>17</Formula></Definition></StoredSpell>" +
			"<Children><Effect><Type>SpellBodyBackup</Type><Effect><BackupBodyId>23</BackupBodyId></Effect></Effect></Children>");
		var refs = PhysicalReferenceCodecs.Effects(xml).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.CharacterInstance).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Body, 23).Includes));
	}

	[DataTestMethod]
	[DataRow("RecentSpeechContext")]
	[DataRow("PsychometricHistory")]
	[DataRow("ItemHidden")]
	[DataRow("MorgueStoredCorpse")]
	[DataRow("CheckResult")]
	public void Effects_CanonicalHistoryAndUnknownNumericFields_DoNotCreatePhysicalLinks(string type)
	{
		Assert.IsFalse(PhysicalReferenceCodecs.Effects(Effect(type,
			"<SpeakerId>17</SpeakerId><TargetId>17</TargetId><BodyId>17</BodyId><Text>17</Text>"))
			.Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[TestMethod]
	public void Effects_MalformedActualField_ReportsExactCodecField()
	{
		var error = Assert.ThrowsException<FormatException>(() =>
			PhysicalReferenceCodecs.Effects(Effect("BodyBackup", "<BackupBodyId>broken</BackupBodyId>")).ToArray());
		StringAssert.Contains(error.Message, "BodyBackup/BackupBodyId");
	}

	[TestMethod]
	public void Effects_BodyOnlyCleanup_DoesNotValidateUnrelatedCharacterIdentityField()
	{
		Assert.AreEqual(0, PhysicalReferenceCodecs.Effects(Effect("HasLegalCounsel", "<Lawyer>broken</Lawyer>"),
			targets: Targets(PhysicalEntityKind.Body)).Count());
	}

	[DataTestMethod]
	[DataRow("Character", true)]
	[DataRow("Body", false)]
	[DataRow("Room:v2", false)]
	[DataRow("Race", false)]
	[DataRow("GameItem", false)]
	public void ExplicitDiscriminator_EqualIdsInOtherDomains_DoNotMatch(string type, bool expected) =>
		Assert.AreEqual(expected, PhysicalReferenceCodecs.Typed(type, "17", "Target").Any(Targets(PhysicalEntityKind.Character).Includes));

	[TestMethod]
	public void Component_BodypartOnly_UsesItsActualWoundListAndBodyFields()
	{
		const string value = "<Definition><OriginalCharacterId>17</OriginalCharacterId><OriginalBodyId>23</OriginalBodyId>" +
			"<Wounds><Wound>&#49;7</Wound></Wounds><BodyPrototypeId>17</BodyPrototypeId></Definition>";
		var refs = PhysicalReferenceCodecs.Component("Bodypart", value, _ => throw new AssertFailedException()).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Wound).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.AreEqual(0, PhysicalReferenceCodecs.Component("Container", value, _ => throw new AssertFailedException()).Count());
	}

	[DataTestMethod]
	[DataRow(0, true)]
	[DataRow(1, false)]
	public void Component_LegacyFinalDeathOnly_ResolvesBodyThroughExactCanonicalOwner(int context, bool expected)
	{
		var refs = PhysicalReferenceCodecs.Component("Corpse",
			$"<Definition><OriginalCharacter>3</OriginalCharacter><RemainsContext>{context}</RemainsContext></Definition>", id => id == 3 ? 17 : null);
		Assert.AreEqual(expected, refs.Any(Targets(PhysicalEntityKind.Body).Includes));
	}

	[DataTestMethod]
	[DataRow("<Member character='17' instance='23' role='1'/>")]
	[DataRow("<Member id='17'/>")]
	[DataRow("<Legacy>17</Legacy>")]
	public void Group_OnlyCodedMemberForms_AreActorReferences(string member)
	{
		var refs = PhysicalReferenceCodecs.Group($"<Group><Action>17</Action><Members>{member}</Members></Group>");
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[TestMethod]
	public void Variable_TagAndCollectionShape_AreRequiredForCharacterReferences()
	{
		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Race, ProgVariableTypes.Location, ProgVariableTypes.Text })
			Assert.AreEqual(0, PhysicalReferenceCodecs.Variable(type.ToStorageString(), "<var>17</var>").Count());
		Assert.IsTrue(PhysicalReferenceCodecs.Variable(ProgVariableTypes.Character.ToStorageString(), "<var>&#49;7</var>").Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsTrue(PhysicalReferenceCodecs.Variable((ProgVariableTypes.Character | ProgVariableTypes.Collection).ToStorageString(), "<vars><var>17</var><var>4</var></vars>").Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsFalse(PhysicalReferenceCodecs.Variable((ProgVariableTypes.Character | ProgVariableTypes.Dictionary).ToStorageString(), "<vars><value><key>17</key><var>4</var></value></vars>").Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[TestMethod]
	public void RouteMotion_OnlyCharacterParticipantsCarryActorInstanceReferences()
	{
		var refs = PhysicalReferenceCodecs.RouteMotion("{\"JourneyId\":17,\"Participants\":[{\"Type\":\"Vehicle\",\"Id\":17},{\"Type\":\"Character\",\"Id\":3,\"InstanceId\":17}]}").ToArray();
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.CharacterInstance).Includes));
	}

	[TestMethod]
	public void UserWait_OnlyCharacterFieldIsAnActorReference()
	{
		Assert.IsTrue(PhysicalReferenceCodecs.UserInput("{\"CharacterId\":17,\"TerminalItemId\":4}").Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsFalse(PhysicalReferenceCodecs.UserInput("{\"CharacterId\":4,\"TerminalItemId\":17}").Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[TestMethod]
	public void LiveReference_NonSavingEffect_HoldsWithoutSerializingOrMaterializing()
	{
		var effect = new Mock<IEffect>(MockBehavior.Strict);
		effect.As<IPhysicalEntityReferenceProvider>().SetupGet(x => x.PhysicalReferences)
			.Returns([new(PhysicalEntityKind.Body, 17, "ActualBody")]);
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([effect.Object], Targets(PhysicalEntityKind.Body)));
		Assert.IsFalse(PhysicalReferenceGuard.HasLiveReference([effect.Object], Targets(PhysicalEntityKind.Character)));
		effect.Verify(x => x.SaveToXml(It.IsAny<Dictionary<IEffect, TimeSpan>>()), Times.Never);
	}

	private sealed class QueryCapture : IQueryExpressionInterceptor
	{
		public readonly HashSet<Type> QueriedTypes = [];
		public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
		{
			new CaptureVisitor(QueriedTypes).Visit(queryExpression);
			return queryExpression;
		}
		private sealed class CaptureVisitor(HashSet<Type> types) : ExpressionVisitor
		{
			protected override Expression VisitExtension(Expression node)
			{
				if (node is EntityQueryRootExpression root) types.Add(root.EntityType.ClrType);
				return base.VisitExtension(node);
			}
		}
	}

	private static FuturemudDatabaseContext Context(QueryCapture? capture = null) => new(
		new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString(), options => options.EnableNullChecks(false))
			.AddInterceptors(capture ?? new QueryCapture()).Options);

	[TestMethod]
	public void PersistedGuard_StaticDefinitionTables_AreNeverQueried()
	{
		var capture = new QueryCapture();
		using var context = Context(capture);
		Assert.IsTrue(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Character), [], [], [], null, out var diagnostic), diagnostic);
		Assert.IsFalse(capture.QueriedTypes.Contains(typeof(Db.ArtificialIntelligence)));
		Assert.IsFalse(capture.QueriedTypes.Contains(typeof(Db.ArmourType)));
		Assert.IsFalse(capture.QueriedTypes.Contains(typeof(Db.AgricultureOperation)));
		Assert.IsFalse(capture.QueriedTypes.Contains(typeof(Db.AutobuilderAreaTemplate)));
		Assert.IsFalse(capture.QueriedTypes.Contains(typeof(Db.AutobuilderRoomTemplate)));
		Assert.IsTrue(capture.QueriedTypes.Contains(typeof(Db.Character)));
	}

	[TestMethod]
	public void PersistedGuard_ComponentDiscriminatorAndRevision_SelectOnlyRealRemains()
	{
		using var context = Context();
		context.GameItemComponentProtos.Add(new() { Id = 1, RevisionNumber = 1, Type = "Container", Name = "container", Definition = "<Definition/>", Description = "" });
		context.GameItemComponentProtos.Add(new() { Id = 1, RevisionNumber = 2, Type = "Bodypart", Name = "part", Definition = "<Definition/>", Description = "" });
		context.GameItemComponents.Add(new() { Id = 3, GameItemComponentProtoId = 1, GameItemComponentProtoRevision = 1, GameItemId = 6, Definition = "<Definition><OriginalBodyId>17</OriginalBodyId></Definition>" });
		context.GameItemComponents.Add(new() { Id = 4, GameItemComponentProtoId = 1, GameItemComponentProtoRevision = 2, GameItemId = 7, Definition = "<Definition><OriginalBodyId>17</OriginalBodyId></Definition>" });
		context.SaveChanges();
		Assert.AreEqual(4L, PhysicalReferenceGuard.RemainsComponents(context).Single().Id);
		Assert.IsFalse(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Body), [], [], [], null, out var diagnostic));
		StringAssert.Contains(diagnostic, "GameItemComponent 4");
		StringAssert.Contains(diagnostic, "Bodypart/OriginalBodyId");
		Assert.IsTrue(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Body), [], [], [], 7, out diagnostic), diagnostic);
	}

	[TestMethod]
	public void PersistedGuard_UnloadedOtherActorEffect_ReportsExactSourceAndField()
	{
		using var context = Context();
		context.Characters.Add(new() { Id = 23, EffectData = Effect("BodyBackup", "<BackupBodyId>17</BackupBodyId>"), Name = "other" });
		context.SaveChanges();
		Assert.IsFalse(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Body), [], [], [], null, out var diagnostic));
		StringAssert.Contains(diagnostic, "Character.EffectData 23");
		StringAssert.Contains(diagnostic, "BodyBackup/BackupBodyId");
	}
	[DataTestMethod]
	[DataRow("AstralProjection", "ProjectionBodyId")]
	[DataRow("MagicalCopy", "CopyBodyId")]
	[DataRow("PhysicalClone", "CloneBodyId")]
	[DataRow("PossessedBody", "ShellBodyId")]
	[DataRow("PossessedCorpse", "OriginalBodyId")]
	[DataRow("AnimatedCorpse", "OriginalBodyId")]
	[DataRow("ScriptedAi", "BodyId")]
	public void InstanceMetadata_DeclaredKinds_KeepStaticAndSourceProvenanceSeparate(string type, string bodyField)
	{
		var refs = PhysicalReferenceCodecs.Effects($"<Effects><{type} AnchorCharacterId='3' AnchorInstanceId='4' {bodyField}='17' PlaneId='17' SourceSpellId='17' SourceTargetCharacterId='5' SourceTargetInstanceId='6'/></Effects>", instanceMetadata: true).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.CharacterInstance).Includes));
	}

	[DataTestMethod]
	[DataRow("SpellCorpsePossession")]
	[DataRow("SpellAnimatedCorpse")]
	[DataRow("SpellDeadSpeak")]
	public void CorpseEffects_ActualAnimatedIdentityPair_RetainsPhysicalInstance(string type)
	{
		var refs = PhysicalReferenceCodecs.Effects(Effect(type, "<AnchorCharacterId>3</AnchorCharacterId><OriginalCharacterId>17</OriginalCharacterId><AnimatedInstanceId>23</AnimatedInstanceId><OriginalBodyId>4</OriginalBodyId>")).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.CharacterInstance, 23).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
	}

	[TestMethod]
	public void PossessedSource_DeathAndQuitRelationship_RetainsBothTypedIds()
	{
		var refs = PhysicalReferenceCodecs.Effects(Effect("SpellPossessedBody", "<AnchorCharacterId>3</AnchorCharacterId><SourceTargetCharacterId>17</SourceTargetCharacterId><SourceTargetInstanceId>23</SourceTargetInstanceId><ShellBodyId>4</ShellBodyId>")).ToArray();
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsTrue(refs.Any(Targets(PhysicalEntityKind.CharacterInstance, 23).Includes));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Body).Includes));
		var metadata = PhysicalReferenceCodecs.Effects("<Effects><PossessedBody AnchorCharacterId='3' AnchorInstanceId='4' ShellBodyId='5' SourceTargetCharacterId='17' SourceTargetInstanceId='23'/></Effects>", instanceMetadata: true).ToArray();
		Assert.IsTrue(metadata.Any(Targets(PhysicalEntityKind.Character).Includes));
		Assert.IsTrue(metadata.Any(Targets(PhysicalEntityKind.CharacterInstance, 23).Includes));
		var live = new SpellPossessedBodyEffect(Mock.Of<ICharacter>(), Mock.Of<IMagicSpellEffectParent>(), "shell",
			3, 4, 5, 6, 17, 23, 17, CharacterInstancePersistencePolicy.DespawnOnReboot, "", "", "", "", "");
		Assert.IsFalse(live.SavingEffect);
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([live], Targets(PhysicalEntityKind.Character)));
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([live], Targets(PhysicalEntityKind.CharacterInstance, 23)));
		Assert.IsFalse(PhysicalReferenceGuard.HasLiveReference([live], Targets(PhysicalEntityKind.Body)));
		var unrelated = new SpellPossessedBodyEffect(Mock.Of<ICharacter>(), Mock.Of<IMagicSpellEffectParent>(), "shell",
			3, 4, 5, 6, 7, 8, 17, CharacterInstancePersistencePolicy.DespawnOnReboot, "", "", "", "", "");
		Assert.IsFalse(PhysicalReferenceGuard.HasLiveReference([unrelated], Targets(PhysicalEntityKind.Character)));
	}

	[TestMethod]
	public void Illusion_TargetAudienceIsAnIdentitySelector_AndDoesNotResolvePhysicalActor()
	{
		var refs = PhysicalReferenceCodecs.Effects(Effect("SpellPhantomIllusion", "<CasterId>3</CasterId><TargetId>17</TargetId>"));
		Assert.IsFalse(refs.Any(Targets(PhysicalEntityKind.Character).Includes));
	}

	[TestMethod]
	public void LivePeer_ConcreteNonSavingBinding_ReportsCharacterInstanceAndBodyWithoutCallbacks()
	{
		var peer = new Mock<ICharacter>();
		peer.SetupGet(x => x.Id).Returns(17);
		peer.SetupGet(x => x.InstanceId).Returns(23);
		peer.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Id == 29));
		var effect = new BeingBound(Mock.Of<ICharacter>()) { Binder = peer.Object };
		Assert.IsFalse(effect.SavingEffect);
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([effect], Targets(PhysicalEntityKind.Character)));
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([effect], Targets(PhysicalEntityKind.CharacterInstance, 23)));
		Assert.IsTrue(PhysicalReferenceGuard.HasLiveReference([effect], Targets(PhysicalEntityKind.Body, 29)));
	}

	[TestMethod]
	public void PersistedVariable_OnlyDeclaredCharacterValuesAreRead_AndDefaultsKeepTheirDeclaredType()
	{
		using var context = Context();
		var ownerType = ProgVariableTypes.Character.ToStorageString();
		context.VariableValues.Add(new() { ReferenceTypeDefinition = ownerType, ReferenceId = 17, ReferenceProperty = "text",
			ValueTypeDefinition = ProgVariableTypes.Text.ToStorageString(), ValueDefinition = new string('1', 1048577) });
		Assert.IsTrue(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Character), [], [], [], null, out var diagnostic), diagnostic);
		context.SaveChanges();
		Assert.IsTrue(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Character), [], [], [], null, out diagnostic), diagnostic);
		context.VariableDefinitions.Add(new() { OwnerTypeDefinition = ownerType, Property = "actor", ContainedTypeDefinition = ownerType });
		context.VariableDefaults.Add(new() { OwnerTypeDefinition = ownerType, Property = "actor", DefaultValue = "<var>17</var>" });
		context.SaveChanges();
		Assert.IsFalse(PhysicalReferenceGuard.PersistedReferencesAreClear(context, Targets(PhysicalEntityKind.Character), [], [], [], null, out diagnostic));
		StringAssert.Contains(diagnostic, "VariableDefault.DefaultValue");
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void DeletedPrincipal_ActualIncomingForeignKey_HoldsUntilTheDependentIsInTheDeletionSet(bool instance)
	{
		using var context = Context();
		object principal;
		object dependent;
		if (instance)
		{
			principal = new Db.CharacterInstance { Id = 17 };
			dependent = new Db.CharacterInstance { Id = 23, AnchorInstanceId = 17 };
		}
		else
		{
			principal = new Db.Wound { Id = 17 };
			dependent = new Db.Infection { Id = 23, WoundId = 17 };
		}
		context.Add(principal); context.Add(dependent); context.SaveChanges();
		var method = typeof(CharacterArchiveService).GetMethod("DeletedPrincipalReferencesAreClear", BindingFlags.NonPublic | BindingFlags.Static)!;
		var removed = new HashSet<object> { principal };
		object?[] args = [context, removed, null];
		Assert.AreEqual(false, method.Invoke(null, args));
		StringAssert.Contains((string)args[2]!, instance ? "AnchorInstanceId" : "WoundId");
		removed.Add(dependent);
		Assert.AreEqual(true, method.Invoke(null, args));
	}

	[TestMethod]
	public void BorrowedCorpse_UsesExactVersionedCodecAndClaims_IncludingEncodedIdentity()
	{
		using var context = Context();
		var now = DateTime.UtcNow;
		var id = Guid.NewGuid();
		context.MagicSpellLifecycles.Add(new() { Id = id, SpellId = 3, Grade = 1, CreatorId = 4, Family = "borrow",
			Mode = (int)SpellLifecycleMode.TemporaryCleanup, State = (int)SpellLifecycleState.Active, Version = 1,
			CreatedUtc = now, UpdatedUtc = now, DeadlineUtc = now.AddMinutes(1),
			Provenance = "<CorpseAnimation version='1' corpse='&#49;7' owner='4' body='6' cell='8' layer='0'><Source>corpse 23</Source></CorpseAnimation>",
			Entities = [new() { LifecycleId = id, Kind = (int)SpellOwnedEntityKind.CharacterInstance, Role = (int)SpellOwnedEntityRole.CreatedEntity, EntityId = 23 }] });
		context.SaveChanges();
		var method = typeof(SpellOwnedCorpseAnimationService).GetMethod("HasUnfinishedBorrow", BindingFlags.NonPublic | BindingFlags.Static)!;
		Assert.AreEqual(true, method.Invoke(null, [context, 17L]));
		Assert.AreEqual(false, method.Invoke(null, [context, 23L]));
	}

}
