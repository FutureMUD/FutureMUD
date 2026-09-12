# Playing with Vancian magic

Your world's builder decides which spells you can select, how many castings you have and how you recover them. The examples below assume a school verb `arcane` and capability `Wizard`; substitute your actual names. Quote names containing spaces.

## Browse and choose

```text
arcane vancian Wizard status
arcane vancian Wizard spells
arcane vancian Wizard spells cantrips 0
arcane vancian Wizard spell "Spark"
arcane vancian Wizard known show
arcane vancian Wizard known begin
arcane vancian Wizard known add cantrips "Spark"
arcane vancian Wizard known draft
arcane vancian Wizard known commit
```

Browsing candidates or editing a draft grants nothing. `known remove <repertoire> <spell>` edits the draft; `known cancel` discards it. Commit replaces the complete selected repertoire and may be refused by your world's choice/cooldown policy. Reordering unchanged choices does not spend a change opportunity. A stale draft must be restarted. A failed bookkeeping hook may leave the choices committed but require staff review; status identifies the operation. Selections in other capabilities are independent.

## Prepare a plan

```text
arcane vancian Wizard loadout new "Daily"
arcane vancian Wizard loadout assign "Daily" first 1 book "Ember Bolt"
arcane vancian Wizard loadout assign "Daily" first 2 book "Ember Bolt"
arcane vancian Wizard loadout validate "Daily"
arcane vancian Wizard loadout select "Daily"
arcane vancian Wizard refresh
arcane vancian Wizard prepared
```

These are two separate memorised copies. `loadouts` lists saved plans; `loadout show`, `copy <old> <new>`, `rename <old> <new>`, `clear <name> <allowance> <ordinal>` and `delete <name>` manage them. Saving, editing, selecting or deleting a plan never refills or changes current castings. An explicit empty plan is valid; unused memorised positions remain Unassigned until the next successful refresh. There is no mid-cycle filling or swapping.

`refresh last` restores the complete last committed pattern, including spent positions, even when its saved plan was edited or deleted. The pattern must still pass current eligibility and book requirements. `cancel` stops current preparation or writing before commitment.

## Books and recovery

Books belong to item instances. Borrowed or stolen books work if you can see, manipulate and use them. Closed/inaccessible containers, configured literacy or policy restrictions may prevent use. More than one book can supply a plan, and one formula can supply several memorised copies.

EveryRefresh requires the book formulae on every preparation. PatternChangesOnly allows an identical book subpattern to be restored without the books; changing its spells, copy counts or casting levels requires the formulae for that whole changed subpattern. Reordering a plan or using a different copy of the same book does not itself change the pattern. Losing a book does not erase current prepared copies or selected cantrips.

Recovery may require a timed preparation action, observed continuous sleep, or sleep followed by preparation. Status explains your mode, duration, interval and earned credit. Unconsciousness, death, stasis, interrupted sleep and time logged out do not count. Playing an awake projection prevents another body from earning sleep credit. A blocked automatic wake refresh retains its credit; fix the reported problem and use `refresh` to retry. Cancelled preparation retains earned sleep credit. Reconnecting, changing bodies or losing/reacquiring the capability does not reset expenditure.

## Cast and upcast

```text
arcane vancian Wizard cast book first "Ember Bolt" 1 target
arcane vancian Wizard cast book first "Ember Bolt" next target
arcane vancian Wizard cast cantrips cantrip "Spark" atwill target
```

Use an ordinal for an exact prepared copy or spontaneous slot. `next` selects the next matching unspent finite position within that named allowance. `atwill` is only for an at-will allowance. Follow it with the normal spell's target/additional arguments. Power comes from that route; you do not supply an extra power word.

At matching level the default power is Standard. Each excess casting level adds one power step, capped at RecklesslyPowerful. A permitted cantrip prepared in a higher finite slot spends that slot and gets its upcast power. Subsequent at-will casts retain their ordinary power. At-will removes only finite-slot expenditure: normal spell resources, materials, restrictions and delays still apply.

A refused target or preflight does not spend the casting. Once committed, resistance, wards or failure on some group members do not refund it. Other group members are still processed. Your old school `cast` route cannot bypass a Vancian-only casting budget; a separately granted legacy casting route remains usable.

## Copy and make scrolls

```text
spellbook show "borrowed grimoire"
spellbook copy Wizard "borrowed grimoire" "Ember Bolt" "my grimoire"
spellbook copy Wizard "ancient scroll" "Ember Bolt" "my grimoire"
spellscroll inscribe Wizard book first "Ember Bolt" 1 "blank scroll"
spellscroll inscribe Wizard cantrips cantrip "Spark" atwill "blank scroll"
spellscroll show "charged scroll"
spellscroll cast "charged scroll" Wizard target
```

Copying takes the destination book's configured time/materials and adds one base formula. A source book is preserved; a successfully transcribed scroll is destroyed without casting its spell. Learning an eligible higher-level formula does not require a slot or an activation check. Duplicate/full destinations refuse before payment.

Inscription reserves a blank scroll and the chosen casting while you work. Completion prepays that casting plus spell and production costs; it does not release damage, healing, conjuration or other spell effects. Interruption before commitment releases the reservation without expenditure. Successful production creates one transferable charge, with no hidden expiry or stockpile cap.

A compatible caster may release an unknown/unprepared scroll even when all personal slots are spent. It uses its stored creator potency with the reader's targeting and ownership. Changing the creator's stats or logging them out does not weaken or strengthen it. The scroll's stored casting level determines whether the reader needs an over-level control check. Inspecting it never casts it. After commitment the scroll is destroyed even if that check fails, a target resists or a ward blocks the spell. Release pays no second slot, materials or resource cost. A revoked or invalid stored spell remains inert for staff inspection.
