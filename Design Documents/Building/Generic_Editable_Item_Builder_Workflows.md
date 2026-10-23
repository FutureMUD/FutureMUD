# Generic Editable Item Builder Workflows

## The common editing workflow

Use `help <command>` (including an alias such as `help te`) to check the family's actual creation arguments and supported operations. `help on commands` shows built-in topics with subsystem, aliases and summaries. Built-in help comes from the installed engine; its source edit date is not recorded. Authored helpfiles retain their own edit history and an exact authored topic takes precedence over a built-in alias.

Most helper-backed families follow this sequence:

1. `<command> list` finds records. Use IDs when names are ambiguous.
2. `<command> edit <id|name>` selects an existing record, or `<command> edit new <creation arguments>` creates and selects one.
3. `<command> show` or bare `<command> edit` inspects the selection; `<command> show <id|name>` inspects another record without selecting it.
4. `<command> set <setting> <arguments>` delegates to that record's builder. `<command> set help` shows its settings.
5. `<command> close` (or `edit close`) clears the selection. It does not save a transaction, discard successful changes, or delete the record.

Quote each multiword operand when more arguments follow it. For example, with an existing clock ID 1:

```text
timezone edit new 1 AUD "Audit Standard Time" 2 30
timezone set name "Audit Time"
timezone show
timezone close
timezone list +AUD
```

Choose an unused alias and a real clock ID. This is a live editable record: successful setters change runtime state and queue normal saving; creation and some specialised editors save directly. There is no common transactional save/cancel command. Follow the multiline editor's own instructions to submit or cancel text. Cancelling that editor preserves the old text; it does not roll back earlier successful settings.

## Revisioned content and review

Revisioned families, such as item skins and item prototypes, keep logical IDs and revision numbers separately. Editing a current revision creates a working revision where supported; editing an existing draft reopens it. Explicit revision selectors, ownership restrictions and read-only protection still apply. A saved draft is not automatically the approved version used by the game.

Use `<command> edit submit [comment]` to validate and submit the selected under-design revision. A successful submission changes its status to pending review and closes the selection. Validation failures explain the missing configuration. Authorised reviewers use `<command> review list`, `review <id>` or the supported batch selectors, inspect the presented content, then accept or reject the review proposal. Do not assume every family uses identical selectors: vehicle routes provide their own explicit revision workflow, and room packages have additional activation and deletion checks.

For an authorised builder with a valid existing skin ID 42:

```text
itemskin edit 42
itemskin show
itemskin set sdesc a weathered travel coat
itemskin edit submit Updated appearance
itemskin review list mine
```

Replace the ID and description with appropriate content. Review permission is separate from permission to author a player skin. `review list mine` discovers pending work; it does not approve it. Closing an unfinished draft leaves it available for later editing. Deletion and obsolescence are separate lifecycle operations and may be unavailable or subject to dependency checks.

NPC spawners, languages and ordinary economy records use live editing, without submission/review. Check-template and decorator editors save immediately but activate after server restart. Body prototypes have structural safeguards when races or bodies use them. See the [body guide](Body_Prototype_Builder_Guide.md), [check/decorator guide](Check_and_Decorator_Editors.md), [item workflows](../Items/Item_System_Content_Workflows.md), and [room guide](Room_Building_Builder_Guide.md) for these exceptions.

## Catalogue search and domain filters

The market, legal, overlay-package and help-editor lists accept an exact numeric ID, `+keyword` and `-keyword`; multiple filters are combined. Keyword matching ignores case and searches names plus relevant displayed fields. Quote the whole keyword token if it contains spaces, for example `"+South Coast"`. Unknown tokens and missing filter operands are rejected. A valid filter with no matches produces an empty table.

| List | Additional exact filters |
| --- | --- |
| `market list` | `zone <id\|name>` |
| `mi list` | `market <id\|name>`, `template <id\|name>` |
| `mit list`, `mc list` | ID and keyword filters |
| `legal list`, `legal patrols <authority>`, `legal enforcements <authority>`, `legal classes <authority>` | ID and keyword filters after the authority where required |
| `legal laws <authority>` | `type <crime-type>` as well as ID/keyword filters |
| `show overlays` | `status <revision-status>`, `builder <account-id>`, `revision <number>` |
| `hedit list` | `category <name>`, `subcategory <name>`, `prog <id\|name>` |

For example, `market list zone "Port District" -closed` narrows the market catalogue, while `show overlays status UnderDesign builder 12` finds that account's drafts. Bank, property and auction helper lists retain their ID/name filters and explicitly reject unsupported custom filters.

Revisioned helper lists also support `all`, `mine`, `by <account>` and `reviewed <account>` where advertised. Their keyword matching includes the family's defined keywords/descriptions. `all` includes history; ordinary lists prefer active/recent revisions. These revision filters do not apply to non-revisioned families.

## Maintaining authored help

`hedit` changes authored database helpfiles, not built-in command source. Every mutation identifies its helpfile; it has no persistent edit selection. The following sequence uses a new multiword topic and existing boolean progs taking one character parameter named `CanReadBuilderNotes` and `CanReadMoreBuilderNotes`. Substitute your own prog names. Submit text when the multiline editor opens, or cancel to preserve the current text.

```text
hedit new "Builder Notes" Building Workflows "Local building conventions"
hedit keywords "Builder Notes" building workflow conventions
hedit category "Builder Notes" Building
hedit subcategory "Builder Notes" Workflows
hedit tagline "Builder Notes" Local building conventions
hedit name "Builder Notes" "Builder Handbook"
hedit prog "Builder Handbook" CanReadBuilderNotes
hedit text "Builder Handbook"
hedit extra add "Builder Handbook" CanReadBuilderNotes
hedit show "Builder Handbook"
hedit extra text "Builder Handbook" 1
hedit extra prog "Builder Handbook" 1 CanReadMoreBuilderNotes
hedit list category Building +handbook
```

Extra-text numbers are one-based and refer to the order in `hedit show`. `hedit extra remove "Builder Handbook" 1` asks for confirmation before deletion. `hedit prog "Builder Handbook" clear` removes the visibility rule. `hedit delete "Builder Handbook"` asks for confirmation before deleting the whole topic. There is no `hedit extra move` command. Use yellow for command syntax, cyan for names/types, green for values, red for errors, and reset colour after marked text. Help should include mandatory operands, accepted optional operands, scope of changes and any unsupported operations.

## Selecting, searching and cloning

Shared non-revisable lists support exact IDs, `+keyword` inclusion and `-keyword` exclusion against names. Additional filter forms are available only when the command family supplies a search handler. Unsupported and empty filters return guidance without selecting or changing anything.

Clone capability is explicit. `itemskin clone <source> <new name>` creates a separate private design owned by the caller, subject to source editing authority and the prototype's player-skin permission. Plane and bank cloning are unsupported and return an explanation. Revisable review commands require `review <id>` for one pending item, `review list mine` for discovery, or `review all by <account>` for a batch; an unrecognised selector never falls back to an arbitrary pending item.

## Focused builder workflows

`npcspawner set npc "City Guard" 2` sets a Multi spawner's template weight; replace `2` with `remove` to delete that entry. Weights must be positive and finite. `language set mutual "Neighbour Speech" Easy` sets the selected language's directional intelligibility relationship. `randomname set dice "Given Name" 1d2` changes the named element's generation count. Quote names containing spaces so subsequent operands remain separate.

`auxiliary clone <source> <new name>` creates a separately named combat action. Combat-message auxiliary restrictions are saved as a set, so repeated saves do not duplicate links and removals survive reload. Accepting combat-message deletion closes every loaded character's editor for that message; stale selections cannot edit it afterward.

Weapon-attack, trait-expression and FutureProg deletion remain unavailable because their persisted consumers require coordinated dependency handling. Bulk room deletion is also unavailable; use the existing per-room deletion workflow with its checks. These paths explain their limitations and leave content unchanged.

## Capability and session discovery

`<command> capabilities` reports whether a shared helper implements creation, cloning, deletion, custom filters and revisions. The flags come from the same delegates used for dispatch, including specialised item/NPC/craft/loot clone parsers. A true flag describes availability, not permission or a guarantee that the chosen target meets its prerequisites. Unsupported operations still refuse without changing content. Identity-only search callbacks are no longer advertised as custom filters.

`buildereditors` lists your open builder selections across families, including revision status and persistence semantics. Live edits take effect immediately and enter normal saving; individual editors may save directly or activate after restart. Revisioned edits retain their existing submission/review lifecycle. Closing a selection never discards successful changes.

Shared helper `undo` restores only your most recent successful single name-setting operation for that family, within 30 minutes and the current runtime session. For example, after `timezone set alias PortTime`, `timezone undo` can restore the previous alias. It refuses if the target has disappeared, the name changed again, current authority is insufficient, or the old name now conflicts. Other settings, bulk renames and creation have no generic undo. It is a compensating live edit, not a database transaction or recovery after a server restart.

## Preflight, references and revision comparison

`<command> validate [id]` runs read-only checks on a shared-helper target or the current selection. Revisable families also accept an explicit revision number and reuse their existing submission checks. Validation reports errors and warnings, does not submit or activate, and does not execute FutureProgs as a preview. A family without a validator says so; a warning-only report is not a complete gameplay certification. Magic-school preflight detects corrupt parent cycles.

Agriculture uses `field crop validate <id|name>`, `field woodland validate <id|name>` and `field operation validate <id|name>`. Crop/woodland checks cover timing, ranges and commodity material/tag/weight references. Harvest-apiary operations check multiplier, cost and outputs. Other operation types report that they have no preflight provider.

Administrators can use:

```text
builderrefs attack <id|name>
builderrefs expression <id|name>
builderrefs prog <id|name>
builderrefs banktype <id|name>
builderrefs item <id|name>
builderrefs skin <id|name>
```

These are typed, incremental reports. Every result names its coverage and each known consuming field. Attack coverage includes weapon types, racial natural attacks (including inheritance), combat messages and manual combat commands. Expression coverage includes attack damage/stun/pain profiles, derived traits and skill caps. Prog coverage includes hook prog lists, weapon attack progs, concrete bank account closing progs and authored-help visibility/extra-text progs. Bank-type coverage includes loaded bank accounts. Item/skin coverage includes skin definitions and/or loaded items, matched by logical ID across revisions.

Unloaded, serialized and other persisted consumers may be absent from these reports. Zero reported references never enables deletion. Existing specialised reports and dependency safeguards remain necessary.

`item diff [id [revision]]` and `itemskin diff [id [revision]]` compare the selected candidate against the same logical item's current approved revision, even when rejected revisions intervene. Both revisions must be visible to the caller. Single-item review also presents the comparison. Skin fields are compared explicitly. Item comparisons cover scalar fields, material/health/group references, component revision references, variables and extra descriptions; tags, planar settings and component internals require normal show/review. Other revision families explicitly report that field comparison is unavailable. Administrators also see the applicable partial reference report; non-admin skin authors do not receive admin reference data.

The shared diagnostics are also forwarded by the item, NPC, craft and project command wrappers, retaining their existing privilege checks.

## Large catalogue controls

Shared editable/revisable helper lists accept output options alongside their existing domain filters:

```text
item list +sword --page 2 --limit 50 --sort name
npcspawner list --count
bank list --limit 100 --sort id --descending --export csv
```

- `--limit 1..1000` bounds the rows formatted; `--page` is one-based and defaults to 50 rows when no limit is supplied.
- `--count` counts matching visible records without formatting rows.
- `--sort id|name` sorts consistently; revisable helpers additionally accept `revision`. `--descending` reverses the primary key and ties use IDs/revisions.
- `--export csv` sends quoted CSV through the existing output channel. It strips ANSI/MXP and neutralises spreadsheet formula prefixes. It exports the selected page; it does not create a server file.
- Output reports “Showing N of M” matching records. Omitting limit/page retains the existing full-list behaviour and client pager. Invalid, duplicate or incomplete options are refused.

These options apply to helper-backed lists; bespoke market/legal/help-editor lists retain their documented domain filters. Generic filters now compose before materialisation, except when a legacy custom filter requires a list. Counting still visits the matching catalogue and sorting still requires its contents; this is not database-side paging. The regression suite measures a synthetic 25,000-record catalogue and protects the bounded row-formatting work without asserting machine-dependent timing thresholds.

## Executable help examples

Help metadata can supply concrete examples and their world prerequisites. These are published in built-in help and consumed by behavioural tests through the actual registered command and permission gate. Timezone examples exercise quoted target selection, delegated setters and close semantics; malformed operands and insufficient authority are tested separately. Colour reset checks are independent of command execution. This is incremental coverage, not a claim that every authored helpfile or built-in example can be replayed without a fixture.


## Bulk renaming

Builder commands backed by `EditableItemHelper` or `EditableRevisableItemHelper` support safe bulk name changes:

```
<command> rename <match regex> <replacement text>
```

The match uses case-sensitive .NET regular expressions by default; use an inline expression option such as `(?i)` when case-insensitive matching is wanted. Quote an argument containing spaces. The command builds and displays every proposed old-to-new name before changing any item.

Names are trimmed, cannot be blank or entirely numeric, and are compared case-insensitively inside the helper's natural namespace. Most helpers use one catalogue-wide namespace. Helpers that intentionally partition their records retain that boundary: for example, spells and powers are scoped to a magic school, coins to a currency, properties to an economic zone, timezones to a clock, units to their system/type, and arena child records to their arena.

The validator compares the complete virtual final state, including untouched records. A swap or rename chain is therefore valid when the final state is unique. Any invalid regex, timeout, invalid result, or collision rejects the entire batch without mutating an item.

For revisable content, `Current`, `PendingRevision`, and `UnderDesign` revisions participate. Active revisions of the same logical ID may share a name; `Rejected`, `Revised`, and `Obsolete` history does not block the operation.

Every matched revisable item must pass the helper's normal edit-ownership check and must not be read-only. Non-administrators may rename only `UnderDesign` revisions; open a fresh revision of approved content before using bulk rename. Administrators retain the existing maintenance command for active revisions. If any match fails these checks, the entire batch is rejected. Visible public items remain in the collision-validation universe even when the caller cannot edit them.

`set name <name>` follows the same validator for helper types whose name-setting command is `name`. A helper whose `Name` is represented by another command key uses that key instead; for example, timezone `Name` tracks its alias, so `set alias <alias>` is validated while `set name <display name>` continues to edit its display description.

Item prototype names are an exception: they represent shared nouns rather than unique identifiers. Noun edits, clones, and bulk noun renames permit duplicate and numeric nouns, while rejecting blank text. Item `UniqueName` retains its separate uniqueness and non-numeric rules.
