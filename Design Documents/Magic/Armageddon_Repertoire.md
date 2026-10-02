# ARM-01 candidate repertoire register

Canonical data: [Armageddon_Repertoire.json](Armageddon_Repertoire.json). Historical inventory preparation revision: `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`. Current implementation baseline: `b0e4339d57bf15bc6e6a6f21fbdd5edc73006e38`.

Status: approved completion scope; runtime, stock and native qualification remain in progress. The exact selected roster is in [Armageddon_Sorcerer_Source_Tree.json](Armageddon_Sorcerer_Source_Tree.json); all 154 dispositions and 25 native cases are tracked in [Armageddon_Completion_Progress.json](Armageddon_Completion_Progress.json). This completion authority supersedes historical proposal text below. This rendering is generated from the JSON; source labels are provenance, not stock player-facing names.

154 entries: 152 historical magic IDs and 2 explicit additions. These are inventory counts, not completion percentages. Shared memberships appear on a single spell record.

## Reading the register

Coverage class describes the intended manifestation effect only. Every entry still depends on the proposed casting/progression/content foundation. A listed runtime method is evidence for its narrow primitive, not proof of the complete candidate spell. The selected 234 existing tests across two non-overlapping runs are documented in [the integration map](Armageddon_Casting_Integration.md); all new entry acceptance scenarios are NOT_RUN.

All grade, resource, practice, variant and production defaults are in [the design contract](Armageddon_Casting_Design.md). The energy formula applies modifiers only for selected variants. A historical no-op or unbound entry is explicitly an adaptation. `proposed_defer` is not an approved omission; no entry uses the approved-defer classification yet.

## Inventory

| Key | Generic name | Memberships | Effect coverage | Release proposal |
| --- | --- | --- | --- | --- |
| `arm.spell.sense_enchantment` | [Sense Enchantment](#sense-enchantment) | Fire, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.ember_lance` | [Ember Lance](#ember-lance) | Fire, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.cinder_rain` | [Cinder Rain](#cinder-rain) | Fire | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.hovering_light` | [Hovering Light](#hovering-light) | Fire, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.unravel_enchantment` | [Unravel Enchantment](#unravel-enchantment) | Fire, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.pillar_of_flame` | [Pillar of Flame](#pillar-of-flame) | Fire | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.consuming_flame` | [Consuming Flame](#consuming-flame) | Fire, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.kindle_enchantment` | [Kindle Enchantment](#kindle-enchantment) | Fire, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.custodian_glyph` | [Custodian Glyph](#custodian-glyph) | Fire, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.fireworks` | [Fireworks](#fireworks) | Fire | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.borrowed_tongues` | [Borrowed Tongues](#borrowed-tongues) | Fire, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.burning_draught_curse` | [Burning Draught Curse](#burning-draught-curse) | Fire | small_missing_primitive | required_completion_candidate |
| `arm.spell.desiccate` | [Desiccate](#desiccate) | Fire | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.flame_knife` | [Flame Knife](#flame-knife) | Fire, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.flame_barrier` | [Flame Barrier](#flame-barrier) | Fire, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.flame_mantle` | [Flame Mantle](#flame-mantle) | Fire | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.ember_seed` | [Ember Seed](#ember-seed) | Fire | small_missing_primitive | required_completion_candidate |
| `arm.spell.daylight` | [Daylight](#daylight) | Fire | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.immolate` | [Immolate](#immolate) | Fire, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.draw_water` | [Draw Water](#draw-water) | Water, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.sense_toxin` | [Sense Toxin](#sense-toxin) | Water, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.mend_flesh` | [Mend Flesh](#mend-flesh) | Water, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.venom_touch` | [Venom Touch](#venom-touch) | Water, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.sheltering_veil` | [Sheltering Veil](#sheltering-veil) | Water, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.purge_toxin` | [Purge Toxin](#purge-toxin) | Water, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.still_anger` | [Still Anger](#still-anger) | Water, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.spring_haven` | [Spring Haven](#spring-haven) | Water | larger_supporting_system | required_completion_candidate |
| `arm.spell.unyielding_veil` | [Unyielding Veil](#unyielding-veil) | Water, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.read_attunement` | [Read Attunement](#read-attunement) | Water, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.thunderclap` | [Thunderclap](#thunderclap) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.hush_hearing` | [Hush Hearing](#hush-hearing) | Water | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.seal_voice` | [Seal Voice](#seal-voice) | Water | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.vital_siphon` | [Vital Siphon](#vital-siphon) | Water | small_missing_primitive | required_completion_candidate |
| `arm.spell.thorn_barrier` | [Thorn Barrier](#thorn-barrier) | Water | small_missing_primitive | required_completion_candidate |
| `arm.spell.draw_wine` | [Draw Wine](#draw-wine) | Water, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.headlong_revel` | [Headlong Revel](#headlong-revel) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.clear_head` | [Clear Head](#clear-head) | Water | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.wither_growth` | [Wither Growth](#wither-growth) | Water | small_missing_primitive | required_completion_candidate |
| `arm.spell.distant_mirage` | [Distant Mirage](#distant-mirage) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.mist_shield` | [Mist Shield](#mist-shield) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.restorative_mud` | [Restorative Mud](#restorative-mud) | Water | small_missing_primitive | required_completion_candidate |
| `arm.spell.purge_disease` | [Purge Disease](#purge-disease) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.water_breathing` | [Water Breathing](#water-breathing) | Water, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.earthen_ward` | [Earthen Ward](#earthen-ward) | Earth | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.sustain_meal` | [Sustain Meal](#sustain-meal) | Earth, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.ground_tremor` | [Ground Tremor](#ground-tremor) | Earth, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.heavy_slumber` | [Heavy Slumber](#heavy-slumber) | Earth, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.earthen_strength` | [Earthen Strength](#earthen-strength) | Earth, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.sand_knife` | [Sand Knife](#sand-knife) | Earth | small_missing_primitive | required_completion_candidate |
| `arm.spell.stone_skin` | [Stone Skin](#stone-skin) | Earth, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.sapping_weight` | [Sapping Weight](#sapping-weight) | Earth | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.trace_the_path` | [Trace the Path](#trace-the-path) | Earth | small_missing_primitive | required_completion_candidate |
| `arm.spell.burrow_refuge` | [Burrow Refuge](#burrow-refuge) | Earth, Sorcerer | larger_supporting_system | required_completion_candidate |
| `arm.spell.clouded_will` | [Clouded Will](#clouded-will) | Earth, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.roused_fury` | [Roused Fury](#roused-fury) | Earth, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.earth_mount` | [Earth Mount](#earth-mount) | Earth, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.sand_barrier` | [Sand Barrier](#sand-barrier) | Earth, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.return_tether` | [Return Tether](#return-tether) | Earth | small_missing_primitive | required_completion_candidate |
| `arm.spell.threshold_alarm` | [Threshold Alarm](#threshold-alarm) | Earth, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.fleet_step` | [Fleet Step](#fleet-step) | Earth, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.sand_shelter` | [Sand Shelter](#sand-shelter) | Earth | larger_supporting_system | required_completion_candidate |
| `arm.spell.clay_sentinel` | [Clay Sentinel](#clay-sentinel) | Earth | small_missing_primitive | required_completion_candidate |
| `arm.spell.sand_effigy` | [Sand Effigy](#sand-effigy) | Earth, Sorcerer | larger_supporting_system | required_completion_candidate |
| `arm.spell.shatter_stone` | [Shatter Stone](#shatter-stone) | Earth | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.restore_object` | [Restore Object](#restore-object) | Earth, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.waystep` | [Waystep](#waystep) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.pierce_concealment` | [Pierce Concealment](#pierce-concealment) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.veil_from_sight` | [Veil from Sight](#veil-from-sight) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.draw_traveller` | [Draw Traveller](#draw-traveller) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.join_traveller` | [Join Traveller](#join-traveller) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.buoyant_lift` | [Buoyant Lift](#buoyant-lift) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.scouring_gust` | [Scouring Gust](#scouring-gust) | Wind | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.gust_hands` | [Gust Hands](#gust-hands) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.banish_traveller` | [Banish Traveller](#banish-traveller) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.air_guardian` | [Air Guardian](#air-guardian) | Wind, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.exchange_places` | [Exchange Places](#exchange-places) | Wind | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.windborne_flight` | [Windborne Flight](#windborne-flight) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.seeking_wind` | [Seeking Wind](#seeking-wind) | Wind | small_missing_primitive | required_completion_candidate |
| `arm.spell.gentle_descent` | [Gentle Descent](#gentle-descent) | Wind | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.driving_gust` | [Driving Gust](#driving-gust) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.wind_barrier` | [Wind Barrier](#wind-barrier) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.wind_mantle` | [Wind Mantle](#wind-mantle) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.wind_hammer` | [Wind Hammer](#wind-hammer) | Wind | small_missing_primitive | required_completion_candidate |
| `arm.spell.false_presence` | [False Presence](#false-presence) | Wind | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.wind_shield` | [Wind Shield](#wind-shield) | Wind | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.wind_courier` | [Wind Courier](#wind-courier) | Wind, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.reveal_the_hidden` | [Reveal the Hidden](#reveal-the-hidden) | Wind, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.anchor_rune` | [Anchor Rune](#anchor-rune) | Wind, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.shroud_eyes` | [Shroud Eyes](#shroud-eyes) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.clear_eyes` | [Clear Eyes](#clear-eyes) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.lift_hex` | [Lift Hex](#lift-hex) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.dread_presence` | [Dread Presence](#dread-presence) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.night_eyes` | [Night Eyes](#night-eyes) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.walking_shadow` | [Walking Shadow](#walking-shadow) | Shadow, Sorcerer | larger_supporting_system | required_completion_candidate |
| `arm.spell.shadow_passage` | [Shadow Passage](#shadow-passage) | Shadow, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.see_the_unbodied` | [See the Unbodied](#see-the-unbodied) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.cooling_shade` | [Cooling Shade](#cooling-shade) | Shadow | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.deep_darkness` | [Deep Darkness](#deep-darkness) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.clinging_hex` | [Clinging Hex](#clinging-hex) | Shadow, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.haunting_shape` | [Haunting Shape](#haunting-shape) | Shadow | small_missing_primitive | required_completion_candidate |
| `arm.spell.shadow_blade` | [Shadow Blade](#shadow-blade) | Shadow | small_missing_primitive | required_completion_candidate |
| `arm.spell.shadowplay` | [Shadowplay](#shadowplay) | Shadow | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.shadow_mantle` | [Shadow Mantle](#shadow-mantle) | Shadow | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.draw_into_flesh` | [Draw into Flesh](#draw-into-flesh) | Shadow, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.champion_blade` | [Champion Blade](#champion-blade) | Shadow | small_missing_primitive | required_completion_candidate |
| `arm.spell.lightning_lance` | [Lightning Lance](#lightning-lance) | Lightning, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.second_breath` | [Second Breath](#second-breath) | Lightning | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.wakeful_mind` | [Wakeful Mind](#wakeful-mind) | Lightning | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.leadfoot` | [Leadfoot](#leadfoot) | Lightning, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.steal_breath` | [Steal Breath](#steal-breath) | Lightning, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.still_limbs` | [Still Limbs](#still-limbs) | Lightning, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.leaping_lightning` | [Leaping Lightning](#leaping-lightning) | Lightning, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.storm_locus` | [Storm Locus](#storm-locus) | Lightning | small_missing_primitive | required_completion_candidate |
| `arm.spell.charged_aegis` | [Charged Aegis](#charged-aegis) | Lightning, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.luminous_trail` | [Luminous Trail](#luminous-trail) | Lightning | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.quickened_healing` | [Quickened Healing](#quickened-healing) | Lightning | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.lightning_pace` | [Lightning Pace](#lightning-pace) | Lightning, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.spark_lash` | [Spark Lash](#spark-lash) | Lightning | small_missing_primitive | required_completion_candidate |
| `arm.spell.sky_lantern` | [Sky Lantern](#sky-lantern) | Lightning | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.storm_spear` | [Storm Spear](#storm-spear) | Lightning, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.raise_servitor` | [Raise Servitor](#raise-servitor) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.call_outsider` | [Call Outsider](#call-outsider) | Void, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.arcane_mark` | [Arcane Mark](#arcane-mark) | Void, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.feign_death` | [Feign Death](#feign-death) | Void, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.silence_the_mind` | [Silence the Mind](#silence-the-mind) | Void | small_missing_primitive | required_completion_candidate |
| `arm.spell.devouring_touch` | [Devouring Touch](#devouring-touch) | Void | small_missing_primitive | required_completion_candidate |
| `arm.spell.compelling_regard` | [Compelling Regard](#compelling-regard) | Void | small_missing_primitive | required_completion_candidate |
| `arm.spell.unmaking_ward` | [Unmaking Ward](#unmaking-ward) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.severing_refuge` | [Severing Refuge](#severing-refuge) | Void | larger_supporting_system | required_completion_candidate |
| `arm.spell.apex_bane` | [Apex Bane](#apex-bane) | Void | larger_supporting_system | required_completion_candidate |
| `arm.spell.empty_aura` | [Empty Aura](#empty-aura) | Void, Sorcerer | existing_primitive_configuration | required_completion_candidate |
| `arm.spell.linked_threshold` | [Linked Threshold](#linked-threshold) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.bar_the_elements` | [Bar the Elements](#bar-the-elements) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.turn_the_elements` | [Turn the Elements](#turn-the-elements) | Void | small_missing_primitive | required_completion_candidate |
| `arm.spell.folded_pocket` | [Folded Pocket](#folded-pocket) | Void | larger_supporting_system | required_completion_candidate |
| `arm.spell.veil_of_elements` | [Veil of Elements](#veil-of-elements) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.cross_the_veil` | [Cross the Veil](#cross-the-veil) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.borrow_the_dead` | [Borrow the Dead](#borrow-the-dead) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.open_threshold` | [Open Threshold](#open-threshold) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.read_enchantment` | [Read Enchantment](#read-enchantment) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.blade_barrier` | [Blade Barrier](#blade-barrier) | Void, Sorcerer | small_missing_primitive | required_completion_candidate |
| `arm.spell.mind_scour` | [Mind Scour](#mind-scour) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.phantasm` | [Phantasm](#phantasm) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.unbodied_journey` | [Unbodied Journey](#unbodied-journey) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.erode_object` | [Erode Object](#erode-object) | Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.leeching_edge` | [Leeching Edge](#leeching-edge) | Void | small_missing_primitive | required_completion_candidate |
| `arm.spell.voice_of_remains` | [Voice of Remains](#voice-of-remains) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.echo_servant` | [Echo Servant](#echo-servant) | Void, Sorcerer | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.drowning_grip` | [Drowning Grip](#drowning-grip) | Water | small_missing_primitive | required_completion_candidate |
| `arm.spell.sow_sickness` | [Sow Sickness](#sow-sickness) | Water, Void | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.caustic_spray` | [Caustic Spray](#caustic-spray) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.gather_puddle` | [Gather Puddle](#gather-puddle) | Water | existing_primitive_plus_bounded_policy_content | required_completion_candidate |
| `arm.spell.wardcraft` | [Wardcraft](#wardcraft) | Earth, Water, Void | existing_primitive_plus_bounded_policy_content | optional_builder_addition |
| `arm.spell.renew_earth` | [Renew Earth](#renew-earth) | Earth, Water | existing_primitive_plus_bounded_policy_content | optional_builder_addition |

## Detailed entries

### Sense Enchantment

Key: `arm.spell.sense_enchantment`. Native school: **Fire**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Detect Magick`, skill ID 8, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:67`; `historical_not_live_parity`.

**Observable effect:** Reveal perceptible magical effects to the recipient.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sense_enchantment.skill`; shared option `arm.skill.fire_casting`.
- Sorcerer: once-only starting grant on explicit permanent enrolment. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.sense_enchantment.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use native perception and stacking; source guild immunities are not imported.

**Runtime evidence:**
- `detectmagick`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:717](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L717), `DetectMagickEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-8 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Sense Enchantment, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sense Enchantment at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Fire.
- Reveal perceptible magical effects to the recipient.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use native perception and stacking; source guild immunities are not imported.

Historical metadata (not proposed policy): element Fire; sphere Divination; mood Revealing; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174505-174507`, `codedump(1).c:84611-84614`, `codedump(1).c:217970-218009`, `codedump(1).c:84616-84648`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Ember Lance

Key: `arm.spell.ember_lance`. Native school: **Fire**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Fireball`, skill ID 11, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:87`; `historical_not_live_parity`.

**Observable effect:** Strike one visible target with fire damage.

**Scaling:** Damage 5*g native units; pain 2*g; stun 0; no automatic splash. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.ember_lance.skill`; shared option `arm.skill.fire_casting`.
- Sorcerer: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.ember_lance.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=eligible other characters in the same physical cell, frozen distinct group; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Elemental immunity and nearby-room echoes need explicit content policy; no automatic source-specific immunity.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-11 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Ember Lance, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Ember Lance at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Fire.
- Strike one visible target with fire damage.
- At grade 2 apply only this scaling contract: Damage 5*g native units; pain 2*g; stun 0; no automatic splash.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Elemental immunity and nearby-room echoes need explicit content policy; no automatic source-specific immunity.

Historical metadata (not proposed policy): element Fire; sphere Invocation; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174517-174519`, `codedump(1).c:87060-87064`, `codedump(1).c:219075-219121`, `codedump(1).c:87066-87133`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Cinder Rain

Key: `arm.spell.cinder_rain`. Native school: **Fire**. Target: `characters`. Lifecycle: `timed`.

**Provenance:** Historical `Rain Of Fire`, skill ID 21, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:107`; `historical_not_live_parity`.

**Observable effect:** Burn eligible others in the current cell with an initial fire strike and timed burning.

**Scaling:** Initial damage 3*g where configured; burn damage 2*g every 10 seconds for 30*g seconds. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.ember_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.cinder_rain.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Invocation.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** A frozen group cast, not a travelling weather hazard; plane exclusions must be authored.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `burning`: [MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs:131](../../MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs#L131), `BurningEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-21 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Cinder Rain, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Cinder Rain at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Fire.
- Burn eligible others in the current cell with an initial fire strike and timed burning.
- At grade 2 apply only this scaling contract: Initial damage 3*g where configured; burn damage 2*g every 10 seconds for 30*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: A frozen group cast, not a travelling weather hazard; plane exclusions must be authored.

Historical metadata (not proposed policy): element Fire; sphere Invocation; mood Destructive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components Invocation.

Historical dump offsets claimed by the reference: `codedump(1).c:174557-174559`, `codedump(1).c:93056-93059`, `codedump(1).c:221028-221053`, `codedump(1).c:93062-93175`, `codedump(1).c:87144-87195`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Hovering Light

Key: `arm.spell.hovering_light`. Native school: **Fire**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Ball Of Light`, skill ID 27, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:128`; `historical_not_live_parity`.

**Observable effect:** Create one temporary light around the recipient.

**Scaling:** One selected prototype; lifetime 300*g seconds; fixed prototype capabilities. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.hovering_light.skill`; shared option `arm.skill.fire_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.hovering_light.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Generic creation does not prove auto-equip or linked cleanup; use a worn-light lifecycle adapter or approve a glow-only substitute.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `glow`: [MudSharpCore/Magic/SpellEffects/GlowEffect.cs:211](../../MudSharpCore/Magic/SpellEffects/GlowEffect.cs#L211), `GlowEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-27 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Hovering Light, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Hovering Light at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Fire.
- Create one temporary light around the recipient.
- At grade 2 apply only this scaling contract: One selected prototype; lifetime 300*g seconds; fixed prototype capabilities.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Generic creation does not prove auto-equip or linked cleanup; use a worn-light lifecycle adapter or approve a glow-only substitute.

Historical metadata (not proposed policy): element Fire; sphere Creation; mood Neutral; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174581-174583`, `codedump(1).c:82290-82297`, `codedump(1).c:216903-216939`, `codedump(1).c:82299-82445`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Unravel Enchantment

Key: `arm.spell.unravel_enchantment`. Native school: **Fire**. Target: `perceivable`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Dispel Magick`, skill ID 29, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:148`; `historical_not_live_parity`.

**Observable effect:** Contest and remove spell-owned effects matching configured dispel keys.

**Scaling:** One normal dispel contest at mapped power; no automatic multi-parent success. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.unravel_enchantment.skill`; shared option `arm.skill.fire_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.unravel_enchantment.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not sweep arbitrary item flags or destroy every conjured object as a side effect.

**Runtime evidence:**
- `dispelmagic`: [MudSharpCore/Magic/SpellEffects/DispelMagicEffect.cs:165](../../MudSharpCore/Magic/SpellEffects/DispelMagicEffect.cs#L165), `DispelMagicEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-29 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Unravel Enchantment, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible perceivable. Invoke Unravel Enchantment at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Fire.
- Contest and remove spell-owned effects matching configured dispel keys.
- At grade 2 apply only this scaling contract: One normal dispel contest at mapped power; no automatic multi-parent success.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not sweep arbitrary item flags or destroy every conjured object as a side effect.

Historical metadata (not proposed policy): element Fire; sphere Enchantment; mood Destructive; targets no explicit target, ethereal plane target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174589-174591`, `codedump(1).c:218147-218393`, `codedump(1).c:84900-85900`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Pillar of Flame

Key: `arm.spell.pillar_of_flame`. Native school: **Fire**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Flamestrike`, skill ID 32, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:167`; `historical_not_live_parity`.

**Observable effect:** Deliver a stronger single-target fire strike.

**Scaling:** Damage 8*g native units; pain 3*g; stun 0. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.ember_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.pillar_of_flame.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=eligible other characters in the same physical cell, frozen distinct group; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native damage/resistance, with explicitly authored immunity and output; source plane rules are not assumed.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-32 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Pillar of Flame, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Pillar of Flame at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Fire.
- Deliver a stronger single-target fire strike.
- At grade 2 apply only this scaling contract: Damage 8*g native units; pain 3*g; stun 0.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native damage/resistance, with explicitly authored immunity and output; source plane rules are not assumed.

Historical metadata (not proposed policy): element Fire; sphere Clerical; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174601-174603`, `codedump(1).c:87198-87201`, `codedump(1).c:219170-219213`, `codedump(1).c:87203-87268`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Consuming Flame

Key: `arm.spell.consuming_flame`. Native school: **Fire**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Demonfire`, skill ID 40, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:187`; `historical_not_live_parity`.

**Observable effect:** Deliver an intense fire strike requiring a consumed reagent.

**Scaling:** Damage 8*g native units; pain 3*g; stun 0. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.pillar_of_flame`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.consuming_flame.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Necromancy.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No demonic affiliation or source racial exceptions; fixed extra material policy is explicit.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-40 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Consuming Flame, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Consuming Flame at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Fire.
- Deliver an intense fire strike requiring a consumed reagent.
- At grade 2 apply only this scaling contract: Damage 8*g native units; pain 3*g; stun 0.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No demonic affiliation or source racial exceptions; fixed extra material policy is explicit.

Historical metadata (not proposed policy): element Fire; sphere Necromancy; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components Necromancy.

Historical dump offsets claimed by the reference: `codedump(1).c:174633-174635`, `codedump(1).c:84458-84460`, `codedump(1).c:217839-217879`, `codedump(1).c:84462-84522`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Kindle Enchantment

Key: `arm.spell.kindle_enchantment`. Native school: **Fire**. Target: `item`. Lifecycle: `timed`.

**Provenance:** Historical `Empower`, skill ID 47, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:207`; `historical_not_live_parity`.

**Observable effect:** Temporarily improve a selected weapon's damage through native enchantment hooks.

**Scaling:** Weapon damage bonus g native units for 60*g seconds; other enchant bonuses zero. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.kindle_enchantment.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No source stat menu, explosion gamble or arbitrary attribute/resource capacity increase.

**Runtime evidence:**
- `itemenchant`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:668](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L668), `ItemEnchantEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-47 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Kindle Enchantment, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Kindle Enchantment at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Fire.
- Temporarily improve a selected weapon's damage through native enchantment hooks.
- At grade 2 apply only this scaling contract: Weapon damage bonus g native units for 60*g seconds; other enchant bonuses zero.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No source stat menu, explosion gamble or arbitrary attribute/resource capacity increase.

Historical metadata (not proposed policy): element Fire; sphere Enchantment; mood Neutral; targets no explicit target; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174661-174663`, `codedump(1).c:218552-218735`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Custodian Glyph

Key: `arm.spell.custodian_glyph`. Native school: **Fire**. Target: `item`. Lifecycle: `timed`.

**Provenance:** Historical `Glyph`, skill ID 51, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:225`; `historical_not_live_parity`.

**Observable effect:** Mark an item so unauthorised taking is refused while the glyph lasts.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.custodian_glyph.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Metadata alone does not intercept taking; requires a bounded inventory permission effect.

**Runtime evidence:**
- `magictag`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:91](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L91), `MagicTagEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-51 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Custodian Glyph, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Custodian Glyph at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Fire.
- Mark an item so unauthorised taking is refused while the glyph lasts.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Metadata alone does not intercept taking; requires a bounded inventory permission effect.

Historical metadata (not proposed policy): element Fire; sphere Enchantment; mood Harmful; targets object in inventory, object in room; minimum position Standing; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174677-174679`, `codedump(1).c:87807-87810`, `codedump(1).c:219562-219601`, `codedump(1).c:87812-87832`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Fireworks

Key: `arm.spell.fireworks`. Native school: **Fire**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Pyrotechnics`, skill ID 66, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:245`; `historical_not_live_parity`.

**Observable effect:** Display an audience-visible burst of coloured flame and light without physical damage.

**Scaling:** One non-interactive scene for 60*g seconds; added light where selected 10*g lux. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.fireworks.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Room text/light adaptation; no fake physical entities or implied fear mechanics.

**Runtime evidence:**
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-66 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Fireworks, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Fireworks at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Fire.
- Display an audience-visible burst of coloured flame and light without physical damage.
- At grade 2 apply only this scaling contract: One non-interactive scene for 60*g seconds; added light where selected 10*g lux.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Room text/light adaptation; no fake physical entities or implied fear mechanics.

Historical metadata (not proposed policy): element Fire; sphere Illusion; mood Passive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174738-174740`, `codedump(1).c:92604-92609`, `codedump(1).c:220959-220977`, `codedump(1).c:92611-92947`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Borrowed Tongues

Key: `arm.spell.borrowed_tongues`. Native school: **Fire**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Tongues`, skill ID 68, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:265`; `historical_not_live_parity`.

**Observable effect:** Allow comprehension of speech without granting fluent speech.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.borrowed_tongues.skill`; shared option `arm.skill.fire_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.borrowed_tongues.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use the native language comprehension contract and duration.

**Runtime evidence:**
- `comprehendlanguage`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:825](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L825), `ComprehendLanguageEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-68 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Borrowed Tongues, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Borrowed Tongues at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Fire.
- Allow comprehension of speech without granting fluent speech.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use the native language comprehension contract and duration.

Historical metadata (not proposed policy): element Fire; sphere Alteration; mood Revealing; targets character in room, self only; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174746-174748`, `codedump(1).c:96577-96581`, `codedump(1).c:222307-222351`, `codedump(1).c:96583-96616`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Burning Draught Curse

Key: `arm.spell.burning_draught_curse`. Native school: **Fire**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Firebreather`, skill ID 179, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:285`; `historical_not_live_parity`.

**Observable effect:** Make later drinking inflict configured burning harm instead of ordinary refreshment.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.burning_draught_curse.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Needs an ingestion event interceptor; an immediate damage or need delta is not equivalent.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-179 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Burning Draught Curse, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Burning Draught Curse at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Fire.
- Make later drinking inflict configured burning harm instead of ordinary refreshment.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs an ingestion event interceptor; an immediate damage or need delta is not equivalent.

Historical metadata (not proposed policy): element Fire; sphere Nature; mood Neutral; targets character in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175104-175106`, `codedump(1).c:87135-87142`, `codedump(1).c:219125-219166`, `codedump(1).c:87144-87195`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Desiccate

Key: `arm.spell.desiccate`. Native school: **Fire**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Parch`, skill ID 180, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:305`; `historical_not_live_parity`.

**Observable effect:** Reduce hydration and apply a small fire injury.

**Scaling:** Apply -5*g native ThirstPoints through FulfilNeeds with native clamps; fire damage 2*g units. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.desiccate.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Immediate adaptation; source conditional dehydration, firebreather and plane interactions are omitted proposals.

**Runtime evidence:**
- `needdelta`: [MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs:154](../../MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs#L154), `NeedDeltaEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-180 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Desiccate, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Desiccate at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Fire.
- Reduce hydration and apply a small fire injury.
- At grade 2 apply only this scaling contract: Apply -5*g native ThirstPoints through FulfilNeeds with native clamps; fire damage 2*g units.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Immediate adaptation; source conditional dehydration, firebreather and plane interactions are omitted proposals.

Historical metadata (not proposed policy): element Fire; sphere Nature; mood Destructive; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175108-175110`, `codedump(1).c:91286-91291`, `codedump(1).c:220630-220670`, `codedump(1).c:91293-91360`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Flame Knife

Key: `arm.spell.flame_knife`. Native school: **Fire**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Fire Jambiya`, skill ID 320, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:325`; `historical_not_live_parity`.

**Observable effect:** Place a temporary fire-themed knife in the recipient's inventory.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.flame_knife.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires temporary item ownership/cleanup proof; automatic wield and permanent high-grade versions are not proposed.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-320 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Flame Knife, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Flame Knife at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Fire.
- Place a temporary fire-themed knife in the recipient's inventory.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires temporary item ownership/cleanup proof; automatic wield and permanent high-grade versions are not proposed.

Historical metadata (not proposed policy): element Fire; sphere Conjuration; mood Aggressive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175540-175542`, `codedump(1).c:86963-86968`, `codedump(1).c:219034-219071`, `codedump(1).c:86971-87057`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Flame Barrier

Key: `arm.spell.flame_barrier`. Native school: **Fire**. Target: `exit`. Lifecycle: `timed`.

**Provenance:** Historical `Wall Of Fire`, skill ID 338, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:345`; `historical_not_live_parity`.

**Observable effect:** Block an exit with a timed barrier and burn actors who attempt to cross.

**Scaling:** Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.flame_barrier.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Blocking exists; traversal fire damage and both-side output require a bounded adapter.

**Runtime evidence:**
- `exitbarrier`: [MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs#L55), `ExitBarrierEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-338 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Flame Barrier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible exit. Invoke Flame Barrier at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Fire.
- Block an exit with a timed barrier and burn actors who attempt to cross.
- At grade 2 apply only this scaling contract: Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Blocking exists; traversal fire damage and both-side output require a bounded adapter.

Historical metadata (not proposed policy): element Fire; sphere Alteration; mood Protective; targets no explicit target; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175596-175598`, `codedump(1).c:97218-97227`, `codedump(1).c:222634-222732`, `codedump(1).c:97230-97234`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Flame Mantle

Key: `arm.spell.flame_mantle`. Native school: **Fire**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Fire Armor`, skill ID 351, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:365`; `historical_not_live_parity`.

**Observable effect:** Apply a finite protective armour layer with a visible warm glow.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.flame_mantle.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Protection follows native armour applicability, not source armour-class or terrain rules.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `glow`: [MudSharpCore/Magic/SpellEffects/GlowEffect.cs:211](../../MudSharpCore/Magic/SpellEffects/GlowEffect.cs#L211), `GlowEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-351 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Flame Mantle, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Flame Mantle at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Fire.
- Apply a finite protective armour layer with a visible warm glow.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Protection follows native armour applicability, not source armour-class or terrain rules.

Historical metadata (not proposed policy): element Fire; sphere Abjuration; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175640-175642`, `codedump(1).c:86768-86773`, `codedump(1).c:218953-218990`, `codedump(1).c:86775-86867`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Ember Seed

Key: `arm.spell.ember_seed`. Native school: **Fire**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Fire Seed`, skill ID 435, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:385`; `historical_not_live_parity`.

**Observable effect:** Create one temporary ember seed carrying a configured fire payload.

**Scaling:** One selected prototype; lifetime 300*g seconds; fixed prototype capabilities. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.ember_seed.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Creation alone is insufficient for delayed activation, paid payload and cleanup.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-435 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Ember Seed, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Ember Seed at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Fire.
- Create one temporary ember seed carrying a configured fire payload.
- At grade 2 apply only this scaling contract: One selected prototype; lifetime 300*g seconds; fixed prototype capabilities.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Creation alone is insufficient for delayed activation, paid payload and cleanup.

Historical metadata (not proposed policy): element Fire; sphere Creation; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175875-175876`, `codedump(1).c:94375-94375`, `codedump(1).c:222526-222555`, `codedump(1).c:94378-94465`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Daylight

Key: `arm.spell.daylight`. Native school: **Fire**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Daylight`, skill ID 452, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:405`; `normal_cast_noop`.

**Observable effect:** Increase room illumination for a bounded duration.

**Scaling:** Add 20*g lux for 60*g seconds. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.sense_enchantment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.daylight.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit proposed effect: source normal-cast body is empty; potion blindness removal is separate historical behavior.

**Runtime evidence:**
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-452 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Daylight, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Daylight at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Fire.
- Increase room illumination for a bounded duration.
- At grade 2 apply only this scaling contract: Add 20*g lux for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit proposed effect: source normal-cast body is empty; potion blindness removal is separate historical behavior.

Historical metadata (not proposed policy): element Fire; sphere Creation; mood Beneficial; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175922-175923`, `codedump(1).c:83778-83781`, `codedump(1).c:223197-223221`, `codedump(1).c:97913-97916`, `codedump(1).c:83783-83812`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Immolate

Key: `arm.spell.immolate`. Native school: **Fire**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Immolate`, skill ID 458, coded family Fire; `armageddon_magic_psionics_reference_second_pass.md:425`; `historical_not_live_parity`.

**Observable effect:** Apply timed fire injury ticks to the target.

**Scaling:** Initial damage 3*g where configured; burn damage 2*g every 10 seconds for 30*g seconds. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Fire: acquired `arm.spell.ember_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.immolate.skill`; shared option `arm.skill.fire_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No automatic stripping of all protection or source fire-armour immunity.

**Runtime evidence:**
- `burning`: [MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs:131](../../MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs#L131), `BurningEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-458 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Fire admission, acquired Immolate, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Immolate at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Fire.
- Apply timed fire injury ticks to the target.
- At grade 2 apply only this scaling contract: Initial damage 3*g where configured; burn damage 2*g every 10 seconds for 30*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No automatic stripping of all protection or source fire-armour immunity.

Historical metadata (not proposed policy): element Fire; sphere Enchantment; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175944-175946`, `codedump(1).c:89160-89164`, `codedump(1).c:219965-220011`, `codedump(1).c:89166-89248`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Draw Water

Key: `arm.spell.draw_water`. Native school: **Water**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Create Water`, skill ID 5, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:449`; `historical_not_live_parity`.

**Observable effect:** Add clean water to an accessible drink container up to capacity.

**Scaling:** Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.draw_water.skill`; shared option `arm.skill.water_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.draw_water.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Current CreateLiquidEffect empty-container capacity/null path needs a focused correction before claiming this complete.

**Runtime evidence:**
- `createliquid`: [MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs#L88), `CreateLiquidEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-5 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Draw Water, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Draw Water at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Water.
- Add clean water to an accessible drink container up to capacity.
- At grade 2 apply only this scaling contract: Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Current CreateLiquidEffect empty-container capacity/null path needs a focused correction before claiming this complete.

Historical metadata (not proposed policy): element Water; sphere Creation; mood Beneficial; targets object in inventory, object in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174493-174495`, `codedump(1).c:83637-83642`, `codedump(1).c:217482-217533`, `codedump(1).c:83644-83695`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sense Toxin

Key: `arm.spell.sense_toxin`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Detect Poison`, skill ID 9, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:469`; `historical_not_live_parity`.

**Observable effect:** Let the recipient inspect detectable active and latent drug poison state.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sense_toxin.skill`; shared option `arm.skill.water_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.sense_toxin.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native drug payloads, not historical poison flags or guaranteed identification of every substance.

**Runtime evidence:**
- `detectpoison`: [MudSharpCore/Magic/SpellEffects/DetectPoisonEffect.cs:34](../../MudSharpCore/Magic/SpellEffects/DetectPoisonEffect.cs#L34), `DetectPoisonEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-9 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Sense Toxin, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sense Toxin at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Water.
- Let the recipient inspect detectable active and latent drug poison state.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native drug payloads, not historical poison flags or guaranteed identification of every substance.

Historical metadata (not proposed policy): element Water; sphere Divination; mood Revealing; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174509-174511`, `codedump(1).c:84651-84657`, `codedump(1).c:218013-218052`, `codedump(1).c:84659-84691`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Mend Flesh

Key: `arm.spell.mend_flesh`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Heal`, skill ID 12, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:489`; `historical_not_live_parity`.

**Observable effect:** Heal existing eligible wounds, worst first, within the numeric healing budget.

**Scaling:** Heal at most 10*g native wound damage units, worst eligible wounds first. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.mend_flesh.skill`; shared option `arm.skill.water_casting`.
- Sorcerer: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.mend_flesh.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=eligible other characters in the same physical cell, frozen distinct group; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No resurrection or blanket repair of missing anatomy.

**Runtime evidence:**
- `heal`: [MudSharpCore/Magic/SpellEffects/HealEffect.cs:173](../../MudSharpCore/Magic/SpellEffects/HealEffect.cs#L173), `HealEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-12 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Mend Flesh, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Mend Flesh at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Water.
- Heal existing eligible wounds, worst first, within the numeric healing budget.
- At grade 2 apply only this scaling contract: Heal at most 10*g native wound damage units, worst eligible wounds first.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No resurrection or blanket repair of missing anatomy.

Historical metadata (not proposed policy): element Water; sphere Clerical; mood Beneficial; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174521-174523`, `codedump(1).c:88873-88875`, `codedump(1).c:219754-219809`, `codedump(1).c:88877-88916`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Venom Touch

Key: `arm.spell.venom_touch`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Poison`, skill ID 15, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:509`; `historical_not_live_parity`.

**Observable effect:** Introduce a configured stock toxin through an explicit vector.

**Scaling:** Apply 0.001*g grams of the explicitly selected stock drug through the configured vector, owned by a 60*g-second spell effect; removal withdraws its linked doses under native rules. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.venom_touch.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Character-only candidate; source food, weapon and liquid poisoning require separate carrier content.

**Runtime evidence:**
- `poison`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs:146](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs#L146), `PoisonEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-15 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Venom Touch, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Venom Touch at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Water.
- Introduce a configured stock toxin through an explicit vector.
- At grade 2 apply only this scaling contract: Apply 0.001*g grams of the explicitly selected stock drug through the configured vector, owned by a 60*g-second spell effect; removal withdraws its linked doses under native rules.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Character-only candidate; source food, weapon and liquid poisoning require separate carrier content.

Historical metadata (not proposed policy): element Water; sphere Clerical; mood Harmful; targets character in room, object in inventory, object in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174533-174535`, `codedump(1).c:91653-91656`, `codedump(1).c:220696-220737`, `codedump(1).c:91658-91726`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sheltering Veil

Key: `arm.spell.sheltering_veil`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Sanctuary`, skill ID 17, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:529`; `historical_not_live_parity`.

**Observable effect:** Give the target a finite protective envelope against configured damage.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sheltering_veil.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Clerical.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed armour adaptation; not a universal damage-halving sanctuary rule.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-17 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Sheltering Veil, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sheltering Veil at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Water.
- Give the target a finite protective envelope against configured damage.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed armour adaptation; not a universal damage-halving sanctuary rule.

Historical metadata (not proposed policy): element Water; sphere Clerical; mood Protective; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components Clerical.

Historical dump offsets claimed by the reference: `codedump(1).c:174541-174543`, `codedump(1).c:93796-93800`, `codedump(1).c:221381-221418`, `codedump(1).c:93802-93866`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Purge Toxin

Key: `arm.spell.purge_toxin`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Cure Poison`, skill ID 22, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:549`; `historical_not_live_parity`.

**Observable effect:** Remove matching spell-owned poison effects and their originator-linked drug doses.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.sense_toxin`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.purge_toxin.skill`; shared option `arm.skill.water_casting`.
- Sorcerer: acquired `arm.spell.sense_toxin`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.purge_toxin.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Matching requires the configured drug, vector and formula text. This proposed limited antidote does not cleanse arbitrary mundane poisoning, items or pathogens.

**Runtime evidence:**
- `removepoison`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs:307](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs#L307), `RemovePoisonEffect.RemoveEffects` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-22 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Purge Toxin, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Purge Toxin at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Water.
- Remove matching spell-owned poison effects and their originator-linked drug doses.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Matching requires the configured drug, vector and formula text. This proposed limited antidote does not cleanse arbitrary mundane poisoning, items or pathogens.

Historical metadata (not proposed policy): element Water; sphere Nature; mood Beneficial; targets character in room, object in inventory, object in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174561-174563`, `codedump(1).c:83815-83819`, `codedump(1).c:217624-217670`, `codedump(1).c:83821-83940`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Still Anger

Key: `arm.spell.still_anger`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Calm`, skill ID 30, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:569`; `historical_not_live_parity`.

**Observable effect:** Apply native pacifism to suppress aggressive behaviour for a duration.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.still_anger.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No blanket stripping of all rage, insomnia or speed effects unless separately configured.

**Runtime evidence:**
- `pacifism`: [MudSharpCore/Magic/SpellEffects/PacifismSpellEffect.cs:99](../../MudSharpCore/Magic/SpellEffects/PacifismSpellEffect.cs#L99), `PacifismSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-30 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Still Anger, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Still Anger at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Apply native pacifism to suppress aggressive behaviour for a duration.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No blanket stripping of all rage, insomnia or speed effects unless separately configured.

Historical metadata (not proposed policy): element Water; sphere Enchantment; mood Dominant; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174593-174595`, `codedump(1).c:83142-83147`, `codedump(1).c:217220-217258`, `codedump(1).c:83149-83244`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Spring Haven

Key: `arm.spell.spring_haven`. Native school: **Water**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Oasis`, skill ID 37, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:589`; `historical_not_live_parity`.

**Observable effect:** Create a temporary sheltered watering place without permanent terrain mutation.

**Scaling:** One haven for 300*g seconds, containing at most g litres of clean water. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.spring_haven.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs linked shelter/container lifecycle and occupancy-safe teardown; current room/liquid effects are only ingredients.

**Runtime evidence:**
- `createliquid`: [MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs#L88), `CreateLiquidEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `roomtemperature`: [MudSharpCore/Magic/SpellEffects/RoomTemperatureEffect.cs:191](../../MudSharpCore/Magic/SpellEffects/RoomTemperatureEffect.cs#L191), `RoomTemperatureEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-37 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Spring Haven, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Spring Haven at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Water.
- Create a temporary sheltered watering place without permanent terrain mutation.
- At grade 2 apply only this scaling contract: One haven for 300*g seconds, containing at most g litres of clean water.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs linked shelter/container lifecycle and occupancy-safe teardown; current room/liquid effects are only ingredients.

Historical metadata (not proposed policy): element Water; sphere Invocation; mood Beneficial; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174621-174623`, `codedump(1).c:91023-91029`, `codedump(1).c:220559-220583`, `codedump(1).c:91031-91125`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Unyielding Veil

Key: `arm.spell.unyielding_veil`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Invulnerability`, skill ID 53, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:609`; `historical_not_live_parity`.

**Observable effect:** Provide a stronger but finite armour pool.

**Scaling:** Absorption budget 20*g native damage units; duration 60*g seconds; still finite. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.unyielding_veil.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit substitute for absolute invulnerability; no universal immunity claim.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-53 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Unyielding Veil, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Unyielding Veil at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Water.
- Provide a stronger but finite armour pool.
- At grade 2 apply only this scaling contract: Absorption budget 20*g native damage units; duration 60*g seconds; still finite.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit substitute for absolute invulnerability; no universal immunity claim.

Historical metadata (not proposed policy): element Water; sphere Abjuration; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174686-174688`, `codedump(1).c:89893-89898`, `codedump(1).c:220198-220238`, `codedump(1).c:89900-89943`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Read Attunement

Key: `arm.spell.read_attunement`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Determine Relationship`, skill ID 54, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:629`; `historical_not_live_parity`.

**Observable effect:** Report an explicitly configured relationship between a target and elemental traditions.

**Scaling:** Enable the configured information overlay for 60*g seconds; disclosure stays permission-gated. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.read_attunement.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires an agreed attunement vocabulary and bounded query; historical setting relationships are unresolved.

**Runtime evidence:**
- `identify`: [MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs:191](../../MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs#L191), `IdentifySpellEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-54 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Read Attunement, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Read Attunement at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Report an explicitly configured relationship between a target and elemental traditions.
- At grade 2 apply only this scaling contract: Enable the configured information overlay for 60*g seconds; disclosure stays permission-gated.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires an agreed attunement vocabulary and bounded query; historical setting relationships are unresolved.

Historical metadata (not proposed policy): element Water; sphere Nature; mood Revealing; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174690-174692`, `codedump(1).c:84693-84697`, `codedump(1).c:218056-218097`, `codedump(1).c:84700-84742`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Thunderclap

Key: `arm.spell.thunderclap`. Native school: **Water**. Target: `characters`. Lifecycle: `timed`.

**Provenance:** Historical `Thunder`, skill ID 62, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:649`; `historical_not_live_parity`.

**Observable effect:** Strike local eligible others with sonic harm and temporary deafness.

**Scaling:** Sonic damage 3*g native units plus deafness for 30*g seconds. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.thunderclap.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No unlimited range, weather simulation or source-specific agility penalty.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `deafness`: [MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs:70](../../MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs#L70), `DeafnessEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-62 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Thunderclap, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Thunderclap at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Strike local eligible others with sonic harm and temporary deafness.
- At grade 2 apply only this scaling contract: Sonic damage 3*g native units plus deafness for 30*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No unlimited range, weather simulation or source-specific agility penalty.

Historical metadata (not proposed policy): element Water; sphere Invocation; mood Neutral; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174722-174724`, `codedump(1).c:96518-96521`, `codedump(1).c:222283-222303`, `codedump(1).c:96523-96574`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Hush Hearing

Key: `arm.spell.hush_hearing`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Deafness`, skill ID 73, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:669`; `historical_not_live_parity`.

**Observable effect:** Temporarily suppress the recipient's hearing.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.hush_hearing.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native deafness semantics and body applicability remain authoritative.

**Runtime evidence:**
- `deafness`: [MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs:70](../../MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs#L70), `DeafnessEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-73 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Hush Hearing, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Hush Hearing at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Temporarily suppress the recipient's hearing.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native deafness semantics and body applicability remain authoritative.

Historical metadata (not proposed policy): element Water; sphere Illusion; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174766-174768`, `codedump(1).c:84078-84081`, `codedump(1).c:217768-217813`, `codedump(1).c:84083-84166`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Seal Voice

Key: `arm.spell.seal_voice`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Silence`, skill ID 74, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:689`; `historical_not_live_parity`.

**Observable effect:** Prevent normal speech through the native magical silence status.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.seal_voice.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not suppress thought, every power or all output text.

**Runtime evidence:**
- `silence`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:168](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L168), `SilenceEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-74 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Seal Voice, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Seal Voice at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Water.
- Prevent normal speech through the native magical silence status.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not suppress thought, every power or all output text.

Historical metadata (not proposed policy): element Water; sphere Enchantment; mood Harmful; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174770-174772`, `codedump(1).c:95558-95563`, `codedump(1).c:221859-221904`, `codedump(1).c:95565-95626`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Vital Siphon

Key: `arm.spell.vital_siphon`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Health Drain`, skill ID 85, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:709`; `historical_not_live_parity`.

**Observable effect:** Harm a target and heal the caster by no more than the harm actually delivered.

**Scaling:** Attempt 5*g damage; heal caster by min(actual delivered damage, 5*g); never credit rejected harm. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.vital_siphon.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires linked measured transfer; independent damage and caster-heal effects would over-credit resisted harm.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `heal`: [MudSharpCore/Magic/SpellEffects/HealEffect.cs:173](../../MudSharpCore/Magic/SpellEffects/HealEffect.cs#L173), `HealEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-85 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Vital Siphon, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Vital Siphon at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Harm a target and heal the caster by no more than the harm actually delivered.
- At grade 2 apply only this scaling contract: Attempt 5*g damage; heal caster by min(actual delivered damage, 5*g); never credit rejected harm.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires linked measured transfer; independent damage and caster-heal effects would over-credit resisted harm.

Historical metadata (not proposed policy): element Water; sphere Invocation; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174808-174810`, `codedump(1).c:88919-88921`, `codedump(1).c:219813-219854`, `codedump(1).c:88923-88969`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Thorn Barrier

Key: `arm.spell.thorn_barrier`. Native school: **Water**. Target: `exit`. Lifecycle: `timed`.

**Provenance:** Historical `Wall Of Thorns`, skill ID 349, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:729`; `historical_not_live_parity`.

**Observable effect:** Block an exit with temporary thorns that injure crossing attempts.

**Scaling:** Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.thorn_barrier.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Traversal injury/entanglement and reciprocal presentation need explicit support.

**Runtime evidence:**
- `exitbarrier`: [MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs#L55), `ExitBarrierEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-349 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Thorn Barrier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible exit. Invoke Thorn Barrier at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Block an exit with temporary thorns that injure crossing attempts.
- At grade 2 apply only this scaling contract: Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Traversal injury/entanglement and reciprocal presentation need explicit support.

Historical metadata (not proposed policy): element Water; sphere Alteration; mood Protective; targets no explicit target; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175632-175634`, `codedump(1).c:222837-222935`, `codedump(1).c:97243-97247`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Draw Wine

Key: `arm.spell.draw_wine`. Native school: **Water**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Create Wine`, skill ID 372, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:748`; `historical_not_live_parity`.

**Observable effect:** Add configured wine to an accessible drink container without exceeding capacity.

**Scaling:** Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.draw_wine.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Same empty-container path concern as Draw Water; never silently convert incompatible mixtures.

**Runtime evidence:**
- `createliquid`: [MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs#L88), `CreateLiquidEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-372 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Draw Wine, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Draw Wine at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Add configured wine to an accessible drink container without exceeding capacity.
- At grade 2 apply only this scaling contract: Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Same empty-container path concern as Draw Water; never silently convert incompatible mixtures.

Historical metadata (not proposed policy): element Water; sphere Creation; mood Dominant; targets object in inventory, object in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175705-175707`, `codedump(1).c:83697-83701`, `codedump(1).c:217537-217583`, `codedump(1).c:83703-83775`, `codedump(1).c:89751-89799`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Headlong Revel

Key: `arm.spell.headlong_revel`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Intoxication`, skill ID 377, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:769`; `historical_not_live_parity`.

**Observable effect:** Increase native drunkenness without conjuring permanent drink objects.

**Scaling:** Apply +0.01*g AlcoholLitres through native FulfilNeeds, subject to native bounds. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.headlong_revel.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit adaptation from incompletely described source item behaviour.

**Runtime evidence:**
- `needdelta`: [MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs:154](../../MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs#L154), `NeedDeltaEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-377 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Headlong Revel, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Headlong Revel at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Increase native drunkenness without conjuring permanent drink objects.
- At grade 2 apply only this scaling contract: Apply +0.01*g AlcoholLitres through native FulfilNeeds, subject to native bounds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit adaptation from incompletely described source item behaviour.

Historical metadata (not proposed policy): element Water; sphere Alteration; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175724-175726`, `codedump(1).c:89746-89749`, `codedump(1).c:220108-220149`, `codedump(1).c:89751-89799`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Clear Head

Key: `arm.spell.clear_head`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Sober`, skill ID 378, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:789`; `historical_not_live_parity`.

**Observable effect:** Reduce native drunkenness toward zero.

**Scaling:** Apply -0.01*g AlcoholLitres through native FulfilNeeds, subject to the native zero floor. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.clear_head.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not remove unrelated drug effects or restore spent stamina.

**Runtime evidence:**
- `needdelta`: [MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs:154](../../MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs#L154), `NeedDeltaEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-378 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Clear Head, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Clear Head at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Water.
- Reduce native drunkenness toward zero.
- At grade 2 apply only this scaling contract: Apply -0.01*g AlcoholLitres through native FulfilNeeds, subject to the native zero floor.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not remove unrelated drug effects or restore spent stamina.

Historical metadata (not proposed policy): element Water; sphere Alteration; mood Neutral; targets character in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175727-175729`, `codedump(1).c:95837-95839`, `codedump(1).c:221998-222032`, `codedump(1).c:95841-95868`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wither Growth

Key: `arm.spell.wither_growth`. Native school: **Water**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Wither`, skill ID 379, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:809`; `historical_not_live_parity`.

**Observable effect:** Damage a targeted plant-like item through an explicit material/tag gate.

**Scaling:** Apply at most 5*g native item damage units to eligible material; do not damage contents. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wither_growth.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source sap transfer and living land/crop accounting are not ordinary item damage; no ecological debit bypass.

**Runtime evidence:**
- `itemdamage`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:345](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L345), `ItemDamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-379 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Wither Growth, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Wither Growth at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Damage a targeted plant-like item through an explicit material/tag gate.
- At grade 2 apply only this scaling contract: Apply at most 5*g native item damage units to eligible material; do not damage contents.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source sap transfer and living land/crop accounting are not ordinary item damage; no ecological debit bypass.

Historical metadata (not proposed policy): element Water; sphere Nature; mood Harmful; targets no explicit target; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175730-175732`, `codedump(1).c:97491-97499`, `codedump(1).c:223171-223194`, `codedump(1).c:97501-97586`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Distant Mirage

Key: `arm.spell.distant_mirage`. Native school: **Water**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Mirage`, skill ID 382, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:829`; `historical_not_live_parity`.

**Observable effect:** Show selected viewers a non-interactive misleading scene.

**Scaling:** One non-interactive scene for 60*g seconds; added light where selected 10*g lux. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.distant_mirage.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not change thirst, terrain facts, navigation or create real entities.

**Runtime evidence:**
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-382 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Distant Mirage, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Distant Mirage at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Water.
- Show selected viewers a non-interactive misleading scene.
- At grade 2 apply only this scaling contract: One non-interactive scene for 60*g seconds; added light where selected 10*g lux.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not change thirst, terrain facts, navigation or create real entities.

Historical metadata (not proposed policy): element Water; sphere Illusion; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175739-175741`, `codedump(1).c:90565-90572`, `codedump(1).c:220510-220527`, `codedump(1).c:90574-90852`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Mist Shield

Key: `arm.spell.mist_shield`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Shield Of Mist`, skill ID 427, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:849`; `historical_not_live_parity`.

**Observable effect:** Surround a character with a finite mist-themed protective layer.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.mist_shield.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No conjured shield item or automatic concealment; proposed presentation adaptation.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-427 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Mist Shield, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Mist Shield at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Water.
- Surround a character with a finite mist-themed protective layer.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No conjured shield item or automatic concealment; proposed presentation adaptation.

Historical metadata (not proposed policy): element Water; sphere Conjuration; mood Protective; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175850-175851`, `codedump(1).c:95168-95170`, `codedump(1).c:221667-221702`, `codedump(1).c:95172-95261`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Restorative Mud

Key: `arm.spell.restorative_mud`. Native school: **Water**. Target: `item`. Lifecycle: `created_payload`.

**Provenance:** Historical `Healing Mud`, skill ID 429, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:869`; `historical_not_live_parity`.

**Observable effect:** Produce one paid topical healing dose in a mud carrier.

**Scaling:** One standard gram dose; heal 10*g native wound units once on authorised topical exposure. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.restorative_mud.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Exposure payload is feasible, but preset manufacturing and frozen-grade binding are not implemented.

**Runtime evidence:**
- `heal`: [MudSharpCore/Magic/SpellEffects/HealEffect.cs:173](../../MudSharpCore/Magic/SpellEffects/HealEffect.cs#L173), `HealEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-429 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Restorative Mud, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Restorative Mud at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Water.
- Produce one paid topical healing dose in a mud carrier.
- At grade 2 apply only this scaling contract: One standard gram dose; heal 10*g native wound units once on authorised topical exposure.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Exposure payload is feasible, but preset manufacturing and frozen-grade binding are not implemented.

Historical metadata (not proposed policy): element Water; sphere Nature; mood Neutral; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175856-175857`, `codedump(1).c:95357-95359`, `codedump(1).c:221745-221780`, `codedump(1).c:95361-95421`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Purge Disease

Key: `arm.spell.purge_disease`. Native school: **Water**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Cure Disease`, skill ID 431, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:889`; `unbound_entry`.

**Observable effect:** Remove a matching spell-owned disease effect and its tracked native infection.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.purge_toxin`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.purge_disease.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Historical skill entry is unbound. Matching uses infection type, difficulty and formula text; this proposed adaptation does not cure arbitrary naturally acquired infections.

**Runtime evidence:**
- `removedisease`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs:633](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs#L633), `RemoveDiseaseEffect.RemoveEffects` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-431 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Purge Disease, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Purge Disease at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Water.
- Remove a matching spell-owned disease effect and its tracked native infection.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Historical skill entry is unbound. Matching uses infection type, difficulty and formula text; this proposed adaptation does not cure arbitrary naturally acquired infections.

Historical metadata (not proposed policy): element Water; sphere Enchantment; mood Beneficial; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175862-175863`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Water Breathing

Key: `arm.spell.water_breathing`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Breathe Water`, skill ID 443, coded family Water; `armageddon_magic_psionics_reference_second_pass.md:902`; `historical_not_live_parity`.

**Observable effect:** Allow compatible breathing underwater while the spell lasts.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.water_breathing.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No survival promise in every liquid, vacuum or poisonous atmosphere.

**Runtime evidence:**
- `waterbreathing`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:501](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L501), `WaterBreathingEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-443 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Water Breathing, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Water Breathing at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Water.
- Allow compatible breathing underwater while the spell lasts.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No survival promise in every liquid, vacuum or poisonous atmosphere.

Historical metadata (not proposed policy): element Water; sphere Abjuration; mood Beneficial; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175892-175893`, `codedump(1).c:82927-82930`, `codedump(1).c:217139-217178`, `codedump(1).c:82932-82991`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Earthen Ward

Key: `arm.spell.earthen_ward`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Armor`, skill ID 1, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:926`; `historical_not_live_parity`.

**Observable effect:** Apply a finite earthen protective armour layer.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.earthen_ward.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.earthen_ward.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native armour types, material and bodypart applicability must be configured.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-1 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Earthen Ward, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Earthen Ward at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Apply a finite earthen protective armour layer.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native armour types, material and bodypart applicability must be configured.

Historical metadata (not proposed policy): element Earth; sphere Abjuration; mood Protective; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174477-174479`, `codedump(1).c:82121-82126`, `codedump(1).c:216815-216854`, `codedump(1).c:82128-82247`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sustain Meal

Key: `arm.spell.sustain_meal`. Native school: **Earth**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Create Food`, skill ID 4, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:946`; `historical_not_live_parity`.

**Observable effect:** Create edible stock food with specified nutrition in the recipient's inventory or at their feet.

**Scaling:** Create g portions of a fixed stock food prototype; each portion has its authored nutrition. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sustain_meal.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.sustain_meal.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed lasting ordinary food; source timed disappearance is deliberately not claimed.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-4 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sustain Meal, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sustain Meal at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Create edible stock food with specified nutrition in the recipient's inventory or at their feet.
- At grade 2 apply only this scaling contract: Create g portions of a fixed stock food prototype; each portion has its authored nutrition.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed lasting ordinary food; source timed disappearance is deliberately not claimed.

Historical metadata (not proposed policy): element Earth; sphere Creation; mood Beneficial; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174489-174491`, `codedump(1).c:83516-83524`, `codedump(1).c:217344-217379`, `codedump(1).c:83526-83621`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Ground Tremor

Key: `arm.spell.ground_tremor`. Native school: **Earth**. Target: `characters`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Earthquake`, skill ID 10, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:966`; `historical_not_live_parity`.

**Observable effect:** Shake grounded local targets, injuring them and testing for a fall.

**Scaling:** Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.ground_tremor.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Damage is available; grounding and knockdown admission require a bounded composite.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-10 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Ground Tremor, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Ground Tremor at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Earth.
- Shake grounded local targets, injuring them and testing for a fall.
- At grade 2 apply only this scaling contract: Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Damage is available; grounding and knockdown admission require a bounded composite.

Historical metadata (not proposed policy): element Earth; sphere Nature; mood Destructive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174513-174515`, `codedump(1).c:86094-86101`, `codedump(1).c:218487-218505`, `codedump(1).c:86103-86172`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Heavy Slumber

Key: `arm.spell.heavy_slumber`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Sleep`, skill ID 18, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:986`; `historical_not_live_parity`.

**Observable effect:** Put a susceptible target into native magical sleep.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.heavy_slumber.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.heavy_slumber.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Existing insomnia interaction remains authoritative.

**Runtime evidence:**
- `sleep`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:222](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L222), `SleepEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-18 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Heavy Slumber, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Heavy Slumber at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Earth.
- Put a susceptible target into native magical sleep.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Existing insomnia interaction remains authoritative.

Historical metadata (not proposed policy): element Earth; sphere Enchantment; mood Dominant; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174545-174547`, `codedump(1).c:95629-95634`, `codedump(1).c:221908-221948`, `codedump(1).c:95636-95757`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Earthen Strength

Key: `arm.spell.earthen_strength`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Strength`, skill ID 19, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1006`; `historical_not_live_parity`.

**Observable effect:** Increase the configured strength trait for a duration.

**Scaling:** Add g native units to the selected trait for 60*g seconds. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.earthen_strength.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.earthen_strength.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use native trait modifiers; no duplicate base-stat mutation.

**Runtime evidence:**
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-19 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Earthen Strength, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Earthen Strength at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Increase the configured strength trait for a duration.
- At grade 2 apply only this scaling contract: Add g native units to the selected trait for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use native trait modifiers; no duplicate base-stat mutation.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Beneficial; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174549-174551`, `codedump(1).c:96167-96170`, `codedump(1).c:222169-222204`, `codedump(1).c:96172-96224`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sand Knife

Key: `arm.spell.sand_knife`. Native school: **Earth**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Sand Jambiya`, skill ID 33, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1026`; `historical_not_live_parity`.

**Observable effect:** Create one temporary sand knife.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sand_knife.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires timed item cleanup; no automatic wielding or permanent highest-grade knife.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-33 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sand Knife, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sand Knife at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Earth.
- Create one temporary sand knife.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires timed item cleanup; no automatic wielding or permanent highest-grade knife.

Historical metadata (not proposed policy): element Earth; sphere Creation; mood Neutral; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174605-174607`, `codedump(1).c:93869-93874`, `codedump(1).c:221422-221469`, `codedump(1).c:93876-93991`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Stone Skin

Key: `arm.spell.stone_skin`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Stone Skin`, skill ID 34, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1046`; `historical_not_live_parity`.

**Observable effect:** Apply stone-like protection with an agility penalty.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.stone_skin.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.earthen_ward`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.stone_skin.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Concrete armour-plus-trait adaptation; native body armour rules are retained.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-34 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Stone Skin, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Stone Skin at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Earth.
- Apply stone-like protection with an agility penalty.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Concrete armour-plus-trait adaptation; native body armour rules are retained.

Historical metadata (not proposed policy): element Stone; sphere Abjuration; mood Protective; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174609-174611`, `codedump(1).c:96067-96071`, `codedump(1).c:222123-222165`, `codedump(1).c:96073-96164`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sapping Weight

Key: `arm.spell.sapping_weight`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Weaken`, skill ID 35, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1066`; `historical_not_live_parity`.

**Observable effect:** Apply a negative modifier to the configured strength trait.

**Scaling:** Subtract g native units from the selected trait for 60*g seconds. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.earthen_strength`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sapping_weight.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.earthen_strength`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.sapping_weight.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No source race/stat special cases are implied.

**Runtime evidence:**
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-35 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sapping Weight, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sapping Weight at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Apply a negative modifier to the configured strength trait.
- At grade 2 apply only this scaling contract: Subtract g native units from the selected trait for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No source race/stat special cases are implied.

Historical metadata (not proposed policy): element Earth; sphere Necromancy; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174613-174615`, `codedump(1).c:97256-97259`, `codedump(1).c:223042-223088`, `codedump(1).c:97261-97310`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Trace the Path

Key: `arm.spell.trace_the_path`. Native school: **Earth**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Show The Path`, skill ID 39, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1086`; `historical_not_live_parity`.

**Observable effect:** Reveal a permitted bounded route toward a known target.

**Scaling:** Inspect at most 10*g traversable cell edges; report only permission-eligible path information. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.trace_the_path.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Needs a permission-aware path information effect; teleport and track tokens are not proof of this behavior.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-39 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Trace the Path, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Trace the Path at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Reveal a permitted bounded route toward a known target.
- At grade 2 apply only this scaling contract: Inspect at most 10*g traversable cell edges; report only permission-eligible path information.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs a permission-aware path information effect; teleport and track tokens are not proof of this behavior.

Historical metadata (not proposed policy): element Earth; sphere Nature; mood Revealing; targets character in world; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174629-174631`, `codedump(1).c:95491-95498`, `codedump(1).c:221823-221855`, `codedump(1).c:95500-95555`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Burrow Refuge

Key: `arm.spell.burrow_refuge`. Native school: **Earth**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Burrow`, skill ID 46, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1105`; `historical_not_live_parity`.

**Observable effect:** Create a temporary underground refuge with a safe return route.

**Scaling:** One temporary refuge/return link for 300*g seconds; capacity g ordinary occupants. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.burrow_refuge.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Requires temporary cell topology, terrain admission, occupancy and shutdown teardown rules.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-46 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Burrow Refuge, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Burrow Refuge at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Earth.
- Create a temporary underground refuge with a safe return route.
- At grade 2 apply only this scaling contract: One temporary refuge/return link for 300*g seconds; capacity g ordinary occupants.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires temporary cell topology, terrain admission, occupancy and shutdown teardown rules.

Historical metadata (not proposed policy): element Earth; sphere Invocation; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174657-174659`, `codedump(1).c:82994-83000`, `codedump(1).c:217182-217216`, `codedump(1).c:83002-83139`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Clouded Will

Key: `arm.spell.clouded_will`. Native school: **Earth**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Feeblemind`, skill ID 48, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1125`; `historical_not_live_parity`.

**Observable effect:** Reduce the target's explicitly configured magical reserve.

**Scaling:** Subtract min(current amount, 5*g) from the configured reserve; caster gains zero. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.clouded_will.skill`; shared option `arm.skill.earth_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.clouded_will.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Adaptation to the source mana reduction, not generic cognition or command impairment.

**Runtime evidence:**
- `magicresourcedelta`: [MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs:75](../../MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs#L75), `MagicResourceDeltaEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-48 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Clouded Will, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Clouded Will at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Reduce the target's explicitly configured magical reserve.
- At grade 2 apply only this scaling contract: Subtract min(current amount, 5*g) from the configured reserve; caster gains zero.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Adaptation to the source mana reduction, not generic cognition or command impairment.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Harmful; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174665-174667`, `codedump(1).c:86696-86699`, `codedump(1).c:218911-218949`, `codedump(1).c:86701-86765`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Roused Fury

Key: `arm.spell.roused_fury`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Fury`, skill ID 49, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1145`; `historical_not_live_parity`.

**Observable effect:** Apply native rage intensity for a duration.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.roused_fury.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Do not assume the exact historical aggression or charm flags.

**Runtime evidence:**
- `rage`: [MudSharpCore/Magic/SpellEffects/RageSpellEffect.cs:100](../../MudSharpCore/Magic/SpellEffects/RageSpellEffect.cs#L100), `RageSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-49 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Roused Fury, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Roused Fury at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Earth.
- Apply native rage intensity for a duration.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Do not assume the exact historical aggression or charm flags.

Historical metadata (not proposed policy): element Earth; sphere Enchantment; mood Harmful; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174669-174671`, `codedump(1).c:87470-87473`, `codedump(1).c:219332-219368`, `codedump(1).c:87475-87590`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Earth Mount

Key: `arm.spell.earth_mount`. Native school: **Earth**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Mount`, skill ID 56, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1165`; `historical_not_live_parity`.

**Observable effect:** Create a rideable temporary earth creature belonging to its summoner.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.earth_mount.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** NPC creation is present; mount control and expiry with riders need bounded lifecycle work.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-56 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Earth Mount, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Earth Mount at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Earth.
- Create a rideable temporary earth creature belonging to its summoner.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: NPC creation is present; mount control and expiry with riders need bounded lifecycle work.

Historical metadata (not proposed policy): element Earth; sphere Creation; mood Dominant; targets no explicit target; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174698-174700`, `codedump(1).c:90855-90860`, `codedump(1).c:220531-220555`, `codedump(1).c:90862-91020`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sand Barrier

Key: `arm.spell.sand_barrier`. Native school: **Earth**. Target: `exit`. Lifecycle: `timed`.

**Provenance:** Historical `Wall Of Sand`, skill ID 64, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1185`; `historical_not_live_parity`.

**Observable effect:** Block a chosen exit temporarily with packed sand.

**Scaling:** Block the selected exit for 60*g seconds; no traversal damage. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sand_barrier.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit no-damage barrier adaptation, not hidden cell construction.

**Runtime evidence:**
- `exitbarrier`: [MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs#L55), `ExitBarrierEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-64 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sand Barrier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible exit. Invoke Sand Barrier at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Block a chosen exit temporarily with packed sand.
- At grade 2 apply only this scaling contract: Block the selected exit for 60*g seconds; no traversal damage.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit no-damage barrier adaptation, not hidden cell construction.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Protective; targets no explicit target; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174730-174732`, `codedump(1).c:222736-222832`, `codedump(1).c:97236-97240`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Return Tether

Key: `arm.spell.return_tether`. Native school: **Earth**. Target: `character`. Lifecycle: `delayed`.

**Provenance:** Historical `Rewind`, skill ID 65, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1204`; `historical_not_live_parity`.

**Observable effect:** Record the caster's position and return them after a bounded delay if still eligible.

**Scaling:** Record one position; attempt return after 30*g seconds; no rewind of health or inventory. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.return_tether.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs durable delayed-return identity and cancellation semantics; not time reversal of health or inventory.

**Runtime evidence:**
- `teleporttarget`: [MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs:75](../../MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs#L75), `TeleportTargetEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-65 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Return Tether, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Return Tether at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Earth.
- Record the caster's position and return them after a bounded delay if still eligible.
- At grade 2 apply only this scaling contract: Record one position; attempt return after 30*g seconds; no rewind of health or inventory.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs durable delayed-return identity and cancellation semantics; not time reversal of health or inventory.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174734-174736`, `codedump(1).c:93743-93746`, `codedump(1).c:221314-221319`, `codedump(1).c:93748-93762`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Threshold Alarm

Key: `arm.spell.threshold_alarm`. Native school: **Earth**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Alarm`, skill ID 70, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1223`; `historical_not_live_parity`.

**Observable effect:** Notify the configured owner of eligible entry through a room alarm.

**Scaling:** Monitor one cell for 120*g seconds; at most one notice per eligible entry event. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.threshold_alarm.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use the native alarm flag and existing notification rules; source immunity filters require policy.

**Runtime evidence:**
- `roomflag`: [MudSharpCore/Magic/SpellEffects/RoomFlagEffect.cs:138](../../MudSharpCore/Magic/SpellEffects/RoomFlagEffect.cs#L138), `RoomFlagEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-70 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Threshold Alarm, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Threshold Alarm at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Notify the configured owner of eligible entry through a room alarm.
- At grade 2 apply only this scaling contract: Monitor one cell for 120*g seconds; at most one notice per eligible entry event.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use the native alarm flag and existing notification rules; source immunity filters require policy.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Revealing; targets no explicit target; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174754-174756`, `codedump(1).c:216742-216742`, `codedump(1).c:216745-216763`, `codedump(1).c:81783-81930`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Fleet Step

Key: `arm.spell.fleet_step`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Godspeed`, skill ID 90, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1243`; `historical_not_live_parity`.

**Observable effect:** Increase movement speed and reduce locomotion effort for a duration.

**Scaling:** Movement delay multiplier 1/(1+0.05*g), agility bonus g; duration 60*g seconds. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.fleet_step.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Trait bonuses and stamina multipliers alone do not establish movement-delay modification.

**Runtime evidence:**
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `staminaexpendrate`: [MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs#L88), `StaminaExpenditureSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-90 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Fleet Step, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Fleet Step at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Increase movement speed and reduce locomotion effort for a duration.
- At grade 2 apply only this scaling contract: Movement delay multiplier 1/(1+0.05*g), agility bonus g; duration 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Trait bonuses and stamina multipliers alone do not establish movement-delay modification.

Historical metadata (not proposed policy): element Stone; sphere Alteration; mood Beneficial; targets character in room, self only; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174826-174828`, `codedump(1).c:87835-87841`, `codedump(1).c:219605-219650`, `codedump(1).c:87843-87961`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sand Shelter

Key: `arm.spell.sand_shelter`. Native school: **Earth**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Sand Shelter`, skill ID 249, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1263`; `historical_not_live_parity`.

**Observable effect:** Create a temporary physically usable shelter with occupants safely released on expiry.

**Scaling:** One shelter for 300*g seconds, capacity g; release occupants safely on collapse. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sand_shelter.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires shelter lifecycle/occupancy proof; a decorative object is an unapproved substitute.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-249 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sand Shelter, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Sand Shelter at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Earth.
- Create a temporary physically usable shelter with occupants safely released on expiry.
- At grade 2 apply only this scaling contract: One shelter for 300*g seconds, capacity g; release occupants safely on collapse.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires shelter lifecycle/occupancy proof; a decorative object is an unapproved substitute.

Historical metadata (not proposed policy): element Earth; sphere Conjuration; mood Protective; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175325-175327`, `codedump(1).c:93994-93999`, `codedump(1).c:221473-221493`, `codedump(1).c:94001-94140`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Clay Sentinel

Key: `arm.spell.clay_sentinel`. Native school: **Earth**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Golem`, skill ID 319, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1283`; `historical_not_live_parity`.

**Observable effect:** Animate a controlled temporary clay guardian.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.clay_sentinel.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs creator command eligibility and teardown; historical golem summary is insufficient to infer those details.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-319 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Clay Sentinel, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Clay Sentinel at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Earth.
- Animate a controlled temporary clay guardian.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs creator command eligibility and teardown; historical golem summary is insufficient to infer those details.

Historical metadata (not proposed policy): element Stone; sphere Conjuration; mood Dominant; targets no explicit target; minimum position Fighting; minimum mana 50; base power 10; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175536-175538`, `codedump(1).c:87965-87965`, `codedump(1).c:219654-219673`, `codedump(1).c:87967-88143`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sand Effigy

Key: `arm.spell.sand_effigy`. Native school: **Earth**. Target: `item`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Sand Statue`, skill ID 411, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1303`; `historical_not_live_parity`.

**Observable effect:** Make an effigy that temporarily projects an immobile copy with shared identity restrictions.

**Scaling:** One effigy and at most one linked immobile copy for 60*g seconds. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sand_effigy.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No proven figurine/copy linkage; do not substitute an independent clone silently.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `createclone`: [MudSharpCore/Magic/SpellEffects/CopyCloneSpellEffects.cs:749](../../MudSharpCore/Magic/SpellEffects/CopyCloneSpellEffects.cs#L749), `CloneSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-411 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Sand Effigy, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Sand Effigy at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Earth.
- Make an effigy that temporarily projects an immobile copy with shared identity restrictions.
- At grade 2 apply only this scaling contract: One effigy and at most one linked immobile copy for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No proven figurine/copy linkage; do not substitute an independent clone silently.

Historical metadata (not proposed policy): element Stone; sphere Creation; mood Passive; targets character in room, object in inventory; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175806-175807`, `codedump(1).c:94143-94151`, `codedump(1).c:221496-221533`, `codedump(1).c:94153-94316`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shatter Stone

Key: `arm.spell.shatter_stone`. Native school: **Earth**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Shatter`, skill ID 447, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1323`; `historical_not_live_parity`.

**Observable effect:** Damage a stone-tagged item with a finite damage budget.

**Scaling:** Apply at most 5*g native item damage units to eligible material; do not damage contents. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shatter_stone.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No automatic holder damage, blast radius or arbitrary item deletion.

**Runtime evidence:**
- `itemdamage`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:345](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L345), `ItemDamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-447 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Shatter Stone, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Shatter Stone at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Damage a stone-tagged item with a finite damage budget.
- At grade 2 apply only this scaling contract: Apply at most 5*g native item damage units to eligible material; do not damage contents.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No automatic holder damage, blast radius or arbitrary item deletion.

Historical metadata (not proposed policy): element Stone; sphere Enchantment; mood Harmful; targets object in inventory, object in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175904-175906`, `codedump(1).c:222559-222595`, `codedump(1).c:222597-222629`, `codedump(1).c:94474-94524`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Restore Object

Key: `arm.spell.restore_object`. Native school: **Earth**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Repair Item`, skill ID 455, coded family Earth; `armageddon_magic_psionics_reference_second_pass.md:1342`; `historical_not_live_parity`.

**Observable effect:** Repair eligible item damage without duplicating material or resetting quality.

**Scaling:** Repair at most 5*g native item damage units; no quality increase, duplicate contents or free material. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.restore_object.skill`; shared option `arm.skill.earth_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** MendEffect treats wounds; an item-repair adapter is still required.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-455 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Restore Object, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Restore Object at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Earth.
- Repair eligible item damage without duplicating material or resetting quality.
- At grade 2 apply only this scaling contract: Repair at most 5*g native item damage units; no quality increase, duplicate contents or free material.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: MendEffect treats wounds; an item-repair adapter is still required.

Historical metadata (not proposed policy): element Earth; sphere Alteration; mood Dominant; targets object in inventory, object in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175933-175935`, `codedump(1).c:93418-93423`, `codedump(1).c:221203-221245`, `codedump(1).c:93425-93530`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Waystep

Key: `arm.spell.waystep`. Native school: **Wind**. Target: `room`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Teleport`, skill ID 2, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1366`; `historical_not_live_parity`.

**Observable effect:** Move the caster to an explicitly resolved permitted cell.

**Scaling:** Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.waystep.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.waystep.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Destination policy is authored; use actual teleport, not a random unbounded destination search.

**Runtime evidence:**
- `teleport`: [MudSharpCore/Magic/SpellEffects/TeleportEffect.cs:78](../../MudSharpCore/Magic/SpellEffects/TeleportEffect.cs#L78), `TeleportEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-2 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Waystep, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Waystep at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Wind.
- Move the caster to an explicitly resolved permitted cell.
- At grade 2 apply only this scaling contract: Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Destination policy is authored; use actual teleport, not a random unbounded destination search.

Historical metadata (not proposed policy): element Wind/Air; sphere Teleportation; mood Neutral; targets object in world, character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174481-174483`, `codedump(1).c:96369-96375`, `codedump(1).c:222231-222279`, `codedump(1).c:96377-96515`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Pierce Concealment

Key: `arm.spell.pierce_concealment`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Detect Invisible`, skill ID 7, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1386`; `historical_not_live_parity`.

**Observable effect:** Let the recipient perceive eligible invisible entities.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.pierce_concealment.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.pierce_concealment.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native perception restrictions still apply; detection is explicitly registered in status templates.

**Runtime evidence:**
- `detectinvisible`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:609](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L609), `DetectInvisibleEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-7 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Pierce Concealment, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Pierce Concealment at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Wind.
- Let the recipient perceive eligible invisible entities.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native perception restrictions still apply; detection is explicitly registered in status templates.

Historical metadata (not proposed policy): element Wind/Air; sphere Divination; mood Revealing; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174501-174503`, `codedump(1).c:84574-84576`, `codedump(1).c:217927-217967`, `codedump(1).c:84578-84608`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Veil from Sight

Key: `arm.spell.veil_from_sight`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Invisibility`, skill ID 13, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1406`; `historical_not_live_parity`.

**Observable effect:** Apply native invisibility to the recipient.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.veil_from_sight.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.veil_from_sight.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No universal silence, scent masking or undetectability.

**Runtime evidence:**
- `invisibility`: [MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs:161](../../MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs#L161), `InvisibilityEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-13 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Veil from Sight, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Veil from Sight at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Wind.
- Apply native invisibility to the recipient.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No universal silence, scent masking or undetectability.

Historical metadata (not proposed policy): element Wind/Air; sphere Illusion; mood Protective; targets character in room, object in inventory, object in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174525-174527`, `codedump(1).c:89802-89806`, `codedump(1).c:220153-220194`, `codedump(1).c:89808-89890`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Draw Traveller

Key: `arm.spell.draw_traveller`. Native school: **Wind**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Summon`, skill ID 20, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1426`; `historical_not_live_parity`.

**Observable effect:** Move a permitted remote character to the caster's current cell.

**Scaling:** Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.draw_traveller.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.draw_traveller.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Conjuration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires explicit destination parameter, consent/resistance and remote-target policy.

**Runtime evidence:**
- `teleporttarget`: [MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs:75](../../MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs#L75), `TeleportTargetEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-20 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Draw Traveller, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Draw Traveller at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Wind.
- Move a permitted remote character to the caster's current cell.
- At grade 2 apply only this scaling contract: Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires explicit destination parameter, consent/resistance and remote-target policy.

Historical metadata (not proposed policy): element Wind/Air; sphere Conjuration; mood Beneficial; targets character in world; minimum position Fighting; minimum mana 50; base power 15; components Conjuration.

Historical dump offsets claimed by the reference: `codedump(1).c:174553-174555`, `codedump(1).c:96227-96233`, `codedump(1).c:222208-222227`, `codedump(1).c:96235-96366`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Join Traveller

Key: `arm.spell.join_traveller`. Native school: **Wind**. Target: `room`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Relocate`, skill ID 23, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1446`; `historical_not_live_parity`.

**Observable effect:** Move the caster to a permitted cell resolved from another character.

**Scaling:** Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.join_traveller.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.join_traveller.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Teleportation.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source Relocate is spatial; current relocate token treats dislocated wounds and must not be used.

**Runtime evidence:**
- `teleport`: [MudSharpCore/Magic/SpellEffects/TeleportEffect.cs:78](../../MudSharpCore/Magic/SpellEffects/TeleportEffect.cs#L78), `TeleportEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-23 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Join Traveller, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Join Traveller at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Wind.
- Move the caster to a permitted cell resolved from another character.
- At grade 2 apply only this scaling contract: Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source Relocate is spatial; current relocate token treats dislocated wounds and must not be used.

Historical metadata (not proposed policy): element Wind/Air; sphere Teleportation; mood Beneficial; targets character in world; minimum position Standing; minimum mana 0; base power 15; components Teleportation.

Historical dump offsets claimed by the reference: `codedump(1).c:174565-174567`, `codedump(1).c:93270-93277`, `codedump(1).c:221137-221157`, `codedump(1).c:93279-93374`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Buoyant Lift

Key: `arm.spell.buoyant_lift`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Levitate`, skill ID 26, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1466`; `historical_not_live_parity`.

**Observable effect:** Apply native persistent levitation.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.buoyant_lift.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.buoyant_lift.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Follow actual movement/weight admission rather than promising unrestricted flight.

**Runtime evidence:**
- `levitate`: [MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs:95](../../MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs#L95), `LevitationEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-26 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Buoyant Lift, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Buoyant Lift at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Wind.
- Apply native persistent levitation.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Follow actual movement/weight admission rather than promising unrestricted flight.

Historical metadata (not proposed policy): element Wind/Air; sphere Alteration; mood Beneficial; targets character in room, object in room, object in inventory; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174577-174579`, `codedump(1).c:89946-89950`, `codedump(1).c:220242-220283`, `codedump(1).c:89952-90087`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Scouring Gust

Key: `arm.spell.scouring_gust`. Native school: **Wind**. Target: `characters`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Sandstorm`, skill ID 41, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1486`; `historical_not_live_parity`.

**Observable effect:** Inflict wind-themed injury and stamina loss on eligible local targets.

**Scaling:** Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.scouring_gust.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** An instantaneous group spell, not a persistent sandstorm simulator.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `staminadelta`: [MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs:94](../../MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs#L94), `StaminaDeltaSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-41 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Scouring Gust, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Scouring Gust at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Wind.
- Inflict wind-themed injury and stamina loss on eligible local targets.
- At grade 2 apply only this scaling contract: Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: An instantaneous group spell, not a persistent sandstorm simulator.

Historical metadata (not proposed policy): element Wind/Air; sphere Nature; mood Destructive; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174637-174639`, `codedump(1).c:94318-94322`, `codedump(1).c:221536-221573`, `codedump(1).c:94324-94372`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Gust Hands

Key: `arm.spell.gust_hands`. Native school: **Wind**. Target: `character_and_exit`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Hands Of Wind`, skill ID 42, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1506`; `historical_not_live_parity`.

**Observable effect:** Move the target along an explicitly selected traversable path.

**Scaling:** At most g legal path steps for Gust Hands; Driving Gust remains one step at every grade. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.gust_hands.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native stepwise movement, anchors and exits remain authoritative.

**Runtime evidence:**
- `handsofwind`: [MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs:554](../../MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs#L554), `ForcedPathMovementEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-42 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Gust Hands, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character_and_exit. Invoke Gust Hands at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Wind.
- Move the target along an explicitly selected traversable path.
- At grade 2 apply only this scaling contract: At most g legal path steps for Gust Hands; Driving Gust remains one step at every grade.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native stepwise movement, anchors and exits remain authoritative.

Historical metadata (not proposed policy): element Wind/Air; sphere Nature; mood Aggressive; targets character in world, cannot target self; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174641-174643`, `codedump(1).c:88533-88537`, `codedump(1).c:219701-219727`, `codedump(1).c:88539-88800`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Banish Traveller

Key: `arm.spell.banish_traveller`. Native school: **Wind**. Target: `character_and_room`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Banishment`, skill ID 45, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1526`; `historical_not_live_parity`.

**Observable effect:** Send a target to a configured permitted destination after resistance.

**Scaling:** Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.banish_traveller.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No implied homeland or elemental-plane law; destination is explicit policy.

**Runtime evidence:**
- `teleporttarget`: [MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs:75](../../MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs#L75), `TeleportTargetEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-45 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Banish Traveller, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character_and_room. Invoke Banish Traveller at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Wind.
- Send a target to a configured permitted destination after resistance.
- At grade 2 apply only this scaling contract: Move one authorised entity to one explicit permitted cell; grade changes price/difficulty, not arbitrary reach.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No implied homeland or elemental-plane law; destination is explicit policy.

Historical metadata (not proposed policy): element Wind/Air; sphere Teleportation; mood Harmful; targets object in world, character in room, cannot target self; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174653-174655`, `codedump(1).c:82613-82618`, `codedump(1).c:216942-216986`, `codedump(1).c:82620-82817`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Air Guardian

Key: `arm.spell.air_guardian`. Native school: **Wind**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Guardian`, skill ID 50, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1546`; `historical_not_live_parity`.

**Observable effect:** Create a temporary protective air creature.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.air_guardian.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Conjuration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** NPC creation does not establish guarding commands, summon ownership or safe expiry.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-50 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Air Guardian, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Air Guardian at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Wind.
- Create a temporary protective air creature.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: NPC creation does not establish guarding commands, summon ownership or safe expiry.

Historical metadata (not proposed policy): element Wind/Air; sphere Conjuration; mood Aggressive; targets no explicit target; minimum position Fighting; minimum mana 33; base power 15; components Conjuration.

Historical dump offsets claimed by the reference: `codedump(1).c:174673-174675`, `codedump(1).c:88146-88151`, `codedump(1).c:219677-219697`, `codedump(1).c:88153-88360`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Exchange Places

Key: `arm.spell.exchange_places`. Native school: **Wind**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Transference`, skill ID 52, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1566`; `historical_not_live_parity`.

**Observable effect:** Swap caster and target spatial locations when native validation allows.

**Scaling:** Swap one caster/target pair once; same position is an ineligible no-op. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.waystep`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.exchange_places.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Alteration | Teleportation.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Same location/layer is a no-op and must not earn mastery.

**Runtime evidence:**
- `transference`: [MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs:767](../../MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs#L767), `TransferenceEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-52 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Exchange Places, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Exchange Places at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Wind.
- Swap caster and target spatial locations when native validation allows.
- At grade 2 apply only this scaling contract: Swap one caster/target pair once; same position is an ineligible no-op.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Same location/layer is a no-op and must not earn mastery.

Historical metadata (not proposed policy): element Wind/Air; sphere Teleportation; mood Passive; targets character in world; minimum position Standing; minimum mana 33; base power 15; components Alteration | Teleportation.

Historical dump offsets claimed by the reference: `codedump(1).c:174681-174684`, `codedump(1).c:96619-96628`, `codedump(1).c:222355-222375`, `codedump(1).c:96630-96778`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Windborne Flight

Key: `arm.spell.windborne_flight`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Fly`, skill ID 60, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1586`; `historical_not_live_parity`.

**Observable effect:** Grant the native magical flight status.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.buoyant_lift`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.windborne_flight.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.buoyant_lift`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.windborne_flight.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Body/plane/movement restrictions remain; it is not the same effect as levitation.

**Runtime evidence:**
- `flying`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:447](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L447), `FlyingEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-60 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Windborne Flight, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Windborne Flight at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Wind.
- Grant the native magical flight status.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Body/plane/movement restrictions remain; it is not the same effect as levitation.

Historical metadata (not proposed policy): element Wind/Air; sphere Invocation; mood Dominant; targets character in room, self only; minimum position Standing; minimum mana 25; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174714-174716`, `codedump(1).c:87323-87325`, `codedump(1).c:219257-219291`, `codedump(1).c:87327-87399`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Seeking Wind

Key: `arm.spell.seeking_wind`. Native school: **Wind**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Stalker`, skill ID 61, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1606`; `historical_not_live_parity`.

**Observable effect:** Summon a temporary tracker with a specifically authorised quarry.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.seeking_wind.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Conjuration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Tracking AI and target authority/lifecycle are required; creating an NPC is not sufficient.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-61 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Seeking Wind, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Seeking Wind at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Wind.
- Summon a temporary tracker with a specifically authorised quarry.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Tracking AI and target authority/lifecycle are required; creating an NPC is not sufficient.

Historical metadata (not proposed policy): element Wind/Air; sphere Conjuration; mood Destructive; targets no explicit target; minimum position Standing; minimum mana 20; base power 15; components Conjuration.

Historical dump offsets claimed by the reference: `codedump(1).c:174718-174720`, `codedump(1).c:95953-95957`, `codedump(1).c:222059-222079`, `codedump(1).c:95959-96018`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Gentle Descent

Key: `arm.spell.gentle_descent`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Feather Fall`, skill ID 71, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1626`; `historical_not_live_parity`.

**Observable effect:** Reduce falling harm through the native featherfall effect.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.buoyant_lift`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.gentle_descent.skill`; shared option `arm.skill.wind_casting`.
- Sorcerer: acquired `arm.spell.buoyant_lift`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.gentle_descent.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Do not grant general flight or ground movement speed.

**Runtime evidence:**
- `featherfall`: [MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs:288](../../MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs#L288), `FeatherFallEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-71 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Gentle Descent, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Gentle Descent at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Wind.
- Reduce falling harm through the native featherfall effect.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Do not grant general flight or ground movement speed.

Historical metadata (not proposed policy): element Wind/Air; sphere Alteration; mood Protective; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174758-174760`, `codedump(1).c:86641-86645`, `codedump(1).c:218861-218907`, `codedump(1).c:86647-86693`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Driving Gust

Key: `arm.spell.driving_gust`. Native school: **Wind**. Target: `character_and_exit`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Repel`, skill ID 84, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1646`; `historical_not_live_parity`.

**Observable effect:** Force a target one eligible step through a chosen exit.

**Scaling:** At most g legal path steps for Gust Hands; Driving Gust remains one step at every grade. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.driving_gust.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source distance/knockback is adapted; normal movement and anchored-body checks remain.

**Runtime evidence:**
- `forcedexitmovement`: [MudSharpCore/Magic/SpellEffects/ForcedExitMovementEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ForcedExitMovementEffect.cs#L55), `ForcedExitMovementEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-84 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Driving Gust, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character_and_exit. Invoke Driving Gust at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Wind.
- Force a target one eligible step through a chosen exit.
- At grade 2 apply only this scaling contract: At most g legal path steps for Gust Hands; Driving Gust remains one step at every grade.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source distance/knockback is adapted; normal movement and anchored-body checks remain.

Historical metadata (not proposed policy): element Wind/Air; sphere Teleportation; mood Dominant; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174804-174806`, `codedump(1).c:93533-93537`, `codedump(1).c:221249-221284`, `codedump(1).c:93539-93655`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wind Barrier

Key: `arm.spell.wind_barrier`. Native school: **Wind**. Target: `exit`. Lifecycle: `timed`.

**Provenance:** Historical `Wall Of Wind`, skill ID 347, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1666`; `historical_not_live_parity`.

**Observable effect:** Block passage through one exit with a wind-themed barrier.

**Scaling:** Block the selected exit for 60*g seconds; no traversal damage. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wind_barrier.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No unimplemented projectile deflection or crossing damage is implied.

**Runtime evidence:**
- `exitbarrier`: [MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs#L55), `ExitBarrierEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-347 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Wind Barrier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible exit. Invoke Wind Barrier at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Wind.
- Block passage through one exit with a wind-themed barrier.
- At grade 2 apply only this scaling contract: Block the selected exit for 60*g seconds; no traversal damage.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No unimplemented projectile deflection or crossing damage is implied.

Historical metadata (not proposed policy): element Wind/Air; sphere Alteration; mood Neutral; targets no explicit target; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175624-175626`, `codedump(1).c:222940-223038`, `codedump(1).c:97250-97254`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wind Mantle

Key: `arm.spell.wind_mantle`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Wind Armor`, skill ID 350, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1685`; `historical_not_live_parity`.

**Observable effect:** Apply a finite wind-themed armour layer.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wind_mantle.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native armour mitigation, not a separate evasive combat system.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-350 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Wind Mantle, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Wind Mantle at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Wind.
- Apply a finite wind-themed armour layer.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native armour mitigation, not a separate evasive combat system.

Historical metadata (not proposed policy): element Wind/Air; sphere Abjuration; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175636-175638`, `codedump(1).c:97314-97318`, `codedump(1).c:223093-223128`, `codedump(1).c:97320-97438`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wind Hammer

Key: `arm.spell.wind_hammer`. Native school: **Wind**. Target: `character_and_exit`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Wind Fist`, skill ID 373, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1705`; `historical_not_live_parity`.

**Observable effect:** Strike and then move a target through a selected exit only if the hit resolves.

**Scaling:** Damage 5*g units; one legal exit movement only after resolved harm/admission. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wind_hammer.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires ordered linked outcome handling; two independent effects could push a rejected target.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `forcedexitmovement`: [MudSharpCore/Magic/SpellEffects/ForcedExitMovementEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ForcedExitMovementEffect.cs#L55), `ForcedExitMovementEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-373 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Wind Hammer, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character_and_exit. Invoke Wind Hammer at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Wind.
- Strike and then move a target through a selected exit only if the hit resolves.
- At grade 2 apply only this scaling contract: Damage 5*g units; one legal exit movement only after resolved harm/admission.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires ordered linked outcome handling; two independent effects could push a rejected target.

Historical metadata (not proposed policy): element Wind/Air; sphere Invocation; mood Harmful; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175709-175711`, `codedump(1).c:97441-97443`, `codedump(1).c:223132-223168`, `codedump(1).c:97445-97488`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### False Presence

Key: `arm.spell.false_presence`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Delusion`, skill ID 380, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1725`; `historical_not_live_parity`.

**Observable effect:** Hide the target's visible presence for a duration.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.false_presence.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source flag supports invisibility; no general mind-deception or fake physical actor is claimed.

**Runtime evidence:**
- `invisibility`: [MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs:161](../../MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs#L161), `InvisibilityEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-380 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired False Presence, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke False Presence at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Wind.
- Hide the target's visible presence for a duration.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source flag supports invisibility; no general mind-deception or fake physical actor is claimed.

Historical metadata (not proposed policy): element Wind/Air; sphere Illusion; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175733-175735`, `codedump(1).c:84169-84175`, `codedump(1).c:217817-217835`, `codedump(1).c:84177-84455`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wind Shield

Key: `arm.spell.wind_shield`. Native school: **Wind**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Shield Of Wind`, skill ID 426, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1745`; `historical_not_live_parity`.

**Observable effect:** Protect a character with a wind-themed armour envelope.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wind_shield.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed character effect replaces source conjured shield item pending approval.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-426 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Wind Shield, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Wind Shield at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Wind.
- Protect a character with a wind-themed armour envelope.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed character effect replaces source conjured shield item pending approval.

Historical metadata (not proposed policy): element Wind/Air; sphere Conjuration; mood Protective; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175847-175848`, `codedump(1).c:95264-95266`, `codedump(1).c:221706-221741`, `codedump(1).c:95268-95354`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wind Courier

Key: `arm.spell.wind_courier`. Native school: **Wind**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Messenger`, skill ID 448, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1765`; `historical_not_live_parity`.

**Observable effect:** Create a temporary courier that delivers a bounded message to an authorised recipient.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wind_courier.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Conjuration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs delivery/recipient permissions and summon expiry; no arbitrary command proxy.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-448 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Wind Courier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Wind Courier at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Wind.
- Create a temporary courier that delivers a bounded message to an authorised recipient.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs delivery/recipient permissions and summon expiry; no arbitrary command proxy.

Historical metadata (not proposed policy): element Wind/Air; sphere Conjuration; mood Neutral; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components Conjuration.

Historical dump offsets claimed by the reference: `codedump(1).c:175908-175910`, `codedump(1).c:90503-90507`, `codedump(1).c:220486-220506`, `codedump(1).c:90509-90562`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Reveal the Hidden

Key: `arm.spell.reveal_the_hidden`. Native school: **Wind**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Dispel Invisibility`, skill ID 453, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1785`; `historical_not_live_parity`.

**Observable effect:** Remove eligible spell invisibility from the target.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.reveal_the_hidden.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not remove unrelated mundane hiding or every invisibility power.

**Runtime evidence:**
- `dispelinvisibility`: [MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs:469](../../MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs#L469), `RemoveInvisibilityEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-453 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Reveal the Hidden, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Reveal the Hidden at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Wind.
- Remove eligible spell invisibility from the target.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not remove unrelated mundane hiding or every invisibility power.

Historical metadata (not proposed policy): element Wind/Air; sphere Enchantment; mood Destructive; targets character in room, object in inventory, object in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175925-175927`, `codedump(1).c:223224-223264`, `codedump(1).c:97953-97996`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Anchor Rune

Key: `arm.spell.anchor_rune`. Native school: **Wind**. Target: `item`. Lifecycle: `timed`.

**Provenance:** Historical `Create Rune`, skill ID 456, coded family Wind; `armageddon_magic_psionics_reference_second_pass.md:1804`; `historical_not_live_parity`.

**Observable effect:** Mark an owned item as an explicitly registered travel anchor.

**Scaling:** One bounded metadata mark/anchor for 300*g seconds; no automatic network membership. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Wind: acquired `arm.spell.pierce_concealment`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.anchor_rune.skill`; shared option `arm.skill.wind_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Topology ownership must be configured; a magic tag alone is not a usable portal endpoint.

**Runtime evidence:**
- `magictag`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:91](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L91), `MagicTagEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `portalnetwork`: [MudSharpCore/Magic/SpellEffects/PortalTopologySpellEffect.cs:82](../../MudSharpCore/Magic/SpellEffects/PortalTopologySpellEffect.cs#L82), `PortalTopologySpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-456 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Wind admission, acquired Anchor Rune, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Anchor Rune at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Wind.
- Mark an owned item as an explicitly registered travel anchor.
- At grade 2 apply only this scaling contract: One bounded metadata mark/anchor for 300*g seconds; no automatic network membership.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Topology ownership must be configured; a magic tag alone is not a usable portal endpoint.

Historical metadata (not proposed policy): element Wind/Air; sphere Creation; mood Neutral; targets no explicit target; minimum position Standing; minimum mana 50; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175937-175939`, `codedump(1).c:83624-83629`, `codedump(1).c:217383-217478`, `codedump(1).c:83631-83635`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shroud Eyes

Key: `arm.spell.shroud_eyes`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Blind`, skill ID 3, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1828`; `historical_not_live_parity`.

**Observable effect:** Apply native magical blindness.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shroud_eyes.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.shroud_eyes.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No damage to physical eyes or permanent loss of sight.

**Runtime evidence:**
- `blindness`: [MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs:72](../../MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs#L72), `BlindnessEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-3 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Shroud Eyes, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Shroud Eyes at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Shadow.
- Apply native magical blindness.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No damage to physical eyes or permanent loss of sight.

Historical metadata (not proposed policy): element Shadow; sphere Alteration; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174485-174487`, `codedump(1).c:82820-82824`, `codedump(1).c:217094-217135`, `codedump(1).c:82826-82924`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Clear Eyes

Key: `arm.spell.clear_eyes`. Native school: **Shadow**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Cure Blindness`, skill ID 6, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1848`; `historical_not_live_parity`.

**Observable effect:** Remove spell-induced blindness through the native removal effect.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.clear_eyes.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.clear_eyes.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not heal missing/damaged eyes or every medical cause.

**Runtime evidence:**
- `removeblindness`: [MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs:148](../../MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs#L148), `RemoveBlindnessEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-6 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Clear Eyes, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Clear Eyes at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Shadow.
- Remove spell-induced blindness through the native removal effect.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not heal missing/damaged eyes or every medical cause.

Historical metadata (not proposed policy): element Shadow; sphere Clerical; mood Beneficial; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174497-174499`, `codedump(1).c:83778-83781`, `codedump(1).c:217587-217620`, `codedump(1).c:83783-83812`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Lift Hex

Key: `arm.spell.lift_hex`. Native school: **Shadow**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Remove Curse`, skill ID 16, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1868`; `historical_not_live_parity`.

**Observable effect:** Remove the eligible magical curse status.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.lift_hex.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.lift_hex.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No arbitrary removal of every negative effect.

**Runtime evidence:**
- `removecurse`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:583](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L583), `RemoveCurseEffect.RemoveEffects` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-16 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Lift Hex, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Lift Hex at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Remove the eligible magical curse status.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No arbitrary removal of every negative effect.

Historical metadata (not proposed policy): element Shadow; sphere Invocation; mood Beneficial; targets character in room, object in inventory, object in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174537-174539`, `codedump(1).c:93377-93381`, `codedump(1).c:221161-221199`, `codedump(1).c:93383-93415`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Dread Presence

Key: `arm.spell.dread_presence`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Fear`, skill ID 24, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1888`; `historical_not_live_parity`.

**Observable effect:** Apply native magical fear to a susceptible target.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.dread_presence.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.dread_presence.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Configured fear behavior, not unrestricted forced commands.

**Runtime evidence:**
- `fear`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:339](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L339), `FearEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-24 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Dread Presence, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Dread Presence at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Apply native magical fear to a susceptible target.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Configured fear behavior, not unrestricted forced commands.

Historical metadata (not proposed policy): element Shadow; sphere Illusion; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174569-174571`, `codedump(1).c:86507-86510`, `codedump(1).c:218822-218857`, `codedump(1).c:86512-86638`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Night Eyes

Key: `arm.spell.night_eyes`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Infravision`, skill ID 31, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1908`; `historical_not_live_parity`.

**Observable effect:** Grant native infravision for a duration.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.night_eyes.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.night_eyes.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Not omniscience or generic perception across planes.

**Runtime evidence:**
- `infravision`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:771](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L771), `InfravisionEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-31 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Night Eyes, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Night Eyes at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Shadow.
- Grant native infravision for a duration.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Not omniscience or generic perception across planes.

Historical metadata (not proposed policy): element Shadow; sphere Alteration; mood Beneficial; targets character in room, self only; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174597-174599`, `codedump(1).c:89556-89560`, `codedump(1).c:220015-220058`, `codedump(1).c:89562-89655`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Walking Shadow

Key: `arm.spell.walking_shadow`. Native school: **Shadow**. Target: `self`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Send Shadow`, skill ID 38, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1928`; `historical_not_live_parity`.

**Observable effect:** Send a controllable shadow while the original body remains subject to its own restrictions.

**Scaling:** At most one new controlled instance from this invocation for 60*g seconds; native anchor/focus rules apply. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.walking_shadow.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native instances are foundations; shadow identity, recognition and liability rules need an explicit supporting contract.

**Runtime evidence:**
- `astralprojection`: [MudSharpCore/Magic/SpellEffects/AstralProjectionSpellEffect.cs:165](../../MudSharpCore/Magic/SpellEffects/AstralProjectionSpellEffect.cs#L165), `AstralProjectionSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `createcopy`: [MudSharpCore/Magic/SpellEffects/CopyCloneSpellEffects.cs:143](../../MudSharpCore/Magic/SpellEffects/CopyCloneSpellEffects.cs#L143), `CopySpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-38 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Walking Shadow, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible self. Invoke Walking Shadow at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Shadow.
- Send a controllable shadow while the original body remains subject to its own restrictions.
- At grade 2 apply only this scaling contract: At most one new controlled instance from this invocation for 60*g seconds; native anchor/focus rules apply.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native instances are foundations; shadow identity, recognition and liability rules need an explicit supporting contract.

Historical metadata (not proposed policy): element Shadow; sphere Illusion; mood Dominant; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174625-174627`, `codedump(1).c:94528-94534`, `codedump(1).c:221577-221605`, `codedump(1).c:94536-94753`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shadow Passage

Key: `arm.spell.shadow_passage`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Ethereal`, skill ID 43, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1948`; `historical_not_live_parity`.

**Observable effect:** Apply an approved ethereal planar-presence overlay.

**Scaling:** One configured planar overlay for 60*g seconds; grade does not invent planes or permission. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.see_the_unbodied`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shadow_passage.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.see_the_unbodied`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.shadow_passage.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Alteration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native corporeality and carried-item interaction replace historical item flag/drop rules.

**Runtime evidence:**
- `planarstate`: [MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs:110](../../MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs#L110), `PlanarStateSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-43 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Shadow Passage, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Shadow Passage at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Shadow.
- Apply an approved ethereal planar-presence overlay.
- At grade 2 apply only this scaling contract: One configured planar overlay for 60*g seconds; grade does not invent planes or permission.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native corporeality and carried-item interaction replace historical item flag/drop rules.

Historical metadata (not proposed policy): element Shadow; sphere Alteration; mood Passive; targets character in room, object in inventory, self only; minimum position Fighting; minimum mana 20; base power 15; components Alteration.

Historical dump offsets claimed by the reference: `codedump(1).c:174645-174647`, `codedump(1).c:86327-86335`, `codedump(1).c:218778-218818`, `codedump(1).c:86337-86504`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### See the Unbodied

Key: `arm.spell.see_the_unbodied`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Detect Ethereal`, skill ID 44, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1968`; `historical_not_live_parity`.

**Observable effect:** Expose eligible ethereal entities to the recipient's perception.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.see_the_unbodied.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.see_the_unbodied.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Divination.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Respects native planar perception restrictions.

**Runtime evidence:**
- `detectethereal`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:663](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L663), `DetectEtherealEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-44 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired See the Unbodied, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke See the Unbodied at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Shadow.
- Expose eligible ethereal entities to the recipient's perception.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Respects native planar perception restrictions.

Historical metadata (not proposed policy): element Shadow; sphere Divination; mood Revealing; targets character in room, self only; minimum position Standing; minimum mana 0; base power 15; components Divination.

Historical dump offsets claimed by the reference: `codedump(1).c:174649-174651`, `codedump(1).c:84525-84529`, `codedump(1).c:217883-217923`, `codedump(1).c:84531-84571`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Cooling Shade

Key: `arm.spell.cooling_shade`. Native school: **Shadow**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Restful Shade`, skill ID 59, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:1988`; `historical_not_live_parity`.

**Observable effect:** Reduce local light and temperature for a bounded duration.

**Scaling:** Reduce light by 10*g lux and temperature by 0.5*g Celsius for 60*g seconds. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.cooling_shade.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed environmental relief; no magical hydration or automatic resting skill gain.

**Runtime evidence:**
- `roomtemperature`: [MudSharpCore/Magic/SpellEffects/RoomTemperatureEffect.cs:191](../../MudSharpCore/Magic/SpellEffects/RoomTemperatureEffect.cs#L191), `RoomTemperatureEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-59 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Cooling Shade, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Cooling Shade at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Reduce local light and temperature for a bounded duration.
- At grade 2 apply only this scaling contract: Reduce light by 10*g lux and temperature by 0.5*g Celsius for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed environmental relief; no magical hydration or automatic resting skill gain.

Historical metadata (not proposed policy): element Shadow; sphere Nature; mood Beneficial; targets no explicit target; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174710-174712`, `codedump(1).c:93658-93662`, `codedump(1).c:221288-221310`, `codedump(1).c:93664-93740`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Deep Darkness

Key: `arm.spell.deep_darkness`. Native school: **Shadow**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Create Darkness`, skill ID 170, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2008`; `historical_not_live_parity`.

**Observable effect:** Reduce room illumination through the native light effect.

**Scaling:** Reduce room illumination by 20*g lux for 60*g seconds, respecting native light bounds. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.deep_darkness.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.deep_darkness.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not suppress all nonvisual senses or magical detection.

**Runtime evidence:**
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-170 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Deep Darkness, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Deep Darkness at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Reduce room illumination through the native light effect.
- At grade 2 apply only this scaling contract: Reduce room illumination by 20*g lux for 60*g seconds, respecting native light bounds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not suppress all nonvisual senses or magical detection.

Historical metadata (not proposed policy): element Shadow; sphere Creation; mood Destructive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175069-175071`, `codedump(1).c:83437-83440`, `codedump(1).c:217319-217340`, `codedump(1).c:83442-83513`, `codedump(1).c:82826-82924`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Clinging Hex

Key: `arm.spell.clinging_hex`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Curse`, skill ID 176, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2029`; `historical_not_live_parity`.

**Observable effect:** Apply the configured native curse modifier.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.dread_presence`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.clinging_hex.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.dread_presence`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.clinging_hex.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No permanent mutation of base defensive skills.

**Runtime evidence:**
- `curse`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:555](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L555), `CurseEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-176 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Clinging Hex, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Clinging Hex at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Apply the configured native curse modifier.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No permanent mutation of base defensive skills.

Historical metadata (not proposed policy): element Shadow; sphere Enchantment; mood Dominant; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175092-175094`, `codedump(1).c:83943-83946`, `codedump(1).c:217674-217714`, `codedump(1).c:83948-84023`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Haunting Shape

Key: `arm.spell.haunting_shape`. Native school: **Shadow**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Haunt`, skill ID 370, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2049`; `historical_not_live_parity`.

**Observable effect:** Summon a temporary unsettling shadow creature.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.haunting_shape.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: Conjuration.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source summons an actor; phantom room text is only an unapproved alternative, not complete coverage.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-370 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Haunting Shape, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Haunting Shape at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Shadow.
- Summon a temporary unsettling shadow creature.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source summons an actor; phantom room text is only an unapproved alternative, not complete coverage.

Historical metadata (not proposed policy): element Shadow; sphere Conjuration; mood Dominant; targets no explicit target; minimum position Standing; minimum mana 25; base power 15; components Conjuration.

Historical dump offsets claimed by the reference: `codedump(1).c:175698-175700`, `codedump(1).c:88803-88810`, `codedump(1).c:219731-219750`, `codedump(1).c:88812-88870`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shadow Blade

Key: `arm.spell.shadow_blade`. Native school: **Shadow**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Shadow Sword`, skill ID 375, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2069`; `historical_not_live_parity`.

**Observable effect:** Create a temporary shadow-themed sword.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shadow_blade.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Creation needs ownership and timed cleanup; no permanent highest-grade branch.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-375 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Shadow Blade, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Shadow Blade at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Shadow.
- Create a temporary shadow-themed sword.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Creation needs ownership and timed cleanup; no permanent highest-grade branch.

Historical metadata (not proposed policy): element Shadow; sphere Creation; mood Neutral; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175717-175719`, `codedump(1).c:94756-94759`, `codedump(1).c:221609-221641`, `codedump(1).c:94761-94853`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shadowplay

Key: `arm.spell.shadowplay`. Native school: **Shadow**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Shadowplay`, skill ID 384, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2089`; `historical_not_live_parity`.

**Observable effect:** Display a non-interactive shadow scene to the selected audience.

**Scaling:** One non-interactive scene for 60*g seconds; added light where selected 10*g lux. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shadowplay.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit presentation adaptation; source room darkness interactions are not silently reproduced.

**Runtime evidence:**
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-384 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Shadowplay, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Shadowplay at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Shadow.
- Display a non-interactive shadow scene to the selected audience.
- At grade 2 apply only this scaling contract: One non-interactive scene for 60*g seconds; added light where selected 10*g lux.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit presentation adaptation; source room darkness interactions are not silently reproduced.

Historical metadata (not proposed policy): element Shadow; sphere Illusion; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175745-175747`, `codedump(1).c:94856-94860`, `codedump(1).c:221645-221663`, `codedump(1).c:94862-95165`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Shadow Mantle

Key: `arm.spell.shadow_mantle`. Native school: **Shadow**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Shadow Armor`, skill ID 434, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2109`; `historical_not_live_parity`.

**Observable effect:** Apply a finite shadow-themed protective layer.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.shadow_mantle.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No source terrain-dependent armour-class stacking or automatic concealment.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-434 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Shadow Mantle, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Shadow Mantle at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Shadow.
- Apply a finite shadow-themed protective layer.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No source terrain-dependent armour-class stacking or automatic concealment.

Historical metadata (not proposed policy): element Shadow; sphere Abjuration; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175871-175873`, `codedump(1).c:218993-219030`, `codedump(1).c:86870-86959`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Draw into Flesh

Key: `arm.spell.draw_into_flesh`. Native school: **Shadow**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Dispel Ethereal`, skill ID 454, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2128`; `historical_not_live_parity`.

**Observable effect:** Remove an eligible spell-owned ethereal overlay.

**Scaling:** One eligible status-removal operation at mapped power; magnitude remains binary. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.shadow_passage`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.draw_into_flesh.skill`; shared option `arm.skill.shadow_casting`.
- Sorcerer: acquired `arm.spell.shadow_passage`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.draw_into_flesh.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native planar return rules apply; not indiscriminate alteration of every carried item's plane.

**Runtime evidence:**
- `removeplanarstate`: [MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs:175](../../MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs#L175), `RemovePlanarStateSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-454 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Draw into Flesh, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Draw into Flesh at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Shadow.
- Remove an eligible spell-owned ethereal overlay.
- At grade 2 apply only this scaling contract: One eligible status-removal operation at mapped power; magnitude remains binary.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native planar return rules apply; not indiscriminate alteration of every carried item's plane.

Historical metadata (not proposed policy): element Shadow; sphere Enchantment; mood Destructive; targets character in room, object in inventory, object in room, ethereal plane target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175929-175931`, `codedump(1).c:223267-223307`, `codedump(1).c:98053-98091`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Champion Blade

Key: `arm.spell.champion_blade`. Native school: **Shadow**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Hero Sword`, skill ID 488, coded family Shadow; `armageddon_magic_psionics_reference_second_pass.md:2147`; `historical_not_live_parity`.

**Observable effect:** Create a temporary enchanted sword with an explicitly selected combat profile.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Shadow: acquired `arm.spell.night_eyes`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.champion_blade.skill`; shared option `arm.skill.shadow_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No summoned-hero linkage without a separate lifecycle adapter.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `itemenchant`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:668](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L668), `ItemEnchantEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-488 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Shadow admission, acquired Champion Blade, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Champion Blade at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Shadow.
- Create a temporary enchanted sword with an explicitly selected combat profile.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No summoned-hero linkage without a separate lifecycle adapter.

Historical metadata (not proposed policy): element Shadow; sphere Conjuration; mood Protective; targets object in inventory, object in room; minimum position Sitting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:176021-176021`, `codedump(1).c:88972-88976`, `codedump(1).c:219858-219894`, `codedump(1).c:88978-89077`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Lightning Lance

Key: `arm.spell.lightning_lance`. Native school: **Lightning**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Lightning Bolt`, skill ID 14, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2171`; `historical_not_live_parity`.

**Observable effect:** Strike a target with electrical damage.

**Scaling:** Damage 5*g native units; pain 2*g; stun 0; no automatic splash. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.lightning_lance.skill`; shared option `arm.skill.lightning_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.lightning_lance.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=eligible other characters in the same physical cell, frozen distinct group; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit damage type and resistance, without source guild immunity assumptions.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-14 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Lightning Lance, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Lightning Lance at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Lightning.
- Strike a target with electrical damage.
- At grade 2 apply only this scaling contract: Damage 5*g native units; pain 2*g; stun 0; no automatic splash.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit damage type and resistance, without source guild immunity assumptions.

Historical metadata (not proposed policy): element Lightning; sphere Invocation; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174529-174531`, `codedump(1).c:90090-90093`, `codedump(1).c:220287-220347`, `codedump(1).c:90095-90148`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Second Breath

Key: `arm.spell.second_breath`. Native school: **Lightning**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Refresh`, skill ID 25, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2191`; `historical_not_live_parity`.

**Observable effect:** Restore a bounded amount of stamina.

**Scaling:** Restore 5*g native stamina units, capped by native maximum. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.second_breath.skill`; shared option `arm.skill.lightning_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.second_breath.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=eligible other characters in the same physical cell, frozen distinct group; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Cannot exceed native stamina limits or restore health automatically.

**Runtime evidence:**
- `staminadelta`: [MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs:94](../../MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs#L94), `StaminaDeltaSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-25 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Second Breath, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Second Breath at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Lightning.
- Restore a bounded amount of stamina.
- At grade 2 apply only this scaling contract: Restore 5*g native stamina units, capped by native maximum.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Cannot exceed native stamina limits or restore health automatically.

Historical metadata (not proposed policy): element Lightning; sphere Clerical; mood Beneficial; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174573-174575`, `codedump(1).c:93178-93182`, `codedump(1).c:221057-221094`, `codedump(1).c:93184-93219`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wakeful Mind

Key: `arm.spell.wakeful_mind`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Insomnia`, skill ID 67, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2211`; `historical_not_live_parity`.

**Observable effect:** Apply native magical wakefulness and its sleep interaction.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.wakeful_mind.skill`; shared option `arm.skill.lightning_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.wakeful_mind.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not remove fatigue or grant indefinite safe sleeplessness.

**Runtime evidence:**
- `insomnia`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:285](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L285), `InsomniaEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-67 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Wakeful Mind, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Wakeful Mind at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Lightning.
- Apply native magical wakefulness and its sleep interaction.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not remove fatigue or grant indefinite safe sleeplessness.

Historical metadata (not proposed policy): element Lightning; sphere Enchantment; mood Destructive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174742-174744`, `codedump(1).c:89658-89662`, `codedump(1).c:220062-220103`, `codedump(1).c:89664-89741`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Leadfoot

Key: `arm.spell.leadfoot`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Slow`, skill ID 76, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2231`; `historical_not_live_parity`.

**Observable effect:** Slow locomotion while applying an agility penalty.

**Scaling:** Movement delay multiplier 1+0.05*g and agility penalty -g for 60*g seconds. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.leadfoot.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** A bounded movement-delay modifier is missing; trait penalties alone are not the whole behavior.

**Runtime evidence:**
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `staminaexpendrate`: [MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs#L88), `StaminaExpenditureSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-76 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Leadfoot, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Leadfoot at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Lightning.
- Slow locomotion while applying an agility penalty.
- At grade 2 apply only this scaling contract: Movement delay multiplier 1+0.05*g and agility penalty -g for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: A bounded movement-delay modifier is missing; trait penalties alone are not the whole behavior.

Historical metadata (not proposed policy): element Lightning; sphere Alteration; mood Dominant; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174778-174780`, `codedump(1).c:95760-95763`, `codedump(1).c:221952-221994`, `codedump(1).c:95765-95834`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Steal Breath

Key: `arm.spell.steal_breath`. Native school: **Lightning**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Stamina Drain`, skill ID 87, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2251`; `historical_not_live_parity`.

**Observable effect:** Remove a bounded amount of the target's stamina.

**Scaling:** Subtract min(current stamina, 5*g) native units; no caster credit. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.steal_breath.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No transfer to caster or resource-cap mutation.

**Runtime evidence:**
- `staminadelta`: [MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs:94](../../MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs#L94), `StaminaDeltaSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-87 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Steal Breath, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Steal Breath at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Lightning.
- Remove a bounded amount of the target's stamina.
- At grade 2 apply only this scaling contract: Subtract min(current stamina, 5*g) native units; no caster credit.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No transfer to caster or resource-cap mutation.

Historical metadata (not proposed policy): element Lightning; sphere Invocation; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174816-174818`, `codedump(1).c:96021-96023`, `codedump(1).c:222083-222119`, `codedump(1).c:96025-96064`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Still Limbs

Key: `arm.spell.still_limbs`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Paralyze`, skill ID 171, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2271`; `historical_not_live_parity`.

**Observable effect:** Apply native magical paralysis after resistance.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.lightning_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.still_limbs.skill`; shared option `arm.skill.lightning_casting`.
- Sorcerer: acquired `arm.spell.lightning_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.still_limbs.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No promised automatic removal of all protective effects.

**Runtime evidence:**
- `paralysis`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs:393](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L393), `ParalysisEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-171 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Still Limbs, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Still Limbs at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Lightning.
- Apply native magical paralysis after resistance.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No promised automatic removal of all protective effects.

Historical metadata (not proposed policy): element Lightning; sphere Alteration; mood Harmful; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175073-175075`, `codedump(1).c:91128-91131`, `codedump(1).c:220587-220626`, `codedump(1).c:91133-91283`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Leaping Lightning

Key: `arm.spell.leaping_lightning`. Native school: **Lightning**. Target: `characters`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Chain Lightning`, skill ID 172, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2291`; `historical_not_live_parity`.

**Observable effect:** Make bounded repeated random strikes against eligible cell characters; the caster can be selected at quarter damage, other targets take full damage, and the source Energy Shield protection must be authored. The approved completion brief supersedes the earlier distinct-target/falloff proposal preserved as historical fields in the JSON inventory.

**Scaling:** Explicit bounded random selection with replacement, caster damage x0.25 and others x1; no implicit distinct-target requirement or diminishing falloff. The phase2C policy fixture uses three hits and native `grade*4` damage for verification only. Exact stock counts, damage and energy require source-informed authoring; the older `min(g+1,8)` / `0.75^jump` / B=9 proposal is not the approved source contract.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.lightning_lance`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.leaping_lightning.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** The phase2C runtime supplies bounded repeated random selection, explicit caster attenuation, physical-body deduplication and live native eligibility/ward checks. Installed Leaping Lightning, Energy Shield binding and final source stock values remain unqualified.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-172 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Leaping Lightning, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Leaping Lightning at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Lightning.
- Execute the approved bounded repeated random source policy, allowing repeated bodies, quarter caster damage and full other-target damage.
- Qualify authored stock hit counts/damage/energy and native Energy Shield protection; the three-hit `grade*4` fixture does not complete this stock scenario.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source inclusion, repeated-hit selection, native protection and one canonical payment/check/progression must remain explicit. The legacy 18-unit proposal above is historical and does not establish stock cost.

Historical metadata (not proposed policy): element Lightning; sphere Conjuration; mood Destructive; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175077-175079`, `codedump(1).c:83247-83251`, `codedump(1).c:217262-217280`, `codedump(1).c:83253-83373`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Storm Locus

Key: `arm.spell.storm_locus`. Native school: **Lightning**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Lightning Storm`, skill ID 173, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2311`; `historical_not_live_parity`.

**Observable effect:** Maintain a local electrical hazard that strikes eligible occupants on bounded ticks.

**Scaling:** For 30*g seconds, every 10 seconds strike eligible local targets for 2*g electrical units; no per-tick training. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.storm_locus.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs a spell-owned hazard lifecycle, not an environmental resource heartbeat.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-173 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Storm Locus, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Storm Locus at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Lightning.
- Maintain a local electrical hazard that strikes eligible occupants on bounded ticks.
- At grade 2 apply only this scaling contract: For 30*g seconds, every 10 seconds strike eligible local targets for 2*g electrical units; no per-tick training.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs a spell-owned hazard lifecycle, not an environmental resource heartbeat.

Historical metadata (not proposed policy): element Lightning; sphere Nature; mood Harmful; targets no explicit target; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175081-175083`, `codedump(1).c:90222-90229`, `codedump(1).c:220386-220406`, `codedump(1).c:90231-90290`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Charged Aegis

Key: `arm.spell.charged_aegis`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Energy Shield`, skill ID 174, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2331`; `historical_not_live_parity`.

**Observable effect:** Provide finite protection against configured electrical harm.

**Scaling:** Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.charged_aegis.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed armour adaptation; no automatic charge absorption or retaliation.

**Runtime evidence:**
- `spellarmour`: [MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs:287](../../MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs#L287), `SpellArmourEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-174 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Charged Aegis, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Charged Aegis at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Lightning.
- Provide finite protection against configured electrical harm.
- At grade 2 apply only this scaling contract: Absorption budget 10*g native damage units; duration 60*g seconds; Stone Skin agility modifier -g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed armour adaptation; no automatic charge absorption or retaliation.

Historical metadata (not proposed policy): element Lightning; sphere Creation; mood Protective; targets character in room, self only; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175085-175087`, `codedump(1).c:86224-86227`, `codedump(1).c:218739-218774`, `codedump(1).c:86230-86297`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Luminous Trail

Key: `arm.spell.luminous_trail`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Fluorescent Footsteps`, skill ID 177, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2351`; `historical_not_live_parity`.

**Observable effect:** Make native tracks left by the target easier to observe.

**Scaling:** Multiply native visual track intensity by 1+0.25*g for 60*g seconds. Energy base B=5; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.luminous_trail.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native track intensity exists; visible glowing footprint prose is content, not a second track store.

**Runtime evidence:**
- `trackmark`: [MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs:433](../../MudSharpCore/Magic/SpellEffects/PersistentSensoryCombatSpellEffects.cs#L433), `TrackMarkEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-177 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Luminous Trail, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Luminous Trail at grade 2 through the explicit route.

- Commit 10 designated energy units once; preserve native school Lightning.
- Make native tracks left by the target easier to observe.
- At grade 2 apply only this scaling contract: Multiply native visual track intensity by 1+0.25*g for 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native track intensity exists; visible glowing footprint prose is content, not a second track store.

Historical metadata (not proposed policy): element Lightning; sphere Alteration; mood Revealing; targets character in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175096-175098`, `codedump(1).c:87271-87276`, `codedump(1).c:219217-219253`, `codedump(1).c:87278-87320`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Quickened Healing

Key: `arm.spell.quickened_healing`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Regenerate`, skill ID 178, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2371`; `historical_not_live_parity`.

**Observable effect:** Increase native natural healing rates for a duration.

**Scaling:** Multiply natural healing rate by 1+0.1*g for 120*g seconds. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.quickened_healing.skill`; shared option `arm.skill.lightning_casting`.
- Sorcerer: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.quickened_healing.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=true, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No resurrection or instant wound closure.

**Runtime evidence:**
- `healingrate`: [MudSharpCore/Magic/SpellEffects/HealingRateSpellEffect.cs:126](../../MudSharpCore/Magic/SpellEffects/HealingRateSpellEffect.cs#L126), `HealingRateSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-178 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Quickened Healing, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Quickened Healing at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Lightning.
- Increase native natural healing rates for a duration.
- At grade 2 apply only this scaling contract: Multiply natural healing rate by 1+0.1*g for 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No resurrection or instant wound closure.

Historical metadata (not proposed policy): element Lightning; sphere Invocation; mood Beneficial; targets character in room; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175100-175102`, `codedump(1).c:93222-93225`, `codedump(1).c:221098-221133`, `codedump(1).c:93227-93267`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Lightning Pace

Key: `arm.spell.lightning_pace`. Native school: **Lightning**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Quickening`, skill ID 188, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2391`; `historical_not_live_parity`.

**Observable effect:** Improve agility and actual movement speed for a duration.

**Scaling:** Movement delay multiplier 1/(1+0.05*g), agility bonus g; duration 60*g seconds. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.lightning_pace.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires the same bounded speed modifier as Fleet Step; no implicit cancellation of all movement effects.

**Runtime evidence:**
- `boost`: [MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs:160](../../MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs#L160), `TraitBoostEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `staminaexpendrate`: [MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs#L88), `StaminaExpenditureSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-188 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Lightning Pace, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Lightning Pace at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Lightning.
- Improve agility and actual movement speed for a duration.
- At grade 2 apply only this scaling contract: Movement delay multiplier 1/(1+0.05*g), agility bonus g; duration 60*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires the same bounded speed modifier as Fleet Step; no implicit cancellation of all movement effects.

Historical metadata (not proposed policy): element Lightning; sphere Clerical; mood Aggressive; targets character in room, self only; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175139-175141`, `codedump(1).c:92950-92954`, `codedump(1).c:220981-221024`, `codedump(1).c:92956-93022`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Spark Lash

Key: `arm.spell.spark_lash`. Native school: **Lightning**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Lightning Whip`, skill ID 374, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2411`; `historical_not_live_parity`.

**Observable effect:** Create a temporary lightning-themed whip.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.spark_lash.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Needs temporary item lifecycle; no permanent staff conversion.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-374 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Spark Lash, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Spark Lash at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Lightning.
- Create a temporary lightning-themed whip.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs temporary item lifecycle; no permanent staff conversion.

Historical metadata (not proposed policy): element Lightning; sphere Creation; mood Neutral; targets no explicit target; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175713-175715`, `codedump(1).c:90293-90298`, `codedump(1).c:220410-220445`, `codedump(1).c:90300-90383`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sky Lantern

Key: `arm.spell.sky_lantern`. Native school: **Lightning**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Illuminant`, skill ID 381, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2431`; `historical_not_live_parity`.

**Observable effect:** Show a luminous sky display where location policy permits.

**Scaling:** One non-interactive scene for 60*g seconds; added light where selected 10*g lux. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.sky_lantern.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No physical light entity, uncontrolled animal fear or implicit weather change.

**Runtime evidence:**
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-381 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Sky Lantern, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Sky Lantern at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Lightning.
- Show a luminous sky display where location policy permits.
- At grade 2 apply only this scaling contract: One non-interactive scene for 60*g seconds; added light where selected 10*g lux.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No physical light entity, uncontrolled animal fear or implicit weather change.

Historical metadata (not proposed policy): element Lightning; sphere Illusion; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175736-175738`, `codedump(1).c:89251-89255`, `codedump(1).c:219939-219961`, `codedump(1).c:89257-89553`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Storm Spear

Key: `arm.spell.storm_spear`. Native school: **Lightning**. Target: `character`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Lightning Spear`, skill ID 444, coded family Lightning; `armageddon_magic_psionics_reference_second_pass.md:2451`; `historical_not_live_parity`.

**Observable effect:** Create a temporary lightning-themed spear.

**Scaling:** One specified weapon prototype, fixed base quality; lifetime 300*g seconds. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Lightning: acquired `arm.spell.second_breath`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.storm_spear.skill`; shared option `arm.skill.lightning_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Placement and expiry need explicit handling; questionable source rune references are not treated as spear semantics.

**Runtime evidence:**
- `createitem`: [MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs:103](../../MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs#L103), `CreateItemEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-444 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Lightning admission, acquired Storm Spear, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Storm Spear at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Lightning.
- Create a temporary lightning-themed spear.
- At grade 2 apply only this scaling contract: One specified weapon prototype, fixed base quality; lifetime 300*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Placement and expiry need explicit handling; questionable source rune references are not treated as spear semantics.

Historical metadata (not proposed policy): element Lightning; sphere Creation; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175895-175896`, `codedump(1).c:90151-90155`, `codedump(1).c:220352-220382`, `codedump(1).c:90158-90219`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Raise Servitor

Key: `arm.spell.raise_servitor`. Native school: **Void**. Target: `corpse`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Animate Dead`, skill ID 28, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2475`; `historical_not_live_parity`.

**Observable effect:** Animate an eligible corpse with selected AI and restore its corpse lifecycle on expiry.

**Scaling:** Animate one eligible corpse for 60*g seconds; no duplicate corpse or inventory. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.raise_servitor.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.raise_servitor.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use native corpse ownership/control constraints; no undeclared resurrection.

**Runtime evidence:**
- `animatecorpse`: [MudSharpCore/Magic/SpellEffects/DirectPossessionSpellEffects.cs:762](../../MudSharpCore/Magic/SpellEffects/DirectPossessionSpellEffects.cs#L762), `AnimateCorpseSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-28 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Raise Servitor, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible corpse. Invoke Raise Servitor at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Animate an eligible corpse with selected AI and restore its corpse lifecycle on expiry.
- At grade 2 apply only this scaling contract: Animate one eligible corpse for 60*g seconds; no duplicate corpse or inventory.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use native corpse ownership/control constraints; no undeclared resurrection.

Historical metadata (not proposed policy): element Void; sphere Necromancy; mood Neutral; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174585-174587`, `codedump(1).c:82081-82089`, `codedump(1).c:216767-216812`, `codedump(1).c:82091-82118`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Call Outsider

Key: `arm.spell.call_outsider`. Native school: **Void**. Target: `room`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Gate`, skill ID 36, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2495`; `historical_not_live_parity`.

**Observable effect:** Summon a temporary otherworldly creature under explicit control policy.

**Scaling:** One selected NPC template, no copied player progress; duration 120*g seconds. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.call_outsider.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.call_outsider.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Historical Gate summons a creature; a portal effect is not equivalent coverage.

**Runtime evidence:**
- `createnpc`: [MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs:80](../../MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs#L80), `CreateNPCEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-36 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Call Outsider, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Call Outsider at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Summon a temporary otherworldly creature under explicit control policy.
- At grade 2 apply only this scaling contract: One selected NPC template, no copied player progress; duration 120*g seconds.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Historical Gate summons a creature; a portal effect is not equivalent coverage.

Historical metadata (not proposed policy): element Void; sphere Conjuration; mood Dominant; targets no explicit target; minimum position Standing; minimum mana 50; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174617-174619`, `codedump(1).c:87593-87601`, `codedump(1).c:219371-219558`, `codedump(1).c:87603-87804`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Arcane Mark

Key: `arm.spell.arcane_mark`. Native school: **Void**. Target: `item`. Lifecycle: `timed`.

**Provenance:** Historical `Mark`, skill ID 55, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2515`; `historical_not_live_parity`.

**Observable effect:** Attach bounded spell-owned magical metadata to an item.

**Scaling:** One bounded metadata mark/anchor for 300*g seconds; no automatic network membership. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.arcane_mark.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.arcane_mark.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Metadata is not an implicit teleport destination or legal ownership transfer.

**Runtime evidence:**
- `magictag`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:91](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L91), `MagicTagEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-55 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Arcane Mark, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Arcane Mark at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Void.
- Attach bounded spell-owned magical metadata to an item.
- At grade 2 apply only this scaling contract: One bounded metadata mark/anchor for 300*g seconds; no automatic network membership.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Metadata is not an implicit teleport destination or legal ownership transfer.

Historical metadata (not proposed policy): element Void; sphere Enchantment; mood Neutral; targets object in inventory; minimum position Standing; minimum mana 25; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174694-174696`, `codedump(1).c:90422-90429`, `codedump(1).c:220449-220482`, `codedump(1).c:90431-90500`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Feign Death

Key: `arm.spell.feign_death`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Pseudo Death`, skill ID 57, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2535`; `historical_not_live_parity`.

**Observable effect:** Simulate death convincingly to configured observations while preserving the living character.

**Scaling:** Duration 60*g seconds; binary effect unless its native definition specifies magnitude. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.feign_death.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Needs coordinated appearance, physiological observation and action restrictions; a corpse description alone is insufficient.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-57 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Feign Death, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Feign Death at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Void.
- Simulate death convincingly to configured observations while preserving the living character.
- At grade 2 apply only this scaling contract: Duration 60*g seconds; binary effect unless its native definition specifies magnitude.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Needs coordinated appearance, physiological observation and action restrictions; a corpse description alone is insufficient.

Historical metadata (not proposed policy): element Void; sphere Illusion; mood Destructive; targets character in room, object in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174702-174704`, `codedump(1).c:92371-92374`, `codedump(1).c:220847-220871`, `codedump(1).c:92376-92458`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Silence the Mind

Key: `arm.spell.silence_the_mind`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Psionic Suppression`, skill ID 58, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2555`; `historical_not_live_parity`.

**Observable effect:** Suppress selected psionic actions and suspend relevant active links.

**Scaling:** Suppress the configured psionic tag set for 30*g seconds; no unbounded school-wide wildcard. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.silence_the_mind.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.silence_the_mind.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Interception exists; active-link teardown and all relevant psionic entry points need explicit coverage.

**Runtime evidence:**
- `personaltagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:267](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L267), `PersonalTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-58 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Silence the Mind, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Silence the Mind at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Void.
- Suppress selected psionic actions and suspend relevant active links.
- At grade 2 apply only this scaling contract: Suppress the configured psionic tag set for 30*g seconds; no unbounded school-wide wildcard.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Interception exists; active-link teardown and all relevant psionic entry points need explicit coverage.

Historical metadata (not proposed policy): element Void; sphere Enchantment; mood Dominant; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174706-174708`, `codedump(1).c:92525-92532`, `codedump(1).c:220918-220955`, `codedump(1).c:92534-92601`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Devouring Touch

Key: `arm.spell.devouring_touch`. Native school: **Void**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Dragon Drain`, skill ID 63, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2575`; `historical_not_live_parity`.

**Observable effect:** Drain delivered vitality into bounded healing for the caster.

**Scaling:** Attempt 5*g damage; heal caster by min(actual delivered damage, 5*g); never credit rejected harm. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.devouring_touch.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Requires measured transfer and configured target restrictions; no dragon cosmology imported.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `heal`: [MudSharpCore/Magic/SpellEffects/HealEffect.cs:173](../../MudSharpCore/Magic/SpellEffects/HealEffect.cs#L173), `HealEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-63 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Devouring Touch, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Devouring Touch at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Drain delivered vitality into bounded healing for the caster.
- At grade 2 apply only this scaling contract: Attempt 5*g damage; heal caster by min(actual delivered damage, 5*g); never credit rejected harm.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Requires measured transfer and configured target restrictions; no dragon cosmology imported.

Historical metadata (not proposed policy): element Void; sphere Necromancy; mood Harmful; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174726-174728`, `codedump(1).c:85958-85963`, `codedump(1).c:218444-218483`, `codedump(1).c:85965-86037`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Compelling Regard

Key: `arm.spell.compelling_regard`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Charm Person`, skill ID 69, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2595`; `historical_not_live_parity`.

**Observable effect:** Establish bounded, resistible influence over allowed NPC behaviour.

**Scaling:** Influence one eligible NPC for 30*g seconds through an explicit allowed-action set. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.compelling_regard.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** One forcecommand is not lasting charm; PC control must remain excluded unless separately approved.

**Runtime evidence:**
- `forcecommand`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1580](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1580), `ForceCommandEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-69 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Compelling Regard, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Compelling Regard at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Void.
- Establish bounded, resistible influence over allowed NPC behaviour.
- At grade 2 apply only this scaling contract: Influence one eligible NPC for 30*g seconds through an explicit allowed-action set.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: One forcecommand is not lasting charm; PC control must remain excluded unless separately approved.

Historical metadata (not proposed policy): element Void; sphere Alteration; mood Passive; targets character in room; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174750-174752`, `codedump(1).c:83376-83378`, `codedump(1).c:217284-217315`, `codedump(1).c:83380-83434`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Unmaking Ward

Key: `arm.spell.unmaking_ward`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Shield Of Nilaz`, skill ID 72, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2615`; `historical_not_live_parity`.

**Observable effect:** Intercept explicitly tagged elemental spell effects against the wearer.

**Scaling:** One configured tag-matching ward for 60*g seconds; native opposition at mapped power. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.unmaking_ward.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Tag matching is configuration; no named setting force or immunity to every magic type.

**Runtime evidence:**
- `personaltagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:267](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L267), `PersonalTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-72 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Unmaking Ward, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Unmaking Ward at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Void.
- Intercept explicitly tagged elemental spell effects against the wearer.
- At grade 2 apply only this scaling contract: One configured tag-matching ward for 60*g seconds; native opposition at mapped power.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Tag matching is configuration; no named setting force or immunity to every magic type.

Historical metadata (not proposed policy): element Void; sphere Invocation; mood Protective; targets character in room, self only; minimum position Standing; minimum mana 25; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174762-174764`, `codedump(1).c:95425-95428`, `codedump(1).c:221784-221819`, `codedump(1).c:95430-95488`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Severing Refuge

Key: `arm.spell.severing_refuge`. Native school: **Void**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Solace`, skill ID 75, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2635`; `historical_not_live_parity`.

**Observable effect:** Create a temporary refuge with explicitly defined isolation from selected elemental interactions.

**Scaling:** One isolated cell ward for 60*g seconds; metaphysical behavior unresolved beyond that proposed substitute. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.severing_refuge.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source metaphysical disconnection is unresolved; a ward-only substitute requires owner approval.

**Runtime evidence:**
- `roomtagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:218](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L218), `RoomTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-75 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Severing Refuge, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Severing Refuge at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Create a temporary refuge with explicitly defined isolation from selected elemental interactions.
- At grade 2 apply only this scaling contract: One isolated cell ward for 60*g seconds; metaphysical behavior unresolved beyond that proposed substitute.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source metaphysical disconnection is unresolved; a ward-only substitute requires owner approval.

Historical metadata (not proposed policy): element Void; sphere Clerical; mood Beneficial; targets no explicit target; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174774-174776`, `codedump(1).c:95871-95875`, `codedump(1).c:222036-222055`, `codedump(1).c:95877-95950`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Apex Bane

Key: `arm.spell.apex_bane`. Native school: **Void**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Dragon Bane`, skill ID 83, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2655`; `historical_not_live_parity`.

**Observable effect:** Harm a builder-tagged class of exceptional magical beings under explicit eligibility rules.

**Scaling:** Proposed 8*g damage against explicitly eligible tagged targets; setting-specific effect remains unresolved. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.apex_bane.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No canonical source creature metaphysics or anti-dragon contract is available; target taxonomy and effect need approval.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-83 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Apex Bane, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Apex Bane at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Harm a builder-tagged class of exceptional magical beings under explicit eligibility rules.
- At grade 2 apply only this scaling contract: Proposed 8*g damage against explicitly eligible tagged targets; setting-specific effect remains unresolved.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No canonical source creature metaphysics or anti-dragon contract is available; target taxonomy and effect need approval.

Historical metadata (not proposed policy): element Void; sphere Invocation; mood Aggressive; targets character in room; minimum position Fighting; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174800-174802`, `codedump(1).c:85903-85907`, `codedump(1).c:218397-218440`, `codedump(1).c:85909-85955`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Empty Aura

Key: `arm.spell.empty_aura`. Native school: **Void**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Aura Drain`, skill ID 86, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2675`; `historical_not_live_parity`.

**Observable effect:** Reduce a specifically configured target reserve without crediting the caster.

**Scaling:** Subtract min(current amount, 5*g) from the configured reserve; caster gains zero. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.empty_aura.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.empty_aura.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=true, charged_wand_staff=true, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_configuration`; `proposed_include`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Draining a reserve is distinct from linked transfer and does not require inventing a second resource system.

**Runtime evidence:**
- `magicresourcedelta`: [MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs:75](../../MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs#L75), `MagicResourceDeltaEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-86 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Empty Aura, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Empty Aura at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Void.
- Reduce a specifically configured target reserve without crediting the caster.
- At grade 2 apply only this scaling contract: Subtract min(current amount, 5*g) from the configured reserve; caster gains zero.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Draining a reserve is distinct from linked transfer and does not require inventing a second resource system.

Historical metadata (not proposed policy): element Void; sphere Invocation; mood Harmful; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:174812-174814`, `codedump(1).c:82250-82253`, `codedump(1).c:216858-216899`, `codedump(1).c:82255-82287`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Linked Threshold

Key: `arm.spell.linked_threshold`. Native school: **Void**. Target: `room`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Travel Gate`, skill ID 181, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2695`; `historical_not_live_parity`.

**Observable effect:** Create a bounded linked travel connection between authorised anchors.

**Scaling:** One authorised two-endpoint link for 120*g seconds; topology/plane/ownership constraints remain. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.open_threshold`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.linked_threshold.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Use native topology persistence/ownership and expiry; no arbitrary cross-plane bypass.

**Runtime evidence:**
- `portalnetwork`: [MudSharpCore/Magic/SpellEffects/PortalTopologySpellEffect.cs:82](../../MudSharpCore/Magic/SpellEffects/PortalTopologySpellEffect.cs#L82), `PortalTopologySpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-181 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Linked Threshold, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Linked Threshold at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Create a bounded linked travel connection between authorised anchors.
- At grade 2 apply only this scaling contract: One authorised two-endpoint link for 120*g seconds; topology/plane/ownership constraints remain.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Use native topology persistence/ownership and expiry; no arbitrary cross-plane bypass.

Historical metadata (not proposed policy): element Void; sphere Teleportation; mood Dominant; targets object in inventory; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175112-175114`, `codedump(1).c:96781-96784`, `codedump(1).c:222379-222400`, `codedump(1).c:96786-97006`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Bar the Elements

Key: `arm.spell.bar_the_elements`. Native school: **Void**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Forbid Elements`, skill ID 183, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2715`; `historical_not_live_parity`.

**Observable effect:** Refuse explicitly tagged elemental interactions in the cell.

**Scaling:** One configured tag-matching ward for 60*g seconds; native opposition at mapped power. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.bar_the_elements.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Content must enumerate tagged routes; existing effects are not all stripped automatically.

**Runtime evidence:**
- `roomtagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:218](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L218), `RoomTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-183 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Bar the Elements, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Bar the Elements at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Refuse explicitly tagged elemental interactions in the cell.
- At grade 2 apply only this scaling contract: One configured tag-matching ward for 60*g seconds; native opposition at mapped power.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Content must enumerate tagged routes; existing effects are not all stripped automatically.

Historical metadata (not proposed policy): element Void; sphere Nature; mood Destructive; targets no explicit target; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175119-175121`, `codedump(1).c:87402-87406`, `codedump(1).c:219295-219328`, `codedump(1).c:87408-87467`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Turn the Elements

Key: `arm.spell.turn_the_elements`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Turn Element`, skill ID 184, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2735`; `historical_not_live_parity`.

**Observable effect:** Redirect an eligible elemental attack to its source once under configured rules.

**Scaling:** For 30*g seconds redirect at most one eligible attack; never redirect a redirected attack. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.turn_the_elements.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** A rejecting ward is not reflection; needs bounded redirect and recursion prevention.

**Runtime evidence:**
- `personaltagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:267](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L267), `PersonalTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-184 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Turn the Elements, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Turn the Elements at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Void.
- Redirect an eligible elemental attack to its source once under configured rules.
- At grade 2 apply only this scaling contract: For 30*g seconds redirect at most one eligible attack; never redirect a redirected attack.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: A rejecting ward is not reflection; needs bounded redirect and recursion prevention.

Historical metadata (not proposed policy): element Void; sphere Abjuration; mood Protective; targets character in room, self only; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175123-175125`, `codedump(1).c:97009-97015`, `codedump(1).c:222404-222434`, `codedump(1).c:97017-97094`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Folded Pocket

Key: `arm.spell.folded_pocket`. Native school: **Void**. Target: `item`. Lifecycle: `created_temporary`.

**Provenance:** Historical `Portable Hole`, skill ID 185, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2755`; `historical_not_live_parity`.

**Observable effect:** Create a carried pocket-space container with safe contents and collapse rules.

**Scaling:** Proposed capacity 2*g kilograms for 300*g seconds; conservation/collapse rules await supporting design. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.folded_pocket.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=false, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=false. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `larger_supporting_system`; `proposed_defer`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Portals do not establish portable storage capacity, ownership, persistence and item conservation.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-185 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Folded Pocket, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Folded Pocket at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Create a carried pocket-space container with safe contents and collapse rules.
- At grade 2 apply only this scaling contract: Proposed capacity 2*g kilograms for 300*g seconds; conservation/collapse rules await supporting design.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Portals do not establish portable storage capacity, ownership, persistence and item conservation.

Historical metadata (not proposed policy): element Void; sphere Creation; mood Neutral; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175127-175129`, `codedump(1).c:91796-91801`, `codedump(1).c:220741-220774`, `codedump(1).c:91803-91870`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Veil of Elements

Key: `arm.spell.veil_of_elements`. Native school: **Void**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Elemental Fog`, skill ID 186, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2775`; `historical_not_live_parity`.

**Observable effect:** Obscure a cell with a configured non-physical elemental mist.

**Scaling:** Reduce light by 5*g lux and show filtered scene text for 60*g seconds; no physical fog collision. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.veil_of_elements.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit visual adaptation; no assertion of source immunity or universal targeting suppression.

**Runtime evidence:**
- `roomlight`: [MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs:188](../../MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs#L188), `RoomLightEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-186 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Veil of Elements, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Veil of Elements at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Void.
- Obscure a cell with a configured non-physical elemental mist.
- At grade 2 apply only this scaling contract: Reduce light by 5*g lux and show filtered scene text for 60*g seconds; no physical fog collision.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit visual adaptation; no assertion of source immunity or universal targeting suppression.

Historical metadata (not proposed policy): element Void; sphere Illusion; mood Neutral; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175131-175133`, `codedump(1).c:86175-86179`, `codedump(1).c:218509-218548`, `codedump(1).c:86181-86221`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Cross the Veil

Key: `arm.spell.cross_the_veil`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Planeshift`, skill ID 187, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2795`; `historical_not_live_parity`.

**Observable effect:** Apply an approved native planar-presence transition.

**Scaling:** One configured planar overlay for 60*g seconds; grade does not invent planes or permission. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.cross_the_veil.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.cross_the_veil.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Does not invent a separate astral framework or a full elemental-plane simulation.

**Runtime evidence:**
- `planeshift`: [MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs:110](../../MudSharpCore/Magic/SpellEffects/PlanarStateSpellEffects.cs#L110), `PlanarStateSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-187 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Cross the Veil, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Cross the Veil at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Apply an approved native planar-presence transition.
- At grade 2 apply only this scaling contract: One configured planar overlay for 60*g seconds; grade does not invent planes or permission.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Does not invent a separate astral framework or a full elemental-plane simulation.

Historical metadata (not proposed policy): element Void; sphere Teleportation; mood Harmful; targets character in room, self only; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175135-175137`, `codedump(1).c:97766-97772`, `codedump(1).c:222480-222521`, `codedump(1).c:97774-97885`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Borrow the Dead

Key: `arm.spell.borrow_the_dead`. Native school: **Void**. Target: `corpse`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Possess Corpse`, skill ID 247, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2815`; `historical_not_live_parity`.

**Observable effect:** Control an eligible corpse through the native shared-identity possession mechanism.

**Scaling:** One eligible corpse instance for 60*g seconds; restore native corpse state on expiry. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.raise_servitor`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.borrow_the_dead.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.raise_servitor`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.borrow_the_dead.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Actual body, focus, corpse restoration and possession policies remain authoritative.

**Runtime evidence:**
- `possesscorpse`: [MudSharpCore/Magic/SpellEffects/DirectPossessionSpellEffects.cs:416](../../MudSharpCore/Magic/SpellEffects/DirectPossessionSpellEffects.cs#L416), `PossessCorpseSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-247 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Borrow the Dead, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible corpse. Invoke Borrow the Dead at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Control an eligible corpse through the native shared-identity possession mechanism.
- At grade 2 apply only this scaling contract: One eligible corpse instance for 60*g seconds; restore native corpse state on expiry.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Actual body, focus, corpse restoration and possession policies remain authoritative.

Historical metadata (not proposed policy): element Void; sphere Necromancy; mood Passive; targets object in room; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175317-175319`, `codedump(1).c:92162-92165`, `codedump(1).c:220808-220843`, `codedump(1).c:92167-92368`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Open Threshold

Key: `arm.spell.open_threshold`. Native school: **Void**. Target: `room`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Portal`, skill ID 248, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2835`; `historical_not_live_parity`.

**Observable effect:** Create a transient permitted exit between resolved cells.

**Scaling:** One authorised two-endpoint link for 120*g seconds; topology/plane/ownership constraints remain. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.open_threshold.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `explicit_historical_sorcerer_comment_not_complete_learnlist`. Default trait `arm.spell.open_threshold.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Native same-zone default, permission and planar checks remain in force.

**Runtime evidence:**
- `portal`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1367](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1367), `PortalSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-248 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Open Threshold, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Open Threshold at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Void.
- Create a transient permitted exit between resolved cells.
- At grade 2 apply only this scaling contract: One authorised two-endpoint link for 120*g seconds; topology/plane/ownership constraints remain.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Native same-zone default, permission and planar checks remain in force.

Historical metadata (not proposed policy): element Void; sphere Teleportation; mood Passive; targets psionic contact target, cannot target self, object in inventory; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175321-175323`, `codedump(1).c:91873-91880`, `codedump(1).c:220778-220804`, `codedump(1).c:91882-92020`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Read Enchantment

Key: `arm.spell.read_enchantment`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Identify`, skill ID 337, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2855`; `historical_not_live_parity`.

**Observable effect:** Let the recipient see authored information about permitted targets through LOOK.

**Scaling:** Enable the configured information overlay for 60*g seconds; disclosure stays permission-gated. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: once-only starting grant on explicit permanent enrolment. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.read_enchantment.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.read_enchantment.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** A required text prog defines disclosure; no automatic exhaustive item-stat reveal.

**Runtime evidence:**
- `identify`: [MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs:191](../../MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs#L191), `IdentifySpellEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-337 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Read Enchantment, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Read Enchantment at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Void.
- Let the recipient see authored information about permitted targets through LOOK.
- At grade 2 apply only this scaling contract: Enable the configured information overlay for 60*g seconds; disclosure stays permission-gated.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: A required text prog defines disclosure; no automatic exhaustive item-stat reveal.

Historical metadata (not proposed policy): element Void; sphere Divination; mood Revealing; targets character in room, self only; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175592-175594`, `codedump(1).c:89080-89083`, `codedump(1).c:219898-219935`, `codedump(1).c:89085-89157`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Blade Barrier

Key: `arm.spell.blade_barrier`. Native school: **Void**. Target: `exit`. Lifecycle: `timed`.

**Provenance:** Historical `Blade Barrier`, skill ID 348, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2875`; `historical_not_live_parity`.

**Observable effect:** Block passage with a timed barrier that cuts crossing attempts.

**Scaling:** Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt. Energy base B=9; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.blade_barrier.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Traversal damage needs an explicit adapter; bare blocking is an unapproved reduction.

**Runtime evidence:**
- `exitbarrier`: [MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs:55](../../MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs#L55), `ExitBarrierEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-348 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Blade Barrier, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible exit. Invoke Blade Barrier at grade 2 through the explicit route.

- Commit 18 designated energy units once; preserve native school Void.
- Block passage with a timed barrier that cuts crossing attempts.
- At grade 2 apply only this scaling contract: Barrier duration 60*g seconds; proposed traversal damage 3*g native units per attempt.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Traversal damage needs an explicit adapter; bare blocking is an unapproved reduction.

Historical metadata (not proposed policy): element Void; sphere Alteration; mood Protective; targets no explicit target; minimum position Fighting; minimum mana 10; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175628-175630`, `codedump(1).c:216993-217090`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Mind Scour

Key: `arm.spell.mind_scour`. Native school: **Void**. Target: `character`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Psionic Drain`, skill ID 376, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2893`; `historical_not_live_parity`.

**Observable effect:** Apply configured psychic injury through normal spell resistance.

**Scaling:** Damage 5*g native units; pain 2*g; stun 0; no automatic splash. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.mind_scour.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No automatic psionic-barrier stripping or caster resource transfer.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-376 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Mind Scour, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Mind Scour at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Void.
- Apply configured psychic injury through normal spell resistance.
- At grade 2 apply only this scaling contract: Damage 5*g native units; pain 2*g; stun 0; no automatic splash.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No automatic psionic-barrier stripping or caster resource transfer.

Historical metadata (not proposed policy): element Void; sphere Invocation; mood Destructive; targets character in room; minimum position Fighting; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175721-175723`, `codedump(1).c:92460-92463`, `codedump(1).c:220875-220914`, `codedump(1).c:92465-92522`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Phantasm

Key: `arm.spell.phantasm`. Native school: **Void**. Target: `room`. Lifecycle: `timed`.

**Provenance:** Historical `Phantasm`, skill ID 383, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2913`; `historical_not_live_parity`.

**Observable effect:** Display an audience-filtered, non-interactive apparition in the cell.

**Scaling:** One non-interactive scene for 60*g seconds; added light where selected 10*g lux. Energy base B=6; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.phantasm.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.phantasm.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No physical actor, combat target, inventory object or objective world-fact replacement.

**Runtime evidence:**
- `phantomillusion`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:1994](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L1994), `PhantomIllusionEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-383 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Phantasm, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Phantasm at grade 2 through the explicit route.

- Commit 12 designated energy units once; preserve native school Void.
- Display an audience-filtered, non-interactive apparition in the cell.
- At grade 2 apply only this scaling contract: One non-interactive scene for 60*g seconds; added light where selected 10*g lux.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No physical actor, combat target, inventory object or objective world-fact replacement.

Historical metadata (not proposed policy): element Void; sphere Illusion; mood Passive; targets no explicit target; minimum position Standing; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175742-175744`, `codedump(1).c:91363-91369`, `codedump(1).c:220674-220692`, `codedump(1).c:91371-91651`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Unbodied Journey

Key: `arm.spell.unbodied_journey`. Native school: **Void**. Target: `self`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Disembody`, skill ID 422, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2933`; `historical_not_live_parity`.

**Observable effect:** Create a focusable native projection while the anchor body remains.

**Scaling:** At most one new controlled instance from this invocation for 60*g seconds; native anchor/focus rules apply. Energy base B=12; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.unbodied_journey.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.unbodied_journey.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Reuse native instance/anchor lifetime rules; no new identity model or historical paralysis assumptions.

**Runtime evidence:**
- `astralprojection`: [MudSharpCore/Magic/SpellEffects/AstralProjectionSpellEffect.cs:165](../../MudSharpCore/Magic/SpellEffects/AstralProjectionSpellEffect.cs#L165), `AstralProjectionSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-422 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Unbodied Journey, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible self. Invoke Unbodied Journey at grade 2 through the explicit route.

- Commit 24 designated energy units once; preserve native school Void.
- Create a focusable native projection while the anchor body remains.
- At grade 2 apply only this scaling contract: At most one new controlled instance from this invocation for 60*g seconds; native anchor/focus rules apply.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Reuse native instance/anchor lifetime rules; no new identity model or historical paralysis assumptions.

Historical metadata (not proposed policy): element Void; sphere Nature; mood Aggressive; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175835-175836`, `codedump(1).c:84747-84751`, `codedump(1).c:218102-218143`, `codedump(1).c:84754-84878`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Erode Object

Key: `arm.spell.erode_object`. Native school: **Void**. Target: `item`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Rot Items`, skill ID 467, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2953`; `historical_not_live_parity`.

**Observable effect:** Inflict finite material-gated damage on an item.

**Scaling:** Apply at most 5*g native item damage units to eligible material; do not damage contents. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.erode_object.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** No arbitrary corrosion of contents, indefinite decay or ecological land debit.

**Runtime evidence:**
- `itemdamage`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:345](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L345), `ItemDamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-467 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Erode Object, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Erode Object at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Void.
- Inflict finite material-gated damage on an item.
- At grade 2 apply only this scaling contract: Apply at most 5*g native item damage units to eligible material; do not damage contents.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: No arbitrary corrosion of contents, indefinite decay or ecological land debit.

Historical metadata (not proposed policy): element Void; sphere Nature; mood Dominant; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175972-175974`, `codedump(1).c:93783-93785`, `codedump(1).c:221331-221377`, `codedump(1).c:93787-93793`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Leeching Edge

Key: `arm.spell.leeching_edge`. Native school: **Void**. Target: `item`. Lifecycle: `timed`.

**Provenance:** Historical `Vampiric Blade`, skill ID 468, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2972`; `historical_not_live_parity`.

**Observable effect:** Enchant a weapon to heal its wielder by a bounded share of actual damage dealt.

**Scaling:** For 60*g seconds heal wielder by min(0.1*g,0.5) times delivered weapon damage; no heal on rejected hits. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.leeching_edge.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Existing flat enchantment bonuses are not a per-hit measured lifesteal hook.

**Runtime evidence:**
- `itemenchant`: [MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs:668](../../MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs#L668), `ItemEnchantEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-468 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Leeching Edge, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible item. Invoke Leeching Edge at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Enchant a weapon to heal its wielder by a bounded share of actual damage dealt.
- At grade 2 apply only this scaling contract: For 60*g seconds heal wielder by min(0.1*g,0.5) times delivered weapon damage; no heal on rejected hits.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Existing flat enchantment bonuses are not a per-hit measured lifesteal hook.

Historical metadata (not proposed policy): element Void; sphere Enchantment; mood Aggressive; targets object in inventory, object in room; minimum position Standing; minimum mana 25; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175976-175978`, `codedump(1).c:97097-97103`, `codedump(1).c:222438-222476`, `codedump(1).c:97105-97215`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Voice of Remains

Key: `arm.spell.voice_of_remains`. Native school: **Void**. Target: `corpse`. Lifecycle: `effect_bound`.

**Provenance:** Historical `Dead Speak`, skill ID 469, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:2992`; `historical_not_live_parity`.

**Observable effect:** Raise a temporary corpse speech proxy using native corpse restoration rules.

**Scaling:** One temporary corpse speech proxy for 60*g seconds; no invented knowledge or free control of other bodies. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.raise_servitor`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.voice_of_remains.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.raise_servitor`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.voice_of_remains.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source knowledge/answer policy is not established; no omniscient answers or recovery of an offline player's mind.

**Runtime evidence:**
- `deadspeak`: [MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs:547](../../MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs#L547), `DeadSpeakSpellEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-469 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Voice of Remains, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible corpse. Invoke Voice of Remains at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Void.
- Raise a temporary corpse speech proxy using native corpse restoration rules.
- At grade 2 apply only this scaling contract: One temporary corpse speech proxy for 60*g seconds; no invented knowledge or free control of other bodies.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source knowledge/answer policy is not established; no omniscient answers or recovery of an offline player's mind.

Historical metadata (not proposed policy): element Void; sphere Necromancy; mood Dominant; targets no explicit target; minimum position Standing; minimum mana 33; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175980-175982`, `codedump(1).c:84026-84028`, `codedump(1).c:217718-217764`, `codedump(1).c:84030-84075`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Echo Servant

Key: `arm.spell.echo_servant`. Native school: **Void**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Recite`, skill ID 489, coded family Void; `armageddon_magic_psionics_reference_second_pass.md:3012`; `historical_not_live_parity`.

**Observable effect:** Relay authorised speech through a selected proxy using native language-aware output.

**Scaling:** One authorised language-aware speech relay for 60*g seconds; no command relay. Energy base B=7; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `coded_element_association_not_learnlist`. Default trait `arm.spell.echo_servant.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.echo_servant.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Explicit adaptation: historical Recite opens a charmed minion's psionic contact, not merely a speech relay.

**Runtime evidence:**
- `reciteproxy`: [MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs:327](../../MudSharpCore/Magic/SpellEffects/ArmageddonInformationSpellEffects.cs#L327), `ReciteProxySpellEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-489 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Void admission, acquired Echo Servant, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Echo Servant at grade 2 through the explicit route.

- Commit 14 designated energy units once; preserve native school Void.
- Relay authorised speech through a selected proxy using native language-aware output.
- At grade 2 apply only this scaling contract: One authorised language-aware speech relay for 60*g seconds; no command relay.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Explicit adaptation: historical Recite opens a charmed minion's psionic contact, not merely a speech relay.

Historical metadata (not proposed policy): element Void; sphere Alteration; mood Dominant; targets character in room; minimum position Fighting; minimum mana 0; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:176024-176024`, `codedump(1).c:223310-223329`, `codedump(1).c:98149-98202`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Drowning Grip

Key: `arm.spell.drowning_grip`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Drown`, skill ID 428, coded family Unspecified; `armageddon_magic_psionics_reference_second_pass.md:3035`; `unbound_entry`.

**Observable effect:** Interfere with breathing through a bounded, resistible temporary effect.

**Scaling:** Suppress effective breathing for at most 5*g seconds subject to native hypoxia; no fabricated damage conversion. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.drowning_grip.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `small_missing_primitive`; `proposed_gap_candidate`; approval `awaiting_roster_approval`. No direct manifestation primitive established for the required behavior.

**Boundary/gap:** Source entry has no bound function; hypoxia rather than generic damage needs a specific adapter and acceptance.

**Runtime evidence:**
- No direct manifestation primitive established; retain the explicit gap.

**Minimum acceptance â€” ARM-R-428 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Drowning Grip, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Drowning Grip at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Interfere with breathing through a bounded, resistible temporary effect.
- At grade 2 apply only this scaling contract: Suppress effective breathing for at most 5*g seconds subject to native hypoxia; no fabricated damage conversion.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source entry has no bound function; hypoxia rather than generic damage needs a specific adapter and acceptance.

Historical metadata (not proposed policy): element Unspecified; sphere n/a; mood n/a; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175853-175854`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Sow Sickness

Key: `arm.spell.sow_sickness`. Native school: **Water**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Historical `Cause Disease`, skill ID 430, coded family Unspecified; `armageddon_magic_psionics_reference_second_pass.md:3048`; `unbound_entry`.

**Observable effect:** Apply one configured native disease to a susceptible target.

**Scaling:** Apply one configured native infection owned by a 60*g-second spell effect; removal withdraws its tracked infection. Strain parameters require content approval; grade does not invent a contagion multiplier. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.sow_sickness.skill`; shared option `arm.skill.water_casting`.
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.sow_sickness.skill`; shared option `arm.skill.void_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source entry is unbound; disease choice and transmission policy are proposed content.

**Runtime evidence:**
- `disease`: [MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs:467](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs#L467), `DiseaseEffect.CreateEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-430 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Sow Sickness, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Sow Sickness at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Apply one configured native disease to a susceptible target.
- At grade 2 apply only this scaling contract: Apply one configured native infection owned by a 60*g-second spell effect; removal withdraws its tracked infection. Strain parameters require content approval; grade does not invent a contagion multiplier.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source entry is unbound; disease choice and transmission policy are proposed content.

Historical metadata (not proposed policy): element Unspecified; sphere n/a; mood n/a; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175859-175860`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Caustic Spray

Key: `arm.spell.caustic_spray`. Native school: **Water**. Target: `characters`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Acid Spray`, skill ID 432, coded family Unspecified; `armageddon_magic_psionics_reference_second_pass.md:3061`; `unbound_entry`.

**Observable effect:** Strike explicitly selected local targets with acid-type damage.

**Scaling:** Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g. Energy base B=8; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.caustic_spray.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source is unbound; no armour corrosion or persistent puddle is implied.

**Runtime evidence:**
- `damage`: [MudSharpCore/Magic/SpellEffects/DamageEffect.cs:179](../../MudSharpCore/Magic/SpellEffects/DamageEffect.cs#L179), `DamageEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-432 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Caustic Spray, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible characters. Invoke Caustic Spray at grade 2 through the explicit route.

- Commit 16 designated energy units once; preserve native school Water.
- Strike explicitly selected local targets with acid-type damage.
- At grade 2 apply only this scaling contract: Damage 3*g native units per eligible local target; secondary stamina loss if selected 2*g.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source is unbound; no armour corrosion or persistent puddle is implied.

Historical metadata (not proposed policy): element Unspecified; sphere n/a; mood n/a; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175865-175866`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Gather Puddle

Key: `arm.spell.gather_puddle`. Native school: **Water**. Target: `room`. Lifecycle: `instantaneous`.

**Provenance:** Historical `Puddle`, skill ID 433, coded family Unspecified; `armageddon_magic_psionics_reference_second_pass.md:3074`; `unbound_entry`.

**Observable effect:** Create or top up a native clean-water puddle in the current cell.

**Scaling:** Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable. Energy base B=3; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.gather_puddle.skill`; shared option `arm.skill.water_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: none.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Source is unbound; this is a direct native puddle adaptation, not container filling.

**Runtime evidence:**
- `createliquid`: [MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs:88](../../MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs#L88), `CreateLiquidEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-433 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Water admission, acquired Gather Puddle, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Gather Puddle at grade 2 through the explicit route.

- Commit 6 designated energy units once; preserve native school Water.
- Create or top up a native clean-water puddle in the current cell.
- At grade 2 apply only this scaling contract: Create up to 0.25*g litres of the specified liquid, capped by carrier capacity where applicable.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Source is unbound; this is a direct native puddle adaptation, not container filling.

Historical metadata (not proposed policy): element Unspecified; sphere n/a; mood n/a; targets character in room; minimum position Standing; minimum mana 20; base power 15; components none.

Historical dump offsets claimed by the reference: `codedump(1).c:175868-175869`. Reference offsets sampled against available renamed dump; per-entry adaptation is a proposal, not proof of original full guild behavior.

### Wardcraft

Key: `arm.spell.wardcraft`. Native school: **Earth**. Target: `character`. Lifecycle: `timed`.

**Provenance:** Explicit proposed addition; no historical source claim.

**Observable effect:** Apply a basic tag-scoped personal ward shared across admitted traditions.

**Scaling:** One configured tag-matching ward for 60*g seconds; native opposition at mapped power. Energy base B=4; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: once-only starting grant on explicit permanent enrolment. Membership evidence: `proposed_membership`. Default trait `arm.spell.wardcraft.skill`; shared option `arm.skill.earth_casting`.
- Water: acquired `arm.spell.draw_water`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.wardcraft.skill`; shared option `arm.skill.water_casting`.
- Void: acquired `arm.spell.arcane_mark`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.wardcraft.skill`; shared option `arm.skill.void_casting`.
- Sorcerer: once-only starting grant on explicit permanent enrolment. Membership evidence: `proposed_membership`. Default trait `arm.spell.wardcraft.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 0 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: No historical requirement asserted.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=true; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Proposed addition for shared-prerequisite and cross-school fixtures; exact ward tags are stock configuration.

**Runtime evidence:**
- `personaltagward`: [MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs:267](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L267), `PersonalTagWardEffect.CreateWardEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-10001 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Wardcraft, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible character. Invoke Wardcraft at grade 2 through the explicit route.

- Commit 8 designated energy units once; preserve native school Earth.
- Apply a basic tag-scoped personal ward shared across admitted traditions.
- At grade 2 apply only this scaling contract: One configured tag-matching ward for 60*g seconds; native opposition at mapped power.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Proposed addition for shared-prerequisite and cross-school fixtures; exact ward tags are stock configuration.

### Renew Earth

Key: `arm.spell.renew_earth`. Native school: **Earth**. Target: `room`. Lifecycle: `timed_treatment`.

**Provenance:** Explicit proposed addition; no historical source claim.

**Observable effect:** Apply a bounded scar-repair treatment to the eligible physical cell.

**Scaling:** Scar budget 2*g units; rate 0.1*g scar units per eligible second; lifetime 120*g seconds; native caps/zero termination win. Energy base B=10; `B * grade * overreach_multiplier * quiet_multiplier * area_multiplier`.

**Membership/acquisition:**
- Earth: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.renew_earth.skill`; shared option `arm.skill.earth_casting`.
- Water: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.renew_earth.skill`; shared option `arm.skill.water_casting`.
- Sorcerer: acquired `arm.spell.wardcraft`, controlled grade >= 2, its route skill >= 20. Membership evidence: `proposed_membership`. Default trait `arm.spell.renew_earth.skill`; shared option `arm.skill.sorcerer_casting`.

**Physical plan:** speech: required; quiet only when explicitly enabled; manipulation: one free manipulating appendage, in addition to held focus if required; actor: focused permitted physical instance; native plane/body gates; target_constraint: accessible owned/consenting target or authorised hostile action; corpse controls retain native ownership and anchor checks.

**Materials/foci:** retained held/wielded focus tagged for the selected tradition at grades 3-7; none at grades 1-2; consume 1 unit(s) of `arm.material.ritual_reagent` at commitment. Practice: retained tradition focus at all grades, no consumed materials. Historical component metadata: No historical requirement asserted.

**Practice:** enabled=true, 30 seconds, full energy, proficiency and mastery eligible under shared gates; no target/caster effects or target wards/resistance. Implementation: `missing_common_practice_slice`.

**Variants:** quiet=false; area=not admitted; quiet+area not admitted.

**Portable eligibility (proposal):** scroll=false, charged_wand_staff=false, substance=false, focus_role=true. Require every effect/trigger/retained numeric binding to pass the existing carrier-specific compatibility gate. Wands/staves and channelling production are missing integrations; never infer eligibility from registration.

**Coverage:** `existing_primitive_plus_bounded_policy_content`; `proposed_adaptation`; approval `awaiting_roster_approval`. Traced existing effect method(s) for listed primitives; the complete proposed content and casting policy are not implemented.

**Boundary/gap:** Reuse delivered treatment budget/rate/lifetime and zero-scar termination; 04C remains a separate acceptance gate.

**Runtime evidence:**
- `rejuvenateland`: [MudSharpCore/Magic/SpellEffects/RejuvenateLandEffect.cs:152](../../MudSharpCore/Magic/SpellEffects/RejuvenateLandEffect.cs#L152), `RejuvenateLandEffect.GetOrApplyEffect` at `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.

**Minimum acceptance â€” ARM-R-10002 (`NOT_RUN_PROPOSED_CONTENT`):** Non-admin fixture with Earth admission, acquired Renew Earth, controlled grade 2, native route skill 42, 100 designated energy units and the specified physical plan; select an eligible room. Invoke Renew Earth at grade 2 through the explicit route.

- Commit 20 designated energy units once; preserve native school Earth.
- Apply a bounded scar-repair treatment to the eligible physical cell.
- At grade 2 apply only this scaling contract: Scar budget 2*g units; rate 0.1*g scar units per eligible second; lifetime 120*g seconds; native caps/zero termination win.
- Reject absent admission or invalid body/target before payment; a resisted paid invocation cannot bypass costs or duplicate advancement.
- Boundary assertion: Reuse delivered treatment budget/rate/lifetime and zero-scar termination; 04C remains a separate acceptance gate.
