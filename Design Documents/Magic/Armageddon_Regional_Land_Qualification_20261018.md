# Regional Land gathering qualification, 18 October 2026

Draft PR [#805](https://github.com/FutureMUD/FutureMUD/pull/805) adds optional Area/Zone shared Land life and configured room fallback. Base: merged PR803 `15ab4b3584133cb07e195082bda0771c1f70eb4b`. Managed validation: `35950ce3bce6e7e90e8cfc514b3c7dde256be8e5`. Final native validation after Python-only acceptance corrections: `311b108360d21894260df1086d4accfa2bc29dae`. Later receipt/documentation changes are outside that frozen execution. The [portable receipt](Armageddon_Regional_Land_Qualification_20261018.json) pins artifacts, source, assemblies, actual outcomes and limits.

Luke approved the formulas and immediate Defiler admission alongside the other Classes on fresh classes-on installs. No activation-policy decision remains. PR803's reviewed head `f1db44d432cba553cf950e36a830e2020470e771` passed the four reported hosted checks; merged-master default workflow [39099627791](https://github.com/FutureMUD/FutureMUD/actions/runs/39099627791) also passed. Pending-choice/CI wording in its dated packaging receipt is historical.

Package availability, installation and player permissions remain distinct. Release/Debug offer the package with full installation default **No**. A new explicit complete install enables approved casting, conditional Classes, independent gathering and approved device/craft policy. Existing-world policies, owner edits, players and unapproved carriers are preserved. Regional use requires builder binding and eligible terrain. Earth remains a fresh player path with native rest funding before Land setup. See the [installer guide](Armageddon_Installer_Integration_Guide.md) and [regional activation/upgrade/rollback guide](Regional_Land_Gathering.md).

## Changed behaviour

Configured Areas precede Zones; overlapping Areas use priority then stable ID. Indexed physical membership and cached terrain-weighted capacity avoid world scans during quotes/payments. One regional callback recovers 0.1 life per online real hour per owner. Nine families plus mixed cover heavy/light forest, field/grass/thorn, cavern/road/ruin, hills/mountain, sewer, desert/salt/silt/shallows, urban and forbidden land. Mixed unknown terrains contribute zero until configured. Private physical presets are representative templates; builders retain native behaviour for distinct physical variants.

Each stock Land action pays one complete shared life first, then its configured local route on shortage, without splitting payment. `land` retains field/woodland/pasture; `wildland` uses private native forage. Stock local fallback recovery is zero, avoiding multiplied regeneration. A new actual binding fills once; capacity growth, rebind and restart never refill it. Pool and receipt checkpoint together before ecology/body/credit; revision checks and quarantine span rooms. Protections and disabled rooms still apply. No comparative performance benchmark is claimed.

## Actual verification

| Check | Result and scope |
| --- | --- |
| Whole fast gate, once at `a58c8b6` | 11,212 passed, 2 failed, 3 skipped; execution/counts complete, stable source. Both failures were stale schema assertions (old latest migration and a single participant index). Corrected four-test database rerun passed at `ac91f10`. Skips are three Unix-only launcher-permission rows on Windows. Original failed gate retained; no second whole-gate result claimed. |
| Focused owning Debug checks | Library 13, gathering runtime 87, model 2 and installer/upgrade 6 passed in separate iteration runs, without skips. They overlap the broad gate and are not added into one total. |
| Harness builds | Debug and Release passed at `35950ce3b`: zero errors, 229 warnings each. |
| Focused Release | At `35950ce3b`: six installer/upgrade and seven regional runtime/protection checks passed; zero failures/skips. |
| Schema and blank snapshot | EF9.0.11-generated migration/designer/snapshot; no pending model changes. Real isolated snapshot refresh/import passed, including empty regional tables/no activation, complete migration history and cleanup. |
| Immutable foundation | Fresh compatible classes-on fixture passed 29 prefix steps, clean package boundary and repeat-prefix refusal/unchanged content. Historical fixtures were not resealed. |
| Original-stock native Release | PASS, 14 assertions at `311b10836`: normal enrolment, seven paid actions, shared6->0, physical movement, native forage3->2, reserve105 and exact pre-player-login cold conservation. Both servers graceful; owned database/Temp/process cleanup verified. |
| Definition-guard regressions | Six positive/negative Python checks passed. First boot stays exact; cold permits only the exact UnixEpoch/zero environmental timeline-default insertion. Formula/resource edits, nondefault or duplicate timelines and other tables remain strict. |
| Hosted exact-head CI | Final PR head requires its own Windows/Linux default-gate result. Keep draft while pending. A duplicate-push coverage guard is not a second test execution. |

The final native run conserved seven terminal receipts/participants, pool0/capacity6, B forage2, reserve105, scarsA6/B1 and saved skills. Natural Gather10-to12 progress was retained; the final B50 quote used actual Gather12/ecology-1. The guard observed only the exact default normalization on generators8 and11. Source start/end SHA256 matched `E6383E15D8718789DAB47EF8A98186FA6F55DCCAD2B6699AE0E81F285F4EC52B`; Release assemblies matched within each run and across the Python-only corrections. Final operator receipt SHA256: `0888662EA970D9EACD585611FBD8D26346689D70B0171802D2E85579E230C31C`.

Both earlier native failures remain retained. The first checked arrival while the Player was still walking; fallback/cold were NOT_RUN. The timing retry passed seven actions and graceful first shutdown, then a raw generator XML guard failed before the regional cold comparison (NOT_RUN). Normal ecology persistence exports two previously implicit timeline defaults. The final Python-only corrections wait for normal movement and accept only that exact cold insertion; every other definition byte remains strict and unexpected differences capture ID/XML. No gameplay timer, Player transport, C# source or approved formula changed. The final run used the same immutable base. Prior failures are not relabelled as passes.

Earlier failed/no-op iterations are also retained: missing restore assets, test constructor/namespace compilation, ambient-balance assertions that did not isolate native production, and missing typed ownership preflight entries. Runtime balance was not retuned to satisfy those assertions. An embedded-Python module lookup failed before guard cases ran; the explicit module-path invocation passed all six.

## Recommendation and limits

Shared-life, local fallback and pre-player-login restart scopes passed. Recommend enabling this optional extension after exact final-head hosted checks pass and builders explicitly configure their world. Keep PR805 draft while CI is pending. No activation-policy decision remains. No existing game activation, merge, deployment or release publication occurred or is authorised by this result.

Prior Class UI, passive-rest funding, paid spell practice, normal-body balance and lifetime/custody evidence retains its dated scope. The regional smoke does not repeat those campaigns, full UI, hour/ash waits, every Class/grade/mode or exceptional failures. Paid/callback flags are checked; no separate callback-register or wound-total certificate is inferred.

`ARM-V1-FOLLOWUP-001` Borrow injected secondary DELETE remains **FAIL**, recovery **NOT_RUN**, deferred and nonblocking by user priority; no repair is claimed. Wider exceptional campaigns and balance tuning stay deferred unless ordinary use exposes a concrete blocker.

Separate scheduled climate workflow [39109779214](https://github.com/FutureMUD/FutureMUD/actions/runs/39109779214) failed before tests on the same base: `.github/workflows/climate-tests.yml:24` restores the full solution on Ubuntu and encounters NETSDK1100 for Windows-targeted MudSharp Watcher, followed by MSB4181. This is an adjacent workflow follow-up; no climate result or repair is claimed. The separate merged-master default workflow passed.
