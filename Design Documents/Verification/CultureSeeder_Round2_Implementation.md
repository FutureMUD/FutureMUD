# CultureSeeder round-two implementation

Implementation commit: `e052681cf6efd6170c3d776afd9ff8b45474fec1`. Branch: `codex/culture-toolkit-round2`. Base: PR #732, `2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86`, including PR #730. No merge, migration, legacy reset, alias change or new chargen screen is part of this change.

The supplied [corrective handoff](../Seeding/CultureSeederRound2Handoff/AGENT_TASK.md) is retained with its original manifest, reference evidence and validator. Runtime inputs remain under `CultureSeederRedesignHandoff`: the revised names, merged exact native bindings, two new policy documents and additional bibliography entries. The explicit 28-resource manifest prevents unrelated files from concealing missing required inputs. JSON duplicate properties and stable keys fail loading.

## Script acquisition

`CultureToolkitScriptSeeder` obtains the existing managed acquisition prog, or creates its single identity with a fail-closed placeholder. Membership reconciliation runs before generation. It preserves builder deletions and additions, applies authored managed-member removals, flushes the graph, and resolves distinct sorted linked trait IDs from actual installed languages. Empty graphs generate `return false`. Knowledge pointers and prog bodies reconcile independently; custom overrides remain authoritative. The engine compiler validates generated progs before the caller-owned transaction can succeed. The seeder is an offline installer; it does not modify delegates in an independently running game process.

## Accent policy and generated defaults

`CultureToolkitAccentPolicy` implements the supplied exact source-accent, source-language and source-module precedence, with narrowing late-tradition markers matched only against original name/group fields. `CultureToolkitAccents` runs for all toolkit-installed language IDs, including learned-only languages. One `AccentAvailability` record owns each combined pointer. Source predicates remain separately recorded so reruns cannot recursively wrap a combined predicate.

The stock predicate combines era eligibility, original source restrictions and learner/non-native role. Regional and reading traditions do not exclude learned speakers. Synthetic source-less learners no longer start permanently false; the unchanged PR732-owned false baseline is repairable. Edited pointers and bodies are preserved. As with all three-way reconciliation, explicitly resetting a value to precisely its original value cannot be distinguished from leaving it unchanged.

Canonical local and learner fallbacks fill missing era-valid roles while preserving retained source accents. The learner-default owner starts from the retained source default, preserving deliberate current-toolkit pointer deletions. Per-language native bindings persist for later language-only runs; full heritage runs refresh the graph. Accent receipts enumerate retained, unavailable and overridden accents separately.

`GeneratedSkillGroupAdapter` uses the editable native-role prog generated from the fixed ethnicity graph, preserves saved/manual accents, and selects deterministic eligible native/regional candidates for native grants. Acquired languages use a valid learner default or another learner/foreign candidate. Missing candidates produce a language/pointer/ethnicity diagnostic. Existing independent skill values and generic group accounting are preserved.

## Native bindings and naming

Fourteen source-qualified rules precede existing native-language fallbacks. Thirteen complete Antiquity source identities; the retained Albanian rule applies in Medieval, Renaissance and Early Modern. Optional description/group overrides affect managed live entities only. A null override preserves the retained source field. Original generator definitions remain unchanged.

The eight targeted name pools contain 585 profile entries: 389 original evidence entries plus 196 authored additions. Gameplay filtering uses `playable_packs` when present and `weight_by_era` for effective weights. Original dates and evidence-era lists remain intact. All 58 required gender/era cells are validated after filtering for at least 20 distinct families; display collisions and non-positive effective weights fail validation. Old Prussian feminine profiles are enabled. Botezata remains in evidence but is excluded from the random birth-name repertoire.

Existing per-element reconciliation preserves builder deletions, custom weights and additions. The floor applies to stock definitions, not edited live profiles. Neutral repertoires, optional bynames, original casing and compound spacing retain their established paths. Review-gate decisions mark the delivered gaps resolved by approved playable reconstruction without claiming attestation.

## Repeatability repair

Repeated integrated fixtures exposed EF service-provider accumulation in isolated source stages. Each stage deliberately has its own temporary InMemory root; caching those providers retained whole source corpora and eventually tripped EF's provider-count guard. Source contexts now disable provider caching, rather than suppressing the guard. Their isolation remains unchanged.

See the [content receipt](CultureSeeder_Round2_Content_Receipt.md) and [verification report](CultureSeeder_Round2_Verification.md) for exact results, database identities and unexecuted checks. The branch remains unmerged.
