# Disfigurement Seeder Builder Reference

## Purpose
This document is the builder-facing companion to `Disfigurement_System_Design.md`.

Use it when authoring stock scar and tattoo template definitions for the seeder. It focuses on the concrete values that the current seeder can resolve today:

- seeded colour names for tattoo inks
- human bodypart shape names
- human bodypart aliases and their display names
- the default values and accepted runtime ranges for scar and tattoo template fields
- wording guidance for `OverrideCharacteristicPlain` and `OverrideCharacteristicWith`

This document intentionally does not try to cover builder-owned world decisions such as:

- chargen gating FutureProgs
- setting-specific knowledges
- world-specific chargen resource policy

## Current Seeded Status
At the moment, the stock disfigurement seeder scaffolding ships with no built-in human, animal, or mythical scar/tattoo templates. The current human lists are empty in `DatabaseSeeder/Seeders/HumanSeeder.Disfigurements.cs`.

That means this handoff is about the values the seeder helper can resolve and the defaults it applies when a builder starts adding new definitions.

## Resolver Rules
The seeder helper resolves names case-insensitively.

- `BodypartShapeNames` must match existing seeded `BodypartShape.Name` values.
- `BodypartAliases` must match the human bodypart alias on the seeded body or one of its ancestors.
- `InkColours` keys must match seeded `Colour.Name` values.
- `ChargenCosts` keys resolve against chargen resource `Name` or `Alias`.
- `CanSelectInChargenProgName` resolves against `FutureProg.FunctionName`.
- `RequiredKnowledgeName` resolves against `Knowledge.Name`.

For human templates, prefer `BodypartAliases` when the content is meant for one exact location and prefer `BodypartShapeNames` when the content should generalise across equivalent anatomy.

## Human Bodypart Shapes
These are the canonical human-compatible shape names currently seeded by `HumanSeederBodyparts.cs` and suitable for `BodypartShapeNames`:

`abdomen`, `ankle`, `belly`, `breast`, `buttock`, `calf`, `cheek`, `chin`, `ear`, `elbow`, `eye`, `eye socket`, `eyebrow`, `face`, `finger`, `foot`, `forearm`, `forehead`, `groin`, `hand`, `head back`, `heel`, `hip`, `inventory`, `knee`, `knee back`, `lower back`, `mouth`, `neck`, `neck back`, `nipple`, `nose`, `penis`, `scalp`, `shin`, `shoulder`, `shoulder blade`, `temple`, `testicles`, `thigh`, `thigh back`, `throat`, `thumb`, `toe`, `tongue`, `upper arm`, `upper back`, `wrist`

Notes:

- `inventory` is conditional and only exists if the human seeder was configured to create an inventory bodypart instead of using hands. It is not a sensible target for scars or tattoos.
- Internal-only human shapes like `Organ` and `Bone` exist in the seed data but are not suitable disfigurement targets because scars and tattoos operate on external bodyparts.

## Human Bodypart Aliases
These are the current seeded human aliases, with the player-facing bodypart description and the resolved shape. They are the safest values to hand to builders who want exact placement.

| Alias | Display Name | Shape |
| --- | --- | --- |
| `abdomen` | abdomen | abdomen |
| `rbreast` | right breast | breast |
| `lbreast` | left breast | breast |
| `rnipple` | right nipple | nipple |
| `lnipple` | left nipple | nipple |
| `uback` | upper back | upper back |
| `belly` | belly | belly |
| `lback` | lower back | lower back |
| `rbuttock` | right buttock | buttock |
| `lbuttock` | left buttock | buttock |
| `rshoulder` | right shoulder | shoulder |
| `lshoulder` | left shoulder | shoulder |
| `rshoulderblade` | right shoulder blade | shoulder blade |
| `lshoulderblade` | left shoulder blade | shoulder blade |
| `inventory` | inventory | inventory |
| `neck` | neck | neck |
| `bneck` | back of neck | neck back |
| `throat` | throat | throat |
| `face` | face | face |
| `chin` | chin | chin |
| `rcheek` | right cheek | cheek |
| `lcheek` | left cheek | cheek |
| `mouth` | mouth | mouth |
| `tongue` | tongue | tongue |
| `nose` | nose | nose |
| `forehead` | forehead | forehead |
| `reyesocket` | right eye socket | eye socket |
| `leyesocket` | left eye socket | eye socket |
| `reye` | right eye | eye |
| `leye` | left eye | eye |
| `rear` | right ear | ear |
| `lear` | left ear | ear |
| `bhead` | back of head | head back |
| `scalp` | scalp | scalp |
| `rbrow` | right brow | eyebrow |
| `lbrow` | left brow | eyebrow |
| `rtemple` | right temple | temple |
| `ltemple` | left temple | temple |
| `rupperarm` | right upper arm | upper arm |
| `lupperarm` | left upper arm | upper arm |
| `relbow` | right elbow | elbow |
| `lelbow` | left elbow | elbow |
| `rforearm` | right forearm | forearm |
| `lforearm` | left forearm | forearm |
| `rwrist` | right wrist | wrist |
| `lwrist` | left wrist | wrist |
| `rhand` | right hand | hand |
| `lhand` | left hand | hand |
| `rthumb` | right thumb | thumb |
| `lthumb` | left thumb | thumb |
| `rindexfinger` | right index finger | finger |
| `lindexfinger` | left index finger | finger |
| `rmiddlefinger` | right middle finger | finger |
| `lmiddlefinger` | left middle finger | finger |
| `rringfinger` | right ring finger | finger |
| `lringfinger` | left ring finger | finger |
| `rpinkyfinger` | right pinky finger | finger |
| `lpinkyfinger` | left pinky finger | finger |
| `rhip` | right hip | hip |
| `lhip` | left hip | hip |
| `rthigh` | right thigh | thigh |
| `lthigh` | left thigh | thigh |
| `rthighback` | right thigh back | thigh back |
| `lthighback` | left thigh back | thigh back |
| `rknee` | right knee | knee |
| `lknee` | left knee | knee |
| `rkneeback` | right knee back | knee back |
| `lkneeback` | left knee back | knee back |
| `rshin` | right shin | shin |
| `lshin` | left shin | shin |
| `rcalf` | right calf | calf |
| `lcalf` | left calf | calf |
| `rankle` | right ankle | ankle |
| `lankle` | left ankle | ankle |
| `rheel` | right heel | heel |
| `lheel` | left heel | heel |
| `rfoot` | right foot | foot |
| `lfoot` | left foot | foot |
| `rbigtoe` | right big toe | toe |
| `lbigtoe` | left big toe | toe |
| `rindextoe` | right index toe | toe |
| `lindextoe` | left index toe | toe |
| `rmiddletoe` | right middle toe | toe |
| `lmiddletoe` | left middle toe | toe |
| `rringtoe` | right ring toe | toe |
| `lringtoe` | left ring toe | toe |
| `rpinkytoe` | right pinky toe | toe |
| `lpinkytoe` | left pinky toe | toe |
| `groin` | groin | groin |
| `testicles` | testicles | testicles |
| `penis` | penis | penis |

## Seeded Colour Names
Any of the following seeded colour names can be used as keys in a tattoo template's `InkColours` dictionary:

`amber`, `amethyst`, `aquamarine`, `ash blonde`, `ash grey`, `ashen off-white`, `auburn`, `azure`, `azure blue`, `banana yellow`, `beet red`, `beige`, `beryl`, `black`, `bland brown`, `bland wheat-coloured`, `bland yellow`, `blonde`, `blood red`, `blotched red`, `blotched white`, `blotchy green`, `blotchy rust-red`, `blue`, `bone white`, `brick brown`, `brick red`, `bright blue`, `bright green`, `bright white`, `brown`, `burnt sienna`, `caramel`, `carrot orange`, `cerulean`, `chalky pale grey`, `charcoal grey`, `chartreuse`, `chartreuse green`, `chocolate`, `chocolate brown`, `cinnamon`, `cobalt`, `cobalt blue`, `cobalt green`, `copper`, `coral`, `coral orange`, `cornflour blue`, `cornflower blue`, `cornsilk yellow`, `crimson`, `cyan`, `cyan blue`, `dark`, `dark blue`, `dark brown`, `dark gray`, `dark green`, `dark grey`, `dark khaki`, `dark orange`, `dark red`, `deep blue`, `deep brown`, `deep indigo`, `deep pink`, `dim blue-black`, `dim grey`, `dim olive`, `dingy green`, `dingy grey`, `dingy off-white`, `dingy purple`, `dingy red`, `dingy yellow`, `dirty beige`, `dirty blonde`, `dirty bone-white`, `dirty purple`, `discoloured green`, `discoloured orange`, `discoloured purple`, `dismal sand brown`, `drab brown`, `drab olive`, `drab peach-coloured`, `dreary beige`, `dreary blue-black`, `dreary brown`, `dull green`, `dull mist grey`, `dull orange`, `dull red`, `dusky slate grey`, `dusty faded purple`, `earthen brown`, `ebony`, `eggshell white`, `emerald green`, `faded black`, `faded blue`, `faded blue-black`, `faded green`, `faded indigo`, `faded purple`, `faded red`, `faded reddish-orange`, `faded salmon`, `faded slate blue`, `fiery orange`, `fire brick brown`, `flame red`, `forest green`, `fuchsia pink`, `gaudy mustard yellow`, `ghost white`, `gleaming white`, `gold`, `gold-coloured`, `golden yellow`, `goldenrod`, `goldenrod yellow`, `gray`, `gray black`, `green`, `grey`, `grimy beige`, `grimy black`, `grimy blue`, `grimy lavender`, `grimy rust-red`, `grimy salmon`, `grisly brownish-green`, `hazel`, `hot pink`, `hunter green`, `indian red`, `ink black`, `ivory`, `ivory white`, `jet black`, `khaki`, `lavender`, `lavender pink`, `light blonde`, `light blue`, `light brown`, `light green`, `light grey`, `light pink`, `light red`, `light salmon pink`, `light steel blue`, `light yellow`, `lime green`, `lurid pale yellow`, `lurid peach-coloured`, `magenta red`, `mahogany`, `maroon red`, `mauve`, `midnight black`, `midnight blue`, `mint green`, `mist grey`, `moccasin brown`, `mulbery`, `murky brown`, `murky olive`, `natural`, `navy blue`, `nut brown`, `obsidian`, `ocean blue`, `ochre`, `off-white`, `olive`, `olive green`, `onyx`, `orange`, `orange brown`, `orange red`, `orchid pink`, `pale blue`, `pale green`, `pale violet`, `pale white`, `pale yellow`, `pallid blue`, `peachpuff pink`, `pearl white`, `pine green`, `pink`, `pitch black`, `platinum blonde`, `plum`, `plum purple`, `powder blue`, `pure white`, `purple`, `red`, `reddish brown`, `rich brown`, `rich green`, `rich indigo`, `rose red`, `royal blue`, `royal purple`, `ruby red`, `saddle brown`, `sage green`, `salmon pink`, `salt-and-pepper`, `sand yellow`, `sandy brown`, `sapphire blue`, `scarlet`, `sea green`, `seashell`, `seashell gray`, `sepia brown`, `shabby beige`, `shabby black`, `shabby green`, `shabby pale yellow`, `shabby sallow-coloured`, `sickly greyish-green`, `sickly pale yellow`, `sickly peach-coloured`, `sienna brown`, `silver blonde`, `silver grey`, `sky blue`, `slate blue`, `slate gray`, `slate grey`, `smoky grey`, `smoky white`, `snow white`, `soft grey`, `sooty grey`, `spotted lavender`, `spotted muddy brown`, `spotted white`, `spring green`, `stained blue`, `stained brown`, `stained ivory`, `stained orange-red`, `stained purple`, `stained red`, `stained salmon`, `stained white`, `steel blue`, `storm blue`, `strawberry blonde`, `sunset orange`, `tan brown`, `tan yellow`, `tattered beige`, `tattered black`, `tattered reddish-orange`, `tattered violet`, `teal`, `teal blue`, `thistle grey`, `topaz hued`, `turquoise blue`, `umber`, `verdant green`, `violet`, `violet red`, `vivid indigo`, `wan ivory`, `well-worn blue`, `wheat yellow`, `white`, `wine red`, `winter blue`, `yellow`

Practical tattoo guidance:

- The resolver allows any seeded colour name, but good tattoo palettes are usually narrow.
- Most tattoo templates should use 1 to 3 colours, with weights that reflect relative ink use.
- If all colours should be equally likely or equally consumed, set all weights to `1.0`.

## Tattoo Field Defaults And Accepted Values
These are the current defaults used by `SeederTattooTemplateDefinition` and the runtime constraints enforced by the tattoo builder/runtime.

| Field | Seeder Default | Accepted / Implemented Range | Notes |
| --- | --- | --- | --- |
| `Name` | required | any unique string per tattoo template | Seeder idempotency keys on `(Type, Name)` |
| `ShortDescription` | required | free text | Should read well in body descriptions |
| `FullDescription` | required | free text | Builder submission rejects the stock placeholder |
| `MinimumBodypartSize` | `Nanoscopic` | any `SizeCategory` | Practical human tattoo values usually start at `Tiny` or larger |
| `RequiredKnowledgeName` | `null` | any seeded knowledge name | Builder-owned |
| `MinimumSkill` | `0.0` | any non-negative number | Uses tattooist trait scale |
| `InkColours` | `null` | any seeded colour names with positive weights | At least one ink is required for a valid tattoo template |
| `BodypartShapeNames` | `null` | any seeded bodypart shape names | Optional; empty means any shape |
| `BodypartAliases` | `null` | any human alias listed above | Optional; resolved to shapes at seeding time |
| `CanSelectInChargen` | `false` | `true` / `false` | If `true`, can also carry costs and a prog |
| `CanSelectInChargenProgName` | `null` | any boolean Chargen prog name | Builder-owned |
| `ChargenCosts` | `null` | any seeded chargen resource names or aliases | Builder-owned |
| `OverrideCharacteristicPlain` | `null` | any lower-case descriptor fragment | Should not include `with` |
| `OverrideCharacteristicWith` | `null` | any lower-case descriptor fragment | Should normally start with `with ` |

### Valid `SizeCategory` Values
These are the current seeded engine size labels:

`Nanoscopic`, `Microscopic`, `Miniscule`, `Tiny`, `VerySmall`, `Small`, `Normal`, `Large`, `VeryLarge`, `Huge`, `Enormous`, `Gigantic`, `Titanic`

## Scar Field Defaults And Accepted Values
These are the current defaults used by `SeederScarTemplateDefinition` and the implemented runtime constraints.

| Field | Seeder Default | Accepted / Implemented Range | Notes |
| --- | --- | --- | --- |
| `Name` | required | any unique string per scar template | Seeder idempotency keys on `(Type, Name)` |
| `ShortDescription` | required | free text | Should read as a concise visible mark |
| `FullDescription` | required | free text | Builder submission rejects the stock placeholder |
| `SizeSteps` | `0` | integer, practically `0` or negative | Builder command rejects positive values |
| `Distinctiveness` | `1` | integer, no hard upper bound currently enforced | Keep values modest and internally consistent |
| `Unique` | `false` | `true` / `false` | `true` prevents multiple copies of the same scar template on one body |
| `DamageHealingScarChance` | `0.0` | `0.0` to `1.0` | Builder command accepts `0-1` or `0-100` input and normalises to `0-1` |
| `SurgeryHealingScarChance` | `0.0` | `0.0` to `1.0` | Same behaviour as damage healing chance |
| `DamageTypes` | `null` | map of `DamageType -> minimum WoundSeverity` | Optional; empty means no damage-origin eligibility |
| `SurgeryTypes` | `null` | list of `SurgicalProcedureType` | Optional; empty means no surgery-origin eligibility |
| `BodypartShapeNames` | `null` | any seeded bodypart shape names | Optional; empty means any shape |
| `BodypartAliases` | `null` | any human alias listed above | Optional; resolved to shapes at seeding time |
| `CanSelectInChargen` | `false` | `true` / `false` | If `true`, can also carry costs and a prog |
| `CanSelectInChargenProgName` | `null` | any boolean Chargen prog name | Builder-owned |
| `ChargenCosts` | `null` | any seeded chargen resource names or aliases | Builder-owned |
| `OverrideCharacteristicPlain` | `null` | any lower-case descriptor fragment | Should not include `with` |
| `OverrideCharacteristicWith` | `null` | any lower-case descriptor fragment | Should normally start with `with ` |

### Practical Scar Value Bands
There are no stock seeded scar templates yet, so there is no existing scar catalogue to derive numeric bands from. The safest interpretation is:

- helper defaults are the starting point
- runtime-enforced bounds are the hard limits
- builder style should stay conservative until live playtesting suggests otherwise

Recommended starting bands for first-pass seeder content:

- `SizeSteps`: `0` to `-3`
- `Distinctiveness`: `1` to `4`
- `DamageHealingScarChance`: `0.05` to `0.30`
- `SurgeryHealingScarChance`: `0.05` to `0.40`

Use larger values only when the template is very specific and intentionally rare or dramatic.

### Valid `DamageType` Values
These are the current code-level damage types accepted by scar templates:

`Slashing`, `Chopping`, `Crushing`, `Piercing`, `Ballistic`, `Burning`, `Freezing`, `Chemical`, `Shockwave`, `Bite`, `Claw`, `Electrical`, `Hypoxia`, `Cellular`, `Sonic`, `Shearing`, `BallisticArmourPiercing`, `Wrenching`, `Shrapnel`, `Necrotic`, `Falling`, `Eldritch`, `Arcane`, `ArmourPiercing`

### Valid `WoundSeverity` Values
These are the current minimum severities accepted in a scar template damage gate:

`None`, `Superficial`, `Minor`, `Small`, `Moderate`, `Severe`, `VerySevere`, `Grievous`, `Horrifying`

In practice, seeded scars should usually start at `Minor` or worse. `None` is technically valid but rarely meaningful.

### Valid `SurgicalProcedureType` Values
These are the current surgery categories accepted by scar templates:

`Triage`, `DetailedExamination`, `InvasiveProcedureFinalisation`, `ExploratorySurgery`, `Amputation`, `Replantation`, `Cannulation`, `TraumaControl`, `OrganExtraction`, `OrganTransplant`, `Decannulation`, `OrganStabilisation`, `SurgicalBoneSetting`, `InstallImplant`, `RemoveImplant`, `ConfigureImplantPower`, `ConfigureImplantInterface`

## Override Characteristic Wording
The override strings feed straight into the `&tattoos`, `&withtattoos`, `&scars`, and `&withscars` characteristic tokens.

That means:

- `OverrideCharacteristicPlain` should read like an adjective or compact descriptor fragment
- `OverrideCharacteristicWith` should read like a phrase that can follow a noun phrase
- neither field should be capitalised unless the text itself demands it
- neither field should end in punctuation

Good mental model:

- plain form answers "what kind of person do I look like?"
- with form answers "what visible feature do I have?"

### Good Scar Examples
| Intended Effect | `OverrideCharacteristicPlain` | `OverrideCharacteristicWith` |
| --- | --- | --- |
| across-eye scar | `facially-scarred` | `with a long scar across the left eye` |
| burned face | `badly-burned` | `with severe burn scarring over the face` |
| one dramatic cheek scar | `scarred` | `with a jagged scar across the right cheek` |
| ritual scarification | `ritually-scarified` | `with ritual scarification across the shoulders` |

### Good Tattoo Examples
| Intended Effect | `OverrideCharacteristicPlain` | `OverrideCharacteristicWith` |
| --- | --- | --- |
| facial tattoo | `facially-tattooed` | `with angular tattoos across the face` |
| ritual ink | `ritually-tattooed` | `with ritual tattoos on the forearms` |
| gang markings | `gang-marked` | `with gang tattoos on the neck and hands` |

### Avoid
- `with a scarred face`
- `has a scar on his cheek`
- `Scarred`
- `With facial tattoos.`

Why these are poor fits:

- the plain form should not start with `with`
- the engine is not expecting a full sentence
- punctuation tends to read awkwardly inside sdesc patterns
- title case looks wrong in the middle of generated text

## Seeder Authoring Recommendations
For first-pass human scar seeding:

- use aliases for exact landmark scars such as `reye`, `lcheek`, `throat`, `forehead`, `rhand`, `lhand`
- use shapes for broader categories such as `cheek`, `forearm`, `thigh`, `shoulder`
- keep broad all-body scars rare and low-weighted
- avoid mixing many overlapping generic facial scars unless you intentionally want the overlap to raise overall scar frequency

For first-pass human tattoo seeding:

- use `MinimumBodypartSize` to keep large motifs off tiny anatomy
- keep colour palettes tight and intentional
- reserve unrestricted shape lists for very small, generic tattoos
- prefer aliases for exact placements such as `lback` for tramp stamps or `rshoulder` / `lshoulder` for insignia

## Suggested Builder Workflow
1. Pick the narrative mark.
2. Choose whether the definition should target exact human aliases or broader human shapes.
3. Set the visible descriptions first.
4. Add the scar/tattoo mechanics second.
5. Add override text only if the mark should meaningfully affect sdesc grammar.
6. Leave chargen prog and knowledge hookup to the builder handling those world decisions.

## Source Of Truth
This document is derived from the currently seeded and coded values in:

- `DatabaseSeeder/Seeders/HumanSeeder.Disfigurements.cs`
- `DatabaseSeeder/Seeders/SeederDisfigurementTemplateUtilities.cs`
- `DatabaseSeeder/Seeders/HumanSeederBodyparts.cs`
- `DatabaseSeeder/Seeders/CoreDataSeeder.cs`
- `MudSharpCore/Body/Disfigurements/ScarTemplate.cs`
- `MudSharpCore/Body/Disfigurements/TattooTemplate.cs`
- `FutureMUDLibrary/Form/Characteristics/IHaveCharacteristics.cs`
