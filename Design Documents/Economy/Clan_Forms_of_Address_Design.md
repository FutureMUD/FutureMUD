# Clan Forms of Address — Draft Design

Status: implemented contract, 11 November 2026. This document records the accepted design and its engine, persistence, FutureProg and seeder integration.

## Purpose and boundaries

Add optional, character-relative forms of address to clan ranks and appointments. A rank name describes standing, a title/abbreviation describes the holder, and the new value supplies direct address: for example, rank `Duke`, abbreviation `Dk.`, form of address `Your Grace`.

The feature is generic. It grants no privileges, changes no rank, creates no appointment and enforces no speech etiquette. Catholic, military, court, commercial and other content supplies its own text and progs. Existing title and abbreviation records retain their meaning and behavior. Personal-name `NameUsage.Title` remains independently declared name data; it does not establish entitlement to a clan address.

The common case must require one record with no prog: `Your Grace` for everyone allowed to know that address. Relative-standing exceptions and holder-dependent styles are optional additions.

## Current implementation findings

- `IRank` and `IAppointment` already expose ordered title/abbreviation rows with optional single-character progs. Their lookup methods are not a suitable new address API: a form of address needs an explicit speaker and addressee.
- Current `Outranks`/`OutranksOrPeer` compare clan-local `RankNumber`. Rank paths restrict promotion paths; they are not independent comparison domains. Those helpers do not currently filter archived memberships, so the new resolver must explicitly select active membership rather than copy that omission.
- Appointment parentage describes authority/remit. Appointments can be stackable, honorary, independent or held by people with different career ranks. It does not supply a universal social hierarchy.
- Shared-clan full-look output is in `BodyPerception.LookText`, under `ClanSimilaritiesShowInLook`. It uses the viewer's `CanViewMembers` flag, but lacks explicit archived-membership and obscured-identity guards. Adding an address hint must not compound that disclosure path.
- Template profiles currently store scalar titles/abbreviations. Template cloning copies their prog references without general rebinding. New address records must have their own complete copy/profile/rebinding contract.

## Address records

Each rank and appointment owns an ordered list of address rules. Initially that list is empty. A rule contains:

| Field | Meaning |
| --- | --- |
| Stable row ID | A persistent GUID; commands also accept the current displayed row number. Address text is not a key. |
| Owner | Exactly one rank or appointment, enforced structurally by its owning child table. |
| Order | Explicit condition evaluation order within that owner. |
| Address text | Literal direct-address text; null represents an explicit `none` result. Whitespace-only text is rejected. |
| Relations | Any, or a mask of lower, peer, higher and outsider speakers. |
| Optional boolean prog | Either `(subject)` or `(speaker, subject)`; a one-character prog always receives the subject. |
| Audience | Members by default; explicit public permission permits outsider resolution. |

An unconditional Any rule is the owner's default and is kept last. At most one such default exists. The short builder form creates or updates that one row; adding an exception inserts it before the default. Non-default rows can be reordered explicitly. There is no implicit specificity scoring between a relation filter and a custom prog.

This avoids three compulsory inferior/peer/superior columns and repeated copies of the same address. One Any row suffices for the simple case; one exception plus one default suffices for most variations.

Program errors or a missing configured prog fail closed: return no address and retain diagnostic context. An absent prog means unconditional; failure to resolve a configured prog must never be treated as unconditional. Predicates are expected to be observational; this feature does not claim to introduce a general side-effect-free FutureProg execution environment.

## Relative standing

Recommended initial rule: compare the speaker's and subject's **active ranks in the selected clan**, using current runtime rank ordinals.

| Selector | Speaker's standing relative to the subject |
| --- | --- |
| `lower` | Speaker has a lower rank number. |
| `peer` | Speaker has the same rank number. |
| `higher` | Speaker has a higher rank number. |
| `outsider` | Speaker has no active membership in that clan. |
| `any` | Any of the above, subject to audience/visibility restrictions. |

Ranks in different clans are never compared. Rank paths do not reset comparison or create artificial peer groups. Self-address, including two bodies of one durable identity, has no automatic result. A null speaker is accepted: try permitted public outsider rules first, then public lower-rank rules only if the entire outsider lookup has no match. Explicit none, ambiguity and predicate errors stop that fallback. A real outsider is never automatically treated as inferior. Two-character predicates receive null as their first argument in this case and should handle it. Missing subject membership is not an outsider case: the subject must actively hold the queried rank or appointment.

Appointment holdings, administrative permission and pay do not automatically alter the relation. For example, an abbot and a professed monk can be rank peers. The abbot's office may still supply `Father Abbot` to everyone through one Any record. More specific office protocol can be expressed by two-character rule progs.

Office authority does not redefine lower/peer/higher automatically; that would need an explicit additional comparison policy. Inferring social precedence from parent links or permission masks would give surprising results for honorary offices, acting commanders and people holding multiple offices. It is not included silently in the recommended initial behavior.

## Resolution and fallback

Within an owner, evaluate rules in order and use the first permitted matching rule. Keep these outcomes distinct internally:

- No match: this owner has supplied no address for this context.
- Text: use the selected literal address.
- Explicit none: intentionally suppress an automatic address.
- Hidden, error or ambiguous: no player-facing address; preserve an explanation for administrator diagnostics.

No rows and explicit none have different purposes. An appointment with no matching rule allows fallback to another source, including the rank. A selected appointment rule containing `none` stops that fallback. A rank with no result produces no address, not its title or abbreviation.

For the general clan lookup, consider the subject's held appointments before their rank. Add an integer `AddressPriority` to appointments, default zero, used solely for presentation. Higher-priority matching sources take precedence. The rank is the final fallback and requires no priority field.

Recommended tie policy: identical text results at the highest priority are deduplicated. Different text, or text versus explicit none, is ambiguous. Return no automatic singular choice and explain the conflict in builder diagnostics; do not choose by database ID, alphabetical name or privilege mask. An explicit appointment lookup remains available to NPC progs that know the conversational context.

Exclude sources the speaker is not permitted to know before evaluating their predicates. Evaluate the remaining appointments in descending priority groups, stopping at the first group with a matching result. An error in a reached group aborts the lookup without falling back; lower-priority groups need not execute. If every permitted appointment produces no match, try the rank. Ambiguity and explicit none also stop fallback. A hidden source does not suppress a permitted source or execute its predicates.

Alternative designs would display all applicable forms, or require one explicitly designated primary address appointment. Priorities are more flexible, but must remain visibly separate from rank and authority.

## FutureProg interface

Text-returning functions:

```text
ClanFormOfAddress(speaker, subject, clan)
RankFormOfAddress(speaker, subject, rank)
AppointmentFormOfAddress(speaker, subject, appointment)
```

All three require an explicit subject and accept a nullable speaker character. The source-specific functions verify that the subject actively holds the supplied rank/appointment. They do not manufacture a style for an unearned office. Missing, hidden, suppressed, ambiguous or invalid results return empty text so scripts can choose a greeting or ordinary name instead.

The internal shared resolver returns structured provenance and status for diagnostics; look output and FutureProg functions use this same resolver rather than implementing competing rules. This also prevents one path reversing the speaker/subject arguments.

A dot property such as `rank.formofaddress` cannot provide the contextual answer because it lacks speaker and subject. A metadata listing would be a separate administrative capability.

## Visibility and look output

Recommended default: member-facing address rules use the same permitted shared-clan recognition context as the look disclosure, strengthened with active-membership and identity checks. No hidden membership or office should become discoverable merely because an NPC calls the new function.

This conservative default also means a fellow member without the existing member-view permission receives no automatic result from a members-only rule. If ordinary members should know courtesy styles without roster access, that needs a distinct address-disclosure policy rather than silently expanding the existing permission. Public rules are one explicit option. Identity checks use existing obscured-identity behavior; this proposal does not introduce a new persistent name-recognition system.

Outsiders are a distinct relation, never assumed to be peers. A builder can explicitly mark a rule public, permitting that address to be resolved for an outsider who can identify the subject. Public permission discloses the address itself; it does not reveal every rank, appointment or membership detail. The ordinary functions do not have an undocumented omniscient mode.

For initial automatic look integration, recommend shared-clan hints only. Respect `ClanSimilaritiesShowInLook`, require normal visibility and identity recognition, exclude archived memberships on both sides, and add a per-clan `ShowFormsOfAddressInLook` switch. Default it on: zero address records still produce no output, so existing worlds remain unchanged. A clan can disable the hint while continuing to use addresses in NPC progs.

Example addition beside an already-permitted shared-clan line:

```text
She is the Prioress of Saint Mary's Priory.
Within that community, you would normally address her as "Mother Prioress".
```

Resolve from the actual viewer to the actual subject. Do not expose the subject's true role or address through an obscured identity or disguise. Do not inject hints into room listings, sdescs, names or emotes. Do not imply that the player is compelled to use the suggested form. Multiple clans can each have a separate result; do not impose one global ranking of organisations.

Public outsider look hints are not enabled; they would be a broader disclosure choice than extending the existing shared-clan line. Recommended initial behavior permits public address use in progs without automatically advertising every public affiliation on look.

## Address text versus complete names

Recommended initial value: a standalone literal vocative, such as `Sir`, `Father`, `Your Grace` or `Mother Prioress`. No implicit prefix/suffix placement, true-name substitution, name-culture conversion, emote tokens or third-person conversion.

NPC progs choose how to use the returned text. `Your Grace` is distinct from a reference style such as `His Grace`, and neither should replace a rank title automatically. Complete address templates such as `Father {given-name}` are possible future work, but introduce name ordering, recognition/disguise and localisation decisions beyond this feature.

For the monastic example, a Professed rank could have an earlier subject-only predicate returning `Father` for a priest and a final default returning `Brother`. Ordination can be checked through whatever generic membership or character data the setting uses. Becoming ordained changes the matching address without changing the person's monastic seniority. An abbot appointment can then override that rank result with `Father Abbot`. These are content rules, not engine knowledge of ordination.

## Builder surface

These commands edit the optional address rules. They operate on the currently edited clan and follow the existing rank/appointment editing permissions.

```text
clan set rank "Captain" address "Captain"
clan set rank "Captain" address add "Sir" from lower
clan set rank "Captain" address add none from higher
clan set rank "Captain" address list
clan set rank "Captain" address rule <id> prog <prog>|none
clan set rank "Captain" address rule <id> from lower|peer|higher|outsider|any
clan set rank "Captain" address rule <id> audience members|public
clan set rank "Captain" address move <id> before <other-id>
clan set rank "Captain" address remove <id>
clan set rank "Captain" address clear

clan set appointment "High Priest" address "Your Reverence"
clan set appointment "High Priest" address priority 50
```

Appointment rules expose the same operations as rank rules. `address none` creates an explicit suppressing default; `address clear` removes all rules and restores fallback. Removing a single rule does not clear unrelated configuration. The builder listing shows order, text/none, relation, audience and prog. The admin command `clan address <clan> <speaker|none> <subject> [hidden]` previews visible local characters and reports the chosen source, rule, relation and status. `hidden` explicitly bypasses address-disclosure checks for this diagnostic only. Use `clan set addresslook` to toggle the current clan's hint.

## Persistence, copying and seeding

Use two new child tables, one for rank address rules and one for appointment address rules, with surrogate row IDs. Do not copy the existing `(text, owner)` title keys: the same text can legitimately occur in more than one conditional rule, and explicit none has no text key. Store owner FK, order, text, relation flags, audience and optional prog FK. Owner deletion cascades; deleting a referenced condition prog must be restricted or deliberately disable the rule, never turn it into an unconditional address.

Persist appointment address priority and the clan look-hint switch. The `ClanFormsOfAddress` EF migration, designer/model snapshot, migration projections and blank database snapshot carry this schema. Existing records start with empty address collections, unchanged titles and unchanged powers. Do not infer addresses for existing worlds from rank names.

Extend generic template/profile definitions with ordered address-rule data. Copy every rule, relation/audience setting and explicit-none value. Template-local or bound-clan predicate progs require explicit generic rebinding metadata and regenerated live references; arbitrary source text is not rewritten. Global utility predicates can remain shared. Variant application must preserve unrelated address rules and apply deliberate rule overrides without flattening them into one unconditional title.

Catholic data can then supply Father/Brother/Sister and house-specific customs using these generic facilities. There will be no Catholic-specific engine command, builtin or comparison branch.

## Implementation and acceptance scope

The implementation includes interfaces/value objects, shared resolution, persistence, generic building commands/help, FutureProg functions, template/profile copying and full-look integration. Broader changes to existing title semantics, personal-name rendering, cross-clan authority or automatic NPC speech are not implied.

Focused acceptance cases must cover:

- Empty configuration, one default rule, all four relations, and default-plus-exception authoring.
- Speaker/subject orientation, two bodies of one identity, active versus archived membership, and current rank ordering after rank edits.
- One-character subject predicates, two-character predicates, errors and missing configured progs.
- Explicit none versus absence, appointment-to-rank fallback, multiple offices, matching ties, conflicting ties and priority changes.
- Source-specific queries rejecting ranks/offices the subject does not hold; outsider/public behavior and visibility parity between look and progs.
- Disguises/obscured identity, hidden memberships, no self-hint, and no changes to room/sdesc/name output.
- Database round-trip, owner deletion, prog-reference protection, template cloning with independent live prog bindings, and preservation through variants/reruns.
- Military rank examples, a shared-rank monastic community, a multi-office character and a nonreligious public office, demonstrating generic behavior.

Verification covers focused resolver, builder, FutureProg, persistence, migration and installer/clone tests. Consult the task result for checks actually executed and live-environment limits.

## MUD-wide look format

The static string `ClanFormOfAddressLookFormat` controls the added line throughout the MUD. Its default is:

```text
Within {clan}, you would normally address {objective} as "{address}".
```

Supported literal tokens are `{address}`, `{clan}`, `{subjective}`, `{objective}` and `{possessive}`. They are substituted once; replacement text is not interpreted as another template. Unknown tokens remain literal. No true name or emote expansion is performed. Empty formatting text suppresses the additional wording. Use the existing administrator command `editstaticstring ClanFormOfAddressLookFormat` to edit the string. Existing worlds obtain missing default strings through the normal runtime static-string initialization.

## Catholic content and upgrades

`CatholicAddressContent` authors editable English defaults in generic profile data: clergy, papal/cardinal/episcopal offices, Brother/Father and Sister, and selected abbatial/prioral offices. Ordination-dependent rules generate subject-only progs against the clone's explicit Church binding. These are gameplay conventions rather than a claim that one English protocol covers every region or historical period. They do not alter existing titles or abbreviations.

Fresh stock installation includes the new rules. A one-time address-content version enriches older, explicitly stock-owned Catholic template profiles and empty address collections, preserving existing custom rows, titles, names, governance and profile edits. Subsequent reruns do not restore cleared or edited rules. Live copies are not silently upgraded. A fresh clone regenerates bound predicates with independent rule IDs and concrete clone references; arbitrary shared predicates are copied by explicit global prog name. Female-house variants patch address recipes while retaining unrelated builder rules.

Shared address predicate dependencies of effective keyed rows are validated by name and Boolean/character signature before native clone creation or variant field/profile writes. Deliberately cleared or replaced rows do not require a retired predicate referenced only by an unused recipe. Missing, renamed, wrong-signature and accepts-any progs reject the operation without creating a partial clone or consuming the variant selection. Correcting the dependency permits retry.
