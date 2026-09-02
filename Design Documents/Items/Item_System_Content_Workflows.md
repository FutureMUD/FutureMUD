# FutureMUD Item System Content Workflows

The modern ranged seeder is rerunnable and setting-neutral; its catalogue and live verification workflow are documented in [Ranged Weapon Balance Pass](../Combat/Ranged_Weapon_Balance_Pass.md).

For modular modern firearms, author the host's slots and fire modes before creating attachment item prototypes. Match slot and attachment form-factor strings exactly apart from case, and compose active accessories from the `FirearmAttachment` profile plus the existing functional component. Use the attachment component's `require add <capability> [reason]` command to declare required sibling capabilities. Builders may assemble components in either order, but `item show`, attach, and detach report incomplete composition, and submission or approval is blocked until all requirements are satisfied. The combat seeder's pump shotgun, select-fire rifle, composed bayonet, underbarrel grenade launcher, weapon light, and impact grenade round are intentionally examples rather than a comprehensive catalogue. See [Modern Firearms, Attachments, and Alternate Fire Modes](../Combat/Modern_Firearms_and_Attachments.md).

## Scope
This document explains the builder-facing workflows developers use when creating, revising, attaching, loading, and validating item content.

The focus is not general builder onboarding. The focus is the minimum command surface developers need when implementing or testing item-system changes.

## Artillery and weapon-carrier workflow

Use `comp edit new artillery`, `artilleryammo`, `artillerychamber`, `artillerymount`, or `weaponcarrier` to author the corresponding component. Artillery pieces require an artillery ranged type, profile, loading mechanism, crew range, emplacement setting, and arcs; ammunition and chambers use matching profile strings (a `|`-separated list is valid for a deliberately compatible family). At runtime, use `artillery <piece> join [role]`, then the documented loading drill, `aim` or `aimpath`, and `fire`; use `emplace` / `artillery ... limber` and `mount` / `unmount` to manage physical installation.

Author the piece's `powder` and `primer` masses and its `spongetag`, `wadtag`, `rammertag`, `venttag`, `linstocktag`, and `fusetag` before approval. The stock catalogue uses `Artillery Sponge`, `Artillery Wadding`, `Artillery Rammer`, `Artillery Vent Tool`, `Artillery Linstock`, and `Artillery Fuse`. A full drill needs a real gunpowder commodity with enough mass, all appropriate tagged tools/consumables, and an `ArtilleryAmmunition` item (or a compatible preloaded `ArtilleryChamber`). Shell and carcass rounds must receive a physical fuse with `artillery <piece> fuse <seconds>` before readying.

For muskets, stock `Musket Cleaning Rod`, `Musket Ramrod`, `Musket Unjamming Tool`, `Match Cord`, and `Musket Wadding` tags alongside a gunpowder commodity and typed musket balls or cartridges. A cleaning rod or ramrod intended to ride on the firearm also needs `Beltable`; use `attach <rod> <musket>` to install it. The loading drill preferentially uses the attached tool and restores it to that same musket after cleaning or tamping. `load <musket> blank` consumes powder and a wad but no projectile; `noclean` and `tapnoclean` deliberately preserve the dirty-load penalty. A musket cannot be readied until every selected loading phase has completed. Validate dry, heavy-rain, underwater, liquid-atmosphere, and vacuum cases when shipping a black-powder profile.

On an established database, the Items seeder offers a `blackpowder` repair scope in addition to the normal `all` scope. The focused scope reconciles only the shared physical ammunition and tools, selected-era firearms and artillery, and the physical black-powder craft. It deliberately skips retirement of unrelated managed catalogue records, so it is safe to use when repairing this dependency slice without waiting for the complete multi-era catalogue pass. For older stock that has subsequently been customized, the focused migration adds only missing functional tags and preserves all other item fields and relationships.

Weapon carriers use `attach` and `detach` to establish or remove the relation and ordinary `sheathe` / `draw` to stow or retrieve an attached weapon. A retained dropped weapon is hanging, so `recover <weapon>` returns it to inventory and `release <weapon>` drops it deliberately.

## Working with Component Prototypes

## Media device composition

Use `comp typehelp` before authoring `Camera`, `Push To Talk Microphone`, `Media Monitor`, `Media Speaker`, `Computer Media Interface`, `Media Deck`, `Media Storage Medium`, `Media Cable`, or `Media Splitter`. Configure matching format keys/capabilities and normal powered-machine settings. A camera’s snapshot interval cannot be below five seconds. Connect local ports with ordinary connector workflows; add a splitter for fan-out. For a TV/VCR or boombox, attach compatible sibling components to one item and enable sibling input acceptance where required.

Use ordinary container commands to insert/remove physical media, then use `media <item> status`, `media <monitor|speaker> volume <volume>`, `media <medium> list`, `media <deck> record <name>`, `media <deck> play <name>`, `media <deck> stop`, and `media <medium> erase <name>`. Monitor and speaker prototypes author the default output volume; players can scale or mute an individual device at runtime. Test duplicate names, write protection, capacity, incompatible formats/capabilities, loud adjacent-cell propagation, and feedback-loop collapse before submitting a composite item. The UsefulSeeder supplies components only, so builders remain responsible for game-specific finished device prototypes.
### Discover available component types
Use:
- `comp types`
- `comp typehelp <type>`

This is the fastest way to verify:
- the registration worked
- the builder-facing type keyword is correct
- the help text is useful

`comp types` respects the `ShowModernItemComponentTypes` and `ShowFuturisticItemComponentTypes` static settings. Both settings default to `true`; setting one to `false` hides its tagged entries from this discovery list without disabling creation, loading, use, or direct `comp typehelp` lookup for those component types.

### Create a new component prototype
Use:
- `comp edit new <type>`

Telecommunications examples include:
- `comp edit new telephone`
- `comp edit new telecommunicationsgridoutlet`
- `comp edit new telecommunicationsgridfeeder`
- `comp edit new cellularphone`
- `comp edit new cellphonetower`
- `comp edit new implanttelephone`
- `comp edit new mediastoragemedium`
- `comp edit new answeringmachine`

Signal-automation examples now include:
- `comp edit new computerhost`
- `comp edit new computerterminal`
- `comp edit new computerstorage`
- `comp edit new networkadapter`
- `comp edit new networkswitch`
- `comp edit new wirelessmodem`
- `comp edit new pushbutton`
- `comp edit new toggleswitch`
- `comp edit new motionsensor`
- `comp edit new lightsensor`
- `comp edit new rainsensor`
- `comp edit new temperaturesensor`
- `comp edit new timersensor`
- `comp edit new keypad`
- `comp edit new filesignalgenerator`
- `comp edit new microcontroller`
- `comp edit new automationmounthost`
- `comp edit new signalcable`
- `comp edit new signallight`
- `comp edit new electronicdoor`
- `comp edit new electroniclock`
- `comp edit new relayswitch`
- `comp edit new alarmsiren`

Explosive-trigger examples include:

- `comp edit new countdowndetonator`
- `comp edit new clockdetonator`
- `comp edit new signaldetonator`
- `comp edit new pinpulldetonator`

Each of these must be attached to an item prototype with a sibling `Bomb` component before the item can be submitted. Countdown, clock, signal, and radio detonators are mutually exclusive armable roles; the pin-pull role is separate.

Power, telecom, and modern medical examples also include:
- `comp edit new electricgridoutlet`
- `comp edit new gridpowersupply`
- `comp edit new unlimitedgenerator`
- `comp edit new gascontainer`
- `comp edit new rebreather`
- `comp edit new externalinhaler`
- `comp edit new inhalergascanister`
- `comp edit new integratedinhaler`
- `comp edit new defibrillator`
- `comp edit new externalorgan`

Ritual-offering examples include:
- `comp edit new incenseburner`
- `comp edit new offeringreceiver`

`UsefulSeeder` now ships stock component examples across those modern families, including lithium batteries, cellular devices, answering-machine audio media, computer/network gear, signal automation, gas containers, rebreathers, inhalers, defibrillators, and external-organ support machines. Its general item package also includes a broad furniture-container catalogue for tables, shelves, bookcases, racks, stands, cabinets, wardrobes, trunks, drawers, counters, bins, beds and display cases, size-labelled door, gate, glass-door, and locking-door component presets for door items intended to match exit `DoorSize` values from `Tiny` through `Gigantic`, contextual latch presets for containers, doors, gates, and portcullis-style barriers, public infinite water-source presets for taps, drinking fountains, pumps, standpipes, troughs, and communal cisterns, and Skill-Package-aware `WornTraitChanger` presets for worn stealth, movement, dexterity, sight, hearing, and athletic bonuses or penalties; the item prototype size itself must still be set on the item. Food presets now have two stock surfaces: `CookingSeeder` installs general `PreparedFood` examples for direct load, forageable stock, stackable servings, and cooking recipe products, while the antiquity food pass keeps its prepared-food prototypes, liquids, vessels, tools, commodity tags, spoilage rules, and crafts in `ItemSeeder` partials so it can consume `AnimalButcherySeeder` outputs without duplicating them. The primary-production historic item pass now seeds portable tool and carrying-aid prototypes with ordinary `Holdable`, `Destroyable_*`, and `Container_*` components, while visible resource deposits and static apparatus deliberately omit `Holdable` so builders can place them as fixed/non-gettable site markers. `AnimalButcherySeeder` installs raw butchery output item prototypes for stock animal and beast-only mythical race profiles. Those outputs are non-edible props by default, use simple held, destroyable, stackable composition, and are tagged for raw meat cuts, raw hides, offal, trophies, venom organs, and crafting animal products. Soft organic outputs morph into the generic rotten-meat prototype, while durable hard products such as bone, tusk, horn, chitin, shell, feather, scale, tooth, beak, claw, and antler remain stable crafting materials. Fax-machine examples and breathing-filter cartridge ecosystems remain later dedicated content passes.

`UsefulSeeder` also ships antiquity ritual examples: `IncenseBurner_Antiquity_BronzeCenser`; the item-only `OfferingReceiver_Antiquity_HouseholdAltar`, `OfferingReceiver_Antiquity_VotiveBasin`, and `OfferingReceiver_Antiquity_FuneralTray`; and four liquid-enabled profiles for a temple libation table, oil-lamp shrine, oracular tripod, and blood-offering bowl. Pair the censer with items tagged `Functions / Household Items / Household Religious Items / Incense Fuel`; the seeded `antiquity_resin_incense_pellets` item is the stock test fuel. Liquid examples use `Materials / Liquids / Ritual Offerings` descendants for libations, lamp oil, and blood offerings.

Prepared-food examples include:
- `comp edit new preparedfood`
- direct-load profiles for apples, berries, mushrooms, and muffins
- stackable `per-stack-unit` serving profiles for berries and mushrooms
- recipe target profiles for `CookedFoodProduct`

See [Food And Cooking System](../Crafting/Food_and_Cooking_System.md) for the runtime model and builder-facing template tokens.

This goes through `GameItemComponentManager`, so failure here often means:
- the type was not registered
- the builder loader name is wrong
- the template output was not fully integrated into the codebase

### Edit, submit, review, and obsolete component prototypes
Key workflows:
- `comp edit <id>`
- `comp set ...`
- `comp edit submit`
- `comp review ...`
- `comp edit obsolete`

Because component prototypes are revisioned content, do not think of them as raw code-only artifacts. They are also builder-managed content definitions.

## Working with Item Prototypes
### Create or edit an item prototype
Use:
- `item edit new`
- `item edit <id|unique name>`
- `item clone <id|unique name> [<new name>]`

Item prototype lookup prefers numeric ids, then exact `UniqueName`, then the legacy noun/name matching that older builder commands expect. Unique names are optional, case-insensitively unique among active revisions, and cannot be entirely numeric.

Cloning without a final argument creates a valid numbered `copy` noun; supplying `<new name>` validates the same trimmed, non-numeric, active-name namespace used by `item rename` before a clone is persisted.

### Bulk rename item prototype names
Use:
- `item rename <match regex> <replacement text>`
- `item rename "^old_(?<name>.+)$" "new_${name}"`

The match expression is case-sensitive by default and operates on the prototype `Name` (its noun), for `Current`, `PendingRevision`, and `UnderDesign` revisions. Standard .NET numbered and named replacement groups are supported; inline regex options such as `(?i)` can opt into case-insensitive matching. Quote either argument when it contains spaces. Names are trimmed and cannot be blank or entirely numeric.

The command first displays the complete proposed old-to-new map. It checks the virtual final state against untouched active prototypes as well as other entries in the batch, so swaps and rename chains are safe. If the expression is invalid or times out, a result is blank or entirely numeric, or distinct prototype IDs would share a case-insensitive name, no prototype is changed. A successful preflight applies all actual changes immediately and marks those revisions for normal persistence.

### Bulk rename item unique names
Use `item renameunique <match regex> <replacement text>` for the optional stable `UniqueName` lookup key. Its case-sensitive matching, active-revision scope, empty-replacement clearing, and final-state collision validation remain unchanged.

Stock rework items seeded by `ItemSeeder` use their stable seeder reference as `UniqueName`. Stock-only metadata such as the stable reference, culture context, source profile, and package notes remains in the seeder definitions and maintained manifest rather than being copied into builder comments. Reruns remove previously seeded note lines while preserving unrelated builder-authored comments. Stable references identify the era, domain, optional culture, and product; they do not include implementation provenance such as `expansion`, `rework`, `pass`, or `content_pass`, and must not repeat an adjacent package/category segment (for example, use `medieval_door_east_asian_bamboo_lattice_gate`, not `medieval_door_east_asian_east_asian_bamboo_lattice_gate`). Keep numeric suffixes only where they distinguish genuine product variants.

### Wildlife shelter anchors
`WildlifeCatalogueSeeder` owns the tagged anchor prototypes and input-free crafts for burrows, dens, ground nests, tree nests/roosts, lairs, lodges, and web nests. They are runtime shelter infrastructure, not decorative examples. Run the wildlife package after its Animal foundations, attach the recommended `Wildlife - ...` or `Managed Animal - ...` AI to an NPC template, and let the configured animal create or claim the matching anchor. The package repairs its untouched stock rows on rerun; clone a prototype or craft before intentional customisation. Do not remove the `Wildlife Shelter` and subtype tags from a clone, because the ownership policy uses them to prevent arbitrary item claims.

Stock tag lists persist only the most-specific tag from any single hierarchy branch. A leaf such as `Market / Military Goods / Weapons / Spears` already implies its `Weapons` and `Military Goods` ancestors; independent leaves from other branches, such as era and function, remain attached.

The shared pre-industrial rework package uses `preindustrial_*` unique names for selected cross-era aliases and new stock. It never renames the source `antiquity_*` or `medieval_*` prototype. Alias source references remain in the seeder definition rather than the persisted builder comment, while public descriptions remain in-world and do not mention aliases or seeder mechanics. Builders selecting Antiquity, Medieval, Renaissance, or Early Modern receive the same `historic_*`, `primary_production_*`, and shared pre-industrial foundation without duplicate prototypes on rerun.

### Attach or detach components
Use:
- `item set add <id|name>`
- `item set remove <id|name>`

This is the key step that turns a generic item prototype into something functional.

The attach and submit workflow enforces prototype-side capability exclusivity. If two attached component protos both advertise the same exclusive marker, such as `IContainerPrototype` or `IWearablePrototype`, the item prototype cannot be submitted until the duplicate role is removed. Aggregate markers such as connection, power, signal-source, and worn trait-change roles can still appear more than once where the runtime intentionally aggregates them.

### Important item settings for developers
Commonly relevant commands include:
- `item set noun`
- `item set sdesc`
- `item set ldesc`
- `item set desc`
- `item set suggestdesc`
- `item set unique <name>`
- `item set unique clear`
- `item set comment <text>`
- `item set comment append [<text>]`
- `item set comment edit`
- `item set comment clear`
- `item set size`
- `item set weight`
- `item set material`
- `item set quality`
- `item set cost`
- `item set group`
- `item set strategy`
- `item set morph`
- `item set destroyed`
- `item set onload`
- `item set register`
- `item set canskin`
- `item set hidden`
- `item set preserve`

These matter because many component features only make sense in combination with item-level settings. For example:
- item groups affect room presentation
- morph and destroyed settings affect component replacement behaviour
- registers and on-load progs can feed variable-style components
- skinnability affects how content can be customised by players

### Explosive trigger runtime workflow

Use `arm <item> [<duration|in-game datetime>] [(<emote>)]` for countdown, clock, signal, and radio triggers. A countdown uses its authored default when no duration is supplied and accepts a player duration only when enabled; a clock trigger requires an exact future in-game date and time interpreted in the component's authored timezone; a signal or radio trigger takes no trigger argument. Use `disarm <item> [(<emote>)]` only when the component profile permits it. Existing `switch <radio detonator> on|off` aliases remain available for compatibility.

Use `pullpin <item> [(<emote>)]` (or `pinpull`) for a pin-pull trigger. This begins its authored fixed delay and cannot be reversed. Countdown and pin-pull deadlines continue against UTC while unloaded or during server downtime and fire on the next load if overdue. Clock triggers follow their authored in-game calendar and clock instead.

For a signal detonator, author a default source component and endpoint, then use the ordinary `electrical` inspection and binding workflow when a live item needs rewiring. A live binding to a specific source item remains tied to that item and becomes inactive when the source is disconnected or moved out of signal range; it never substitutes another item merely because the component prototype matches. Test both inactive and active starting states, source disconnection and reconnection, power loss and restoration when power is required, threshold direction, and the selected activation policy. Edge mode must require a fresh inactive-to-active transition after arming or repowering; level mode may detonate as soon as an active level is observed. A microcontroller can provide arbitrary electronic logic now, and later mechanical trap sources can use the same endpoint contract.
- unique names give builders, scripts, and content references a stable lookup key that is not the item noun
- builder comments preserve design intent, associations, and maintenance notes without changing runtime behaviour

### Listing and searching item prototypes
`item list` includes the unique name column. The `+<text>` and `-<text>` filters match ordinary keywords and noun/name text, and also search unique names and builder comments. Use `comment:<text>` to search builder comments directly.

## Loading and Testing Live Items
### Manual load workflow
Use:
- `item load [<quantity>] <id|unique name> [<extra args>]`

This is the normal developer validation path for checking whether a prototype, its components, and its runtime behaviour work together.

For commodity piles, use the staff command `loadcommodity <weight> <material> [<tag>|tag <tag>] [characteristic <definition> <value>]...`. The optional characteristic clauses create commodity-owned `IHaveCharacteristics` values, such as a colour on a fabric bolt, without requiring a bespoke item prototype.

Commodity piles can also be governed by builder-owned `commodityspoilage` rules. Use `commodityspoilage list`, `show`, `new`, `clone`, `edit`, `close`, and `set` to author rules that match one exact material or one material tag, optionally narrow to a pile tag, and output a replacement material and optional pile tag after a configured duration. This is the preferred content workflow for perishable commodity stock such as raw meat, prepared meat, salted meat, dried meat, smoked meat, or broth bases.

### Outfit template workflow
Outfit templates are admin-only global editable data for creating full equipment sets from item prototypes. They are not revisable: changing a template only affects future loads, not any character outfits it has already created.

An outfit item can bind one current skin to its prototype entry, allowing civic, institutional, ceremonial, and mourning presentation without duplicate behaviour prototypes. Institutional admission text remains builder policy for shops, NPCs, chargen, and FutureProg callers; it does not imply a universal eligibility subsystem.

The ItemSeeder also installs documented stock clothing manifests for selected eras after their item phases finish. Stock templates use concise deterministic names, stable prototype references as template-local keys, document order as wear order, and default-profile `Worn` placement. Their builder-facing descriptions retain useful grouping and purpose context but omit source filenames. A stock ownership marker is kept in the description so reruns can reconcile seeder-owned templates without silently overwriting an unrelated builder-authored template with the same name; an unmarked name collision stops the seed operation for builder review. Renaissance military armour manifests use the same ownership and update path, but are generated from their canonical military table and contain only wearable armour or barding. They validate every reference and wearable profile before generation, so shields and handheld weapons remain separately equipped rather than being incorrectly loaded as worn items.

Use:
- `outfittemplate list [<filters>]`
- `outfittemplate show <id|name>`
- `outfittemplate new <name>`
- `outfittemplate clone <old> <new>`
- `outfittemplate edit <id|name>`
- `outfittemplate close`
- `outfittemplate set ...`
- `outfittemplate load <template> <target> [<outfit name>] [args <load args>]`

Important setters:
- `outfittemplate set name <name>`
- `outfittemplate set description <description>`
- `outfittemplate set exclusivity none|below|all`
- `outfittemplate set item add <key> <prototype> [worn [profile]|wielded|inventory|room|container <key>|attached [belt key]|sheathed [sheath key]] [args <load args>]`
- `outfittemplate set item remove <key>`
- `outfittemplate set item key <old> <new>`
- `outfittemplate set item proto <key> <prototype>`
- `outfittemplate set item placement <key> worn [profile]|wielded|inventory|room|container <key>|attached [belt key]|sheathed [sheath key]`
- `outfittemplate set item args <key> <load args|clear>`
- `outfittemplate set item swap <key1> <key2>`

- outfittemplate set item skin <key> <skin|clear>

Template item keys are stable within the template and are used for container, belt, and sheath references. Prototypes must be current and manually loadable. Worn entries must use wearable prototypes; if no wear profile is specified, the created item uses its default wear profile at load time. Wielded entries must have a wieldable component and are readied directly rather than first passing through the held-item size checks, which is important for pikes and other oversized two-handed weapons. Container entries must point at another template item whose prototype has container capability, and container references must not be cyclic. Attached and sheathed entries may name a template-local belt or sheath key, or omit the key to use the first suitable belt or sheath already available on the target after earlier template placements.

The item skin setter accepts only a current skin that targets the entry's selected prototype; clear removes it. Template display shows the chosen skin, and load preflight rejects a missing, stale, incompatible, or unavailable one before creating any item.

The manual `outfittemplate load` command materialises a template directly onto a character without requiring a FutureProg. The optional `args <load args>` tail is appended to every template item's own load arguments, which lets admins apply common variable values such as colour across a whole outfit load.

NPC templates can reference outfit templates as load-time additions. When a new NPC is created from the template, the outfit template materialises before the NPC template `OnLoadProg`, so scripts and AI can see the created gear. The NPC template stores only the outfit template id and optional created outfit name; item prototype validation and safe placement remain owned by the outfit template materializer.

An on-load FutureProg can pre-contaminate materialised items with `exposetoliquid(item, liquidId, volume)`. The volume may be either a number in base fluid units or text with an explicit fluid-volume unit, such as `"25 ml"`. The function applies the specified volume from above through the normal surface-liquid system, so material absorbency, saturation descriptions, drying, residues, cleaning and persistence remain authoritative. It returns false for a null item, unknown liquid id, invalid unit expression or non-positive volume. This is intended for generated state such as blood-splattered NPC clothing; it does not fill liquid containers or bypass ordinary contamination behaviour.

NPC templates can also create installed implants and prosthetics as load-time body equipment. These entries store template-local keys, current item prototype references, optional target bodyparts, and optional implant power or neural-link keys. At load time the engine creates real item instances, verifies that the created item exposes the expected implant or prosthetic component and is compatible with the NPC body, installs it through the normal body methods, then applies any power or neural links that resolve to installed implants.

### Extra arguments
The builder-facing load flow supports:
- skin selection
- register variable overrides
- quantity for stackable content

Developers should use these to validate:
- variable-driven item behaviour
- stackable behaviour
- skin overrides

For deliberate poison coatings, validate both command routes against real held melee weapons and ammunition:
- `apply <source> to <weapon> [volume <amount>]`
- `apply <source> to <ammo> [count <number|all>] [volume <amount>]`
- `dip <weapon|ammo> in <source> [count <number|all>]`

The source must be an open liquid container with a liquid whose drug supports the `Touched` or `Injected` vector. Failed `apply` checks waste the attempted dose and can contaminate the handling bodypart with touched-vector drugs; `dip` is easier but only causes accidental contact on major failure. Builders should confirm that ordinary spilled liquid contamination still cleans, dries, and describes as before, but does not create `WeaponPoisonCoating` and therefore does not deliver through combat.

For combat validation, check a melee hit and an ammunition hit with the same coated liquid. Piercing, armour-piercing, and ballistic-armour-piercing damage use the default full injected multiplier; slashing, chopping, shearing, bite, claw, ballistic, and shrapnel use the default partial multiplier; all other damage types default to no injected delivery. The exact capacity fractions, dose per hit, delivery clamps, dip difficulty adjustment, damage multipliers, wound-severity multipliers, and wound-nature multipliers are all static configuration entries named with the `WeaponPoison...` prefix.

For telecommunications content, also validate:
- whether the phone number belongs to the handset or the connected endpoint
- whether the device gets power from the correct source
- whether off-hook and busy-line behaviour match the intended call flow
- whether shared-number endpoints ring together and let later pickups join the live call
- whether fax machines can send to other fax numbers, reject or garble voice-only destinations, and expose the intended player command surface
- whether inbound faxes queue correctly when the machine has no paper or ink, and then flush from memory once supplies are restored
- whether cellular handsets only work when a powered cell tower on the same telecom grid covers the current zone
- whether implant telephones are linked to a neural interface, draw implant power correctly, and still obey the same cell-tower coverage rules as other cellular devices
- whether a chained answering machine lets downstream handsets ring first, then answers after its configured ring count
- whether the telecommunications grid creator enables hosted voicemail with the expected access number and keeps that reserved number unavailable for ordinary subscribers
- whether a custom greeting plays back with preserved language metadata and timing before the beep
- whether caller speech is recorded onto the inserted audio-capable medium, and whether medium full or write-protect only blocks recording rather than the basic answer flow
- whether an unanswered hosted-voicemail-enabled line routes to the exchange mailbox only after local answering-machine opportunities have passed
- whether dialling the hosted voicemail access number from the subscribed line announces mailbox counts, plays message contents, and honours keypad deletion commands
- whether `dial <phone> <digits>` starts a call while idle and sends keypad digits once the call is already connected
- whether keypad-driven targets receive `TelephoneDigitsReceived` with the expected source item and digit string

For incense and offering content, validate:
- ordinary `put` and `take` against the component's transparent container behaviour
- `put <fuel> in <censer>`, `light <censer>`, source-room LOOK scent text, adjacent-room distant scent text, and `extinguish <censer>`
- `tracks` smell discoveries for ambient scent, confirming that scent trails are listed separately from movement tracks
- optional inhaled drug dosing only when the incense burner prototype explicitly configures a drug, dose, pulse, and drug range
- `offer <item> at <focus>` for accepted and rejected offerings
- `burn <item> at <focus>` for contained offerings, including default consumption and optional residue prototype replacement
- `CanOfferProg`, `OnOfferProg`, and `OnBurnProg` with `(Character actor, Item focus, Item offering)`
- `OfferingReceived`, `OfferingReceivedWitness`, `OfferingBurned`, and `OfferingBurnedWitness` event dispatch for downstream customisation
- `libate <amount|all> from <open held container> at <focus> [(emote)]` for accepted/rejected mixtures, volume bounds, actual source consumption, summary persistence, and copy reset
- `CanOfferLiquidProg`, `WhyCannotOfferLiquidProg`, `OnOfferLiquidProg`, and `OracleResponseProg` with `(Character actor, Item focus, Item source, LiquidMixture liquid, Number amount)`
- blocked-liquid precedence, unrestricted empty allow lists, and descendant-tag admission for every constituent liquid in a mixture
- `LiquidOfferingReceived` and `LiquidOfferingReceivedWitness` event dispatch exactly once after successful consumption

For the current signal-automation slice, also validate:
- whether `computerhost` correctly tracks its powered state, mounted storage devices, terminal connections, network adapters, files, and stored executables
- whether `computerhost` exposes the expected built-in applications through `programming apps` and whether the host reports those applications consistently even when a mounted storage device is selected as the current mutable programming owner
- whether `computerstorage` mounts into the intended host and exposes the expected files and executables through that host
- whether `computerterminal` only allows sessions when switched on, powered, and connected to a powered host
- whether `networkadapter` reports the expected connected host, attached telecommunications grid, canonical published address, stable device id, access routes, and current network-ready state
- whether `networkswitch` correctly passes telecommunications access through one uplink and the expected number of downstream ports, including daisy-chaining to another switch without losing transport state
- whether `wirelessmodem` joins the expected telecommunications grid through powered `cellphonetower` coverage, loses network transport when coverage is removed, and reports the same address, device-id, and access-route state as a wired adapter
- whether source and sink bindings resolve to the intended local sibling components and endpoint keys on the same item
- whether `pushbutton` emits the expected value and then returns to zero after its authored duration
- whether `toggleswitch` changes between its authored on and off values through the normal switch flow
- whether `motionsensor` reacts only to its configured witnessed movement mode and minimum size, then returns to zero after its authored duration and only while powered
- whether `lightsensor` emits the current ambient illumination while powered and updates when the room lighting changes
- whether `rainsensor` emits the expected rain-intensity scale outdoors or climate-exposed and returns to zero while sheltered indoors
- whether `temperaturesensor` emits the current ambient temperature in Celsius while powered and updates when the room temperature changes
- whether `timersensor` alternates between its authored active and inactive values on the expected schedule and only emits live signal while powered
- whether `keypad` only emits its configured signal value after the correct numeric code is entered, and otherwise reports no activation
- whether `filesignalgenerator` creates its designated backing file, parses valid numeric contents into live signal output, drops back to zero on invalid or empty contents, and reports its current file status clearly through `electrical` and `programming`
- whether `microcontroller` input bindings compile successfully after every `input add`, `input remove`, or `logic` change
- whether `automationmounthost` bay names, mount types, optional `AutomationHousing` access requirements, and mounted-power expectations match the intended content composition
- whether `signalcable` successfully mirrors a source across the intended one-room exit hop and clears correctly when unrouted
- whether `signallight` responds to threshold and invert settings without redundant extra echoes when the effective lit state is unchanged
- whether `electronicdoor` responds to threshold and invert settings and reaches the commanded open or closed state once any lock conditions permit it
- whether `electroniclock` responds to threshold and invert settings and correctly drives the underlying lock state
- whether `relayswitch` correctly opens or closes its relay according to its configured activation condition and only provides power while effectively closed
- whether `alarmsiren` only sounds while switched on, powered, and above its effective activation condition
- whether the live `electrical` and `programming` verbs target the intended parent items and components through normal keyword targeting, without relying on raw component ids
- whether `programming item <item> file [<component>]`, `file edit`, `file write`, and `file public on|off` can inspect and mutate the backing file on a live `filesignalgenerator`
- whether live `electrical` inspection clearly shows controller input bindings, cable mirror routes, current values, and resolved versus broken signal links so end-to-end debugging is practical in game
- whether live `electrical` inspection for a `filesignalgenerator` also shows the designated signal file, parse status, and whether the file is currently published for anonymous remote access
- whether live `electrical` inspection also surfaces nearby routed cable segments clearly enough that builders can see which cable mirrors which source and through which exit hop
- whether `Directory` and `SysMon` show device ids and access-route summaries clearly enough that builders can tell why a host is reachable, private, or exchange-local
- whether public-network discovery excludes exchange-private or VPN-only devices unless the querying host shares the required subnet or VPN membership
- whether an exchange-local private subnet behaves like an isolated field network at that exchange, with devices visible to each other but hidden from unrelated public hosts elsewhere on the linked-grid graph
- whether authenticated VPN tunnels add only the expected temporary route memberships to the active terminal session, exposing private hosts there without changing hardware discovery for other sessions
- whether service access is correctly blocked by a closed `AutomationHousing` item around a mount bay or cable end
- whether check failure still costs time, but does not permanently consume tools or materials
- whether abject failure on electrical work produces the intended shock echo and electrical damage
- whether switched-on mounted powered machines recover host-derived power after reboot or late topology initialisation without requiring a manual off/on cycle
- whether motion sensors ignore administrator movement when the mover is using `IImmwalkEffect`

For readable book content, also validate:
- whether `comp set title <text|clear>` gives fresh books the expected default title
- whether `comp set content add <page> <language> <script> [provenance]` creates immutable printed content that appears on newly loaded books
- whether `content list`, `content copy <page> <writing id>`, `content drawing <page> <drawing id>`, `content edit`, `content remove`, and `content clear` round-trip cleanly through component save/load
- whether `writingcollection import markdown|json` can upload multi-page content and either append to or replace an editable collection without leaving partial imports on invalid input
- whether `comp set content collection <collection> [append|page <number>]` causes fresh books to expand the collection into individual page-level readable references
- whether `writingcollection apply <collection> <book> [append|page <number>]` can apply a collection wholesale to an existing live book while preserving page order and capacity checks
- whether prototype content rejects invalid pages and text that exceeds the page capacity supplied by the selected paper prototype
- whether two loaded copies of the same book share immutable readable references for prototype content, but adding handwriting or other readables to one copy does not mutate the prototype, source collection, or another copy
- whether fresh and copied books or paper sheets persist non-zero readable ids after a save/reload cycle
- whether player handwriting can still be added after printed content when the page has spare capacity
- whether `copybook <source> to <destination>` appends all readable page references to another book without tearing, retitling, or exceeding destination capacity
- whether tearing a page preserves the normal book behaviour and does not create dangling readable rows in the remaining pages
- whether on-load FutureProgs can use `addprintedwriting`, `copywritingto`, and `setbooktitle` for generated manuals, certificates, letters, or personalised documents

For the current modern breathing and emergency-medicine slice, also validate:
- whether `gascontainer` and `rebreather` connector genders and socket types match the intended gas-line ecosystem
- whether `externalinhaler` and `inhalergascanister` agree on canister type, and whether gas-bearing presets only exist when the required stock gas was seeded
- whether `integratedinhaler` starts with the intended gas and consumes the expected amount per puff
- whether `defibrillator` power draw and shock emotes match the intended medical tier
- whether `externalorgan` presets only exist when the target body and organ set are present, and whether their venous and arterial connector types match the expected cannula ecosystem

### Manual load restrictions
Some components set `PreventManualLoad`, and item prototypes surface that through `PreventManualLoad`.

That flag exists for content that should only enter the world through controlled runtime systems, such as:
- corpses
- bodyparts
- currency-style special cases

For corpses and severed bodyparts, the controlled runtime path is also responsible for capturing source-body identity.
- creation should record both the original character and the specific body instance that produced the remains
- any later gameplay that inspects anatomy, wounds, inventory, or surgery compatibility on those remains should resolve through that stored source body, not through the character's current body
- this matters now that characters can own multiple dormant forms and switch between them

If a newly added component sets this flag, developers should verify the restriction is intentional and documented.

## Revision and Update Workflows
### Why revisions matter
Items and components are not a simple "edit in place" system. Revisions matter because the world may already contain:
- current live items
- older item prototype revisions
- older component prototype revisions

### Updating prototypes and live items
The `comp update` workflow exists to reconcile:
- item prototypes that reference outdated component prototype revisions
- live items whose item or component definitions have moved forward

Use:
- `comp update`
- `comp update all`

This is especially important after changing component prototype structure or moving a current component into a new revision.

## Skins, Groups, and Related Content
### Skins
Skins are item-prototype-adjacent content that override presentation details such as:
- item name
- short description
- long description
- full description
- quality

From a workflow perspective, skins matter when:
- the base item prototype is intentionally generic
- builders need controlled presentation variants without duplicating item behaviour

Skin authors edit these variants with `itemskin set ...`. Admins can also use `itemskin set suggestdesc [<optional extra context>]` to ask the configured description AI for full-description override suggestions. That prompt is deliberately framed as a variant of the base item: it includes the prototype's physical properties and tags, the effective short and long descriptions, the skin's overrides, and the item-description markup available to full descriptions. The AI call is admin-only so ordinary player skin authors cannot create unbounded API cost.

### Item groups
Item groups are content-side presentation tools for rooms. They let many similar items collapse into grouped room descriptions instead of flooding room output.

Use them when validating:
- new high-volume item content
- environmental props
- grouped furniture or clutter

## Recommended Developer Workflow
When adding a new item capability, the most reliable end-to-end workflow is:

1. Implement and register the component code.
2. Use `comp types` and `comp typehelp` to confirm registration.
3. Create a test component with `comp edit new <type>`.
4. Configure and submit the component.
5. Create or clone a test item prototype.
6. Attach the component with `item set add`.
7. Configure item-level settings.
8. Load a live test item with `item load`.
9. Exercise the runtime behaviour in-game.
10. Run `comp update` when revision churn has occurred.

For the generic-media answering-machine workflow, a practical end-to-end pass is:
1. Create a `mediastoragemedium` component and set its format key, audio capability, and duration capacity.
2. Create an `answeringmachine` component and configure wattage, connectors, ring volume, premote, and answer-after-rings.
3. Attach both to test item prototypes and load them with a compatible telecom outlet and handset.
4. Insert the audio medium into the live machine.
5. Use `select <machine> greeting record`, speak in the same location, and finish with `select <machine> greeting stop`.
6. Place a call and let it ring through to validate greeting playback, beep timing, and message capture.
7. While the call is connected, use `dial <phone> 123#` to validate keypad relay instead of a second outbound call attempt.
8. Use `select <machine> messages play`, `select <machine> message <index>`, `select <machine> erase <index>`, and `select <machine> erase all` to validate inbox handling.

For exchange-hosted voicemail, extend that pass with:
1. Enable hosted voicemail and set the access digits on the `telecommunicationsgridcreator` component before loading the grid.
2. Enable hosted voicemail on the live subscribed line with the handset or endpoint control surface for that number.
3. Place an unanswered call and let the exchange pick it up after the configured ring-out threshold.
4. Speak a test message, then hang up or send `#` to end the recording.
5. From the subscribed line, dial the hosted voicemail access number and use keypad digits such as `1`, `3`, `7`, `9`, and `#` to validate playback, deletion, menu repeat, and hangup.

For the current microcontroller workflow, a practical end-to-end pass is:
1. Create a `pushbutton` component and set its keyword, signal value, duration, and emote.
2. Alternatively create a measurement or timed input such as `lightsensor`, `rainsensor`, `temperaturesensor`, or `timersensor`, or a coded `keypad`, and set its powered-machine and input-specific options.
3. Create either a `signallight`, `electronicdoor`, `electroniclock`, `relayswitch`, or `alarmsiren` sink component and set its source component prototype, threshold, and invert mode. In the current shipped slice, this binds to that source component family's default local `signal` endpoint.
4. Optionally create a `microcontroller` component and use `comp set input add <variable> <sourcecomponent>` for each sibling source component prototype. In the current shipped slice, these input bindings also target the source component's default local `signal` endpoint.
5. Use `comp set logic` on the microcontroller to author inline logic that returns a number.
6. Attach the authored components to the same item prototype with `item set add`.
7. Load the item and exercise the input:
   - `select <item> <button keyword>` for `pushbutton`
   - `select <item> <digits>` for `keypad`
   - `switch <item> on` / `switch <item> off` for `toggleswitch`
   - move through the same location as the composed item for `motionsensor`
   - change ambient lighting or move between differently lit locations for `lightsensor`
   - wait for weather changes or test in rain-exposed versus sheltered locations for `rainsensor`
   - change the ambient room temperature or move between differently heated cells for `temperaturesensor`
   - wait through at least one full active/inactive cycle for `timersensor`
8. Confirm the sink reacts through the authored sibling source component prototype or through the microcontroller output as authored.

For the live player workflow on an already loaded composed item, a practical end-to-end pass is:
1. Use `electrical <item>` to inspect local signal sources and configurable sinks and note the parent item / component names you intend to target.
2. If the item is an automation host, use `electrical install <host> <module> [<bay>]` to install a loose microcontroller or other compatible module item into a named bay.
3. If there is an `AutomationHousing`, confirm it must be open before install, remove, programming, or rewiring is allowed, and confirm access can be reached through normal `open` / `close` subtargets such as `open north panel`.
4. Use `programming <item>` or `programming <host@module>` to inspect the live microcontroller list, compile state, and current input bindings.
5. Use `programming <item> logic <component>` or `programming <item> logic <component> <text>` to replace controller logic on the live item.
6. Use `programming <item> input add <component> <variable> <source> [<endpoint>]` and `programming <item> input remove <component> <variable>` to manage live input bindings.
7. Use `electrical <item> bind <component> <source> [<endpoint>]`, `clear`, `threshold`, and `mode` to reconfigure live sinks, targeting sources by their parent item keywords unless an explicit `item@component` form is needed.
8. Use `electrical route <cable> <source> <exit> [<housing>]` to route a signal cable one room hop away and optionally place its destination end inside an adjacent `AutomationHousing` item.
9. Use `electrical unroute <cable>` and `electrical remove <host> <bay>` to undo the physical setup.
10. Use `electrical <item>` during debugging to confirm not just the authored bindings but also the live resolved chain, including controller inputs, cable mirror sources, nearby routed cable segments, current signal values, and whether each link is presently resolved or broken.
11. Confirm those actions run as staged delayed work rather than instantaneous mutation.
12. Confirm the required tool tag is available, the actor holds or readies the needed tool through the inventory plan, and the tool is restored after completion or cancellation.
13. Confirm ordinary failures cost the action time but do not consume materials.
14. Confirm abject electrical failure produces electrical shock rather than deleting or breaking components.
15. Confirm administrator characters bypass the tool, check, and delay layer for those live item actions while still respecting visibility and service-access gating.
16. When testing movement-driven automation, use an ordinary mover rather than an administrator with `IImmwalkEffect`, because immwalk movement no longer emits the witnessed movement events that motion sensors consume.

For broader manual regression coverage of the current shipped automation slice, use the dedicated [Automation End-To-End Test Scenarios](../Technology/Automation_End_To_End_Test_Scenarios.md) document.

For the standalone player-owned computer-program workflow in the current phase, a practical pass is:
1. Use `programming help` to inspect the computer-safe subset of FutureProg types, statements, functions, and collection helpers.
2. Use `programming new function <name>` or `programming new program <name>` to create a workspace executable.
3. Use `programming set return <type>`, `programming parameter add|remove|swap`, and `programming set source` while editing it.
4. Use `programming compile` to verify the executable in the relevant computer context.
5. Use `programming execute <which> [<parameters>]` to run it manually.
6. If the executable is a program that calls `sleep`, use `programming processes` and `programming kill <process>` to inspect or terminate persisted processes.

For the first real in-world computer workflow, a practical pass is:
1. Create and submit `computerhost`, `computerterminal`, `computerstorage`, and optionally `networkadapter` component prototypes.
2. Attach them to suitable item prototypes and ensure the host and terminal are also authored with the required power and connectable composition.
3. Load a live host, terminal, and storage item and physically connect them through the normal item connectivity rules.
4. Power the host and terminal on.
5. Use `programming terminal connect <terminal>` to create a live session.
6. Use `programming terminal status` to confirm the connected host and mounted storage list.
7. Use `programming terminal owner host` or `programming terminal owner <storage>` to pick the current programming owner.
8. Use the normal `programming list`, `new`, `edit`, `set`, `parameter`, `compile`, and `execute` verbs and confirm they now operate on the selected real computer owner rather than the private workspace.
9. Use file-oriented programs to validate `ReadFile`, `WriteFile`, `AppendFile`, `FileExists`, and `GetFiles`.
10. Use terminal-oriented programs to validate `WriteTerminal` and `ClearTerminal`.
11. Use `programming apps` and confirm the connected powered host exposes the expected built-in application list.
12. Use `programming app sysmon` and confirm it runs as a host process, writes diagnostics to the connected terminal session, and reports host power, storage, process, and local signal state.
13. Use `programming app filemanager` and confirm it opens as a foreground interactive host process that immediately waits on `UserInput()` rather than completing.
14. Use `type owners`, `type list`, `type show <file>`, `type edit <file>`, `type write <file> <text>`, `type append <file> <text>`, `type copy <file> host`, `type use <storage>`, and `type exit` to confirm the connected terminal session is driving FileManager and that it can move between the host file system and mounted storage devices.
15. If the host item also has a `filesignalgenerator`, confirm `type owners` lists that component as an additional selectable local file owner and that `type use <owner>` plus `type show <file>` or `type edit <file>` can inspect and change its backing signal file.
16. When testing `type edit <file>`, confirm it hands off to the normal multiline editor, recalls the current file contents, saves on `@`, and leaves the file unchanged on `*cancel`.
17. Use `programming app directory` and confirm it opens as a foreground interactive host process that immediately waits on `UserInput()` rather than completing.
18. If a `networkadapter` is present, attach it to a telecommunications grid and, if desired, link that exchange to another grid that also has a powered reachable host with its own `networkadapter`.
19. Use `type summary`, `type services`, `type storage`, `type terminals`, `type adapters`, `type routes`, `type gateways`, `type hosts`, `type show <host>`, `type services <host>`, and `type exit` to confirm the connected terminal session is driving Directory, that it still exposes the local host and directly connected devices, and that it now discovers reachable hosts across the linked telecommunications-grid graph.
20. On a reachable host that should act as a shared identity and mail server, use `programming network domain add <domain>`, `programming network account add <user@domain> <password>`, and if appropriate `programming network vpn add <vpn>` while connected to that host as an administrator.
21. If that host should act as a VPN gateway, use `type gateways` and then `type tunnel connect <host> <user@domain> <password> [vpn]` from another session and confirm `type routes` now shows the added temporary tunnel route without changing unrelated sessions.
22. Use `type hosts` before and after the tunnel is connected and confirm VPN-only hosts appear only for the authenticated session that opened the tunnel.
23. If the same host should also act as a mail server, use `programming mail service on` and confirm `type services <host>` in `Directory` from another reachable host now advertises `Mail` with its hosted domain details instead of reporting no implemented services.
24. Use `programming app mail` and confirm it opens as a foreground interactive host process that immediately waits on `UserInput()` rather than completing.
25. Use `type login <user@domain> <password>`, `type inbox`, `type read <id>`, and `type delete <id>` to confirm the mail client authenticates against a reachable shared network identity on a hosted domain and can inspect mailbox state.
26. Use `type send <user@domain>`, `type subject <text>`, `type body`, `type post`, and `type exit` to confirm the mail client can compose and deliver mail, and that `type body` hands off to the ordinary multiline editor before returning to the terminal session.
27. On a reachable host that should also expose network boards, use `programming boards service on` and `programming boards add <board>` while connected to that host as an administrator.
28. Use `type services <host>` in `Directory` from another reachable host and confirm the board server now advertises `Boards` with its hosted board names instead of reporting no implemented services.
29. Use `programming app boards` and confirm it opens as a foreground interactive host process that immediately waits on `UserInput()` rather than completing.
30. Use `type hosts`, `type open <host>`, `type login <user@domain> <password>`, `type boards`, `type use <board>`, `type list`, `type read <id>`, `type post <title>`, `type delete <id>`, `type logout`, and `type exit` to confirm the boards client can authenticate through the shared identity system, browse remote hosted boards, and create or remove network-authored posts using the normal multiline editor for post text.
31. On a reachable host that should expose public files, use `programming ftp service on`, `programming ftp file list`, and `programming ftp file publish <file>` while connected to that host as an administrator.
32. If that host also has a `filesignalgenerator`, confirm the component owner's backing signal file can be published and later addressed through the same local file-owner selection workflow.
33. Use `type services <host>` in `Directory` from another reachable host and confirm the file server now advertises `FTP` with public-file detail text.
34. Use `programming app ftp` and confirm it opens as a foreground interactive host process that immediately waits on `UserInput()` rather than completing.
35. Use `type hosts`, `type open <host>`, `type list`, `type show <file>`, and `type get <file>` to confirm anonymous FTP access can see and copy only published public files.
36. Use `type login <user> <password>`, `type owners`, `type use <owner>`, `type put <local-file> [remote-file]`, `type delete <file>`, and `type exit` to confirm authenticated FTP can manage files on the target host and its mounted storage devices.
37. Use `programming app filemanager` on another host and confirm `type list public <host>`, `type show public <host> <file>`, and `type copy public <host> <file>` expose the same anonymously readable public files without requiring a separate FTP login.
38. Create or load a host-backed program that writes a prompt with `WriteTerminal(...)`, then calls `UserInput()`, and confirm `programming execute <which>` leaves it suspended rather than completed.
39. Use `programming processes` and confirm the waiting process is shown as a `UserInput` wait rather than a timed `Sleep`.
40. Use `type <text>` while connected and confirm the terminal input surface routes through the current terminal session, resumes the waiting program, and passes the typed text back into that program rather than the private workspace.
41. If there is only one nearby terminal, or one terminal clearly associated with the current `PositionTarget`, confirm `type <text>` auto-resolves and auto-connects to it even without a prior explicit `programming terminal connect`.
42. Create or load a host-backed program that calls `WaitSignal("<source name>")` for a signal source component on the real host item and confirm `programming execute <which>` leaves it suspended rather than completed.
43. Use `programming processes` and confirm the waiting process is shown as a `Signal` wait with the awaited host signal binding rather than a timed `Sleep` or terminal `UserInput`.
44. Trigger that host signal source and confirm the waiting program resumes and receives the non-zero numeric signal value.
45. Use `LaunchProgram` and `KillProgram` from a host-backed executable to validate local host process control.
46. Disconnect with `programming terminal disconnect` and confirm the command surface falls back to the private workspace.

In the current shipped phase, `SysMon`, `FileManager`, `Directory`, `Mail`, `FTP`, and `Boards` have built-in application runtime behaviour. `Directory` is still the first proof of the telecom-backed network layer, but it now also shows the currently implemented remote network services when reachable hosts have `Mail`, `FTP`, or `Boards` enabled and configured, and it can now open and close authenticated VPN tunnels that temporarily extend discovery only for the active terminal session. `Messenger` remains the reserved built-in identity for a future phase.

## Failure Patterns to Watch
- `comp edit new <type>` fails: registration problem.
- item loads but does nothing: the item probably lacks the component or the runtime component is not implementing the expected interface.
- boot-time load fails: likely missing or mismatched database loader registration.
- updated component changes do not appear on old content: update workflow was not run or the update hooks are incomplete.
- a microcontroller refuses submission: its inline logic probably does not compile in the `ComputerFunction` context or one of its input names is invalid
- a sink never responds: the authored sibling source component prototype probably does not match any source component instance on the same item, or the expected local endpoint is not present
- `electrical` or `programming` starts but immediately aborts: the configured tool tag or trait static setting is probably missing or resolves to a non-existent content record
- a live reconfiguration appears to complete but nothing changes: the targeted component may not implement the runtime-configurable interface, or the wrong nearby item / component may have been selected
- `type` says nothing is waiting for terminal input: no program on that terminal session is currently suspended in `UserInput()`, or the active terminal / user pairing does not match the waiting process metadata
- `waitsignal("<name>")` fails at runtime: the current execution host is not a real in-world host item, or there is no signal source component with that name on the real execution host item
- `programming app <name>` reports no match or no available applications: the actor is probably not connected to a powered terminal session on a powered host, or the named built-in application is not implemented for the current shipped phase
- `type list` or `type show` in FTP unexpectedly returns nothing: the remote host may not have FTP enabled, may not be reachable through the telecom-backed adapter graph, or the file may not be published for anonymous access
- `type put` or `type delete` in FTP fails: the session is probably still anonymous rather than authenticated, or the selected remote owner does not exist on the target host

## Thermal Source Workflow
Thermal-source items now have a standard content workflow:

1. Create the relevant component with `comp edit new electricheatercooler`, `fuelheatercooler`, `consumableheatercooler`, or `solidfuelheatercooler`.
2. Author the shared thermal profile:
   ambient, intimate, immediate, proximate, distant, and verydistant.
3. Author the activation model:
   - electric: wattage
   - fuel-fed: medium, specific fuel, burn rate, connector
   - consumable: duration, optional spent item
   - solid-fuel: valid fuel tag, capacity, seconds-per-weight
4. Submit the component revision.
5. Attach it to an item prototype with `item set add`.
6. Load a live test item and validate both room-temperature and proximity-temperature outcomes.

Recommended validation passes by family:
- electric
  - connect it to a suitable power source
  - switch it on and verify the room temperature changes indoors but not outdoors
  - move a character or item between proximity bands and verify the local effect scales correctly
- fuel-fed
  - connect it to a compatible liquid or gas source
  - confirm the authored fuel works and the wrong fuel does not
  - confirm disconnect or exhaustion stops the thermal effect
- consumable
  - load or create the item and verify it auto-starts
  - confirm it deletes or swaps to the authored spent item when expended
- solid-fuel
  - load it with correctly tagged fuel
  - confirm untagged items are rejected
  - confirm fuel burns one item at a time in load order

## Historical Equipment Closure Workflow

When closing an item-content dependency:

1. confirm whether the capability already exists before introducing a new component family;
2. implement and test runtime persistence and builder commands;
3. seed component profiles and stock items with stable names and rerun-repair behavior;
4. export `Seeded_Item_Components.json`, `Item_Component_Types.json` when applicable, and `SeededTagHierarchy.csv`;
5. update the era source reference and consolidated dependency ledger only after source and export names match.

For the Industrialised catalogue, run `scripts/audit-industrialised-prerequisites.ps1` after changing runtime registrations, reusable component stock, or the prerequisite resource set. The command regenerates runtime-factual component metadata and joins it to the maintained seeded catalogue; use `-Check -NoBuild` in verification. Human overrides in the audit TSV must use a `manual:` disposition so refreshes preserve an intentional decision rather than preserving stale factual state.

For the initial historical-arms tranche, verify locked till selection, container tag admission, legacy paper-cartridge XML, musket charge/bore compatibility, bayonet slot and firing rules, spanning-tool inventory plans, and crossbow ready-state persistence.

For the standards, signals, and instruments tranche, additionally verify:

1. all nine Antiquity instruments can start, tick, stop, and interrupt through their intended physical use mode, including on inventory-model bodies carrying unrelated items;
2. the four signal profiles accept only their named patterns, enforce a per-item cooldown, and keep failed audio unrecognisable;
3. all six standard profiles begin unowned, unclaimed, unassociated, unplanted, and at zero captures;
4. character and clan ownership, a configured `CanBearProg`, hostile transfers, recovery, planting, ownership changes, copy/reset behavior, and exact-once hooks follow the custody contract;
5. rerunning either seeder repairs owned definitions and associations without duplicating rows or altering unrelated builder content.

## Restaurant Serviceware Workflow

When authoring a cafe menu, first create and test the real items: stock drinks or dishes, optional craft output, a reusable plate or tray, a takeaway carton, and an outer bag. Build tables as `Table` plus `Container` furniture and place them in the intended service cells. Use normal item containment tests to verify that the food fits every nesting level. Then configure the restaurant menu against the corresponding shop merchandise. Keep menu entries inactive while changing `mode`, `craft`, `plate`, or `package`; configure the outer bag once for the restaurant, and stock reusable plates, cartons, bags, and tools in physical kitchen containers with the relevant storage roles. Activation is the builder-facing compatibility check. Test one dine-in order with a real chef craft, plating, a server placing the plate onto the table, a diner eating from it, and the paced multi-plate cleanup. Also test prepaid takeaway containing multiple ready items: it should use the fewest bags that fit the whole collection, spill into another bag only when needed, and fall back safely if a configured reusable store is unavailable or full.

## Electronic Access-Control Workflow

Use `access <reader> [<credential>]` for ordinary authentication; legacy `select <keypad> <code>` remains valid. Programming uses `electrical <keypad> keypad code`, `electrical <scanner> biometric add|remove|list`, `electrical <reader> keycard add|remove|list`, and `electrical <writer> writecard add|remove|clear|list`. Administrators complete those same operations immediately, while players use the configured electronics tool, trait, staged check, cancellation, failure, and electrical shock-risk path. Enrollment by command requires a present character, and cards or preserved anatomy must be held.

For installed mode, bind the reader's `signal` endpoint to an `ElectronicLock` configured to be locked at idle zero and unlocked during the successful pulse. For embedded mode, select one sibling lock with `electrical <reader> selftarget`. Keep service panels open for programming, installation, removal, and rewiring; ordinary `access` remains usable from the host surface. Acceptance should include rejection, pulse expiry, power loss, restart, code revocation, biometric revocation, severed-anatomy spoofing, and multi-code card matching.

## Modern Appliance Workflow

Build refrigerator and dryer component prototypes before attaching them to appliance item prototypes. Match their capacity and maximum size to the physical description, select a power draw appropriate to the setting, and combine the component with a compatible power connector or supply arrangement. For a display chiller, turn `closable` off; test it specifically against the powered-open and unpowered-open rates. For a dryer, verify that the door closes, switching on fails while open, opening stops a running dryer, and a power interruption pauses rather than inventing elapsed drying.

Prepared food and biological remains are refrigeration-aware automatically. For spoilage represented by an item morph, explicitly run `item set refrigeration`; raw butchery products seeded by the stock animal catalogue opt in. Test a sensitive and a normal morph side by side through containment, door, and power changes so content authors do not accidentally slow unrelated transformations.

For a portable bank, author distinct compatible input and output connectors, then test charging, output under load, simultaneous charging/discharging, depletion, and recovery. Capacity is watt-hours; charging efficiency applies only on input. Output never becomes unlimited passthrough, so size storage and output watts for the intended device duty cycle.

Washing machines and dryers both operate on the authoritative `SurfaceLiquidState`. Do not add legacy contamination effects to make washing or drying content work. Validate empty-load rejection, door interlocks, detergent use, power interruption, cancellation/restart, and spin drying before publishing an appliance profile.
