# Interrupted explosive-triggerer session

This is an evidence record rather than a raw harness transcript. The raw socket was reset immediately after the non-administrative NPC triggerer moved through the armed exit trap, before the MUD tester could flush its transcript file.

Observed sequence:

- The builder had loaded a cranial bomb on Cell #2668 and armed it with trap template #1, as recorded in [37_LabMUD_Tripwire_Explosive_Builder_And_Layer.txt](37_LabMUD_Tripwire_Explosive_Builder_And_Layer.txt).
- A loaded NPC test subject was controlled as the triggerer and sent north through the armed exit.
- The client connection reset before post-trigger output or a `look` assertion could be captured.
- The subsequent restart/recovery session found the bomb absent. The server console emitted a warning that `ExplosionHeardEcho` was unset, but did not provide a managed exception stack for the detonation.

Therefore this establishes that the exit-trigger / detonation path was reached, but it is **not** a clean end-to-end acceptance pass. No credentials or connection strings are recorded here.
