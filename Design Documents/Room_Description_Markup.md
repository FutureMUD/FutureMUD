# Room Description Markup

## Scope

This document describes the inline markup language used in cell and room descriptions as implemented by:

- `MudSharpCore/Construction/CellDescription.cs`
- `FutureMUDLibrary/Framework/StringUtilities.cs`
- `MudSharpCore/Commands/Modules/RoomBuilderModule.cs`

It covers the text builders can place directly into cell names and cell descriptions. It does not cover character description markup or emote markup.

## Where This Markup Runs

`CellDescription.SubstituteDescriptionVariables` currently applies the room markup pipeline in this order:

1. `environment{...}` weather/light/time substitution
2. ANSI colour substitution
3. `@shop`
4. `check{trait,minvalue}{pass}{fail}`
5. `writing{language,script,...}{readable}{unreadable}`

Important consequence:

- the same substitution path is used for both the room name and the room description body

So builders can use the same inline markup in either place.

## Supported Markup

### `environment{conditions=text}...{fallback}`

This is the weather/light/time/season selector implemented in `CellDescription.cs`.

Form:

```text
environment{qualifiers=text}{qualifiers=text}...{fallback}
```

Current runtime supports:

- up to 8 conditional branches
- one optional fallback branch with no `=` inside it
- comma-separated qualifier lists inside each conditional branch

All qualifiers in a branch must match for that branch to fire.

Example:

```text
environment{night,dim=Moonlight leaks through the broken roof.}{rain=Rain drips steadily from the beams.}{The roof above is broken in several places.}
```

#### Time Qualifiers

Supported time keywords:

- `day`
- `night`
- `morning`
- `afternoon`
- `dusk`
- `dawn`
- `notnight`

`day` currently means morning or afternoon.

#### Season Qualifiers

Season matching is not hardcoded to a fixed English list. The parser compares the qualifier against the current regional climate season names.

That means custom climate season names are valid here.

#### Precipitation Qualifiers

Supported precipitation words are the strings recognised by `PrecipitationFromString`, including:

- `parched`
- `dry`
- `humid`
- `lightrain` / `lrain`
- `rain`
- `heavyrain` / `hrain`
- `torrentialrain` / `torrential` / `torrent` / `train`
- `lightsnow` / `lsnow`
- `snow`
- `heavysnow` / `hsnow`
- `blizzard`
- `sleet`

#### Recent Precipitation

Prefix a precipitation qualifier with `*` to check recent maximum precipitation instead of the current weather event.

Example:

```text
environment{*rain=Everything here still glistens with recent rain.}
```

Current runtime treats this as `highest recent precipitation >= requested level`.

#### Negation

Prefix any qualifier with `!` to negate it.

Examples:

```text
environment{!night=Sunlight still reaches the far wall.}
environment{!rain,!snow=The courtyard remains dry.}
```

#### Threshold Checks

Prefix light or precipitation qualifiers with `>` or `<`.

Current runtime semantics are:

- `>qualifier` means `>=` the threshold
- `<qualifier` means `<` the threshold

This applies to:

- precipitation levels
- light descriptions resolved through the light model

Examples:

```text
environment{>dim=Only the broadest outlines can be made out.}
environment{<bright=The corners remain in shadow.}
environment{>rain=Water runs in thin rivulets along the floor.}
```

#### Light Qualifiers

Light text is matched through the current light model, not through a hardcoded list inside `CellDescription.cs`.

In other words:

- the qualifier strings must match the light model's description names
- `>` and `<` compare against the minimum illumination threshold for that named description

Builder help currently prints the common stock list, but runtime truth comes from the active light model.

### `writing{language,script,...}{readable}{unreadable}`

This is the written-language markup implemented by `StringUtilities.SubstituteWrittenLanguage`.

Form:

```text
writing{language,script,skill=...,style=...,colour=...}{text if readable}{text if unreadable}
```

Language and script can be supplied by:

- numeric id
- name

Recognised attributes are:

- `skill`
- `style`
- `colour`
- `color`

Examples:

```text
writing{english,latin}{The label reads "Poison."}{something written in a familiar script}
writing{english,latin,skill=45,style=block,colour=red}{Danger: Do not drink}{a red warning label}
```

#### Runtime Behaviour

Current behaviour is:

- if the viewer has no language interface, the readable text is returned
- if the language or script entry is malformed or unknown, the readable text is returned
- if the viewer knows both the language and the script and meets the required skill, the readable text is shown
- if the viewer knows the script but not the language, the unreadable text is shown
- if the viewer knows the language and script but lacks the required skill, the unreadable text is shown
- if the alternate text is omitted, the engine uses `DefaultAlternateTextValue`

#### `skill` and `minskill`

Current runtime now supports both attribute names:

- `skill`
- `minskill`

They are treated as aliases and feed the same required-skill threshold.

So both of these work:

```text
writing{english,latin,skill=45}{readable text}{fallback text}
writing{english,latin,minskill=45}{readable text}{fallback text}
```

#### Attribute Operators

The attribute regex allows operators such as `=`, `>`, and `<`, but current implementation only reads the numeric or named value and ignores the operator itself.

So:

- `skill=45`
- `skill>45`
- `skill<45`

all currently end up behaving as though the value is simply `45`.

That is runtime truth, even though it is easy to assume otherwise from the syntax.

### `check{trait,minvalue}{pass}{fail}`

This is the trait-check markup implemented by `StringUtilities.SubstituteCheckTrait`.

Form:

```text
check{trait,minvalue}{text if trait >= value}{text if trait < value}
```

`trait` can be supplied by id or name.

Current runtime behaviour:

- if the viewer does not expose traits, the pass text is returned
- if the trait cannot be found, the pass text is returned
- if the difficulty value cannot be parsed, the pass text is returned
- if the viewer's trait value is `>= minvalue`, the pass text is returned
- otherwise the fail text is returned
- if the fail text is omitted, the failed branch becomes blank

Example:

```text
check{appraisal,35}{You notice the maker's stamp worked into the metal.}{At a glance it looks ordinary.}
```

### `@shop`

`@shop` is a simple literal replacement inside `SubstituteDescriptionVariables`.

Current behaviour:

- if `Cell.Shop` is set, it substitutes the shop name
- otherwise it substitutes `An Empty Shop`

Important nuance:

- this inline replacement only checks `Cell.Shop`
- later room-description notices also detect shop stalls and other coded room services

So `@shop` and the later "A shop is here..." notice do not use exactly the same detection path.

## Full Room Description Render Order

`CellFullDescription` builds the visible room output in this order:

1. room short name, already passed through the markup pipeline
2. admin info line for administrators, otherwise a blank spacer line
3. exits
4. processed builder-authored room description text
5. coded illumination, weather addenda, and `IDescriptionAdditionEffect` text
6. coded notices for shop, bank, auction house, property, jobs, estate, and morgue functions
7. trial notice, if applicable

After that broader look output continues elsewhere with characters and items rendered through their own long descriptions.

That means:

- builder-authored room markup controls the cell's own name and prose block
- engine-appended notices are a separate later layer
- character and item long descriptions, including pmote/omote influenced output, are not part of the room-markup system itself

## Weather and Light Addenda Versus Builder Text

The base builder-authored room description is processed first. Illumination text, weather addenda, and description-addition effects are appended after that.

Depending on account settings:

- coded additions may appear inline on the same paragraph
- or they may be emitted on fresh lines

So when you are writing a room description, do not assume your text is the final paragraph in the rendered output.

## Examples

### Time and Weather

```text
environment{dawn=Thin gold light spills through the eastern shutters.}{night=The room lies in soft darkness, broken only by starlight.}{The shutters are half-open.}
```

### Literacy and Script

```text
A brass plaque reads writing{english,latin,skill=30}{Property of the Guild Archive}{writing you cannot quite decipher}.
```

### Trait-Gated Detail

```text
check{forensics,55}{You spot a faint brown smear near the skirting board.}{}
```

### Shop Name

```text
The painted board above the door reads @shop.
```

## Practical Guidance

- Use `environment` for genuinely dynamic environmental prose, not for every sentence.
- Use `writing` when the text should be readable only to the right viewers; keep the alternate text natural enough that it still reads well in context.
- Use `check` for small observational details, not for major information gates that should be handled by separate mechanics.
- Prefer short, robust fallback text. Rooms should still read well when none of the specialised branches trigger.
- `skill` and `minskill` are both accepted for written-language thresholds.
