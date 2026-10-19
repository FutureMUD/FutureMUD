# Armageddon physical impacts

Fresh Armageddon Combat installs separate Bash and Kick auxiliaries and binds the existing manual verbs to them. Their explicit character skills, Counter, Guarding and Agility references are saved in a reusable `PhysicalImpact` XML profile. Ordinary auxiliaries retain their existing resolution path. Clone a configured action and use `impact <field> <value>` to change its profile; `impact none` restores ordinary auxiliary resolution.

Recovered source behaviour informs admission, size/agility advantage, trained counters, Bash miss/hit seating and victim delay, Kick shield contact and untrained use, guard interception, flight/physical-plane handling and skill-scaled stamina. Bash requires training except against a paralysed target. Kick requires a usable leg. Both reject mounted participants. A strong same-skill counter seats the attacker; Bash can also damage the attacker. Kick does not seat or delay its victim.

The numeric FutureMUD damage/stun expressions, heavy exertion, base delays and shield equipment integration are authored adaptations. They are independently editable; they are not presented as recovered source damage equations. Held armour shields use native armour absorption and condition damage; remaining damage reaches the body once. Native magic defence receives crushing physical damage and retains its transformed remainder. Full absorption prevents Bash seating/delay.

An impact captures actor and target bodies, embodiment, instance, room/layer and combat context. Revalidation surrounds admission, skill/prog/defence callbacks, guard engagement and wound mutation. A replacement body cannot execute or pay for an old impact. Guard engagement has its own independent authority continuation, including effect applicability. Pre-attempt rejection spends no stamina; an admitted attempt consumes its unseen opening and retains its captured-body cost even if a later callback invalidates continuation.

Focused regression receipt `20261019T150500Z-f27a5f5162a5`:314 passed,0 failed/skipped, stable source. Combined native gameplay remains a separate acceptance requirement. Incomplete heritage physical profiles remain unavailable.
