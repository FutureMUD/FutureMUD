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
- whether cellular handsets only work when a powered cell tower on the same telecom grid covers the current zone
- whether implant telephones are linked to a neural interface, draw implant power correctly, and still obey the same cell-tower coverage rules as other cellular devices
- whether a chained answering machine lets downstream handsets ring first, then answers after its configured ring count
- whether a custom greeting plays back with preserved language metadata and timing before the beep
- whether caller speech is recorded onto the inserted tape, and whether tape full or write-protect only blocks recording rather than the basic answer flow
- whether `dial <phone> <digits>` starts a call while idle and sends keypad digits once the call is already connected
- whether keypad-driven targets receive `TelephoneDigitsReceived` with the expected source item and digit string

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

## Failure Patterns to Watch
- `comp edit new <type>` fails: registration problem.
- item loads but does nothing: the item probably lacks the component or the runtime component is not implementing the expected interface.
- boot-time load fails: likely missing or mismatched database loader registration.
- updated component changes do not appear on old content: update workflow was not run or the update hooks are incomplete.
