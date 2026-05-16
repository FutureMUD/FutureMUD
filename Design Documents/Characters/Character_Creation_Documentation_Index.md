# Character Creation Documentation Index

This is the entry point for the chargen documentation suite.

## Documents
- [Character Creation Runtime](./Character_Creation_Runtime.md)
- [Character Creation Builder Workflows](./Character_Creation_Builder_Workflows.md)
- [Character Creation Seeder](./Character_Creation_Seeder.md)

## Scope
The runtime documents describe how the in-game character creation pipeline is assembled, persisted, and customised.

The seeder document describes what `ChargenSeeder` installs, how its stock answers affect the initial storyboard graph, and what builders are expected to customise afterwards.

## Recommended Reading Order
1. Read [Character Creation Runtime](./Character_Creation_Runtime.md) to understand the stage graph, application model, and persistence flow.
2. Read [Character Creation Builder Workflows](./Character_Creation_Builder_Workflows.md) before editing storyboards in game.
3. Read [Character Creation Seeder](./Character_Creation_Seeder.md) when installing the stock chargen package or rerunning it to repair missing stock content.
