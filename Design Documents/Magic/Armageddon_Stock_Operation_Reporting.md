# Stock spell operation reporting

At base `1ad24d636925da64e7a82ffa858b0af470e56b0a`, native prepared item creation and container filling bypassed the configured casting operation report. Heal, Dispel Magic and Detect Magick also supplied no explicit report. Effects could manifest while the durable casting receipt said `applied=false`; next-grade ordinary casting consequently could not sample mastery.

`IMagicSpellEffectApplicationOperation` is an optional extension of the existing prepared application interface. Configured casting calls its `Apply` once, attaches its returned child exactly as before, and qualifies mastery only for an intended `Applied` report. Non-reporting prepared applications retain their previous path. Legacy and production snapshot callers continue to use `Create`.

Native lifecycle item applications report successful creation only after every requested output completes creation and placement. Container fills compare actual volume before and after the existing merge operation: a declined/no-op merge does not qualify. Exceptions still escape to the existing paid-operation quarantine; partial application does not become a successful mastery sample or an automatic replay/refund.

Heal compares eligible wounds' damage around the existing native heal operation. Zero budgets and undamaged targets report `NoChange`; non-character targets report `Rejected`; non-finite observations report `Unknown`. Detect Magick reports its actual newly constructed perception child; non-character targets refuse. Dispel compares only matched, contest-eligible parents and reports actual removal or shortened scheduled duration (stable during clock countdown), including timed substance parents and existing proxy targets. It never reports success merely because a dispel was attempted.

The repair changes no spell identity, grade profile, receipt schema, opportunity timer, payment, durable recovery, device eligibility or snapshot behavior. The central completion ledger and existing historical receipts remain untouched. This checkpoint requires independent parent review before integration.

Verification: 426 focused unit cases passed on the actual repair source and Debug assemblies; 17 directly cover reporting. Source/assembly SHA-256 values and the retained TRX path are in Armageddon_Stock_Operation_Reporting_Receipt.json. Native five-stock qualification is a separate checkpoint, not claimed here.

