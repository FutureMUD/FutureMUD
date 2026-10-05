# Water Breathing stock content and dependency contract

This is a **blocked draft content checkpoint**, following independent clearance of adapter `560ea460`, lung-order preservation `2099cda` and compatibility correction `c859a252`. Its editable definition and explicit mapping validation are implemented; normal stock construction and paid/native acceptance await allocation of the Standing query below. It is not completion clearance. Earlier source, adapter and compatibility receipts retain their original results.

## Content contract for the installer owner

`ArmageddonWaterBreathingStock.Create(world, school, castingSkill, resource, waterLiquids)` creates ordinary editable content with canonical key `arm.spell.water_breathing` and name **Water Breathing**. Supply the intended native Water school under the Sorcerer route, an existing character-owned native skill/resource and an explicit nonempty list of distinct current native liquid definitions. There is no inferred liquid name, historical-number/native-ID conversion or automatic capability/acquisition installation. The factory copies liquid IDs before construction; normal effect builders can edit mappings afterward.

The normal builder route is:

```text
magic spell edit new stock water-breathing <school> <casting skill> <resource> <water liquid> [other water liquids]
```

Quote names containing spaces. Water dispatch is confined to a new builder partial and the lane's existing perception-stock partial; the central stock dispatcher and shared native harness dispatcher are unchanged. Clone, spell/effect XML, resource cost and duration expression remain ordinary editable content.

| Contract | Value |
|---|---|
| Canonical immediate prerequisite | `arm.spell.draw_wine`, raw proficiency **80**; no invented controlled-grade requirement |
| Full source path | Unravel Enchantment root → Draw Water raw80 → Draw Wine raw80 → Water Breathing |
| Opening / raw cap / outgoing branch threshold | **30 / 90 / 80** |
| Capability grading | Relative proficiency gates; seven-grade profile; practice enabled without a separate practice maximum |
| Printed minimum energy | **20**, using the reviewed source-efficiency envelope |
| Delivery | Ordinary same-cell character/self trigger; no area, save or consumed component |
| Position | Standing caster through an editable target/caster filter; currently blocked on its query |
| Active terrain/plane restrictions | None recovered; the old Nilaz rejection was commented out. No copied Silt, Fire or underwater-only restriction |
| Effect | Existing `sourcewaterbreathing` adapter, exclusive target effect, no caster effect |
| Random increment | One inclusive draw `floor(grade/2)..3*grade`; clamp zero to one unit |
| Lifetime | Group `armageddon.water_breathing`; 600 seconds/unit; sum normalized remaining units; cap36; strongest source grade/native power |
| Generic duration expression | `0`; prepared source selection supplies duration |
| Fluid scope | Explicit current native ILiquid identities/IDs; no CountsAs, constituent/mixture or gas inheritance |
| Stored scroll delivery | Unsupported; stock scroll opt-in disabled |

The installer may author admission with `casting entry add <waterSpell>`, `casting entry skill <waterSpell> 30 90 relative` and `casting prerequisite add <waterSpell> <drawWineSpell> 0 80`, after admitting the real Draw Wine spell with its intended distinct native skill. Leave Water Breathing out of starting grants. This is a content/dependency contract, not an installer change or qualification of the production branch. The dedicated fixture prepares the same edge to test raw79 refusal, raw80 acquisition, opening30 and cap90 once the missing query is allocated.

The [historical source qualification](Armageddon_Water_Breathing_Source_Qualification.md) distinguishes executable source rules from superseded catalogue proposals and unavailable native setting IDs. The [adapter integration](Armageddon_Water_Breathing_Adapter_Integration.md) records exact-deadline normalization and saved remaining-time restoration as engine adaptations. No cross-entry Firebreather/Parch protection rule, organ repair, poison/gas immunity, refill or general survival guarantee is added by this stock.

## Required allocation before normal construction

The stock eligibility source is `return isstanding(@caster)`. Existing Character/PerceivedItem FutureProg properties and registered functions expose no position query, and existing spell/casting policies have no editable Standing requirement. The proposed addition is one pure `isstanding(character)` built-in in a new file, registered through existing reflection discovery and usable by ordinary editable filters. It must evaluate current native position and return false for null/unavailable characters. The native Standing-posture semantics need explicit allocation; no shared character, casting, lifecycle, interface or dispatch edit is proposed.

The user required coordination of shared API additions before broad edits. Allocation has been requested and has not been received at this checkpoint. No function implementation is added. The factory's existing support-prog compilation rejects the unavailable query inside its transaction; normal stock construction is therefore not yet available. Neither passing XML tests nor a harness build proves paid stock delivery.

## Dedicated verification boundary

The new stock tests exercise real template load/clone/save/reload and editable scope/profile policy, plus early refusal of empty, duplicate, missing and replacement native liquid mappings. The new harness project has its own entrypoint and runner, uniquely named disposable MySQL instances/output directories and the existing exact-instance guards. It compiles and contains assertions for builder construction/duplicate refusal, canonical prerequisite acquisition, paid grades1/7, Standing/missing-target/scope/reach refusal before payment, one draw, cap36, strongest grade, no-change/no-mastery, callback drift, mapped/unmapped native partless breathing, removal, cap90 grade7 paid practice and fresh-reader persistence/expiry. These dependent native assertions have **not run** while the Standing query is unavailable.

Full race physiology/login strategy selection, breathing tick/exposure damage, gas-supply depletion, production world boot and non-self paid reflection remain explicit limits from the adapter checkpoint. Native partless breathing uses real Character/Body with an explicitly selected fixture strategy and mocked environment/racial compatibility. Fixture-authored acquisition and deterministic improvement do not qualify the actual installer branch. Central progress/repertoire records, prior receipts, installer files, charged carriers, combat authority and shared dispatch remain unchanged.
