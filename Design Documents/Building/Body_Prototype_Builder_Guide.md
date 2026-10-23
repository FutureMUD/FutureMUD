# Body Prototype Builder Guide

See the [common builder workflow](Generic_Editable_Item_Builder_Workflows.md) for selection, closing, help discovery and persistence conventions.

The `bodyproto` command creates, inspects, and edits body prototypes in game. Body prototypes define the anatomy available to races, including bodypart roles, limbs, positions, movement speeds, and wearable sizing.

## Create or derive a prototype

Use `bodyproto create <name> size <wear-rule-id>` to create a standalone prototype. The wearable-size rule is required and can be selected from the existing rule catalogue by ID. A new standalone prototype begins with no bodyparts or limbs; add these before assigning it to a race.

Use `bodyproto create <name> size <wear-rule-id> from <parent>` to create a prototype that inherits anatomy from another prototype. `bodyproto clone <source> as <new-name>` is shorthand for a derived prototype using the source's wearable-size rule. Inherited parts retain their original IDs and remain owned by their original prototype. `bodyproto set parent none` removes inheritance when no instantiated body in the affected descendant tree would be changed.

## Build a standalone anatomy

Use `bodyproto part add <template-part> <new-name>` while editing the destination prototype to create an owned part using an existing part as a template. This creates a new bodypart row and ID with the selected type and scalar settings. It does not reuse or reassign the template's ID. The copy starts without count-as, upstream, internal-organ, bone-coverage, or limb references. Rebuild those relationships against parts owned by the new prototype using `bodypart edit <body> <part>` and the part's normal building commands.

Every bodypart belongs to one prototype. The `bodyproto set part role <part> core|male|female|extra` command changes role membership only for parts owned by the body being edited. `bodyproto set part remove <part>` can exclude an inherited part from this prototype; `restore` removes that exclusion. To change an inherited part's definition, edit it on its owner. The command refuses structural changes when a race or instantiated body references the edited prototype or one of its descendants. The existing `bodypart` builder also refuses cloning or structural part edits in an affected tree; names, descriptions, and scalar tuning remain editable.

Create limbs after their external parts with:

```text
bodyproto set limb add <name> type <limb-type> root <part> parts <id,id,...> [spine <id,id,...>] [damage <multiplier>] [pain <multiplier>]
```

The root and every listed limb member must be external parts in the edited prototype's anatomy; inherited parts retain their source IDs. The root is included automatically. Spinal references are separate internal spine parts in that anatomy. Limb types are `Arm`, `Leg`, `Appendage`, `Head`, `Wing`, `Torso`, and `Genitals`. Part IDs are shown by `bodyproto show` and `bodypart show`. In-progress anatomy can be inspected before its limbs are complete.

## Positions, movement, and sizing

`bodyproto set position add|remove <position>` controls valid position states; at least one position must remain. Add movement with:

```text
bodyproto set speed add <position> <alias> <rate> <stamina-rate> <1st-person-verb> <3rd-person-verb> <participle>
bodyproto set speed edit <speed-id> <alias> <rate> <stamina-rate> <1st-person-verb> <3rd-person-verb> <participle>
```

Add at least one speed for each position the body should use for movement. Speeds load in ascending database ID order, so the first speed for a position is the deterministic default; a separate default selection is not stored. Editing preserves the speed ID and inherited speeds must be edited on their owner. `bodyproto set wearSize <rule-id>` changes the selected wearable-size rule. The rule must already exist. `bodyproto set smashing <part>` selects the default external bodypart for unarmed door smashing.

## Editing and validation

`bodyproto list`, `show`, `edit`, and `close` follow the usual builder pattern. While editing, `bodyproto edit` shows the current prototype and `bodyproto set` changes its parent, tracking/consider text, singular/plural wielder and leg labels, standing-leg and flying-wing thresholds, default smashing part, wearable-size rule, part roles/exclusions, positions, speeds, and limbs. `bodyproto show` reuses the prototype's detailed runtime display.

Parent changes reject self-inheritance and cycles. Part-role changes reject parts from another prototype. Limb creation checks membership and type of roots, members, and spinal parts. Structural changes are blocked if any persisted body or race references the edited prototype or a descendant, because inherited anatomy changes could otherwise alter existing bodies or race chargen. Clone through inheritance or create a standalone prototype and copy parts into it to make a safe working variant.
