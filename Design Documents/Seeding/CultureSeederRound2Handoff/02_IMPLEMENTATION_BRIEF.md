# Round-two implementation brief

## 1. Resource placement and authority

At the reviewed commit the seeder embeds `Design Documents/Seeding/CultureSeederRedesignHandoff/data/*.json` as `CultureToolkit.data.<filename>` and `research/*.json` as `CultureToolkit.research.<filename>`. `CultureToolkitCatalogue` hard-codes a total of 26 resources. New runtime resources therefore require an explicit loader/validation change.

Apply this pack as follows:

| Supplied file | Runtime action |
|---|---|
| `data/targeted_name_corpora.json` | Replace the existing resource of the same name, preserving stable pool/profile/entry keys. The original evidence fields survive. |
| `data/native_binding_patch.json` | Merge its `exact_source_bindings` array into existing `data/legacy_native_language_rules.json`. This patch document itself is not a new runtime resource. |
| `data/accent_era_policy.json` | Add as a new required `CultureToolkit.data.accent_era_policy.json` resource and implement its resolver. |
| `data/name_playability_policy.json` | Add as a new required `CultureToolkit.data.name_playability_policy.json` resource; use it to validate authored stock coverage. |
| `data/finding_disposition.json`, `data/regression_cases.json` | Development/specification inputs. Do not embed them accidentally. |
| `research/*` in this new pack | Keep with the round-two task documents outside the old runtime research wildcard. Add newly cited bibliography IDs to the existing runtime `research/sources.json` where required. |

Replace the bare total check with an explicit required-resource-name set containing the existing 26 names and the two new names, or an equally strict manifest that validates those exact 28 required inputs. Extra task/audit files must not mask a missing runtime resource. Malformed JSON, duplicate keys and unresolved references still fail validation. Update resource-count assertions, manifest/checksum receipts and the existing Python catalogue validator deliberately; do not delete the checks.

The supplied original-name evidence snapshot was computed from the repository input with Git blob SHA `49cdb53c249fd8a8e7d093b4910f2b527cad3f7f`. A newer checkout may contain later changes. Merge by stable entry keys rather than overwriting later builder/content work, and report conflicts.

## 2. R1/R2: narrow the acceptance boundary

Do not change old alias meanings or implement old-culture migration. Keep the new toolkit's canonical menu choices correct. Source staging in `CultureToolkitInstaller` is private data reuse, not an instruction to bring every old Culture row into the live database.

Add a fresh-install test that enumerates each active culture emitted by `CultureToolkitSocialCultures.Upsert`, obtains its actual `SkillStartingValueProg`, and evaluates native language value through the engine's cap/boost path with stock settings. It must produce 200 without boosts. Verify a non-language skill still follows the original prog, and a stronger independent language value is not reduced. The excluded R2 scenario is specifically an old external culture remaining selected after an unsupported legacy upgrade.

## 3. R3: effective script memberships drive acquisition

### Defect

`CultureToolkitScriptSeeder.Upsert` currently builds `allLanguageIds` and the acquisition FutureProg before reconciling `ScriptLanguage` records. It unions desired members back into that set, even when the builder has deliberately deleted one. The resulting persisted graph and generated eligibility can disagree.

### Required algorithm

1. Resolve desired source/canonical language identities and preflight all references without writing.
2. Obtain or create stable script, knowledge and managed acquisition-prog identities. Where FK creation order requires a prog before membership exists, use the existing stable managed prog or a temporary fail-closed body on the same new identity inside the caller-owned transaction. Never publish a permissive placeholder or create a second prog on rerun.
3. Reconcile membership against its stored baseline. Respect current-toolkit builder deletions and additions. Apply any authored removal using the same three-way policy rather than deleting an arbitrary custom link.
4. Persist/flush the membership changes within the same transaction, then query the **effective installed `ScriptsDesignedLanguages` graph** for that script. Resolve actual linked trait IDs, distinct and sorted. Include builder-added languages even when they have no toolkit canonical key.
5. Build the desired generated acquisition prog solely from those effective trait IDs. No surviving members means `return false`. Do not use the pre-reconciliation union as an input.
6. Reconcile the managed prog body and the knowledge's acquisition-prog pointer separately. Preserve genuinely edited bodies and custom pointers; report any deliberate override whose behaviour differs from the stock graph. The stock generated body must match the graph exactly when it is unmodified.
7. Compile changed generated progs and refresh any cached runtime delegates before returning a successful result. A compile/reference failure rolls back the whole caller transaction, not just the last phase.

Keep ordinary script acquisition separate from free educational script grants. Do not introduce another broad free-knowledge block that bypasses this corrected acquisition rule. A custom builder grant remains a custom grant and should be reported as such rather than silently removed.

## 4. R4 and R5: one coherent accent policy

### 4.1 Inputs and classification

Implement the supplied `accent_era_policy.json` as an authorable input. Use source-qualified identities (`source_pack`, original language name, original accent name) or existing managed stable keys. Player-visible language aliases such as English are not identifiers.

Keep imported source records, descriptions, suffixes and phonetic detail. The stock era mask changes availability; it does not erase source content or copy a future pronunciation indiscriminately into an earlier native fallback.

Policy resolution is deterministic:

- An exact source accent override supplies its mask and optional role.
- Otherwise use the exact source-language override, or the source-module default.
- Intersect that default with every matching late-tradition marker. Marker matching is restricted to original source accent name/group, never player prose. Exact overrides have priority.
- A source-less canonical fallback uses the canonical language's installed-era membership.

These are deliberate broad-toolkit policy defaults, not a universal history of every dialect. The delivered table includes the important Latin exceptions: retained Classical is reusable in Antiquity; Liturgical begins in Dark Ages; Neo-Classical starts in Renaissance. A source-file date by itself must not exclude explicitly reusable material.

### 4.2 Availability state ownership

Use one managed availability record per accent for the **combined stock predicate**. Other scalar reconciliation passes must exclude `ChargenAvailabilityProgId` once that owner exists. Do not have the source importer and native-role pass alternately overwrite each other's field baselines.

The unmodified stock predicate is:

```text
allowed by selected era
AND original source-specific availability (null means true)
AND role eligibility
```

Native/regional/reading traditions have a true role predicate: learned speakers may still choose an appropriate regional pronunciation. The learner/foreign/crude role predicate is false for an ethnic native grant of this language and true otherwise. Do not use a culture label, current skill value or provisional ordinary/group pick to infer native status.

A builder override to the combined prog body or pointer remains authoritative under three-way reconciliation, and is recorded in the receipt. Do not silently enforce the era mask over deliberate post-toolkit edits. Store the source predicate separately enough to avoid wrapping the same composite recursively on every rerun.

### 4.3 Fix the synthetic learner bug

For source-less stages, stop giving the new `.accent.learner` a permanently false chargen predicate. Generate/reuse its managed role/era eligibility. The known PR #732 seed-created AlwaysFalse state is repairable when its current field and stored source baseline are unchanged. This is a correction to the current toolkit, not a pre-redesign migration.

Do not replace every AlwaysFalse pointer globally. Distinguish an unchanged toolkit-owned synthetic learner from a builder-created accent, an edited pointer, an edited prog body, or a genuine source-specific restriction. A field explicitly reset to the identical original value cannot be distinguished from an unchanged value by three-way comparison; record that limitation rather than inventing edit history.

Run policy generation for **all installed language IDs**, not just languages with entries in the native-ethnicity dictionary. A learned-only language still needs a usable non-native learner path. Missing native ethnicities mean an empty native set, not a reason to skip the language.

### 4.4 Fallbacks and native/acquired default choice

If a canonical language has no era-eligible native/regional tradition, create or reuse the supplied `.accent.local` fallback even when ineligible preserved native accents exist. Do not fabricate a new dialect description: use the canonical language's existing fallback description and suffixes. An explicitly builder-disabled fallback should cause a diagnostic rather than being revived.

On a fresh installation the default learner pointer must lead to a usable learner/foreign role for a non-native applicant. If the source pointer is an out-of-era or inappropriate stock default, retain the source evidence but bind the live default to the existing or new managed learner fallback. Do not globally replace a builder-edited default pointer.

In `GeneratedSkillGroupAdapter`, assign accents by provenance:

- A native ethnic language selects a deterministic eligible native/regional candidate (respecting saved/manual choices).
- An acquired language uses an eligible `DefaultLearnerAccent`; if a different learner candidate is explicitly configured, it may be selected deterministically.
- Do not fall back from a blocked learner to the first native accent merely because it passes availability. If configuration intentionally leaves no valid learner path, fail with a precise diagnostic naming language, pointer and applicable restrictions.

The ordinary player accent picker may offer eligible regional accents to learned speakers; this is distinct from silently assigning one as an NPC default. Once an accent is learned on an existing character, the seeder must not remove it retroactively.

### 4.5 Optional-install ordering

Era gating must work for language-only installs as well as full heritage installs. A first language-only installation can use an empty native set; the later heritage pass recomputes it. A language-only rerun after a full installation must recover/preserve the already-installed fixed native bindings rather than making every native eligible for learner accents again. The sequence language-only -> full -> same-era language-only -> full must be stable.

### 4.6 Receipt

Emit one row per live imported/generated accent: stable/source identity, canonical language, selected policy rule, allowed packs, role, original predicate, effective predicate ID, default-pointer status and builder-override status. Retained but unavailable accents must appear explicitly. Also list languages with no eligible native candidate or no non-native learner path; stock fresh installations may not silently ignore these shortages.

## 5. C1: exact source-native mappings

Merge `native_binding_patch.json.exact_source_bindings` into the existing legacy-native rules. Extend `CultureToolkitNativeBindings.Source` so exact `(source pack, source ethnicity, selected era)` matching precedes old Antiquity/regional/name-culture fallbacks. Duplicate applicable rows or unresolved language keys are validation errors.

Use the supplied canonical or source-qualified language references, resolve them to installed language IDs and then actual linked traits, and include those source keys in `activeRetainedLanguageKeys` before source-language availability is finalised. Do not create new duplicate language rows merely because the mapping uses a retained-source key.

Apply the supplied description/group overrides to the corresponding **managed live ethnicity** through `CultureToolkitEntityWriter` and its baseline reconciliation. Keep the original generator source definition and original prose in the retained source/audit material. In particular, Anatolian here means the exact Hellenised civic identity; the Libyan mapping means the exact Punic coastal source identity; Numidian is an explicit broad ancient North African gameplay language, not a modern Amazigh standard.

All 56 Antiquity source identities reported by the baseline receipt must resolve in a clean full Antiquity installation after these 13 omissions are filled. The retained Albanian identity must resolve in Medieval, Renaissance and Early Modern. Do not generalise this into enabling out-of-region identities in other source modules. Produce exact exclusion reasons for any other identity rather than hiding a missing reference behind a generic count.

The new native grants use the existing native factor, base 200, cap protection and deduplication. Update generated native-accent predicates and free-skill/starting-value partitions from the same final mapping graph.

## 6. C2: exact playable repertoires, not more research

### 6.1 Data semantics

The supplied eight-pool `targeted_name_corpora.json` is a complete replacement for this one resource. All 389 original entries retain every original evidence field and value. Added metadata and 196 new profile entries express approved fiction/borrowing. One original Romanian byname, Botezata, remains in evidence but is inactive as a random BirthName.

Change `CultureToolkitNameCatalogue.Build` to:

```text
pool available in selected era = selected era in pool.packs
entry available = selected era in (entry.playable_packs when supplied, otherwise entry.packs)
weight = entry.weight_by_era[selected era] when supplied, otherwise entry.weight
```

`date` and original `packs` are evidence metadata. They are not the gameplay filter when `playable_packs` is supplied. Empty `packs` on a reconstructed form is intentional, not missing content. Validate a positive effective weight for each active entry, and preserve the original display case and compound spacing.

### 6.2 Floor and persistence

Check the 58 requirements in `name_playability_policy.json` against the **post-era-filtered stock definitions**, counting distinct `family_key` values (case-insensitive), not aliases. The delivered data reaches the minimum without duplicate spellings. Do not add an arbitrary 50- or 100-name requirement or a surname quota.

Enable both gender profiles for Old Prussian and use the supplied feminine forms. Add the Dark Ages target repertoires for Finnish/Estonian/Old Prussian and the Medieval target repertoires for Lithuanian/Latvian. The corresponding ethnicity overlays already exist in those eras. Preserve their existing canonical target keys and the established gender/link mapping in `CultureToolkitNameSeeder`.

Continue the separate shared neutral repertoire and the existing optional-byname patterns. Do not introduce productive gender morphology, force surname generation, copy a male-only fallback into female suggestions, or change the generic name parser beyond any specific tested need.

Persist using the existing per-element three-way reconciliation. The minimum applies to authored stock and pristine installs, not to a later builder's chosen deletions. A rerun must add new untouched stock forms without restoring deliberately deleted old forms or overwriting custom weights. Report the lower effective live count as a builder deviation where applicable.

### 6.3 Evidence and text

The new entries use `editorial-reconstruction` or `regional-borrowing`; temporal additions to retained entries are recorded in `editorial_basis`. No source is claimed to attest a full invented female name or its year. The Old Prussian female forms use retained male-name shapes and cross-Baltic analogy, not an alleged proven Prussian suffix rule. The unresolved scholarly article is a bibliography lead, not a production blocker or an attestation source.

Rename the relevant stock profile display labels from Documentary/Register to Household where provided, using stable identity and builder-preserving reconciliation. Keep in-world descriptions in present tense without words about evidence, realism, migration or balance. Research qualifications belong only in author documents and machine metadata.

### 6.4 Superseded gates

Update existing documentation, validation and `research/review_gates.json` so these specific delivered repertoire gaps are marked **resolved by approved playable reconstruction**, retaining their historical-evidence limitations as nonblocking notes. Do not re-disable Old Prussian female because an older status field says no corpus was available. Do not remove unrelated review items automatically.

## 7. Shared implementation discipline

No generic skill-group rewrite, new proficiency entity, adopted-name selection, new starting-language answer, global Latin-1 regression, or broad intelligibility retuning is needed. Builder configuration stays in-game through existing progs/commands. Add any task-specific source policy as data rather than scattered magic-name special cases.

Use current toolkit stable keys, source provenance and existing transactional/reconciliation helpers. Do not catch a validation failure and continue with a partly active pack. If a migration is truly necessary, generate it through the established EF workflow; content-only fixes should not invent a new database schema without a demonstrated need.

All five clean-era results, changed stock-availability outcomes, before/after name counts, exact added native bindings, and preserved overrides must be present in the completion receipt. The passing Python handoff validator is not a substitute for this engine/database evidence.
