# FutureMUD Shared Pre-Industrial Leisure Design Reference

## Scope

This shared baseline covers ordinary portable games and toys that are historically credible across Medieval, Renaissance, and Early Modern installations. It intentionally uses one cross-era form where regional presentation, markings, names, or rules can be expressed by skins or builder-authored content.

## Implemented catalogue

`SeedSharedPreIndustrialLeisureItems()` seeds twelve stable references:

- bone dice and a leather dice cup;
- nine men's morris, draughts, chess, and race-game boards;
- wooden counters in a pouch;
- spinning top, willow hoop, rag doll, carved whistle, and pull horse.

Dice use the existing `Dice_d6` component. Boards, counters, cards, scoring, whistles, and toys have no new runtime rules: play resolution, named games, gambling stakes, and signals remain social, scripted, or builder-led. The board-game surfaces are deliberately not advertised as a new game-engine subsystem.

## Admission and reuse

The catalogue is installed only when an installation selects Medieval, Renaissance, or Early Modern. Items use `Era / Pre-Industrial Era` plus the standard household-wares market path; culture, shop, school, tavern, household, and date admission remains a world-building decision. Do not create era-prefixed duplicates unless a later design needs a genuinely different component or construction.

## Boundary

This is the closure slice for the prior cross-manifest games-and-toys gap. It does not implement competitive-game scoring, gambling/economy resolution, toy animation, sound propagation, or game-specific commands. Those are runtime work, not item-seeder gaps.
