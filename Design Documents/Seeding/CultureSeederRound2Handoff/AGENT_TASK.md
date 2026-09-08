# Codex task: CultureSeeder round-two corrections and playable name coverage

Implement this self-contained corrective pack in `FutureMUD/FutureMUD`, starting from master containing PR #732 (`2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86`) and PR #730. Follow repository `AGENTS.md` files. Make a focused implementation branch and return an implementation/verification report with the completed change; do not merge the branch yourself.

## Required work

1. **Scope:** no migration or behavioural backwards compatibility is required for culture packs installed before PR #732. Leave current historical alias meanings alone. Do not add a version lock, destructive reset, legacy-culture migration or blanket wrapper for builder-created cultures. Fresh installs and same-era reruns of the redesigned toolkit must work and preserve builder edits.
2. **Script acquisition:** reconcile script-language membership first and generate the default acquisition prog from the effective surviving membership. Preserve builder additions/deletions and custom prog pointers/bodies. A removed language must not remain authorized by stale desired data.
3. **Accents:** repair source-less stage learner accents that receive `AlwaysFalse`; apply era and role eligibility to all installed languages, not only languages with a native ethnicity. Use the supplied deterministic accent policy. Preserve out-of-era source content while making it unavailable by stock chargen rules. Native/acquired default assignment must not silently substitute a native accent for an unavailable learner default.
4. **Native language coverage:** merge the 14 supplied exact source-qualified mappings before existing fallback resolution. They enable the 13 missing Antiquity identities and the retained Albanian identity. Use the supplied live description/group overrides without altering the original retained source corpus.
5. **Playable naming:** install the supplied `data/targeted_name_corpora.json`. It retains all 389 original evidence entries and adds 196 authored profile entries. Use `playable_packs` and `weight_by_era`; do not mistake a null attestation date or empty evidence-era list for a reason to discard an explicitly approved fictional form. All 58 stock gender/era cells must contain at least 20 distinct name families. Old Prussian female is explicitly enabled.
6. Update embedded resource validation, existing input receipts/checksums, focused tests and maintainer documentation. Do not weaken missing-file or content-validation tests to obtain a passing build.

## Content authority

`01_SCOPE_AND_FINDING_DISPOSITION.md` and `02_IMPLEMENTATION_BRIEF.md` define behaviour. The data files define exact name forms, eligibility and mappings. `03_CONTENT_CATALOGUE.md` is a readable rendering. `research/` distinguishes source evidence from editorial reconstruction. Use the supplied content; do not substitute a generator of invented suffixes or return the task to historical research.

Historical-fiction reconstruction and controlled regional borrowing are expressly approved. Evidence uncertainty is recorded outside player prose, not used as an activation blocker for these delivered profiles. Do not claim that reconstructed forms were attested.

## Existing features to keep

Five overlapping era toolkits; independent ethnicity and social culture; fixed ethnic native languages; native 200 / fluent 180 / educated 150 / conversational 100 / elementary 50 through editable progs; non-additive entitlement merging and original boosts once; existing generic group selection; separate literacy/script education; ethnicity-first naming without adopted-name or upbringing screens; Latin-1 player text; directed intelligibility; safe same-era reconciliation.

## Completion

Execute the acceptance cases in `04_ACCEPTANCE_AND_VERIFICATION.md` and supply `CultureSeeder_Round2_Implementation.md`, `CultureSeeder_Round2_Content_Receipt.md` and `CultureSeeder_Round2_Verification.md`. Report exact commands/results and unexecuted checks separately. Include all five clean-era receipts and a current-toolkit rerun with builder-edit preservation. Do not call specification-only validation an engine test.
