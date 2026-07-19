#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    #region Speeds and Positions
    private void SetupSpeeds(BodyProto quadrupedBody, BodyProto avianBody, BodyProto serpentBody,
            BodyProto fishProto, BodyProto crabProto, BodyProto octopusProto, BodyProto jellyfishProto,
            BodyProto pinnipedProto, BodyProto cetaceanProto, BodyProto wormBody, BodyProto insectBody,
            BodyProto wingedInsectBody)
    {
        Console.WriteLine($"[{_stopwatch.Elapsed.TotalSeconds:N1}s] Setting up Speeds");
        long nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

        #region Speeds

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "stalk",
            FirstPersonVerb = "stalk",
            ThirdPersonVerb = "stalks",
            PresentParticiple = "stalking",
            Multiplier = 2,
            StaminaMultiplier = 0.4
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 1,
            StaminaMultiplier = 0.8
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "amble",
            FirstPersonVerb = "amble",
            ThirdPersonVerb = "ambles",
            PresentParticiple = "ambling",
            Multiplier = 0.8,
            StaminaMultiplier = 1.2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "pace",
            FirstPersonVerb = "pace",
            ThirdPersonVerb = "paces",
            PresentParticiple = "pacing",
            Multiplier = 0.6,
            StaminaMultiplier = 1.9
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "trot",
            FirstPersonVerb = "trot",
            ThirdPersonVerb = "trots",
            PresentParticiple = "trotting",
            Multiplier = 0.4,
            StaminaMultiplier = 2.4
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 1,
            Alias = "gallop",
            FirstPersonVerb = "gallop",
            ThirdPersonVerb = "gallops",
            PresentParticiple = "galloping",
            Multiplier = 0.2,
            StaminaMultiplier = 4.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 5,
            StaminaMultiplier = 1.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 7,
            Alias = "shuffle",
            FirstPersonVerb = "shuffle",
            ThirdPersonVerb = "shuffles",
            PresentParticiple = "shuffling",
            Multiplier = 7,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = quadrupedBody,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 1,
            Alias = "hop",
            FirstPersonVerb = "hop",
            ThirdPersonVerb = "hops",
            PresentParticiple = "hopping",
            Multiplier = 1,
            StaminaMultiplier = 0.8
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 5,
            StaminaMultiplier = 1.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 7,
            Alias = "shuffle",
            FirstPersonVerb = "shuffle",
            ThirdPersonVerb = "shuffles",
            PresentParticiple = "shuffling",
            Multiplier = 7,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 18,
            Alias = "slowfly",
            FirstPersonVerb = "slowly fly",
            ThirdPersonVerb = "slowly flies",
            PresentParticiple = "slowly flying",
            Multiplier = 2.7,
            StaminaMultiplier = 8
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = avianBody,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = fishProto,
            PositionId = 6,
            Alias = "flop",
            FirstPersonVerb = "flop",
            ThirdPersonVerb = "flops",
            PresentParticiple = "flopping",
            Multiplier = 6,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = fishProto,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = fishProto,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = fishProto,
            PositionId = 16,
            Alias = "quickswim",
            FirstPersonVerb = "swim quickly",
            ThirdPersonVerb = "swims quickly",
            PresentParticiple = "swimming quickly",
            Multiplier = 1.0,
            StaminaMultiplier = 2
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = jellyfishProto,
            PositionId = 16,
            Alias = "float",
            FirstPersonVerb = "float",
            ThirdPersonVerb = "floats",
            PresentParticiple = "floating",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 6,
            Alias = "slither",
            FirstPersonVerb = "slither",
            ThirdPersonVerb = "slithers",
            PresentParticiple = "slithering",
            Multiplier = 1.5,
            StaminaMultiplier = 1.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 6,
            Alias = "slowslither",
            FirstPersonVerb = "slither slowly",
            ThirdPersonVerb = "slithers slowly",
            PresentParticiple = "slowly slithering",
            Multiplier = 2.5,
            StaminaMultiplier = 0.75
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 6,
            Alias = "quickslither",
            FirstPersonVerb = "slither quickly",
            ThirdPersonVerb = "slithers quickly",
            PresentParticiple = "quickly slithering",
            Multiplier = 1.0,
            StaminaMultiplier = 2.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = serpentBody,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 6,
            Alias = "slither",
            FirstPersonVerb = "slither",
            ThirdPersonVerb = "slithers",
            PresentParticiple = "slithering",
            Multiplier = 1.5,
            StaminaMultiplier = 1.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 6,
            Alias = "slowslither",
            FirstPersonVerb = "slither slowly",
            ThirdPersonVerb = "slithers slowly",
            PresentParticiple = "slowly slithering",
            Multiplier = 2.5,
            StaminaMultiplier = 0.75
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 6,
            Alias = "quickslither",
            FirstPersonVerb = "slither quickly",
            ThirdPersonVerb = "slithers quickly",
            PresentParticiple = "quickly slithering",
            Multiplier = 1.0,
            StaminaMultiplier = 2.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wormBody,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = insectBody,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 1,
            StaminaMultiplier = 0.8
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = insectBody,
            PositionId = 1,
            Alias = "run",
            FirstPersonVerb = "run",
            ThirdPersonVerb = "runs",
            PresentParticiple = "running",
            Multiplier = 0.5,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = insectBody,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 1.5,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = insectBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 2,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = insectBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.5,
            StaminaMultiplier = 10
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wingedInsectBody,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 1,
            StaminaMultiplier = 0.8
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wingedInsectBody,
            PositionId = 1,
            Alias = "run",
            FirstPersonVerb = "run",
            ThirdPersonVerb = "runs",
            PresentParticiple = "running",
            Multiplier = 0.5,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wingedInsectBody,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 1.5,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wingedInsectBody,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 2,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = wingedInsectBody,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.5,
            StaminaMultiplier = 10
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 3,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 1,
            Alias = "run",
            FirstPersonVerb = "run",
            ThirdPersonVerb = "runs",
            PresentParticiple = "running",
            Multiplier = 1.75,
            StaminaMultiplier = 5.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 5,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 7,
            Alias = "shuffle",
            FirstPersonVerb = "shuffle",
            ThirdPersonVerb = "shuffles",
            PresentParticiple = "shuffling",
            Multiplier = 7,
            StaminaMultiplier = 5.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim slowly",
            ThirdPersonVerb = "swims slowly",
            PresentParticiple = "slowly swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 16,
            Alias = "quickswim",
            FirstPersonVerb = "swim quickly",
            ThirdPersonVerb = "swims quickly",
            PresentParticiple = "swimming quickly",
            Multiplier = 1.0,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = pinnipedProto,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 1.5,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 1,
            Alias = "skitter",
            FirstPersonVerb = "skitter",
            ThirdPersonVerb = "skitters",
            PresentParticiple = "skittering",
            Multiplier = 1.0,
            StaminaMultiplier = 2.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 5,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim slowly",
            ThirdPersonVerb = "swims slowly",
            PresentParticiple = "slowly swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 16,
            Alias = "quickswim",
            FirstPersonVerb = "swim quickly",
            ThirdPersonVerb = "swims quickly",
            PresentParticiple = "swimming quickly",
            Multiplier = 1.0,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = crabProto,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 1,
            Alias = "walk",
            FirstPersonVerb = "walk",
            ThirdPersonVerb = "walks",
            PresentParticiple = "walking",
            Multiplier = 1.5,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 1,
            Alias = "run",
            FirstPersonVerb = "run",
            ThirdPersonVerb = "runs",
            PresentParticiple = "running",
            Multiplier = 1.0,
            StaminaMultiplier = 2.25
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 6,
            Alias = "crawl",
            FirstPersonVerb = "crawl",
            ThirdPersonVerb = "crawls",
            PresentParticiple = "crawling",
            Multiplier = 5,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 15,
            Alias = "climb",
            FirstPersonVerb = "climb",
            ThirdPersonVerb = "climbs",
            PresentParticiple = "climbing",
            Multiplier = 3,
            StaminaMultiplier = 3
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim slowly",
            ThirdPersonVerb = "swims slowly",
            PresentParticiple = "slowly swimming",
            Multiplier = 2,
            StaminaMultiplier = 1.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 16,
            Alias = "quickswim",
            FirstPersonVerb = "swim quickly",
            ThirdPersonVerb = "swims quickly",
            PresentParticiple = "swimming quickly",
            Multiplier = 1.0,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 18,
            Alias = "fly",
            FirstPersonVerb = "fly",
            ThirdPersonVerb = "flies",
            PresentParticiple = "flying",
            Multiplier = 1.8,
            StaminaMultiplier = 15
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = octopusProto,
            PositionId = 18,
            Alias = "franticfly",
            FirstPersonVerb = "frantically fly",
            ThirdPersonVerb = "frantically flies",
            PresentParticiple = "frantically flying",
            Multiplier = 1.4,
            StaminaMultiplier = 25
        });

        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = cetaceanProto,
            PositionId = 6,
            Alias = "flop",
            FirstPersonVerb = "flop",
            ThirdPersonVerb = "flops",
            PresentParticiple = "flopping",
            Multiplier = 6,
            StaminaMultiplier = 3.0
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = cetaceanProto,
            PositionId = 16,
            Alias = "swim",
            FirstPersonVerb = "swim",
            ThirdPersonVerb = "swims",
            PresentParticiple = "swimming",
            Multiplier = 1.5,
            StaminaMultiplier = 2
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = cetaceanProto,
            PositionId = 16,
            Alias = "slowswim",
            FirstPersonVerb = "swim slowly",
            ThirdPersonVerb = "swims slowly",
            PresentParticiple = "swimming slowly",
            Multiplier = 2,
            StaminaMultiplier = 1.5
        });
        _context.MoveSpeeds.Add(new MoveSpeed
        {
            Id = nextId++,
            BodyProto = cetaceanProto,
            PositionId = 16,
            Alias = "quickswim",
            FirstPersonVerb = "swim quickly",
            ThirdPersonVerb = "swims quickly",
            PresentParticiple = "swimming quickly",
            Multiplier = 1.0,
            StaminaMultiplier = 2
        });
        _context.SaveChanges();

        #endregion
    }

    private void SetupPositions(BodyProto quadrupedBody, BodyProto avianBody, BodyProto serpentBody,
            BodyProto fishProto, BodyProto crabProto, BodyProto octopusProto, BodyProto jellyfishProto,
            BodyProto pinnipedProto, BodyProto cetaceanProto, BodyProto wormProto, BodyProto insectBody,
            BodyProto wingedInsectBody)
    {
        Console.WriteLine($"[{_stopwatch.Elapsed.TotalSeconds:N1}s] Setting up Positions");
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 2 }); // Sitting
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 3 }); // Kneeling
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 4 }); // Lounging
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 5 }); // Lying Down
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = quadrupedBody, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 7 }); // Prostrate
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 11 }); // Leaning
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = quadrupedBody, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 2 }); // Sitting
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 4 }); // Lounging
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 5 }); // Lying Down
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = pinnipedProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 11 }); // Leaning
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = pinnipedProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 2 }); // Sitting
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 3 }); // Kneeling
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 5 }); // Lying Down
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 7 }); // Prostrate
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = crabProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = octopusProto, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = octopusProto, Position = 5 }); // Lying Down
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = octopusProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = octopusProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = octopusProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = octopusProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = octopusProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 2 }); // Sitting
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 5 }); // Lying Down
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = avianBody, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = serpentBody, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = serpentBody, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = serpentBody, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wormProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = insectBody, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 1 }); // Standing
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = wingedInsectBody, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = fishProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = cetaceanProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions { BodyProto = cetaceanProto, Position = 6 }); // Prone
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = cetaceanProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = cetaceanProto, Position = 16 }); // Swimming
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = cetaceanProto, Position = 15 }); // Climbing

        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = jellyfishProto, Position = 8 }); // Sprawled
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = jellyfishProto, Position = 18 }); // Flying
        _context.BodyProtosPositions.Add(new BodyProtosPositions
        { BodyProto = jellyfishProto, Position = 16 }); // Swimming
        _context.SaveChanges();
    }

    private void ApplyDefaultCombatSettingsToSeededRaces()
    {
        foreach (AnimalRaceTemplate template in RaceTemplates.Values)
        {
            Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
            if (race is null)
            {
                continue;
            }

            CharacterCombatSetting setting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey);
            double expectedHealthMultiplier =
                ResolveAnimalRaceHealthMultiplier(template, template.Size, template.BodypartHealthMultiplier);
            bool needsUpdate = race.DefaultCombatSettingId != setting.Id ||
                               race.NaturalArmourTypeId != _naturalArmour?.Id ||
                               Math.Abs(race.BodypartHealthMultiplier - expectedHealthMultiplier) > 0.0001 ||
							   !SatiationLimitSeederHelper.MatchesLimits(
								   race,
								   template.MaximumFoodSatiatedHours,
								   template.MaximumDrinkSatiatedHours);
            if (!needsUpdate)
            {
                continue;
            }

            race.DefaultCombatSetting = setting;
            race.NaturalArmourType = _naturalArmour;
            race.BodypartHealthMultiplier = expectedHealthMultiplier;
			SatiationLimitSeederHelper.ApplyLimits(
				race,
				template.MaximumFoodSatiatedHours,
				template.MaximumDrinkSatiatedHours);
        }

        _context.SaveChanges();
        ApplyDefaultAttributeAlterationsToSeededRaces();
    }

    private void ApplyDefaultAttributeAlterationsToSeededRaces()
    {
        List<TraitDefinition> attributes = _context.TraitDefinitions
            .Where(x => x.Type == (int)TraitType.Attribute || x.Type == (int)TraitType.DerivedAttribute)
            .ToList();
        foreach (AnimalRaceTemplate template in RaceTemplates.Values)
        {
            Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
            if (race is null)
            {
                continue;
            }

            NonHumanAttributeProfile profile = GetAnimalAttributeProfile(template);
            foreach (TraitDefinition attribute in attributes)
            {
                RacesAttributes? alteration = _context.RacesAttributes
                    .FirstOrDefault(x => x.RaceId == race.Id && x.AttributeId == attribute.Id);
                if (alteration is null)
                {
                    _context.RacesAttributes.Add(new RacesAttributes
                    {
                        Race = race,
                        Attribute = attribute,
                        IsHealthAttribute = attribute.TraitGroup == "Physical",
                        AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile),
                        DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, profile)
                    });
                    continue;
                }

                alteration.IsHealthAttribute = attribute.TraitGroup == "Physical";
                alteration.AttributeBonus = NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, profile);
                alteration.DiceExpression = NonHumanAttributeScalingHelper.GetAttributeDiceExpression(attribute, profile);
            }
        }

        _context.SaveChanges();
    }

    #endregion
}
