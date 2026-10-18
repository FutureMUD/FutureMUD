# Core tag foundations

Core owns the complete former Useful tag taxonomy, including all era/culture, resource/commodity, functional, market, consumable, offering and wilderness autobuilder classifications. Optional components and generated templates remain in Useful and consume Core identities. Core tags alone never count as installed Useful packages. There is no Useful `tags` question or replay answer.

## Manifest and identity

`DatabaseSeeder/Assets/Manifests/CoreTags.tsv` is the embedded authority. Each declaration records an immutable key, intended full parent path, source family and original declaration spelling. A key is the reviewed `core.tag.` plus the first 32 hexadecimal SHA-256 characters of the lower-case declared path; runtime never regenerates keys from live names or parentage. Identical declarations share an identity; different full paths with the same leaf remain separate.

The 18 October planning audit counted 2,306 declarations and 2,268 paths. Current-source qualification found three omitted `foreach` loops in the original Useful tag pass: 25 Renaissance cultures, 36 Early Modern cultures and six institutions. The complete manifest therefore has **2,373 declarations and 2,335 distinct paths**. Former Useful content contributes 2,158 declarations and 2,147 paths; 27 overlap existing Core paths. All previously reviewed keys are retained. This is a source correction, not additional setting scope.

The shared `SeededTagHierarchy.csv` remains tab-separated. Its 2,544 paths include every Core path plus 209 pre-existing optional-content paths outside this manifest's ownership. The manifest, rather than that multi-owner export, defines the Core installation contract.

## Existing databases and reruns

Core validates the complete legacy graph before saving the tag phase. Cycles, missing parents, duplicate same-path rows, conflicting ownership modules/types, retired claims, missing retained IDs and shared IDs across distinct keys block reconciliation with diagnostics. Nothing is merged or globally reparented by leaf name.

A unique existing exact-path row is bound read-only, retaining its ID and relationships. New rows receive creation provenance. Retained keys continue to resolve the same IDs after builder renames or reparenting, and those differences are reported. The recognised `Materials / Liquids / Water Soluable` legacy alias retains its ID without renaming builder data. Foreign-parent aliases are reported; intended missing paths are added without changing existing references. Such reports require explicit review before adopting legacy component/template references.

Fresh Core and its supported catalogue rerun install taxonomy before material/fluid/gas/terrain memberships. Six firearm/wadding tag settings are bound after IDs exist: `MusketRamrodTag`, `MusketCleaningRamrodTag`, `MusketUnjammingToolTag`, `MusketMatchCordTag`, `MusketIgnitionSourceTag`, `WadItemTagId`. `GunpowderMaterialId` additionally requires a unique installed gunpowder material. Missing/zero settings are filled, equal stock values retained, and differing custom or invalid values preserved with diagnostics. Early-firearm question validation additionally requires the seven configured IDs to resolve, plus its approved optional Sheath donor. Bootstrap accounts and world creation remain one-shot.

Useful repair-kit, cartridge-container, incense and offering references resolve canonical Core paths. Existing differing protected component tag references are retained and reported. Optional modern fuel equipment consumes `Materials / Liquids / Fuel`; moving this classification does not install equipment.

## Autobuilder selectors

Owned generated room templates serialize optional `FrameworkTagBindings` entries with independent `selector` text and `id`. Description matching, grouping, roads and adjacency continue using their original strings. Framework tag application uses the explicit IDs. Unbound text labels such as Physical Primary/Secondary remain textual; no extra framework tags are invented. Bindings survive room-template serialization and cloning. Missing explicit IDs fail at load; ambiguous unbound legacy names fail with a review diagnostic instead of choosing enumeration order.

New templates receive Useful ownership baselines. Untouched owned templates can refresh; renamed, builder-edited or unproven legacy templates are preserved and reported. Missing retained IDs and retired/conflicting claims block explicit reconciliation rather than replacing rows. The area template's original feature, adjacency and road strings and terrain-ID lists are retained.

## Dependencies and verification

The reduced baseline is Core → Time/attributes → SkillPackage → Human → preindustrial Combat → Chargen → Armageddon. Full package preflight composes native binding/anatomy/health/calendar/language/check validation with exact Core spanning-tag and Combat checks. Generic Item and AnimalButchery keep their real Useful component dependencies. Optional early firearms still require an approved Sheath donor; broad replay retains an optional Useful ordering hint for that branch. Agriculture/Cooking order after available skill seeders, without a Useful tag prerequisite. Broad replay still includes every enabled optional seeder once.

`CoreTagCatalogueTests`, terrain/material tests, Useful component/template tests and runtime `AutobuilderTagBindingTests` protect identity, references, corruption refusal and dependency boundaries. The native `core-foundations` lane in `tests/ArmageddonPreparedSeederNativeHarness` uses the normal owned MySQL wrapper, captures one new reduced base, seals SQL/descriptor read-only, restores it into a second owned database, verifies schema/content hashes, exercises supported Core rerun and full Armageddon install without Useful, and boots the isolated server. It does not truncate production replay or mutate historical fixtures. Consult its saved receipt for executed results; this document alone is no execution certificate.

Native reduced-base qualification exposed Hovering Light’s formerly hidden Animal-only `Head` shape assumption. Fresh binding can now fall back to Human’s existing `Head Back` attachment; existing aliases and builder renames retain their target IDs. Readiness validates this attachment before the resumable package writes. No new anatomy or runtime mechanic is created.
