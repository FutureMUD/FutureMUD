# Predator Hunting

## Ownership and compatibility

`AnimalAI` owns prey policy, observation, assessment, preparation and the hunt lifecycle. Combat strategies and authored attacks own physical actions. `NaturalTrapAI` deploys traps; the trap system decides whether a victim was actually caught. These capabilities are independent: choosing an opening does not choose the follow-up tactic.

Existing AnimalAI XML without `<Hunting>` keeps its previous behaviour. New profiles opt in with `hunting on`. Configuration is saved in the existing AI definition; per-NPC intent is stored in a saving `AnimalHuntEffect`, never on the shared AI object. Loading intent performs no attacks, movement, trap activation or venom injection. Subsequent ticks revalidate observation, policy, pursuit deadlines and physical feasibility.

Self-defence, territory defence and protection of young remain separate from hunting. Refusing to eat people does not prevent an animal defending itself against them. Wildlife group hunting uses the individual members' prey policy and assessment; participation does not grant an exception to those gates.

## Prey policy

The people policy is `Never`, `Desperate` or `Eligible`. Desperate requires the hunter's needs model to report **Starving**; ordinary hunger and peckishness do not qualify. By default, the engine's animal-lineage classifier distinguishes animals from people. An optional Boolean classification prog taking `(Character hunter, Character prey)` may override this classification; true means people.

Race includes, excludes and preferences also apply to descendants. An exclusion takes precedence over an include, preference, helpless opportunity or starvation. Optional apparent size bounds use **prey size minus hunter size**, rather than reading the target's exact mass. No universal prey-weight cap is imposed. Flight and hauling still enforce physical limits at the point of action.

An optional Boolean eligibility prog takes the same two characters. An optional Number preference prog supplies a tie-break score among otherwise suitable prey. `Safest` selection sorts by assessment, preference and distance; `Nearest` prioritises distance, and `LargestManageable` prioritises apparent size. All three first apply the eligibility and minimum assessment gates.

## Observable assessment

The assessment is an imperfect judgement on a 0–100 scale, not a probability of winning. Its default formula is:

```text
50 + 10 size + 20 injury + 25 vulnerability + 10 tactic + 5 support
   - 10 weapons - 20 owninjury - 20 fatigue + confidence
```

Size advantage and committed support advantage are limited to -3 through +3; the other factors are limited to 0 through 1. Target inputs come from apparent size, glance-visible wounds, posture/incapacity, visible wielded weapons, actual owned-trap restraint, and visibly committed combatants. Target skills, internal health totals, stamina and drug doses are not queried for assessment. The hunter may use its own health and stamina. Concealment in its ambush layer supplies tactical advantage.

| Profile | Start a hunt | Abandon an active hunt |
| --- | ---: | ---: |
| Cautious | 70 | 45 |
| Balanced | 60 | 35 |
| Bold | 50 | 25 |

Starvation reduces both thresholds by 10 by default, without bypassing prey policy. Separate thresholds prevent small fluctuations from repeatedly starting and cancelling a hunt. Builders can adjust both thresholds, the starvation adjustment, confidence and each factor weight.

## Openings and follow-ups

`Direct` approaches normally. `Ambush` prepares its preferred layer and attempts to hide. An authored `AmbushAttack` performs legal cross-layer ingress and a normal defended natural strike in one combat action. Missing the strike leaves the attacker on the prey's layer. A successful strike can attempt an opposed initial grapple; it does not grant full limb control. Ordinary armour, defences and subsequent escape/control actions still apply.

`TrapWait` prepares its shelter and hides. It attacks when an owned local trap has actually applied restraint, or, when `hunting opportunity on`, when an eligible observed target becomes helpless. Deployment, proximity and triggering alone are not capture. The appended `TrapCaughtPrey` event is emitted after restraint application, including delayed payloads. Restraint blocks movement, not all combat actions. Natural traps can use `home on` to anchor at the NPC's home item. Stock natural traps ignore their creator; generic traps retain their previous default.

Preparing a missing den or web is part of hunting, so hunger does not suppress its construction. Shelter craft IDs survive the AI-before-craft boot order and resolve when crafts become available. Natural trap maintenance observes its minute tick independently of the primary hunting AI's event handling.

After a pursuit ends away from home, trap waiters return through normal pathing before hiding at their hunting site again. Threat responses, survival needs and group movement ownership retain precedence.

Item-anchored traps reindex their proximity receiver on login after world placement. Restored webs therefore detect later arrivals, and repeated login does not duplicate the registration or replay a capture.

Applied restraints save their trap creator and origin cell. A spent one-use trap can disappear before a delayed payload or the next hunting tick; the restraint receipt still identifies its captured prey. Older restraint saves without this metadata require the original owned local trap to remain available. Expired restraints and captures belonging to another creator do not qualify.

`Fight` uses ordinary combat after the opening. `Extract` establishes control and tries a matching authored pull to the preferred environment, first in the current cell and then through a compatible adjacent exit. Reaching that layer returns to ordinary combat/control. An extraction cannot replace the opposed grip and forced-movement checks.

The existing `Dropper` strategy establishes a controlled grapple and uses authored layer pulls to ascend. Each action moves to the next viable higher layer. It accepts an already-airborne hunter and checks the current target plus both participants' carried equipment against lift capacity. Loss of control, flight capability or affordable carrying prevents further ascent; reaching the ceiling releases the target into the existing fall system. The authored combat action supplies its stamina charge.

Incapacitation or full grapple control can clear a victim's combat target without ending the attacker's combat. A victim unable to acquire a target remains in combat while the ordinary combat leave rules say an opponent still threatens it; unopposed victims and dead combatants can leave normally. This lets control and transport finish through scheduled combat actions.

Layer pulls put the carrier into the appropriate climbing, swimming or flying posture. A controlled victim stays supported only while its capable carrier is on the same layer and can bear the current burden. Losing that support restores ordinary gravity; a deliberate drop releases the hold and falls immediately. Tree wind checks still apply to the carrier. Dropper preserves its clinch while acquiring a grip, and both Dropper and Drowner are valid approach strategies that survive save and reload.

A temporary layer-change cooldown prevents another carry action but does not remove the support of an existing grip. Stock control and carry attacks permit climbing; leopards and panthers also receive climbing-only copies of their ordinary attacks so they can continue fighting after reaching the trees without changing their ground attack weighting.

Dropper and Drowner close to melee before selecting their ordinary attacks. Flying droppers remain in flight when descending through tree layers toward ground prey. Character relocation preserves the requested destination layer, including accompanying riders; refreshing a held victim's posture does not force a flying, climbing or swimming carrier to kneel.

`VenomWithdrawal` prefers an available envenoming attack, then disengages only after the move reports an actual positive injected dose through a qualifying wound. A miss or armour-stopped attack is not delivery. The hunter shadows using sightings, its last observed location and detectable, unambiguous same-race trails; it does not follow the unseen target's current coordinates or inspect venom timers. It returns when it observes incapacitation. Defaults bound pursuit to five cells from its origin, five minutes total and sixty seconds without a sighting. Route-cell trails are not reduced to whole-cell exits; unsupported or ambiguous trails wait until a sighting or timeout.

## Builder workflow

Clone a stock AI before customising it. On an edited AnimalAI, representative settings are:

```text
ai set hunting on
ai set hunting opening Ambush
ai set hunting followup Extract
ai set hunting layer InTrees
ai set prey people Desperate
ai set prey selection Safest
ai set assessment balanced
ai set hunting range 5
ai set hunting timeout 300
ai set hunting lost 60
```

Other controls include `prey include|exclude <race>` (toggle, descendants included), `prey prefer <race> <score>`, `prey sizes <minimum> <maximum>` or `any`, `prey eligibility|classification|preference <prog>` or `clear`, and `assessment weight <factor> <number>`. Quote multiword race names. `assessment engage|abandon|starvation|confidence <number>` adjusts numeric settings. `hunting opportunity on|off` controls helpless opportunities for trap waiters.

Attach the required natural attacks to the race and choose a compatible combat setting. Ambush attacks expose `source`, `destination`, `seize` and `resist` controls. New ambush attacks include the climbing posture so a stationary hunter in trees can use them; customised attacks must also permit the hunter's waiting posture. Extractors and droppers need initiate-grapple, appropriate extend-grapple attacks and a pull attack usable at grapple range. Venom withdrawal requires an envenoming attack and a valid venom liquid. Movement, anatomy, combat permissions and available terrain remain relevant.

`ai show` reports hunting configuration. Founder diagnostics under `impdebug wildlife` report hunt phase, target, deadline, eligibility, threshold and assessment contributions for observed prey. Use `impdebug wildlife <npc id> <prey id>` to inspect a rejected candidate; unseen candidates receive no assessment. `impdebug wildlife hungry <npc id>` uses the real needs model; `impdebug flush` saves pending changes before an independent persistence check.

## Stock content

The repeatable Wildlife Catalogue owns the following new profiles. Generic pre-existing profiles retain their original settings. Each listed species has one normal wildlife recommendation; natural-trap auxiliaries remain additional AI attachments.

| Species | Opening / follow-up | People | Assessment |
| --- | --- | --- | --- |
| Leopard, Panther | Tree ambush / extract | Desperate | Balanced |
| Jaguar, Tiger | Ground ambush / fight | Desperate | Balanced |
| Crocodile | Water ambush / extract | Eligible | Balanced |
| Alligator | Water ambush / extract | Desperate | Balanced |
| Caiman | Water ambush / extract | Never | Balanced |
| Eagle | Direct / dropper | Never | Cautious |
| Spider | Web trap / venom withdrawal | Never | Cautious |
| Tarantula | Web trap / fight | Never | Cautious |
| Scorpion | Burrow trap / venom withdrawal | Never | Cautious |
| Adder, Cobra, Coral Snake, Mamba, Moccasin, Rattlesnake, Viper | Ground ambush / venom withdrawal | Never | Cautious |
| Bunyip, Yacumama | Water ambush / extract | Eligible | Bold |
| Griffin, Garuda, Giant Eagle, Wyvern, Fell Beast | Direct / dropper | Eligible | Bold |
| Giant Spider | Web trap / venom withdrawal | Eligible | Bold |
| Giant Scorpion, Giant Centipede | Burrow trap / venom withdrawal | Eligible | Bold |
| Ankheg, Giant Worm, Colossal Worm | Burrow trap / fight | Eligible | Bold |

Stock serpent attacks include separate prone-compatible fang and venom attacks, including venom in a clinch, without changing the shared source attacks. Animal installation resolves the canonical Breathable Atmosphere gas. Catalogue reconciliation adds that missing tolerance to named stock air-breathing animals while preserving existing gas tolerances and custom races. It also repairs the known obsolete StandardRange fallback on stock Dropper/Drowner settings while preserving other authored approach modes.

Giant Spider defaults to Beast Clincher so it closes with caught prey. Leopard and Panther default to Beast Brawler so they continue fighting after hauling prey into trees. Reconciliation repairs their former Beast Skirmisher defaults while preserving a different custom setting. Existing NPCs retain their selected combat setting; builders can select the repaired default for those NPCs. Low-priority general venom and clinch-resistance messages cover new attacks and missing outcomes without displacing higher-priority authored messages.

Stock ambush and control attacks reuse seeded anatomy and damage definitions. Giant spiders and scorpions reuse their smaller relatives' configured venom liquids; the giant centipede uses the stock mixed spider venom because the ordinary centipede has no seeded venom attack. Reruns reconcile named stock rows and links with stable identities; differently named custom clones remain independent. Predator race defaults preserve custom combat settings, repairing only missing or known stock defaults (including Yacumama's old Beast Clincher setting). Builders should clone source-owned definitions rather than editing them in place.

The stock organic animal and human corpse models include material mappings for every decay state: flesh while fresh through decaying, bony flesh when decayed, and bone when skeletal. Earlier stock models omitted those mappings, making living prey appear inedible to predators. Animal, mythical-animal and wildlife content reconciliation supplies missing stock maps while preserving any existing authored map and custom corpse model.
