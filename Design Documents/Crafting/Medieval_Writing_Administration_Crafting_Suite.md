# Medieval Writing Administration Crafting Suite

The medieval writing and administration suite is the main v1 surface that benefits from live `SealStamp`, `Sealable`, and `MeasuringInstrument` components. It covers documents, sealable containers, signets, seal matrices, writing tools, record bundles, market measures, standard and false weights, sealed bales, ledger storage, tax/customs kits, and culture-specific office props.

## Culture Administration Stock

For every culture key, the seeder creates `medieval_writing_{culture}_{administration_item}` entries:

| Token | Use |
| --- | --- |
| `office_bundle` | A sealed administrative document bundle with writable and sealable behaviour. |
| `record_tablet` | A short-record surface for memoranda, practice, accounts, or temporary records. |
| `tally_bundle` | Counting, rent, debt, custody, tax, or delivery prop. |
| `seal_tag_packet` | Tags, slips, cords, labels, and authority markers for record-keeping. |

Examples include charter strips, sealed writs, runic tallies, notarial notes, paper contracts, chrysobull copies, chancery seals, birchbark notes, paiza tags, and official chops. Culture text is held in builder notes and knowledge gates; visible craft text remains culture-neutral.

## Writing And Office Catalogue

| Family | Stable References |
| --- | --- |
| Writable and sealable documents | `medieval_writing_parchment_charter`, `medieval_writing_sealable_envelope`, `medieval_writing_account_roll` |
| Writing implements and ink | `medieval_writing_quill_pen`, `medieval_writing_reed_pen`, `medieval_writing_ink_horn` |
| Writing surfaces and books | `medieval_writing_wax_tablet`, `medieval_writing_paper_sheet`, `medieval_writing_parchment_quire`, `medieval_writing_bound_codex` |
| Tally and seal support | `medieval_writing_tally_sticks`, `medieval_writing_charter_tag_set`, `medieval_writing_seal_cord_bundle`, `medieval_writing_wax_seal_cake` |
| Seals and office kits | `medieval_writing_office_signet_ring`, `medieval_writing_office_seal_matrix`, `medieval_writing_notary_kit`, `medieval_writing_guild_stamp` |
| Document containers | `medieval_writing_document_satchel`, `medieval_writing_ledger_chest` |

## Trade And Measurement Catalogue

| Stable Reference | Component Use |
| --- | --- |
| `medieval_trade_balance_scale` | `MeasuringInstrument_Antiquity_BalanceScale`. |
| `medieval_trade_standard_weight_set` | `MeasuringInstrument_Antiquity_StandardWeights`. |
| `medieval_trade_false_weight_set` | `MeasuringInstrument_Antiquity_FalseWeights`. |
| `medieval_trade_grain_measure` | `MeasuringInstrument_Antiquity_GrainMeasure`. |
| `medieval_trade_tax_customs_kit` | `MeasuringInstrument_Antiquity_TaxAssessorKit`, `Sealable_Container_Wax`. |
| `medieval_trade_sealable_bale` | `Sealable_Container_Wax`. |
| `medieval_surveyor_measuring_rope` | Prop-only until length/surveying measurement modes exist. |

## Implemented Live Components

| Stable Reference | Component Use |
| --- | --- |
| `medieval_writing_{culture}_office_bundle` | `PaperSheet_Scroll`, `Sealable_Document_Wax`. |
| `medieval_writing_parchment_charter` | `PaperSheet_Scroll`, `Sealable_Document_Wax`. |
| `medieval_writing_sealable_envelope` | `Container_Envelope`, `PaperSheet_Envelope`, `Sealable_Envelope`. |
| `medieval_writing_account_roll` | `PaperSheet_Scroll`, `Sealable_Document_Wax`. |
| `medieval_writing_document_satchel` | `Container_Tote`, `Wear_Shoulder`, `Sealable_Container_Wax`. |
| `medieval_writing_ledger_chest` | `Container_Trunk`, `Sealable_Container_Wax`. |
| `medieval_writing_office_signet_ring` | `Wear_Ring`, `SealStamp_Antiquity_BronzeSignet`. |
| `medieval_writing_office_seal_matrix` | `SealStamp_Antiquity_BronzeSignet`. |
| `medieval_writing_notary_kit` | `SealStamp_Antiquity_BronzeSignet`, `Sealable_Container_Wax`. |
| `medieval_writing_guild_stamp` | `SealStamp_Antiquity_BronzeSignet`. |

## Knowledge Gates

| Surface | Knowledge Pattern | Trait |
| --- | --- | --- |
| Culture office bundles and short-record props | `Medieval Administration Pattern {culture}` | `Tailoring`, `Carpentry`, or `Writing` category craft paths |
| Generic documents, books, seal cords, and satchels | `Medieval Workshop Practice` | `Tailoring`, `Leathermaking`, or `Carpentry` |
| Seals and stamps | `Medieval Workshop Practice` | `Silversmithing` |
| Weights, scales, and measuring kits | `Medieval Workshop Practice` | `Blacksmithing` or `Carpentry` |
| Wax and lighting-adjacent seal support | `Medieval Workshop Practice` | `Candlemaking` |

## Craft Inputs And Tools

Common inputs:

- `Paper Pulp Stock`, `Paper Sheet Stock`, `Parchment Sheet Stock`, `Seal Cord Stock`, and `Sealing Wax Stock` for paper and parchment records, envelopes, seal tags, cords, and sealed documents.
- `Bookbinding Leather Stock` for codices and `Prepared Leather Panel` for document satchels and notary kits.
- `Standard Weight Blank`, `Tally Stick Stock`, and `Sealable Bale Wrapper Stock` for guild weights and measures, customs kits, sealed bales, and account tallies.
- `Tool Blank Stock` for signets, seal matrices, guild stamps, balances, and keys/fittings.
- `Furniture Panel Stock` for ledger chests and office storage.

Required tools are backed by historic sewing needles, shears, awls, hammers, anvils, forge tongs, and hot-fire support, plus medieval bookbinding, papermaking, and seal-preparation tools: `Bookbinder's Sewing Frame`, `Book Press`, `Leather Paring Knife`, `Mould and Deckle`, `Papermaking Vat`, and `Wax Spatula`.

The production-chain pass distinguishes paper and parchment workflows without making visible craft text culture-specific. Song, Islamic, and mercantile paperwork can consume `Paper Sheet Stock`, while charters, account rolls, codices, and legal packets keep `Parchment Sheet Stock` where that better fits the object.

## Deferred Administration Gaps

`medieval_surveyor_measuring_rope` is deliberately prop-only. The current `MeasuringInstrument` prototypes cover weight, dry measure, and fluid volume, not length or surveying. Printing presses, spectacles, mechanical clocks, complex chancery workflows, bureaucracy templates, and document-authentication AI remain later slices.
