# Physical manipulation and bodypart audit

Audit date: 2026-09-27. Baseline: `666efc94a2065fb94ef75a1ffe6dd42359940c6c`. This records the working-tree changes made for the physical command, runtime manipulation, room-access and lawful-transfer audit.

## Behaviour and coverage

Ordinary manual actions now require at least one present, functioning grab/manipulator location. They use `CanUseBodypart`, including limb damage, pain, severing, grappling, restraints, missing bones, spinal damage and nonfunctional prosthetics. This does not require an empty hand unless the particular operation already requires one. Tentacles and other configured manipulators qualify.

The shared runtime check sits below command parsing. Actorful component feasibility, diagnostics and execution validate the acting body and relevant item endpoints; AI and direct component calls use the same gates. Delayed treatment, crafting entry, butchery, styling, writing callbacks, traps and component configuration revalidate where listed in the ledger. A successful start does not grant indefinite physical access.

Item reach walks containment and automation mounts to the actual world/inventory root. It checks planes, closed containers, mount housing access, current cell/layer, installed doors from either side, guarding/access restrictions and ordinary inventory consent. External locks on a closed door remain reachable; they are not treated as contents inside the door. Covert inventory transfer retains its existing intentional consent bypass.

Amount-based currency, quantity and weight pickup validate the original source, including pickup reservations and source `CanTake`, before splitting or removing anything. Actorful container emptying preflights all original contents before mutation. Real stock splits retain exact shop/merchandise identity for existing theft hooks, and stock split/merge notifications distinguish a change in pile count from acquiring or losing goods. No new crime category or general ownership policy is introduced.

| Audited pathway | Enforcement/change |
| --- | --- |
| Core anatomy, grab/wield, inventory, dressing and restraints | Shared bodypart usability, runtime inventory gates and explicit external outfit placement. |
| Doors, locks, containers, connectors, switches and selectable devices | Component Can/Why/execute guards; both connector ends and relevant tools; all-content empty preflight. |
| Ranged weapons, ammunition handling, explosives, artillery and attachments | Operator guards before load/ready/fire/arming actions; breath and command-role exceptions below. |
| Electronics, telecom, computers, networks, media, power, gas and liquid machinery | Actor-driven controls and physical connection/container operations; automatic signal/lifecycle primitives retain their system semantics. |
| Writing, books, inscriptions, seals, measurement, furniture, repairs and devices | Shared runtime guards; delayed editor/configuration rechecks; actorful measurement interface. |
| Medicine, surgery, butchery, skinning, foraging and styling | Actor checks at relevant start/runtime and staged callbacks; administered recipients do not need hands. |
| Mounting, vehicle control/service/repair/installation/hitching | Movement eligibility for mounting and runtime manual/access gates for manual vehicle work. |
| Lawful transfers and shops | Original-source permission, reservation and reach checks; split metadata and stock accounting. |

## Deliberate exemptions and unchanged mechanisms

These are explicit decisions, not unimplemented audit findings. Where a function is also in the change ledger, only the indicated generic hand requirement is exempt; reach or its own capability check may have changed.

| Functions/pathways | Reason and retained protection |
| --- | --- |
| `Body.CanStand`, character movement/climbing/flying capability methods, natural-attack selection | Existing posture, support/limb, wing, movement and attack-bodypart requirements describe the action. A universal hand requirement would block anatomically valid actors. They benefit from the corrected shared bodypart usability where called. |
| `Body.CanWear` overloads; new `WearExternally`, `CanSheatheExternally`, `SheatheExternally`; arena, body-instance and template-outfit placement | Fit is a property of the recipient and garment. An externally dressed or newly created body need not be able to dress itself. Normal actor `Wear`, `Sheathe` and dresser paths remain guarded. |
| `Body.CanEat`, `CanDrink`, `CanSwallow`, their runtime actions, `EatSilent`, `DrinkSilent`, `Swallow` administration paths | Eating/drinking can use a mouth directly; administered effects are not recipient hand actions. Reach was added to voluntary source checks; existing mouth, breathing, diet and swallowing rules remain. The person feeding/injecting/applying has the manual requirement. |
| `InstrumentGameItemComponent.CanPlay` / `WhyCannotPlay` / `CanContinue`, `SignalInstrumentGameItemComponent.WhyCannotUse` | The prototype's `RequiredHands` is authoritative; zero-hand instruments are legitimate. Reach and configured usable-hand counts are checked. |
| `BiometricScannerGameItemComponent.CanScan` self-scan branch | The configured exposed, usable bodypart supplies the biometric sample. Presenting a severed specimen remains manual. |
| `BlowgunGameItemComponent.CanReady`, `Ready`, `CanFire`, `Fire` and their Why methods | Breath operation uses a functioning, uncovered mouth and breathing; it need not use a grab location. Authored free-hand ready requirements remain. Loading/unloading remain manual. |
| `ArtilleryPieceGameItemComponent.CanPerform` command/leadership role | Giving the crew an order is not physical gun handling. Operating roles remain guarded. |
| `ItemManipulationGuard.CanManipulate` same-body internal `IImplantRespondToCommands` branch | Neural controls do not use hands. Every supplied endpoint must be a non-external implant installed in that same actor; an external tool/endpoint cannot inherit the exception. |
| `TelekineticManipulation.TryPrepare` and its execute callback | A power supplies its own item eligibility. The internal scoped exception is actor-specific, restored on disposal and revalidated at execution. Ordinary body inventory requirements remain. |
| `Craft.CanDoCraft`, `CanResumeCraft`, phase inventory plans; `BeginCraft` / `ResumeCraft` now revalidate them | Authored crafts may be non-manual. Configured tools, plans, progs, location and phase requirements determine capability; a universal hand count would override builder intent. |
| `AgricultureModule.StartOperation`, agriculture `ApplyOperation`, generic project labour and completion actions | Starting an operation queues a project; applying its result is the project-completion state mutation. Generic labour may be supervision or another non-manual contribution. These are not converted into universal hand-gated primitives. |
| `VehicleOperationalReadinessService.CanPerformAction` boarding; actorless automatic vehicle motion | Boarding has movement/access rules, while automatic controls operate without an actor. Manual control, service, repair and hitching are guarded. |
| `Body.CanDrop` / `Drop`, weapon unready/release, dismount and drag release | A disabled actor must be able to let go or stop an action. Existing state/safety and movement restrictions remain; these do not acquire a blanket hand gate. |
| Traction/dragging and animal-harness movement | Their movement, weight and traction rules support animals that pull without hands. Attaching vehicle hitch equipment is separately guarded. |
| Raw `GameItem.Get` / `Drop`, body `Take`, `IContainer.Put` / `Take`, raw connect/disconnect, liquid merge/remove primitives | These are also used for persistence, loading, rollback, destruction, production and already-validated transfers. They remain low-level mutations; actorful entry paths must do their checks first. |
| Projectile/ammunition `Fire` resolution, power-pack discharge, automatic detonator/signal callbacks, phone teardown, timers and machine lifecycle events | Once triggered or powered, these are system resolution rather than a new manual action. Actorful weapon/device entry methods are guarded; null actors retain established system semantics. |
| Builder/admin data editors, observation, social/mental actions and speech/communication strategies | Configuration, observation and thought do not imply hand manipulation. Existing language, breathing, vocal/anatomical and permission rules remain where applicable; an admin performing a real physical action still passes its runtime physical gate. |

## Verification

The final wrappers ran after rebasing the audit branch onto current `master` (`9255c19690c27ddc74401c8eedd726af94d6fc6f`). Both wrappers confirmed source stability for their selected project scopes. Only this verification receipt was completed afterward; no runtime or test code changed after those runs.

| Check | Result | Evidence |
| --- | --- | --- |
| Full `MudSharpCore Unit Tests` | **PASS: 3,803 passed, 0 failed, 0 skipped** | Run `20260926T224506Z-98484526fd06`; `.artifacts/test-runs/20260926T224506Z-98484526fd06/summary.json`. |
| Full `FutureMUDLibrary Unit Tests` | **PASS: 505 passed, 0 failed, 0 skipped** | Run `20260926T224754Z-0ee15d0847a4`; `.artifacts/test-runs/20260926T224754Z-0ee15d0847a4/summary.json`. |
| Focused manipulation/split/shop/installation run before the final quantity and merge additions | PASS: 54 passed, 0 failed, 0 skipped | `.artifacts/physical-audit-final-focus/physical-audit.trx`; the later full core run covers the final additions too. |
| Diff, ledger and paths | PASS | `git diff --check`; all 183 source links resolve; all 890 ledger declarations match the syntax-based changed-function inventory; no whitespace-only functions counted. |

Earlier pre-rebase verification exposed an unrelated title-case failure in unchanged library code; that earlier result is superseded by the passing full library suite above on current `master`. Earlier core verification also found fixture failures after physical/planar requirements became explicit; those fixtures now supply the required anatomy and location, and the complete final suite passes. Intermediate focused failures were repaired and are covered by the final suites. An initial wrapper attempt blocked on local Git/line-ending reporting and was not counted as a pass.

Commands used for the final runs:

```powershell
.\scripts\test-unit-core.ps1 -OutputMode Compact -TimeoutSeconds 1200
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj' -TimeoutSeconds 1200
```


No live MUD/Telnet, database persistence, AI scenario, or hosted CI run was performed for this audit. Unit tests exercise shared runtime entries and delayed checks, but do not prove every builder configuration or every one of the guarded component methods independently. This broad behavioural change should receive a representative in-game pass before release.

## Complete changed-function ledger

Each row names an exact changed/added C# method, constructor or interface method, including overloads. Nested callbacks/local functions are covered by their enclosing method. Whitespace-only changes are excluded. Paths link to the owning source file; the recorded line is the current declaration line. The new enum member `CanUseBodypartResult.CantUseLimbRestrained = 512` is a non-method change and is recorded separately here.

This inventory contains **825 runtime/interface declarations** and **65 test/helper declarations** in **183 C# files**. Existing functions that inherit enforcement through a changed callee are covered in the pathway/exemption tables rather than mislabelled as edited.

### [FutureMUDLibrary/Body/IInventory.cs](../../FutureMUDLibrary/Body/IInventory.cs)

| Function | Change |
| --- | --- |
| `IInventory.WearExternally(IGameItem item, IWearProfile? profile = null)` (line 263; added) | Declares the explicit system/external outfit path, separating target fit from the target performing a manual action. |
| `IInventory.CanSheatheExternally(IGameItem item, IGameItem sheath)` (line 286; added) | Declares the explicit system/external outfit path, separating target fit from the target performing a manual action. |
| `IInventory.SheatheExternally(IGameItem item, IGameItem sheath)` (line 289; added) | Declares the explicit system/external outfit path, separating target fit from the target performing a manual action. |
| `IInventory.WhyCannotGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 397; changed) | Corrects the diagnostic contract from bool to string, matching the implemented failure explanation. |
| `IInventory.WhyCannotGetByWeight(IGameItem item, IGameItem container, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 399; changed) | Corrects the diagnostic contract from bool to string, matching the implemented failure explanation. |

### [FutureMUDLibrary/Body/ManualActionExtensions.cs](../../FutureMUDLibrary/Body/ManualActionExtensions.cs)

| Function | Change |
| --- | --- |
| `ManualActionExtensions.CanPerformManualAction(this IBody body, out string reason)` (line 11; added) | Adds one shared working-manipulator check using HoldLocs and CanUseBodypart; occupancy alone is allowed and nonhuman grab locations qualify. |
| `ManualActionExtensions.CanPerformManualAction(this ICharacter actor, out string reason)` (line 23; added) | Adds one shared working-manipulator check using HoldLocs and CanUseBodypart; occupancy alone is allowed and nonhuman grab locations qualify. |

### [FutureMUDLibrary/Character/ItemReachExtensions.cs](../../FutureMUDLibrary/Character/ItemReachExtensions.cs)

| Function | Change |
| --- | --- |
| `ItemReachExtensions.CanReachItem(this ICharacter actor, IGameItem item, bool requireInventoryPermission = true)` (line 15; added) | Adds cycle-safe ancestor traversal, planar interaction, closed containment, mount-host access, room/layer or installed-door adjacency, room guard access and optional inventory-owner consent. |

### [FutureMUDLibrary/Economy/IShop.cs](../../FutureMUDLibrary/Economy/IShop.cs)

| Function | Change |
| --- | --- |
| `IShop.RegisterStockItemSplit(IGameItem source, IGameItem split)` (line 85; added) | Declares split/merge notifications so changing the number of existing display piles does not invent stock acquisitions or losses. |
| `IShop.RegisterStockItemMerge(IGameItem target, IGameItem absorbed)` (line 87; added) | Declares split/merge notifications so changing the number of existing display piles does not invent stock acquisitions or losses. |

### [FutureMUDLibrary/GameItems/Interfaces/IMeasuringInstrument.cs](../../FutureMUDLibrary/GameItems/Interfaces/IMeasuringInstrument.cs)

| Function | Change |
| --- | --- |
| `IMeasuringInstrument.CanMeasure(MudSharp.Character.ICharacter actor, IGameItem target, out string error)` (line 34; added) | Adds an actorful CanMeasure overload so runtime callers can validate capability and access as well as measurement compatibility. |

### [MudSharpCore/Arenas/Core/ArenaEvent.cs](../../MudSharpCore/Arenas/Core/ArenaEvent.cs)

| Function | Change |
| --- | --- |
| `ArenaEvent.TryWearItem(IBody body, IGameItem item, long? wearProfileId)` (line 2097; changed) | Routes engine-created outfit/body-clone placement through WearExternally so it does not require the equipped target to perform the dressing action. |

### [MudSharpCore/Arenas/Npc/ArenaNpcService.cs](../../MudSharpCore/Arenas/Npc/ArenaNpcService.cs)

| Function | Change |
| --- | --- |
| `ArenaNpcService.WearItem(IBody body, IGameItem item, long? wearProfileId)` (line 285; changed) | Routes engine-created outfit/body-clone placement through WearExternally so it does not require the equipped target to perform the dressing action. |

### [MudSharpCore/Body/Implementations/BodyBehaviours.cs](../../MudSharpCore/Body/Implementations/BodyBehaviours.cs)

| Function | Change |
| --- | --- |
| `Body.CanOpen(IOpenable openable)` (line 157; changed) | Applies the shared guard before open/close/could-open feasibility, preserving the existing lock, seal and limb-specific rules. |
| `Body.Open(IOpenable openable, ICharacter openableOwner, IEmote playerEmote, bool useCouldLogic = false)` (line 214; changed) | Revalidates the body open/close gate immediately before mutation; installed-door output also tolerates a portable door without an installed exit. |
| `Body.CouldOpen(IOpenable openable)` (line 297; changed) | Applies the shared guard before open/close/could-open feasibility, preserving the existing lock, seal and limb-specific rules. |
| `Body.CanClose(IOpenable openable)` (line 340; changed) | Applies the shared guard before open/close/could-open feasibility, preserving the existing lock, seal and limb-specific rules. |
| `Body.Close(IOpenable openable, ICharacter openableOwner, IEmote playerEmote)` (line 385; changed) | Revalidates the body open/close gate immediately before mutation; installed-door output also tolerates a portable door without an installed exit. |
| `Body.CanConnect(IConnectable connectable, IConnectable other)` (line 418; changed) | Applies the shared guard to both connector parents in feasibility and diagnostic paths used by runtime execution. |
| `Body.WhyCannotConnect(IConnectable connectable, IConnectable other)` (line 451; changed) | Applies the shared guard to both connector parents in feasibility and diagnostic paths used by runtime execution. |
| `Body.CanDisconnect(IConnectable connectable, IConnectable other)` (line 467; changed) | Applies the shared guard to both connector parents in feasibility and diagnostic paths used by runtime execution. |
| `Body.WhyCannotDisconnect(IConnectable connectable, IConnectable other)` (line 500; changed) | Applies the shared guard to both connector parents in feasibility and diagnostic paths used by runtime execution. |

### [MudSharpCore/Body/Implementations/BodyInventory.cs](../../MudSharpCore/Body/Implementations/BodyInventory.cs)

| Function | Change |
| --- | --- |
| `Body.CanWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None)` (line 305; changed) | Filters to present, functioning wield locations even when free-hand checks are ignored; specific-hand calls reject an unusable or absent location and diagnostics use the same candidates. |
| `Body.CanWield(IGameItem item, IWield? specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)` (line 343; changed) | Filters to present, functioning wield locations even when free-hand checks are ignored; specific-hand calls reject an unusable or absent location and diagnostics use the same candidates. |
| `Body.WhyCannotWield(IGameItem item, ItemCanWieldFlags flags = ItemCanWieldFlags.None)` (line 399; changed) | Filters to present, functioning wield locations even when free-hand checks are ignored; specific-hand calls reject an unusable or absent location and diagnostics use the same candidates. |
| `Body.WhyCannotWield(IGameItem item, IWield? specificHand, ItemCanWieldFlags flags = ItemCanWieldFlags.None)` (line 476; changed) | Filters to present, functioning wield locations even when free-hand checks are ignored; specific-hand calls reject an unusable or absent location and diagnostics use the same candidates. |
| `Body.CanSheathe(IGameItem item, IGameItem sheath)` (line 901; changed) | Requires the acting body to have a usable manipulator before ordinary wear/sheath execution; existing profile/sheath checks remain. |
| `Body.CanSheatheExternally(IGameItem item, IGameItem sheath)` (line 906; added) | Separates fit/placement from actor action so outfits and externally dressed bodies retain their existing placement rules without requiring the recipient to have hands. |
| `Body.WhyCannotSheathe(IGameItem item, IGameItem sheath)` (line 1013; changed) | Requires the acting body to have a usable manipulator before ordinary wear/sheath execution; existing profile/sheath checks remain. |
| `Body.Sheathe(IGameItem item, IGameItem sheath, IEmote? playerEmote = null, OutputFlags additionalFlags = OutputFlags.Normal, bool silent = false)` (line 1144; changed) | Requires the acting body to have a usable manipulator before ordinary wear/sheath execution; existing profile/sheath checks remain. |
| `Body.SheatheExternally(IGameItem item, IGameItem sheath)` (line 1156; added) | Separates fit/placement from actor action so outfits and externally dressed bodies retain their existing placement rules without requiring the recipient to have hands. |
| `Body.SheatheInternal(IGameItem item, IGameItem sheath, IEmote? playerEmote, OutputFlags additionalFlags, bool silent)` (line 1162; added) | Separates fit/placement from actor action so outfits and externally dressed bodies retain their existing placement rules without requiring the recipient to have hands. |
| `Body.CanGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 1308; changed) | Checks current reach, original-item pickup restrictions and a usable manipulator before stack-merge shortcuts; container overloads require real membership and CanTake unless the explicit internal ignore flag applies. |
| `Body.CanGet(IGameItem item, IGameItem container, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 1407; changed) | Checks current reach, original-item pickup restrictions and a usable manipulator before stack-merge shortcuts; container overloads require real membership and CanTake unless the explicit internal ignore flag applies. |
| `Body.WhyCannotGet(IGameItem item, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 1447; changed) | Checks current reach, original-item pickup restrictions and a usable manipulator before stack-merge shortcuts; container overloads require real membership and CanTake unless the explicit internal ignore flag applies. |
| `Body.WhyCannotGet(IGameItem item, IGameItem container, int quantity, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 1570; changed) | Checks current reach, original-item pickup restrictions and a usable manipulator before stack-merge shortcuts; container overloads require real membership and CanTake unless the explicit internal ignore flag applies. |
| `Body.CanPut(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity, bool allowLesserAmounts)` (line 1878; changed) | Checks destination reach and the acting manipulator while preserving covert transfer consent semantics; corpse clothing placement uses external wear after validation. |
| `Body.WhyCannotPut(IGameItem item, IGameItem container, ICharacter? containerOwner, int quantity, bool allowLesserAmounts)` (line 1913; changed) | Checks destination reach and the acting manipulator while preserving covert transfer consent semantics; corpse clothing placement uses external wear after validation. |
| `Body.CanPut(IGameItem item, IGameItem container, string profile)` (line 2094; changed) | Checks destination reach and the acting manipulator while preserving covert transfer consent semantics; corpse clothing placement uses external wear after validation. |
| `Body.WhyCannotPut(IGameItem item, IGameItem container, string profile)` (line 2140; changed) | Checks destination reach and the acting manipulator while preserving covert transfer consent semantics; corpse clothing placement uses external wear after validation. |
| `Body.Put(IGameItem item, IGameItem container, string profile, IEmote? playerEmote = null, bool silent = false)` (line 2219; changed) | Checks destination reach and the acting manipulator while preserving covert transfer consent semantics; corpse clothing placement uses external wear after validation. |
| `Body.CanGive(IGameItem item, IBody target, int quantity = 0)` (line 2341; changed) | Requires a functioning giver and current target access/colocation, then retains the recipient pickup and item-drop checks. |
| `Body.WhyCannotGive(IGameItem item, IBody target, int quantity = 0)` (line 2356; changed) | Requires a functioning giver and current target access/colocation, then retains the recipient pickup and item-drop checks. |
| `Body.CanGive(IGameItem item, ICorpse target, int quantity = 0)` (line 2445; changed) | Requires a functioning giver and current target access/colocation, then retains the recipient pickup and item-drop checks. |
| `Body.WhyCannotGive(IGameItem item, ICorpse target, int quantity = 0)` (line 2462; changed) | Requires a functioning giver and current target access/colocation, then retains the recipient pickup and item-drop checks. |
| `Body.Swap(IGameItem firstItem, IGameItem secondItem)` (line 2629; changed) | Requires a working manipulator, chooses functioning free destinations, and validates both destinations before removing either held/wielded item. |
| `Body.RemoveItem(IGameItem item, IEmote playerEmote, ICharacter remover)` (line 2864; changed) | Checks the remover and reach to the original worn item while preserving existing dressing consent; execution rechecks before removal. |
| `Body.Restrain(IGameItem item, IWearProfile profile, ICharacter restrainer, IGameItem targetItem, IEmote? emote = null, bool silent = false)` (line 3086; changed) | Rejects an incapable or non-colocated restrainer before changing restraints or inventory. |
| `Body.WearExternally(IGameItem item, IWearProfile? profile = null)` (line 3140; added) | Separates fit/placement from actor action so outfits and externally dressed bodies retain their existing placement rules without requiring the recipient to have hands. |
| `Body.Wear(IGameItem item, IWearProfile profile, IEmote? playerEmote = null, bool silent = false)` (line 3145; changed) | Requires the acting body to have a usable manipulator before ordinary wear/sheath execution; existing profile/sheath checks remain. |
| `Body.WearInternal(IGameItem item, IWearProfile profile, IEmote? playerEmote, bool silent)` (line 3156; added) | Separates fit/placement from actor action so outfits and externally dressed bodies retain their existing placement rules without requiring the recipient to have hands. |
| `Body.CanBeRemoved(IGameItem item, ICharacter remover)` (line 3224; changed) | Checks the remover and reach to the original worn item while preserving existing dressing consent; execution rechecks before removal. |
| `Body.CanDress(IGameItem item, ICharacter dresser, IWearProfile profile = null)` (line 3264; changed) | Checks the dresser rather than the recipient, including colocation; successful dressing uses external wear after the existing consent and fit checks. |
| `Body.WhyCannotDress(IGameItem item, ICharacter dresser, IWearProfile profile = null)` (line 3297; changed) | Checks the dresser rather than the recipient, including colocation; successful dressing uses external wear after the existing consent and fit checks. |
| `Body.Dress(IGameItem item, ICharacter dresser, IWearProfile profile = null, IEmote? emote = null)` (line 3360; changed) | Checks the dresser rather than the recipient, including colocation; successful dressing uses external wear after the existing consent and fit checks. |
| `Body.WhyCannotBeRemoved(IGameItem item, ICharacter remover)` (line 3739; changed) | Checks the remover and reach to the original worn item while preserving existing dressing consent; execution rechecks before removal. |
| `Body.AccessibleRoomCurrencyPiles()` (line 3947; added) | Selects original currency piles through the normal body CanGet path (including source CanTake and no-get effects), ignoring only whole-pile weight; used consistently for feasibility, diagnostics and execution. |
| `Body.AccessibleContainerCurrencyPiles(IGameItem container)` (line 3954; added) | Selects original currency piles through the normal body CanGet path (including source CanTake and no-get effects), ignoring only whole-pile weight; used consistently for feasibility, diagnostics and execution. |
| `Body.CanGet(ICurrency currency, decimal amount, bool exact)` (line 3962; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.CanGet(ICurrency currency, IGameItem container, decimal amount, bool exact)` (line 3981; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.WhyCannotGet(ICurrency currency, decimal amount, bool exact)` (line 4007; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.WhyCannotGet(ICurrency currency, IGameItem container, decimal amount, bool exact)` (line 4043; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.Get(ICurrency currency, IGameItem containerItem, decimal amount, bool exact, IEmote? playerEmote, bool silent, IEnumerable<IHandleEvents> witnessHandlers)` (line 4086; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.Get(ICurrency currency, decimal amount, bool exact, IEmote? playerEmote, bool silent, IEnumerable<IHandleEvents> witnessHandlers)` (line 4120; changed) | Uses the same accessible original-pile selection in Can/Why/Get; retains ownership grouping, exact-amount checks and normal body transfer, with source-specific failure reasons. |
| `Body.CanGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4403; changed) | Implements finite positive weight validation, original-source CanGet/CanTake before preview creation, and capacity checks on the requested temporary portion. |
| `Body.CanGetByWeight(IGameItem item, IGameItem container, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4410; changed) | Implements finite positive weight validation, original-source CanGet/CanTake before preview creation, and capacity checks on the requested temporary portion. |
| `Body.WhyCannotGetByWeight(IGameItem item, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4419; changed) | Implements string diagnostics for invalid weights or original-source pickup denial, followed by temporary split capacity checks. |
| `Body.WhyCannotGetByWeight(IGameItem item, IGameItem container, double weight, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4428; changed) | Implements string diagnostics for invalid weights or original-source pickup denial, followed by temporary split capacity checks. |
| `Body.GetByWeight(IGameItem item, double weight, IEmote? playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4438; changed) | Implements guarded weight pickup: validates the source before any split/debit and delegates final transfer to Body.Get; only a validated detached container split ignores original membership. |
| `Body.GetByWeight(IGameItem item, IGameItem container, double weight, IEmote? playerEmote = null, bool silent = false, ItemCanGetIgnore ignoreFlags = ItemCanGetIgnore.None)` (line 4451; changed) | Implements guarded weight pickup: validates the source before any split/debit and delegates final transfer to Body.Get; only a validated detached container split ignores original membership. |

### [MudSharpCore/Body/Implementations/BodyNeeds.cs](../../MudSharpCore/Body/Implementations/BodyNeeds.cs)

| Function | Change |
| --- | --- |
| `Body.CanEat(IEdible edible, IContainer? container, ITable? table, double bites)` (line 433; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.WhyCannotEat(IEdible edible, IContainer container, ITable table, double bites)` (line 474; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.CanEat(ICorpse corpse, double bites)` (line 518; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.CanEat(ISeveredBodypart bodypart, double bites)` (line 541; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.CanDrink(ILiquidContainer container, ITable table, double quantity)` (line 630; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.WhyCannotDrink(ILiquidContainer container, ITable table, double quantity)` (line 673; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.CanSwallow(ISwallowable swallowable, IContainer container, ITable table)` (line 814; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |
| `Body.WhyCannotSwallow(ISwallowable swallowable, IContainer container, ITable table)` (line 853; changed) | Adds physical/planar reach to the food, drink or swallowable source; retains mouth, breathing, diet and container rules without imposing hands on eating/drinking. |

### [MudSharpCore/Body/Implementations/BodyParts.cs](../../MudSharpCore/Body/Implementations/BodyParts.cs)

| Function | Change |
| --- | --- |
| `Body.CanUseBodypart(IBodypart part)` (line 377; changed) | Rejects parts absent from the current body and propagates limb restraints as CantUseLimbRestrained alongside existing damage, pain, severing, grapple, bone, spine and prosthetic failures. |

### [MudSharpCore/Body/PartProtos/GrabbingBodypartProto.cs](../../MudSharpCore/Body/PartProtos/GrabbingBodypartProto.cs)

| Function | Change |
| --- | --- |
| `GrabbingBodypartProto.CanGrab(IGameItem item, IInventory body)` (line 91; changed) | Maps unusable or restrained bodyparts to grab/wield failure; wield-only locations now consult bodypart usability too. |

### [MudSharpCore/Body/PartProtos/GrabbingWieldingBodypartProto.cs](../../MudSharpCore/Body/PartProtos/GrabbingWieldingBodypartProto.cs)

| Function | Change |
| --- | --- |
| `GrabbingWieldingBodypartProto.CanWield(IGameItem item, IInventory body)` (line 89; changed) | Maps unusable or restrained bodyparts to grab/wield failure; wield-only locations now consult bodypart usability too. |

### [MudSharpCore/Body/PartProtos/WieldingBodypartProto.cs](../../MudSharpCore/Body/PartProtos/WieldingBodypartProto.cs)

| Function | Change |
| --- | --- |
| `WieldingBodypartProto.CanWield(IGameItem item, IInventory body)` (line 88; changed) | Maps unusable or restrained bodyparts to grab/wield failure; wield-only locations now consult bodypart usability too. |

### [MudSharpCore/Character/Character.cs](../../MudSharpCore/Character/Character.cs)

| Function | Change |
| --- | --- |
| `Character.CanManipulateItem(IGameItem item)` (line 2946; changed) | Centralises manual capability and recursive item reach, then retains mounted manipulation restrictions. |
| `Character.WhyCannotStyle(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value)` (line 3632; changed) | Checks manual capability for styling; delayed callbacks recheck the full current styling conditions and remove their own effect on failure. |
| `Character.CanStyle(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value)` (line 3708; changed) | Checks manual capability for styling; delayed callbacks recheck the full current styling conditions and remove their own effect on failure. |
| `Character.Style(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value, bool force = false)` (line 3781; changed) | Checks manual capability for styling; delayed callbacks recheck the full current styling conditions and remove their own effect on failure. |

### [MudSharpCore/Character/CharacterInstanceService.cs](../../MudSharpCore/Character/CharacterInstanceService.cs)

| Function | Change |
| --- | --- |
| `CharacterInstanceService.CloneInventory( ICharacter source, ICharacterInstance target, out CharacterInstanceInventoryCloneResult result)` (line 998; changed) | Routes engine-created outfit/body-clone placement through WearExternally so it does not require the equipped target to perform the dressing action. |

### [MudSharpCore/Character/CharacterMountable.cs](../../MudSharpCore/Character/CharacterMountable.cs)

| Function | Change |
| --- | --- |
| `Character.CanBeMountedBy(ICharacter rider)` (line 45; changed) | Requires the rider to be colocated and currently able to move before mounting; diagnostic path mirrors feasibility. |
| `Character.WhyCannotBeMountedBy(ICharacter rider)` (line 86; changed) | Requires the rider to be colocated and currently able to move before mounting; diagnostic path mirrors feasibility. |

### [MudSharpCore/Commands/Modules/GameModule.cs](../../MudSharpCore/Commands/Modules/GameModule.cs)

| Function | Change |
| --- | --- |
| `GameModule.Forage(ICharacter actor, string input)` (line 1295; changed) | Requires a usable manipulator before gathering and rechecks at the delayed forage callback. |

### [MudSharpCore/Commands/Modules/HealthModule.cs](../../MudSharpCore/Commands/Modules/HealthModule.cs)

| Function | Change |
| --- | --- |
| `HealthModule.CPR(ICharacter actor, string command)` (line 34; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Relocate(ICharacter actor, string command)` (line 251; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Bind(ICharacter actor, string command)` (line 406; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.CleanWounds(ICharacter actor, string command)` (line 460; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Suture(ICharacter actor, string command)` (line 638; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Tend(ICharacter actor, string command)` (line 798; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Repair(ICharacter actor, string command)` (line 901; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `HealthModule.Dislodge(ICharacter actor, string command)` (line 1190; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |

### [MudSharpCore/Commands/Modules/InventoryModule.cs](../../MudSharpCore/Commands/Modules/InventoryModule.cs)

| Function | Change |
| --- | --- |
| `InventoryModule.Get(ICharacter actor, string input)` (line 919; changed) | Routes both room and container weight pickup through Body.GetByWeight so the original source is checked before splitting. |

### [MudSharpCore/Commands/Modules/LiteracyModule.cs](../../MudSharpCore/Commands/Modules/LiteracyModule.cs)

| Function | Change |
| --- | --- |
| `LiteracyModule.Graffiti(ICharacter actor, string command)` (line 464; changed) | Checks manual capability and, for item graffiti, target/implement access again when editor text is submitted. |
| `LiteracyModule.GraffitiRoom(ICharacter actor, StringStack ss)` (line 594; changed) | Checks manual capability and, for item graffiti, target/implement access again when editor text is submitted. |

### [MudSharpCore/Commands/Modules/ManipulationModule.cs](../../MudSharpCore/Commands/Modules/ManipulationModule.cs)

| Function | Change |
| --- | --- |
| `ManipulationModule.Weigh(ICharacter actor, string command)` (line 562; changed) | Uses the actorful measurement check before invoking the instrument. |
| `ManipulationModule.Measure(ICharacter actor, string command)` (line 627; changed) | Uses the actorful measurement check before invoking the instrument. |
| `ManipulationModule.Haul(ICharacter actor, string command)` (line 764; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.HaulOut(ICharacter actor, StringStack ss)` (line 896; changed) | Requires usable manipulators, reachable source and item, and source CanTake before extracting the contents. |
| `ManipulationModule.Apply(ICharacter character, string command)` (line 1753; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Dip(ICharacter character, string command)` (line 1948; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Inject(ICharacter character, string command)` (line 2356; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Feed(ICharacter character, string command)` (line 2557; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Swallow(ICharacter character, string command)` (line 2854; changed) | Uses reach-only checks for food/drink/table sources, retaining non-manual eating and drinking while still respecting access. |
| `ManipulationModule.Eat(ICharacter character, string command)` (line 3044; changed) | Uses reach-only checks for food/drink/table sources, retaining non-manual eating and drinking while still respecting access. |
| `ManipulationModule.Drink(ICharacter character, string command)` (line 3223; changed) | Uses reach-only checks for food/drink/table sources, retaining non-manual eating and drinking while still respecting access. |
| `ManipulationModule.Knock(ICharacter character, string command)` (line 4481; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Lob(ICharacter actor, string command)` (line 8709; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Bundle(ICharacter actor, string command)` (line 8975; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |
| `ManipulationModule.Roll(ICharacter actor, string command)` (line 9061; changed) | Adds the manual gate to this command-only physical action; medical/component operations also use the shared runtime or delayed-effect checks listed below. |

### [MudSharpCore/Commands/Modules/TrapModule.cs](../../MudSharpCore/Commands/Modules/TrapModule.cs)

| Function | Change |
| --- | --- |
| `TrapModule.DisarmTrap(ICharacter actor, StringStack command)` (line 505; changed) | Checks item/tool reach and manual capability at both action start and delayed completion. |
| `TrapModule.RecoverTrap(ICharacter actor, StringStack command)` (line 596; changed) | Checks item/tool reach and manual capability at both action start and delayed completion. |

### [MudSharpCore/Economy/Shops/Shop.cs](../../MudSharpCore/Economy/Shops/Shop.cs)

| Function | Change |
| --- | --- |
| `Shop.RegisterStockItemSplit(IGameItem source, IGameItem split)` (line 483; added) | Indexes an existing stock split without a stock transaction; increases pile count only for commodities and ignores repeat notifications. |
| `Shop.RegisterStockItemMerge(IGameItem target, IGameItem absorbed)` (line 499; added) | Removes the absorbed pile ID without a stock transaction; preserves stack-unit totals and decrements non-stack pile counts only once. |
| `Shop.MerchandiseForStockItem(IGameItem item)` (line 533; changed) | Uses the exact display-effect merchandise before generic prototype matching when a split is not yet indexed. |
| `Shop.RemoveFromStockInternal(ICharacter actor, IGameItem item, ShopTransactionType transactionType)` (line 546; changed) | Records the split weight loss and decrements an indexed pile, while preserving the residual source count for legacy unindexed commodity splits. |
| `Shop.BuyCommodityWeightFromStockedItems(ICharacter actor, IMerchandise merchandise, double weight, IPaymentMethod method, List<IGameItem> stockedItems)` (line 1207; changed) | Passes the actual indexed pile count removed by the purchase to reconciliation, preventing legitimate partial/whole purchases from becoming stock discrepancies. |

### [MudSharpCore/Effects/Concrete/Binding.cs](../../MudSharpCore/Effects/Concrete/Binding.cs)

| Function | Change |
| --- | --- |
| `Binding.ExpireEffect()` (line 132; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/Butchering.cs](../../MudSharpCore/Effects/Concrete/Butchering.cs)

| Function | Change |
| --- | --- |
| `Butchering.ExpireEffect()` (line 133; added) | Rechecks manual capability and target/tool access at each staged callback; stops/removes the effect before doing further work when access is lost. |

### [MudSharpCore/Effects/Concrete/CleaningWounds.cs](../../MudSharpCore/Effects/Concrete/CleaningWounds.cs)

| Function | Change |
| --- | --- |
| `CleaningWounds.ExpireEffect()` (line 167; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/InkingTattoo.cs](../../MudSharpCore/Effects/Concrete/InkingTattoo.cs)

| Function | Change |
| --- | --- |
| `InkingTattoo.ExpireEffect()` (line 122; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/ItemComponentConfigurationAction.cs](../../MudSharpCore/Effects/Concrete/ItemComponentConfigurationAction.cs)

| Function | Change |
| --- | --- |
| `ItemComponentConfigurationAction.ExpireEffect()` (line 120; changed) | Rechecks manual capability and target/tool access at each staged callback; stops/removes the effect before doing further work when access is lost. |

### [MudSharpCore/Effects/Concrete/PerformingCPR.cs](../../MudSharpCore/Effects/Concrete/PerformingCPR.cs)

| Function | Change |
| --- | --- |
| `PerformingCPR.ExpireEffect()` (line 18; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/Skinning.cs](../../MudSharpCore/Effects/Concrete/Skinning.cs)

| Function | Change |
| --- | --- |
| `Skinning.ExpireEffect()` (line 92; added) | Rechecks manual capability and target/tool access at each staged callback; stops/removes the effect before doing further work when access is lost. |

### [MudSharpCore/Effects/Concrete/SurgicalProcedureEffect.cs](../../MudSharpCore/Effects/Concrete/SurgicalProcedureEffect.cs)

| Function | Change |
| --- | --- |
| `SurgicalProcedureEffect.ExpireEffect()` (line 46; added) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/Suturing.cs](../../MudSharpCore/Effects/Concrete/Suturing.cs)

| Function | Change |
| --- | --- |
| `Suturing.ExpireEffect()` (line 86; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/Effects/Concrete/TendingWounds.cs](../../MudSharpCore/Effects/Concrete/TendingWounds.cs)

| Function | Change |
| --- | --- |
| `TendingWounds.ExpireEffect()` (line 87; changed) | Rechecks the operator at each treatment/surgery/tattoo callback and stops/removes the action before its next mutation when the manipulator is unusable. |

### [MudSharpCore/GameItems/Components/AccessControlReaderGameItemComponent.cs](../../MudSharpCore/GameItems/Components/AccessControlReaderGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `AccessControlReaderGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 305; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AccessControlReaderGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 332; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/AmmoClipGameItemComponent.cs](../../MudSharpCore/GameItems/Components/AmmoClipGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `AmmoClipGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 135; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/AnsweringMachineGameItemComponent.cs](../../MudSharpCore/GameItems/Components/AnsweringMachineGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `AnsweringMachineGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 278; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 291; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 319; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `AnsweringMachineGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 349; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 359; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 386; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `AnsweringMachineGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 899; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 920; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `AnsweringMachineGameItemComponent.Switch(ICharacter actor, string setting)` (line 937; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanPickUp(ICharacter actor, out string error)` (line 966; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.PickUp(ICharacter actor, out string error)` (line 995; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanDial(ICharacter actor, string number, out string error)` (line 1026; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.Dial(ICharacter actor, string number, out string error)` (line 1066; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanSendDigits(ICharacter actor, string digits, out string error)` (line 1088; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.SendDigits(ICharacter actor, string digits, out string error)` (line 1117; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanAnswer(ICharacter actor, out string error)` (line 1135; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.Answer(ICharacter actor, out string error)` (line 1158; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanHangUp(ICharacter actor, out string error)` (line 1173; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.HangUp(ICharacter actor, out string error)` (line 1190; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.CanSelect(ICharacter character, string argument)` (line 1603; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AnsweringMachineGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 1613; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AnsweringMachineGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 1898; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/ArtilleryPieceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ArtilleryPieceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ArtilleryPieceGameItemComponent.CanPerform(ICharacter character, ArtilleryCrewAction action, out string reason)` (line 202; changed) | Requires physical capability/access for operating roles while retaining the non-manual command/leadership role. |
| `ArtilleryPieceGameItemComponent.Limber(ICharacter actor)` (line 321; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ArtilleryPieceGameItemComponent.Emplace(ICharacter actor)` (line 337; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ArtilleryPieceGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 350; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ArtilleryPieceGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 378; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ArtilleryPieceGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 425; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ArtilleryPieceGameItemComponent.CanReady(ICharacter readier)` (line 543; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ArtilleryPieceGameItemComponent.WhyCannotReady(ICharacter readier)` (line 557; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ArtilleryPieceGameItemComponent.Ready(ICharacter readier)` (line 572; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ArtilleryPieceGameItemComponent.CanUnload(ICharacter loader)` (line 618; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ArtilleryPieceGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 628; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ArtilleryPieceGameItemComponent.Unload(ICharacter loader)` (line 641; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ArtilleryPieceGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 669; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ArtilleryPieceGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 681; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ArtilleryPieceGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 696; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/AttachableConnectableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/AttachableConnectableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `AttachableConnectableGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 121; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AttachableConnectableGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 141; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AttachableConnectableGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 174; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `AttachableConnectableGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 204; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AttachableConnectableGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 215; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AttachableConnectableGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 247; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/AutomationMountHostGameItemComponent.cs](../../MudSharpCore/GameItems/Components/AutomationMountHostGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `AutomationMountHostGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 238; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AutomationMountHostGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 266; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AutomationMountHostGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 305; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `AutomationMountHostGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 330; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `AutomationMountHostGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 340; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `AutomationMountHostGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 393; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/BatteryChargerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BatteryChargerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BatteryChargerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 440; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/BatteryPoweredGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BatteryPoweredGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BatteryPoweredGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 661; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BatteryPoweredGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 677; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BatteryPoweredGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 706; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BatteryPoweredGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 736; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BatteryPoweredGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 746; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BatteryPoweredGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 774; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BatteryPoweredGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 986; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/BenchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BenchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BenchGameItemComponent.AddChair(ICharacter character, IChair chair)` (line 253; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BenchGameItemComponent.CanAddChair(ICharacter character, IChair chair)` (line 265; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BenchGameItemComponent.WhyCannotAddChair(ICharacter character, IChair chair)` (line 276; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BenchGameItemComponent.CanRemoveChair(ICharacter character, IChair chair)` (line 299; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BenchGameItemComponent.WhyCannotRemoveChair(ICharacter character, IChair chair)` (line 310; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BenchGameItemComponent.RemoveChair(ICharacter character, IChair chair)` (line 325; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BenchGameItemComponent.Flip(ICharacter flipper, IEmote? playerEmote = null, bool silent = false)` (line 393; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BenchGameItemComponent.CanFlip(ICharacter flipper)` (line 425; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BenchGameItemComponent.WhyCannotFlip(ICharacter flipper)` (line 445; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/BiometricScannerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BiometricScannerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BiometricScannerGameItemComponent.CanScan(ICharacter actor, IGameItem? severedBodypart, out long identityId, out string error)` (line 115; changed) | Checks reach to the scanner; presenting a severed specimen also requires manipulation of scanner and specimen, while self scanning retains the configured usable bodypart rule. |

### [MudSharpCore/GameItems/Components/BlowgunGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BlowgunGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BlowgunGameItemComponent.CanUseBreathToFire(ICharacter actor)` (line 131; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.CanReady(ICharacter readier)` (line 155; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.WhyCannotReady(ICharacter readier)` (line 192; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.Ready(ICharacter readier)` (line 230; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.CanUnload(ICharacter loader)` (line 289; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BlowgunGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 299; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BlowgunGameItemComponent.Unload(ICharacter loader)` (line 319; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BlowgunGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 342; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BlowgunGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 358; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BlowgunGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 381; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BlowgunGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 432; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 443; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |
| `BlowgunGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 470; changed) | Uses current reach plus a present, usable, uncovered breathing mouth for breath operation; retains authored free-hand ready requirements and avoids a generic grabber requirement for firing. |

### [MudSharpCore/GameItems/Components/BodypartGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BodypartGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BodypartGameItemComponent.Butcher(ICharacter butcher, string subcategory = null)` (line 729; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BodypartGameItemComponent.Skin(ICharacter skinner)` (line 849; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BodypartGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 918; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/BoltActionGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BoltActionGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BoltActionGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 97; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BoltActionGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 120; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BoltActionGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 193; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BoltActionGameItemComponent.CanUnload(ICharacter loader)` (line 229; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BoltActionGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 239; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BoltActionGameItemComponent.Unload(ICharacter loader)` (line 254; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BoltActionGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 285; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BoltActionGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 295; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/BookGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BookGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BookGameItemComponent.CanWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 302; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BookGameItemComponent.WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 339; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BookGameItemComponent.Write(ICharacter character, IWritingImplement implement, IWriting writing)` (line 377; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BookGameItemComponent.WhyCannotGiveTitle(ICharacter character, string title)` (line 403; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BookGameItemComponent.CanGiveTitle(ICharacter character, string title)` (line 414; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BookGameItemComponent.GiveTitle(ICharacter character, string title)` (line 424; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BookGameItemComponent.CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 463; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BookGameItemComponent.WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 500; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BookGameItemComponent.Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 538; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BookGameItemComponent.Turn(ICharacter actor, double turnExtent, IEmote emote)` (line 594; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BookGameItemComponent.CanTurn(ICharacter actor, double turnExtent)` (line 624; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BookGameItemComponent.WhyCannotTurn(ICharacter actor, double turnExtent)` (line 655; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BookGameItemComponent.Tear(ICharacter actor, IEmote emote)` (line 852; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BookGameItemComponent.CanTear(ICharacter actor)` (line 927; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BookGameItemComponent.WhyCannotTear(ICharacter actor)` (line 952; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/BottomlessTorchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BottomlessTorchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BottomlessTorchGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 37; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BottomlessTorchGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 47; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/BowGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BowGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BowGameItemComponent.CanReady(ICharacter readier)` (line 164; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BowGameItemComponent.WhyCannotReady(ICharacter readier)` (line 194; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BowGameItemComponent.Ready(ICharacter readier)` (line 224; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BowGameItemComponent.CanUnload(ICharacter loader)` (line 280; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BowGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 290; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BowGameItemComponent.Unload(ICharacter loader)` (line 310; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BowGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 334; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BowGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 350; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BowGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 376; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `BowGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 422; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `BowGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 432; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `BowGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 453; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/BreathingFilterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/BreathingFilterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `BreathingFilterGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 173; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/CannulaGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CannulaGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CannulaGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 310; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CannulaGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 331; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CannulaGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 358; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CannulaGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 388; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CannulaGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 398; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CannulaGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 425; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/CashRegisterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CashRegisterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CashRegisterGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 243; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `CashRegisterGameItemComponent.CanSelect(ICharacter character, string argument)` (line 333; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CashRegisterGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 363; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CellPhoneTowerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CellPhoneTowerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CellPhoneTowerGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 141; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellPhoneTowerGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 152; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CellPhoneTowerGameItemComponent.Switch(ICharacter actor, string setting)` (line 164; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CellularPhoneGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CellularPhoneGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CellularPhoneGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 305; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 336; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CellularPhoneGameItemComponent.Switch(ICharacter actor, string setting)` (line 359; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CellularPhoneGameItemComponent.CanPickUp(ICharacter actor, out string error)` (line 395; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.PickUp(ICharacter actor, out string error)` (line 424; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CellularPhoneGameItemComponent.CanDial(ICharacter actor, string number, out string error)` (line 455; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.Dial(ICharacter actor, string number, out string error)` (line 501; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CellularPhoneGameItemComponent.CanSendDigits(ICharacter actor, string digits, out string error)` (line 533; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.SendDigits(ICharacter actor, string digits, out string error)` (line 562; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CellularPhoneGameItemComponent.CanAnswer(ICharacter actor, out string error)` (line 580; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.Answer(ICharacter actor, out string error)` (line 609; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CellularPhoneGameItemComponent.CanHangUp(ICharacter actor, out string error)` (line 624; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CellularPhoneGameItemComponent.HangUp(ICharacter actor, out string error)` (line 641; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ClockDetonatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ClockDetonatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ClockDetonatorGameItemComponent.CanArm(ICharacter actor, string argument)` (line 129; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ClockDetonatorGameItemComponent.WhyCannotArm(ICharacter actor, string argument)` (line 139; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ClockDetonatorGameItemComponent.Arm(ICharacter actor, string argument, IEmote? playerEmote = null)` (line 161; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ClockDetonatorGameItemComponent.CanDisarm(ICharacter actor)` (line 187; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ClockDetonatorGameItemComponent.WhyCannotDisarm(ICharacter actor)` (line 197; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ClockDetonatorGameItemComponent.Disarm(ICharacter actor, IEmote? playerEmote = null)` (line 214; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CombustionEngineGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CombustionEngineGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CombustionEngineGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 77; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CombustionEngineGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 92; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CombustionEngineGameItemComponent.Switch(ICharacter actor, string setting)` (line 111; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CompressorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CompressorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CompressorGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 124; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CompressorGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 145; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CompressorGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 172; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CompressorGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 202; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CompressorGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 212; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CompressorGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 239; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ComputerHostGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ComputerHostGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ComputerHostGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 664; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerHostGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 679; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerHostGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 715; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ComputerHostGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 730; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerHostGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 740; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerHostGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 773; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ComputerStorageGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ComputerStorageGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ComputerStorageGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 304; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerStorageGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 316; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerStorageGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 347; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ComputerStorageGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 364; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerStorageGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 374; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerStorageGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 405; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ComputerTerminalGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ComputerTerminalGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ComputerTerminalGameItemComponent.TryConnectSession(ICharacter actor, out IComputerTerminalSession? session, out string error)` (line 138; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerTerminalGameItemComponent.TrySelectOwner(ICharacter actor, IComputerExecutableOwner owner, out string error)` (line 210; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerTerminalGameItemComponent.TryType(ICharacter actor, string text, out string error)` (line 236; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerTerminalGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 258; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerTerminalGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 270; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerTerminalGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 295; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ComputerTerminalGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 312; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ComputerTerminalGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 322; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ComputerTerminalGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 352; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ConnectableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ConnectableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ConnectableGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 170; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ConnectableGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 197; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ConnectableGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 224; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ConnectableGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 260; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ConnectableGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 270; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ConnectableGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 297; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ContainerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ContainerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ContainerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 448; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/CorpseGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CorpseGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CorpseGameItemComponent.Butcher(ICharacter butcher, string subcategory = null)` (line 447; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CorpseGameItemComponent.Skin(ICharacter skinner)` (line 547; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CountdownDetonatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CountdownDetonatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CountdownDetonatorGameItemComponent.CanArm(ICharacter actor, string argument)` (line 57; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CountdownDetonatorGameItemComponent.WhyCannotArm(ICharacter actor, string argument)` (line 69; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CountdownDetonatorGameItemComponent.Arm(ICharacter actor, string argument, IEmote? playerEmote = null)` (line 91; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CountdownDetonatorGameItemComponent.CanDisarm(ICharacter actor)` (line 116; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CountdownDetonatorGameItemComponent.WhyCannotDisarm(ICharacter actor)` (line 126; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CountdownDetonatorGameItemComponent.Disarm(ICharacter actor, IEmote? playerEmote = null)` (line 143; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/CrossbowGameItemComponent.cs](../../MudSharpCore/GameItems/Components/CrossbowGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `CrossbowGameItemComponent.Emplace(ICharacter actor, out string reason)` (line 182; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CrossbowGameItemComponent.Limber(ICharacter actor, out string reason)` (line 205; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CrossbowGameItemComponent.CanReady(ICharacter readier)` (line 228; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CrossbowGameItemComponent.WhyCannotReady(ICharacter readier)` (line 265; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CrossbowGameItemComponent.Ready(ICharacter readier)` (line 311; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CrossbowGameItemComponent.CanUnload(ICharacter loader)` (line 387; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CrossbowGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 397; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CrossbowGameItemComponent.Unload(ICharacter loader)` (line 418; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CrossbowGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 442; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CrossbowGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 463; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CrossbowGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 494; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `CrossbowGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 534; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `CrossbowGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 544; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `CrossbowGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 569; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/DefibrillatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/DefibrillatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `DefibrillatorGameItemComponent.CanShock(ICharacter shocker, IBody target)` (line 23; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `DefibrillatorGameItemComponent.WhyCannotShock(ICharacter shocker, IBody target)` (line 55; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `DefibrillatorGameItemComponent.Shock(ICharacter shocker, IBody target)` (line 95; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/DoorGameItemComponentBase.cs](../../MudSharpCore/GameItems/Components/DoorGameItemComponentBase.cs)

| Function | Change |
| --- | --- |
| `DoorGameItemComponentBase.Knock(ICharacter actor, IEmote? playerEmote = null)` (line 351; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/DripGameItemComponent.cs](../../MudSharpCore/GameItems/Components/DripGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `DripGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 300; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `DripGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 321; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `DripGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 348; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `DripGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 378; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `DripGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 388; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `DripGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 415; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `DripGameItemComponent.CanSelect(ICharacter character, string argument)` (line 442; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `DripGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 458; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/DryerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/DryerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `DryerGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 93; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `DryerGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 96; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `DryerGameItemComponent.Switch(ICharacter actor, string setting)` (line 101; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/EarpieceRadioGameItemComponent.cs](../../MudSharpCore/GameItems/Components/EarpieceRadioGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `EarpieceRadioGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 251; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `EarpieceRadioGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 272; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `EarpieceRadioGameItemComponent.Switch(ICharacter actor, string setting)` (line 290; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ElectricGridFeederGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ElectricGridFeederGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ElectricGridFeederGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 156; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ElectricGridFeederGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 177; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ElectricGridFeederGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 204; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ElectricGridFeederGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 234; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ElectricGridFeederGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 244; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ElectricGridFeederGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 271; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ElectricGridOutletGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ElectricGridOutletGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ElectricGridOutletGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 329; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ElectricGridOutletGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 350; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ElectricGridOutletGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 377; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ElectricGridOutletGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 407; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ElectricGridOutletGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 417; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ElectricGridOutletGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 444; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ElectricLightGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ElectricLightGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ElectricLightGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 175; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ElectricLightGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 188; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ElectricLightGameItemComponent.Switch(ICharacter actor, string setting)` (line 224; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ExternalInhalerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ExternalInhalerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ExternalInhalerGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 85; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ExternalInhalerGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 116; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ExternalInhalerGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 142; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ExternalInhalerGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 152; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ExternalInhalerGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 177; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ExternalInhalerGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 218; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ExternalOrganGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ExternalOrganGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ExternalOrganGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 354; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ExternalOrganGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 375; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ExternalOrganGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 402; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ExternalOrganGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 432; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ExternalOrganGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 442; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ExternalOrganGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 469; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ExternalOrganGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 575; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ExternalOrganGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 588; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ExternalOrganGameItemComponent.Switch(ICharacter actor, string setting)` (line 624; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/FaxMachineGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FaxMachineGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FaxMachineGameItemComponent.Switch(ICharacter actor, string setting)` (line 233; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FaxMachineGameItemComponent.CanSendFax(ICharacter actor, string number, IReadable document, out string error)` (line 253; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FaxMachineGameItemComponent.SendFax(ICharacter actor, string number, IReadable document, out string error)` (line 294; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FaxMachineGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 543; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/FirearmBaseGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FirearmBaseGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FirearmBaseGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 140; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FirearmBaseGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 163; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FirearmBaseGameItemComponent.Switch(ICharacter actor, string setting)` (line 199; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FirearmBaseGameItemComponent.CanReady(ICharacter readier)` (line 299; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FirearmBaseGameItemComponent.WhyCannotReady(ICharacter readier)` (line 315; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FirearmBaseGameItemComponent.Ready(ICharacter readier)` (line 332; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FirearmBaseGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 413; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/FlareGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FlareGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FlareGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 187; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FlareGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 203; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FlareGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 228; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FlareGameItemComponent.CanExtinguish(ICharacter lightee)` (line 248; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FlareGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 258; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FlareGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 270; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/FuelGeneratorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FuelGeneratorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FuelGeneratorGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 393; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FuelGeneratorGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 409; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FuelGeneratorGameItemComponent.Switch(ICharacter actor, string setting)` (line 435; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/FuelHeaterCoolerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FuelHeaterCoolerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FuelHeaterCoolerGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 194; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FuelHeaterCoolerGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 211; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FuelHeaterCoolerGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 238; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FuelHeaterCoolerGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 258; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FuelHeaterCoolerGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 268; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FuelHeaterCoolerGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 299; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/FuseGameItemComponent.cs](../../MudSharpCore/GameItems/Components/FuseGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `FuseGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 161; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FuseGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 176; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FuseGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 196; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `FuseGameItemComponent.CanExtinguish(ICharacter lightee)` (line 216; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `FuseGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 236; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `FuseGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 262; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/GasContainerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/GasContainerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `GasContainerGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 381; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GasContainerGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 402; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GasContainerGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 429; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `GasContainerGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 459; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GasContainerGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 469; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GasContainerGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 496; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/GridLiquidSourceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/GridLiquidSourceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `GridLiquidSourceGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 237; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GridLiquidSourceGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 253; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GridLiquidSourceGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 281; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `GridLiquidSourceGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 311; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GridLiquidSourceGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 321; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GridLiquidSourceGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 348; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/GunGameItemComponent.cs](../../MudSharpCore/GameItems/Components/GunGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `GunGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 89; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GunGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 112; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `GunGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 171; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GunGameItemComponent.CanUnload(ICharacter loader)` (line 204; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GunGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 214; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `GunGameItemComponent.Unload(ICharacter loader)` (line 229; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `GunGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 260; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `GunGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 270; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/HandheldRadioGameItemComponent.cs](../../MudSharpCore/GameItems/Components/HandheldRadioGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `HandheldRadioGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 230; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `HandheldRadioGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 259; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `HandheldRadioGameItemComponent.Switch(ICharacter actor, string setting)` (line 285; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/IVBagGameItemComponent.cs](../../MudSharpCore/GameItems/Components/IVBagGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `IVBagGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 622; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `IVBagGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 660; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `IVBagGameItemComponent.Switch(ICharacter actor, string setting)` (line 698; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `IVBagGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 953; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `IVBagGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 974; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `IVBagGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 1008; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `IVBagGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 1038; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `IVBagGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 1048; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `IVBagGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 1081; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/ImplantContainerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ImplantContainerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ImplantContainerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 237; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/IncenseBurnerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/IncenseBurnerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `IncenseBurnerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 486; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `IncenseBurnerGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 525; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `IncenseBurnerGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 537; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `IncenseBurnerGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 562; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `IncenseBurnerGameItemComponent.CanExtinguish(ICharacter lightee)` (line 583; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `IncenseBurnerGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 593; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `IncenseBurnerGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 610; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/InscribableSurfaceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/InscribableSurfaceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `InscribableSurfaceGameItemComponent.CanWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 156; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InscribableSurfaceGameItemComponent.WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 190; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `InscribableSurfaceGameItemComponent.Write(ICharacter character, IWritingImplement implement, IWriting writing)` (line 224; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `InscribableSurfaceGameItemComponent.WhyCannotGiveTitle(ICharacter character, string title)` (line 252; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `InscribableSurfaceGameItemComponent.CanGiveTitle(ICharacter character, string title)` (line 263; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InscribableSurfaceGameItemComponent.GiveTitle(ICharacter character, string title)` (line 273; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `InscribableSurfaceGameItemComponent.CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 303; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InscribableSurfaceGameItemComponent.WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 337; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `InscribableSurfaceGameItemComponent.Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 371; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/InstrumentGameItemComponent.cs](../../MudSharpCore/GameItems/Components/InstrumentGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `InstrumentGameItemComponent.WhyCannotPlay(ICharacter actor, string style)` (line 75; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |
| `InstrumentGameItemComponent.CanContinue(ICharacter actor)` (line 227; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |

### [MudSharpCore/GameItems/Components/InternalMagazineGunGameItemComponent.cs](../../MudSharpCore/GameItems/Components/InternalMagazineGunGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `InternalMagazineGunGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 111; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InternalMagazineGunGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 134; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `InternalMagazineGunGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 213; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `InternalMagazineGunGameItemComponent.CanUnload(ICharacter loader)` (line 265; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InternalMagazineGunGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 275; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `InternalMagazineGunGameItemComponent.Unload(ICharacter loader)` (line 290; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `InternalMagazineGunGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 335; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `InternalMagazineGunGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 345; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/KeycardWriterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/KeycardWriterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `KeycardWriterGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 138; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `KeycardWriterGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 169; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/KeypadGameItemComponent.cs](../../MudSharpCore/GameItems/Components/KeypadGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `KeypadGameItemComponent.CanSelect(ICharacter character, string argument)` (line 68; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `KeypadGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 71; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/KeyringGameItemComponent.cs](../../MudSharpCore/GameItems/Components/KeyringGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `KeyringGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 362; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/LanternGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LanternGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LanternGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 502; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LanternGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 518; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LanternGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 548; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LanternGameItemComponent.CanExtinguish(ICharacter lightee)` (line 590; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LanternGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 605; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LanternGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 625; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/LaserGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LaserGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LaserGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 146; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LaserGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 169; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LaserGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 201; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LaserGameItemComponent.CanUnload(ICharacter loader)` (line 230; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LaserGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 240; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LaserGameItemComponent.Unload(ICharacter loader)` (line 255; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LaserGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 286; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LaserGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 296; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LaserGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 306; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LaserGameItemComponent.CanReady(ICharacter readier)` (line 351; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LaserGameItemComponent.WhyCannotReady(ICharacter readier)` (line 372; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LaserGameItemComponent.Ready(ICharacter readier)` (line 394; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LaserGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 449; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LaserGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 469; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LaserGameItemComponent.Switch(ICharacter actor, string setting)` (line 490; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/LatchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LatchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LatchGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 121; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LatchGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 141; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LatchGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 166; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LatchGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 186; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/LiquidConsumingPropGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LiquidConsumingPropGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LiquidConsumingPropGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 455; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LiquidConsumingPropGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 471; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LiquidConsumingPropGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 499; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LiquidConsumingPropGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 529; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LiquidConsumingPropGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 539; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LiquidConsumingPropGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 566; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/LiquidPumpGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LiquidPumpGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LiquidPumpGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 349; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LiquidPumpGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 365; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LiquidPumpGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 395; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `LiquidPumpGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 425; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LiquidPumpGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 435; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LiquidPumpGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 464; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/LockingCashRegisterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LockingCashRegisterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LockingCashRegisterGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 127; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingCashRegisterGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 139; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LockingCashRegisterGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 157; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingCashRegisterGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 167; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/LockingContainerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LockingContainerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LockingContainerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 237; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `LockingContainerGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 446; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingContainerGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 466; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LockingContainerGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 498; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingContainerGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 513; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/LockingDoorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/LockingDoorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `LockingDoorGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 121; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingDoorGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 141; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `LockingDoorGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 176; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `LockingDoorGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 186; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/MeasuringInstrumentGameItemComponent.cs](../../MudSharpCore/GameItems/Components/MeasuringInstrumentGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `MeasuringInstrumentGameItemComponent.CanMeasure(ICharacter actor, IGameItem target, out string error)` (line 125; added) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |
| `MeasuringInstrumentGameItemComponent.Measure(ICharacter actor, IGameItem target)` (line 130; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |
| `MeasuringInstrumentGameItemComponent.Calibrate(ICharacter actor)` (line 147; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |
| `MeasuringInstrumentGameItemComponent.CalibrateWrong(ICharacter actor, double bias, bool percentageBias, out string error)` (line 163; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |

### [MudSharpCore/GameItems/Components/MediaEndpointPoweredComponentBase.cs](../../MudSharpCore/GameItems/Components/MediaEndpointPoweredComponentBase.cs)

| Function | Change |
| --- | --- |
| `MediaEndpointPoweredComponentBase.CanConnect(ICharacter? actor, IConnectable other)` (line 103; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MediaEndpointPoweredComponentBase.Connect(ICharacter? actor, IConnectable other)` (line 121; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MediaEndpointPoweredComponentBase.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 155; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MediaEndpointPoweredComponentBase.CanDisconnect(ICharacter actor, IConnectable other)` (line 183; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MediaEndpointPoweredComponentBase.Disconnect(ICharacter actor, IConnectable other)` (line 193; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MediaEndpointPoweredComponentBase.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 227; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/MicrocontrollerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/MicrocontrollerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `MicrocontrollerGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 480; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MicrocontrollerGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 495; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MicrocontrollerGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 530; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MicrocontrollerGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 563; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MicrocontrollerGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 573; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MicrocontrollerGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 610; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/MilitaryStandardGameItemComponent.cs](../../MudSharpCore/GameItems/Components/MilitaryStandardGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `MilitaryStandardGameItemComponent.Plant(ICharacter actor, IEmote? playerEmote = null)` (line 228; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MilitaryStandardGameItemComponent.TakeUp(ICharacter actor, IEmote? playerEmote = null)` (line 263; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MilitaryStandardGameItemComponent.Signal(ICharacter actor, string pattern, IEmote? playerEmote = null)` (line 293; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/MusketGameItemComponent.cs](../../MudSharpCore/GameItems/Components/MusketGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `MusketGameItemComponent.CanUnjam(ICharacter actor)` (line 380; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MusketGameItemComponent.WhyCannotUnjam(ICharacter actor)` (line 402; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MusketGameItemComponent.Unjam(ICharacter actor)` (line 429; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.Emplace(ICharacter actor, out string reason)` (line 470; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.Limber(ICharacter actor, out string reason)` (line 493; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 541; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MusketGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 598; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MusketGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 701; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.CanReady(ICharacter readier)` (line 1023; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MusketGameItemComponent.WhyCannotReady(ICharacter readier)` (line 1058; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MusketGameItemComponent.Ready(ICharacter readier)` (line 1105; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.CanUnload(ICharacter loader)` (line 1169; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MusketGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 1194; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MusketGameItemComponent.Unload(ICharacter loader)` (line 1219; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 1275; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `MusketGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 1295; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `MusketGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 1376; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `MusketGameItemComponent.TryInstallIgnitionStone(ICharacter actor, IGameItem stone, out string reason)` (line 1641; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/NetworkAdapterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/NetworkAdapterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `NetworkAdapterGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 242; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `NetworkAdapterGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 264; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `NetworkAdapterGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 299; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `NetworkAdapterGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 324; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `NetworkAdapterGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 335; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `NetworkAdapterGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 377; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/NetworkSwitchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/NetworkSwitchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `NetworkSwitchGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 205; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `NetworkSwitchGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 223; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `NetworkSwitchGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 257; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `NetworkSwitchGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 267; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `NetworkSwitchGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 277; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `NetworkSwitchGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 310; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/OfferingReceiverGameItemComponent.cs](../../MudSharpCore/GameItems/Components/OfferingReceiverGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `OfferingReceiverGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 296; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `OfferingReceiverGameItemComponent.CanOffer(ICharacter actor, IGameItem offering)` (line 330; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `OfferingReceiverGameItemComponent.WhyCannotOffer(ICharacter actor, IGameItem offering)` (line 340; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `OfferingReceiverGameItemComponent.CanBurnOffering(ICharacter actor, IGameItem offering)` (line 395; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `OfferingReceiverGameItemComponent.WhyCannotBurnOffering(ICharacter actor, IGameItem offering)` (line 408; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `OfferingReceiverGameItemComponent.CanOfferLiquid(ICharacter actor, IGameItem source, double amount)` (line 466; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `OfferingReceiverGameItemComponent.WhyCannotOfferLiquid(ICharacter actor, IGameItem source, double amount)` (line 492; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `OfferingReceiverGameItemComponent.OfferLiquid(ICharacter actor, IGameItem source, double amount, IEmote? playerEmote)` (line 562; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/PaperSheetGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PaperSheetGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PaperSheetGameItemComponent.CanWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 172; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PaperSheetGameItemComponent.WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing)` (line 209; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PaperSheetGameItemComponent.Write(ICharacter character, IWritingImplement implement, IWriting writing)` (line 246; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `PaperSheetGameItemComponent.WhyCannotGiveTitle(ICharacter character, string title)` (line 274; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PaperSheetGameItemComponent.CanGiveTitle(ICharacter character, string title)` (line 285; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PaperSheetGameItemComponent.GiveTitle(ICharacter character, string title)` (line 295; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `PaperSheetGameItemComponent.CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 337; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PaperSheetGameItemComponent.WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 374; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PaperSheetGameItemComponent.Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)` (line 411; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/PencilSharpenerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PencilSharpenerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PencilSharpenerGameItemComponent.CanSharpen(ICharacter actor, IGameItem otherItem)` (line 62; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PencilSharpenerGameItemComponent.WhyCannotSharpen(ICharacter actor, IGameItem otherItem)` (line 83; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PencilSharpenerGameItemComponent.Sharpen(ICharacter actor, IGameItem otherItem)` (line 106; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/PhotocopierGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PhotocopierGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PhotocopierGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 489; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/PileGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PileGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PileGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? emote = null)` (line 356; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/PinPullDetonatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PinPullDetonatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PinPullDetonatorGameItemComponent.CanPullPin(ICharacter actor)` (line 56; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PinPullDetonatorGameItemComponent.WhyCannotPullPin(ICharacter actor)` (line 67; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PinPullDetonatorGameItemComponent.PullPin(ICharacter actor, IEmote? playerEmote = null)` (line 89; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/PowerSocketGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PowerSocketGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PowerSocketGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 306; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PowerSocketGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 327; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `PowerSocketGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 354; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PowerSocketGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 384; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PowerSocketGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 394; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `PowerSocketGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 421; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/PoweredMachineBaseGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PoweredMachineBaseGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PoweredMachineBaseGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 225; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PoweredMachineBaseGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 242; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `PoweredMachineBaseGameItemComponent.Switch(ICharacter actor, string setting)` (line 279; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ProgLightGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ProgLightGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ProgLightGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 117; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ProgLightGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 127; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ProgLightGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 137; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ProgLightGameItemComponent.CanExtinguish(ICharacter lightee)` (line 153; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ProgLightGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 163; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ProgLightGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 173; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ProgLockGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ProgLockGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ProgLockGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 151; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ProgLockGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 161; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ProgLockGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 198; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ProgLockGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 208; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/PushButtonGameItemComponent.cs](../../MudSharpCore/GameItems/Components/PushButtonGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `PushButtonGameItemComponent.CanSelect(ICharacter character, string argument)` (line 114; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `PushButtonGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 125; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/RadioDetonatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/RadioDetonatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `RadioDetonatorGameItemComponent.CanArm(ICharacter actor, string argument)` (line 136; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorGameItemComponent.WhyCannotArm(ICharacter actor, string argument)` (line 146; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RadioDetonatorGameItemComponent.Arm(ICharacter actor, string argument, IEmote playerEmote = null)` (line 168; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RadioDetonatorGameItemComponent.CanDisarm(ICharacter actor)` (line 187; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorGameItemComponent.WhyCannotDisarm(ICharacter actor)` (line 197; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RadioDetonatorGameItemComponent.Disarm(ICharacter actor, IEmote playerEmote = null)` (line 209; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RadioDetonatorGameItemComponent.CanSelect(ICharacter character, string argument)` (line 261; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 282; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RadioDetonatorGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 316; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 329; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RadioDetonatorGameItemComponent.Switch(ICharacter actor, string setting)` (line 363; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/RadioDetonatorTransmitterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/RadioDetonatorTransmitterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `RadioDetonatorTransmitterGameItemComponent.CanSelect(ICharacter character, string argument)` (line 159; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorTransmitterGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 186; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RadioDetonatorTransmitterGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 227; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RadioDetonatorTransmitterGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 240; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RadioDetonatorTransmitterGameItemComponent.Switch(ICharacter actor, string setting)` (line 274; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/RcsThrusterGameItemComponent.cs](../../MudSharpCore/GameItems/Components/RcsThrusterGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `RcsThrusterGameItemComponent.CanConnect(ICharacter actor, IConnectable other)` (line 63; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RcsThrusterGameItemComponent.Connect(ICharacter actor, IConnectable other)` (line 76; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RcsThrusterGameItemComponent.WhyCannotConnect(ICharacter actor, IConnectable other)` (line 102; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RcsThrusterGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 132; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RcsThrusterGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 142; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RcsThrusterGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 167; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/RebreatherGameItemComponent.cs](../../MudSharpCore/GameItems/Components/RebreatherGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `RebreatherGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 184; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RebreatherGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 204; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RebreatherGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 230; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `RebreatherGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 265; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `RebreatherGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 275; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RebreatherGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 300; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/RepairKitGameItemComponent.cs](../../MudSharpCore/GameItems/Components/RepairKitGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `RepairKitGameItemComponent.Repair(IWound wound, ICharacter repairer)` (line 116; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `RepairKitGameItemComponent.Repair(IEnumerable<IWound> wounds, ICharacter repairer)` (line 133; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SealableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SealableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SealableGameItemComponent.CanSeal(ICharacter actor, ISealStamp stamp, IGameItem? medium, out string error)` (line 104; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SealableGameItemComponent.Seal(ICharacter actor, ISealStamp stamp, IGameItem? medium)` (line 133; changed) | Rechecks CanSeal, including stamp/medium access, before creating the impression or setting seal state. |
| `SealableGameItemComponent.BreakSeal(ICharacter? actor, string reason)` (line 148; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SelectableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SelectableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SelectableGameItemComponent.CanSelect(ICharacter character, string argument)` (line 74; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SelectableGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 86; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SheathGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SheathGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SheathGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 409; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/ShopStallGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ShopStallGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ShopStallGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 549; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `ShopStallGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 778; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ShopStallGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 798; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ShopStallGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 830; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ShopStallGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 845; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SignalDetonatorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SignalDetonatorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SignalDetonatorGameItemComponent.CanArm(ICharacter actor, string argument)` (line 180; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SignalDetonatorGameItemComponent.WhyCannotArm(ICharacter actor, string argument)` (line 190; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SignalDetonatorGameItemComponent.Arm(ICharacter actor, string argument, IEmote? playerEmote = null)` (line 203; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SignalDetonatorGameItemComponent.CanDisarm(ICharacter actor)` (line 229; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SignalDetonatorGameItemComponent.WhyCannotDisarm(ICharacter actor)` (line 239; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SignalDetonatorGameItemComponent.Disarm(ICharacter actor, IEmote? playerEmote = null)` (line 252; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SignalInstrumentGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SignalInstrumentGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SignalInstrumentGameItemComponent.WhyCannotUse(ICharacter actor)` (line 83; changed) | Checks current reach and the configured functioning-hand requirement at start/continuation; a builder-authored zero-hand instrument remains usable. |

### [MudSharpCore/GameItems/Components/SimpleLockGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SimpleLockGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SimpleLockGameItemComponent.CanUnlock(ICharacter actor, IKey key)` (line 155; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SimpleLockGameItemComponent.Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 175; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SimpleLockGameItemComponent.CanLock(ICharacter actor, IKey key)` (line 213; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SimpleLockGameItemComponent.Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)` (line 228; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SlingGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SlingGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SlingGameItemComponent.CanReady(ICharacter readier)` (line 132; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SlingGameItemComponent.WhyCannotReady(ICharacter readier)` (line 163; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SlingGameItemComponent.Ready(ICharacter readier)` (line 194; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SlingGameItemComponent.CanUnload(ICharacter loader)` (line 252; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SlingGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 262; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SlingGameItemComponent.Unload(ICharacter loader)` (line 282; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SlingGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 305; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SlingGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 321; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SlingGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 344; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SlingGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 393; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SlingGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 403; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SlingGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 423; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SmokeableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SmokeableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SmokeableGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 242; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SmokeableGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 258; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SmokeableGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 289; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `SmokeableGameItemComponent.CanExtinguish(ICharacter lightee)` (line 309; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SmokeableGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 324; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SmokeableGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 344; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SolidFuelHeaterCoolerGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SolidFuelHeaterCoolerGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SolidFuelHeaterCoolerGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 307; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/StackableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/StackableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `StackableGameItemComponent.Split(int quantity)` (line 120; changed) | Preserves shop display identity and notifies the shop when a real stack is split; preview splits remain temporary. |

### [MudSharpCore/GameItems/Components/SwitchableThermalSourceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SwitchableThermalSourceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SwitchableThermalSourceGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 91; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `SwitchableThermalSourceGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 101; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `SwitchableThermalSourceGameItemComponent.Switch(ICharacter actor, string setting)` (line 113; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/SyringeGameItemComponent.cs](../../MudSharpCore/GameItems/Components/SyringeGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `SyringeGameItemComponent.Inject(IBody target, IBodypart part, double amount, ICharacter injector)` (line 320; changed) | Checks the implement, manual capability, patient colocation and current target bodypart before dose/resource use; existing injection/application eligibility remains. |

### [MudSharpCore/GameItems/Components/TableGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TableGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TableGameItemComponent.AddChair(ICharacter character, IChair chair)` (line 208; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TableGameItemComponent.CanAddChair(ICharacter character, IChair chair)` (line 220; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TableGameItemComponent.WhyCannotAddChair(ICharacter character, IChair chair)` (line 243; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TableGameItemComponent.CanRemoveChair(ICharacter character, IChair chair)` (line 269; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TableGameItemComponent.WhyCannotRemoveChair(ICharacter character, IChair chair)` (line 280; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TableGameItemComponent.RemoveChair(ICharacter character, IChair chair)` (line 290; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TableGameItemComponent.Flip(ICharacter flipper, IEmote? playerEmote = null, bool silent = false)` (line 358; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TableGameItemComponent.CanFlip(ICharacter flipper)` (line 390; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TableGameItemComponent.WhyCannotFlip(ICharacter flipper)` (line 410; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/TelecommunicationsGridFeederGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TelecommunicationsGridFeederGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TelecommunicationsGridFeederGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 287; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelecommunicationsGridFeederGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 300; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelecommunicationsGridFeederGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 328; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TelecommunicationsGridFeederGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 358; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelecommunicationsGridFeederGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 368; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelecommunicationsGridFeederGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 395; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/TelecommunicationsGridOutletGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TelecommunicationsGridOutletGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TelecommunicationsGridOutletGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 263; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelecommunicationsGridOutletGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 276; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelecommunicationsGridOutletGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 303; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TelecommunicationsGridOutletGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 333; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelecommunicationsGridOutletGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 343; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelecommunicationsGridOutletGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 370; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/Components/TelephoneGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TelephoneGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TelephoneGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 217; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 233; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 261; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TelephoneGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 291; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 301; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 328; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TelephoneGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 626; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 657; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TelephoneGameItemComponent.Switch(ICharacter actor, string setting)` (line 680; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.CanPickUp(ICharacter actor, out string error)` (line 716; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.PickUp(ICharacter actor, out string error)` (line 745; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.CanDial(ICharacter actor, string number, out string error)` (line 776; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.Dial(ICharacter actor, string number, out string error)` (line 816; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.CanSendDigits(ICharacter actor, string digits, out string error)` (line 838; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.SendDigits(ICharacter actor, string digits, out string error)` (line 867; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.CanAnswer(ICharacter actor, out string error)` (line 885; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.Answer(ICharacter actor, out string error)` (line 908; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TelephoneGameItemComponent.CanHangUp(ICharacter actor, out string error)` (line 923; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TelephoneGameItemComponent.HangUp(ICharacter actor, out string error)` (line 940; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/ThrownWeaponGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ThrownWeaponGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ThrownWeaponGameItemComponent.CanUnload(ICharacter loader)` (line 107; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ThrownWeaponGameItemComponent.WhyCannotUnload(ICharacter loader)` (line 117; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ThrownWeaponGameItemComponent.Unload(ICharacter loader)` (line 127; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ThrownWeaponGameItemComponent.CanLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 137; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ThrownWeaponGameItemComponent.WhyCannotLoad(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 147; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ThrownWeaponGameItemComponent.Load(ICharacter loader, bool ignoreEmpty = false, LoadMode mode = LoadMode.Normal)` (line 157; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ThrownWeaponGameItemComponent.CanFire(ICharacter actor, IPerceivable target)` (line 168; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ThrownWeaponGameItemComponent.WhyCannotFire(ICharacter actor, IPerceivable target)` (line 178; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ThrownWeaponGameItemComponent.Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IEmoteOutput defenseEmote, IPerceiver originalTarget)` (line 194; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `ThrownWeaponGameItemComponent.CanReady(ICharacter readier)` (line 404; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ThrownWeaponGameItemComponent.WhyCannotReady(ICharacter readier)` (line 414; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ThrownWeaponGameItemComponent.Ready(ICharacter readier)` (line 424; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/TimePieceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TimePieceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TimePieceGameItemComponent.CanSetTime(ICharacter actor)` (line 87; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |

### [MudSharpCore/GameItems/Components/ToggleSwitchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/ToggleSwitchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `ToggleSwitchGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 111; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `ToggleSwitchGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 122; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `ToggleSwitchGameItemComponent.Switch(ICharacter actor, string setting)` (line 142; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/TopicalCreamGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TopicalCreamGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TopicalCreamGameItemComponent.Apply(IBody target, IBodypart part, double amount, ICharacter applier)` (line 80; changed) | Checks the implement, manual capability, patient colocation and current target bodypart before dose/resource use; existing injection/application eligibility remains. |

### [MudSharpCore/GameItems/Components/TorchGameItemComponent.cs](../../MudSharpCore/GameItems/Components/TorchGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `TorchGameItemComponent.CanLight(ICharacter lightee, IPerceivable ignitionSource)` (line 193; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TorchGameItemComponent.WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)` (line 209; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TorchGameItemComponent.Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)` (line 234; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `TorchGameItemComponent.CanExtinguish(ICharacter lightee)` (line 254; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `TorchGameItemComponent.WhyCannotExtinguish(ICharacter lightee)` (line 269; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `TorchGameItemComponent.Extinguish(ICharacter lightee, IEmote playerEmote)` (line 289; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/UnlimitedGeneratorGameItemComponent.cs](../../MudSharpCore/GameItems/Components/UnlimitedGeneratorGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `UnlimitedGeneratorGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 222; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `UnlimitedGeneratorGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 234; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `UnlimitedGeneratorGameItemComponent.Switch(ICharacter actor, string setting)` (line 254; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/VehicleCargoSpaceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/VehicleCargoSpaceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `VehicleCargoSpaceGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 123; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/VendingMachineGameItemComponent.cs](../../MudSharpCore/GameItems/Components/VendingMachineGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `VendingMachineGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 558; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |
| `VendingMachineGameItemComponent.CanInsert(ICharacter actor, IGameItem item)` (line 796; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `VendingMachineGameItemComponent.Insert(ICharacter actor, IGameItem item, IEmote playerEmote, bool silent = false)` (line 811; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `VendingMachineGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent)` (line 863; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `VendingMachineGameItemComponent.CanSelect(ICharacter character, string argument)` (line 923; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |

### [MudSharpCore/GameItems/Components/WashingMachineGameItemComponent.cs](../../MudSharpCore/GameItems/Components/WashingMachineGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `WashingMachineGameItemComponent.CanSelect(ICharacter character, string argument)` (line 706; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `WashingMachineGameItemComponent.Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)` (line 733; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WashingMachineGameItemComponent.Empty(ICharacter emptier, IContainer intoContainer, IEmote? playerEmote = null)` (line 981; changed) | Preflights the actor, source, optional destination and every original content item through CanTake before clearing/moving any contents. |

### [MudSharpCore/GameItems/Components/WaterSourceGameItemComponent.cs](../../MudSharpCore/GameItems/Components/WaterSourceGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `WaterSourceGameItemComponent.CanSwitch(ICharacter actor, string setting)` (line 601; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `WaterSourceGameItemComponent.WhyCannotSwitch(ICharacter actor, string setting)` (line 619; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `WaterSourceGameItemComponent.Switch(ICharacter actor, string setting)` (line 660; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/WeaponCarrierAttachmentGameItemComponent.cs](../../MudSharpCore/GameItems/Components/WeaponCarrierAttachmentGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `WeaponCarrierAttachmentGameItemComponent.CanAttach(IGameItem weapon, ICharacter actor, out string reason)` (line 36; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `WeaponCarrierAttachmentGameItemComponent.Attach(IGameItem weapon, ICharacter actor, out string reason)` (line 86; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WeaponCarrierAttachmentGameItemComponent.Detach(ICharacter actor, out string reason)` (line 104; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WeaponCarrierAttachmentGameItemComponent.Recover(ICharacter actor, out string reason)` (line 135; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WeaponCarrierAttachmentGameItemComponent.Release(ICharacter actor, out string reason)` (line 160; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |

### [MudSharpCore/GameItems/Components/WirelessModemGameItemComponent.cs](../../MudSharpCore/GameItems/Components/WirelessModemGameItemComponent.cs)

| Function | Change |
| --- | --- |
| `WirelessModemGameItemComponent.CanConnect(ICharacter? actor, IConnectable other)` (line 201; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `WirelessModemGameItemComponent.Connect(ICharacter? actor, IConnectable other)` (line 213; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WirelessModemGameItemComponent.WhyCannotConnect(ICharacter? actor, IConnectable other)` (line 243; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |
| `WirelessModemGameItemComponent.CanDisconnect(ICharacter actor, IConnectable other)` (line 260; changed) | Adds the shared manual and item-access guard before the existing feasibility checks, including separately manipulated endpoints/tools passed by this operation. |
| `WirelessModemGameItemComponent.Disconnect(ICharacter actor, IConnectable other)` (line 270; changed) | Rechecks the shared manual and item-access guard at runtime entry before resource use or mutation; established null-actor system paths remain available. |
| `WirelessModemGameItemComponent.WhyCannotDisconnect(ICharacter actor, IConnectable other)` (line 300; changed) | Returns the shared manual/access failure before the existing operation-specific diagnostic. |

### [MudSharpCore/GameItems/GameItem.cs](../../MudSharpCore/GameItems/GameItem.cs)

| Function | Change |
| --- | --- |
| `GameItem.Merge(IGameItem otherItem)` (line 2352; changed) | Notifies the display shop after a successful stack, currency or commodity merge so the absorbed pile is no longer counted as missing stock. |
| `GameItem.NotifyStockItemMerge(IGameItem absorbed)` (line 2388; added) | Notifies the exact shared shop/merchandise for an absorbed display item, preserving the distinction between merging goods and losing goods. |
| `GameItem.GetByWeight(IBody getter, double weight)` (line 2617; changed) | Creates a real state-preserving commodity copy, transfers display metadata, debits only the requested source weight and performs the established load/get lifecycle. |
| `GameItem.CopyStockDisplayToSplit(IGameItem split)` (line 2641; added) | Copies each exact shop/merchandise display marker to a real split and notifies the shop to preserve its stock index without inventing a stock transaction. |
| `GameItem.PeekSplitByWeight(double weight)` (line 2653; changed) | Creates a temporary commodity copy with only the preview weight changed; avoids registering/saving a real item during eligibility checks. |

### [MudSharpCore/GameItems/Inventory/TemplateOutfit.cs](../../MudSharpCore/GameItems/Inventory/TemplateOutfit.cs)

| Function | Change |
| --- | --- |
| `TemplateOutfit.Materialise(ICharacter target, string outfitNameOverride = null, string loadArguments = null)` (line 1019; changed) | Uses explicit external wear/sheath fit and execution paths when materialising an outfit, preserving fallback placement. |
| `TemplateOutfit.ResolveSheath(IOutfitTemplateItem templateItem, Dictionary<string, IGameItem> createdItems, ICharacter target, IGameItem item)` (line 1217; changed) | Uses explicit external wear/sheath fit and execution paths when materialising an outfit, preserving fallback placement. |

### [MudSharpCore/GameItems/ItemManipulationGuard.cs](../../MudSharpCore/GameItems/ItemManipulationGuard.cs)

| Function | Change |
| --- | --- |
| `ItemManipulationGuard.CanManipulate(ICharacter? actor, out string reason, params IGameItem?[] items)` (line 14; added) | Combines current manual capability with CanManipulateItem for every supplied endpoint; preserves null-actor system operations, bounded telekinesis and same-body internal neural controls. |
| `ItemManipulationGuard.BeginTelekineticOperation(ICharacter actor, Func<IGameItem, bool> eligible)` (line 67; added) | Creates an internal actor-specific scope with an eligibility callback; ordinary callers cannot request this bypass. |
| `ItemManipulationGuard.RestoreScope.Dispose()` (line 76; added) | Restores the previous telekinetic scope when the operation ends, including nested scopes and exceptions. |

### [MudSharpCore/Health/Surgery/SurgicalProcedure.cs](../../MudSharpCore/Health/Surgery/SurgicalProcedure.cs)

| Function | Change |
| --- | --- |
| `SurgicalProcedure.PerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments)` (line 75; changed) | Requires the surgeon to have a working manipulator in Can/Why and revalidates procedure eligibility at execution; staged checks do not rerun already-consumed inventory plans. |
| `SurgicalProcedure.CanPerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments)` (line 101; changed) | Requires the surgeon to have a working manipulator in Can/Why and revalidates procedure eligibility at execution; staged checks do not rerun already-consumed inventory plans. |
| `SurgicalProcedure.WhyCannotPerformProcedure(ICharacter surgeon, ICharacter patient, params object[] additionalArguments)` (line 169; changed) | Requires the surgeon to have a working manipulator in Can/Why and revalidates procedure eligibility at execution; staged checks do not rerun already-consumed inventory plans. |

### [MudSharpCore/Magic/TelekineticManipulation.cs](../../MudSharpCore/Magic/TelekineticManipulation.cs)

| Function | Change |
| --- | --- |
| `TelekineticManipulation.TryPrepare(ICharacter actor, IGameItem item, string operation, StringStack arguments, Func<IGameItem, bool> eligible, out Func<bool> execute, out string error)` (line 22; changed) | Wraps preparation and execution in the scoped telekinetic eligibility context; execution reparses saved arguments and checks current eligibility again. |
| `TelekineticManipulation.TryPrepareCore(ICharacter actor, IGameItem item, string operation, StringStack arguments, Func<IGameItem, bool> eligible, out Func<bool> execute, out string error)` (line 42; added) | Wraps preparation and execution in the scoped telekinetic eligibility context; execution reparses saved arguments and checks current eligibility again. |

### [MudSharpCore/Vehicles/VehicleHitchGraphService.cs](../../MudSharpCore/Vehicles/VehicleHitchGraphService.cs)

| Function | Change |
| --- | --- |
| `VehicleHitchGraphService.CanAddVehicleTowLink(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint, IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, out string reason)` (line 236; changed) | Checks the acting manipulator, vehicle exteriors and any separate hitch item before admitting the attachment. |
| `VehicleHitchGraphService.CanAddCharacterVehicleHitch(ICharacter actor, ICharacter source, IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, IDragAid? dragAid, out string reason)` (line 357; changed) | Checks the acting manipulator, vehicle exteriors and any separate hitch item before admitting the attachment. |

### [MudSharpCore/Vehicles/VehicleOperationalReadinessService.cs](../../MudSharpCore/Vehicles/VehicleOperationalReadinessService.cs)

| Function | Change |
| --- | --- |
| `VehicleOperationalReadinessService.CanPerformAction(IVehicle vehicle, ICharacter actor, VehicleOperationalAction action, out VehicleAccessPermissionResult result)` (line 53; changed) | Requires manual capability for control, service, repair and hitch permissions, including administrators; boarding and actorless automatic operation retain their distinct rules. |

### [MudSharpCore/Vehicles/VehicleSystemInstances.cs](../../MudSharpCore/Vehicles/VehicleSystemInstances.cs)

| Function | Change |
| --- | --- |
| `VehicleInstallation.CanInstall(ICharacter actor, IGameItem item, out string reason)` (line 353; changed) | Checks current manual capability and access to both module and vehicle exterior after structural/mount/access checks; Install/Remove already call these gates. |
| `VehicleInstallation.CanRemove(ICharacter actor, out string reason)` (line 442; changed) | Checks current manual capability and access to both module and vehicle exterior after structural/mount/access checks; Install/Remove already call these gates. |

### [MudSharpCore/Work/Butchering/RaceButcheryProfile.cs](../../MudSharpCore/Work/Butchering/RaceButcheryProfile.cs)

| Function | Change |
| --- | --- |
| `RaceButcheryProfile.CanButcher(ICharacter butcher, IGameItem targetItem)` (line 249; changed) | Validates manual capability and corpse access before building the butchery inventory plan; diagnostic path returns the shared failure. |
| `RaceButcheryProfile.WhyCannotButcher(ICharacter butcher, IGameItem targetItem)` (line 286; changed) | Validates manual capability and corpse access before building the butchery inventory plan; diagnostic path returns the shared failure. |

### [MudSharpCore/Work/Crafts/Craft.cs](../../MudSharpCore/Work/Crafts/Craft.cs)

| Function | Change |
| --- | --- |
| `Craft.BeginCraft(ICharacter character)` (line 1201; changed) | Calls the existing CanDo/CanResumeCraft check inside Begin/Resume execution; preserves configured tool plans and does not impose hands on every authored craft. |
| `Craft.ResumeCraft(ICharacter character, IActiveCraftGameItemComponent active)` (line 1281; changed) | Calls the existing CanDo/CanResumeCraft check inside Begin/Resume execution; preserves configured tool plans and does not impose hands on every authored craft. |

## Regression tests and fixture changes

The new regressions cover missing/unusable/nonhuman manipulators, actual severing/restraint checks, delayed hand loss, nested/guarded/mounted/planar reach, direct mutation denial, neural and telekinetic boundaries, breath operation, source-before-split validation, reserved currency and shop split conservation. Existing fixtures now supply anatomy and reach where they exercise a physical actor; their original behavioural assertions were retained.

### [MudSharpCore Unit Tests/CombatActionAvailabilityTests.cs](../../MudSharpCore%20Unit%20Tests/CombatActionAvailabilityTests.cs)

| Function | Change |
| --- | --- |
| `CombatActionAvailabilityTests.CanFire_MatchlockWithoutCurrentWeather_ReturnsTrue()` (line 110; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/ConditionMaintenanceTests.cs](../../MudSharpCore%20Unit%20Tests/ConditionMaintenanceTests.cs)

| Function | Change |
| --- | --- |
| `ConditionMaintenanceTests.CreateActor(IFuturemud gameworld)` (line 290; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/ExplosiveTriggerTests.cs](../../MudSharpCore%20Unit%20Tests/ExplosiveTriggerTests.cs)

| Function | Change |
| --- | --- |
| `ExplosiveTriggerTests.CreateActor()` (line 367; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/ForagingRuntimeTests.cs](../../MudSharpCore%20Unit%20Tests/ForagingRuntimeTests.cs)

| Function | Change |
| --- | --- |
| `ForagingRuntimeTests.Forage_DiscreteItemAndCommodityOutput_RejectsFractionalYield(bool commodityOutput)` (line 870; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/GridSystemTests.cs](../../MudSharpCore%20Unit%20Tests/GridSystemTests.cs)

| Function | Change |
| --- | --- |
| `GridSystemTests.CreateCharacter(IFuturemud gameworld, long id, ICell location)` (line 1850; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/ItemSplitAccessTests.cs](../../MudSharpCore%20Unit%20Tests/ItemSplitAccessTests.cs)

| Function | Change |
| --- | --- |
| `ItemSplitAccessTests.CommodityPreview_IsTemporaryAndDoesNotChangeStockOrRegisterItems()` (line 22; added) | Checks real split/preview behaviour, source quantity or weight, retained shop identity, and stock-split notification (previews must not register or save items). |
| `ItemSplitAccessTests.CommoditySplit_PreservesShopStockIdentityAndOnlyDebitsRequestedWeight()` (line 40; added) | Checks real split/preview behaviour, source quantity or weight, retained shop identity, and stock-split notification (previews must not register or save items). |
| `ItemSplitAccessTests.StackSplit_PreservesShopStockIdentityOnBothPortions()` (line 61; added) | Checks real split/preview behaviour, source quantity or weight, retained shop identity, and stock-split notification (previews must not register or save items). |
| `ItemSplitAccessTests.RepeatedCommoditySplits_MergeNotifiesTheShopAndConservesWeight()` (line 78; added) | Checks real split/preview behaviour, source quantity or weight, retained shop identity, and stock-split notification (previews must not register or save items). |
| `ItemSplitAccessTests.CreateItem()` (line 95; added) | Builds real GameItem/component fixtures with shop display metadata for split regressions. |
| `ItemSplitAccessTests.Components(GameItem item)` (line 104; added) | Builds real GameItem/component fixtures with shop display metadata for split regressions. |
| `ItemSplitAccessTests.AddCommodity(GameItem item)` (line 108; added) | Builds real GameItem/component fixtures with shop display metadata for split regressions. |
| `ItemSplitAccessTests.AddShopDisplay(GameItem item)` (line 118; added) | Builds real GameItem/component fixtures with shop display metadata for split regressions. |

### [MudSharpCore Unit Tests/OutfitTemplateTests.cs](../../MudSharpCore%20Unit%20Tests/OutfitTemplateTests.cs)

| Function | Change |
| --- | --- |
| `OutfitTemplateTests.MaterialiseWornPlacementWithoutProfileUsesCreatedItemDefaultProfile()` (line 252; changed) | Updates mock expectations to the explicit external wear/sheath APIs; preserves outfit placement and failure assertions. |
| `OutfitTemplateTests.MaterialiseWornPlacementRemovesTemporaryRoomAnchor()` (line 270; changed) | Updates mock expectations to the explicit external wear/sheath APIs; preserves outfit placement and failure assertions. |
| `OutfitTemplateTests.MaterialiseFailedWornPlacementLeavesTemporaryRoomAnchorWhenNotHeld()` (line 309; changed) | Updates mock expectations to the explicit external wear/sheath APIs; preserves outfit placement and failure assertions. |
| `OutfitTemplateTests.MaterialiseSheathedPlacementUsesAvailableSheath()` (line 407; changed) | Updates mock expectations to the explicit external wear/sheath APIs; preserves outfit placement and failure assertions. |

### [MudSharpCore Unit Tests/PhysicalManipulationTestHelper.cs](../../MudSharpCore%20Unit%20Tests/PhysicalManipulationTestHelper.cs)

| Function | Change |
| --- | --- |
| `PhysicalManipulationTestHelper.SetUpUsableHands(Mock<ICharacter> actor, Mock<IBody>? existingBody = null)` (line 15; added) | Provides an explicit usable manipulator and material-plane presence for existing component fixtures; existing behavioural assertions remain in force. |

### [MudSharpCore Unit Tests/PhysicalManipulationTests.cs](../../MudSharpCore%20Unit%20Tests/PhysicalManipulationTests.cs)

| Function | Change |
| --- | --- |
| `PhysicalManipulationTests.ManualAction_NoManipulators_IsRejected()` (line 33; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.ManualAction_UnusableManipulator_IsRejected(CanUseBodypartResult failure)` (line 41; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.ManualAction_OneWorkingNonhumanManipulator_IsEnoughEvenWhenOccupied()` (line 58; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Body_CanUseBodypart_RejectsASeveredPartEvenWithoutALimbEffect()` (line 67; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Body_CanUseBodypart_PropagatesLimbRestraints()` (line 74; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_NestedClosedContainer_IsRejectedBeforeRoomAccess()` (line 97; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_GuardedAncestor_IsRejected()` (line 117; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_ClosedDoorExternalLock_RemainsReachableFromEitherSide()` (line 129; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_OtherInventory_RequiresPermissionAndProximity()` (line 150; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_DifferentLayerOrCell_AndContainmentCycle_AreRejected()` (line 166; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Guard_NullActor_IsReservedForSystemOperations()` (line 182; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Guard_NeuralControl_OnlyExemptsInternalImplantsInstalledInTheActor()` (line 188; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Reach_MountedModule_RequiresAccessibleHousingAndHostLocation()` (line 206; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Inventory_GetWithNoHands_RejectsBeforeConsideringStackMerging()` (line 226; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Inventory_WeightRetrieval_DeniedSourceNeverSplitsOrTakes()` (line 236; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Inventory_RoomWeightRetrieval_GuardedSourceNeverSplits()` (line 255; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.CurrencyRetrieval_OriginalPilePickupRestriction_PreventsConsumption(bool fromContainer)` (line 269; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Inventory_QuantityRetrieval_PreviewCannotBypassOriginalPickupRestriction()` (line 319; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Blowgun_HandlessActorCanFireButAnUnusableMouthCannot()` (line 343; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Switch_DirectRuntimeCall_RejectsHandLossAndLostAccessWithoutMutation()` (line 364; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.Telekinesis_BypassesHandsOnlyInsideEligibleOperation_AndRechecksBeforeExecution()` (line 385; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.MedicalContinuation_HandLossStopsBeforeTreatment()` (line 407; added) | Adds the regression named by this function; denied operations are checked before their relevant mutation, with positive exceptions covered separately. |
| `PhysicalManipulationTests.ActorWithHand()` (line 419; added) | Creates the minimum real-body or mocked world/anatomy fixture for the new physical-access regressions. |
| `PhysicalManipulationTests.Item(Mock<IFuturemud> world, Mock<ICell>? cell = null)` (line 439; added) | Creates the minimum real-body or mocked world/anatomy fixture for the new physical-access regressions. |
| `PhysicalManipulationTests.EmptyBody()` (line 449; added) | Creates the minimum real-body or mocked world/anatomy fixture for the new physical-access regressions. |
| `PhysicalManipulationTests.SetField(Body body, string name, object value)` (line 461; added) | Creates the minimum real-body or mocked world/anatomy fixture for the new physical-access regressions. |

### [MudSharpCore Unit Tests/ScribingWritingComponentsTests.cs](../../MudSharpCore%20Unit%20Tests/ScribingWritingComponentsTests.cs)

| Function | Change |
| --- | --- |
| `ScribingWritingComponentsTests.InscribableSurface_UsesConfiguredImplementTypesAndCapacity()` (line 90; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/SealAndMeasurementComponentTests.cs](../../MudSharpCore%20Unit%20Tests/SealAndMeasurementComponentTests.cs)

| Function | Change |
| --- | --- |
| `SealAndMeasurementComponentTests.Instrument_ActorAlreadyPlayingAnotherInstrument_IsRejected()` (line 291; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `SealAndMeasurementComponentTests.CreateActor(IFuturemud gameworld, long id)` (line 1127; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `SealAndMeasurementComponentTests.CreateInstrumentActor( IFuturemud gameworld, Mock<IGameItem> instrument)` (line 1140; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/ShopTests.cs](../../MudSharpCore%20Unit%20Tests/ShopTests.cs)

| Function | Change |
| --- | --- |
| `ShopTests.LoseFromStock_PartialCommodity_RecordsWeightLossAndKeepsResidualPileCount()` (line 360; added) | Regresses commodity split/loss accounting: residual pile count, weight-based value, idempotent split registration and absence of synthetic stock transactions. |
| `ShopTests.BuyCommodityWeight_WithAnotherDisplayedSplit_DoesNotInventStockTransactions()` (line 388; added) | Regresses commodity split/loss accounting: residual pile count, weight-based value, idempotent split registration and absence of synthetic stock transactions. |
| `ShopTests.RegisterStockItemMerge_RepeatedCommodityPickupDoesNotInventStockLoss()` (line 426; added) | Regresses commodity split/loss accounting: residual pile count, weight-based value, idempotent split registration and absence of synthetic stock transactions. |
| `ShopTests.TestShop.RecordedStockCount(IMerchandise merchandise)` (line 825; added) | Exposes the existing protected count to assert conservation of stock through splitting, buying and loss. |
| `ShopTests.TestShop.ReconcileStock(IMerchandise merchandise)` (line 826; added) | Exposes the existing protected count to assert conservation of stock through splitting, buying and loss. |

### [MudSharpCore Unit Tests/SignalAutomationTests.cs](../../MudSharpCore%20Unit%20Tests/SignalAutomationTests.cs)

| Function | Change |
| --- | --- |
| `SignalAutomationTests.Keypad_Select_OnlyEmitsSignalForCorrectPoweredCode()` (line 1080; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `SignalAutomationTests.BiometricScanner_HeldSeveredPartContainingShape_UsesOriginalIdentity()` (line 1210; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/VehicleEngineInstallationTests.cs](../../MudSharpCore%20Unit%20Tests/VehicleEngineInstallationTests.cs)

| Function | Change |
| --- | --- |
| `VehicleEngineInstallationTests.CanInstall_MatchingEngineFormFactor_AcceptsCompatibleModule()` (line 28; changed) | Retains the compatible installation success case and adds denial after target access or manipulator loss. |
| `VehicleEngineInstallationTests.CreateInstallation(string mountType)` (line 47; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/VehicleMovementCommandTests.cs](../../MudSharpCore%20Unit%20Tests/VehicleMovementCommandTests.cs)

| Function | Change |
| --- | --- |
| `VehicleMovementCommandTests.VehicleMovement_InitialAction_BeginsTransitAndSchedulesDelayedMove()` (line 261; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `VehicleMovementCommandTests.CreateVehicleAccessCommandContext(bool accessOpen)` (line 474; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/VehicleMovementStrategyTests.cs](../../MudSharpCore%20Unit%20Tests/VehicleMovementStrategyTests.cs)

| Function | Change |
| --- | --- |
| `VehicleMovementStrategyTests.CreateVehicle(ICharacter controller, IEnumerable<VehicleMovementProfileType> movementTypes, SizeCategory exteriorSize, VehicleMovementEnvironment environment = VehicleMovementEnvironment.Unrestricted)` (line 669; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/VehicleOperationalReadinessServiceTests.cs](../../MudSharpCore%20Unit%20Tests/VehicleOperationalReadinessServiceTests.cs)

| Function | Change |
| --- | --- |
| `VehicleOperationalReadinessServiceTests.CanPerformAction_ManualOperation_RechecksManipulatorLoss(VehicleOperationalAction action)` (line 25; added) | Covers control/service/repair/hitch denial after hand loss and the boarding exemption. |
| `VehicleOperationalReadinessServiceTests.CreateCharacter(long id)` (line 866; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |

### [MudSharpCore Unit Tests/VehicleTowServiceTests.cs](../../MudSharpCore%20Unit%20Tests/VehicleTowServiceTests.cs)

| Function | Change |
| --- | --- |
| `VehicleTowServiceTests.Move_WithThreeVehicleTrain_MovesAllVehicles()` (line 503; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `VehicleTowServiceTests.Move_WithInvalidTowTrain_BlocksBeforePowerConsumption()` (line 532; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
| `VehicleTowServiceTests.CreateActor()` (line 847; changed) | Supplies the usable anatomy, planar presence, reachable target or exterior item now required by the runtime path; preserves the original test assertions. |
