# Multiple Body Forms and Instances Builder Guide

## Purpose

This guide is for builders and staff who want to author, test, or troubleshoot characters with more than one possible body. It covers both related systems:

- **Multiple body forms**: one character identity owns several bodies, but only one is active at a time. This is used for ordinary transformations, racial alternate forms, curses, and exclusive body switching.
- **Multiple simultaneous instances**: one character identity has more than one active world actor at the same time. This is used for passive bodies, player focus switching, NPC or scripted AI secondaries, astral projection, magical copies, and physical clones.

The most important design rule is:

```text
Use forms when the character becomes a different body.
Use instances when the character has more than one active body at once.
```

## Terminology

| Term | Builder Meaning |
| --- | --- |
| Identity | The durable character: account, name, merits, culture, role, journal, identity-level permissions, and legal or community standing. |
| Form | An owned body option for an identity. A form is usually dormant until switched into or spawned as an instance. |
| Primary instance | The ordinary loaded body for the character. Legacy character fields mirror this instance. |
| Secondary instance | Another loaded body belonging to the same identity. It is cell-local and not inserted into global character or NPC caches. |
| Focus | Which controllable instance receives the player's commands. |
| Body-local inventory | Items worn, wielded, held, implanted, lodged, or otherwise physically present on one body. Inventory is not identity-wide. |

## Choosing The Right Tool

| Goal | Use | Notes |
| --- | --- | --- |
| Character turns into another race/body and leaves the old body inactive | `form`, `body switch`, `SwitchForm`, or `ForceSwitchForm` | This is exclusive form switching. |
| Staff adds a test alternate body | `body addform` | Creates a dormant form owned by the character. |
| Staff removes an incorrectly created dormant body | `body delform ... confirm` | Refuses current bodies, embodied bodies, persisted instance references, backups, and remains references. |
| Staff creates a second visible body for testing | `instance spawn ... passive` | Default secondary is not player-controllable. |
| Staff creates a second body the player can control | `instance spawn ... focusable` then player uses `focus` | Focus is runtime-only and resets on login/logout. |
| Staff creates an AI-controlled duplicate of a PC | `instance spawn ... scriptai` or `SpawnBodyInstance(..., "scriptai", ..., ais)` | Use normal AI builder commands to define the AI. |
| NPC identity has another AI body | `instance spawn <npc> <form> ... npcai` | Secondary remains out of global NPC caches. |
| A spell creates an astral body | `astralprojection` spell effect | Creates a temporary player-focusable projection with planar rules. |
| A spell creates an illusion-style duplicate | `createcopy` spell effect | Can be passive or focusable, tangible or planar/intangible. |
| A spell creates a tangible clone | `createclone` spell effect | Tangible body-local clone. Inventory is not copied implicitly. |
| A prog creates a body instance | `SpawnBodyInstance(...)` | Best for scripted events, staff smoke tests, and content hooks. |

## Builder Command Reference

### Exclusive Forms

Player command:

```text
form
form <form>
```

Founder/admin commands:

```text
body addform <character> <race> [<ethnicity>] [<gender>]
body delform <character> <form> confirm
body formset <character> <form> alias <alias>
body formset <character> <form> trauma <auto|transfer|stash>
body formset <character> <form> echo <text>|default|none
body formset <character> <form> allow [true|false]
body formset <character> <form> canprog <prog>|clear
body formset <character> <form> whycantprog <prog>|clear
body formset <character> <form> visibleprog <prog>|clear
body formset <character> <form> sdescpattern <pattern>|random|clear
body formset <character> <form> fdescpattern <pattern>|random|clear
body switch <character> <form>
```

Notes:

- `body addform` provisions the new dormant body, generates target-race characteristics, target-race height and weight, blood, stamina, organs, bodyparts, and description pattern values.
- Transformation echo snapshotting uses the old and new descriptions so the echo does not become "A dog transforms into a dog."
- Health transfer preserves blood percentage rather than absolute volume when the form has a different blood capacity.
- Apparent age transfers by life-stage percentage rather than literal birthday when races have different lifespans.
- `body delform` is destructive and requires `confirm`. Prefer `body delform` for cleaning incorrect test forms rather than manual database edits.

### Simultaneous Instances

Staff commands:

```text
instance list <character>
instance spawn <character> <form> [here|room <cell id>] [persistent|temporary] [passive|focusable|ai|npcai|scriptai] [cloneinventory]
instance move <instance id|target> here|room <cell id>
instance retire <instance id|target>
instance audit <character>|all
```

Player commands:

```text
instances
focus
focus primary
focus <#>
```

Spawn modes:

| Mode | Use |
| --- | --- |
| `passive` | Visible, lookable, targetable, and movable by staff, but not controllable by the player. |
| `focusable` | Player-controllable secondary. The player uses `instances` and `focus <#>`. |
| `ai` | NPC targets get NPC AI control. PC targets get scripted AI control. |
| `npcai` | Explicit NPC AI secondary. Use for NPC identities. |
| `scriptai` | Explicit scripted AI secondary for PC-style identities, useful for evil twins and scripted encounters. |

Persistence:

| Policy | Builder Syntax | Behaviour |
| --- | --- | --- |
| Temporary on reboot | `temporary` | Default for staff-spawned secondaries; pruned on reboot. |
| Persistent | `persistent` | Reloads with the owning identity where supported. |
| Logout/effect-bound | Spell and service policy | Used by specific spell/runtime effects. |

Safety notes:

- One live embodied instance per body is enforced.
- Secondary instances remain out of `Gameworld.Characters`, `Gameworld.NPCs`, `Gameworld.Actors`, and cached actor collections.
- Retiring a secondary does not kill the primary identity.
- Temporary secondaries should be retired after manual tests when the MUD will keep running.
- `cloneinventory` is for deliberate staff or scripted scenarios only. Ordinary copy and clone spells do not copy inventory by default.

### AI Builder Commands

Useful commands:

```text
ai types
ai new <type> <name>
ai set ...
ai add <ai> <npc-or-scripted-instance>
ai remove <ai> <npc-or-scripted-instance>
ai npclist <ai>
```

For an aggressive same-identity test, `trackingaggressor` is a useful AI type:

```text
ai new trackingaggressor EvilTwinAggressor
ai set attackprog EvilTwinWillAttack
ai set range 0
ai set delay 1
```

`ai add` and `ai remove` can target loaded scripted-AI secondaries as well as ordinary NPCs.

## FutureProg Reference

### Form Functions

Inspection and switching:

```text
HasForm(character, form)
CurrentForm(character)
CurrentFormId(character)
CanSwitchForm(character, form)
WhyCantSwitchForm(character, form)
SwitchForm(character, form)
ForceSwitchForm(character, form)
```

Provisioning and editing:

```text
EnsureForm(character, formKey, race)
EnsureForm(character, formKey, race, ethnicity)
EnsureForm(character, formKey, race, ethnicity, gender)
CanSeeForm(character, form)
SetFormAlias(character, form, alias)
SetFormSortOrder(character, form, sortOrder)
SetFormTraumaMode(character, form, traumaMode)
SetFormTransformationEcho(character, form, echo)
ClearFormTransformationEcho(character, form)
SetFormShortDescriptionPattern(character, form, patternId)
RandomiseFormShortDescriptionPattern(character, form)
ClearFormShortDescriptionPattern(character, form)
SetFormFullDescriptionPattern(character, form, patternId)
RandomiseFormFullDescriptionPattern(character, form)
ClearFormFullDescriptionPattern(character, form)
SetFormAllowVoluntary(character, form, true|false)
SetFormVisibilityProg(character, form, prog)
ClearFormVisibilityProg(character, form)
SetFormCanSwitchProg(character, form, prog)
ClearFormCanSwitchProg(character, form)
SetFormWhyCantProg(character, form, prog)
ClearFormWhyCantProg(character, form)
```

### Instance Functions

Identity and physical-instance comparison:

```text
SameIdentity(lhs, rhs)
SamePhysicalInstance(character, target)
CharacterIdentityId(character)
CharacterInstanceId(character)
CharacterBodyId(character)
ToCharacterInstance(identityId, instanceId)
```

Spawning:

```text
SpawnBodyInstance(owner, form, location, mode)
SpawnBodyInstance(owner, form, location, mode, cloneInventory)
SpawnBodyInstance(owner, form, location, mode, cloneInventory, ais)
```

Parameters:

| Parameter | Meaning |
| --- | --- |
| `owner` | Loaded character identity whose dormant form becomes active. |
| `form` | Form alias or body id. |
| `location` | Cell where the secondary appears. Usually `@ch.location`. |
| `mode` | `passive`, `focusable`, `ai`, `npcai`, or `scriptai`. |
| `cloneInventory` | Boolean. If true, deep-clones direct worn, wielded, and held items from the source body. |
| `ais` | Text list of AI names or ids, separated by comma, semicolon, or pipe. Used by AI modes. |

`SpawnBodyInstance` returns the new physical actor as a `Character` prog value. It errors with a builder-facing reason if the form is missing, already embodied, structurally invalid, or an AI is missing or not ready.

## Spell Effect Reference

### `astralprojection`

Creates a temporary player-focusable astral projection from a keyed form.

Builder options:

```text
formkey <text>
race <which>
ethnicity <which>|clear
gender <which>|clear
alias <text>|clear
sort <number>|clear
plane <which>
anchorpolicy <helpless|sleep|stasis|none>
projectionecho <text>|default|none
anchorecho <text>|default|none
anchorroomecho <emote>|default|none
projectionroomecho <emote>|default|none
collapseecho <text>|default|none
backlashecho <text>|default|none
sdescoverride <text>|clear
```

The `sdescoverride` template supports `$desc` or `$sdesc` to insert the anchor body's current short description, for example:

```text
sdescoverride a spectral image of $desc
```

Astral projections are planar and cannot use ordinary physical interactions against material-only targets unless later content grants that through planar rules.

### `createcopy`

Creates a magical copy or mirror-image style secondary.

Builder options:

```text
formkey <text>
race <which>
ethnicity <which>|clear
gender <which>|clear
alias <text>|clear
sort <number>|clear
focusable [true|false]
persistent|temporary
persistence <persistent|temporary|logout|effect>
intangible [true|false]
plane <which>
collapseecho <text>|default|none
backlashecho <text>|default|none
```

Copies are temporary by default, non-corpse-producing, and do not copy inventory.

### `createclone`

Creates a tangible physical clone secondary.

Builder options:

```text
formkey <text>
race <which>
ethnicity <which>|clear
gender <which>|clear
alias <text>|clear
sort <number>|clear
focusable [true|false]
persistent|temporary
persistence <persistent|temporary|logout|effect>
deathecho <text>|default|none
backlashecho <text>|default|none
```

Clones are tangible, body-local actors. They do not copy inventory unless a separate staff or scripted path explicitly does so.

## Worked Example 1: Staff Adds And Tests A Dog Form

Use this when you want an exclusive transformation, not a simultaneous body.

```text
body addform me Dog
body formset me Dog alias dog
body formset me dog allow true
body formset me dog trauma transfer
body formset me dog echo default
body formset me dog sdescpattern random
body formset me dog fdescpattern random
form
form dog
health
form primary
```

Expected result:

- The character can voluntarily switch into the dog form.
- The old body is inactive while the dog body is active.
- Blood percentage and life-stage percentage are preserved across the switch.
- The transformation echo names the old and new forms correctly.

Cleanup:

```text
body delform me dog confirm
```

Only do this after switching out of the form and confirming it is not embodied by any secondary instance.

## Worked Example 2: Staff Spawns A Passive Secondary Body

Use this to verify that a second body can be present without focus switching or AI.

```text
body addform me Human
body formset me Human alias second
instance spawn me second here temporary passive
look
instance list me
instance move <instance id> room <cell id>
instance retire <instance id>
instance list me
```

Expected result:

- The primary does not move.
- The secondary appears as a separate lookable actor.
- `instance list` shows `Primary` plus a non-primary passive instance.
- Retiring the secondary removes it from the room and frees the body.

## Worked Example 3: Staff Creates A Player-Focusable Secondary

Use this for acceptance testing PC focus switching.

```text
body addform me Human
body formset me Human alias focusbody
instance spawn me focusbody here temporary focusable
instances
focus 2
look
focus primary
instances
instance retire <instance id>
```

Expected result:

- The player sees a numbered list from `instances`.
- `focus 2` moves command context to the secondary.
- `focus primary` returns context to the primary and removes the prompt focus marker.
- `quit` while secondary-focused logs out the primary identity and does not retire the secondary as a side effect.

## Worked Example 4: Astral Projection Spell

This assumes an existing magic school, capability, spell, and power framework. Exact spell/power command names depend on your local magic buildout, but the effect configuration is:

```text
magic spell edit <spell>
magic spell effect add astralprojection
magic spell effect set formkey astral
magic spell effect set race <astral race>
magic spell effect set alias astral
magic spell effect set plane Astral Plane
magic spell effect set anchorpolicy helpless
magic spell effect set projectionecho default
magic spell effect set anchorecho none
magic spell effect set anchorroomecho @ go|goes slack as &0's awareness slips outward.
magic spell effect set projectionroomecho @ shimmer|shimmers into being.
magic spell effect set collapseecho default
magic spell effect set sdescoverride a spectral image of $desc
```

Smoke test:

```text
cast "<spell>" standard
instances
focus primary
focus 2
look
```

Expected result:

- The primary body remains in its room.
- The projection appears on the configured plane and receives focus.
- Material-only item manipulation is blocked by planar rules.
- Collapse, expiry, logout, retire, or projection death returns focus to the primary when viable.

## Worked Example 5: Magical Copy And Physical Clone Spells

Magical copy:

```text
magic spell effect add createcopy
magic spell effect set formkey mirror
magic spell effect set race Human
magic spell effect set alias mirror
magic spell effect set focusable false
magic spell effect set temporary
magic spell effect set intangible true
magic spell effect set plane Astral Plane
magic spell effect set collapseecho default
```

Physical clone:

```text
magic spell effect add createclone
magic spell effect set formkey clone
magic spell effect set race Human
magic spell effect set alias clone
magic spell effect set focusable true
magic spell effect set temporary
magic spell effect set deathecho default
```

Expected result:

- Copies and clones use owned dormant forms.
- Inventory is not copied by default.
- Copy collapse creates no corpse.
- Clone death creates clone-specific remains and does not final-kill the identity.

## Worked Example 6: Prog-Created Passive Instance

Create a simple prog:

```text
prog edit new SpawnPassiveSecondBody
prog set return Character
prog set parameter add ch Character
prog set text
return SpawnBodyInstance(@ch, "second", @ch.location, "passive")
@
```

Execute:

```text
prog execute SpawnPassiveSecondBody me
instance list me
```

Expected result:

- The prog returns the spawned actor.
- The primary remains focused.
- The secondary is passive and temporary by default.

## Worked Example 7: Evil Twin Scripted AI

This is a deliberate staff smoke-test scenario: a PC identity gets a second AI-controlled body that attacks the primary. It is not a normal player-facing feature.

Create the will-attack prog:

```text
prog edit new EvilTwinWillAttack
prog set return Boolean
prog set parameter add attacker Character
prog set parameter add target Character
prog set text
if (SameIdentity(@attacker, @target))
    if (SamePhysicalInstance(@attacker, @target) == false)
        return true
    end if
end if
return false
@
```

Create the AI:

```text
ai new trackingaggressor EvilTwinAggressor
ai set attackprog EvilTwinWillAttack
ai set range 0
ai set delay 1
```

Ensure a form and create the spawn prog:

```text
body addform me Human
body formset me Human alias twin

prog edit new SpawnEvilTwin
prog set return Character
prog set parameter add ch Character
prog set text
return SpawnBodyInstance(@ch, "twin", @ch.location, "scriptai", true, "EvilTwinAggressor")
@
```

Execute and verify:

```text
prog execute SpawnEvilTwin me
look
instance list me
```

Expected result:

- The returned actor is a second body for the same identity.
- `instance list` shows a `ScriptedAi` secondary with AI details.
- The AI attacks the primary because `SameIdentity` is true and `SamePhysicalInstance` is false.
- `cloneInventory` is true in this test, so direct worn, wielded, and held items are cloned onto the spawned body where possible.

Cleanup:

```text
instance retire <instance id>
```

## Diagnostics And Troubleshooting

Use:

```text
instance audit <character>
instance audit all
```

Look for:

- multiple primary rows for one identity;
- more than one live embodied instance for one body;
- missing body or location references;
- malformed instance effect XML;
- embodied instances without locations;
- controllable instances with `NotControllable` policy;
- stale temporary rows after reboot.

Common issues:

| Symptom | Likely Cause | Fix |
| --- | --- | --- |
| `SpawnBodyInstance` cannot find a form | Alias mismatch or the form belongs to a different identity | Use `form`, `body formset ... alias`, or pass body id. |
| Spawn refuses because body is already embodied | The dormant body is already active as an instance | Retire the old secondary or create/provision another form. |
| Player cannot focus a secondary | It is passive, dead, in stasis, not loaded, not embodied, or not player-focusable | Spawn with `focusable` or use a spell/effect configured as focusable. |
| `body delform` refuses deletion | The body is current, embodied, backed up, referenced by an instance, or represented by remains | Retire instances and resolve references first. |
| AI secondary does nothing | AI has no valid target prog, is not ready, or was not attached | Check `ai show`, `ai npclist`, and `instance list`. |
| Two bodies look identical | Description patterns or sdesc override are intentionally matching | Use form pattern commands or spell `sdescoverride`. |

## Safety Checklist Before Live Use

Before shipping a new multi-body feature:

```text
instance audit <character>
instance audit all
instances
focus
look
quit while primary-focused
quit while secondary-focused
reboot if temporary cleanup matters
kill or retire the secondary if death policy matters
verify primary compatibility fields still match the primary body
```

For spells:

```text
cast starts in the expected body
room echoes are visible to bystanders where intended
self echoes are not duplicated
focus returns on expiry, death, retire, logout, and reboot cleanup
material interaction is blocked when planar/intangible
inventory is not copied unless explicitly configured by staff/prog
```

## Related Documents

- [Multiple Body Form System](./Multiple_Body_Form_System.md)
- [Multiple Simultaneous Body Instances Design](./Multiple_Simultaneous_Body_Instances_Design.md)
- [Character Description System](../Markup/Character_Description_System.md)
- [Magic System Spells](../Magic/Magic_System_Spells.md)
- [NPC AI and Group AI Runtime](../AI/NPC_AI_and_Group_AI_Runtime.md)
- [Corporeality and Planes](../World/Corporeality_and_Planes.md)
