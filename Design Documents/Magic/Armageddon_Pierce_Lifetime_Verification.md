# Pierce accumulation verification checkpoint

Entry verification passes on source commit `7108d334415965ed15233ca9bf6a3f1e3f3c4fe6`, branch `codex/armageddon-five-stock-spells`, in the isolated `FutureMUD-five-stock` worktree based exactly on `1ad24d636925da64e7a82ffa858b0af470e56b0a`. Pierce stock remains pending parent independent review and integration acceptance. The [entry-specific receipt](Armageddon_Pierce_Lifetime_Verification_Receipt.json) sets `entry_verification_passed=true`, `stock_clear=false` and `independent_review_pending=true`. Its source binding is the execution commit above; this subsequent documentation commit is not a new execution claim.

The optional detection lifetime policy and the stock opt-in are separate local commits:

| Commit | Change |
|---|---|
| `d230a2efa7370f8df504917681ecbd77d1a57b2b` | Editable policy, persisted source grade, guarded cohort capture/revalidation and existing lifecycle application |
| `3162587cf93aab183149bb1101c493891792e6c7` | Read remaining duration through the native scheduler |
| `64b0f6cf3744574eb1fdc24f890155e04658b08a` | Separate Pierce stock policy and dedicated native/restart controls |
| `16e9373e81321b00a53ae18fe1367915f87c1f11` | Preserve caster-only cohorts across empty primary phases and capture prepared cohorts before callbacks |
| `7108d334415965ed15233ca9bf6a3f1e3f3c4fe6` | Preserve absent-policy prepared parents' original zero transient `ResolvedDuration` metadata |

The [policy contract](Detection_Accumulation_Lifetime_Policy.md) and [integration note](Armageddon_Pierce_Lifetime_Integration.md) describe authoring, failure semantics and source evidence. Recipient-local tagged parents accumulate across caster/spell IDs, retain the strongest source grade separately from editable native power, and become a fresh parent/child pair with current provenance. Untagged and differently grouped effects remain independent. Invalid increments, metadata and ambiguous cohorts refuse before payment; paid callback ambiguity uses existing review-required quarantine without replay or refund. Application reporting compares actual scheduler deadlines and retained strength, including paid no-change casts without mastery samples.

## Final frozen-source evidence

The strict ten-project unit run `20261005T132111Z-bd3bd16c3e43` passed **7,477 tests, zero failures and zero skips**, with complete counts and execution. All **57** Pierce lifetime, detection-operation and stock cases passed within that aggregate. The tests cover source whole-unit/inclusive-offset boundaries, independent source-grade/native-power mappings, both existing invocation routes, cross-caster/spell grouping, caster-only effects, malformed persisted metadata, prepayment refusal, callback mutation, cleanup, expiry, no-change reporting and absent-policy behavior.

The final native packet `.artifacts/test-runs/pierce-lifetime-final-native-02` passed all six modes: `pierce-stock`, `pierce-shared-runtime`, `provision-stock`, `five-stock`, `regressions` and `devices`. Both Pierce entries assert paid low/high casts, cap48 and strongest grade7, actual-deadline no-change reporting, configured and selected-grade prepared construction, cross-spell grouping, independent effects, prepayment refusal, independent-process policy/strength reload followed by ordinary paid low-grade accumulation, and expiry. Every mode confirmed deletion of its uniquely owned database and shutdown/deletion of its disposable server. Source and assembly manifests remained stable throughout the packet.

| Fingerprint | SHA-256 |
|---|---|
| Unit source start/end | `1144b01f3244af02df24cad3c15d234354203c5ccda2d754cf18dd40eda09159` |
| Native source manifest, 7,444 files | `bf9c5b8f3c747302c4dfeb8f7b0e1cbbfe20ba03cc37aef8757bdae3b51fbe9e` |
| Native assembly manifest | `901140a59af8a86c9d47afde96e70d269ef510ecbc2f6d5f20b63b28ca96758b` |
| Identical unit/Pierce/device `MudSharp.dll` | `385259f782aadaf44f94699c9c9035e57e1679f3de899f0bfbc66c144b25e9b8` |

The receipt binds 143 source files to Git blobs and byte hashes, 21 assembly records, 76 final artifacts and retained diagnostic artifacts. Both native harness builds succeeded with zero errors; their 13 and four warnings are existing referenced harness warnings. SDK was `10.0.401`. Earlier failed caster-only diagnosis, earlier focused runs and the preceding source-bound unit/native packets are preserved with their own scope; they are not substituted for this final execution.

## Historical fidelity and verification limits

Supported Library reads of `codedump.c` establish detect-invisibility duration5*grade, cap48, maximum power and same-affect accumulation/replacement. The source's `RT_ZAL_HOUR=600` is explicitly authored as this stock policy's real-time unit, independently of configurable native world hours. Boundary tests normalize the historical inclusive one-second expiry endpoint to native exact/subsecond deadlines. Existing saved remaining-time and cached/offline restoration pause/restore the native duration; that is a declared engine adaptation, not a claim about historical offline behavior. No additional expiry authority or schema is introduced.

Native qualification exercises real character/body/effect/resource/SQL paths with controlled catalogues, checks, terrain and clock. It does not certify full Telnet/login or setting acquisition. Cross-caster grouping and adversarial callbacks are qualified by durable runtime tests; native grouping specifically exercises different spell IDs. Prepared policy invocation requires an explicit selected-grade casting copy; unbound legacy releases refuse instead of deriving grade from power.

Historical provision/Pierce/shared-repair receipts and the central progress/repertoire ledgers remain unchanged. The parent-cleared shared runtime checkpoint retains its original evidence. No installer factory, shared native dispatch, charged-carrier implementation, main worktree, primary checkout or remote state is changed by this checkpoint. The existing owned stock-builder contribution is sufficient; no additional registry or shared dispatch addition is requested. There is no outstanding implementation or test failure. Parent independent review and integration acceptance are the remaining gate.
