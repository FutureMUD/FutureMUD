# Medieval Writing and Administration Crafting Suite

The medieval writing and administration suite should represent culture-specific recordkeeping media, legal forms, office tools, seals, accounting systems, and scholarly objects.

The first merged implementation added generic document bundles, record tablets, tally bundles, and seal-tag packets for each culture. The second pass must add exact culture-specific media and fix component mismatches.

## Design Principle

Writing/admin content should distinguish the social institution behind the object:

- Monastery
- Court/chancery
- Town/guild
- Market/customs
- Frontier lordship
- Steppe messenger network
- Scholar/bureaucratic office
- Religious endowment or temple/mosque/church institution

## Runtime Component Guidance

Use existing component families deliberately:

| Media / Tool | Expected Component Family |
| --- | --- |
| Loose paper sheets | `PaperSheet` |
| Parchment charters and scrolls | `PaperSheet` / scroll-style components |
| Codices, books, registers | `Book` |
| Wax tablets | `InscribableSurface` |
| Wooden tablets and record boards | `InscribableSurface` |
| Birchbark letters | `InscribableSurface` or a suitable non-paper writable surface |
| Sealed charters, packets, bales | `Sealable` |
| Seal rings, matrices, guild stamps, official chops | `SealStamp` |
| Balance scales, weights, dry/liquid measures | `MeasuringInstrument` |

Do not use `PaperSheet_Scroll` for a wooden record tablet unless the item is actually a scroll-like paper/parchment object.

## Minimum Culture Surface

Each culture should receive at least six explicit writing/admin items.

| Slot | Examples |
| --- | --- |
| Primary document | charter, decree, contract, register leaf, printed notice, birchbark letter |
| Short-record surface | wax tablet, wooden tablet, tally, tag, account slip |
| Seal or seal packet | signet, office seal, official chop, seal-tag packet |
| Accounting/tax object | tally, rent record, tax roll, toll record, trade account |
| Scholarly/religious object | codex, prayer slip, scholar notebook, manuscript leaf, endowment note |
| Container or kit | document pouch, writing box, notary kit, tablet wallet, ledger chest |

## Culture Targets

Exact stable references are listed in `Medieval_Culture_Catalogue.md`.

Examples by culture:

| Culture | Writing/Admin Direction |
| --- | --- |
| Early Anglo-Saxon | wax diptychs, charter strips, monastic manuscript leaves, reeve tallies, gospel pouches |
| Norse | runic tally sticks, trade tags, cargo tallies, wax tablets, tablet wallets |
| Norman / Angevin | sealed parchment charters, exchequer rolls, writ packets, notary seals, manorial accounts |
| High British | manor account rolls, guild register leaves, sealed writs, levy lists, chapel books |
| Byzantine | chrysobull copies, icon-label tablets, monastery codices, tax registers, court order rolls |
| Abbasid / Fatimid / Andalusi / Seljuk | paper decrees, contracts, chancery seal packets, scholar notebooks, waqf/endowment records |
| Rus / Novgorod | birchbark letters, princely seal tags, river trade tallies, fur-tax records |
| Steppe Turkic | paiza tag props, sealed leather pouches, herd tallies, tribute strips, messenger packets |
| Song China | paper registers, printed notices, scholar notebooks, official chop documents, examination booklets |

## Craft Inputs

Use the existing medieval stocks:

| Product | Suggested Inputs |
| --- | --- |
| paper documents | `Paper Sheet Stock`, ink stock where available, seal wax/cord if sealed |
| parchment charters | `Parchment Sheet Stock`, `Seal Cord Stock`, `Sealing Wax Stock` |
| wax tablets | wooden panel stock, beeswax or `Sealing Wax Stock`, stylus support |
| birchbark letters | birch/bark material stock where available, or plant/wood stock plus inscribable surface |
| codices/books | `Parchment Sheet Stock` or `Paper Sheet Stock`, `Bookbinding Leather Stock`, sewing thread/cord |
| tallies | `Tally Stick Stock`, cutting/notching tools |
| seal matrices/stamps | bronze/silver `Tool Blank Stock`, engraving tools |
| notary/guild/office kits | container, seal, wax, cord, document stock, writing implements |

## Craft Naming

Use product-specific final craft names:

```text
prepare a Norman sealed parchment charter
notch a Norse runic trade tally
write a Rus birchbark letter
prepare a Song official chop document
bind an Abbasid scholar notebook
```

Do not use `regional record tablet` as the final object name for explicit culture records.

## Test Requirements

Add tests that verify:

- Each culture has at least 6 explicit writing/admin references.
- Wooden, wax, birchbark, and tablet items do not use `PaperSheet_Scroll` unless explicitly justified.
- Sealed documents include `Sealable`.
- Seal tools include `SealStamp`.
- Exact references appear in `Medieval_Culture_Catalogue.md`.
