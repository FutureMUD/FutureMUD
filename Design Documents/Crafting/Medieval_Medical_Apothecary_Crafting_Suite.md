# Medieval Medical Apothecary Crafting Suite

The medieval medical and apothecary suite covers portable treatment stock, infirmary furniture props, herbal preparation, barber-surgeon tools, monastic care, battlefield evacuation, and household healing. It is designed for roleplay-rich scenes rather than a complete medical simulation rewrite.

## Seeded Stock

| Family | Stable References |
| --- | --- |
| Bandaging and poultices | `medieval_medical_linen_bandage_roll`, `medieval_medical_poultice_cloth` |
| Herbal and apothecary prep | `medieval_medical_apothecary_mortar`, `medieval_medical_herb_pouch`, `medieval_medical_salve_pot`, `medieval_medical_apothecary_jar`, `medieval_medical_herb_drying_tray` |
| Medical vessels and treatment props | `medieval_medical_cupping_horn`, `medieval_medical_bleeding_bowl` |
| Surgery and repair | `medieval_medical_surgical_roll`, `medieval_medical_bone_saw`, `medieval_medical_splint_set` |
| Mobility and evacuation | `medieval_medical_field_stretcher`, `medieval_medical_crutch_pair` |
| Complete kits | `medieval_medical_physicians_bag`, `medieval_medical_monastic_infirmary_kit` |

## Implemented Components

| Stable Reference | Component Notes |
| --- | --- |
| `medieval_medical_linen_bandage_roll` | Uses `Bandage_Simple`. |
| `medieval_medical_poultice_cloth` | Uses `Bandage_Simple` as the closest live treatment component. |
| `medieval_medical_field_stretcher` | Uses `DragAid_Antiquity_FieldStretcher` until a medieval-specific stretcher variant is worth splitting. |
| Bags, rolls, pouches, kits, and trays | Use existing container components such as `Container_Pouch`, `Container_Tote`, `Container_Pack`, and `Container_Tray`. |
| Salve pots, jars, and bleeding bowls | Use existing liquid-container behaviour where useful for fluid handling. |

## Crafting

Crafts are registered through `SeedMedievalMedicalApothecaryCrafts()` and use `Medieval Workshop Practice`.

| Surface | Trait | Inputs And Tools |
| --- | --- | --- |
| Bandages and poultice cloths | `Tailoring` | Linen `Garment Cloth`, `Shears`, and sewing tools when needed. |
| Herb pouches, surgical rolls, physician bags, and infirmary kits | `Leathermaking` | `Prepared Leather Panel`, thread, awls, and shears. |
| Mortars and stone tools | `Masonry` | Stone tool stock and hammer. |
| Pots, jars, and bowls | `Pottery` | `Pottery Clay Body` and `Hot Fire`. |
| Bone, horn, or hard-organic pieces | `Bonecarving` | Bone tool stock and awls. |
| Saws and metal instruments | `Blacksmithing` | Metal tool stock, anvil, and hammer. |
| Crutches, splints, and trays | `Carpentry` | Wood stock, hammer, and awl. |

## Builder Workflows

The suite supports:

- Monastic infirmaries, hospital wards, shrine care, charity houses, and pilgrim hostels.
- Household healers, wise-women, herbalists, apothecaries, barber-surgeons, and physicians.
- Campaign camps, city watches, castle guardrooms, naval sick bays, and battlefield aftermath scenes.
- Market stalls selling simples, salves, bandages, jars, and treatment bundles.

Future depth belongs in a medical/apothecary subsystem pass for salve effects, poultice recipes, cupping behaviour, procedural surgery tools, infection, disease, drug dosage, and learned medical traditions.
