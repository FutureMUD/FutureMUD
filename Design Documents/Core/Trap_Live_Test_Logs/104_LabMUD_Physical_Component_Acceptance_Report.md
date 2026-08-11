# LabMUD Physical Trap Component Acceptance Report

Date: 12 August 2026  
Database: `labmud_dbo`  
Listener: isolated local `127.0.0.1:4001`  
Account: existing development administrator; credentials redacted by the harness  
Triggerer: existing NPC template #77

## Outcome

PASS after one implementation correction found by the live run.

- The rerunnable Trap System Starter Pack added the `Trap Components` hierarchy and refreshed all stock mechanical definitions with tagged trigger/payload requirements.
- Item prototype #812r2 was built, submitted, reviewed, and approved with `Tripwire Trigger`, `Explosive Trap Payload`, and `Bear Trap Mechanism` tags plus Good quality.
- Stock Tripwire Explosive #6 bound that one item to separate trigger and payload requirements and displayed it as `TriggerAndPayload` on the south exit.
- `get tagged` was rejected while the part was installed.
- NPC exit traversal detonated the selected payload component.
- Stock Bear Trap #10 used the same item as a dual-role mechanism, reached Spent, retained the component, and `trap recover` successfully salvaged it and removed the trap.
- A zero-recovery explosive left no trap after the corrected trigger completed.
- The final Release seeder rerun reconciled the retained LabMUD hierarchy to `Functions / Trap Components`, added the signal-trigger tag contract, and reported a successful refresh.

## Defect found and corrected

The first successful detonation was followed immediately by harness shutdown. On reboot, the trap remained `Resolving` with its already-deleted component because charge consumption originally occurred after payload execution. The implementation now commits charge/state before running payloads, finalises cleanup after payload scheduling, and reconciles historical `Resolving` effects during load. The final retest first showed the old residue had been removed, then deployed and triggered a fresh tripwire and immediately received `You do not see a trap on that anchor.`

## Transcript index

- [97](97_LabMUD_Physical_Component_Discovery.txt): stock templates/tags and candidate item discovery.
- [98](98_LabMUD_Component_Builder_Experience.txt): complete item builder/review experience.
- [99](99_LabMUD_Tripwire_Component_Runtime.txt): ambiguous old `bomb` target correctly rejected as untagged.
- [100](100_LabMUD_Tripwire_Component_Runtime_Retest.txt): noun-only fixture keyword diagnostic; no trap created.
- [101](101_LabMUD_Tripwire_Component_Runtime_Final.txt): successful binding, debug display, pickup reservation, and detonation.
- [102](102_LabMUD_Spent_Cleanup_And_Recovery.txt): reboot residue discovery plus successful dual-role bear-trap spent recovery.
- [103](103_LabMUD_Resolution_Recovery_And_Cleanup_Retest.txt): final load-repair and post-detonation cleanup pass.
