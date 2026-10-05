# Water Breathing stock content and dependency contract

This implements editable stock content following independent clearance of adapter `560ea460`, lung-order preservation `2099cda` and compatibility correction `c859a252`. The separately allocated pure native position query is committed as `b590edb77`; the stock uses it in an editable minimum-position filter. The [blocked draft receipt](Armageddon_Water_Breathing_Stock_Blocked_Receipt.json) retains the original missing-query failure. Stock acceptance is recorded separately from adapter evidence and production installer qualification.

## Content contract for the installer owner

`ArmageddonWaterBreathingStock.Create(world, school, castingSkill, resource, waterLiquids)` creates ordinary editable content with canonical key `arm.spell.water_breathing` and name **Water Breathing**. Supply the intended native Water school under the Sorcerer route, an existing character-owned native skill/resource and an explicit nonempty list of distinct current native liquid definitions. There is no inferred liquid name, historical-number/native-ID conversion or automatic capability/acquisition installation. The factory copies liquid IDs before construction; normal effect builders can edit mappings afterward.

The normal builder route is:

```text
magic spell edit new stock water-breathing <school> <casting skill> <resource> <water liquid> [other water liquids]
```

Quote names containing spaces. Water dispatch is confined to a new builder partial and the lane's existing perception-stock partial; the central stock dispatcher and shared native harness dispatcher are unchanged. Clone, spell/effect XML, resource cost and duration expression remain ordinary editable content.

| Contract | Value |
|---|---|
| Canonical immediate prerequisite | Acquired `arm.spell.draw_wine`, raw proficiency **80**; native grade1 acquisition baseline, no additional mastery requirement |
| Full source path | Unravel Enchantment root → Draw Water raw80 → Draw Wine raw80 → Water Breathing |
| Opening / raw cap / outgoing branch threshold | **30 / 90 / 80** |
| Capability grading | Relative proficiency gates; seven-grade profile; practice enabled without a separate practice maximum |
| Printed minimum energy | **20**, using the reviewed source-efficiency envelope |
| Delivery | Ordinary same-cell character/self trigger; no area, save or consumed component |
| Position | Source minimum Standing, mapped through an editable native-posture/noncombat filter; standing variants, flying, swimming and riding remain eligible |
| Active terrain/plane restrictions | None recovered; the old Nilaz rejection was commented out. No copied Silt, Fire or underwater-only restriction |
| Effect | Existing `sourcewaterbreathing` adapter, exclusive target effect, no caster effect |
| Random increment | One inclusive draw `floor(grade/2)..3*grade`; clamp zero to one unit |
| Lifetime | Group `armageddon.water_breathing`; 600 seconds/unit; sum normalized remaining units; cap36; strongest source grade/native power |
| Generic duration expression | `0`; prepared source selection supplies duration |
| Fluid scope | Explicit current native ILiquid identities/IDs; no CountsAs, constituent/mixture or gas inheritance |
| Stored scroll delivery | Unsupported; stock scroll opt-in disabled |

The installer may author admission with `casting entry add <waterSpell>`, `casting entry skill <waterSpell> 30 90 relative` and `casting prerequisite add <waterSpell> <drawWineSpell> 1 80`, after admitting the real Draw Wine spell with its intended distinct native skill. The existing prerequisite API requires grade1, the ordinary acquisition baseline; grade0 disables the route as invalid. This adds no mastery progression requirement beyond acquiring Draw Wine. Leave Water Breathing out of starting grants. This is a content/dependency contract, not an installer change or qualification of the production branch. The dedicated fixture authors this edge to test raw79 refusal, raw80 acquisition, opening30 and cap90. Its initial invalid-grade0 enrolment run is retained as failed evidence.

The [historical source qualification](Armageddon_Water_Breathing_Source_Qualification.md) distinguishes executable source rules from superseded catalogue proposals and unavailable native setting IDs. The [adapter integration](Armageddon_Water_Breathing_Adapter_Integration.md) records exact-deadline normalization and saved remaining-time restoration as engine adaptations. No cross-entry Firebreather/Parch protection rule, organ repair, poison/gas immunity, refill or general survival guarantee is added by this stock.

## Recovered minimum position and explicit native mapping

Supported Library reads of `codedump.c` establish that the skill record contains `min_pos` (166904-166916), Water Breathing selects `POSITION_STANDING` (175892-175895), and the ordinary nonimmortal casting wrapper refuses `GET_POS(ch) < skill[spl].min_pos` (173833-173852). The position constants run from Dead0 through Fighting6 to Standing7 (26440-26448). This is a minimum, not an independent equality check against a literal standing pose. The source description explicitly handles flying/floating inside `POSITION_STANDING` (65328-65334 and65463-65470); successful mounting sets the independent riding relationship (106235-106269), not a new rider posture. Those mechanics do not establish an additional source posture rejection. Native swimming is an explicit engine adaptation: the recovered position constants contain no swimming state. The source immortal bypass is recorded as historical evidence, not introduced as a new player entitlement or native staff bypass here.

Inspection found no existing Character/PerceivedItem FutureProg posture property or equivalent built-in. The allocated `positionid(character)` query reads the native position ID once, returns0 for null/unavailable input and preserves ordinary argument-error/type handling. It does not interpret IDs as an ordering, inspect combat, run eligibility scripts or mutate state. Its registration uses existing reflection discovery and normal metadata; no shared character, position, casting, lifecycle, interface or dispatch implementation changes.

The stock's authored prog samples that ID into a local `posture` variable, then requires the existing `@caster.incombat` property to be false and an explicit allowed native posture. Native IDs are identities, so comparison to an ordinal threshold would be incorrect. The mapping admits Standing1, Attention9, Ease10, Leaning11, Hanging13, Squatting14, Climbing15, Swimming16, Floating in Water17, Flying18, Riding19 and Zero Gravity20. It refuses Undefined0, Sitting2, Kneeling3, Lounging4, Lying5, Prone6, Prostrate7, Sprawled8 and Slumped12. Native standing-equivalent and active mobility states map to the source minimum; new native movement states have no one-to-one historical enum identity. Existing native body, speech, hand, movement, reach and planar gates remain independent and may still refuse an otherwise allowed posture. This explicit builder-editable mapping, and native combat membership as the source Fighting-state boundary, are engine adaptations. The filter tests the caster's posture, not the recipient's.

## Dedicated verification boundary

The stock tests exercise real template load/clone/save/reload, editable scope/profile policy, compilation and execution of the minimum-position filter for every native posture, combat/unavailable-posture refusal, and early refusal of empty, duplicate, missing and replacement liquid mappings. The new harness project has its own entrypoint and runner, uniquely named disposable MySQL instances/output directories and the existing exact-instance guards. It asserts builder construction/duplicate refusal, canonical prerequisite acquisition, paid grades1/7, minimum-position/missing-target/scope/reach refusal before payment, one draw, cap36, strongest grade, no-change/no-mastery, callback drift, mapped/unmapped native partless breathing, removal, cap90 grade7 paid practice and fresh-reader persistence/expiry. Acceptance results and source/assembly fingerprints belong to the separate stock receipt; source/harness compilation alone is not gameplay clearance.

Full race physiology/login strategy selection, breathing tick/exposure damage, gas-supply depletion, production world boot and non-self paid reflection remain explicit limits from the adapter checkpoint. Native partless breathing uses real Character/Body with an explicitly selected fixture strategy and mocked environment/racial compatibility. Fixture-authored acquisition and deterministic improvement do not qualify the actual installer branch. Central progress/repertoire records, prior receipts, installer files, charged carriers, combat authority and shared dispatch remain unchanged.
