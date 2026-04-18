# FutureMUD Item System Content Workflows

## Scope
This document explains the builder-facing workflows developers use when creating, revising, attaching, loading, and validating item content.

The focus is not general builder onboarding. The focus is the minimum command surface developers need when implementing or testing item-system changes.

## Working with Component Prototypes
### Discover available component types
Use:
- `comp types`
- `comp typehelp <type>`

This is the fastest way to verify:
- the registration worked
- the builder-facing type keyword is correct
- the help text is useful

### Create a new component prototype
Use:
- `comp edit new <type>`

Telecommunications examples include:
- `comp edit new telephone`
- `comp edit new telecommunicationsgridoutlet`
- `comp edit new telecommunicationsgridfeeder`
- `comp edit new cellularphone`
- `comp edit new cellphonetower`
- `comp edit new implanttelephone`
- `comp edit new tape`
- `comp edit new answeringmachine`

Signal-automation examples now include:
- `comp edit new computerhost`
- `comp edit new computerterminal`
- `comp edit new computerstorage`
- `comp edit new networkadapter`
- `comp edit new pushbutton`
- `comp edit new toggleswitch`
- `comp edit new motionsensor`
- `comp edit new lightsensor`
- `comp edit new rainsensor`
- `comp edit new temperaturesensor`
- `comp edit new timersensor`
- `comp edit new keypad`
- `comp edit new microcontroller`
- `comp edit new automationmounthost`
- `comp edit new signalcable`
- `comp edit new signallight`
- `comp edit new electronicdoor`
- `comp edit new electroniclock`
- `comp edit new relayswitch`
- `comp edit new alarmsiren`

This goes through `GameItemComponentManager`, so failure here often means:
- the type was not registered
- the builder loader name is wrong
- the template output was not fully integrated into the codebase

### Edit, submit, review, and obsolete component prototypes
Key workflows:
- `comp edit <id>`
- `comp set ...`
- `comp edit submit`
- `comp review ...`
- `comp edit obsolete`

Because component prototypes are revisioned content, do not think of them as raw code-only artifacts. They are also builder-managed content definitions.

## Working with Item Prototypes
### Create or edit an item prototype
Use:
- `item edit new`
- `item edit <id>`
- `item clone <id>`

### Attach or detach components
Use:
- `item set add <id|name>`
- `item set remove <id|name>`

This is the key step that turns a generic item prototype into something functional.

### Important item settings for developers
Commonly relevant commands include:
- `item set noun`
- `item set sdesc`
- `item set ldesc`
- `item set desc`
- `item set size`
- `item set weight`
- `item set material`
- `item set quality`
- `item set cost`
- `item set group`
- `item set strategy`
- `item set morph`
- `item set destroyed`
- `item set onload`
- `item set register`
- `item set canskin`
- `item set hidden`
- `item set preserve`

These matter because many component features only make sense in combination with item-level settings. For example:
- item groups affect room presentation
- morph and destroyed settings affect component replacement behaviour
- registers and on-load progs can feed variable-style components
- skinnability affects how content can be customised by players

## Loading and Testing Live Items
### Manual load workflow
Use:
- `item load [<quantity>] <id> [<extra args>]`

This is the normal developer validation path for checking whether a prototype, its components, and its runtime behaviour work together.

### Extra arguments
The builder-facing load flow supports:
- skin selection
- register variable overrides
- quantity for stackable content

Developers should use these to validate:
- variable-driven item behaviour
- stackable behaviour
- skin overrides

For telecommunications content, also validate:
- whether the phone number belongs to the handset or the connected endpoint
- whether the device gets power from the correct source
- whether off-hook and busy-line behaviour match the intended call flow
- whether shared-number endpoints ring together and let later pickups join the live call
- whether fax machines can send to other fax numbers, reject or garble voice-only destinations, and expose the intended player command surface
- whether inbound faxes queue correctly when the machine has no paper or ink, and then flush from memory once supplies are restored
- whether cellular handsets only work when a powered cell tower on the same telecom grid covers the current zone
- whether implant telephones are linked to a neural interface, draw implant power correctly, and still obey the same cell-tower coverage rules as other cellular devices
- whether a chained answering machine lets downstream handsets ring first, then answers after its configured ring count
- whether the telecommunications grid creator enables hosted voicemail with the expected access number and keeps that reserved number unavailable for ordinary subscribers
- whether a custom greeting plays back with preserved language metadata and timing before the beep
- whether caller speech is recorded onto the inserted tape, and whether tape full or write-protect only blocks recording rather than the basic answer flow
- whether an unanswered hosted-voicemail-enabled line routes to the exchange mailbox only after local answering-machine opportunities have passed
- whether dialling the hosted voicemail access number from the subscribed line announces mailbox counts, plays message contents, and honours keypad deletion commands
- whether `dial <phone> <digits>` starts a call while idle and sends keypad digits once the call is already connected
- whether keypad-driven targets receive `TelephoneDigitsReceived` with the expected source item and digit string

For the current signal-automation slice, also validate:
- whether `computerhost` correctly tracks its powered state, mounted storage devices, terminal connections, network adapters, files, and stored executables
- whether `computerstorage` mounts into the intended host and exposes the expected files and executables through that host
- whether `computerterminal` only allows sessions when switched on, powered, and connected to a powered host
- whether `networkadapter` reports the expected connected host, local address, and local network-ready state
- whether source and sink bindings resolve to the intended local sibling components and endpoint keys on the same item
- whether `pushbutton` emits the expected value and then returns to zero after its authored duration
- whether `toggleswitch` changes between its authored on and off values through the normal switch flow
- whether `motionsensor` reacts only to its configured witnessed movement mode and minimum size, then returns to zero after its authored duration and only while powered
- whether `lightsensor` emits the current ambient illumination while powered and updates when the room lighting changes
- whether `rainsensor` emits the expected rain-intensity scale outdoors or climate-exposed and returns to zero while sheltered indoors
- whether `temperaturesensor` emits the current ambient temperature in Celsius while powered and updates when the room temperature changes
- whether `timersensor` alternates between its authored active and inactive values on the expected schedule and only emits live signal while powered
- whether `keypad` only emits its configured signal value after the correct numeric code is entered, and otherwise reports no activation
- whether `microcontroller` input bindings compile successfully after every `input add`, `input remove`, or `logic` change
- whether `automationmounthost` bay names, mount types, optional `AutomationHousing` access requirements, and mounted-power expectations match the intended content composition
- whether `signalcable` successfully mirrors a source across the intended one-room exit hop and clears correctly when unrouted
- whether `signallight` responds to threshold and invert settings without redundant extra echoes when the effective lit state is unchanged
- whether `electronicdoor` responds to threshold and invert settings and reaches the commanded open or closed state once any lock conditions permit it
- whether `electroniclock` responds to threshold and invert settings and correctly drives the underlying lock state
- whether `relayswitch` correctly opens or closes its relay according to its configured activation condition and only provides power while effectively closed
- whether `alarmsiren` only sounds while switched on, powered, and above its effective activation condition
- whether the live `electrical` and `programming` verbs target the intended parent items and components through normal keyword targeting, without relying on raw component ids
- whether live `electrical` inspection clearly shows controller input bindings, cable mirror routes, current values, and resolved versus broken signal links so end-to-end debugging is practical in game
- whether live `electrical` inspection also surfaces nearby routed cable segments clearly enough that builders can see which cable mirrors which source and through which exit hop
- whether service access is correctly blocked by a closed `AutomationHousing` item around a mount bay or cable end
- whether check failure still costs time, but does not permanently consume tools or materials
- whether abject failure on electrical work produces the intended shock echo and electrical damage
- whether switched-on mounted powered machines recover host-derived power after reboot or late topology initialisation without requiring a manual off/on cycle
- whether motion sensors ignore administrator movement when the mover is using `IImmwalkEffect`

### Manual load restrictions
Some components set `PreventManualLoad`, and item prototypes surface that through `PreventManualLoad`.

That flag exists for content that should only enter the world through controlled runtime systems, such as:
- corpses
- bodyparts
- currency-style special cases

If a newly added component sets this flag, developers should verify the restriction is intentional and documented.

## Revision and Update Workflows
### Why revisions matter
Items and components are not a simple "edit in place" system. Revisions matter because the world may already contain:
- current live items
- older item prototype revisions
- older component prototype revisions

### Updating prototypes and live items
The `comp update` workflow exists to reconcile:
- item prototypes that reference outdated component prototype revisions
- live items whose item or component definitions have moved forward

Use:
- `comp update`
- `comp update all`

This is especially important after changing component prototype structure or moving a current component into a new revision.

## Skins, Groups, and Related Content
### Skins
Skins are item-prototype-adjacent content that override presentation details such as:
- item name
- short description
- long description
- full description
- quality

From a workflow perspective, skins matter when:
- the base item prototype is intentionally generic
- builders need controlled presentation variants without duplicating item behaviour

### Item groups
Item groups are content-side presentation tools for rooms. They let many similar items collapse into grouped room descriptions instead of flooding room output.

Use them when validating:
- new high-volume item content
- environmental props
- grouped furniture or clutter

## Recommended Developer Workflow
When adding a new item capability, the most reliable end-to-end workflow is:

1. Implement and register the component code.
2. Use `comp types` and `comp typehelp` to confirm registration.
3. Create a test component with `comp edit new <type>`.
4. Configure and submit the component.
5. Create or clone a test item prototype.
6. Attach the component with `item set add`.
7. Configure item-level settings.
8. Load a live test item with `item load`.
9. Exercise the runtime behaviour in-game.
10. Run `comp update` when revision churn has occurred.

For the stage-1 answering-machine workflow, a practical end-to-end pass is:
1. Create a `tape` component and set its storage minutes.
2. Create an `answeringmachine` component and configure wattage, connectors, ring volume, premote, and answer-after-rings.
3. Attach both to test item prototypes and load them with a compatible telecom outlet and handset.
4. Insert a tape into the live machine.
5. Use `select <machine> greeting record`, speak in the same location, and finish with `select <machine> greeting stop`.
6. Place a call and let it ring through to validate greeting playback, beep timing, and message capture.
7. While the call is connected, use `dial <phone> 123#` to validate keypad relay instead of a second outbound call attempt.
8. Use `select <machine> messages play`, `select <machine> message <index>`, `select <machine> erase <index>`, and `select <machine> erase all` to validate inbox handling.

For exchange-hosted voicemail, extend that pass with:
1. Enable hosted voicemail and set the access digits on the `telecommunicationsgridcreator` component before loading the grid.
2. Enable hosted voicemail on the live subscribed line with the handset or endpoint control surface for that number.
3. Place an unanswered call and let the exchange pick it up after the configured ring-out threshold.
4. Speak a test message, then hang up or send `#` to end the recording.
5. From the subscribed line, dial the hosted voicemail access number and use keypad digits such as `1`, `3`, `7`, `9`, and `#` to validate playback, deletion, menu repeat, and hangup.

For the current microcontroller workflow, a practical end-to-end pass is:
1. Create a `pushbutton` component and set its keyword, signal value, duration, and emote.
2. Alternatively create a measurement or timed input such as `lightsensor`, `rainsensor`, `temperaturesensor`, or `timersensor`, or a coded `keypad`, and set its powered-machine and input-specific options.
3. Create either a `signallight`, `electronicdoor`, `electroniclock`, `relayswitch`, or `alarmsiren` sink component and set its source component prototype, threshold, and invert mode. In the current shipped slice, this binds to that source component family's default local `signal` endpoint.
4. Optionally create a `microcontroller` component and use `comp set input add <variable> <sourcecomponent>` for each sibling source component prototype. In the current shipped slice, these input bindings also target the source component's default local `signal` endpoint.
5. Use `comp set logic` on the microcontroller to author inline logic that returns a number.
6. Attach the authored components to the same item prototype with `item set add`.
7. Load the item and exercise the input:
   - `select <item> <button keyword>` for `pushbutton`
   - `select <item> <digits>` for `keypad`
   - `switch <item> on` / `switch <item> off` for `toggleswitch`
   - move through the same location as the composed item for `motionsensor`
   - change ambient lighting or move between differently lit locations for `lightsensor`
   - wait for weather changes or test in rain-exposed versus sheltered locations for `rainsensor`
   - change the ambient room temperature or move between differently heated cells for `temperaturesensor`
   - wait through at least one full active/inactive cycle for `timersensor`
8. Confirm the sink reacts through the authored sibling source component prototype or through the microcontroller output as authored.

For the live player workflow on an already loaded composed item, a practical end-to-end pass is:
1. Use `electrical <item>` to inspect local signal sources and configurable sinks and note the parent item / component names you intend to target.
2. If the item is an automation host, use `electrical install <host> <module> [<bay>]` to install a loose microcontroller or other compatible module item into a named bay.
3. If there is an `AutomationHousing`, confirm it must be open before install, remove, programming, or rewiring is allowed, and confirm access can be reached through normal `open` / `close` subtargets such as `open north panel`.
4. Use `programming <item>` or `programming <host@module>` to inspect the live microcontroller list, compile state, and current input bindings.
5. Use `programming <item> logic <component>` or `programming <item> logic <component> <text>` to replace controller logic on the live item.
6. Use `programming <item> input add <component> <variable> <source> [<endpoint>]` and `programming <item> input remove <component> <variable>` to manage live input bindings.
7. Use `electrical <item> bind <component> <source> [<endpoint>]`, `clear`, `threshold`, and `mode` to reconfigure live sinks, targeting sources by their parent item keywords unless an explicit `item@component` form is needed.
8. Use `electrical route <cable> <source> <exit> [<housing>]` to route a signal cable one room hop away and optionally place its destination end inside an adjacent `AutomationHousing` item.
9. Use `electrical unroute <cable>` and `electrical remove <host> <bay>` to undo the physical setup.
10. Use `electrical <item>` during debugging to confirm not just the authored bindings but also the live resolved chain, including controller inputs, cable mirror sources, nearby routed cable segments, current signal values, and whether each link is presently resolved or broken.
11. Confirm those actions run as staged delayed work rather than instantaneous mutation.
12. Confirm the required tool tag is available, the actor holds or readies the needed tool through the inventory plan, and the tool is restored after completion or cancellation.
13. Confirm ordinary failures cost the action time but do not consume materials.
14. Confirm abject electrical failure produces electrical shock rather than deleting or breaking components.
15. Confirm administrator characters bypass the tool, check, and delay layer for those live item actions while still respecting visibility and service-access gating.
16. When testing movement-driven automation, use an ordinary mover rather than an administrator with `IImmwalkEffect`, because immwalk movement no longer emits the witnessed movement events that motion sensors consume.

For broader manual regression coverage of the current shipped automation slice, use the dedicated [Automation End-To-End Test Scenarios](./Automation_End_To_End_Test_Scenarios.md) document.

For the standalone player-owned computer-program workflow in the current phase, a practical pass is:
1. Use `programming help` to inspect the computer-safe subset of FutureProg types, statements, functions, and collection helpers.
2. Use `programming new function <name>` or `programming new program <name>` to create a workspace executable.
3. Use `programming set return <type>`, `programming parameter add|remove|swap`, and `programming set source` while editing it.
4. Use `programming compile` to verify the executable in the relevant computer context.
5. Use `programming execute <which> [<parameters>]` to run it manually.
6. If the executable is a program that calls `sleep`, use `programming processes` and `programming kill <process>` to inspect or terminate persisted processes.

For the first real in-world computer workflow, a practical pass is:
1. Create and submit `computerhost`, `computerterminal`, `computerstorage`, and optionally `networkadapter` component prototypes.
2. Attach them to suitable item prototypes and ensure the host and terminal are also authored with the required power and connectable composition.
3. Load a live host, terminal, and storage item and physically connect them through the normal item connectivity rules.
4. Power the host and terminal on.
5. Use `programming terminal connect <terminal>` to create a live session.
6. Use `programming terminal status` to confirm the connected host and mounted storage list.
7. Use `programming terminal owner host` or `programming terminal owner <storage>` to pick the current programming owner.
8. Use the normal `programming list`, `new`, `edit`, `set`, `parameter`, `compile`, and `execute` verbs and confirm they now operate on the selected real computer owner rather than the private workspace.
9. Use file-oriented programs to validate `ReadFile`, `WriteFile`, `AppendFile`, `FileExists`, and `GetFiles`.
10. Use terminal-oriented programs to validate `WriteTerminal` and `ClearTerminal`.
11. Use `type <text>` while connected and confirm the terminal input surface routes through the current terminal session rather than the private workspace.
12. If there is only one nearby terminal, or one terminal clearly associated with the current `PositionTarget`, confirm `type <text>` auto-resolves and auto-connects to it even without a prior explicit `programming terminal connect`.
13. Use `LaunchProgram` and `KillProgram` from a host-backed executable to validate local host process control.
14. Disconnect with `programming terminal disconnect` and confirm the command surface falls back to the private workspace.

## Failure Patterns to Watch
- `comp edit new <type>` fails: registration problem.
- item loads but does nothing: the item probably lacks the component or the runtime component is not implementing the expected interface.
- boot-time load fails: likely missing or mismatched database loader registration.
- updated component changes do not appear on old content: update workflow was not run or the update hooks are incomplete.
- a microcontroller refuses submission: its inline logic probably does not compile in the `ComputerFunction` context or one of its input names is invalid
- a sink never responds: the authored sibling source component prototype probably does not match any source component instance on the same item, or the expected local endpoint is not present
- `electrical` or `programming` starts but immediately aborts: the configured tool tag or trait static setting is probably missing or resolves to a non-existent content record
- a live reconfiguration appears to complete but nothing changes: the targeted component may not implement the runtime-configurable interface, or the wrong nearby item / component may have been selected

## Thermal Source Workflow
Thermal-source items now have a standard content workflow:

1. Create the relevant component with `comp edit new electricheatercooler`, `fuelheatercooler`, `consumableheatercooler`, or `solidfuelheatercooler`.
2. Author the shared thermal profile:
   ambient, intimate, immediate, proximate, distant, and verydistant.
3. Author the activation model:
   - electric: wattage
   - fuel-fed: medium, specific fuel, burn rate, connector
   - consumable: duration, optional spent item
   - solid-fuel: valid fuel tag, capacity, seconds-per-weight
4. Submit the component revision.
5. Attach it to an item prototype with `item set add`.
6. Load a live test item and validate both room-temperature and proximity-temperature outcomes.

Recommended validation passes by family:
- electric
  - connect it to a suitable power source
  - switch it on and verify the room temperature changes indoors but not outdoors
  - move a character or item between proximity bands and verify the local effect scales correctly
- fuel-fed
  - connect it to a compatible liquid or gas source
  - confirm the authored fuel works and the wrong fuel does not
  - confirm disconnect or exhaustion stops the thermal effect
- consumable
  - load or create the item and verify it auto-starts
  - confirm it deletes or swaps to the authored spent item when expended
- solid-fuel
  - load it with correctly tagged fuel
  - confirm untagged items are rejected
  - confirm fuel burns one item at a time in load order
