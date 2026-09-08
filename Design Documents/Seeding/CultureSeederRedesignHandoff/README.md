# FutureMUD CultureSeeder redesign handoff

Revision: final D1–D6 decisions, 7 September 2026. Existing filenames are retained.

## Start here

Read `AGENT_TASK.md`, then `01_DESIGN_DECISIONS.md` and `02_IMPLEMENTATION_BRIEF.md`. The content files and their JSON counterparts are a single delivery. This revision supersedes the previous instructions to build generic skill groups or add home-language/adopted-name chooser screens.

**Implemented dependency:** PR #730, commit `961efca81da0d788bbfb86ccf55fea313c9bf065`, or a descendant. Consume its groups, builder commands, resolver, persistence and accounting. Do not rebuild them.

## Current scope

Five overlapping era toolkits; 110 authored social backgrounds; 68 ethnicity overlays; 91 canonical language specifications plus one family resolver; 15 broad script specifications; 33 naming-family reuse plans; 28 elective group recipes; 177 explicit intelligibility pairs (354 directed settings), plus 16 conditional retained-language pairs.

The existing stock corpus is preserved by reference to its repository source. A lossless original-corpus export remains a required implementation step; this archive is not a copy of every historical name or accent already in the code.

## Final decisions

Ethnicity grants its fixed native language automatically. No upbringing/home-language override or adopted-name chooser. Native base is 200; other tiers are editable FutureProg defaults, not a new proficiency subsystem. General educational/contact electives use PR #730. Explicit learned cultures receive Literacy and specified writing traditions. All configuration uses existing in-game tools; no new seeder tuning questions.

## Name research delivered and its boundary

`data/targeted_name_corpora.json` contains 389 given-name entries in eight separate bounded repertoires. Each entry identifies its source, evidence class, original form, Latin-1 display form and date or explicit date limitation. Documentary, editorially normalised, dynastic, literary and devotional evidence are not interchangeable.

**Residual gap:** no defensible new feminine Old Prussian pool was retrieved. That replacement is inactive, not filled with German/Lithuanian names or invented suffixes. Latvian, Estonian and Romanian feminine selections are smaller bounded samples. They are not claims that those cultures lack larger corpora. See document 6 for exact counts and implementation treatment.

## Files

- `01_DESIGN_DECISIONS.md`: settled requirements and exclusions.
- `02_IMPLEMENTATION_BRIEF.md`: remaining engineering, dependency integration and acceptance criteria.
- `03_SOCIAL_CULTURE_CATALOGUE.md`: contemporary prose and concrete language/education defaults.
- `04_LANGUAGE_AND_SCRIPT_CATALOGUE.md`: labels, languages, scripts and intelligibility matrices.
- `05_NAMING_AND_ETHNICITY_PLAN.md`: preservation, named inventories, structures and ethnic defaults.
- `06_RESEARCH_AND_REVIEW_REGISTER.md`: evidence, limits and source register.
- `data/`: machine-readable content and contracts.
- `research/`: source register, original prose ledger, review decisions and original Latvian specimen.
- `tools/validate_catalogue.py`: standalone handoff-data validation.

Run `python tools/validate_catalogue.py` from this directory. `validation_report.json` records the actual run. These checks are not C# compilation, live MySQL migration, a telnet test or proof of historical attestation. The agent must perform those implementation checks separately.
