# Emote Markup
- The `Emote` class in `MudSharp.PerceptionEngine` provides a lightweight markup language for dynamically tailored messages.
- Prefer to use Emotes and Emote Markup anywhere you need to dynamically build echoes that involve characters or items.
- Emotes substitute placeholders at parse time so each viewer sees a grammatically correct perspective of the same event. 
- The string passed to an `Emote` **must not** contain raw `{` characters
- Targets referenced in the text must be supplied to the constructor in the same order.

## Source Token
`@` represents the source of the emote.

- `@` – short description of the source.
- `@#` – subject pronoun (he/she/they).
- `@!` – object pronoun (him/her/them).
- `@'s` – possessive form.
If the emote lacks `@`, setting `forceSourceInclusion` prepends the source description automatically.

## Referencing Perceivables (internal `$` tokens)
Internal tokens refer to perceivables passed to the constructor (`$0`, `$1`, …).

- `$0` – description of target 0 / “you”.
- `$0's` – possessive form.
- `!0` – description without article / “you”.
- `&0` – object pronoun (“him/her/them”) / “you”.
- `#0` – subject pronoun (“he/she/they”) / “you”.
- `%0` – reflexive (“himself/herself/itself”) / “yourself”.

## Player Lookup Tokens
When parsing free-form text from players, lookups use `~` for characters and `*` for items.
These forms accept the same modifiers as internal tokens and target strings such as `2.tall.man`.

- `~tall.man` – description / “you”.
- `~!tall.man` – object pronoun.
- `~#tall.man` – subject pronoun.
- `~?tall.man` – reflexive.
- `~tall.man's` or `~!tall.man's` – possessive (“man's” / “your” or “his” / “your”).

## First/Third Person Variants
Use `|` to supply alternative text for the referenced perceiver versus everyone else.

- `verb1|verb2` – first person for the source, third person for others (`@ smile|smiles`).
- `$0|your|his` or `~tall.man|your|his` – “your” for the target, “his” for others.

## Plurality and Pronoun Number
- `&0|is|are` – uses “is” when token 0 is singular, “are” when plural.
- `%0|stop|stops` – conjugates based on the pronoun number of token 0 (“stop” for you/they, “stops” for he/she/it).

## Optional and Conditional Tokens
- `$?2|on $2||$` – includes `on $2` only if perceivable 2 exists.
- `$0=1` – if tokens 0 and 1 refer to the same entity, outputs “yourself”; `$0=1's` gives “your own”.

## Speech and Culture
- Text inside quotes (`"spoken text"`) is parsed as speech and routed through language handling.
- Culture-specific text can be written as `&cultureA,cultureB:text|fallback&`.

## Examples
```csharp
new Emote("@ smile|smiles at $0.", actor, target);
// actor: "You smile at Bob."
// target: "Alice smiles at you."
// others: "Alice smiles at Bob."

new Emote("@ pat|pats $0 on &0 shoulder.", actor, target);
// actor: "You pat Bob on his shoulder."
// target: "Alice pats you on your shoulder."

new Emote("$0 %0|stop|stops here.", actor, group);
// group: "You stop here."
// others: "The guards stop here."
```