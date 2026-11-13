# Staged performances and rites

**Prototype and design.** A parish can stage a service, a company can put on a play, and a band can arrange a concert. Players and staff can author reusable works. An in-world organiser casts a work and books the places and props. During the performance, the cast determines the pace and the world determines whether the actions succeed.

## Playing the prototype

For a first production, follow the [builder walkthrough](../Building/Staged_Performance_Builder_Guide.md). Setting packages can install a reusable published work using the [seeder guide](../Seeding/Staged_Performance_Seeder_Guide.md).

Use `performance work help` for the implemented editor and `performance help` for staging. The proposed long-form syntax in the worked designs below illustrates the intended content. The following commands use the running prototype's syntax. Create each fixture once, then edit or publish the resulting ordinary work.

```text
performance work fixture mass
performance work fixture concert
performance work fixture procession
performance work show "Parish High Mass"
performance work publish "Parish High Mass"
performance plan new "Sunday service" "Parish High Mass" "Parish"
```

Substitute the actual name of a host where you can assign tasks. Staff can omit the host when setting up a rehearsal. The plan is selected on creation. Use `performance plan "Sunday service"` to return to it later. At the sacristy, bind the room and a visible missal. With the sanctuary through the south door, a player can finish the room and exit bindings there.

```text
performance bind place sacristy here
performance bind place sanctuary south
performance bind exit sacristy-door south
performance bind prop missal book
```

Use local visible character keywords to cast each person. The selected server will fetch the missal and physically walk it to the sanctuary. An organiser can assign eligible host employee NPCs and invite PCs. Cues that stage an audience link or guard access require control of the relevant premises. A publicly traversable procession leg only needs a reachable exit.

```text
performance cast server Tomas
performance cast celebrant Ana
performance cast reader Elin fallback Jo
performance preview
performance call
```

Elin goes to the sanctuary and types `performance signin "Sunday service" reader`. Another local PC may type `performance volunteer "Sunday service" reader`. Before kickoff the organiser can type `performance assign "Sunday service" reader Tali` or `performance remove "Sunday service" reader`. Use `performance start` to start as soon as the cast is ready. Use `performance status` to inspect the run and `performance done` as the reader to yield an expressive cue. For a physical cue, perform the ordinary in-world action. A return performance can reopen the plan with `performance call`.

### Concert with ushers

In this example the platform is north of the stalls and south of the gallery. The foyer can be several rooms away. The two guards need separate performers. Use the target keywords of actual local performers for each casting command. Bind the guitar while you can target it, for example while holding it or while it is on the floor. Then give it to the guitarist and check they have enough free hands.

```text
performance work publish "Here Comes the Sun"
performance plan new "Evening concert" "Here Comes the Sun" "Concert company"
performance bind place platform here
performance bind place stalls south
performance bind place gallery north
performance bind prop guitar guitar
performance cast guitarist Mo
performance cast singer Lee fallback Jess
```

Go to the stalls and enter `performance bind exit stalls-stairs north`. In the gallery use `performance bind exit gallery-stairs south`. In the foyer enter the following commands once the ushers are present.

```text
performance bind place foyer here
performance cast usher-stalls Robin
performance cast usher-gallery Kit
performance preview
performance call
```

Lee signs in on the platform. Leave the optional introducer vacant for a shorter concert. At kickoff the ushers walk to their separate posts, then guard the exits from the audience to the platform. Patrons can leave by the other exits. Both rooms receive the platform feed. The guitarist starts playing and continues through the singer's turn. After the bow the production releases both views and both posts.

### Lantern procession

For a chapel, square and shrine connected by two northward legs, start in the chapel with the lantern and cast present. The leader and bearer in this example are NPCs. The petitioner must be a PC.

```text
performance work publish "Lantern Procession"
performance plan new "Feast procession" "Lantern Procession" "Parish"
performance bind place chapel here
performance bind place square north
performance bind exit chapel-square north
performance bind prop lantern lantern
performance cast leader Ana
performance cast bearer Tomas
performance cast petitioner Elin
```

Walk north to the square, bind `performance bind place shrine north` and `performance bind exit square-shrine north`, then open the call. Elin signs in at the chapel with `performance signin "Feast procession" petitioner`. At the first travel cue she walks north. At the square she gives her petition and uses `performance done`. She walks north again when cued. The bearer gives the lantern to the leader at the shrine. A missing petitioner holds a required role open through the latest start.

To rehearse a mid-procession handover, cast a player as leader with `performance cast leader <player> fallback <npc>` and have the player sign in at the chapel. Once the leader reaches the square, have them leave by a side street while the petitioner is due. The understudy must travel from the chapel to the square before taking the next direction. The organiser's `performance status` shows the vacant part during that walk and the new leader on arrival. The opening address is already complete.

To rehearse a blocked route, post a guard on the square's shrineward exit after the leader crosses. To rehearse loss of the lantern, have someone take the bound item from the bearer at the square. Removing the obstruction or returning the actual lantern during the cue's grace period allows the procession to continue. On timeout the organiser can inspect the failed cue and the named leg or prop. The local NPC cast can deliver the failure line at each occupied station. If no cast NPC there can deliver it, the room gets a cancellation notice and an able cast player receives the suggested words privately. A guard posted independently keeps their post. A bystander who took the lantern keeps possession after cancellation.

### Putting the venue and cast back in order

The last cue records the show outcome immediately. On cancellation the cast first gives the local failure announcement. In both cases the stage feed closes, the music stops and ushers release their posts before anyone starts packing up. Use `performance status` to see outstanding returns alongside the outcome and, afterwards, which returns need attention.

| What the builder wants | How to stage it |
| --- | --- |
| The lantern goes back to the chapel | Start with the lantern on the chapel floor or inside an accessible chest. Use a Get cue and the normal procession handoff. The NPC who ends up holding it walks back and drops it at its original room and layer or puts it in the same chest. |
| A personal item goes back to its lender | Use Give while the actor holds their own prop. A cast NPC with the item can hand it back to the original lender. An NPC lender stays available for the handoff. A PC who ends with a borrowed prop receives a private reminder and decides what to do. |
| A gift stays at the shrine | Before publishing, use `performance work edit "Lantern Procession" prop cleanup lantern leave`. Use `return` to change it back. Props which the work only binds or plays have no loan to repay. |
| Ushers go back to work | By default each NPC walks to the room and layer they occupied when first called, including an understudy sent to a mark. Their usual AI then takes over and can choose their next duty. To dismiss an usher where they finish, use `performance work edit "Here Comes the Sun" role cleanup usher-stalls release`. Use `return` to restore the walk back. An NPC carrying borrowed equipment deals with it before dismissal. |

Returns use ordinary walking and inventory actions. A bystander keeps anything they took. Closed storage, a blocked route or full hands can be dealt with through play until the cleanup deadline. The organiser gets a manual-return entry if the item is missing, the named recipient is unavailable, or the deadline expires. Combat, incapacity and external control release an NPC in place. A PC always chooses their own destination.

Cleanup gets one cue grace period from the end of the show, two real minutes by default, shared by all return trips. Travel and handoffs do not extend it. Production status keeps pending returns separate from the artistic outcome. The same production can open another call once returns have been resolved or released. A restart resumes recorded returns against their original deadline. Bookings made before return tracking gained these fields can still load, and an interrupted older run with no recorded origins can only release its existing effects.

### Scheduling and prototype boundaries

For a future call use `performance book once "<local date and time>"` followed optionally by a native recurrence such as `every 1 week`. For example, the rehearsal calendar accepted `performance book once "17/lavoisier/55 03:00 EST"`. Use your own calendar's date and clock zone. Reselect the production after logging back in to see its advertised start. For a conditional call use `performance book when <public boolean prog accepting a room>`. The prototype evaluates that condition against the booking room and requires it to go false before it can trigger another call. Stage links can also be managed through `stage open <source> <audience> <label>`, `stage list`, `stage close <handle>` and the two FutureProg functions below.

The prototype uses named performers, one nominated NPC understudy per part, individual props and ordinary rooms with layers. The call opens 600 in-game seconds before kickoff and permits 300 more for late arrivals. Each cue has a two-minute real-time deadline for an unmet condition. The planner reserves named people, props and the platform for the current occurrence, with a conservative duration based on those deadlines and the venue clock. Each recurrence is checked again. A later call checks whether the cast and borrowed equipment are still returning.

The longer design below includes candidate pools, custom grace policies, employment task integration and consequence progs as possible extensions. The three fixtures are short playable excerpts. Builders supply the script and music appropriate to their setting.

### What the rehearsals demonstrated

| Rehearsal | Observed in the disposable game world |
| --- | --- |
| Parish High Mass | The call waited for sign-in. The server carried the missal into the sanctuary, the reader improvised and yielded, and a physical handoff advanced the cue. When the reader withdrew mid-reading, the understudy took the part and the former reader retained the book. Rehearsal could continue once the understudy actually received it. |
| Here Comes the Sun | Both ushers walked from a separate foyer to their assigned audience rooms. Ordinary patrons were stopped at the stageward exits and could start walking away by an unguarded exit. The handheld guitar had to reach the guitarist's hands. Playing continued through the sung line, which a spectator received with the platform prefix. The finale released the guards and the production's stage links. |
| Lantern Procession | The petitioner stayed in the chapel while the others moved ahead. The cue held until the player crossed to the square. They improvised the petition, walked to the shrine, and witnessed the bearer give the lantern to the leader. |
| Leader leaves the square | The PC leader gave the opening address, took the first leg and left by a side street. Their pending commands could no longer advance the procession. The NPC understudy walked from the chapel to the square. When the departed PC temporarily held the lantern, the run waited for its return. The petitioner then yielded, and the bearer handed the same lantern to the understudy at the shrine. |
| Guard blocks the shrineward exit | A guard took the square post after the leader had crossed. The roster placed the leader at the shrine and both remaining performers at the square. The bearer waited at the named leg. At timeout the leader and bearer each spoke once to their own station. The chapel witness heard nothing and the independent guard kept their post. |
| Lantern carried away | While the petitioner had the cue, the bearer gave the bound lantern to a bystander who carried it back to the chapel. The cue waited for the bearer to recover it. When the two-minute grace expired, the leader announced the cancellation once in the square. The petitioner lost the active cue, and the bystander still held the lantern in the chapel. |
| Clearing the procession | The NPC leader was called from the square and the bearer from the foyer. At the finale the leader took the lantern at the shrine, walked it back to the chapel floor, then returned to the square. The bearer returned to the foyer. The PC petitioner stayed at the shrine. On a separate cancellation a bystander kept the lantern, both NPCs returned, and status recorded the outstanding manual return. |
| Guarded return across restart | The bearer borrowed the lantern from a bag on the chapel floor. After the finale a guard blocked the leader's trip back to the chapel. The completed outcome, original bag, current holder and cleanup deadline survived a server restart. When the guard stood down, the leader put the same lantern in the same bag and returned to the square. |
| Interrupted concert | The server stopped with two guards, four NPC claims and two performance-owned links active. On reboot the production recorded the interruption, all six character effects and both owned links cleared, and an independently opened link between the same rooms survived. Earlier completed productions retained their results. |
| Conditional call | An organiser authored and published a short work through the editor. A false public condition kept the plan booked. Rebooking it with a true condition opened registration, and repeated evaluation retained the same occurrence. |
| Player-authored concert | An ordinary clan organiser created a concert from the fixture, edited a role's public eligibility rule and the cancellation script, and published it. The organiser bound the four rooms, two stageward exits and guitar, cast four employees and ran the concert to its final cue. Attempts to use a numeric room ID, a concealed exit, an incompatible or private prog, an unauthorised NPC and a venue outside the host's control were refused. |
| Future booking across restart | The organiser booked the same production for 03:00 on the venue clock. After a process restart, the organiser could reselect the booked production and see its start. The rehearsal clock was advanced to just before the call window. Heartbeats opened registration, started the cast at the advertised time, and carried the concert through its final cue without a manual call or start. |
| Private eligibility on a published work | The player could inspect the concert and copy it. The hidden eligibility rule appeared as unavailable. The copy required a replacement before it could pass validation, and the player supplied a public character predicate by name. |
| Stage sight and hearing | Two ordinary spectators saw different platform names from the same trait-gated room title. A blind listener heard the singer and audible guitar. Playing a silent guitar produced visible action for one patron and no music for the blind patron. A deaf spectator saw the singer and gestures without the words. With both impairments the spectator received neither. Unfamiliar speech remained unintelligible and whispers were absent. |

For stage output, a sighted patron can see a bow or a singer moving their mouth. Hearing alone carries speech and song in the listener's own language comprehension, plus perceptible instrument sound. A speaker's accompanying visual emote requires sight. Unclassified public room text uses sight. Quiet speech and whispers stay off the stage link. A room broadcasting a performance uses its recipient-specific short name when visible and the label “Stage” when the patron has only sound. Independent links keep the label their creator supplied.

The case for an engine feature rests on coordinating a mixed PC and NPC cast, advancing on observed actions across several rooms, and helping the organiser recover a disrupted performance. Timed speeches are already well served by recurring prog schedules and delayed callbacks. The prototype earns further investment if builders can stage the three productions below without writing coordination progs. [Recurring schedules](../../MudSharpCore/FutureProg/ProgSchedule.cs) · [Delayed progs](../../MudSharpCore/FutureProg/Statements/DelayProg.cs)

## What the builder, stager and performer own

| Record | Who uses it | Contents |
| --- | --- | --- |
| Work and revision | Player author or staff builder | Named roles, eligibility, required and optional parts, named places and directional exits, prop requirements, cue sheet and failure instructions. Publishing validates references. |
| Production plan | Stager | Work revision, sponsor, casting pool and invitations, bindings to actual rooms, exits and props, start trigger, call time and latest start. Another parish or theatre can bind the same work to its own people and building. |
| Performance | Stager and cast | One occurrence, pinned revision, actual physical participants and items, current cue, outstanding conditions, deadlines, effects acquired by the run and outcome. Editing next week's plan cannot alter tonight's show. |

Clans already implement the employment-host interface. A parish clan or another host can sponsor a plan and authorise use of its staff and property. A guest needs no employment contract. A role can qualify through appointment, membership, employment, skill or an approved eligibility prog. The parish decides who qualifies as its celebrant. An employment action can request a production, following the precedent of arena administration. Individual staff tasks can fetch props. The performance owns the cast and cue clock. [Clan contract](../../FutureMUDLibrary/Community/IClan.cs) · [Arena employment action](../../MudSharpCore/Economy/Employment/EmploymentActionSteps.cs)

### Casting and the start call

Every role has a control mode, independently of whether the role is required.

| Mode | Advance casting | At the advertised start |
| --- | --- | --- |
| NPC only | Select an eligible NPC or let the plan draw from an authorised pool. | Use an available NPC who has reached the role's starting mark. |
| PC only | Invite a named PC. | Give the part to the attending PC. Hold for a missing required part until the latest start, then cancel. Omit an absent optional part and its cues. |
| Agnostic | Cast a PC ahead of time and nominate an eligible NPC understudy or pool. An NPC can also be the first choice. | Give the attending PC their part. If the PC has not turned up, use a ready NPC understudy. Without one, apply the required or optional policy. |

During gathering, `performance calls` at a registration mark shows the imminent productions and volunteer parts open to that character, including standby requests. A pre-approved PC uses `performance signin <production> <role>` in person. An eligible walk-up uses `performance volunteer <production> <role>` at the same mark. Volunteering registers the PC's willingness for that part. The orchestrator can assign them without making them sign twice. An advance assignment reserves a part but conveys no attendance. Being present or being assigned by the orchestrator never signs a character in.

Until the run actually starts, the orchestrator can use `performance assign <production> <role> <person>` and `performance remove <production> <role>`, including during a delayed start. Notify anyone displaced, clear their claim, and require the replacement's registration for that role. A signed-in PC who leaves the mark before kickoff must sign in again on returning. Launch rechecks mode, eligibility, physical presence, layer, ability and active player control, then commits the cast and changes the run to performing as one operation. An assignment racing kickoff either takes effect before that transition or receives an “already started” result.

**Early departure.** A player can use `performance withdraw` at any time. Disconnect, incapacity or leaving a required mark unexpectedly also puts the affected cue on hold. Scripted travel is expected progress. In an agnostic part, offer the next unfinished direction to a ready eligible NPC at the next safe handover. The NPC must be present with any required props. If the departing player still has the missal, the replacement must obtain it through play. Completed speech, transfers and consequences stay completed. Revoke the departing player's pending cue so a late `done` cannot advance the replacement's part. For a PC-only part, use an eligible registered PC replacement if available, skip an optional part, or hold a required part until its deadline and cancel. Rejoining after kickoff follows the same safe handover rules.

An agnostic role has **one cue sheet**. An NPC executes the authored direction using the bound targets. A PC privately sees the same direction, suggested words or movement, target and completion condition. They may follow it, improvise, wait or withdraw. For a reading, the player can yield the floor with the proposed command `performance done`. For a handoff, the intended recipient must actually receive the item. Typing `done` cannot move the book.

### Editing a cue and booking a run

Player authoring follows the item-skin precedent of actor-specific help, revision ownership and editing access. A player can create a work, edit their own revisions, define roles and cues, validate it and make it available for staging. Work authorship and authority to stage a production are separate. Creating a concert gives its author no claim on another host's staff, props or premises. [Item skin authoring](../../MudSharpCore/Commands/Modules/ItemBuilderModule.cs)

Store one canonical definition and one set of stable bindings. Pass the acting character into the methods that **list, resolve, assign and describe** references. Those methods select the player or administrator path using current authority. Use them for editing, search, cloning, previews, roster reports, validation errors and clickable commands. The runtime consumes the resolved bindings. It must never reparse player-entered text as administrator input.

| Field | Player editor | Administrator editor |
| --- | --- | --- |
| Place or exit | Stand in the hall and bind a place with `here`, or bind an adjacent place through a visible relative exit such as `north`. Resolve from the author's actual room, layer and perception, then store the resolved room or directed exit. Display the symbolic mark and player-visible description. | May additionally resolve explicit room IDs and inspect exact endpoint identities. |
| Prog | Select by player-visible name from public progs compatible with that field's return type and parameters. Use the actor-aware lookup at assignment and revalidate the binding on publication and before use. A private or subsequently hidden prog produces the same player-facing failure as an unavailable reference. | Can select compatible private progs and inspect technical diagnostics. |
| Cast, props and host | Resolve visible people and items or candidates and assets the host authorises this stager to select. Show the availability of known resources. For a hidden competing production, report “unavailable” without disclosing its cast, room or host. | May inspect exact records and conflicts using staff tools. |

Use typed cue actions with the authority of the participant and host. Prog visibility determines what a player may discover. Each field defines what a selected prog can be used for, for example a boolean role eligibility check. A player cannot turn that reference into an arbitrary privileged callback. Apply the same rules when a player copies a staff-authored work: omit inaccessible bindings from the editable copy and require substitution where needed. Validate player and staff forms through the same domain operation after resolving their inputs. [Actor-aware prog lookup](../../MudSharpCore/FutureProg/ProgLookupFromBuilderInput.cs) · [Perceiver-relative exit resolution](../../MudSharpCore/Commands/Modules/PerceptionModule.cs)

The commands below are proposed editor interactions. Example names are local content. Existing game verbs such as `guard north`, `play` and `sing` retain their normal meaning. The builder can enter one row per cue, attach as many role directions to that row as needed, and use the multiline editor for speech and emotes.

| Cue field | Builder choice |
| --- | --- |
| Who and where | One role, several named roles, or the production for an operation such as opening a stage link. Bind a mark or directed exit where needed. |
| Direction | Speak, sing, emote, travel a bound leg, give a bound item, start or stop playing an instrument, guard or release an exit, open or close a stage link, or wait. Role and prop references resolve to perceivables. |
| Advance when | A player yields, an NPC completes its direction and pacing, an action succeeds, a duration elapses, or all specified conditions hold. Sustained actions can continue across later cues. |
| If interrupted | Choose continuing requirements, grace period and disposition. Hold, skip an optional part, offer a substitute at this boundary, or cancel. Identify an optional failure speaker, backup and the rooms where the line can be delivered. |

Publishing catches unknown references, mandatory cues that depend on omitted roles, and waits without deadlines. A work supplies an inherited deadline and failure policy for each kind of wait. For these fixtures, use 45 elapsed seconds for a disrupted physical action, two minutes for a player's turn and three minutes for a travel leg. The stager can explicitly extend a hold. The NPC pacing delay starts after the NPC successfully delivers the line.

Preview displays NPC directions alongside PC prompts. Booking examines the whole roster. If two people can read and only one can officiate, preserve that candidate for the scarce role. Acquire confirmed performers, props and exclusive venue slots together across competing bookings. Reserve a designated understudy through their last covered cue so they can replace a PC who leaves early. Show pool-only cover as conditional on an eligible NPC still being available. Audience rooms and public route legs can remain shared. A reserved guitar can still be stolen.

The stager's report distinguishes booked, travelling, arrived and ready. During gathering, NPCs walk to their marks and staff retrieve props through ordinary access. Recheck the actual people, custody, quantities, route and venue immediately before each dependent action. For example, *Reader ready in sanctuary. Missal still in sacristy cupboard. Server cannot open cupboard. Holding for 45 seconds.* Stagers can inspect the missing condition, correct it through play, substitute at an allowed boundary, or cancel.

## Worked production 1 — a parish High Mass

This shortened High Mass fixture exercises the reading and handoff. A builder supplies the parish's full liturgical order and texts for a complete production. For the prototype, prepare a parish, eligible celebrant, server, PC reader, NPC understudy, missal, and two rooms joined by a traversable exit.

```text
performance work new "Parish High Mass"
performance role add celebrant npc-only required eligible ParishCelebrant
performance role add reader agnostic required
performance role add server npc-only required
performance place add sacristy
performance place add sanctuary
performance prop add missal quantity 1 reusable
performance cue add 1 celebrant at sanctuary speak "We gather for the reading." advance after 4s
performance cue add 2 server at sanctuary give missal to reader advance when reader holds missal
performance cue add 3 reader at sanctuary speak "A reading for this feast." advance on yield npc-after 8s
performance cue add 4 reader at sanctuary give missal to server advance when server holds missal
performance cue add 5 celebrant at sanctuary speak "Let us pray." advance after 4s
performance failure speaker celebrant backup server "We will pause the service here."
performance work validate
performance work publish
```

1. Create **Sunday parish service** from the published revision. Select the parish clan, bind its sacristy and sanctuary, and select the actual missal and its cupboard. Set the sanctuary as all three starting marks. Give the server the preparation direction to collect the missal and bring it to the sanctuary. Cast qualified NPCs as celebrant and server.
2. Invite Elin to read and nominate an eligible NPC understudy. Book the service on the parish calendar's weekly rest day at 09:00 local time. Open the call ten game minutes beforehand and allow five game minutes for a required shortage. Preview shows whether the server can obtain the book and whether each performer and understudy can reach their mark.
3. Elin accepts and checks in. At cue 3 she sees privately, *Reader, at the sanctuary lectern. Suggested opening: “A reading for this feast.” You have the missal. Deliver the reading in your own words. Use PERFORMANCE DONE when you yield the floor.* Her words go through ordinary speech. The celebrant waits even if she takes longer than the NPC's eight seconds. At cue 4 the server must actually receive the book.
4. Stage the same revision with Elin absent. At the advertised start the ready understudy takes her part and delivers the authored reading. In a third run, remove the book before cue 2. Allow the server to recover it during the handoff's grace period. On timeout, the able celebrant gives the failure line and the run releases its claims. The record names the failed handoff and missing book.
5. During another call, local volunteer Tali registers for reader. Before kickoff the stager removes Elin and assigns Tali. After delivering the reading, Tali withdraws before returning the book. The understudy receives cue 4 once the book is in their hands. The congregation hears the reading once. If Tali walks away with the book, the stager sees a custody hold and can ask for its return.

## Worked production 2 — a concert with ushers

For **Here Comes the Sun**, the hall has a platform, front stalls, upper gallery and foyer. The stalls and gallery each have a stageward stair and a separate way out to the foyer. The band assembles on the platform. The opening cue deploys ushers from the foyer.

```text
performance work new "Here Comes the Sun"
performance role add singer agnostic required
performance role add guitarist npc-only required
performance role add introducer pc-only optional
performance role add stalls-usher npc-only required
performance role add gallery-usher npc-only required
performance place add platform
performance place add stalls
performance place add gallery
performance place add foyer
performance exit add stalls-stairs from stalls to platform
performance exit add gallery-stairs from gallery to platform
performance prop add guitar quantity 1 reusable requires playable-instrument
```

| Cue | What the builder enters | What releases the next cue |
| --- | --- | --- |
| 1. Take posts | Send each usher from the foyer to their assigned audience room. At their post, guard its bound stageward exit. | Both ushers have native guard effects on the correct outbound exits at their marks. Both posts remain required through the finale. |
| 2. Open the stage | Enable platform sight and sound for stalls and gallery. Have the introducer welcome the audience from the platform if present. | Both links active. The introducer yields, or the optional part was omitted. |
| 3. Instrumental opening | Guitarist starts playing the bound guitar in one of its supported styles. | That performer is playing that item. Failed checks or occupied hands give the stager a reason to hold. |
| 4. Vocal entry | Give the singer their opening vocal direction while the guitar continues. | The singer yields or the NPC finishes the authored passage. Guitar must remain in play. |
| 5. Finale | Cue the guitarist to stop and the performers to bow. Keep the links open for the bow. | The instrument stops and the bow cue completes. Close this run's stage links and release its guards. |

For the player-author fixture, the bandleader writes and publishes their own revision, then makes a one-off booking using their authority at the Assembly Hall. Bind the foyer while standing in it. Bind the stalls while standing there and bind the platform through the visible north stair. Walk to the upper gallery and bind it there. Bind each **outbound** stair from its audience side. Bind the guitar and its performer, and select two available hall ushers with routes from the foyer to their posts. Reject an usher already committed to another guard duty. Assemble performers before the guard cue or use a backstage entrance. The following excerpts show the player-facing inputs at the respective rooms.

```text
performance plan new "Friday concert" work "Here Comes the Sun" revision 1 host "Assembly Hall"
performance bind place foyer here
performance bind place stalls here
performance bind place platform north
performance bind exit stalls-stairs from stalls north
performance bind place gallery here
performance bind exit gallery-stairs from gallery up
performance cast singer "Mira" fallback "Jo"
performance cast introducer "Ren"
performance preview
performance book once <local date and time>
```

At curtain a patron in the stalls sees their usher take position. Attempting `north` meets the ordinary guard. The patron can still use the foyer exit, and a performer can come off stage in the reverse direction subject to normal movement rules. A patron entering the gallery midway receives the next perceptible stage action automatically. A present Ren chooses their own introduction. An absent Ren is omitted.

Use the native operation behind `guard <direction>` and confirm the resulting effect on the usher and actual exit. New exit guards start with ally passage disabled. Native exemptions can extend to a traveller's party. Worlds with explicit hostile guard challenges retain their usual `!!` behaviour. Movement, melee or incapacity can break a post. Hold and attempt a substitute if a post is lost. Track the precise guard effect acquired by this run, including durable ownership for reboot cleanup. A newer, unrelated guard assignment must survive the curtain call. [Guard command](../../MudSharpCore/Commands/Modules/CombatModule.cs) · [Exit guard](../../MudSharpCore/Effects/Concrete/GuardingExit.cs) · [Hostile guard challenges](../Combat/Hostile_Guard_Challenges.md)

## Worked production 3 — a procession through town

Create **Procession of the Lantern** as an independent work. Add a required agnostic leader, required NPC lantern bearer, required PC-only petitioner and one reusable lantern. Define chapel, square and shrine places and two directed route legs. The stager binds actual public rooms and exits, casts the petitioner, and selects a lantern from chapel stores. Onlookers may watch at any stop or walk alongside the procession under their own control.

| Cue | Direction | Completion or failure |
| --- | --- | --- |
| Muster at chapel | Bearer takes the lantern. Leader addresses those present. | All three cast members are at the starting mark and the bearer has the lantern. A missing petitioner holds until latest start. |
| Chapel to square | Leader steps out first. Cue bearer and petitioner to take the first exit. | Wait at the square for three physical arrivals and the lantern. Each departure and arrival is perceived in its actual room. |
| Station in the square | Petitioner offers a petition in their own words. | Their private cue invites them to yield. A passer-by speaking cannot advance it. |
| Square to shrine | Cast walks the second leg. | Recheck the exit at departure and wait for all at the shrine. A gate closing mid-procession identifies the blocked leg and separated performers. |
| Arrival | Bearer presents the lantern. Leader closes. | Require bearer and lantern at the shrine. Finish and release claims. |

Publish and preview the route. Stage it once manually, then add a conditional call: an authorised host rule requests the procession when the feast-day condition first becomes true. Give this occurrence the plan and feast date as its key. Repeated evaluation during the feast produces one call. If a gate closes after the leader crosses, show the stager who is on each side. Give participants time to open it through ordinary play. On timeout the leader at the shrine and an able backup in the square can explain the cancellation locally. A bystander in the empty chapel hears nothing. Recovery leaves the lantern with its actual holder.

## Stage viewing as an independent room feature

A stage is a **directed link from a source room to an audience room**. One source can serve several rooms. A run can open links at curtain and close them after the bow. An administrator can open a stage for an unscripted play. A FutureProg can open or close a link without creating a performance. All three entry points use the same validation, ownership and delivery rules.

| Entry point | Proposed operation |
| --- | --- |
| Cue editor | `open stage platform to stalls gallery` acquires handles owned by the run. `close stage` releases those handles. |
| Staff command | `stage open <source> <audience>`, `stage list [room]`, `stage close <link-id>`. Listing shows both endpoints, source label, owner and lifetime. Staff may explicitly close an identified link. |
| FutureProg | `openstagelink(source, audience, label)` returns a handle ID on success. `closestagelink(id)` reports whether it closed a live handle. The caller can retain the ID for later cleanup. Offer these to trusted progs. Production authors use the plan's authorised venue bindings. |

Attach an observation effect to the source and enumerate eligible occupants of the audience room for each output. A spectator can arrive, leave, go unconscious or change seats without subscribing. A performance and an administrator can independently open the same pair. Deliver one copy and keep the pair active until its last handle closes. Manual and independent prog links should survive until explicit close or their configured expiry. Release performance-owned links when the run ends. Remove stale endpoints when rooms are deleted or temporary venues retire.

There are three relevant precedents. The existing room observation effect already finds people in another room, but deliberately shows only the visible act of speaking. Arena watchers receive structured output rendered for each spectator. The room output router discovers observation effects and honours watcher suppression on structured output. Introduce a stage-specific effect and delivery policy, preserving the narrow visual contract of existing observation links and the privileges of administrative spying. [Room observation](../../MudSharpCore/Effects/Concrete/RoomObservationEffect.cs) · [Arena watcher](../../MudSharpCore/Arenas/Observation/ArenaWatcherEffect.cs) · [Room output routing](../../MudSharpCore/PerceptionEngine/OutputExtensions.cs)

- Forward ordinary room-wide echoes by default, including public room text, unscripted entrances, spoken lines and onstage interruptions. Use the watch and arena presentation forms: prepend a bracketed, viewer-visible source area to the original output, for example *[Hall Platform] The guitarist bows.* Preserve perspective, language comprehension, sight, hearing, concealment and notice rules for structured output. Send ordinary public room-wide text with the same prefix when its delivery scope and source mark are known. Direct messages, private cues, whispered words, staff traffic, electronic-only output and watcher-suppressed echoes stay off the feed. A deaf patron may see a singer begin. A blind patron may hear the words without identifying the singer by sight. [Watch presentation](../../MudSharpCore/Effects/Concrete/Watch.cs) · [Arena forwarding of room text and structured output](../../MudSharpCore/Arenas/Observation/ArenaWatcherEffect.cs)
- Deliver straight to the observer. The relayed line cannot trigger another stage link, a room hook, duplicate recording or second physical witness event. It grants no remote targeting or movement permissions. When a guitar also generates an ordinary distant sound in the stalls, deliver at most one rendition of that action to each patron. The local emote and distant sound are separate native emissions, so retain an emission identity or equivalent context across both paths. Deduplicating by text would suppress genuine musical repetitions. [Instrument output](../../MudSharpCore/GameItems/Components/InstrumentGameItemComponent.cs)
- Bind each endpoint to an ordinary room and layer in the prototype. Verify the origin before forwarding. Reject route-coordinate stage bindings until the router can represent their origin and audience precisely. The procession can traverse multiple ordinary rooms independently of stage viewing.

## Runtime and recovery

Use game calendar listeners for scheduled calls, condition evaluation or events for conditional calls, and elapsed time for cue pacing. Persist an occurrence key before accepting a duplicate trigger. Record expired calls as missed after downtime. Persist the pinned revision, physical actor and prop bindings, run state and reason, and ownership of durable effects. A single coordinator advances the current cue. Tag callbacks with the run and cue instance so that a stale arrival or timeout cannot advance a later cue.

Keep the NPC's controller and add a temporary performance assignment. Voluntary wandering and employment claims defer while an NPC is committed. Account for existing movement and independently ticking group AI before casting. Combat, injury and escape behaviour can interrupt a performance. On release, the NPC returns to its normal duties. A player executes their own actions throughout. Match cue observations to the physical participant and exact prop or exit, then verify the result. [NPC event dispatch](../../MudSharpCore/NPC/NPC.cs) · [Group AI heartbeat](../../MudSharpCore/NPC/AI/Groups/GroupAI.cs)

On failure, stop accepting normal cues. Give an able, present speaker the failure direction for their location. A show can leave the link open briefly for a cancellation line from the platform. A missing or silent speaker cannot hold cleanup hostage. Stop the playing, guard and viewing effects acquired by the run, release claims and record the reason once. Consumed supplies, opened doors and stolen property retain their world state. On restart cancel interrupted live runs and reconcile their owned effects before the next booking. Retain future bookings. A later completion hook that records a vow or changes standing needs a durable invocation record and an inspectable interrupted outcome. [Arena restart recovery](../../MudSharpCore/Arenas/Lifecycle/ArenaLifecycleService.cs) · [FutureProg notification recovery](./FutureProg_Execution_Runtime.md)

## Prototype acceptance

1. A builder publishes all three works through the cue editor. Another stager can book them by binding local people, places and items. Preview names a double-booked person or prop and a missing or unusable route.
2. Stage the Mass with a present PC who improvises and with an absent PC and ready NPC understudy. Compare the underlying cue and target. Remove and recover the missal during the grace period, then repeat and let the run cancel. Check the concert's absent optional PC and the procession's missing required PC. No PC acts automatically.
3. Start the concert with two ushers. Check both audience-to-stage exits, reverse travel and foyer egress. Interrupt a guard, substitute another usher, then repeat without a replacement. Confirm cancellation releases only this run's guards. Run guitar and singer together, interrupt the instrument and inspect the hold.
4. View an unscripted stage emote and concert through both audience links. Check late arrival, departure, blindness, deafness, unfamiliar language, whispers, private cues and ordinary distant instrument audio. Exercise independent staff and prog open/close, overlap with a scripted link, and form a relay cycle. Verify one perceptible copy per action and correct ownership on close.
5. Stage the procession across all three rooms. Hold at the square for a lagging PC, then close the second leg after the leader crosses. Check what each room sees on cancellation. Re-evaluate the same feast trigger twice. Restart a concert with guards and links active and verify release, future bookings and absence of repeated side effects.
6. A player authors the concert and binds the hall using `here` and visible directions. Exercise both a public compatible prog and a private one. Try a raw room ID, a hidden exit, an incompatible public prog and a copied staff-only binding. Check help, list, show, clone, preview and error output for undisclosed IDs, private names and hidden destinations. Repeat as staff and confirm that both permitted editing paths produce equivalent runtime bindings.
7. At an open call, try a pre-approved PC who is present but unsigned, an eligible walk-up, and an ineligible volunteer. Assign, remove and replace a PC just before launch. Try an assignment after launch. Withdraw a signed-in PC before kickoff and an active volunteer during a cue. Verify that the ready NPC covers an agnostic role, a PC-only part retains its mode, and stale acknowledgements cannot duplicate dialogue or physical actions.
8. Send an ordinary public room-text echo and a structured, unscripted stage action. Check the visible source-area prefix for both and verify that private or watcher-suppressed messages do not reach the audience.

Proceed beyond the prototype if a builder can change cast, pacing or venue without scripting coordination, a player can improvise while physical outcomes remain meaningful, and a stager can diagnose an interrupted show in game. Stage viewing can ship independently if it proves useful for unscripted events.
