# Magical Substances

## Purpose and ownership

Magical substances provide builder-authored potions, oils, salves, magical food and inhaled preparations. They reuse physical delivery and dose measurement, while owning a separate magical lifecycle. A carrier can deliver both a conventional drug and a magical substance. Substances do not require drug definitions.

The runtime contract is `IMagicalSubstance`. `MagicalSubstance` owns builder configuration; `MagicalExposure` collects a delivery action; `SubstanceSpellResolver` validates and applies compatible spell templates; `SubstanceExposureEffect` retains doses and owns persistent spell children. `SubstanceDose` supplies absorption coefficients shared with the drug implementation.

This change provides runtime and builder tools. A generic fantasy alchemy catalogue, reagent recipes and spellcaster production traditions remain future content work.

## Builder workflow

Use the normal editable-item workflow:

```text
magic substance edit new Restorative Draught
magic substance set reference 1
magic substance set power 5
magic substance set vectors ingested injected
magic substance set bind liquid <liquid-id> <quantity-per-engine-volume-unit>
magic substance set add <spell-id> activation
magic substance set entry 1 maximum 2
magic substance set check
magic substance show
```

Create the payload with `magic spell`. Select `substancecharacter` or `substanceitem` as its trigger, then author its target effects normally. These triggers do not appear in player spell lists and cannot be cast directly. They do not require a caster trait, inventory plan, casting emote, or spell duration formula. Substance delivery does not pay spell resource costs or casting delays. The substance supplies potency and timing.

Other substance commands are `list`, `edit <id|name>`, `clone <id|name> <name>`, `show`, `close` and `set name <name>`. Clones receive distinct effect-entry identities. `set unbind <liquid|gas|item> <id>` removes a carrier binding; `set remove <entry-number>` removes a payload entry. Unbinding stops new deliveries; builders should remove existing effects explicitly if they also want to stop active doses.

`set check` lists missing references and incompatible payloads. An invalid substance is skipped during delivery, so finish the definition before using it. A definition can be built before any carrier is bound.

### Quantity and potency

`reference` is the quantity representing one full dose. Bindings specify quantity per engine fluid-volume unit for liquids/gases, or quantity per complete consumed item for pills, creams, smokeables and incense fuel. Use the world's configured fluid units when converting an intended millilitre dose to a binding: **quantity per engine unit = reference quantity / engine volume of one reference serving**. Item bindings use item prototype IDs, not component prototype IDs.

For example, if one serving occupies 20 engine fluid units, `reference 1` and binding quantity `0.05` make that serving one dose. Drinking five units delivers 0.25 doses. Dilution and mixing preserve constituent quantities, so adding water does not create potency.

`power` is an authored `SpellPower` value from 0 to 10 (5 is Standard). The consumer, maker and applicator do not provide casting traits. Eligible trait formulas may use `power`, `outcome`, `degrees`, `success` and `variable`; trait references and other named options are rejected. Resolution uses a fixed successful spell outcome. Ordinary target resistance checks and incoming magic interdiction still apply. Reflection becomes failure because there is no magical caster to reflect toward.

`maximum` caps the dose used for an entry. `minimum` specifies the minimum fractional dose in one delivery action. Separate subthreshold actions do not earn activation credit. Physical traversal of one action across multiple liquid portions is grouped before checking this threshold. Indivisible instantaneous effects and indivisible maintained/magnitude effects require a positive minimum. Set minimum to 1 for a full-dose-only transformation or cure.

### Lifecycles

| Entry lifecycle | Behavior | Timing |
| --- | --- | --- |
| `activation` | Instantaneous effects resolve once; persistent children are attached once. | Timed children use the entry duration and duration cap. |
| `maintained` | Persistent children update in place while enough physical dose remains. | Internal absorption/clearance or retained external liquid controls lifetime. |
| `periodic`, `pulse presence` | Instantaneous effects pulse while the physical dose remains. | `interval` controls pulses; magnitude follows current active dose. |
| `periodic`, `pulse timed` | Instantaneous effects pulse for a dose-scaled independent lifetime. | `duration`, `durationcap` and `interval` control lifetime and cadence. |

Set options with `magic substance set entry <number> <setting> <value>`:

- `scale duration` scales persistent activation duration; `scale magnitude` scales supported numeric values instead. Instantaneous activation values always scale with dose.
- `duration <seconds>` and `durationcap <seconds>` define the reference lifetime and maximum. Timed pulses always scale lifetime by dose; magnitude scaling can additionally scale their per-pulse strength.
- `interval <seconds>` must be at least one second. Periodic payloads must contain only instantaneous templates, so they cannot repeatedly attach persistent children.
- `stack aggregate` combines dose, caps effective magnitude, and extends timed lifetime up to its cap.
- `stack replace` suppresses the previous retained contribution and replaces it.
- `stack strongest` retains contributions but uses the strongest active one.
- `stack independent` creates independent parents.

Each delivery action is one stacking contribution, even when it contains several liquid portions or lots. Strongest compares complete action doses, and timed lifetime is calculated once from the action total. Timed aggregate effects store a single capped dose/lifetime reservoir; they retain provenance only for liquid still held by the target so dispelling can suppress that coating without accumulating an internal dose history. Saved contributions retain all associated liquid lot identities and still accept older single-lot saves.

Internal maintained/presence doses absorb every ten seconds using drug delivery coefficients, and clear by `clearance` quantity per tick. Aggregated internal contributions share their absorption reservoir per vector. External liquid coatings use actual retained liquid: washing, transfer and evaporation reduce the effect. Coatings prompt physical drying during their updates. Creams use their existing touched absorption pathway rather than inventing a liquid coating.

Timers persist remaining lifetime and pulse/absorption progress. Reload does not repeat activation or replay a backlog of missed pulses. Offline time does not simulate repeated pulses. A duration shorter than the pulse interval may expire without a pulse; builders should align minimum dose, interval and intended lifetime.

### Dispel and output

Persistent effects are normal magic-spell parents for dispelling. Removing a parent or one of its owned children suppresses that dose. Retained surface liquid carries that suppression into subsequent transfers; a fresh dose can work. Duration-changing FutureProg functions and shortening dispels use the logical remaining lifetime, not the update scheduler. Physically maintained effects have no independent duration to shorten; remove them to dispel them.

Activation uses the payload spell's target emote. Resistance uses its target-resisted emote. In substance emotes both `$0` and `$1` refer to the affected target; write these as effects happening to the target, without describing a caster. There is no casting emote on every maintenance update.

## Carriers and conservation

- Drinking (including silent ingestion), injection and IV delivery use the actual delivered liquid mixture.
- Body/item contact includes spills, coating and transfer. Liquid resting inside a container is not exposure to that container.
- Pills, topical cream and smokeables use the actual consumed fraction of the item; the final partial drag is capped by remaining fuel.
- External and integrated inhalers use gas volume per puff. Ambient and equipment breathing supply successful inhaled gas volume. Non-breathers do not inhale magical gas.
- Incense binds to the burned fuel's item prototype, uses the consumed fuel fraction, and follows the burner's range and spatial recipients with distance dilution.
- Prepared food retains magical liquid constituents and spends their proportional quantity per bite. Cooking transfers remaining magical ingredients and divides them across the produced quantity. The existing remove-drugs-and-food-effects recipe option also removes magical ingredients.
- Weapon poison coatings accept magical touched/injected liquids and preserve their payload through delivery filtering. The normal wound-based delivery chance still applies.
- Trap gas clouds accept gas bindings. Their optional `volume` payload setting is engine volume per recipient per five-second pulse (default 1), independent of the existing drug `dose` setting.

A liquid constituent carries a lot identity plus spent/suppressed entry identities. Splitting, copying, mixing, blood/colour variants and XML persistence retain this state. Fresh and spent constituents cannot merge into a homogeneous refreshed charge. A valid activation attempt consumes the charge even if a ward or resistance prevents the spell. Moving that spent liquid does not cast again. Below-threshold contact does not spend activation charge.

Ordinary drugs retain their original dose and metabolism behavior. Magical numeric doses are not inserted into the drug ledger.

## Spell compatibility and extension

Compatibility is explicit in `SubstanceSpellResolver`, rather than invoking every spell template with a missing caster. Unknown types fail readiness. Each template must also accept the chosen character/item trigger.

Supported numeric families include healing/mending, damage, stamina and magic-resource changes, needs, trait bonuses, glow, weight, healing/stamina/need rates, rage/pacifism intensity, item damage/enchantment and spell-armour capacity. Multipliers scale around 1, not 0. Maintained updates change the existing child, preserving identity and state such as absorbed armour damage.

Supported indivisible families include invisibility and senses, flight/water breathing, sleep/paralysis/silence, fear/curse, planar states and shifts, transformation forms (character targets only), magic tags, item creation/destruction, and compatible removal templates. A removal template retains its existing meaning: for example, removing a matching magical poison payload is not a new universal cure for all physiological drugs.

Caster-linked/concentration effects, extra-target effects, cell/exit effects and unadapted templates are rejected. New compatibility requires source-independent application, explicit dose scaling or a threshold, lifecycle validation, persistent-child update behavior where relevant, and tests. Do not broaden the whitelist solely because a template happens not to throw when given a null caster.

## Persistence and verification

Migration `20260911080024_MagicalSubstances` adds `MagicalSubstances(Id, Name, Definition)`. Definition XML stores bindings, authored potency, clearance and stable entry identities. The runtime loads substances after spells. Liquid charges live in existing liquid XML; retained dose/child state lives in existing saved-effect XML; prepared-food magic lives in component XML. Existing carrier tables require no new columns.

The bundled blank snapshot includes the EF-generated migration delta with the dump's lower-case table identifiers. Healing and mending overflow advance through each eligible wound once, including when a dose exceeds all remaining damage. Regression tests cover fractional dosing, threshold grouping, spent transfers, wards, physical removal, dispel, absorption, pulse timing, stacking, exact short durations and saved state. Live database import and interactive exposure tests are separate from these deterministic tests.
