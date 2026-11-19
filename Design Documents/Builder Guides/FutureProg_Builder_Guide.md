---
slug: futureprog
category: Builder tools
summary: Write, compile, test and attach FutureProgs for permissions, events and staged in-game behaviour.
---

# FutureProg for builders

A **prog** is a named script you write in game. Other systems call it to make a decision, calculate a value or do something in the world. A shop might ask a prog whether a customer can trade. An event hook can call another prog after a character finishes moving into a room.

A **signature** describes what the caller sends and what it expects back. `Boolean(Character)` means the caller supplies one character and reads a Boolean result. `Void(Character, Location, Exit)` means the caller sends those three values in that order and expects no result. Parameter names are yours to choose for a new prog. In the body, refer to a parameter named `viewer` as `@viewer`. An event template supplies parameter names as well as types.

Use the caller's editor help to choose the signature, then use `prog help type <type>` to inspect dot properties such as `character.roles`. Some editors accept several signatures and send different values for each one. A successful compile says the source and declared types are consistent. Test the actual caller to see which values it supplies and how it handles the result.

## The in-game editor

The `prog` command requires Admin authority. Creating a prog gives it an ID and selects it for editing. Progs have live definitions, so work under a new name until you are ready to attach it. Keep the ID for inspecting existing bindings with `builderrefs prog <id>`.

| Command | What it does |
| --- | --- |
| `prog list +<keyword>` or `prog list uncompiled` | Find a prog by name or comment, or list progs with compile errors. |
| `prog show <name or id>` | See the source, signature, static mode and compile state. |
| `prog edit new <name>` | Start a blank prog. Add the return type and parameters for its caller. |
| `prog edit newevent <event> <name>` | Start a Void prog with the event's parameter types and names. |
| `prog edit <name or id>` and `prog edit close` | Select a prog, or close the current edit selection. |
| `prog set return <type>` | Choose the result type, including `boolean` or `void`. |
| `prog set parameter add <name> <type>` | Append an input. The order matters when the prog takes more than one. |
| `prog set text` and `prog set append` | Replace the source or append to it. The editor compiles on submission. |
| `prog execute <prog> [arguments]` | Resolve typed arguments and really run the prog. Use disposable objects for actions that change the world. |
| `show events` and `show event <event>` | Discover event names, recipients and arguments. |

In the text editor, submit with `@` on a line by itself or leave without applying the text with `*cancel`. A submission with a compile error still updates the prog's source. Correct the error and submit again before binding it. `prog compile` recompiles all progs, so use the edit result or `prog show` when iterating on just one. Leave static mode off for progs that read roles, room state or registers, or perform actions.

## Write a role-gated help note

The role check comes from an in-game starting-outfit prog. It used `.any` to ask whether any of the character's chargen roles matched the outfit policy. Here the same pattern controls whether a workshop help note appears. The base help remains visible to everyone.

1. Use `role list` to find a role held by one test character and absent on another. Replace `123` below with that role's ID. Substitute your own unused names if this example already exists in your game.
2. Set up the prog and open its source editor.

```text
prog edit new CanReadWorkshopNotes
prog set return boolean
prog set parameter add viewer character
prog set category Workshop
prog set text
```

3. Paste this source and submit it.

```text
if (isnull(@viewer))
  return false
end if
return @viewer.roles.any(role, @role.id == 123)
```

`.any` tests each role using the condition on the right. `role` is a name local to that expression. An empty collection gives `false`. The initial check also returns `false` when an explicit null is supplied.

4. Check for a successful compile and test all three cases from an Admin account. Use the names of nearby test characters for the arguments in angle brackets.

```text
prog show CanReadWorkshopNotes
prog execute CanReadWorkshopNotes <character-with-role>
prog execute CanReadWorkshopNotes <character-without-role>
prog execute CanReadWorkshopNotes null
```

Expect `true`, `false` and `false` in that order. `self` supplies your own character, including their actual roles. Use `prog help arguments` for quoted names, room selectors such as `here`, collections and other parameter types.

5. Create a test help topic. Enter a line of ordinary text in the editor and submit. Then add the extra text with the prog and submit that editor too.

```text
hedit new "Workshop Notes Test" Building Examples Workshop practice notes
hedit extra add "Workshop Notes Test" CanReadWorkshopNotes
```

Try `help Workshop Notes Test` as both player characters. Both should see the base text. Only the character with the selected role should see the extra paragraph. This is a display rule. If you need to control an action such as starting a craft, attach a suitable Boolean prog to that action's permission field and test the action separately. [Crafting workflows](Crafting_System_Builder_Workflows.md)

## Send a notice when a test character moves

This example uses the same separation found in a scripted introduction. The event decides *when* to call the prog and the prog decides *what* to announce. Use two connected test rooms and a second character that can walk between them. Record the destination room ID with `room show`.

```text
show event CharacterEnterRoomFinish
prog edit newevent CharacterEnterRoomFinish WorkshopArrivalNotice
prog set text
```

The template supplies `mover` as Character, `destination` as Location and `exit` as Exit. Replace `456` with the destination room ID and submit this source.

```text
if (isnull(@destination))
  return
end if
if (@destination.id == 456)
  send(@mover, "The workshop notice board is just inside the door.")
end if
```

Run `prog execute WorkshopArrivalNotice self 456 null` with the same room ID to test the echo. The test supplies no exit because the prog never reads it. Then try an actual movement event. Creating and installing an event hook requires Senior Admin authority.

```text
hook create WorkshopArrivalHook WorkshopArrivalNotice CharacterEnterRoomFinish
hook install <nearby-test-character-keyword> WorkshopArrivalHook
hook list *WorkshopArrivalNotice
```

Have that character walk into the room normally through an exit. The moving character should see the notice once when movement finishes. Have them walk back to the other room, where there should be no notice. The hook is on the moving character for this event. A witness event has a different recipient and may supply different parameters. To stop the test, stand with the character and use `hook remove <nearby-test-character-keyword> WorkshopArrivalHook`, then walk the route again.

## Queue a second stage

An authored subway loop uses a persisted register on its zone to remember the current stop. A separate Number parameter records the current step. Its `switch` handles each stage, from announcing arrival through changing the available exit and closing the doors. The terminal stage queues no further call. The following small adaptation demonstrates the stage counter with two private messages.

Create `WorkshopClosingNotice` as a Void prog with `viewer` (Character) followed by `step` (Number). Paste this source into `prog set text` and submit it.

```text
if (isnull(@viewer))
  return
end if
switch (@step)
  case (1)
    send(@viewer, "The workshop will close shortly.")
    delayprog WorkshopClosingNotice 5000 @viewer 2
  case (2)
    send(@viewer, "The workshop is now closed.")
end switch
```

Run `prog execute WorkshopClosingNotice self 1`. Expect the first message immediately and the second after about five seconds. Running it with `2` should send only the second message. Neither branch closes a shop or changes room access. This example queues work in the current running game. The register used by the subway example keeps state separately from the timer.

A scripted room introduction uses a Boolean register to record whether a presentation is already playing. That flag guards against launching a second copy. The simple test above starts a second sequence if you run stage one twice, so test one invocation at a time.

- Decide which object owns the state before creating the register. A Location register is useful when each room has its own presentation. A Zone register is useful when several rooms share one service. Use `register show <type>` to inspect declarations and `setregister <type> <target> <name> <value>` for a deliberate test reset.
- Use `delayprog` for an explicitly queued follow-up. Its delay is in milliseconds. A periodic caller and a self-delaying chain can both schedule more work. Give the sequence one start path and test repeated starts and interruption before using it in a public area.
- Resolve game-local rooms, roles, languages, item prototypes and overlay packages on the target game. Treat IDs in example scripts as local configuration. Verify the expected rooms and overlays before running a stage that changes exits.
- Separate a state-reading policy from the action that changes things. In a multi-prog encounter controller, test the eligibility rule, the phase transition and the world action separately before connecting the whole sequence.

## Troubleshooting

| Symptom | What to check |
| --- | --- |
| The editor shows an unknown name or dot property | Inspect the actual parameter name and type with `prog show`. Use `@` before the variable and check `prog help type <type>` for its properties. |
| A caller refuses the prog | Match its input types and ordering and its required return type. Check compile output first. Use `newevent` for event arguments. |
| Manual execution works, but the event does nothing | Check the installed target, exact event and actual game action. Repeat using an ordinary test character walking through the exit. |
| A check behaves differently for two characters | Compare roles, nulls, and the actual values the subsystem passes. Check that the prog isn't set to cache a result. |
| A staged script runs twice or stays busy | Check the register owner and whether more than one start path queued work. Test the terminal stage and the conditions for clearing its state. |
| You're ready to retire a prog | Run `builderrefs prog <id>`, inspect the owning editors and remove or reassign the bindings. Preserve the old definition until you have checked the replacement. |

For signatures and available operations, see the [FutureProg function reference](https://futuremud.com/docs/futureprog/functions), [types](https://futuremud.com/docs/futureprog/types), [collection extensions](https://futuremud.com/docs/futureprog/collections) and [events and hooks](../AI/Event_System_for_AI_and_Hooks.md).
