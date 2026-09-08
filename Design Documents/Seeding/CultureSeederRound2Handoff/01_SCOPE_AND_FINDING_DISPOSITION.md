# Scope and finding disposition

## Baseline

Repository: `FutureMUD/FutureMUD`. Reviewed implementation: PR #732, commit `2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86`. Generic chargen skill groups: PR #730. The current source has been read to distinguish old-installation compatibility concerns from fresh-install defects.

## Compatibility boundary

No backwards-compatibility guarantee is required for installations of historical culture packs predating PR #732. This is an **unenforced project scope decision**, not a requirement to detect and reject old databases. Do not spend this tranche building alias migrations, old-culture starting-prog upgrades, database reset commands or automatic era transitions.

This waiver does not remove source preservation. Existing authored names, accents and source definitions remain valuable inputs. It also does not waive post-redesign same-era repeatability, stable managed identities, preservation of genuine builder changes, or the integrity of shared chargen systems. Do not delete approved characters, arbitrary custom entities or old data as a shortcut. An old installation may remain unsupported without being proactively destroyed.

| Finding | Decision | Actual work |
|---|---|---|
| R1: old MedievalEurope aliases changed meaning | Close as waived | Leave current aliases as implemented. Verify/document canonical Antiquity, Dark Ages, Medieval, Renaissance and Early Modern choices. No claim of preserved old semantics. |
| R2: old external culture still gives language 100 | Close the legacy-upgrade case as waived | No new failure is established for a clean toolkit installation. Test every fresh toolkit-created selectable culture through the actual starting-value hook; an ethnic native grant must reach 200 with stock settings. Do not patch unrelated cultures. |
| R3: stale script eligibility after membership edit | Correct | The defect occurs when rerunning a current toolkit after a builder edits memberships. Generate from the effective graph. |
| R4: source-less stage learner is blocked | Correct | Fresh source-less language construction sets its learner to AlwaysFalse; the accent pass treats that stock value as a restriction to preserve. This is independent of pre-redesign data. |
| R5: future-era accents become selectable | Correct | Canonical source aggregation preserves accents from multiple modules without sufficient era eligibility. Preservation and availability must be separated. |
| C1: unresolved in-scope native languages | Complete authored mappings | Use the exact 14 supplied rows. Do not globally enable every unresolved or out-of-region source ethnicity. |
| C2: thin per-era naming pools | Complete playable data | Use the delivered 20-family floor per gender/era, including explicit speculative Old Prussian female forms. |

## Existing design, restated

Use broad overlapping toolkits: Antiquity; Dark Ages roughly 500–1100; Medieval roughly 1000–1400; Renaissance roughly 1400–1600; Early Modern roughly 1600–1750. They are builder starting points, not fixed historical snapshots. The geographical focus is Europe and the Near East, including Iran, the Caucasus, Egypt, Arabia, the Maghreb and the Pontic steppe. Only one historical era is selected per world.

Ethnicity supplies its authored native language automatically. Culture supplies an independent social/class/regional background and any authored vernacular or educational entitlements. Names remain ethnicity-first. No adopted-name, upbringing, home-language or bilingual-choice subsystem is in scope.

Existing language-proficiency progs remain the configuration surface: native 200, fluent 180, educated 150, conversational 100 and elementary 50 at stock baseline. These are skill values, not comprehension percentages. Maximum applicable entitlement wins; original boost delta is applied once; stronger independent values and intentional builder edits remain protected. Keep literacy and script knowledge separate from spoken-language skills.

Continue using generic PR #730 skill groups for elective curricula. Do not alter their cost accounting, stable ownership, exact/ranged counts, CountKnown semantics or protected eligibility context as part of these corrections. Keep the current directed intelligibility catalogue and 7–9 calibration unchanged.

## Fictional expansion policy

A useful historical-fiction starter pack may contain informed reconstructions, modest temporal reuse and regional borrowings. These are now approved **authored stock content**, not pending research. Each form must carry honest metadata, but ordinary player descriptions describe the society in its own time and contain no editorial caveats.

A minimum means 20 **distinct name families** in each binary-gender profile after era filtering, not 20 spelling variants across all eras. Here a family is a lemma/spelling-variant cluster, not every distinct compound that happens to share a naming stem. Shared male/female Finnic forms are allowed where the gender distinction is not securely documented; inventing an unsupported feminine suffix is not required. Exact authored Prussian and Lithuanian feminine forms are allowed, but do not generalise them into an engine morphology rule.
