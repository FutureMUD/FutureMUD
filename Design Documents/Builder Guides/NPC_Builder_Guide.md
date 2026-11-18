---
slug: npcs-and-ai
category: NPCs and AI
summary: Build and approve an NPC template, give it a two-room route, and configure a population spawner.
---

# NPC templates, spawners and AI

An **NPC template** defines the character that a new NPC starts as, including its body, skills, equipment and attached behaviours. A live NPC is one character created from that template. An **AI definition** supplies a particular behaviour, such as choosing a destination and walking there. A **spawner** periodically checks the population in its monitored zones and creates more NPCs when the population is below its minimum.

Approve an NPC template before loading it. AI and spawner settings take effect as you edit them. Use a dedicated AI and test template while iterating, since other NPCs may share an AI definition. `npc` requires Junior Admin authority. `ai`, `npcspawner` and the FutureProg editor require Admin authority.

## Make a guard walk between two rooms

Choose two test rooms connected in both directions by ordinary open exits. Use `room show` in each to record the room IDs and zone. Keep both rooms in the same monitored zone for the population test. Pick a Current simple NPC template with a suitable body and no competing movement or combat AI. The example uses room IDs `101` and `102`. Use your own IDs in the prog.

### Decide where the guard should go

Create a location-returning prog. The AI supplies the NPC as its one Character argument.

```text
prog edit new WorkshopGuardDestination
prog set return location
prog set parameter add guard character
prog set text
```

Paste the following source with the room IDs replaced and submit with `@` on a line by itself.

```text
if (isnull(@guard))
  return null("location")
end if
if (isnull(@guard.location))
  return null("location")
end if
if (@guard.location.id == 101)
  return tolocation(102)
end if
if (@guard.location.id == 102)
  return tolocation(101)
end if
return null("location")
```

Test `prog execute WorkshopGuardDestination self` from each endpoint and from a third room. Expect the opposite endpoint at each of the two rooms and a null Location elsewhere. This small route alternates endpoints when the AI reevaluates after movement. A null destination gives it no new route. For a one-way post assignment, return only the post as the destination. [FutureProg for builders](FutureProg_Builder_Guide.md)

### Attach the behaviour to a test template

Use the stock `AlwaysTrue` prog if it is available and accepts any parameters. You can also make a Boolean prog with a single Character parameter that returns `true` for this test AI. New `PathToLocation` definitions start with pathing disabled.

```text
ai typehelp pathtolocation
ai edit new pathtolocation WorkshopGuardRoute
ai set pathingenabled AlwaysTrue
ai set target WorkshopGuardDestination
ai show
ai close

npc clone <current-simple-template-id> WorkshopGuardTest
npc set ai add WorkshopGuardRoute
npc edit
npc edit submit
```

Check the clone before submission. It copies settings from the source. Remove any inherited behaviour that would interfere, using `npc set ai remove <ai>` while editing the clone. Note the new template ID. On a reviewer account, use `npc review <new-template-id>` and `accept edit <comment>` to approve it.

Stand at either endpoint and test one manually loaded NPC first.

```text
npc show <new-template-id>
npc load <new-template-id>
npc instances <new-template-id>
ai npclist WorkshopGuardRoute
```

The template should be Current, the live NPC should appear in both listings, and you should see it traverse the connected rooms. Inspect the test NPC from both ends. Avoid adding a second AI that wants to move the same NPC until the first route works. To pause the route during testing, edit this AI's `pathingenabled` prog to one that returns false, or use `ai remove WorkshopGuardRoute <live-npc>` for a single test instance. An NPC may first finish a move already in progress.

## Add a population spawner

Use a separate test zone or count the manually loaded guard as part of the monitored population. The Simple strategy uses authored spawn rooms. The minimum is the refill threshold and the target is the number the spawner tries to reach after it triggers. For example, minimum two and target two will ask for one more NPC if the original test guard is still in the zone.

```text
npcspawner edit new WorkshopGuardPopulation
npcspawner set npc <new-template-id>
npcspawner set strategy Simple
npcspawner set minimum 2
npcspawner set target 2
npcspawner set zone <test-zone-id>
npcspawner set cell <first-room-id>
npcspawner show WorkshopGuardPopulation
```

Confirm the zone, spawn room, template, current count, minimum and target before activating it. The `cell` option uses the room's ID. Zone and room selectors toggle membership. Set the activation prog last so the spawner has a complete configuration on its first active check.

```text
npcspawner set active AlwaysTrue
npcspawner close
```

The active prog must support a call with no arguments. Use an explicit Boolean prog with no parameters in place of `AlwaysTrue` if needed. The engine checks the population every five real minutes. After a check, use `npcspawner show WorkshopGuardPopulation` and `npc instances <new-template-id>` to confirm what now exists. Set `npcspawner set active AlwaysFalse` while editing to stop future fills after the test. Deactivation leaves existing NPCs in the world, so use a disposable test area and arrange cleanup with the game operator.

## Which editor to use

| Goal | Command |
| --- | --- |
| Find a template or see its loaded characters | `npc list +<keyword>`, `npc show <id>`, `npc instances <id>` |
| Change the AI future copies receive | `npc edit <template-id>`, then `npc set ai add <ai>` or `npc set ai remove <ai>`. Submit and review the revision. |
| Attach behaviour to one existing NPC | `ai add <ai> <npc>` or `ai remove <ai> <npc>` |
| Inspect or change a shared behaviour | `ai show <ai>`, `ai edit <ai>`, `ai set ?`, `ai npclist <ai>` |
| Run a prog when this spawner creates a character | `npcspawner set onspawn <prog>` using a Void prog that accepts one Character |
| Have future loads from a template receive an event hook | `npc set hook add <hook>` on the template, then submit and review. Check the event's recipient and arguments first. |

### Troubleshooting

| Symptom | What to check |
| --- | --- |
| Template loads are refused | Check that the selected revision is Current and your test room has a usable location. |
| The NPC stands still | Check `ai npclist`, the enable prog and the target prog. A null destination or the NPC's own room yields no new path. Inspect the actor's posture and other behaviours. |
| The prog resolves a destination but the NPC cannot reach it | Try a one-exit test route first. Check the current room overlay, both exit directions, terrain and door rules. `PathToLocation` searches up to twelve exits. Inspect `ai show` before toggling settings such as `ai set opendoors`. |
| Spawner count seems wrong | Verify template, monitored zones, and any counting prog. A manual load of the same template within the zone can already satisfy the minimum. An NPC walking out of the monitored zone stops contributing to that count. |
| Nothing appears after a spawner check | Verify that the active prog returns true, the count is strictly below minimum, and at least one valid spawn room is configured. Keep the test spawn room inside a monitored zone so a later check can count the NPC. |

For behaviour selection beyond this example, see [NPC AI and group AI](../AI/NPC_AI_and_Group_AI_Runtime.md) and the [event and hook reference](../AI/Event_System_for_AI_and_Hooks.md). See the [room-building guide](Room_Building_Builder_Guide.md) for exit and overlay setup.
