# Optional Psionics Seeder

The optional **Psionics** package installs Basic Psionics and Advanced Psionics after the core and supernatural dependencies. Declining `install-psionics` writes nothing. All Debug replay profiles answer this optional question explicitly.

Basic Psionics supplies contact, directed speech, barrier, self-audit and expulsion. Advanced Psionics includes those foundations, the existing advanced powers, thirteen new technique families, and four ordinary spells exposed through power verbs: projection, live possession, levitation and a caster-scoped description illusion.

The package installs two unassigned skill-level capabilities. It creates no merits, chargen choices, character assignments or automatic access hooks. Builders must grant capability access through their setting's chosen mechanism, then arrange the Psionic Discipline skill. Power unlock bands are 0, 20, 40, 60 and 80; invasive control and possession occupy the last band. The spell-knowledge progs only test existing power access.

Focus has a cap of 100 and regenerates five points per real minute while conscious, doubled while resting out of combat. `PsionicStockContent` is the canonical source for ordinary power and backing-spell costs, durations and unlock bands. Existing named definitions retain their customisation on rerun; conflicting model or school identities produce an explicit error instead of overwriting content.

`EnablePsychometricImpressions` remains false unless a builder enables it using `editstaticconfiguration`. `VNPCWitnessReportDelaySeconds` remains zero; 120 seconds is recommended where witness-forgetting gameplay is wanted. Neither setting is silently changed by this package. See [Psychic Powers, Impressions and Witness Memory](../Magic/Psychic_Powers_Impressions_and_Witness_Memory.md) for disabled-state semantics, legal evidence, temporary/permanent forgetting, and restoration commands.


## Authored power defaults and contact variants

`PsionicPowerEmotes` is the shared source for seeded emotes and newly instantiated power defaults. Private mental events stay private; visible echoes describe the caster's observable behaviour. Contact recipient echoes use `{0}` for the identity permitted by concealment and the identity prog. Caster echoes use `$1` for the contacted mind. Directed speech uses `{0}` for identity and `{1}` for message on the recipient, and `{0}` for message on the caster. Speech substitution happens once; language mode supplies its own speech output.

Basic `contact` reaches the same zone; Advanced `contact` reaches the same shard. Both use a Very Easy activation check, require a passing outcome, cost two Focus and maintain one concentration slot plus one Focus per minute. Successful connections announce their presence; failed attempts are silent to the recipient by default. `connectback last` in either school reaches `AnyConnectedMindOrConnectedTo` so an unknown incoming presence can be answered. Its distinct `disconnectback` verb avoids conflicting with ordinary contact. A distant familiar target can be selected by dub.

`PsionicsTargetKnowsIdentity` (false placeholder) and `PsionicsTargetEligible` (true placeholder) are separate editable progs, also separate from general invocation permission. The identity placeholder deliberately leaves an incoming mind unfamiliar until builders configure identity disclosure. No school access is granted.

Reruns repair recognised old generic echo placeholders field by field, separate the old shared permission prog references, and upgrade old same-location contact ranges. Custom text, non-default ranges, and custom progs remain intact. Missing connect-back definitions are installed and appended to the corresponding capability without changing existing unlocks. Existing non-placeholder prose and spell customisations remain preserved; new installations and builder-created powers use the revised defaults.

### Per-power content review

| Power | Gameplay and echo treatment |
| --- | --- |
| Contact / connect back | Distinguish arrival, failed contact and departure for both participants; conceal identity through the supported placeholder. |
| Directed speech | Name the destination to the sender; substitute permitted identity and message once for the recipient. |
| Barrier | Separate raising, lowering, blocked intrusion and breached defence; do not identify a hidden intruder. |
| Audit | An inward search; detection warning describes the searched mind using `$0`. |
| Expel | An outward surge; distinguish a broken connection from one that withstands expulsion. |
| Psychometry | Failure is an indistinct impression, not invented history; graded recorded results remain authoritative. |
| Somatic sense | Describe difficulty separating bodily sensations; successful results retain graded fatigue, pain and wounds. |
| Dreamsend | Weave imagery into the referenced sleeper's dreams; no command or identity disclosure. |
| Guard mind | Describe a watchful boundary to both willing participants; maintain protector-paid upkeep. |
| Lend / siphon | Retain bounded resource transfer and explicit transferred amount; failed flow refers to the other participant. |
| Disrupt concentration | A pulse of distraction against the referenced mind; it does not claim an effect was destroyed. |
| Forgetting | A veil over a selected memory; failures do not disclose the target's hidden witness inventory. |
| Psychic circle | Shared conversational context, explicit joining and departure, no private-thought access. |
| Psychic feedback | Taut answering pressure distinguishes the defence from a passive barrier. |
| Telekinesis | A slipping mental grasp on failure; successful manipulation retains a visible item reference and component restrictions. |
| Emotional influence | Difficulty finding purchase in another's feelings; supported emotion results remain distinct. |
| Attention suppression | Attention slides past a softened presence; no assertion of physical invisibility. |
| Delayed suggestion | A thought poised for its trigger, not an immediate command or guaranteed future activation. |
| Clairvoyance | Borrowed imagery; public casting echo describes an unfocused gaze without revealing the mental target. |
| Suggestion | A proposed thought with no obedience claim. |
| Empathy | Still concentration followed by a wound-transfer shudder; separate interruption and safety withdrawal. |
| Hex | Visible tension, private hostile pressure; finite penalty retained. |
| Psychic bolt | Visible tension rather than narration of invisible force to bystanders; victim feels the impact. |
| Trace | Failure to resolve connections; permitted results and concealment remain authoritative. |
| Prescience | Shaping a question, with staff-mediated response; no guaranteed prediction. |
| Hear | Listening along mental links, distinct from ordinary hearing. |
| Clairaudience | Sounds reaching the linked person, followed by return to one's own hearing. |
| Allspeak | Meaning emerging from unfamiliar words, then becoming strange again. |
| Magic sense | Awareness of magical presence, distinct from activity sensitivity. |
| Danger sense | Watchful anticipation, distinct warning and defensive reaction. |
| Sensitivity | Ripples of activity with supported kind/description substitutions. |
| Babble | Confused words rather than anatomical jargon. |
| Coerce | Bodily discomfort, no suggestion of obedience. |
| Project emotion | Communicated feeling; does not claim to control the recipient's emotions. |
| Projection | Inward, motionless focus and awareness loosening from the body. |
| Possession | Focus on a referenced person; intrusive bodily control and resistance described separately. |
| Levitation | Steadying oneself, followed by unseen support taking one's weight. |
| Scoped illusion | Attention to one's outline and a subjective shimmer, not a universal visual claim. |

All existing finite costs, skill bands, durations and safety bounds are retained except the explicitly described contact range/check tuning. Projection and possession remain in the final unlock band. The spell-backed adapter continues to use the backing spell's authored echoes rather than imposing a second set of generic power echoes.

Validation: all nine default suites passed (4,152 tests), followed by focused core and seeder reruns after compatibility/repair refinements. Seeder Release built successfully. A disposable-MUD rerun verified rendered contact, speech, disconnect and barrier echoes plus connect-back installation and its no-incoming-presence refusal. Unknown-presence resolution uses the existing incoming-link targeting path; the live walkthrough did not simulate a second player replying.
