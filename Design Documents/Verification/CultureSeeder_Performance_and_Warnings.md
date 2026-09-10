# CultureSeeder performance and warning investigation

The reported Renaissance install selected all optional content and emitted 97 unresolved source-crosswalk messages plus two missing foreign-accent associations. The installer had a progress callback, but `CultureSeeder.SeedData` did not pass one. It now prints elapsed-time progress, including counts during language and accent processing.

## Changes

- Procedural source staging defers random-name leaves until scalar writes finish. This avoids repeatedly scanning a growing name corpus on every source save. Source fallback validation still runs after those names are persisted.
- Duplicate-name detection uses case-insensitive hash sets instead of scanning lists. Source-language crosswalks use a tuple-key dictionary instead of scanning all bindings.
- Targeted names run after language/chargen generation. Retained profile additions are persisted together after profile identities and baselines are reconciled.
- New source accents are inserted and baselined per language, replacing per-accent saves. Existing accents retain their reconciliation path.
- Accent availability progs are allocated and baselined in batches per language. Signature validation, original predicate composition, compiler validation and builder-edited bodies/pointers are preserved. A regression verifies that 25 progs and their parameters require two saves rather than 50.
- Accent ownership is grouped once by language. An installation-scoped ownership index replaces repeated stable-key database queries, observes tracked additions/deletions, and is disposed after each run.

No migration, content deletion, era switch or authored ethnicity mapping was introduced. Existing builder edits remain subject to the same three-way baseline reconciliation.

## Staging measurement

`scripts/CultureToolkitSourceProbe` supports `--benchmark-staging <report.json>`. It reads configured prerequisites and compares immediate versus deferred source name tracking in isolated InMemory contexts. Both paths use the current duplicate-name helper. This measures the staging sequencing change, not a complete old/new installer comparison. These are single local observations with concurrent verification activity; they are not benchmark thresholds.

| Source module | Immediate tracking | Deferred tracking | Name elements |
| --- | ---: | ---: | ---: |
| Antiquity | 32.83 s | 6.15 s | 10,064 |
| Dark Ages/Medieval | 10.08 s | 6.63 s | 8,657 |
| Renaissance Europe | 19.43 s | 6.73 s | 11,376 |
| Renaissance world expansion | 7.83 s | 5.34 s | 6,127 |
| Total | 70.16 s | 24.84 s | 36,224 |

All four comparisons preserved every naming structure, profile identity, suggestion-prog name, gender, element spelling/usage/weight and dice expression. The source-stage reduction was approximately 65%.

## Fresh local MySQL observation

The final Renaissance all-options import into `futuremud_culture_live_2145_p0910c` reached its last naming-reconciliation milestone at **58.8 seconds**. Source accent/language insertion, generated progs and names were included; prerequisite seeding and schema migrations were excluded. This is a single local measurement, not a promised runtime on other hardware. The preceding otherwise-optimized run without batched accent progs reached the same milestone at 124.1 seconds; those observations used separate disposable databases.

The final committed same-era rerun passed with unchanged entity counts and **zero reconciliation conflicts**, reaching the naming milestone at 150.3 seconds. It retained 81 social backgrounds, 188 ethnicity identities and the same 97 deliberately unresolved native bindings. Reruns remain slower than fresh installs because they load and reconcile the existing content graph; this pass does not claim that path is equally fast. The complete local receipt and logs are retained under `.appdata/culture-verification/`.

## Warning implications

The 97 retained ethnicity messages identify Ukrainian, Cossack and 95 world-expansion identities without authored native-language crosswalks. These source identities remain retained but unavailable in chargen. They represent incomplete playable coverage, not failed database writes. A rerun cannot resolve missing authored content; guessing mappings from names or broad regions would bypass the explicit source-binding policy.

The two accent messages were defects: modern German/Spanish association labels were reused for historical English, whose module installs High German, Low German and Castilian. Source-specific associations now resolve these historical identities while keeping modern associations unchanged. Missing associations impaired foreign-accent selection; they did not remove the languages themselves. Existing association overrides remain protected.

## Verification scope

The MySQL probe now enables the installer's lazy-loading behavior and clears completed prerequisite tracking before CultureSeeder and its rerun. The earlier probe retained all prior prerequisite graphs and inflated save/change-detection costs; its timings must not be treated as interactive-install measurements. A failed disposable fixture can resume its uncompleted steps only with its matching failure receipt and completed migration entry.

The final DatabaseSeeder unit suite passed all 861 tests. Debug and Release builds passed; PE metadata inspection confirmed replay profiles are Debug-only. The four-module source parity benchmark passed. No live telnet/editor behavior is claimed by these installer checks.
