# Building Vancian magic

This is an engine feature with no installed class catalogue, stock spell set, books, scrolls or character grants. The [player guide](Vancian_Magic_Player_Guide.md) explains normal use; the [runtime reference](Vancian_Magic_Runtime.md) explains persistence and extension. Keep the [full specification](Vancian_Magic_Design_and_Implementation.md) as the decision reference.

## Prerequisites and policy programs

Examples assume you have created a school named `Arcane` whose player verb is `arcane`, a concentration/casting trait named `Spellcasting`, and ready ordinary-cast spells `Spark` (level 0) and `Ember Bolt` (level 1). These are illustrative names for your authored content, not shipped records. Use `magic spell set level <number>` to assign a non-negative base level. Existing spells migrate to level zero without becoming known or scroll eligible. Use `magic spell set scroll true` to opt in, and `magic spell set scrollcheck` to inspect compatibility. `scroll false` explicitly revokes unused scrolls while retaining already-applied effects.

Use the normal `prog` editor to create the ten programs in [Vancian_Example_Progs.json](Vancian_Example_Progs.json). That file contains exact names, parameter order and executable bodies. `VancianExampleProgTests` compiles every body against the real compiler and executes every example, including both sides of the elapsed-time permission check. Register these character variables before compiling:

```text
register character vancian_level number
register default character vancian_level 0
register character wizard_known_changed datetime
register character vancian_progression_allowed boolean
register default character vancian_progression_allowed false
```

Set the test character's progression explicitly, for example `setregister character <character> vancian_level 3`. The datetime register's unset value must be an old date (the normal DateTime default is suitable), not the current time, if the first daily selection should be permitted. A builder who wants to prohibit the first choice can set it to a recent date. Use separate register names if separate capabilities should have independent choice intervals.

| Program | Return and ordered parameter types | Behavior |
| --- | --- | --- |
| `vancian_example_level` | number(character owner, magiccapability capability) | Reads your MUD's `vancian_level` progression register. |
| `vancian_example_candidates` | boolean(character owner, magiccapability capability, magicspell spell) | True; engine school, readiness, trigger and configured level bounds still apply. Replace with your catalogue policy. |
| `vancian_example_limit` | number(character owner, magiccapability capability, number casterlevel, number spelllevel) | Three selected spells at each permitted base level. |
| `vancian_example_count` | number(character owner, magiccapability capability, number casterlevel, number spelllevel) | Two positions when caster level reaches that allowance's slot level; otherwise zero. |
| `vancian_choose_once` | boolean(character owner, magiccapability capability, magicspell collection previous, magicspell collection proposed) | Allows a change only while the previous selected union is empty. |
| `vancian_daily_change` | same permission signature | Requires at least 24 elapsed real hours since the recorded timestamp. |
| `vancian_record_change` | void(character owner, magiccapability capability, magicspell collection previous, magicspell collection proposed) | Records UTC `now()` only after a committed change. |
| `vancian_progression_change` | same permission signature | Reads the MUD-owned boolean progression flag; your calendar/event/progression systems set it. |
| `vancian_fixed_known` | same permission signature | Always false, including initial selection; use explicit staff grants. |
| `vancian_free_change` | same permission signature | Always true. |

The daily pair uses these exact supported statements, rather than comparing a timestamp to a duration:

```text
return totalhours(now() - getregister(@owner, "wizard_known_changed")) >= 24
```

```text
setregister @owner "wizard_known_changed" now()
return
```

Candidate, count, progression and permission progs must be pure. A choice callback receives canonical-owner and immutable old/new collection snapshots after commit; it is not a permission hook. A false permission denies initial choices, additions and replacements. A callback error leaves the choice committed and blocks further changes until staff repair/acknowledge it. Reordering identical per-rule sets runs neither hook.

## A mixed Wizard

Run the following with the capability editor. Each `accept vancian` confirms the preceding structural link change. You can use normal elapsed times such as `00:00:05` for a disposable demonstration and restore your intended game times afterward.

```text
magic capability edit new vancian Wizard Arcane Spellcasting
magic capability set casterlevel vancian_example_level
magic capability set canchangeknown vancian_daily_change
magic capability set onchangeknown vancian_record_change
magic capability set recovery mode PreparationAction
magic capability set recovery preparetime 00:10:00
magic capability set recovery interval 20:00:00
magic capability set repertoire add cantrips Selected
magic capability set repertoire cantrips levels 0 0
magic capability set repertoire cantrips candidates vancian_example_candidates
magic capability set repertoire cantrips limit vancian_example_limit
magic capability set repertoire add book Spellbook
magic capability set repertoire book levels 1 6
magic capability set repertoire book candidates vancian_example_candidates
magic capability set repertoire book bookpolicy EveryRefresh
magic capability set allowance add cantrip AtWill none
magic capability set allowance cantrip levels 0 0
magic capability set allowance cantrip repertoire add cantrips
accept vancian
magic capability set allowance add first Memorised 1
magic capability set allowance first levels 0 1
magic capability set allowance first count vancian_example_count
magic capability set allowance first repertoire add book
accept vancian
magic capability set allowance first repertoire add cantrips
accept vancian
magic capability set check
```

This single capability gives selected at-will cantrips and finite book-based memorisation; the same finite allowance also permits a selected cantrip upcast to level one. Add higher allowances explicitly with their intended slot level and bounds. No class-name test exists in the engine. A progression increase changes capacity queries but requires refresh before any added positions exist.

For PatternChangesOnly, use `magic capability set repertoire book bookpolicy PatternChangesOnly`. Repeating the complete prior book subpattern without a book then works; changing a spell, number of copies or casting level requires accessible formulae for the changed subpattern. Switching back to EveryRefresh requires books each time. Current prepared copies survive book loss in either configuration.

## Selected-repertoire Cleric

This configuration memorises from a chosen list. It uses the MUD's progression flag to control known changes and requires qualifying sleep followed by preparation.

```text
magic capability edit new vancian Cleric Arcane Spellcasting
magic capability set casterlevel vancian_example_level
magic capability set canchangeknown vancian_progression_change
magic capability set recovery mode SleepThenPreparation
magic capability set recovery sleeptime 08:00:00
magic capability set recovery preparetime 00:10:00
magic capability set recovery interval 20:00:00
magic capability set repertoire add prayers Selected
magic capability set repertoire prayers levels 0 6
magic capability set repertoire prayers candidates vancian_example_candidates
magic capability set repertoire prayers limit vancian_example_limit
magic capability set allowance add first Memorised 1
magic capability set allowance first levels 0 1
magic capability set allowance first count vancian_example_count
magic capability set allowance first repertoire add prayers
accept vancian
magic capability set check
```

Your MUD can set `vancian_progression_allowed` from a calendar observance, class advancement, quest or other external policy. Changing selected prayers affects future preparation; existing copies remain usable unless a hard casting predicate separately prohibits them. For a fixed NPC repertoire, use `canchangeknown vancian_fixed_known` and explicitly grant choices using `magic vancian known grant <character> Cleric prayers <spell>`, then `accept vancian`. Staff grants are audited and do not invoke player choice hooks. Granting the capability itself remains your ordinary merit/effect/content workflow.

## Spontaneous Sorcerer

This capability spends a refreshed finite slot on any eligible selected spell in the linked rule. SleepAutomatic earns one credit per observed episode and attempts refresh once on waking.

```text
magic capability edit new vancian Sorcerer Arcane Spellcasting
magic capability set casterlevel vancian_example_level
magic capability set canchangeknown vancian_choose_once
magic capability set recovery mode SleepAutomatic
magic capability set recovery sleeptime 08:00:00
magic capability set recovery interval 20:00:00
magic capability set repertoire add known Selected
magic capability set repertoire known levels 0 6
magic capability set repertoire known candidates vancian_example_candidates
magic capability set repertoire known limit vancian_example_limit
magic capability set allowance add first Spontaneous 1
magic capability set allowance first levels 0 1
magic capability set allowance first count vancian_example_count
magic capability set allowance first repertoire add known
accept vancian
magic capability set check
```

A purely spontaneous configuration does not need a memorisation plan. It still starts without finite slots and must refresh normally. After selecting Ember Bolt, the player uses `arcane vancian Sorcerer cast known first "Ember Bolt" next <target>`. Spontaneous/at-will allowances cannot link Spellbook sources.

## Complete capability editor surface

All settings use `magic capability set` on the open capability. `magic capability show <name>` and `set check` expose missing/invalid progs, unresolved links and schema issues. Incomplete intermediate definitions may be saved, but are disabled until valid.

| Setting | Accepted arguments / effect |
| --- | --- |
| `casterlevel`, `canchangeknown` | Required exact-signature prog. |
| `onchangeknown`, `cancast` | Prog or `none`; callback or hard actor-based casting restriction. |
| `basepower`, `upcaststep`, `reliableoutcome` | SpellPower; non-negative integer; MinorPass/Pass/MajorPass. Defaults Standard, 1, Pass. |
| `maxloadouts` | 1–1,000 saved plans. |
| `repertoire add` | Unique alias and Selected/Spellbook; starts incomplete. |
| `repertoire <alias> name`, `alias`, `order` | Text/text/non-negative order; stable key retained. |
| `repertoire <alias> levels`, `candidates`, `limit`, `bookpolicy` | Minimum/maximum; exact candidate prog; Selected per-level count prog; EveryRefresh/PatternChangesOnly. |
| `repertoire remove` | Alias; confirmation shows linked allowances. Reusing the alias later gets a new key. |
| `allowance add` | Unique alias, Memorised/Spontaneous/AtWill, finite level or `none`. |
| `allowance <alias> name`, `alias`, `order`, `levels` | Presentation and base spell bounds. |
| `allowance <alias> repertoire add/remove` | Explicit repertoire link; confirmed structural version change. |
| `allowance <alias> count`, `eligibility` | Finite count prog; optional extra spell predicate or `none`. AtWill rejects count. |
| `allowance <alias> mode` | Mode and level/none; confirmed version change. |
| `allowance remove` | Confirm removal; historical positions suspend. |
| `recovery mode` | PreparationAction/SleepAutomatic/SleepThenPreparation. |
| `recovery preparetime`, `sleeptime`, `interval` | Elapsed TimeSpan; required action/sleep durations positive, interval non-negative. |
| `recovery canrefresh`, `onrefresh` | Optional owner/capability permission/callback or `none`. |
| `bookuse`, `transcribe`, `inscribe`, `oninscribe`, `scrolluse` | Optional policy/callback or `none`; signatures below. |
| `scrolltrait`, `scrollthreshold`, `scrolldifficulty` | Reader check trait; successful minimum outcome; optional difficulty override or `none`. |
| `check` | All configuration errors, no state mutation. |

Standard concentration, regenerator, prompt and inherent-power settings remain supported. Renaming/reordering never grants slots. A structural edit suspends affected positions until refresh; it never retargets them by alias. Book rule source changes use remove/add with new identity.

| Additional hook | Exact signature |
| --- | --- |
| `cancast` | boolean(character actor, magiccapability capability, magicspell spell) |
| `canrefresh` / `onrefresh` | boolean / void(character owner, magiccapability capability) |
| `bookuse` | boolean(character actor, magiccapability capability, item book) |
| `transcribe` | boolean(character actor, magiccapability capability, magicspell spell, item source, item destination) |
| `inscribe` / `oninscribe` | boolean / void(character actor, magiccapability capability, magicspell spell, item scroll, number castinglevel) |
| `scrolluse` | boolean(character actor, magiccapability capability, magicspell spell, item scroll) |
| `scrolldifficulty` | number(character actor, magiccapability capability, magicspell spell, number storedlevel, number normalceiling) |

`scrolldifficulty` must return an integral valid Difficulty value. Invalid returns refuse before destruction. Default control difficulty is Normal for one excess level, staged once per further level and capped at Impossible. The control trait defaults to the concentration trait; the minimum is MinorPass. Reliable source potency is independent of this reader control check.

## Components and authored formulae

```text
comp edit new spellbook
comp set name VancianGrimoire
comp set capacity 100
comp set duration 600 + spelllevel * 60
comp set readable false
comp set check
comp edit submit
comp edit new spellscroll
comp set name VancianScroll
comp set duration 600 + castinglevel * 60
comp set readable false
comp set check
comp edit submit
```

Use the normal component review/approval workflow, attach each approved component to an ordinary held item prototype with `item set add <component-id>`, complete its normal material/description fields and approve/load instances. Every new book is empty and every new scroll blank. These commands do not install any stock items. For a readable surface, add an appropriate normal readable component and authored comprehensible writing, then enable `readable true`. The magic formula list is structured instance data and is never parsed from prose.

Shared component settings are `capacity <0..100000>` (books; zero means no formulae), `duration <expression>`, `eligibility <prog|none>`, `usable <prog|none>`, `readable <true|false>`, `plan add <normal inventory action>`, `plan remove <ordinal>`, `start|complete|cancel|release|fizzle <emote>` and `check`. There is no unlimited-capacity sentinel. Duration evaluates spelllevel, castinglevel and casterlevel in elapsed seconds and must be finite/positive at use. Ordinary inventory actions author retained tools, consumed items and liquids. Inscription combines this plan with the spell's own plan; an insufficient shared consumable quantity cannot satisfy both costs.

Book eligibility is boolean(item book, magicspell spell). Scroll eligibility is boolean(character actor, magicspell spell, number storedlevel), checked for inscription and release. Usability is boolean(character actor, item item). Missing configured references, bad signatures, unknown schema and malformed duration expressions are visible readiness errors. Emotes use `$0` actor, `$1` destination/scroll and `$2` source when copying (otherwise the destination). They are validated with the engine emote parser.

Spellscroll cannot share an item with stackable, container or spellbook components. Prototype composition checks work in either order. Copying an item with a charged scroll creates a blank scroll, never another charge. Books copy their formula lists independently. Prototype revisions do not replace instance payloads. Ordinary morphs do not transfer or multiply a charge.

Staff can author a formula onto an instance using `magic vancian book add <book> <spell>`. Removal uses `magic vancian book remove <book> <spell>` and confirmation. Populate two books, then change one; the other must retain its formulae. Use ordinary player transcription for paid copies. A book-to-book copy preserves the source, while successful scroll transcription destroys its source and stores only the base formula. It needs no personal casting capacity or activation roll.

## Crafting and FutureProg entry points

The [FutureProg API](Vancian_Magic_FutureProg.md) lists exact typed functions. Player commands and script/craft entry points call the same `VancianMagicService`. A true start result means a timed operation was accepted, not that a charge already exists. `begininscribespellscroll` returns the live engine-issued token; `completevancianwriting` requires that token, the original actor and elapsed duration, and revalidates every debit/access condition. `cancelvancianwriting` releases only precommit work. After reboot, inspect/cancel the persisted reservation instead of manufacturing a replacement token.

No public craft setter accepts arbitrary spell XML, a prepaid flag or an invented creator snapshot. Caster-side effects remain release effects: they are not transcribed into production costs. The [effect inventory](Vancian_Scroll_Compatibility.md) records supported numerical fields and unsupported reasons. `spell set scrollcheck` refuses unsupported effects before inscription payment.

## Staff repair and walkthrough

Use `magic vancian show <character> <capability>`, `operations <character> [capability]` and `scroll show <scroll>` for diagnosis. State output distinguishes current slots, saved plans, last pattern, suspended entries and callback status. Operations identify owner/capability, item IDs, stage, UTC time and diagnostic. `resolve <operation-id> cancel` is only for Reserved precommit work. `resolve <operation-id> acknowledge` closes indeterminate committed bookkeeping after you inspect/repair it. Both require confirmation, neither replays effects/hooks or grants replacement castings. `refresh <character> <capability>` is an audited forced refresh that still validates the pattern and books. `reset <character> <capability>` archives the prior definition and clears state only after outstanding work is resolved; it grants no slots.

Run the [player walkthrough](Vancian_Magic_Player_Guide.md) as a non-admin with an explicitly granted test capability. Demonstrate two copied Ember Bolts; spend one and verify the other remains. Borrow the source book, prepare, return it, and expend the second copy. Test both book policies with the same and a changed plan. Select Spark through the daily hook, verify unchanged commit does not update the register, then upcast it in a finite allowance and compare its ordinary at-will power. Run the Sorcerer workflow after sleep. Copy a high-level scroll formula without slots, and separately release an upcast scroll as a weaker reader with all personal slots spent. Change or remove the creator and verify stored numerical potency and reader attribution. Interrupt sleep and writing, reconnect, and inspect the unchanged expenditure/earned credit. Use the [verification report](Vancian_Magic_Verification.md) for which scenarios have actually run and the remaining live-environment limits.
