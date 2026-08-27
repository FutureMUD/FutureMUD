# Generic Editable Item Builder Workflows

Builder commands backed by `EditableItemHelper` or `EditableRevisableItemHelper` support safe bulk name changes:

```
<command> rename <match regex> <replacement text>
```

The match uses case-sensitive .NET regular expressions by default; use an inline expression option such as `(?i)` when case-insensitive matching is wanted. Quote an argument containing spaces. The command builds and displays every proposed old-to-new name before changing any item.

Names are trimmed, cannot be blank or entirely numeric, and are compared case-insensitively inside the helper's natural namespace. Most helpers use one catalogue-wide namespace. Helpers that intentionally partition their records retain that boundary: for example, spells and powers are scoped to a magic school, coins to a currency, properties to an economic zone, timezones to a clock, units to their system/type, and arena child records to their arena.

The validator compares the complete virtual final state, including untouched records. A swap or rename chain is therefore valid when the final state is unique. Any invalid regex, timeout, invalid result, or collision rejects the entire batch without mutating an item.

For revisable content, `Current`, `PendingRevision`, and `UnderDesign` revisions participate. Active revisions of the same logical ID may share a name; `Rejected`, `Revised`, and `Obsolete` history does not block the operation.

`set name <name>` follows the same validator for helper types whose name-setting command is `name`. A helper whose `Name` is represented by another command key uses that key instead; for example, timezone `Name` tracks its alias, so `set alias <alias>` is validated while `set name <display name>` continues to edit its display description.
