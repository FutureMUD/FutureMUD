using System;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private void SeedInsectoid(BodyProto insectProto)
    {
        ResetCachedParts();
        var order = 1;
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

        AddBodypart(insectProto, "thorax", "thorax", "Insect Thorax", BodypartTypeEnum.Wear, null,
            Alignment.Front, Orientation.Centre, 50, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true,
            isVital: true, implantSpace: 2, stunMultiplier: 0.2);
        AddBodypart(insectProto, "head", "head", "head", BodypartTypeEnum.BonyDrapeable, "thorax",
            Alignment.Front, Orientation.High, 40, -1, 80, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true, implantSpace: 1, stunMultiplier: 1.0);
        AddBodypart(insectProto, "abdomen", "abdomen", "Insect Abdomen", BodypartTypeEnum.Wear, "thorax",
            Alignment.Rear, Orientation.Low, 50, -1, 90, order++, "chitin", SizeCategory.Small, "Torso", true,
            isVital: true, implantSpace: 1, stunMultiplier: 0.2);
        AddBodypart(insectProto, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "mandibles", "mandibles", "Mandible", BodypartTypeEnum.Wear, "head",
            Alignment.Front, Orientation.High, 10, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
            Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true);
        AddBodypart(insectProto, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
            Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true);

        for (var i = 0; i < 6; i++)
        {
            var side = i % 2 == 0 ? "r" : "l";
            var num = (i / 2) + 1;
            var desc = side == "r" ? "right" : "left";
            AddBodypart(insectProto, $"{side}leg{num}", $"{desc} leg {num}", "Upper Leg", BodypartTypeEnum.Standing,
                "thorax", side == "r" ? Alignment.Right : Alignment.Left, Orientation.Low, 30, -1, 40, order++,
                "chitin", SizeCategory.Small, "Leg", true);
        }

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
        AddRace("Ant", "Formic", "Small colony insects", insectProto, SizeCategory.Tiny, false, 0.05,
            "Small Insect", "Small Insect", false, "insect");
        AddRace("Beetle", "Beetle", "Hard-shelled insects", insectProto, SizeCategory.VerySmall, false, 0.1,
            "Small Insect", "Small Insect", false, "insect");
        AddRace("Mantis", "Mantis", "Predatory insects with long forelegs", insectProto, SizeCategory.Small, false,
            0.2, "Medium Insect", "Medium Insect", false, "insect");
    }

    private void SeedWingedInsectoid(BodyProto insectProto)
    {
        ResetCachedParts();
        var order = 1;
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

        AddBodypart(insectProto, "thorax", "thorax", "Insect Thorax", BodypartTypeEnum.Wear, null,
            Alignment.Front, Orientation.Centre, 50, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true,
            isVital: true, implantSpace: 2, stunMultiplier: 0.2);
        AddBodypart(insectProto, "head", "head", "head", BodypartTypeEnum.BonyDrapeable, "thorax",
            Alignment.Front, Orientation.High, 40, -1, 80, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true, implantSpace: 1, stunMultiplier: 1.0);
        AddBodypart(insectProto, "abdomen", "abdomen", "Insect Abdomen", BodypartTypeEnum.Wear, "thorax",
            Alignment.Rear, Orientation.Low, 50, -1, 90, order++, "chitin", SizeCategory.Small, "Torso", true,
            isVital: true, implantSpace: 1, stunMultiplier: 0.2);
        AddBodypart(insectProto, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "head",
            Alignment.FrontRight, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "head",
            Alignment.FrontLeft, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "mandibles", "mandibles", "Mandible", BodypartTypeEnum.Wear, "head",
            Alignment.Front, Orientation.High, 10, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
        AddBodypart(insectProto, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
            Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true);
        AddBodypart(insectProto, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
            Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
            isVital: true);

        for (var i = 0; i < 6; i++)
        {
            var side = i % 2 == 0 ? "r" : "l";
            var num = (i / 2) + 1;
            var desc = side == "r" ? "right" : "left";
            AddBodypart(insectProto, $"{side}leg{num}", $"{desc} leg {num}", "Upper Leg", BodypartTypeEnum.Standing,
                "thorax", side == "r" ? Alignment.Right : Alignment.Left, Orientation.Low, 30, -1, 40, order++,
                "chitin", SizeCategory.Small, "Leg", true);
        }

        AddBodypart(insectProto, "rwingbase", "right wing base", "Wing Base", BodypartTypeEnum.BonyDrapeable, "thorax",
            Alignment.FrontRight, Orientation.High, 20, -1, 50, order++, "chitin", SizeCategory.Small, "Right Wing");
        AddBodypart(insectProto, "lwingbase", "left wing base", "Wing Base", BodypartTypeEnum.BonyDrapeable, "thorax",
            Alignment.FrontLeft, Orientation.High, 20, -1, 50, order++, "chitin", SizeCategory.Small, "Left Wing");
        AddBodypart(insectProto, "rwing", "right wing", "Wing", BodypartTypeEnum.Wing, "rwingbase",
            Alignment.FrontRight, Orientation.High, 20, 30, 80, order++, "chitin", SizeCategory.Small, "Right Wing");
        AddBodypart(insectProto, "lwing", "left wing", "Wing", BodypartTypeEnum.Wing, "lwingbase",
            Alignment.FrontLeft, Orientation.High, 20, 30, 80, order++, "chitin", SizeCategory.Small, "Left Wing");

        _context.SaveChanges();

        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
        AddRace("Dragonfly", "Dragonfly", "Long-bodied flying insects", insectProto, SizeCategory.VerySmall, false,
            0.1, "Small Insect", "Small Insect", false, "insect");
        AddRace("Bee", "Bee", "Striped pollinating insects", insectProto, SizeCategory.VerySmall, false,
            0.1, "Small Insect", "Small Insect", false, "insect");
        AddRace("Butterfly", "Butterfly", "Colourful winged insects", insectProto, SizeCategory.VerySmall, false,
            0.1, "Small Insect", "Small Insect", false, "insect");
    }
}
