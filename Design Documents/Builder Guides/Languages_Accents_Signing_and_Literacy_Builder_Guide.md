---
slug: languages-accents-signing-and-literacy
category: Characters and bodies
summary: Build spoken and signed languages, regional accents, scripts and routes to literacy. Give a bilingual ferry port its voices, deck signals, passenger notices and handwritten manifests.
---

# Languages, accents, signing and literacy

A passenger can recognise the pilot’s accent, follow her directions and still need someone to read the departure board. Another passenger can read the board and converse with a signing deckhand while missing every shouted announcement. Build the language abilities separately and those encounters follow naturally.

Start with [speech](#spoken-languages), [accents](#accents-and-native-language), [signing](#signed-languages) and [literacy](#scripts-and-literacy). The [bilingual ferry port](#worked-example-the-bilingual-ferry-port) puts all four into play. Commands containing angle brackets need an installed record from your world. Quote multiword names where a command takes several arguments.

## What a character knows

| Builder decision | Record that carries it | Consequence in play |
| --- | --- | --- |
| Which languages can this person speak? | One spoken language linked to a skill for each independently learned language | Having both skills lets a pilot switch languages. A directional intelligibility link offers a listener a chance to understand a relative of a language they know. |
| How does their speech sound? | An accent belonging to the language being spoken, with a role, group and familiarity | A local can recognise the speaker’s regional accent. A visitor might hear the same rhythm without being able to place it. |
| What can they sign or understand visually? | A signed language with its own linked skill, optional varieties and at least one suitable articulation profile for production | An injured signer can retain comprehension even while their current body cannot make the signs. |
| Which writing systems can they recognise and use? | A script linked to a knowledge, general literacy and the language of the particular text | Two languages can share one script. Someone who recognises the letters can still struggle with the words. |
| Who starts with each ability? | Culture and ethnicity defaults, chargen skill and knowledge grants, role training or NPC template skills | Upbringing can supply a home language, a profession can require a second and a clerk can learn the local handwriting tradition on the job. |

**A linked skill is an acquisition path.** When a character gains the trait, the engine learns languages linked to it. Give independently learned spoken and signed languages different skills. Reusing a skill intentionally couples their acquisition and progression. A script instead uses a **knowledge**. Awarding that knowledge also grants its linked scripts. Use separate knowledges when schooling in one script should leave another unfamiliar. [Skills, checks and progression](Skills_Checks_and_Progression_Builder_Guide.md) · [Playable peoples and character creation](Character_Creation_Builder_Workflows.md)

## Spoken languages

A spoken language links a skill to a sentence-difficulty model and gives outsiders a description of its sound. Build or identify the skill first. Use `show languages` to see the installed names, IDs, traits, models and obfuscation factors. Open with `language edit new "<name>" "<trait>"` or `language edit <name or ID>`. Use `language show <name>` to inspect it and `language close` to finish.

| After `language set` | What the setting does |
| --- | --- |
| `name <name>`, `trait <trait>` | Set the catalogue name or linked skill. Decide which people should learn this together before changing a link in an established world. |
| `unknown <text>` | Supply the phrase heard by someone unable to identify the language. Write something an outsider could actually hear, such as “a language with long vowels and clipped endings”. |
| `model <name or ID>` | Select an installed sentence-difficulty model. The Simple model rates text Automatic. A WordList model can use both the hardest word and the longest sentence to rate an utterance. The language editor selects the model. |
| `obfuscation <percentage>` | Set how much text is obscured per degree of failure. A fresh language uses 20%. Increasing it makes a partial failure more costly. |
| `mutual "<other language>" <difficulty>`, `remove "<other language>"` | Set or remove a **listener-to-speaker** link. Edit the language the listener knows. The difficulty is the minimum for that route to understanding. Set the reverse direction separately if it fits the setting. |

An ordinary listener first needs to hear the speaker. Volume, proximity and local audio conditions affect the listen check. The engine then checks comprehension using the language trait, the sentence rating and accent familiarity. If someone fails the direct check in an unknown language, a language they already know can provide another route. Results can include a recognisable accent with missing words, the sound of an unidentified language, or no intelligible speech at all. **Knowing two languages** lets the speaker deliberately choose either one. **Mutual intelligibility** models partial understanding and gives no entitlement to speak the other language.

## Accents and native language

An accent belongs to the **language being spoken**. A River Tongue speaker’s accent when speaking Harbour Speech belongs to Harbour Speech. Associate it with River Tongue as the learner’s source language. A Reedbank regional accent *within* River Tongue belongs to River Tongue.

| Accent role | When to use it |
| --- | --- |
| Native | A regional or social variety of the language among its native speakers. More than one Native accent can belong to one language. |
| Foreign | A learner accent associated with one or more source languages. It can be chosen automatically when someone with a matching native language acquires this language. |
| Fallback | A general learner accent when no source-specific Foreign accent matches. Creating a new spoken language also creates a Fallback accent called **foreign**. |

The **native language** identifies one *known* spoken language. An explicit chargen choice takes precedence, followed by the ethnicity’s default, the culture’s default, and then the highest raw known language skill, with ties resolved by language ID. Someone with no spoken language can have no native language. Set the upbringing default with `culture set native <language>`, or an ancestry-specific exception with `ethnicity set native <language>`. Each accepts `none`. Grant the skill as well. On the AccentPicker storyboard, `chargen set native` toggles whether the applicant can choose. For a live correction, `setnativelanguage <character> <known language or auto>` sets the identity.

A character can have three different answers to “which accent?” The **acquisition accent** records how they learned the language. The **preferred accent** is what they usually choose. The **current accent** is the one they are using now. For a non-native language, automatic acquisition tries an associated Foreign accent, then a Fallback, then a heard Native accent, another Native accent, and finally any accent. The lowest ID breaks ties within a tier. An accentless language can still be learned and spoken.

Use `show accents "<language>"` for IDs, then `accent edit new "<accent name>" "<language>"`, `accent edit <ID>`, `accent show <ID>` and `accent close`. Names need only be unique within the language. IDs avoid ambiguity when several languages have a “foreign” accent.

| After `accent set` | Builder decision |
| --- | --- |
| `name <name>`, `description <text>` | Give the accent a recognisable name and explain its sound in prose. |
| `suffix <text>`, `vague <text>` | Write two endings that fit a speech echo. A familiar listener can hear “with a North Quay lilt”. Someone unfamiliar might hear “with a rising cadence”. |
| `difficulty <difficulty>`, `group <group>` | Choose how difficult the unfamiliar accent is and which other accents give a familiarity benefit. Use a deliberate group name for each related family. |
| `role Native`, `role Foreign`, `role Fallback` | Set how automatic accent acquisition uses the record. |
| `associated "<source language>"` | Toggle the learner’s source-language association. Inspect the accent before repeating the command. |
| `chargen <prog>` | A Boolean prog taking one chargen controls availability during creation. Live learning has its own rules. |

**Sounding local takes practice.** With the usual settings a non-native speaker can improve familiarity with a Native accent only as far as Easy until they reach their own raw language-skill cap and master the acquisition accent. Mastery means Automatic familiarity, or Trivial when automatic familiarity is disabled. Familiarity with related accents can help everyday understanding without satisfying that mastery requirement. `accents "<language>"` shows the acquisition and preferred markers. `preferaccent "<accent>" "<language>"` changes preference. `speak "<language>" "<accent>"` selects how to speak now. If you need to tune the checks, start with `SpokenLanguageSpeakCheck` for expression, `LanguageListenCheck` for audibility and `SpokenLanguageHearCheck` for understanding. Signed expression and comprehension have their own `SignedLanguageExpressCheck` and `SignedLanguageUnderstandCheck`.

## Signed languages

A signed language has its own vocabulary, skill and regional varieties. Give it an independent record even in a community where everyone also speaks the same language. Receiving a live signed message requires sight of the signer. Producing one requires both knowledge and a suitable body. `signto` addresses a particular visible person, but other observers who can see the exchange can also understand it.

Use `signedlanguage list`, `signedlanguage show <name>`, `signedlanguage edit new "<name>" "<trait>"`, `signedlanguage edit <name>` and `signedlanguage close`. The `name`, `trait`, `model`, `unknown` and `obfuscation` settings parallel spoken language. A directional `mutual <signed language> <difficulty>` link is between **signed** languages. Remove it with `removemutual <signed language>`.

| After `signedlanguage set` | How to choose |
| --- | --- |
| `profile add <name> <body prototype> <shape> <minimum> <preferred>` | Add an alternative way to produce the language. A humanoid profile might require one functional hand and prefer two. Select the actual body prototype and part shape from your game. |
| `profile requirement <profile> <shape> <minimum> <preferred>` | Add a requirement to that profile or update its counts. Requirements within a profile apply together. Alternative profiles let other bodies articulate the same language. `profile remove <name>` removes a profile. |
| `variety add <name> <description> <suffix> <vague suffix> <difficulty>` | Add a regional variety. Quote each multiword field. Its recognition difficulty and two suffixes let a viewer distinguish familiar signs from unfamiliar regional usage. `variety remove <name>` removes the variety. |

Below the minimum functional count, that profile cannot be used. Meeting the minimum but missing preferred parts makes expression harder. A character who has no usable profile can still watch others sign. For a non-humanoid community, build another profile using the functional shapes its signers actually use. [Body prototypes](Body_Prototype_Builder_Guide.md)

Gaining the linked skill teaches the language. Simple and variable NPC templates can supply it the same way. Regional familiarity is separate. `signedlanguage grantvariety <character> "<language>" "<variety>"` grants familiarity, and grants the parent language if necessary. `revokevariety` removes the named variety. Players use `signedlanguages` to see what they know and `signlanguage "<language>" ["<variety>"]` to choose. [Building NPCs](NPC_Builder_Guide.md)

## Scripts and literacy

A **script** defines a writing system. A **writing** is one inscription or passage, in a particular language and script, on something a character can read. A shipping company can issue the same notice in two languages using the same alphabet. A clerk can know the alphabet and understand only one version.

1. **General literacy.** The world can make all characters literate with `CharactersLiterateByDefault`. Otherwise possession of the trait designated by `LiteracySkillId` supplies literacy. Decide whether this is universal, a product of schooling, or a skill learned during play.
2. **Knowledge of the script.** Create a knowledge and link it to the script. Receiving the knowledge at character creation or during play grants its linked scripts. Distinct scripts can intentionally share one schooling grant.
3. **Language and comprehension.** For an ordinary physical writing, a reader needs literacy, the script and a successful writing comprehension check. A known related language can provide a more difficult reading route. The author’s proficiency, the handwriting style and the reader’s skills can affect the result. An administrator’s ability to read everything is a poor guide to a passenger’s experience.
4. **Ability to write here.** The writer selects a known script and language, holds a usable implement and chooses a reachable writable surface with room for the entry. A book needs an accessible page. Changing the spoken language does not change the writing language.

Use `show scripts` to find installed scripts. `script edit new "<name>" "<knowledge>"` creates one. Use `script edit <name>`, `script show <name>` and `script close` to maintain it.

| After `script set` | What changes |
| --- | --- |
| `name <name>`, `knowledge <knowledge>` | Set the script’s identity and the knowledge that grants it. Link the knowledge before awarding it to characters. |
| `known <description>`, `unknown <description>` | Describe the recognised script and what an untrained observer can see, for example “the Quayside Hand” and “columns of narrow, hooked strokes”. |
| `language <language>` | Toggle a language the script was designed to represent. This supplies conventional pairings. A literate writer can choose another known language with the script. |
| `length <percentage>`, `ink <percentage>` | Length sets how much space a passage occupies. Both settings start at 100%. The ink setting is stored and displayed, but ordinary handwriting currently does not use this separate multiplier to charge the implement. |

Build the linked knowledge with `knowledge edit new "<name>"`. `desc` and `ldesc` explain what its holder has learned. `type` and `subtype` organise it. Use `knowledge show <name>` to inspect acquisition before changing anything. `knowledge set learnable LearnableFromTeacher` toggles teaching, while `sessions`, `learndifficulty`, `teachdifficulty` and the Boolean character `learnprog` govern lessons. In play the teacher can use `teach <student> "<knowledge>"`. Prefix the name inside the quotes with `*` to disambiguate it from a skill.

For an elective, enable `LearnableAtChargen`, set the `chargenprog` for the appropriate applicant and current trait, and give the knowledge picker enough picks. For a guaranteed education, use the picker's `free` prog returning a collection of knowledges. Staff can award a particular live character `giveknowledge <character> "<knowledge>"`. Open the general literacy and language paths separately.

## Worked example: the bilingual ferry port

At Reedbank Landing, **River Tongue** is the upriver community’s language and **Harbour Speech** is the language of the estuary. Pilots serve passengers in both. Among the crew and passengers are users of **Quay Sign**. The shipping office teaches **Quayside Hand** for timetables and cargo manifests. Use your existing Reedbank and Estuary cultures if you built the [playable-peoples example](Character_Creation_Builder_Workflows.md#build-reedbank-and-estuary). The port also works with two cultures of your own.

### Give the two shores their voices

First use `show languages`, `signedlanguage list` and `skill list`. Reuse the existing language records and their linked skills where present. If starting fresh, create three ordinary skills using `skill edit new "River Tongue"`, `skill edit new "Harbour Speech"` and `skill edit new "Quay Sign"`. For each, set the cap, improver, chargen and teaching settings appropriate to your game, or clone an established ordinary language skill and review the copy. The example uses identical names for the language and its skill. The first argument to language creation names the language and the second selects its skill. If the spoken languages already exist, start with the mutual-intelligibility settings below.

```text
language edit new "River Tongue" "River Tongue"
language set unknown a language with long vowels and softly tapped consonants
language set obfuscation 20%
language close
language edit new "Harbour Speech" "Harbour Speech"
language set unknown a language with short vowels and sharply stressed endings
language set obfuscation 20%
language close
```

In this setting the two languages share some vocabulary. Harbour speakers can often follow upriver directions. The reverse is harder. Configure both directions explicitly. For unrelated languages, omit these two links and let bilingual crew interpret.

```text
language edit "Harbour Speech"
language set mutual "River Tongue" Normal
language edit "River Tongue"
language set mutual "Harbour Speech" Hard
language close
```

Give each shore a native sound. Then add how an upriver native sounds when learning Harbour Speech. Keep the generic Fallback for visitors from other places.

```text
accent edit new Reedbank "River Tongue"
accent set role Native
accent set group Upriver
accent set description Reedbank speakers hold the vowel before the last consonant.
accent set suffix with the long vowels of Reedbank
accent set vague with drawn-out final vowels
accent edit new Estuary "Harbour Speech"
accent set role Native
accent set group Estuary
accent set description Estuary speakers lift the pitch on the last syllable of a call.
accent set suffix with an estuary lilt
accent set vague with a rising cadence
accent edit new "Upriver Harbour" "Harbour Speech"
accent set role Foreign
accent set associated "River Tongue"
accent set group Harbour Learners
accent set description Upriver speakers carry the rhythm of River Tongue into Harbour Speech.
accent set suffix with an upriver cadence
accent set vague with measured, lengthened vowels
accent set difficulty Normal
accent close
culture edit Reedbank
culture set native "River Tongue"
culture close
culture edit Estuary
culture set native "Harbour Speech"
culture close
```

The culture settings establish identity when that language is known. On the skills screen, grant River Tongue to Reedbank applicants and Harbour Speech to Estuary applicants. Give pilots and clerks both. A pilot who selects `speak "River Tongue"` can call `say The east landing is closed. Take the steps by the crane.` An upriver passenger can follow in their own language. A passenger who knows only Harbour Speech might catch some of the call. A bilingual pilot can repeat it in Harbour Speech rather than relying on that chance.

### Give the deck crew Quay Sign

Use `bodyproto list` and `show bodypartshapes` to find the crew’s body prototype and hand shape. In this community one usable hand suffices, while two make expression easier. The North Quay variety has local signs for the piers. Both the deck crew and passengers from that neighbourhood may know it.

```text
signedlanguage edit new "Quay Sign" "Quay Sign"
signedlanguage set unknown an unfamiliar signed language
signedlanguage set profile add "Quayside crew" <body-prototype-id> <hand-shape-id> 1 2
signedlanguage set variety add "North Quay" "The usage of the northern ferry piers" "in the North Quay variety" "in an unfamiliar local variety" Normal
signedlanguage show "Quay Sign"
signedlanguage close
```

Give the linked skill to the roles or individuals who learned Quay Sign. For an established North Quay deckhand called Mara, staff can use `signedlanguage grantvariety Mara "Quay Sign" "North Quay"`. As Mara, use:

```text
signlanguage "Quay Sign" "North Quay"
sign (points to the crane) The east landing is closed.
signto Anja Take the steps on your left.
```

Anja can understand the instructions if she sees Mara and knows enough Quay Sign. A crew member with neither hand functioning can watch the same exchange but cannot answer using this profile. Passengers who turn toward a different speaker still need to be able to see Mara. For a distant pier in another room, give people a local signaller or a posted notice.

### Teach the shipping office its script

Create the schooling knowledge before the script and teach it after both exist. The two languages are conventional uses for the same script. On a new record, check the teaching flag with `knowledge show` and toggle it on if needed.

```text
knowledge edit new "Quayside Lettering"
knowledge set desc Read and form the hooked letters used in the ferry ledgers.
knowledge set ldesc You can distinguish cargo marks, pier names and dates written in Quayside Hand.
knowledge set type Education
knowledge set subtype Writing
knowledge set sessions 3
knowledge set learndifficulty Normal
knowledge set teachdifficulty Normal
knowledge show "Quayside Lettering"
knowledge close
script edit new "Quayside Hand" "Quayside Lettering"
script set known the Quayside Hand
script set unknown a script of narrow, hooked strokes
script set language "River Tongue"
script set language "Harbour Speech"
script set length 100%
script show "Quayside Hand"
script close
```

If the knowledge is closed to lessons, use `knowledge edit "Quayside Lettering"`, then `knowledge set learnable LearnableFromTeacher`. Set a suitable `learnprog` if only certain pupils are eligible. The senior clerk offers a lesson with `teach apprentice "Quayside Lettering"`. The apprentice types `accept` to take it. Allow time between lessons for learning fatigue to subside. The sessions setting controls the learning required, so a single offer need not confer the knowledge. For an established clerk, a staff award with `giveknowledge <clerk> "Quayside Lettering"` models completed schooling.

### Connect the port to character creation

Give home languages through upbringing and workplace training through the profession. These sample policies assume culture and role selection precede the skill screen, and skills precede knowledges. Create the **Ferry Clerk** profession if you want it selectable. Use `role new Profession Ferry Clerk`, write its `role set blurb` and use `role set prog clear` for an open profession. The earlier guide supplies Ferry Pilot and Senior Ferry Pilot.

**FerryLanguageSkills** returns a trait collection and takes one chargen named `app`. Here both pilot grades learn both spoken languages and the crew’s signed language. Clerks learn both spoken languages and the world’s literacy skill. Ordinary residents learn their home language. Replace `Literacy` with the actual trait designated by your world if literacy is skill-based. On a universally literate game, omit that grant.

```futureprog
var skills as trait collection
var pilot = @app.roles.any(r, @r.name == "Ferry Pilot" or @r.name == "Senior Ferry Pilot")
var clerk = @app.roles.any(r, @r.name == "Ferry Clerk")
if (@app.culture.name == "Reedbank" or @pilot or @clerk)
  additem skills totrait("River Tongue")
end if
if (@app.culture.name == "Estuary" or @pilot or @clerk)
  additem skills totrait("Harbour Speech")
end if
if (@pilot)
  additem skills totrait("Quay Sign")
end if
if (@clerk)
  additem skills totrait("Literacy")
end if
return @skills
```

**FerrySchooling** returns a knowledge collection and takes one chargen named `app`. A clerk leaves school able to use the office script. Other roles can still learn it from the office later.

```futureprog
var schooling as knowledge collection
if (@app.roles.any(r, @r.name == "Ferry Clerk"))
  additem schooling toknowledge("Quayside Lettering")
end if
return @schooling
```

Compile the progs after verifying the names of the content they reference. On a world with existing free-grant progs, merge these rules into their returned collections, preserving grants such as Boat Handling. Each setting holds a single prog. For a fresh setup using the two pickers, bind them as follows:

```text
chargen edit SkillPicker
chargen set freeskills FerryLanguageSkills
chargen close
chargen edit KnowledgePickerBySkill
chargen set free FerrySchooling
chargen close
```

A pilot can select Quay Sign without a regional variety. If the named deck crew are NPCs, give their templates the Quay Sign skill. Grant a variety to a specific character when you want that extra familiarity. The manual accent picker can offer several accents. For an upbringing-specific origin story, a Boolean prog taking one chargen and returning `@app.culture.name == "Reedbank"` can control which applicants may pick the Reedbank accent. Attach it with `accent set chargen <prog>` on that accent. The Foreign role and source association still describe how a character can acquire Upriver Harbour during play.

### Write a bilingual manifest

Supply a writable sheet or an open ledger with space on the current page, and a writing implement the clerk can hold and use. Give the items suitable keywords such as `ledger` and `pen`. Use a **Book** component for a reusable ledger or **PaperSheet** for a loose manifest. A serviceable **Pencil** can stand in for the clerk’s pen. Inspect its point and keep a sharpener at the desk. A sheet filled by the upriver entry has no room for its harbour counterpart. [Item authoring](Item_Authoring_Guidelines.md)

As the literate **player clerk**, use `scripts` to see the script and `languages` to see the two languages. Hold the pen. Use the following commands, finishing the text editor after each `write`:

| Command | What the clerk does |
| --- | --- |
| `script "Quayside Hand" "River Tongue"` | Select the script and writing language. The clerk can continue speaking Harbour Speech to the purser. |
| `write ledger` | Enter “Four casks of cider for Reedbank. Unload at the crane on the north quay.” and save. |
| `script "Quayside Hand" "Harbour Speech"` | Keep the script and switch the language of the next entry. |
| `write ledger` | Enter the paired wording, “North Quay crane, Reedbank. Cargo due ashore: four cider casks.” and save. |
| `read ledger`, `read ledger 1`, `read ledger 2` | Inspect the entries on the accessible page or select a particular passage. These numbers assume a fresh page. |

Each entry has its own language, script and author. The clerk writes the second wording by hand. Selecting Harbour Speech records how the new passage should be read. If the clerk wants a particular hand, `show writingstyles` lists choices and `set writing <style>` selects one their handwriting skill permits.

A fluent, unschooled pilot can explain the cargo to the porter after the clerk reads it aloud. A literate River Tongue reader with Quayside Lettering can try the first passage directly. The second calls on Harbour Speech or a related language at the configured difficulty. A bilingual reader with a different script still sees unfamiliar marks. These differences give the office a reason to employ an interpreter and the crew a reason to seek lessons.

**For a port timetable distributed in identical copies**, build a separate Book component for the timetable and set its paper prototype and page count. While editing that component, use `comp set content add 1 "River Tongue" "Quayside Hand" Reedbank Ferry Office` and enter the upriver schedule. Use `comp set content add 2 "Harbour Speech" "Quayside Hand" Estuary Ferry Office` for the other page. The last argument records the source of the printed text. Check `comp set content list`, approve the revision and attach it to the timetable item. Fresh copies contain those printed passages. The cargo ledger stays available for the clerk’s own entries.

### Put a notice beside the gangway

Use description markup for permanent text or a painted depiction. Use a writable item for a notice players can compose, carry or amend. Here are fragments for the landing’s room description:

```text
An enamel plate reads writing{Harbour Speech,Quayside Hand}{Last ferry: use the north steps.}{a line of hooked blue letters}.
Below it a painted deckhand makes sign{Quay Sign}{the sign for the north steps}{a deliberate movement toward the north steps}.
```

Written markup looks for the specified known language and script and can take `minskill=30` for a more demanding inscription. It does not roll the physical document’s comprehension check. Signed markup can specify a variety and a minimum skill. The variety changes the recognition hint. A viewer may understand the depiction without recognising its regional variety. It describes a pictured sign without a live signer. Write the fallback as something a person can actually perceive. Everyone at these steps deserves a visible arrow even when the lettering is unfamiliar. [Room descriptions and building](Room_Building_Builder_Guide.md)

## When the port behaves unexpectedly

| What you see | What to inspect |
| --- | --- |
| A Reedbank applicant has no River Tongue | Check the linked skill grant and the order of culture and skill selection. A native default can select only a language the character knows. |
| All visitors acquire the same “foreign” accent | Inspect the visitor’s native language and the Harbour Speech accent’s Foreign role and source association. The new language already has a generic Fallback. |
| One shore understands the other more often | Inspect the link on **each listener’s** language, both configured difficulties, the accent and audibility. One-way links are legitimate. |
| A deckhand knows Quay Sign but cannot answer | Inspect the active body, compatible profile, part shapes and functional counts. If the observer misses the answer instead, check line of sight. |
| A clerk can speak both languages but cannot select the script | Check general literacy and the linked knowledge. `script` is the builder editor on an administrator character. Use the ordinary clerk for the player workflow. |
| Someone can read a notice in the room description but struggles with a handwritten copy | The description uses explicit markup conditions. The physical copy also has a writer, style, writing quality and a reader comprehension check. |
