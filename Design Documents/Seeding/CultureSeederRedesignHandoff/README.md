# FutureMUD CultureSeeder redesign handoff

> Round-two update: the corrective brief in [CultureSeederRound2Handoff](../CultureSeederRound2Handoff/AGENT_TASK.md) supersedes earlier naming activation gates and resource totals below. The runtime catalogue now has 28 required inputs and 585 name entries, including 389 unchanged evidence entries and 196 approved fictional profile entries. All 58 playable gender/era cells meet the 20-family floor; Old Prussian feminine profiles are enabled. Earlier evidence limitations remain historical notes, not activation blockers.


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

`data/targeted_name_corpora.json` contains 585 given-name entries (389 original evidence entries plus 196 approved fictional additions) in eight separate bounded repertoires. Each entry identifies its source, evidence class, original form, Latin-1 display form and date or explicit date limitation. Documentary, editorially normalised, dynastic, literary and devotional evidence are not interchangeable.

The supplied round-two reconstructions resolve the playable naming gaps, including Old Prussian feminine profiles. All 58 required cells meet the 20-family floor. Original evidence limitations remain explicit: these additions do not establish attestation or a proven historical suffix rule. See document 6 and the round-two content receipt for counts and evidence boundaries.

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
