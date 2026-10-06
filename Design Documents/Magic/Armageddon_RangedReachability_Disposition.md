# Ranged command reachability disposition

Source mapping only, checkpoint17; no native Musket/Artillery execution or repair is claimed. Stock Raise Servitor excludes explicit load/ready/unready/unload/fire/aim/artillery/attach/board. Builder included-clear permits nonbanned actions; included-artillery admits that dispatcher. These options remain supported. Autonomous wielded IRangedWeapon strategy and opt-in ArtilleryCrewAI are independently reachable and must be labelled separately from an ordered stock action.

| Path | Concrete callback seam requiring native proof before conditional repair |
| --- | --- |
| Musket generic load/ready/fire and attach | Load/CreateLoadingBundle publishes split/Login/ItemFinishedLoading and extraction across callbacks; Ready/Unready ignition cord state; TryInstallIgnitionStone old/new stone claims. Preserve six ignition choices, loading/cartridge/loose/powder/bore/wield/condition/rest/rifling options. |
| Artillery extended dispatcher and independent crew | Load/Ready/Unready state after output/plan callbacks; Fire invokes Ammo.Fire before releasing original containment, then current-load cleanup; chamber load splits/Login/Take before adoption. Preserve four accepted mechanisms, profile/crew/emplacement/arcs/charges/tools. |

Mapped source references: GameModule.cs112–150 and CommandableAI.cs67–137/204–233 dispatch and filters; CombatModule.cs2517–2931 generic ranged actions and3043–3318 artillery dispatcher; MusketGameItemComponent.cs815–897/968–994/1174–1188/1225–1230/1873–1892; ArtilleryPieceGameItemComponent.cs577–592/656–668/683–686/875–924/1092–1097; AmmunitionGameItemComponent.cs742/826/833; StrategyBase.cs17–25/800–842; ArtilleryCrewAI.cs39–81. Line numbers are mapping hints; fingerprints below bind the current source. An established reachable defect becomes a bounded repair, not a requirement to qualify every weapon configuration.

Current source fingerprints:

- MudSharpCore/GameItems/Components/MusketGameItemComponent.cs `c460c363e7f3debfa56f42961c2b3c3e6e12bd32cfc43068b1c89386651c3242`
- MudSharpCore/GameItems/Components/ArtilleryPieceGameItemComponent.cs `989574ee9da6451b4736476c1968094aca5feb3f7115b9fa7dbb34eab100af60`
- MudSharpCore/GameItems/Components/AmmunitionGameItemComponent.cs `5077586d2747809d8a2a9a26ee3d16f9b96f11e326832774de503c7d008c5c09`
- MudSharpCore/NPC/AI/CommandableAI.cs `9e7fcf1c10c9a1abb6430046704e147e8b3e5f064930c1d7f079938eaeb8e3b3`
- MudSharpCore/NPC/AI/ArtilleryCrewAI.cs `84f6de632410922130d8e556f196f56b6a875eb47a8f6ae4dbb6a0b532e8cdfb`
- MudSharpCore/Commands/Modules/CombatModule.cs `a2e58c364718003ee3c9dd9be337a3da2e127f13424a4e3c72992669aec559d8`
- MudSharpCore/Combat/Strategies/StrategyBase.cs `cc7693fc6974dcebbe9d80b9691e3addd8531ba7fe2062c519d141cdb67e2d65`
