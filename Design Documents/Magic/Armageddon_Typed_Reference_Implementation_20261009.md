# Armageddon code-proven reference implementation — 9 October 2026

The approved replacement is implemented in `13204460aa4524d5ecce29e6baf0aefc4928166b` and qualified for the two retained paid NPCs and their separate-process completed retry. [The checkpoint](Armageddon_Typed_Reference_Checkpoint_20261009.json) binds the source, binaries and fresh evidence. The [preceding review](Armageddon_Typed_Reference_Review_20261009.md) remains historical design evidence. Full Armageddon completion and release remain pending.

The archiver checks relationships that the code actually stores or consumes. It no longer discovers text columns, walks arbitrary numeric values, guesses `*Id` properties or validates unrelated static definitions. Agriculture, Armour, AI and Autobuilder configuration cannot acquire a physical dependency by containing the same number. The native retry preserves those definitions byte for byte.

## Changed-function ledger

| Surface | Final implementation and reason |
| --- | --- |
| [PhysicalEntityReference and provider](../../FutureMUDLibrary/Framework/IPhysicalEntityReferenceProvider.cs) | Shared Character/Body/CharacterInstance/Wound identities; direct actors also report their current body, and butcherable targets report their exact original body. Identity domains never share a numeric match. |
| [CharacterArchiveService](../../MudSharpCore/Character/CharacterArchiveService.cs) | Removed serialized-column discovery and guessed-ID guards. Existing canonical/body/NPC FK and ownership proof remains; incoming deleted instance/wound FKs are also checked against the exact removal set. CharacterCombatSetting ownership is retained as canonical configuration. |
| [PhysicalReferenceGuard](../../MudSharpCore/Character/PhysicalReferenceGuard.cs) | Exact discriminator queries and known payload channels only. A hold names the source row, provider field, identity kind and target. Character variable types are selected before value payloads are read; unrelated large text values do not hold. |
| [PhysicalReferenceCodecs](../../MudSharpCore/Character/PhysicalReferenceCodecs.cs) | Pure, explicit field readers selected by actual effect, metadata, component or value type. XML entities are decoded by XML parsing, with no numeric text search or fallback. |
| [EffectPhysicalReferences](../../MudSharpCore/Effects/EffectPhysicalReferences.cs) and private-field overrides | Direct live objects and stored IDs, including non-saving effects and inactive loaded bodies. No SaveToXml, SaveDefinition, lazy actor lookup, effect factory, InitialEffect or prog callback is used. |
| [CharacterBodyCleanup](../../MudSharpCore/Character/CharacterBodyCleanup.cs) | Earlier body cleanup uses the same typed guard and includes the body's wound IDs. Forms, backups, exact custody and ownership checks remain. The old body-field-name matcher is removed. |
| [SpellOwnedNpcService](../../MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.cs) | Remains recovery selects Corpse/Bodypart component prototype type and exact revision first, then reads original-body fields and supported final-death legacy ownership. Ambiguity still holds. |
| [Corpse animation service](../../MudSharpCore/Magic/Lifecycle/SpellOwnedCorpseAnimationService.cs) and [creation preflight](../../MudSharpCore/Magic/Lifecycle/SpellOwnedCreation.Corpse.cs) | Versioned CorpseAnimation borrow metadata and exact claims identify a corpse. Persisted corpse eligibility requires the real Corpse discriminator. |
| [Structural effect helper](../../MudSharpCore/Character/NpcArchiveReferencePolicy.cs) | Retains only IsEmptyEffects; numeric matcher and Agriculture/Armour/AI archive classifiers and their tests are deleted. Autobuilder classifier work is cancelled. |
| [Owned repair](../../scripts/FuryCalmSmokeWorld/OwnedBoneArmourRepair.cs) and [native retry](../../scripts/FuryCalmSmokeWorld/npc_archival.py) | Valid stock repair, CAS rollback, builder preservation and idempotency remain. Formula parsing uses ExpressionEngine. Diagnostic digit-collision census is removed; source/binary hashes and explicit preservation assertions remain. |

## Proven persisted relationships

| Selected channel | Fields used by the concrete reader |
| --- | --- |
| GameItem ownership/position; Character/CharacterInstance position | OwnerType/OwnerId and PositionTargetType/PositionTargetId; exact identity discriminator. |
| Corpse and Bodypart components | Prototype ID **and revision**, Type Corpse/Bodypart; OriginalBody/OriginalBodyId. Only final-death legacy forms resolve OriginalCharacter/OriginalCharacterId to the known body. Bodypart Wounds/Wound is an actual wound-row dependency. |
| EffectData on Character, CharacterInstance, Body, GameItem and Room | Effects/Effect/Type selects the known contract. MagicSpellParent/SubstanceExposure recurse into Children/Effect; StoredSpell is static configuration. |
| Actor effects | Counsel Lawyer; custody Enforcer; Lawyering EngagedBy; trial Prosecutor/Defender; Clairaudience Target; PsionicTrace source/target; WitnessedClanMemberDeath Member; AnimalHunt/MonsterIntent Target; MonsterState Provoker/WarningTarget; delayed suggestion source; burrow food PendingVictimId; trap creator/scheduled target. |
| Spell effects | Parent Caster/CasterInstance; Identify/Recite caster and linked speaker; possession anchor/target/source/shell/animated pairs; corpse OriginalBodyId; DeadSpeak linked speaker. BackupBodyId, BaselineBodyId, FormBodyId/PriorBodyId and SubstanceExposure SourceBody are Body identities. Tethers preserve AnchorType/AnchorId. |
| CharacterInstance metadata | AstralProjection, MagicalCopy, PhysicalClone, PossessedBody, PossessedCorpse, AnimatedCorpse and ScriptedAi anchor character/instance and named physical-body fields. PossessedBody source character/instance is a death/quit lifecycle link. |
| GroupAi Definition | Members/Member character (legacy id) and optional instance; supported legacy numeric member child. Action/Alertness and Data tuning do not define actor links. |
| VariableValue and VariableDefault | Declared Character value type, with actual scalar/collection/dictionary/collection-dictionary XML shape. Default type comes from its joined VariableDefinition. Owner keys and dictionary keys are separate from target values. |
| ActiveRouteMotion StateData | Participants with Type Character, Id and optional InstanceId. Journey/leg/vehicle numbers are excluded. |
| Active computer UserInput wait | Exact CharacterId from ComputerProcessWaitArguments; TerminalItemId and other wait kinds are separate domains. |

The live provider covers action/medical/surgical peers, binding/butchering/skinning/hitching, clinching/grappling/rescue/dressing/dragging, mind connection/choking/anesthesia, posturing/herd peers, spy/notify/switched observers, clairaudience/intercession and tether anchors. Private ID overrides cover counsel/custody/trial, witnessed clan death, spell-parent children, pending burrow victim, source body and the already-held rejuvenation caster. Reading these providers does not serialize state or resolve a lazy cache.

## Justified exclusions

Static Agriculture/Armour/AI/Autobuilder definitions are not queried. Inspected GroupAI Data, injury extras, land-detail JSON, hospital procedure parameters, employment opaque payloads, patrol StrategyData and computer generic StateJson/ResultJson contain no proven physical identity decoder in the audited paths. Their actual relational fields retain existing FK dispositions; arbitrary payload numbers are ignored.

RecentSpeechContext, ItemHidden witnesses, PsychometricHistory, CheckResult, trap-restraint creators, wildlife shelter ACLs, guarding/toll permissions, morgue/prisoner belongings and recorded answering-machine voices use canonical attribution, identity matching, access policy or recorded presentation. They do not retain a body through an arbitrary equal ID. Illusion TargetId is an audience selector; its CasterId can be resolved for the viewer prog and is guarded, including SubjectiveDescription's legacy FixedPerceiver fallback. SourceSpellId, PlaneId, prototype IDs and narrative/formula values remain static.

WitnessedClanMemberDeath is a deliberate exception to purely historical display: its current consumer dereferences the clan-member actor and can cache that object. Its exact Member field therefore holds physical compaction until that effect's consumer becomes archive-aware; it is not a blanket rule for witness/history numbers. PossessedBody source IDs likewise remain physical lifecycle links because death and quit consume SourceTargetInstanceId, despite also describing provenance.

Known malformed identity-bearing contracts fail closed with a precise field diagnostic. Known channel inspection is bounded at 100,000 rows and 1 MiB per payload; archived wound summaries retain their separate existing bounds. Unrelated malformed configuration is outside the decision. This coverage ledger describes the inspected existing codecs, not arbitrary future extension encodings. A new physical relationship must add its pure live provider and persisted reader when its owning codec is implemented.

## Verification and remaining work

Full managed run: **5,938 Core + 668 Library = 6,606 passed**, zero failed/skipped, stable source. Final focused run: **173 passed**, zero failed/skipped, stable source; this is a subset and is not added to the full count. It includes exact domains, encoded IDs, static-table non-querying, unloaded effects, malformed actual fields, nested children, metadata, real incoming instance/wound FKs, Character-only variable defaults, and concrete unsaved binding/possession references.

Native retained-world run: **60 assertions passed**. Both previously paid TemporaryCleanup lifecycle records reached Completed with NPC/Body/CharacterInstance rows absent, canonical/archive/claims retained and original native death unchanged. A separate server process preserved the exact completed lifecycle and archive rows. Fury and all observed Agriculture/Armour/AI/Autobuilder definitions stayed exact. The owned bone-armour repair changed zero rows and passed builder-refusal, rollback and rerun checks. Both server processes exited zero with stopped collectors; MySQL stopped and its data was retained. No manual heavy-row deletion was used.

Earlier compile failures and the historical failed AI/Autobuilder receipts remain recorded. The inherited broad installer/full-magic milestone flag remains false; the current bounded gates are qualified by their explicit assertions and promoted phase marker. Existing prepayment proof is inherited from the exact original paid receipt, not recast in this retry.

Next: N14 installed stock early-death acceptance: cast the approved guardian/summon family through the normal installed command path; retain configured remains and foreign carried goods through original expiry, a cold restart and actual decay, then exercise default dissipation. Use the same code-proven references and preserve canonical history and custody. N15 same-template lifecycle modes and N16 high-volume/per-cycle restart remain separate follow-ups; no static-schema classifier stage.

All 154 candidates (152 required/two optional), 82 Sorcerer spells/four roots/12 supports and eight larger features remain unchanged. Whole-plan completion, publication and release remain disabled. No PR, push, merge, deployment or production/shared database operation was performed.

## N20 carrier reference extension — 10 October 2026

Folded Pocket retirement adds a distinct `GameItem` identity domain to the same
reference protocol. `FromItem` supplies the native item identity while retaining
the original-body reference for remains. PositionTargetType/PositionTargetId on
GameItem, Character and CharacterInstance rows, GameItem OwnerType/OwnerId, and
Crime ThirdPartyIItemType/ThirdPartyId use their actual item discriminator. Final
carrier removal also checks the existing declared incoming EF foreign keys.

ZeroGravityTether's live provider now reports both its Anchor and PhysicalTether;
its concrete persisted reader reads PhysicalTetherId as GameItem and preserves
AnchorType/AnchorId's separate domain. A character or body with the same number
cannot match this item target. Remains component definitions are selected only
when Body or Wound is in the requested target set; they have no GameItem identity
field and are not inspected for carrier retirement. These additions do not query
static AI/Autobuilder definitions, search numeric text, or guess identity fields.

Pocket binding records its borrowed SourceItem as immutable creation provenance.
That historical ID is not a live ownership claim or a teardown dependency: pocket
access, capacity and collapse consume the frozen carrier policy and lifecycle.
The N20 checkpoint records the fresh bounded validation and remaining gaps;
the earlier counts and publication statement above remain historical.
