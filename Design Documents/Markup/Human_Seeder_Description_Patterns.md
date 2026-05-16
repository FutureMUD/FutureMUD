# Human Seeder Description Patterns

## Scope

This document extends [Character Description System](./Character_Description_System.md) with the human-specific seeded description grammar and vocabulary implemented by:

- `DatabaseSeeder/Seeders/HumanSeederCharacteristics.cs`
- `DatabaseSeeder/Seeders/HumanSeeder.cs`
- `MudSharpCore/Form/Characteristics/CharacteristicValue.cs`
- `MudSharpCore/Form/Characteristics/MultiformCharacteristicValue.cs`
- `MudSharpCore/Form/Characteristics/ColourCharacteristicValue.cs`
- `MudSharpCore/Form/Characteristics/GrowableCharacteristicValue.cs`

It is intentionally seeder-specific. The generic parser rules live in the main character-description document.

## Seeded Human Variables

The human seeder defines these description variables:

| Variable | Pattern / aliases | Notes |
| --- | --- | --- |
| `eyecolour` | `^eyecolou?r` | bodypart-specific, ordinary count 2 |
| `eyeshape` | `^eyeshape` | bodypart-specific, ordinary count 2 |
| `nose` | `^nose` | bodypart-specific, ordinary count 1 |
| `ears` | `^ears` | bodypart-specific, ordinary count 2 |
| `haircolour` | `^haircolou?r` | colour-backed |
| `facialhaircolour` | `^facialhaircolou?r` | colour-backed, seeded for male usage |
| `hairstyle` | `^hairstyle` | styleable / growable |
| `facialhairstyle` | `^facialhairstyle` | styleable / growable, seeded for male usage |
| `skincolour` / `skintone` | `^skin(colou?r|tone)` | colour-backed alias pair |
| `frame` | `^frame` | weighted by body build progs |
| `person` | `^person` | seeded person-word vocabulary |
| `distinctivefeature` | `^(distinctive)?feature` | optional seeder choice |

## Seeded Value Model

The human pack relies on the generic `GetValue`, `GetBasicValue`, and `GetFancyValue` surfaces, but the concrete meaning depends on the characteristic value class.

### Standard and Multiform Values

For the helper used by `AddCharacteristicValue(id, definition, name, value, additionalValue, ...)`:

- ordinary form = `Name`
- basic form = `Value`
- fancy form = `AdditionalValue`

That is the key rule behind patterns like:

- `$frame` -> `burly`
- `$framebasic` -> `muscular`
- `$framefancy` -> `broad and squat, with a thickly muscular frame...`

### Colour-Backed Values

For colour characteristics such as eye colour, hair colour, facial hair colour, and skin colour:

- ordinary form = colour `Name`
- basic form = colour basic enum name, lower-cased
- fancy form = colour fancy string

So the same value can expose:

- `$eyecolour` -> `blue-grey`
- `$eyecolourbasic` -> `blue`
- `$eyecolourfancy` -> `a cold, pale blue-grey`

The exact strings depend on the seeded colour library.

### Styleable / Growable Values

Hair styles and facial hair styles use a growable/styleable value model, but the description surfaces still map cleanly:

- ordinary form = `Name`
- basic form = `Value`
- fancy form = the prose portion stored inside `AdditionalValue`

That is why seeded patterns can use:

- `$hairstyle` for the plain named style
- `$hairstylebasic` for compact sdesc wording
- `$hairstylefancy` for richer fdesc prose

## Person Word Seeding

`person` is not a single static noun. The seeder adds many weighted, prog-gated values through `AddPersonWord(name, basic, prog, weight)`.

Important consequences:

- the ordinary form is the seeded surface noun such as `woman`, `maiden`, `geezer`, or `child`
- the basic form groups those into a more stable family such as `woman`, `man`, `person`, `youth`, or `old woman`
- the fancy form is blank for these values
- the associated FutureProg controls which age/gender bucket can receive the word
- the weight controls how likely that term is when the pack chooses among eligible values

Examples:

- `woman` and `lady` both map to a basic form of `woman`
- `maiden` also maps to `woman`, but is gated to young-woman logic
- `geezer` maps to `old man`

### Extra Person Words

If the seeder answer `includeextraperson` is enabled, additional informal words are added, such as:

- `dude`
- `gal`
- `wench`
- `stud`
- `punk`

If that option is disabled, those looser-tone values are not seeded at all.

## Optional Distinctive Features

`distinctivefeature` only exists if the seeder answer `distinctive` is enabled.

When it is disabled:

- the definition is not seeded
- any patterns that rely on `$distinctivefeature...` should be considered part of the "distinctive-enabled" branch of the stock pack, not universally available runtime vocabulary

## Stock Human Pattern Conventions

The human pack follows a few strong conventions.

### 1. Sdescs Stay Short and Searchable

Stock sdesc patterns usually combine:

- one anchor feature
- one supporting feature or summary mark
- a person word

Typical shapes include:

```text
&a_an[$eyeshape[%eyeshape[$eyeshape-eyed][1:one-eyed][0:eyeless] ][]$person] with $eyecolour[%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]][$]
&a_an[$frame[@ ][]$person] with $eyeshape[%eyeshape[$eyeshape eyes][1:one $eyeshape eye][0:no eyes]][$]
&a_an[$haircolourbasic[@-haired ][]$person]
&a_an[&tattoos $person]
&a_an[$distinctivefeaturebasic[@ ][]$person] &withscars
```

The goal is discoverability first. The pack avoids stuffing full prose into sdescs.

### 2. Basic Forms Carry the Sdesc Load

The seeded human sdescs lean heavily on:

- `$...basic`
- compact `%...` branches
- short mark summaries such as `&withscars` and `&withtattoos`

This keeps sdescs readable in room listings and easier to target by keyword.

### 3. Fancy Forms Belong in Fdescs

The stock full descriptions use fancy values far more often:

- `$framefancy`
- `$eyecolourfancy`
- `$nosefancy`
- `$hairstylefancy`
- `$distinctivefeaturefancy`

That is the basic style split of the seeded pack:

- basic for short descriptions
- fancy for full descriptions

### 4. Relative Height Uses Both Exact and Comparative Grammar

The stock human fdescs use both height systems together:

```text
This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you].
```

That pairing is deliberate:

- `&height` gives the exact measured height string
- `$?height[...]` gives relative wording only when the relative-height descriptor is not the default

### 5. Bodypart-Count Grammar Is Used Aggressively

Eyes, ears, and nose all use `%var[...]` branches so the stock pack still reads naturally when the body is damaged, missing parts, or otherwise unusual.

Representative examples:

```text
%eyecolour[&he has $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][2-3:&he has % $eyecolourbasic $eyeshape eyes that are $eyecolourfancy][1:&he has a single $eyecolourbasic $eyeshape eye that is $eyecolourfancy][0:&he has no eyes, only empty sockets]
%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears, just scars where the ears should be]
%nose[$nosefancy][0:a gaping hole where &his nose used to be]
```

### 6. Facial Hair Is Explicitly Gated

Facial hair patterns are not simply "sometimes blank". The stock pack uses different applicability progs:

- `IsHumanoidFemale`
- `IsHumanoidNonFemale`

So male and non-female branches can carry facial hair clauses while female branches omit them entirely.

### 7. Distinctive Features Are Gated Twice

Distinctive features are controlled by:

- seeder-time installation choice
- actual characteristic presence at runtime

The pack uses both plain and optional forms depending on the pattern:

- `$distinctivefeaturebasic[@ ][]$person`
- `&he has $distinctivefeaturefancy.`

If you extend the pack, be deliberate about whether a distinctive feature is meant to be core to the sentence or an optional flourish.

## Representative Stock Patterns

### Compact Sdesc

```text
&a_an[$skincolour[@-skinned ][]$person]
```

Typical output:

- `a dark-skinned woman`
- `a pale-skinned man`

### Scar-Aware Sdesc

```text
&a_an[$eyecolour[%eyecolour[$eyecolourbasic-eyed][1:one-eyed][0:eyeless] ][]$person] &withscars
```

Typical output:

- `a green-eyed man scarred across the face`
- `a one-eyed woman with prominent scars`

### Full Description Skeleton

```text
This $person is &a_an[&male] &race that is &height tall and $?height[$height relative to you][about the same height as you]. You would describe &him as $frame, as &he is $framefancy.
```

This shows the stock rhythm:

- person word
- race/gender class
- exact height
- comparative height
- compact frame label
- fancy frame prose

### Hair and Facial Hair Gating

```text
$hairstyle[$?hairstyle[&his hair is $haircolourfancy, and has been styled so that &he has &?a_an[$hairstylefancy]][&his head is bald, with no hair at all]][You cannot tell what sort of hair style or even hair colour &he has because &he is wearing $].
$?facialhairstyle[&he has &?a_an[$facialhairstyle], which is $facialhaircolourfancy][&he does not have any facial hair, with a clean, smooth chin].
```

This is a good example of stock human style:

- the outer `$hairstyle[...]` handles obscuring items
- the inner `$?hairstyle[...]` handles bald/default versus styled hair
- facial hair uses optional presence grammar instead of forcing a blank noun phrase

## Guidance for Extending the Human Pack

- Keep the generic grammar rules in the generic character-description doc. Put only human-specific conventions here.
- Preserve the ordinary/basic/fancy split. If a new value needs rich prose, put it in the fancy surface and keep the basic surface short.
- Add new bodypart-specific definitions with a sensible ordinary count, then update patterns to use `%var[...]` instead of assuming the normal anatomy.
- Keep sdescs short enough to remain good targeting text in busy rooms.
- Prefer full descriptions for elaborate prose and sentence flow.
- If you add new informal person words, wire them through the `includeextraperson` choice unless they are meant to be part of the baseline tone.
- If you add new gendered or age-gated person words, follow the existing prog-gated approach rather than hardcoding assumptions in prose.
- If you add a new optional descriptive family similar to `distinctivefeature`, decide up front whether it should affect both sdesc and fdesc pattern pools.
