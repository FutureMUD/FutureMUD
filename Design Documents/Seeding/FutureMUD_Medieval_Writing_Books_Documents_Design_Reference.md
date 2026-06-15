# FutureMUD Medieval Writing Implements, Books, and Documents Seeder Design Reference

This document defines the research, scope, component strategy, grouping model, and item catalogue for the medieval writing-implements, books, documents, and scribal-goods package of the FutureMUD Item Seeder.

It includes the accepted item catalogue with stable unique references, nouns, short descriptions, material, size, quality, weights, costs, components, and tags. Player-facing full descriptions are maintained in the companion standalone CSV file `FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv`, keyed by `unique_reference`.

## Executive summary

- This package covers **finished medieval literacy and document goods**: writing surfaces, books, loose documents, scrolls, wax tablets, palm-leaf and birch-bark documents, writing implements, inks and pigments, document containers, seals and authentication goods, bookmaking and document-production tools, and a narrow set of writing-specific support goods. The accepted catalogue contains **286 item prototype targets**.
- The chronological band is the existing medieval seeder band, **500AD to 1300AD**, interpreted through the same culture-family anchors used by the medieval clothing, household, and military references.
- This is a **region-light but material/form-sensitive package**. Most items should be shared prototypes. Regional variation matters primarily where the writing surface, document format, implement type, institutional use, or book-production method visibly changes.
- Western and northern European coverage should centre on parchment sheets, charters, rolls, codices, wax tablets, quills, styluses, seals, and monastic/administrative book production.
- Byzantine, Islamic Mediterranean, Near Eastern, and North African coverage should include parchment and paper codices, scrolls and chancery documents where useful, qalam/reed pens, calligraphy goods, ink and pigment tools, seals, document cases, and scholarly/religious manuscript forms.
- South Asian coverage should include palm-leaf manuscript bundles, wooden manuscript boards, styluses, inked palm/birch-bark manuscripts where regionally useful, temple/scholastic document goods, and paper where appropriate to the region and date.
- East Asian coverage should include paper sheets, handscrolls, folded or stitched books, brush-and-ink tools, inkstones, ink sticks, document wrappers, book boxes, and woodblock-printing tools.
- Rus / Novgorod coverage should include birch-bark letters and styluses alongside parchment books and Orthodox church manuscripts.
- Steppe and caravan contexts should mostly reuse shared parchment/paper/scroll/document-container goods, with separate entries only for clearly portable courier, seal, and storage forms.
- The catalogue should avoid duplicating furniture and generic containers that are already handled by the household-goods package. Only writing-specific containers and supports belong here by default.
- The package should be implemented as a **functional literacy package**, not just props. The current seed data provides medieval-specific component templates for sheet and scroll surfaces, codices and ledgers, non-paper writing surfaces, pre-modern writing implements, seal stamps, and writing-specific document containers.
- The catalogue should use those exact current component names by default rather than borrowing modern `Paper_A4`, `Paper_A5`, generic `Book_*`, `Biro`, `Pencil`, or `Crayon` prototypes.
- The current seed data includes both **seal tools** (`SealStamp_Medieval_*`) and **sealable target components** (`Sealable_*`) for tamper-evident documents, scrolls, rolls, letters, writs, envelopes, palm-leaf bundles, document pouches, archive boxes, and containers. Sealed-document rows should use these components directly where mechanical seal behaviour is desired.

## Scope and era model

### Chronological band

The design band is **500AD to 1300AD**. This should be treated as a family of period anchors rather than one universal year. A c. 800 Anglo-Saxon parchment gospel book, a c. 1050 Fatimid paper document, a c. 1100 Song brush-and-paper kit, a c. 1150 Goryeo woodblock-related object, and a c. 1250 High English legal roll can all be valid if each remains credible for its own context.

Do not include late medieval, Renaissance, or early-modern book and stationery forms merely because they are familiar. Objects primarily associated with post-1300 western book culture, mature European paper-book abundance, fifteenth-century print shops, movable metal type, modern stationery, graphite pencils, fountain pens, ballpoint pens, paperclips, staples, rubber erasers, and modern notebooks belong outside this branch unless a later fantasy or modern package explicitly approves them.

### Geographic coverage

Use the same broad medieval world slice as the completed medieval packages:

- Britain and Ireland
- Scandinavia and the North Sea
- Western, central, and southern Europe
- Iberia
- Byzantium
- The Levant
- Egypt and North Africa
- The Eurasian steppe
- Rus / Novgorod
- Northern and southern India
- China
- Korea
- Japan

### Historical inspiration families

The following labels are builder-facing organizational buckets, not public item wording:

- Early Anglo-Saxon
- Anglo-Danish
- Norse / Viking Age
- Norman
- Anglo-Norman
- High English / British
- Irish / Gaelic
- Scottish / Gaelic-Lowland
- Carolingian / Frankish
- Capetian French
- Holy Roman Empire / German
- Christian Iberian
- Andalusian
- Byzantine
- Abbasid
- Fatimid
- Seljuk / Ayyubid-Mamluk
- Magyar
- Rus / Novgorod
- Steppe Turkic / Mongol
- North Indian / Rajput
- South Indian / Chola
- Song China
- Goryeo Korea
- Heian / Kamakura Japan

### Resolution

This package creates **standard item prototypes**, not exhaustive manuscript-library taxonomy. The base catalogue should provide useful, behaviour-stable objects that are credible when unskinned. Skins and later local overrides should carry:

- exact titles
- religious texts
- legal formulas
- school or guild labels
- library shelf marks
- owner names
- household, monastery, shrine, temple, chancery, court, or clan marks
- heraldic signs
- exact calligraphic hands
- local script and language presentation
- tooling patterns
- painted decoration
- gilded initials
- seal designs
- lacquer colours
- cover ornamentation
- manuscript illumination style
- setting-specific names

Public base descriptions should describe visible form and construction rather than making claims about exact historical culture, real-world textual content, or a specific named manuscript.

### Exclusions

The following are outside this package by default:

- post-1300 western moveable-type print-shop equipment
- Renaissance or early-modern printing presses as standard medieval goods
- modern stationery, including graphite pencils, pencil sharpeners, fountain pens, ballpoint pens, felt-tip markers, paperclips, staples, rubber erasers, binders, clipboards, carbon paper, envelopes in modern postal form, and machine-made exercise books
- generic desks, bookcases, cupboards, stools, benches, lighting, tables, lecterns, church furnishings, and ordinary boxes already covered by the household package unless a writing-specific form or component changes the object
- exact content-specific holy books or literary works as base prototypes
- magical writing, cursed books, living books, animated pens, or fantasy script mechanics unless a later fantasy package approves them
- craft-only raw materials that are not meaningful player-facing goods unless the later crafting pass specifically needs them as stock
- unportable room architecture, library rooms, scriptorium rooms, archive rooms, and fixed building installations unless a later fixture-specific pass asks for them

## Shared-first branch architecture

### Default rule

Create shared prototypes wherever the object can plausibly serve multiple cultures with only presentation changes. Most quills, reed pens, styluses, charcoal sticks, ink cakes, inkwells, parchment sheets, wax tablets, document pouches, scroll ties, bookbinding needles, scraping knives, parchment pumice, book presses, and storage boxes should begin as shared prototypes.

### When to create a regional or culture-family item

Create a separate regional item when one or more of the following is true:

- The writing surface visibly differs, such as parchment, rag paper, palm leaf, birch bark, bamboo slips, clay, wax, slate, or wooden tablets.
- The document format visibly differs, such as codex, roll, charter roll, handscroll, folded book, stitched book, palm-leaf bundle, wax diptych, wax triptych, or birch-bark letter.
- The implement type differs mechanically or visibly, such as quill, reed qalam, calligraphy brush, stylus, chisel, charcoal, ink dauber, or woodblock-printing baren.
- The item should accept a different `WritingImplementType`, such as `Quill`, `ReedPen`, `Brush`, `Stylus`, `Charcoal`, or `Chisel`.
- The component differs, such as `Book`, `PaperSheet`, `InscribableSurface`, `ScribingImplement`, `SealStamp`, `Sealable`, `Container_*`, or `LockingContainer_*`.
- The item reflects a production technology with real gameplay grouping value, such as woodblock printing, parchmentmaking, papermaking, bookbinding, scrollmaking, or palm-leaf manuscript preparation.
- The item represents a distinct institutional use case, such as monastic codices, chancery rolls, tax ledgers, madrasa manuscripts, temple manuscripts, sutra scrolls, or trade account tablets.
- The primary material materially changes damage, craft, market, or visual identity.

### What skins should carry instead

Do not create new prototypes merely for:

- language
- script
- title
- author
- prayer text
- scripture excerpt
- poetry
- heraldic device
- monastic house mark
- owner label
- calligrapher's flourish
- exact ink colour
- exact leather dye
- decorative border
- gilded initial
- cover stamp
- library chain mark
- seal design if the seal component can store the design
- local fantasy name

Those belong in skins, writing content, seal metadata, or local world customisation unless they change behaviour, material, surface, capacity, size, or major silhouette.

## Culture coverage table

The table is builder-facing. Public item text should describe the object rather than naming a real-world culture unless the word is a useful object-form term.

| Inspiration family | Reference anchor | Coverage boundary | Writing/book-design focus |
|---|---:|---|---|
| Early Anglo-Saxon | c. 800AD | lowland England before sustained Anglo-Danish synthesis | parchment codices, Gospel books, prayer books, charters, wax tablets, quills, styluses, simple book satchels, and monastic writing tools |
| Anglo-Danish | c. 950AD | late Anglo-Saxon England with strong Scandinavian settlement influence | parchment documents and codices in church/court settings, wax tablets, quills, styluses, sealed writs, and limited North-Sea wooden or runic note objects as skins |
| Norse / Viking Age | c. 950AD | Scandinavia and diaspora settlements before the close of the Viking Age | wooden/bone writing sticks and incised marks as limited objects, imported or church-linked parchment books, wax tablets for accounting, and portable document cases rather than broad local book culture |
| Norman | c. 1066AD | ducal Normandy and conquest-generation Norman sphere | parchment charters, rolls, sealed writs, quills, wax tablets, household/account writing kits, and monastic codices |
| Anglo-Norman | c. 1150AD | post-Conquest England and Norman-influenced Britain | parchment rolls, legal documents, household accounts, codices, wax-tablet diptychs, quills, seals, document pouches, and archive boxes |
| High English / British | c. 1250AD | high-medieval England and Welsh March/British court-facing styles | charter rolls, account rolls, court and estate records, parchment codices, university/scholastic book forms, wax seals, quills, ruling boards, and bookbinding tools |
| Irish / Gaelic | c. 1100AD | Gaelic Ireland before strong late-medieval English tailoring influence | monastic parchment codices, small Gospel/prayer books, book satchels and protective cases, quills, ink goods, and learned-poet or legal manuscript objects handled mostly by skins |
| Scottish / Gaelic-Lowland | c. 1250AD | medieval Scotland across Gaelic and Lowland spheres | monastic and administrative parchment books, charters, sealed documents, wax tablets, quills, portable cases, and Lowland court/legal overlaps with British/French models |
| Carolingian / Frankish | c. 800AD | Frankish court and early medieval western Europe | parchment codices, capitularies, Gospel books, wax tablets, quills, ruling boards, pen knives, and monastic scriptoria goods |
| Capetian French | c. 1200AD | northern and central French high-medieval sphere | parchment codices, scholastic books, charter rolls, sealed documents, luxury illuminated leaves, bookbinding, quill preparation, and legal/theological manuscripts |
| Holy Roman Empire / German | c. 1200AD | German-speaking imperial and central European regions | parchment codices, monastic and urban documents, wax tablets, sealed charters, book boxes, ledgers, quills, and bookbinding or archive goods |
| Christian Iberian | c. 1200AD | Leonese, Castilian, Aragonese, Navarrese, and neighbouring Christian Iberian contexts | parchment and paper documents, legal/religious codices, interfaith scholarly manuscripts, quills and reed pens, wax seals, document boxes, and frontier chancery records |
| Andalusian | c. 1100AD | al-Andalus and western Islamic Iberia | paper and parchment codices, qalam/reed pens, ink and calligraphy goods, scholarly/religious manuscripts, legal documents, document wrappers, and fine bindings |
| Byzantine | c. 1000AD | middle Byzantine Constantinopolitan and provincial contexts | parchment and paper codices, liturgical books, wax tablets, official documents, quills/reed pens, ink goods, book covers, and monastic/private library materials |
| Abbasid | c. 850AD | Baghdad-centred early medieval Islamic urban and scholarly contexts | paper sheets, paper codices, qalam pens, ink cakes and inkwells, scholarly books, chancery documents, document folders, seal goods, and calligraphy tools |
| Fatimid | c. 1050AD | Fatimid Egypt and North African urban/textile contexts | paper and parchment manuscripts, administrative documents, qalam pens, ink goods, book wrappers, legal documents, document cases, and court/religious book forms |
| Seljuk / Ayyubid-Mamluk | c. 1200AD | Anatolia, Syria, Egypt, and crusading-era eastern Mediterranean | paper codices, scholarly and religious manuscripts, qalam and brush goods, calligraphy tools, bookbinding, document chests, sealed correspondence, and chancery forms |
| Magyar | c. 950AD | Carpathian Basin Magyar and steppe-to-central-Europe context | limited church/court parchment documents, portable courier containers, wax seals, imported or clerical codices, and shared stylus/quill objects where literate administration is represented |
| Rus / Novgorod | c. 1100AD | Kievan Rus and Novgorod-facing northern Slavic/Norse/Byzantine intersections | birch-bark letters, styluses, parchment religious codices, wax tablets, official seals, wooden writing tablets, and Orthodox church manuscript forms |
| Steppe Turkic / Mongol | c. 1200AD | Inner Eurasian mounted steppe before and during early Mongol expansion | portable document cases, courier scroll tubes, seal and tag goods, paper or parchment documents adopted through neighbouring literate systems, and compact administrative writing kits |
| North Indian / Rajput | c. 1100AD | north and north-western Indian contexts before heavy late Sultanate/Mughal tailoring | palm-leaf and birch-bark manuscript forms where regionally useful, wooden manuscript boards, styluses, inked paper documents in contact zones, temple/scholarly manuscript bundles, and wrapped book storage |
| South Indian / Chola | c. 1050AD | Chola and neighbouring south Indian contexts | palm-leaf manuscripts, styluses, manuscript cords, wooden covers, temple record bundles, copper-plate-adjacent document props if later approved, and wrapped manuscript storage |
| Song China | c. 1100AD | Northern and Southern Song literary, administrative, and printing baseline | paper sheets, handscrolls, stitched or folded books, writing brushes, inksticks, inkstones, document wrappers, book boxes, woodblock-printing tools, and official/scholarly document goods |
| Goryeo Korea | c. 1150AD | Goryeo-period Korean court, Buddhist, and scholarly baseline | paper books, handscrolls, brush-and-ink sets, woodblock-printing tools, Buddhist sutra storage, document wrappers, book boxes, and high-quality paper/manuscript goods |
| Heian / Kamakura Japan | c. 1200AD | late Heian to early Kamakura court, temple, and warrior contexts | paper scrolls, folded books, brush-and-ink goods, poetry and letter papers as skins, sutra-copying goods, document boxes, wrappers, inkstones, and portable writing sets |

## Religious and learned-institution coverage table

This table is builder-facing. The base catalogue should avoid hardcoding real scripture content. It can provide object forms that builders and skins later populate with local titles, language/script content, and religious or institutional motifs.

| Service or institution family | Catalogue treatment | Notes and limits |
|---|---|---|
| Latin Christian | Shared parchment codices, Gospel/prayer books, Psalter-like books, liturgical book forms, charter rolls, monastic writing kits, book satchels, quills, ink, ruling boards, and bookbinding tools. | Public text should describe codex size, cover, binding, parchment, ruling, and decoration. Avoid naming exact Bible books or saints in base prototypes unless the item is intentionally generic, such as `gospel book` or `psalter-like codex`. |
| Eastern Christian / Byzantine-rite | Parchment/paper codices, liturgical books, icon-adjacent book covers, monastic library documents, wax tablets, official documents, and book boxes. | Keep Byzantine, Rus, and frontier Orthodox variants mostly as skins unless the book form, cover, or support differs. |
| Islamic scholarly, legal, and devotional settings | Paper codices, loose paper documents, qalam pens, ink and calligraphy kits, Qur'an-appropriate book forms, legal documents, document wrappers, and bindings. | Islamicate literacy goods are not just religious goods. Include scholarly, chancery, legal, medical, mathematical, literary, and mercantile uses. Public base text should not quote scripture. |
| Jewish communities | Parchment scrolls, codices, document pouches, school tablets, quills/reed pens, ink goods, genizah-style storage boxes if not already household, and learned-book overlays. | Torah ark and synagogue furnishing containers belong to household/religious furniture; personal or portable document goods can live here. |
| Hindu temple and scholastic traditions | Palm-leaf manuscripts, wooden covers, manuscript cords, styluses, inked leaves, wrapped manuscript bundles, temple document bundles, and account/land-record forms. | Sect labels and exact text titles belong to skins. Copper-plate grants can be a later expansion if metal-inscription behaviour is desired. |
| Jain communities | Palm-leaf or paper manuscript bundles, manuscript covers, cloth wrappers, styluses, ink, and protective cases. | Public descriptions should avoid assuming a single sect or text. Rich manuscript covers can be skins over the same bundle form. |
| Buddhist traditions | Sutra scrolls, paper books, palm-leaf manuscript bundles, woodblock-printing tools, book boxes, scroll cases, wrappers, brushes, inkstones, and copyist kits. | East Asian Buddhist printing and copying deserve real prototype coverage where the object form changes. Content remains in skins or writing. |
| Daoist traditions | Paper scrolls, registers, talisman-paper goods, brush-and-ink sets, folded books, wrappers, and document boxes. | Talismanic or magical effects are out of scope; visual written-paper forms are in scope. |
| Shinto shrine contexts | Paper records, folded ritual papers, brush-and-ink goods, document boxes, and shrine register forms. | Modern standardized shrine stationery is outside scope; keep to conservative court/shrine record goods. |
| Secular administration and commerce | Charters, deeds, rolls, account tablets, ledgers, tax records, toll rolls, tally-linked document cases, seal boxes, office inkwells, copyist boards, and courier document tubes. | This is a major reason for the package. Do not over-religionize literacy goods. |
| Schools, universities, madrasas, monasteries, and scholarly households | Wax tablets, reusable boards, loose sheets, quires, book satchels, writing kits, ruling boards, exemplar books, commentary-ready codices, ink, and pen knives. | Exact curriculum, scholastic text names, and language should be skins or written content. |

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` seeder call shape during implementation.
- Recommended unique-reference prefix for this branch: `medieval_writing_`.
- `uniqueReference` values use lowercase snake case ASCII and should remain stable once accepted.
- `noun` is compact and singular, usually the object type: `sheet`, `scroll`, `roll`, `codex`, `book`, `tablet`, `stylus`, `quill`, `pen`, `brush`, `inkwell`, `inkstone`, `case`, `pouch`, `box`, `knife`, `press`, `frame`, or `seal`.
- `sdesc` is player-facing, concise, usually begins with `a`, `an`, or `a pair of`, and should not end with a full stop.
- `ldesc` should normally be `null` for ordinary portable goods. Use a room-contents sentence only for deliberately installed boards, archive fittings, or fixtures.
- `fdesc` should be player-facing in-world prose: visible material, form, construction, folding, binding, ruling, stitching, covers, clasps, ties, cases, stains, wax, seals, prepared surface, tool edge, bristles, nib, writing point, or storage affordances.
- `material` must be an exact seeded solid material. Liquids and gases are not substitutes for the primary material argument.
- `tags` must be exact seeded hierarchical tag paths.
- `components` must be exact seeded component prototype names.
- `inherentCost` is denominated in farthings. Use whole-farthing values unless a fractional farthing is deliberately intended.
- Finished writing goods should normally be skinnable and player-visible.
- Ordinary portable goods include `Holdable` unless a component or fixture use-case specifically makes them non-portable.
- No ordinary item in this branch should use a morph target, morph emote, morph timer, long description, hidden-player flag, or non-skinnable flag unless a later implementation explicitly marks a special exception.

## Component and material support strategy

### Current support posture

The current seed data contains a usable medieval writing support layer. The catalogue can author functional rows directly against medieval-specific component prototype names for most object families instead of using modern paper sizes, generic book components, or prose-only writing props.

No large runtime system is required for this package. The intended runtime model is:

- `PaperSheet` for writable/readable loose sheet, folded sheet, letter, roll, scroll, palm-leaf strip, and similar single-surface document forms.
- `Book` for multi-page or multi-leaf objects where page count and page turning matter.
- `InscribableSurface` for non-paper surfaces where allowed implement type matters, such as wax tablets, slate tablets, wooden tablets, birch bark, bamboo slips, ostraca, and practice boards.
- `ScribingImplement` for pre-modern writing implements with implement family, writing colour, and finite or non-consuming use counts.
- `SealStamp` for signets, seal matrices, and office seals that carry seal-stamp behaviour.
- `Sealable` for documents, scrolls, envelopes, document pouches, archive boxes, and containers that should record seal state and tamper evidence.
- ordinary `Container_*`, `LockingContainer_*`, `Destroyable_*`, `Stack_Number`, `Wear_*`, and `Beltable` components for storage, portability, damage, stacking, and wearing behaviour.

### Seeded component templates to use

The following component templates are present in the current seeded component inventory and should be treated as available implementation vocabulary for the catalogue.

#### Document containers and writing supports

Use these when the object is a writing-specific container, archive container, scroll case, seal-tool box, bookcase surface, or writing desk surface rather than a generic household item:

- `Container_Archive_Box`
- `Container_Archive_Chest`
- `Container_Document_Bookcase_Shelves`
- `Container_Document_Pouch`
- `Container_Document_Satchel`
- `Container_Scroll_Tube`
- `Container_Seal_Box`
- `Container_Writing_Desk_Drawers`
- `Container_Writing_Desk_Surface`

These components do not require a new document-container runtime family. They are still ordinary container components, so apply the normal dry-container exclusivity rule: do not stack them with another `Container_*`, `LockingContainer_*`, `Book`, `Door`, or liquid-container behaviour unless the engine explicitly supports that hybrid.

#### `PaperSheet`-family surface templates

Use these for single-surface writeable/readable documents, sheets, rolls, and scroll-like forms:

- `Medieval_East_Asian_Paper_Scroll_Surface`
- `Medieval_East_Asian_Paper_Sheet_Surface`
- `Medieval_Palm_Leaf_Manuscript_Surface`
- `Medieval_Papyrus_Scroll_Surface`
- `Medieval_Papyrus_Sheet_Surface`
- `Medieval_Parchment_Bifolium_Surface`
- `Medieval_Parchment_Roll_Surface`
- `Medieval_Parchment_Sheet_Surface`
- `Medieval_Rag_Paper_Letter_Surface`
- `Medieval_Rag_Paper_Scroll_Surface`
- `Medieval_Rag_Paper_Sheet_Surface`

Do not use `Paper_A3`, `Paper_A4`, or `Paper_A5` for default medieval rows unless a world intentionally wants modern-size paper mechanics in a medieval-looking object. The medieval-specific names make the row intent clearer and avoid modern A-size leakage.

#### `Book`-family templates

Use these for multi-page, multi-leaf, or bound objects that should use book/page behaviour:

- `Medieval_Account_Ledger_90_Page`
- `Medieval_East_Asian_Stitched_Book`
- `Medieval_Palm_Leaf_Manuscript_Bundle`
- `Medieval_Parchment_Codex_20_Page`
- `Medieval_Parchment_Codex_40_Page`
- `Medieval_Parchment_Codex_90_Page`
- `Medieval_Rag_Paper_Codex_40_Page`

Do not stack `Book` with dry-container components. A codex, ledger, stitched book, or palm-leaf manuscript bundle is a read/write object, not a storage box, unless a later engine feature explicitly supports book-as-container behaviour.

#### `InscribableSurface` templates

Use these where a surface should accept only appropriate writing or incising implements:

- `Medieval_Bamboo_Slip_Surface`
- `Medieval_Birch_Bark_Surface`
- `Medieval_Ostracon_Surface`
- `Medieval_Practice_Board_Surface`
- `Medieval_Slate_Tablet_Surface`
- `Medieval_Wax_Diptych_Surface`
- `Medieval_Wax_Tablet_Surface`
- `Medieval_Wax_Triptych_Surface`
- `Medieval_Wooden_Tablet_Surface`

Default pairing policy:

- Wax tablet, diptych, triptych: stylus objects.
- Slate tablet: stylus, chisel, or other incising implement according to configured component policy.
- Wooden tablet and practice board: stylus, chisel, charcoal, brush, reed pen, or quill only where the seeded component permits those implement families.
- Birch bark: stylus/incising and ink use according to seeded component configuration.
- Bamboo slip: brush and stylus use according to seeded component configuration.
- Ostracon: reed pen, quill, brush, or charcoal style use according to seeded component configuration.

Descriptions must not promise an implement works on a surface unless the component actually accepts that implement family.

#### `ScribingImplement` templates

Use these for ordinary pre-modern writing and incising implements:

- `Medieval_Bone_Stylus`
- `Medieval_Bronze_Stylus`
- `Medieval_Calligraphy_Brush`
- `Medieval_Charcoal_Stick`
- `Medieval_East_Asian_Writing_Brush`
- `Medieval_Fine_Quill_Pen`
- `Medieval_Iron_Stylus`
- `Medieval_Qalam`
- `Medieval_Quill_Pen`
- `Medieval_Reed_Pen`
- `Medieval_Reed_Stylus`
- `Medieval_Scribing_Chisel`

Use-count policy:

- Quills, fine quills, reed pens, qalams, brushes, and charcoal sticks should be finite-use writing implements.
- Bone, bronze, iron, and reed styluses should normally be non-consuming.
- Scribing chisels can be non-consuming or high-use depending on the component definition, but public item descriptions should present them as durable incising tools rather than ink-bearing pens.
- Richer versions should differ by item quality, material, cost, and description rather than by creating unnecessary duplicate component templates.

Do not use `Biro`, `Pencil`, `Crayon`, or `PencilSharpener` components for ordinary 500-1300 medieval rows.

#### `SealStamp` templates

Use these for seal tools:

- `SealStamp_Medieval_BrassOfficeSeal`
- `SealStamp_Medieval_BronzeSignet`
- `SealStamp_Medieval_IronSealMatrix`
- `SealStamp_Medieval_LeadSealMatrix`

These templates support seal stamps, signets, and matrices as active authority tools. They should be attached to the object that applies the seal, not to the document receiving the seal.

#### `Sealable` templates

Use these for target objects that should accept a seal and retain tamper-evidence state.

Shared or cross-period target templates:

- `Sealable_Document_Wax`
- `Sealable_Document_Clay`
- `Sealable_Envelope`
- `Sealable_Scroll`
- `Sealable_Container_Wax`

Medieval-specific target templates:

- `Sealable_Medieval_Parchment_Charter`
- `Sealable_Medieval_Parchment_Roll`
- `Sealable_Medieval_Rag_Paper_Letter`
- `Sealable_Medieval_Official_Writ`
- `Sealable_Medieval_East_Asian_Scroll`
- `Sealable_Medieval_Palm_Leaf_Bundle`
- `Sealable_Medieval_Document_Pouch`
- `Sealable_Medieval_Archive_Box`

Use the most specific medieval target template where it fits the item form. Use the shared `Sealable_*` templates for generic envelopes, scrolls, wax-sealed documents, clay-sealed documents, or wax-sealed containers that are not covered by a medieval-specific name. Do not use antiquity-specific or modern-specific sealable templates in this medieval package unless a later cross-era item explicitly calls for them.

A sealable target can coexist with writable/readable document components or container components when their exclusive interfaces do not conflict. A charter can therefore pair a `PaperSheet` surface with a `Sealable_Medieval_Parchment_Charter`; a document pouch can pair an appropriate document-container component with `Sealable_Medieval_Document_Pouch`; an archive box can pair an archive container component with `Sealable_Medieval_Archive_Box`. Do not stack multiple `Sealable_*` components on the same item.

Rows with a `Sealable_*` component can truthfully be described as sealable and tamper-evident. If an item should spawn already sealed with a specific impression, confirm whether the implementation can seed initial component-instance state for that exact item. Otherwise, author the row as a sealable blank that becomes sealed through normal gameplay with a `SealStamp` tool and a permitted medium.

### Component boundaries and future component work

The current component vocabulary is sufficient for this catalogue. No new writing, book, pre-modern writing-implement, seal-stamp, seal-target, or document-container component family is required before implementation.

No new component family is recommended for inks or pigment vessels in this pass. Ink cakes, ink sticks, soot cakes, gall-ink ingredients, inkwells, pigment shells, inkstones, and pigment mullers can be ordinary items, dry containers, liquid containers only if an exact liquid-ink system exists, or future craft prerequisites. Writing colour and use-count behaviour belongs to the `ScribingImplement` component.

The `Board` component type remains outside the default row set unless the world has an actual configured board record and permissions. Most notice boards, roster boards, school boards, or decree boards in this package should be inert surfaces or `InscribableSurface` items.

### Material support

The current solid material inventory has enough coverage for this catalogue. Use exact material names, not invented synonyms.

Recommended primary-material mapping:

| Object family | Preferred exact material names |
|---|---|
| Parchment sheets, bifolia, rolls, codices | `parchment` |
| Rag-paper sheets, letters, paper scrolls, East Asian paper goods | `paper` |
| Papyrus sheets and scrolls | `papyrus` |
| Palm-leaf manuscript strips and bundles | `leaf` unless a more specific palm-leaf material is added |
| Bamboo slips, brush handles, East Asian document goods | `bamboo` where it is the dominant substance |
| Birch-bark letters and bark documents | `birch` unless a more specific bark material is added |
| Wax tablets and wax seal cakes | `beeswax` or `wax` if an exact broader wax material is preferred by the implementation |
| Slate tablets | `slate` |
| Clay tablets, tablet envelopes, ceramic inkwells | `clay`, `fired clay`, `earthenware`, or `ceramic` according to visible form |
| Quills | `feather` |
| Reed pens, reed styluses, qalams | `reed` |
| Charcoal sticks and black pigment stock | `charcoal` or `soot` |
| Styluses, chisels, seal matrices, signets | `bone`, `bronze`, `brass`, `lead`, `wrought iron`, or another exact seeded metal/animal product as appropriate |
| Pouches, satchels, wraps, ties, covers | `leather`, `linen`, `cotton`, `silk`, or `hemp` according to the visible dominant textile |
| Scroll tubes, archive boxes, book boards, desks, presses | `cedar`, `oak`, `cypress`, `bamboo`, `teak`, `wood`, or another exact seeded wood according to region and item form |
| Pigment shells, burnishing shells, pumice tools | `shell` or `pumice` |

If a more specific material is missing, use the nearest exact seeded material and keep the precise visual detail in `fdesc`. Examples: use `paper` for rag-paper objects, `leaf` for palm-leaf manuscripts, and `birch` for birch-bark documents unless dedicated `rag paper`, `palm leaf`, or `birch bark` materials are added later.

## Implementation rules by object family

### Writing surfaces and documents

- Writable objects should carry exactly one appropriate writing-surface component: `PaperSheet`, `Book`, or `InscribableSurface`.
- Ordinary loose sheets should normally use `Holdable`, `Destroyable_Paper`, and a sheet-surface component.
- Ordinary writable loose sheets should not be stackable if the engine cannot write on individual members of a stack. Use separate non-writeable stack items for bundles of blank stock.
- Scrolls can use `PaperSheet`-style components if they behave as long single writing surfaces. Do not combine scrolls with dry-container components unless a future engine model supports scrolls containing contents.
- Codices and ledgers should use `Book`-family components, not generic `Container_*`.
- Wax tablets, wooden tablets, birch-bark strips, bamboo slips, slate tablets, and ostraca should use the seeded medieval `InscribableSurface` templates. Dedicated medieval clay-tablet behaviour is not part of the current component list; use ordinary clay document props or an antiquity clay-tablet surface only if the implementation deliberately imports that component.
- Sealed variants should add an appropriate `Sealable_*` component only where seal behaviour should exist. A tied but non-mechanical scroll can remain unsealed mechanically and simply describe a tie.
- Do not include actual readable text in public `fdesc` unless using a writing block or the item’s readable component. A base item can be blank, ruled, lined, folded, sealed, stained, smudged, or labelled without embedding content.

### Books and bound manuscripts

- Use `Book` components for player-writeable/readable multi-page objects.
- Primary material should usually be `parchment`, `paper`, `leaf`, `papyrus`, `leather`, `wood`, or `bamboo`, depending on the dominant body. For a parchment codex with leather covers, use `parchment` if the page body is the intended primary substance; use `leather` only for cover-dominant book cases or protective covers.
- Do not stack `Book` with `Container_*`, `LockingContainer_*`, or another openable/lockable component.
- Large liturgical books, huge choir books, and display codices can be `Normal` or `Large` and high cost, but should remain portable prototypes unless explicitly implemented as fixtures.
- Public descriptions should mention folded quires, sewn leaves, boards, covers, clasps, ties, tooling, ruling, margins, and visible wear rather than naming exact texts.
- Exact text contents should be inserted by dynamic writing, fixed writing blocks, or skins.

### Writing implements

- Use `ScribingImplement` for medieval implements when available.
- Do not use `Biro`, `Pencil`, `Crayon`, or `PencilSharpener` for ordinary medieval rows.
- Ordinary quills, reed pens, qalams, brushes, charcoal sticks, styluses, and chisels include `Holdable` and one destroyable component.
- Material should be the dominant visible material: `feather` for quills, `reed` or `papyrus` for reed pens if both are accepted, `wood` or `bamboo` for brushes, `charcoal` for charcoal sticks, `bone`, `bronze`, `wrought iron`, or `ivory` for styluses.
- Use `Good` quality for fine calligraphy brushes, elite seal-handled styluses, or elaborate qalam cases only when the object is visibly better made.
- Do not describe a pen as carrying infinite ink unless the component has a use count or the object is deliberately a non-consuming stylus.

### Ink, paint, pigment, and preparation goods

- Inks, pigment cakes, inkstones, inkwells, ink pots, pigment shells, mullers, pounce bags, pumice, chalk, and sizing brushes belong in this package.
- A plain inkwell can be an inert prop with `Holdable` and `Destroyable_Misc` or `Destroyable_Glassware`.
- A liquid-container inkwell should only be used if an exact seeded liquid such as `ink` exists and the item is intended to hold pourable liquid. If no such liquid exists, prefer an inert inkwell or pigment/ink-cake item.
- Primary material for ink cakes can be `soot`, `charcoal`, `cinnabar`, `malachite`, `azurite`, or another exact pigment-like seeded material.
- Inkstones should use `stone`, `slate`, `jade`, or another exact stone material.
- Paint and illumination tools can include pigment shells, pigment mullers, pen rests, ruling boards, and manuscript prickers.
- Do not hardcode exact pigment recipes unless the material and gameplay systems support them.

### Bookmaking, scrollmaking, and manuscript-production tools

- Include tools needed by later crafts if they are visible finished goods.
- Parchmentmaking tools include lunellum/scraping knives, stretching frames, lacing cords, pegs, pumice, whitening chalk, pounce bags, and fleshing beams.
- Papermaking tools include mould-and-deckle, vats, couching blankets, press felts, lay presses, sizing brushes, rag-sorting knives, and rag-beating troughs. Exclude post-period machinery such as Hollander beaters from this medieval pass.
- Bookbinding tools include bookbinder's needles, punches, sewing frames, support cords, backing hammers, leather paring knives, book presses, and lying presses.
- Scrollmaking tools include scroll roller rods, end knobs, ties, seal cords, label tabs, smoothing stones, and scroll cases.
- Calligraphy tools include pen knives, qalam cutters, pen rests, pen racks, pen wipers, ruling boards, manuscript prickers, and fine brushes.
- Woodblock-printing tools include block carving knives, clearing chisels, ink daubers, paper dampening brushes, impression spoons, printing barens, paste pots, and registration pins. This is primarily East Asian and Buddhist/court/scholarly coverage within this date band.

### Document containers and storage

- Use ordinary container components for document pouches, wrappers, cases, satchels, tubes, boxes, and archive chests.
- Use `Container_Scroll_Tube` for narrow scroll tubes and cylindrical document cases.
- Use `Container_Document_Pouch` for small leather or textile document pouches and tablet wraps.
- Use `Container_Document_Satchel` for larger document bags and satchels if the item is hand-carried.
- Use `Container_Archive_Box` or `Container_Archive_Chest` for non-locking archive storage where their capacities fit. Use `Container_Footlocker` or `Container_Trunk` only when a generic household profile is a better mechanical fit.
- Use `LockingContainer_Lockbox`, `LockingContainer_Footlocker`, or `LockingContainer_SafeChest` for built-in-lock document and archive storage.
- Do not combine built-in locking containers with loose locks, latches, keyrings, or ordinary container components.
- Public descriptions may mention straps, flaps, cords, caps, inner sleeves, wax staining, labels, and reinforced corners, but should not promise hidden storage or lock behaviour without components.

### Seals, authentication, and document security

- Use `SealStamp_Medieval_*` components for seal matrices, signet stamps, and office seals. The stamp or matrix is the tool that applies an authority impression.
- Use `Sealable_*` components for the document, pouch, scroll, envelope, bundle, box, or container that receives the seal and should retain tamper-evidence state.
- Use medieval-specific target templates where possible: `Sealable_Medieval_Parchment_Charter`, `Sealable_Medieval_Parchment_Roll`, `Sealable_Medieval_Rag_Paper_Letter`, `Sealable_Medieval_Official_Writ`, `Sealable_Medieval_East_Asian_Scroll`, `Sealable_Medieval_Palm_Leaf_Bundle`, `Sealable_Medieval_Document_Pouch`, and `Sealable_Medieval_Archive_Box`.
- Use shared target templates for general cases: `Sealable_Document_Wax`, `Sealable_Document_Clay`, `Sealable_Envelope`, `Sealable_Scroll`, and `Sealable_Container_Wax`.
- Add exactly one `Sealable_*` component to a target item when mechanical seal behaviour is desired. If the item is only visually tied, corded, marked, wax-stained, or prepared for sealing but should not track tampering, omit `Sealable_*`.
- Sealable writeable targets may break seals when read, written on, drawn on, graffitied, retitled, or opened, according to the current literacy and item-open hooks. Descriptions may therefore call these objects tamper-evident when the appropriate component is present.
- Do not include a functional signet ring here if jewellery/wearable ring behaviour is expected and not yet defined. A loose seal stamp can stand in for the functional object; a wearable signet ring can be handled by an accessory pass or a later hybrid.
- Use `beeswax`, `clay`, `lead`, `bronze`, `brass`, `wrought iron`, `stone`, or `ivory` as exact primary materials as appropriate.
- Do not promise that a seal authenticates a specific office, clan, or owner unless the stamp metadata, item skin, or written content supports that presentation.

### Boards and public notices

- The `Board` component type is for actual player-facing message boards and should only be used when the world has a configured board record and appropriate permissions.
- Most medieval notice boards, law boards, shop posting boards, school boards, wax roster boards, and temple decree boards should instead use `InscribableSurface` or remain inert props.
- Do not create public posting boards that imply persistent player posting unless the `Board` component is deliberately configured.

### Furniture overlap

The household package already covers furniture, shelves, cupboards, desks, lecterns, lights, and religious furnishings. This writing package should only include furniture-like objects when the writing use is specific enough to justify it:

- portable writing desk
- scribe's sloped writing board
- lap desk
- account board
- book press
- bookbinder's sewing frame
- parchment stretching frame
- papermaking frame
- small manuscript stand
- portable book rest
- scroll rack if it is a document-specific container and not a generic shelf

Otherwise, reuse household goods.

## Tags

Use exact seeded tags wherever possible. The following tag families are especially relevant.

### Writing-surface and writing-goods tags

- `Functions / Writing Surface`
- `Functions / Writing Surface / Loose Sheet`
- `Functions / Writing Surface / Scroll`
- `Functions / Writing Surface / Codex`
- `Functions / Writing Surface / Wax Tablet`
- `Functions / Writing Surface / Clay Tablet`
- `Functions / Writing Surface / Wooden Writing Block`
- `Functions / Writing Surface / Ostracon`
- `Functions / Writing Goods`
- `Materials / Writing Product`

### Writing-material market tags

- `Market / Writing Materials`
- `Market / Writing Materials / Ink`
- `Market / Writing Materials / Papyrus`
- `Market / Writing Materials / Paper`
- `Market / Writing Materials / Parchment`
- `Market / Writing Materials / Wax Tablets`
- `Market / Writing Materials / Clay Tablets`
- `Market / Writing Materials / Writing Implements`
- `Market / Writing Materials / Document Containers`
- `Market / Writing Materials / Scrolls`
- `Market / Writing Materials / Codices`

### Scribing, calligraphy, and document-preparation tool tags

- `Functions / Tools / Scribing Tools`
- `Functions / Tools / Scribing Tools / Ink Brush`
- `Functions / Tools / Scribing Tools / Inkwell`
- `Functions / Tools / Scribing Tools / Charcoal Stick`
- `Functions / Tools / Scribing Tools / Pen Knife`
- `Functions / Tools / Scribing Tools / Quill Pen`
- `Functions / Tools / Scribing Tools / Reed Pen`
- `Functions / Tools / Scribing Tools / Scraper Knife`
- `Functions / Tools / Scribing Tools / Seal Stamp`
- `Functions / Tools / Scribing Tools / Stylus`
- `Functions / Tools / Scribing Tools / Wax Spatula`
- `Functions / Tools / Calligraphy Tools`
- `Functions / Tools / Calligraphy Tools / Calligrapher's Brush`
- `Functions / Tools / Calligraphy Tools / Manuscript Pricker`
- `Functions / Tools / Calligraphy Tools / Pen Rack`
- `Functions / Tools / Calligraphy Tools / Pen Rest`
- `Functions / Tools / Calligraphy Tools / Pen Wiper`
- `Functions / Tools / Calligraphy Tools / Qalam Cutter`
- `Functions / Tools / Calligraphy Tools / Quill Curing Sand`
- `Functions / Tools / Calligraphy Tools / Ruling Board`

### Craft-stock and production-tool tags

- `Functions / Material Functions / Writing Craft Stock`
- `Functions / Material Functions / Writing Craft Stock / Papyrus Sheet Stock`
- `Functions / Material Functions / Writing Craft Stock / Parchment Sheet Stock`
- `Functions / Material Functions / Writing Craft Stock / Tablet Blank`
- `Functions / Material Functions / Writing Craft Stock / Waxed Tablet Board`
- `Functions / Material Functions / Writing Craft Stock / Ink Stock`
- `Functions / Material Functions / Writing Craft Stock / Pen Blank`
- `Functions / Material Functions / Writing Craft Stock / Bookbinding Stock`
- `Functions / Material Functions / Writing Craft Stock / Scrollmaking Stock`
- `Functions / Tools / Papyrusmaking Tools`
- `Functions / Tools / Parchmentmaking Tools`
- `Functions / Tools / Papermaking Tools`
- `Functions / Tools / Bookbinding Tools`
- `Functions / Tools / Scrollmaking Tools`
- `Functions / Tools / Woodblock Printing Tools`

### Container and household tags

Use these only where behaviour warrants them:

- `Functions / Container`
- `Functions / Container / Open Container`
- `Functions / Container / Porous Container`
- `Functions / Household Items / Household Wares`
- `Functions / Household Items / Household Furniture`
- `Market / Household Goods / Standard Wares`
- `Market / Household Goods / Luxury Wares`
- `Market / Professional Tools / Standard Tools`

## Materials

Use exact seeded solid material names. The most relevant confirmed materials include:

- sheet and manuscript materials: `paper`, `parchment`, `papyrus`, `leaf`, `bamboo`, `birch`, `wood`, `linen`, `silk`, `cotton`, `hemp`
- tablet and inscription materials: `beeswax`, `clay`, `fired clay`, `ceramic`, `earthenware`, `terracotta`, `slate`, `stone`, `wood`, `bamboo`
- implement materials: `feather`, `reed`, `papyrus`, `wood`, `bamboo`, `bone`, `ivory`, `horn`, `bronze`, `brass`, `wrought iron`, `charcoal`
- ink, pigment, and preparation materials: `soot`, `charcoal`, `cinnabar`, `malachite`, `azurite`, `gold`, `silver`, `lead`, `copper`, `tin`, `shell`, `pumice`, `sand`, `agate`, `stone`, `jade`, `glass`
- containers and covers: `leather`, `linen`, `silk`, `cotton`, `hemp`, `wood`, `cedar`, `cypress`, `pine`, `bamboo`, `lacquer`, `brass`, `bronze`, `wrought iron`

Material notes:

- Use `parchment`, not unseeded `vellum`, unless a separate material addition is approved.
- Use `leaf` for palm-leaf manuscripts unless a more specific seeded palm-leaf material is added before implementation.
- Use `paper` for rag-paper goods; do not invent `rag paper` unless it is added as a seeded material.
- Use `reed` for reed pens where possible; if the final material list in the implementation environment lacks `reed`, use `papyrus` only for actual papyrus-reed objects.
- Use `soot` or `charcoal` for dry carbon ink cakes and ink sticks where the vessel itself is not dominant.
- Primary material is the dominant gameplay abstraction, not every decorative detail. A leather-covered parchment codex can still use `parchment`; a cedar scroll case with a bronze cap can still use `cedar`.

## Sizes, quality, weight, and cost assumptions

### Size

- `Tiny`: loose sheets, small tags, seal cakes, styluses, quills, reed pens, charcoal sticks, small ink cakes, small seal stamps, scroll ties.
- `VerySmall`: folded letters, bifolia, small wax tablets, small inkwells, pen knives, pigment shells, pounce bags, manuscript prickers.
- `Small`: ordinary codices, scrolls, manuscript bundles, tablet diptychs, document pouches, small book boxes, inkstones, writing kits.
- `Normal`: large codices, wax triptychs, archive boxes, satchels, portable writing desks, ruling boards, small presses, stretching frames.
- `Large`: book presses, parchment stretching frames, large archive chests, major liturgical books, papermaking vats or frames, larger document furniture.
- `VeryLarge` and above should be rare in this pass and usually indicate a fixture better handled separately.

### Quality

- `Standard`: ordinary functional writing goods.
- `Good`: fine calligraphy implements, well-bound codices, good parchment, sturdy document cases, lockable record boxes, polished inkstones, quality bookbinding tools.
- `VeryGood` or higher: rare elite illuminated codices, luxury covers, court presentation books, jewelled bindings, exceptional seal matrices, or luxury manuscript containers.
- `Substandard` or below: only for deliberately damaged, rough, improvised, practice, used, or degraded items.

### Cost

Costs should be farthing-denominated relative seeder baselines rather than market-history claims. Use broad tiers:

- very low: charcoal sticks, blank wax scraps, simple ties, rough practice surfaces
- low: loose sheets, ordinary quills, simple styluses, ink cakes, small inkwells
- moderate: scrolls, wax tablets, parchment sheets, document pouches, simple codices, bookmaking tools
- high: parchment codices, lockable document boxes, fine inkstones, fine brushes, book presses, archive chests
- elite: illuminated codices, luxury covers, jewelled book cases, official seal matrices, high-status religious or court manuscripts

## Skin strategy

All finished goods in this package should normally be skinnable.

Skins should carry high-variance presentation:

- exact title
- language and script presentation
- owner names
- scribe marks
- library marks
- monastery, guild, school, temple, shrine, household, clan, or office marks
- heraldic badges
- calligraphic style
- coloured initials
- marginal ornament
- illumination style
- leather dye
- cover tooling
- lacquer pattern
- silk wrapper colour
- seal designs
- local religious motifs
- court or chancery marks
- fantasy-culture naming

Skins should not change behaviour. A skinned parchment codex remains a book component with the same page capacity. A skinned wax tablet remains an inscribable surface with the same allowed implement type. A skinned document case remains the same container profile. A skinned seal stamp should not imply different authority unless the seal component metadata or gameworld rules support it.

Default unskinned items still need to be credible. Skinability must not be used as an excuse for a bland or out-of-world base description.

## Public text and writing-content policy

- Public `noun`, `sdesc`, and `fdesc` should not mention skins, builders, seeder mechanics, component names, historical uncertainty, or later customization.
- Public text may say that an item is blank, ruled, folded, sewn, tied, sealed, pricked, scraped, polished, ink-stained, wax-filled, wrapped, or prepared for writing.
- Public text should not claim readable words are present unless those words are represented by a writing component or a writing block.
- Fixed labels, stamps, and inscriptions can use writing blocks when a specific text should be readable only by characters with the right language/script/skill.
- Player-authored or dynamic content belongs to `IReadable` and `IWriteable` components.
- Religious books should describe physical format, cover, writing surface, and decoration rather than quoting scripture.
- Legal and administrative documents should describe seals, folds, format, rulings, countersigns, or storage marks rather than embedding real legal text.
- Do not force modern academic culture labels into public descriptions. Use form-based terms: `codex`, `scroll`, `roll`, `quire`, `bifolium`, `wax tablet`, `diptych`, `triptych`, `palm-leaf manuscript`, `birch-bark letter`, `handscroll`, `stitched book`, `inkstone`, `qalam`, `quill`, `stylus`, `seal matrix`, `scroll case`, `book box`, or comparable visible-object language.

## Recommended catalogue grouping

The item catalogue uses these category headings.

### 1. Writing surfaces and loose documents

Include:

- parchment sheets
- parchment bifolia
- parchment quires
- parchment letters
- rag-paper sheets
- paper letters
- papyrus sheets and scrolls where still useful in Mediterranean/North African contexts
- birch-bark letters
- palm-leaf leaves
- bamboo slips
- wooden writing blocks
- ostraca
- slate tablets
- clay tablets where culturally appropriate or as conservative inherited forms
- blank labels and tags
- practice sheets and practice boards

### 2. Scrolls, rolls, books, and manuscript forms

Include:

- parchment scrolls and charter rolls
- paper scrolls
- handscrolls
- small codices
- large codices
- ledgers
- account books
- prayer-book-like codices
- liturgical-book-like codices
- scholastic codices
- East Asian folded books
- East Asian stitched books
- palm-leaf manuscript bundles
- wrapped manuscript bundles
- woodblock-printed book forms if supported by item presentation

### 3. Tablets and reusable writing surfaces

Include:

- single wax tablets
- wax diptychs
- wax triptychs
- wooden practice tablets
- slate tablets
- clay tablets
- tablet envelopes
- reusable account boards
- student practice boards

### 4. Writing implements

Include:

- ordinary quills
- fine quills
- reed pens
- qalams
- writing brushes
- fine calligraphy brushes
- charcoal sticks
- bone styluses
- bronze styluses
- iron styluses
- reed styluses
- chisels or inscription points
- pen knives
- qalam cutters
- pen wipers
- pen rests and racks

### 5. Ink, pigment, and illumination goods

Include:

- soot-black ink cakes
- ink sticks
- red pigment cakes
- mineral pigment cakes
- inkstones
- inkwells
- ink pots
- pigment shells
- pigment mullers
- pounce bags
- pumice
- chalk
- burnishing stones
- ruling boards
- manuscript prickers
- small palettes
- gold or silver leaf/pigment props if material support is clear

### 6. Document containers and protection

Include:

- scroll ties
- scroll cords
- label tabs
- scroll cases
- document pouches
- tablet wraps
- book satchels
- manuscript wrappers
- book boxes
- lacquered document boxes
- archive boxes
- deed boxes
- seal boxes
- messenger tubes
- courier document cases
- lockable document chests
- lockable archive chests

### 7. Seals, authentication, and secured documents

Include:

- seal matrices
- seal stamps
- official seal boxes
- wax seal cakes
- clay sealing lumps
- lead bullae or seal tags where appropriate
- sealed scroll variants
- sealed letter variants
- sealed document packets
- clay tablet envelopes
- wax-sealed container variants

### 8. Bookmaking, parchmentmaking, papermaking, and scrollmaking tools

Include:

- parchment scraping knives
- lunellum-like scrapers
- parchment stretching frames
- parchment lacing cords
- parchment pegs
- parchment pumice
- pounce bags
- whitening chalk
- papermaker's mould and deckle
- papermaker's vat
- couching blankets
- press felts
- lay presses
- rag sorting knives
- rag beating troughs
- sizing brushes
- bookbinder's needles
- bookbinder's punches
- sewing frames
- support cords
- backing hammers
- leather paring knives
- lying presses
- book presses
- scroll roller rods
- scroll smoothing stones
- scroll end knobs

### 9. Woodblock printing and East Asian copying tools

Include:

- block carving knives
- block clearing chisels
- ink daubers
- paper dampening brushes
- paste pots
- printing barens
- impression spoons
- registration pins
- carved text blocks as inert or inscribable props
- sutra-printing support objects

This category should be regionally concentrated. It is appropriate for Song China, Goryeo Korea, and some Buddhist manuscript contexts. It is not a default Western European medieval category.

### 10. Writing-specific support goods

Include only support goods that are not already adequately covered by the household package:

- portable writing boards
- lap desks
- writing boxes
- copyist boards
- small manuscript rests
- account boards
- scribe's tool rolls
- portable book stands
- narrow archive trays
- school tablet racks

Do not duplicate generic desks, shelves, cupboards, benches, lamps, or lecterns.

## Regional variation summary

The package is less regionally specific than clothing or military equipment, but it is not region-free. The key regional axes are **surface**, **format**, **implement**, and **institutional use**.

### Western and northern Europe

Default surface and book forms:

- parchment sheet
- parchment bifolium
- parchment quire
- parchment codex
- parchment scroll or roll
- charter roll
- wax tablet
- wax diptych/triptych
- quill
- stylus
- ink cake/pot
- wax seal

Important regional variants:

- Anglo-Saxon, Carolingian, Irish, and early Frankish contexts lean heavily toward monastic parchment codices.
- Norman, Anglo-Norman, High English, Capetian, German, and Christian Iberian contexts need legal rolls, charters, account documents, seals, and university/scholastic book forms.
- Norse and Anglo-Danish contexts should not be overfilled with native book goods. Use church, court, trade, and imported manuscript objects plus incised wooden/bone note objects where useful.
- Rus / Novgorod gets real distinctive coverage through birch-bark letters and styluses.

### Byzantine and Eastern Christian contexts

Default surface and book forms:

- parchment codices
- paper codices where appropriate
- liturgical books
- wax tablets
- official documents
- quills/reed pens
- ink goods
- book boxes

Regional distinction is mostly in book presentation and religious/institutional use, not core mechanics.

### Islamic Mediterranean, Near Eastern, and North African contexts

Default surface and book forms:

- paper sheets
- paper codices
- parchment manuscripts where appropriate
- legal and scholarly documents
- qalam/reed pens
- inkwells
- calligraphy tools
- document wrappers
- book boxes
- seals and chancery goods

This area should have stronger calligraphy and paper coverage than western Europe. Do not make paper a rare exotic item in Abbasid, Fatimid, Andalusian, or Seljuk/Ayyubid-Mamluk urban contexts.

### South Asia

Default surface and book forms:

- palm-leaf manuscripts using seeded `leaf` unless a more specific palm-leaf material is added
- wooden manuscript covers
- manuscript cords
- styluses
- wrapped bundles
- birch-bark manuscripts where northern or Himalayan/Northwestern contexts call for them
- paper documents where appropriate to date and region

South Indian coverage should emphasize palm-leaf bundles and styluses. North Indian coverage may include palm leaf, birch bark, and paper depending on setting and date.

### East Asia

Default surface and book forms:

- paper sheets
- handscrolls
- folded books
- stitched books
- brushes
- inksticks
- inkstones
- document wrappers
- book boxes
- woodblock-printing tools

Song China and Goryeo Korea should have the strongest woodblock-printing support. Heian/Kamakura Japan should emphasize brush, paper, scroll, letter, sutra-copying, and folded-book goods rather than woodblock printing as the main catalogue centre.

### Steppe and caravan contexts

Default forms:

- portable document cases
- scroll tubes
- messenger packets
- seal goods
- adopted paper/parchment documents
- compact writing kits

Do not overbuild a separate native scribal technology package unless the item form changes. Steppe literacy goods are best handled as portable administrative, religious, diplomatic, and trade objects.

## Catalogue distribution model

The catalogue uses the following category distribution:

| Category | Row count |
|---|---:|
| Writing surfaces and loose documents | 37 |
| Scrolls, rolls, books, and manuscript forms | 35 |
| Tablets and reusable writing surfaces | 20 |
| Writing implements | 29 |
| Ink, pigment, and illumination goods | 30 |
| Document containers and protection | 33 |
| Seals, authentication, and secured documents | 25 |
| Bookmaking, parchmentmaking, papermaking, and scrollmaking tools | 36 |
| Woodblock printing and East Asian copying tools | 18 |
| Writing-specific support goods | 23 |
| **Total** | **286** |

## Suggested implementation passes

### Pass 1: Functional foundation

- Use the existing medieval-specific `PaperSheet`, `Book`, `InscribableSurface`, `ScribingImplement`, `SealStamp`, `Sealable`, and writing-container component templates from the seeded component inventory.
- Author shared blank writing surfaces, ordinary books, quills, reed pens, qalams, brushes, styluses, charcoal sticks, ink goods, basic document containers, seal tools, and tamper-evident sealable target objects.
- Keep all public descriptions culture-neutral and form-based.
- Use `Sealable_*` target components where sealed charters, sealed letters, sealed rolls, sealed scrolls, sealed pouches, or sealed archive boxes should have mechanical tamper-evidence.

### Pass 2: Document and manuscript forms by material

- Add parchment, paper, papyrus, birch-bark, palm-leaf, bamboo, wax, clay, slate, and wooden document forms.
- Add codex, scroll, roll, handscroll, folded-book, stitched-book, palm-leaf bundle, wax diptych, wax triptych, and tablet variants.
- Add regional notes and skins where the material/form combination is not enough.

### Pass 3: Containers, seals, and administrative goods

- Add document pouches, scroll cases, book boxes, archive chests, deed boxes, courier tubes, seal matrices, seal cakes, sealable charters, sealable rolls, sealable letters, sealable pouches, sealable archive boxes, folded letters, and clay tablet envelopes.
- Add account rolls, ledgers, tax records, and office writing kits as blank forms, not exact text content.

### Pass 4: Craft and workshop tools

- Add parchmentmaking, papermaking, bookbinding, scrollmaking, calligraphy, illumination, and woodblock-printing tools.
- Match exact seeded tags and make craft-tool rows skinnable only where they are finished tools rather than raw stock.

### Pass 5: Religious, scholarly, and institutional overlays

- Add generic liturgical-book, scholastic-book, sutra-scroll, temple-manuscript, madrasa-manuscript, charter, legal-roll, and school-tablet forms.
- Keep exact texts, scripts, and religious imagery in skins or dynamic writing.

## Item catalogue targets

This final catalogue contains **286** finished item prototype targets. These rows define the stable implementation fields for the package, while the companion full-description CSV provides player-facing `fdesc` prose keyed by `unique_reference`.

### Catalogue distribution

| Category | Row count |
|---|---:|
| Writing surfaces and loose documents | 37 |
| Scrolls, rolls, books, and manuscript forms | 35 |
| Tablets and reusable writing surfaces | 20 |
| Writing implements | 29 |
| Ink, pigment, and illumination goods | 30 |
| Document containers and protection | 33 |
| Seals, authentication, and secured documents | 25 |
| Bookmaking, parchmentmaking, papermaking, and scrollmaking tools | 36 |
| Woodblock printing and East Asian copying tools | 18 |
| Writing-specific support goods | 23 |
| **Total** | **286** |

### Writing surfaces and loose documents

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_plain_parchment_sheet` | `sheet` | a plain parchment sheet | `parchment` | `Tiny` / `Standard` | 7.0g / 3.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_fine_parchment_sheet` | `sheet` | a fine parchment sheet | `parchment` | `Tiny` / `Good` | 6.0g / 8.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_ruled_parchment_sheet` | `sheet` | a ruled parchment sheet | `parchment` | `Tiny` / `Standard` | 7.0g / 5.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_scraped_parchment_bifolium` | `bifolium` | a scraped parchment bifolium | `parchment` | `Tiny` / `Standard` | 14.0g / 7.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Bifolium_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_fine_parchment_bifolium` | `bifolium` | a fine parchment bifolium | `parchment` | `Tiny` / `Good` | 12.0g / 12.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Bifolium_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_blank_parchment_charter` | `charter` | a blank parchment charter | `parchment` | `Tiny` / `Good` | 10.0g / 8.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_blank_parchment_writ` | `writ` | a blank parchment writ | `parchment` | `Tiny` / `Standard` | 8.0g / 6.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_parchment_deed_sheet` | `deed` | a blank parchment deed | `parchment` | `Tiny` / `Standard` | 9.0g / 7.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_parchment_label_tag` | `tag` | a parchment label tag | `parchment` | `Tiny` / `Standard` | 2.0g / 1.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_plain_rag_paper_sheet` | `sheet` | a plain rag-paper sheet | `paper` | `Tiny` / `Standard` | 4.0g / 1.5m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_fine_rag_paper_sheet` | `sheet` | a fine rag-paper sheet | `paper` | `Tiny` / `Good` | 4.0g / 4.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_ruled_rag_paper_sheet` | `sheet` | a ruled rag-paper sheet | `paper` | `Tiny` / `Standard` | 4.5g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_folded_rag_paper_letter` | `letter` | a folded rag-paper letter | `paper` | `Tiny` / `Standard` | 5.0g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Letter_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_rag_paper_account_slip` | `slip` | a rag-paper account slip | `paper` | `Tiny` / `Standard` | 3.0g / 1.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Letter_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_rag_paper_petition_leaf` | `leaf` | a petition paper leaf | `paper` | `Tiny` / `Standard` | 4.0g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_plain_papyrus_sheet` | `sheet` | a plain papyrus sheet | `papyrus` | `Tiny` / `Standard` | 4.0g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Papyrus` |
| `medieval_writing_mediterranean_papyrus_letter` | `letter` | a folded papyrus letter | `papyrus` | `Tiny` / `Standard` | 5.0g / 2.5m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Papyrus` |
| `medieval_writing_papyrus_account_sheet` | `sheet` | a papyrus account sheet | `papyrus` | `Tiny` / `Standard` | 4.5g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Papyrus` |
| `medieval_writing_east_asian_paper_sheet` | `sheet` | an East Asian paper sheet | `paper` | `Tiny` / `Standard` | 3.5g / 2.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_fine_east_asian_paper_sheet` | `sheet` | a fine East Asian paper sheet | `paper` | `Tiny` / `Good` | 3.0g / 5.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_east_asian_calligraphy_sheet` | `sheet` | a calligraphy paper sheet | `paper` | `Tiny` / `Good` | 3.0g / 4.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_east_asian_official_slip` | `slip` | an official paper slip | `paper` | `Tiny` / `Standard` | 2.5g / 2.5m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Sheet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_birch_bark_letter` | `letter` | a birch-bark letter | `birch` | `Tiny` / `Standard` | 12.0g / 1.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Birch_Bark_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_birch_bark_account_strip` | `strip` | a birch-bark account strip | `birch` | `Tiny` / `Standard` | 8.0g / 0.8m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Birch_Bark_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_birch_bark_practice_strip` | `strip` | a birch-bark practice strip | `birch` | `Tiny` / `Substandard` | 7.0g / 0.5m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Birch_Bark_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_palm_leaf_strip` | `leaf` | a palm-leaf manuscript strip | `leaf` | `Tiny` / `Standard` | 5.0g / 1.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_pierced_palm_leaf_strip` | `leaf` | a pierced palm-leaf strip | `leaf` | `Tiny` / `Standard` | 5.0g / 1.2m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_inked_palm_leaf_strip` | `leaf` | an ink-ready palm-leaf strip | `leaf` | `Tiny` / `Good` | 5.0g / 1.5m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_bamboo_slip` | `slip` | a bamboo writing slip | `bamboo` | `Tiny` / `Standard` | 18.0g / 1.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bamboo_Slip_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_bamboo_document_tag` | `tag` | a bamboo document tag | `bamboo` | `Tiny` / `Standard` | 12.0g / 0.8m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bamboo_Slip_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_potsherd_ostracon` | `ostracon` | a smoothed potsherd ostracon | `fired clay` | `VerySmall` / `Standard` | 80.0g / 0.5m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Ostracon_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Clay Tablets` |
| `medieval_writing_plastered_ostracon` | `ostracon` | a pale plastered ostracon | `ceramic` | `VerySmall` / `Standard` | 90.0g / 1.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Ostracon_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Clay Tablets` |
| `medieval_writing_nested_parchment_quire` | `quire` | a nested parchment quire | `parchment` | `VerySmall` / `Standard` | 65.0g / 20.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |
| `medieval_writing_rag_paper_quire` | `quire` | a quire of rag-paper sheets | `paper` | `VerySmall` / `Standard` | 45.0g / 10.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_bamboo_slip_bundle` | `bundle` | a bundle of bamboo slips | `bamboo` | `VerySmall` / `Standard` | 260.0g / 12.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_loose_scroll_label_tabs` | `tabs` | a bundle of scroll label tabs | `parchment` | `VerySmall` / `Standard` | 20.0g / 4.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_blank_paper_seal_tags` | `tags` | a bundle of blank seal tags | `paper` | `VerySmall` / `Standard` | 18.0g / 4.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |

### Scrolls, rolls, books, and manuscript forms

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_parchment_roll` | `roll` | a parchment writing roll | `parchment` | `Small` / `Standard` | 120.0g / 24.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_charter_roll` | `roll` | a blank charter roll | `parchment` | `Small` / `Good` | 140.0g / 32.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_account_roll` | `roll` | an account parchment roll | `parchment` | `Small` / `Standard` | 130.0g / 28.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_court_roll` | `roll` | a court record roll | `parchment` | `Small` / `Good` | 150.0g / 36.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_estate_roll` | `roll` | an estate account roll | `parchment` | `Small` / `Standard` | 145.0g / 34.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_tax_roll` | `roll` | a tax account roll | `parchment` | `Small` / `Standard` | 135.0g / 32.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_papyrus_scroll` | `scroll` | a simple papyrus scroll | `papyrus` | `Small` / `Standard` | 65.0g / 10.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_long_papyrus_scroll` | `scroll` | a long papyrus scroll | `papyrus` | `Small` / `Standard` | 90.0g / 16.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_papyrus_archive_roll` | `roll` | a papyrus archive roll | `papyrus` | `Small` / `Standard` | 80.0g / 14.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_rag_paper_scroll` | `scroll` | a rag-paper scroll | `paper` | `Small` / `Standard` | 55.0g / 12.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_east_asian_handscroll` | `scroll` | an East Asian handscroll | `paper` | `Small` / `Good` | 80.0g / 22.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_east_asian_poetry_scroll` | `scroll` | a blank poetry handscroll | `paper` | `Small` / `Good` | 70.0g / 18.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_east_asian_edict_scroll` | `scroll` | a formal paper edict scroll | `paper` | `Small` / `Good` | 95.0g / 28.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Scroll_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Scroll`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Scrolls` |
| `medieval_writing_small_parchment_codex` | `codex` | a small parchment codex | `parchment` | `Small` / `Standard` | 360.0g / 52.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_20_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_plain_parchment_codex` | `codex` | a plain parchment codex | `parchment` | `Small` / `Standard` | 520.0g / 80.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_40_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_large_parchment_codex` | `codex` | a large parchment codex | `parchment` | `Normal` / `Good` | 1100.0g / 170.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_prayer_codex` | `codex` | a small prayer codex | `parchment` | `Small` / `Good` | 380.0g / 72.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_20_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_scholastic_codex` | `codex` | a scholastic parchment codex | `parchment` | `Normal` / `Good` | 1250.0g / 190.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_liturgical_codex` | `codex` | a large liturgical codex | `parchment` | `Normal` / `Good` | 1600.0g / 260.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_monastic_rule_codex` | `codex` | a monastic rule codex | `parchment` | `Small` / `Good` | 580.0g / 95.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Codex_40_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_merchant_account_ledger` | `ledger` | a merchant account ledger | `parchment` | `Normal` / `Standard` | 1150.0g / 140.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Account_Ledger_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_estate_account_ledger` | `ledger` | an estate account ledger | `parchment` | `Normal` / `Good` | 1250.0g / 150.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Account_Ledger_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_toll_ledger` | `ledger` | a toll account ledger | `parchment` | `Normal` / `Standard` | 1000.0g / 130.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Account_Ledger_90_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_rag_paper_codex` | `codex` | a rag-paper codex | `paper` | `Small` / `Standard` | 360.0g / 44.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Codex_40_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_paper_scholarly_codex` | `codex` | a paper scholarly codex | `paper` | `Small` / `Good` | 420.0g / 70.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Codex_40_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_paper_devotional_codex` | `codex` | a paper devotional codex | `paper` | `Small` / `Good` | 430.0g / 78.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Codex_40_Page` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_east_asian_stitched_book` | `book` | an East Asian stitched book | `paper` | `Small` / `Standard` | 240.0g / 42.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Stitched_Book` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_east_asian_sutra_book` | `book` | a stitched sutra-copying book | `paper` | `Small` / `Good` | 280.0g / 64.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Stitched_Book` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_east_asian_account_book` | `book` | a stitched account book | `paper` | `Small` / `Standard` | 250.0g / 45.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Stitched_Book` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_palm_leaf_manuscript_bundle` | `bundle` | a palm-leaf manuscript bundle | `leaf` | `Small` / `Standard` | 420.0g / 42.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Bundle` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_temple_palm_leaf_bundle` | `bundle` | a temple palm-leaf bundle | `leaf` | `Small` / `Good` | 460.0g / 60.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Bundle` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_scholarly_palm_leaf_bundle` | `bundle` | a scholarly palm-leaf bundle | `leaf` | `Small` / `Good` | 440.0g / 55.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Bundle` | `Functions / Writing Surface`<br>`Functions / Writing Surface / Codex`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_wrapped_parchment_manuscript_bundle` | `bundle` | a wrapped parchment manuscript bundle | `parchment` | `Small` / `Standard` | 620.0g / 70.0m | `Holdable`<br>`Destroyable_Paper` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Codices` |
| `medieval_writing_wrapped_paper_manuscript_bundle` | `bundle` | a wrapped paper manuscript bundle | `paper` | `Small` / `Standard` | 380.0g / 42.0m | `Holdable`<br>`Destroyable_Paper` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_blank_commentary_gathering` | `gathering` | a blank commentary gathering | `parchment` | `Small` / `Standard` | 95.0g / 28.0m | `Holdable`<br>`Destroyable_Paper` | `Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Parchment` |

### Tablets and reusable writing surfaces

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_wax_tablet` | `tablet` | a wax writing tablet | `beeswax` | `Small` / `Standard` | 420.0g / 12.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_small_wax_tablet` | `tablet` | a small wax tablet | `beeswax` | `VerySmall` / `Standard` | 240.0g / 8.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_student_wax_tablet` | `tablet` | a student wax tablet | `beeswax` | `Small` / `Standard` | 360.0g / 9.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_account_wax_tablet` | `tablet` | an account wax tablet | `beeswax` | `Small` / `Standard` | 450.0g / 14.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_wax_diptych` | `diptych` | a wax tablet diptych | `beeswax` | `Small` / `Standard` | 850.0g / 25.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Diptych_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_wax_triptych` | `triptych` | a wax tablet triptych | `beeswax` | `Normal` / `Standard` | 1250.0g / 36.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Triptych_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_bronze_framed_wax_tablet` | `tablet` | a bronze-framed wax tablet | `beeswax` | `Small` / `Good` | 620.0g / 35.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wax_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Wax Tablets` |
| `medieval_writing_wooden_writing_tablet` | `tablet` | a wooden writing tablet | `wood` | `Small` / `Standard` | 300.0g / 7.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wooden_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_cedar_writing_tablet` | `tablet` | a cedar writing tablet | `cedar` | `Small` / `Good` | 280.0g / 9.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Wooden_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_wooden_practice_tablet` | `tablet` | a wooden practice tablet | `wood` | `Small` / `Standard` | 260.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_wooden_account_board` | `board` | a wooden account board | `wood` | `Normal` / `Standard` | 520.0g / 12.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_school_practice_board` | `board` | a school practice board | `wood` | `Normal` / `Standard` | 480.0g / 10.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_slate_tablet` | `tablet` | a slate writing tablet | `slate` | `Small` / `Standard` | 620.0g / 10.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Slate_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_school_slate_tablet` | `tablet` | a school slate tablet | `slate` | `Small` / `Standard` | 500.0g / 8.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Slate_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_slate_roster_tablet` | `tablet` | a slate roster tablet | `slate` | `Normal` / `Standard` | 900.0g / 16.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Slate_Tablet_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_bamboo_practice_panel` | `panel` | a bamboo practice panel | `bamboo` | `VerySmall` / `Standard` | 160.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bamboo_Slip_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_reusable_tally_board` | `board` | a reusable tally board | `wood` | `Normal` / `Standard` | 650.0g / 14.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_merchant_roster_board` | `board` | a merchant roster board | `wood` | `Normal` / `Good` | 780.0g / 18.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials` |
| `medieval_writing_clay_document_envelope` | `envelope` | a clay document envelope | `clay` | `Small` / `Standard` | 720.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Sealable_Document_Clay` | `Functions / Writing Goods`<br>`Market / Writing Materials / Clay Tablets` |
| `medieval_writing_clay_seal_tablet` | `tablet` | a clay seal tablet | `clay` | `Small` / `Standard` | 600.0g / 3.0m | `Holdable`<br>`Destroyable_Misc`<br>`Sealable_Document_Clay` | `Functions / Writing Goods`<br>`Market / Writing Materials / Clay Tablets` |

### Writing implements

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_plain_quill_pen` | `quill` | a plain quill pen | `feather` | `Tiny` / `Standard` | 5.0g / 1.5m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Quill_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Quill Pen` |
| `medieval_writing_goose_quill_pen` | `quill` | a goose quill pen | `feather` | `Tiny` / `Standard` | 5.0g / 1.8m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Quill_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Quill Pen` |
| `medieval_writing_fine_quill_pen` | `quill` | a fine quill pen | `feather` | `Tiny` / `Good` | 4.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Fine_Quill_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Quill Pen` |
| `medieval_writing_swan_quill_pen` | `quill` | a broad swan quill | `feather` | `Tiny` / `Good` | 6.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Fine_Quill_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Quill Pen` |
| `medieval_writing_trimmed_reed_pen` | `pen` | a trimmed reed pen | `reed` | `Tiny` / `Standard` | 6.0g / 1.2m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Reed_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Reed Pen` |
| `medieval_writing_broad_reed_pen` | `pen` | a broad reed pen | `reed` | `Tiny` / `Standard` | 7.0g / 1.4m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Reed_Pen` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Reed Pen` |
| `medieval_writing_qalam` | `qalam` | a cut qalam pen | `reed` | `Tiny` / `Standard` | 8.0g / 2.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Qalam` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Qalam Cutter` |
| `medieval_writing_fine_qalam` | `qalam` | a fine cut qalam | `reed` | `Tiny` / `Good` | 8.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Qalam` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Qalam Cutter` |
| `medieval_writing_east_asian_brush` | `brush` | an East Asian writing brush | `bamboo` | `Tiny` / `Standard` | 22.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_East_Asian_Writing_Brush` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Ink Brush` |
| `medieval_writing_fine_calligraphy_brush` | `brush` | a fine calligraphy brush | `bamboo` | `Tiny` / `Good` | 28.0g / 12.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Calligraphy_Brush` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Calligrapher's Brush` |
| `medieval_writing_sutra_copying_brush` | `brush` | a sutra-copying brush | `bamboo` | `Tiny` / `Good` | 24.0g / 10.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Calligraphy_Brush` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Calligrapher's Brush` |
| `medieval_writing_labeling_brush` | `brush` | a stiff labeling brush | `wood` | `Tiny` / `Standard` | 26.0g / 6.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_East_Asian_Writing_Brush` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Ink Brush` |
| `medieval_writing_charcoal_stick` | `stick` | a charcoal writing stick | `charcoal` | `Tiny` / `Standard` | 12.0g / 0.5m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Charcoal_Stick` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Charcoal Stick` |
| `medieval_writing_bone_stylus` | `stylus` | a polished bone stylus | `bone` | `Tiny` / `Standard` | 22.0g / 2.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bone_Stylus` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_ivory_stylus` | `stylus` | an ivory writing stylus | `ivory` | `Tiny` / `Good` | 25.0g / 18.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bone_Stylus` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_bronze_stylus` | `stylus` | a bronze writing stylus | `bronze` | `VerySmall` / `Standard` | 45.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Bronze_Stylus` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_iron_stylus` | `stylus` | an iron writing stylus | `wrought iron` | `VerySmall` / `Standard` | 55.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Iron_Stylus` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_reed_stylus` | `stylus` | a sharpened reed stylus | `reed` | `Tiny` / `Standard` | 5.0g / 0.5m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Reed_Stylus` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_scribing_chisel` | `chisel` | a small scribing chisel | `wrought iron` | `VerySmall` / `Standard` | 85.0g / 6.0m | `Holdable`<br>`Destroyable_Misc`<br>`Medieval_Scribing_Chisel` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Stylus` |
| `medieval_writing_bronze_pen_knife` | `knife` | a bronze pen knife | `bronze` | `VerySmall` / `Standard` | 70.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Pen Knife` |
| `medieval_writing_iron_pen_knife` | `knife` | an iron pen knife | `wrought iron` | `VerySmall` / `Standard` | 80.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Pen Knife` |
| `medieval_writing_qalam_cutter` | `cutter` | a qalam cutting knife | `bronze` | `VerySmall` / `Standard` | 65.0g / 7.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Qalam Cutter` |
| `medieval_writing_pen_rest` | `rest` | a small pen rest | `wood` | `VerySmall` / `Standard` | 60.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Pen Rest` |
| `medieval_writing_bamboo_pen_rack` | `rack` | a bamboo pen rack | `bamboo` | `Small` / `Standard` | 180.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Pen Rack` |
| `medieval_writing_linen_pen_wiper` | `wiper` | a linen pen wiper | `linen` | `VerySmall` / `Standard` | 18.0g / 1.5m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Pen Wiper` |
| `medieval_writing_quill_curing_sand` | `sand` | a tray of quill-curing sand | `sand` | `Small` / `Standard` | 950.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Calligraphy Tools / Quill Curing Sand` |
| `medieval_writing_spare_quill_bundle` | `bundle` | a bundle of spare quills | `feather` | `VerySmall` / `Standard` | 35.0g / 6.0m | `Holdable`<br>`Destroyable_Misc`<br>`Stack_Number` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Quill Pen` |
| `medieval_writing_reed_pen_bundle` | `bundle` | a bundle of reed pens | `reed` | `VerySmall` / `Standard` | 60.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Stack_Number` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Reed Pen` |
| `medieval_writing_charcoal_stick_bundle` | `bundle` | a bundle of charcoal sticks | `charcoal` | `VerySmall` / `Standard` | 120.0g / 3.0m | `Holdable`<br>`Destroyable_Misc`<br>`Stack_Number` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Charcoal Stick` |

### Ink, pigment, and illumination goods

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_soot_ink_cake` | `cake` | a soot-black ink cake | `soot` | `Tiny` / `Standard` | 35.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_carbon_ink_stick` | `stick` | a carbon ink stick | `soot` | `Tiny` / `Standard` | 40.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_lampblack_packet` | `packet` | a packet of lampblack | `soot` | `Tiny` / `Standard` | 25.0g / 2.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_red_ink_cake` | `cake` | a red pigment ink cake | `cinnabar` | `Tiny` / `Good` | 32.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_blue_pigment_cake` | `cake` | a blue pigment cake | `azurite` | `Tiny` / `Good` | 30.0g / 12.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_green_pigment_cake` | `cake` | a green pigment cake | `malachite` | `Tiny` / `Good` | 30.0g / 9.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_gold_leaf_packet` | `packet` | a packet of gold leaf | `gold` | `Tiny` / `VeryGood` | 3.0g / 80.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_silver_leaf_packet` | `packet` | a packet of silver leaf | `silver` | `Tiny` / `Good` | 4.0g / 30.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_powdered_chalk` | `chalk` | a pouch of whitening chalk | `chalk` | `VerySmall` / `Standard` | 90.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_pounce_bag` | `bag` | a small pounce bag | `linen` | `VerySmall` / `Standard` | 80.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_pumice_piece` | `pumice` | a piece of parchment pumice | `pumice` | `Tiny` / `Standard` | 55.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Writing Materials / Ink`<br>`Functions / Material Functions / Writing Craft Stock / Ink Stock` |
| `medieval_writing_small_clay_inkwell` | `inkwell` | a small clay inkwell | `fired clay` | `VerySmall` / `Standard` | 160.0g / 4.0m | `Holdable`<br>`Destroyable_Glassware` | `Market / Writing Materials / Ink`<br>`Functions / Tools / Scribing Tools / Inkwell` |
| `medieval_writing_ceramic_ink_pot` | `inkpot` | a ceramic ink pot | `ceramic` | `VerySmall` / `Standard` | 180.0g / 6.0m | `Holdable`<br>`Destroyable_Glassware` | `Market / Writing Materials / Ink`<br>`Functions / Tools / Scribing Tools / Inkwell` |
| `medieval_writing_glass_inkwell` | `inkwell` | a small glass inkwell | `glass` | `VerySmall` / `Good` | 130.0g / 14.0m | `Holdable`<br>`Destroyable_Glassware` | `Market / Writing Materials / Ink`<br>`Functions / Tools / Scribing Tools / Inkwell` |
| `medieval_writing_bronze_travel_inkwell` | `inkwell` | a bronze travel inkwell | `bronze` | `VerySmall` / `Good` | 210.0g / 22.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Writing Materials / Ink`<br>`Functions / Tools / Scribing Tools / Inkwell` |
| `medieval_writing_double_ceramic_inkwell` | `inkwell` | a double ceramic inkwell | `ceramic` | `VerySmall` / `Standard` | 320.0g / 12.0m | `Holdable`<br>`Destroyable_Glassware` | `Market / Writing Materials / Ink`<br>`Functions / Tools / Scribing Tools / Inkwell` |
| `medieval_writing_slate_inkstone` | `inkstone` | a slate inkstone | `slate` | `VerySmall` / `Standard` | 620.0g / 18.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Calligraphy Tools` |
| `medieval_writing_stone_inkstone` | `inkstone` | a polished stone inkstone | `stone` | `VerySmall` / `Standard` | 700.0g / 20.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Calligraphy Tools` |
| `medieval_writing_agate_inkstone` | `inkstone` | a fine agate inkstone | `agate` | `VerySmall` / `Good` | 520.0g / 60.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Calligraphy Tools` |
| `medieval_writing_jade_inkstone` | `inkstone` | a small jade inkstone | `jade` | `VerySmall` / `VeryGood` | 480.0g / 120.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Calligraphy Tools` |
| `medieval_writing_pigment_shell` | `shell` | a pigment mixing shell | `shell` | `Tiny` / `Standard` | 28.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Pigment Shell` |
| `medieval_writing_shell_palette` | `palette` | a shallow shell palette | `shell` | `Tiny` / `Standard` | 35.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Pigment Shell` |
| `medieval_writing_stone_pigment_muller` | `muller` | a stone pigment muller | `stone` | `VerySmall` / `Standard` | 620.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Pigment Muller` |
| `medieval_writing_palette_slab` | `slab` | a stone palette slab | `stone` | `VerySmall` / `Standard` | 850.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Palette Slab` |
| `medieval_writing_agate_burnisher` | `burnisher` | an agate burnisher | `agate` | `Tiny` / `Good` | 85.0g / 18.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Agate Burnisher` |
| `medieval_writing_gesso_pot` | `pot` | a small gesso pot | `ceramic` | `VerySmall` / `Standard` | 240.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Gesso Pot` |
| `medieval_writing_gilding_knife` | `knife` | a gilding knife | `wrought iron` | `Tiny` / `Good` | 75.0g / 9.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Gilding Knife` |
| `medieval_writing_gilding_tip` | `tip` | a soft gilding tip | `feather` | `Tiny` / `Good` | 15.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Gilding Tip` |
| `medieval_writing_gold_leaf_cushion` | `cushion` | a gold-leaf cushion | `leather` | `VerySmall` / `Good` | 220.0g / 16.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Gold Leaf Cushion` |
| `medieval_writing_miniature_detail_brush` | `brush` | a miniature detail brush | `wood` | `Tiny` / `Good` | 18.0g / 12.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Illumination Tools / Miniature Detail Brush` |

### Document containers and protection

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_cedar_scroll_tube` | `tube` | a cedar scroll tube | `cedar` | `Small` / `Standard` | 450.0g / 18.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Scroll_Tube` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_bamboo_scroll_tube` | `tube` | a bamboo scroll tube | `bamboo` | `Small` / `Standard` | 260.0g / 12.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Scroll_Tube` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_leather_scroll_case` | `case` | a leather scroll case | `leather` | `Small` / `Good` | 520.0g / 20.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Scroll_Tube` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_silk_scroll_wrapper` | `wrapper` | a silk scroll wrapper | `silk` | `VerySmall` / `Good` | 80.0g / 18.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_linen_scroll_wrap` | `wrap` | a linen scroll wrap | `linen` | `VerySmall` / `Standard` | 90.0g / 5.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_leather_document_pouch` | `pouch` | a leather document pouch | `leather` | `Small` / `Standard` | 260.0g / 14.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_linen_document_pouch` | `pouch` | a linen document pouch | `linen` | `Small` / `Standard` | 120.0g / 6.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_tablet_wrap` | `wrap` | a padded tablet wrap | `linen` | `VerySmall` / `Standard` | 90.0g / 4.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_wax_tablet_pouch` | `pouch` | a wax-tablet pouch | `leather` | `Small` / `Standard` | 180.0g / 10.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_book_satchel` | `satchel` | a leather book satchel | `leather` | `Normal` / `Good` | 720.0g / 32.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Satchel` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_document_satchel` | `satchel` | a document satchel | `leather` | `Normal` / `Standard` | 640.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Satchel` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_courier_document_satchel` | `satchel` | a courier document satchel | `leather` | `Normal` / `Good` | 760.0g / 40.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Satchel` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_notary_satchel` | `satchel` | a notary's document satchel | `leather` | `Normal` / `Good` | 820.0g / 48.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Satchel` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_palm_leaf_wrapper` | `wrapper` | a palm-leaf manuscript wrapper | `cotton` | `Small` / `Standard` | 140.0g / 8.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_silk_manuscript_wrapper` | `wrapper` | a silk manuscript wrapper | `silk` | `Small` / `Good` | 100.0g / 24.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_cypress_book_box` | `box` | a cypress book box | `cypress` | `Normal` / `Good` | 1800.0g / 36.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_cedar_book_box` | `box` | a cedar book box | `cedar` | `Normal` / `Standard` | 1700.0g / 30.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_lacquered_book_box` | `box` | a lacquered book box | `lacquer` | `Normal` / `Good` | 1500.0g / 70.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_small_archive_box` | `box` | a small archive box | `oak` | `Normal` / `Standard` | 2600.0g / 34.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_large_archive_chest` | `chest` | a large archive chest | `oak` | `Large` / `Standard` | 22000.0g / 120.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Chest` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_deed_box` | `box` | a deed storage box | `walnut` | `Normal` / `Good` | 2400.0g / 44.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_scripture_box` | `box` | a scripture storage box | `cypress` | `Normal` / `Good` | 2100.0g / 58.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_seal_matrix_box` | `box` | a seal matrix box | `cedar` | `Small` / `Good` | 900.0g / 24.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Seal_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_wax_and_seal_box` | `box` | a wax and seal box | `cedar` | `Small` / `Standard` | 1000.0g / 26.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Seal_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_messenger_tube` | `tube` | a messenger document tube | `bamboo` | `Small` / `Standard` | 320.0g / 14.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Scroll_Tube` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_courier_scroll_case` | `case` | a courier scroll case | `leather` | `Small` / `Good` | 600.0g / 28.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Scroll_Tube` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_school_tablet_case` | `case` | a school tablet case | `wood` | `Small` / `Standard` | 520.0g / 12.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_document_bookcase_shelves` | `shelves` | portable document shelves | `wood` | `Large` / `Standard` | 7800.0g / 55.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Bookcase_Shelves` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_locking_document_box` | `box` | a locking document box | `oak` | `Small` / `Good` | 3400.0g / 70.0m | `Holdable`<br>`Destroyable_Furniture`<br>`LockingContainer_Lockbox` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Market / Household Goods / Luxury Wares` |
| `medieval_writing_locking_deed_chest` | `chest` | a locking deed chest | `oak` | `Normal` / `Good` | 18000.0g / 180.0m | `Holdable`<br>`Destroyable_Furniture`<br>`LockingContainer_Footlocker` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Market / Household Goods / Luxury Wares` |
| `medieval_writing_locking_archive_chest` | `chest` | a locking archive chest | `oak` | `Large` / `Good` | 26000.0g / 220.0m | `Holdable`<br>`Destroyable_Furniture`<br>`LockingContainer_Footlocker` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Market / Household Goods / Luxury Wares` |
| `medieval_writing_chancery_strong_chest` | `chest` | a chancery strong chest | `wrought iron` | `Large` / `Good` | 62000.0g / 680.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`LockingContainer_SafeChest` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Market / Household Goods / Luxury Wares` |
| `medieval_writing_seal_office_lockbox` | `lockbox` | a seal office lockbox | `brass` | `Small` / `Good` | 4200.0g / 120.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`LockingContainer_Lockbox` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Market / Household Goods / Luxury Wares` |

### Seals, authentication, and secured documents

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_bronze_signet_stamp` | `stamp` | a bronze signet stamp | `bronze` | `VerySmall` / `Good` | 75.0g / 45.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`SealStamp_Medieval_BronzeSignet` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_brass_office_seal` | `seal` | a brass office seal | `brass` | `VerySmall` / `Good` | 320.0g / 95.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`SealStamp_Medieval_BrassOfficeSeal` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_iron_seal_matrix` | `matrix` | an iron seal matrix | `wrought iron` | `VerySmall` / `Good` | 260.0g / 55.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`SealStamp_Medieval_IronSealMatrix` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_lead_seal_matrix` | `matrix` | a lead seal matrix | `lead` | `VerySmall` / `Good` | 380.0g / 65.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`SealStamp_Medieval_LeadSealMatrix` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_ivory_handled_seal_stamp` | `stamp` | an ivory-handled seal stamp | `ivory` | `VerySmall` / `VeryGood` | 120.0g / 120.0m | `Holdable`<br>`Destroyable_Misc`<br>`SealStamp_Medieval_BronzeSignet` | `Market / Writing Materials / Writing Implements`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_wax_seal_cake` | `cake` | a wax seal cake | `beeswax` | `Tiny` / `Standard` | 55.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_red_wax_seal_cake` | `cake` | a red wax seal cake | `beeswax` | `Tiny` / `Good` | 60.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_clay_sealing_lump` | `lump` | a lump of sealing clay | `clay` | `Tiny` / `Standard` | 120.0g / 1.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_lead_bulla_blank` | `bulla` | a blank lead bulla | `lead` | `Tiny` / `Standard` | 85.0g / 8.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_lead_seal_tag` | `tag` | a lead seal tag | `lead` | `Tiny` / `Standard` | 55.0g / 6.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_seal_cord` | `cord` | a seal cord | `linen` | `Tiny` / `Standard` | 12.0g / 1.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Writing Goods`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Tools / Scribing Tools / Seal Stamp` |
| `medieval_writing_sealable_parchment_charter` | `charter` | a sealable parchment charter | `parchment` | `Tiny` / `Good` | 12.0g / 12.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface`<br>`Sealable_Medieval_Parchment_Charter` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealed_parchment_roll` | `roll` | a sealable parchment roll | `parchment` | `Small` / `Good` | 150.0g / 40.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface`<br>`Sealable_Medieval_Parchment_Roll` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_rag_paper_letter` | `letter` | a sealable rag-paper letter | `paper` | `Tiny` / `Standard` | 6.0g / 3.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Letter_Surface`<br>`Sealable_Medieval_Rag_Paper_Letter` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_official_writ` | `writ` | a sealable official writ | `parchment` | `Tiny` / `Good` | 10.0g / 14.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface`<br>`Sealable_Medieval_Official_Writ` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_east_asian_scroll` | `scroll` | a sealable paper handscroll | `paper` | `Small` / `Good` | 90.0g / 30.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_East_Asian_Paper_Scroll_Surface`<br>`Sealable_Medieval_East_Asian_Scroll` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_palm_leaf_bundle` | `bundle` | a sealable palm-leaf bundle | `leaf` | `Small` / `Good` | 460.0g / 62.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Palm_Leaf_Manuscript_Bundle`<br>`Sealable_Medieval_Palm_Leaf_Bundle` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_wax_sealable_document` | `document` | a wax-sealable document | `parchment` | `Tiny` / `Standard` | 8.0g / 9.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Sheet_Surface`<br>`Sealable_Document_Wax` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_clay_sealable_document` | `document` | a clay-sealable document | `papyrus` | `Tiny` / `Standard` | 6.0g / 6.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Papyrus_Sheet_Surface`<br>`Sealable_Document_Clay` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_paper_envelope` | `envelope` | a sealable paper envelope | `paper` | `Tiny` / `Standard` | 8.0g / 4.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Rag_Paper_Letter_Surface`<br>`Sealable_Envelope` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_scroll` | `scroll` | a general sealable scroll | `parchment` | `Small` / `Standard` | 130.0g / 28.0m | `Holdable`<br>`Destroyable_Paper`<br>`Medieval_Parchment_Roll_Surface`<br>`Sealable_Scroll` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Document Containers` |
| `medieval_writing_sealable_document_pouch` | `pouch` | a sealable document pouch | `leather` | `Small` / `Good` | 280.0g / 18.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Pouch`<br>`Sealable_Medieval_Document_Pouch` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_sealable_archive_box` | `box` | a sealable archive box | `oak` | `Normal` / `Good` | 2800.0g / 48.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box`<br>`Sealable_Medieval_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_sealable_archive_chest` | `chest` | a sealable archive chest | `oak` | `Large` / `Good` | 24000.0g / 150.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Chest`<br>`Sealable_Medieval_Archive_Box` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |
| `medieval_writing_wax_sealable_container` | `box` | a wax-sealable document box | `cedar` | `Normal` / `Standard` | 2400.0g / 42.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Archive_Box`<br>`Sealable_Container_Wax` | `Functions / Container`<br>`Market / Writing Materials / Document Containers`<br>`Functions / Writing Goods` |

### Bookmaking, parchmentmaking, papermaking, and scrollmaking tools

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_parchment_scraping_knife` | `knife` | a parchment scraping knife | `bronze` | `VerySmall` / `Standard` | 120.0g / 9.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Scraping Knife` |
| `medieval_writing_parchment_lunellum` | `lunellum` | a crescent parchment lunellum | `wrought iron` | `VerySmall` / `Good` | 180.0g / 14.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Lunellum` |
| `medieval_writing_parchment_stretching_frame` | `frame` | a parchment stretching frame | `wood` | `Large` / `Standard` | 6200.0g / 22.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Stretching Frame` |
| `medieval_writing_parchment_lacing_cord` | `cord` | a parchment lacing cord | `hemp` | `VerySmall` / `Standard` | 180.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Lacing Cord` |
| `medieval_writing_parchment_pegs` | `pegs` | a set of parchment pegs | `wood` | `VerySmall` / `Standard` | 260.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Pegs` |
| `medieval_writing_parchment_fleshing_beam` | `beam` | a parchment fleshing beam | `wood` | `Large` / `Standard` | 9500.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Fleshing Beam` |
| `medieval_writing_parchment_whitening_chalk` | `chalk` | a lump of whitening chalk | `chalk` | `Tiny` / `Standard` | 120.0g / 2.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Whitening Chalk` |
| `medieval_writing_parchment_pumice` | `pumice` | a piece of parchment pumice | `pumice` | `Tiny` / `Standard` | 55.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Parchment Pumice` |
| `medieval_writing_parchment_pounce_bag` | `bag` | a parchment pounce bag | `linen` | `VerySmall` / `Standard` | 85.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Parchmentmaking Tools / Pounce Bag` |
| `medieval_writing_papermaker_mould_deckle` | `mould` | a mould and deckle | `wood` | `Normal` / `Good` | 1800.0g / 30.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Mould and Deckle` |
| `medieval_writing_papermaker_vat` | `vat` | a papermaker's vat | `wood` | `Large` / `Standard` | 22000.0g / 80.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Papermaker's Vat` |
| `medieval_writing_couching_blanket` | `blanket` | a couching blanket | `wool` | `Normal` / `Standard` | 900.0g / 12.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Couching Blanket` |
| `medieval_writing_press_felt` | `felt` | a papermaker's press felt | `felt` | `Normal` / `Standard` | 720.0g / 12.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Press Felt` |
| `medieval_writing_lay_press` | `press` | a papermaker's lay press | `wood` | `Large` / `Good` | 18000.0g / 90.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Lay Press` |
| `medieval_writing_rag_sorting_knife` | `knife` | a rag-sorting knife | `wrought iron` | `VerySmall` / `Standard` | 95.0g / 6.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Rag Sorting Knife` |
| `medieval_writing_rag_beating_trough` | `trough` | a rag-beating trough | `wood` | `Large` / `Standard` | 12000.0g / 45.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Rag Beating Trough` |
| `medieval_writing_paper_sizing_brush` | `brush` | a paper sizing brush | `wood` | `VerySmall` / `Standard` | 120.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Paper Sizing Brush` |
| `medieval_writing_gelatine_sizing_pot` | `pot` | a sizing pot | `ceramic` | `Small` / `Standard` | 850.0g / 10.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Gelatine Sizing Pot` |
| `medieval_writing_paper_burnishing_agate` | `agate` | a paper burnishing agate | `agate` | `Tiny` / `Good` | 90.0g / 16.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Paper Burnishing Agate` |
| `medieval_writing_watermark_wire` | `wire` | a watermark wire | `brass` | `Tiny` / `Good` | 20.0g / 10.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Papermaking Tools / Watermark Wire` |
| `medieval_writing_bookbinders_needle` | `needle` | a bookbinder's needle | `bronze` | `Tiny` / `Standard` | 18.0g / 5.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Bookbinder's Needle` |
| `medieval_writing_endband_needle` | `needle` | an endband needle | `bronze` | `Tiny` / `Standard` | 14.0g / 5.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Endband Needle` |
| `medieval_writing_bookbinders_punch` | `punch` | a bookbinder's punch | `bronze` | `VerySmall` / `Standard` | 110.0g / 7.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Bookbinder's Punch` |
| `medieval_writing_bookbinder_sewing_frame` | `frame` | a bookbinder's sewing frame | `wood` | `Normal` / `Good` | 2600.0g / 34.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame` |
| `medieval_writing_sewing_support_cords` | `cords` | a set of sewing support cords | `hemp` | `VerySmall` / `Standard` | 150.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Sewing Support Cords` |
| `medieval_writing_backing_hammer` | `hammer` | a bookbinder's backing hammer | `wrought iron` | `Small` / `Good` | 620.0g / 16.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Backing Hammer` |
| `medieval_writing_leather_paring_knife` | `knife` | a leather paring knife | `wrought iron` | `VerySmall` / `Good` | 90.0g / 10.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Leather Paring Knife` |
| `medieval_writing_lying_press` | `press` | a bookbinder's lying press | `wood` | `Large` / `Good` | 11500.0g / 70.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Lying Press` |
| `medieval_writing_book_press` | `press` | a wooden book press | `wood` | `Large` / `Good` | 18000.0g / 110.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Book Press` |
| `medieval_writing_book_plough` | `plough` | a bookbinder's plough | `wood` | `Normal` / `Good` | 3200.0g / 48.0m | `Holdable`<br>`Destroyable_Furniture` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Bookbinding Tools / Book Plough` |
| `medieval_writing_scroll_roller_rod` | `rod` | a scroll roller rod | `cedar` | `Small` / `Standard` | 90.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll Roller Rod` |
| `medieval_writing_scroll_smoothing_stone` | `stone` | a scroll smoothing stone | `stone` | `Tiny` / `Standard` | 180.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll Smoothing Stone` |
| `medieval_writing_scroll_end_knob` | `knob` | a scroll end knob | `wood` | `Tiny` / `Standard` | 45.0g / 3.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll End Knob` |
| `medieval_writing_scroll_tie_ribbon` | `ribbon` | a scroll tie ribbon | `linen` | `Tiny` / `Standard` | 8.0g / 0.6m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll Tie Ribbon` |
| `medieval_writing_scroll_seal_cord` | `cord` | a scroll seal cord | `linen` | `Tiny` / `Standard` | 12.0g / 1.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll Seal Cord` |
| `medieval_writing_scroll_label_tab` | `tab` | a scroll label tab | `parchment` | `Tiny` / `Standard` | 4.0g / 0.8m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Scrollmaking Tools / Scroll Label Tab` |

### Woodblock printing and East Asian copying tools

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_block_carving_knife` | `knife` | a block carving knife | `wrought iron` | `VerySmall` / `Standard` | 85.0g / 8.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Block Carving Knife` |
| `medieval_writing_block_clearing_chisel` | `chisel` | a block clearing chisel | `wrought iron` | `VerySmall` / `Standard` | 110.0g / 8.0m | `Holdable`<br>`Destroyable_HeavyMetal` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Block Clearing Chisel` |
| `medieval_writing_woodblock_ink_dauber` | `dauber` | a woodblock ink dauber | `bamboo` | `VerySmall` / `Standard` | 120.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Ink Dauber` |
| `medieval_writing_paper_dampening_brush` | `brush` | a paper dampening brush | `bamboo` | `VerySmall` / `Standard` | 140.0g / 7.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Paper Dampening Brush` |
| `medieval_writing_printing_paste_pot` | `pot` | a printing paste pot | `ceramic` | `VerySmall` / `Standard` | 300.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Paste Pot` |
| `medieval_writing_printing_baren` | `baren` | a printing baren | `bamboo` | `VerySmall` / `Good` | 90.0g / 10.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Printing Baren` |
| `medieval_writing_impression_spoon` | `spoon` | an impression spoon | `wood` | `Tiny` / `Standard` | 70.0g / 5.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Impression Spoon` |
| `medieval_writing_registration_pins` | `pins` | a set of registration pins | `bamboo` | `Tiny` / `Standard` | 35.0g / 4.0m | `Holdable`<br>`Destroyable_Misc` | `Market / Professional Tools / Standard Tools`<br>`Functions / Tools / Woodblock Printing Tools / Registration Pin` |
| `medieval_writing_blank_woodblock` | `block` | a blank printing woodblock | `wood` | `Normal` / `Standard` | 1500.0g / 12.0m | `Holdable`<br>`Destroyable_Furniture` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_carved_text_block` | `block` | a carved text woodblock | `wood` | `Normal` / `Good` | 1600.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_sutra_printing_block` | `block` | a sutra printing block | `wood` | `Normal` / `Good` | 1700.0g / 36.0m | `Holdable`<br>`Destroyable_Furniture` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_bamboo_printing_board` | `board` | a bamboo printing board | `bamboo` | `Normal` / `Standard` | 980.0g / 16.0m | `Holdable`<br>`Destroyable_Furniture` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_woodblock_proof_sheet` | `sheet` | a woodblock proof sheet | `paper` | `Tiny` / `Standard` | 4.0g / 2.0m | `Holdable`<br>`Destroyable_Paper` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_printing_paper_stack` | `stack` | a stack of printing paper | `paper` | `Small` / `Standard` | 220.0g / 24.0m | `Holdable`<br>`Destroyable_Paper`<br>`Stack_Number` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools`<br>`Functions / Writing Goods`<br>`Materials / Writing Product`<br>`Market / Writing Materials / Paper` |
| `medieval_writing_inked_printing_pad` | `pad` | an inked printing pad | `linen` | `VerySmall` / `Standard` | 180.0g / 10.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_block_rubbing_pad` | `pad` | a block rubbing pad | `linen` | `VerySmall` / `Standard` | 160.0g / 8.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_print_registration_frame` | `frame` | a print registration frame | `wood` | `Normal` / `Good` | 1600.0g / 24.0m | `Holdable`<br>`Destroyable_Furniture` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_print_drying_line` | `line` | a print drying line | `hemp` | `Small` / `Standard` | 220.0g / 6.0m | `Holdable`<br>`Destroyable_Misc` | `Functions / Tools / Woodblock Printing Tools`<br>`Market / Professional Tools / Standard Tools` |

### Writing-specific support goods

| Unique reference | Noun | SDesc | Material | Size / Quality | Weight / Cost | Components | Tags |
|---|---|---|---|---|---:|---|---|
| `medieval_writing_scribes_sloped_board` | `board` | a scribe's sloped board | `wood` | `Normal` / `Standard` | 2400.0g / 30.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_portable_lap_desk` | `desk` | a portable lap desk | `wood` | `Normal` / `Good` | 3200.0g / 42.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_writing_desk_drawers` | `desk` | a small writing desk | `oak` | `Large` / `Good` | 12000.0g / 85.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Drawers` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_copyists_board` | `board` | a copyist's writing board | `wood` | `Normal` / `Standard` | 2600.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_manuscript_rest` | `rest` | a small manuscript rest | `wood` | `Normal` / `Standard` | 1400.0g / 22.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Small_Table` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_portable_book_stand` | `stand` | a portable book stand | `wood` | `Normal` / `Good` | 2200.0g / 32.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Small_Table` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_account_table_board` | `board` | an account table board | `wood` | `Normal` / `Standard` | 1600.0g / 20.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Medieval_Practice_Board_Surface` | `Functions / Writing Surface`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_school_tablet_rack` | `rack` | a school tablet rack | `wood` | `Large` / `Standard` | 4200.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Narrow_Shelves` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_narrow_archive_tray` | `tray` | a narrow archive tray | `wood` | `Small` / `Standard` | 900.0g / 16.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Tray` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_scribe_tool_roll` | `roll` | a scribe's tool roll | `leather` | `Small` / `Standard` | 260.0g / 16.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_calligrapher_tool_roll` | `roll` | a calligrapher's tool roll | `silk` | `Small` / `Good` | 180.0g / 34.0m | `Holdable`<br>`Destroyable_Misc`<br>`Container_Document_Pouch` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_copyist_kit_box` | `box` | a copyist's kit box | `cedar` | `Small` / `Standard` | 1100.0g / 28.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Seal_Box` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_clerk_writing_box` | `box` | a clerk's writing box | `oak` | `Normal` / `Good` | 3600.0g / 52.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Drawers` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_travelling_scribe_box` | `box` | a travelling scribe's box | `cedar` | `Normal` / `Good` | 2900.0g / 46.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Drawers` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_scholars_book_board` | `board` | a scholar's book board | `wood` | `Normal` / `Standard` | 2100.0g / 26.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_standing_copy_board` | `board` | a standing copy board | `wood` | `Large` / `Standard` | 5200.0g / 45.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_document_sorting_tray` | `tray` | a document sorting tray | `wood` | `Small` / `Standard` | 800.0g / 14.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Tray` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_seal_impression_tray` | `tray` | a seal impression tray | `bronze` | `Small` / `Good` | 850.0g / 32.0m | `Holdable`<br>`Destroyable_HeavyMetal`<br>`Container_Tray` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_ink_and_pen_tray` | `tray` | an ink and pen tray | `wood` | `Small` / `Standard` | 600.0g / 12.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Tray` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_small_manuscript_shelf` | `shelf` | a small manuscript shelf | `wood` | `Large` / `Standard` | 5200.0g / 42.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Bookcase_Shelves` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_portable_scroll_rack` | `rack` | a portable scroll rack | `wood` | `Large` / `Standard` | 4800.0g / 38.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Document_Bookcase_Shelves` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_wax_tablet_stand` | `stand` | a wax-tablet stand | `wood` | `Normal` / `Standard` | 1200.0g / 18.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Small_Table` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |
| `medieval_writing_lectern_board` | `board` | a portable lectern board | `wood` | `Large` / `Good` | 3600.0g / 40.0m | `Holdable`<br>`Destroyable_Furniture`<br>`Container_Writing_Desk_Surface` | `Functions / Container`<br>`Functions / Writing Goods`<br>`Functions / Household Items / Household Wares`<br>`Market / Professional Tools / Standard Tools` |


## Full-description catalogue

The player-facing full-description catalogue is supplied as a standalone CSV file named `FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv`. It contains one row for every item in the main catalogue and uses the columns `unique_reference`, `sdesc`, and `fdesc`.

The design reference keeps the main implementation catalogue and the prose-description catalogue separate so that the implementation rows remain compact while the description pass can be reviewed, edited, and imported independently.

## Source-grounding notes

This reference follows the established medieval package pattern from the completed clothing, household, and military seeder references: shared-first catalogue design, builder-facing culture labels, player-facing culture-neutral descriptions, exact seeded components/materials/tags, ordinary portable finished goods, and strict 500AD-1300AD period boundaries.

The package should use the seeded medieval writing templates that follow the writing-suite model: `ScribingImplement` for pre-modern writing tools, `InscribableSurface` for non-paper writeable surfaces, `PaperSheet` for single-surface sheets and scrolls, `Book` for codices, ledgers, stitched books, and palm-leaf bundles, `SealStamp` for seal tools, and `Sealable` for tamper-evident document and archive targets. Modern `Biro`, `Pencil`, `Crayon`, and A-size paper defaults are not part of the default medieval catalogue.

Historical research anchors used for this reference include:

- The Metropolitan Museum of Art, "The Art of the Book in the Middle Ages," especially its summary of medieval book production as handmade, labour-intensive work involving parchment preparation, quire sewing, ink mixing, pen preparation, page ruling, scribal copying, and illumination.
- UNESCO World Heritage Centre, "Haeinsa Temple Janggyeong Panjeon, the Depositories for the Tripitaka Koreana Woodblocks," for the 13th-century Goryeo woodblock-printing anchor.
- The existing FutureMUD antiquity writing implementation, which already establishes a component and item-family pattern for papyrus, parchment, scrolls, codices, wax tablets, clay tablets, wooden blocks, ostraca, reed pens, quills, ink brushes, charcoal sticks, styluses, ink goods, and document containers.

## Remaining future work

1. **Initial sealed-state policy:** Decide whether any prototypes should spawn already sealed with a specific impression. The available `Sealable_*` target components support sealing, inspection, breaking, and residue through gameplay; pre-seeded sealed state may require item-instance setup rather than ordinary prototype component assignment.
2. **Specific material additions:** The catalogue proceeds with exact existing materials such as `paper`, `parchment`, `papyrus`, `leaf`, `birch`, `bamboo`, `reed`, `feather`, `soot`, and `charcoal`. Add more specific `rag paper`, `palm leaf`, `birch bark`, `vellum`, or `ink` materials only if the gameworld wants those distinctions to matter as primary material values rather than as descriptive text.
3. **Ink-as-liquid policy:** Confirm whether an exact liquid `ink` exists in the active liquid list. If not, ink pots should remain ordinary props or containers, and solid ink cakes/sticks should use solid pigment materials such as `soot`, `charcoal`, `cinnabar`, `azurite`, or `malachite`.
4. **Board use:** Decide whether any actual `Board` component prototypes and board records should be configured. Otherwise, public notice boards should be inert or `InscribableSurface` items.
5. **Religious-content policy:** Decide whether generic labels like `gospel book`, `psalter`, `sutra scroll`, `Qur'an stand`, or `temple manuscript` are acceptable public item names, or whether even those should be skin-only in the gameworld.
6. **Craft recipe pass:** Convert the production-tool rows into craft chains and one-step-back stock recipes after the finished-good and full-description catalogues are accepted.
