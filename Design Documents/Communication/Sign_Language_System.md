# Signed Language Communication System

## Scope

Signed languages are first-class communication languages. They are deliberately separate from spoken `Language` records, writing scripts, and accents: a character can know any combination independently, and can understand a signed language their current anatomy cannot produce.

The runtime is implemented by `ISignedLanguage`, `SignedLanguage`, `SignedLanguageInfo`, `SignedCommunicationService`, the character language state, and the body communication strategies. Persistence owns signed languages, varieties, directional mutual intelligibility, articulation profiles and character familiarity.

## Player Workflow

- `signedlanguages` lists understood signed languages and known varieties; `signedlanguages list [<name filter>]` browses the complete game catalogue.
- `signlanguage [<language> [<variety>]]` views or selects the current signed language.
- `sign [(<emote>)] <message>` signs to the current location.
- `signto <target> [(<emote>)] <message>` directs signing toward a visible target without making it private.
- In a player emote, matching backticks delimit signed input: ``emote @ smiles and signs, `Welcome home.` ``. Backticks are an input convention chosen for ordinary keyboards; rendered signed content uses guillemets, for example `«Welcome home.»`, so it remains visually distinct from speech.

Signing is purely visual. A recipient must be able to see the signer; hearing and vocal anatomy are irrelevant. The normal comprehend-language effect is considered only after that visual gate.

## Comprehension

Expression and understanding use dedicated `SignedLanguageExpressCheck` and `SignedLanguageUnderstandCheck` checks. Exact knowledge uses the signed language's linked trait and difficulty model. Directional mutual-intelligibility records allow a language known by the observer to understand another signed language at an additional configured difficulty. No relationship is inferred merely because two signed languages are used in the same country or share a spoken-language name.

Regional varieties are subordinate to a signed language. Variety recognition can add difficulty and controls whether the precise or vague suffix is shown.

## Anatomy

Each language has one or more alternative articulation profiles tied to a body prototype. A profile contains bodypart-shape requirements with minimum and preferred functional counts. Falling below a minimum blocks production; missing preferred parts stages the expression check upward. `CanUseBodypart` is checked at the moment of signing, so wounds, severing, restraints, or other loss of function are respected.

The stock humanoid profiles require at least one functional hand and prefer two. This models practical one-handed adaptation without claiming that an entire natural signed language is intrinsically one- or two-handed. Builders can require two hands, tentacles, facial structures, or other setting-specific anatomy when appropriate.

## Builder Workflow

Administrators use `signedlanguage list`, `edit`, `show`, `set`, and `close`. `show signedlanguages [<name filter>]` provides the same full catalogue through the general builder browser. Settings cover the linked trait and difficulty model, unknown-language text, obfuscation, directional mutual intelligibility, regional varieties, and articulation profiles. Profiles are alternatives. `profile add` creates one with its first bodypart-shape requirement, and `profile requirement` adds or updates further minimum and preferred functional-part requirements.

Regional familiarity is deliberately separate from language knowledge. Staff use `signedlanguage grantvariety <who> <language> <variety>` and `signedlanguage revokevariety <who> <language> <variety>` to assign it. Granting a variety also grants the parent signed language for comprehension when necessary; it does not bypass that language's anatomy requirements for production.

NPC templates grant signed-language knowledge from the same linked skill relationship used at runtime: when a generated NPC receives a skill linked to a signed language, that language is added automatically and becomes its current signed language if it has no earlier selection. Simple and variable NPC-template displays show these inferred signed languages so builders can verify the result before spawning.

## FutureProg and Events

`SignedLanguage` and `SignedVariety` are first-class FutureProg types. `SignedLanguageVariety` remains accepted as a compatibility alias for existing progs. Signed languages expose identity, trait, unknown-description and variety properties; signed varieties expose identity, parent language, description, presentation suffixes and recognition difficulty.

The five signing events pass the signed-language and optional signed-variety objects directly rather than their names. Their metadata uses the same first-class types, so event progs can inspect properties such as `language.name`, `language.trait`, `variety.language`, and `variety.difficulty`. The variety argument is null when no variety is selected.

## Description Markup

Descriptions that already process written-language markup also process signed depictions:

`sign{American Sign Language,London,minskill=30}{a pictured open hand sweeping forward}{an unfamiliar hand sign}`

The language is required. A variety and `skill`/`minskill` threshold are optional. The final brace group is optional fallback text. This markup represents a visual depiction or diagram of a sign, not a live act of communication.

## Seeder Content

The Earth-Modern culture pack asks independently whether to seed signed languages. It installs 24 signed languages, eight regional BSL varieties, humanoid articulation profiles, and evidence-based BANZSL links between BSL, Auslan, and New Zealand Sign Language. The links are directional records even when stock content seeds both directions.

## Extension Boundary

Version one is visual signing. Tactile signing is intentionally left as an extension seam because it needs contact, consent, targeting, and touch-perception rules rather than bypassing the visual check on ordinary `sign` output.

## Research Basis and Corrections

- The World Federation of the Deaf describes signed languages as full natural languages structurally independent from co-existing spoken languages. It also stresses that regional and social variation is normal; this supports separate language records and first-class varieties.
- Sign language is not universal. Shared national spoken languages do not imply shared signed languages, so the seeder does not infer links from country or spoken language.
- BSL uses a two-handed fingerspelling alphabet and ASL uses a one-handed alphabet, but that fact is about fingerspelling rather than every sign in either language. The engine therefore does not globally hard-block BSL with one hand; stock profiles require one functional hand and prefer two, while builders can model stricter setting-specific systems.
- The BSL Corpus sampled Belfast, Birmingham, Bristol, Cardiff, Glasgow, London, Manchester and Newcastle and documents substantial regional lexical variation, which is the basis for the eight stock varieties.
- Tactile signing is a real reception mode used by deafblind signers, but it is not equivalent to ordinary distant visual signing and therefore remains a separate extension.

Starting references:

- [World Federation of the Deaf statement on signed-language independence and variation](https://wfdeaf.org/wfd-statement-on-standardized-sign-language/)
- [World Federation of the Deaf FAQ on non-universality](https://wfdeaf.org/contact/faqs/)
- [BSL Corpus Project regional coverage](https://bslcorpusproject.org/project-information/)
- [British Sign: BSL and ASL fingerspelling alphabets](https://www.british-sign.co.uk/pages/)
- [Deafblind Australia: visual-frame, tracking, and tactile reception](https://www.deafblind.org.au/deafblind-communication/)
- [NZSL reference discussing BSL, Auslan, and NZSL similarity and intelligibility](https://hmk.am/wp-content/uploads/2022/12/453832.pdf)
