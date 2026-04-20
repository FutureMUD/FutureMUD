# Character Description System

## Scope

This document describes FutureMUD's generic character-description system as implemented by:

- `FutureMUDLibrary/Form/Characteristics/IHaveCharacteristics.cs`
- `FutureMUDLibrary/Form/Shape/IEntityDescriptionPattern.cs`
- `FutureMUDLibrary/CharacterCreation/ICharacterTemplate.cs`
- `MudSharpCore/Body/Implementations/BodyPerception.cs`
- `MudSharpCore/Body/Implementations/BodyCharacteristics.cs`

It covers:

- short, possessive, long, and full character descriptions
- the precedence rules between custom text, selected patterns, disguises, obscurers, and runtime addenda
- the generic description-pattern grammar used by sdescs and fdescs

Human-seeder specifics live in [Human Seeder Description Patterns](./Human_Seeder_Description_Patterns.md).

## Description Modes

FutureMUD uses the following description modes for characters:

| Mode | Runtime purpose |
| --- | --- |
| `Short` | Nominal sdesc, such as `a short, blue-eyed man` |
| `Possessive` | Possessive version of the short description, such as `your` or `a short, blue-eyed man's` |
| `Long` | Scenic presence text, such as `A short, blue-eyed man is standing here.` |
| `Full` | Detailed look text |

`Contents` and `Evaluate` exist on the enum for other perceivables, but the key character-description authoring targets are `Short`, `Possessive`, `Long`, and `Full`.

## Runtime Rendering Pipeline

### High-Level Precedence

When a character is rendered, the engine applies these layers in order:

1. top-level override effects
2. identity-obscurer overrides, where applicable
3. selected entity description pattern text, if one is chosen
4. raw custom text stored on the body/template
5. characteristic parsing
6. written-language substitution where that mode uses it
7. spacing, sentence, wrapping, and runtime addenda

### Override Effects

`BodyPerception.HowSeen` checks `IOverrideDescEffect` before the normal description pipeline. If an effect says it overrides the requested description type for that perceiver, and the perceiver can see the target, that override wins immediately.

This sits above normal pattern/custom-text rendering.

### Short Description

For `DescriptionType.Short`, the runtime flow is:

1. visibility checks and corpse redirects happen first
2. the engine chooses the base text in this order:
   - `IObscureIdentity.OverriddenShortDescription`
   - selected short-description pattern
   - stored short-description text
3. the text is parsed through `ParseCharacteristics`
4. the result is passed through `SubstituteWrittenLanguage`
5. spacing is normalised
6. proper-casing is applied when requested
7. `ProcessDescriptionAdditions` overlays dubs/admin name display and `ISDescAdditionEffect` text

`ProcessDescriptionAdditions` is also where account name-overlay settings and dubs can replace or extend the raw sdesc.

### Possessive Description

For `DescriptionType.Possessive`:

- self-viewers get `your` or `Your`
- everyone else gets the rendered short description, parsed again for characteristics, with `'s` appended

This means possessive mode is built from the short-description pipeline rather than maintained as separate author-authored text.

### Long Description

For `DescriptionType.Long`, the engine builds a scenic sentence from the short description plus context such as:

- current position
- riding / being ridden
- combat state
- `ILDescSuffixEffect` text

In practice this is the line that appears in room listings and look output when a character is present in the scene.

### Full Description

For `DescriptionType.Full`, the runtime flow is:

1. if the viewer cannot see the character, return `You cannot make out their features.`
2. choose the base text in this order:
   - `IObscureIdentity.OverriddenFullDescription`
   - selected full-description pattern
   - stored full-description text
3. run `SubstituteWrittenLanguage`
4. run `ParseCharacteristics`
5. append every applicable `IDescriptionAdditionEffect`
6. normalise spacing, sentence-case, and wrapping

This order differs slightly from short descriptions: full descriptions run written-language substitution before characteristic parsing.

## Pattern Selection Versus Raw Text

Both chargen templates and live bodies can carry:

- raw stored sdesc/fdesc text
- selected `IEntityDescriptionPattern` references

Current runtime prefers the selected pattern text when present:

- short descriptions use `_shortDescriptionPattern?.Pattern ?? _shortDescription`
- full descriptions use `_fullDescriptionPattern?.Pattern ?? _fullDescription`

That makes pattern selection the effective canonical source once a pattern is chosen.

## Obscurers and Disguises

Two different systems can change what is shown:

- `IObscureIdentity`
  Replaces the whole short or full description with an override string.
- `IObscureCharacteristics`
  Leaves the description pattern in place, but changes how individual characteristic variables render.

`BodyCharacteristics.GetObscurer` currently returns the last worn visible item whose `IObscureCharacteristics` component says it obscures that definition.

So if multiple worn items obscure the same characteristic, the last applicable one wins.

## Chargen Preview Behaviour

Chargen/template preview uses `ParseCharacteristicsAbsolute`, not the live-body obscurer-aware pipeline.

Important consequences:

- it operates on the selected characteristics passed into the template
- it does not use worn obscurers
- it does not need a live body instance
- it still resolves the same generic grammar

This is why chargen help and preview can show a faithful pattern expansion before the character exists as a live body in the world.

## Generic Pattern Grammar

### Characteristic Variables

The base forms are:

| Form | Meaning |
| --- | --- |
| `$var` | ordinary form |
| `$varbasic` | basic form |
| `$varfancy` | fancy form |

What "ordinary", "basic", and "fancy" mean depends on the specific characteristic value implementation. The generic system does not hardcode the prose; it asks the characteristic value for `GetValue`, `GetBasicValue`, and `GetFancyValue`.

### Obscurer-Aware Variable Branches

Form:

```text
$var[visible text][obscured text]
```

Behaviour:

- if the characteristic is not obscured, the first branch is used
- if it is obscured, the second branch is used
- if only one branch is supplied, only that branch exists

Branch placeholders:

| Placeholder | Meaning inside the branch |
| --- | --- |
| `@` | the characteristic text |
| `$` | obscuring item `Name`, lower-cased |
| `*` | obscuring item sdesc |

Examples:

```text
$eyecolour[@ eyes][eyes hidden behind $]
$hairstyle[@ hair][hair hidden beneath *]
```

### Default-or-Missing Checks

Form:

```text
$?var[present text][default-or-missing text]
```

Behaviour:

- if the characteristic is missing or still at its default value, the second branch is used
- otherwise the first branch is used

This is the normal way to gate things like hair style, facial hair, or optional seeded characteristics.

### Bodypart-Count Grammar

Form:

```text
%var[normal][n:text][n-m:text][x:text]
```

This grammar is only meaningful for `IBodypartSpecificCharacteristicDefinition` values.

Behaviour:

- if the owner has the characteristic definition's ordinary bodypart count, the `normal` branch is used
- otherwise up to three alternate branches can be matched by exact count or inclusive range
- if no alternate matches, the normal branch is reused

Branch placeholders inside `%var[...]`:

| Placeholder | Meaning |
| --- | --- |
| `@` | rewrites to `$var` |
| `%` | numeric count |
| `*` | wordy count |

Example:

```text
%eyecolour[$eyecolour eyes][1:one $eyecolour eye][0:no eyes]
```

### Articles

Forms:

```text
&a_an[content]
&?a_an[content]
```

Behaviour:

- `&a_an[...]` always applies `a` or `an` to the expanded content
- `&?a_an[...]` first checks whether the expanded content is grammatically plural; if so it leaves the content alone, otherwise it applies `a` or `an`

Examples:

```text
&a_an[$person]
&?a_an[$ears[%ears[$earsbasic][1:one-eared][0:earless] ][]$person]
```

### Pronoun-Number Alternates

Form:

```text
&pronoun|plural text|singular text&
```

Behaviour:

- plural text is used for indeterminate and non-binary pronoun-number handling
- singular text is used for the standard singular pronoun cases

This is separate from `&he`, `&him`, and similar direct pronoun helpers.

### Extra Variables

The generic parser also supports these extra variables:

| Variable | Meaning |
| --- | --- |
| `&he` | subjective pronoun |
| `&him` | objective pronoun |
| `&his` | possessive pronoun |
| `&himself` | reflexive pronoun |
| `&male` | gender class word |
| `&race` | race name |
| `&culture` | culture name |
| `&ethnicity` | ethnicity name |
| `&ethnicgroup` | ethnic group |
| `&personword` | culture person word |
| `&age` | age category |
| `&height` | exact height string via the unit manager |
| `&tattoos` | tattoo summary, or blank |
| `&withtattoos` | alternate tattoo summary, or blank |
| `&scars` | scar summary, or blank |
| `&withscars` | alternate scar summary, or blank |

Important distinction:

- `$height` is the relative-height descriptor characteristic
- `&height` is the exact measured height string

They are not interchangeable.

### Tattoo and Scar Helpers

The tattoo and scar helpers are generated dynamically from visible marks.

Current runtime:

- checks visible and exposed tattoos or scars
- honours special overrides where a tattoo/scar template provides one
- otherwise falls back to stock summary strings such as "scarred", "heavily scarred", "inked", and their `with...` variants

These helpers are useful in sdescs because they compress many visible marks into short readable phrases.

## Advanced Deferred Parsing

The parser also supports a deferred inner form using `!`, such as:

- `$!var[...]`
- `&!a_an[...]`

This leaves part of the markup for a later parsing pass instead of resolving it immediately. It is mostly useful in engine-authored nested templates and is not usually needed for ordinary builder-authored sdescs or fdescs.

## Examples

### Short Description Pattern

```text
&a_an[$haircolourbasic[@-haired ][]$person]
```

Typical expansion:

- `a black-haired man`
- `a grey-haired woman`

### Optional Hair Style

```text
$?hairstyle[&his hair is $haircolourfancy and styled into &?a_an[$hairstylefancy]][&his head is bald]
```

### Relative Height Versus Exact Height

```text
This $person is &height tall and $?height[$height relative to you][about the same height as you].
```

Typical expansion:

- `This woman is 172 cm tall and taller relative to you.`
- or `This man is 5'10" tall and about the same height as you.`

### Bodypart Count

```text
$ears[%ears[&his ears are $earsfancy][1:&he only has one ear, which is $earsfancy][0:&he has no ears]]
```

## Practical Guidance

- Keep sdescs concise. Use basic forms more often there than fancy forms.
- Use fancy forms in fdescs where extra prose pays off.
- Use `$?var[...]` for truly optional details instead of leaving awkward blanks in the sentence.
- Use `%var[...]` whenever the characteristic is tied to a bodypart count that might vary.
- Prefer `&?a_an[...]` when plurality can vary after expansion.
- When documenting or editing a specific seeded family, keep the generic rules here separate from the seeder-specific examples in the human-seeder extension document.
