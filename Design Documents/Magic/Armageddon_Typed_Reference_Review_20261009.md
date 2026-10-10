# Armageddon retirement reference review — 9 October 2026

Status: **SOURCE REVIEW COMPLETE; TYPED REFERENCE REPLACEMENT PENDING**.

Reviewed branch `codex/armageddon-continuation-20261008` at
`81b9b6d1a04bff63389477be5f035ca6cf9bb1e9`. This review changes the active plan and
documents the required replacement. It does not change runtime code or claim new
managed/native qualification.

## Direction and relationship to Armageddon

The user's current instruction takes precedence over earlier conservative scanning
plans: retirement checks must use known, provable relationships from the code. Do
not search text for character/body/instance/wound numbers, infer relationships from
property-name suffixes, or add static-schema exemptions to rescue that approach.

The original completion brief requires temporary summons to release their heavy
physical graphs while preserving real dependencies, foreign possessions and historical
attribution, including repeated expiry/death/decay/restart scenarios. That is the
Armageddon dependency being addressed. Validating unrelated AI, Agriculture, Armour
or Autobuilder configuration is not that dependency. The planned Autobuilder
classifier milestone is superseded and will not be implemented.

The existing guard is both unnecessarily expensive and unable to prove a relationship:
it queries unrelated payload tables, visits arbitrary XML/JSON values and compares
numbers from different identity domains. It can retain a summon because an enum,
formula or narrative contains the same number. A successful static-codec exemption
does not establish reference coverage elsewhere. The replacement must also cover
actual live relationships, including unsaved effects; persisted text searches do not
establish those relationships.

## Changed-function disposition

Paths below are repository-relative. Lines identify the reviewed revision.

| Existing path / entry point | Finding | Required disposition |
| --- | --- | --- |
| `MudSharpCore/Character/NpcArchiveReferencePolicy.cs:14`, `HasReferenceOrUncertainty`, `HasJsonReference`, `Matches` | Arbitrary XML leaves/attributes, JSON keys/scalars and raw text are compared with mixed character/body/instance/wound numbers; `Matches` uses a regular expression. | Remove numeric-text matching and its generic fallback. Preserve the separate structural `IsEmptyEffects` ownership check, moving it if the class is removed. |
| `MudSharpCore/Character/CharacterArchiveService.cs:280`, `SerializedReferencesAreClear` | EF string properties are selected by names such as Definition/Data/Value/StateData rather than actual reference contracts. | Replace table/property-name discovery with exact mapped relations, typed scalar pairs and registered identity-bearing payload readers. |
| `CharacterArchiveService.cs:350`, `UnmappedReferencesAreClear` | Every unclassified non-FK long `*Id` is compared against all identity numbers. The name and equality do not prove a physical reference. | Remove suffix-based selection. Enumerate specific code-proven fields and preserve their discriminators and semantics. |
| `NpcArchiveReferencePolicy.Agriculture.cs`, `.Combat.cs`, `.ArtificialIntelligence.cs` | Archive-only validators for static Agriculture, Armour and Judge/Mount/Animal/Monster configuration work around the generic matcher. Unknown unrelated configuration can still hold retirement. | Remove these classifiers from the archive pipeline; replace associated archival tests. Do not expand the exemption catalogue. |
| `MudSharpCore/Character/RetiredBodyReferencePolicy.cs:12`, `IsReference` | Traverses arbitrary XML and treats names ending in body/bodyid/body_id as references; malformed unrelated XML holds. | Replace with exact owner/type/field contracts shared with retired-body cleanup. |
| `MudSharpCore/Character/CharacterBodyCleanup.cs:46`, `HasSavedRuntimeBodyReference` | Serializes each saving effect and runs the field-name heuristic. Serialization is not a reference API and can have side effects. | Inspect actual typed live relationships, including non-saving effects. Do not call effect save/load factories to discover references. |
| `CharacterBodyCleanup.cs:120`, `HasPersistedReferenceToRetiredBody` | Reads all component definitions and several complete EffectData tables without component/effect identity contracts. | Query known component families and inspect registered effect types in known owner channels. Preserve true backup-body and form relationships. |
| `MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.cs:188`, `FindPersistedRemains` | SQL candidates use `Definition.Contains(bodyId)` or `Contains("&#")`, then inspect body fields. The initial text filter is still an ID search. | Select Corpse/Bodypart rows by actual component prototype type/revision joins, then decode their specific fields and supported legacy lineage. Preserve ambiguous-remains refusal. |
| `MudSharpCore/Magic/Lifecycle/SpellOwnedCorpseAnimationService.cs:154`, `IsBorrowedCorpse`; `SpellOwnedCreation.Corpse.cs:45`, `PersistedCorpseError` | Recognizes borrowed corpse identity with a provenance substring containing its numeric ID. | Reuse the known CorpseAnimation metadata reader and compare its typed corpse field. Preserve lifecycle/claim/version checks. |
| `SpellOwnedCreation.Corpse.cs:83`, `IsExactCorpseDefinition` | Exact OriginalCharacter/OriginalBody fields are a usable contract, but field names alone do not prove the component family. | Prove the Corpse component discriminator before using this reader. |
| `scripts/FuryCalmSmokeWorld/npc_archival.py` | The serialized digit-collision census discovers text columns and searches values for candidate numbers. | Remove future digit-census diagnostics; record provider, source row, exact field, identity kind, target ID and retention reason. Preserve historical receipts unchanged. |
| `scripts/FuryCalmSmokeWorld/OwnedBoneArmourRepair.cs:66` | The valid stock repair is coupled to an archive-only Armour validator and arbitrary character/body IDs. | Retain the repair and transactional preservation checks; qualify the corrected formula through its domain contract without the archive classifier. |

This is a disposition ledger, not a recommendation to revert whole commits. The
Agriculture/Armour milestone also contains registration-only Body shutdown, which
remains useful. The stock bone-armour formula repair is a separate real defect and
must remain. Prepayment, spawn admission, exact ownership claims, payment/death proof,
foreign custody, history, idempotency and post-commit runtime release also remain.

## Proven relationship inventory

Each reader must return a typed relationship with its owner and retention meaning.
A canonical Character ID can be preserved in history without keeping a Body alive.
A Body ID, CharacterInstance ID or Wound ID is a different identity domain, even when
the numeric values coincide. A loader that resolves a live actor is evidence to assess
physical dependency; it does not automatically make every canonical reference a hold.

| Surface and actual reader | Code-proven fields / relationship | Replacement use |
| --- | --- | --- |
| `CharacterArchiveService.cs:171` and EF mappings | Actual incoming foreign keys to Characters/Bodies; NPC bodyguard FK; ownership rows for the exact deletion set. | Retain the FK safety sweep and explicit ownership/history dispositions. Extend incoming-FK coverage to deleted CharacterInstance and Wound principals rather than relying only on today's known consumers. |
| `MudsharpDatabaseLibrary/Database/FuturemudDatabaseContextCharacterInstances.cs:113` | CharacterId/BodyId ownership and AnchorInstanceId FK; generated EmbodiedBodyId/PrimaryCharacterId keys duplicate established ownership. | Preserve same-body/primary-instance proof and anchor dependency. Generated keys are not independent external references. |
| `MudSharpCore/Character/CharacterInstances.cs:509`; `FutureMUDLibrary/Framework/PersistedFrameworkItemReference.cs`; `MudSharpCore/Framework/Futuremud.cs:210` | GameItem OwnerId/OwnerType and PositionTargetId/PositionTargetType; Character and CharacterInstance position-target pairs use the same typed resolver. | Query exact type-and-ID pairs, including unloaded owners. Preserve Room/Cell wire identity compatibility. Never compare unrelated entity IDs. |
| `CorpseGameItemComponent.cs:110`; `BodypartGameItemComponent.cs:143`; their prototype database registrations | Corpse OriginalCharacter/OriginalBody; Bodypart OriginalCharacterId/OriginalBodyId. OriginalBody accessors resolve the physical body, including supported legacy fallback. | Select component types `Corpse`/`Bodypart` first. Parse only these fields; distinguish retained canonical lineage from actual body/remains dependence. |
| `MudSharpCore/NPC/AI/Groups/GroupAI.cs:84`, `LoadMemberReferences`, resolver at 187 | Group Definition Members/Member character (legacy id), optional instance and role; the coded legacy numeric child form is also a member reference. | Read only the Members contract and resolve its character/optional-instance meaning. Group Action/Alertness, static template IDs and arbitrary narrative are unrelated. Map Group Data separately only where its concrete loader proves a relationship. |
| `MudSharpCore/Effects/Effect.cs:74`, `LoadEffect`; `PerceivedItem.cs:394`, 484 | Saved Effects containers select concrete codecs by each effect's Type. Known EffectData owner models are Character, CharacterInstance, Body, GameItem and Room. | Use pure reference readers keyed by exact effect type/owner; recursively inspect known child effects. Do not instantiate effects, execute InitialEffect, load actors or call SaveDefinition. |
| `MudSharpCore/Effects/Concrete/MagicSpellParent.cs:36`, 116, 172 | Caster and optional CasterInstance; nested child effect contracts. Stored spell snapshots contain static spell configuration. | Keep canonical identity and selected instance distinct. Inspect children through their concrete contracts; ignore unrelated snapshot numbers. |
| `Effects/Concrete/BodyBackupEffect.cs:107`; `SpellEffects/SpellBodyBackupEffect.cs:32`; `CharacterBodyBackups.cs:16` | BackupBodyId is resolved within the owning character's forms through IBodyBackupEffect. | Preserve owner context and the physical backup relationship. DestinationCellId and applicability prog IDs have other meanings. |
| `MudSharpCore/Character/CharacterInstanceMetadata.cs:82`, 268; direct-possession and possessed-body effect readers | Typed anchor character/instance, possessed source target/instance and applicable original-body/corpse lineage. Existing TryGet metadata readers are pure. | Reuse known formats, versions and explicit fields. Distinguish prototype/provenance identity from an independently live physical dependency. |
| `MudSharpCore/Vehicles/VehicleRouteMotionPersistence.cs:70`, 350, 389 | StateData Participants carry Type, Id and optional InstanceId; restore dispatches Character participants to actor/instance lookup. | Inspect participants with the actual type tag. Journey, leg, sequence and tuning numbers cannot retain a character. |
| `MudSharpCore/Computers/ComputerProcessWaitArguments.cs:27`; `ComputerExecutionService.cs:989`, 1546 | UserInput wait payload CharacterId and TerminalItemId restore a waiting user interaction. | Decode this wait kind only; classify the user relationship separately from its item endpoint and from other wait kinds. |
| `MudSharpCore/FutureProg/VariableRegister.cs:125`, 327, 386, 463, 686, 867 | VariableValue.ValueTypeDefinition selects scalar/collection/dictionary/collection-dictionary readers; Character reference values are exact integer var fields whose resolver calls TryGetCharacter. ReferenceTypeDefinition/ReferenceId identifies the register owner. VariableDefault uses the declared VariableDefinition type. | Include actual Character-valued entries/defaults and coded collection shapes, not all XML numbers or dictionary keys. Separate owner identity from value target identity and preserve static Race/Room/etc type domains. |
| `MudSharpCore/Character/CharacterArchiveService.cs:327`, `HasRuntimeDependants` | Live combat, controller, movement, followers, riders/mounts, positions, targets, groups and bodyguard relationships. | Retain direct object checks and add proven effect relationships without serialization or save callbacks. |

Known saved-effect contracts found in the bounded review include the following. Their
concrete loaders, rather than spelling alone, establish each field's meaning:

- `CreaturePursuitEffect.Target`; `DelayedPsychicSuggestionEffect.source`;
  `HasLegalCounsel.Lawyer`; `InCustodyOfEnforcer.Enforcer`; `Lawyering.EngagedBy`;
  `OnTrial.Prosecutor`/`Defender`; `MagicClairaudienceConcentrationEffect.Target`.
- `PsionicTraceEffect.SourceCharacterId`/`TargetCharacterId`;
  `WitnessedClanMemberDeath.Member`; `NpcBurrowFoodEffect.PendingVictimId`.
- `TrapEffect.CreatorId` and `TrapPayloadSchedule.CreatorId`/`TargetCharacterId`;
  the specific scheduled-payload contract must be used, not arbitrary prog parameters.
- Spell parent, direct-possession, possessed-body, Armageddon information/corpse
  lineage and body-backup contracts listed above.
- `CheckResult` has TargetId/TargetType and ToolId/ToolType attributes. It compares
  typed cached identities rather than loading an actor; classify its retention meaning
  explicitly instead of turning its canonical identity into a blanket Body hold.

The implementation must finish the identity-bearing codec coverage ledger for the
channels it changes. This bounded review identifies concrete providers and negative
cases; it is not a claim that every extension codec in the engine is already mapped.

## Justified exclusions and canonical history

These exclusions follow the inspected writers/readers, not the absence of a matching
number in sample data:

- Agriculture Operation/Crop/Profile tuning, Armour formulas/enums and the observed
  Judge/Mount/Animal/Monster definitions use static configuration IDs, scalars or text.
  They do not acquire physical actor relationships by containing a summon ID. The
  archiver must not query or validate these definitions. Any other concrete AI or
  Autobuilder codec would require actual reader evidence before entering the guard.
- Inspected wound ExtraInformation codecs contain wound state, counters/enums/text;
  ActorOriginId and Infection.WoundId are separate mapped fields. No physical identity
  field was found in those extras. Do not infer one from arbitrary injury XML.
- Inspected land-detail JSON describes native organic yield room/field/generation/
  definition/profile identities. Inspected hospital procedure parameters describe
  patient bodypart/organ selections and opaque tokens. Neither proves a stored
  character/body/instance/wound ID relationship.
- Computer process StateJson/ResultJson restore paths inspected in
  `ComputerProgramExecutor.cs:669` do not resolve actor IDs in their generic value
  decoder. This differs from VariableRegister's explicit Character resolver. A type
  label in opaque JSON alone is not enough; follow any actual consumer before adding
  a contract. No adjacent serialization repair is part of this milestone.
- Inspected employment OperationalPayload/CommandArguments and patrol StrategyData
  do not establish arbitrary embedded character IDs. Actual employment actor/candidate
  columns and any other explicit consumers must retain their separately proven checks.
  Runtime execution-patrol condemned identity is not a field in its inspected saved
  StrategyData writer.
- Writing authors/true authors, Drawing authors, Crime, CharacterLog and wound actor
  attribution remain canonical history. Do not delete them or materialize archived
  physical actors to display history. CharacterCombatSetting.CharacterOwnerId is
  canonical configuration ownership; a real FK still needs a correct disposition,
  rather than an automatic physical hold. EstateHeirId/Type similarly needs its typed
  metadata meaning preserved.

## Replacement contract and efficiency

1. Start with the explicit owned deletion set and real incoming EF foreign keys.
   Query known indexed scalar/discriminator relationships directly. Keep these checks
   in the existing transaction and lifecycle/version boundary.
2. Inspect only proven serialized reference channels and concrete provider types.
   Decode exact identity fields using pure readers aligned with the actual loader,
   including supported legacy and XML-encoded forms. Decode XML/JSON structure; never
   search its text for a number. Preserve the identity kind throughout comparison.
3. Inspect direct live references through a small shared capability where needed,
   following shared-interface ownership in FutureMUDLibrary. Avoid a parallel world
   identity system. Reading relationships must not execute progs, RNG, effect factories,
   save callbacks, actor materialization or health ticks.
4. Record an actual source row, provider/type, field, target kind/ID and reason for a
   hold. Malformed input in a proven identity-bearing contract can hold with a precise
   diagnostic. Malformed unrelated static configuration is outside this decision.
   Missing provider coverage must be recorded and resolved through code mapping;
   numeric-text fallback is never an acceptable substitute.
5. Eliminate the per-NPC sweep of every Definition/Data/Value column. Some genuine
   serialized relationships currently lack a reverse index and will still require
   decoding their known rows. Measure that bounded work before choosing persistence
   changes. If an index is needed, add it only for a proven relationship with correct
   write/update/restart consistency; do not introduce an unsafe cached reference result
   or a general-purpose scan-backed registry.

## Revised milestones and acceptance

**Next milestone: replace retirement reference heuristics with code-proven links.**
Apply the disposition ledger to NPC archival, shared retired-body cleanup, remains
recovery and corpse-animation provenance checks. Remove the archive-only classifier
pipeline and diagnostic digit census, decouple the valid stock repair, and retain
existing ownership, safety, historical identity and post-commit release behavior.
Finish the provider coverage/disposition ledger before enabling physical compaction.

Replace heuristic tests with meaningful cases: actual references in known fields
hold; equal numbers in other identity domains and unrelated configuration do not;
supported encoded/legacy identity fields still work; malformed actual reference
fields report their provider; unloaded persisted and unsaved live dependencies hold;
canonical history survives; exact owned graph deletion, custody and idempotent restart
remain intact. Verify that unrelated static-definition tables are not queried. Run the
appropriate focused suites and full Core suite once inputs are frozen, plus dependent
checks if the final change justifies them. Do not retain assertions whose purpose is
to require arbitrary numeric-text matching.

Then rerun the unchanged owned native retirement gate for retained lifecycles
`476b6713-766b-4f23-b8e9-c1693c29220c` (character/body 10) and
`9a536f89-f899-461b-9111-a11b2ede595b` (character/body 11), followed by the separate-process
completed cold retry. Require real physical graph release; do not manually delete the
held graph or weaken the assertions to make it pass.

**Subsequent milestones:** continue N14/N15 installed command/world acceptance and
N16 repeated lifecycle/restart/heavy-row checks using the same typed reference rule.
Later summon, projection, possession, corpse, control and topology features must
declare actual identity relationships with their codecs/readers when added. They must
not restart an AI/Autobuilder/Calendar/etc static-schema exemption sequence. Authored
AI behavior can still be a separate feature requirement (Guardian/Mount/Courier),
with its own gameplay acceptance; its static definition is not a retirement reference.

All 154 candidates (152 required, two optional), the exact 82 Sorcerer spells/four
roots/12 supports, and all eight required larger systems remain unchanged. Release,
publication and full-plan completion remain disabled/pending.

## Evidence boundary

The [AI checkpoint](Armageddon_Npc_AI_Reference_Checkpoint_20261009.json) remains
immutable historical evidence: 6,038 full Core passes, a 164-test subset and a native
15-pass/one-fail attempt held on AutobuilderAreaTemplate.Definition. Those tests
qualified the previous design; they do not qualify this replacement. Both physical
graphs remain held and the completed cold retry was not reached. The stock repair,
Fury/Calm and previous failed receipts remain preserved separately.

This review used source inspection and bounded read-only code mapping. No build,
automated test, native startup, database write or runtime implementation change was
performed for the new direction. Documentation validation is reported separately.

Documentation validation passed: both central JSON ledgers parse; preserved inventory
and historical evidence objects compare equal to the reviewed HEAD; all changed Markdown
file links and 19 distinct qualified source paths resolve; the historical AI checkpoint
has no diff; recorded full-Core/native receipt hashes match. Release/publication flags
remain false. `git diff --check` passes. These are documentation checks, not gameplay
qualification.
