# Acceptance and verification

## Evidence levels

The supplied Python validator checks the handoff data, references, playable counts, preservation snapshot and file integrity. It does not execute C#, FutureProg, MySQL, chargen or telnet. The coding agent must produce the following separate execution evidence.

## Required regression cases

### R2-01 — Canonical menu and compatibility scope

**Setup:** Clean prerequisites; no pre-PR732 historical entities.

**Action:** Resolve each of antiquity, darkages, medieval, renaissance, earlymodern and show their installer choices.

**Expected:** All five canonical choices resolve correctly. No legacy migration/version-block feature is added. An old-alias compatibility test is not an acceptance blocker.

### R2-02 — Fresh native-200 invariant

**Setup:** Clean full toolkit for each era, stock original starting prog/caps, each toolkit-created culture and a resolved native ethnicity.

**Action:** Execute the actual culture starting prog and capped creation path for native, acquired and non-language skills; add one paid-boost case and a stronger independent value.

**Expected:** Unboosted native 200; existing 180/150/100/50 tiers when applicable; original non-language result; original boost delta once; stronger independent value retained. No blanket wrapper for unrelated custom cultures.

### R2-03 — Deleted script member

**Setup:** Full current toolkit; Latin script with English member and unchanged generated acquisition prog.

**Action:** Delete only the membership, rerun twice, compile/execute the actual prog for English.

**Expected:** Membership stays absent and English is false. Default prog generation must not use desired members before reconciliation.

### R2-04 — Builder-added script member

**Setup:** R2-03 plus a custom language/skill not in toolkit canonical data.

**Action:** Add that actual language to the script, rerun, execute acquisition for its linked trait.

**Expected:** Custom membership survives and generated prog returns true for its real linked trait. No canonical-key requirement for a builder-added language.

### R2-05 — Empty script graph

**Setup:** A managed script after builder deletes all of its memberships; default prog remains unedited.

**Action:** Rerun and evaluate unrelated sample traits.

**Expected:** Empty effective graph generates false, with no hidden desired-language fallback.

### R2-06 — Custom script acquisition

**Setup:** Separate fixtures: custom knowledge acquisition pointer; edited body of the stock generated prog.

**Action:** Rerun with desired graph changes.

**Expected:** Pointer/body edits survive, appear as overrides and are not claimed as stock graph equivalence.

### R2-07 — Atomic script failure

**Setup:** Relational caller-owned transaction and a deliberately invalid generated-reference/compile fixture.

**Action:** Fail after membership reconciliation but before successful completion.

**Expected:** Transaction rolls back script, knowledge, membership and prog changes together. No partly active graph survives.

### R2-08 — Fresh synthetic learner

**Setup:** Fresh Early Modern pack creating english.earlymodern without a retained source language row.

**Action:** Inspect DefaultLearnerAccent and execute its actual availability for Welsh non-native and English-native applicants; inspect eligible native candidates.

**Expected:** Welsh non-native has a usable learner default; native applicant has an eligible native/regional candidate and is not offered the stock learner role. Never left at unchanged AlwaysFalse.

### R2-09 — NPC accent provenance

**Setup:** R2-08 with opt-in generated templates, fixed generation seed and no manual accent.

**Action:** Generate one ethnic native and one acquired-language speaker, then repeat with saved manual accents.

**Expected:** Native picks eligible native/regional; acquired picks learner path. Manual/saved accents remain authoritative. No fallback to arbitrary native accent because learner is blocked.

### R2-10 — No-native-ethnicity language

**Setup:** Installed learned-only fixture language with no native mapping rows.

**Action:** Seed policies and generate a non-native learner.

**Expected:** Empty native set is supported; learner/default policy is still created and evaluated. The accent pass does not skip this language.

### R2-11 — Current stock repair versus custom false

**Setup:** A PR732-owned synthetic learner still at its unchanged AlwaysFalse baseline; separate fixture with edited/custom predicate.

**Action:** Apply round-two rerun.

**Expected:** Unchanged stock false is repaired. Genuine custom pointer/body and unowned accents are preserved and reported, not globally cleared.

### R2-12 — Optional installation ordering

**Setup:** Clean prerequisites for one source-less-stage pack.

**Action:** Languages-only -> full -> languages-only -> full; capture accent IDs, predicates and native maps at each step.

**Expected:** Era eligibility applies from first installation; after heritage exists later language-only runs do not erase native restrictions; stable IDs, no duplicate predicates, final graph equals direct full installation apart from audit timestamps.

### R2-13 — Explicit Latin era rules

**Setup:** Import retained source Latin traditions into separate fresh Antiquity, Dark Ages, Renaissance and Early Modern worlds.

**Action:** Evaluate retained RenaissanceEurope Classical/Liturgical/Neo-Classical accents and DarkAges Carolingian with unmodified predicates.

**Expected:** Classical allowed in Antiquity; Liturgical/Carolingian absent from stock Antiquity choices but allowed from Dark Ages; Neo-Classical unavailable until Renaissance. Source definitions remain retained.

### R2-14 — Fallback with preserved ineligible natives

**Setup:** A canonical language with preserved native-tradition rows but none era-eligible in a fixture.

**Action:** Seed the selected era.

**Expected:** Create/reuse canonical local fallback despite presence of ineligible source rows. Keep old rows unavailable. A builder-disabled fallback is not silently revived.

### R2-15 — Single accent-field owner

**Setup:** Full pack with combined managed era/role predicates and one edited combined predicate/default pointer.

**Action:** Run twice, comparing stable IDs, unedited predicate bodies and edited values.

**Expected:** No competing writers or recursive wrappers; untouched results stable, custom changes retained and reported.

### R2-16 — All Antiquity native bindings

**Setup:** Fresh full Antiquity, all 56 exact source ethnicity identities.

**Action:** Resolve/activate all and execute free-skill and starting-value progs for each of the 13 new rows.

**Expected:** 56 of 56 source identities have resolved native references. Each new mapping grants its exact supplied installed language at 200; no synonym duplicate language is created.

### R2-17 — Retained Albanian

**Setup:** Fresh Medieval, Renaissance and Early Modern worlds.

**Action:** Resolve source.earthrenaissanceeurope.ethnicity.Albanian.

**Expected:** All three use canonical albanian, not a name-culture guess or a ruler language. Ethnicity is not left disabled solely for missing mapping.

### R2-18 — Crosswalk failure handling

**Setup:** Fixtures with duplicate same source/name/era rules, misspelled language key and unrelated out-of-scope ethnicity.

**Action:** Preflight and attempt install.

**Expected:** Invalid applicable rules fail before activation/commit; unrelated unresolved identities are not blanket enabled. Exact source identities are included in diagnostics.

### R2-19 — All 58 playable name cells

**Setup:** Load revised source definitions before DB writes.

**Action:** Compose each required pool/era/gender from name_playability_policy.json.

**Expected:** At least 20 distinct family keys and displays per cell after playable filtering; positive effective weights; no inflation by orthographic variants; exact delivered count receipt.

### R2-20 — Old Prussian feminine activation

**Setup:** All four eras supporting ethnicity.old-prussian.

**Action:** Build/persist male, female and shared profiles; exercise female suggestions and generated names.

**Expected:** Female profile enabled with supplied reconstructed core (20 in Dark Ages;26 in later packs), correct gender link and no fallback to unrelated names. Male remains populated.

### R2-21 — Evidence not rewritten

**Setup:** Read the original389-entry evidence snapshot plus revised corpus.

**Action:** Compare every original field; inspect added forms and deactivated Botezata entry.

**Expected:** Original evidence/date/packs/weights unchanged. Added full forms claim no attestation. Botezata remains in evidence but is not selected as BirthName. No false source citation.

### R2-22 — Playable weights and builder edits

**Setup:** An unchanged PR732 targeted profile; a separate copy with deleted original element, altered weight and custom added element.

**Action:** Apply revised catalogue twice, then compute effective counts and weights.

**Expected:** New stock forms installed by stable keys; original applicable weights retained; specified editorial weights used by era. Custom deletion/weight/addition persist; floor does not undo builder customizations.

### R2-23 — Actual name rendering

**Setup:** All active full forms including Bar Sauma, apostrophes and Latin-1 names, with optional byname zero/one.

**Action:** Parse and render through actual NameCulture, NamePicker/PersonalName and random generator; reload database name/profile records.

**Expected:** No forced surname, spurious space, duplicate suggestion from wrong gender, case damage, non-Latin-1 game display or compound truncation. Neutral repertoire remains separate.

### R2-24 — Five-era install and rerun

**Setup:** Five independent clean prerequisite databases/fixtures.

**Action:** Full install then same-era rerun for each; compare counts and keys; also test names-only and heritage/no-language option paths.

**Expected:** No new hard-coded missing-resource failures, missing target source dependencies, duplicate IDs or unexpected activation losses. Optional paths are explicitly verified, not assumed.

### R2-25 — Resource receipt and negative test

**Setup:** The existing26 runtime resources plus2 new required inputs.

**Action:** Validate all28; remove each new one in a fixture; add an irrelevant extra JSON; corrupt a reference.

**Expected:** Exactly identified missing/invalid resources fail; extra count cannot conceal missing input. Documentation and checksum receipts match committed content.

### R2-26 — Interactive completion check

**Setup:** Disposable game seeded with round-two content; ordinary chargen and an educated character fixture.

**Action:** Use builder/prog editor, edit one script association, reopen a pending application, choose group/open skills and accents, create a named character, reconnect.

**Expected:** No new choice screens, no regressions to group/open selection or boosts, actual final language/accents/names match receipts. Record a transcript or explicitly disclose that this live check was not executed.

## Test locations and execution

Follow the current root and project `AGENTS.md` instructions. Prefer the repository scripts for the normal fast unit suite. Typical focused locations are `DatabaseSeeder Unit Tests/CultureToolkit*Tests.cs`, core chargen/GeneratedSkillGroupAdapter tests, and name-parser tests in the appropriate library/core project. Use runtime/prog execution tests rather than only string assertions for availability.

A typical local sequence (adjust only for the actual current repository scripts and installed SDK):

```text
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
dotnet test "DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj" --no-restore -m:1 --filter FullyQualifiedName~CultureToolkit
dotnet test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" --no-restore -m:1 --filter "FullyQualifiedName~Culture|FullyQualifiedName~GeneratedSkillGroup|FullyQualifiedName~PersonalName"
git diff --check
```

Run the normal fast suite through `scripts/test-unit.ps1` or the current platform-equivalent repository script. Report the actual commands and number of executed tests; the illustrative filter is not itself a statement of coverage. If schema changes are introduced, run the standard EF pending-model and migration/snapshot workflow.

Execute five independent fresh-era installs and reruns against disposable MySQL databases where available. EF InMemory results do not establish relational behaviour, FK correctness, rollback or MySQL collation effects. Never seed a production world to obtain test evidence. A genuine environment limitation must be stated; do not mark an unexecuted relational check as passing.

## Required reports

Produce these documents in `Design Documents/Verification/`:

**`CultureSeeder_Round2_Implementation.md`:** final commit/base identity, changed files, R1/R2 compatibility dispositions, exact R3/R4/R5 fixes, content loading and resource changes. Include no reference to an external chat.

**`CultureSeeder_Round2_Content_Receipt.md`:** each of14 new source-native bindings and resolved IDs; all56 Antiquity outcomes; Albanian in3 later eras; all58 name cells with family counts and evidence-class totals; retained/unavailable/overridden accents and chosen rules; final script graphs versus default acquisition eligibility; original-entry preservation results; any deliberate builder departures from stock minima.

**`CultureSeeder_Round2_Verification.md`:** map each R2-01…R2-26 case to concrete tests/commands and pass/fail/not-run results; exact normal-suite and focused counts; each disposable database path; interactive transcript or explicit not-run entry; unresolved defects with reproduction details.

A passing content receipt must not count disabled ethnicities as playable, source-record aliases as new names, or preserved but unavailable accents as contemporary choices. Do not replace behavioural tests with the pack's Python validator.
