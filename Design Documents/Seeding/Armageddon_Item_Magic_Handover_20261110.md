# Armageddon item magic: continuation handover

Date: 10 November 2026.

## Status and remaining work

The three approved implementation areas are present: item curse cleansing, Ethereal equipment transitions, and damaging glyph interactions. There are no pending user design approvals for these areas. Focused automated checks pass. This is an implementation handover, not a claim that a full world installation, live spellcasting, or save/restart walkthrough has been qualified.

Continue from the published item-magic PR branch in a fresh worktree. Do not copy the old mixed worktree wholesale: it also contains unrelated clan, Discord and character-generation changes owned by other work. The PR contains only the item-magic files, their documentation and this report.

| Priority | Remaining task | Evidence and completion criterion |
| --- | --- | --- |
| 1 | Check item-attunement access admission | `EtherealItemAttunementEffect.CapturePreparedSelection` checks local item identity, custody, weight, affinity state and grade. Unlike `PickupHazardEffect`, it does not explicitly call `CanSee` or `CanReachItem`. Establish whether the real mixed character/item trigger and casting service guarantee those checks. Exercise hidden/unseen targets, closed containing objects and another character's inventory. If a bypass exists, enforce the appropriate native admission before payment, preserving callback-free raw-token checks. This is a concrete inspection target, not a confirmed exploit. |
| 2 | Exercise the complete native casting/payment path | Current new tests mostly exercise effect operations and tokens directly. Cast the imported Glyph and both item stages of Shadow Passage through the configured casting service, including rejected/overweight/duplicate targets, selected grades, native component payment and costs. Confirm that a spent/dispelled glyph can be cast again on the same item and that a third Ethereal cast is refused without another destruction roll. Preserve the established native validation/payment safety. |
| 3 | Qualify selective inventory transitions with real equipment | Cover held, wielded and worn ready roots, non-ready roots, nested containers, ready children inside dropped parents, carried DISINTEGRATE items, and the worn/wielded exemption. Existing regressions exercise retirement transferring a root and a container callback transferring another root. Add targeted coverage for a sight-only additive planar overlay and for two detached roots when the first drop callback moves the actor. These latter fixes were reviewed statically but do not yet have dedicated regression cases. Confirm no foreign inventory is reclaimed and no detached item is orphaned. |
| 4 | Run a disposable full installation and restart smoke | Seed a new disposable setting/worldfile database using the private package; load the current engine and verify imported spell definitions, starting effects, opening stock and future reset-created items. Dispel/trigger/cleanse items, save, restart, and confirm that removed initial flags do not return. Include native copies/splits, item inspection, pickup, and return from the Shadow plane. No such live/full-install qualification was performed in this tranche. |
| 5 | Recheck the clean branch against its current base and hosted CI | Tests below used base `5048963c` plus the isolated item-magic patch. At publication preflight, `master` was `e67e9369` with the separate clan election PR #850 added; its changed files did not overlap this tranche. Reassess live base/CI rather than assuming either identifier remains current. Run only checks justified by new integration changes or failures. |

Fix confirmed defects found by these steps within the tranche. Bring additional source-behaviour choices to the user; do not invent new setting policy. Wider corpse possession/body replacement and remaining NO_ES tool/belt quirks are outside this tranche and remain in the broader [gap register](Armageddon_Worldfile_Remaining_Gaps_20261030.md).

## Decisions already approved

- Remove Curse clears source EVIL and NO_DROP item state without a grade contest. It releases only the designated Drop/Give restrictions; unrelated permission components and restrictions remain.
- Source Glyph is permanent, owner-agnostic and one-shot. It deals grade d4 HP damage and grade d6 stun, consumes itself and aborts that pickup. The caster is not exempt. Source Invulnerability allows pickup without firing or consuming it; the import uses the existing Unyielding Veil spell identity for this specific exemption, without turning that finite armour spell into general immunity.
- General item dispelling removes invisibility first. An armed glyph requires grade 3 and stops later intrinsic cleanup below that grade. Other intrinsic presentation/ethereal states and magic aura follow. Character curse cleansing remains separate.
- Item Ethereal has two permanent stages: readiness, then ethereal detection visibility. Each successful stage has grade-percent destruction risk. Weight must be strictly less than 3 kg per grade using the importer's existing base-unit conversion. An already ethereal item does not receive a third stage/risk roll.
- Ethereal visibility preserves material physical handling. Character Shadow Passage retains its installed native timed planar overlay and opts into ready-equipment retention. Non-ready descendants are ejected from the original inventory trees; ready children remain in their immediate parent, even when that parent drops. Returning clears direct equipment visibility while retaining readiness. Draw Into Flesh also clears item visibility without removing readiness.
- The Armageddon installer is a one-shot data installation. Do not introduce rerun/upsert or a repair of an existing walkthrough database as part of this continuation.

## Implementation map

| Surface | Main files and contract |
| --- | --- |
| Curse state | `FutureMUDLibrary/GameItems/Interfaces/ICurseRemovableItem.cs`; `ItemPermissionsGameItemComponent` and its prototype. Optional curse metadata and a saved removed state release only explicitly designated restrictions. Permission/callback-state differences prevent merging. |
| Item targeting | `CastingTriggerCharacter.cs` and `.Filter.cs`: optional `CanTargetItems`, default false, uses native inventory/local item lookup. Mixed triggers use perceivable filters. `MagicSpell.cs` retains character reflection when this mixed trigger is enabled. |
| Curse spell | `RemoveCurseEffect.Items.cs` adds optional `IncludeItems`; the shared Lift Hex stock definition enables it. Existing character cleansing remains. |
| Pickup glyph | `ItemPickupHazardEffect.cs`, `PickupHazardEffect.cs`, `IItemPickupAttemptEffect.cs`, and `ItemPickupAttempt.cs`. Permanent saved active/spent state, boolean immunity prog, native HP/stun damage, actual-action callback and custody fences. The existing ownership glyph mode remains available. |
| Intrinsic dispel | `IIntrinsicItemMagicEffect.cs`, `DispelMagicEffect.IntrinsicItems.cs` and operation reporting. `IncludeIntrinsicItemEffects` defaults false. Only unrestricted Remove/AnyCaster/AllowHostile/effect-key-any dispels strip these states; scoped spell/school/tag/illusion policies do not. Contest settings continue to govern spell-owned parents. |
| Item affinity | `EtherealItemAffinityEffect.cs`, `EtherealItemAttunementEffect.cs`, and `IPlaneSpecificInventoryRetention.cs`. Saved readiness/visibility, selected-grade token validation, native deletion for destruction risk, optional character planar route. |
| Planar transitions | `PlanarPresenceDefinition/Extensions`, `BodyPlanarTransitions.cs`, the preservation overload in `BodyFormSwitching.cs`, and native/spell planar effect callbacks. `retainPlaneSpecificInventory` defaults false. Effective remaining presence is used for removal cleanup. Existing default bag contents are not recursively filtered. |
| Return spell | `RemovePlanarStateSpellEffect` in `PlanarStateSpellEffects.cs`: optional `ClearItemEtherealVisibility`, independent of character `OnlySpellOwnedEthereal`. |
| Initialisation/persistence | `ItemInitialisation.cs` stores keyed one-time setup markers. Its native copy/morph hook preserves keys across stack splits/copies and prevents incompatible merging. This replaced an interim register approach because item copies do not automatically copy arbitrary register values. Do not restore that interim implementation. |
| Prog helpers | `addpickuphazard`, `addetherealitemaffinity`, `initialiseitemonce`. Existing spent/dispelled markers are preserved by onload helpers; explicit later spellcasting may enchant again. |
| Import wiring | `ArmageddonWorldfileItemFlags.cs`: effective prototypes, opening-stock XML, future-item onload setup, owned Glyph/Unravel/Shadow Passage/Draw Into Flesh spell overrides. It resolves owned identities rather than inventing global Invulnerability. |
| Tests | `ItemCurseRemovalTests.cs`, `ItemIntrinsicMagicTests.cs`, `EtherealItemMagicTests.cs`, and `ArmageddonWorldfileItemFlagTests.cs`. |

No EF model or migration is introduced. State uses existing component/effect XML and native onload progs. Glyph harm in this mode uses HP strategies; complex-health damage is not provided by the new template and is unnecessary for the approved import.

## Verification and its limits

Final isolated Core selection: **49 passed, 0 failed, 0 skipped**; stable source and complete counts. Receipt `20261110T002456Z-8f8d8aee34ed`.

Armageddon selection: **7 passed, 0 failed, 0 skipped**; stable source and complete counts. Receipt `20261110T001858Z-e58f75f75d6d`. This included generated-prog compilation with the external private package. Later changes were confined to runtime review repairs/tests; the seeder selection was not repeated afterward.

The earlier curse selection separately passed 45 checks. It overlaps the later selection and must not be added to the final count as unique coverage. Initial compile/fixture failures and the failing recursive-transition regression were repaired; their local receipts were retained. The final tests do not demonstrate full-suite, Linux, MySQL, live Telnet or deployment success.

Local evidence is under `.artifacts/item-magic/isolated/.artifacts/curse-gates/` in the original `07ba` worktree. These ignored artifacts will not exist in a fresh clone and are not required to build the branch. Reproduction commands from the new repository root:

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -Filter '(FullyQualifiedName~ItemIntrinsicMagicTests|FullyQualifiedName~EtherealItemMagicTests|FullyQualifiedName~DispelNativeOperationReportingTests|FullyQualifiedName~ItemCurseRemovalTests|FullyQualifiedName~StealthPlanarInventorySecurityTests|FullyQualifiedName~CorporealityCommandTests)&TestCategory!=Integration'

# Set this only to a private, external package directory. Do not copy its contents into the repository.
$env:FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY = '<private-package-directory>'
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'ArmageddonSeeder Unit Tests/ArmageddonSeeder Unit Tests.csproj' -Filter '(FullyQualifiedName~ArmageddonWorldfileItemFlagTests|FullyQualifiedName~ArmageddonPrimitiveStockDefinitionTests)&TestCategory!=Integration'
Remove-Item Env:FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY
```

Follow the checked-out test-execution instructions and restore missing standalone projects if required. The wrappers serialise builds with `-m:1`; do not run another build or mutate inputs during a recorded run. Use `dotnet` directly rather than launching generated executables if Windows blocks them.

## Clean continuation and boundaries

Fetch and check out the published item-magic PR branch into a new worktree; after it is merged, start from current `origin/master` instead. The branch commit contains the complete intended patch and this report. Do not depend on `.artifacts/item-magic/owned-paths.txt`, an old stash, another agent's branch, or unstaged changes in `07ba` to reconstruct it.

Use one source writer per worktree and isolated/disjoint worktrees for delegated implementation. Keep a single operator for any test database/server. Consult current `AGENTS.md` and relevant skills in the new checkout before continuing.

Original ArmSolo C files, zip contents and worldfile/data exports must remain outside Git. The PR includes native translation code, generated-prog construction, tests and documentation only. Private-package tests require the separately supplied package; its absence is an unexecuted check, not a pass.

The existing walkthrough instance/database was neither changed nor restarted for this tranche. Further work must not modify it without a new explicit instruction.

For the source-to-native mapping and builder options, see [item flags](Armageddon_Worldfile_Item_Flags.md), [configurable casting](../Magic/Configurable_Casting.md), [item runtime model](../Items/Item_System_Runtime_Model.md), and [corporeality and planes](../World/Corporeality_and_Planes.md).
