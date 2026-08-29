# Signed-language live test log

## Environment

- Local LabMUD development world, using the existing administrative test character.
- The session helper redacted login secrets in every transcript.
- The temporary local server was stopped after testing.

## Transcript index

1. [Startup smoke test](sign-language-live-labmud-smoke.log)
2. [Command and builder discovery](sign-language-live-discovery-utf8.log)
3. [Initial signed-language builder run and trait-lifecycle reproduction](sign-language-live-builder-before-fix.log)
4. [Post-lifecycle-fix command and emote run](sign-language-live-runtime-after-lifecycle-fix.log)
5. [Description-pattern builder setup](sign-language-live-description-setup.log)
6. [Description markup rendering: known, unknown, and restored](sign-language-live-description-rendering.log)
7. [Initial regional-variety workflow reproduction](sign-language-live-variety-workflow.log)
8. [Corrected regional-variety workflow](sign-language-live-variety-final.log)
9. [Anatomy enforcement and restored production](sign-language-live-anatomy-workflow.log)
10. [Final builder state, sign, signto, inline-emote, and delimiter validation](sign-language-live-final-commands.log)

## Defects found and fixed

1. Adding or removing the trait linked to a signed language did not update an already-loaded character's signed-language knowledge. Character and body trait lifecycle handling now learns and forgets linked signed languages.
2. A player emote with one unmatched backtick was accepted. The emote parser now rejects unmatched signing delimiters.
3. A signed inline emote could receive an extra trailing full stop after its closing backtick. Sentence termination now recognises a final backtick.
4. Builder-created regional varieties had no staff assignment path, so a character could not select one in play. Administrators can now use `signedlanguage grantvariety` and `signedlanguage revokevariety`.
5. The new variety workflow used incorrect wording when staff targeted themselves. Self-target output now uses `yourself` and `your`.

## Final validation

- `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` succeeded. It reported two pre-existing nullable-annotation warnings in unrelated engine files.
- `dotnet test FutureMUDLibrary Unit Tests\FutureMUDLibrary Unit Tests.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed 440/440.
- `dotnet test MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` passed 2,530/2,530.
