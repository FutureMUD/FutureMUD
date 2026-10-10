# Guardian churn checkpoint — 9 October 2026

N16 passes with runtime `3f97b3ba7802f5b95b87abebb7c9aeddc793cc60` and corrected batch scenario `01547b72bd87cb080b3152b3c57df9bf561047c3`:
**32 paid summon/use/kill/expire cycles**, each with a live
cleanup census before its cold restart, followed by **128 temporary guardians in one
uninterrupted process**. The campaign retires **176 outputs** from **33 paid casts**,
spending exactly **297 energy**, with **2,728 passing native assertions**.
The current Core and shared Library suites pass **7,069 tests, zero failures or skips**.
[The JSON ledger](Armageddon_Guardian_Churn_Checkpoint_20261009.json) pins common runtime source/binary inputs, the
normal-cycle and batch scenario revisions separately, and each retained receipt.
The normal 32 cycles remain source-identical at `3f97b3ba7802f5b95b87abebb7c9aeddc793cc60`. The batch-only client
correction uses `01547b72bd87cb080b3152b3c57df9bf561047c3`; every common captured runtime source and binary hash matches.

| Qualification | Observed result |
| --- | --- |
| 32 restart cycles | Eight cycles each for temporary cleanup / death on expiry, crossed with early death / natural expiry; sixteen single and sixteen paired casts produce 48 independent outputs. |
| Continuous batch | One configured 128-output cast; 64 early deaths and 64 natural expiries; no restart during cleanup. |
| Live runtime state | Actual actor/cache/NPC and detached/attached body counts, main/effect schedule heaps, every heartbeat cadence and creator guardian/follow delegates return to the exact baseline before restart. |
| Durable physical state | Exact NPC/body/instance identity sets return to baseline. Native corpses point through known loader fields to the correct output and disappear through scheduled decay. |
| Retained state | 176 canonical identity/archive pairs, lifecycle journals and 352 original typed claims; 33 unclaimed ordinary staffs reach their exact original rooms. The N15 permanent guardian remains the same ordinary living NPC/body. |
| Cold restart | Complete terminal journals/versions and claims stay byte-identical; no creation or payment replay; exact physical identities and custody remain unchanged. |

The stable census contains seven actors, six NPCs, seven attached body objects, zero
cached actors or detached bodies, eleven main schedules and zero effect schedules.
The retained permanent guardian explains one follower and its subscriptions. The
continuous batch adds exactly 128 actors/NPCs/attached bodies and the code-proven
creator event subscriptions; all return to baseline in that same process.

All 50 owned server processes exit successfully with stopped stdout collectors.
Every stage verifies MySQL shutdown and retains its data. Guardian/Fury definitions,
corpse policy and the temporarily distinguished permanent fixture's description and
canonical name fields are restored byte-for-byte with the MUD stopped.

The harness uses explicit typed claims, actual table identities/foreign keys, native
corpse loader fields and direct event delegates. It performs no numeric ID text scan,
guessed-field classification or archive-only schema classifier. Stable terminal state
is proven; individual internal callback invocation counts are not instrumented.

The first managed attempt found a test namespace collision. Two retained native
attempts caught a personal-name keyword collision before casting, then a mistaken
assumption that the detached-body registry contains ordinary actor bodies. Native
recovery retired the interrupted paid output without replay or refund and preserved
its foreign staff. A later campaign stopped when the shared SQL helper stripped the
old last journal's empty diagnostic delimiter after a new row sorted later. A read-only
audit proved that all 42 prior rows were unchanged; the sole difference was that TSV
delimiter. Complete nineteen-field JSON transport fixes it, and five regressions pass.
The fresh campaign starts at stage zero; the earlier 24 retired outputs, 16 ordinary
staffs and 144 spent energy remain explained, beyond the earlier single paid output.
Those attempts, read-only audit and recovery are separate from qualification.

A second campaign stopped after physical retirement when its full effect-schedule
count was two against a baseline of one. Three bounded probes passed; the instrumented
probe identified the baseline as NoTraitGain on ordinary guard #6. The failed extra
timer's owner remains unproven. The final fixture uses supported native nogain 0 on
the exact scalar-linked improvement models and lets existing effects expire naturally
before the baseline. Combat and skill gains remain active; no schedule is deleted or
excluded. Each stage restores all improver definition bytes. The second campaign's
ten retired outputs/seven staffs/63 energy and the probes' six outputs/three staffs/27
energy remain retained. These earlier attempts account for 41 outputs, 27 ordinary
staffs and 243 energy before the final normal-cycle campaign.

The 32 normal cycles all passed at the pinned runtime revision. Their first 128-output
batch stopped when the client compared the durable early-death count before its
queued kill commands caught up. A read-only exact-identity audit proves all 64 commands
completed before their original deadlines. Native recovery retired all 128 interrupted
outputs; a separate read-only audit proves no heavy records, unchanged archive/claim
links, retained staff #123 in the original room and the original nine-energy debit.
A second batch submitted 56 kills with 55 individual acknowledgements, then exceeded
the eight-second command-response limit during a recorded 18.3-second scheduler pause.
Native recovery and a hardened read-only audit verify all 128 interrupted outputs
retired, all 56 submitted kills were early, staff #124 survives and payment remains
charged. The retained transcript independently records those 56 submissions.

The successful corrected batch submits all 64 kills with bounded reads, then waits
at most 60 seconds for exactly 64 durable early-death records. All deaths must precede
their original 240-second deadlines; counts and the ordinary 900-second scenario
deadline remain fixed. Both failed batches, their recoveries and read-only audits do
not qualify N16. Seven mocked boundary checks prove malformed IDs issue no audit SQL
and unrelated recovery lineage is rejected. Historic missing-room receipts require
the preceding qualified fixture room and unchanged caster location; new in-progress
receipts capture the original recipient room immediately.
Retained nonqualifying N16 history totals 297 outputs, 29 staffs and 261 spent energy.
Qualifying reserve starts at 712, reaches 424 after 32 cycles, 415 and 406 after the
failed paid batches and recoveries, and 397 after the successful batch. The observed
315 debit includes the retained failed batches' 18 energy; qualifying casts spend 297.
No refund or purge occurred. Native response latency was observed, not qualified as a
separate performance or historical timing guarantee.

The 30*grade / 30-second corpse and 240-second batch timings are fixture policy,
not historical balance parity. Full Air Guardian wind-template/stats/components,
liking/prerequisite/seeder catalogue and other creation/topology adapters remain
pending. All 154 candidates, 152 required/two optional, 82 Sorcerer spells/four roots/
12 supports and eight larger systems remain unchanged. Full programme and Release
remain disabled. Draft [PR #788](https://github.com/FutureMUD/FutureMUD/pull/788) remains unmerged.

Next: N17: map and implement the bounded occupied-shelter dependencies for Spring Haven, Burrow Refuge and Sand Shelter, then qualify real utility, finite water, occupied expiry/destruction and safe fallback with preserved actors and foreign goods. Full Air Guardian catalogue/timing parity and other stock/readiness gates remain pending. Use only code-proven references; no static-definition classifier stage.
