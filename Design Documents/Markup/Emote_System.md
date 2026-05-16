# FutureMUD Emote System

## Scope

This document is the canonical reference for FutureMUD's emote markup as implemented by:

- `MudSharpCore/PerceptionEngine/Parsers/Emote.cs`
- `MudSharpCore/PerceptionEngine/Parsers/EmoteTokens.cs`

It covers engine-authored emotes, player-authored emotes, speech handling, and the current runtime behaviour of the parser. It does not cover the character-description markup or room-description markup; those live in separate design documents.

## Core Classes

- `Emote`
  The normal parser for engine-authored echoes. It expects internal numeric target references such as `$0`.
- `PlayerEmote`
  The player-facing variant. It still uses the same token engine, but it additionally resolves live target lookups such as `~guard` and `*sword`.
- `NoFormatEmote`
  Despite the name, this is not a separate markup language. It still scouts targets and still ultimately uses `string.Format` with the parsed token list. In current runtime behaviour it should be treated as an `Emote` with the same token support.

Related variants exist for language handling:

- `NoLanguageEmote`
- `FixedLanguageEmote`

## Authoring Rules

- Raw `{` characters are reserved. `Emote` rewrites recognised tokens into `{0}`, `{1}`, and so on before formatting.
- Internal target order matters. `$0`, `$1`, `$2`, etc. refer to perceivables passed to the constructor in that order.
- `forceSourceInclusion` prepends the source token if the emote text never explicitly included a plain `@`.
- The parser auto-closes an odd number of quote characters by appending a trailing `"`.
- Quoted speech is tokenised before the normal emote tokens.
- Player emotes are sanitised before parsing.

## Parsing Model

The parser resolves tokens into a single shared raw format string, then renders that string separately for each perceiver. That is why the same emote can show `you`, `your`, `him`, `herself`, or a full sdesc depending on who is viewing it.

The practical split is:

- engine-authored/internal tokens for code that already knows the exact targets
- player lookup tokens for pmotes, omotes, socials, and other free-form player text

## Engine-Authored Tokens

### Source Tokens

`@` refers to the emote source.

| Token | Meaning |
| --- | --- |
| `@` | source description, or `you` for the source viewer |
| `@!` | source objective pronoun |
| `@#` | source subjective pronoun |
| `@'s` | source noun possessive |
| `@!'s` | source possessive pronoun |
| `@#'s` | source possessive pronoun |

Notes:

- Current runtime accepts `@!` and `@#`, even though some older help text says source tokens cannot be modified.
- `@!'s` and `@#'s` both resolve to the possessive pronoun form.

### Target Tokens

Internal target tokens point at constructor-supplied perceivables by index.

| Token | Meaning |
| --- | --- |
| `$0` | description / `you` |
| `$0's` | noun possessive / `your` |
| `!0` | description without leading article / `you` |
| `!0's` | bare noun possessive / `your` |
| `&0` | objective pronoun / `you` |
| `&0's` | possessive pronoun / `your` |
| `#0` | subjective pronoun / `you` |
| `%0` | reflexive / `yourself` |
| `^0` | description, even for the target viewer |
| `^0's` | noun possessive, even for the target viewer |

The same forms work for any index: `$1`, `&2`, `!3's`, `^4`, and so on.

Notes:

- `^0` is the non-self form of a target token. It renders through `HowSeen` with `IgnoreSelf`, so the target viewer sees their own short description instead of `you`. Use it for echoes like body transformations where the player needs to see what they have become.

### First/Third-Person Alternates

These are used when one viewer should see a different literal string than everyone else.

| Token | Meaning |
| --- | --- |
| `verb|verbs` | source viewer sees `verb`, everyone else sees `verbs` |
| `$0|your|his` | target 0 sees `your`, everyone else sees `his` |

Notes:

- Bare `verb|verbs` is tied to the source.
- `$0|first|third` is the normal engine-authored targeted form.

### Plurality and Pronoun-Number Alternates

These are different systems.

| Token | Meaning |
| --- | --- |
| `&0|is|are` | chooses by whether target 0 is a single entity |
| `%0|stop|stops` | chooses by pronoun number: `you/they stop`, `he/she/it stops` |

That distinction matters for groups. A grouped perceivable can be plural even when its displayed noun phrase is not simply a normal pronoun case.

### Optional and Self-Collapse Tokens

| Token | Meaning |
| --- | --- |
| `$?2|on $2||$` | if perceivable 2 exists, emit `on $2`, otherwise emit the null branch |
| `$0=1` | if targets 0 and 1 are the same entity, collapse to reflexive wording |
| `$0=1's` | if targets 0 and 1 are the same entity, collapse to reflexive possessive wording |

Notes:

- `$0=1` produces reflexive-style output such as `yourself` or the target's reflexive pronoun when the two references are the same entity.
- `$0=1's` produces reflexive possessive output such as `your own`.
- The null-perceivable token reparses its chosen branch as a nested emote, so the branch text can itself contain normal emote tokens.

## Player Lookup Tokens

`PlayerEmote` adds live target lookup tokens based on the source character's normal targeting rules.

- `~...` targets characters
- `*...` targets items

The lookup portion can be a normal MUD target string such as `guard`, `2.guard`, `tall.man`, or `3.long.sword`.

### Player Lookup Forms

| Token | Meaning |
| --- | --- |
| `~guard` | character description / `you` |
| `~!guard` | objective pronoun |
| `~#guard` | subjective pronoun |
| `~?guard` | reflexive |
| `~guard's` | noun possessive |
| `~!guard's` | possessive pronoun |
| `~#guard's` | possessive pronoun |
| `*sword` | item description / `you` if appropriate |
| `*!sword` | item pronoun form |
| `*sword's` | item possessive |

The same first/third-person alternate syntax exists in player mode:

- `smile|smiles`
- `~guard|your|his`
- `*sword|your|its`

Notes:

- In player mode the parser only accepts one optional modifier before the lookup key: `!`, `#`, or `?`.
- For possessives, `!` and `#` both collapse to possessive-pronoun output.

## Speech Handling

Quoted speech such as `"hello there"` becomes a language token before the rest of the emote is parsed.

Current behaviour:

- If language output is permitted, the speech is rendered through the source's current language and accent.
- If the caller uses `PermitLanguageOptions.IgnoreLanguage`, the raw quoted text is left alone.
- If the caller uses one of the restrictive language modes, the speech becomes the relevant replacement such as muffled, choking, gasping, or clicking output.
- If speech is forbidden with `PermitLanguageOptions.LanguageIsError`, parsing fails.

## Culture Tokens

`EmoteTokens.cs` defines a regex for culture tokens in this format:

`&culture1,culture2:text if culture|fallback&`

However, current runtime does not actually apply `CultureTokenRegex` anywhere in `ScoutTargets`. In other words:

- the syntax is documented in code comments
- the regex exists
- the parser does not currently process it

Treat culture tokens as non-functional unless the runtime is changed.

## Examples

### Engine Authored Echo

```csharp
new Emote("@ smile|smiles at $0.", actor, target);
```

Typical output:

- source sees: `You smile at Bob.`
- target sees: `Alice smiles at you.`
- others see: `Alice smiles at Bob.`

### Possessives and Pronouns

```csharp
new Emote("@ pat|pats &0 on &0's shoulder.", actor, target);
```

Typical output:

- source sees: `You pat him on his shoulder.`
- target sees: `Alice pats you on your shoulder.`
- others see: `Alice pats him on his shoulder.`

### Optional Target

```csharp
new Emote("@ lock|locks $0 $?1|with $1||$.", actor, door, maybeKey);
```

Typical output:

- with a key: `Alice locks the iron door with a brass key.`
- without a key: `Alice locks the iron door.`

### Self-Collapse

```csharp
new Emote("@ point|points $0=0.", actor, actor);
```

Typical output:

- source sees: `You point yourself.`
- others see reflexive wording for the source rather than a repeated noun phrase

### Player Pmote or Omote

Player text:

```text
smile|smiles at ~guard and tap|taps ~!guard's spear.
```

Typical output:

- source sees first-person verbs and `you` forms when appropriate
- the guard sees `you`
- bystanders see third-person verbs and the guard's normal sdesc/pronoun forms

### Speech

```csharp
new Emote("@ say|says, \"Stay back.\"", actor);
```

The quoted speech is rendered as spoken language output, not as plain literal text, unless the caller intentionally suppresses language parsing.

## Runtime Caveats Worth Remembering

- `NoFormatEmote` is not presently a "skip emote markup" mode.
- Culture tokens are currently defined but not executed.
- Older help for player emotes is incomplete around source modifiers and some possessive forms.
- The canonical truth for supported tokens is `EmoteTokens.cs`, not the shorter command help text.
