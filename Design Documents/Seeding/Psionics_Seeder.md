# Optional Psionics Seeder

The optional **Psionics** package installs Basic Psionics and Advanced Psionics after the core and supernatural dependencies. Declining `install-psionics` writes nothing. All Debug replay profiles answer this optional question explicitly.

Basic Psionics supplies contact, directed speech, barrier, self-audit and expulsion. Advanced Psionics includes those foundations, the existing advanced powers, thirteen new technique families, and four ordinary spells exposed through power verbs: projection, live possession, levitation and a caster-scoped description illusion.

The package installs two unassigned skill-level capabilities. It creates no merits, chargen choices, character assignments or automatic access hooks. Builders must grant capability access through their setting's chosen mechanism, then arrange the Psionic Discipline skill. Power unlock bands are 0, 20, 40, 60 and 80; invasive control and possession occupy the last band. The spell-knowledge progs only test existing power access.

Focus has a cap of 100 and regenerates five points per real minute while conscious, doubled while resting out of combat. `PsionicStockContent` is the canonical source for ordinary power and backing-spell costs, durations and unlock bands. Existing named definitions retain their customisation on rerun; conflicting model or school identities produce an explicit error instead of overwriting content.

`EnablePsychometricImpressions` remains false unless a builder enables it using `editstaticconfiguration`. `VNPCWitnessReportDelaySeconds` remains zero; 120 seconds is recommended where witness-forgetting gameplay is wanted. Neither setting is silently changed by this package. See [Psychic Powers, Impressions and Witness Memory](../Magic/Psychic_Powers_Impressions_and_Witness_Memory.md) for disabled-state semantics, legal evidence, temporary/permanent forgetting, and restoration commands.
