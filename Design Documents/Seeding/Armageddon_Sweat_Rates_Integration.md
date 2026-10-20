# Armageddon sweat defaults: owner integration pending

20 October 2026. **Documentation and integration patch only; not applied to the setting worktree.** Target: `codex/armageddon-setting-20261019` at `05e30d2bb`, `C:/Users/luker/Documents/Codex/2026-10-19/task-3/FutureMUD`. No part of that implementation was cherry-picked. This deliverable accompanies the applied common-seeder correction described in [Seeded Sweat Rates](Seeded_Sweat_Rates.md). Publication remains last.

## Full approved roster

Base means whole-body litres per real minute before exertion multipliers. Effective values assume 60 qualifying minute ticks per real hour. They are **authored game adaptations**, not recovered Armageddon sweat settings. No numeric sweat rate was recovered from the supplied historical code/proposal. Do not confuse the roster's approval with activation of its incomplete physical profiles.

| Profile (stable key) | Base L/min | Heavy 0.4: L/h | VeryHeavy 0.7: L/h | ExtremelyHeavy 0.9: L/h | Rationale and provenance |
| --- | ---: | ---: | ---: | ---: | --- |
| Human (`human`) | 0.02 | 0.48 | 0.84 | 1.08 | Common Human exercise anchor H; source PC mass 60-90 kg |
| City Elf (`city_elf`) | 0.02 | 0.48 | 0.84 | 1.08 | Source 70-90 kg; taller/slender does not prove a different whole-body sweat rate |
| Desert Elf (`desert_elf`) | 0.02 | 0.48 | 0.84 | 1.08 | Same source 70-90 kg; recovered environmental thirst reductions do not quantify sweat output; no invented sweat immunity |
| Dwarf (`dwarf`) | 0.02 | 0.48 | 0.84 | 1.08 | Source 80-100 kg; compact geometry alone does not establish a distinct gland rate; independent of Middle-earth Dwarf |
| Half-Elf (`half_elf`) | 0.02 | 0.48 | 0.84 | 1.08 | Source 60-90 kg; conservative Human-order default |
| Half-Giant (`half_giant`) | 0.10 | 2.4 | 4.2 | 5.4 | Source 750-900 kg; authored surface-area analogy relative to a 75 kg Human: `(825/75)^(2/3)` is about 4.95; round 0.02 times this to 0.10; no linear x11 mass scaling or guarantee of heat safety |
| Mul (`mul`) | 0.03 | 0.72 | 1.26 | 1.62 | Source 100-150 kg; `(125/75)^(2/3)` about 1.41; round 0.0281 to 0.03; an authored modest increase, not a recovered endurance mechanic |

The additional **unavailable common Elf parent** (`arm.race.elf`) uses **0.02 L/min; 0.48/0.84/1.08 L/hour**. Both elf children receive explicit stored defaults as well; they do not depend on live parent propagation. All seven profile rows plus this parent retain `AlwaysFalse` availability. Desert Elf/Half-Giant/Mul application policy is unchanged.

Biological scale anchor H is [Barnes et al. 2019](https://pubmed.ncbi.nlm.nih.gov/31230518/), with the limitations and consumer effects in the common report. Fictional surface-area scaling is dimensional analogy, not a validated heat-balance model: a Half-Giant may still be unable to dissipate its metabolic heat under particular conditions. Existing Human blood-volume, needs and stamina systems require separate physical qualification before activation.

Setting provenance is the target branch's `Design Documents/Armageddon/Armageddon_Physical_Proposal_20261019.md`, especially PC weight constraints and the physiology audit. Its supplied `codedump.c` SHA-256 is `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`: PC weights at lines 141320-141375; stone/ten-stone interpretation at 142056-142062; environmental Desert Elf hunger/thirst logic at 79575-79654. Those constraints support the **size analogy**, not any claim that the source specifies these sweat numbers. This task did not alter or reinterpret ages, heights, names or the approved roster.

Old copied 0.8 would give 19.2/33.6/43.2 L/hour on every profile. Withdrawn Giant **2.4 L/min** would give 57.6/100.8/129.6. It remains withdrawn. Corrected Giant **0.10 L/min** happens to yield **2.4 L/hour at Heavy**; that is a different unit and quantity. There is no resurrection of the old 2.4 L/min proposal.

## Exact owner application

The accompanying [Armageddon_Sweat_Rates.patch](Armageddon_Sweat_Rates.patch) changes only `DatabaseSeeder/Seeders/ArmageddonSettingSeeder/ArmageddonHeritage.cs`: one typed rate lookup, one assignment before applying the common Elf parent, and one assignment before applying each owned profile. It leaves `Scaffold`'s saved baselines intact and passes the new desired value through `ArmageddonSettingOwnership.Apply`.

Common prerequisite commit: `b8da57998240cf4c306ff009ff636aeaffb67e03`. Integrate that common-seeder commit and this separate handoff-documentation commit into a suitably isolated owner branch, then check/apply the patch there:

```powershell
git apply --check 'Design Documents/Seeding/Armageddon_Sweat_Rates.patch'
git apply 'Design Documents/Seeding/Armageddon_Sweat_Rates.patch'
```

Do not apply to the current-master sweat branch, which has no setting installer. Resolve context drift explicitly if the owner has changed the target since `05e30d2bb`. The current deliverable was checked against an isolated minimal copy of the exact supplied file, without running the original installer or changing its worktree.

`ArmageddonHeritage.Scaffold` clones Human values only for first creation; reruns start from the recorded baseline. Explicit rate assignment is therefore required on every desired owned row, even after common Human is fixed. `Apply`'s existing three-way reconciliation is essential: unchanged old stock 0.8 can reconcile to the new desired number when an owner deliberately runs it; a custom current number/liquid or availability override must be retained and reported. The patch does not mutate bootstrap Human/Admin or existing characters. It changes no liquid reference, ages, naming corpus, application gate or availability logic. There is no live-world migration in this task.

Update the owner's physical proposal sweat section/table to reference this roster, retaining its historical audit of the withdrawn proposal. Append this acceptance boundary: sweat values have a documented candidate and explicit installer defaults, while all incomplete profiles remain disabled pending the remaining physical qualification. Do not present successful patch application as physical-profile acceptance.

## Required owner checks before considering integration complete

- Add a data test for `SweatRateForProfile` covering the seven keys above plus rejection of an unknown key; check the 24/42/54 hourly factors against every row.
- In the existing isolated heritage fixture, assert all eight stored race rates, their existing sweat liquid references, correct parent graph and `AlwaysFalse` availability after first install. Assert bootstrap Human/Admin/characters are unchanged.
- Rerun unchanged stock and confirm idempotence. In a separate fixture, set a profile's sweat rate to 0.007, choose a custom sweat liquid, and deliberately alter availability; rerun and assert all three custom values survive with reconciliation messages. Include custom zero. Verify old recorded stock baselines reconcile only through the normal explicit installer path, never a blanket UPDATE.
- Run the DatabaseSeeder suite and relevant `ArmageddonHeritage` / `ArmageddonSettingOwnership` tests on the owner's integrated revision. No seeder/replay may target an existing/shared world for these checks. No activation, push, merge, PR or deployment is authorised here.

The current-master branch's common tests and isolated patch applicability check are evidence for those limited scopes. The setting installer and these owner-specific scenarios have **not been executed** by this task; integration remains pending.

## Patch provenance and applicability receipt

The supplied `ArmageddonHeritage.cs` SHA-256 was `061f129774b281c59d57bc13af423f2a6275c9914bae66e62b813e1dea24945f`. The isolated applicability copy was byte-identical to that file. Default `git apply --check` passed after retaining LF patch bytes. Patch SHA-256: `7a94acb4f3427c6986a5e9f0021d454d73422550905e92a3d5a843506c21ee96`. The `.gitattributes` rule is scoped to this patch alone to retain LF on Windows and allow canonical unified-diff context prefixes (a space before original tabs/blank lines) in whitespace checks. C# and documentation retain ordinary whitespace checks.
