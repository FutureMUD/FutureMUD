# NPC memory and disposition mapping

## Source audit

The private source snapshot uses three independent linked lists of live character pointers. New NPCs start with empty lists (`db.c:2512–2514`). There is no saved memory payload or timed expiry. `memory.c` owns the add/query/remove routines. Its queries ignore non-NPC and Unique actors. Ordinary hate insertion excludes the Allanak militia and tribe 8; raw magical hate insertion bypasses that exclusion.

Confirmed producers and consumers:

| Reaction | Producers | Consumers |
| --- | --- | --- |
| Hate | Tribal damage responses (`combat.c:2594–2597,2973–2977`); Jakhal targeting a mount (`npc.c:100–104`); Chain Lightning, Earthquake, Immolate and Rain of Fire callbacks (`magick.c:1808,4664,7705,11628`); builder commands | Local aggression and target selection; adjacent approach; Hunt-trained pursuit |
| Fear | Successful fleeing removes hate and adds fear for visible characters left in the original room (`combat.c:5912–5940`), including bystanders; corresponding wagon escape branch; builder commands | Visible local feared characters cause escape before aggression unless the observer is hiding; relevant acquisition paths refuse feared targets |
| Like | Guardian and Hero Sword summons record their caster (`magick.c:6853–6855,7582–7584`); builder commands | Suppresses discretionary aggression and permits source mount admission. Does not itself trigger ally assistance |

Hate does not bypass every target exclusion. Preserve the actual `act_aggressive` and `is_target` ordering when connecting the import, including the source's limited hate overrides for particular guild/faction exclusions. The ordinary source damage response is not simply “every victim hates every attacker”: the tribal producer conditions and ordinary/raw insertion distinction matter.

`will_chase` excludes sentinels, merchants, Jakhal and Gesra, and requires a standing, non-fighting actor, visible non-immortal target and lawful combat admission. Ordinary adjacent acquisition covers one room. Hunt searches within `max(5, proficiency / 10)` rooms, then invokes the source hunting skill. Source visibility and searching do not establish permission for unrestricted native world-wide knowledge or teleportation.

Source defects deliberately excluded from native memory storage: `remove_from_likes` accesses the fear list incorrectly, and extraction removes other characters' hate/fear pointers without equivalent likes cleanup. Native cleanup must safely remove all three kinds.

## Approved lifecycle and implementation status

Luke approved session-only memory on 8 November 2026: no timed expiry, no persistence across restart, and removal when either participant logs out, dies or is removed. Native memory uses the live character instance rather than a historical character ID. Replacement NPCs start empty; no history is fabricated from worldfiles.

The reusable foundation is a non-saving `NpcDispositionMemory` effect with `remember`, `remembers`, `forget` and `rememberedcharacters` prog functions. Reactions remain independent. AggressorAI's opt-in `RespectDispositionMemory` setting suppresses attacks on liked/feared subjects (including delayed engagement) and uses native escape movement for visible local fears. The importer enables it for non-Unique actors, including reactive non-aggressors; these retain non-aggressive ambient classification. Generic targeting uses source hate overrides at the evidenced guild/faction, scavenger and mount gates, preserving the remaining exclusions and native admission.

Own and witnessed damage hooks produce ordinary tribal hate, retaining the militia/tribe-8 exclusion. Native mapped clans and the previously approved unnamed-group register supply group recognition. Jakhal mount acquisition remembers its target. The three character-targeted native spell equivalents use authored execute-prog callbacks; Rain excludes staff, nonphysical/Shadow recipients and an active Flame Mantle before damage can consume that protection. Native paid casting, selection, wards and resistance still control delivery; no callback invents a victim outside the native selected recipients. Ground Tremor uses an optional generic per-local-recipient callback after harm. The source raw-hate route bypasses ordinary militia exclusions but still ignores Unique actors and self targets.

Clay Sentinel and Air Guardian compose existing spell onload callbacks with a session Like toward their caster. Champion Blade remains its existing native temporary-item adaptation; no fictitious sword NPC or item-to-character memory is created. Native mount admission accepts a Like as temporary training permission while retaining occupancy, owner/approved-rider and handler restrictions. This never writes permanent taming, rider approval or ownership registers.

Actual exit escape emits native event 167 with departure-visible characters still left behind, including bystanders. Its imported hook clears hate toward those subjects and records fear. Cancelled movement, merely breaking melee and native route-coordinate retreat do not count as source room escape. The observer releases on cancellation, completion, death, logout or deletion. Source broken like-removal and stale extracted pointers are repaired rather than reproduced.

Integrated validation: shared library, runtime, seeder and both owning test projects build. Forty-four focused runtime checks pass, covering session/death-hook cleanup, silent native effect removal, independent/live-instance reactions, delayed engagement, actual escape/cancellation, pursuit admission/continuation and Ground Tremor callbacks alongside existing quake behaviour. Four seeder checks pass, including generated policy execution, complete private-corpus filter/pursuit-prog compilation, family/flag attachment and mount permission compilation. Earlier failed fixtures and the legacy null-emote save defect were corrected before this gate. The private-corpus compilation case takes about four minutes with the standalone EF fixture.

The repository test wrapper stalled without completed evidence. Direct `dotnet` checks supplied the recorded results; the seeder run used `testhost.dll` with its generated executable temporarily moved aside and restored in a `finally` block. Initial failure/incomplete receipts remain under `.artifacts/npc-memory-final*`; repaired receipts are under `.artifacts/npc-memory-fixed`. No fresh worldfile installation, live movement/spell session, database update or walkthrough restart has occurred.

Luke approved bounded native pursuit on 8 November 2026. Ordinary non-sentinels retain one-room acquisition and native combat-settings approach; Hunt-trained actors additionally use TrackingAggressorAI's opt-in pursuit-only mode within `max(5, Hunt / 10)` rooms. It owns pathing only; Aggressor retains engagement delay and authored openers. Sentinel, merchant, Jakhal and Gesra exclusions remain. Native visibility, peaceful-room/pacifism admission, doors and movement apply; changing memories or admission stops pursuit at the next native evaluation. Unavailable targets are not materialised, recalled or teleported.
