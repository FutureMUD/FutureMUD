# ARM-01 owner decision summary

27 September 2026. Design/audit package only; production implementation is not authorised.

## Selected by Luke for this design

- Extend an existing configurable capability where feasible; never implement elemental
  schools as different capability types. The audit supports optional policy on `skilllevel`.
- Support both per-spell and shared-tradition native proficiency through configuration.
- Separate acquired spell/controlled grade from proficiency and current bodily permission.
- Include incantations, quiet/area variants, scrolls/substances and both charged/focus
  wand/staff roles in the release design.
- Accept spell names or formulas through equivalent casting rules.
- Permit practice to advance proficiency and controlled grade with full energy payment
  and shared limits.
- Permit suitably trained non-casters to activate charged portable items.
- Author testable provisional numerical tables instead of pretending historical numbers
  are FutureMUD balance.

These decisions came from the conversation and the subsequently approved document-package
plan. They do not ratify the entire proposed roster, numeric profile or every design detail.

## Proposed decisions awaiting approval

| Decision | Concrete proposal | Approval consequence |
| --- | --- | --- |
| Stock balance | Seven grades, thresholds 20/40/55/70/85/95, 25% eligible mastery chance, 600-second mastery interval; costs and all other units in the design. | Required before implementing preset balance. |
| Stock proficiency | Individual spell skills by default; configurable shared-tradition binding. | Determines installed skills, not runtime support. |
| Candidate roster | Every one of the 152 supplied magic entries plus explicitly marked Wardcraft and land-repair additions. Per-entry generic names, effects, memberships and edges are in the register. | Approve each included adaptation; source count is not completion percentage. |
| Sorcerer admission | Curated explicit cross-school subset; historical comments are evidence, not an imported complete learnlist. | No automatic access to every elemental spell or passive recharge. |
| Acquisition | One canonical learned-spell record even when individual skills are used; idempotent enrolment and capability-scoped edges. | Avoids two competing acquisition models for the two proficiency configurations. |
| Portable list | Only explicit supported spell/carrier pairs, with separate missing production/charged-item integration. | Approval does not make currently unsupported snapshot effects portable. |
| Scroll learning / production improvement | Disabled stock defaults, explicit configuration if later enabled. | No implicit learning through possession, activation or manufacturing. |
| Omissions/substitutes | `proposed_defer` and `proposed_adaptation` rows remain unapproved. | Exact gap implementation cards may be issued only for an approved roster. |
| ARM-02 | Foundation slice in its own proposed brief. | Requires contract/brief approval before production edits. |

## Evidence limitations and retained gate

The historical second-pass reference contains 152 unique magic skill IDs. External guild
learn-tree files are absent, so exact guild trees and complete Sorcerer membership cannot
be recovered from it. The available `codedump.c` matches sampled offsets cited as
`codedump(1).c`; the latter exact filename is absent and byte identity to it is unproved.
Some reference summaries confuse scheduled cleanup with creation and point at unrelated
helper snippets. Source-specific claims require the actual named function, not that
summary alone; affected register rows explicitly distinguish adaptations and uncertainty.

The live checkout records the 04C checkpoint/deadline correction and a historical passing
run. This assignment neither re-reviews that correction nor closes its final-preset gate.
No separate supplied pipeline-status attachment was located in the bounded source search.

See [the design](Armageddon_Casting_Design.md), [register](Armageddon_Repertoire.md),
[evidence](Armageddon_Casting_Integration.md) and [ARM-02 proposal](ARM02_Configurable_Casting_Implementation_Brief.md).
