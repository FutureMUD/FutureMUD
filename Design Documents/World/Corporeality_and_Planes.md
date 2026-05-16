# Corporeality And Planes

## Purpose
The corporeality and planes system models metaphysical presence separately from ordinary room layering. Room layers still answer where a character or item is vertically or physically positioned inside a cell. Planes answer whether two perceivables can see, hear, speak to, touch, fight, carry, medically treat, magically affect, or physically block one another.

The default behaviour is intentionally conservative. Existing worlds, bodies, and items with no planar XML resolve as corporeal on the default `Prime Material` plane. They remain visible to ordinary sight, physically interactable, subject to breathing, drowning, falling, and environmental contact, and able to keep inventory normally.

## Core Concepts
### Plane
`IPlane` is first-class world data loaded into `IFuturemud.Planes`. A plane has a name, aliases, description, display order, an `IsDefault` flag, optional room-description addendum text, an optional room-name format, and an optional remote-observation ldesc tag. The core seeder and runtime loader ensure there is at least one default plane named `Prime Material`.

Builders manage planes with:

- `plane list`
- `plane show <plane>`
- `plane create <name>`
- `plane set <field> <value>`

The default plane is used by fallback material presence and by content that does not specify an explicit plane.

The optional room presentation fields let a plane alter how ordinary cells are shown without duplicating room content. `RoomNameFormat` is a string format where `{0}` is replaced with the normal layer-adjusted room name, for example `Astral Plane {0}`. `RoomDescriptionAddendum` is appended with other coded room-description cues, such as shop and bank notices. `RemoteObservationTag` is a string format where `{0}` is replaced with the plane name; it is appended to character and item ldescs when the viewer can see the target because the viewer can perceive or occupy that other plane. ANSI colour codes are supported. Null or blank values leave room names, room descriptions, and remote ldesc tags unchanged.

The stock core data now includes:

- `Prime Material`, the default plane, with no extra room-name, room-description, or remote-observation presentation.
- `Astral Plane`, with the stock room-name format `Astral Plane {0}`, an astral room-description addendum, and the stock remote-observation tag `({0})`.

### Planar Presence
`PlanarPresenceDefinition` is the XML-backed authored model stored on body prototypes, item prototypes, and overlay effects. It records:

- planes where the perceivable is present
- planes that can see it
- planes that it can perceive
- interaction permissions per `PlanarInteractionKind`
- whether physical contact, breathing, falling, and mundane environmental contact are suspended
- whether inventory follows the transition or is gently ejected
- whether mundane closed doors or magical barriers can be crossed
- whether player `manifest` or `dissipate` commands are allowed

Runtime code resolves this into `PlanarPresence` through `IHavePlanarPresence`, `IPlanarOverlayEffect`, and `IPlanarOverlayMerit`.

### Interaction Kinds
The shared interaction enum is:

- `Observe`
- `Hear`
- `Speak`
- `Physical`
- `Combat`
- `Inventory`
- `Medical`
- `MovementBarrier`
- `Magic`

This lets a visible ghost still be targetable for speech while combat, inventory, surgery, and ordinary physical manipulation remain blocked.

## Authoring Surfaces
### Body And Item Prototype Defaults
`BodyProtos.PlanarData` and `GameItemProtos.PlanarData` are nullable XML fields. Null or invalid XML is treated as default Prime Material corporeality.

Item prototypes expose a builder command:

- `item set planar default`
- `item set planar corporeal [plane]`
- `item set planar noncorporeal [plane] [visible]`
- `item set planar xml <xml>`

Body prototype XML is loaded through the same shared parser and is intended for stock race/body definitions, permanent spirits, or builder-authored forms.

### Overlay Effects
`PlanarStateEffect` is a saved overlay effect implementing `IPlanarOverlayEffect`. It is the common runtime state used by staff commands, FutureProg functions, player transitions, and non-spell power paths.

Spell-owned planar changes use `SpellPlanarStateEffect` underneath a `MagicSpellParent`, so they expire and save with the spell effect stack rather than becoming ordinary permanent state.

Permanent character states can also come from the `Planar State` merit. Drug-driven temporary states use `DrugType.PlanarState`, which applies a non-saving drug-induced planar overlay while the drug intensity remains active.

The stock merits package seeds reusable planar-state merits for builders to assign to races, roles, or chargen choices:

- `Always Astral`
- `Astral Manifestation`
- `Astral Ignorant of Prime Material`
- `Astral Visual Manifestation`
- `Astral Sight`
- `Dual Natured`

### Admin And Debug
Staff can inspect or force state with:

- `corporeality show <target>`
- `corporeality set <target> corporeal [planes] [duration]`
- `corporeality set <target> noncorporeal [plane] [duration] [visible]`
- `corporeality add <target> corporeal <planes> [duration]`
- `corporeality add <target> see <planes> [duration]`
- `corporeality add <target> visibleto <planes> [duration]`
- `corporeality clear <target>`

`set` is a hard admin override that replaces existing ordinary `PlanarStateEffect` overrides. `add` layers an additional non-overriding planar capability onto the target, which is useful for storytelling scenes where a target should also see another plane, be visible from another plane, or be corporeal on several planes at once. Plane lists accept IDs, aliases, quoted names, and comma-separated tokens. Durations schedule expiry; undurated effects persist until cleared. Any `set`, `add`, `clear`, or timed expiry that changes whether an observer in the room can see the target sends that observer a fade-in or fade-out echo.

`PerceiveIgnoreFlags.IgnorePlanes` bypasses planar visibility for admin, debug, and true-description style views.

### Player Transitions
Players can use:

- `manifest`
- `dissipate`

These commands only work when the active planar overlay grants the relevant transition flag. Permanent ghosts, temporary powers, spells, merits, or staff actions can therefore decide whether a player can voluntarily change state.

### FutureProg
Current FutureProg hooks are:

- `planeof(perceivable)`
- `planesof(perceivable)`
- `canperceiveplanar(perceiver, perceivable)`
- `caninteractplanar(perceivable, perceivable, text)`
- `applyplanarstate(perceivable, text)`
- `applyplanarstate(perceivable, text, text)`
- `clearplanarstate(perceivable)`
- `setplane(perceivable, text)`

The text state argument accepts `corporeal`, `manifest`, `manifested`, `noncorporeal`, `incorporeal`, `dissipate`, or `dissipated` where relevant.

### Magic
Spell effect tokens are:

- `planarstate`
- `planeshift`
- `removeplanarstate`

`SamePlaneOnly` power filtering and `SensePower` now use shared planar overlap checks instead of the older placeholder logic.

### Merits And Drugs
The `Planar State` merit stores the same `PlanarData` XML and can be configured with `planar corporeal`, `planar noncorporeal`, `planar xml`, `priority`, and `override`.

Drugs support `PlanarState` as a drug type. Builders set its intensity with the normal `type intensity PlanarState <%>` command and configure the state with `type planar <corporeal|noncorporeal> [plane] [visible]`.

## Runtime Behaviour
### Perception And Targeting
`BodyPerception.CanSee` runs biological and sensory checks first, then applies planar visibility before ordinary obscuring effects. Cells and other locations bypass the perceivable planar-presence check so an observer who is corporeally present on the Astral Plane still sees the room through the normal lighting model instead of failing because the cell itself is default-material. Characters and items keep the normal planar visibility rules. `VisualEthereal` and `SenseEthereal` perception grants can reveal targets whose planar profile allows ethereal detection.

Sight-based targeting still resolves visible-only targets. This is deliberate so commands such as `tell`, `whisperto`, and other speech or observation commands can use visible spirits even when physical interaction would fail.

Long descriptions for characters and items can include a remote-plane addendum such as `(Astral Plane)`. The addendum appears only when the viewer sees the target through a remote plane the viewer can perceive or occupy. It does not appear when the target is visible to the viewer's current plane as a noncorporeal manifestation, and it does not appear when a fully multi-planar target is visible on the viewer's current plane.

`survey` reports the perceiver's current plane. If a perceiver is present on more than one plane, the default plane is treated as the current presentation plane when present; otherwise the lowest display-order presence plane is used.

### Physical Interaction
High-traffic physical paths call `CanInteractPlanar` with the relevant interaction kind. Combat engagement, item manipulation, inventory intervention, medical action, and movement barriers are separated so future content can allow one kind without allowing all physical access.

### Movement
Noncorporeal profiles can bypass mundane closed doors when `CanCrossClosedDoors` is true. Magical barriers remain blocking unless the profile explicitly grants magical barrier crossing.

Movement costs such as stamina, pain, or damage are reserved for transition profiles. The first implementation records the profile name but applies the default no-extra-trauma behaviour.

### Physiology
When `SuspendsPhysicalContact` is true:

- breathing need and breathing failure checks are suspended
- drowning is prevented through the breathing suspension
- falling checks treat the body as not subject to ordinary falling contact
- mundane environmental contact is expected to be ignored by callers using the shared helper

This is the default for fully noncorporeal profiles.

### Inventory And Implants
If a transition does not propagate inventory, the body gently ejects direct held, wielded, worn, implanted, and prosthetic items into the current cell/layer. This is intentionally non-traumatic by default; more violent transitions should be expressed by a specific transition profile or additional effects.

## User Stories
- A builder creates an `Ethereal Plane` and makes a ghost body prototype present there, visible to Prime Material observers, but unable to be touched or fought.
- A spirit with detect-ethereal visibility can see otherwise hidden planar entities because its perception grants include ethereal senses.
- A player under a temporary power can `dissipate` to pass a closed mundane door, but a magical warded exit still blocks them.
- A staff member uses `corporeality set <target> noncorporeal Ethereal visible 00:10:00` for a temporary scene effect and later clears it.
- A spell uses `planeshift Ethereal noncorporeal` to move a target into an untouchable state, while `removeplanarstate` restores the ordinary prototype baseline.
- A character who becomes noncorporeal through a non-propagating effect drops worn, carried, prosthetic, and implanted physical objects into the room without extra injury.

## Implementation Map
| Area | Runtime surface |
| --- | --- |
| Shared contracts | `FutureMUDLibrary/Planes` |
| Persistence | `Planes`, `Planes.RoomDescriptionAddendum`, `Planes.RoomNameFormat`, `Planes.RemoteObservationTag`, `BodyProtos.PlanarData`, `GameItemProtos.PlanarData` |
| Runtime data | `MudSharpCore/Planes/Plane`, `PlanarStateEffect`, `SpellPlanarStateEffect`, `PlanarStateMerit`, `DrugInducedPlanarStateEffect` |
| Loading | `FuturemudLoaders.LoadPlanes()` before other world systems depend on defaults |
| Perception | `BodyPerception.CanSee`, `PerceiveIgnoreFlags.IgnorePlanes` |
| Interaction | `PlanarPresenceExtensions.CanInteractPlanar` |
| Movement | `CharacterMovement` closed-door, magical-barrier, and falling checks |
| Physiology | `BodyBiology.NeedsToBreathe` and `CanBreathe` |
| Inventory transition | `Body.EjectInventoryForPlanarTransition()` |
| Magic | `MagicPowerBase`, `SensePower`, planar spell effects |
| Builder commands | `plane`, `corporeality`, item prototype `planar` |
| FutureProg | `MudSharpCore/FutureProg/Functions/Planes` |

## Current Limits
The first implementation supplies the shared data model, persistence, builder/admin/player surfaces, default runtime integration, magic/FutureProg hooks, a permanent merit path, and a drug-driven temporary path. Transition profiles are stored but do not yet apply custom stamina, pain, or damage costs.
