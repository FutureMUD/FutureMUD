# 5. Naming and ethnicity plan

## 5.1 Fixed interface and preservation

Names remain ethnicity-first. This tranche adds no adopted-name chooser, new name-profile selection screen or extra name-identity system. A dedicated local NameCulture may reuse a common structural template without sharing all its random profiles. This avoids suggestions leaking across unrelated peoples.

Original name strings, weights, genders, usages, regexes, styles and accents remain recoverable. A corrected active default must not erase a source element. The coding agent performs a source-preservation export before refactoring.

## 5.2 Targeted repertoires delivered

Full entries and profile recipes are in `data/targeted_name_corpora.json`. Dates denote individual records only where marked; otherwise they are source-wide ranges or expressly unknown. Liturgical and literary entries are named as such, not passed off as ordinary birth records. All shown display forms are Latin-1; the original source form is stored separately.

| Repertoire | Masculine entries | Feminine entries | Boundary |
|---|---:|---:|---|
| Finnish Documentary Names | 30 | 31 | Bounded source/era scope; see metadata. |
| Lithuanian Register Names | 26 | 39 | Bounded source/era scope; see metadata. |
| Latvian Documentary Names | 30 | 14 | Fourteen feminine name groups in the inspected selection. This is not a finding that the language has only fourteen attested female names. Use this bounded repertoire without padding variants. |
| Estonian Documentary Names | 20 | 10 | Ten feminine groups in the inspected material. Some belong to the multilingual urban record environment rather than securely identified ethnic Estonian women. No artificial feminine derivation or modern-name padding. |
| Old Prussian Personal Names | 30 | 0 | No defensible new feminine pool was extracted. The specific 2023 women-identification study was accessible only as an abstract. Do not invent feminine suffixes, borrow neighbouring female pools, or advertise a complete replacement. Preserve the prior profile as legacy, without claiming new validation; no automatic activation of a replacement across all genders. |
| Romanian Household Names | 28 | 17 | Seventeen feminine groups, varying by era. Dynastic sources do not establish a commoner distribution. Use equal provisional weights and retain the source bias in research metadata, not in player prose. |
| Coptic Christian Names | 34 | 40 | Bounded source/era scope; see metadata. |
| Syriac Christian Names | 20 | 20 | Bounded source/era scope; see metadata. |

The 389 entries are not 389 independently verified modern-language lemmas or proof of historical frequency. Lemma grouping is provisional where the source gives only documentary forms. Orthographic aliases must not be counted as extra independent names.

**Old Prussian feminine replacement remains incomplete and inactive.** Preserve the existing female profile as legacy without certifying it; do not fill the replacement from German, Lithuanian or other neighbouring inventories. This is the one wholly missing gendered replacement, not an instruction for Codex to invent one.

Smaller Latvian, Estonian and Romanian feminine sets are usable bounded selections within their source scope, but not comprehensive regional inventories. Some register evidence is multilingual and cannot prove a bearer’s ethnicity. Do not push the sixteenth-century Christian repertoires back into early pagan defaults.

Coptic and Syriac pools separate documentary atoms from devotional/literary names. Names appearing in a church calendar are evidence of that tradition, not of a particular ethnic bearer or frequency. Current romanisation may be used as presentation without claiming it was the spelling of a medieval manuscript.

## 5.3 Structure and generation

For each new local pool: one BirthName element; optional byname only where a supplied local recipe supports it. Default new byname random count is zero rather than inventing surnames. Existing valid byname inventories are preserved. Multiword names can be one element. No automatic title, saint prefix, regnal ordinal or universal hereditary surname requirement.

New morphology is deliberately avoided. Use the supplied atomic forms and existing proven gender-aware structures. Keep nonbinary/indeterminate fallback within the same local repertoire. An empty required repertoire must not pull random English names from the generic fallback. The inactive Old Prussian female replacement is explicitly exempted from activation, not from disclosure.

`data/naming_pattern_tests.json` supplies constructed test fixtures. They test parsing/rendering, not attestation of a full historical person. Test all existing NameStyles, optional-element spacing, compound values, case preservation and Latin-1 input/output.

## 5.4 Existing naming-family reuse decisions

### Roman civic and Latin

Key: `names.roman`. Eras: antiquity.
Preserve the existing Roman citizen structures, sex-specific patterns and status distinctions. Reuse the current assigned name culture. Do not add an adopted-name selector or impose three names on every bearer.
Action: preserve-existing-status-structures-without-new-choice-ui.
Retained source modules: CultureSeeder.Names.cs.
Sources: R04, R08, H07.

### Antiquity regional inventories

Key: `names.ancient-regional`. Eras: antiquity.
Reuse each existing regional structure; preserve dated scope. Ancient Veneti are not Renaissance Venetians; city-Roman and eastern Greek-Roman are not the same canonical identity.
Action: preserve-entire-source-catalogue.
Retained source modules: CultureSeeder.Names.cs, CultureSeeder.Heritage.Antiquity.cs.
Research/implementation boundary: Ancient Egyptian, poorly recorded tribal and geographically broad inventories need per-record checks before new defaults are asserted.
Sources: R04, R08, H17, H18, H24.

### Early English and Anglo-Danish

Key: `names.english.preconquest`. Eras: darkages, medieval.
Given name plus optional patronymic, byname or locality, following the existing patterns. Preserve native compound names and supported thorn, eth and ash; do not carry the whole pre-Conquest frequency distribution into Renaissance defaults.
Action: reuse-existing-distinct-profiles.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs.
Sources: R06, H02, H03.

### Norman and Anglo-Norman

Key: `names.norman`. Eras: darkages, medieval.
Reuse continental and insular inventories. Retain inherited versus literal locative particles. A regional Norman or marcher byname is not proof of noble rank.
Action: reuse-existing-distinct-profiles.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs.
Sources: R04, R05, R06, R07.

### Later English

Key: `names.english.later`. Eras: medieval, renaissance, earlymodern.
Reuse later medieval given names and byname/surname stock. Use a transitional profile for non-hereditary bynames, and the same hereditary-family form for Renaissance and Early Modern unless a specific additional repertoire is justified.
Action: share-Renaissance-and-EarlyModern-baseline.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Separate demonstrably later additions or specialised religious naming fashions; do not replace the entire list.
Sources: R04, R05, R06, R07.

### French

Key: `names.french`. Eras: medieval, renaissance, earlymodern.
Reuse given-plus-byname/family structure as appropriate. Retain local forms and particles without automatic English title casing.
Action: shared-inventory-period-profile.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### Occitan

Key: `names.occitan`. Eras: medieval, renaissance, earlymodern.
A structurally compatible French name culture can be reused; do not silently replace an Occitan given-name repertoire with exclusively northern French forms.
Action: reuse-structure-not-assume-inventory-equivalence.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Identify local attested forms in the existing corpus; a new Occitan-specific profile remains review-gated.
Sources: R04, R05, R06, R07.

### Frankish

Key: `names.frankish`. Eras: darkages.
Keep existing early continental names and patronymic/byname structures; a single political Frankish label does not mandate a single vernacular.
Action: reuse.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs.
Sources: R04, R05, R06, R07.

### German regional traditions

Key: `names.german`. Eras: medieval, renaissance, earlymodern.
Reuse most inventory; distinguish a literal patronymic or occupational byname from an inherited surname when profiles differ. Multiple regional ethnicities may reference the same name culture.
Action: reuse-common-catalogue.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Frisian-specific naming should not be certified by the generic Imperial German assignment alone.
Sources: R04, R05, R06, R07.

### Low Countries

Key: `names.lowcountries`. Eras: medieval, renaissance, earlymodern.
Preserve patronymic and family/byname alternatives and particles. Do not require a fixed modern hereditary surname for all rural households.
Action: reuse-structure-and-inventory-with-optional-elements.
Retained source modules: CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### Italian regional traditions

Key: `names.italian`. Eras: medieval, renaissance, earlymodern.
Shared name culture with existing regional random profiles; retain patronymics, localities and family labels. Do not make every household bear a famous patrician family name.
Action: reuse.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Sardinian and other clearly distinct local repertoires require an explicit profile rather than proof by political geography.
Sources: R04, R05, R06, R07.

### Christian Iberian traditions

Key: `names.iberian`. Eras: medieval, renaissance, earlymodern.
Share structural templates, not one compulsory Castilian lexicon. Preserve given, patronymic and family/locative elements; two modern legal surnames are not a universal requirement.
Action: regional-profile-overlay.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Catalan, Portuguese, Galician and Aragonese inventories must be individually identifiable before replacing their defaults.
Sources: R04, R05, R06, R07.

### Basque

Key: `names.basque`. Eras: darkages, medieval, renaissance, earlymodern.
Reuse existing given and family/locality names, retaining native and contact-language forms in appropriately marked profiles.
Action: reuse.
Retained source modules: CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### Irish and Scottish Gaelic

Key: `names.gaelic`. Eras: darkages, medieval, renaissance, earlymodern.
Keep given names, patronymics and lineage names distinct; retain gender-conditioned grammar from existing profiles. Shared underlying roots need not mean one identical Irish/Scottish orthographic inventory.
Action: reuse-and-select-local-profile.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Do not invent regular female surnames by mechanically appending an English suffix.
Sources: R04, R05, R06, R07.

### Welsh

Key: `names.welsh`. Eras: darkages, medieval, renaissance, earlymodern.
Provide a patronymic-chain profile and a later inherited-surname profile drawing on retained inventory. Change is overlapping and household-dependent; ap/ab/ferch are components, not mandatory hyphenation.
Action: genuine-structure-variant.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Any new surname morphology needs attestation; this recipe does not impose one universal date of adoption.
Sources: R04, R05, R06, R07.

### Breton and Cornish

Key: `names.breton-cornish`. Eras: darkages, medieval, renaissance, earlymodern.
Retain Breton material as Breton. A related language is not permission to relabel that whole inventory as Cornish.
Action: reuse-Breton; retain-Cornish-if-already-authored.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: New Cornish profile is a separate evidence gate if the retained corpus does not provide it.
Sources: R04, R05, R06, R07.

### Norse and Scandinavian

Key: `names.norse`. Eras: darkages, medieval, renaissance, earlymodern.
Patronymics remain the baseline where appropriate, with gendered forms and local spelling. Preserve elite family names as optional special profiles rather than imposing them on all Scandinavians.
Action: share-roots-with-genuine-local-profiles.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Icelandic and Norwegian must not be assumed identical to Danish solely because the old mapping says so.
Sources: R04, R05, R06, R07.

### Polish

Key: `names.polish`. Eras: medieval, renaissance, earlymodern.
Retain given names and existing gender-sensitive family forms. Noble house/heraldic association is not automatically a surname, and Polish noble culture does not force a non-Polish ethnicity to change its name.
Action: reuse.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Validate particles and adjective agreement on any newly added forms.
Sources: R04, R05, R06, R07.

### Czech and Slovak

Key: `names.westslavic`. Eras: darkages, medieval, renaissance, earlymodern.
Share compatible structural templates while keeping Czech and Slovak lexical forms distinct in profiles. Preserve attested bynames rather than requiring a modern civil surname system everywhere.
Action: retain-source-and-split-profile-only-as-needed.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Existing umbrella inventory must be partitioned or reviewed; do not certify all elements for both communities.
Sources: R04, R05, R06, R07.

### Rus, Ruthenian and Russian

Key: `names.eastslavic`. Eras: darkages, medieval, renaissance, earlymodern.
Given name plus patronymic and optional byname/family. Use appropriate local forms; do not require universal modern three-part official naming. Cossack culture uses the selected heritage repertoire, often an East Slavic one.
Action: reuse-and-correct-Cossack-default.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### South Slavic regional traditions

Key: `names.southslavic`. Eras: medieval, renaissance, earlymodern.
A compatible template may be shared, but Serbian, Croatian and other local inventories are not simply Czech names. Keep local patronymic morphology and religiously conditioned repertoire options.
Action: retain-template; review-lexical-defaults.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: New production profile must not be generated by blindly swapping suffixes on a northern Slavic list.
Sources: R04, R05, R06, R07.

### Hungarian

Key: `names.hungarian`. Eras: darkages, medieval, renaissance, earlymodern.
Keep given/family ordering and patronymic/byname alternatives from the appropriate source; preserve early Magyar repertoire separately from later Christian defaults.
Action: reuse-existing-genuine-variants.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### Baltic corrections

Key: `names.baltic`. Eras: medieval, renaissance, earlymodern.
Do not use the legacy label as an inventory claim. Lithuanian, Latvian and Old Prussian need distinct given-name evidence and byname grammar; compatible structural patterns can still be shared.
Action: use-bounded-local-inventories-and-preserve-originals.
Retained source modules: CultureSeeder.Names.cs.
Supplied pools: `names.target.lithuanian`, `names.target.latvian`, `names.target.old-prussian`.
Research/implementation boundary: Old Prussian feminine replacement remains inactive. Latvian 14-form female pool is a disclosed bounded sample; do not pad spelling variants.
Sources: R04, R05, H21, H22.

### Finnic corrections

Key: `names.finnic`. Eras: darkages, medieval, renaissance, earlymodern.
Estonian and Finnish are not interchangeable simply because they are Finnic. Retain actual existing local names, distinguish documentary German/Latin spellings from vernacular reconstructions.
Action: use-separate-local-documentary-inventories.
Retained source modules: CultureSeeder.Names.cs.
Supplied pools: `names.target.finnish`, `names.target.estonian`.
Research/implementation boundary: Respect per-entry era scope and the bounded Estonian female sample; no universal Finno-Ugric pool.
Sources: R04, R05, H21.

### Vlach and Romanian correction

Key: `names.romanian`. Eras: medieval, renaissance, earlymodern.
Given name plus appropriate byname, parentage or family, with Romanian forms. Monastic names are an adopted-name option. Do not manufacture all bynames using a modern -escu rule.
Action: use-bounded-local-repertoire.
Retained source modules: CultureSeeder.Names.cs.
Supplied pools: `names.target.romanian`.
Research/implementation boundary: 28 masculine/17 feminine groups; elite-biased provenance, no unsupported frequency claims or productive surname generation.
Sources: R05, H23, H25.

### Greek and eastern Roman

Key: `names.greek`. Eras: darkages, medieval, renaissance, earlymodern.
Retain given and family/byname forms with their actual gender and grammar. Ancient and later Christian repertoires are not interchangeable, even when the pattern is compatible.
Action: reuse-existing-later-profiles.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Sources: R04, R05, R06, R07.

### Oghuz, Turkish and Ottoman

Key: `names.turkish`. Eras: medieval, renaissance, earlymodern.
Use personal name plus optional parentage, locality or established family/byname as appropriate. Avoid universal modern Turkish hereditary surnames. Ottoman culture alone does not force Turkish names.
Action: reuse-and-correct-status-mapping.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Keep Circassian, Kurdish and Arab repertoires distinct; do not inherit a Turkish pool merely from a shared Mamluk political template.
Sources: R04, R05, R06, R07.

### Arabic regional traditions

Key: `names.arabic`. Eras: darkages, medieval, renaissance, earlymodern.
Reuse ism, nasab and nisba structures. Kunya and laqab are optional use-context elements, not obligatory birth names. Separate gender-sensitive parentage forms and preserve local repertoire variants.
Action: share-structures-and-most-inventory.
Retained source modules: CultureSeeder.Packs.DarkAgesAndMedieval.cs, CultureSeeder.Packs.RenaissanceWorld.cs, CultureSeeder.Names.cs.
Research/implementation boundary: A Christian Arabic repertoire must be intentional, not a random subset of an exclusively Muslim generated list.
Sources: R04, R05, R06, R07.

### Iranian and Persianate

Key: `names.persian`. Eras: darkages, medieval, renaissance, earlymodern.
Reuse New Persian and Persianate structures across medieval-to-early-modern packs when appropriate; personal name plus parentage/locality/affiliation, not a mandatory modern legal surname.
Action: reuse-New-Persian-baseline.
Retained source modules: CultureSeeder.Names.cs, CultureSeeder.Packs.RenaissanceWorld.cs.
Research/implementation boundary: Pre-Islamic Iranian repertoire remains separate; a Timurid court inventory does not make every bearer ethnically Persian.
Sources: R04, R07, H08, H15.

### Armenian, Georgian and Kurdish

Key: `names.caucasus`. Eras: medieval, renaissance, earlymodern.
Reuse the existing distinct name cultures and their substantial pools. Adjust period guidance and optional affiliations; do not duplicate a whole inventory solely for Early Modern.
Action: reuse-existing-distinct-catalogues.
Retained source modules: CultureSeeder.Packs.RenaissanceWorld.cs.
Sources: R04, R05, R06, R07.

### Jewish regional and religious naming

Key: `names.jewish`. Eras: darkages, medieval, renaissance, earlymodern.
Keep Hebrew religious names and local everyday names as legitimate distinct use profiles. Retain attested inherited Sephardi family names; do not impose universal fixed Ashkenazi surnames before 1750.
Action: reuse-with-region-and-use-profile.
Retained source modules: CultureSeeder.Names.cs.
Research/implementation boundary: Do not let one communal culture force all Jewish characters to share a home vernacular or one naming inventory.
Sources: R04, R05, R06, R07.

### Coptic and Syriac Christian corrections

Key: `names.coptic-syriac`. Eras: darkages, medieval, renaissance, earlymodern.
Choose a reviewed Coptic/Syriac or local Christian Arabic repertoire; preserve parentage/locality structure. Greek-looking names need local attestations, not automatic import of a whole ancient Greek pool.
Action: use-separate-traditional-and-documentary-pools.
Retained source modules: CultureSeeder.Packs.RenaissanceWorld.cs, CultureSeeder.Packs.DarkAgesAndMedieval.cs.
Supplied pools: `names.target.coptic-christian`, `names.target.syriac-christian`.
Research/implementation boundary: Entry evidence classes must remain visible in research. Do not claim each liturgical name has a contemporary birth-record attestation.
Sources: R06, R07, H18, H24.

### Amazigh and Maghrebi regional traditions

Key: `names.amazigh`. Eras: darkages, medieval, renaissance, earlymodern.
Keep separate local and Arabic-contact repertoire choices. Shared Islamic names do not justify deleting Amazigh names or assuming all regional languages have the same morphology.
Action: reuse-by-local-profile.
Retained source modules: CultureSeeder.Packs.RenaissanceWorld.cs, CultureSeeder.Names.cs.
Research/implementation boundary: Do not infer a uniform pre-Islamic inventory from the later Maghrebi pool.
Sources: R04, R05, R06, R07.

## 5.5 Ethnicity overlays and fixed native grants

Blockquotes are the contemporary replacement descriptions. Binding notes are implementation metadata. Exact retained regional defaults take precedence over broad compatibility identities.

### German

> The Germans belong to the German-speaking lands of central Europe. Their towns, lordships and rural communities preserve strong local identities, while related speech and the affairs of the Empire connect families across the region.

Key: `ethnicity.german`.
Fixed native bindings: medieval: german.middle-high; renaissance: german.early-new-high; earlymodern: german.early-new-high.

### Austrian

> The Austrians belong to the German-speaking communities of the Danube lands and the neighbouring Alpine valleys. Towns, estates and mountain settlements sustain their regional customs and their ties to the wider Empire.

Key: `ethnicity.austrian`.
Fixed native bindings: medieval: german.middle-high; renaissance: german.early-new-high; earlymodern: german.early-new-high.

### Dutch

> The Dutch inhabit the towns, lowlands and waterways near the mouths of the Rhine and Meuse. Their speech and local customs unite busy trading communities with the farms and villages of the surrounding countryside.

Key: `ethnicity.dutch`.
Fixed native bindings: medieval: dutch.middle; renaissance: dutch; earlymodern: dutch.

### French

> The French belong to the French-speaking communities of the kingdom and its neighbouring lands. Royal towns, provincial lordships and village communities preserve their own customs within a wider world of related speech and allegiance.

Key: `ethnicity.french`.
Fixed native bindings: medieval: french.old; renaissance: french.middle; earlymodern: french.earlymodern.

### Occitan

> The Occitans belong to the southern lands where the tongue of oc is spoken. Their towns and rural communities preserve local customs and a rich tradition of verse, with connections across the mountains and shores of the western Mediterranean.

Key: `ethnicity.occitan`.
Fixed native bindings: medieval: occitan; renaissance: occitan; earlymodern: occitan.

### English

> The English inhabit the shires, towns and villages of England. Shared speech and the institutions of the kingdom connect communities whose local customs and family loyalties remain strong.

Key: `ethnicity.english`.
Fixed native bindings: medieval: english.middle; renaissance: english.middle; earlymodern: english.earlymodern.

### Venetian

> The Venetians belong to Venice, its lagoon and the neighbouring communities of the northern Adriatic. Their speech, civic loyalties and family connections join the city to its surrounding lands and maritime settlements.

Key: `ethnicity.venetian`.
Fixed native bindings: medieval: venetian; renaissance: venetian; earlymodern: venetian.

### Florentine

> The Florentines belong to Florence and its surrounding Tuscan country. Their families carry the speech and customs of a city whose workshops, commerce and public affairs reach deeply into neighbouring towns and villages.

Key: `ethnicity.florentine`.
Fixed native bindings: medieval: italian; renaissance: italian; earlymodern: italian.

### Milanese

> The Milanese belong to Milan and the neighbouring Lombard communities of the Po valley. Their speech and local traditions connect a powerful urban centre with the estates, villages and market towns of its surrounding lands.

Key: `ethnicity.milanese`.
Fixed native bindings: medieval: legacy:Lombard; renaissance: legacy:Lombard; earlymodern: legacy:Lombard.

### Neapolitan

> The Neapolitans belong to Naples and the neighbouring communities of southern Italy. Local speech, family ties and the affairs of the kingdom connect the great city with towns and villages throughout its surrounding country.

Key: `ethnicity.neapolitan`.
Fixed native bindings: medieval: legacy:Neapolitan; renaissance: legacy:Neapolitan; earlymodern: legacy:Neapolitan.

### Sicilian

> The Sicilians belong to the towns and countryside of Sicily. Their speech and family traditions reflect the island's close connections with the Italian mainland and the many ports of the Mediterranean.

Key: `ethnicity.sicilian`.
Fixed native bindings: medieval: sicilian; renaissance: sicilian; earlymodern: sicilian.

### Corsican

> The Corsicans inhabit the mountains, villages and coastal towns of Corsica. Strong family and local ties sustain their communities, while maritime connections bring them into frequent dealings with the neighbouring islands and Italian ports.

Key: `ethnicity.corsican`.
Fixed native bindings: medieval: italian; renaissance: italian; earlymodern: italian.

### Sardinian

> The Sardinians belong to the towns, farming districts and pastoral communities of Sardinia. Their language and local customs give the island a distinct character, maintained through family connections and the affairs of its villages.

Key: `ethnicity.sardinian`.
Fixed native bindings: medieval: sardinian; renaissance: sardinian; earlymodern: sardinian.

### Castilian

> The Castilians belong to Castile's towns, estates and village communities. Their speech and regional traditions connect the northern uplands with the expanding settlements and cultivated lands farther south.

Key: `ethnicity.castilian`.
Fixed native bindings: medieval: castilian.old; renaissance: castilian; earlymodern: castilian.

### Catalan

> The Catalans belong to the Catalan-speaking towns and countryside of the eastern Iberian lands. Family connections and local institutions link their inland communities to ports and settlements across the western Mediterranean.

Key: `ethnicity.catalan`.
Fixed native bindings: medieval: catalan; renaissance: catalan; earlymodern: catalan.

### Galician

> The Galicians inhabit the valleys, villages and coastal communities of northwestern Iberia. Their speech and local traditions connect the Atlantic shore with the neighbouring lands of León and Portugal.

Key: `ethnicity.galician`.
Fixed native bindings: medieval: galician-portuguese; renaissance: galician; earlymodern: galician.

### Portuguese

> The Portuguese belong to the towns, villages and coastal communities of Portugal. Their speech and family traditions follow the Atlantic-facing lands from the northern valleys to the settlements and ports farther south.

Key: `ethnicity.portuguese`.
Fixed native bindings: medieval: galician-portuguese; renaissance: portuguese; earlymodern: portuguese.

### Basque

> The Basques inhabit the valleys and coastal lands on both sides of the western Pyrenees. Their distinctive language and local customs endure among communities bound by family ties, neighbouring settlements and the affairs of their valleys.

Key: `ethnicity.basque`.
Fixed native bindings: medieval: basque; renaissance: basque; earlymodern: basque.

### Welsh

> The Welsh belong to the Welsh-speaking communities of Wales and its borderlands. Kinship, local custom and a strong tradition of poetry sustain their identity among the hills, farms and towns of their country.

Key: `ethnicity.welsh`.
Fixed native bindings: darkages: welsh; medieval: welsh; renaissance: welsh; earlymodern: welsh.

### Breton

> The Bretons belong to the Breton-speaking communities of Brittany. Their local traditions, family ties and maritime connections join villages and coastal towns to the wider affairs of the peninsula.

Key: `ethnicity.breton`.
Fixed native bindings: darkages: breton; medieval: breton; renaissance: breton; earlymodern: breton.

### Irish Gael

> The Irish Gaels belong to the Gaelic-speaking kindreds and communities of Ireland. Lineage, local custom and the learning of poets and genealogists bind families to their lands and neighbouring lordships.

Key: `ethnicity.irish-gael`.
Fixed native bindings: darkages: gaelic.medieval; medieval: gaelic.medieval; renaissance: irish; earlymodern: irish.

### Scottish Gael

> The Scottish Gaels belong to the Gaelic-speaking communities of the Highlands and western islands of Scotland. Kindred, lordship and sea routes connect their households, while learned families preserve the words and histories of their people.

Key: `ethnicity.scottish-gael`.
Fixed native bindings: darkages: gaelic.medieval; medieval: gaelic.medieval; renaissance: scottish-gaelic; earlymodern: scottish-gaelic.

### Lowland Scot

> The Lowland Scots belong to the burghs, farming districts and border communities of Scotland's lowlands. Their speech and local institutions link neighbouring families to the affairs of the Scottish crown.

Key: `ethnicity.lowland-scot`.
Fixed native bindings: medieval: scots; renaissance: scots; earlymodern: scots.

### Polish

> The Poles belong to the Polish-speaking communities of Poland and the neighbouring lands. Towns, noble estates and villages preserve their regional traditions within a wider world of related speech and shared institutions.

Key: `ethnicity.polish`.
Fixed native bindings: medieval: polish; renaissance: polish; earlymodern: polish.

### Czech

> The Czechs belong to the Czech-speaking communities of Bohemia and Moravia. Their families maintain local traditions in towns, estates and villages whose affairs are closely connected to the lands of the Bohemian crown.

Key: `ethnicity.czech`.
Fixed native bindings: medieval: czech; renaissance: czech; earlymodern: czech.

### Slovak

> The Slovaks belong to the Slavic-speaking communities of the northern Hungarian lands. Mountain valleys, market towns and farming settlements preserve their local speech and family traditions alongside their dealings with neighbouring peoples.

Key: `ethnicity.slovak`.
Fixed native bindings: medieval: slovak; renaissance: slovak; earlymodern: slovak.

### Ruthenian

> The Ruthenians belong to the Rus communities of the western and southern lands. Related speech, churches and family traditions connect their towns and villages across the territories of local princes and the Polish and Lithuanian rulers.

Key: `ethnicity.ruthenian`.
Fixed native bindings: medieval: slavic.old-east; renaissance: ruthenian; earlymodern: ruthenian.

### Russian

> The Russians belong to the Rus communities centred upon Muscovy and the northeastern lands. Shared speech, religious institutions and family connections join their towns and villages to the affairs of the Muscovite rulers.

Key: `ethnicity.russian`.
Fixed native bindings: medieval: slavic.old-east; renaissance: russian; earlymodern: russian.

### Croat

> The Croats belong to the Croatian lands and their neighbouring Adriatic and inland communities. Local speech, family tradition and the affairs of towns and lordships sustain their identity across coast and countryside.

Key: `ethnicity.croat`.
Fixed native bindings: medieval: legacy:Serbo-Croatian; renaissance: legacy:Serbo-Croatian; earlymodern: legacy:Serbo-Croatian.

### Serb

> The Serbs belong to the Serbian lands and neighbouring communities of the Balkan interior. Their speech, family traditions and churches connect towns and villages across the territories of local rulers and lords.

Key: `ethnicity.serb`.
Fixed native bindings: medieval: legacy:Serbo-Croatian; renaissance: legacy:Serbo-Croatian; earlymodern: legacy:Serbo-Croatian.

### Bosnian

> The Bosnians belong to the towns, valleys and upland communities of Bosnia. Local speech, family connections and the affairs of neighbouring lordships sustain their regional identity among the peoples of the Balkan lands.

Key: `ethnicity.bosnian`.
Fixed native bindings: medieval: legacy:Serbo-Croatian; renaissance: legacy:Serbo-Croatian; earlymodern: legacy:Serbo-Croatian.

### Vlach

> The Vlachs are a Romance-speaking people of the Danube lands and the Balkan mountains. Their communities include farmers, herdsmen and townspeople, joined by local speech, kinship and routes between upland and lowland settlements.

Key: `ethnicity.vlach`.
Fixed native bindings: medieval: romanian; renaissance: romanian; earlymodern: romanian.

### Lithuanian

> The Lithuanians belong to the Lithuanian-speaking communities of the eastern Baltic lands. Family and regional traditions connect the countryside with towns and noble households across the grand duchy's territories.

Key: `ethnicity.lithuanian`.
Fixed native bindings: medieval: lithuanian; renaissance: lithuanian; earlymodern: lithuanian.

### Latvian

> The Latvians belong to the Latvian-speaking communities of the eastern Baltic coast and its inland districts. Local customs and family connections link their farms, villages and market settlements along rivers and trading routes.

Key: `ethnicity.latvian`.
Fixed native bindings: medieval: latvian; renaissance: latvian; earlymodern: latvian.

### Old Prussian

> The Prussians belong to the Baltic-speaking communities of Prussia. Their inherited speech and local traditions persist among the settlements and countryside beside the towns and institutions of German-speaking neighbours and rulers.

Key: `ethnicity.old-prussian`.
Fixed native bindings: darkages: old-prussian; medieval: old-prussian; renaissance: old-prussian; earlymodern: old-prussian.

### Estonian

> The Estonians belong to the Finnic-speaking communities of the eastern Baltic coast and nearby islands. Village ties, regional speech and family traditions connect their farms and settlements to neighbouring towns and ports.

Key: `ethnicity.estonian`.
Fixed native bindings: darkages: estonian; medieval: estonian; renaissance: estonian; earlymodern: estonian.

### Finnish

> The Finns belong to the Finnic-speaking communities of the northern forests, lake districts and Baltic shores. Local speech, farming, fishing and family connections sustain settlements linked by waterways and seasonal routes.

Key: `ethnicity.finnish`.
Fixed native bindings: darkages: finnish; medieval: finnish; renaissance: finnish; earlymodern: finnish.

### Hungarian

> The Hungarians, also called Magyars, belong to the Hungarian-speaking communities of the Carpathian basin. Their speech and regional traditions connect noble households, market towns and villages across the lands of Hungary.

Key: `ethnicity.hungarian`.
Fixed native bindings: darkages: hungarian; medieval: hungarian; renaissance: hungarian; earlymodern: hungarian.

### Danish

> The Danes inhabit Jutland and the Danish islands. Their language and family traditions connect farms, towns and coastal settlements whose affairs are closely bound to the seas and the Danish crown.

Key: `ethnicity.danish`.
Fixed native bindings: medieval: danish; renaissance: danish; earlymodern: danish.

### Swedish

> The Swedes belong to the Swedish-speaking communities of the northern kingdom. Farming districts, woodland settlements, mines and ports are joined by local customs, family connections and the affairs of the crown.

Key: `ethnicity.swedish`.
Fixed native bindings: medieval: swedish; renaissance: swedish; earlymodern: swedish.

### Norwegian

> The Norwegians belong to the fjords, valleys and coastal communities of Norway. Their speech and regional customs follow the routes between mountain farms, fishing settlements and towns facing the northern seas.

Key: `ethnicity.norwegian`.
Fixed native bindings: medieval: norwegian; renaissance: norwegian; earlymodern: norwegian.

### Icelandic

> The Icelanders belong to the farming households and coastal communities of Iceland. Family connections, assemblies and a strong tradition of remembered and written accounts sustain their society across widely separated settlements.

Key: `ethnicity.icelandic`.
Fixed native bindings: medieval: icelandic; renaissance: icelandic; earlymodern: icelandic.

### Greek Roman

> The Greek-speaking Romans belong to the towns and countryside of the eastern Roman world. Roman identity, Greek speech and the traditions of their churches connect families across the Aegean, Anatolia and neighbouring Mediterranean lands.

Key: `ethnicity.greek-roman`.
Fixed native bindings: darkages: greek.medieval; medieval: greek.medieval; renaissance: greek.medieval; earlymodern: greek.medieval.

### Turkish

> The Turks belong to Turkish-speaking communities of Anatolia and neighbouring lands. Their settled towns, farming villages and pastoral households maintain related speech and family traditions alongside their ties to rulers and local lords.

Key: `ethnicity.turkish`.
Fixed native bindings: medieval: turkic.oghuz; renaissance: turkish.ottoman; earlymodern: turkish.ottoman.

### Persian

> The Persians belong to the Persian-speaking communities of Iran and neighbouring lands. Their towns, villages and learned households share a language of daily life, letters and public affairs that travels far beyond their home districts.

Key: `ethnicity.persian`.
Fixed native bindings: darkages: persian.new; medieval: persian.new; renaissance: persian.new; earlymodern: persian.new.

### Arab

> The Arabs belong to communities linked by Arabic speech, family descent and local traditions. Their households are found in towns, villages and tribal lands, joined through kinship, trade and dealings among neighbouring peoples.

Key: `ethnicity.arab`.
Fixed native bindings: darkages: arabic.mashriqi; medieval: arabic.mashriqi; renaissance: arabic.mashriqi; earlymodern: arabic.mashriqi.

### Egyptian Arab

> The Egyptian Arabs belong to Egypt's Arabic-speaking towns, villages and tribal communities. Their families share the life of the Nile lands and maintain connections through local custom, commerce and the speech of daily affairs.

Key: `ethnicity.egyptian-arab`.
Fixed native bindings: darkages: arabic.mashriqi; medieval: arabic.mashriqi; renaissance: arabic.mashriqi; earlymodern: arabic.mashriqi.

### Copt

> The Copts belong to Egypt's Christian communities. Family ties, village life, churches and monasteries sustain their people, while Coptic readings and worship preserve a cherished religious inheritance alongside the tongues of daily life.

Key: `ethnicity.copt`.
Fixed native bindings: darkages: coptic; medieval: arabic.mashriqi; renaissance: arabic.mashriqi; earlymodern: arabic.mashriqi.

### Syriac

> The Syriac Christians belong to communities across Mesopotamia and neighbouring lands. Churches, monasteries and family connections sustain their traditions, while Syriac books and worship join towns and villages whose everyday speech includes Aramaic and Arabic.

Key: `ethnicity.syriac`.
Fixed native bindings: darkages: syriac; medieval: syriac; renaissance: syriac; earlymodern: syriac.

### Armenian

> The Armenians belong to the Armenian-speaking towns, villages and highland communities of Armenia and their settlements abroad. Family connections, churches and a shared written tradition sustain their people across the lands between neighbouring empires.

Key: `ethnicity.armenian`.
Fixed native bindings: darkages: armenian; medieval: armenian; renaissance: armenian; earlymodern: armenian.

### Georgian

> The Georgians belong to the Georgian-speaking towns and countryside south of the Caucasus. Family traditions, local lordships and the institutions of their churches connect mountain valleys with fertile lowlands and market towns.

Key: `ethnicity.georgian`.
Fixed native bindings: darkages: georgian; medieval: georgian; renaissance: georgian; earlymodern: georgian.

### Kurdish

> The Kurds belong to the highland towns, villages and tribal communities between the Anatolian, Mesopotamian and Iranian lands. Local speech, kindred and the affairs of neighbouring lordships sustain their communities across mountain routes and cultivated valleys.

Key: `ethnicity.kurdish`.
Fixed native bindings: medieval: kurdish.kurmanji; renaissance: kurdish.kurmanji; earlymodern: kurdish.kurmanji.

### Ashkenazi Jewish

> The Ashkenazi Jews belong to the Jewish communities of the German lands and their settlements farther east. Family networks, congregations and learned traditions join them across towns and regions, alongside the tongues used in daily life and trade.

Key: `ethnicity.ashkenazi-jewish`.
Fixed native bindings: darkages: german.old-high; medieval: yiddish; renaissance: yiddish; earlymodern: yiddish.

### Sephardic Jewish

> The Sephardic Jews belong to the Jewish communities of Sepharad and to families who carry its traditions abroad. Congregations, household customs and learned connections sustain their identity among the towns and ports of Iberia and the Mediterranean.

Key: `ethnicity.sephardic-jewish`.
Fixed native bindings: darkages: arabic.andalusi; medieval: castilian.old; renaissance: judeo-spanish; earlymodern: judeo-spanish.

### Near Eastern Jewish

> The Jewish communities of Syria, Mesopotamia and Iran maintain their congregations, family traditions and centres of learning. Local languages accompany daily affairs, while sacred texts and correspondence connect them with other Jewish communities across distant lands.

Key: `ethnicity.near-eastern-jewish`.
Fixed native bindings: darkages: aramaic; medieval: aramaic; renaissance: aramaic; earlymodern: aramaic.

### Maghrebi Arab

> The Maghrebi Arabs belong to Arabic-speaking towns, villages and tribal communities of the western lands. Family connections and local customs join coastal ports, cultivated plains and inland settlements to wider networks of trade and kinship.

Key: `ethnicity.maghrebi-arab`.
Fixed native bindings: darkages: arabic.maghrebi; medieval: arabic.maghrebi; renaissance: arabic.maghrebi; earlymodern: arabic.maghrebi.

### Andalusi Arab

> The Andalusi Arabs belong to the Arabic-speaking communities of al-Andalus and families established abroad from those lands. Urban, rural and learned traditions sustain their households, alongside ties to neighbouring Iberian and Maghrebi communities.

Key: `ethnicity.andalusi-arab`.
Fixed native bindings: darkages: arabic.andalusi; medieval: arabic.andalusi; renaissance: arabic.andalusi; earlymodern: arabic.andalusi.

### Amazigh

> The Amazigh belong to the mountain, valley and rural communities of the Maghreb. Their local tongues, kindreds and customary institutions maintain distinct traditions across districts linked by pastoral routes, cultivation and trade.

Key: `ethnicity.amazigh`.
Fixed native bindings: darkages: amazigh.tashelhit; medieval: amazigh.tashelhit; renaissance: amazigh.tashelhit; earlymodern: amazigh.tashelhit.

### Anglo-Saxon

> The Anglo-Saxons belong to the English-speaking communities of Britain's southern and eastern lands. Kingdom, shire, kindred and local custom shape the affairs of their towns, estates and villages.

Key: `ethnicity.anglo-saxon`.
Fixed native bindings: darkages: english.old; medieval: english.old.

### Anglo-Danish English

> The Anglo-Danish English belong to communities shaped by the settlement of Danes among the English. Family connections, local law and the speech of both peoples give their towns and countryside a distinctive character.

Key: `ethnicity.anglo-danish-english`.
Fixed native bindings: darkages: english.old; medieval: english.old.

### Frank

> The Franks belong to the peoples of the Frankish realms, whose royal houses and great families command lands on either side of the Rhine. Regional speech and local traditions accompany a wider identity sustained through service, lordship and allegiance.

Key: `ethnicity.frank`.
Fixed native bindings: darkages: german.old-high; medieval: german.middle-high.

### Continental Saxon

> The continental Saxons belong to the Saxon lands of northern Germany. Their local speech, family traditions and communities distinguish them from neighbouring peoples along the coasts, plains and river valleys.

Key: `ethnicity.continental-saxon`.
Fixed native bindings: darkages: german.old-saxon; medieval: german.middle-low.

### Bavarian

> The Bavarians belong to the German-speaking communities of Bavaria and the neighbouring Alpine and Danubian lands. Regional speech and local traditions connect their towns, estates and valley settlements.

Key: `ethnicity.bavarian`.
Fixed native bindings: darkages: german.old-high; medieval: german.middle-high; renaissance: german.early-new-high; earlymodern: german.early-new-high.

### Alemannic Swabian

> The Alemannic and Swabian peoples belong to the southwestern German lands. Related speech, family connections and local traditions join communities along the upper rivers, wooded hills and approaches to the Alps.

Key: `ethnicity.alemannic-swabian`.
Fixed native bindings: darkages: german.old-high; medieval: german.middle-high; renaissance: german.early-new-high; earlymodern: german.early-new-high.

### Frisian

> The Frisians belong to the coastal communities of the North Sea. Their distinctive speech and local customs join farming settlements, islands and trading places through kinship and maritime connections.

Key: `ethnicity.frisian`.
Fixed native bindings: darkages: frisian; medieval: frisian; renaissance: frisian; earlymodern: frisian.

### Norman

> The Normans belong to the towns, estates and countryside of Normandy and their settlements abroad. Ducal allegiance, family connections and local speech bind their communities to the affairs of lords, ports and neighbouring lands.

Key: `ethnicity.norman`.
Fixed native bindings: darkages: norman.old; medieval: norman.old.

### Anglo-Norman

> The Anglo-Normans belong to families established in England and neighbouring lordships from Norman and French origins. Household traditions and French speech connect them across the Channel, alongside growing ties to the lands in which they live.

Key: `ethnicity.anglo-norman`.
Fixed native bindings: medieval: french.anglonorman.

### Norse-Gael

> The Norse-Gaels belong to communities formed where Norse settlers and Gaelic kindreds meet around the western seas. Family alliances, local speech and maritime connections join their households across islands, coasts and neighbouring lordships.

Key: `ethnicity.norse-gael`.
Fixed native bindings: darkages: gaelic.medieval; medieval: gaelic.medieval.
