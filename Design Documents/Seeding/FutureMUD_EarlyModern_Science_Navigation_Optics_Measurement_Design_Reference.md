# FutureMUD Early Modern Science, Navigation, Optics, and Measurement Design Reference

## Scope

This branch makes Early Modern the first major scientific-instrument era while reusing the shared twelve-item navigation/science set.

## Shared stock to reuse

Reuse the magnetic compass, dividers, cross-staff, mariner's astrolabe, chart case, measuring chain, plane table, spectacles, magnifying lens, draw-tube telescope, balance scales, and specimen jar stable references.

## Planned slices

- microscopes, lens cases, lens blanks, optical tubes/stands, and optical-workshop stock;
- barometers, thermometers, pendulum clocks, pocket watches, hourglasses, sundials, cases, and stands;
- backstaves, quadrants, late-edge octants, parallel rulers, measuring rods, protractors, sectors, weight sets, and culture-specific instruments;
- globes, celestial globes, armillary spheres, and strictly date-bounded orrery-like devices;
- herbaria, mineral boxes, shell drawers, insect cases, nets, sample vials, cabinet drawers, labels, and specimen furniture;
- alembic/retort props, laboratory glass, medicine chests, surgical kits, and specialist balances/weights.

## Dependency contract

Brass, bronze, copper, wrought iron, spring steel, glass, glass blank, lead glass, paper, parchment, oak, leather, and common container/tool tags are live and represented in the maintained material export. Lensmaking/Lensmaker, Clockmaking/Clockmaker, Surveying/Surveyor, Navigation/Navigator, Cartography/Cartographer, and Instrument Making/Instrument Maker are live stock skills.

Most shared instruments are descriptive or container-backed. Functional navigation, observation, timekeeping, temperature, pressure, and measurement mechanics require explicit component work.

## Acceptance criteria

- No shared instrument is cloned for an era prefix.
- Dates gate late-edge octants and orrery-like devices.
- Public descriptions distinguish visible form from unsupported accuracy or sensing behaviour.
- Fixed cabinets and stands omit `Holdable`; portable instruments include it.
