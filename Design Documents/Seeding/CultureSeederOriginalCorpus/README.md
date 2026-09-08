# Original CultureSeeder corpus

Captured from unmodified CultureSeeder source at `420fa26a7cb2be11fbb7b973e7e5270976e95067`, a descendant of PR #730.

`original_sources.json` contains all 28 original CultureSeeder source files as exact base64 bytes with SHA-256 hashes. This includes authored values which the original generator itself might deduplicate or replace. Do not regenerate this baseline from a redesigned seeder.

The seven pack JSON files contain the original in-memory generator's scalar database fields. They retain complete name-culture XML (regexes, usages, counts, styles and descriptions), random profile/gender associations, dice, names and weights, language and trait references, scripts, accents and directed intelligibility. `inventory.json` records table counts and file checksums.

An original row is addressed by **source pack + entity type + original primary key** (including every component for composite keys). Pack-local numeric IDs are export references, not installed database IDs or authority to merge similarly named identities across packs. The neutral `none` pack supplies common defaults and is also present in the original combined pack outputs; counts across exports therefore include deliberate copies of shared source content.

| Source pack | Profiles | Name elements | Accents |
|---|---:|---:|---:|
| none | 10 | 237 | 0 |
| earthantiquity | 51 | 10,064 | 411 |
| earthdarkagesandmedieval | 60 | 8,657 | 183 |
| earthrenaissanceeurope | 65 | 11,376 | 740 |
| earthrenaissanceworldexpansion | 60 | 6,127 | 253 |
| earthmodern | 74 | 11,495 | 610 |
| middleearth | 42 | 5,260 | 116 |

The exporter at `scripts/CultureSeederCorpusExport` runs against an isolated EF in-memory context and refuses to overwrite an existing pack export. It never opens MySQL. Build it with single-node MSBuild, then invoke its DLL with a new output directory and optionally one source pack key. It uses minimal prerequisite fixtures; heritage execution is excluded. Middle-Earth accent predicates are preserved as explicit symbolic external references in `ExternalProgReferences`, not falsely represented as compiled original predicates. Their full generating source is retained in the source snapshot.

This baseline is preservation evidence, not a database import, source-to-active reconciliation policy, historical certification, or a completed post-refactor preservation diff. All those implementation gates remain separate.
