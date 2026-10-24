# Approved Armageddon naming integration

The [embedded manifest](../../DatabaseSeeder/Assets/Manifests/Armageddon/naming.json) is a **user-approved Dark Sun-inspired adaptation**, not historical Armageddon canon. It includes all15 logical traditions (five upbringing, ten racial),8 canonical personal pools,321 distinct display forms /401 racial associations,62 supplementary components and78 original element descriptions. Human inheritance for Half-Elf/Half-Giant is a reference to one pool; it does not duplicate weights. Standalone Aarakocra, Halfling, Pterran and Thri-kreen naming cultures create no races, languages or playable profiles.

## Source recovery and provenance

The incoming task JSON was visibly truncated. Its complete defaults, source associations and design constraints were retained; all personal forms, component collections and78 descriptions were recovered read-only from the owner's [approved Page](https://chatgpt.com/space/page_8c0b135aaf4c8191844f463d670a1dea), sequence2, updated `2026-10-20T11:53:37.278700Z`. The manifest records individual source block IDs/hashes. The descriptions' ordered ID/text SHA-256 is `9173A85FE57040D4F0AA7200B5C245F5A202CCED0B8B745BD9F8E576D71C47E0`, pinned in tests as source provenance, independently of the current player prompts.

The supplied original attachment `dark_sun_names_expanded(1).json`, Library ID `libfile_14743ad7f57c81918aa223712a2e5cce`, has **upstream-reported** SHA-256 `5a8ce2cf4bc84bc6482db977cd86c58dc5a89b8e36ab7cd3256fb01e548fecc9`. This work did not read/re-hash those raw attachment bytes. The attempted source reference read returned “Pages are not available to this task”; it was not retried. The parent's Library upload had been denied separately and was not retried or bypassed. Source page numbers are owner-supplied references to an unidentified attachment source book, not independently verified citations. No outside corpus was invented or researched here.

## Native representation and player entry

Each logical tradition has one owned native NameCulture and five exact-gender RandomNameProfiles (75 native rows): Male uses male+shared; Female female+shared; Indeterminate, Neuter and NonBinary use the union. An empty gender subset falls back to shared then union. Every eligible exact form has weight1. The new culture opt-in disables the engine's legacy wildcard matching for these profiles in both chargen and NPC generation. Existing worlds retain their legacy policy. Case, curly apostrophes and Unicode hyphens remain exact; all supplied strings are NFC, BMP and at most13 characters, within current schema capacity.

## Current player entry (24 October correction)

The installed naming rules ask for exactly one **Given Name**, zero or one **Family Name**, and zero or one **Nickname**. Each has a short explanation of how the name is used in everyday life. Free entry, case and Unicode support remain available. Titles, affiliations, tribes, clans, communities and earned path names are no longer chargen questions; there are no background confirmation or tribe-display steps.

All 15 traditions and 75 gender-specific suggestion profiles remain. The original manifest's 78 descriptions and supplementary collections are retained as historical source data inside `CuratedNamingDefinition`; they no longer define the executable prompts. Random suggestions still select one given name and no optional components. Full display uses given and family names; nickname styles use the nickname when supplied. Surname-only display falls back to the given name when no family name was entered.

Upbringing naming remains the default. A configurable NamePicker style prog offers the selected upbringing and matching racial tradition; `/style <number>` changes the convention. Standalone naming traditions do not add playable races. Native NameCulture and suggestion-profile names no longer carry the Armageddon prefix. The legacy `arm.naming.personal` identity remains valid for existing name references.

Reruns update unchanged owned definitions and dice to these simpler defaults, retaining IDs and suggestion pools. Builder-edited definitions, entries, weights, dice and storyboard settings remain subject to the existing three-way reconciliation. Previously saved names retain their own components; this changes future naming entry, not stored character names. The evidence below describes earlier versions, including the superseded supplementary-entry rules.

## Preservation and verification

Named entities use existing typed ownership; profile entries and dice use owned reference reconciliation. IDs, builder renames/definitions, edited weights, added/deleted exact forms, zero dice, culture links and custom storyboard fields survive cold EF reload and repeated reruns. This is not an existing-world migration or an instruction to apply defaults to live characters.

Focused seeder run `20261020T124932Z-d6f09d624712`: **12 passed,0 failed/skipped**, stable source; includes all15 cultures, all75 gender profiles, provenance/counts/descriptions, custom Toon/any-parameter delegates, invalid-delegate preflight refusal and preserving reruns. Runtime run `20261020T124357Z-2ed7cb3f1e7e` passed157 affected checks (including breathing/liver/lifecycle/name regressions) with two new UI fixture failures; corrected fixture run `20261020T125019Z-770d2263b4b6` passed all5 naming-policy tests. The earlier install failures revealed a legacy64-bit prog-type cast; current code uses current-schema `ReturnTypeDefinition` storage. Failed receipts remain recorded.

Focused native MySQL follow-up `20261020T130517Z_2cf49ac3b9514d6fa35879ba2fe5ce60` **passes**: exact15 culture definitions/78 descriptions and constraints, all75 per-gender source sets/321 distinct forms, Unicode storage, and two cold process reruns preserving builder name entries/weights/deletions/zero dice. All eight race scaffolds remain unavailable. Source stayed stable and the owned MySQL/database/TEMP cleanup completed. This is native installer/persistence qualification; no fresh whole-Setting/server/gameplay acceptance is claimed for this slice. The original failed fixture and environment-interrupted attempt retain separate receipts in the execution ledger. Nothing has been published.

Final integrated full seeder suite at `8e823b55a`: `20261020T130941Z-24ff80709a8d`,2068 passed/0 failed/3 Unix-only skips, stable source. Debug and current-source Release installer builds pass. Later evidence finalization changes documentation only.


Executable-completion verification is recorded separately in the Setting evidence ledger. Earlier receipts qualify the personal-only/curated implementation, not executable supplementary entry. There is no automatic membership/achievement synchronization, entity lookup, arbitrary free-text full-name decomposition, or current-source whole-server/gameplay certificate for this completion slice. Those are distinct from optional manual naming support.


Completion qualification: all78 exact descriptions are executable, covered by final focused seeder12/0/0 and runtime36/0/0 runs. Native run `20261020T141805Z_dc2d2e18c50a4d25a3afea329c3bd0e8` passes actual NamePicker/Chargen save-hydration during confirmations/display choice, exact optional metadata/aliases/earned-prefix persistence and two cold preserving reruns. Final integrated full seeder run `20261020T142526Z-cb03597fa8c2` at implementation commit `adf066425` passes2068/0/3 (three Unix-only skips), with stable source and complete counts. See the [single Setting evidence ledger](Armageddon_Setting_Implementation_20261019.md) and its JSON sidecar for hashes and retained failures. No profiles were enabled and nothing was published.
