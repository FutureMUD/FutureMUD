# CultureSeeder native-language and foreign-accent review

Reviewed 2026-09-11. Scope: native defaults in Antiquity, Dark Ages, Medieval and Renaissance; existing accent roles across the stock packs; neighbouring-language accent gaps through Renaissance. No new language skills, migrations or live database edits.

## Results

- Added 113 exact ethnicity/era rules using existing language identities, including retained Asian and African source languages. These include corrections and earlier-stage defaults as well as previously unmapped identities.
- Added native identity fallbacks for 26 social cultures. An ethnicity default still wins; these fallbacks do not change the existing `home`-based language grants.
- Authored 96 directed neighbouring-language accent definitions. Installation reuses an existing Foreign accent associated with the same source language; otherwise it creates a managed accent. Therefore 96 is a definition count, not a promised number of inserted rows.
- Reviewed explicit role/association metadata for 318 existing accent rows. This includes already-Foreign accents needing source associations, newly corrected Foreign roles and two corrected Native roles.

## Source ethnicity coverage

Counts below describe retained procedural source identities before overlay aliases merge them. Canonical overlays are also checked for a resolvable default in every reviewed era.

| Toolkit | Source identities mapped | Explicitly deferred |
| --- | ---: | ---: |
| Antiquity | 56 / 56 | 0 |
| Dark Ages | 75 / 89 | 14 |
| Medieval | 134 / 140 | 6 |
| Renaissance | 159 / 176 | 17 |

The Dark Ages source module includes later medieval examples. Where an earlier stage of the same language is already available, English and German defaults use that stage. Distinct languages absent from that toolkit are deferred rather than assigned a neighbouring language.

## Culture coverage

| Toolkit | Cultures with an explicit native fallback | Cultures inheriting ethnicity |
| --- | ---: | ---: |
| Antiquity | 17 / 24 | 7 |
| Dark Ages | 39 / 54 | 15 |
| Medieval | 56 / 79 | 23 |
| Renaissance | 57 / 81 | 24 |

Geographically broad, multi-language backgrounds such as Western European households, steppe households, Caucasian households and Jewish communities still inherit the ethnicity default. A religious or educational background is not automatically native in its liturgical language. These are intentional heritage-dependent cultures, not missing-language defects. Mozarabic communities also remain heritage-dependent pending a suitable Romance language.

## Deferred languages and identities

These 31 distinct source identities have explicit deferrals (some occur in more than one era). The installer reports the missing language and retains its existing unresolved-native availability gate. On rerun, an unchanged stock native default inherited from the wrong naming template is cleared; a builder override is preserved.

| Era | Source ethnicity | Missing language / stage |
| --- | --- | --- |
| darkages, medieval | Chola-Era Telugu | period Telugu |
| darkages, medieval | Song Min-Speaking Han | period Min |
| darkages, medieval | Conquest-Period Pecheneg | Pecheneg |
| darkages, medieval | High Medieval Alan | Alanic |
| darkages, medieval | Fatimid-Era Kutama | Kutama Berber |
| darkages, medieval | High Medieval Mozarab | Mozarabic Romance |
| renaissance | Renaissance Mingrelian | Mingrelian |
| renaissance | Renaissance Circassian Mamluk | Circassian |
| renaissance | Renaissance Punjabi | Punjabi |
| renaissance | Renaissance Sylheti | Sylheti |
| renaissance | Renaissance Tuluva | Tulu |
| renaissance | Renaissance Gan-Speaking Han | Gan |
| renaissance | Renaissance Hakka Han | Hakka |
| renaissance | Renaissance Jeju Islander | Jeju |
| renaissance | Renaissance Northern Tai | Northern Thai |
| renaissance | Renaissance Lao Tai | Lao |
| renaissance | Renaissance Sundanese | Sundanese |
| renaissance | Renaissance Madurese | Madurese |
| renaissance | Renaissance Acehnese | Acehnese |
| renaissance | Renaissance Tigrayan | Tigrinya |
| renaissance | Renaissance Agaw | Agaw |
| renaissance | Renaissance Soninke | Soninke |
| renaissance | Renaissance Manyika | Manyika |
| darkages | Anglo-Norman | Anglo-Norman |
| darkages | Marcher Anglo-Norman | Anglo-Norman |
| darkages | High Medieval Leonese | Astur-Leonese |
| darkages | High Medieval Aragonese | Aragonese |
| darkages | High Medieval Catalan | Catalan |
| darkages | High Medieval Galician | Galician-Portuguese |
| darkages | High Medieval Portuguese | Galician-Portuguese |
| darkages | High Medieval Kurdish | Kurmanji Kurdish |

The Dark Ages-only Anglo-Norman, Astur-Leonese, Aragonese, Catalan, Galician-Portuguese and Kurmanji deferrals concern toolkit availability: these languages exist in later toolkits. Telugu and Min also have later retained counterparts. The other rows require suitable language content before native defaults can be assigned.

## Decisions and accent semantics

- Ukrainian and the broad Cossack source use Old East Slavic in Medieval and Ruthenian in Renaissance. The Turkish naming template does not determine their native language.
- Gorani Kurds use Gorani; Turkic Mamluks use Kipchak; Chagatai Turks use Chagatai; Maghrebi Arabs use Maghrebi Arabic. Timurid Mongol and Delhi Turk defaults use the existing Chagatai gameplay skill for the Turkic-speaking court/military setting. This is a representative default, not a claim about every individual.
- The source Late Middle Chinese skill supplies the broad Song Han/Jiangnan/Wu examples. Song Min is deferred; the Renaissance catalogue can distinguish Guanhua, Wu, Yue and three Min languages. Hakka and Gan remain absent.
- Egyptian/Coptic defaults retain the existing era policy: Coptic in Dark Ages, Mashriqi Arabic in later everyday settings. The seeder does not infer a native language from liturgical practice alone.
- Romanised provincial Latin accents with an identifiable non-Latin source now have Foreign roles and source associations. Sabine and ordinary Roman registers retain Native roles. Koine renditions explicitly described as spoken by Attic, Aeolic, Ionic and Doric speakers are Foreign within the engine's separate-language model.
- Greek-accented Coptic, Japanese/Korean/Vietnamese readings of Chinese, Hausa as a second language and explicit cross-language Middle-Earth renditions are Foreign. Generic Foreign/Learner/Crude accents remain Fallback.
- Balachka and Upper Sannian are regional Ukrainian varieties and now have Native roles despite their old foreign group label. Learned traditions are not reclassified solely because they are learned.
- Existing historical Foreign accents receive source associations where the source is identifiable. Broad labels such as Indian, Balkan, Romance and Franco-Provencal remain Foreign without invented associations when a distinct source cannot be resolved. New neighbour accents use explicit existing source identities.

Historical distinctions checked during review: [Mingrelian](https://en.wikipedia.org/wiki/Mingrelian_language) is distinct from its Georgian literary language; [Alanic/Ossetic history](https://www.iranicaonline.org/articles/ossetic/i-history-and-description/) supports keeping Alans separate from the Kipchak template; [Ruthenian](https://en.wikipedia.org/wiki/Ruthenian_language) supplies context for the Cossack default. Exact proficiency and accent-contact defaults are authored gameplay choices, not historical population measurements.

## Exact additions and corrections

| Era | Source ethnicity | Existing language reference |
| --- | --- | --- |
| darkages, medieval | Early Medieval Rajput | `source.earthdarkagesandmedieval.language.Western Apabhramsha` |
| darkages, medieval | Early Medieval Chauhan Rajput | `source.earthdarkagesandmedieval.language.Western Apabhramsha` |
| darkages, medieval | Early Medieval Paramara Rajput | `source.earthdarkagesandmedieval.language.Western Apabhramsha` |
| darkages, medieval | Chola-Era Tamil | `source.earthdarkagesandmedieval.language.Medieval Tamil` |
| darkages, medieval | Song Han Chinese | `source.earthdarkagesandmedieval.language.Late Middle Chinese` |
| darkages, medieval | Song Northern Han | `source.earthdarkagesandmedieval.language.Late Middle Chinese` |
| darkages, medieval | Song Jiangnan Han | `source.earthdarkagesandmedieval.language.Late Middle Chinese` |
| darkages, medieval | Song Wu-Speaking Han | `source.earthdarkagesandmedieval.language.Late Middle Chinese` |
| darkages, medieval | Goryeo Korean | `source.earthdarkagesandmedieval.language.Early Middle Korean` |
| darkages, medieval | Goryeo Gyeonggi Korean | `source.earthdarkagesandmedieval.language.Early Middle Korean` |
| darkages, medieval | Goryeo Gyeongsang Korean | `source.earthdarkagesandmedieval.language.Early Middle Korean` |
| darkages, medieval | Heian-Kamakura Japanese | `source.earthdarkagesandmedieval.language.Medieval Japanese` |
| darkages, medieval | Heian-Kamakura Kinai Japanese | `source.earthdarkagesandmedieval.language.Medieval Japanese` |
| darkages, medieval | Heian-Kamakura Kanto Japanese | `source.earthdarkagesandmedieval.language.Medieval Japanese` |
| darkages | High Medieval Saxon | `german.old-saxon` |
| medieval | High Medieval Saxon | `german.middle-low` |
| medieval | Ukrainian | `slavic.old-east` |
| medieval | Cossack | `slavic.old-east` |
| renaissance | Ukrainian | `ruthenian` |
| renaissance | Cossack | `ruthenian` |
| renaissance | Renaissance Gorani Kurd | `kurdish.gorani` |
| renaissance | Renaissance Turkic Mamluk | `turkic.kipchak` |
| renaissance | Renaissance Maghrebi Arab | `arabic.maghrebi` |
| renaissance | Late Medieval Chagatai Turk | `source.earthrenaissanceworldexpansion.language.Chagatai` |
| renaissance | Renaissance Chagatai Turk | `source.earthrenaissanceworldexpansion.language.Chagatai` |
| renaissance | Renaissance Timurid Mongol | `source.earthrenaissanceworldexpansion.language.Chagatai` |
| renaissance | Renaissance Delhi Turk | `source.earthrenaissanceworldexpansion.language.Chagatai` |
| renaissance | Renaissance Hindustani | `source.earthrenaissanceworldexpansion.language.Hindavi` |
| renaissance | Late Medieval Afghan | `source.earthrenaissanceworldexpansion.language.Pashto` |
| renaissance | Renaissance Afghan Pashtun | `source.earthrenaissanceworldexpansion.language.Pashto` |
| renaissance | Renaissance Indo-Persian | `persian.new` |
| renaissance | Late Medieval Rajput | `source.earthrenaissanceworldexpansion.language.Old Rajasthani` |
| renaissance | Renaissance Mewar Rajput | `source.earthrenaissanceworldexpansion.language.Old Rajasthani` |
| renaissance | Renaissance Marwar Rajput | `source.earthrenaissanceworldexpansion.language.Old Rajasthani` |
| renaissance | Renaissance Amber Rajput | `source.earthrenaissanceworldexpansion.language.Old Rajasthani` |
| renaissance | Late Medieval Bengali | `source.earthrenaissanceworldexpansion.language.Middle Bengali` |
| renaissance | Renaissance Eastern Bengali | `source.earthrenaissanceworldexpansion.language.Middle Bengali` |
| renaissance | Renaissance Western Bengali | `source.earthrenaissanceworldexpansion.language.Middle Bengali` |
| renaissance | Late Medieval Kannadiga | `source.earthrenaissanceworldexpansion.language.Kannada` |
| renaissance | Renaissance Kannadiga | `source.earthrenaissanceworldexpansion.language.Kannada` |
| renaissance | Renaissance Telugu | `source.earthrenaissanceworldexpansion.language.Telugu` |
| renaissance | Renaissance Tamil | `source.earthrenaissanceworldexpansion.language.Tamil` |
| renaissance | Ming Han Chinese | `source.earthrenaissanceworldexpansion.language.Ming Guanhua` |
| renaissance | Renaissance Northern Han | `source.earthrenaissanceworldexpansion.language.Ming Guanhua` |
| renaissance | Renaissance Jiangnan Han | `source.earthrenaissanceworldexpansion.language.Wu Chinese` |
| renaissance | Renaissance Wu-Speaking Han | `source.earthrenaissanceworldexpansion.language.Wu Chinese` |
| renaissance | Renaissance Yue-Speaking Han | `source.earthrenaissanceworldexpansion.language.Yue` |
| renaissance | Renaissance Southern Min Han | `source.earthrenaissanceworldexpansion.language.Southern Min` |
| renaissance | Renaissance Eastern Min Han | `source.earthrenaissanceworldexpansion.language.Eastern Min` |
| renaissance | Renaissance Northern Min Han | `source.earthrenaissanceworldexpansion.language.Northern Min` |
| renaissance | Joseon Korean | `source.earthrenaissanceworldexpansion.language.Middle Korean` |
| renaissance | Renaissance Gyeonggi Korean | `source.earthrenaissanceworldexpansion.language.Middle Korean` |
| renaissance | Renaissance Gyeongsang Korean | `source.earthrenaissanceworldexpansion.language.Middle Korean` |
| renaissance | Renaissance Jeolla Korean | `source.earthrenaissanceworldexpansion.language.Middle Korean` |
| renaissance | Renaissance Hamgyong Korean | `source.earthrenaissanceworldexpansion.language.Middle Korean` |
| renaissance | Muromachi Japanese | `source.earthrenaissanceworldexpansion.language.Late Middle Japanese` |
| renaissance | Renaissance Kinai Japanese | `source.earthrenaissanceworldexpansion.language.Late Middle Japanese` |
| renaissance | Renaissance Kanto Japanese | `source.earthrenaissanceworldexpansion.language.Late Middle Japanese` |
| renaissance | Renaissance Tohoku Japanese | `source.earthrenaissanceworldexpansion.language.Late Middle Japanese` |
| renaissance | Renaissance Kyushu Japanese | `source.earthrenaissanceworldexpansion.language.Late Middle Japanese` |
| renaissance | Late Medieval Tibetan | `source.earthrenaissanceworldexpansion.language.Central Tibetan` |
| renaissance | Renaissance U-Tsang Tibetan | `source.earthrenaissanceworldexpansion.language.Central Tibetan` |
| renaissance | Renaissance Khampa | `source.earthrenaissanceworldexpansion.language.Khams Tibetan` |
| renaissance | Renaissance Amdo Tibetan | `source.earthrenaissanceworldexpansion.language.Amdo Tibetan` |
| renaissance | Late Medieval Viet | `source.earthrenaissanceworldexpansion.language.Middle Vietnamese` |
| renaissance | Renaissance Red River Kinh | `source.earthrenaissanceworldexpansion.language.Middle Vietnamese` |
| renaissance | Renaissance Thanh-Nghe Kinh | `source.earthrenaissanceworldexpansion.language.Middle Vietnamese` |
| renaissance | Late Medieval Tai | `source.earthrenaissanceworldexpansion.language.Ayutthaya Thai` |
| renaissance | Renaissance Ayutthaya Central Tai | `source.earthrenaissanceworldexpansion.language.Ayutthaya Thai` |
| renaissance | Late Majapahit Javanese | `source.earthrenaissanceworldexpansion.language.Middle Javanese` |
| renaissance | Renaissance Central Javanese | `source.earthrenaissanceworldexpansion.language.Middle Javanese` |
| renaissance | Renaissance Eastern Javanese | `source.earthrenaissanceworldexpansion.language.Middle Javanese` |
| renaissance | Late Medieval Malay | `source.earthrenaissanceworldexpansion.language.Classical Malay` |
| renaissance | Renaissance Malaccan Malay | `source.earthrenaissanceworldexpansion.language.Classical Malay` |
| renaissance | Renaissance Sumatran Malay | `source.earthrenaissanceworldexpansion.language.Classical Malay` |
| renaissance | Renaissance Bornean Malay | `source.earthrenaissanceworldexpansion.language.Classical Malay` |
| renaissance | Late Medieval Amhara | `source.earthrenaissanceworldexpansion.language.Amharic` |
| renaissance | Renaissance Amhara | `source.earthrenaissanceworldexpansion.language.Amharic` |
| renaissance | Late Medieval Somali | `source.earthrenaissanceworldexpansion.language.Somali` |
| renaissance | Renaissance Darod Somali | `source.earthrenaissanceworldexpansion.language.Somali` |
| renaissance | Renaissance Hawiye Somali | `source.earthrenaissanceworldexpansion.language.Somali` |
| renaissance | Renaissance Isaaq Somali | `source.earthrenaissanceworldexpansion.language.Somali` |
| renaissance | Renaissance Dir Somali | `source.earthrenaissanceworldexpansion.language.Somali` |
| renaissance | Renaissance Digil-Mirifle Somali | `source.earthrenaissanceworldexpansion.language.Maay` |
| renaissance | Late Medieval Swahili | `source.earthrenaissanceworldexpansion.language.Swahili` |
| renaissance | Renaissance Lamu Swahili | `source.earthrenaissanceworldexpansion.language.Swahili` |
| renaissance | Renaissance Mombasa Swahili | `source.earthrenaissanceworldexpansion.language.Swahili` |
| renaissance | Renaissance Zanzibar Swahili | `source.earthrenaissanceworldexpansion.language.Swahili` |
| renaissance | Renaissance Kilwa Swahili | `source.earthrenaissanceworldexpansion.language.Swahili` |
| renaissance | Late Medieval Manding | `source.earthrenaissanceworldexpansion.language.Manding` |
| renaissance | Renaissance Mandinka | `source.earthrenaissanceworldexpansion.language.Manding` |
| renaissance | Renaissance Bambara | `source.earthrenaissanceworldexpansion.language.Manding` |
| renaissance | Renaissance Songhay | `source.earthrenaissanceworldexpansion.language.Songhay` |
| renaissance | Late Medieval Hausa | `source.earthrenaissanceworldexpansion.language.Hausa` |
| renaissance | Renaissance Kano Hausa | `source.earthrenaissanceworldexpansion.language.Hausa` |
| renaissance | Renaissance Katsina Hausa | `source.earthrenaissanceworldexpansion.language.Hausa` |
| renaissance | Renaissance Gobir Hausa | `source.earthrenaissanceworldexpansion.language.Hausa` |
| renaissance | Renaissance Zazzau Hausa | `source.earthrenaissanceworldexpansion.language.Hausa` |
| renaissance | Late Medieval Bakongo | `source.earthrenaissanceworldexpansion.language.Kikongo` |
| renaissance | Renaissance Mpemba Bakongo | `source.earthrenaissanceworldexpansion.language.Kikongo` |
| renaissance | Renaissance Mbata Bakongo | `source.earthrenaissanceworldexpansion.language.Kikongo` |
| renaissance | Renaissance Soyo Bakongo | `source.earthrenaissanceworldexpansion.language.Kikongo` |
| renaissance | Late Medieval Shona | `source.earthrenaissanceworldexpansion.language.Karanga` |
| renaissance | Renaissance Karanga | `source.earthrenaissanceworldexpansion.language.Karanga` |
| renaissance | Renaissance Zezuru | `source.earthrenaissanceworldexpansion.language.Zezuru` |
| renaissance | Renaissance Korekore | `source.earthrenaissanceworldexpansion.language.Korekore` |
| darkages | High Medieval English | `english.old` |
| darkages | High Medieval Northern English | `english.old` |
| darkages | High Medieval Midland English | `english.old` |
| darkages | High Medieval German | `german.old-high` |
| darkages | High Medieval Franconian | `german.old-high` |
| darkages | High Medieval Thuringian | `german.old-high` |
| darkages | High Medieval Low German | `german.old-saxon` |

## Verification

- Full DatabaseSeeder suite: 875 passed before the final additional coverage checks.
- Final CultureToolkit suite: 89 passed, including all expanded identities, all Antiquity and European identities, all canonical overlays, era-valid endpoints, persisted culture defaults, duplicate avoidance, deferred-default clearing and builder override preservation.
- Tests use catalogue/source checks and EF in-memory fixtures. No MySQL installation or telnet session was run.
- Maintained embedded-resource checksums include the two new accent catalogues. Original source-corpus snapshots are unchanged.
