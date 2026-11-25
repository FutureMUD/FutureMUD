# FutureMUD

FutureMUD is a configurable engine for Roleplay Intensive MUDs. The repository contains the game server, shared libraries, database/bootstrap tools, optional installers and companion products.

Start with the [design and operations documentation](Design%20Documents/README.md). Each area has a master guide, practical workflows and separate verification/proposal records. The [engine maturity audit](Design%20Documents/Core/Engine_Maturity_Audit_20261114.md) describes supported scope and known gaps with explicit evidence limits.

## Products and development

- **Engine:** `MudSharpCore` with shared contracts in `FutureMUDLibrary` and persistence in `MudsharpDatabaseLibrary`. Use the checked-out project SDK/framework/package settings.
- **Database Seeder:** schema/bootstrap and foundational setup. Optional item content is a separate product: open `ItemSeeder.sln` for Item Seeder and `ArmageddonSeeder.sln` for the optional setting/worldfile installer.
- **Companions:** [MudClient](MudClient/DEPLOYMENT.md), [TerrainPlanner](TerrainPlanner/DEPLOYMENT.md), [Discord integration](Design%20Documents/Core/Discord_Remote_Operations.md) and the website have their own runtime/deployment boundaries.
- **Development:** `MudSharp.Development.slnf` provides the focused engine development graph. Repository tests are selected through the [test-suite guide](Design%20Documents/Core/Unit_Test_Suite_Quality_and_Maintenance.md) and paired scripts; a full solution build is not the default verification step.

The [release product manifest](FutureMUD.Web/Configuration/release-products.json) defines published product ownership. For engine introduction and licensing terms, see the [engine readme](MudSharpCore/Readme.txt). Documentation maintenance follows the [suite naming and consolidation guide](Design%20Documents/Documentation_Guide.md).
