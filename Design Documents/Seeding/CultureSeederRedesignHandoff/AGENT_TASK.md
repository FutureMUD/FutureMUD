# Codex task: implement the complete CultureSeeder redesign

> Round-two update: the corrective brief in [CultureSeederRound2Handoff](../CultureSeederRound2Handoff/AGENT_TASK.md) supersedes earlier naming activation gates and resource totals below. The runtime catalogue now has 28 required inputs and 585 name entries, including 389 unchanged evidence entries and 196 approved fictional profile entries. All 58 playable gender/era cells meet the 20-family floor; Old Prussian feminine profiles are enabled. Earlier evidence limitations remain historical notes, not activation blockers.


Use this archive as the content and implementation specification. Begin with `01_DESIGN_DECISIONS.md` and `02_IMPLEMENTATION_BRIEF.md`. Existing filenames are intentional; this is the final-decision revision of the earlier handoff.

## Baseline

Start from PR #730, commit `961efca81da0d788bbfb86ccf55fea313c9bf065`, or a descendant. Read the repository's AGENTS instructions and the existing skill-selection-group guide. That feature is implemented: reuse `ChargenSkillSelectionGroupSeeder.Upsert`, the resolver, screen wrappers, claims, builder commands and NPC adapter. Do not implement them again.

## Implement

1. Five self-contained culture-era toolkits using shared source-qualified identities. Preserve all original naming and accent data through a lossless pre-refactor manifest and non-destructive baseline reconciliation.
2. The supplied 110 social-background cultures, 68 ethnicity overlays, language-stage labels, scripts, explicit mutual-intelligibility matrix and retained source content.
3. Fixed ethnic native languages through existing mandatory free-skill progs. No home-language, upbringing, working-language or bilingual chooser. Specific cultures can add their fixed vernacular/education; broad groups supply optional neighbouring/educational languages.
4. Native base 200 and scaled tiers via existing in-game-editable starting-value progs. Preserve original non-language values, existing boost deltas, caps and stronger independent values. No new proficiency subsystem or seeder tuning questionnaire.
5. The 28 consumer group recipes using the implemented helper and its exact compiled prog contracts. Retain optional 0..1 CountKnown semantics, one slot per skill and existing cost accounting.
6. Explicit learned-background Literacy and writing-tradition grants. Avoid leaving the old broad script-award block active beside the narrower policy.
7. Targeted naming repertoires, source/date/evidence metadata, dedicated local profile identities where needed, existing ethnicity-first naming and actual parser/Latin-1 tests. No adopted-name selector or invented productive surname morphology.

## Research boundary

The name supplement is a bounded research delivery, not a claim of demographic coverage. Documentary, editorial, dynastic, devotional and literary entries have separate evidence labels. The feminine Old Prussian replacement remains inactive because no defensible corpus was retrieved. Small local feminine selections are disclosed. Preserve old source material; do not use an unrelated pool to hide a gap, and do not manufacture names or historical citations.

Existing profiles outside these targeted changes remain preserved rather than newly certified. Further per-entry historical research is not delegated to Codex. Implement the supplied content and report genuine unresolved source bindings; do not silently substitute modern peoples, languages or naming conventions.

## Completion

Meet CF01–CF20 in document 2 and retain SG01–SG20 as dependency regressions. Report the actual commit and tests, source-preservation diff, compiled prog identities, resolved group/candidate IDs, directed intelligibility settings, naming counts by gender/era and explicit exclusions. Distinguish C# tests, live MySQL migration/import, telnet/editor execution and handoff-data checks. No live checks may be claimed unless actually executed.
