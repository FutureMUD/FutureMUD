# Attribute Describer Ranges

## Purpose and scope

The Attribute seeder's RPI decorator uses a universal extended scale for every attribute package. The LabMUD decorator uses six specialised extended scales when paired with the LabMUD attribute package. Other packages paired with the LabMUD decorator retain their existing descriptions pending vocabulary design. Modern and Raw decorators are unchanged.

These are presentation changes only. They do not alter attribute values, racial bonuses, caps, checks, combat formulas or abilities. Terms such as Deathless, All-Seeing and Indestructible are figurative and do not grant their literal effects.

The current mythical balance profiles include effective Strength targets of 95 for griffins and wyverns, 180 for colossal worms, 250 for eastern dragons and 285 for dragons. Dragon Constitution targets 105 and Aura 90. Targets are not engine ceilings. The extended scale therefore provides several distinctions within the hundreds and reserves Godlike for much higher values.

## Preserved descriptions

Integer scores through 25 retain their historical descriptions. This includes the requested preservation through 18 and the familiar Super, Epic and Legendary transition.

| Integer value | Description |
| --- | --- |
| 0 and below | Abysmal |
| 1–3 | Terrible |
| 4–6 | Bad |
| 7–9 | Poor |
| 10–11 | Average |
| 12–13 | Good |
| 14–15 | Great |
| 16–17 | Excellent |
| 18–20 | Super |
| 21–23 | Epic |
| 24–25 | Legendary |

## Extended descriptions

All columns share the same numerical thresholds. Tables show integer scores.

| Value | RPI | LabMUD Strength | LabMUD Dexterity | LabMUD Constitution |
| --- | --- | --- | --- | --- |
| 26–30 | Prodigious | Prodigious | Deft | Robust |
| 31–40 | Phenomenal | Herculean | Uncanny | Stalwart |
| 41–55 | Formidable | Trans-Herculean | Preternatural | Ironclad |
| 56–75 | Tremendous | Towering | Sublime | Adamantine |
| 76–100 | Immense | Immense | Otherworldly | Inexhaustible |
| 101–140 | Monumental | Monumental | Transcendent | Deathless |
| 141–190 | Colossal | Colossal | Celestial | Eternal |
| 191–250 | Titanic | Titanic | Empyrean | Primordial |
| 251–350 | Mythic | Worldshaking | Mythic | Mythic |
| 351–500 | Demigodlike | Demigodlike | Demigodlike | Demigodlike |
| 501–750 | Godlike | Godlike | Godlike | Godlike |
| 751–1,000 | Transcendent | Worldbreaking | Beyond Divine | Beyond Divine |
| Above 1,000 | Ineffable | Immeasurable | Ineffable | Indestructible |

| Value | LabMUD Intelligence | LabMUD Willpower | LabMUD Perception |
| --- | --- | --- | --- |
| 26–30 | Brilliant | Resolute | Acute |
| 31–40 | Genius | Dauntless | Piercing |
| 41–55 | Sage | Indomitable | Uncanny |
| 56–75 | Profound | Adamantine | Preternatural |
| 76–100 | Enlightened | Inexorable | Oracular |
| 101–140 | Transcendent | Unconquerable | Revelatory |
| 141–190 | Transhuman | Eternal | All-Seeing |
| 191–250 | Cosmic | Primordial | Cosmic |
| 251–350 | Mythic | Mythic | Mythic |
| 351–500 | Demigodlike | Demigodlike | Demigodlike |
| 501–750 | Godlike | Godlike | Godlike |
| 751–1,000 | Beyond Divine | Beyond Divine | Beyond Divine |
| Above 1,000 | Ineffable | Absolute | Omniscient |

## Runtime boundaries and persistence

`AttributeDescriberDefinitions` generates the stock XML stored in `TraitDecorators.Contents`. Decorator names, trait bindings, empty prefixes/suffixes and effective colour settings are preserved. No schema migration is required.

The runtime `RangeDecorator` consumes these definitions through `RankedRange`. At a shared boundary the earlier interval wins: 25 is Legendary, any value greater than 25 through 30 uses the next description, and 30.001 moves into the following tier. The same historical fractional behaviour is retained below 25. The last XML interval ends at `int.MaxValue` because decorator bounds are parsed as integers; the existing open-upper-bound lookup extends its description to larger finite values.

Fresh installations receive the new definitions. The Attribute seeder's reconciliation path intentionally preserves installed decorators, including builder customisation. Rerunning it does not upgrade old describers. An existing-world update requires a separate explicit change to the selected installed definitions; do not overwrite decorators by name alone or assume rerunning this package applies the new scale.

## Verification

`AttributeDescriberSeederTests` seeds in-memory worlds and loads their persisted XML through the actual runtime decorator. Coverage includes historical values and fractions through 25, both sides of every new boundary, all upper vocabulary, final-tier overflow, effective colour flags, trait bindings across every RPI package, representative mythical Strength targets, and preservation of custom definitions on rerun.
