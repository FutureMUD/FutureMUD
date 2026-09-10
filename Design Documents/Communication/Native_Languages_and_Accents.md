# Native Languages and Accents

## Character identity

Characters have one nullable native spoken language. New characters use an explicit selection, then an ethnicity default, then a culture default, then the highest raw known language skill (language ID breaks ties). Unknown defaults never grant a language. Characters without spoken languages have no native language.

Legacy characters infer native identity after loading all their languages and traits. The result is saved and does not change as skills improve. Forgetting the native language selects a replacement from the remaining known languages. Staff can use `setnativelanguage <target> <known language|auto>`; `languages` displays the result.

Each known language also stores an acquisition accent, separate from preferred and current accents. Legacy records use their preferred accent, then their most familiar known accent, then the acquisition resolver. Changing preference does not change the acquisition accent. Forgetting an accent retains this identity, so the character must reacquire it to satisfy mastery.

## Roles and acquisition

Accent roles are `Native` (0), `Foreign` (1), and `Fallback` (2). Accents can associate with any number of source languages. Associations describe a learner's native language, not additional languages in which the accent is spoken.

Automatic acquisition in a non-native language selects a Foreign accent associated with the native language, then a Fallback accent, then the heard accent if Native, then any Native accent, then any accent. Each tier uses ascending accent ID. Without accents, the character still knows the language. A scoped acquisition context carries the heard accent through skill branching, including psychic spoken-language comprehension; signed languages are unaffected.

For the native language, selected Native accents take precedence, followed by an available Native accent and then any available accent. Explicit template choices are retained. Automatic creation choices respect chargen availability progs. Normal gameplay acquisition does not treat chargen-only restrictions as learning restrictions.

Builder commands, within the usual editable-item workflow:

```text
accent set role Native|Foreign|Fallback
accent set associated <language>       # toggle a source-language association
culture set native <language|none>
ethnicity set native <language|none>
npc set native <language|none>
```

Simple NPC templates require explicit overrides to be known languages. Variable NPC overrides are applied only when the generated character knows that language. The existing AccentPicker storyboard has a `native` toggle to enable a native-language choice before accents; omitted XML settings default to disabled. Choosing `default` retains automatic inference. No new chargen stage is required.

## Mastery limit

`NativeAccentFamiliarityFloor` defaults to `Easy`. Until mastery requirements are met, Native-role accents of non-native languages cannot improve below that difficulty. The limit applies to stored acquisition and effective difficulty after related-accent bonuses.

Both conditions are required: raw language skill has reached its character-specific cap, and the acquisition accent has reached Automatic familiarity when `AllowAccentsToGetToAutomatic` is true, otherwise Trivial. Familiarity used for this prerequisite excludes related-accent bonuses. If the acquisition accent is itself Native, reaching the skill cap unlocks that accent first; other Native accents still require its mastery.

Existing better familiarity is preserved but cannot improve further before qualification. Native-language accents are exempt. New character starting accents obey the same limit. A later cap increase can prevent further improvement without taking away earned familiarity.

## Persistence and compatibility

`NativeLanguagesAndAccentRoles` adds nullable native-language references to characters and heritage, a nullable acquisition-accent reference on character-language rows, accent roles, and `AccentsAssociatedLanguages`. The migration converts old learner-pointer targets to Fallback before removing `Languages.DefaultLearnerAccentId`; other existing accents begin as Native. Existing familiarity and preferred accents are not rewritten. Missing optional NPC/chargen XML remains valid.

The C# learner-accent property, language builder setting, and FutureProg `language.defaultaccent` property are removed. Custom progs using that property require manual updates; use `language.accents` and accent roles/associations instead. Seeded progs and maintained seeder code must not depend on the removed property. New readable properties are `character.nativelanguage`, `chargen.nativelanguage`, `culture.nativelanguage`, `ethnicity.nativelanguage`, `accent.role` (text), and `accent.associatedlanguages` (language collection).

CultureToolkit uses explicit role metadata and resolved native bindings. The first declared native binding supplies the ethnicity default; culture defaults use resolved vernacular selectors where those identify a language independently of heritage. Other grants remain intact. Modern and other legacy source packs classify existing foreign accents and resolve only supported associations; ambiguous regional labels do not invent language bindings. Roles, associations, and heritage defaults preserve builder overrides through managed baselines.

The maintained blank snapshot includes the generated migration delta. Live refresh/import verification depends on a compatible local MySQL authentication setup; static migration and snapshot checks are separate from that verification.

## Implementation verification (2026-09-10)

Integrated PR #734's seeder ownership reorganization. Engine and seeder builds passed. The serial fast-test gate passed all 4,641 tests across nine projects (including final affected-suite reruns). EF reports no pending model changes; migration conversion ordering and blank-snapshot parsing are covered by database tests. Live migration/import and telnet gameplay were not exercised. A full snapshot refresh was attempted but blocked by the local MySQL client's unsupported auth_gssapi_client plugin; the snapshot uses the EF-generated idempotent delta instead.
