# Industrialised clothing — Wave 1 reuse and scope review

## Review boundary

Reviewed against the current worktree on 2026-09-03. This is the scope-level review supporting [Gate 1](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md), not proof of finished prose, historical prices or wearable runtime compositions. The [inventory](./Industrialised_Clothing_Wave1_Inventory.md) remains the row authority; the [evidence and coverage register](./Industrialised_Clothing_Wave1_Evidence_and_Coverage.md) records the scope/count approval and conventional-colour clarification dated 2026-09-03.

The review compares physical forms, materials, coverage and proposed production economics rather than exact English names alone. A different regional label, profession, colour or decoration is not a new base. Conversely, matching an existing noun is insufficient when material, cut, closures, storage or intrinsic economics differ. Public garment names do not define game-world culture.

### Source authority and limitations

- [ItemSeeder](../../DatabaseSeeder/Seeders/ItemSeeder.cs) runs earlier-era modules before the documented outfit supplement. [ClothingOutfitManifests](../../DatabaseSeeder/Seeders/ItemSeeder.ClothingOutfitManifests.cs) explicitly skips already registered item aggregates. First executed definitions, not the last textual occurrence in a file, own the item.
- [MedievalClothing](../../DatabaseSeeder/Seeders/ItemSeeder.MedievalClothing.cs) contains direct definitions, including the cotton/silk saris, short cotton bodice, leather smith apron, amice, wimple and garters. Their generated outfit references are not a substitute for those definitions.
- [Pre-industrial aliases](../../DatabaseSeeder/Seeders/ItemSeeder.PreIndustrialBaseline.Aliases.cs) own the shared belt/sash aliases. Reuse their canonical targets; do not recreate legacy identities.
- [Generated clothing definitions](../../DatabaseSeeder/Seeders/ItemSeeder.ClothingOutfitManifestData.Generated.cs) supply Renaissance and Early Modern additions and historical outfit dependencies. Their builder notes sometimes suggest material-changing skins or merged garments; those notes do not override the new specification.
- [Antiquity](../../DatabaseSeeder/Seeders/ItemSeeder.Antiquity.cs), other ItemSeeder partials and the [maintained manifest](./Seeded_Item_Manifest.json) were searched for earlier equivalents. The checker now examines literal definitions throughout `ItemSeeder*.cs`, including multiline `CreateItem` and alias calls, rather than requiring every reused item to appear in the generated file.

The automated check proves literal source and manifest presence only. It does not execute seed-order permutations, resolve every alias transitively or compare live database customisations. Gate 2/6 must ensure that selecting Industrial alone loads the authoritative reused dependencies, and that adding later eras cannot change a shared item's definition or price according to order.

## Explicit duplicate corrections

Eighteen proposed additions were changed to reuse during this review. Each retains its planning key and compatible additional skin briefs; its `Source` now identifies the existing prototype. Existing reused defaults are complete items, not templates to replace indiscriminately.

| Planning key | Reuse decision |
|---|---|
| stocking_garters | `medieval_tablet_woven_garters`; tied wool bands already exist; no invented elastic mechanism |
| underskirt_cotton | `earlymodern_italian_clothing_striped_cotton_petticoat`; compatible cotton petticoat, retaining two colour channels and a viable striped default |
| gathered_cotton_skirt | `earlymodern_colonial_clothing_full_cotton_skirt`; no extra base merely for a different outfit role |
| cotton_sari | `medieval_cotton_sari`; existing drape already includes lower pleats and upper fold |
| silk_sari | `medieval_fine_silk_sari`; preserve its silk material and distinct economics |
| choli | `medieval_short_cotton_bodice`; fitted short cotton construction already present |
| kurta | `renaissance_southasian_long_sideslit_tunic`; existing long cotton tunic and side slits |
| churidar | `renaissance_southasian_bunched_ankletrousers`; narrow lower leg and extra ankle length |
| cotton_changshan | `earlymodern_qing_clothing_long_sidefastened_robe`; existing long cotton side-fastened form |
| baji | `earlymodern_joseon_clothing_full_baji_trousers`; existing named full cotton form, with ankle detail to verify when authoring |
| wrapper_cotton | `renaissance_africancourt_broad_waistwrapper`; broad full-length cotton wrapper |
| embroidered_huipil | `renaissance_mesoamerican_rectangular_blouse`; rectangular cotton form, decoration belongs in presentations |
| westafrican_inner_tunic | `renaissance_africancourt_longsleeve_sideslit_tunic`; no invented collar change solely to justify a duplicate |
| leather_apron | `medieval_leather_smith_apron`; existing heavy chest-and-lap coverage, reused across occupations |
| espadrilles | `earlymodern_footwear_indianocean_ropesole_deckshoes`; compatible canvas upper and flexible rope sole |
| structured_riding_boots | `renaissance_frontier_split_skirt_riding_boots`; source explicitly supplies stiff high boots, not only soft historical riding boots |
| silk_stockings | `earlymodern_noble_shared_silk_stockings`; retain existing luxury economics unless a genuinely different evidenced production version is approved |
| ceremonial_stole | `medieval_latin_stole`; existing separate silk shoulder vestment |

Earlier Wave 1 corrections also reused the existing poncho and wool prayer garments. A cotton prayer-garment skin over a wool base was rejected. The new corrections do not silently reprice or change the material of any of this stock.

The presentation review removed 24 further briefs that merely restated the ordinary default or a solid colour selection. Those appearances remain available through complete unskinned bases; they have not been removed from the wardrobe. A peaked dinner-jacket lapel is a construction difference, so that proposed alternative moved from a skin brief to `peaked_dinner_jacket`, with its own complete default and outfit. Compatible figured facing and piping remain additional skins on the shawl-collar base. The net count change is recorded in the approval register rather than hidden behind a skin quota.

## Family-by-family scope disposition

Every inventory family is assigned one row below. These are acceptance obligations for the proposed designs; they are not claims that every historical observation or runtime dependency is already verified. Individual retained designs and additional presentations remain enumerated in the inventory.

| Family | Existing comparison and retained distinction | Later proof required |
|---|---|---|
| underwear | Reuse linen drawstring drawers, wraps and undershirts. Buttoned cotton drawers, knitted elastic forms, access-flap union suits, silk camisoles and rayon slips differ in closure, material or coverage | Fibre/elastic chronology; separate stock route/value; complete underwear layering |
| foundation | Reuse breast wrap, wool garters and cotton petticoat. Existing canvas stays, conical/wheel farthingales and silk-faced stays do not supply short corded stays, long cotton corsets, steel cages, crinolettes, bustles or later cup/elastic supports | Boning/support materials, distinct silhouettes, attachment and wear profiles; no new body-deformation simulation |
| infant | Folded nappy, crotch-opening bodysuit and open-bottom gown have real construction differences, not just smaller adult sizes | Separate pin fastening, safe descriptions, anatomy/size and assisted dressing |
| upper | Earlier linen tunics and gathered shirts are not automatically cotton button-placket shirts, integral-collar forms or jersey tees. Neckline, closure, sleeve length and material define retained bases | Cut-specific evidence; redundant hand/batch economic versions must be merged if evidence cannot justify them |
| knitwear | Compare historical knitted waistcoats and caps. Sleeves, openings, hoods, pockets, roll necks, heavy cotton knit and later fleece-like constructions distinguish proposed forms | Hand versus machine knitting/value, fibre/date admissions; no automatic quality superiority |
| lower | Reuse gathered wool and cotton skirts. Retain tailored wool, twill, denim, corduroy, pocketed cargo, stretch and panel-cut/shorter skirt constructions | Actual pocket capacities and cloth weights; fastening and reinforcement; fit without cultural/gender locks |
| dresses | Earlier wool/canvas bodices and combined court-gown records are not substitutes for independent silk bodices, trained/bustle skirts or true one-piece cotton/silk dresses | Multipart independence, silhouette support and authored material truth; no attached accessories invented in prose |
| tailoring | Existing `earlymodern_western_clothing_plain_wool_waistcoat` has no container capability; retained pocketed waistcoat must have real pockets. Lounge, cutaway, evening-tail and short dinner coats retain specific cuts; linen is not a wool skin | Exact pockets, lapels and cut; independent price/quality evidence for hand/batch variants |
| outerwear | Reuse mantles/shawls. Compare prior fur-lined coats and canvas watch coats; retained oilskin/rubberised coats, leather jackets, denim, synthetic insulation and shells require actual distinct material/closure definitions | Do not promise waterproofing, crash protection or powered climate control through prose |
| accessories | Reuse belts, sash and calf gaiters. Detachable collars, cuffs, shirtfront, braces, insignia, guarded pin pair and short buttoned spats are independently removable. Spats do not duplicate calf gaiters | Attachment targets, quantities/pair grammar and wear order; no sewn-in buttons as standalone inventory |
| hosiery | Reuse wool long hosiery, silk luxury stockings and split-toe socks. Cotton machine-knit, nylon, tights, sports and infant forms retain material/coverage distinctions | No Industrial nylon; consistent garter/support relationships and sock/stocking lengths |
| gloves | Reuse ordinary leather gloves and wool mittens. Earlier `renaissance_shared_clothing_cloth_gloves` is wool, not cotton; cotton service gloves are not its skin. Forearm-length, fingerless and perforated forms retain coverage/construction differences | Exact material/coverage and real PPE ownership; no driving or surgical bonus |
| headwear | Reuse veils, turban/headwrap and specific religious caps. Crown, brim, visor and structural materials distinguish bowler, boater, bonnet, pith and service forms; batch knitted cap needs its own route/value rationale | Do not use a nation label as a physical difference; conventional colours selected by outfit defaults, not base/skin locks |
| sleepwear | Existing undershirts and long nightcaps do not replace cotton sleep shirts, independently wearable pyjama pieces or belted pocketed/towelling robes | Complete matching palettes; cotton/silk/pile material differences; absorbency and drying claims tested |
| swimwear | Separate wool bathing top/bottom, knitted one-piece and later synthetic one-/two-piece garments retain physical distinctions | Era-appropriate fibres, lining and coverage; no UV or buoyancy guarantees by description |
| sportswear | Separate jersey, shorts, track pieces, padded cycling garments, leotard/tutu and heavy cotton training garments; do not merge whole kits | Actual cut/closures and activity suitability, no invented performance subsystem |
| workwear | Reuse leather apron. Existing `medieval_cotton_waist_apron` lacks container capability; retained pocketed apron must genuinely supply it. Scrubs remain separate; boiler suit genuinely one piece | PPE/medical claims belong to their owners; washable is not automatically sterile or chemical-proof |
| uniforms | Working, service, field, naval, mess and parade cuts stay distinct. Prior Early Modern long uniform coats, canvas gaiters and leather neck stocks are not a later wool battledress blouse or cotton cargo construction | Actual cut/pockets/materials; detachable badges and rank slides; national/branch skins only within compatible bases |
| footwear | Reuse leather sandals/slippers, wooden clogs, moccasins, stiff riding boots, rope-soled canvas shoes and specific religious forms. Retain lacing/button/bar differences, rubber overshoes/boots, distinct heels, reinforced dance shoes, studs and later sole materials | Exact heel/sole/upper comparisons; no safety/performance claims without dedicated mechanics |
| regional | Reuse the specific cotton drapes, tunics, trousers and wrappers listed above. Retain cotton `jeogori`/`chima` rather than reskinning existing ramie jackets or silk skirts; barkcloth tiputa is not a cotton poncho | Object-specific chronology and draping; do not infer that all local garments are interchangeable; broad robe plus inner garments stay separate |
| religious | Reuse separately defined robes, stoles, cuffs, amice, cincture, wimple, caps and wraps. Reject combined wimple-and-veil source for a two-piece ensemble. New clerical collar/shirt and backed fastening zone retain genuine construction obligations | Tradition-specific ensembles, overridable outfit colour defaults and wearing order; no universal clergy costume, authority or magical effect |
| institutional | Reuse gown and square cap; add separately wearable academic hood rather than incorporate it into gown prose | Chosen degree/institution's cut/lining and colour relationship, not a mandatory hood for every ceremony |

### Economic-only candidates

The hand/batch long corsets, collarless shirts, knitted jumpers, cardigans, lounge jackets and welted boots, plus machine-knit cap and hosiery variants, are explicit research candidates. Route labels alone are not evidence of intrinsic price differences. Gate 1 approval accepts the work to investigate these candidates, not a presumption that hand manufacture is expensive or better. If comparable evidence supports no meaningful physical/economic distinction, consolidate the candidate, update its outfits and counts, and obtain approval for the revised scope before production acceptance. Never manufacture a superficial difference to retain an allocated row.

## Ensemble corrections and remaining production work

The ensemble pass added a separate academic hood, liturgical amice/cincture/maniple, deacon over-vestment, Eastern cuffs/belt and inner/outer vestments, convent wimple/veil, selected additional religious ensembles, calf gaiters, ankle spats and independent nappy fasteners. Stockings in the initial worker examples now include their supports. Pacific shirt-and-wrap examples no longer depend on an arbitrarily named West African inner tunic.

The religious/regional examples propose representative scope, not universal or timeless dress codes. Ritual eligibility, local regulations and exact ceremonial admissions need evidence before final authoring. Bare feet in selected examples are intentional, not an omitted footwear row. Worn clothing lists are not complete PPE, weapon or ritual-equipment loadouts; those domains remain separately owned.

Gate 2/3 must validate every distinct composition, especially garments using broad legacy wear profiles: garters using `Wear_Leggings`, robe-layer combinations, collar/cuff attachments, nappy pins, skirt supports and shoes inside overshoes or gaiters. These are explicit proof obligations, not successful live tests. Retaining a correct garment in scope is not permission to misrepresent its function if integration fails.

## Scope-review decision

The user approved the current counts and overall scope on 2026-09-03, including the recorded conditional candidates and deferrals, with conventional colours moved to outfit defaults. The review removed unsupported duplication and enumerated smaller pieces; it did not accept draft item prose or alter installed content. Four former colour-only mourning briefs now specify tonal embroidery/braid finishing, while black belongs to the outfit; they remain subject to compatibility and editorial proof, not a quota to preserve. The evidence register carries the reviewed fingerprints, unchanged count tables, approval and outstanding Gates 2–7 obligations.
