# Admin building tools audit — 23 October 2026

Audit baseline: `0fbdd1143ea8dc72e68b3f59e4bdd6acbdc896f8` (detached HEAD). The checkout was clean before this audit. These findings describe that revision, not regressions attributed to a particular change. The initial audit changed no game implementation, configuration, database or live world. Subsequent authorised P1, P2 and P3 repairs and Q01–Q07 implementations are recorded below; the inventory and original findings retain their baseline meaning.

The main problems are broken authoring paths, incomplete persistence and lifecycle handling, and help that describes a different parser contract. The engine already has extensive building tools; replacing them with another framework would create unnecessary duplication. Repair and extend their existing command/helper/domain boundaries.

The report records **44 findings: 6 P1, 29 P2 and 9 P3**, plus **7 explicitly labeled improvement proposals**. A finding may cover several commands sharing the same defect; these counts are not counts of missing systems or broken command declarations.

| Requested audit category | Main findings |
| --- | --- |
| 1. Missing entirely | F28–F31: missing productive agriculture/apiary configuration, complete body-prototype authoring, and check-template/decorator editors. These are missing building capabilities, not missing runtime simulation. |
| 2. Incomplete | F01–F32: permission, graph, save/load, parser, clone, revision and lifecycle failures. |
| 3. Missing/incomplete docs and help | F15, F26–F29, F32–F38, F43–F44. No command declaration is entirely missing its HelpInfo attribute. |
| 4. Confusing formatting/colour | F35–F36, F38, F40; validate actual rendered output, not just source strings. |
| 5. Flow differs from documentation | Parser, revision and selector failures throughout F07–F27, plus F33–F39 and F41. |
| 6. Workflow/scalability | F02, F24, F26, F40, F42–F44 and Q01–Q07. |
| 7. List/search/show conveniences | F22, F28, F30–F31, F41–F44 and Q02/Q05/Q06. |

## Scope and evidence

The audit covers the `MudSharp.Commands` surface under `MudSharpCore/Commands`, including command trees, help dispatch, editable/revisable helpers, and selected delegated builders, serializers and consumers. Player-facing and mixed-permission modules were included in the inventory so that admin branches were not excluded simply because they live outside a Builder module.

| Inventory measure | Count |
| --- | ---: |
| C# files under Commands | 129 |
| Files under Commands/Modules | 87 |
| Source command declarations | 633 |
| Declared command words, including aliases | 810 |
| Declarations explicitly annotated Guide or higher | 243 |
| Declarations lacking a `HelpInfo` attribute | **0** |
| Core methods named `BuildingCommand` returning bool | 806 |
| Files containing those methods | 777 |

These are Roslyn syntax counts, not unique commands available to one character. Permission comes from both command-tree membership and annotations/runtime guards. The 243 figure does not count every mixed-permission command. An earlier lexical scan undercounted interpolated-string-heavy files; the attached compiler-parser inventory supersedes it.

Artifacts: [full command and delegated-handler inventory](Admin_Building_Command_Inventory_2026-10-23.json) and [reproduction script](Inventory-Commands.ps1). Run the script with PowerShell 7; it uses its bundled Roslyn parser and does not execute game code.

**Evidence level:** findings below are supported by static source and call-path analysis. Reproduction steps are proposed acceptance scenarios, not claims that a Telnet session was run. No game build, unit suite, database round trip or server session ran. Live database helpfiles can override built-in help and were not inspected. Inventory coverage is complete for the declared source surface; detailed behavioral review is targeted, not a claim that all 806 handlers and every subordinate branch have passed acceptance.

Priority: **P1** = permissions, world/relationship integrity or server stability; **P2** = broken or missing usable authoring behavior; **P3** = documentation, presentation or convenience. Category numbers correspond to the user's seven requested audit categories. Missing authoring interfaces are distinguished from missing simulation systems.

## P1 repair follow-up — 23 October 2026

F01–F06 are implemented in the local working tree. The remaining P2/P3 findings and improvement proposals are outside this repair scope.

| Finding | Repair | Behavioural regression coverage |
| --- | --- | --- |
| F01 | Shared revision editing checks authority for every selector and rechecks it on set, submit, delete and obsolete. Read-only objects cannot be modified. Non-admins cannot mutate a selected revision after approval. Skin description editor submission and AI suggestion acceptance recheck permission/status when applying text. | Foreign public skins cannot be opened or revised; revoked authority blocks selected-item mutations; owner/admin draft editing remains available; actual editor callbacks reject stale approval/demotion and permit authorised saves. |
| F02 | Package deletion refuses any draft revision still supplying a room's active overlay, lists affected rooms and requires an explicit approved replacement before deletion. In-game help and the room guide describe the rule. | Active-package deletion has no world/selection/save effects; inactive draft deletion removes only that revision and its overlays, retaining the active revision's database rows. |
| F03 | Existing-predecessor moves are no-ops, self-targets are refused, and successful moves update both display order and progression links while retaining terminal stages. | Command-driven moves in both directions, no-ops and dependency rejection; real storyboard save/reload retains order, progression and disconnected Menu termination. |
| F04 | Magic-school parenting rejects self, descendants and already-cyclic ancestor chains; ancestry queries terminate on corrupt historical graphs. | Actual school builders exercise rejection, valid reparenting/clearing and bounded ancestry queries. |
| F05 | Appointment parenting validates the entire candidate ancestor chain; authority traversal terminates on repeated appointments. | Parent builder rejects invalid chains, permits reparenting/clearing, and cyclic authority checks terminate without granting an unrelated member control. |
| F06 | Appointment saving writes both parent and minimum-appointer rank IDs, including null clears. No schema change is required. | Runtime save/reload retains both restrictions and their removal; inherited appointment authority follows the reloaded parent relationship. |

Validation: the initial focused run passed 49 cases. A subsequent full-suite attempt stopped at a new test-fixture compilation error, which was corrected. The final full core run, `20261023T084929Z-63eb22ccb40e`, built successfully and passed **8,233/8,233 tests**, with zero failures or skips and matching source fingerprints at start/end. Command: `scripts/test-unit-core.ps1 -OutputMode Compact -TimeoutSeconds 900`. The focused final code review found no remaining actionable defects; `git diff --check` passed.

The added tests assert permissions, graph integrity and persisted behaviour, rather than source text or implementation shape. Database round trips use the runtime save/load code with an isolated EF in-memory provider. No live MySQL migration, server restart or Telnet acceptance run was performed. Historical graph corruption is guarded against, not automatically rewritten. Test receipts are under `.artifacts/test-runs/20261023T084929Z-63eb22ccb40e/` in this worktree.

## P2 repair follow-up — 23 October 2026

All 29 P2 findings are addressed in the local working tree: F07–F31, F33–F34, F39 and F41. P1 repairs remain intact. P3 findings and Q01–Q07 remain deferred. The baseline findings below are retained as historical evidence.

| Findings | Implemented outcome |
| --- | --- |
| F07–F08 | FutureProg append retains the original source; replacement remains replacement. Shop account closure completes the existing accept/reject/expiry flow without throwing. |
| F09–F10 | Skin cloning creates an authorised, independently named private draft. Shared clone/list dispatch rejects unsupported capabilities and invalid filters safely. |
| F11–F13 | Multi-NPC weights/removal, directional language intelligibility and random-name dice consume their operands correctly and persist changes. |
| F14 | Timezone creation, ordinary edit selection and legacy multi-argument updates reach distinct handlers. The runtime timezone now implements the editable interface required by legacy updates. |
| F15–F19 | Market clone source/destination parsing, per-user credit limits and restaurant credit-account selection are corrected. Magic-school and auxiliary-action clones retain their requested new names. |
| F20, F26 | Bank cloning and unfinished deletion/maintenance commands now give explicit limitations and supported alternatives. Bank cloning, dependency-aware weapon-attack/trait-expression/prog deletion and bulk room deletion are still unsupported. |
| F21–F22 | Route editing prefers a working revision, approval selects pending revisions, explicit revision selectors are supported, and bare vehicle-prototype show uses the current selection. |
| F23–F25 | Combat-message auxiliary joins reconcile removals/resaves; deletion clears staff editors and stale mutations are refused. Magic prompt visibility survives save/load with a compatible legacy default. |
| F27 | Invalid review selectors cannot silently open an arbitrary pending item. Skin review help describes the supported list/account/ID flows. |
| F28–F29 | Crop outputs, seed metadata and woodland outputs have indexed list/add/set/remove authoring. Apiary install/tend/yield settings are exposed; unusable harvest configurations are refused. Commodity production rejects overflowing weight calculations. |
| F30 | A registered bodyproto editor supports creation, inheritance, owned part copying, roles/exclusions, limbs, positions, movement and wearable sizing, with list/show/edit/close. Structural edits are blocked for prototypes or descendants used by races or persisted bodies. |
| F31 | Registered checktemplate, traitdecorator and stackdecorator editors support list/show/new/clone/edit/set/close, validated typed settings and advanced XML. Check assignment preserves existing enum-backed identities. |
| F33–F34 | Property creation help matches its economic-zone/value/account contract and explains account-derived ownership; population examples invoke marketpopulation. |
| F39, F41 | Programming shorthand preserves quoted targets. Item-group inclusion/exclusion combines group names and all forms correctly, including empty groups. |

New guides: [body prototype building](../Body_Prototype_Builder_Guide.md) and [check/decorator editors](../Check_and_Decorator_Editors.md). The owning agriculture, generic building, item, economy, magic, vehicle, time and FutureProg documents and relevant in-game help were updated.

Regression tests exercise command routing, permissions, revision selection, editor callbacks, runtime factories, rejected-input isolation, ownership, graph/cache behaviour and persistence. Coverage includes independent bodypart copies and limb reloads; inherited movement/default replacement; all supported check/decorator factories; timezone create/update/edit; agriculture configuration round trips; and actual apiary product weight/store consumption. No help-text or source-shape snapshot tests were added.

Final validation passed with complete counts, zero failures/skips/inconclusive results and matching source fingerprints at each run's start/end:

| Check | Result | Receipt under .artifacts/test-runs/ |
| --- | --- | --- |
| Focused P2 runtime/editor regressions | 112/112 passed | 20261023T095855Z-4b5c635e9497 |
| Full core suite | 8,293/8,293 passed | 20261023T100010Z-38257a6aadec |
| Shared-library project | 800/800 passed | 20261023T100448Z-2921c6254d48 |

Commands: `scripts/test-unit-core.ps1 -OutputMode Compact -TimeoutSeconds 900` and `scripts/test-unit.ps1 -OutputMode Compact -Project 'FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj' -TimeoutSeconds 900`. The focused filter selected DefinitionEditorTests, BodyPrototypeEditorTests, BodyPrototypeMovementSpeedTests, BuilderWorkflowRegressionTests, AgricultureOperationTests and AgricultureAuthoringTests.

Earlier focused attempts exposed bodypart-role composite-key persistence and legacy timezone interface defects, both corrected, alongside corrected compilation/test-fixture issues. Earlier failure receipts remain under the same artifact root; they are superseded by the final successful runs above. The focused independent reviews were read-only; their actionable findings were corrected before final validation. `git diff --check` passed.

Database round trips use isolated EF InMemory contexts. No live MySQL, Telnet or server-restart acceptance was run. Check/decorator edits save immediately but activate after restart; body-prototype cloning deliberately derives through inheritance, while owned part copying creates new identities. Crop seed requirements remain metadata; sowing project material requirements are configured separately.

## P3 repair follow-up — 23 October 2026

All nine P3 findings are addressed locally: F32, F35–F38, F40 and F42–F44. P1/P2 repairs remain in place. Q01–Q07 are separate improvement proposals and remain deferred.

| Finding | Outcome |
| --- | --- |
| F32 | Bank builder help directs account inspection to operational commands and identifies account-type deletion as unsupported. Property tenant help and handlers explicitly identify clan/shop additions as unsupported and direct users to individual tenant declarations. Existing supported operations are retained. |
| F35 | Natural-attack help includes the mandatory quality; butchery creation includes body, bodypart, item prototype and optional quantity. |
| F36 | Help-editor syntax now includes every target, closes brackets, fixes spelling and removes the nonexistent extra-text move operation. The shared guide supplies a complete authored-help workflow and explains editor cancellation and confirmation. |
| F37 | NPC-spawner help describes live editable records and close semantics, removes the nonexistent revision lifecycle, and lists supported filters. Unsupported custom filters are refused. |
| F38 | Spatial-package help describes version 5 and compatibility with versions 1–4, current room identities, landmarks and exit anchors; the cell alias remains supported. |
| F40 | Bodypart-group prompts translate their colour markup, the trait-expression example uses command colour, and built-in help reports source provenance rather than fabricating a current edit date. Authored help retains real edit dates with corrected wording. Metadata formatting distinguishes names, commands and ordinary prose. |
| F42 | Shared filtering now powers market, legal, overlay-package and authored-help lists: IDs, positive/negative keywords and relevant exact domain filters. Keyword signs retain their meaning for numeric text. Unknown/incomplete filters are refused. Bank/property/auction helper lists reject unsupported custom filters. Privileged help-editor catalogue discovery includes restricted topics; reader help continues to enforce visibility. |
| F43 | Registration carries executable command words/aliases, subsystem, access level, capability and summary into the existing help index. Exact aliases resolve before ambiguous prefixes; authored exact topics retain precedence and category/visibility restrictions apply throughout lookup. Built-in browsing shows the metadata. The adjacent Ward help-name collision with Clinch was corrected. |
| F44 | The common builder guide now covers live versus revisioned editing, create/edit/set/show/list/close, submission/review, persistence timing, cancellation, filters, family exceptions and concrete examples. Existing index linkage is retained and family guides link back to it. |

New tests protect reusable filter semantics, command integration, alias/permission/category precedence, authoritative authored topics, restricted editor catalogue visibility, quoted legal-authority selection, and registered words that differ from internal command identifiers. Alias-help rendering also runs at 40 and 120 columns. No tests snapshot help prose or assert source-code shape.

Final validation passed with complete counts, zero failures/skips/inconclusive results and matching source fingerprints at each run's start/end:

| Check | Result | Receipt under .artifacts/test-runs/ |
| --- | --- | --- |
| Focused help, discovery and builder regressions | 61/61 passed | 20261023T103112Z-de4a2eb2cb1b |
| Full core suite | 8,313/8,313 passed | 20261023T103327Z-55af9beb2537 |

Commands: `scripts/test-unit-core.ps1 -OutputMode Compact -Filter 'FullyQualifiedName~BuilderDiscoveryTests|FullyQualifiedName~CommandHelpInfoTests|FullyQualifiedName~CommandHelpSecurityTests|FullyQualifiedName~BuilderWorkflowRegressionTests' -TimeoutSeconds 600` followed by `scripts/test-unit-core.ps1 -OutputMode Compact -TimeoutSeconds 900`. Both build and test phases succeeded. The final source fingerprint was `58491418ba099b0d1e029fe71f4f26a16b3046304070edf99f2c4b0e505d307f`; the audit receipt was added afterward. The focused independent review's findings were corrected, and its follow-up found no remaining concrete defects. Earlier 56- and 59-case focused passes were superseded by the final runs above. Twenty new behavioural test cases were added for P3.

No live MySQL or Telnet session was run. The narrow-width check exercises formatted command output in unit tests; it is not a live terminal or monochrome-client acceptance claim. Unsupported bank account-type deletion and clan/shop tenant addition remain explicit limitations, as permitted by F32.


## Q01–Q07 implementation follow-up — 23 October 2026

All seven proposals now have working implementations in the existing builder command/helper architecture. Coverage is incremental where the proposal calls for typed providers; unsupported operations and uninspected dependency families are explicitly identified. P1–P3 repairs remain in place.

| Proposal | Implemented outcome and coverage |
| --- | --- |
| Q01 | Helper capabilities derive from the operation delegates used by dispatch, including specialised item/NPC/craft/loot cloning. Creation, cloning and deletion refuse unsupported operations before mutation. Identity-only custom-search callbacks were removed so they are not advertised as search capabilities. `capabilities` and shared fallback help report the flags. |
| Q02 | The admin `builderrefs` command reports exact known consuming fields for weapon attacks, trait expressions, FutureProgs, bank account types, item prototypes and skins. Providers use typed runtime references and state their coverage. Reports are partial: unloaded/serialized/other persisted consumers can remain, and zero results never authorise deletion. Unsupported deletion paths retain refusal and point to reporting where applicable. |
| Q03 | Shared `validate` uses existing revision submission checks without activation. Magic-school validation detects parent cycles; crop/woodland/apiary-harvest checks cover supported numeric/range and material/tag/output dependencies. Diagnostics distinguish errors/warnings and provide inspection commands for missing dependencies. Unsupported validators say so. Current validation call paths were checked for FutureProg execution; no preview execution or runtime simulation is performed. Craft's gate retains its existing cached-validity semantics. |
| Q04 | `item diff` and `itemskin diff`, and single-item review, compare explicit fields with the current approved revision rather than the immediately previous revision. Both sides require visibility. Skins include their content fields; prototypes include listed scalar fields, component references, variables and extra descriptions. Tags, planar settings and component internals remain normal show/review work; other families explicitly report unavailable field comparison. Admins also see the scoped reference report. |
| Q05 | `buildereditors` lists open framework-item selections and their live/revision persistence semantics. Shared `undo` narrowly restores the last single name-setting edit per family within 30 minutes in the same runtime session. It checks target identity, intervening names, current authority/status and collisions before restoring the exact captured text. Closing never rolls back; creation, bulk renames and other settings have no generic undo. |
| Q06 | Shared lists support limit/page/count, stable sorting and CSV output, with matching-result counts. Filters compose lazily until materialisation is required; row formatting follows page selection. Plane display order and hint priority are applied before paging unless explicitly overridden. CSV strips display markup, quotes fields and neutralises spreadsheet formula prefixes. Bespoke lists keep their own domain-filter contracts. |
| Q07 | Help metadata publishes concrete examples with prerequisites. Tests parse and replay the timezone examples through registered command dispatch, permissions and delegated setters, and cover malformed input and close semantics separately. Independent checks validate colour resets, including the engine's default-colour reset. Coverage is intentionally incremental; there are no prose snapshots or source-shape tests. |

The [shared builder guide](../Generic_Editable_Item_Builder_Workflows.md) documents syntax, lifecycle, provider coverage, limitations and recovery. Agriculture, item, economy and time documentation link their affected workflows to it.

Validation on the completed working tree:

- Focused builder checks: **93 passed, 0 failed, 0 skipped**, run `20261023T111528Z-f2806f642928`.
- Full core suite: **8,355 passed, 0 failed, 0 skipped**, run `20261023T111835Z-bf520e094d21`.
- Both builds and test executions exited 0, with complete counts and no receipt issues. Both runs used baseline HEAD `0fbdd1143ea8dc72e68b3f59e4bdd6acbdc896f8` plus the working-tree changes; source fingerprints matched at start and end: `664455f9102a209edf78cfef70dd232d5bea3562dacf7c630ffe4f9277e61a1f`.
- The proposal pass adds **42 behavioural cases** covering catalogue parsing/bounded formatting, permission and revision boundaries, guarded recovery, dependency consumers and executable help examples. The focused run also includes 51 existing builder regression/discovery cases.
- Receipts and detailed results are retained under `.artifacts/test-runs/<run-id>/`.

The focused review found dispatch-placement, approval-time undo, exact-text recovery and family-order paging issues; all were corrected, with regression coverage, and follow-up review found no remaining concrete defects. Initial compilation and test-fixture failures remain in their artifact receipts and are superseded by the final successful runs above. The new tests protect behaviour and bounded work rather than copied help prose or implementation shape.

The focused run includes a synthetic 25,000-entry catalogue measurement:

| Selection | Formatted rows | Elapsed milliseconds | Allocated bytes |
| --- | ---: | ---: | ---: |
| `--limit 50` | 50 | 139.30 | 5,377,944 |
| Default full list | 25,000 | 170.16 | 66,711,688 |
| `--count` | 0 | 0.46 | 201,488 |

These are single local measurements, including cold/JIT effects, rather than general performance guarantees. The regression assertions protect bounded row formatting and zero row formatting for count-only requests; they impose no timing threshold.

No live MySQL, Telnet or restart acceptance was performed. No persistence schema or shared-library contract changed in this proposal pass. Runtime sessions and supported name-undo records are transient; dependency reports are not deletion proofs.


## Findings requiring early repair (audit baseline)

### F01 — P1: Item-skin editing bypasses ownership on the ordinary target path

Categories: 2, 5. In [BaseBuilderModule.cs:166](../../../MudSharpCore/Commands/Modules/BaseBuilderModule.cs), `GenericRevisableEditDefault` applies `CanEditItemFunc` only when an explicit numeric revision is supplied. The ordinary ID/name branch can select any visible public skin. [EditableRevisableItemHelper.cs:1016](../../../MudSharpCore/Commands/Helpers/EditableRevisableItemHelper.cs) deliberately includes public skins in the visible set while line 1072 restricts editing to their associated builder. The shared set path does not recheck ownership, and [GameItemSkin.cs:284](../../../MudSharpCore/GameItems/GameItemSkin.cs) directly changes the description.

With another builder's public under-design/pending skin, a non-admin can use `itemskin edit <id>` followed by `itemskin set sdesc ...` to mutate that revision. An approved public skin can instead be used to create an unauthorized revision. The explicit `<id> <revision>` path correctly refuses the same caller. Centralize the edit check after resolution and check mutation/lifecycle actions too. Verify both selector forms, public/private visibility, ownership and revision states. This matters even though the requested audit is admin-oriented: the admin authoring infrastructure is shared with player skin creation.

### F02 — P1: Deleting an overlay package can orphan newly created rooms

Categories: 2, 6. [RoomBuilderModule.cs:4024](../../../MudSharpCore/Commands/Modules/RoomBuilderModule.cs) deletes package overlays without ensuring each surviving room retains a current overlay. `room package new Audit`, `room new`, then `room package delete` reaches a room whose only overlay belongs to that package. [Room.cs:830](../../../MudSharpCore/Construction/Room.cs) can set its current overlay to null while leaving the room and occupants registered. Saving dereferences it at line 1362; loading expects a matching overlay at line 1550. The database relationship permits null on deletion.

Refuse unsafe deletion or implement explicit relocation and destruction of exclusively new rooms. Acceptance must include occupied/new rooms, safe fallback overlays, save and restart; a confirmation prompt alone is insufficient.

### F03 — P1: Chargen reordering can corrupt progression and disagree with displayed order

Categories: 2, 5. [ChargenStoryboard.cs:89](../../../MudSharpCore/CharacterCreation/ChargenStoryboard.cs), reached by [ChargenModule.cs:325](../../../MudSharpCore/Commands/Modules/ChargenModule.cs), mishandles `chargen reorder <screen> <its existing predecessor>`. For P → S → F, it produces P → F and S → S. The no-op request disconnects the screen and creates a self-loop. Separately, a genuine successful move updates `_defaultNextStage` but not `_defaultOrder`, leaving overview, menus, dependency checks and saved order inconsistent. [ChargenScreen.cs:317](../../../MudSharpCore/CharacterCreation/ChargenScreen.cs) persists the two representations.

Rebuild both from one validated sequence; reject self-targeting and treat an existing-predecessor move as a no-op. Test both ordinary and no-op moves through save/reload and applicant progression.

### F04 — P1: Magic-school parenting permits cycles

Categories: 2. [MagicSchool.cs:309](../../../MudSharpCore/Magic/MagicSchool.cs) checks `school.IsChildSchool(school)`, rather than testing the candidate against the edited school. `magic school set parent <self>` and A → B → A are accepted on initially valid graphs. Recursive ancestry traversal at line 98 has no cycle guard and can exhaust the stack when queried against an unrelated school.

Reject self/descendant parents and defend traversal against already-invalid saved graphs. Cover self, two-node and longer cycles as well as normal ancestry.

### F05 — P1: Clan appointment parenting permits cycles

Categories: 2. [ClanModule.cs:6114](../../../MudSharpCore/Commands/Modules/ClanModule.cs) rejects direct self-parenting but permits `clan set appointment A parent B`, then `clan set appointment B parent A`. [ClanCommandUtilities.cs:19](../../../MudSharpCore/Community/ClanCommandUtilities.cs) recursively follows parents without tracking visited appointments; ordinary clan visibility checks can enter that traversal.

Validate the entire ancestor chain and make reads cycle-safe. Test viewing a restricted clan as a member holding neither office after attempts to create a cycle.

### F06 — P1: Appointment permission changes disappear after reload

Categories: 2. The parent and minimum-appointer setters in [ClanModule.cs:6075](../../../MudSharpCore/Commands/Modules/ClanModule.cs) and line 6122 mark appointments changed. [Appointment.cs:94](../../../MudSharpCore/Community/Appointment.cs) saves neither `ParentAppointmentId` nor `MinimumRankToAppointId`, although loading reads both. A builder sees the new restriction working until reload restores the old database value.

Persist both set and clear operations and test their permission effects after reload. F05 and F06 are independent: the cycle is dangerous during the current session even though the parent edit currently fails to persist.

## Broken and unfinished authoring operations

### F07 — P2: FutureProg append replaces source

Categories: 2, 5. [ProgModule.cs:930](../../../MudSharpCore/Commands/Modules/ProgModule.cs) and line 950 open an empty editor and assign the returned text directly to `FunctionText` for both append and replace. `prog set append` therefore discards the old source on submission despite the help promising append. Preserve existing source explicitly and test append versus replace on a nonempty prog.

### F08 — P2: Shop account closure throws after presenting confirmation

Categories: 2. [EconomyModule.cs:3064](../../../MudSharpCore/Commands/Modules/EconomyModule.cs) throws `NotImplementedException` after installing the acceptance proposal for `shop account close <account>`. This is a reachable operational command, not an unused stub. Complete the already-present confirmation flow and verify accept, reject and expired proposals without exceptions.

### F09 — P2: Shared dispatch exposes unavailable clone implementations

Categories: 2, 5. `itemskin clone <old> <new>` is advertised and routed to [BaseBuilderModule.cs:681](../../../MudSharpCore/Commands/Modules/BaseBuilderModule.cs), which always throws. `plane clone ...` reaches the non-revisable shared clone at line 956, but [EditableItemHelper.cs:2754](../../../MudSharpCore/Commands/Helpers/EditableItemHelper.cs) does not supply `EditableCloneAction`, causing a null delegate call. Plane help does not promise cloning; its exposed dispatcher still must reject it safely. Loot tables intercept cloning and have their own implementation, so they are not affected by the generic throw.

Make capabilities explicit and guard absent delegates. Implement skin cloning with appropriate ownership, revision and name semantics; either support plane cloning or return a clear unsupported-operation message.

### F10 — P2: Time-related list commands can invoke a null search delegate

Categories: 2, 7. Clock, timezone and calendar helpers in [EditableItemHelperTime.cs:32](../../../MudSharpCore/Commands/Helpers/EditableItemHelperTime.cs), line 143 and line 285 omit `CustomSearch`. [BaseBuilderModule.cs:908](../../../MudSharpCore/Commands/Modules/BaseBuilderModule.cs) calls it for a token not handled by the built-in ID or `+`/`-` filters. For example, `clock list foo` throws instead of filtering or reporting an invalid filter.

Provide a safe default/rejection and document supported filters. Existing ID and `+keyword`/`-keyword` filtering must remain available.

### F11 — P2: Template arguments are not consumed in multi-NPC spawners

Categories: 2, 5. [NPCSpawner.cs:424](../../../MudSharpCore/NPC/NPCSpawner.cs) resolves the whole `<template> <weight|remove>` tail as a template, then tries to interpret the same tail as its second argument. `npcspawner set npc <id> 2` and `... <id> remove` cannot perform the documented Multi-strategy operation. Consume the template token once, then read the weight/removal argument; test quoted names, IDs and persistence.

### F12 — P2: Language mutual intelligibility cannot parse its operands

Categories: 2, 5. [Language.cs:270](../../../MudSharpCore/Communication/Language/Language.cs) resolves `<language> <difficulty>` as a single language name. `language set mutual <id> Easy` fails before configuring the directional relationship. Consume the language separately, validate difficulty, then verify the chosen direction and reload behavior.

### F13 — P2: Random-name element dice cannot be configured

Categories: 2, 5. [RandomNameProfile.cs:555](../../../MudSharpCore/Character/Name/RandomNameProfile.cs) resolves the entire `<element> <dice>` tail as an element and reuses that tail as dice. `randomname set dice <existing-element> 1d2` fails. Separate the operands and test quoted element names, invalid dice and a persisted successful change.

F11–F13 share a parser misuse: `SafeRemainingArgument` is non-consuming ([StringStack.cs:45](../../../FutureMUDLibrary/Framework/StringStack.cs)). It is appropriate for final free text, not a replacement for consuming a leading argument.

### F14 — P2: Legacy timezone update syntax cannot reach its handler

Categories: 2, 5. [TimeModule.cs:426](../../../MudSharpCore/Commands/Modules/TimeModule.cs) advertises `timezone edit <clock> <alias> "<name>" <hours> [minutes]`. The routing gate at line 439 sends `edit` straight to the generic editor; the legacy `edit` branch at line 483 is unreachable. Disambiguate ordinary editor opening and the legacy multi-argument update, and test both.

### F15 — P2: Market template, influence and category clone parsers reuse the whole tail

Categories: 2, 3, 5. [EconomyModule.cs:10182](../../../MudSharpCore/Commands/Modules/EconomyModule.cs), line 10574 and line 10815 resolve the whole `<source> <new name>` tail, then reuse it as the destination name. `mit clone 12 NewTemplate` and `mc clone 12 NewCategory` cannot resolve their sources; the influence variant can clone with a misleading name. Help also omits the source argument in affected clone forms. Consume the source with `PopSpeech()`, then read/validate the destination; cover ID and quoted-name sources.

### F16 — P2: Per-user shop credit limits cannot parse the documented user and amount

Categories: 2, 5. [EconomyModule.cs:2882](../../../MudSharpCore/Commands/Modules/EconomyModule.cs) feeds `<user> <limit>` to `GetByPersonalName` and subsequently to currency parsing without consuming the user. Repair `shop account userlimit <account> <user> <limit>` and test quoted users, finite amounts and zero/unlimited behavior.

### F17 — P2: Restaurant credit-account selection consumes input inside a predicate

Categories: 2, 5. [EconomyModule.Restaurants.cs:1255](../../../MudSharpCore/Commands/Modules/EconomyModule.Restaurants.cs) calls `PopSpeech()` for each account tested by `FirstOrDefault`. `bill pay mine credit <second-account>` consumes the desired name while checking the first account, so subsequent accounts compare against the wrong token. Capture the selector before searching. Test second/later accounts and quoted names across payment/refund consumers of this helper.

### F18 — P2: Magic-school clones ignore their requested name

Categories: 2, 5. The helper validates the new name at [EditableItemHelperMagic.cs:379](../../../MudSharpCore/Commands/Helpers/EditableItemHelperMagic.cs), but [MagicSchool.cs:46](../../../MudSharpCore/Magic/MagicSchool.cs) copies `rhs.Name` instead of `newName`. `magic school clone <source> DistinctName` creates another source-named school. Use the validated name and verify the saved clone remains independent and uniquely named.

### F19 — P2: Auxiliary combat-action clones ignore their requested name

Categories: 2, 5. [CombatBuilderModule.cs:877](../../../MudSharpCore/Commands/Modules/CombatBuilderModule.cs) never consumes the documented destination name and calls parameterless `Clone()`. [AuxiliaryCombatAction.cs:57](../../../MudSharpCore/Combat/AuxiliaryCombatAction.cs) copies the old name. Repair `auxiliary clone <id> NewName`, including validation and round-trip checks.

### F20 — P2: Bank cloning is advertised but silently does nothing

Categories: 2, 3, 5. `bank clone <bank> <name> <code>` appears in [EconomyModule.cs:5060](../../../MudSharpCore/Commands/Modules/EconomyModule.cs). Its delegate at [EditableItemHelperEconomy.cs:322](../../../MudSharpCore/Commands/Helpers/EditableItemHelperEconomy.cs) has the actual implementation commented out at lines 350–354; it also conflates destination name and code. Valid input can exit without an outcome. Define what is cloned, including account types/branches and unique codes, or clearly mark the operation unsupported.

### F21 — P2: Vehicle-route edit and approval resolve the wrong revision

Categories: 2, 5. [VehicleModule.Routes.cs:216](../../../MudSharpCore/Commands/Modules/VehicleModule.Routes.cs) uses the general resolver at line 313. [RevisableAll.cs:168](../../../MudSharpCore/Framework/RevisableAll.cs) prefers Current. With current r0 and pending r1, `vehicleroute approve <id> <comment>` reapproves r0 and leaves r1 pending; editing by ID can create another revision instead of reopening a draft. Reuse the separate edit/review selection approach already present for vehicle prototypes. Test current+pending, current+draft and explicit revisions.

### F22 — P2: Bare vehicle-prototype show can display another prototype

Categories: 2, 5, 7. [VehicleModule.cs:2803](../../../MudSharpCore/Commands/Modules/VehicleModule.cs) resolves an empty selector instead of using the editing item. The empty-name lookup can match an unrelated Current prototype. The documented workflow uses bare `vehicleproto show` to retrieve child IDs after editing. Fall back to the current editor or require an explicit selector, and test while editing the second of two prototypes.

### F23 — P2: Combat-message auxiliary restrictions do not save removals

Categories: 2. [CombatMessage.cs:247](../../../MudSharpCore/Combat/CombatMessage.cs) reconciles weapon-attack joins but only adds auxiliary-action joins. Toggle an auxiliary move on, save, toggle it off with `cm set move <id>`, then reload: the old database association returns. Another save with a retained association can attempt a duplicate composite-key insert. Reconcile both collections and test add, unchanged resave, removal and reload.

### F24 — P2: Deleting a combat message leaves a stale editor

Categories: 2, 6. [CombatBuilderModule.cs:1262](../../../MudSharpCore/Commands/Modules/CombatBuilderModule.cs) deletes the message but leaves `BuilderEditingEffect<ICombatMessage>`. With an already-flushed message, `cm delete`, accept, then `cm set priority 1` edits and requeues the deleted object. [CombatMessage.cs:238](../../../MudSharpCore/Combat/CombatMessage.cs) dereferences the missing database row. Clear every editor referencing the deleted object and refuse subsequent mutation; include a second staff editor in acceptance.

### F25 — P2: Magic capability prompt visibility is not persisted

Categories: 2. `magic capability set showpower` toggles `ShowMagicResourcesInPrompt` at [SkillLevelBasedMagicCapability.cs:425](../../../MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs). Neither its loader at line 81 nor `SaveToXml` at line 124 includes the flag, so false reverts to the default true after reload. Persist it with a backward-compatible default and test both values.

### F26 — P2: Several deletion/maintenance commands remain exposed stubs

Categories: 2, 3, 6. `weaponattack delete` prints `TODO` at [CombatBuilderModule.cs:542](../../../MudSharpCore/Commands/Modules/CombatBuilderModule.cs); `te delete` prints `TODO` at [BuilderModule.cs:2198](../../../MudSharpCore/Commands/Modules/BuilderModule.cs); `prog delete` routes to a stub at [ProgModule.cs:1197](../../../MudSharpCore/Commands/Modules/ProgModule.cs). Mass room deletion is also explicitly unfinished at [RoomBuilderModule.cs:724](../../../MudSharpCore/Commands/Modules/RoomBuilderModule.cs).

These are missing lifecycle capabilities, not evidence that dependent content can safely be removed. Establish reference-aware refusal, reassignment/retirement where appropriate, confirmation and cleanup semantics. Until implemented, return a precise limitation instead of placeholder output.

### F27 — P2: Review selectors advertised as names or “mine” select the first pending record

Categories: 2, 3, 5. Item-skin admin help advertises `review all|mine|<builder name>|<id>` at [ItemBuilderModule.cs:1600](../../../MudSharpCore/Commands/Modules/ItemBuilderModule.cs). [BaseBuilderModule.cs:439](../../../MudSharpCore/Commands/Modules/BaseBuilderModule.cs) treats any nonnumeric unrecognized selector as the first pending item, without applying builder ownership/name filtering. `itemskin review mine` or a builder name therefore opens an arbitrary first pending skin. The selected record is displayed before confirmation, but the selector's promise is false. Align help with `review list mine` / `review all by <account>` or implement the advertised selectors; reject unknown input instead of silently changing its meaning.

## Authoring interfaces missing or materially incomplete

### F28 — P2: Crop and woodland definitions lack harvest-output authoring

Categories: 1, 2, 7. [AgricultureModule.cs:1077](../../../MudSharpCore/Commands/Modules/AgricultureModule.cs) and line 1674 create definitions with empty output collections. The setters do not expose output configuration; crops also lack seed-requirement authoring. Harvesting consumes these collections at [AgricultureField.cs:1226](../../../MudSharpCore/Work/Agriculture/AgricultureField.cs) and line 1470. Thus `field crop create Moonwheat` or `field woodland create Moonwood` cannot be completed into a productive definition using the available editor.

Add list/add/set/remove/show operations for material/tag/product quantities and seed requirements, following existing herd-output conventions. Verify create → configure → reload → harvest. This is a missing builder interface over existing agriculture simulation.

### F29 — P2: Apiary operation configuration has no command route

Categories: 1, 2, 3. [AgricultureOperation.cs:292](../../../MudSharpCore/Work/Agriculture/AgricultureOperation.cs) has installation, tending and yield setter methods, but none has a caller in the checkout. [AgricultureModule.cs:1872](../../../MudSharpCore/Commands/Modules/AgricultureModule.cs) exposes operation types without wiring those parameters. A custom HarvestApiary operation cannot be given the necessary commodity products/multiplier through commands; defaults include no products and a zero multiplier.

Expose the existing capability through operation editing and show/help output, with validation for a usable operation. Test a new operation through save/reload and an actual harvest.

### F30 — P2: Body prototypes have inspection and component editing, but no complete in-game authoring workflow

Categories: 1, 2, 7. [ShowModule.cs:984](../../../MudSharpCore/Commands/Modules/ShowModule.cs) lists bodies and line 1014 shows a body. `bodypart` can edit and clone parts; race creation selects an existing prototype. [BodyPrototype.cs:36](../../../MudSharpCore/Body/Implementations/BodyPrototype.cs) loads the database model and has no `BuildingCommand`/new/clone builder surface. No prototype creation route was found in Commands or a `BodyProtos.Add` creation route in Core. The `body` command in CharacterInformation inspects character anatomy and manages alternate forms; it is not a body-prototype editor.

A builder cannot author an entirely new anatomy graph through the available in-game workflow. Add a deliberate prototype editor for inheritance, limbs/parts, positions, movement and wear sizing with graph validation. This is a substantial design slice; cloning one part is not a substitute, and the runtime anatomy system itself is not missing.

### F31 — P2: Check templates and display decorators are inspectable but lack authoring interfaces

Categories: 1, 7. [ShowModule.cs:734](../../../MudSharpCore/Commands/Modules/ShowModule.cs) reads check templates directly for display; lines 769 and 808 list check types/instances. Trait decorators are listed/shown at lines 1494/1519 and stack decorators listed at line 3624. No create/edit command route was found for these catalogue definitions in Commands; decorator implementations likewise have no builder dispatch. Traits can select decorators and trait expressions can change formulas, which is useful but does not edit template policies, difficulty modifiers or decorator definitions.

Provide scoped editors for the configurable records, preserving hard-coded check-type identities. Prioritize operations demonstrably needed by game builders; this is not a proposal to make every engine enum dynamically editable.

### F32 — P3: Bank and property subordinate workflows advertise unavailable operations

Categories: 2, 3, 5. [Bank.cs:203](../../../MudSharpCore/Economy/Banking/Bank.cs) advertises `accounts`/`account` under builder help, but the following dispatcher has neither. `bank set type remove <name>` reaches “Coming soon” at line 344. Property tenant `addclan` and `addshop` likewise return “Coming soon” at [PropertyModule.cs:1019](../../../MudSharpCore/Commands/Modules/PropertyModule.cs) and line 1031. Complete or clearly label those operations. Skill pause/unpause are also unfinished, but their help already discloses that limitation; they should not be mislabeled as undocumented surprises.

## Documentation, help and presentation defects

### F33 — P2: Property creation help describes a different contract

Categories: 3, 5. [PropertyModule.cs:92](../../../MudSharpCore/Commands/Modules/PropertyModule.cs) says `<name> <code> <economiczone>`. [EditableItemHelperEconomy.cs:44](../../../MudSharpCore/Commands/Helpers/EditableItemHelperEconomy.cs) actually requires `<name> <economiczone> <value> <bank account>`; that account determines ownership. Correct syntax and explain the ownership side effect. Validate a complete quoted-name example against the parser.

### F34 — P2: Market-population help names the market command

Categories: 3, 5. [EconomyModule.cs:11193](../../../MudSharpCore/Commands/Modules/EconomyModule.cs) prefixes its population examples with `market`, while registration is `marketpopulation`/`mp` at line 11215. Copying examples can create/edit a market instead of its population definition. Correct every prefix, including create, close and set examples; this is more consequential than a cosmetic typo.

### F35 — P3: Combat and butchery examples omit mandatory arguments

Categories: 3, 4, 5. [CombatBuilderModule.cs:284](../../../MudSharpCore/Commands/Modules/CombatBuilderModule.cs) omits the mandatory quality argument for `weaponattack natural`, required at line 711. [ActivityBuilderModule.cs:337](../../../MudSharpCore/Commands/Modules/ActivityBuilderModule.cs) says `butcheringproduct edit new <name>`, but creation at line 532 also requires body, bodypart and item prototype. The [butchering guide:107](../../Crafting/Butchering_System.md) already contains fuller syntax. Synchronize examples. The `shield` spelling elsewhere in combat help is an accepted abbreviation of `shieldtype` under current dispatch; it is not counted as a broken command.

### F36 — P3: The help editor's own help omits targets and has malformed syntax

Categories: 3, 4, 5. [HelpModule.cs:685](../../../MudSharpCore/Commands/Modules/HelpModule.cs) omits `<helpfile>` from `keywords`, `category`, `subcategory` and `extra prog`; `text <helpfile` and `subcategory <subcategory` lack closing brackets, and `helpile` is misspelled. Its handlers require the missing operands. Nested extra-text usage also mentions `move`, which has no dispatch case. Fix the help from the handlers and cover one working example for each branch. This editor should demonstrate the expected help conventions.

### F37 — P3: NPC-spawner help promises a revision lifecycle it does not have

Categories: 3, 5. [NPCBuilderModule.cs:1097](../../../MudSharpCore/Commands/Modules/NPCBuilderModule.cs) advertises submission/review while the entry point uses the non-revisable helper at line 1108. Align the help with immediate editable-item changes, or explicitly design a revision lifecycle; merely copying revisable help makes the workflow misleading.

### F38 — P3: Spatial-package in-game help is behind the documented format

Categories: 3, 4, 5. [RoomBuilderModule.SpatialPackages.cs:17](../../../MudSharpCore/Commands/Modules/RoomBuilderModule.SpatialPackages.cs) describes version 3 and importing versions 1–2, while the [room-building guide:131](../Room_Building_Builder_Guide.md) describes current version 5 and compatibility with versions 1–4. Help also says “room, room” and uses old CELL PACKAGE wording. Retain the supported `cell` alias, but present the current room/package model and compatibility contract consistently.

### F39 — P2: Documented programming shorthand loses quoted targets

Categories: 2, 5. [ElectronicsModule.cs:725](../../../MudSharpCore/Commands/Modules/ElectronicsModule.cs) reconstructs a stack from an already-unquoted token. `programming "control unit" logic ...` becomes target `control` and verb `unit`; `programming item "control unit" ...` preserves the explicit form. Help still promises shorthand. Forward the original/undone stack or preserve quoting, and test both forms with multiword targets.

### F40 — P3: Colour markup and builder presentation still have concrete defects

Categories: 4. The bodypart-group type prompt in [EditableItemHelper.cs:334](../../../MudSharpCore/Commands/Helpers/EditableItemHelper.cs) sends literal `#3...#0` without substitution. [PlayerOutputHandler.cs:41](../../../MudSharpCore/PerceptionEngine/Handlers/PlayerOutputHandler.cs) wraps/buffers strings but does not translate that markup; the player connection also does not substitute it. The example at [BuilderModule.cs:1967](../../../MudSharpCore/Commands/Modules/BuilderModule.cs) starts a formula with `#0` instead of command/example colour. [CommandHelpInfo.cs:190](../../../MudSharpCore/Commands/CommandHelpInfo.cs) prints “Lasted edited” and reports `UtcNow` as the built-in help's edit date rather than source provenance.

Use yellow command syntax (`ColourCommand`/`#3`), cyan names/types, green values, red errors, and explicit resets; use the established titled show blocks and spacing. Keep checks semantic: a green table border is not by itself a violation. Include narrow terminals, monochrome output and rendered markup in presentation acceptance. Do not treat all legacy prose or every different heading as a defect without checking its use.

## Search, show and workflow gaps

### F41 — P2: Item-group filters disagree with their documented meaning

Categories: 5, 7. [ItemBuilderModule.cs:1435](../../../MudSharpCore/Commands/Modules/ItemBuilderModule.cs) omits group names from positive matching. Negative filtering uses `Forms.Any(nonmatching)` instead of excluding groups with any matching form. A group with one tavern form and another form remains in `itemgroup list -tavern`; a name-only match disappears from `+keyword`. Fix group/name/form semantics and test mixed, empty and name-only groups.

### F42 — P3: Several list surfaces discard filters or provide no domain filters

Categories: 6, 7. Market template/influence/category/market lists at [EconomyModule.cs:10286](../../../MudSharpCore/Commands/Modules/EconomyModule.cs), 10674, 10919 and 11075 do not process filters. Legal lists at [LegalModule.cs:982](../../../MudSharpCore/Commands/Modules/LegalModule.cs), 1031, 1080 and 1125 consume the authority but ignore further criteria. Property/auction/bank helper custom searches return input unchanged. [ShowModule.cs:3658](../../../MudSharpCore/Commands/Modules/ShowModule.cs) similarly has no overlay filter processing; `hedit list` at [HelpModule.cs:761](../../../MudSharpCore/Commands/Modules/HelpModule.cs) lists all authored help without using its remaining arguments.

Add relevant owner/zone/type/status/dependency filters, or reject unknown tokens. Do not claim all searching is absent: generic `+keyword`, `-keyword` and numeric-ID filters already work for the helper-backed families. Unrecognized input silently returning everything is particularly unhelpful on large catalogues.

### F43 — P3: Built-in help discovery lacks alias and subsystem metadata

Categories: 3, 6, 7. [CommandHelpInfo.cs:137](../../../MudSharpCore/Commands/CommandHelpInfo.cs) exposes only `HelpName` as a keyword, assigns every built-in entry `Commands / Built-In`, and supplies a generic tagline. [HelpModule.cs:824](../../../MudSharpCore/Commands/Modules/HelpModule.cs) searches those help names/keywords, not registered aliases. Thus an alias such as `te` or `wp` works as a command but need not resolve its built-in topic via `help te` / `help wp` unless a database helpfile supplies it.

Carry canonical name, aliases, subsystem, capability and summary metadata into the existing help index. `commands`, `immcommands`, `help on commands` and `show` already exist; an entirely new discovery system is not required.

### F44 — P3: Shared builder workflow documentation is much narrower than its title

Categories: 3, 6, 7. [Generic_Editable_Item_Builder_Workflows.md](../Generic_Editable_Item_Builder_Workflows.md) thoroughly explains bulk name changes but not the general edit/new/set/show/list/close workflow, the difference between live editable records and revisioned drafts, submission/review, filters, persistence timing, editor cancellation or the meaning of close. Those contracts are spread across [BaseBuilderModule.cs](../../../MudSharpCore/Commands/Modules/BaseBuilderModule.cs) and many separate help constants.

Extend this existing document into the common reference, with command-family exceptions and runnable examples. Link to it from the building documentation index and family guides. Do not imply that closing a non-revisable editor discards changes: it merely removes the editing effect.

## Improvements worth designing after the defects

These are proposals, not claims that documented features are broken or that no subsystem already implements a local version.

| Proposal | Evidence and intended benefit | Acceptance boundary |
| --- | --- | --- |
| Q01: Shared capability metadata | F09/F10/F26/F32 show dispatch and help do not reliably know which create/clone/delete/filter operations exist. | One capability source drives advertised operations and safe dispatch; retain established helper APIs. |
| Q02: Dependency/where-used reporting | The generic helper contract has no common reference report. Authors need to know who uses a material, expression, prog, template or prototype before replacement/deletion. | Incremental typed providers, starting with the unfinished deletion families; show exact references and offer safe refusal. Existing subsystem-specific reports remain usable. |
| Q03: Preflight and preview before activation | Loot tables, routes and vehicle prototypes already offer useful validation/preview models. Other editors can produce incomplete definitions or bad relationships (F02–F06, F28–F29). | A consistent `validate` result with errors/warnings and dependency links; no blanket requirement to execute arbitrary FutureProgs during preview. |
| Q04: Revision diff and change review | Generic review shows full objects/history, but lacks a shared field-level diff between proposed and current content. | Reviewers can see changed fields and affected references; retain the existing revision status model. |
| Q05: Editor-session overview and recovery | Independent `BuilderEditingEffect<T>` contexts make it possible to edit several families concurrently; non-revisable edits are immediate. | An overview of open editors and current targets, explicit commit semantics, and narrowly designed undo/rollback where supported. Do not falsely label close as cancel. |
| Q06: Catalogue-scale output | Generic list code materializes/filter-copies whole catalogues and renders full tables. PlayerOutputHandler does page output, so pagination is **not absent**; the full result is still built first. | Optional limit/page/count/sort/export, useful “showing N of M” counts and domain filters; benchmark a representative large catalogue before promising speedups. |
| Q07: Executable help contracts | All 633 commands have HelpInfo, yet F11–F16 and F33–F39 demonstrate drift below that level. Existing help tests cover selected modules and attributes, not every documented workflow. | Parse and replay representative examples against actual handlers, including quoting, malformed input, permission tiers and delegated `set` operations; validate colour resets separately. Avoid asserting only that help contains a keyword. |

## Coverage by system and existing strengths

| Surface | Review depth and important existing capability |
| --- | --- |
| Command trees, module registration, shared builders, help | Direct inspection of dispatch, permissions, review/list/show/set/clone and help discovery; complete syntax inventory. Generic list/search/show/edit, bulk naming, revision history and delete confirmation already exist. |
| Rooms, overlays, routes, spatial packages | Targeted lifecycle and help tracing. Room filters, reviewable overlays, route validation and spatial transfer already exist; a general worldbuilding system is not missing. |
| Heritage, languages, name generation, NPCs, chargen | Targeted delegated parsing, graph and persistence inspection. NPC instances, AI type help, unique names and existing filters are available. |
| Time and weather | Entry/help/helper inspection and targeted timezone routing. Configurable calendar/clock/timezone and weather editors exist; no weather simulation acceptance run was made. |
| Items, skins, components, crafting, butchery, projects, traps | Entry/helper review and selected delegated contracts. Revision/review, component type help, craft analysis/find/categories/material previews and trap parameter help/validation already exist. Every component/craft-product subtype was not deeply inspected. |
| Combat, health, magic | Selected setters, clone/delete and persistence consumers inspected. Every spell effect, power implementation, surgery subtype and combat action was not exhausted. |
| Agriculture | Definition creation, output/mutation paths and harvesting consumers inspected. List/show/create/set/delete already exist; missing filtering/cloning is a convenience proposal distinct from missing productive output configuration. |
| Economy, property, legal, clans | Selected parser, relationship, lifecycle, reporting and save/load contracts inspected. Analytics, employment diagnostics, rent/revenue reporting, and shop/hospital/stable/restaurant administration already exist. |
| Vehicles | Prototype/route selection and documented workflow traced. Prototype validation, access audits, fleet/service scheduling and transit inspection already exist. |
| FutureProg and electronics | Selected editor/quoted-target paths inspected; argument help, typed registers and explicit programming forms already exist. Not every network/application command was reviewed deeply. |
| Arenas, AI storytellers, staff/implementor | Registrations and selected helper surfaces inspected. Arena ratings/management and AI subscription/tool/reference filters exist. Detailed acceptance of all subordinate setters, debug commands and tool-call settings remains outside the completed trace coverage. |
| Remaining player/NPC modules | Included in the full declaration/help inventory and screened for admin entry points. Ordinary gameplay behavior is not certified by this builder audit. |

Relevant source documentation consulted included room building/spatial packages, generic editable workflows, butchering, economy runtime/workflows, clan, FutureProg and vehicle material. The audit did not read every design document or every database-authored helpfile. No finding equates absence of a command-specific design document with proof that all documentation is absent.

## Suggested delivery order and verification

1. **Protect existing worlds and permissions:** F01–F06, followed by F07/F23/F24/F25. Add focused permission, graph and persistence regressions. Preserve stable identities and repair existing invalid data only through explicit, bounded migration/maintenance policy.
2. **Restore advertised operations:** parser/clone/revision failures F08–F22, F27 and F39/F41. Use parameterized cases for IDs, quoted names, missing operands, unwanted trailing tokens and multiple candidate records.
3. **Finish essential authoring:** agriculture outputs/apiary configuration, then separately designed body/check/decorator editors and reference-aware removal. Define the smallest useful slice before implementing these larger capabilities.
4. **Synchronize documentation and help:** correct the concrete examples now; then extend the common workflow guide and alias discovery. Validate rendered help in narrow/standard/wide terminals and colour-disabled accounts.
5. **Scale and improve workflows:** Q01–Q07 incrementally, measuring real catalogue sizes and reusing existing services.

For each implementation slice, run its owning unit tests, then a disposable-world builder scenario that creates, configures, shows/lists, saves, reloads and uses the authored content. Graph/revision/deletion changes need negative cases and, where relevant, two simultaneous staff editing contexts. A successful build or help-attribute presence check would not prove these workflows work.

Audit artifact validation: the Roslyn inventory generator was executed successfully; generated JSON, referenced file paths, line bounds, document links and whitespace were checked. This validates the audit artifacts, not gameplay. The game source stayed at the audited revision throughout the review.
