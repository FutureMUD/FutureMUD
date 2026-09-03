# Industrialised Clothing, Footwear and Uniforms Design Reference

## Authority, status and production goal

This is the authoritative package specification for clothing, footwear and uniforms across Industrial, Modern, Nuclear and Information eras. Its requirements were approved through the clothing design discussion; approval of this document does not approve individual catalogue rows or certify implementation. It specialises the [Industrialised programme](./FutureMUD_Industrialised_Item_Seeder_Programme.md), [shared baseline](./FutureMUD_Industrialised_Shared_Baseline_Design_Reference.md) and [Industrial master](./FutureMUD_Industrial_Item_Seeder_Master_Era_Design_Reference.md).

Production goal: deliver a comprehensive, reusable wardrobe of physically truthful garments, richly authored skins, complete wearable ensembles and meaningful production crafts, entirely through ItemSeeder. Completeness means accepted garment and ensemble coverage, not filling a prototype quota.

### State at the documentation baseline

| State | Meaning and current evidence |
|---|---|
| Approved requirements | The rules, implementation obligations and gates in this document are accepted design requirements. |
| Draft catalogue content | The existing generated 600 shared and 70 Industrial clothing rows are placeholders for review, not an accepted wardrobe. Their systematic descriptor/context combinations are not the authoring model to retain. |
| Implemented infrastructure | Runtime skins, skin-bearing craft products and skin-bearing outfit entries exist. The Industrialised typed catalogue and generator do not yet express the complete clothing contract below. |
| Approved scope | Gate 1 passed on 2026-09-03: the user accepted the Wave 1 counts and overall scope, with conventional colours moved to outfit defaults. Individual production content remains unaccepted. |
| Verified production readiness | Not established. Gates 2–7 remain open; existing row counts, generic validation and earlier replay results do not close them. |

The current code makes `industrial` selectable. That is an implementation fact, not evidence that this package or Stage 2 has passed production acceptance. This documentation change does not alter activation. The programme must resolve that discrepancy in its implementation/activation work; clothing approval alone never authorises Industrial activation.

## Catalogue model and coverage

### R01 — Inventory before outfits

Start with an authored garment-family inventory. Do not generate a Cartesian product of material, occupation and adjectives, or invent "workshop", "field" and similar garment variants to fill counts. Context may describe a real use but is not itself a distinct garment.

The inventory must cover:

- underwear and foundation/support garments;
- shirts, blouses, tunics and other upper-body separates;
- trousers, shorts, skirts and other lower-body separates;
- dresses and genuinely one-piece garments, including coveralls;
- tailoring, including separate jackets, waistcoats and accompanying garments;
- outerwear and weather layers;
- hosiery, headwear, gloves, neckwear and wearable accessories;
- sleepwear, bathing, sporting and specialist clothing;
- footwear and independently worn protective outer footwear.

Review each family against everyday/formal use, income levels, climates, occupations, institutions, religious traditions, regional forms and all four later-era bands. Record included examples and justified absences; this is a coverage matrix, not a requirement to populate every possible combination. Technology compatibility profiles do not determine clothing culture. Prior forms remain available where historically and mechanically appropriate.

### R02 — Complete garments and complete ensembles

Make an element a separate item when it is independently worn, removed, exchanged or meaningfully used. Include detachable collars, cuffs, braces, stocking supports and comparable elements where the ensemble warrants them. Do not turn ordinary sewn-in cuffs, seams or every button into separate inventory objects.

Do not collapse multipart outfits, especially women's clothing, into one "dress" item. Identify the actual foundation layers, petticoats, separate skirt/bodice/blouse, outer layers, hosiery, footwear and accessories for each selected ensemble. Apply the same standard to men's tailoring, religious dress and ceremonial uniforms. A genuinely one-piece garment remains one item. Pair conventions must match the existing wearable model and use natural descriptions.

### R03 — Coverage and count control

The old 600 shared/70 Industrial clothing base-prototype quotas are superseded. Gate 1 approves an inventory and separate totals for new bases, reused bases, authored skins, crafts and outfits, broken down by layer/admissions. Aim for more authored skins than base garments across the package, not an arbitrary ratio for every garment family. Any package-level variance from that aim needs explicit approval and rationale.

For Stage 2 planning, unchanged non-clothing allocations leave `5,200 + C_shared` shared and `580 + C_industrial` Industrial ordinary prototypes, or `5,780 + C_shared + C_industrial` combined. `C_shared` and `C_industrial` are approved new clothing bases, not skins or reused pre-industrial items. These are reconciliation formulas, not new quotas. Reconcile later-era deltas separately as their inventories are approved. Update programme totals, tests and audits in the implementation wave together; never silently retain old exact-count assertions or count skins as prototypes.

### R04 — Bases, skins and identity

Use a separate base for meaningful differences in construction, silhouette, material properties, coverage, layering, weight, insulation/protection, storage, attachments or intrinsic economic properties. Similar component coverage does not alone make a blouse, tunic and fitted shirt interchangeable.

Use skins for compatible differences in colour scheme, pattern, trim, embroidery, finishing, insignia and presentation. A skin cannot invent functional pockets, change cotton into wool, or turn a short jacket into a long coat. Surface resemblance does not override the physical contract.

Every base garment's plain/default presentation is a viable standalone item with no skin applied. It is not a hidden template, incomplete placeholder or skin-required shell. It must have finished descriptions, physical behaviour, valid colour configuration, a normal production route and appropriate price/quality, and be usable in ordinary stock, crafting and outfits without a skin. The plain presentation counts once as the base, not again as a skin: promote an appropriate planned plain presentation to the base and remove the redundant skin entry. Distinct additional presentations remain skins. Unskinned does not mean colourless or unfinished; the variable-colour default still applies.

Reuse unchanged `preindustrial_*` identities. New shared bases use `industrialised_*`; era deltas use their canonical era prefix. Family and presentation references must remain stable and independent of technology profile. Record why every new base cannot be satisfied by an existing identity or an honest skin. Retire superseded managed stock through provenance, preserving customised records.

## Colour, manufacture and value

### R05 — Variable colour by default

Every garment and applicable skin uses variable colour. Conventional religious, ceremonial, mourning, institutional and occupational colours belong in overridable outfit-template defaults, not global restrictions on a base or skin. A white clerical collar or black mourning veil is a selected presentation of reusable stock; another valid colour must not require a duplicate prototype or skin. Colour alone does not justify an additional skin.

Wave 1 has no approved fixed-colour exceptions. A future exception requires explicit approval, evidence that colour is intrinsic to that particular presentation rather than merely customary, and proof that an outfit default cannot meet the requirement. Record the exact affected colour channel, reason and evidence; never extrapolate it to an entire garment family, religion or institution. Material-native surfaces and finishes still constrain truthful colour choices, but an old hard-coded description or colour-bearing stable identity does not establish an exception.

An author writes the complete prose, including supported `$colour` substitution where appropriate. Substitution is not fragment-generated prose. Choose palettes appropriate to the material and presentation; check grammar and visible descriptions across every allowed value. Resolve each variable through a real characteristic definition/profile, not an unresolved token.

Outfits must declare coordinated colour defaults where garments are meant to match, including explicit per-entry accents and conventional colours. Resolve a valid explicit outfit-instance choice first, then the entry default, then the ensemble palette; never silently replace an invalid choice or allow independent random loading to break a match. Defaults apply to the materialised ensemble, not every copy of its garments. Unskinned stock and crafts remain usable outside any outfit. Any future approved fixed-colour presentation must agree with underlying characteristics and short/full descriptions, and reject incompatible selections before mutation. Craft output must preserve or deliberately select a valid colour consistently with its inputs and declared product. Additional colour channels require supported bindings, not invented markup.

### R06 — Production route per presentation

Each authored presentation has one normal route: hand construction, machine-assisted individual construction or standardised batch manufacture. An unskinned base is also a presentation and needs a route. An alternative route is an explicit reviewed exception, not an automatically generated second recipe for every garment.

Equivalent presentations may share a base when physical and intrinsic economic properties match. Where price or behaviour differs, create distinct bases and retain a shared garment-family relationship. Do not force incompatible records together to minimise the base count.

Crafts must reflect real inputs, tools, work phases, quantities, outputs and failure behaviour. Distinguish machine-assisted tailoring from unattended factory production. A batch recipe is not proof of a production-line subsystem; any unavailable automation is a ledger dependency, not a prose claim. Maintenance and alteration only claim supported operations, with explicit dependent references and tests.

### R07 — Price and quality are independent, period-sensitive decisions

Assess price and expected quality separately by garment, construction route and period. Neither handmade nor machine-made automatically costs more or produces superior work. A proposed relationship in the early Nuclear/atomic period, or any other period, remains a research hypothesis until supported. Visible finishing should convey construction without repeating an unsupported "handmade" or "factory-made" label.

Use the [historical pricing methodology](./FutureMUD_Industrialised_Historical_Pricing_Methodology.md): preserve original price/unit/currency and matched local unskilled-wage evidence; normalise each locale independently; combine comparable observations by weighted median in log space; set `CostIndex = 10 × labour days`, quantised to the 1–2–5 ladder. CPI only bridges nearby years in the same locale. No exchange-rate or modern-money conversion establishes the index.

Every item needs usable price evidence or a documented approved analogue, not a source gateway masquerading as an observation. Explain production-route price differences and period comparisons. Keep typical quality rationale distinct from price evidence; do not infer quality from price alone or invent numerical historical precision.

Keep each prototype's price deterministic. Multiple selected eras must not reprice the same identity according to seed order. Genuinely different period-specific economic versions need explicit identities, admissions and rationale; ordinary inflation, locale labels and cosmetic changes do not justify duplication.

Stock skin quality overrides remain unset by default. The runtime uses a skin quality override ahead of the item's stored quality, which can mask craft-earned outcomes. Configure typical stock quality on the prototype and earned outcomes in crafts; any exceptional skin override must be explicitly approved and tested against that consequence.

## Outfits, uniforms and footwear

### R08 — Reuse and physically valid outfit composition

Reuse garments and skins across as many appropriate outfits as needed. Each outfit declares complete layers, exact prototype/skin references, colour selections, intended placement/wear profile and workable wear order. Validate the ensemble as a whole, including compatible coverage, layer thickness and attachments; independently valid garments may still form an invalid outfit.

Use outfits to expose missing foundation layers, accessories and weather protection. A bodice must not describe a separately supplied skirt as attached; a coat must not permanently describe separately supplied medals. Optional/weather alternatives must resolve to explicit testable ensembles rather than ambiguous production entries. Document intended body/fit applicability without turning cultural or gender presentation into an unsupported wear restriction.

### R09 — Uniform purpose before affiliation

Distinguish working/fatigue, service, combat, dress and ceremonial uniform roles, with relevant climate/weather versions. National and branch differences become skins only within compatible construction. Camouflage pattern is presentation; concealment performance requires a real supported mechanic. Do not promise armour, weather protection or stealth solely through a skin.

Model removable insignia, badges, rank slides, medals and similar accessories independently where wearing, removing or exchanging them matters. Permanent compatible decoration may remain in a skin. Reference domain-owned PPE, armour and equipment where required, without duplicating them; do not load handheld weapons as worn clothing.

### R10 — Footwear breadth and cultural language

Provide functional, occupational, fashionable, sporting, religious, ceremonial and regional footwear. Compare construction, coverage, fastening, sole, heel, material and weather/activity suitability. A regional label alone neither creates a new base nor licenses collapsing a distinctive form into generic shoes.

Public text is predominantly descriptive. Established English garment names are acceptable even when inseparable from an Earth-specific cultural word. Record precise historical inspirations for builders without assuming the game world's nations or religions. Avoid a rigid country matrix, real brands and stereotypes about culture, poverty, manufacture or quality.

## Authored content and implementation obligations

### R11 — Finished prose is source, not generator output

An agent or human author must write and store every complete base and skin description. Follow the [item presentation guide](../Items/Item_System_Presentation_and_Integration.md), [content workflow](../Items/Item_System_Content_Workflows.md) and the physical-prose guidance in the [medieval clothing reference](./Medieval_Clothing_Seeder_Design_Reference.md); its medieval prices and period-specific exclusions do not replace this contract.

Descriptions should convey relevant visible material, shape, cut, drape, seams, folds, closures and finishing with editorial judgement, not the same checklist in the same order. Use concise, predominantly single-word noun heads and natural garment/pair grammar. Unskinned bases must also read as finished products. No filler, invented wearer reactions, unseen provenance, imaginary accompanying outfit, or unsupported material/mechanics claim is accepted.

Scripts may validate, package, fingerprint and export authored text. They must not assemble sentences from vocabulary matrices or overwrite curated descriptions during refresh. Source notes, review records and price derivations remain developer metadata, never persisted into player-facing descriptions or builder provenance comments.

### R12 — Source records and existing runtime seams

Retain source file/row locations, stable family/base/presentation references, exact era admissions, reuse/split rationale, physical dependencies, production route, price and quality rationale, colour bindings/exceptions, craft/outfit relationships and review status. Authoritative TSVs store finished text; generated audits/manifests record traceability and validation, not a second editable source of truth.

The current [typed catalogue](../../DatabaseSeeder/Seeders/IndustrialisedItemCatalogue.cs) has no skin collection, skin-specific craft product binding or rich ordered outfit-entry model. Its craft row describes a product and a single material input; its outfit row lists item references. Those are known integration gaps, not proof that the engine lacks skins or crafts.

Existing seams to reuse:

- [IGameItemSkin](../../FutureMUDLibrary/GameItems/IGameItemSkin.cs) targets one prototype and overrides presentation and optionally quality, not material, components, weight or inherent cost.
- [SimpleProduct](../../MudSharpCore/Work/Crafts/Products/SimpleProduct.cs) persists a product skin and applies craft quality. Confirm material overrides and colour outcomes remain compatible with the authored skin.
- [TemplateOutfitItem](../../MudSharpCore/GameItems/Inventory/TemplateOutfitItem.cs) carries skin, load arguments, wear profile, placement and order. Reuse these capabilities and prove colour coordination rather than assuming independent random loading is sufficient.
- [Documented clothing manifests](../../DatabaseSeeder/Seeders/ItemSeeder.ClothingOutfitManifests.cs) already reconcile skin-bearing historical stock. Extend the established provenance/manifest path rather than introducing another installer package.

Before bulk authoring, extend typed sources and preflight for these requirements and change the current [sync script](../../scripts/sync-industrialised-item-catalogue.ps1) into a safe consumer of authored clothing source. Its current refresh regenerates source rows; do not run it over curated clothing expecting preservation. Existing `-Check` success proves draft-generator parity, not this specification's completeness.

### R13 — Delivery waves

| Wave | Required deliverable | Exit dependency |
|---|---|---|
| 1. Inventory and evidence | Family/coverage matrix, reusable references, admissions, production routes, evidence plan, proposed ensembles and reconciled counts | Gate 1 |
| 2. Infrastructure | Typed authored skins/colour/craft/outfit support, safe source ownership, preflight and manifest/provenance integration | Gate 2 |
| 3. Representative proving set | Reviewed examples from every major family and difficult interaction, including multipart ensembles and production variants | Gate 3; complete before bulk authoring |
| 4. Catalogue authoring | Reviewed cross-domain waves of finished bases/skins, evidence, meaningful crafts, outfits and lifecycle relationships | Gates 4 and 5 |
| 5. Integration and acceptance | Fresh/update/customised-world proof, final audits/manifests, live inspection and documented acceptance | Gates 6 and 7 |

Each wave captures review and manifest evidence without independently activating an era. Later-era shared rows retain exact admissions and remain unavailable to ordinary installation until their era is selectable; canonical manifest capture still accounts for planned inactive shared admissions. New prerequisites go to their established owners. Concrete missing behaviour and dependent references go to the [dependency ledger](./FutureMUD_Item_Content_Engine_Dependency_Ledger.md) before admission.

Wave 1 working documents are the [garment inventory](./Industrialised_Clothing_Wave1_Inventory.md), [proposed outfits](./Industrialised_Clothing_Wave1_Outfits.md), [family and reuse review](./Industrialised_Clothing_Wave1_Reuse_Review.md) and [evidence, coverage and scope approval record](./Industrialised_Clothing_Wave1_Evidence_and_Coverage.md). Their references, counts and reviewed fingerprints are checked by the read-only [Wave 1 checker](../../scripts/check-industrialised-clothing-wave1.ps1) with `-SelfTest -CheckReview`. Their scope and counts are approved for implementation planning; they are not authored production descriptions or accepted finished content.

## R14 — Completion gates and evidence register

Use `not-started`, `in-progress`, `blocked` and `passed` for gate status. For each gate retain reviewer/owner, evidence paths or run IDs, date and tested source fingerprint, open blockers and any explicit approval. A changed dependency, source row or generated contract invalidates affected prior evidence. An automated pass cannot replace editorial review; no placeholder or unresolved critical dependency may pass production acceptance.

| Gate | Required proof | Current status | Evidence / blockers at this baseline |
|---|---|---|---|
| 1. Scope approved | Complete garment-family/coverage matrix; explicit inclusions/deferrals; separate base/skin/craft/outfit counts and programme reconciliation approved without padding | passed | User approved current counts and overall scope on 2026-09-03, with conventional colours moved to outfit defaults: 251 new bases, 113 reused bases, 697 additional skin briefs and 134 outfit proposals. Review, checks and fingerprints are in the Wave 1 approval record; later content/economic evidence remains conditional |
| 2. Dependencies resolved | All materials, components, characteristics, tags and references resolve; skin/base compatibility, layering, admissions and supported claims validate before mutation | not-started | Typed source gaps and safe generator conversion in R12 remain |
| 3. Proving set accepted | Reviewed finished prose and live examples cover every major family, multipart outfits, variable colours, overridable conventional outfit defaults, coordinated palettes, any explicitly approved fixed exception, production variants, uniforms and diverse footwear | not-started | No proving set accepted against this specification; no fixed exception is currently approved |
| 4. Content complete | Every admitted base/skin has reviewed finished prose; each unskinned base is a complete standalone garment, not a template; all colour exceptions/routes recorded; usable price evidence or approved analogues and independent quality rationale present | not-started | Generated clothing remains draft; research and editorial acceptance pending |
| 5. Crafts and outfits complete | Exact products/skins and ordered ensembles resolve; tools, inputs, quantities, phases, quality/failure paths work; complete outfits are wearable; colours survive materialisation and reload | not-started | Production crafts and complete skin-aware ensembles pending |
| 6. Repeatability proven | Fresh install, identical rerun, addition to pre-industrial worlds, untouched-stock updates, customised-record preservation, retirement and failed-preflight rollback pass | not-started | Existing replay history is not evidence for the replacement catalogue |
| 7. Production acceptance | Builds, focused/full suites, source/audit/manifest checks, database runs and live inspection pass; editorial sign-off and final counts recorded | not-started | Depends on Gates 1–6; no production sign-off |

### Verification scenarios

- Parser and source tests: exact headers, invariant numbers, stable ordering, source-file/line diagnostics, unique references, explicit admissions and authored-text preservation across refresh/check. All consumed new TSVs must affect the manifest fingerprint.
- Colour tests: every variable resolves; palette values render coherently; conventional defaults are overridable without new bases/skins; explicit selections take precedence and invalid values fail preflight. Load the same unskinned base and skin in differently coloured ensembles and independently of any outfit. Any approved fixed exception must be justified and internally consistent; coordinated outfits and crafted products retain their intended values after persistence/reload.
- Unskinned-base tests: load, craft, describe, wear and save/reload representative bases with no skin; verify complete text, valid colours, price/quality and behaviour. No base requires a skin to become usable, and its default presentation is not also counted as an additional skin.
- Composition tests: missing/wrong-prototype/stale skins, invalid components, non-solid materials, redundant tags, unknown characteristics, unsupported claims and incompatible outfit layers fail preflight with actionable diagnostics.
- Manufacture/value tests: expected routes and skin products, resolvable inputs/tools, quantities, success/failure phases, material compatibility and unmasked craft quality; evidence calculations and 1–2–5 quantisation; era-selection order cannot change a stable prototype's price or quality.
- Graph tests: complete references/admission compatibility, supported lifecycle links, craft acyclicity where required by the programme, exact outfit placement/order and wearable fit. Reuse must not introduce duplicate managed identities.
- Reconciliation tests: all Gate 6 scenarios, including skin/outfit changes and coordinated colours. Source refresh must not overwrite curated prose or customised live records.
- Final runs: MudSharpCore and DatabaseSeeder Debug builds, DatabaseSeeder Release build, focused and full DatabaseSeeder/MudSharpCore unit suites, neutral/custom Industrial replay profiles, prerequisite/catalogue/manifest `-Check`, fresh and populated database runs, and `git diff --check`. Use repository single-node Windows build/test guidance. Record skipped or unavailable runs as outstanding, never passed.
- Live proof: inspect representative items/skins from every major family and every distinct mechanical composition; wear complete multilayer outfits, load matching uniforms, craft representative hand/machine/batch products, and save/reload their state. Record commands, observed results and transcript paths.

Recalculate programme craft/lifecycle percentages using the accepted ordinary base-item population, never skins, repeated outfit use or multiple recipes for one base. Preserve the programme requirement for at least 35% directly craftable/cookable bases plus a distinct additional 20% with supported lifecycle participation; reconcile actual numerators and denominator after clothing changes. Do not force every domain independently to the programme percentages or count decorative graph edges as gameplay.

### Requirement traceability

| Requirements | Implementation obligation | Acceptance gates |
|---|---|---|
| R01–R03 | Inventory-first coverage, physical completeness and count reconciliation | 1, 3, 4, 5, 7 |
| R04–R05 | Correct identity/skin boundary, variable colours, overridable coordinated outfit defaults and individually approved exceptions only | 2, 3, 4, 5, 6 |
| R06–R07 | Truthful production, source-backed price and independent quality | 2, 3, 4, 5, 6, 7 |
| R08–R10 | Reusable complete outfits, uniform roles and diverse footwear/cultural treatment | 1, 2, 3, 4, 5, 7 |
| R11–R12 | Finished authored prose, metadata, safe source pipeline and runtime reuse | 2, 3, 4, 6, 7 |
| R13–R14 | Staged delivery, evidence, repeatability and explicit production acceptance | 1–7 |

## Boundaries and document acceptance

This change creates and synchronises documentation only. It does not author/install catalogue content, change source TSVs, runtime behaviour or era selectability, introduce a selectable package, assume an EF migration, publish or tag a release. Existing worktree changes remain intact.

The specification covers all four later eras but preserves Stage 2 shared/Industrial delivery and subsequent era gates. It introduces no speculative factory, clothing, camouflage or economy subsystem. Unsupported behaviour is either honestly excluded with approval or tracked as blocking work; a deferred requirement does not silently count as delivered.

Document acceptance is separate from package acceptance: all approved requirements must map to obligations and gates, local references must resolve, contradictory owning-document guidance must be reconciled and `git diff --check` must pass. Gate 1 scope approval is now recorded separately; production Gates 2–7 remain open.
