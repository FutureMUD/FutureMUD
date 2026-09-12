# Industrialised Historical Pricing Methodology

## Purpose

This document governs the Stage 2 historical-price corpus at [historical-price-evidence.tsv](../../DatabaseSeeder/Seeders/IndustrialisedCatalogue/Pricing/historical-price-evidence.tsv). It is developer evidence for the one global `CostIndex`; it neither creates an installer choice nor changes ItemSeeder orchestration, in-world currency or economy prices.

The existing draft corpus includes seven named anchors—`industrial_food`, `industrial_tools`, `industrial_clothing`, `industrial_household`, `industrial_medical`, `industrial_weapons` and `industrial_vehicle`—and British, North American and Continental evidence gateways. Its late-industrial US catalogue anchors and British wage/survey references are research starting points, not complete coverage of every item or all four later-era bands. Technology-profile selection does not select a pricing locale or clothing culture. Each item needs its own admissible observation or a justified comparable-family analogue.

This methodology records approved requirements, not certification that existing TSV values were calculated correctly. The documentation-only clothing pass does not rewrite the corpus or item prices. Re-extract, calculate and validate those values during implementation; old generic anchor assignments are not grandfathered into production acceptance. See the [clothing specification](./FutureMUD_Industrialised_Clothing_Footwear_Uniforms_Design_Reference.md) for production-route and period-sensitive value/quality requirements.

## Evidence and row contract

The TSV fields are fixed:

`EvidenceId`, `EraBand`, `Locale`, `StartYear`, `EndYear`, `Currency`, `NominalPrice`, `QuotedUnit`, `DailyWage`, `LabourDays`, `CostIndex`, `SourceClass`, `ComparableFamily`, `Confidence`, `SourceUrl`, `SourcePage`, `Notes`.

An empty `NominalPrice`, `DailyWage`, `LabourDays` or `CostIndex` means a row is a source gateway, not a price ready for catalogue admission. It must never be treated as zero. Gateway records exist so the loader/maintainer can find the correct historical series without inventing an observation; a completed item evidence row supplies all four values and an exact table/page/image locator. The 1897 United States anchors use the Census historical-statistics lower-skilled weekly earnings observation of $8.40. This corpus applies an explicit six-day normalisation convention to obtain $1.40 per labour day; it records that assumption on every affected row rather than presenting it as a fact supplied by the wage table.

| Evidence class | May establish | Cannot establish |
|---|---|---|
| `primary-catalogue` | A finished good's quoted price in its stated market and year | An average national retail price or a different country's durable price |
| `official-survey` / `official-series` | Food/rent/wage basket and a documented index or rate | A SKU price missing from the table |
| `scholarly-series` / `scholarly-official-splice` | Time-series food, fuel, raw-material and wage comparisons | A finished manufactured retail price from CPI/wholesale data |
| `*-gateway` / dataset index | The discoverable source to extract | A quoted item price until the underlying series/table is named |

## Normalisation

Keep the original historical quote and quoted unit, and convert quantities explicitly to the item or batch being compared. Calculate affordability using a contemporaneous local unskilled-labour wage in the same nominal currency:

`LabourDays = NominalPrice / DailyWage`

Normalise each locale independently before combining observations. Record the wage occupation, locale, date, period, source/page and every unit conversion. A weekly/hourly wage requires an evidenced daily conversion or an explicit, reviewable working-time assumption; never silently impose a universal working day. The draft six-day convention above is an assumption to review, not a universal rule.

Combine only genuinely comparable observations: account for garment family, material/construction, production route, condition, quoted quantity and period. Record confidence weights and rationale before aggregation, and do not count republications of one quote as independent evidence. Use the weighted median of `log(LabourDays)` and exponentiate to obtain the representative labour days. A single valid observation remains itself. For an exact half-weight interval, use the midpoint in log space so the result is deterministic.

Calculate `RawCostIndex = 10 × RepresentativeLabourDays`. Quantise to the nearest member of `{1, 2, 5} × 10^n` in log space, choosing the lower value on an exact tie. Preserve raw observations, weights, aggregate and rounded value so rounding is auditable. Anchors provide sanity checks, not an alternative formula or a mandated hierarchy of goods. Any exceptional departure requires explicit documented approval; gameplay balancing must not silently replace the evidence calculation.

CPI may bridge nearby years only within the same locale, with the series, interval and adjustment recorded. Do not use CPI for cross-locale conversion. Wholesale indices, advertised catalogue prices and broad price series remain separately labelled; they cannot supply an absent retail quote. Exchange rates and present-day money conversions are not used. Freight, installation, accessories, credit and tax are excluded unless included in the quoted source and stated in `Notes`.

## Garment production, quality and stable economics

Assess hand construction, machine-assisted individual work and standardised batch manufacture by garment and period. Neither price nor quality has a universal ordering across those routes. A source-backed price difference needs its own rationale; higher price alone does not establish superior quality. Keep typical-quality reasoning separate from monetary evidence and record uncertainty rather than inventing historical quality scores.

Skins do not have an inherent-cost field. Mechanically similar garments with different intrinsic prices require distinct base identities, not conflicting prices attached to skins. Period-specific economic versions need explicit justification and admissions. A stable prototype has one deterministic stored price, independent of selected-era order; cosmetic colour, regional labelling and ordinary inflation are not reasons to duplicate it. Skin quality overrides are unset by default so craft-earned quality remains effective.

## Stage 2 admission

For each future typed catalogue row:

1. Find an appropriate source by date, locale, comparable family and production route; a gateway is only a discovery aid.
2. Capture a precise source page/table/image, quote, currency and sales unit; replace a gateway-derived empty field with the observed value.
3. Record the matched local unskilled wage and unit/time assumptions; calculate `LabourDays`, record comparison weights and aggregate comparable observations.
4. Calculate and quantise `10 × labour days`, preserving the derivation and evidence references in maintainer metadata without persisting them as builder comments.
5. Apply the normal Stage 2 dependency, prose, component, technology-profile, lifecycle and craft admission checks.

Every ordinary item references usable evidence or an explicitly approved analogue explaining material, construction, route, period and quantity comparability. High-value machinery, weapons, electrical goods and vehicles retain the programme's direct-contemporary-evidence requirement. A broad clothing anchor alone cannot justify all garments or an assumed handmade premium. Catalogue-source notes must not substitute for the exact price and wage source locators.

Validate positive quantities/wages, reproducible unit conversions, locale matching, aggregation and ladder rounding, unavailable/gateway observations, analogue approvals and seed-order determinism. Existing corpus rows must be rechecked against these rules before production acceptance. The evidence remains developer infrastructure, not a new installer choice, regional currency system or automatic repricing subsystem.
