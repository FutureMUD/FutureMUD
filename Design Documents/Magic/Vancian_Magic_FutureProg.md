# Vancian FutureProg contracts

All functions register in the **Vancian Magic** category through `VancianFunction.Contracts`. Arguments below are positional and strongly typed. Repertoire and allowance text accepts a builder alias or its stable GUID. Capability arguments are actual `magiccapability` values, not school names. Spells are `spell` values. A collection dictionary groups each rule alias/key with its proposed spell collection; mutation validates the whole capability before committing any selections.

Character queries and mutations use the canonical identity for persisted choices, ledger and progression. Physical access, acting-body requirements, costs and scroll potency use the actual actor where specified. Queries do not initialise capacity, refresh, roll casting/control checks or invoke notification callbacks. On first state or item access, lazy recovery may durably cancel abandoned precommit writing; it never refunds committed expenditure. Availability means a currently usable route; it does not resolve a target or promise a target-specific material/resource preflight will succeed. Game-authored candidate, eligibility and progression progs must be pure. Recursive policy evaluation fails closed.

`spellbookspells` and scroll inspection functions are trusted item queries and do not themselves grant a player access route. A game exposing them to players must apply visibility and book/scroll usability checks. Scroll queries validate stored compatibility and consumption tombstones; a payload is not global spell knowledge.

AtWill has no finite count: both count functions return **zero**, with `vancianisatwill` providing the explicit distinction. Prepared collections preserve duplicate usable copies. Known collections include committed suspended selections for policy comparison; they do not turn suspended choices into casting access.

## Functions

| Function | Parameters | Return | Contract |
| --- | --- | --- | --- |
| `vanciancasterlevel` | character, magiccapability | number | Effective caster level from the canonical identity's configured pure progression policy. |
| `vancianknownspells` | character, magiccapability | spell collection | Distinct committed selections, including suspended selections; does not grant access or capacity. |
| `vancianknownspells` | character, magiccapability, text | spell collection | Distinct committed selections in one repertoire alias or stable key, including suspended selections. |
| `vanciancandidates` | character, magiccapability, text | spell collection | Permitted ordinary-cast candidates in one repertoire. Candidate policies must be pure and must not call this query recursively. |
| `vancianpreparedspells` | character, magiccapability | spell collection | One spell per usable unspent memorised copy. Duplicate spells are intentional; reserved, spent and suspended copies are omitted. |
| `vancianslotsremaining` | character, magiccapability, text | number | Usable unspent finite copies/slots. AtWill returns the sentinel zero: use vancianisatwill to distinguish it. |
| `vancianslotcapacity` | character, magiccapability, text | number | Current finite configured capacity, regardless of spent entries. It does not initialise new slots. AtWill returns zero. |
| `vancianisatwill` | character, magiccapability, text | boolean | Whether the named allowance is AtWill; no fictitious finite capacity is returned. |
| `vancianselectedloadout` | character, magiccapability | text | Selected next saved plan name, or empty text. This is independent of the last committed pattern. |
| `vanciancanrefresh` | character, magiccapability | boolean | Pure validation of the selected plan, current book access, recovery and permission conditions. |
| `vanciancancast` | character, magiccapability, spell, text, text | boolean | Whether an eligible current route has a usable prepared/spontaneous/at-will casting. Does not resolve targets or pay costs. |
| `spellbookspells` | item | spell collection | Structured formulae on this item instance; no prose parsing or prototype defaults. |
| `scrollspell` | item | spell | Logical source spell for an intact usable charge, or null. Numeric potency belongs to the stored snapshot. |
| `scrollcastinglevel` | item | number | Stored casting level; zero for blank/spent/invalid items. Check scrollischarged to distinguish a real level-zero charge. |
| `scrollpower` | item | number | Stored SpellPower numeric value; zero for blank/spent/invalid items. |
| `scrollischarged` | item | boolean | True only for an intact, compatible, unconsumed stored charge. |
| `setvancianknownspells` | character, magiccapability, spell collection dictionary | boolean | Commit the whole Selected repertoire, keyed by rule alias/stable key. Canonical-owner permission and post-change hooks run once; no automatic capacity grant. |
| `selectvancianloadout` | character, magiccapability, text | boolean | Select an existing saved plan for the next refresh; does not refill slots. |
| `refreshvancian` | character, magiccapability | boolean | Request the configured refresh. True means accepted/started, not necessarily completed. All normal recovery and policy rules apply. |
| `copyspellformula` | character, magiccapability, item, spell, item | boolean | Begin timed copying. True means started; destination owns time/material costs. Scroll sources are consumed at commitment, with no effects or activation check. |
| `inscribespellscroll` | character, magiccapability, text, text, spell, number, item | boolean | Begin timed finite inscription using a positive slot ordinal; true means started. Actual actor pays and supplies potency; identity owns the reserved slot. |
| `inscribeatwillspellscroll` | character, magiccapability, text, text, spell, item | boolean | Begin timed inscription through an explicit AtWill allowance, with normal production/spell costs and no finite slot debit. |
| `begininscribespellscroll` | character, magiccapability, text, text, spell, number, item | text | Begin timed finite inscription and return the engine-issued reservation token, or empty text on refusal. |
| `vancianlastoperation` | character, magiccapability | text | Most recent operation token for this canonical owner/capability, or empty text. |
| `vancianoperationstatus` | character, text | text | Status of an operation owned by the actor's identity, or empty text. Pending/Invoking/NeedsReview are never automatically replayed. |
| `completevancianwriting` | character, text | boolean | Trusted crafting completion using a live engine-issued reservation token. Revalidates elapsed time, actor, state, item and all debits; no prepaid boolean bypass exists. |
| `cancelvancianwriting` | character, text | boolean | Cancel a live precommit writing reservation owned by this actor. Committed costs cannot be refunded. |

## Trusted crafting and notifications

`begininscribespellscroll` returns a token only after the normal shared service reserves an eligible casting and blank item. `completevancianwriting` requires that live engine-issued token, the original physical actor, elapsed production time, matching state version, accessible items and all remaining costs. A fabricated token or an early/repeated completion fails. It cannot accept a caller-provided prepaid flag or arbitrary snapshot XML. Player commands and script/craft adapters call this same service.

Boolean start functions report that work was accepted, which may mean a timed action is still running. Inspect the operation status or wait for the action's completion output before assuming a charge/formula exists. A false completion or an error after commitment may require staff inspection; it is never permission to replay a consumed charge.

Known-selection callbacks receive immutable distinct spell sets on the canonical identity. A callback is marked `Invoking` durably before execution. Errors become `NeedsReview`; neither reload nor another commit automatically retries it. Staff can acknowledge an inspected committed operation or cancel a precommit reservation. Progs should not implement an independent slot store.

The exact configurable hook signatures and worked interval examples are in the [builder guide](Vancian_Magic_Builder_Guide.md). The ten programs in [Vancian_Example_Progs.json](Vancian_Example_Progs.json) are compiled and executed by `EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; all public function overloads are compiled with their declared types by `EveryPublicVancianFunctionCompilesWithItsExactDeclaredTypes`.

## Spell metadata

Spell dot properties include `spelllevel` (number) and `scrollallowed` (boolean). These are the logical spell's current metadata. `scrollcastinglevel` and `scrollpower` describe the immutable charge instead. Existing spell availability, active-effect and parent-spell queries compare logical spell IDs so a detached snapshot still belongs to its logical spell without inheriting new numerical edits.
