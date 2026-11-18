---
slug: combat-and-arenas
category: Combat
summary: Test combat settings and equipment in a spar, then run a two-player test arena event.
---

# Combat and arenas

**Combat settings** are a character's preferences about targets, movement, attacks and defences. **Equipment and anatomy** determine which moves are available. The engine then checks the current situation, including reach, posture, stance, stamina and any move-specific permission. Changing a preference can change what the character tries without changing the weapon or its actual attacks.

A **spar** starts with an invitation and continues as friendly combat. It can still inflict in-game injury. An **arena** manages registration, staging, the bout and cleanup. A combatant class governs who can sign up. An event type supplies the reusable rules, and an event is one scheduled instance with its own participants and state.

## Test an opponent and equipment in a spar

Use a test room, two disposable characters and equipment already approved for practice in your game. Start without weapons if you want a baseline, then change one piece of equipment. Record the test characters' health, stamina, equipment and settings so you can compare the runs from similar starting conditions.

On each character, use `combat list` to find a suitable setting. Review it and set it before offering the spar.

```text
combat show <setting>
combat set <setting>
combat targets
inventory
```

`combat targets` reports whether the current settings would choose local targets. Use `combat config help` to see the editable options. Clone a setting with `combat clone <setting> <new-name>` when you want a separate practice configuration. For a simple comparison, try `combat defense dodge` on a test character, then repeat with `combat defense none` to clear the preference. A preferred defence still has to be available.

1. Stand together in the same room and use `spar <other-character>`. The other player types `accept spar` to accept the invitation before it expires.
2. Use `combat status` to check the participants and state. Observe a few actions, then have one of the players type `flee` to request an end to the friendly fight.
3. Check that the bout has ended and inspect both characters' equipment and condition. Repeat with one change, such as a different practice item. Record what attacks, defences and echoes were observed.

A spar can also end when someone becomes unconscious or helpless. Keep this trial in the practice room and use `spar` to issue the invitation. `hit` starts ordinary combat.

### Add an NPC practice partner

Choose a disposable live NPC from an approved template with suitable anatomy and no conflicting hostile AI. Test its ordinary equipment and combat settings first. As an Admin, create a `SparPartner` AI and attach it to that one NPC.

```text
ai typehelp sparpartner
ai edit new sparpartner WorkshopSparPartner
ai show
ai close
ai add WorkshopSparPartner <live-npc>
```

Check the displayed Will Spar prog and response delay. The type defaults to an always-true rule and a short random delay before accepting. Test `spar <live-npc>` as a player, then use `flee` to stop. When finished, `ai remove WorkshopSparPartner <live-npc>` removes the behaviour from that instance. To supply it to future NPCs, attach it to a template revision and review that revision. [NPC builder guide](NPC_Builder_Guide.md)

## Stage a two-person test arena event

Use two private test rooms, one for waiting and one for the bout. Reserve them for this arena. Choose a test economic zone and its currency, an arena manager and two consenting test players. The first test uses each player's own practice equipment, one slot per side, surrender enabled, and no automatic elimination. Keep fees at zero and leave betting and NPC backfill out of the exercise.

### Define the arena and one combatant class

These builder commands require Admin authority. Use the manager character's numeric ID. Use IDs or quote names containing spaces.

```text
combatarena edit new "Workshop Arena" <economic-zone> <currency>
combatarena set cell Waiting add <waiting-room-id>
combatarena set cell ArenaFloor add <bout-room-id>
combatarena set manager add <manager-character-id>
combatarena show "Workshop Arena"
combatarena close
```

The arena needs at least one Waiting room and one ArenaFloor to host a bout. Optional roles include Observation, Infirmary, NpcStables and AfterFight. Decide where participants should return and what staff should do about treatment before opening a public event. [Room building](Room_Building_Builder_Guide.md)

Use a Boolean prog accepting one Character for the eligibility rule. For a closed test, make a prog called `WorkshopArenaEligible` whose body is `return true`. Follow the FutureProg guide to create and compile it. Use a narrower rule when eligibility needs to depend on the character. [FutureProg for builders](FutureProg_Builder_Guide.md)

```text
combatantclass edit new "Workshop Arena" "Practice Class" WorkshopArenaEligible
combatantclass close
arenaeventtype edit new "Workshop Arena" "Practice Pair" 2
arenaeventtype show "Practice Pair"
arenaeventtype close
```

Creating the class first makes it available to both newly created sides. Confirm the two capacities are one, each side allows Practice Class, the type uses its own participants' gear, and surrender is enabled. `arenaeventtype set byo` and `arenaeventtype set surrender` toggle their current values. Use `show` to inspect them before changing anything. `arenaeventtype set side <side-number> capacity <count>` changes how many participants the side needs.

### Open registration and start

```text
arenaevent edit new "Workshop Arena" "Practice Pair" <future-date-and-time>
arenaevent close
arenaevent show <event-id>
```

Use a future real-world date and time in your account's format. Record the ID reported on creation. As the arena manager, inspect `arena events` and use `arena manager phase <event-id> RegistrationOpen` if the event has yet to open and you're ready to start registration. The manager phase command uses normal transitions and checks prerequisites before proceeding.

At the arena, player one enters `arena signup <event-id> 1 "Practice Class"` and player two enters `arena signup <event-id> 2 "Practice Class"`. With both sides full, registration can advance to Preparing. Confirm both players are staged and have their practice equipment. The event can advance on its normal timer. For an attended test the manager can advance sequentially with `arena manager phase <event-id> Staged` and `arena manager phase <event-id> Live` if those transitions are still due. Use `arenaevent show <event-id>` as an Admin or `arena events` as a player to track the state.

### End the test and inspect cleanup

Have one participant type `arena surrender <event-id>` while Live. Check the event state. With no automatic elimination the manager may still need to advance it through `Resolving`, `Cleanup` and `Completed` with `arena manager phase <event-id> <phase>`. Skip phases the event already finished. To terminate a stalled test, the manager can choose `Aborted` as the target phase.

Confirm each player is back under ordinary control and has the expected inventory and location. Inspect the final event state rather than assuming that a stopped bout completed. Arena events persist their history. Leave this test event out of any public rankings or betting campaign.

## Troubleshooting

| Symptom | What to check |
| --- | --- |
| The opponent doesn't accept | Check co-location, the invitation timeout, the target's current combat and, for NPCs, the attached AI and Will Spar prog. |
| The fight starts but the expected attack never happens | Verify the weapon is ready or the body has the intended natural attack. Check the combat setting's allowed classifications and intentions, current reach and position, stamina, and any usability prog. |
| The defence preference seems ineffective | Check which defences are possible in the current position with this equipment, stamina and incoming move. The setting can prefer a defence the body currently cannot perform. |
| Registration or staging is refused | Inspect arena room roles, event phase, side capacity and the class's eligibility prog. Confirm the player signed up for the intended event and side. |
| You can't change the phase as staff | Check the arena's own manager assignment. Verify the current state and unmet transition requirements before attempting to advance it again. |

For deeper rules, see the [combat strategy reference](../Combat/Combat_Strategy_Runtime.md), [combat settings defaults](../Combat/Combat_Settings_Defaults_Design.md) and [arena system](../Combat/Combat_Arenas_System.md).
