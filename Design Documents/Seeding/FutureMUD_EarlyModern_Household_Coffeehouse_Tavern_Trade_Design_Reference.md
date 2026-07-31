# FutureMUD Early Modern Household, Coffeehouse, Tavern, and Trade Design Reference

## Scope

This branch owns Early Modern domestic furniture, service culture, public-house goods, and consumption-ready commodity presentation. Shared trade packages remain dependencies, not duplicates.

## Implemented foundation

`SeedEarlyModernHouseholdCoffeehouseTavernAndTrade()` now seeds 25 era-gated prototypes. The implemented set covers coffee, tea, chocolate, tobacco/snuff, punch and tavern service; dice, cards, and a score board; coffeehouse furniture and account storage; and domestic mirrors, clocks, desk, display, hearth, and lighting accessories. Service vessels use existing liquid-container components and installed furniture omits `Holdable`. Cards, clocks, mirrors, scoring, locks, and smoking remain descriptive where no matching runtime component exists.

## Covered slices

- coffee cups and pots, grinders/roasters, trays, benches, tables, account boxes, and print displays;
- teapots, tea bowls/cups, caddies, sugar bowls, strainers, and porcelain services;
- chocolate pots, cacao cups, stirring tools, punch bowls, ladles, and drinking glasses;
- pipes, pipe cases, tobacco and snuff boxes, spittoons, and regionally admitted hookah/narghile systems;
- mirrors, framed pictures, clocks, cabinets, chests of drawers, escritoires, portable desks, bookcases, display cabinets, cupboards, fire screens, snuffers, chandeliers, sconces, and lanterns;
- tankards, mugs, bottles, jugs, taps/spigots where component-safe, dice/cards, scoreboards, benches, counters, and till/account systems.

## Reuse and dependency boundary

Use the live tea chest, coffee sack, cacao sack, tobacco bale, sugar hogshead, indigo cake box, porcelain crate, bottle crate, silk bale, cotton bale, and spice chest references. Those containers do not prove that their contents or processing chains exist.

Porcelain, faience, earthenware, stoneware, glass, lead glass, oak, brass, copper, pewter, linen, leather, paper, sugarcane, cacao, cacao bean, cacao nibs, coffee bean, roasted coffee, tea leaf, tea brick, tobacco leaf, tobacco twist, snuff, molasses, chocolate block, and sugar loaf are live material foundations. Consumption-ready item prototypes and processing crafts remain branch work.

## Acceptance criteria

- A package is never duplicated solely because the contents are Early Modern.
- Installed furniture omits `Holdable`; portable service goods include it.
- Liquid containers use real capacity components and exact live liquids.
- Culture/date admission distinguishes European coffeehouse growth from earlier Ottoman coffee traditions and East Asian tea traditions.
