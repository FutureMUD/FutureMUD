# Scroll compatibility and stored numerical context

The authoritative inventory is `ScrollSpellCompatibility.Inventory`. The regression `CompatibilityManifestCoversEveryRegisteredTypeAndRequiredFamilies` compares this inventory with both live load and builder registrations. New effect types default to unsupported until explicitly audited. This is a compatibility decision for prepaid scrolls; it does not disable the effect for ordinary spells.

Inscription checks both target and caster templates before reserving or paying. Activation reconstructs the frozen configuration, validates its schema, checksum, expression bindings and retained references, then checks the source spell's current scroll opt-in and school. Revocation prevents unused scroll activation. Existing persistent effects may reload their retained snapshot after source deletion; they still require referenced definitions.

## Required adapters

| Family | Captured fields | Live context |
| --- | --- | --- |
| Damage and self damage | `DamageExpression`, creator trait bindings | Current target, body applicability, opposed outcome, damage origin is reader |
| Heal and mend | `HealingAmount`, creator trait bindings | Current wounds and opposed outcome |
| Stamina and magic resource delta | `Formula`, creator trait bindings | Target's current stamina/resource; referenced resource must exist |
| Magical armour | `MaximumDamageAbsorbed`, material/type/shapes, scalar configuration | Reader-owned effect and later attacks; numerical context survives parent/child reload |
| Boost, status and perception | Scalar template XML and duration expression | Current target, applicability predicates and reader ownership |
| Teleport, target teleport, forced exit movement | Scalar movement configuration | Reader's party and actual trigger room/character/exit; no creator lookup |

Each duration binds `variable` to the spell casting trait in `SpellDuration` context. The seven numerical effect adapters bind in their actual effect contexts. `spelllevel`, `castinglevel`, `casterlevel`, `power`, synthetic `degrees`/`success`, trait parameters, `variable`, and extended trait options are retained. Per-target opposed `outcome` is supplied during resolution. Capture never evaluates damage/healing expressions, invents a target or pre-rolls random formula results.

These are per-invocation expression wrappers. Neither scroll release nor an overlapping ordinary cast changes the catalogue spell's shared formulas. Resource and material costs are prepaid and omitted from stored release. See [runtime and commitment semantics](Vancian_Magic_Runtime.md) and [verification](Vancian_Magic_Verification.md).

## Complete registered inventory

| Type | Status | Reason / semantics |
| --- | --- | --- |
| `animatecorpse` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `astralprojection` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `blindness` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `bodybackup` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `boost` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `burning` | Unsupported | Periodic damage/tracking expressions require a dedicated retained-context adapter. |
| `changecharacteristic` | Unsupported | Characteristic profile/value selection and its live referenced definitions require a dedicated compatibility adapter. |
| `comprehendlanguage` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `corpseconsume` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `corpsemark` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `corpsepreserve` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `corpsespawn` | Unsupported | Creation/template callbacks and referenced prototype lifecycle require a dedicated adapter. |
| `createclone` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `createcopy` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `createitem` | Unsupported | Creation/template callbacks and referenced prototype lifecycle require a dedicated adapter. |
| `createliquid` | Unsupported | Creation/template callbacks and referenced prototype lifecycle require a dedicated adapter. |
| `createnpc` | Unsupported | Creation/template callbacks and referenced prototype lifecycle require a dedicated adapter. |
| `createtrap` | Unsupported | Nested prepared-payload and trap ownership require a dedicated adapter. |
| `cureblindness` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `curse` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `damage` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `deadspeak` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `deafness` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `destroyitem` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `detectethereal` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `detectinvisible` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `detectmagick` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `detectpoison` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `disease` | Unsupported | Pathogen/drug payloads contain additional numerical and live-reference fields without a snapshot adapter. |
| `dispelinvisibility` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `dispelmagic` | Unsupported | Active interception/dispelling policies require a dedicated compatibility adapter. |
| `dispeltrap` | Unsupported | Nested prepared-payload and trap ownership require a dedicated adapter. |
| `executeprog` | Unsupported | Arbitrary script/command effects have no typed numerical snapshot adapter. |
| `exitbarrier` | Unsupported | Retained topology/attachment lifecycle requires a dedicated snapshot adapter. |
| `fear` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `featherfall` | Unsupported | Sustained movement, path or inventory-control ownership requires a dedicated adapter. |
| `flying` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `forcecommand` | Unsupported | Arbitrary script/command effects have no typed numerical snapshot adapter. |
| `forcedexitmovement` | Actor-context-only supported | Reader supplies movement, party and exit context. |
| `forcedpathmovement` | Unsupported | Sustained movement, path or inventory-control ownership requires a dedicated adapter. |
| `glow` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `handsofwind` | Unsupported | Sustained movement, path or inventory-control ownership requires a dedicated adapter. |
| `heal` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `healingrate` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `identify` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `ignite` | Unsupported | Periodic damage/tracking expressions require a dedicated retained-context adapter. |
| `infravision` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `insomnia` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `invisibility` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `itemdamage` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `itemenchant` | Unsupported | Item/corpse mutation and nested item configuration require a dedicated compatibility adapter. |
| `levitate` | Unsupported | Sustained movement, path or inventory-control ownership requires a dedicated adapter. |
| `magicaltether` | Unsupported | Retained topology/attachment lifecycle requires a dedicated snapshot adapter. |
| `magicresourcedelta` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `magictag` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `mend` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `needdelta` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `needrate` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `pacifism` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `paralysis` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `personaltagward` | Unsupported | Active interception/dispelling policies require a dedicated compatibility adapter. |
| `personalward` | Unsupported | Active interception/dispelling policies require a dedicated compatibility adapter. |
| `phantomillusion` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `placetrap` | Unsupported | Nested prepared-payload and trap ownership require a dedicated adapter. |
| `planarstate` | Unsupported | Planar transition and linked instance lifecycle require a dedicated adapter. |
| `planeshift` | Unsupported | Planar transition and linked instance lifecycle require a dedicated adapter. |
| `poison` | Unsupported | Pathogen/drug payloads contain additional numerical and live-reference fields without a snapshot adapter. |
| `portal` | Unsupported | Retained topology/attachment lifecycle requires a dedicated snapshot adapter. |
| `portalnetwork` | Unsupported | Retained topology/attachment lifecycle requires a dedicated snapshot adapter. |
| `possessbody` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `possesscorpse` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `rage` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `reciteproxy` | Unsupported | Arbitrary script/command effects have no typed numerical snapshot adapter. |
| `relocate` | Unsupported | This legacy implementation is a healing variant with specialised wound logic; use the audited heal or mend adapter. |
| `removeblindness` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removecomprehendlanguage` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removecurse` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removedetectethereal` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removedetectinvisible` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removedetectmagick` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removedisease` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removefear` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeflying` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeinfravision` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeinsomnia` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeinvisibility` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removemagictag` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeparalysis` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeplanarstate` | Unsupported | Planar transition and linked instance lifecycle require a dedicated adapter. |
| `removepoison` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removeroomflag` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `removesilence` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removesleep` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `removetrap` | Unsupported | Nested prepared-payload and trap ownership require a dedicated adapter. |
| `removewaterbreathing` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `resurrect` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `roomatmosphere` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `roomflag` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `roomgravity` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `roomlight` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `roomtagward` | Unsupported | Active interception/dispelling policies require a dedicated compatibility adapter. |
| `roomtemperature` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `roomward` | Unsupported | Active interception/dispelling policies require a dedicated compatibility adapter. |
| `seizebody` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `selfdamage` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `silence` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `sleep` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `spellarmour` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `staminadelta` | Supported | Every numerical field has a typed captured binding; opposed outcome and random rolls remain live. |
| `staminaexpendrate` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `staminaregenrate` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `subjectivedesc` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `subjectivesdesc` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `telepathy` | Unsupported | Information or subjective-perception policy scripts require an explicit live-context adapter. |
| `teleport` | Actor-context-only supported | Reader supplies movement, party and exit context. |
| `teleporttarget` | Actor-context-only supported | Reader supplies movement, party and exit context. |
| `trackmark` | Unsupported | Periodic damage/tracking expressions require a dedicated retained-context adapter. |
| `tracktrail` | Unsupported | Periodic damage/tracking expressions require a dedicated retained-context adapter. |
| `transference` | Unsupported | Sustained movement, path or inventory-control ownership requires a dedicated adapter. |
| `transformform` | Unsupported | Identity/body lifecycle and control-transfer configuration requires a dedicated snapshot adapter. |
| `waterbreathing` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
| `weatherchange` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `weatherchangefreeze` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `weatherfreeze` | Unsupported | Environmental controller and referenced-world configuration require a dedicated adapter. |
| `weight` | Supported | Scalar configuration and duration are frozen; live target applicability and reader attribution remain. |
