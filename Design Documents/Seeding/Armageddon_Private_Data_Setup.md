# Private Armageddon worldfile input

FutureMUD contains the importer and native engine adaptations, but does not distribute ArmSolo source code, worldfiles, or the generated world corpus. The Armageddon Worldfile seeder remains a one-shot installation with no questions. The ordinary Armageddon Setting foundations do not require this private corpus.

Before running the Worldfile seeder, configure `FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY` to a private directory outside the repository containing these three files:

- `ArmageddonWorldfiles.json.gz`
- `ArmageddonWorldfiles.manifest.json`
- `ArmageddonWorldfileSpecialText.json`

For example, in the PowerShell session used to launch the seeder:

```powershell
$env:FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY = "$env:LOCALAPPDATA\FutureMUD\Armageddon"
dotnet run --project DatabaseSeeder/DatabaseSeeder.csproj
```

The same environment variable works with the published seeder. Install the documented Armageddon Setting and Animal dependencies before choosing Armageddon Worldfile. The seeder refuses missing inputs with setup guidance and validates the package hash and qualified corpus inventory before importing. It never resets an existing world.

`scripts/armageddon-worldfile-package.py --source <private ArmSolo directory> --output <private output directory>` prepares the world package and manifest without executing historical code. The separately extracted special-text file is also a private input; preserve it alongside the prepared package. The parser refuses an output directory inside this checkout. Do not put original archives, source trees, extracted text or generated exports into Git. Ignore rules are defence against accidental staging, not permission to publish the files with `git add -f`.

Public builds contain none of these files. Synthetic parser and engine tests run without them. Corpus-dependent seeder tests explicitly report skipped/inconclusive when the private inputs are absent; set the environment variable to exercise those tests locally. Test logs and artifacts must not include the private corpus.

After installation, the game server uses its database and does not need these input files. Keep backups of both the private inputs and your walkthrough database separately from the repository.
