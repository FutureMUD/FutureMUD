---
slug: clans
category: Economy
summary: Build and administer clans, elect a town council and mayor, appoint a vassal militia captain, collect taxes, manage property and treasury rooms, and reuse clan templates.
---

# Building clans and town government

A clan can represent a town, regiment, guild or church. **Membership** connects a character to the organisation. A **rank** records standing, often along a promotion path. An **appointment** gives a character an office such as Mayor, and several people can hold the same appointment if it has enough seats. A **paygrade** gives a rank or appointment a scheduled pay amount. A member's effective privileges combine their rank privileges with the grants from every appointment they hold.

This guide builds **Alderbridge** (`alder`) and the independent membership roster of the **Alderbridge Militia** (`aldermilitia`). Residents elect three Councillors. Seated Councillors elect one of their own as Mayor, and whoever holds that office can appoint the militia Captain. The town collects trading taxes and owns a civic building and a market workshop.

Use a test town and ordinary player characters for the checks. Administrators have overrides that can hide missing privileges. Replace angle-bracket fields with real IDs, names or amounts. Sample prices use a currency that accepts amounts such as `5cr`. Quote multiword arguments where another argument follows.

## Create the town and delegate administration

Start as an administrator. A new clan comes with the **Member** rank and a one-seat **Founder** appointment with all privileges. The creator receives both. Keep Founder on the builder while you test the limited offices, then use `clan resign alder Founder` from the founder's character if you want to retire their broad authority. Preserve a way for authorised staff to maintain the town's structure. Setting the description opens the text editor.

```text
clan create new alder Alderbridge
clan edit alder
clan set desc
clan set calendar <calendar-id>
clan set rank Member privileges add CanViewClanOfficeHolders
clan set rank Member privileges add CanViewClanStructure
clan create rank Clerk Clk after Member
clan set rank Clerk privileges add CanInduct
clan set rank Clerk privileges add CanViewMembers
clan set rank Clerk privileges add CanViewClanOfficeHolders
clan set rank Clerk privileges add CanViewClanStructure
clan create appointment Councillor Cllr
clan set appointment Councillor maxholders 3
clan create appointment Mayor Myr
clan set appointment Mayor maxholders 1
clan set appointment Mayor privileges add CanManageEconomicZones
clan set appointment Mayor privileges add CanManageClanProperty
clan set appointment Mayor privileges add CanViewTreasury
clan set appointment Mayor privileges add CanManageBankAccounts
clan set appointment Mayor privileges add CanCreateBudgets
clan view alder
```

Use `clan view alder appointment <office>` and `clan view alder rank <rank>` to inspect the settings. The corresponding `clan set` commands list editor options when you omit the field. Grants belong to the particular rank or appointment. Add visibility to both ranks here because promoting a Member to Clerk changes the character's rank. Assign Mayor's financial powers to a Treasurer appointment instead if you want separate officeholders for taxes and spending. In this example, the Mayor can set taxes and draw funds while Councillors can see the election without receiving those powers.

| Routine | Commands and checks |
| --- | --- |
| Admit a resident | Use `clan invite <person> alder` with the person present and have them `accept`. If the town has multiple rank paths, specify the desired path as the final argument. Use `clan members alder` to check the roster as a Clerk or administrator. |
| Change standing | Use `clan promote <person> alder Clerk` for a new clerk and `clan demote <person> alder Member` for a return to ordinary standing. Check the actor's promote or demote grants and relative rank. Use `clan castout <person> alder` for removal from the organisation. |
| Fill an ordinary office | Use `clan appoint <person> alder <office>` and `clan dismiss <person> alder <office>`. Configure `minrank`, `minappointer`, `maxholders` and optionally `parent` in the appointment editor. A parent can confer authority over subordinate offices. A holder can use `clan resign alder <office>` if resignation is enabled and their job permits it. |
| Inspect the organisation | Use `clans` for your own memberships and pay, `clan list` for an overview and `clan view alder` for clan details. Member lists, officeholders, structure and treasury use separate view privileges. |
| Configure scheduled pay | Use `clan create paygrade "Clerk Stipend" ClkPay <currency> 5cr` while editing the clan. For an appointment allowance, use `clan set appointment <office> paygrade "Clerk Stipend"`. For rank pay, use `clan set rank Clerk paygrade add "Clerk Stipend"` and check each existing clerk's assigned paygrade in their membership. `clan paygrade promote <person> alder` advances an eligible member within their rank's paygrades. Set a cadence with `clan payinterval alder "every 1 weeks"`, cap accrued backpay with `clan maxpay alder <multiple>` and inspect `clan payroll alder`. |

For membership granted at character creation, bind the intended live clan and rank or appointment to a chargen role. [Character creation builder workflows](Character_Creation_Builder_Workflows.md)

## Elect three Councillors, then one Mayor

The example uses one Councillor appointment with three seats, so all candidates compete on the same ballot. Each voter backs one candidate. Voting again changes that choice. The tally ranks candidates with positive votes, with earlier clan join dates breaking ties. Arrange for three different candidates to receive votes in your first test. A candidate with zero votes can leave a vacant seat even on an uncontested ballot.

Invite at least four test residents and find the actual Alderbridge clan ID with `clan view alder` or `clan list`. Create the four FutureProgs below. Each uses one `character` parameter called `ch`. Use `prog edit new <name>`, `prog set return <type>`, `prog set parameter add ch character` and `prog set text`. Submit the source with `@` on its own line and check the compilation result. Leave static mode off, since membership and appointments change. Replace `<town-id>` in each prog with the numeric ID. [FutureProg authoring](FutureProg_Builder_Guide.md)

**AlderResidentVotes** returns Number. The ordinary nomination command already requires active membership, so the town needs only this voting prog for the resident ballot.

```text
if (IsClanMember(@ch, ToClan(<town-id>)))
  return 1
end if
return 0
```

**AlderCanRunForMayor** returns Boolean. The extra argument to `IsClanMember` checks the actual Councillor appointment.

```text
return IsClanMember(@ch, ToClan(<town-id>), ToAppointment(ToClan(<town-id>), "Councillor"))
```

**AlderWhyNotMayor** returns Text.

```text
return "Only seated Alderbridge councillors can stand for Mayor."
```

**AlderMayoralVotes** returns Number.

```text
if (IsClanMember(@ch, ToClan(<town-id>), ToAppointment(ToClan(<town-id>), "Councillor")))
  return 1
end if
return 0
```

### Seat the council first

Set the council's timing with the freshly created town temporarily in template mode so no ballots open mid-setup. Inspect the template flag before either toggle. Once the town goes live, its calendar drives the opening ballot and regular terms.

```text
clan edit alder
clan set template
clan set appointment Councillor election enable 90
clan set appointment Councillor election nomination 3
clan set appointment Councillor election voting 3
clan set appointment Councillor election lead 1
clan set appointment Councillor election vote AlderResidentVotes
clan set template
clan elections alder
```

Use the ID of the council ballot whose nomination window is open. With this fresh, temporarily templated town and no existing ballots, expect an initial regular election. If you enable a vacant office on a live clan instead, the immediate ballot may be a by-election followed by a scheduled regular election. As each candidate, run `clan nominate <council-election-id>` during nominations. Once voting opens, have residents run `clan vote <council-election-id> <candidate-name>`. Test that a non-member cannot vote. Check `clan election view <id>` and `clan view alder` after installation. Verify all three Councillors are seated before opening nominations for Mayor.

### Have the seated Councillors elect the Mayor

Enable the second election after the council takes office. On a live clan, enabling elections for a vacant office starts its first by-election immediately using the default seven-day nomination and seven-day voting windows. Use `clan election view <id>` for the dates on that ballot. Later timing edits apply to newly created ballots.

```text
clan edit alder
clan set appointment Mayor election enable 90
clan set appointment Mayor election nominate AlderCanRunForMayor AlderWhyNotMayor
clan set appointment Mayor election vote AlderMayoralVotes
clan set appointment Mayor election term none
clan set appointment Mayor election nomination 3
clan set appointment Mayor election voting 3
clan elections alder
```

The example gives the Mayor indefinite tenure, with a new ballot when the seat becomes vacant. Setting the term to `none` while the regular ballot is still scheduled removes that cycle and leaves the vacancy ballot. Have two Councillors nominate themselves and all three vote once. As a fourth resident, try both commands and check that nomination explains the rule and voting refuses. After installation, the winner should hold both appointments. Only the Mayor's membership should gain the grants assigned to that office among your ordinary testers.

The nomination prog checks eligibility when somebody nominates. The vote prog checks it when somebody votes. A council turnover does not automatically remove a Mayor, cancel a candidacy or withdraw a vote already cast. Adopt a charter rule that a Mayor who loses their council seat must `clan resign alder Mayor`. Review pending candidacies when Council membership changes and use authorised intervention if a holder won't resign. If you prefer fixed mayoral terms, use `election term <days>` and inspect both calendars before each handover so nominations and voting open after the council seats are settled. Use `clan withdrawnomination <id>` during nominations for a candidate who no longer qualifies.

Other election options include `election secret` for secret ballots, `election terms <consecutive> <total>` for term limits (zero leaves that limit open), and `election confirmation <clan> <appointment>` for a result awaiting another office's approval. A confirmer uses `clan election confirm <id>` or `clan election veto <id>`. Leave confirmation off for this town unless its charter names a separate authority.

For an optional council recall, set `election recall initiate AlderCanRunForMayor`, `election recall vote AlderMayoralVotes`, `election recall threshold 50%` and `election recall enable` on Mayor. A qualified councillor uses `clan recall <mayor-name> alder Mayor`, then seated Councillors use `clan vote <recall-id> yes` or `no`. Removal requires strictly more than half the weight cast in this example. If the council seat has already been lost and the Mayor remains in office, the seated Councillors can still vote to remove the Mayor. [Election timing, confirmation, resignation and recall](../Economy/Clan_Elections_and_External_Control.md)

## Set abbreviations, titles and forms of address

Alderbridge's clerk appears as **Town Clerk** in full text and **TC** where a compact label is used. The Mayor can be referred to as **First Magistrate**, abbreviated **FM**. In conversation, a resident of lower rank says **Your Worship** to the Mayor, while a fellow clerk says **Magistrate**.

### Set the labels for a rank and an office

The internal rank name is still `Clerk` and the appointment name is still `Mayor`. Keep using those names in builder commands and FutureProg lookups. Add each display value, then move it to position `0`, the start of its list.

```text
clan edit alder
clan set rank Clerk abbreviation add TC
clan set rank Clerk abbreviation reorder TC 0
clan set rank Clerk title add "Town Clerk"
clan set rank Clerk title reorder "Town Clerk" 0
clan set appointment Mayor abbreviation add FM
clan set appointment Mayor abbreviation reorder FM 0
clan set appointment Mayor title add "First Magistrate"
clan set appointment Mayor title reorder "First Magistrate" 0
clan view alder rank Clerk
clan view alder appointment Mayor
```

The detail views show the configured lists. As a character with the rank or office, run `clans` to check the full titles in membership output. Once public fame is set up, check the Mayor's title in `notables` too.

The first applicable row wins in each list. To add a conditional title or abbreviation, append a Boolean prog name or ID to the `add` command after the displayed text, then move that row ahead of the unconditional fallback. Keep an unconditional row last so there is always a match. These progs receive one character, and which character is supplied depends on the display. Test conditions in the output where players will see them. Use a form of address when you need a rule that explicitly knows both speaker and subject.

### Address the Mayor according to the speaker's rank

After the election, promote the test Mayor to `Clerk` if they still have the `Member` rank. Use a separate Clerk as the peer speaker and a Member as the lower-ranking speaker. Address relations compare **clan ranks**. A fellow Councillor can be lower-ranked, and holding the Mayor appointment alone does not raise a character's rank.

```text
clan promote <mayor-character> alder Clerk
clan edit alder
clan set rank Clerk address Clerk
clan set appointment Mayor address Magistrate
clan set appointment Mayor address add "Your Worship" from lower
clan set appointment Mayor address priority 20
clan set appointment Mayor address list
```

The lower-rank exception appears before the default. The Mayor's matching office rule takes precedence over the Clerk rank's `Clerk` address. Priority decides between held offices with matching rules. Different answers at the same highest priority leave automatic resolution ambiguous. A rule set to `none` suppresses fallback, while removing the rule lets the resolver look for another match.

For this test, ensure both member speakers have `CanViewMembers`. The Clerk already has it in this walkthrough. To let all residents see the member roster and use the members-only address, run `clan set rank Member privileges add CanViewMembers`. If the roster should stay restricted, keep the grant on a test office or make only the intended address rows public.

As an administrator with the Mayor and all three test speakers present, compare the results.

| Preview | Expected result |
| --- | --- |
| `clan address alder <member-speaker> <mayor-character>` | **Your Worship**, from the Mayor's lower-rank rule |
| `clan address alder <clerk-speaker> <mayor-character>` | **Magistrate**, from the Mayor's default rule |
| `clan address alder <outsider-speaker> <mayor-character>` | No address with the members-only rules above |

To offer outsiders their own courtesy, add `clan set appointment Mayor address add "Mayor of Alderbridge" from outsider public` and repeat the outsider preview. For the optional full-look hint, enable the game's `ClanSimilaritiesShowInLook` setting and toggle `clan set addresslook` on for Alderbridge. Then use `look <mayor-character>` from each member speaker. Full look uses the shared-clan hint, so the public outsider rule is useful for progs and previews even though outsiders get no automatic hint.

For NPC dialogue, resolve the address with the actual speaker and subject. This example is the body of a Text prog with two Character parameters, `speaker` and `subject`. Replace `<town-id>` with Alderbridge's numeric ID.

```text
return ClanFormOfAddress(@speaker, @subject, ToClan(<town-id>))
```

Use the result in the NPC's greeting. On empty text, use an ordinary greeting. See the address reference for predicates, rule editing and visibility. [Clan forms of address](../Economy/Clan_Forms_of_Address.md)

## Give the Mayor control of the militia Captain

Create the militia as a separate clan. Leave Captain unelected, with one vacant seat. The militia Founder or another member with `CanSubmitClan` submits that seat to Alderbridge and confirms the proposal.

```text
clan create new aldermilitia Alderbridge Militia
clan edit aldermilitia
clan create appointment Captain Cpt
clan set appointment Captain maxholders 1
clan set appointment Captain privileges add CanViewMembers
clan set appointment Captain privileges add CanInduct
clan submit aldermilitia Captain alder Mayor 1
accept
```

The reservation belongs to the Mayor's **office**, so it continues under the next Mayor. Leave `CanManageClanVassals` off ordinary town ranks and offices. A member with that broad grant can also exercise vassal control, and the Founder starts with it. Keep Mayor free of a parent appointment if you intend ordinary control to stay with its holder.

Have the Mayor meet an intended Captain in person. Invite them to the militia in advance if membership and rank should be a deliberate part of the story. External appointment can otherwise induct the candidate or raise them to the minimum rank.

```text
clan vassal appoint <candidate> aldermilitia Captain alder
```

As an administrator or a militia member with structure and officeholder visibility, inspect `clan view aldermilitia` to verify the Captain. Try appointing as an ordinary Councillor without the relevant grants. Once the seat is occupied, replace the Captain through `clan vassal dismiss <current-captain> aldermilitia Captain alder` followed by an external appointment. Ordinary local dismissal cannot remove a holder installed by the liege. A mayoral succession leaves the Captain in place until the office is vacated or the new Mayor changes the appointment. To change which office controls the existing reservation, use `clan vassal control aldermilitia Captain <new-liege-office> alder` with the appropriate authority. Review holders before using `clan release` or `clan transfer`, since a transfer requires rebinding the new liege's controlling office. [External control and reserved seats](../Economy/Clan_Elections_and_External_Control.md#external-control-model)

## Fund the town from an economic zone

An economic zone supplies currency, trading tax rules and a financial calendar. Start with a configured zone and a test shop in it. Assign Alderbridge custody as an administrator. While visiting the building where players should buy, sell and lease property, flag that room for conveyancing. Check its current flag before toggling.

```text
ez edit <zone-id>
ez set clan alder
ez set realty
ez show <zone-id>
```

As the Mayor, list the controlled zones and introduce a 5% sales tax. The tax creation parser takes the **name followed by type**. Existing taxes can also apply at checkout, so use a test zone with a known baseline.

```text
clan economiczone list
clan economiczone <zone-id> salestax add "Town Sales Tax" vat
clan economiczone <zone-id> salestax "Town Sales Tax" rate 5%
clan economiczone <zone-id> shops
clan economiczone <zone-id> taxinfo
```

A town can also set profit taxes (for example, `profittax add "Town Profit Duty" net`, then set that tax's `rate 2%`). Shop profit taxes depend on the financial period and the configured profit basis. `clan economiczone <zone-id> shop <shop>` shows what the shop owes. Use `taxinfo` for zone figures and `revenue ledger` for actual receipts.

1. Have a customer buy a normally taxed item with a pre-tax price of `100cr` at the test shop. With only this tax and no other adjustments, expect a `105cr` checkout and `5cr` in sales tax.
2. Have the shop manager check the books and use `shop paytax all` when tax is owed. Compare the shop's tax position and the zone revenue ledger before and after remittance. Tax collected at checkout can remain owing at the shop. Only money received by the zone is available for withdrawal.
3. As the Mayor, use `clan economiczone <zone-id> revenue withdraw 5cr` once the ledger shows sufficient held revenue. This issues physical cash. Carry it to the town bank or deposit it in the clan's virtual treasury. Check the active currency before using the treasury. The zone ledger and clan balances record separate transactions.

Use that revenue for town payroll, a recurring works budget or a purchase such as the market workshop below. For a militia stipend, deposit the appropriated funds into a separate militia account for its payroll. The Captain reservation does not share accounts or upkeep. [Shop taxes, currency and banking](Shop_and_Banking_Builder_Guide.md)

## Set up the bank, treasury, hall and administrative rooms

At the matching bank branch, use `bank openclan <account-type> alder` as the Mayor or another authorised clan officer. Choose an account type that allows clan ownership and uses the town's currency. Note its bank code and account number. Select it as the clan's default account and fund it.

```text
clan edit alder
clan set bankaccount <bankcode>:<account-number>
bank deposit <account-number> <cash-amount>
clan balance alder
```

The bank account is useful for property income and civic spending. Use `clan budget alder create <office> <amount> "<interval>" <name>` to assign an allowance. For example, `clan budget alder create Mayor 40cr "every 1 weeks" Town Repairs` gives the Mayor a periodic spending limit. An authorised holder can `clan budget alder draw "Town Repairs" 10cr Repair the market counter`. Check `clan budget alder audit "Town Repairs"` to see the reason, the balance and how much of the period's allowance remains. The allowance does not create money. Fund the account and verify the draw. [Clan budgets and payroll](../Economy/Clan_Command_Surface_and_Permissions.md#runtime-finance-commands)

The three room roles can overlap. Set each by standing in the intended room as an administrator and running the corresponding toggle. Check the room lists in `clan view alder` before repeating a command.

| Room in Alderbridge | Command from that room | Effect |
| --- | --- | --- |
| Council vault | `clan treasury alder toggle` | Establishes a physical treasury room. Place actual currency piles directly in the room or in containers in the vault if you want payroll to draw from physical stores. Use doors and locks for access. |
| Council chamber | `clan hall alder` | Counts as a workplace for the clan employment host. Rooms in properties with any clan ownership share are already included. A hall flag is useful for a workplace outside clan property. |
| Clerk's office | `clan admin alder` | Gives members a place to collect accumulated pay using `payday`. If a paymaster NPC or item is configured, make it available there and test with a member owed pay. |

The **virtual treasury** is separate from physical treasury room contents. An officer with `CanManageBankAccounts` can use `clan treasury alder deposit <amount>` or `withdraw <amount>` with their active currency, and `CanViewTreasury` allows `clan treasury alder balance` and `ledger`. A matching default bank account can contribute funds to virtual treasury withdrawals and payroll. To test pay, give a clerk a paygrade, let a payday accrue or add a small amount of backpay using `clan pay <clerk> alder <amount>`, then have the clerk use `payday` at the administrative room. Check `clan payroll alder member <clerk>` and the actual source balance. [Employment hosts](Employment_Hosts_Builder_Guide.md)

## Own the town hall and lease out a workshop

**Initial civic property.** As a builder, stand in the town hall foyer and create a property against the clan-owned account. The account's owner becomes the property owner. Stand in each additional room to link it to the current property, then inspect the result.

```text
property edit new "Town Hall" <zone-id> 1000cr <bankcode>:<account-number>
property set desc
```

In the council chamber and clerk's office, run `property set room` once per room. This toggles the current room. Check the property before adding the vault, and use doors, locks and keys to control access even when all those rooms belong to the same property. The clan's treasury and administration room flags remain separate from its ownership share.

**Buy and manage an existing property.** As the current owner of a test property called Market Workshop, use `property sell "Market Workshop" 100cr` and `property approvesale "Market Workshop"`. Check that the order is open and any other owners have consented. Give the town account enough funds for the purchase in addition to the tax receipt. At a conveyancing office in the same zone, switch to the Mayor and use the clan purchase path to put title in Alderbridge's name. Supply a clan account the Mayor can debit. Omitting the account uses accessible cash.

```text
clan buy alder "Market Workshop" <bankcode>:<account-number>
clan properties
property preview "Market Workshop"
property setbank "Market Workshop" <bankcode>:<account-number>
property makelease "Market Workshop" 20cr 10cr
property approvelease "Market Workshop"
```

The example sets rent and a bond. Inspect the lease order for the payment interval, allowed length, renewal and relisting settings. Have a test tenant lease the property and verify the clan's ownership and the tenant's separate lease. Each owner's share governs their portion of revenue. `property setbank` directs future proceeds for an authorised ownership record. If its account is cleared, inspect the virtual balance with `property revenue "Market Workshop" ledger` and use `property revenue "Market Workshop" withdraw <amount>` when due.

Use `property addkey <property> <keyitem> <keyname> <replacement-cost>` to register a real key and test it on the door. An active lease gives the eligible leaseholder the claim on associated shops, hospitals and stables, using `property claimshops <property>` in the conveyancing office. Claiming can replace the current business roster. Arrange the handover with the tenant before using it. [Property-backed shops](Shop_and_Banking_Builder_Guide.md#put-a-permanent-shop-on-a-property)

| Other ownership action | Command at conveyancing |
| --- | --- |
| Lease premises for the clan while the owner retains title | `clan lease alder <property> "<term>" [<bankcode>:<account-number>]`. Use a term the lease order accepts. |
| Transfer part of the clan's ownership | `clan divest alder <property> <percentage> *<other-clan>` for a clan, or supply a character as the recipient. Check the ownership split, approval prompt before confirming, then review the ownership and revenue records. |
| Offer clan property for sale | `property sell <property> <price>` and `property approvesale <property>`. Review co-owner consent, current lease, revenue destination and any secured lending restrictions before proceeding. |

Test a plain Councillor as well as the Mayor. `CanManageClanProperty` authorises management of clan property. Use `CanAccessLeasedProperties` or `UseClanProperty` for the separate access or usage roles where the relevant feature checks them, and issue physical keys to those who should be able to enter.

## Create and maintain reusable clan templates

Build a fresh source clan with the ranks, paygrades, offices and privilege grants you want to repeat. For example, make a militia template with Member, a Sergeant rank and one unelected Captain. As an administrator, flag the source and list it.

```text
clan create new townmilitiapattern Town Militia Template
clan edit townmilitiapattern
clan create rank Sergeant Sgt after Member
clan create appointment Captain Cpt
clan set appointment Captain maxholders 1
clan set appointment Captain minrank Sergeant
clan set template
clan templates
clan view townmilitiapattern
clan create template townmilitiapattern eastmilitia Eastgate Militia
clan view eastmilitia
```

A live copy starts with the creator at the highest rank and installs them in a copied all-privileges appointment if one exists. Non-admin creation also depends on the world's `PlayersCanCreateClansProg` setting. The clone receives its own ranks, paygrades and appointments, including hierarchies, fame and election settings. Local confirming-office references are rebound to the equivalent copied office. People, votes, dates, history, properties, room assignments, bank balances and external seat reservations belong to the live organisation. Recreate the latter links deliberately on the copy, for example with a new `clan submit` from Eastgate Militia to Eastgate's Mayor.

An administrator can instead use `clan create staged townmilitiapattern eastmilitia Eastgate Militia` **in place of** the live-copy command. This starts in template mode without founder membership. Assign the intended policy and check the links before activating an ordinary unprofiled copy with `clan edit eastmilitia` and `clan set template`. Authorised staff can then induct the intended members. A profiled template, such as a seeded diocesan template, requires the staged path and its provisioning actions to bind the declared live offices, apply any variants and activate the result. [Profiled clan provisioning example](../Seeding/Catholic_Clan_Template_Provisioning.md)

A template stores election rules but runs no ballots. Existing live clans retain their own settings when you edit the source. If you make a template from Alderbridge, the example's custom FutureProgs contain the town's numeric ID. In a staged copy, duplicate and retarget those progs to the new clan and bind the new versions before activation. Check `clan elections` on both the source and copy. Avoid toggling a governing clan into template mode for ordinary edits. Doing so suspends its ballot participation and listeners until it goes live again.

## Make officeholders public through WHO and NOTABLES

These are two independent settings. A town may want its Mayor recognisable in town lore while keeping the online count disabled, or report member activity without making any individual famous.

```text
clan edit alder
clan set sphere Alderbridge
clan set notable
clan set notablemembers
clan set appointment Mayor fame NameAndDescription
clan set appointment Councillor fame NameOnly
clan set rank Member fame None
```

- **`clan set notable`** toggles the clan's member count in `who`. It adds a count for viewers who belong to that clan, subject to the viewer's sphere. When the viewer has a nonempty sphere, it must match the clan's sphere. The count uses the game's WHO population and can include eligible NPCs marked to count.
- **`clan set notablemembers`** enables the clan's entries in `notables`. The viewer can be an outsider. Only active memberships with a famous rank or at least one famous appointment qualify. `NameOnly` prints the full name and applicable famous titles. `NameAndDescription` also reveals the character's appearance, ignoring disguises. Use `None` for ranks and offices you want omitted. An individual with more than one famous office still displays a description if any qualifying rank or office requests it.
- **`clan set sphere <name>`** assigns the clan's sphere. The game gets the viewer's sphere from the configured `CharacterSphereProgId` FutureProg. Coordinate the exact text with that prog. In a test world, a High Admin can set the ID of a Text prog with one Character parameter using `editstaticconfiguration CharacterSphereProgId`. A live world can already use this setting for WHO and landmarks, so preserve those existing sphere rules when adding Alderbridge. `clan set sphere clear` makes the clan unscoped for `notables`, so any viewer can see its famous people. If the viewer's prog returns empty text, their `notables` list includes only unscoped clans, while their `who` clan counts have no sphere filter.

Try `who` and `notables` as an Alderbridge member, as an outsider in the same sphere, and as an outsider from another sphere. Expect the same-sphere outsider to know the public officeholders but receive no Alderbridge member count. Check that ordinary residents are absent from the notables list. Following a mayoral handover, check that the new Mayor's name appears and that the former Mayor only appears if another qualifying rank or office still makes them notable.

## Diagnose the town as an ordinary player

| Symptom | Check |
| --- | --- |
| Residents can see the council ballot but cannot vote | Inspect the ballot stage and the appointment's Number-returning vote prog. A missing prog gives zero votes. Use the ID of the ballot currently accepting votes. |
| The mayoral ballot has no eligible candidates | Check that the council results are installed and the eligibility prog points to this town and its Councillor appointment. The candidate must already hold it when nominating. |
| The Mayor cannot appoint the Captain | Confirm the reservation links the militia's Captain to Alderbridge's Mayor, has capacity, and the Mayor and target are together. Inspect existing external holders before attempting replacement. |
| A taxable sale appears but the town cannot withdraw the money | Compare shop tax owing with actual payments and the zone's held revenue. Check the zone of the shop and that the Mayor has `CanManageEconomicZones`. |
| A clerk cannot collect pay | Check backpay, the administrative room, the paymaster if configured and accessible funds in the matching currency. Review the physical treasury and the virtual or bank balance separately. |
| An officer can see the property but cannot transact | Use the conveyancing office, confirm their clan property rights and check whether the clan owns a share, leases the property or has a valid purchase or lease offer. |
| A public office is missing from `notables` | Check both the clan's famous-members switch and the office's fame setting, the viewer's sphere and the officeholder's active membership. |
