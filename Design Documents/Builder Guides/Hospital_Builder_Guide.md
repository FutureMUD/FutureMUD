---
slug: hospitals
category: Health and environment
summary: Build a modern hospital with staffed treatment, surgery, recovery and blood services, or a witch healer's cottage with basic surgery and a retail apothecary.
---

# Building hospitals

A hospital sells **care**. A patient requests a service, an employment task assigns the work, and a medical worker uses the game's ordinary wound or surgery system. A shop sells **items**. A person who buys a dressing owns a dressing. A person who pays for binding is asking someone to use one. The distinction matters when you decide which room, employee and stockpile should fulfil each promise.

The first example builds a general hospital. The second builds a witch healer who tends and closes wounds, fits a carved prosthesis and sells remedies over the counter. Both use the treatment items and surgical authoring commands from [Building the health system](Health_System_Builder_Guide.md).

## Harbour General, a modern hospital

### Establish reception, theatre, stores and recovery

As an administrator, stand in reception. Connect reception to a theatre, supply store, recovery ward and staff room. Give the recovery ward a bed suitable for your patients. Use a configured economic zone and a bank account in its currency, or enter `none` to use the hospital's virtual cash ledger. Replace angle-bracket fields throughout. Prices such as `10cr` below assume a currency that accepts credits in that form. Substitute your zone's real amount format and quote multiword amounts.

```text
hospital create "Harbour General" <economic-zone-id> <account-or-none>
hospital close
hospital room add theatre <theatre-room-id>
hospital room add supply <supply-room-id>
hospital room add recovery <recovery-room-id>
hospital room add staff <staff-room-id>
hospital info
```

Creation registers reception as a waiting room and starts the hospital trading. Closing it during setup lets you open a complete service for the first patient. Room roles can overlap. Waiting rooms advertise services, theatres can be reserved for procedures, supply rooms hold physical stock, and recovery rooms receive patients after routed treatment. Check door access from reception to each room as an ordinary character.

### Hire the doctor and the orderly

Use two able NPCs. Give the doctor the knowledge and skill for the procedures you intend to offer. For this example the doctor should know a `DetailedExamination` plus the `Clinic Exploration` and `Clinic Closure` pair from the health guide. Use `giveknowledge <doctor> <knowledge-id>` as staff and test the procedures directly before offering them to patients. An orderly can gather and stage supplies without learning to operate.

Create two worker AIs. A fresh `EmploymentWorker` has tasking enabled and can deliver items. Each `ai set capability` command **toggles** its capability, so inspect existing AIs before changing them. Attach them to nearby loaded NPCs.

```text
ai edit new employmentworker HarbourDoctor
ai set host hospital
ai set currency <zone-currency>
ai set capability doctor
ai show
ai close
ai add HarbourDoctor <doctor-npc>
ai edit new employmentworker HarbourOrderly
ai set host hospital
ai set currency <zone-currency>
ai set capability orderly
ai show
ai close
ai add HarbourOrderly <orderly-npc>
```

Stand in a hospital room with both NPCs. As an administrator or manager with hiring authority, make the contracts and inspect the roster.

```text
hospital employ <doctor-npc>
hospital orderly <orderly-npc>
hospital contracts
hospital tasks diagnose
```

These commands immediately create active, unpaid, indefinite contracts with an any-time schedule. Check for medical-service authority on the doctor and supply-preparation authority on the orderly. Each also needs the corresponding AI capability. For paid recruitment, use `hospital openings create MedicalWorker <hourly-wage> 1` or `HospitalOrderly`, inspect `hospital applications` and accept the relevant application. Configure the AI on NPC templates when future spawned staff should inherit it. [Employment hosts](Employment_Hosts_Builder_Guide.md) · [NPC builder guide](NPC_Builder_Guide.md)

### Build the service menu and stock the rooms

Create the following services as a manager at the hospital. The first field is the treatment family, the second is one currency amount, and the rest is the name patients will see. Use quotation marks when selecting an existing multiword name. Ordinary binding, cleaning, closing and tending call bedside treatment. A surgical family such as `explore` can select a compatible procedure, or you can bind the exact procedure as below.

```text
hospital service add bind 10cr Trauma binding
hospital service add clean 8cr Wound cleaning
hospital service add close 15cr Wound closing
hospital service add tend 12cr Wound tending
hospital service add relocate 20cr Bone relocation
hospital service add triage 5cr Triage
hospital service add detailed 12cr Medical examination
hospital service add explore 120cr Exploratory procedure
hospital service add finalise 35cr Surgical closure
hospital service set "Exploratory procedure" procedure "Clinic Exploration"
hospital service set "Surgical closure" procedure "Clinic Closure"
hospital service "Surgical closure"
```

New services are active and available both individually and as components of combined treatment. `hospital service set "<service>" offering combined` makes a step available only inside combined care. Use `standalone` for individual requests only, or `both` to restore both uses. The game supplies `Stabilisation` and `Full Treatment` automatically. Configure and price the component services they may perform. Surgical-family services default to theatre preference and recovery routing. Inspect those flags before toggling `theatre` or `recovery`.

| Where | Stock for this example | What to check |
| --- | --- | --- |
| Supply store | Loaded Trauma dressings, cleaners, Close packets and Tend supplies | The component must support the named treatment and have uses left. The orderly needs access to both stores and theatre. |
| Theatre | An instrument and two disposable packs matching the exploration and closure **phase tags** | Each procedure uses a pack. Stage a spare where the worker can obtain it for the second request. |
| Recovery | A usable bed and an accessible route back to reception | Expect a patient whose service routes to recovery to arrive here. Waiting-room fallback is available when a recovery room is absent. |

If the service itself should require a separate reusable tool or disposable supply, add an equipment selector. A prototype ID selects copies of that prototype, `&<tag>` selects a category and `*<item-id>` selects a particular live item for reusable equipment. Keep an additional service consumable distinct from a pack already consumed by a procedure phase, or provide enough stock for both.

```text
hospital service set "Exploratory procedure" equipment add 1 reusable <extra-tool-prototype-id>
hospital service set "Exploratory procedure" equipment show
```

Set up anaesthesia for procedures that require it, including closure. Pick a game drug with the `Anesthesia` effect and the `Injected` vector. When no cannulation procedure is assigned, the worker can administer the configured drug directly. For an IV workflow, choose a `Cannulation` procedure the doctor knows, provide its phase tools and stage compatible cannula, drip and a prepared IV bag. The numeric setting below is the game's target intensity.

```text
hospital service set "Surgical closure" anesthesia <drug-id> 1.25
hospital service set "Surgical closure" anesthesia show
```

For the IV version, add `hospital service set "Surgical closure" cannulation <cannulation-procedure-id>`. Test direct care first, then repeat with the IV route and inspect the supplies actually used.

### Add blood collection and transfusion

Set up a collection service that **pays** donors and a transfusion service that **charges** patients. Put empty IV bags with usable space in stores for collection. For transfusion, provide compatible, filled bags and check the actual available volume. Supply the drip, cannula and access procedures as well. A target stock level alone creates no liquid.

```text
hospital service add donation 10cr Blood donation
hospital service add transfusion 20cr Blood transfusion
hospital service set "Blood donation" cannulation <cannulation-procedure-id>
hospital service set "Blood transfusion" cannulation <cannulation-procedure-id>
hospital bloodstock set <blood-type> target 2
hospital bloodstock set <blood-type> price 10cr
hospital bloodstock show
hospital deposit <working-cash-amount>
hospital cash
```

The example targets two litres of the selected game blood type. The policy price is the donor rate per litre below target. Give the depositor the cash and verify the hospital can fund a donation. Have an eligible test donor request collection at reception. Watch for an actual filled bag returning to supply storage and a payout after successful collection. For a separate transfusion test, inspect compatible stock before and after the request. An empty bag is useful for one service and explains a shortage for the other. [Blood and hospital workflows](../Health/Health_Medical_Interactions.md#economy-mediated-hospital-requests)

### Open the doors and follow one patient

Run `hospital open` as a manager. Bring the idle doctor and a test patient to reception for the first request. Choose a patient with the body type and exposed or accessible target site configured for both operations. Use a separate character with the hospital's currency to test public commands.

1. As the patient, run `hospital services`, `hospital service "Medical examination"` and `hospital request "Medical examination" cash`. A request should acquire an ID and enter the task queue. A visible, conscious third-party patient may need to accept the request.
2. As the manager, use `hospital requests`, `hospital requestshow <id>` and `hospital operations`. Follow the assigned worker, theatre reservation, staged stock, patient movement and recovery. Let this request finish before starting the next one.
3. As the patient, use `hospital request "Exploratory procedure" target <bodypart> cash`. Check the examination result and the outstanding closure requirement. Once the service finishes, bring doctor and patient together for `hospital request "Surgical closure" target <same-bodypart> cash`. Confirm completion, patient location and remaining packs.
4. As the cashier, inspect `hospital ledger` and `hospital debt`. For a combined-care test, use `hospital debt pay <amount> cash` to create prepaid credit or configure an appropriate `hospital maxdebt <amount>`. Request `hospital request "Full Treatment" debt` on an eligible test patient. Compare the component work performed with the amounts charged. Combined care is usage-billed, and cancelling a request can retain charges for phases already started.

To test a failure, remove one required tool from all accessible hospital stock before a new request. Read the availability reason, restore the tool and repeat. `hospital failures` records failed requests, and `hospital tasks diagnose` explains which employees can claim work. A menu entry tells you what the hospital offers. The request, task, patient's condition and ledger together tell you what it delivered.

## The hedge witch's cottage and remedy shop

The cottage needs one healer, one public counter, a private treatment room and a supply cupboard. Give it a small menu of wound care and one **prosthetic-fitting procedure**. A fitted carved hand is a concrete surgical service a low-technology healer can provide without an IV system or follow-up invasive closure. The shelves can sell both a topical pain remedy and a specific antidote. Keep one set of loaded items for treatment and a separate set for sale.

### Offer bedside care and a simple operation

Create a hospital from the counter. In this plan the treatment room also serves as recovery and the healer waits near the public counter between patients. Attach a fresh hospital worker AI to the witch using the doctor example above and grant the `doctor` capability. For this one-worker example, also enable `orderly` on her AI. A medical worker's contract grants authority to deliver care. Grant supply preparation separately so she can fetch her own supplies. Staging all supplies in the treatment room keeps the initial one-worker test simple.

```text
hospital create "Hedge Witch" <economic-zone-id> none
hospital close
hospital room add staff here
hospital room add theatre <treatment-room-id>
hospital room add recovery <treatment-room-id>
hospital room add supply <cupboard-room-id>
hospital employ <witch-npc>
hospital contracts
hospital contracts delegate <witch-contract-id> grant PrepareMedicalSupplies
hospital contracts delegate <witch-contract-id> show
hospital service add bind 3cr Linen binding
hospital service add close 5cr Stitching a wound
hospital service add tend 4cr Herbal aftercare
hospital service add prosthetic 12cr Carved hand fitting
```

Make a functional wooden prosthetic for a specific sever location as described in the health guide. Give its live item a distinctive keyword such as `carvedhand`. For the witch, choose an existing medical knowledge or create the procedure against a knowledge she can learn. Configure a held fitting tool with an item tag and place a matching tool in reach.

```text
surgery edit new InstallProsthetic <body-id> <knowledge-id> "Hedge Medicine" "Hedge Hand Fitting" "fitting"
surgery set trait <surgery-skill-id>
surgery set phase add 12 @ check|checks the fit for $1.
surgery set phase 1 held <fitting-tool-tag>
surgery edit
surgery close
giveknowledge <witch-npc> <knowledge-id>
hospital service set "Carved hand fitting" procedure "Hedge Hand Fitting"
hospital open
```

Use a living test patient with the matching visible sever and no prosthesis at that location. The fitter must **already hold the prosthesis** and have a free hand for the separate fitting tool. Bring both characters to the counter and make sure the witch still holds the device before the patient runs `hospital request "Carved hand fitting" params carvedhand cash`. The argument identifies the device in the fitter's hands. The hospital cannot infer that choice from bags in the cupboard. Check `hospital operations`, then the patient's fitted limb and recovery location. An intact limb or a device left with the customer gives you a useful negative test. For everyday patients, `hospital request "Stitching a wound" cash` uses the ordinary Close treatment on a suitable stabilised wound.

### Open the apothecary counter

Stand at the public counter and create a permanent shop in the same economic zone. If the cottage is a property, give the intended owner or leaseholder the opportunity to claim the shop. Use the health guide to build three approved holdable prototypes for the shop. Stock `moonleaf_salve`, a jar with the topical analgesic, `mothberry_remedy`, a pill carrying the named antidote, and `clinic_dressing`, a Trauma dressing. Give their live items the keywords `salve`, `remedy` and `dressing` respectively.

```text
shop create "Hedge Witch Remedies" <economic-zone-id>
shop merch new salve moonleaf_salve 4cr
shop merch close
shop merch new remedy mothberry_remedy 6cr
shop merch close
shop merch new dressing clinic_dressing 2cr
shop merch close
item load <salve-prototype-id>
shop stock salve
item load <remedy-prototype-id>
shop stock remedy
item load <dressing-prototype-id>
shop stock dressing
shop merch list
```

Place the stocked items on the retail floor or in the shop's designated stockroom. A new permanent shop starts trading. As a customer, `list`, `buy salve` and `apply salve <character> <bodypart>`. Test `buy remedy` and `swallow remedy` on a character affected by the fictional Mothberry Blight. Confirm the shop's stock count fell and that an independently stocked hospital dressing is still available for a patient. Shop restocking rules replenish merchandise and the hospital supply workflow stages its own stock. [Shop setup and stocking](Shop_and_Banking_Builder_Guide.md)

## When a request stalls

| What you see | Where to look |
| --- | --- |
| The counter lists no usable care | Stand in the flagged waiting room, inspect `hospital info` and the service's active and offering settings. Check that the hospital is open. |
| “No medical employee available” | Check the active contract, medical authority, hospital AI capability, tasking flag, current worker state and other assignments. |
| Tools exist somewhere in the building | Check the exact prototype or tag, remaining consumable quantity, room role, worker access, theatre reservations and an eligible employee for supply staging. |
| The surgeon knows the procedure but a request fails | Check body type, target arguments, eligible injury or missing part, medical knowledge, reachable phase tools, and anaesthesia or recovery requirements. Compare with performing the procedure directly. |
| A payer is refused or a cancellation retains a charge | Inspect prepaid credit, the debt ceiling and the request's billing history. Compare a pending request with one that has already received treatment. |
