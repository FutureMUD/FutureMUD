# Armageddon age categories: native mapping decision

20 October 2026. The recovered category ranges and Elf creation height are approved. **21 October owner update: Luke approved the recommended adult lifecycle mapping and separate recovered descriptive bands. Implementation remains pending; no age fields or chargen policies have been changed.** All eight new Race rows remain unavailable. This is a requirements gate, not a denied operation.

## Verified source contract

The [physical proposal](Armageddon_Physical_Proposal_20261019.md) contains the seven approved whole-MUD-year tables. In the supplied `codedump.c` (SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`), `age_class` at81048-81056 produces virtual Young1-19, Adult20-29, Mature30-40, Middle-aged41-60, Old61-80. Display callers clamp its fallback index6 to Ancient, so positive virtual ages81+ display Ancient. The120 array entry is a sentinel, not a maximum age. Virtual0 falling through to Ancient is a source edge-case defect and must not be adopted as an infancy rule.

`get_virtual_age` at81060-81100 uses integer `calendarYears * 100 / denominator`: Human100, both Elf profiles116, Dwarf220, Half-Elf108, Half-Giant180, Mul75. No Baby/Child/Youth subdivisions are recovered from these six descriptions. Source chargen limits are a separate20%-85% calculation against missing external `max_age` values (141380-141394); they cannot establish minimum playable ages or lifespans.

## Why copying six numbers is incorrect

Native `AgeCategory` has **Baby, Child, Youth, YoungAdult, Adult, Elder, Venerable** (`FutureMUDLibrary/Character/Heritage/IRace.cs:262-270`). Its six persisted Race fields are starts of Child, Youth, YoungAdult, Adult, Elder and Venerable; they are not six interchangeable display labels. Hydration and save are in `MudSharpCore/Character/Heritage/Race.cs:601-607,1267-1272`.

The native lifecycle has three juvenile stages and four adult stages. The recovered descriptions have one undivided Young band and five later descriptions. A one-to-one assignment would either discard juvenile stages or misclassify a recovered category. For example, assigning the six recovered starts to the six fields would make Human1 start Child,20 start Youth and30 start YoungAdult: neither the approved descriptions nor a justified maturation policy.

These categories have mechanics: group AI assigns Baby/Child/Youth to juvenile roles (`MudSharpCore/NPC/AI/Groups/GroupTypes/GroupAIType.cs:255-264`), NPC generation consumes category bounds, and body-form age translation preserves lifecycle progress (`MudSharpCore/Character/CharacterForms.cs:280-332`). They are not merely prose labels.

Stock `MinimumAgeChargen` returns `@ch.Race.YoungAdultAge`, while `MaximumAgeChargen` returns `@ch.Race.VenerableAge * 1.1` (`DatabaseSeeder/Seeders/ChargenSeeder/ChargenSeeder.cs:426-439`). Consequently changing lifecycle thresholds also changes stock playable-age policy unless deliberately separated. Approval of age descriptions does not approve those derived chargen limits. There is no maximum-lifespan Race field in this mapping; Venerable is open-ended.

## Approved adult translation, not yet applied

Preserve the six recovered descriptions independently of the mechanical lifecycle. Luke approved the following coarse native adult stages on21 October, including Mature and Middle-aged sharing a mechanical stage:

| Recovered description | Approved native mechanical stage |
| --- | --- |
| Young | Retain distinct Baby/Child/Youth; their subdivision is not recovered |
| Adult | YoungAdult |
| Mature | Adult |
| Middle-aged | Adult, retaining its distinct descriptive band |
| Old | Elder |
| Ancient | Venerable, open-ended |

This gives the following **approved adult lifecycle lower bounds**, before resolving the native boundary issue below. They are not installed values or playable-age limits.

| Profile | YoungAdult | Adult | Elder | Venerable |
| --- | ---: | ---: | ---: | ---: |
| Human | 20 | 30 | 61 | 81 |
| Both Elves | 24 | 35 | 71 | 94 |
| Dwarf | 44 | 66 | 135 | 179 |
| Half-Elf | 22 | 33 | 66 | 88 |
| Half-Giant | 36 | 54 | 110 | 146 |
| Mul | 15 | 23 | 46 | 61 |

The coarse mechanical translation with separate exact descriptions is approved. Baby/Child/Youth subdivisions and independent numeric chargen bounds remain undecided; the approval does not supply those values. Existing Human3/10 thresholds remain explicitly inactive scaffolding, not approved racial infancy/childhood policy. No scaling, new earliest playable age or maximum lifespan is proposed. Keep configured chargen policy separate from approved descriptive bands; do not silently derive new minimum/maximum playable ages from this table.

## Native boundary defect to address before applying thresholds

`Race` creates consecutive ranges sharing endpoints, for example Youth `[YouthAge, YoungAdultAge]` then YoungAdult `[YoungAdultAge, AdultAge]`. `RankedRange.FindRange` uses inclusive comparisons and returns the first matching range (`FutureMUDLibrary/Framework/RankedRange.cs:63-76`). Therefore native `AgeCategory(20)` would still return Youth if `YoungAdultAge=20`, despite `MinimumAgeForCategory(YoungAdult)` returning20. The same one-year mismatch affects every shared endpoint. Writing19 to obtain a runtime20 transition would make the published minimum, chargen and persistence contract wrong instead.

Recommend a narrow lifecycle-specific correction so persisted fields consistently mean inclusive category starts; do not change generic `RankedRange` semantics globally. Qualification must cover every exact start, preceding/following year, minimum getters, save/reload and builder changes. This existing defect has been identified by source inspection, not repaired or counted as a passed runtime check in this slice.

## Evidence and pending inputs

The focused heritage regression covers all six current inactive lifecycle fields across persistence reload and two reruns, deliberate builder age edits, later shared-Human changes not becoming new scaffold defaults, and all eight new Race rows remaining unavailable. It **passed** in full seeder run `20261020T120610Z-758ae8332326` (2048 passed,0 failed,3 platform-specific skips; stable source), including the full heritage class. Its first execution, `20261020T114959Z-6a4b088ff25c`, built successfully but was **INCONCLUSIVE** because sandbox VSTest could not connect to testhost within90 seconds; that attempt remains recorded and is not counted as a pass. This regression does not certify an approved native translation, MySQL cold boot or new gameplay. Independent source verification checked the supplied file checksum and all approved integer category boundaries through calendar year1000.

Parallel current-master tasks supplied separate breathing and sweat corrections, now integrated locally, including explicit approved Armageddon sweat defaults. Naming input was recovered read-only from the owner's approved Page after the text transfer was truncated; its implementation and provenance are recorded in [the naming note](Armageddon_Naming_Integration_20261020.md). Neither follow-up supplies juvenile thresholds or chargen age policy. The adult mapping was subsequently approved on21 October; its implementation and boundary repair remain outstanding. Publication remains last.

21 October subsequent approval of chat item5: ordinary creation minima at the mapped adult starts are approved (Human20, both Elves24, Dwarf44, Half-Elf22, Half-Giant36, Mul15), with explicit policy for younger exceptions. Upper creation bounds and juvenile subdivision values remain unspecified. The physical/appearance approach in that item is approved; it supplies no missing numerical values.
