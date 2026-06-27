# Medieval Jewellery Seeder Dependency Request

This file is a Codex-facing dependency checklist for the medieval decorative jewellery item-seeder catalogue. It should be actioned before the full 400-row jewellery implementation pass so the seeder can create historically useful jewellery rows without skipping major forms or substituting inaccurate components, tags, or materials.

The intended consumer is an implementation agent working in the FutureMUD codebase. The list below intentionally uses the longer/full dependency set rather than the minimal unblocker set.

## Purpose

The medieval jewellery reference targets about 400 decorative personal-adornment item prototypes across commoner, merchant, professional, noble, court, child/apprentice, festival, and ephemeral organic jewellery.

The existing seed inventory already supports many generic jewellery rows, including rings, necklaces, chokers, bracelets, armlets, anklets, earrings, nose rings, toe rings, headband-like ornaments, headwear-like ornaments, and waist-worn objects. However, a historically satisfying medieval catalogue also needs brooches, pins, badges, hair ornaments, temple rings, combs, circlets, diadems, coronets, crowns, garlands, wreaths, neck rings, torcs, waist chains, belt plaques, and girdle ornaments.

Rather than forcing those forms through approximate slots or leaving them inert, add the components, tags, materials, and optional variables below.

## Grounding assumptions

- Item rows must continue to use exact seeded component prototype names.
- Item rows must continue to use exact seeded solid material names for the `material` argument.
- Item rows must continue to use exact seeded hierarchical tag paths.
- Liquids and gases are not substitutes for solid primary materials.
- Finished decorative jewellery should remain skinnable, player-visible, portable, and normally `Holdable`.
- Decorative jewellery should not receive armour, insulation, container, lock, hidden-storage, identity-obscuring, light-source, weapon, or magic behaviour unless a later branch explicitly designs such behaviour.
- Component names below are proposed exact prototype names. If an equivalent prototype already exists, keep the existing exact name and update the jewellery reference accordingly rather than adding a duplicate.
- Plural wearable components should support intentionally paired or set-like rows without making the item author fake two separate items.

## Required wearable component prototypes

All components in this section should be ordinary wearable component prototypes unless the implementation notes state otherwise. The goal is precise wearable behaviour, not new mechanical powers.

| Proposed component name | Needed for | Notes |
|---|---|---|
| `Wear_Brooch` | ring brooches, annular brooches, disc brooches, penannular brooches, jewelled court brooches, secular dress brooches | Core blocker. Should behave as a small accessory fastened to clothing or cloak area without implying container, armour, or lock behaviour. |
| `Wear_Brooches` | paired northern garment brooches, paired oval/bowl brooch silhouettes, paired court brooches | Plural/set version for rows that are visibly a matched pair. |
| `Wear_Pin` | cloak pins, dress pins, ring pins, straight pins, decorative fastening pins | Should cover pin-like personal ornaments and garment fasteners. Descriptions can mention pin and catch when this exists. |
| `Wear_Badge` | secular household badges, civic badges, love badges, guild/professional badges, livery-adjacent badges | Keep devotional/pilgrim badges out of the decorative jewellery branch unless a separate devotional branch uses the component. |
| `Wear_Hairpin` | East Asian, South Asian, Byzantine, Islamic, and elite hairpin ornaments | Should occupy a hair/head accessory position rather than a hat slot where possible. |
| `Wear_Hairpins` | paired or set hairpin rows | Plural/set version. Useful for East Asian and courtly sets. |
| `Wear_Hair_Comb` | single comb-backed hair ornament | Needed for comb ornaments that are not ordinary hand combs. |
| `Wear_Hair_Combs` | paired comb ornaments or matched comb sets | Plural/set version. |
| `Wear_Hair_Ornament` | generic decorative hair plaques, hair rings, hair tablets, or hard-to-classify hair jewellery | Useful fallback for regional hair jewellery that is neither a pin nor comb. |
| `Wear_Hair_Ornaments` | paired or grouped generic hair ornaments | Plural/set version. |
| `Wear_Temple_Rings` | Rus/Novgorod, Magyar, Slavic, and steppe temple-ring-like ornaments | Should represent ornaments worn at the temples or suspended near the sides of the head. |
| `Wear_Circlet` | light metal circlets, pearl circlets, fine head jewellery, court circlets | Avoids treating every circlet as a hat. |
| `Wear_Diadem` | narrow elite forehead/head diadems | Useful for Byzantine, western court, and high noble rows. |
| `Wear_Coronet` | lesser noble head regalia | Allows noble rank head jewellery without using full crown language. |
| `Wear_Crown` | true crown/regalia pieces, if included | Only needed if crowns are intended as ordinary seedable items. Keep rare. |
| `Wear_Chaplet` | ribbon chaplets, flower chaplets, simple festival chaplets | Head-worn chaplets should not have to use generic headband substitutions. |
| `Wear_Wreath` | leaf wreaths, laurel-style wreaths, ivy wreaths, dried wreaths, festival wreaths | Should support both fresh and dried morph-target rows. |
| `Wear_Head_Garland` | head-worn flower garlands and blossom garlands | Needed for fresh head garlands and festival head adornment. |
| `Wear_Neck_Garland` | neck-worn flower garlands, South Asian festival garlands, court/festival garlands | Needed because necklace substitution loses ephemeral/floral semantics. |
| `Wear_Wrist_Garland` | flower wrist garlands, herb wrist garlands, festival bracelet-garlands | Needed for short-lived commoner and festival pieces. |
| `Wear_Ankle_Garland` | blossom anklets, flower ankle garlands, festival ankle adornment | Needed especially for warm-climate and South Asian-inspired festival jewellery. |
| `Wear_Torc` | rigid neck rings and torc-like collars | Avoids forcing torcs through choker or necklace slots. |
| `Wear_Neck_Ring` | non-torc rigid neck rings, plain neck rings, collar-like metal neck ornaments | Useful when the form is a ring/collar but not culturally or visually a torc. |
| `Wear_Waist_Chain` | decorative waist chains | Should not provide belt attachment behaviour. |
| `Wear_Girdle_Ornament` | dangling girdle ornaments, girdle jewels, decorative girdle mounts | For jewellery-like waist/girdle pieces that are not functional belts. |
| `Wear_Belt_Ornament` | decorative belt-mounted ornaments and belt jewels | For single belt ornaments that are worn/displayed rather than containers or beltable tools. |
| `Wear_Belt_Plaques` | steppe, Magyar, merchant, and court belt-plaque display rows | Plural/set version for plaque arrays. |
| `Wear_Waist_Ornament` | generic waist jewellery that is not specifically a chain, belt plaque, or girdle jewel | Useful fallback slot. |
| `Wear_Forehead_Ornament` | South Asian head jewellery, forehead pendants, brow ornaments, East Asian/court head ornaments | Strict medieval catalogue should use this carefully, but it prevents inaccurate hat/circlet substitutions. |

### Wearable component behaviour notes

- Brooch, pin, and badge components should not claim to mechanically hold a cloak closed unless the wearable behaviour is intended to allow that presentation. If the engine only models wearing, that is still sufficient; item descriptions should avoid promising garment-fastening mechanics beyond visible fastening.
- Hairpin, hair comb, temple ring, and hair ornament components should ideally be compatible with ordinary clothing and not occupy generic torso/hat slots.
- Circlets, diadems, coronets, crowns, chaplets, head garlands, and wreaths may be mutually exclusive with each other if the wearable system requires one head-jewellery item at a time, but they should not be implemented as ordinary helmets.
- Torcs and neck rings may be mutually exclusive with necklaces/chokers if the system has a single neck jewellery slot. If multiple neck adornment layering is supported, use the narrower slot semantics.
- Waist chains, girdle ornaments, belt ornaments, and belt plaques must not behave as functional belts unless a separate belt component is deliberately added to a specific future item row.

## Conditional seal-stamp component prototypes for signet rings

Decorative signet-like rings can use `Wear_Ring` only. Functional seal rings need dedicated seal-stamp prototypes that are safe to combine with `Wear_Ring`.

Add these only if wearable rings are allowed to apply seals mechanically:

| Proposed component name | Component family | Needed for | Notes |
|---|---|---|---|
| `SealStamp_Medieval_RingSignet` | `SealStamp` | generic functional signet rings | Should be compatible with a wearable ring row. |
| `SealStamp_Medieval_PersonalSignetRing` | `SealStamp` | personal seal rings | Used for personal authority or household identity. |
| `SealStamp_Medieval_MerchantSignetRing` | `SealStamp` | merchant/professional signet rings | Useful for merchant, notary, guild, and trade rows. |
| `SealStamp_Medieval_NobleSignetRing` | `SealStamp` | noble or court seal rings | High-status seal rings. |

Implementation note: if seal metadata requires per-instance state, these components should only define behaviour. Exact seal devices, owner names, guild marks, household marks, and heraldry should remain in skins, seal metadata, or written content.

## Required finished-jewellery function tags

Add the following exact tag paths under `Functions / Worn Items / Jewellery`.

```text
Functions / Worn Items / Jewellery / Armlets
Functions / Worn Items / Jewellery / Badges
Functions / Worn Items / Jewellery / Bead Strings
Functions / Worn Items / Jewellery / Belt Ornaments
Functions / Worn Items / Jewellery / Belt Plaques
Functions / Worn Items / Jewellery / Brooches
Functions / Worn Items / Jewellery / Chaplets
Functions / Worn Items / Jewellery / Chokers
Functions / Worn Items / Jewellery / Circlets
Functions / Worn Items / Jewellery / Coronets
Functions / Worn Items / Jewellery / Crowns
Functions / Worn Items / Jewellery / Diadems
Functions / Worn Items / Jewellery / Forehead Ornaments
Functions / Worn Items / Jewellery / Garlands
Functions / Worn Items / Jewellery / Girdle Ornaments
Functions / Worn Items / Jewellery / Hair Ornaments
Functions / Worn Items / Jewellery / Head Ornaments
Functions / Worn Items / Jewellery / Neck Garlands
Functions / Worn Items / Jewellery / Neck Rings
Functions / Worn Items / Jewellery / Pendants
Functions / Worn Items / Jewellery / Pins
Functions / Worn Items / Jewellery / Temple Rings
Functions / Worn Items / Jewellery / Toe Rings
Functions / Worn Items / Jewellery / Torcs
Functions / Worn Items / Jewellery / Waist Chains
Functions / Worn Items / Jewellery / Waist Ornaments
Functions / Worn Items / Jewellery / Wreaths
```

### Required piercing sub-tags

Add the following under `Functions / Worn Items / Jewellery / Piercings`.

```text
Functions / Worn Items / Jewellery / Piercings / Ear Studs
Functions / Worn Items / Jewellery / Piercings / Nose Rings
Functions / Worn Items / Jewellery / Piercings / Nose Studs
```

### Optional but useful tag refinements

These are not strict blockers if the tag hierarchy should stay shallow, but they would make validation and search cleaner for the 400-row catalogue.

```text
Functions / Worn Items / Jewellery / Garlands / Dried Garlands
Functions / Worn Items / Jewellery / Garlands / Flower Garlands
Functions / Worn Items / Jewellery / Garlands / Fresh Garlands
Functions / Worn Items / Jewellery / Garlands / Herb Garlands
Functions / Worn Items / Jewellery / Garlands / Leaf Garlands
Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths
Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths
Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths
Functions / Worn Items / Jewellery / Wreaths / Herb Wreaths
Functions / Worn Items / Jewellery / Wreaths / Leaf Wreaths
```

## Required market tags

The current decorative jewellery rows should not need to masquerade as generic household wares. Add a dedicated finished-jewellery market branch.

```text
Market / Jewellery
Market / Jewellery / Children's Jewellery
Market / Jewellery / Commoner Jewellery
Market / Jewellery / Court Jewellery
Market / Jewellery / Ephemeral Jewellery
Market / Jewellery / Festival Jewellery
Market / Jewellery / Luxury Jewellery
Market / Jewellery / Merchant Jewellery
Market / Jewellery / Noble Jewellery
Market / Jewellery / Professional Jewellery
Market / Jewellery / Regalia
Market / Jewellery / Simple Jewellery
Market / Jewellery / Standard Jewellery
```

### Market-tag usage guidance

- `Market / Jewellery / Simple Jewellery`: cheap copper, bone, shell, wood, cord, common glass, and simple commoner rows.
- `Market / Jewellery / Standard Jewellery`: ordinary market jewellery, pewter/brass/bronze rows, modest glass or silver rows.
- `Market / Jewellery / Luxury Jewellery`: precious metals, gems, pearls, fine craft, elite merchant pieces, and gentry jewellery.
- `Market / Jewellery / Court Jewellery`: court chains, circlets, diadems, high-status gold/gem rows, and formal display pieces.
- `Market / Jewellery / Festival Jewellery`: fresh garlands, flower chaplets, ribbons, courtship pieces, and seasonal rows.
- `Market / Jewellery / Children's Jewellery`: child-sized bead strings, tiny rings, cord bracelets, and harmless small adornments.
- `Market / Jewellery / Merchant Jewellery`: respectable urban display pieces, signet-like rings, merchant chains, guild-colour bead strings.
- `Market / Jewellery / Noble Jewellery`: high noble but not necessarily royal pieces.
- `Market / Jewellery / Regalia`: rare crowns, coronets, symbolic court collars, and regalia-adjacent rows.
- `Market / Jewellery / Ephemeral Jewellery`: short-lived organic pieces with morph targets or festival disposability.

## Required material additions

Add the following solid materials. Use lower-case exact material names unless the material system already has a different naming convention. These are primary-material candidates, not liquids or gases.

### High-priority jewellery materials

```text
coral
rock crystal
faience
enamel
niello
silver-gilt
gilded bronze
gilded copper
mother-of-pearl
nacre
cowrie shell
conch shell
tortoiseshell
```

### High-priority material notes

| Material | Needed for | Notes |
|---|---|---|
| `coral` | Mediterranean, Islamic, South Asian, and elite bead/pendant jewellery | Treat as an organic/animal-derived ornamental material, not a generic gemstone if the material taxonomy distinguishes it. |
| `rock crystal` | rings, pendants, beads, high-status stones, clear cabochons | Better than using generic quartz when a clear medieval rock-crystal presentation is wanted. |
| `faience` | coloured bead strings, glassy ceramic beads, cheaper blue/green beadwork | Useful for lower-cost decorative beads and regional beadwork. |
| `enamel` | enamelled brooches, rings, plaques, badges, circlets, court pieces | If enamel is better modelled as a surface treatment rather than a primary material, still add it for enamel-dominant small plaques and craft stock. |
| `niello` | dark inlay on silver/gilt rings, brooches, belt ornaments | Primarily an inlay material; useful as primary material for craft stock or inlay-dominant tiny items. |
| `silver-gilt` | gilded silver jewellery | This is a historically important luxury category between plain silver and gold. Prefer this over pretending every gilt silver piece is pure gold. |
| `gilded bronze` | mid-status brooches, badges, rings, and plaques | Useful for gold-looking but less expensive metalwork. |
| `gilded copper` | cheap-to-mid-status gilt jewellery and ornaments | Useful for inexpensive gold-looking adornment. |
| `mother-of-pearl` | inlay, pendants, beadwork, hair combs, shell jewellery | Preferred player-facing canonical name if only one of `mother-of-pearl` and `nacre` is kept. |
| `nacre` | inlay and shell material | Add as alias or separate material only if the material system supports synonym-like materials cleanly. |
| `cowrie shell` | shell beads, Indian Ocean trade pieces, South Asian and commoner jewellery | Useful as a specific shell form rather than generic shell. |
| `conch shell` | shell bangles, beads, pendants, South Asian shell jewellery | Useful for warm-climate and Indian Ocean jewellery rows. |
| `tortoiseshell` | hair combs, hair ornaments, elite comb-backed pieces | Use only for decorative material; avoid wildlife-trade modern framing in public item text. |

### Ephemeral and floral jewellery materials

```text
flower
fresh flower
wilted flower
dried flower
petal
dried petal
rose
violet
daisy
jasmine
lotus flower
marigold
lily
chrysanthemum
blossom
ivy
laurel
rush
straw
```

### Ephemeral/floral material notes

- `flower`, `fresh flower`, `wilted flower`, and `dried flower` allow generic morphing garlands without forcing every row into a specific species.
- Species materials such as `rose`, `jasmine`, `lotus flower`, `marigold`, and `chrysanthemum` support regional skins and base rows where the flower itself is the dominant visible substance.
- `ivy` and `laurel` support leaf wreaths and chaplets without using generic `leaf` for everything.
- `rush` and `straw` support cheap woven festival rings, rustic chaplets, and temporary commoner adornment.
- If the material system should avoid stateful materials such as `fresh flower` and `wilted flower`, add only `flower` and `dried flower`, then handle freshness through item descriptions and morph targets. The jewellery catalogue would still benefit from separate `dried flower` for morph targets.

### Suggested material-taxonomy tags

Use existing material taxonomy paths where they already exist. If new taxonomy paths are needed, prefer these broad categories:

```text
Materials / Animal Product / Coral
Materials / Animal Product / Shell
Materials / Animal Product / Tortoiseshell
Materials / Manufactured Materials / Ceramic / Faience
Materials / Manufactured Materials / Glass / Enamel
Materials / Manufactured Materials / Manufactured Metal / Gilded Metal
Materials / Manufactured Materials / Manufactured Metal / Inlay Material
Materials / Natural Materials / Plant Product / Flower
Materials / Natural Materials / Plant Product / Dried Flower
Materials / Natural Materials / Plant Product / Leaf
Materials / Natural Materials / Stone / Economically Useful Stone / Gemstone
```

These taxonomy suggestions are not meant to override existing conventions. They are included so the Codex agent has a clear classification target when adding the materials.

## Optional variable component additions

The existing colour, two-colour, common-stone, gem, and fine-gem variable components are sufficient for most jewellery rows. The variables below are optional but requested as part of the longer dependency set because they will reduce duplicate rows and keep base descriptions varied without adding behaviour-heavy prototypes.

| Proposed variable component | Variable token | Needed for | Suggested values |
|---|---|---|---|
| `Variable_JewelleryMotif` | `$motif` | decorative motifs on rings, brooches, pins, badges, chains, plaques, and circlets | knotwork, interlace, vine, rosette, wave, bird, beast, star, crescent, leaf, flower, geometric, spiral, dotted, punched |
| `Variable_Flower` | `$flower` | fresh garlands, chaplets, wreaths, flower bracelets, blossom anklets | rose, violet, jasmine, marigold, lotus, lily, daisy, chrysanthemum, blossom |
| `Variable_MetalFinish` | `$finish` | metal rings, bracelets, brooches, chains, badges, pins, plaques | polished, burnished, darkened, gilt, silvered, tinned, hammered, chased, stamped |
| `Variable_BeadPattern` | `$beadpattern` | bead strings, necklaces, bracelets, anklets, temple ornaments | alternating, graduated, clustered, spaced, mixed, matched, irregular, patterned |
| `Variable_JewelleryShape` | `$shape` | brooches, badges, pendants, plaques, beads, ring settings | round, oval, square, lozenge, crescent, lunate, teardrop, leaf-shaped, tablet-shaped |
| `Variable_InlayStyle` | `$inlay` | enamel-like panels, niello-like lines, glass inlay, shell inlay | enamelled, dark-inlaid, glass-inlaid, shell-inlaid, dotted, wire-inlaid |

### Variable usage examples

```text
a $motif silver ring
a $flower garland
a $finish bronze brooch
a $beadpattern glass bead necklace
a $shape enamelled badge
a $inlay silver-gilt pin
```

Variable components should not be used to imply unseeded primary materials. If a material materially changes the item category or behaviour, create a distinct row or add the material first.

## Content explicitly not needed for this dependency pass

- No new liquids are required for decorative jewellery.
- No new gases are required for decorative jewellery.
- No new destroyable components are required; existing heavy metal, glassware, misc, clothing, paper, and similar destroyable components are enough.
- No new armour components are required.
- No new insulation components are required.
- No new container components are required for ordinary jewellery.
- No magic, trait-changing, identity-obscuring, lock, hidden-storage, weapon, or light-source components should be added for ordinary decorative jewellery.

## Implementation priority

Recommended order for the Codex agent:

1. Add missing wearable component prototypes.
2. Add signet-ring seal-stamp prototypes only if functional wearable signet rings are approved.
3. Add finished-jewellery function tags.
4. Add `Market / Jewellery` tags.
5. Add missing materials with appropriate material taxonomy tags.
6. Add optional variable components if the variable framework can support them cleanly.
7. Run a validation pass over the medieval jewellery design reference and update any dependency notes that are now obsolete.

## Acceptance checklist

The dependency pass is complete when:

- All required wearable component prototype names above exist exactly or an intentional existing equivalent is documented.
- Decorative brooches, paired brooches, pins, badges, hairpins, hair combs, temple rings, circlets, diadems, coronets, chaplets, wreaths, garlands, torcs, neck rings, waist chains, girdle ornaments, belt plaques, and forehead ornaments can be authored without inaccurate wearable substitutions.
- All required finished-jewellery tags exist exactly.
- All required market tags under `Market / Jewellery` exist exactly.
- All required solid materials exist exactly or a conscious canonical-alias decision has been made.
- Optional variable components either exist or are explicitly deferred with a note that skins will carry those variations.
- The item authoring rules remain satisfied: exact component names, exact solid materials, exact tags, no unsupported component-dependent claims, and no devotional overlap in the decorative jewellery branch.

## Notes for the later 400-row jewellery catalogue

- Do not skip brooches, pins, badges, hair ornaments, circlets, garlands, or wreaths once these dependencies exist. They are historically and socially important, not edge cases.
- Do not collapse all commoner jewellery into one or two cheap ring rows. Include cord, bone, shell, wood, glass, flower, herb, leaf, copper, brass, and bead pieces.
- Do not make the catalogue elite-only. Merchant, professional, apprentice, child, rural, festival, and ephemeral pieces are part of the intended social coverage.
- Keep devotional objects out of this branch even when they are wearable. Pilgrim badges, prayer beads, relic pendants, sacred amulets, reliquary lockets, and scripture-inscribed charms belong to a devotional/religious personal-items branch.
- Keep signet-ring function honest. A decorative signet-like ring is just jewellery; a functional signet ring needs seal-stamp behaviour.
- Fresh organic jewellery should use morph targets where useful: fresh item -> wilted item -> dried item if the dried object is worth keeping.
- Skins should carry exact household marks, guild marks, heraldry, clan signs, dynastic devices, flower species, bead ordering, exact gemstone choice, local fantasy names, and decorative inscriptions unless those details change behaviour.
