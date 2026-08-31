# Computer and Electronics A/V Framework

## Scope

The media framework is the typed A/V layer between physical sensors, local connectors, computer hosts, telecom-backed feeds, durable recordings, and playback devices. It is deliberately separate from the numeric `ComputerSignal` automation bus. A source publishes regardless of whether it has zero, one, or many consumers; displays, recorders, splitters, network gateways, and physical decks independently subscribe through stable endpoint bindings.

Version one supports audio, video, and combined A/V. It records observable in-character output, structured spoken and signed language, visual/audible events, and canonical scene snapshots. It does not model tape-head position, rewind, editing, transcoding, a historical live backlog, or a global recording-duration cap.

## Runtime model

`MediaEndpointAddress` identifies an endpoint by item id, component id, endpoint key, and direction. `MediaCapabilities` is a flags value (`Audio`, `Video`) and `MediaPacket` is an immutable ordered event with a stream id, sequence, UTC timestamp, capabilities, source, provenance, and typed payload.

Language packets retain language/accent or variety, outcome, volume, raw text, pre-language and optional emotes, and an immutable speaker identity snapshot. Playback uses the normal language output path, so a recipient applies current language comprehension rather than seeing a recorder-side translation. Signed language is visual; spoken language is audio. Ordinary audible and visual IC output is retained as sensor-viewpoint text, and a scene packet contains canonical full-look output plus its SHA-256 content hash.

`MediaChannelService` routes packets only to active compatible sinks. It rejects invalid output endpoints, refuses a stream that has already visited a destination, and applies a 16-hop provenance limit. A component can capture another display once when it is legitimately observable. If a camera attempts to recapture loud playback whose provenance already contains that camera, the cycle is terminated and the playback device emits one very-loud local electronic-feedback echo. That feedback is marked non-capturable and is not propagated to adjacent cells, so it cannot seed another media or noise loop.

Capture is performed by a purpose-built camera/microphone sensor perceiver. It follows normal location, layer, plane, closed-visibility, illumination, audibility, and output visibility rules. OOC, staff-only, ignored-watcher, and non-normal output is not captured. A powered camera captures a canonical frame immediately when a consumer binds and then at its configured interval, never below five seconds. Consecutive equal snapshot hashes extend a frame range rather than duplicate scene storage.

Automatic crimes also enter video as a content-free `CrimeWitness` packet linked to the authoritative crime id. A camera emits that packet only when its video sensor can see the offender in the crime location. The packet survives live routing, computer files, network feeds, and physical-media copies. A character is added to the crime's witness list only when they actually see that packet through an ambient or opted-in monitor, or inspect the corresponding five-second stored still through the Media application; linkdead player bodies and audio-only playback cannot confer witness status. The ordinary `WitnessedCrime` event is raised at viewing time, so witness-profile and AI behavior remains consistent with direct observation. Filmed crimes are initialised immediately to obtain a durable id before the ordered packet is written.

## Persistent recordings

`MediaRecordings` stores immutable recording metadata, capabilities, status, duration, and logical compressed size. `MediaRecordingChunks` stores ordered, versioned UTF-8 JSON packet batches as Brotli payloads. A runtime buffer flushes at five seconds or 64 KiB uncompressed, so an abrupt process loss discards at most that current buffer. `MediaSceneSnapshots` stores Brotli-compressed canonical scenes keyed by SHA-256. `MediaRecordingFrames` maps time ranges to those deduplicated snapshots. `MediaRecordingReferences` names a recording under a game-item component, allowing the same immutable recording to appear in more than one computer filesystem or physical medium.

Recordings begin active, finalise normally or as interrupted/failed, and cannot be altered after finalisation. Startup recovery marks orphaned active rows as interrupted. Power loss and orderly shutdown interrupt active recorder/deck jobs. Deleting a reference removes the recording and unreferenced snapshot blobs when it was the last reference; that work occurs in one database transaction. Computer quota charges each media-file reference its complete logical compressed size, including referenced snapshot data even when blobs are deduplicated. Physical media instead enforces configured duration capacity.

Text computer files remain XML-backed. `IComputerFile` identifies `Text` versus `Media`, carries an optional recording id for media, and rejects text read/append operations clearly. File Manager and FTP can list, copy, move, and delete media references without interpreting them as text. A copy creates a second immutable reference and is charged at full quota; a move is a safe reference copy followed by deletion.

## Physical components and routing

All media-capable components use normal `IConnectable` topology conventions while media traffic remains separate from `ComputerSignal`.

- `Camera` is a powered video or A/V capture source with sensitivity, stable output endpoint, port count, and a snapshot interval of five seconds or longer.
- `Push To Talk Microphone` is a powered audio source implementing `ITransmit`; `transmit` and `transmitwith` publish typed spoken-language packets.
- `Media Monitor` is a video or A/V sink. `ambient` prototypes relay eligible playback into the cell; opt-in models are watched with `watch feed <monitor>` and stopped with `watch feed none`. `LOOK <monitor>` always includes the latest live/playback frame. Audio presentation can be independently disabled, and its output volume can be scaled from silent through dangerously loud.
- `Media Speaker` is an audio sink for decks, boomboxes, microphones, and computer output, with the same adjustable output-volume range.
- `Computer Media Interface` exposes named Media-application inputs/outputs through a sibling `ComputerHost`.
- `Media Deck` records, plays, or both, requires a compatible inserted `MediaStorageMedium`, and has no tape-position state in v1.
- `MediaStorageMedium` has a format key, capabilities, duration capacity, write protection, and named immutable recording references. Duplicate names require explicit erase.
- `Media Cable` and `Media Splitter` are passive relay components. A cable has one input/one output; a splitter has one input and multiple outputs for fan-out.

Composite items can bind sibling endpoints explicitly. A TV/VCR is a `MediaMonitor` plus a VHS `MediaDeck` whose sibling output is enabled. A boombox is a compact-cassette `MediaDeck` plus `MediaSpeaker`. Ordinary container operations insert and remove physical media.

Installation uses the ordinary player-facing item workflow: place compatible devices together, use `connect <item> <item>` for each camera/cable/splitter/monitor/deck link, use `disconnect <item> <item>` to change it, and then switch powered endpoints on. These commands run the same port, capability, colocation, and free-connector validation as builder-created fixtures. Builders should not persist endpoint XML or database rows as a substitute for proving the player workflow.

The common physical command surface is:

```
media <item> status
media <monitor|speaker> volume <silent|faint|quiet|decent|loud|very loud|extremely loud|dangerously loud>
media <medium> list
media <deck> record <name>
media <deck> play <name>
media <deck> stop
media <medium> erase <name>
```

Spoken packets retain their authored speech volume and ordinary audio packets retain the source `AudioVolume`. A sink's `decent` setting is unity gain; other settings shift the source volume up or down and clamp it to the engine range, while `silent` removes audio without suppressing video. Every audible monitor or speaker presentation raises the normal `NoiseEmitted` event. Output below `loud` remains local; `loud` and higher output uses the existing cell/RouteCell audio propagation system and can be heard, with normal attenuation and direction text, in nearby locations.

## Computer and network workflows

The host-owned built-in `Media` application is entered through a connected terminal. Jobs belong to the computer host and continue after the terminal session closes, but active recording/playback never resumes after a power loss. The application provides `inputs`, `outputs`, `files`, `jobs`, `feeds`, `record`, `recordloop`, `recordsplit`, `recordevent`, `snapshot`, `play`, `stop`, `publish`, `acl`, `subscribe`, `unsubscribe`, and `still`.

`record <input> as <file>` attaches a recorder to a local media interface input. `snapshot <input> as <file>` creates a media file containing the current canonical still. `play <file> to <output>` starts at the beginning. `still <file> [timestamp]` displays the relevant stored scene. Live feeds retain only current-frame/current-tick traffic: slow and disconnected subscribers never receive an implicit historical backlog.

Surveillance retention is implemented as host software policy over immutable recordings:

- `recordloop <input> as <base-file> retain <duration> segments <duration>` continuously creates timestamped segments. On each rotation it deletes this job's finalised segments whose finalisation time falls outside the retention window. The active segment plus one boundary segment can therefore make retained coverage slightly longer than the requested window, but never shorter. Copies made elsewhere are independent references and are not erased.
- `recordsplit <input> as <base-file> every <duration>` finalises the current recording and immediately starts a new timestamped file at each interval. It performs no automatic expiry.
- `recordevent <input> as <base-file> for <duration>` remains armed without creating a file. Any captured non-snapshot media event starts a timestamped recording; each further event extends the deadline, and the file finalises after the configured quiet period. Periodic scene frames and playback-state markers do not trigger or extend the window, while ordinary movement/actions, speech, signed language, visual emotes, and filmed crimes do.

Automatic filenames use the computer host's local in-character calendar and clock, for example `<stem>-1703jan01T040000-<sequence><extension>`. Calendar month aliases are normalised to filename-safe letters and digits, negative years use an `m` prefix, and `.av` is supplied when the base has no extension. A host with no accessible IC calendar uses `ic-undated` rather than exposing real-world time. Internal packet ordering, chunk offsets, job deadlines, and retention calculations remain based on monotonic/UTC runtime time so accelerated or paused game clocks cannot corrupt recording integrity. Segment and event windows have a five-second minimum. `jobs` reports the mode, current file or base name, and policy state (`armed` versus actively recording). `stop <job>` finalises any current segment and disarms the policy. Quota remains authoritative: if a new segment or an active segment's compressed growth cannot fit, the job stops cleanly rather than silently discarding newer evidence.

Hosts publish named inputs as `public` or `private` feeds through the existing direct, exchange, and VPN route model. The Media network service must be enabled on the publishing host. Public delivery requires current route reachability. Private delivery also requires an enabled shared `user@domain` account in the feed ACL. Login uses the existing interactive account flow. `subscribe ... to ...` creates a live, host-session subscription; add `save <name>` when it must persist and reactivate after startup. Persisted feeds and subscriptions store stable account IDs only, never passwords; a saved private subscription revalidates that account and its ACL membership whenever delivery is attempted. Unsaved subscriptions end on host power loss.

Computer-program APIs are deliberately content-safe: `GetMediaInputs()`, `GetMediaOutputs()`, `StartMediaRecording(input, filename)`, `StartMediaPlayback(filename, output)`, `CaptureMediaStill(input, filename)`, `StopMediaJob(jobId)`, `PublishMediaFeed(input, feed, isPublic)`, `SubscribeMediaFeed(address, output, savedSubscription)`, and `WaitMediaEvent(endpoint)`. `WaitMediaEvent` is a persisted Media process wait. It resumes only for the next matching endpoint event and returns a text dictionary containing `event`, `source`, `capabilities`, `timestamputc`, `sequence`, `recordingid`, `feed`, and `jobid`; it deliberately exposes no transcript or scene content and queues nothing while the program is not waiting.

## Legacy audio media

The experimental `Tape` component and its XML recording payload are retired. They are not migrated or loaded as compatibility data. Development item/prototype rows using that component must be recreated as `MediaStorageMedium` configurations. The answering machine now accepts an audio-capable generic medium and writes immutable typed audio recordings through the common recording service; its legacy telephone playback surface is reconstructed from those audio packets.

## Builder and verification checklist

1. Configure compatible endpoint keys/capabilities and power, then prove the links with ordinary player `connect` / `disconnect` and `switch` commands. Use a passive splitter for fan-out rather than assuming a camera has only one listener.
2. For composite TV/VCR or boombox items, use sibling source acceptance on the monitor/speaker/deck as appropriate, author a sensible default output volume, and use ordinary container content for the medium.
3. Test darkness, plane/layer separation, closed visibility, and inaudible output before relying on a sensor feed.
4. Test record/play, duplicate names, write protection, full medium/quota, erase, power loss, and incompatible format/capability handling.
5. For network feeds, test public routing, private ACL membership, disabled accounts, ACL removal, VPN/direct/exchange reachability, and reconnect without retaining credentials.
