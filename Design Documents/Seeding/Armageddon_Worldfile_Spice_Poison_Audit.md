# Armageddon worldfile spice and poison mapping

This document records the spice consumption tranche and the separate poison/cure source audit. Source references are relative to the public ArmSolo archive. No historical character records are imported.

## Spice consumption

The retained package contains 90 ITEM_SPICE definitions (type 25). Value 0 is remaining potency/charges, value 1 is the spell/skill identifier, value 2 distinguishes loose spice (0) from rolled spice (1), and value 3 carries a legacy poison index. There are 81 loose definitions and nine rolled definitions. Five definitions have obsolete/invalid skill identifiers: Vnum402, Vnum19544, Vnum19700, Vnum25806 and Vnum63022. Their data is retained without inventing effects. Names alone are not reliable: Vnum71000's blue moss uses Zharal's skill identifier.

The approved conversion defines one source potency unit as **1 mg of active native drug**. This is authored game calibration, not a recovered measurement. Native inhaled absorption, body weight and metabolism remain active. Intensity per gram is 70; relative metabolism is 0.0002 (Qel 0.0006), calibrated against a 70 kg body with liver removal of 0.01 kg/hour. This approximates 30 minutes per ordinary unit and 10 minutes per Qel unit before differences in absorption, sex/body metabolism and clearance. It does not promise identical source durations for every character.

| Source skill | Native effect |
| --- | --- |
| 141 Methelinoc | Wisdom bonus |
| 142 Melem Tuek | Strength bonus |
| 143 Krelez | Agility bonus |
| 144 Kemen | Maximum HP bonus |
| 145 Aggross | Endurance bonus |
| 146 Zharal | Offensive check bonus |
| 147 Thodeliv | Native analgesia; successful dosing ends Mul Rage and active/latent Thodeliv prevents its activation |
| 148 Krentakh | Approved native stamina-regeneration adaptation |
| 149 Qel | Mana capacity change; sign selected from the actor's ecology at consumption |

Item command progs implement `sniff <held spice>`, `smoke <held roll>` and `shave <held spice>`. Sniffing consumes an entire loose portion of at most five units. Smoking doses one unit and removes one charge, without lighting or continuous burning. Shaving requires at least five units and produces the canonical three-unit pinch, conserving potency. Native item creation runs that pinch's own onload progs. Failed delivery leaves the original quantity intact. The progs refuse consumption while asleep, incorporeal or in combat. They do not reproduce the source's punitive combat-attempt branch.

Saved item registers hold the remaining amount and initialization marker. Creation progs install conditional hooks; directly persisted opening shop stock receives matching register rows and hook links because database loading does not execute creation progs. Checkpoint power uses the remaining count, with static prototype values as fallback for uninitialized legacy stock. Rolled spice remains excluded from checkpoint detection, as in the source.

The approved sale policy refuses `sell` for partially used/shaved remnants. Fresh portions and newly shaved pinches remain tradable at native prototype prices. The source's dynamically reduced brick price and derived pinch price are not copied into native shops. `make smoke <paper or book> <spice> [written]` now creates the corresponding rolled prototype with the pinch's remaining potency and poison. Native blank sheets/book pages are preferred; the explicit `written` option permits deliberate consumption of a selected written page. Native book bindings survive exhaustion. Shorter-than-stock rolls retain the existing sale refusal; source per-instance price changes are not imported. Authored source pages await native script mapping and are not falsely treated as blank paper.

## Mul Rage

Natural rage is an involuntary racial response, not a player command. `npc.c:2530–2581` checks a fighting Mul once per `point_update`. `socket.c` runs that update every 1,200 quarter-second pulses: **five real minutes**. The inclusive roll is zero through `floor(remaining HP × 2000 / maximum HP)`; only zero proceeds to the Willpower resistance roll. Natural rage lasts up to **ten real minutes**, and ends earlier at zero stamina. The import translates the five-minute interval through the installed clock, using the same native schedule conversion as source weather updates. Later clock-rate edits can change that wall-clock interval.

The approved separate **Willpower skill** is distinct from any mental attribute. Its source index is 91, it is hidden/innate, and its cap is 100. `limits.c:init_psi_skills` initializes players at 5; imported NPCs retain the higher of their explicit skill and the source Wisdom learning floor, with the normal missing-skill baseline of 5. The controller initializes missing skills on loaded, awake physical characters without resetting existing values. Resistance and advancement use native normal-difficulty skill checks and the installed skill improver; the source's `gain_skill` implementation is not duplicated.

The source-specific controller applies the reusable, saved Combat Frenzy effect with +15 offensive and defensive check bonuses, +5 physical melee damage, and a fourfold hunger multiplier. The effect uses native rage/flee/truce rules, prevents sitting/lying/sleeping, enforces no mercy without changing the saved manual preference, and prevents **stun** unconsciousness while allowing pain unconsciousness and HP death. Its benefits apply only to the physical body that received it. Native movement stamina costs remain, as approved. Source regeneration's special hunger arithmetic is represented by normal native modifier composition. No separate source “daze” state is invented.

While a raging player is awake and out of combat, each five-minute update considers visible non-admin room characters in order, with a 25% roll per candidate and no clan/allied exemption. It starts ordinary native combat against the first selected target; native combat admission still applies. Source `ch_act_agg` does not redirect an ongoing fight and is not called for NPCs, so neither does the imported controller.

Thodeliv's successful consumption prog removes the frenzy immediately. `hasdrugdosage` checks both latent and active doses before later racial activation. This cancellation targets Combat Frenzy, preserving unrelated chemical rage effects and other drug doses.

The import also configures the installed, owned **Roused Fury** and **Still Anger** definitions through their optional native source profiles. Other profiles keep their existing behaviour by default. Fury retains its ordinary bonus and duration; an eligible, unsuppressed recipient additionally gains independent frenzy. Recasting adds the newly resolved Fury duration to the remaining frenzy duration, as source `affect_join(..., FALSE, FALSE)` does, so rage can outlast ordinary Fury and survives its dispelling. Calm clears the captured frenzy after Fury countering permits the cast to continue, before the residual Calm save is applied. Reflected casts use the actual admitted recipient. The existing prepared-casting checks capture the relevant body, suppression and frenzy state; callbacks cannot cause a replacement effect to be removed accidentally.

## Dependence and overdose

Source `spice.c` contains `spice_hit` (67–203), spice effects (398–540), sniff/smoke (1047–1174), roll manufacture and shaving (1189–1293), detox (1296–1383), withdrawal (1416–1456) and relapse growth (1460–1488). `limits.c` applies the withdrawal factor to HP, stun, Mana and movement caps. `handler.c` applies the trait and capacity modifiers.

The source grows dependence on use, doubles gain below the historical peak, penalizes attributes at ten-point thresholds (Krentakh twenty), and recovers one point every ninety real minutes after its detox delay. Severe withdrawal scales four capacities exponentially. There are restoration defects, including writing the wrong attribute and an unreachable Krentakh recovery condition.

The approved native adaptation keeps **active-drug-over-time dependence growth**, existing persistent exposure records and native withdrawal onset/recovery. Builder-configurable modifier steps express reversible penalties instead of permanently rewriting base traits. Imported steps use the source ten/twenty-point thresholds, Kemen's five-HP loss per step, and Zharal's defensive penalty. Relapse gain is doubled below the historical peak. Continuous exposure decay is sixteen points per real day; it does not copy the source's initial detox delay or discrete timer. Native historical-peak decay remains.

Severe withdrawal uses configurable bounded native penalties: up to half HP/Mana capacity and stamina regeneration, with increased stamina costs. This is an adaptation rather than the source's exponential reductions to all four reserves. Qel deliberately has neither dependence nor overdose because its source path bypasses `spice_hit`.

Flat HP changes apply before capacity multipliers, consistently with Mana. HP penalties retain a minimum maximum of one; a valid Mana reserve can fall to zero. This prevents a Kemen dependence loss and withdrawal multiplier from accidentally creating a zero HP maximum solely through modifier ordering. Existing wound and overdose death rules still apply.

Lethal overdose is a reusable native drug effect, configured here at 0.002 active grams per effective Endurance point. The eight ordinary spices share the approved `Armageddon Spice` overdose group; their combined active mass contributes to the same limit, while Qel is excluded. It invokes native death on the current physical body. Native toxicity evaluates active drug mass; Armageddon instead sums duration-derived spice load. These quantities are not exact equivalents, and native absorption delays the point at which a dose becomes active. Ungrouped native drugs retain independent overdose limits.

## Poison and cure audit — implementation gaps

The subsequent [poison-effects, delivery and cleansing tranches](Armageddon_Worldfile_Poison_Effects.md) create calibrated native profiles, staged symptoms, permanent onset/natural-expiry consequences and a native poison resistance check. Valid spice lacing, food/drink/herb doses, oral antidotes, poison-vial application and native single-use weapon/dart coatings are connected. Timed generic-poison HP-cap penalties, Heramide recovery suppression, Grishen exhaustion sleep and Peraine combat cessation are implemented. Character spells cure one ordered family per cast, Kelwin admits his explicit source families plus approved native venoms, and an item-targeted native spell cleanses coatings and food/drink payloads. Halfling healer eligibility remains magical-poison-only. Skellebain involuntary actions and plague transmission are implemented in the later symptom/infection tranches; their separate receipts remain authoritative.

This tranche does not activate all legacy poison/cure fields. The package contains 27 poison, 66 herb and 15 cure definitions.

| Source type/field | Actual source consumer and import status |
| --- | --- |
| Poison 27, value 0 | Legacy poison index. `constants.c:2672–2685`: none, generic, grishen, skellebain, methelinoc, terradin, peraine, heramide, plague, skellebain_2, skellebain_3. Recreational Methelinoc and poison methelinoc are distinct. Calibrated profiles and held-vial application progs are installed; malformed indices remain inert provenance. |
| Poison value 1 | Nominal amount; the audited application path does not use it as a measured dose. Do not treat it as grams. |
| Weapon value 5 plus poisoned flag | `skills.c:7673–7736` applies poison; `combat.c:4280–4297,4520–4533` consumes the flag on a hit. Imported initial/runtime coatings use a persistent native single-use delivery prog, uniform nominal 1–4 mg, native wound eligibility and native poison save. Native throwing shares that delivery path. Source type-24 darts now use native piercing thrown profiles rather than holdable fallback. |
| Food/spice/dart value 3; drink value 5 | Typed poison payloads. Food and drink/sip consumption have different amount-derived durations (`object.c:3146,3489,4633`). Transfers preserve drink poison. Approved conversion is 1 mg per ten-minute source duration unit. Native food uses proportional 2 mg per original food unit; drink uses 3 mg per 50 mL consumed. Spice uses random 1–2 units. Plague's fixed dose and duplicate refusal are native drug-wide policies. Vials replace food/spice payloads or native beverage lineages; darts use single-use hit delivery. Source shaving does not copy a brick's poison onto its generated pinch, which is preserved. |
| Herb 33, values 0–3 | Herb kind, edibility, colour and nominal poison. `object.c:3378–3407` only gives generic poison for edibility 2 and ignores value 3; ordinary builder values are 0/1. Do not activate the unused field as a typed toxin. |
| Cure 34, values 0–4 | Poison index, form (vial/tablet/syringe), side-effect poison, taste and nominal strength. Vials/tablets now use native oral consumption and purge active/latent doses; Skellebain indices cure the whole family as approved. Index zero cures nothing. Side-effect toxins are delivered without a save. Strength-reduction code is disabled. Vials leave available Vnum13248 (`object.c:3046–3072`); native wounds remain unhealed, preserving tablets' generic-poison limitation (`3294–3348`). No packaged syringe-form item exists. |
| Cure spell | `magick.c:2347–2411` tries an ordered poison list; two skellebain variants mistakenly remove the base effect. Item targets clear the relevant poison slot (`2413–2448`). Do not copy the erroneous variant removal into a new native mapping. |

`utility.c:840–972` establishes onset and default durations, while `limits.c:1175–1816` supplies nominal three-real-second poison updates. The detailed mapping records the approved dose calibration, peraine threshold, sleeping lethality and permanent Wisdom losses. Previously approved healers remain limited to the mapped magical toxin and explicit installed venom lists; recreational spice is not added merely because a source name overlaps.

## Verification boundary

Generated prog compilation, fixture stock state and focused native runtime tests are the automated verification surfaces. A complete installed-world consumption, persistence/restart, shop and healer playthrough requires separate live verification; source audit and successful compilation alone do not establish that result.

Final direct, serial `dotnet` verification passed 11 library tests, 249 core tests and 55 seeder tests. The core selection covers drug metadata/dependence/dosing, grouped overdose, capacity reconciliation, actual HP/stun/pain outcomes, physical melee bonuses, frenzy ownership/persistence/exhaustion, and Fury/Calm admission and callback cases. The seeder selection compiles every generated spice/rage prog, verifies opening stock, executes sale refusal, final-charge smoking and successful/failed shaving, and checks canonical Fury/Calm XML mapping alongside existing justice/healer/vendor fixtures. Relevant product builds passed; unchanged compiled dependencies were reused. Initial compile/fixture failures and the corrected Calm result-reporting failure are not claimed as passing runs. No fresh MySQL installation or live Telnet/restart proof was performed.
