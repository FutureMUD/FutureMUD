# 4. Language and script catalogue

## Final-decision integration note

Ethnic native grants are now automatic, with Native base 200 and no home/working-language chooser. See `ethnicity_language_defaults.json`, `legacy_native_language_rules.json` and document 2. Era selectors resolve fixed content; they are not extra skill groups. The 177-pair/354-direction intelligibility matrix and 16 conditional legacy pairs below remain unchanged. PR #730 supplies optional group infrastructure; this task consumes it.

Literacy and primary writing traditions are specified in `literacy_script_grants.json`. Keep the 15 broad script families below and all genuine retained legacy systems; do not give every compatible script for free. The separately added Lusitanian definition at the end closes a missing native-language binding without pretending it is an existing record.

## Stable stages, variable labels

This is an explicit binding/presentation specification. The blockquoted player-facing prose is contemporary setting text; original language/accent detail remains preserved in the source ledger and is retained in active prose where compliant. Preserve older stages as historical/learned content in later packs. A retained language is not automatically contemporary, a default home language or a free descendant-language skill.

The longer language descriptions below are supplied for linked-skill chargen guidance and known-language help. They do not imply an existing long-description column on the Language model. Keep the separate short unknown-language wording for listeners who cannot identify the tongue.

The English example intentionally follows the suggested naming progression. Medieval contains both Old and Middle English; which is used in a background is resolved by canonical stage and period guidance, not by the label English. These are gameplay pack labels, not claims about linguistic periodisation. [H20]

| Canonical key | Antiquity | Dark Ages | Medieval | Renaissance | Early Modern |
|---|---|---|---|---|---|
| `latin` | Latin | Latin | Latin | Latin | Latin |
| `greek.ancient` | Ancient Greek | Ancient Greek | Ancient Greek | Ancient Greek | Ancient Greek |
| `greek.koine` | Koine Greek | Koine Greek | Koine Greek | Koine Greek | Koine Greek |
| `greek.medieval` | — | Greek | Greek | Greek | Greek |
| `english.old` | — | Saxon | English | Old English | Old English |
| `english.middle` | — | — | Middle English | English | Middle English |
| `english.earlymodern` | — | — | — | Later English | English |
| `norman.old` | — | Norman | Norman | Old Norman | Old Norman |
| `french.anglonorman` | — | — | Anglo-Norman French | Anglo-Norman French | Anglo-Norman French |
| `french.old` | — | French | French | Old French | Old French |
| `french.middle` | — | — | — | French | Middle French |
| `french.earlymodern` | — | — | — | — | French |
| `occitan` | — | — | Occitan | Occitan | Occitan |
| `welsh` | — | Welsh | Welsh | Welsh | Welsh |
| `breton` | — | Breton | Breton | Breton | Breton |
| `cornish` | — | Cornish | Cornish | Cornish | Cornish |
| `gaelic.old` | — | Old Irish | Old Irish | Old Irish | Old Irish |
| `gaelic.medieval` | — | Medieval Gaelic | Medieval Gaelic | Medieval Gaelic | Medieval Gaelic |
| `irish` | — | — | — | Irish | Irish |
| `scottish-gaelic` | — | — | — | Scottish Gaelic | Scottish Gaelic |
| `scots` | — | — | Scots | Scots | Scots |
| `norse.old` | — | Old Norse | Old Norse | Old Norse | Old Norse |
| `danish` | — | — | Danish | Danish | Danish |
| `swedish` | — | — | Swedish | Swedish | Swedish |
| `norwegian` | — | — | Norwegian | Norwegian | Norwegian |
| `icelandic` | — | — | Icelandic | Icelandic | Icelandic |
| `german.old-high` | — | High German | Old High German | Old High German | Old High German |
| `german.middle-high` | — | — | High German | Middle High German | Middle High German |
| `german.early-new-high` | — | — | — | High German | High German |
| `german.old-saxon` | — | Continental Saxon | Old Saxon | Old Saxon | Old Saxon |
| `german.middle-low` | — | — | Low German | Low German | Low German |
| `franconian.old-low` | — | Old Low Franconian | Old Low Franconian | Old Low Franconian | Old Low Franconian |
| `dutch.middle` | — | — | Middle Dutch | Middle Dutch | Middle Dutch |
| `dutch` | — | — | — | Dutch | Dutch |
| `frisian` | — | Frisian | Frisian | Frisian | Frisian |
| `italian` | — | — | Italian | Italian | Italian |
| `venetian` | — | — | Venetian | Venetian | Venetian |
| `sicilian` | — | — | Sicilian | Sicilian | Sicilian |
| `sardinian` | — | Sardinian | Sardinian | Sardinian | Sardinian |
| `castilian.old` | — | Old Castilian | Old Castilian | Old Castilian | Old Castilian |
| `castilian` | — | — | — | Castilian | Castilian |
| `leonese` | — | — | Astur-Leonese | Astur-Leonese | Astur-Leonese |
| `aragonese` | — | — | Aragonese | Aragonese | Aragonese |
| `catalan` | — | — | Catalan | Catalan | Catalan |
| `galician-portuguese` | — | — | Galician-Portuguese | Galician-Portuguese | Galician-Portuguese |
| `galician` | — | — | — | Galician | Galician |
| `portuguese` | — | — | — | Portuguese | Portuguese |
| `basque` | Basque | Basque | Basque | Basque | Basque |
| `polish` | — | Polish | Polish | Polish | Polish |
| `czech` | — | Czech | Czech | Czech | Czech |
| `slovak` | — | — | Slovak | Slovak | Slovak |
| `slavic.old-east` | — | Old East Slavic | Old East Slavic | Old East Slavic | Old East Slavic |
| `ruthenian` | — | — | Ruthenian | Ruthenian | Ruthenian |
| `russian` | — | — | — | Russian | Russian |
| `church-slavonic` | — | Church Slavonic | Church Slavonic | Church Slavonic | Church Slavonic |
| `bulgarian` | — | Bulgarian | Bulgarian | Bulgarian | Bulgarian |
| `slovene` | — | — | Slovene | Slovene | Slovene |
| `romanian` | — | — | Romanian | Romanian | Romanian |
| `albanian` | — | — | Albanian | Albanian | Albanian |
| `hungarian` | — | Hungarian | Hungarian | Hungarian | Hungarian |
| `lithuanian` | — | — | Lithuanian | Lithuanian | Lithuanian |
| `latvian` | — | — | Latvian | Latvian | Latvian |
| `old-prussian` | — | Old Prussian | Old Prussian | Old Prussian | Old Prussian |
| `estonian` | — | Estonian | Estonian | Estonian | Estonian |
| `finnish` | — | Finnish | Finnish | Finnish | Finnish |
| `arabic.classical` | — | Classical Arabic | Classical Arabic | Classical Arabic | Classical Arabic |
| `arabic.mashriqi` | — | Mashriqi Arabic | Mashriqi Arabic | Mashriqi Arabic | Mashriqi Arabic |
| `arabic.maghrebi` | — | Maghrebi Arabic | Maghrebi Arabic | Maghrebi Arabic | Maghrebi Arabic |
| `arabic.andalusi` | — | Andalusi Arabic | Andalusi Arabic | Andalusi Arabic | Andalusi Arabic |
| `arabic.old` | Old Arabic | Old Arabic | Old Arabic | Old Arabic | Old Arabic |
| `persian.middle` | Persian | Middle Persian | Middle Persian | Middle Persian | Middle Persian |
| `persian.new` | — | Persian | Persian | Persian | Persian |
| `turkic.oghuz` | — | Oghuz Turkic | Oghuz Turkic | Oghuz Turkic | Oghuz Turkic |
| `turkish.ottoman` | — | — | Ottoman Turkish | Ottoman Turkish | Ottoman Turkish |
| `turkic.kipchak` | — | Cuman-Kipchak | Cuman-Kipchak | Cuman-Kipchak | Cuman-Kipchak |
| `turkic.crimean` | — | — | — | Crimean Tatar | Crimean Tatar |
| `armenian` | — | Armenian | Armenian | Armenian | Armenian |
| `armenian.classical` | — | Classical Armenian | Classical Armenian | Classical Armenian | Classical Armenian |
| `georgian` | — | Georgian | Georgian | Georgian | Georgian |
| `kurdish.kurmanji` | — | — | Kurmanji Kurdish | Kurmanji Kurdish | Kurmanji Kurdish |
| `kurdish.gorani` | — | — | Gorani | Gorani | Gorani |
| `aramaic` | Aramaic | Aramaic | Aramaic | Aramaic | Aramaic |
| `syriac` | — | Syriac | Syriac | Syriac | Syriac |
| `coptic` | Coptic | Coptic | Coptic | Coptic | Coptic |
| `hebrew` | Hebrew | Hebrew | Hebrew | Hebrew | Hebrew |
| `yiddish` | — | — | Yiddish | Yiddish | Yiddish |
| `judeo-spanish` | — | — | — | Judeo-Spanish | Judeo-Spanish |
| `amazigh.tashelhit` | — | Tashelhit | Tashelhit | Tashelhit | Tashelhit |
| `amazigh.kabyle` | — | — | Kabyle | Kabyle | Kabyle |
| `amazigh.tarifit` | — | — | Tarifit | Tarifit | Tarifit |

## Mutual intelligibility: calibration and direction

These settings are authored gameplay choices under the requested scale, not measured historical probabilities. Modern-language research distinguishes structural resemblance from exposure and recognises that listening and reading can differ; it does not assign the numerical values below. Symmetric defaults here are intentional simplifications, with each direction stored separately. [U01, MI01–MI03]

| Difficulty | Enum | Value | Intended allowance |
|---|---|---:|---|
| Very Hard | `VeryHard` | 7 | Very close languages or compatible historical stages; includes Middle English heard through later English. |
| Extremely Hard | `ExtremelyHard` | 8 | Larger related-language, stage or register gaps. |
| Insane | `Insane` | 9 | A narrow chance of following shared material; broad Romance-to-Romance baseline. |

These are **minimum listening difficulties**, not bonuses, percentages or starting-skill levels. Existing speech checks use the maximum of the utterance, accent and intelligibility difficulties and test a language the listener actually knows. A difficult accent can therefore still make a close-language check harder. An absent link resolves to Impossible; no self-edge or transitive closure is permitted. [R21–R23]

The direction **A hears B** means `ListenerLanguage=A`, `TargetLanguage=B`. It is not permission for an A speaker to speak B. Reading paths may reuse these links, but retain literacy and actual-script requirements; a shared script never adds an edge. [R24]

### Pair matrix

Every row below is two explicitly stored directions. All listed endpoints must exist in the selected pack; the JSON records their exact co-installation packs. A preserved old stage may participate without becoming a free home-language grant.

| First language | Second language | First hears second | Second hears first |
|---|---|---|---|
| Kabyle (`amazigh.kabyle`) | Tarifit (`amazigh.tarifit`) | Insane | Insane |
| Tashelhit (`amazigh.tashelhit`) | Kabyle (`amazigh.kabyle`) | Insane | Insane |
| Tashelhit (`amazigh.tashelhit`) | Tarifit (`amazigh.tarifit`) | Insane | Insane |
| Classical Arabic (`arabic.classical`) | Andalusi Arabic (`arabic.andalusi`) | Insane | Insane |
| Classical Arabic (`arabic.classical`) | Maghrebi Arabic (`arabic.maghrebi`) | Insane | Insane |
| Classical Arabic (`arabic.classical`) | Mashriqi Arabic (`arabic.mashriqi`) | Extremely Hard | Extremely Hard |
| Maghrebi Arabic (`arabic.maghrebi`) | Andalusi Arabic (`arabic.andalusi`) | Extremely Hard | Extremely Hard |
| Mashriqi Arabic (`arabic.mashriqi`) | Andalusi Arabic (`arabic.andalusi`) | Insane | Insane |
| Mashriqi Arabic (`arabic.mashriqi`) | Maghrebi Arabic (`arabic.maghrebi`) | Insane | Insane |
| Old Arabic (`arabic.old`) | Classical Arabic (`arabic.classical`) | Extremely Hard | Extremely Hard |
| Aramaic (`aramaic`) | Syriac (`syriac`) | Extremely Hard | Extremely Hard |
| Armenian (`armenian`) | Classical Armenian (`armenian.classical`) | Extremely Hard | Extremely Hard |
| Finnish (`finnish`) | Estonian (`estonian`) | Extremely Hard | Extremely Hard |
| Lithuanian (`lithuanian`) | Latvian (`latvian`) | Insane | Insane |
| Old Prussian (`old-prussian`) | Latvian (`latvian`) | Insane | Insane |
| Old Prussian (`old-prussian`) | Lithuanian (`lithuanian`) | Insane | Insane |
| Cornish (`cornish`) | Breton (`breton`) | Very Hard | Very Hard |
| Medieval Gaelic (`gaelic.medieval`) | Irish (`irish`) | Very Hard | Very Hard |
| Medieval Gaelic (`gaelic.medieval`) | Scottish Gaelic (`scottish-gaelic`) | Very Hard | Very Hard |
| Old Irish (`gaelic.old`) | Medieval Gaelic (`gaelic.medieval`) | Extremely Hard | Extremely Hard |
| Irish (`irish`) | Scottish Gaelic (`scottish-gaelic`) | Very Hard | Very Hard |
| Welsh (`welsh`) | Breton (`breton`) | Insane | Insane |
| Welsh (`welsh`) | Cornish (`cornish`) | Extremely Hard | Extremely Hard |
| Dutch (`dutch`) | Early New High German (`german.early-new-high`) | Insane | Insane |
| Dutch (`dutch`) | Middle Low German (`german.middle-low`) | Extremely Hard | Extremely Hard |
| Middle Dutch (`dutch.middle`) | Middle Low German (`german.middle-low`) | Extremely Hard | Extremely Hard |
| Frisian (`frisian`) | Dutch (`dutch`) | Insane | Insane |
| Frisian (`frisian`) | Middle Low German (`german.middle-low`) | Insane | Insane |
| Early New High German (`german.early-new-high`) | Middle Low German (`german.middle-low`) | Insane | Insane |
| Middle High German (`german.middle-high`) | Middle Low German (`german.middle-low`) | Insane | Insane |
| Middle Dutch (`dutch.middle`) | Dutch (`dutch`) | Very Hard | Very Hard |
| Old Low Franconian (`franconian.old-low`) | Dutch (`dutch`) | Insane | Insane |
| Old Low Franconian (`franconian.old-low`) | Middle Dutch (`dutch.middle`) | Extremely Hard | Extremely Hard |
| Old English (`english.old`) | Old Saxon (`german.old-saxon`) | Insane | Insane |
| Early Modern English (`english.earlymodern`) | Scots (`scots`) | Very Hard | Very Hard |
| Middle English (`english.middle`) | Scots (`scots`) | Very Hard | Very Hard |
| Middle English (`english.middle`) | Early Modern English (`english.earlymodern`) | Very Hard | Very Hard |
| Old English (`english.old`) | Middle English (`english.middle`) | Extremely Hard | Extremely Hard |
| Middle French (`french.middle`) | Early Modern French (`french.earlymodern`) | Very Hard | Very Hard |
| Old French (`french.old`) | Early Modern French (`french.earlymodern`) | Extremely Hard | Extremely Hard |
| Old French (`french.old`) | Middle French (`french.middle`) | Very Hard | Very Hard |
| Middle High German (`german.middle-high`) | Early New High German (`german.early-new-high`) | Very Hard | Very Hard |
| Old High German (`german.old-high`) | Early New High German (`german.early-new-high`) | Insane | Insane |
| Old High German (`german.old-high`) | Middle High German (`german.middle-high`) | Extremely Hard | Extremely Hard |
| Ancient Greek (`greek.ancient`) | Koine Greek (`greek.koine`) | Very Hard | Very Hard |
| Ancient Greek (`greek.ancient`) | Medieval Greek (`greek.medieval`) | Insane | Insane |
| Koine Greek (`greek.koine`) | Medieval Greek (`greek.medieval`) | Extremely Hard | Extremely Hard |
| Old Saxon (`german.old-saxon`) | Middle Low German (`german.middle-low`) | Extremely Hard | Extremely Hard |
| Anglo-Norman French (`french.anglonorman`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Anglo-Norman French (`french.anglonorman`) | Middle French (`french.middle`) | Extremely Hard | Extremely Hard |
| Anglo-Norman French (`french.anglonorman`) | Old French (`french.old`) | Very Hard | Very Hard |
| Old Norman (`norman.old`) | Anglo-Norman French (`french.anglonorman`) | Very Hard | Very Hard |
| Old Norman (`norman.old`) | Middle French (`french.middle`) | Extremely Hard | Extremely Hard |
| Old Norman (`norman.old`) | Old French (`french.old`) | Very Hard | Very Hard |
| Danish (`danish`) | Icelandic (`icelandic`) | Insane | Insane |
| Danish (`danish`) | Norwegian (`norwegian`) | Very Hard | Very Hard |
| Danish (`danish`) | Swedish (`swedish`) | Extremely Hard | Extremely Hard |
| Old Norse (`norse.old`) | Danish (`danish`) | Extremely Hard | Extremely Hard |
| Old Norse (`norse.old`) | Icelandic (`icelandic`) | Very Hard | Very Hard |
| Old Norse (`norse.old`) | Norwegian (`norwegian`) | Extremely Hard | Extremely Hard |
| Old Norse (`norse.old`) | Swedish (`swedish`) | Extremely Hard | Extremely Hard |
| Norwegian (`norwegian`) | Icelandic (`icelandic`) | Extremely Hard | Extremely Hard |
| Swedish (`swedish`) | Icelandic (`icelandic`) | Insane | Insane |
| Swedish (`swedish`) | Norwegian (`norwegian`) | Very Hard | Very Hard |
| Middle Persian (`persian.middle`) | Persian (`persian.new`) | Extremely Hard | Extremely Hard |
| Aragonese (`aragonese`) | Catalan (`catalan`) | Insane | Insane |
| Castilian (`castilian`) | Aragonese (`aragonese`) | Extremely Hard | Extremely Hard |
| Castilian (`castilian`) | Catalan (`catalan`) | Insane | Insane |
| Castilian (`castilian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Castilian (`castilian`) | Middle French (`french.middle`) | Insane | Insane |
| Castilian (`castilian`) | Galician (`galician`) | Insane | Insane |
| Castilian (`castilian`) | Judeo-Spanish (`judeo-spanish`) | Very Hard | Very Hard |
| Castilian (`castilian`) | Astur-Leonese (`leonese`) | Very Hard | Very Hard |
| Castilian (`castilian`) | Occitan (`occitan`) | Insane | Insane |
| Castilian (`castilian`) | Portuguese (`portuguese`) | Insane | Insane |
| Castilian (`castilian`) | Romanian (`romanian`) | Insane | Insane |
| Old Castilian (`castilian.old`) | Aragonese (`aragonese`) | Extremely Hard | Extremely Hard |
| Old Castilian (`castilian.old`) | Castilian (`castilian`) | Very Hard | Very Hard |
| Old Castilian (`castilian.old`) | Galician-Portuguese (`galician-portuguese`) | Insane | Insane |
| Old Castilian (`castilian.old`) | Judeo-Spanish (`judeo-spanish`) | Very Hard | Very Hard |
| Old Castilian (`castilian.old`) | Astur-Leonese (`leonese`) | Very Hard | Very Hard |
| Catalan (`catalan`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Catalan (`catalan`) | Middle French (`french.middle`) | Insane | Insane |
| Catalan (`catalan`) | Galician (`galician`) | Insane | Insane |
| Catalan (`catalan`) | Galician-Portuguese (`galician-portuguese`) | Insane | Insane |
| Catalan (`catalan`) | Portuguese (`portuguese`) | Insane | Insane |
| Catalan (`catalan`) | Romanian (`romanian`) | Insane | Insane |
| Old French (`french.old`) | Old Castilian (`castilian.old`) | Insane | Insane |
| Old French (`french.old`) | Italian (`italian`) | Insane | Insane |
| Old French (`french.old`) | Occitan (`occitan`) | Insane | Insane |
| Galician (`galician`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Galician (`galician`) | Middle French (`french.middle`) | Insane | Insane |
| Galician (`galician`) | Occitan (`occitan`) | Insane | Insane |
| Galician (`galician`) | Portuguese (`portuguese`) | Very Hard | Very Hard |
| Galician (`galician`) | Romanian (`romanian`) | Insane | Insane |
| Galician-Portuguese (`galician-portuguese`) | Galician (`galician`) | Very Hard | Very Hard |
| Galician-Portuguese (`galician-portuguese`) | Portuguese (`portuguese`) | Very Hard | Very Hard |
| Italian (`italian`) | Castilian (`castilian`) | Insane | Insane |
| Italian (`italian`) | Old Castilian (`castilian.old`) | Insane | Insane |
| Italian (`italian`) | Catalan (`catalan`) | Insane | Insane |
| Italian (`italian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Italian (`italian`) | Middle French (`french.middle`) | Insane | Insane |
| Italian (`italian`) | Galician (`galician`) | Insane | Insane |
| Italian (`italian`) | Occitan (`occitan`) | Insane | Insane |
| Italian (`italian`) | Portuguese (`portuguese`) | Insane | Insane |
| Italian (`italian`) | Romanian (`romanian`) | Insane | Insane |
| Italian (`italian`) | Sardinian (`sardinian`) | Insane | Insane |
| Italian (`italian`) | Sicilian (`sicilian`) | Extremely Hard | Extremely Hard |
| Italian (`italian`) | Venetian (`venetian`) | Extremely Hard | Extremely Hard |
| Judeo-Spanish (`judeo-spanish`) | Italian (`italian`) | Insane | Insane |
| Judeo-Spanish (`judeo-spanish`) | Portuguese (`portuguese`) | Insane | Insane |
| Astur-Leonese (`leonese`) | Galician-Portuguese (`galician-portuguese`) | Insane | Insane |
| Occitan (`occitan`) | Catalan (`catalan`) | Extremely Hard | Extremely Hard |
| Occitan (`occitan`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Occitan (`occitan`) | Middle French (`french.middle`) | Insane | Insane |
| Occitan (`occitan`) | Romanian (`romanian`) | Insane | Insane |
| Portuguese (`portuguese`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Portuguese (`portuguese`) | Middle French (`french.middle`) | Insane | Insane |
| Portuguese (`portuguese`) | Occitan (`occitan`) | Insane | Insane |
| Portuguese (`portuguese`) | Romanian (`romanian`) | Insane | Insane |
| Romanian (`romanian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Romanian (`romanian`) | Middle French (`french.middle`) | Insane | Insane |
| Sardinian (`sardinian`) | Castilian (`castilian`) | Insane | Insane |
| Sardinian (`sardinian`) | Catalan (`catalan`) | Insane | Insane |
| Sardinian (`sardinian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Sardinian (`sardinian`) | Middle French (`french.middle`) | Insane | Insane |
| Sardinian (`sardinian`) | Galician (`galician`) | Insane | Insane |
| Sardinian (`sardinian`) | Occitan (`occitan`) | Insane | Insane |
| Sardinian (`sardinian`) | Portuguese (`portuguese`) | Insane | Insane |
| Sardinian (`sardinian`) | Romanian (`romanian`) | Insane | Insane |
| Sicilian (`sicilian`) | Castilian (`castilian`) | Insane | Insane |
| Sicilian (`sicilian`) | Catalan (`catalan`) | Insane | Insane |
| Sicilian (`sicilian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Sicilian (`sicilian`) | Middle French (`french.middle`) | Insane | Insane |
| Sicilian (`sicilian`) | Galician (`galician`) | Insane | Insane |
| Sicilian (`sicilian`) | Occitan (`occitan`) | Insane | Insane |
| Sicilian (`sicilian`) | Portuguese (`portuguese`) | Insane | Insane |
| Sicilian (`sicilian`) | Romanian (`romanian`) | Insane | Insane |
| Sicilian (`sicilian`) | Sardinian (`sardinian`) | Insane | Insane |
| Venetian (`venetian`) | Castilian (`castilian`) | Insane | Insane |
| Venetian (`venetian`) | Catalan (`catalan`) | Insane | Insane |
| Venetian (`venetian`) | Early Modern French (`french.earlymodern`) | Insane | Insane |
| Venetian (`venetian`) | Middle French (`french.middle`) | Insane | Insane |
| Venetian (`venetian`) | Galician (`galician`) | Insane | Insane |
| Venetian (`venetian`) | Occitan (`occitan`) | Insane | Insane |
| Venetian (`venetian`) | Portuguese (`portuguese`) | Insane | Insane |
| Venetian (`venetian`) | Romanian (`romanian`) | Insane | Insane |
| Venetian (`venetian`) | Sardinian (`sardinian`) | Insane | Insane |
| Venetian (`venetian`) | Sicilian (`sicilian`) | Insane | Insane |
| Bulgarian (`bulgarian`) | Russian (`russian`) | Insane | Insane |
| Bulgarian (`bulgarian`) | Ruthenian (`ruthenian`) | Insane | Insane |
| Bulgarian (`bulgarian`) | Slovene (`slovene`) | Insane | Insane |
| Church Slavonic (`church-slavonic`) | Bulgarian (`bulgarian`) | Extremely Hard | Extremely Hard |
| Church Slavonic (`church-slavonic`) | Russian (`russian`) | Insane | Insane |
| Church Slavonic (`church-slavonic`) | Ruthenian (`ruthenian`) | Insane | Insane |
| Church Slavonic (`church-slavonic`) | Old East Slavic (`slavic.old-east`) | Extremely Hard | Extremely Hard |
| Czech (`czech`) | Russian (`russian`) | Insane | Insane |
| Czech (`czech`) | Slovak (`slovak`) | Very Hard | Very Hard |
| Czech (`czech`) | Slovene (`slovene`) | Insane | Insane |
| Polish (`polish`) | Czech (`czech`) | Extremely Hard | Extremely Hard |
| Polish (`polish`) | Russian (`russian`) | Insane | Insane |
| Polish (`polish`) | Ruthenian (`ruthenian`) | Extremely Hard | Extremely Hard |
| Polish (`polish`) | Slovak (`slovak`) | Extremely Hard | Extremely Hard |
| Polish (`polish`) | Slovene (`slovene`) | Insane | Insane |
| Ruthenian (`ruthenian`) | Russian (`russian`) | Very Hard | Very Hard |
| Old East Slavic (`slavic.old-east`) | Russian (`russian`) | Extremely Hard | Extremely Hard |
| Old East Slavic (`slavic.old-east`) | Ruthenian (`ruthenian`) | Very Hard | Very Hard |
| Slovak (`slovak`) | Russian (`russian`) | Insane | Insane |
| Slovak (`slovak`) | Slovene (`slovene`) | Insane | Insane |
| Cuman-Kipchak (`turkic.kipchak`) | Crimean Tatar (`turkic.crimean`) | Very Hard | Very Hard |
| Oghuz Turkic (`turkic.oghuz`) | Crimean Tatar (`turkic.crimean`) | Extremely Hard | Extremely Hard |
| Oghuz Turkic (`turkic.oghuz`) | Cuman-Kipchak (`turkic.kipchak`) | Insane | Insane |
| Oghuz Turkic (`turkic.oghuz`) | Ottoman Turkish (`turkish.ottoman`) | Very Hard | Very Hard |
| Ottoman Turkish (`turkish.ottoman`) | Crimean Tatar (`turkic.crimean`) | Extremely Hard | Extremely Hard |
| Ottoman Turkish (`turkish.ottoman`) | Cuman-Kipchak (`turkic.kipchak`) | Insane | Insane |
| Early New High German (`german.early-new-high`) | Yiddish (`yiddish`) | Extremely Hard | Extremely Hard |
| Middle High German (`german.middle-high`) | Yiddish (`yiddish`) | Extremely Hard | Extremely Hard |

### Conditional retained-language pairs

These do not create new placeholder languages. Bind the retained row by provenance, and use the setting only when both endpoints are present. **Ukrainian is not an alias for Ruthenian.** The Russian/Ukrainian Very Hard setting records the user's calibration example where a distinct retained Ukrainian row exists. The original 90-language specification remains intact.

| First language | Retained language | Both directions |
|---|---|---|
| Russian | Ukrainian | Very Hard |
| Ruthenian | Ukrainian | Very Hard |
| Polish | Ukrainian | Extremely Hard |
| Italian | Lombard | Extremely Hard |
| Venetian | Lombard | Extremely Hard |
| Italian | Neapolitan | Extremely Hard |
| Sicilian | Neapolitan | Very Hard |
| Irish | Manx | Extremely Hard |
| Scottish Gaelic | Manx | Extremely Hard |
| Finnish | Karelian | Very Hard |
| Estonian | Karelian | Insane |
| Polish | Wendish | Extremely Hard |
| Czech | Wendish | Extremely Hard |
| Slovene | Serbo-Croatian | Extremely Hard |
| Bulgarian | Serbo-Croatian | Insane |
| Church Slavonic | Serbo-Croatian | Insane |

### Reconciliation of original settings

Archive every original directed edge and value. Explicit matrix rows replace matching unchanged stock settings. For a genuine unlisted positive legacy stock edge, preserve it but raise any value below Very Hard to Very Hard; retain Extremely Hard and Insane. Keep Impossible/disabled rows disabled. Emit all retained exceptions with final values in the agent's resolved catalogue report. Preserve and report builder-owned edits rather than silently retuning them. No global retuning of Modern, Middle-Earth, signed-language or unselected content is requested. [R03, U01]

## Record-level instructions

### `latin` — Latin

**Player-facing description:**

> Latin is heard in Roman civic affairs, household instruction and public speech. Its books, records and learned traditions carry the language wherever Roman institutions and learning reach.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

One skill by policy; retain all existing classical, medieval and learned pronunciation traditions. Not native to every later western ethnicity.

Legacy lookup candidates: `Latin`.
Vernacular eligibility: antiquity.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No new Latin-to-Romance listening edge is inferred solely from descent. Preserve and recalibrate any explicitly retained original edge through the legacy ledger.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `greek.ancient` — Ancient Greek

**Player-facing description:**

> The Greek of the old poets, philosophers and civic orators is preserved in books and in careful recitation. Its words remain central to the study of Greek letters.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The measured pronunciation used by learned readers in the recitation of inherited books.

**Implementation notes:**

Ancient/literary Greek is a learned competency in later packs. Do not bind ambiguous legacy Greek without checking its defining source.

Legacy lookup candidates: `Ancient Greek`, `Greek`.
Vernacular eligibility: antiquity.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Koine Greek (`greek.koine`) | Very Hard | antiquity, darkages, medieval, renaissance, earlymodern |
| Medieval Greek (`greek.medieval`) | Insane | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `greek.koine` — Koine Greek

**Player-facing description:**

> The common Greek tongue travels among the cities and communities of the eastern Mediterranean. It serves household, market and correspondence, as well as the reading of widely known books.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The measured pronunciation used by learned readers in the recitation of inherited books.

**Implementation notes:**

Retain the existing learned/Koine distinction; do not merge with the vernacular merely because the script is shared.

Legacy lookup candidates: `Koine Greek`.
Vernacular eligibility: antiquity.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Ancient Greek (`greek.ancient`) | Very Hard | antiquity, darkages, medieval, renaissance, earlymodern |
| Medieval Greek (`greek.medieval`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `greek.medieval` — Medieval Greek

**Player-facing description:**

> Greek is spoken in the towns, villages and households of the eastern Roman world. The speech of everyday affairs stands beside the cultivated language of courts, churches and learned books.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Broad vernacular continuum reused for this historical range; no automatic Classical Greek mastery.

Legacy lookup candidates: `Medieval Greek`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Ancient Greek (`greek.ancient`) | Insane | darkages, medieval, renaissance, earlymodern |
| Koine Greek (`greek.koine`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `english.old` — Old English

**Player-facing description:**

> The English tongue is spoken among the shires, estates and village communities of Britain. Local ways of speaking distinguish neighbouring districts, while chronicles and religious books preserve its written words.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the English communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the English communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Requested relative-label convention. Medieval English here means its early Saxon component, not all speech throughout 1000-1400. Continental Saxon is a different language.

Legacy lookup candidates: `Old English`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Old Saxon (`german.old-saxon`) | Insane | darkages, medieval, renaissance, earlymodern |
| Middle English (`english.middle`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `english.middle` — Middle English

**Player-facing description:**

> English is heard in the households, markets and villages of England. Its local speech and written accounts carry the words of everyday affairs alongside those brought into use through French and Latin learning.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the English communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Medieval pack also needs this language. Renaissance English is an intentional early-Renaissance default, with the later stage separately available.

Legacy lookup candidates: `Middle English`.
Vernacular eligibility: medieval, renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Scots (`scots`) | Very Hard | medieval, renaissance, earlymodern |
| Early Modern English (`english.earlymodern`) | Very Hard | renaissance, earlymodern |
| Old English (`english.old`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `english.earlymodern` — Early Modern English

**Player-facing description:**

> English serves the households, towns and public affairs of England. Books, correspondence and the speech of courts and markets carry its words among people from many districts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

New stage for later Renaissance and Early Modern. Avoid two language or trait records both named English in the same installation.

Legacy lookup candidates: `Early Modern English`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Scots (`scots`) | Very Hard | renaissance, earlymodern |
| Middle English (`english.middle`) | Very Hard | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `norman.old` — Old Norman

**Player-facing description:**

> Norman is the French speech of Normandy's towns, estates and coastal communities. Families, lordships and maritime connections carry it between the mainland and neighbouring islands.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Norman communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Norman communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Late Dark Ages/Conquest-facing content, not uniformly applicable from 500.

Legacy lookup candidates: `Old Norman`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Anglo-Norman French (`french.anglonorman`) | Very Hard | medieval, renaissance, earlymodern |
| Middle French (`french.middle`) | Extremely Hard | renaissance, earlymodern |
| Old French (`french.old`) | Very Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `french.anglonorman` — Anglo-Norman French

**Player-facing description:**

> French is spoken among many of the noble households and learned institutions of England. It is used in law, administration and cultivated society, alongside the other tongues heard in household and public affairs.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the French-speaking English communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Later ordinary grant eligibility narrows. Law French is retained as a specialist variety, not bestowed on every Early Modern noble. Early Modern retention is principally historical/legal study. It is not a native-language default or a requirement for all English nobles.

Legacy lookup candidates: `Anglo-Norman`.
Vernacular eligibility: medieval, renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Extremely Hard | renaissance, earlymodern |
| Old French (`french.old`) | Very Hard | medieval, renaissance, earlymodern |
| Old Norman (`norman.old`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `french.old` — Old French

**Player-facing description:**

> French is spoken across the northern French lands in local forms familiar to their towns and lordships. Songs, tales and written affairs carry its words beyond the household.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the French communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the French communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Later portion of Dark Ages only.

Legacy lookup candidates: `Old French`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early Modern French (`french.earlymodern`) | Extremely Hard | earlymodern |
| Middle French (`french.middle`) | Very Hard | renaissance, earlymodern |
| Anglo-Norman French (`french.anglonorman`) | Very Hard | medieval, renaissance, earlymodern |
| Old Norman (`norman.old`) | Very Hard | darkages, medieval, renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Insane | darkages, medieval, renaissance, earlymodern |
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `french.middle` — Middle French

**Player-facing description:**

> French is heard in the court, towns and countryside of the kingdom. Letters, books and public records give its written forms a place beside the many local ways of speaking.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the French communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

New explicit stage only where the legacy Renaissance French definition is confirmed to represent it.

Legacy lookup candidates: `Middle French`.
Vernacular eligibility: renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early Modern French (`french.earlymodern`) | Very Hard | earlymodern |
| Old French (`french.old`) | Very Hard | renaissance, earlymodern |
| Anglo-Norman French (`french.anglonorman`) | Extremely Hard | renaissance, earlymodern |
| Old Norman (`norman.old`) | Extremely Hard | renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Italian (`italian`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | renaissance, earlymodern |
| Venetian (`venetian`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `french.earlymodern` — Early Modern French

**Player-facing description:**

> French serves the court and public affairs of the kingdom and is cultivated in learned and polite society. Books, correspondence and personal connections carry it beyond the French-speaking lands.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Do not manufacture Received Pronunciation-style descriptions for historical accents.

Legacy lookup candidates: `Early Modern French`.
Vernacular eligibility: earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Middle French (`french.middle`) | Very Hard | earlymodern |
| Old French (`french.old`) | Extremely Hard | earlymodern |
| Anglo-Norman French (`french.anglonorman`) | Insane | earlymodern |
| Castilian (`castilian`) | Insane | earlymodern |
| Catalan (`catalan`) | Insane | earlymodern |
| Galician (`galician`) | Insane | earlymodern |
| Italian (`italian`) | Insane | earlymodern |
| Occitan (`occitan`) | Insane | earlymodern |
| Portuguese (`portuguese`) | Insane | earlymodern |
| Romanian (`romanian`) | Insane | earlymodern |
| Sardinian (`sardinian`) | Insane | earlymodern |
| Sicilian (`sicilian`) | Insane | earlymodern |
| Venetian (`venetian`) | Insane | earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `occitan` — Occitan

**Player-facing description:**

> The tongue of oc is heard across the southern French lands and their neighbouring communities. Poetry, local correspondence and the speech of towns and villages sustain its use.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Occitan`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Old French (`french.old`) | Insane | medieval, renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |
| Catalan (`catalan`) | Extremely Hard | medieval, renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `welsh` — Welsh

**Player-facing description:**

> Welsh is the language of the kindreds, villages and towns of Wales. Poetry, learned traditions and household speech preserve its words across the country's districts and borderlands.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Welsh`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Breton (`breton`) | Insane | darkages, medieval, renaissance, earlymodern |
| Cornish (`cornish`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `breton` — Breton

**Player-facing description:**

> Breton is spoken among the communities of Brittany's western lands. It is heard in households, local gatherings and the affairs of villages and coastal settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Breton`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Cornish (`cornish`) | Very Hard | darkages, medieval, renaissance, earlymodern |
| Welsh (`welsh`) | Insane | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `cornish` — Cornish

**Player-facing description:**

> Cornish is the speech of Cornwall's communities. Family life, local dealings and religious teaching carry its words among the peninsula's villages and towns.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Cornish`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Breton (`breton`) | Very Hard | darkages, medieval, renaissance, earlymodern |
| Welsh (`welsh`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `gaelic.old` — Old Irish

**Player-facing description:**

> The Irish preserved in old books and recitations carries the words of learned households and religious communities. Its forms are studied with care by those who keep the earlier writings.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the Irish communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Irish communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Irish communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-DarkAges:**

> Irish is the speech of the kindreds and communities of Ireland. Learned households and religious houses preserve its words in teaching, recitation and books.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Irish`.
Vernacular eligibility: darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Medieval Gaelic (`gaelic.medieval`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `gaelic.medieval` — Medieval Gaelic

**Player-facing description:**

> Gaelic joins households and learned communities across Ireland and Scotland's western lands. Poets, teachers and families sustain its spoken and written traditions across the seas between them.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Gaelic communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Medieval Gaelic`.
Vernacular eligibility: darkages, medieval, renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Irish (`irish`) | Very Hard | renaissance, earlymodern |
| Scottish Gaelic (`scottish-gaelic`) | Very Hard | renaissance, earlymodern |
| Old Irish (`gaelic.old`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `irish` — Irish

**Player-facing description:**

> Irish is the Gaelic speech of Ireland's kindreds, towns and countryside. Household life, poetry and local learning sustain its use among neighbouring communities.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Irish`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Medieval Gaelic (`gaelic.medieval`) | Very Hard | renaissance, earlymodern |
| Scottish Gaelic (`scottish-gaelic`) | Very Hard | renaissance, earlymodern |

**Conditional retained links:** Manx — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `scottish-gaelic` — Scottish Gaelic

**Player-facing description:**

> Gaelic is spoken among the kindreds and communities of Scotland's Highlands and western islands. Family connections and learned traditions carry its words across land and sea.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Scottish Gaelic`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Medieval Gaelic (`gaelic.medieval`) | Very Hard | renaissance, earlymodern |
| Irish (`irish`) | Very Hard | renaissance, earlymodern |

**Conditional retained links:** Manx — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `scots` — Scots

**Player-facing description:**

> Scots is heard in Scotland's Lowland burghs, courts and countryside. It serves the affairs of households and towns, alongside the written words of records, letters and verse.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Scots`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early Modern English (`english.earlymodern`) | Very Hard | renaissance, earlymodern |
| Middle English (`english.middle`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `norse.old` — Old Norse

**Player-facing description:**

> The Norse tongue is carried among the northern lands and the settlements of the North Atlantic. Household speech, assemblies and recited accounts keep its words familiar across the seas.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Norse communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Norse communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Norse`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Danish (`danish`) | Extremely Hard | medieval, renaissance, earlymodern |
| Icelandic (`icelandic`) | Very Hard | medieval, renaissance, earlymodern |
| Norwegian (`norwegian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Swedish (`swedish`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `danish` — Danish

**Player-facing description:**

> Danish is spoken in Jutland and the Danish islands. The speech of farms and towns follows the routes of local trade and the affairs of the Danish crown.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Danish`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Icelandic (`icelandic`) | Insane | medieval, renaissance, earlymodern |
| Norwegian (`norwegian`) | Very Hard | medieval, renaissance, earlymodern |
| Swedish (`swedish`) | Extremely Hard | medieval, renaissance, earlymodern |
| Old Norse (`norse.old`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `swedish` — Swedish

**Player-facing description:**

> Swedish is heard among the towns and rural communities of Sweden. Local speech connects households with markets, religious institutions and the affairs of the kingdom.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Swedish`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Danish (`danish`) | Extremely Hard | medieval, renaissance, earlymodern |
| Old Norse (`norse.old`) | Extremely Hard | medieval, renaissance, earlymodern |
| Icelandic (`icelandic`) | Insane | medieval, renaissance, earlymodern |
| Norwegian (`norwegian`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `norwegian` — Norwegian

**Player-facing description:**

> Norwegian is spoken along Norway's coasts, fjords and inland valleys. District speech and household traditions remain familiar among communities linked by sea and mountain routes.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Norwegian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Danish (`danish`) | Very Hard | medieval, renaissance, earlymodern |
| Old Norse (`norse.old`) | Extremely Hard | medieval, renaissance, earlymodern |
| Icelandic (`icelandic`) | Extremely Hard | medieval, renaissance, earlymodern |
| Swedish (`swedish`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `icelandic` — Icelandic

**Player-facing description:**

> Icelandic is the speech of Iceland's households and assemblies. Books, recited accounts and family traditions preserve its words across the island's settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Icelandic`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Danish (`danish`) | Insane | medieval, renaissance, earlymodern |
| Old Norse (`norse.old`) | Very Hard | medieval, renaissance, earlymodern |
| Norwegian (`norwegian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Swedish (`swedish`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `german.old-high` — Old High German

**Player-facing description:**

> The German speech of the upper and central lands is heard in households, courts and religious instruction. The words of its regions are also preserved in old writings and learned recitations.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the German communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the German communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the German communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old High German`.
Vernacular eligibility: darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early New High German (`german.early-new-high`) | Insane | renaissance, earlymodern |
| Middle High German (`german.middle-high`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `german.middle-high` — Middle High German

**Player-facing description:**

> The German of the upper and central lands is spoken in towns, lordships and countryside. Verse, household instruction and written affairs carry its forms between neighbouring regions.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the German communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the German communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Middle High German`.
Vernacular eligibility: medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Middle Low German (`german.middle-low`) | Insane | medieval, renaissance, earlymodern |
| Early New High German (`german.early-new-high`) | Very Hard | renaissance, earlymodern |
| Old High German (`german.old-high`) | Extremely Hard | medieval, renaissance, earlymodern |
| Yiddish (`yiddish`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `german.early-new-high` — Early New High German

**Player-facing description:**

> The German of the upper and central lands serves household, town and public affairs. Books, correspondence and religious teaching spread written forms among communities with strong local ways of speaking.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `German`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Dutch (`dutch`) | Insane | renaissance, earlymodern |
| Middle Low German (`german.middle-low`) | Insane | renaissance, earlymodern |
| Middle High German (`german.middle-high`) | Very Hard | renaissance, earlymodern |
| Old High German (`german.old-high`) | Insane | renaissance, earlymodern |
| Yiddish (`yiddish`) | Extremely Hard | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `german.old-saxon` — Old Saxon

**Player-facing description:**

> Saxon is spoken in the northern continental Saxon lands. Its words are heard in local communities and preserved in religious verse and other writings.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the Saxon communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Saxon communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Saxon communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Saxon`.
Vernacular eligibility: darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Old English (`english.old`) | Insane | darkages, medieval, renaissance, earlymodern |
| Middle Low German (`german.middle-low`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `german.middle-low` — Middle Low German

**Player-facing description:**

> Low German is heard among the northern German towns and their surrounding countryside. Merchants and civic institutions carry it between ports and trading communities along the northern seas.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Middle Low German`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Dutch (`dutch`) | Extremely Hard | renaissance, earlymodern |
| Middle Dutch (`dutch.middle`) | Extremely Hard | medieval, renaissance, earlymodern |
| Frisian (`frisian`) | Insane | medieval, renaissance, earlymodern |
| Early New High German (`german.early-new-high`) | Insane | renaissance, earlymodern |
| Middle High German (`german.middle-high`) | Insane | medieval, renaissance, earlymodern |
| Old Saxon (`german.old-saxon`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `franconian.old-low` — Old Low Franconian

**Player-facing description:**

> The Low Franconian speech belongs to the lower Rhine and neighbouring river lands. It is heard in local households and communities, while old writings preserve its earlier forms.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the lower Rhine communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the lower Rhine communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the lower Rhine communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Low Franconian`.
Vernacular eligibility: darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Dutch (`dutch`) | Insane | renaissance, earlymodern |
| Middle Dutch (`dutch.middle`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `dutch.middle` — Middle Dutch

**Player-facing description:**

> The speech of the Low Countries is heard in towns, villages and trading places beside the great rivers. Civic records, letters and verse preserve the words used across its local communities.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Low Countries communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Middle Dutch`.
Vernacular eligibility: medieval, renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Middle Low German (`german.middle-low`) | Extremely Hard | medieval, renaissance, earlymodern |
| Dutch (`dutch`) | Very Hard | renaissance, earlymodern |
| Old Low Franconian (`franconian.old-low`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `dutch` — Dutch

**Player-facing description:**

> Dutch serves the towns, households and public affairs of the Low Countries. Trade and correspondence carry its words along waterways and between busy ports.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Dutch`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early New High German (`german.early-new-high`) | Insane | renaissance, earlymodern |
| Middle Low German (`german.middle-low`) | Extremely Hard | renaissance, earlymodern |
| Frisian (`frisian`) | Insane | renaissance, earlymodern |
| Middle Dutch (`dutch.middle`) | Very Hard | renaissance, earlymodern |
| Old Low Franconian (`franconian.old-low`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `frisian` — Frisian

**Player-facing description:**

> Frisian is spoken in the coastal communities of the North Sea. Local speech, customary affairs and family connections sustain it among islands, farms and maritime settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Frisian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Dutch (`dutch`) | Insane | renaissance, earlymodern |
| Middle Low German (`german.middle-low`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `italian` — Italian

**Player-facing description:**

> The Italian cultivated in letters and public affairs is heard beside the many local tongues of the peninsula. Books, merchants and educated households carry it between cities and regions.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Italian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Old French (`french.old`) | Insane | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Insane | medieval, renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Judeo-Spanish (`judeo-spanish`) | Insane | renaissance, earlymodern |

**Conditional retained links:** Lombard — Extremely Hard in both directions; Neapolitan — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `venetian` — Venetian

**Player-facing description:**

> Venetian is the speech of Venice, its lagoon and neighbouring communities. Civic affairs, trade and maritime connections carry its words through the northern Adriatic and beyond.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Venetian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Italian (`italian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | medieval, renaissance, earlymodern |

**Conditional retained links:** Lombard — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `sicilian` — Sicilian

**Player-facing description:**

> Sicilian is spoken in the towns and countryside of Sicily. Household speech, local affairs and verse sustain its use across the island.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Sicilian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Italian (`italian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Insane | medieval, renaissance, earlymodern |

**Conditional retained links:** Neapolitan — Very Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `sardinian` — Sardinian

**Player-facing description:**

> Sardinian is heard among the towns and rural communities of Sardinia. Local traditions and written affairs preserve forms of speech familiar to the island's districts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Sardinian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `castilian.old` — Old Castilian

**Player-facing description:**

> Castilian is spoken in the towns and communities of Castile. Its words serve household and public affairs and are preserved in chronicles, verse and other books.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Castilian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Castilian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Castilian`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Aragonese (`aragonese`) | Extremely Hard | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Very Hard | renaissance, earlymodern |
| Galician-Portuguese (`galician-portuguese`) | Insane | medieval, renaissance, earlymodern |
| Judeo-Spanish (`judeo-spanish`) | Very Hard | renaissance, earlymodern |
| Astur-Leonese (`leonese`) | Very Hard | medieval, renaissance, earlymodern |
| Old French (`french.old`) | Insane | darkages, medieval, renaissance, earlymodern |
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `castilian` — Castilian

**Player-facing description:**

> Castilian is heard across the Castilian lands and carried by officials, traders and travellers farther afield. Letters, books and public records give it a wide written use.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Castilian`, `Spanish`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Aragonese (`aragonese`) | Extremely Hard | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Judeo-Spanish (`judeo-spanish`) | Very Hard | renaissance, earlymodern |
| Astur-Leonese (`leonese`) | Very Hard | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Very Hard | renaissance, earlymodern |
| Italian (`italian`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | renaissance, earlymodern |
| Venetian (`venetian`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `leonese` — Astur-Leonese

**Player-facing description:**

> The speech of León and Asturias is heard in towns, valleys and village communities. Local custom, household life and written affairs preserve its distinctive words and forms.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Leonese`, `Leonese`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Very Hard | renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Very Hard | medieval, renaissance, earlymodern |
| Galician-Portuguese (`galician-portuguese`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `aragonese` — Aragonese

**Player-facing description:**

> Aragonese is spoken among the communities of Aragon and the neighbouring Pyrenean valleys. Local speech and written affairs connect the mountains with the settlements farther south.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Aragonese`, `Aragonese`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Extremely Hard | renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `catalan` — Catalan

**Player-facing description:**

> Catalan is spoken in the eastern Iberian lands and their Mediterranean settlements. Commerce, civic records and letters carry it between inland towns and ports.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Catalan`, `Catalan`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Aragonese (`aragonese`) | Insane | medieval, renaissance, earlymodern |
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Galician-Portuguese (`galician-portuguese`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | medieval, renaissance, earlymodern |
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |
| Occitan (`occitan`) | Extremely Hard | medieval, renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `galician-portuguese` — Galician-Portuguese

**Player-facing description:**

> The speech shared across Galicia and Portugal is heard in villages, towns and cultivated verse. Poets and courts carry its songs beyond the western Iberian lands.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the western Iberian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the western Iberian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Galician-Portuguese`.
Vernacular eligibility: medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Old Castilian (`castilian.old`) | Insane | medieval, renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Galician (`galician`) | Very Hard | renaissance, earlymodern |
| Portuguese (`portuguese`) | Very Hard | renaissance, earlymodern |
| Astur-Leonese (`leonese`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `galician` — Galician

**Player-facing description:**

> Galician is spoken among the communities of northwestern Iberia. Household speech and local traditions sustain it across valleys, villages and coastal settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Galician`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | renaissance, earlymodern |
| Portuguese (`portuguese`) | Very Hard | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | renaissance, earlymodern |
| Galician-Portuguese (`galician-portuguese`) | Very Hard | renaissance, earlymodern |
| Italian (`italian`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | renaissance, earlymodern |
| Venetian (`venetian`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `portuguese` — Portuguese

**Player-facing description:**

> Portuguese serves the towns, countryside and public affairs of Portugal. Trade and correspondence carry its words along the Atlantic-facing lands and between distant ports.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Portugese`, `Portuguese`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | renaissance, earlymodern |
| Galician (`galician`) | Very Hard | renaissance, earlymodern |
| Galician-Portuguese (`galician-portuguese`) | Very Hard | renaissance, earlymodern |
| Italian (`italian`) | Insane | renaissance, earlymodern |
| Judeo-Spanish (`judeo-spanish`) | Insane | renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Occitan (`occitan`) | Insane | renaissance, earlymodern |
| Romanian (`romanian`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | renaissance, earlymodern |
| Venetian (`venetian`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `basque` — Basque

**Player-facing description:**

> Basque is spoken in the communities on both sides of the western Pyrenees. Its words are familiar in household life, local gatherings and dealings among neighbouring valleys.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Ancient Aquitanian/Basque continuity is not a licence to identify all ancient Aquitanians as modern Basques. Antiquity native grants require a separate attested-language review.

Legacy lookup candidates: `Basque`.
Vernacular eligibility: antiquity, darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No related represented language is supplied; geographical contact with Romance does not itself create a link.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `polish` — Polish

**Player-facing description:**

> Polish is heard among the towns, villages and noble households of Poland. Local speech, instruction and written affairs carry it throughout the kingdom's communities.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Polish`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Czech (`czech`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Russian (`russian`) | Insane | renaissance, earlymodern |
| Ruthenian (`ruthenian`) | Extremely Hard | medieval, renaissance, earlymodern |
| Slovak (`slovak`) | Extremely Hard | medieval, renaissance, earlymodern |
| Slovene (`slovene`) | Insane | medieval, renaissance, earlymodern |

**Conditional retained links:** Ukrainian — Extremely Hard in both directions; Wendish — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `czech` — Czech

**Player-facing description:**

> Czech is spoken in Bohemia and Moravia. It serves household and civic life alongside the words of books, correspondence and religious teaching.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Czech`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Russian (`russian`) | Insane | renaissance, earlymodern |
| Slovak (`slovak`) | Very Hard | medieval, renaissance, earlymodern |
| Slovene (`slovene`) | Insane | medieval, renaissance, earlymodern |
| Polish (`polish`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

**Conditional retained links:** Wendish — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `slovak` — Slovak

**Player-facing description:**

> Slovak is heard in the towns and village communities of the northern Hungarian lands. Local forms of speech follow the valleys and routes between neighbouring settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Slovak`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Czech (`czech`) | Very Hard | medieval, renaissance, earlymodern |
| Polish (`polish`) | Extremely Hard | medieval, renaissance, earlymodern |
| Russian (`russian`) | Insane | renaissance, earlymodern |
| Slovene (`slovene`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `slavic.old-east` — Old East Slavic

**Player-facing description:**

> The speech of the Rus lands joins towns and countryside through household life, trade and princely affairs. Chronicles and other writings preserve its words for learned readers.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Rus communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Rus communities. Readers and teachers keep its words in use through the study of inherited texts.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old East Slavic`.
Vernacular eligibility: darkages, medieval.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Church Slavonic (`church-slavonic`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Russian (`russian`) | Extremely Hard | renaissance, earlymodern |
| Ruthenian (`ruthenian`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `ruthenian` — Ruthenian

**Player-facing description:**

> Ruthenian is spoken among the western and southern Rus communities. Towns, households and public offices use its words across the lands ruled by local princes and the Polish and Lithuanian crowns.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Ruthenian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Bulgarian (`bulgarian`) | Insane | medieval, renaissance, earlymodern |
| Church Slavonic (`church-slavonic`) | Insane | medieval, renaissance, earlymodern |
| Polish (`polish`) | Extremely Hard | medieval, renaissance, earlymodern |
| Russian (`russian`) | Very Hard | renaissance, earlymodern |
| Old East Slavic (`slavic.old-east`) | Very Hard | medieval, renaissance, earlymodern |

**Conditional retained links:** Ukrainian — Very Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `russian` — Russian

**Player-facing description:**

> Russian is heard in Muscovy and the northeastern Rus lands. Household speech, correspondence and the business of rulers and towns carry it among neighbouring communities.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Russian`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Bulgarian (`bulgarian`) | Insane | renaissance, earlymodern |
| Church Slavonic (`church-slavonic`) | Insane | renaissance, earlymodern |
| Czech (`czech`) | Insane | renaissance, earlymodern |
| Polish (`polish`) | Insane | renaissance, earlymodern |
| Ruthenian (`ruthenian`) | Very Hard | renaissance, earlymodern |
| Old East Slavic (`slavic.old-east`) | Extremely Hard | renaissance, earlymodern |
| Slovak (`slovak`) | Insane | renaissance, earlymodern |

**Conditional retained links:** Ukrainian — Very Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `church-slavonic` — Church Slavonic

**Player-facing description:**

> Church Slavonic is the solemn language of sacred books and worship in the Slavic churches. Clergy and learned readers study its forms and recite its words in their local traditions.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The measured pronunciation used by learned readers in the recitation of inherited books.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Church Slavonic`.
Vernacular eligibility: none.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Bulgarian (`bulgarian`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Russian (`russian`) | Insane | renaissance, earlymodern |
| Ruthenian (`ruthenian`) | Insane | medieval, renaissance, earlymodern |
| Old East Slavic (`slavic.old-east`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

**Conditional retained links:** Serbo-Croatian — Insane in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `south-slavic` — South Slavic

**Implementation notes:** This is a family resolver, not a language row. Resolve genuine constituent languages. It receives no intelligibility edges or single language skill.

### `bulgarian` — Bulgarian

**Player-facing description:**

> Bulgarian is spoken among the towns and village communities of the Bulgarian lands. Household life and local tradition sustain its words beside the learned language of the churches.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Bulgarian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Russian (`russian`) | Insane | renaissance, earlymodern |
| Ruthenian (`ruthenian`) | Insane | medieval, renaissance, earlymodern |
| Slovene (`slovene`) | Insane | medieval, renaissance, earlymodern |
| Church Slavonic (`church-slavonic`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

**Conditional retained links:** Serbo-Croatian — Insane in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `slovene` — Slovene

**Player-facing description:**

> Slovene is heard among the Slavic communities of the southeastern Alpine lands. Local speech connects households and settlements across mountain valleys and neighbouring lowlands.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Slovene`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Bulgarian (`bulgarian`) | Insane | medieval, renaissance, earlymodern |
| Czech (`czech`) | Insane | medieval, renaissance, earlymodern |
| Polish (`polish`) | Insane | medieval, renaissance, earlymodern |
| Slovak (`slovak`) | Insane | medieval, renaissance, earlymodern |

**Conditional retained links:** Serbo-Croatian — Extremely Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `romanian` — Romanian

**Player-facing description:**

> Romanian is spoken among the Romance-speaking communities of the Danube lands and neighbouring mountains. Family life, cultivation, pastoral routes and local trade carry its words between settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Romance, not Slavic. Cyrillic is the main historical default here; Latin is an exceptional documented-use option, not a universal pre-1750 default.

Legacy lookup candidates: `Romanian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Insane | renaissance, earlymodern |
| Catalan (`catalan`) | Insane | medieval, renaissance, earlymodern |
| Galician (`galician`) | Insane | renaissance, earlymodern |
| Italian (`italian`) | Insane | medieval, renaissance, earlymodern |
| Occitan (`occitan`) | Insane | medieval, renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |
| Early Modern French (`french.earlymodern`) | Insane | earlymodern |
| Middle French (`french.middle`) | Insane | renaissance, earlymodern |
| Sardinian (`sardinian`) | Insane | medieval, renaissance, earlymodern |
| Sicilian (`sicilian`) | Insane | medieval, renaissance, earlymodern |
| Venetian (`venetian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `albanian` — Albanian

**Player-facing description:**

> Albanian is spoken among the towns, villages and kindreds of the western Balkan lands. Local forms of speech connect coastal districts with inland valleys and mountain communities.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Albanian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No sufficiently close represented counterpart is specified.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `hungarian` — Hungarian

**Player-facing description:**

> Hungarian is the speech of the Magyars and is heard across the Hungarian lands. Household life, local affairs and written learning sustain its use among towns and countryside.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Old Hungarian`, `Hungarian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

Do not infer Finnish/Estonian comprehension from a broad Uralic classification.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `lithuanian` — Lithuanian

**Player-facing description:**

> Lithuanian is spoken among the Baltic communities of Lithuania. Family life, village affairs and regional traditions keep its words familiar across the countryside and neighbouring settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Written attestation occurs later than spoken existence. A script association makes writing mechanically possible, not evidence of universal historical literacy or a settled orthography.

Legacy lookup candidates: `Lithuanian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Latvian (`latvian`) | Insane | medieval, renaissance, earlymodern |
| Old Prussian (`old-prussian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `latvian` — Latvian

**Player-facing description:**

> Latvian is heard among the communities of the eastern Baltic coast and its inland districts. Household speech, local dealings and instruction carry its words along rivers and between settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Written attestation occurs later than spoken existence. A script association makes writing mechanically possible, not evidence of universal historical literacy or a settled orthography.

Legacy lookup candidates: `Latvian`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Lithuanian (`lithuanian`) | Insane | medieval, renaissance, earlymodern |
| Old Prussian (`old-prussian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `old-prussian` — Old Prussian

**Player-facing description:**

> Prussian is spoken in the Baltic communities of Prussia. Local speech and household traditions preserve its words beside the tongues heard in neighbouring towns and lordships.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Written attestation occurs later than spoken existence. A script association makes writing mechanically possible, not evidence of universal historical literacy or a settled orthography.

Legacy lookup candidates: `Prussian`, `Old Prussian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Latvian (`latvian`) | Insane | medieval, renaissance, earlymodern |
| Lithuanian (`lithuanian`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `estonian` — Estonian

**Player-facing description:**

> Estonian is spoken among the Finnic communities of the eastern Baltic and nearby islands. Local speech, family connections and village affairs sustain it across the region.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Written attestation occurs later than spoken existence. A script association makes writing mechanically possible, not evidence of universal historical literacy or a settled orthography. Finnic; do not classify as Baltic linguistically.

Legacy lookup candidates: `Estonian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Finnish (`finnish`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

**Conditional retained links:** Karelian — Insane in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `finnish` — Finnish

**Player-facing description:**

> Finnish is heard among the communities of the northern lake districts, forests and Baltic shores. Household life and local dealings carry its words along waterways and between settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Written attestation occurs later than spoken existence. A script association makes writing mechanically possible, not evidence of universal historical literacy or a settled orthography. Finnic; do not classify as Baltic linguistically.

Legacy lookup candidates: `Finnish`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Estonian (`estonian`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

**Conditional retained links:** Karelian — Very Hard in both directions.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `arabic.classical` — Classical Arabic

**Player-facing description:**

> The Arabic of scripture, scholarship and cultivated letters is studied and recited across the Muslim lands. Teachers preserve its forms, while books and correspondence carry its words far beyond any one town.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The measured pronunciation used by learned readers in the recitation of inherited books.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Quranic Arabic`, `Classical Arabic`.
Vernacular eligibility: none.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Andalusi Arabic (`arabic.andalusi`) | Insane | darkages, medieval, renaissance, earlymodern |
| Maghrebi Arabic (`arabic.maghrebi`) | Insane | darkages, medieval, renaissance, earlymodern |
| Mashriqi Arabic (`arabic.mashriqi`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Old Arabic (`arabic.old`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `arabic.mashriqi` — Mashriqi Arabic

**Player-facing description:**

> Arabic is spoken in the towns, villages and tribal communities of the eastern Arab lands. Local forms are heard in household life and commerce across Egypt, Syria and Mesopotamia.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Mashriqi Arabic`, `Levantine Arabic`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Classical Arabic (`arabic.classical`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Andalusi Arabic (`arabic.andalusi`) | Insane | darkages, medieval, renaissance, earlymodern |
| Maghrebi Arabic (`arabic.maghrebi`) | Insane | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `arabic.maghrebi` — Maghrebi Arabic

**Player-facing description:**

> The Arabic of the western lands is heard in Maghrebi towns, villages and tribal communities. Household speech and local trade connect settlements along the coasts and inland routes.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Mahgrebi Arabic`, `Maghrebi Arabic`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Classical Arabic (`arabic.classical`) | Insane | darkages, medieval, renaissance, earlymodern |
| Andalusi Arabic (`arabic.andalusi`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Mashriqi Arabic (`arabic.mashriqi`) | Insane | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `arabic.andalusi` — Andalusi Arabic

**Player-facing description:**

> The Arabic of al-Andalus is spoken among its towns and countryside. Local speech and family tradition carry its distinctive forms between Iberian communities and families settled across the sea.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Andalusi Arabic`.
Vernacular eligibility: darkages, medieval, renaissance.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Classical Arabic (`arabic.classical`) | Insane | darkages, medieval, renaissance, earlymodern |
| Maghrebi Arabic (`arabic.maghrebi`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |
| Mashriqi Arabic (`arabic.mashriqi`) | Insane | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `arabic.old` — Old Arabic

**Player-facing description:**

> The Arabic of older communities is preserved in inscriptions, remembered words and recited accounts. Its forms are known among those who study the speech and traditions of the Arab lands.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the Arab communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Arab communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Arab communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Antiquity:**

> Arabic is heard among communities of the Arab lands. Household speech, travel and remembered accounts carry its words between kindreds and settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Arabic script association applies to the late antique/early Islamic end, not all antiquity. Retain distinct ancient South Arabian or Nabataean scripts/languages already present.

Legacy lookup candidates: `Old Arabic`.
Vernacular eligibility: antiquity, darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Classical Arabic (`arabic.classical`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `persian.middle` — Middle Persian

**Player-facing description:**

> The Persian of older royal records, books and religious learning is preserved by careful readers and teachers. Its written forms remain part of the inherited learning of Iran.

**Player-facing description for Earth-Medieval:**

> The language preserved in older writings and recitations of the Persian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Renaissance:**

> The language preserved in older writings and recitations of the Persian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-EarlyModern:**

> The language preserved in older writings and recitations of the Persian communities. Readers and teachers keep its words in use through the study of inherited texts.

**Player-facing description for Earth-Antiquity:**

> Persian is heard among the households and settlements of Iran and used in royal and religious affairs. Scribes and learned readers preserve its written traditions.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Middle Persian`.
Vernacular eligibility: antiquity, darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Persian (`persian.new`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `persian.new` — Persian

**Player-facing description:**

> Persian serves household life, letters and public affairs across Iran and neighbouring lands. Poets, scribes, courts and traders carry its words far beyond the communities in which it is spoken at home.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Persian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Middle Persian (`persian.middle`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `turkic.oghuz` — Oghuz Turkic

**Player-facing description:**

> The Oghuz tongue is heard among Turkic pastoral and settled communities stretching towards the Anatolian and Iranian lands. Family speech, travel and service carry its words between kindreds and settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Oghuz Turkic`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Crimean Tatar (`turkic.crimean`) | Extremely Hard | renaissance, earlymodern |
| Cuman-Kipchak (`turkic.kipchak`) | Insane | darkages, medieval, renaissance, earlymodern |
| Ottoman Turkish (`turkish.ottoman`) | Very Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `turkish.ottoman` — Ottoman Turkish

**Player-facing description:**

> Turkish is spoken among the Ottoman lands and used in imperial service and learned writing. Courts, offices and households preserve cultivated forms beside the speech of towns and countryside.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Courtly Ottoman and vernacular Oghuz are a deliberate playable distinction. Later Turkish labels must not imply the twentieth-century language reform or Latin alphabet.

Legacy lookup candidates: `Ottoman Turkish`, `Turkish`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Oghuz Turkic (`turkic.oghuz`) | Very Hard | medieval, renaissance, earlymodern |
| Crimean Tatar (`turkic.crimean`) | Extremely Hard | renaissance, earlymodern |
| Cuman-Kipchak (`turkic.kipchak`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `turkic.kipchak` — Cuman-Kipchak

**Player-facing description:**

> The Kipchak tongue is heard among the steppe communities north of the Black Sea and neighbouring lands. Travel, military service and trade carry it between pastoral households and settled towns.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Cuman-Kipchak`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Crimean Tatar (`turkic.crimean`) | Very Hard | renaissance, earlymodern |
| Oghuz Turkic (`turkic.oghuz`) | Insane | darkages, medieval, renaissance, earlymodern |
| Ottoman Turkish (`turkish.ottoman`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `turkic.crimean` — Crimean Tatar

**Player-facing description:**

> The Tatar speech of Crimea is heard among its towns, villages and neighbouring steppe communities. Household ties, courtly affairs and commerce sustain its use across the peninsula and beyond.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Crimean Tatar`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Cuman-Kipchak (`turkic.kipchak`) | Very Hard | renaissance, earlymodern |
| Oghuz Turkic (`turkic.oghuz`) | Extremely Hard | renaissance, earlymodern |
| Ottoman Turkish (`turkish.ottoman`) | Extremely Hard | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `armenian` — Armenian

**Player-facing description:**

> Armenian is spoken among the communities of Armenia and their settlements abroad. Family connections, letters and religious institutions carry its words across highland and town.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Armenian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Classical Armenian (`armenian.classical`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `armenian.classical` — Classical Armenian

**Player-facing description:**

> The Armenian of sacred books and learned letters is carefully studied and recited. Churches, teachers and manuscript readers preserve its forms alongside the speech of everyday life.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The measured pronunciation used by learned readers in the recitation of inherited books.

**Implementation notes:**

Separate learned tradition only if needed alongside a vernacular competency; otherwise retain existing Armenian and model learned usage conservatively.

Legacy lookup candidates: `Classical Armenian`.
Vernacular eligibility: none.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Armenian (`armenian`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `georgian` — Georgian

**Player-facing description:**

> Georgian is heard in the towns and countryside south of the Caucasus. It serves household life and public affairs and carries a rich written tradition in churches and learned communities.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Georgian`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No sufficiently close represented counterpart is supplied; shared region with Armenian does not supply comprehension.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `kurdish.kurmanji` — Kurmanji Kurdish

**Player-facing description:**

> Kurmanji is spoken among Kurdish communities of the northern highlands and neighbouring districts. Kindred, household life and local exchange sustain its words across mountain routes and settled valleys.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Kurmanji Kurdish`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No new Gorani/Persian edge is asserted solely from broad Iranian affiliation or contact.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `kurdish.gorani` — Gorani

**Player-facing description:**

> Gorani is heard in communities of the western Iranian highlands and cultivated in local religious and literary traditions. Learned recitation stands beside the speech of households and neighbouring settlements.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Gorani`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

No new Kurmanji/Persian edge is asserted solely from broad Iranian affiliation or contact.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `aramaic` — Aramaic

**Player-facing description:**

> Aramaic is spoken and written among communities of Syria, Mesopotamia and neighbouring lands. Local speech, correspondence and learned traditions preserve its many familiar forms.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Aramaic`.
Vernacular eligibility: antiquity, darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Syriac (`syriac`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `syriac` — Syriac

**Player-facing description:**

> Syriac carries the sacred books, worship and learned writings of Christian communities across Mesopotamia and neighbouring lands. Churches, monasteries and teachers preserve its words through reading, copying and recitation.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Syriac`.
Vernacular eligibility: darkages, medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Aramaic (`aramaic`) | Extremely Hard | darkages, medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `coptic` — Coptic

**Player-facing description:**

> Coptic carries the speech and written traditions of Egypt's Christian communities. Sacred readings, worship and the work of teachers and copyists preserve its words in churches and monasteries.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Coptic`.
Vernacular eligibility: antiquity, darkages.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

The shared or related writing tradition does not confer Greek comprehension.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `hebrew` — Hebrew

**Player-facing description:**

> Hebrew is preserved in the sacred books, prayer and learning of Jewish communities. Teachers, readers and correspondents carry its words across lands whose households speak many different tongues.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Hebrew`.
Vernacular eligibility: antiquity.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

Do not infer Arabic/Aramaic comprehension from broad Semitic ancestry or sacred use; learned language acquisition is separate.

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `yiddish` — Yiddish

**Player-facing description:**

> Yiddish is heard in the households and communal affairs of Ashkenazi Jews. Family connections, local trade and correspondence carry it between communities in the German lands and farther east.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Yiddish`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Early New High German (`german.early-new-high`) | Extremely Hard | renaissance, earlymodern |
| Middle High German (`german.middle-high`) | Extremely Hard | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `judeo-spanish` — Judeo-Spanish

**Player-facing description:**

> The Spanish speech of Sephardic households preserves the words carried from Iberia. Family traditions and communal life sustain it among Jewish settlements around the Mediterranean.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Ladino`, `Judeo-Spanish`.
Vernacular eligibility: renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Castilian (`castilian`) | Very Hard | renaissance, earlymodern |
| Old Castilian (`castilian.old`) | Very Hard | renaissance, earlymodern |
| Italian (`italian`) | Insane | renaissance, earlymodern |
| Portuguese (`portuguese`) | Insane | renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `amazigh.tashelhit` — Tashelhit

**Player-facing description:**

> Tashelhit is spoken among Amazigh communities of the southwestern Maghreb. Household speech and local exchange sustain its words across mountain valleys, farming districts and neighbouring towns.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Tashelhit`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Kabyle (`amazigh.kabyle`) | Insane | medieval, renaissance, earlymodern |
| Tarifit (`amazigh.tarifit`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `amazigh.kabyle` — Kabyle

**Player-facing description:**

> Kabyle is heard among the Amazigh communities of the Kabyle mountains and their neighbouring settlements. Kindred, village affairs and local trade sustain its use.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Kabyle`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Tarifit (`amazigh.tarifit`) | Insane | medieval, renaissance, earlymodern |
| Tashelhit (`amazigh.tashelhit`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

### `amazigh.tarifit` — Tarifit

**Player-facing description:**

> Tarifit is spoken among Amazigh communities of the Rif and nearby districts. Family life, village custom and dealings between neighbouring settlements preserve its local forms.

**Unknown-language wording:**

> an unfamiliar language

**Fallback accent description (player-facing):**

> The familiar pronunciation heard in the households and gatherings of the local community.

**Implementation notes:**

Preserve all existing relevant accents. This is a deliberately broad game-language competency, not a claim of a single unchanged standard.

Legacy lookup candidates: `Tarifit`.
Vernacular eligibility: medieval, renaissance, earlymodern.
Reuse detailed authored accents; the fallback is only for a genuinely missing appropriate accent, not permission to erase the retained catalogue.

**Speakers of this language can attempt to understand:**

| Target speech | Difficulty | Active when both installed in |
|---|---|---|
| Kabyle (`amazigh.kabyle`) | Insane | medieval, renaissance, earlymodern |
| Tashelhit (`amazigh.tashelhit`) | Insane | medieval, renaissance, earlymodern |

Other targets receive no newly authored link. Document any preserved/recalibrated legacy exception in the resolved implementation report.

## Broad script families

All script descriptions below are player-facing; policy exceptions and membership are implementation metadata. Keep actual literacy and script knowledge separate from speaking a language.

### Arabic (`arabic`)

**Player-facing description:**

> The Arabic letters carry scripture, correspondence and the records of courts and merchants. Scribes and readers preserve their forms in books and documents throughout the lands where these letters are used.

**Known wording:** Arabic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `arabic.classical`, `arabic.mashriqi`, `arabic.maghrebi`, `arabic.andalusi`, `arabic.old`, `persian.new`, `turkic.oghuz`, `turkish.ottoman`, `turkic.kipchak`, `turkic.crimean`, `kurdish.kurmanji`, `kurdish.gorani`, `amazigh.tashelhit`, `amazigh.kabyle`, `amazigh.tarifit`.

### Aramaic (`aramaic`)

**Player-facing description:**

> The Aramaic letters are used in correspondence, records and learned writings. Their forms are kept by scribes and readers whose work joins local communities to wider traditions of written affairs.

**Known wording:** Aramaic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `arabic.old`, `aramaic`.

### Armenian (`armenian`)

**Player-facing description:**

> The Armenian letters preserve the words of books, worship and correspondence. Churches, learned households and copyists maintain their forms across Armenian communities.

**Known wording:** Armenian script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `armenian`, `armenian.classical`.

### Coptic (`coptic`)

**Player-facing description:**

> The Coptic letters preserve Egypt's Christian books, readings and correspondence. Teachers and copyists keep them familiar through the study and copying of manuscripts.

**Known wording:** Coptic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `coptic`.

### Cyrillic (`cyrillic`)

**Player-facing description:**

> The Cyrillic letters carry sacred books, correspondence and public records among the communities that use them. Clergy, scribes and learned readers preserve their forms through instruction and written work.

**Known wording:** Cyrillic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `slavic.old-east`, `ruthenian`, `russian`, `church-slavonic`, `bulgarian`, `romanian`.

### Georgian (`georgian`)

**Player-facing description:**

> The Georgian letters serve sacred books, correspondence and the affairs of court and household. Scribes and readers maintain their written traditions in churches and learned communities.

**Known wording:** Georgian script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

Retain genuinely distinct historical Georgian alphabets, but do not create separate skills for mere calligraphic styles.
Languages/constituents: `georgian`.

### Glagolitic (`glagolitic`)

**Player-facing description:**

> The Glagolitic letters preserve Slavic sacred writings and the words of worship. Clergy and copyists keep their forms through reading, instruction and the making of manuscripts.

**Known wording:** Glagolitic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `church-slavonic`.

### Greek (`greek`)

**Player-facing description:**

> The Greek letters are used in books, correspondence and public records. Their familiar forms carry the words of learning, worship and everyday written affairs.

**Known wording:** Greek script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `greek.ancient`, `greek.koine`, `greek.medieval`, `albanian`.

### Hebrew (`hebrew`)

**Player-facing description:**

> The Hebrew letters carry sacred texts, prayer books and the correspondence of Jewish communities. Teachers and scribes preserve their forms wherever these communities maintain their learning.

**Known wording:** Hebrew script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `persian.new`, `aramaic`, `hebrew`, `yiddish`, `judeo-spanish`.

### Latin (`latin`)

**Player-facing description:**

> The Latin letters are used in books, records and correspondence. Readers and scribes carry their written forms between courts, towns, religious houses and centres of learning.

**Known wording:** Latin script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `latin`, `english.old`, `english.middle`, `english.earlymodern`, `norman.old`, `french.anglonorman`, `french.old`, `french.middle`, `french.earlymodern`, `occitan`, `welsh`, `breton`, `cornish`, `gaelic.old`, `gaelic.medieval`, `irish`, `scottish-gaelic`, `scots`, `norse.old`, `danish`, `swedish`, `norwegian`, `icelandic`, `german.old-high`, `german.middle-high`, `german.early-new-high`, `german.old-saxon`, `german.middle-low`, `franconian.old-low`, `dutch.middle`, `dutch`, `frisian`, `italian`, `venetian`, `sicilian`, `sardinian`, `castilian.old`, `castilian`, `leonese`, `aragonese`, `catalan`, `galician-portuguese`, `galician`, `portuguese`, `basque`, `polish`, `czech`, `slovak`, `slovene`, `romanian`, `albanian`, `hungarian`, `lithuanian`, `latvian`, `old-prussian`, `estonian`, `finnish`, `turkic.kipchak`.

### Ogham (`ogham`)

**Player-facing description:**

> Ogham letters are formed from ordered groups of strokes, familiar from inscribed names and remembered accounts of the letters. Learned readers preserve their forms and their place in written tradition.

**Known wording:** Ogham script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `gaelic.old`.

### Old Hungarian (`old-hungarian`)

**Player-facing description:**

> The Hungarian runic letters preserve words in a distinctive set of signs. Those instructed in their forms use and remember them alongside the other letters encountered in written affairs.

**Known wording:** Old Hungarian script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `hungarian`.

### Pahlavi (`pahlavi`)

**Player-facing description:**

> The Pahlavi letters preserve Persian records and books of inherited learning. Scribes and religious readers study their forms and the written conventions carried in the manuscripts.

**Known wording:** Pahlavi script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `persian.middle`.

### Runic (`runic`)

**Player-facing description:**

> Runes carry names, messages and remembered words in distinctive cut or written signs. Their forms are learned within the communities that use them and preserved in inscriptions and accounts of the letters.

**Known wording:** Runic script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

Runic is a family selector where Futhorc, Younger Futhark and other genuinely distinct alphabets already exist; choose the correct retained record, never erase those distinctions.
Languages/constituents: `english.old`, `norse.old`.

### Syriac (`syriac`)

**Player-facing description:**

> The Syriac letters carry scripture, worship and learned correspondence among Syriac Christian communities. Monasteries, churches and teachers preserve their forms through reading and the copying of books.

**Known wording:** Syriac script.
**Unknown wording:** an unfamiliar writing system.

**Implementation notes:** Reuse existing broad script and its authored descriptions. No subdivision for handwriting, typeface or calligraphic hand alone.

No handwriting-only subdivision.
Languages/constituents: `syriac`.

## Shorthand

No new shorthand system is mandated in this version. Retain one already authored where it is a genuinely distinct system; otherwise add only after a named historical and gameplay use has been researched. Do not add an unsourced generic “medieval shorthand” to satisfy coverage.

Sources are listed in [06_RESEARCH_AND_REVIEW_REGISTER.md](06_RESEARCH_AND_REVIEW_REGISTER.md).

## Additional native-language binding: Lusitanian

Canonical key: `lusitanian`. Label: Lusitanian in every toolkit where retained. Native default only for the Antiquity Lusitanian identity; later retention is not a free modern/descendant skill. Script: Latin. No new intelligibility links. Source: [N18].

> The Lusitanian tongue is spoken among the communities of western Hispania. It carries the words of households, local gatherings and offerings to the gods.

Unknown description: “an unfamiliar tongue”. Neutral native accent description: “The familiar speech of Lusitanian households and their neighbours.” This is a minimal authored native default, not a reconstruction of specific ancient phonetic contrasts. Preserve a genuine existing Lusitanian record if found; do not relabel Celtiberian or Gallaecian.

The broad Dark Ages toolkit also exposes the Tashelhit regional-continuum definition for the Amazigh compatibility default. This is a coverage abstraction for the broad period, not a claim that a modern standard already exists in 500. Preserve and prefer exact regional source bindings. This note is metadata, not player prose.
