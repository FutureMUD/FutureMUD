# Armageddon psionics

The Armageddon Setting seeder supplies the Way alongside its classes. It does not require or alter the generic Psionics seeder.

## Class access

| Capability | Classes | Powers |
| --- | --- | --- |
| Normal | Ordinary main guilds, magic classes including Sorcerers, Jihaen and Southern Templars | Contact, connect-back, directed speech, Barrier, Expel |
| Lirathan Templar | Lirathan Templar | Twenty source techniques plus the two Contact communication commands |
| Psionicist | Psionicist | All 27 psionic skills in the retained branch tree, plus the two Contact communication commands |

Profiles share power records and canonical skills. Each main class grants one profile; subclasses do not add a second profile. Learning a power's skill unlocks it. Expel branches from Barrier at 70. Ordinary magic classes receive Contact 20/80, Barrier 5/80 and Expel 5/80, matching the retained Sorcerer support tree and ordinary guild values. Sorcerers have no separately evidenced extra psionic powers.

The shared school's existing `lpsi` verb is retained for compatibility. For example, `lpsi contact`, `lpsi say`, `lpsi barrier` and `lpsi expel` use the same powers across profiles. Class availability restrictions still apply. The seeder changes class definitions and does not enroll existing characters or rewrite their skills.

## Additional mechanics

- **Probe** reads a contacted mind of the same race. At skill 10 it reveals age, height and weight; at 30, native racial attributes; at 50, player identity, immediate and long-term plans, affiliations and current contacts. At 60 and above an optional `information` text prog receives `(actor, target, skill)` for world-authored memories. FutureMUD has no equivalent public source biography/shadow-description store: those details are not fabricated or obtained from private account/staff notes. Builders can supply them through this prog, using 60/70 thresholds.
- **Rejuvenate** exchanges remaining stun for health, hunger, thirst, sobriety or stamina. Its budget is the smaller of remaining stun minus one and skill percent of maximum stun. The conversion cost is `(200 - skill) / 100`; failure loses two-thirds of the potential gain, bounded by the condition's native floor. Costs are paid before benefits. Health repairs only wounds eligible for magical mending. Native hunger/thirst use satiated hours; sobriety uses centilitres of alcohol. Ordinary bodily invocation costs also apply.
- **Disorient** reduces the configured attribute (Agility by default) by at least one, scaling with skill/20, for one or two ten-minute source ticks. It drops equipment the target can drop and releases grapples held by that target. Native inventory restrictions remain authoritative.
- **Sense Presence** is sustained awareness, not a room scan or incoming-contact audit. It adds learned skill to spotting sneaking characters with psionic capabilities and reveals unconcealed Vanish in the same room and plane.
- **Vanish** cloaks visual appearance through an opposed mental check for each observer. Observer results are cached for 30 seconds, with a bounded cache; range and Sense Presence are rechecked before using a cached result. `room`, `near`, `far` and `veryfar` require 0/25/50/75 skill and map to the room, adjacent rooms, zone and shard. These geographic scopes are explicit native adaptations. Ordinary planar perception still applies.
- **Compel** uses the existing restricted forced-command path over an outgoing mental contact. **Control** retains its same-room requirement. Both respect involuntary-traffic opt-outs and command restrictions, with eligibility rechecked immediately before execution. Builders can change `samelocation` separately from the distance policy.
- **Project** projects an emotion; it is not astral projection. **Suggest** delivers a thought, **Babble** disrupts speech, and **Conceal** hides mental identity/activity. These use existing power implementations with canonical Armageddon skill bindings.

Shared sustained powers use bodily costs and skill/Wisdom upkeep. The Tuluk exemption additionally requires the Lirathan class. Setting authors can configure the territory prog without making upkeep free for every character in that territory.

Mental speech (`say` and `tell`) uses the ordinary power admission checks and pays its configured invocation cost on a successful or failed skill attempt. Both verbs share the Contact skill and bodily-cost binding in this package.

## Persistence and reruns

Existing Lirathan school, powers, capability and merit keep their IDs and stable ownership keys. New records use the existing late class-content phase, after canonical skills and Shadowwalk dependencies exist. Each role's original installer remains the sole owner of its merit collection. Builder-modified fields and collections are preserved and reported rather than overwritten.

Older Sorcerer casting support traits and saved enrollment receipts remain intact. A separate role-progression merit grants the canonical Contact/Barrier/Expel skills to magic classes. Their cap expressions include this authored graph. This avoids changing the trait identities embedded in historical casting receipts.

The five new power models are `psionicprobe`, `psionicrejuvenate`, `psionicdisorient`, `psionicsensepresence` and `psionicvanish`. Effects use the ordinary saved-effect and concentration/upkeep infrastructure. No schema migration is needed.
