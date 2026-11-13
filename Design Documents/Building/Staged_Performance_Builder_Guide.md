# Building and staging a performance

A **work** is the reusable script. Its roles, places, exits and props have names such as `speaker` and `platform`. A **production** uses one published revision of the work and binds those names to people and things in your game. You can rehearse the same script in a different hall by making another production.

## Write a first work

This short address takes one player and one room. An ordinary player can write it, stage it and perform it.

To start from an existing script instead, type `performance work fixtures` to list the three templates. For example, `performance work fixture concert "Friday Concert"` creates a draft for your account. Use `performance work show "Friday Concert"` to inspect its roles and cues, then edit and publish it before binding the venue.

```text
performance work new Evening Address
performance work edit "Evening Address" place add platform
performance work edit "Evening Address" role add speaker pc true platform
performance work edit "Evening Address" cue add speaker platform speak - 0 Welcome to tonight's gathering.
performance work edit "Evening Address" failure speaker The address has been called off.
performance work show "Evening Address"
performance work validate "Evening Address"
performance work publish "Evening Address"
```

Use short, distinct keys. `true` makes the role required. The dash occupies an unused cue argument, and `0` requests no additional pause. Validation checks the script's references. Publication makes the revision available to other organisers. Editing a published work creates a draft revision. Productions that already use an earlier revision keep it.

## Cast it and open the doors

Stand where the address should happen. Substitute the local keyword for the speaker, or use `self` if you are taking the part.

```text
performance plan new "Friday address" "Evening Address"
performance bind place platform here
performance cast speaker self
performance preview
performance call
performance signin "Friday address" speaker
performance start
```

The speaker receives the suggested words privately. Say or emote your own address, then type `performance done` to yield the floor. Use `performance status` to check the outcome. A later `performance call` on the selected production opens a fresh registration. For another staging of the same work, create a new named production.

## Choose roles and cues

| Role mode | What happens at the call |
| --- | --- |
| `npc` | Cast an eligible NPC. A player organiser needs a host with authority to assign that employee. |
| `pc` | The designated player must register at the starting room and layer. A required empty part holds the start. |
| `either` | Cast a PC or NPC. Use `performance cast speaker <player> fallback <npc>` for an understudy who can take over when needed. |

Players can use `performance calls` at a starting mark and `performance volunteer "<production>" <role>` to offer themselves. An organiser can use `performance assign "<production>" <role> <person>` or `performance remove "<production>" <role>` until kickoff. A player can withdraw. Travel and physical cues advance when the action succeeds.

Enter cues in performance order with `performance work edit "<work>" cue add <role|-> <place|-> <verb> <target|-> <seconds> [text]`. Use `performance work show "<work>"` to see cue numbers. Remove a cue by its current number, starting at 1. Append subsequent cues in the intended order.

| Verb | Role, place and target | Final text |
| --- | --- | --- |
| `speak`, `sing`, `emote` | Acting role at its mark, target `-` | Words or emote. An NPC delivers the direction. A PC chooses how to respond. Seconds pace an NPC line. |
| `travel` | Acting role, destination mark, directed exit key or `-` for travel to the mark | Omit |
| `get`, `give` | Acting role, current mark, prop key | For `give`, enter the required recipient's role key. |
| `play`, `stop` | Musician, current mark, instrument prop key | Optional style for `play`. Playing continues across cues until stopped. |
| `guard` | Usher, exit's origin mark, exit key | Omit. Use the audience side to guard the way onto a stage. |
| `stage`, `unstage` | Role `-`, source mark, audience mark | Omit. Add one cue per audience room. |
| `wait` | Role, place and target may all be `-` | Omit. Set the pause in whole seconds. |

## Bind the route and equipment

Define both places before adding an exit with `performance work edit "<work>" exit add <leg> <from> <to>`. After publication, bind the places with `performance bind place <mark> here` or a visible exit direction. From the origin, use `performance bind exit <leg> <direction>`. A return trip in the script needs its own leg if you want to require a particular exit.

Declare a prop on the work, then `performance bind prop <key> <item>` while you can see and target the actual item. Put handheld instruments in the musician's hands. Use a Get cue to borrow a prop from a floor or accessible container. A cast NPC can return borrowed equipment after the show and walk back to the room and layer where they were called. For a ceremonial gift use `performance work edit "<work>" prop cleanup <key> leave`. For an NPC who should resume normal duties where they finish, use `performance work edit "<work>" role cleanup <key> release`.

Players can copy published works and name public role rules that return Boolean and accept a character. Staff can inspect other drafts, select works and compatible role rules by ID, and bind places by room ID. Private eligibility rules require staff publication and staging authority. Conditional bookings always use a public Boolean rule accepting the hosting room. Stand in the appropriate venue when booking. A player staging guard or stage cues needs authority over the relevant premises.

## Set up a stage for spectators

For a concert, define a source place such as `platform` and an audience place such as `stalls`. Add a Stage cue where the audience should start receiving public action, and an Unstage cue at the end. For example, with those two places already declared:

```text
performance work edit "Opening Night" cue add - platform stage stalls 0
performance work edit "Opening Night" cue add singer platform sing - 8 The song begins.
performance work edit "Opening Night" cue add - platform unstage stalls 0
```

These three commands assume an existing draft called Opening Night with a singer role. Add another Stage and Unstage pair for each additional audience room. To post an usher before the song, place their travel and guard cues before the Stage cue. The guard uses a directed exit from the audience toward the platform. Spectators receive labelled public output subject to their sight, hearing and language comprehension. Private cues stay with the performer.

High administrators can also open an independent link with `stage open <source> <audience> <label>`. Use `stage list here` at either end to find its handle and `stage close <handle>` to remove it. An independent link has its own lifetime even when a performance uses the same two rooms.

## Rehearse, schedule and troubleshoot

Use `performance preview` before booking. If a bind fails, check the work's symbolic key, then the actual target, exit direction and layer. During a hold, `performance status` names the cue, missing prop or blocked leg. Correct the condition in the world while the grace period is open. A PC-only part needs a player. `performance done` yields an expressive cue after it has been prompted.

Use `performance book once "<local date and time>"` for a future call. Append `every 1 week` for a weekly event. Use `performance book when <public room prog>` for a condition that can turn false and later become true again. Cancellation delivers the failure direction locally where possible. Check status for borrowed equipment requiring a manual return.

The Mass, concert with ushers and procession walkthroughs include complete binding examples and interruption rehearsals. [Worked productions](../Core/Staged_Performances_and_Rites.md#playing-the-prototype)

## Add local instructions for players

Use a setting help topic to tell parishioners where registration takes place or concertgoers how to reach the gallery. With staff help-editing permission, enter `hedit new "Parish Services" Religion Attendance [tagline]`, replacing `[tagline]` with a short description. Enter the public text in the editor. Include the local meeting place, the exact command a player should type, and what they should expect to see next.

Use `hedit extra add "Parish Services" <staff rule>` to append instructions visible only to readers who pass a Boolean Character prog. Check `help "Parish Services"` as a player and as a staff builder. A setting package can also supply works and local help topics during installation. [Seeder guide](../Seeding/Staged_Performance_Seeder_Guide.md)
