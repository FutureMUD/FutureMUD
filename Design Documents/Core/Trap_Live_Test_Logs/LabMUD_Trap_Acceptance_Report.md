# LabMUD Trap Acceptance Report

Date: 11 August 2026  
Database: `labmud_dbo`  
Method: local FutureMUD telnet/raw-socket harness using the existing administrator account and disposable NPC instances. Credentials are redacted from every raw transcript.

## Result

The original live exercise created, deployed, and retained all requested scenario fixtures. It uncovered builder/electrical defects and an item-proximity trigger defect. The original observations below are retained as historical baseline evidence. The final proximity-event retest supersedes the old automatic-proximity conclusion: an NPC moving into an ordinary cell triggered a fresh item-anchored trap, which persisted as `Spent / 0`.

| Scenario | Builder/layer result | Triggerer result | Outcome |
| --- | --- | --- | --- |
| Tripwire -> explosive | Armed on a loaded bomb | NPC exit traversal reset the client immediately after trigger | Incomplete; trigger path reached but not a clean pass |
| Bear trap | Proximity trap with DirectDamage + Restraint deployed | Risky NPC disarm failure triggered it; NPC escaped restraint | Pass for damage/restraint payload path; automatic proximity remains failing |
| Magical proximity -> drug gas | Magical GasCloud trap configured with gas #7 | Automatic proximity did not fire; forced trigger emitted cloud but dosage display stayed empty | Partial; cloud creation passes, automatic trigger/drug-dose assertion fails |
| Tripwire -> delayed signal | Exit trap with five-second EmitSignal deployed on electronic gate | NPC triggered it; after delay gate became locked but remained open | Partial; trigger/delay/signal/lock pass, close fails |

## Follow-up verification

Subsequent controlled retests preserved the overall acceptance result, but resolved two ambiguities in the original evidence:

- [77_LabMUD_Bear_Trap_Delayed_State_Inspection.txt](77_LabMUD_Bear_Trap_Delayed_State_Inspection.txt) confirms that the cell-anchored proximity trap remains armed after the mover has had ample time to complete movement. This is a runtime automatic-proximity trigger defect, not merely a short client read window.
- [78_LabMUD_Npc_Movement_Suppression_Inspection.txt](78_LabMUD_Npc_Movement_Suppression_Inspection.txt) shows that NPC template #77 is a **NonBreather**. The earlier absence of a drug dose therefore does not demonstrate a gas-cloud dosing defect; a breathing NPC is required for that assertion. The automatic proximity trigger that should create the cloud remains independently failing.
- [79_LabMUD_Signal_Trap_Post_Electronic_Lock_Fix_Retest.txt](79_LabMUD_Signal_Trap_Post_Electronic_Lock_Fix_Retest.txt) confirms that the five-second signal payload locks the electronic gate without the former invalid-emote exception. [80_LabMUD_Signal_Trap_Post_Fix_Door_State_Assertion.txt](80_LabMUD_Signal_Trap_Post_Fix_Door_State_Assertion.txt) then confirms that it remains open. The remaining defect is electronic-door response to the trap signal, not the lock emote.

## Patch verification (11 August 2026)

- [88b_LabMUD_Fresh_Gate_Trap_Signal_Close_Lock_Validated.txt](88b_LabMUD_Fresh_Gate_Trap_Signal_Close_Lock_Validated.txt) is the clean signal vertical slice: the actual five-second trap payload closes, then locks, a fresh non-player-operable electronic gate. This validates the automatic-door response rather than a player command path.
- [92d_LabMUD_Patched_Item_Proximity_Armed.txt](92d_LabMUD_Patched_Item_Proximity_Armed.txt), [92e_LabMUD_Patched_Item_Proximity_Npc_Entry.txt](92e_LabMUD_Patched_Item_Proximity_Npc_Entry.txt), and [92f_LabMUD_Patched_Item_Proximity_Validated.txt](92f_LabMUD_Patched_Item_Proximity_Validated.txt) form a fresh item-anchor vertical slice. The specifically selected fifth bomb begins `Armed / 1`; after an NPC enters the ordinary destination cell it is `Spent / 0`, with both `DirectDamage` and `Restraint` payloads recorded.
- [93_LabMUD_Proximity_Event_Retest.txt](93_LabMUD_Proximity_Event_Retest.txt) is the final event-system verification. It uses the new opt-in `PerceivableProximityChanged` routing, a fresh seventh bomb, a Distant ordinary-cell threshold, and a non-administrator NPC crossing Cell #2668 to #2669. The trap is `Armed / 1` before the move and `Spent / 0` afterwards. It also records and verifies the startup-hydration safeguard required to prevent saved position restoration from being treated as a live proximity transition.
- The magical gas test's observer is still NPC template #77, a NonBreather. The patch rejects non-inhalable drug gases during template validation and refuses to dose them at runtime; a breathing observer is still required for a live drug-ledger assertion. The repaired proximity routing is shared by mechanical and magical proximity triggers.
- The explosive-client disconnect was not changed by this patch. The engine has an `ExplosionHeardEcho` default, so the captured socket reset does not yet establish a trap-framework source defect. It remains an isolated live-client investigation item.

## Key raw transcripts

Builder and retained fixture creation:

- [05_LabMUD_Builder_Three_Templates.txt](05_LabMUD_Builder_Three_Templates.txt) - first three template authoring surfaces.
- [12_LabMUD_Drug_Gas_Configuration.txt](12_LabMUD_Drug_Gas_Configuration.txt) and [20_LabMUD_Magical_Trap_Uses_Drug_Gas.txt](20_LabMUD_Magical_Trap_Uses_Drug_Gas.txt) - gas #7 and magical payload binding.
- [21_LabMUD_Signal_Trap_Template_Create.txt](21_LabMUD_Signal_Trap_Template_Create.txt) through [36b_LabMUD_Electronic_Gate_Door_Close_Mode.txt](36b_LabMUD_Electronic_Gate_Door_Close_Mode.txt) - signal source, electronic door/lock, and installed gate.
- [68_LabMUD_Tripwire_Template_Approval_Passed.txt](68_LabMUD_Tripwire_Template_Approval_Passed.txt) and [70_LabMUD_Remaining_Trap_Template_Approvals.txt](70_LabMUD_Remaining_Trap_Template_Approvals.txt) - complete submit/review/accept workflow; all four templates end Current.

Layer and triggerer evidence:

- [37_LabMUD_Tripwire_Explosive_Builder_And_Layer.txt](37_LabMUD_Tripwire_Explosive_Builder_And_Layer.txt) and [38_LabMUD_Tripwire_Explosive_Triggerer_Interrupted.md](38_LabMUD_Tripwire_Explosive_Triggerer_Interrupted.md).
- [41_LabMUD_Bear_Trap_Builder_And_Layer.txt](41_LabMUD_Bear_Trap_Builder_And_Layer.txt), [48_LabMUD_Bear_Trap_NonAdmin_Triggerer.txt](48_LabMUD_Bear_Trap_NonAdmin_Triggerer.txt), and [61_LabMUD_Bear_Trap_Retry_Triggerer.txt](61_LabMUD_Bear_Trap_Retry_Triggerer.txt).
- [54_LabMUD_Magical_Gas_Trap_Builder_And_Layer.txt](54_LabMUD_Magical_Gas_Trap_Builder_And_Layer.txt), [55_LabMUD_Magical_Gas_Trap_Triggerer.txt](55_LabMUD_Magical_Gas_Trap_Triggerer.txt), [56_LabMUD_Magical_Gas_Trap_Forced_Payload.txt](56_LabMUD_Magical_Gas_Trap_Forced_Payload.txt), and [59_LabMUD_Magical_Gas_Drug_Dose_Observed.txt](59_LabMUD_Magical_Gas_Drug_Dose_Observed.txt).
- [51_LabMUD_Gate_Anchor_Selection.txt](51_LabMUD_Gate_Anchor_Selection.txt), [52_LabMUD_Signal_Trap_Triggerer_And_Delay.txt](52_LabMUD_Signal_Trap_Triggerer_And_Delay.txt), and [53_LabMUD_Signal_Trap_Result_Inspection.txt](53_LabMUD_Signal_Trap_Result_Inspection.txt).
- [69_LabMUD_Normal_Trap_Layer_Experience.txt](69_LabMUD_Normal_Trap_Layer_Experience.txt) - current-template `trap lay` took the normal skill check and correctly left no trap after failure.

Recovery and final retained state:

- [64_LabMUD_Server_Restart_After_Template_Accept.txt](64_LabMUD_Server_Restart_After_Template_Accept.txt) - boot recovery after repairing the electronic-lock fixture.
- [67_LabMUD_Server_Restart_After_Review_Fix.txt](67_LabMUD_Server_Restart_After_Review_Fix.txt) - clean boot after both fixes.
- [71_LabMUD_Retained_Fixture_Final_State_After_Fixes.txt](71_LabMUD_Retained_Fixture_Final_State_After_Fixes.txt) - final runtime state.
- [93_LabMUD_Proximity_Event_Retest.txt](93_LabMUD_Proximity_Event_Retest.txt) - final proximity-event, NPC-movement, persistence, and restart verification.

## Builder, layer, and triggerer experience

Builders use `traptemplate edit`, `traptemplate set`, `traptemplate edit submit`, `traptemplate review`, and `accept edit`. The normal layer surface is `trap lay <template> on <anchor>`; it requires a Current revision and performs `SetTrapCheck`. Staff-only `trap create`, `trap debug`, and `trap trigger` were used solely to deploy deterministic fixtures and isolate payload execution.

The NPC triggerer was exercised via `as <npc> <command>` and, for the bear trap, ordinary `trap disarm here` / `trap struggle`. This produced direct triggerer-facing output for spotting, disarm failure, restraint, and escape.

## Defects found and repaired during verification

1. **Electronic lock no-actor defaults could crash startup.** `ProgLockGameItemComponentProto` created no-actor emotes with `$1`, yet the runtime supplies only the lock as an emote source. The retained component prototype #473 was repaired in `labmud_dbo` without deletion, and the engine defaults now use valid one-source emotes. The clean restart is captured in [67_LabMUD_Server_Restart_After_Review_Fix.txt](67_LabMUD_Server_Restart_After_Review_Fix.txt).
2. **Trap-template approvals crashed.** `EditableItemReviewProposal<T>` lacked an `ITrapTemplate` branch, so `accept edit` threw instead of resolving the current revision collection. The missing branch is fixed and guarded by a targeted automated test. The live passing approval flow is in [68_LabMUD_Tripwire_Template_Approval_Passed.txt](68_LabMUD_Tripwire_Template_Approval_Passed.txt) and [70_LabMUD_Remaining_Trap_Template_Approvals.txt](70_LabMUD_Remaining_Trap_Template_Approvals.txt).
3. **Item proximity was previously inferred from cell-entry witnesses.** The generic non-route-cell spatial service reports `Distant` even for co-located objects, so the historical `Immediate` fixture was not a meaningful ordinary-cell configuration. The engine now publishes opt-in, changed-only `PerceivableProximityChanged` events from movement, route/position/layer, containment, and party changes; traps enter a configured band rather than normalising a cell witness. Omitted proximity limits resolve to `Distant` in ordinary cells and `Immediate` in RouteCells. The final item-anchor proof is [93_LabMUD_Proximity_Event_Retest.txt](93_LabMUD_Proximity_Event_Retest.txt).
4. **Automatic doors reused player-only close eligibility.** An electronically controlled, non-player-operable door therefore ignored a signal that should close it. Automatic control now evaluates its physical state independently of player permissions, while retaining safety checks. The delayed trap proof is [88b_LabMUD_Fresh_Gate_Trap_Signal_Close_Lock_Validated.txt](88b_LabMUD_Fresh_Gate_Trap_Signal_Close_Lock_Validated.txt).
5. **Gas traps could be configured with a non-inhalable drug.** Template validation and gas-cloud dosing now both require a positive dose and an inhalable drug vector, preventing a misleading but inert configuration.

## Retained fixtures

- Overlay package #98, with test cells #2666, #2668 (origin), and #2669 (destination).
- Trap templates #1–#4, all Current after the exercise.
- Gas #7, `labmud test chuteslepan mist`, configured with the Chuteslepan drug.
- NPC template #77, `LabMUDTrapTestSubject`; several loaded NPC instances remain in the destination cell.
- Signal components #471–#473 and retained electronic gate prototype #912, installed in the north exit.
- The signal, bear, and two magical-gas instances are retained as Spent. The isolated item in Cell #2666 is intentionally untrapped because the normal `trap lay` check failed.

- Fresh sixth and seventh cranial-bomb anchors in Cell #2669 retain spent item-proximity bear-trap effects. The seventh is the final `PerceivableProximityChanged` NPC-movement proof.

No test fixtures were deleted. No telnet-client screenshot is included: the harness operates through a raw socket and does not provide an authentic terminal-rendering surface. The redacted raw transcripts above are the complete captured client evidence.
