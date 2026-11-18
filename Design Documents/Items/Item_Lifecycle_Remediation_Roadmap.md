# Item lifecycle remediation roadmap

Start with the two demonstrated data-loss paths. Both can be repaired and shipped independently of boot changes. Then establish who may mutate an item and when it becomes active. Power, timers and late-loading owners can build on those guarantees. Finding numbers refer to the [November 2026 review](Item_Lifecycle_Review_20261117.md). The [lifecycle guide](Item_Lifecycle.md) describes the implemented behaviour. The review preserves the original evidence.

| Delivery | Depends on | Findings covered |
| --- | --- | --- |
| 1. Protect revision updates and copies | Can start immediately as two small fixes | 1, 7, partial copy publication |
| 2. Protect item identity and custody | Can start alongside 1 | 6, constructor cycles, competing ownership, stale destructive traversal |
| 3. Make activation and suspension repeatable | 2 for safe publication and removal | 4, revision-added components from 1, composite locks, lodged items, failed finalisation, previews |
| 4. Restore effects and clocks | 3 | 5, zero-duration expiry, periodic wound ticks, decay continuity |
| 5. Repair electrical and connector lifecycles | 2–3 for complete integration | 2, 3, waiting consumers, stale grid members, socket composition, port replay, relogin |
| 6. Complete replacement, escrow and deletion | 2–3, plus timer and power fixtures for acceptance | Morph/death branches, auction-only roots, partial teardown and retry |
| 7. Qualify a cold restart | 1–6 | Cross-system acceptance and historical data compatibility |

## 1. Stop destructive writes and copies

- **Revision updates.** Change only the intended revision and quality fields on the stored item. Exercise the actual maintenance path with a revised prototype, including its offline-item option. Seed every unrelated scalar with a distinctive value and give the item components, custody, effects and morph progress. Assert the MySQL update touches the intended columns and a fresh context sees all unrelated state intact. Preserve the existing intended quality-update behaviour. Handle activation of newly added components in delivery 3.
- **Severed body parts.** Keep the existing copy contract for permanent copies and previews, including remains nested inside a larger copy. Reconstruct the severed part's owned children, implants, wounds and lodged items through its component with independent IDs and valid reverse references. The component still restricts public `Put`. Delete source and copy in both orders and verify surviving child IDs, custody and wound ownership. A preview must leave no new stored rows, registry entries or listeners.
- **Historical damage.** The write fix protects subsequent updates. For an affected world, compare suspect rows with an independent backup or audit record before proposing restoration. A null owner or zero condition alone is insufficient evidence for reconstructing an earlier value.

## 2. Establish one authoritative instance and one owning parent

Make item unregistration conditional on the registry still containing that exact instance. Apply the same check to secondary indexes. Trace detached lookup callers and distinguish permitted offline maintenance from stale writes. A superseded object must be able to release its own callbacks without saving over or deleting the current object's row. Keep the change item-scoped unless all other users of the shared collection support the same contract.

Track item IDs under construction and defer owned-child binding where necessary. Distinguish an ownership cycle from a valid reciprocal connector. Reconcile parent XML with child custody and other roots before adopting a child. Admit an unambiguous legacy claim, report conflicting evidence with both IDs and preserve the child until its owner can be established. Recheck ownership before cascading deletion.

**Exit test.** Load the same fixtures in both discovery orders. Include a detached twin, a self-reference, a two-container cycle, a legal pair of connected peers, a null legacy parent, a clip and bag claiming the same child, and a child already in a body or room. Assert lookup by reference, one owner, bounded loading and preservation of foreign rows through a quit/delete attempt.

## 3. Give the item responsibility for each active interval

Keep structural finalisation separate from activation. Distinguish work in progress, completed work and failure. Components that acquire external resources need a way to unwind before retry. Make repeated login and quit safe at the item boundary, including reentry from component callbacks. Preserve the world sweep for independently discovered items and the owning-component traversal needed for inventory and late roots.

Traverse snapshots, recheck current membership and retirement after callbacks, and stop remaining sibling work if the parent leaves play. Forward whole-item lifecycle calls for owned lock items and wound-lodged children. Reconcile added and removed components on a revised **active** item without replaying unaffected siblings. An offline revision or temporary preview must stay dormant.

**Exit test.** A nested child, mounted module and independently discovered peer each activate once. A child can detonate or move itself during login, and a sibling can retire the parent. Repeated calls leave counts stable. A real quit followed by login starts one new active interval. Inject a failure in the second component's finaliser and verify recovery without duplicating the first component's registrations.

## 4. Preserve each timer's offline policy

Restore a cached schedule before delivering the effect's single login callback so an absolute-deadline effect can reassert its own deadline. Preserve unscheduled effects. Give new persisted data an explicit distinction between an unscheduled effect and a scheduled effect already due. Treat historical zero values according to the established legacy semantics unless the effect has independent evidence of expiry. Restart periodic item health ticks on same-instance relogin only for strategies and wounds that need them.

**Exit test.** Use a controlled clock for an ordinary timed effect, a self-removing effect, an overdue deadline, a lit fuse, a paused refrigerated morph, corpse decay, perishable food/liquid and periodic wounds. Compare the same interval spent active, logged out and across restart. Check callback count, remaining time or effective age and the next tick after resumption. A single blanket downtime rule would change gameplay for several of these families.

## 5. Repair power from the producer to the endpoint

The generator fix can ship early. Restore switch and fuel as passive state, start the heartbeat on activation, checkpoint partial consumption and release all live work on quit or delete. Checkpoint again at a durable suspension boundary while preserving the configured switch position.

For grid restoration, record exact endpoint identity and role. Read the legacy grid writer's component IDs in their own namespace and reconcile them with saved component affiliations before supplying power. Preserve unresolved late endpoints until their owning load phase. Treat repeated evidence from the two sides as one membership. For physical connectors, restore the saved port where identifiable and report ambiguous legacy ports. Validate missing or competing primary socket producers both at authoring time and on old data.

Track the producer a consumer registered with independently of whether power was delivered. End that registration on disconnect, switch-off, quit or delete, including while waiting for capacity. Restore parent and host subscriptions on same-instance login. Check upstream draw and downstream consumers on batteries, power banks, outlets, feeders and mounted machines.

**Exit test.** Repeat the review's 100 W producer/13 W ID collision and 10-unit generator fixtures. Check exact endpoints, supply and fuel after save and restart. Cut power, quit while waiting, restore supply and assert zero callbacks to the old consumer. Relog the same machine, let the retry window expire, then reconnect its parent. Repeat with reversed discovery order, two compatible ports, one-sided saved data and direct deletion.

## 6. Finish the paths that enter or leave play through another subsystem

- **Morph and damage death.** Give every successful replacement branch one registration, one committed destination and one activation. Preserve the branch's intended event order. Cover floor, held, contained and component-handled moves, refusal, callback redirection and no replacement. Assert identity through both world lookup and the destination, and verify the old item and its owned resources reach their intended outcome.
- **Auction escrow.** Suspend active-time behaviour while listed, retain absolute deadlines, structurally finalise an auction-only asset after materialisation, then place and activate it on claim or cancellation. This is the approved auction policy. Test listing and claiming with and without an intervening restart. Preserve the existing distinct suspension policy for reserved craft inputs.
- **Deletion failure.** Identify the database commit boundary for ordinary deletion and the existing specialised currency/spell paths. Use a transaction for related durable changes where feasible and make remaining cleanup resumable. Inject a callback failure after child cleanup and a provider refusal before parent deletion. Assert that surviving children keep valid custody, retries do not replay destructive observers, and memory and storage converge after restart.

## 7. Acceptance and closeout

Promote the seven original probes to deterministic regressions as their fixes land. For each source-traced issue, record a failing fixture before repair or evidence identifying the guard that already prevents it. Keep quick boundary tests in the default suite. Use an opt-in disposable MySQL world for provider writes, interrupted deletion and two consecutive fresh-process boots.

The acceptance world should contain a revised item, nested powered device, self-removing timed child, colliding grid/item IDs, composite lock, lodged item, auction-only asset and populated remains. Capture durable IDs and expected relationships before shutdown. After each boot and a player logout/login, compare stored rows, exact registry objects, custody at both ends, delivered wattage, fuel, remaining time and callback counts. Use Telnet to operate the device and retrieve the auction asset. Check representative boot duration and first-save volume for repeated reconciliation work.

Close each review finding with its fix or evidenced disposition, test name and result. Update the lifecycle and component-authoring guides with the delivered contracts and record any remaining unsupported historical-data cases.

## Implementation and verification ledger

| Finding | Implemented disposition and evidence |
| --- | --- |
| 1. Revision update overwrites durable fields | The update writes the tracked row, and `comp update all` uses a scoped lookup for offline targets. `CheckPrototypeForUpdate_ExistingRow_ChangesOnlyRevisionAndQuality` checks the EF modified fields and fresh-context values. The MySQL maintenance command advanced the coffer from revision 0 to 1 and preserved hashes of 16 other fields on both rows, including the nested custody and morph remainder. |
| 2. Grid endpoint ID collision | Legacy endpoint IDs resolve in the component namespace. Grid entries preserve exact IDs and roles, and activation reconciles endpoint affiliations. `LegacyProducerId_UsesComponentNamespaceEvenWhenAnItemIdCollides` verifies 100 W from the intended producer with a 13 W collision in the item namespace. |
| 3. Fuel generator runs while dormant | Loading restores the switch and fuel as passive state. Login starts consumption, quit or deletion detaches the heartbeat, and each consumption tick dirties the durable component. The 10-unit generator fixture checks activation, repeated login, suspension and resumption. |
| 4. Duplicate or mutating login traversal | The parent tracks activation and walks snapshots with membership and retirement checks. The fixtures exercise nested escrow activation, an effect that deletes its parent, and activation/finalisation failure followed by retry. |
| 5. Timed effect login runs twice | Item activation restores cached schedules before a single effect login dispatch. `Escrow_SuspendsOwnedItemsAndResumesEachEffectOnce` covers suspension and repeated activation attempts. The new schedule flag preserves a scheduled zero remainder. |
| 6. A detached twin unregisters a live item | Unregistration checks object identity, and stale item guards block durable writes and destructive traversal. `RemoveExact_SupersededInstance_DoesNotUnregisterCurrentInstance` covers the registry boundary. |
| 7. Severed-bodypart copy deletes original children | The initial refusal has been replaced by a component-owned copy. Both preview and permanent copies get independent contents, implants, tattoos, wounds and lodged items. `DeepCopy_NestedSeveredPart_OwnsIndependentItemsAndWounds` exercises deletion in both orders. `DeepCopy_SeveredPart_PersistsAndReloadsOwnedItemsAndFracture` exercises the save queue, EF round-trip and canonical wound ownership. |

The Release core suite passed all 9,367 tests, and the Debug server built successfully. `PersistedBatteryConnection_LatePeer_RestoresBothExactPorts` exercises battery-to-generic connector replay in both endpoint discovery orders. `PowerBank_PreservesStoredChargeAndResumesWaitingOutputAcrossLogout` checks waiting demand, charge, one heartbeat and the connection event subscription through activation, quit, relogin and deletion.

The first tranche's disposable MySQL and Telnet scenario passed 28 assertions across one initial startup and two subsequent fresh processes. The auction-only coffer retained its nested child and 305 morph seconds across both restarts. After cancellation it returned to hand, retained the child through a drop and pickup, and saved 266 morph seconds. A repeat passed 27 assertions. The lot retained 242 morph seconds across the process restart and saved 207 seconds after cancellation, drop and pickup, and character logout and login. The coffer and child remained in the same custody on return. Both cancellation and completed-sale claim place recovered items before calling `Login`. The smoke driver required corrections to its login timeout, Telnet and SQL assertions, and the timing of the post-cancellation save. The passing runs include graceful server shutdown and stop the disposable MySQL instance.

### Second tranche

| Work | Evidence and disposition |
| --- | --- |
| Native container deletion | Ordinary native containment with supported leaf components commits child and parent rows together before runtime teardown. The disposable MySQL and Telnet scenario passed 29 assertions across four boots. An ID-scoped trigger refused the parent delete after the transactional child delete. Both rows and custody survived. The trigger was removed while the MUD was stopped. A fresh boot recovered the item and a retry removed both rows. Another boot confirmed their absence. The provider refusal reached the fatal-error handler. |
| Legacy custody | Ordinary containers, clips, piles and battery assemblies arbitrate backed-up forward XML against reverse custody during boot. Competing and cyclic claims are retained in component XML and left inactive as relationships. Regressions reverse discovery order, check authoritative room and container records, and exercise cycle serialization using a native container. |
| Persisted port replay | The shared replay path prefers reciprocal evidence across candidate components. Without it, replay needs one distinct compatible remote descriptor. The tranche extends replay and pending serialization to the standard connector XML used by electrical grid endpoints, telephone and telecom endpoints, network switches, liquid and gas components, medical components and compressors. Attachable connectors retain exact peer component identity. Named automation bays retain their separate model. The feeder fixture restores an exact powered port and measures 100 W at login, 0 W after quit and 100 W after relogin. |
| Claim and restart coverage | The winning-bidder command test checks release, ownership, custody and activation in order. The live cash-buyout scenario passed 31 assertions. An NPC seller listed the lot and could not claim it after the player paid. The unclaimed coffer held its child and 581 morph seconds across two fresh processes. The winning player claimed it once, logged out and back in, and retained the child and durable ownership with 574 morph seconds remaining. This lot remained in escrow during the boot measurement. MySQL's statement digest count for writes to the item and item component tables stayed at one across the measured restarts before the claim. Engine boot durations were 26.5–29.2 seconds on this fixture and 26.1–30.2 seconds on the deletion fixture. |

The Release core suite passed all 9,377 tests. The Debug server built successfully. The optional MySQL scenarios use the [disposable live operator](../../scripts/EmploymentRestartSmoke/README.md).

### Third tranche

| Work | Implemented disposition |
| --- | --- |
| Complex native deletion | A durable checkpoint brackets callbacks, wound and component teardown, runtime custody and effect cleanup, and the teardown save flush. Startup completes the final row transaction from a completed checkpoint before loading world items. An interrupted callback leaves an item ID and a reconciliation hold at startup. The ordinary atomic transaction and spell-owned retirement still follow their dedicated contracts. |
| Specialised custody | Boot checks forward claims from severed contents and implants, firearms and their attachments, sheaths, belts, tables, locking and implant containers, both kinds of refrigerators, dryers, folded pockets, automation housings, battery chargers and generator locks. Held claims retain their original XML, including attachment slot attributes. Destructive traversal checks exact current custody before cascading. |
| Physical connector identity | Current prototype XML stores per-port IDs and current item links save both endpoint port IDs. Replay and free-port allocation respect identity through prototype port reordering. A descriptor-only record remains pending when its physical port is ambiguous. |
| Unlimited generator | Loading and copying preserve the saved switch without subscribing to a heartbeat. Login starts output for one active interval, quit withdraws it, and deletion drops registrations. |

See the [item deletion lifecycle](Item_Lifecycle.md#leaving-play-replacement-and-deletion) for checkpoint recovery and the [relationship model](Item_Lifecycle.md#relationships-and-power) for custody and connector evidence.

The disposable MySQL and Telnet test passed 15 assertions over three fresh MUD processes. The provider refused deletion of the sealable envelope after its nested paper sheet was gone. MySQL retained the envelope at stage `2`. After the trigger was removed, startup deleted the envelope automatically and verified both rows were absent. Another startup verified that both remained absent. The two recovery processes exited cleanly. Startup took 27.7–28.7 seconds on this fixture, including 0.26–0.51 seconds in the deletion-recovery phase.

The live severed-copy test passed 22 assertions over three fresh MUD processes. The staff `instance spawn ... cloneinventory` command created a persistent secondary body. The primary and secondary bodies held distinct copies of the severed part throughout both restarts. Each item retained its own holdable and bodypart components and its link to the original character and body. The automated copy regression also reloads independent contents, implants, a fracture and a lodged item from a fresh database context.

The powered refrigerator test passed 16 assertions over three fresh MUD processes on the final Debug build. The generator came back switched on, the refrigerator stayed closed, and its apple held 574 morph seconds through both restarts. The refrigerator's component XML and the child rows agreed on the apple and mounted lock. The cumulative MySQL statement count for writes to item and item-component tables increased from three to four across the first restart and first explicit save. Startup took 27.1–27.9 seconds on this fixture.

A 12-assertion follow-up opened the powered refrigerator and verified that the apple's morph time resumed, falling from 574 to 554 seconds during repeated saves. A FutureProg added a timed description to the apple. MySQL retained the effect's absolute deadline, it reloaded with about 31 seconds left, and it expired in play and cleared from `EffectData`. The apple and mounted lock kept their custody throughout. Both processes shut down cleanly.

The final Release core run passed 9,404 tests against stable source, and all 14 blank-snapshot tests passed. EF reported model parity and the Debug server built successfully. The disposable live world applied the migration on startup.

### Fourth tranche

| Work | Implementation |
| --- | --- |
| Specialised custody | The boot index, loaders and writers now cooperate for washers, cash registers, locking cash registers, fax machines, photocopiers and shop stalls. An unresolved ownership claim stays in its original element. Teardown keeps membership until the exact child's deletion has been handled. |
| Native wounds and lodging | The atomic graph includes supported item-owned wounds, detached lodged items and locking containers. The transaction validates both sides of the custody edges and rejects extra persisted children, infections or competing lodging. |
| Parallel connectors | Standard paired connections store the selected local and reciprocal physical IDs on the live link. Item callbacks run after both ends have registered. The connection state belongs to the link, so it survives independent instances that share a prototype. |
| Periodic scent persistence | Lighting the test censer exposed an XML serialization failure during room save. The burner now requests an uncoloured source description, and ambient scents strip terminal codes from their persisted text fields while saving the display colour separately. |

The focused Release regression run passed 115 tests across item lifecycle, specialised custody, connectors, remains compatibility, and the seal and measurement tests that cover ambient scents. The cases include a disputed lodged item, a refused lodged-child deletion, a cash register with no room, competing legacy claims, link replacement from inside a connection observer and serializing a scent with coloured source text. The full Release runs passed 9,427 core tests and 882 shared-library tests. The Debug game server built successfully after the ambient scent fix.

The wound-lodging fixture used a stopped disposable MySQL world to seed one canonical wound and its lodged item on the original severed part. A same-socket character logout and login preserved that graph. The normal `instance spawn ... cloneinventory` command then produced a second part, wound and lodged item with independent IDs. The completed two-process acceptance passed 17 assertions, including both body inventories, both wound owner/lodged IDs and both component wound references after cold boots. First-loop startup took 33.286 and 33.215 seconds. The cumulative item and item-component write digest was seven after each first explicit save. Both MUD processes shut down cleanly.

The ID-scoped MySQL refusal on the atomic wound path fired during the parent delete. After failure, the parent, lodged child and exact wound all still existed, along with the parent's only room membership. The child retained its wound custody without gaining a room or body claim. With the trigger removed, the normal purge committed all three removals. A subsequent boot confirmed that all three rows were still absent.

The periodic censer acceptance passed 11 reported assertions on the rebuilt Debug server. After lighting the censer and flushing saves, MySQL showed it lit with its exact fuel item and 133 seconds remaining. The room's saved `AmbientScent` referenced the censer and contained no terminal escape characters. The room rendered the scent after a character logout and login on the same server process and after two cold boots. The burn countdown resumed after the first cold boot, dropping from 27 to 21 seconds, and the second cold boot retained the lit state and exact fuel with nine seconds remaining. All three MUD processes shut down cleanly and the disposable MySQL instance stopped.

### Replacement acceptance

The initial regression run reproduced six placement failures across timed morph and damage death. A belt-attached item could morph into a result on the belt and floor simultaneously. A replacement for a loose chair skipped floor placement. The held-item handler could leave an unlisted replacement with a holder pointer after a rejected swap, or take it from custody committed by an earlier callback. Native wearable, belt and door installation handlers now check for existing custody before moving the replacement.

The 26 replacement regressions cover the morph scheduler and damage-death entry point. They assert exact room, body, container or belt membership and successful registry lookup before login, followed by one finished-loading event. The matrix includes a refused hand swap, redirected held, worn and belt-attached items, a loose chair and an uninstalled door, and deletion with no replacement. Separate cases confirm a reentrant death observer cannot publish twice, an observer can retire the source before construction, and a replacement that quits during login receives no finished-loading event. The focused Release run passed 109 tests, and the full Release core fast suite passed 9,453.

The disposable MySQL and Telnet case loaded four items through the ordinary item builder command and used `attach`, `put`, `get` and `drop` to place them. The 60-second morph retired all four originals. Its replacements had the same IDs and exclusive custody after a cold process restart.

| Source → replacement | Verified custody after restart |
| --- | --- |
| `#37` → `#41` | Belt `#36` has the exact `<Connected>41</Connected>` entry. The child has no direct room, body or container claim. |
| `#38` → `#42` | Envelope `#26` and the child's `ContainerId` agree. `look in envelope` shows the result. |
| `#39` → `#43` | Body `#1` retains the same held item in the right hand. |
| `#40` → `#44` | Room `#1` retains the same floor item. |

The same live run also destroyed loose chair `#45` through a FutureProg calling the native `wound` function. The test avatar lacked the default body part for `smash`. Chair replacement `#46` was in room `#1` after the damage and after the second cold restart. That restart rechecked each of the four morph results by exact ID, including the belt and envelope component records, and confirmed that all five originals were absent. The run passed 16 reported assertions. All three processes ran the same Debug build, stopped gracefully, and left the disposable MySQL instance stopped.

### Source-traced review closeout

| Review finding | Current evidence |
| --- | --- |
| Recursive construction and conflicting custody | The item-loading guard bounds recursive lookup. `LegacyCycle_IsHeldAndSerializedInEitherDiscoveryOrder`, the specialised owner discovery cases, and the cash register round trip cover conservative recovery of competing or circular claims. |
| Replacement publication | `ItemReplacementLifecycleTests` exercises the morph scheduler and native damage-death entry point for floor, held, contained and component-redirected results. The shared completion path gives component placement priority, retires the source and registers the surviving replacement before login and the finished-loading event. `Morph_RemainsConservationHold_DoesNotConstructOrActivateReplacement` covers a held removal. |
| Waiting power and same-instance reconnection | The machine tracks the producer that accepted its registration and unsubscribes from it even after power delivery stops. Login reattaches parent and host observers. The powered-machine retry test, the power-bank suspend/resume test and the feeder replay test cover the active interval and power restoration. |
| Socket composition and grid cleanup | Prototype and runtime validation check the primary producer. Grid endpoints join on activation and quit removes live registration while preserving the affiliation for relogin. `GridPowerSupply_RetainsConfigurationWhileSuspendedAndRegistersOncePerActiveInterval` and the feeder replay test verify representative paths. |
| Connector replay | Exact-ID, legacy ambiguity and late-peer regressions exercise replay. The parallel-link regressions cover shared prototypes, generic XML, reordered ports and queued notifications after disconnect or reconnect. |
| Child lifecycle and late auction items | Whole-item traversal reaches owned locks and lodged children through the native lifecycle. `Escrow_SuspendsOwnedItemsAndResumesEachEffectOnce` and the live auction, refrigerator and copy receipts cover registration, placement, and timing. |
| Temporary copies and incomplete teardown | The severed-part and generator fixtures cover independently owned copy contents and passive restored switches. The deletion ledger distinguishes atomic graph recovery, completed checkpoints and uncertain callbacks. |
| Finalisation and timer boundaries | `FinaliseLoad_AfterComponentFailure_RetrySkipsCompletedHooks`, the timed-effect regressions and the refrigerator receipts cover separate finalisation/activation and their clock policies. |

### Remaining exit criteria

- Checkpointed deletion can finish an acknowledged teardown after restart. Automatic continuation from the middle of an arbitrary callback would require that callback to commit related changes atomically or to recognise its own durable operation key. The current recovery stops for reconciliation when the callback outcome is unknown.
- An ambiguous historical physical port requires independent evidence before it can be rebound. Components with bespoke one-link attachment state retain their own persistence contracts.
