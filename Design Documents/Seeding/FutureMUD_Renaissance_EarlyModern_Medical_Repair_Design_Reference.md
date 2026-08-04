# Renaissance and Early Modern Medical and Repair Catalogue

**Status:** implemented generated item catalogue; data prerequisites are owned by the health and useful-seeder foundation slice.  **Era bands:** Renaissance 1400-1600; Early Modern 1600-1750.

The catalogue is deliberately product-led. A row is admitted only when its form, delivery, institution, repair target, or practical use differs; it does not multiply cosmetic colourways or provenance labels. `scripts/generate-era-medical-repair-catalogues.py --check` validates the generated embedded TSV catalogues.

| Era | Clinical / surgery | Apothecary / pharmacy | Drugs / delivery | Public health | Mobility / prosthesis | Veterinary | Repair | Raw medical stock | Total |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Renaissance | 96 | 70 | 60 | 38 | 32 | 22 | 30 | 18 | **366** |
| Early Modern | 168 | 142 | 154 | 82 | 74 | 46 | 66 | included in pharmacy | **732** |

Renaissance stock includes barber-surgeon, apothecary, hospital, campaign, obstetric, dental, veterinary, mobility and precision-repair forms current to 1400-1600. It excludes cinchona, ipecacuanha, variolation, isolated quinine, vaccination, ether, and post-1600 medical technology. Early Modern adds professional pocket, naval and hospital cases, pharmacy and public-health forms, variolation props, eighteenth-century surgical material, and raw pharmacy stock such as cinchona bark, ipecacuanha root, calomel, tartar emetic and Epsom salts. It excludes isolated quinine, vaccination and post-1750 technology; ether remains excluded.

## Mechanical gap ledger

| Substance or practice | Era | Current representation | Status | Missing seam / rationale |
| --- | --- | --- | --- | --- |
| Opium tincture | Renaissance and Early Modern | 100ml medicine vessel or concentrated dose | mechanic-now | Existing ingested analgesic, sedation, respiratory-risk and dependence vectors. |
| Laudanum | Early Modern only | 100ml medicine vessel or concentrated dose | mechanic-now | Existing ingested analgesic, sedation, respiratory-risk and dependence vectors. |
| Foxglove tincture | inherited historical baseline | no newly admitted catalogue row | mechanic-now | The older health tier already provides organ-function and adverse-effect vectors; this package does not assert a new pre-1750 digitalis claim. |
| Camphor, myrrh and rosewater topical preparations | both | topical wrapper | mechanic-now | Existing touched analgesic or respiratory presentation, without disease claims. |
| Senna, rhubarb and Epsom salts | dated Early Modern where applicable | ingested dose | mechanic-now | Existing nausea/need-rate effects are bounded presentation. |
| Tobacco and benzoin smoke | both where dated | smokeable wrapper | mechanic-now | Existing inhaled/dependence vectors; no cure claim. |
| Cinchona / Jesuit bark | Early Modern only | bark tonic prop/dose | engine-extension | Add intermittent-fever/malaria disease and disease-specific cure; no Renaissance cinchona. |
| Isolated quinine | post-era (1820) | deliberately absent | excluded-post-era | Use cinchona bark in this package; add quinine only in a later industrial-era catalogue with the same malaria extension. |
| Variolation | Early Modern only | inoculation prop | engine-extension | Add infection-risk plus durable immunity state; it is not vaccination. |
| Jennerian vaccination | post-era (1796) | deliberately absent | excluded-post-era | Add vaccination stock only in a later-era package with durable immunity and infection-risk mechanics. |
| Ether anaesthesia | post-era clinical use | deliberately absent | excluded-post-era | Add ether only in a later-era package; the older generic `pre-modern` health tier is not inherited here. |
| Mercury | both | ointment/powder prop | engine-extension | Dose-response poisoning and venereal-disease state required. |
| Antimony / tartar emetic | both / Early Modern | wine or dose prop | engine-extension | Explicit emesis and poisoning mechanics required. |
| Ergot | Early Modern | raw stock prop | engine-extension | Obstetric uterine-contraction and haemorrhage state required. |
| Ipecacuanha | Early Modern | syrup prop | engine-extension | Explicit emesis mechanic required. |
| Guaiacum and sarsaparilla | both where dated | decoction/syrup prop | engine-extension | Disease-specific claim requires a venereal-disease model. |
| Bezoar | both | raw/display prop | prop-only | No credible general mechanical effect; retain for apothecary scenes. |
| Mummy powder | both | raw/display prop | prop-only | No credible general mechanical effect; retain for apothecary scenes. |
| Powdered pearl | both | apothecary prop | prop-only | Deliberately no cure effect. |
| Sympathetic powder | both | apothecary prop | prop-only | Deliberately no distant or supernatural wound effect. |
| Weapon salve | both | apothecary prop | prop-only | Deliberately no distant or sympathetic wound effect. |
| Royal-touch token | both | institutional prop | prop-only | Builder-facing cultural material; no extension proposed. |
| Astrological diagnostic glass | both | diagnostic prop | prop-only | Builder-facing cultural material; no diagnostic extension proposed. |

## Foundation contract

The item package consumes the medical, market and repair target tags listed in the implementation request, six `Repair_*` precision component families at Poor/base/Good quality, automatic drug-delivery wrappers, and the era medicine vessels. All names are mirrored in the maintained material, liquid, gas, tag and item-component exports. It fails closed through `SeedStraightforwardEraCatalogueItems`; no fallback tag, material or component name is invented.

## Sources

- National Library of Medicine, [Cullen's 1789 *Treatise of the Materia Medica*](https://collections.nlm.nih.gov/catalog/nlm:nlmuid-2548014RX1-mvpart), used only to check formulation lineage and rejected for any item whose first evidence falls after the 1750 admission cutoff.
- Wellcome Collection, [surgical-instrument collections and eighteenth-century cases](https://wellcomecollection.org/search/works?query=surgical%20instrument).
- NCBI, [history of cinchona and quinine](https://www.ncbi.nlm.nih.gov/books/NBK234333/): bark reached Europe in the seventeenth century, while quinine was isolated in 1820.
- [Cinchona historical review](https://pmc.ncbi.nlm.nih.gov/articles/PMC5298425/), including seventeenth-century Jesuit powder evidence.
