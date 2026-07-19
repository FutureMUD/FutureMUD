#nullable enable

using Humanizer;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System;
using TimeOfDay = MudSharp.Celestial.TimeOfDay;
using TimeZoneInfo = MudSharp.Models.TimeZoneInfo;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
    public void SeedColours(FuturemudDatabaseContext context)
    {
        CharacteristicDefinition colourDef = new()
        {
            Type = 2,
            Name = "Colour",
            Pattern = "colou?r",
            Description = "The base variable for all colour types",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(colourDef);
        context.SaveChanges();

        context.CharacteristicDefinitions.Add(new CharacteristicDefinition
        {
            Type = 2,
            Name = "Colour1",
            Pattern = "colou?r1",
            Description = "A child of the Colour variable for use when an item has multiple colours",
            Model = "standard",
            ParentId = colourDef.Id
        });
        context.CharacteristicDefinitions.Add(new CharacteristicDefinition
        {
            Type = 2,
            Name = "Colour2",
            Pattern = "colou?r2",
            Description = "A child of the Colour variable for use when an item has multiple colours",
            Model = "standard",
            ParentId = colourDef.Id
        });
        context.CharacteristicDefinitions.Add(new CharacteristicDefinition
        {
            Type = 2,
            Name = "Colour3",
            Pattern = "colou?r3",
            Description = "A child of the Colour variable for use when an item has multiple colours",
            Model = "standard",
            ParentId = colourDef.Id
        });
        CharacteristicDefinition fineColourDef = new()
        {
            Type = 2,
            Name = "Fine Colour",
            Pattern = "^finecolou?r(ed)?$",
            Description = "A legacy RPI Engine variable for fine colour values such as $finecolor.",
            Model = "standard",
            ParentId = colourDef.Id
        };
        context.CharacteristicDefinitions.Add(fineColourDef);
        CharacteristicDefinition drabColourDef = new()
        {
            Type = 2,
            Name = "Drab Colour",
            Pattern = "^(drabcolou?r|drobcolor)$",
            Description = "A legacy RPI Engine variable for drab colour values such as $drabcolor.",
            Model = "standard",
            ParentId = colourDef.Id
        };
        context.CharacteristicDefinitions.Add(drabColourDef);
        CharacteristicDefinition gemDef = new()
        {
            Type = 2,
            Name = "Gem",
            Pattern = "^gem(colou?r)?$",
            Description = "A legacy RPI Engine variable for common gem values such as $gemcolor, plus the importer $gem alias.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(gemDef);
        CharacteristicDefinition fineGemDef = new()
        {
            Type = 2,
            Name = "Fine Gem",
            Pattern = "^finegem(colou?r)?$",
            Description = "A legacy RPI Engine variable for refined gem values such as $finegemcolor.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(fineGemDef);
        CharacteristicDefinition commonStoneDef = new()
        {
            Type = 2,
            Name = "Common Stone",
            Pattern = "^(common)?stone$",
            Description = "A legacy RPI Engine variable for common stone values such as $commonstone, plus the importer $stone alias.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(commonStoneDef);
        CharacteristicDefinition jewelleryMotifDef = new()
        {
            Type = 2,
            Name = "Jewellery Motif",
            Pattern = "^motif$",
            Description = "A medieval jewellery variable for decorative motifs such as $motif.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(jewelleryMotifDef);
        CharacteristicDefinition flowerDef = new()
        {
            Type = 2,
            Name = "Flower",
            Pattern = "^flower$",
            Description = "A medieval jewellery variable for flower and garland values such as $flower.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(flowerDef);
        CharacteristicDefinition metalFinishDef = new()
        {
            Type = 2,
            Name = "Metal Finish",
            Pattern = "^finish$",
            Description = "A medieval jewellery variable for metal finish values such as $finish.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(metalFinishDef);
        CharacteristicDefinition beadPatternDef = new()
        {
            Type = 2,
            Name = "Bead Pattern",
            Pattern = "^beadpattern$",
            Description = "A medieval jewellery variable for bead arrangement values such as $beadpattern.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(beadPatternDef);
        CharacteristicDefinition jewelleryShapeDef = new()
        {
            Type = 2,
            Name = "Jewellery Shape",
            Pattern = "^shape$",
            Description = "A medieval jewellery variable for visible jewellery shape values such as $shape.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(jewelleryShapeDef);
        CharacteristicDefinition inlayStyleDef = new()
        {
            Type = 2,
            Name = "Inlay Style",
            Pattern = "^inlay$",
            Description = "A medieval jewellery variable for inlay and surface decoration values such as $inlay.",
            Model = "standard"
        };
        context.CharacteristicDefinitions.Add(inlayStyleDef);
        context.SaveChanges();

        // Colours
        List<Colour> colours = new();
        context.Colours.Add(new Colour
        { Id = 1, Name = "black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a pure shade of black" });
        context.Colours.Add(new Colour
        { Id = 2, Name = "white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "a pure shade of white" });
        context.Colours.Add(new Colour
        { Id = 3, Name = "grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "a pure shade of grey" });
        context.Colours.Add(new Colour
        {
            Id = 4,
            Name = "light grey",
            Basic = 2,
            Red = 175,
            Green = 175,
            Blue = 175,
            Fancy = "a pure shade of light grey"
        });
        context.Colours.Add(new Colour
        {
            Id = 5,
            Name = "dark grey",
            Basic = 2,
            Red = 75,
            Green = 75,
            Blue = 75,
            Fancy = "a pure shade of dark grey"
        });
        context.Colours.Add(new Colour
        { Id = 6, Name = "red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "a pure shade of red" });
        context.Colours.Add(new Colour
        {
            Id = 7,
            Name = "dark red",
            Basic = 3,
            Red = 160,
            Green = 0,
            Blue = 0,
            Fancy = "a pure shade of dark red"
        });
        context.Colours.Add(new Colour
        { Id = 8, Name = "blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "a pure shade of blue" });
        context.Colours.Add(new Colour
        {
            Id = 9,
            Name = "dark blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 160,
            Fancy = "a pure shade of dark blue"
        });
        context.Colours.Add(new Colour
        { Id = 10, Name = "green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a pure shade of green" });
        context.Colours.Add(new Colour
        { Id = 11, Name = "brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "a pure shade of brown" });
        context.Colours.Add(new Colour
        {
            Id = 12,
            Name = "dark green",
            Basic = 5,
            Red = 0,
            Green = 160,
            Blue = 0,
            Fancy = "a pure shade of dark green"
        });
        context.Colours.Add(new Colour
        {
            Id = 13,
            Name = "hazel",
            Basic = 10,
            Red = 175,
            Green = 255,
            Blue = 0,
            Fancy = "a complex mixture of brown, green and gold"
        });
        context.Colours.Add(new Colour
        { Id = 14, Name = "pale white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "a pale white" });
        context.Colours.Add(new Colour
        {
            Id = 15,
            Name = "olive",
            Basic = 5,
            Red = 25,
            Green = 160,
            Blue = 0,
            Fancy = "the dark, deep brownish-green of an olive"
        });
        context.Colours.Add(new Colour
        {
            Id = 16,
            Name = "caramel",
            Basic = 10,
            Red = 185,
            Green = 175,
            Blue = 0,
            Fancy = "the deep, rich brown of melted caramel"
        });
        context.Colours.Add(new Colour
        {
            Id = 17,
            Name = "ebony",
            Basic = 0,
            Red = 10,
            Green = 10,
            Blue = 10,
            Fancy = "the deep, rich black of polished ebony"
        });
        context.Colours.Add(new Colour
        {
            Id = 18,
            Name = "emerald green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 15,
            Fancy = "the radiant green of a cut emerald"
        });
        context.Colours.Add(new Colour
        {
            Id = 19,
            Name = "cerulean",
            Basic = 4,
            Red = 0,
            Green = 75,
            Blue = 255,
            Fancy = "the vibrant, bright cyan of cerulean"
        });
        context.Colours.Add(new Colour
        {
            Id = 20,
            Name = "violet",
            Basic = 8,
            Red = 225,
            Green = 0,
            Blue = 225,
            Fancy = "the bright pure purple colour of violet"
        });
        context.Colours.Add(new Colour
        {
            Id = 21,
            Name = "sandy brown",
            Basic = 10,
            Red = 125,
            Green = 125,
            Blue = 10,
            Fancy = "the light brown colour of beach sand"
        });
        context.Colours.Add(new Colour
        {
            Id = 22,
            Name = "light brown",
            Basic = 10,
            Red = 125,
            Green = 125,
            Blue = 0,
            Fancy = "a rich, light brown"
        });
        context.Colours.Add(new Colour
        { Id = 23, Name = "dark brown", Basic = 10, Red = 225, Green = 225, Blue = 0, Fancy = "a dark brown" });
        context.Colours.Add(new Colour
        {
            Id = 24,
            Name = "auburn",
            Basic = 3,
            Red = 160,
            Green = 10,
            Blue = 10,
            Fancy = "the rich, reddish brown of auburn"
        });
        context.Colours.Add(new Colour
        {
            Id = 25,
            Name = "ebony",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the deep, rich black of polished ebony"
        });
        context.Colours.Add(new Colour
        {
            Id = 26,
            Name = "onyx",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of an onyx gemstone"
        });
        context.Colours.Add(new Colour
        { Id = 27, Name = "obsidian", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a deep obsidian black" });
        context.Colours.Add(new Colour
        {
            Id = 28,
            Name = "midnight black",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of a starless midnight sky"
        });
        context.Colours.Add(new Colour
        {
            Id = 29,
            Name = "ink black",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of a pot of black ink"
        });
        context.Colours.Add(new Colour
        { Id = 30, Name = "jet black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "a tenebrous, jet black" });
        context.Colours.Add(new Colour
        {
            Id = 31,
            Name = "pitch black",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of absolute darkness"
        });
        context.Colours.Add(new Colour
        {
            Id = 32,
            Name = "ivory",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of polished ivory"
        });
        context.Colours.Add(new Colour
        {
            Id = 33,
            Name = "seashell",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of a weathered seashell"
        });
        context.Colours.Add(new Colour
        {
            Id = 34,
            Name = "snow white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of fresh snow"
        });
        context.Colours.Add(new Colour
        {
            Id = 35,
            Name = "gleaming white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "a bright, gleaming white"
        });
        context.Colours.Add(new Colour
        {
            Id = 36,
            Name = "pure white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "a perfect, pure white"
        });
        context.Colours.Add(new Colour
        {
            Id = 37,
            Name = "pearl white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of a polished pearl"
        });
        context.Colours.Add(new Colour
        {
            Id = 38,
            Name = "bright white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "a bold, bright white"
        });
        context.Colours.Add(new Colour
        {
            Id = 39,
            Name = "bone white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of weathered bone"
        });
        context.Colours.Add(new Colour
        {
            Id = 40,
            Name = "ghost white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "a ghostly shade of white"
        });
        context.Colours.Add(new Colour
        {
            Id = 41,
            Name = "mist grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of a thick morning mist"
        });
        context.Colours.Add(new Colour
        {
            Id = 42,
            Name = "charcoal grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of charcoal"
        });
        context.Colours.Add(new Colour
        {
            Id = 43,
            Name = "thistle grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of a thistle bush"
        });
        context.Colours.Add(new Colour
        {
            Id = 44,
            Name = "smoky grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of wood smoke"
        });
        context.Colours.Add(new Colour
        {
            Id = 45,
            Name = "slate grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the grey-blue colour of a slab of slate"
        });
        context.Colours.Add(new Colour
        {
            Id = 46,
            Name = "silver grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "a rich, silvery grey"
        });
        context.Colours.Add(new Colour
        { Id = 47, Name = "soft grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "a soft grey" });
        context.Colours.Add(new Colour
        {
            Id = 48,
            Name = "ash grey",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the grey colour of cold ash"
        });
        context.Colours.Add(new Colour
        {
            Id = 49,
            Name = "crimson",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "a strong, deep crimson red"
        });
        context.Colours.Add(new Colour
        {
            Id = 50,
            Name = "scarlet",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the bright, orangey-red colour of scarlet"
        });
        context.Colours.Add(new Colour
        {
            Id = 51,
            Name = "ruby red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of a ruby gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 52,
            Name = "blood red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of arterial blood"
        });
        context.Colours.Add(new Colour
        {
            Id = 53,
            Name = "rose red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of a rose in bloom"
        });
        context.Colours.Add(new Colour
        {
            Id = 54,
            Name = "wine red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the deep purplish-red of wine"
        });
        context.Colours.Add(new Colour
        {
            Id = 55,
            Name = "flame red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the bold red of burning flame"
        });
        context.Colours.Add(new Colour
        {
            Id = 56,
            Name = "coral",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the dark reddish-pink of coral"
        });
        context.Colours.Add(new Colour
        {
            Id = 57,
            Name = "copper",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the reddish-brown of the metal copper"
        });
        context.Colours.Add(new Colour
        {
            Id = 58,
            Name = "fiery orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the bold orange of a burning flame"
        });
        context.Colours.Add(new Colour
        {
            Id = 59,
            Name = "ochre",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the yellowish-brown of ochre"
        });
        context.Colours.Add(new Colour
        {
            Id = 60,
            Name = "sunset orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the brilliant yellowish-orange of a sunset"
        });
        context.Colours.Add(new Colour
        {
            Id = 61,
            Name = "amber",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the colour of a block of polished amber"
        });
        context.Colours.Add(new Colour
        {
            Id = 62,
            Name = "goldenrod",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the yellow of the goldenrod flower"
        });
        context.Colours.Add(new Colour
        {
            Id = 63,
            Name = "pale yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "a gentle, pale yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 64,
            Name = "golden yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "a rich, golden yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 65,
            Name = "sand yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the light brownish-yellow of beach sand"
        });
        context.Colours.Add(new Colour
        {
            Id = 66,
            Name = "topaz hued",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of the topaz gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 67,
            Name = "gold-coloured",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "a full-bodied gold hue"
        });
        context.Colours.Add(new Colour
        {
            Id = 68,
            Name = "spring green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the light, bluish-green of early spring"
        });
        context.Colours.Add(new Colour
        {
            Id = 69,
            Name = "sea green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the deep bluish-green of the sea"
        });
        context.Colours.Add(new Colour
        {
            Id = 70,
            Name = "hunter green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the dark green of woodland leaves"
        });
        context.Colours.Add(new Colour
        { Id = 71, Name = "olive green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "an olive green" });
        context.Colours.Add(new Colour
        {
            Id = 72,
            Name = "sage green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the light, pale green of sage leaves"
        });
        context.Colours.Add(new Colour
        {
            Id = 73,
            Name = "pine green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the rich, dark green of pine leaves"
        });
        context.Colours.Add(new Colour
        {
            Id = 74,
            Name = "bright green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "a bold, bright green"
        });
        context.Colours.Add(new Colour
        { Id = 75, Name = "rich green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a deep, rich green" });
        context.Colours.Add(new Colour
        {
            Id = 76,
            Name = "pale green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "a pale greyish-green"
        });
        context.Colours.Add(new Colour
        {
            Id = 77,
            Name = "verdant green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the verdant green of summer"
        });
        context.Colours.Add(new Colour
        { Id = 78, Name = "forest green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "a forest green" });
        context.Colours.Add(new Colour
        {
            Id = 79,
            Name = "chartreuse",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the slight greenish-yellow colour of chartreuse"
        });
        context.Colours.Add(new Colour
        {
            Id = 80,
            Name = "slate blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the greyish-blue of slate"
        });
        context.Colours.Add(new Colour
        {
            Id = 81,
            Name = "bright blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "a bright, vibrant blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 82,
            Name = "powder blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the bright cyan colour of powder snow"
        });
        context.Colours.Add(new Colour
        {
            Id = 83,
            Name = "sapphire blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of the sapphire gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 84,
            Name = "royal blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "a dark shade of azure blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 85,
            Name = "ocean blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the deep, multihued blue of the ocean"
        });
        context.Colours.Add(new Colour
        { Id = 86, Name = "teal", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "a dark bluish-green" });
        context.Colours.Add(new Colour
        {
            Id = 87,
            Name = "cornflour blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the medium blue of the sapphire gem"
        });
        context.Colours.Add(new Colour
        {
            Id = 88,
            Name = "sky blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the rich colour of a cloudless sky"
        });
        context.Colours.Add(new Colour
        {
            Id = 89,
            Name = "azure",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the blue colour of the azure gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 90,
            Name = "beryl",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the bluish-green colour of the beryl gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 91,
            Name = "cerulean",
            Basic = 11,
            Red = 0,
            Green = 75,
            Blue = 255,
            Fancy = "a rich, pure cerulean blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 92,
            Name = "cobalt",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "a medium-dark cobalt blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 93,
            Name = "rich indigo",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the vibrant colour of indigo dye"
        });
        context.Colours.Add(new Colour
        {
            Id = 94,
            Name = "deep indigo",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the deep indigo of the bottom of a rainbow"
        });
        context.Colours.Add(new Colour
        {
            Id = 95,
            Name = "vivid indigo",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "a vivid, bold indigo"
        });
        context.Colours.Add(new Colour
        {
            Id = 96,
            Name = "earthen brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the brown of rich soil"
        });
        context.Colours.Add(new Colour
        {
            Id = 97,
            Name = "deep brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "a deep, dark brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 98,
            Name = "rich brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "a rich, bold brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 99,
            Name = "burnt sienna",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of fired sienna earth"
        });
        context.Colours.Add(new Colour
        {
            Id = 100,
            Name = "chocolate",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of dark chocolate"
        });
        context.Colours.Add(new Colour
        {
            Id = 101,
            Name = "cinnamon",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the light pinkish-brown of cinnamon"
        });
        context.Colours.Add(new Colour
        {
            Id = 102,
            Name = "mahogany",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the rich, dark colour of mahogany timber"
        });
        context.Colours.Add(new Colour
        {
            Id = 103,
            Name = "nut brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the light brown of a chestnut shell"
        });
        context.Colours.Add(new Colour
        {
            Id = 104,
            Name = "umber",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the deep reddish-brown of umber"
        });
        context.Colours.Add(new Colour
        {
            Id = 105,
            Name = "amethyst",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the vivid purple of the amethyst gemstone"
        });
        context.Colours.Add(new Colour
        {
            Id = 106,
            Name = "mauve",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the greyish-magenta of the mallow flower"
        });
        context.Colours.Add(new Colour
        {
            Id = 107,
            Name = "mulbery",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the rich blackish-purple hue of ripe mulberries"
        });
        context.Colours.Add(new Colour
        {
            Id = 108,
            Name = "plum",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the rich reddish-purple hue of a plum"
        });
        context.Colours.Add(new Colour
        {
            Id = 109,
            Name = "lavender",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the pure light-purple of a lavender bush"
        });
        context.Colours.Add(new Colour
        {
            Id = 110,
            Name = "royal purple",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "a rich, dark purple"
        });
        context.Colours.Add(new Colour
        { Id = 111, Name = "faded black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 112, Name = "tattered black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 113, Name = "shabby black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 114, Name = "grimy black", Basic = 0, Red = 0, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 115, Name = "off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 116, Name = "dingy grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 117, Name = "blotched red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 118, Name = "dull orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 119, Name = "bland yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 120, Name = "faded green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 121, Name = "faded blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 122, Name = "faded indigo", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 123, Name = "faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 124, Name = "drab brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 125, Name = "dim grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 126, Name = "dusky slate grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 127, Name = "sooty grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 128, Name = "chalky pale grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 129, Name = "dull mist grey", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 130, Name = "ashen off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 131, Name = "dirty bone-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 132, Name = "wan ivory", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 133, Name = "spotted white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 134, Name = "stained white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 135, Name = "blotched white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 136, Name = "dingy off-white", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 137, Name = "stained ivory", Basic = 1, Red = 255, Green = 255, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 138, Name = "shabby sallow-coloured", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 139, Name = "lurid pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 140, Name = "dingy yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 141, Name = "gaudy mustard yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 142, Name = "sickly pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 143, Name = "shabby pale yellow", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 144, Name = "murky brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 145, Name = "stained brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 146, Name = "dreary brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 147, Name = "bland brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 148, Name = "spotted muddy brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 149, Name = "dismal sand brown", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 150, Name = "dreary beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 151, Name = "grimy beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 152, Name = "shabby beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 153, Name = "dirty beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 154, Name = "tattered beige", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 155, Name = "bland wheat-coloured", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 156, Name = "drab olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 157, Name = "murky olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 158, Name = "dim olive", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 159, Name = "dingy green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 160, Name = "shabby green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 161, Name = "dull green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 162, Name = "sickly greyish-green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 163, Name = "grisly brownish-green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 164, Name = "discoloured green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 165, Name = "blotchy green", Basic = 5, Red = 0, Green = 255, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 166, Name = "grimy rust-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 167, Name = "blotchy rust-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 168, Name = "grimy salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 169, Name = "stained salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 170, Name = "blotched red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 171, Name = "dull red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 172, Name = "faded red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 173, Name = "stained red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 174, Name = "dingy red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 175, Name = "faded salmon", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 176, Name = "well-worn blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 177, Name = "faded slate blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 178, Name = "pallid blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 179, Name = "stained blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 180, Name = "grimy blue", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 181, Name = "dim blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 182, Name = "faded blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 183, Name = "dreary blue-black", Basic = 4, Red = 0, Green = 0, Blue = 255, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 184, Name = "dull orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 185, Name = "faded reddish-orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 186, Name = "tattered reddish-orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 187, Name = "discoloured orange", Basic = 7, Red = 255, Green = 165, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 188, Name = "stained orange-red", Basic = 3, Red = 255, Green = 0, Blue = 0, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 189, Name = "drab peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 190, Name = "lurid peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 191, Name = "sickly peach-coloured", Basic = 9, Red = 255, Green = 192, Blue = 203, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 192, Name = "tattered violet", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 193, Name = "grimy lavender", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 194, Name = "spotted lavender", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 195, Name = "discoloured purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 196, Name = "dirty purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 197, Name = "dingy purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 198, Name = "faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 199, Name = "stained purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        { Id = 200, Name = "dusty faded purple", Basic = 8, Red = 128, Green = 0, Blue = 128, Fancy = "" });
        context.Colours.Add(new Colour
        {
            Id = 201,
            Name = "blonde",
            Basic = 6,
            Red = 0,
            Green = 125,
            Blue = 125,
            Fancy = "a fair, yellow blonde"
        });
        context.Colours.Add(new Colour
        {
            Id = 202,
            Name = "dirty blonde",
            Basic = 6,
            Red = 0,
            Green = 125,
            Blue = 125,
            Fancy = "a darker brownish blonde"
        });
        context.Colours.Add(new Colour
        {
            Id = 203,
            Name = "silver blonde",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "a light blonde with a silvery hue"
        });
        context.Colours.Add(new Colour
        {
            Id = 204,
            Name = "ash blonde",
            Basic = 6,
            Red = 0,
            Green = 55,
            Blue = 55,
            Fancy = "an ashen, greyish blonde"
        });
        context.Colours.Add(new Colour
        {
            Id = 205,
            Name = "strawberry blonde",
            Basic = 6,
            Red = 55,
            Green = 100,
            Blue = 100,
            Fancy = "the colour of blonde with a reddish tinge"
        });
        context.Colours.Add(new Colour
        {
            Id = 206,
            Name = "platinum blonde",
            Basic = 6,
            Red = 0,
            Green = 25,
            Blue = 25,
            Fancy = "an almost whitish shade of blonde"
        });
        context.Colours.Add(new Colour
        {
            Id = 207,
            Name = "light blonde",
            Basic = 6,
            Red = 0,
            Green = 55,
            Blue = 55,
            Fancy = "a pure, light shade of blonde"
        });
        context.Colours.Add(new Colour
        {
            Id = 208,
            Name = "salt-and-pepper",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "a dark greyish-black with speckles of grey"
        });
        context.Colours.Add(new Colour
        { Id = 209, Name = "orange", Basic = 7, Red = 200, Green = 100, Blue = 100, Fancy = "a pure orange" });
        context.Colours.Add(new Colour
        {
            Id = 210,
            Name = "light blue",
            Basic = 4,
            Red = 50,
            Green = 50,
            Blue = 255,
            Fancy = "a light shade of blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 211,
            Name = "light green",
            Basic = 5,
            Red = 0,
            Green = 100,
            Blue = 0,
            Fancy = "a light shade of green"
        });
        context.Colours.Add(new Colour
        {
            Id = 212,
            Name = "pale blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 200,
            Fancy = "a pale shade of blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 213,
            Name = "yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "a pure shade of yellow"
        });
        context.Colours.Add(new Colour
        { Id = 214, Name = "cyan", Basic = 11, Red = 0, Green = 75, Blue = 255, Fancy = "a light, greenish blue" });
        context.Colours.Add(new Colour
        {
            Id = 215,
            Name = "navy blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 180,
            Fancy = "a very dark shade of the colour blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 216,
            Name = "reddish brown",
            Basic = 10,
            Red = 200,
            Green = 155,
            Blue = 10,
            Fancy = "the colour of brown with a tinge of red"
        });
        context.Colours.Add(new Colour
        {
            Id = 217,
            Name = "beige",
            Basic = 10,
            Red = 75,
            Green = 75,
            Blue = 0,
            Fancy = "the pale brown of natural wool"
        });
        context.Colours.Add(new Colour
        {
            Id = 218,
            Name = "light red",
            Basic = 3,
            Red = 115,
            Green = 0,
            Blue = 0,
            Fancy = "a light shade of red"
        });
        context.Colours.Add(new Colour
        {
            Id = 219,
            Name = "purple",
            Basic = 8,
            Red = 180,
            Green = 180,
            Blue = 0,
            Fancy = "a pure shade of purple"
        });
        context.Colours.Add(new Colour
        { Id = 220, Name = "pink", Basic = 7, Red = 255, Green = 245, Blue = 245, Fancy = "a pure shade of pink" });
        context.Colours.Add(new Colour
        {
            Id = 221,
            Name = "dark",
            Basic = 10,
            Red = 225,
            Green = 225,
            Blue = 0,
            Fancy = "a dark brown verging on black"
        });
        context.Colours.Add(new Colour
        {
            Id = 222,
            Name = "indian red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of indian red"
        });
        context.Colours.Add(new Colour
        {
            Id = 223,
            Name = "light pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of light pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 224,
            Name = "violet red",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the colour of violet red"
        });
        context.Colours.Add(new Colour
        {
            Id = 225,
            Name = "hot pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of hot pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 226,
            Name = "maroon red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of maroon red"
        });
        context.Colours.Add(new Colour
        {
            Id = 227,
            Name = "plum purple",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the colour of plum purple"
        });
        context.Colours.Add(new Colour
        {
            Id = 228,
            Name = "magenta red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of magenta red"
        });
        context.Colours.Add(new Colour
        {
            Id = 229,
            Name = "cobalt blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the strikingly rich, deep blue colour of cobalt"
        });
        context.Colours.Add(new Colour
        {
            Id = 230,
            Name = "light steel blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of light steel blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 231,
            Name = "slate gray",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of slate gray"
        });
        context.Colours.Add(new Colour
        {
            Id = 232,
            Name = "turquoise blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of turquoise blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 233,
            Name = "cyan blue",
            Basic = 11,
            Red = 0,
            Green = 75,
            Blue = 255,
            Fancy = "the colour of cyan blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 234,
            Name = "cobalt green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of cobalt green"
        });
        context.Colours.Add(new Colour
        {
            Id = 235,
            Name = "lime green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of lime green"
        });
        context.Colours.Add(new Colour
        {
            Id = 236,
            Name = "ivory white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of ivory white"
        });
        context.Colours.Add(new Colour
        {
            Id = 237,
            Name = "goldenrod yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of goldenrod yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 238,
            Name = "dark khaki",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of dark khaki"
        });
        context.Colours.Add(new Colour
        {
            Id = 239,
            Name = "banana yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of banana yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 240,
            Name = "orange red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of orange red"
        });
        context.Colours.Add(new Colour
        {
            Id = 241,
            Name = "moccasin brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of moccasin brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 242,
            Name = "tan yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of tan yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 243,
            Name = "brick brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of brick brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 244,
            Name = "carrot orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the colour of carrot orange"
        });
        context.Colours.Add(new Colour
        {
            Id = 245,
            Name = "peachpuff pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of peachpuff pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 246,
            Name = "sienna brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of sienna brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 247,
            Name = "saddle brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of saddle brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 248,
            Name = "salmon pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of salmon pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 249,
            Name = "sepia brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of sepia brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 250,
            Name = "fire brick brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of fire brick brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 251,
            Name = "teal blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of teal blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 252,
            Name = "dark gray",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of dark gray"
        });
        context.Colours.Add(new Colour
        {
            Id = 253,
            Name = "pale violet",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the colour of pale violet"
        });
        context.Colours.Add(new Colour
        {
            Id = 254,
            Name = "violet red",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the colour of violet red"
        });
        context.Colours.Add(new Colour
        {
            Id = 255,
            Name = "lavender pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of lavender pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 256,
            Name = "hot pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of hot pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 257,
            Name = "deep pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of deep pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 258,
            Name = "maroon red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of maroon red"
        });
        context.Colours.Add(new Colour
        {
            Id = 259,
            Name = "orchid pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of orchid pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 260,
            Name = "plum purple",
            Basic = 8,
            Red = 128,
            Green = 0,
            Blue = 128,
            Fancy = "the colour of plum purple"
        });
        context.Colours.Add(new Colour
        {
            Id = 261,
            Name = "fuchsia pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of fuchsia pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 262,
            Name = "magenta red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of magenta red"
        });
        context.Colours.Add(new Colour
        {
            Id = 263,
            Name = "midnight blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "darkest night-sky blue, easily mistaken for black in poor lighting"
        });
        context.Colours.Add(new Colour
        {
            Id = 264,
            Name = "cobalt blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the strikingly rich, deep blue colour of cobalt"
        });
        context.Colours.Add(new Colour
        {
            Id = 265,
            Name = "cornflower blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of cornflower blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 266,
            Name = "light steel blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of light steel blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 267,
            Name = "steel blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of steel blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 268,
            Name = "slate gray",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of slate gray"
        });
        context.Colours.Add(new Colour
        { Id = 269, Name = "gray", Basic = 2, Red = 127, Green = 127, Blue = 127, Fancy = "the colour of gray" });
        context.Colours.Add(new Colour
        {
            Id = 270,
            Name = "turquoise blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of turquoise blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 271,
            Name = "azure blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of azure blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 272,
            Name = "cyan blue",
            Basic = 11,
            Red = 0,
            Green = 75,
            Blue = 255,
            Fancy = "the colour of cyan blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 273,
            Name = "aquamarine",
            Basic = 11,
            Red = 0,
            Green = 75,
            Blue = 255,
            Fancy = "the colour of aquamarine"
        });
        context.Colours.Add(new Colour
        {
            Id = 274,
            Name = "cobalt green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of cobalt green"
        });
        context.Colours.Add(new Colour
        {
            Id = 275,
            Name = "mint green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of mint green"
        });
        context.Colours.Add(new Colour
        {
            Id = 276,
            Name = "lime green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of lime green"
        });
        context.Colours.Add(new Colour
        {
            Id = 277,
            Name = "chartreuse green",
            Basic = 5,
            Red = 0,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of chartreuse green"
        });
        context.Colours.Add(new Colour
        {
            Id = 278,
            Name = "ivory white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of ivory white"
        });
        context.Colours.Add(new Colour
        {
            Id = 279,
            Name = "light yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of light yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 280,
            Name = "goldenrod yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of goldenrod yellow"
        });
        context.Colours.Add(new Colour
        { Id = 281, Name = "khaki", Basic = 10, Red = 175, Green = 175, Blue = 0, Fancy = "the colour of khaki" });
        context.Colours.Add(new Colour
        {
            Id = 282,
            Name = "dark khaki",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of dark khaki"
        });
        context.Colours.Add(new Colour
        { Id = 283, Name = "gold", Basic = 6, Red = 255, Green = 255, Blue = 0, Fancy = "the colour of gold" });
        context.Colours.Add(new Colour
        {
            Id = 284,
            Name = "banana yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of banana yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 285,
            Name = "cornsilk yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of cornsilk yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 286,
            Name = "orange red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of orange red"
        });
        context.Colours.Add(new Colour
        {
            Id = 287,
            Name = "wheat yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of wheat yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 288,
            Name = "moccasin brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of moccasin brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 289,
            Name = "eggshell white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of eggshell white"
        });
        context.Colours.Add(new Colour
        {
            Id = 290,
            Name = "tan yellow",
            Basic = 6,
            Red = 255,
            Green = 255,
            Blue = 0,
            Fancy = "the colour of tan yellow"
        });
        context.Colours.Add(new Colour
        {
            Id = 291,
            Name = "tan brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of tan brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 292,
            Name = "brick brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of brick brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 293,
            Name = "brick red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of brick red"
        });
        context.Colours.Add(new Colour
        {
            Id = 294,
            Name = "carrot orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the colour of carrot orange"
        });
        context.Colours.Add(new Colour
        {
            Id = 295,
            Name = "dark orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the colour of dark orange"
        });
        context.Colours.Add(new Colour
        {
            Id = 296,
            Name = "peachpuff pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of peachpuff pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 297,
            Name = "seashell gray",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of seashell gray"
        });
        context.Colours.Add(new Colour
        {
            Id = 298,
            Name = "sienna brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of sienna brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 299,
            Name = "chocolate brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of chocolate brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 300,
            Name = "saddle brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of saddle brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 301,
            Name = "light salmon pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of light salmon pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 302,
            Name = "salmon pink",
            Basic = 9,
            Red = 255,
            Green = 192,
            Blue = 203,
            Fancy = "the colour of salmon pink"
        });
        context.Colours.Add(new Colour
        {
            Id = 303,
            Name = "coral orange",
            Basic = 7,
            Red = 255,
            Green = 165,
            Blue = 0,
            Fancy = "the colour of coral orange"
        });
        context.Colours.Add(new Colour
        {
            Id = 304,
            Name = "sepia brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of sepia brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 305,
            Name = "orange brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of orange brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 306,
            Name = "fire brick brown",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the colour of fire brick brown"
        });
        context.Colours.Add(new Colour
        {
            Id = 307,
            Name = "beet red",
            Basic = 3,
            Red = 255,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of beet red"
        });
        context.Colours.Add(new Colour
        {
            Id = 308,
            Name = "teal blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of teal blue"
        });
        context.Colours.Add(new Colour
        {
            Id = 309,
            Name = "smoky white",
            Basic = 1,
            Red = 255,
            Green = 255,
            Blue = 255,
            Fancy = "the colour of smoky white"
        });
        context.Colours.Add(new Colour
        {
            Id = 310,
            Name = "dark gray",
            Basic = 2,
            Red = 127,
            Green = 127,
            Blue = 127,
            Fancy = "the colour of dark gray"
        });
        context.Colours.Add(new Colour
        {
            Id = 311,
            Name = "gray black",
            Basic = 0,
            Red = 0,
            Green = 0,
            Blue = 0,
            Fancy = "the colour of gray black"
        });
        context.Colours.Add(new Colour
        {
            Id = 312,
            Name = "deep blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "a blue that's several shades darker than most"
        });
        context.Colours.Add(new Colour
        {
            Id = 313,
            Name = "winter blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of a pale, cold winter sky"
        });
        context.Colours.Add(new Colour
        {
            Id = 314,
            Name = "storm blue",
            Basic = 4,
            Red = 0,
            Green = 0,
            Blue = 255,
            Fancy = "the colour of a storm at sea, flat slatey blue with shadowy depths"
        });
        context.Colours.Add(new Colour
        {
            Id = 315,
            Name = "natural",
            Basic = 10,
            Red = 175,
            Green = 175,
            Blue = 0,
            Fancy = "the natural colour of lips, unadorned by makeup"
        });

        long nextId = context.CharacteristicValues.Select(x => x.Id).Max() + 1;
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "black",
            DefinitionId = colourDef.Id,
            Value = "1",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "white",
            DefinitionId = colourDef.Id,
            Value = "2",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grey",
            DefinitionId = colourDef.Id,
            Value = "3",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light grey",
            DefinitionId = colourDef.Id,
            Value = "4",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark grey",
            DefinitionId = colourDef.Id,
            Value = "5",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "red",
            DefinitionId = colourDef.Id,
            Value = "6",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark red",
            DefinitionId = colourDef.Id,
            Value = "7",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blue",
            DefinitionId = colourDef.Id,
            Value = "8",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark blue",
            DefinitionId = colourDef.Id,
            Value = "9",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "green",
            DefinitionId = colourDef.Id,
            Value = "10",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "brown",
            DefinitionId = colourDef.Id,
            Value = "11",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark green",
            DefinitionId = colourDef.Id,
            Value = "12",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "hazel",
            DefinitionId = colourDef.Id,
            Value = "13",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pale white",
            DefinitionId = colourDef.Id,
            Value = "14",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "olive",
            DefinitionId = colourDef.Id,
            Value = "15",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "caramel",
            DefinitionId = colourDef.Id,
            Value = "16",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ebony",
            DefinitionId = colourDef.Id,
            Value = "17",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "emerald green",
            DefinitionId = colourDef.Id,
            Value = "18",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cerulean",
            DefinitionId = colourDef.Id,
            Value = "19",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "violet",
            DefinitionId = colourDef.Id,
            Value = "20",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sandy brown",
            DefinitionId = colourDef.Id,
            Value = "21",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light brown",
            DefinitionId = colourDef.Id,
            Value = "22",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark brown",
            DefinitionId = colourDef.Id,
            Value = "23",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "auburn",
            DefinitionId = colourDef.Id,
            Value = "24",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ebony",
            DefinitionId = colourDef.Id,
            Value = "25",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "onyx",
            DefinitionId = colourDef.Id,
            Value = "26",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "obsidian",
            DefinitionId = colourDef.Id,
            Value = "27",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "midnight black",
            DefinitionId = colourDef.Id,
            Value = "28",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ink black",
            DefinitionId = colourDef.Id,
            Value = "29",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "jet black",
            DefinitionId = colourDef.Id,
            Value = "30",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pitch black",
            DefinitionId = colourDef.Id,
            Value = "31",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ivory",
            DefinitionId = colourDef.Id,
            Value = "32",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "seashell",
            DefinitionId = colourDef.Id,
            Value = "33",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "snow white",
            DefinitionId = colourDef.Id,
            Value = "34",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gleaming white",
            DefinitionId = colourDef.Id,
            Value = "35",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pure white",
            DefinitionId = colourDef.Id,
            Value = "36",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pearl white",
            DefinitionId = colourDef.Id,
            Value = "37",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bright white",
            DefinitionId = colourDef.Id,
            Value = "38",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bone white",
            DefinitionId = colourDef.Id,
            Value = "39",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ghost white",
            DefinitionId = colourDef.Id,
            Value = "40",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "mist grey",
            DefinitionId = colourDef.Id,
            Value = "41",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "charcoal grey",
            DefinitionId = colourDef.Id,
            Value = "42",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "thistle grey",
            DefinitionId = colourDef.Id,
            Value = "43",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "smoky grey",
            DefinitionId = colourDef.Id,
            Value = "44",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "slate grey",
            DefinitionId = colourDef.Id,
            Value = "45",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "silver grey",
            DefinitionId = colourDef.Id,
            Value = "46",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "soft grey",
            DefinitionId = colourDef.Id,
            Value = "47",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ash grey",
            DefinitionId = colourDef.Id,
            Value = "48",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "crimson",
            DefinitionId = colourDef.Id,
            Value = "49",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "scarlet",
            DefinitionId = colourDef.Id,
            Value = "50",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ruby red",
            DefinitionId = colourDef.Id,
            Value = "51",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blood red",
            DefinitionId = colourDef.Id,
            Value = "52",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "rose red",
            DefinitionId = colourDef.Id,
            Value = "53",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "wine red",
            DefinitionId = colourDef.Id,
            Value = "54",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "flame red",
            DefinitionId = colourDef.Id,
            Value = "55",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "coral",
            DefinitionId = colourDef.Id,
            Value = "56",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "copper",
            DefinitionId = colourDef.Id,
            Value = "57",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "fiery orange",
            DefinitionId = colourDef.Id,
            Value = "58",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ochre",
            DefinitionId = colourDef.Id,
            Value = "59",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sunset orange",
            DefinitionId = colourDef.Id,
            Value = "60",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "amber",
            DefinitionId = colourDef.Id,
            Value = "61",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "goldenrod",
            DefinitionId = colourDef.Id,
            Value = "62",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pale yellow",
            DefinitionId = colourDef.Id,
            Value = "63",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "golden yellow",
            DefinitionId = colourDef.Id,
            Value = "64",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sand yellow",
            DefinitionId = colourDef.Id,
            Value = "65",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "topaz hued",
            DefinitionId = colourDef.Id,
            Value = "66",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gold-coloured",
            DefinitionId = colourDef.Id,
            Value = "67",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "spring green",
            DefinitionId = colourDef.Id,
            Value = "68",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sea green",
            DefinitionId = colourDef.Id,
            Value = "69",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "hunter green",
            DefinitionId = colourDef.Id,
            Value = "70",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "olive green",
            DefinitionId = colourDef.Id,
            Value = "71",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sage green",
            DefinitionId = colourDef.Id,
            Value = "72",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pine green",
            DefinitionId = colourDef.Id,
            Value = "73",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bright green",
            DefinitionId = colourDef.Id,
            Value = "74",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "rich green",
            DefinitionId = colourDef.Id,
            Value = "75",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pale green",
            DefinitionId = colourDef.Id,
            Value = "76",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "verdant green",
            DefinitionId = colourDef.Id,
            Value = "77",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "forest green",
            DefinitionId = colourDef.Id,
            Value = "78",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "chartreuse",
            DefinitionId = colourDef.Id,
            Value = "79",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "slate blue",
            DefinitionId = colourDef.Id,
            Value = "80",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bright blue",
            DefinitionId = colourDef.Id,
            Value = "81",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "powder blue",
            DefinitionId = colourDef.Id,
            Value = "82",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sapphire blue",
            DefinitionId = colourDef.Id,
            Value = "83",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "royal blue",
            DefinitionId = colourDef.Id,
            Value = "84",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ocean blue",
            DefinitionId = colourDef.Id,
            Value = "85",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "teal",
            DefinitionId = colourDef.Id,
            Value = "86",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cornflour blue",
            DefinitionId = colourDef.Id,
            Value = "87",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sky blue",
            DefinitionId = colourDef.Id,
            Value = "88",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "azure",
            DefinitionId = colourDef.Id,
            Value = "89",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "beryl",
            DefinitionId = colourDef.Id,
            Value = "90",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cerulean",
            DefinitionId = colourDef.Id,
            Value = "91",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cobalt",
            DefinitionId = colourDef.Id,
            Value = "92",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "rich indigo",
            DefinitionId = colourDef.Id,
            Value = "93",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "deep indigo",
            DefinitionId = colourDef.Id,
            Value = "94",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "vivid indigo",
            DefinitionId = colourDef.Id,
            Value = "95",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "earthen brown",
            DefinitionId = colourDef.Id,
            Value = "96",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "deep brown",
            DefinitionId = colourDef.Id,
            Value = "97",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "rich brown",
            DefinitionId = colourDef.Id,
            Value = "98",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "burnt sienna",
            DefinitionId = colourDef.Id,
            Value = "99",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "chocolate",
            DefinitionId = colourDef.Id,
            Value = "100",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cinnamon",
            DefinitionId = colourDef.Id,
            Value = "101",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "mahogany",
            DefinitionId = colourDef.Id,
            Value = "102",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "nut brown",
            DefinitionId = colourDef.Id,
            Value = "103",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "umber",
            DefinitionId = colourDef.Id,
            Value = "104",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "amethyst",
            DefinitionId = colourDef.Id,
            Value = "105",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "mauve",
            DefinitionId = colourDef.Id,
            Value = "106",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "mulbery",
            DefinitionId = colourDef.Id,
            Value = "107",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "plum",
            DefinitionId = colourDef.Id,
            Value = "108",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lavender",
            DefinitionId = colourDef.Id,
            Value = "109",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "royal purple",
            DefinitionId = colourDef.Id,
            Value = "110",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded black",
            DefinitionId = colourDef.Id,
            Value = "111",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tattered black",
            DefinitionId = colourDef.Id,
            Value = "112",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "shabby black",
            DefinitionId = colourDef.Id,
            Value = "113",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy black",
            DefinitionId = colourDef.Id,
            Value = "114",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "off-white",
            DefinitionId = colourDef.Id,
            Value = "115",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy grey",
            DefinitionId = colourDef.Id,
            Value = "116",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blotched red",
            DefinitionId = colourDef.Id,
            Value = "117",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dull orange",
            DefinitionId = colourDef.Id,
            Value = "118",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bland yellow",
            DefinitionId = colourDef.Id,
            Value = "119",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded green",
            DefinitionId = colourDef.Id,
            Value = "120",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded blue",
            DefinitionId = colourDef.Id,
            Value = "121",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded indigo",
            DefinitionId = colourDef.Id,
            Value = "122",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded purple",
            DefinitionId = colourDef.Id,
            Value = "123",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "drab brown",
            DefinitionId = colourDef.Id,
            Value = "124",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dim grey",
            DefinitionId = colourDef.Id,
            Value = "125",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dusky slate grey",
            DefinitionId = colourDef.Id,
            Value = "126",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sooty grey",
            DefinitionId = colourDef.Id,
            Value = "127",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "chalky pale grey",
            DefinitionId = colourDef.Id,
            Value = "128",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dull mist grey",
            DefinitionId = colourDef.Id,
            Value = "129",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ashen off-white",
            DefinitionId = colourDef.Id,
            Value = "130",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dirty bone-white",
            DefinitionId = colourDef.Id,
            Value = "131",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "wan ivory",
            DefinitionId = colourDef.Id,
            Value = "132",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "spotted white",
            DefinitionId = colourDef.Id,
            Value = "133",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained white",
            DefinitionId = colourDef.Id,
            Value = "134",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blotched white",
            DefinitionId = colourDef.Id,
            Value = "135",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy off-white",
            DefinitionId = colourDef.Id,
            Value = "136",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained ivory",
            DefinitionId = colourDef.Id,
            Value = "137",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "shabby sallow-coloured",
            DefinitionId = colourDef.Id,
            Value = "138",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lurid pale yellow",
            DefinitionId = colourDef.Id,
            Value = "139",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy yellow",
            DefinitionId = colourDef.Id,
            Value = "140",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gaudy mustard yellow",
            DefinitionId = colourDef.Id,
            Value = "141",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sickly pale yellow",
            DefinitionId = colourDef.Id,
            Value = "142",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "shabby pale yellow",
            DefinitionId = colourDef.Id,
            Value = "143",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "murky brown",
            DefinitionId = colourDef.Id,
            Value = "144",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained brown",
            DefinitionId = colourDef.Id,
            Value = "145",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dreary brown",
            DefinitionId = colourDef.Id,
            Value = "146",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bland brown",
            DefinitionId = colourDef.Id,
            Value = "147",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "spotted muddy brown",
            DefinitionId = colourDef.Id,
            Value = "148",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dismal sand brown",
            DefinitionId = colourDef.Id,
            Value = "149",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dreary beige",
            DefinitionId = colourDef.Id,
            Value = "150",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy beige",
            DefinitionId = colourDef.Id,
            Value = "151",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "shabby beige",
            DefinitionId = colourDef.Id,
            Value = "152",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dirty beige",
            DefinitionId = colourDef.Id,
            Value = "153",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tattered beige",
            DefinitionId = colourDef.Id,
            Value = "154",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "bland wheat-coloured",
            DefinitionId = colourDef.Id,
            Value = "155",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "drab olive",
            DefinitionId = colourDef.Id,
            Value = "156",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "murky olive",
            DefinitionId = colourDef.Id,
            Value = "157",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dim olive",
            DefinitionId = colourDef.Id,
            Value = "158",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy green",
            DefinitionId = colourDef.Id,
            Value = "159",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "shabby green",
            DefinitionId = colourDef.Id,
            Value = "160",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dull green",
            DefinitionId = colourDef.Id,
            Value = "161",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sickly greyish-green",
            DefinitionId = colourDef.Id,
            Value = "162",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grisly brownish-green",
            DefinitionId = colourDef.Id,
            Value = "163",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "discoloured green",
            DefinitionId = colourDef.Id,
            Value = "164",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blotchy green",
            DefinitionId = colourDef.Id,
            Value = "165",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy rust-red",
            DefinitionId = colourDef.Id,
            Value = "166",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blotchy rust-red",
            DefinitionId = colourDef.Id,
            Value = "167",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy salmon",
            DefinitionId = colourDef.Id,
            Value = "168",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained salmon",
            DefinitionId = colourDef.Id,
            Value = "169",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blotched red",
            DefinitionId = colourDef.Id,
            Value = "170",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dull red",
            DefinitionId = colourDef.Id,
            Value = "171",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded red",
            DefinitionId = colourDef.Id,
            Value = "172",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained red",
            DefinitionId = colourDef.Id,
            Value = "173",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy red",
            DefinitionId = colourDef.Id,
            Value = "174",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded salmon",
            DefinitionId = colourDef.Id,
            Value = "175",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "well-worn blue",
            DefinitionId = colourDef.Id,
            Value = "176",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded slate blue",
            DefinitionId = colourDef.Id,
            Value = "177",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pallid blue",
            DefinitionId = colourDef.Id,
            Value = "178",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained blue",
            DefinitionId = colourDef.Id,
            Value = "179",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy blue",
            DefinitionId = colourDef.Id,
            Value = "180",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dim blue-black",
            DefinitionId = colourDef.Id,
            Value = "181",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded blue-black",
            DefinitionId = colourDef.Id,
            Value = "182",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dreary blue-black",
            DefinitionId = colourDef.Id,
            Value = "183",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dull orange",
            DefinitionId = colourDef.Id,
            Value = "184",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded reddish-orange",
            DefinitionId = colourDef.Id,
            Value = "185",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tattered reddish-orange",
            DefinitionId = colourDef.Id,
            Value = "186",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "discoloured orange",
            DefinitionId = colourDef.Id,
            Value = "187",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained orange-red",
            DefinitionId = colourDef.Id,
            Value = "188",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "drab peach-coloured",
            DefinitionId = colourDef.Id,
            Value = "189",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lurid peach-coloured",
            DefinitionId = colourDef.Id,
            Value = "190",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sickly peach-coloured",
            DefinitionId = colourDef.Id,
            Value = "191",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tattered violet",
            DefinitionId = colourDef.Id,
            Value = "192",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "grimy lavender",
            DefinitionId = colourDef.Id,
            Value = "193",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "spotted lavender",
            DefinitionId = colourDef.Id,
            Value = "194",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "discoloured purple",
            DefinitionId = colourDef.Id,
            Value = "195",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dirty purple",
            DefinitionId = colourDef.Id,
            Value = "196",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dingy purple",
            DefinitionId = colourDef.Id,
            Value = "197",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "faded purple",
            DefinitionId = colourDef.Id,
            Value = "198",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "stained purple",
            DefinitionId = colourDef.Id,
            Value = "199",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dusty faded purple",
            DefinitionId = colourDef.Id,
            Value = "200",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "blonde",
            DefinitionId = colourDef.Id,
            Value = "201",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dirty blonde",
            DefinitionId = colourDef.Id,
            Value = "202",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "silver blonde",
            DefinitionId = colourDef.Id,
            Value = "203",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ash blonde",
            DefinitionId = colourDef.Id,
            Value = "204",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "strawberry blonde",
            DefinitionId = colourDef.Id,
            Value = "205",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "platinum blonde",
            DefinitionId = colourDef.Id,
            Value = "206",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light blonde",
            DefinitionId = colourDef.Id,
            Value = "207",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "salt-and-pepper",
            DefinitionId = colourDef.Id,
            Value = "208",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "orange",
            DefinitionId = colourDef.Id,
            Value = "209",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light blue",
            DefinitionId = colourDef.Id,
            Value = "210",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light green",
            DefinitionId = colourDef.Id,
            Value = "211",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pale blue",
            DefinitionId = colourDef.Id,
            Value = "212",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "yellow",
            DefinitionId = colourDef.Id,
            Value = "213",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cyan",
            DefinitionId = colourDef.Id,
            Value = "214",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "navy blue",
            DefinitionId = colourDef.Id,
            Value = "215",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "reddish brown",
            DefinitionId = colourDef.Id,
            Value = "216",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "beige",
            DefinitionId = colourDef.Id,
            Value = "217",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light red",
            DefinitionId = colourDef.Id,
            Value = "218",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "purple",
            DefinitionId = colourDef.Id,
            Value = "219",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pink",
            DefinitionId = colourDef.Id,
            Value = "220",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark",
            DefinitionId = colourDef.Id,
            Value = "221",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "indian red",
            DefinitionId = colourDef.Id,
            Value = "222",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light pink",
            DefinitionId = colourDef.Id,
            Value = "223",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "violet red",
            DefinitionId = colourDef.Id,
            Value = "224",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "hot pink",
            DefinitionId = colourDef.Id,
            Value = "225",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "maroon red",
            DefinitionId = colourDef.Id,
            Value = "226",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "plum purple",
            DefinitionId = colourDef.Id,
            Value = "227",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "magenta red",
            DefinitionId = colourDef.Id,
            Value = "228",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cobalt blue",
            DefinitionId = colourDef.Id,
            Value = "229",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light steel blue",
            DefinitionId = colourDef.Id,
            Value = "230",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "slate gray",
            DefinitionId = colourDef.Id,
            Value = "231",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "turquoise blue",
            DefinitionId = colourDef.Id,
            Value = "232",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cyan blue",
            DefinitionId = colourDef.Id,
            Value = "233",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cobalt green",
            DefinitionId = colourDef.Id,
            Value = "234",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lime green",
            DefinitionId = colourDef.Id,
            Value = "235",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ivory white",
            DefinitionId = colourDef.Id,
            Value = "236",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "goldenrod yellow",
            DefinitionId = colourDef.Id,
            Value = "237",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark khaki",
            DefinitionId = colourDef.Id,
            Value = "238",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "banana yellow",
            DefinitionId = colourDef.Id,
            Value = "239",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "orange red",
            DefinitionId = colourDef.Id,
            Value = "240",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "moccasin brown",
            DefinitionId = colourDef.Id,
            Value = "241",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tan yellow",
            DefinitionId = colourDef.Id,
            Value = "242",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "brick brown",
            DefinitionId = colourDef.Id,
            Value = "243",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "carrot orange",
            DefinitionId = colourDef.Id,
            Value = "244",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "peachpuff pink",
            DefinitionId = colourDef.Id,
            Value = "245",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sienna brown",
            DefinitionId = colourDef.Id,
            Value = "246",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "saddle brown",
            DefinitionId = colourDef.Id,
            Value = "247",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "salmon pink",
            DefinitionId = colourDef.Id,
            Value = "248",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sepia brown",
            DefinitionId = colourDef.Id,
            Value = "249",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "fire brick brown",
            DefinitionId = colourDef.Id,
            Value = "250",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "teal blue",
            DefinitionId = colourDef.Id,
            Value = "251",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark gray",
            DefinitionId = colourDef.Id,
            Value = "252",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "pale violet",
            DefinitionId = colourDef.Id,
            Value = "253",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "violet red",
            DefinitionId = colourDef.Id,
            Value = "254",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lavender pink",
            DefinitionId = colourDef.Id,
            Value = "255",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "hot pink",
            DefinitionId = colourDef.Id,
            Value = "256",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "deep pink",
            DefinitionId = colourDef.Id,
            Value = "257",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "maroon red",
            DefinitionId = colourDef.Id,
            Value = "258",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "orchid pink",
            DefinitionId = colourDef.Id,
            Value = "259",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "plum purple",
            DefinitionId = colourDef.Id,
            Value = "260",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "fuchsia pink",
            DefinitionId = colourDef.Id,
            Value = "261",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "magenta red",
            DefinitionId = colourDef.Id,
            Value = "262",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "midnight blue",
            DefinitionId = colourDef.Id,
            Value = "263",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cobalt blue",
            DefinitionId = colourDef.Id,
            Value = "264",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cornflower blue",
            DefinitionId = colourDef.Id,
            Value = "265",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light steel blue",
            DefinitionId = colourDef.Id,
            Value = "266",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "steel blue",
            DefinitionId = colourDef.Id,
            Value = "267",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "slate gray",
            DefinitionId = colourDef.Id,
            Value = "268",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gray",
            DefinitionId = colourDef.Id,
            Value = "269",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "turquoise blue",
            DefinitionId = colourDef.Id,
            Value = "270",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "azure blue",
            DefinitionId = colourDef.Id,
            Value = "271",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cyan blue",
            DefinitionId = colourDef.Id,
            Value = "272",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "aquamarine",
            DefinitionId = colourDef.Id,
            Value = "273",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cobalt green",
            DefinitionId = colourDef.Id,
            Value = "274",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "mint green",
            DefinitionId = colourDef.Id,
            Value = "275",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "lime green",
            DefinitionId = colourDef.Id,
            Value = "276",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "chartreuse green",
            DefinitionId = colourDef.Id,
            Value = "277",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "ivory white",
            DefinitionId = colourDef.Id,
            Value = "278",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light yellow",
            DefinitionId = colourDef.Id,
            Value = "279",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "goldenrod yellow",
            DefinitionId = colourDef.Id,
            Value = "280",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "khaki",
            DefinitionId = colourDef.Id,
            Value = "281",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark khaki",
            DefinitionId = colourDef.Id,
            Value = "282",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gold",
            DefinitionId = colourDef.Id,
            Value = "283",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "banana yellow",
            DefinitionId = colourDef.Id,
            Value = "284",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "cornsilk yellow",
            DefinitionId = colourDef.Id,
            Value = "285",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "orange red",
            DefinitionId = colourDef.Id,
            Value = "286",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "wheat yellow",
            DefinitionId = colourDef.Id,
            Value = "287",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "moccasin brown",
            DefinitionId = colourDef.Id,
            Value = "288",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "eggshell white",
            DefinitionId = colourDef.Id,
            Value = "289",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tan yellow",
            DefinitionId = colourDef.Id,
            Value = "290",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "tan brown",
            DefinitionId = colourDef.Id,
            Value = "291",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "brick brown",
            DefinitionId = colourDef.Id,
            Value = "292",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "brick red",
            DefinitionId = colourDef.Id,
            Value = "293",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "carrot orange",
            DefinitionId = colourDef.Id,
            Value = "294",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark orange",
            DefinitionId = colourDef.Id,
            Value = "295",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "peachpuff pink",
            DefinitionId = colourDef.Id,
            Value = "296",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "seashell gray",
            DefinitionId = colourDef.Id,
            Value = "297",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sienna brown",
            DefinitionId = colourDef.Id,
            Value = "298",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "chocolate brown",
            DefinitionId = colourDef.Id,
            Value = "299",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "saddle brown",
            DefinitionId = colourDef.Id,
            Value = "300",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "light salmon pink",
            DefinitionId = colourDef.Id,
            Value = "301",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "salmon pink",
            DefinitionId = colourDef.Id,
            Value = "302",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "coral orange",
            DefinitionId = colourDef.Id,
            Value = "303",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "sepia brown",
            DefinitionId = colourDef.Id,
            Value = "304",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "orange brown",
            DefinitionId = colourDef.Id,
            Value = "305",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "fire brick brown",
            DefinitionId = colourDef.Id,
            Value = "306",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "beet red",
            DefinitionId = colourDef.Id,
            Value = "307",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "teal blue",
            DefinitionId = colourDef.Id,
            Value = "308",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "smoky white",
            DefinitionId = colourDef.Id,
            Value = "309",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "dark gray",
            DefinitionId = colourDef.Id,
            Value = "310",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "gray black",
            DefinitionId = colourDef.Id,
            Value = "311",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "deep blue",
            DefinitionId = colourDef.Id,
            Value = "312",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "winter blue",
            DefinitionId = colourDef.Id,
            Value = "313",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "storm blue",
            DefinitionId = colourDef.Id,
            Value = "314",
            Default = false,
            Pluralisation = 0
        });
        context.CharacteristicValues.Add(new CharacteristicValue
        {
            Id = nextId++,
            Name = "natural",
            DefinitionId = colourDef.Id,
            Value = "315",
            Default = false,
            Pluralisation = 0
        });

        var nextColourCharacteristicValue = 316;
        foreach (var value in new[]
                 {
                     "madder red", "kermes scarlet", "lac crimson", "alkanet purple", "orchil violet",
                     "tyrian purple", "woad blue", "egyptian blue", "azurite blue", "lapis blue",
                     "malachite green", "verdigris green", "orpiment yellow", "realgar orange",
                     "hematite red", "cinnabar red", "red ochre", "yellow ochre", "lamp black",
                     "bone black", "lead white", "chalk white", "walnut brown", "oak-gall black",
                     "pomegranate yellow", "saffron yellow", "henna orange", "faded madder red",
                     "dull ochre", "dusty red ochre", "dull egyptian blue", "faded woad blue",
                     "smoky lamp black", "chalky white", "tarnished lead white", "muddy walnut brown",
                     "dull verdigris green", "faded tyrian purple", "stained saffron yellow"
                 })
        {
            context.CharacteristicValues.Add(new CharacteristicValue
            {
                Id = nextId++,
                Name = value,
                DefinitionId = colourDef.Id,
                Value = (nextColourCharacteristicValue++).ToString(),
                Default = false,
                Pluralisation = 0
            });
        }

        string[] gemColours =
        [
            "aquamarine",
            "topaz",
            "citrine",
            "amethyst",
            "rose quartz",
            "quartz",
            "blue topaz",
            "agate",
            "peridot",
            "amber",
            "moonstone",
            "garnet",
            "sardonyx",
            "smoky quartz",
            "carnelian",
            "moss agate",
            "blue aventurine"
        ];

        string[] fineGemColours =
        [
            "jet",
            "ruby",
            "diamond",
            "sapphire",
            "opal",
            "emerald",
            "lapis lazuli",
            "onyx",
            "pearl",
            "sunstone",
            "turquoise",
            "jade",
            "malachite"
        ];

        string[] commonStones =
        [
            "granite",
            "shale",
            "flint",
            "basalt",
            "slate"
        ];

        string[] jewelleryMotifs =
        [
            "rosette",
            "vine scroll",
            "leaf",
            "knotwork",
            "interlace",
            "geometric",
            "star",
            "crescent",
            "sunburst",
            "cross",
            "heraldic beast",
            "floral spray",
            "beaded border",
            "filigree"
        ];

        string[] flowers =
        [
            "rose",
            "violet",
            "daisy",
            "jasmine",
            "lotus flower",
            "marigold",
            "lily",
            "chrysanthemum",
            "blossom",
            "ivy",
            "laurel"
        ];

        string[] metalFinishes =
        [
            "polished",
            "burnished",
            "brushed",
            "hammered",
            "chased",
            "engraved",
            "gilded",
            "silver-gilt",
            "blackened",
            "nielloed",
            "enameled"
        ];

        string[] beadPatterns =
        [
            "single strand",
            "double strand",
            "alternating beads",
            "graduated beads",
            "clustered beads",
            "knotted intervals",
            "spaced pendants",
            "symmetrical drops"
        ];

        string[] jewelleryShapes =
        [
            "round",
            "oval",
            "crescent",
            "teardrop",
            "lozenge",
            "quatrefoil",
            "rosette",
            "cabochon",
            "disc",
            "plaque",
            "hoop",
            "spiral"
        ];

        string[] inlayStyles =
        [
            "flush inlay",
            "raised inlay",
            "cloisonne",
            "champleve",
            "niello",
            "enamel",
            "mosaic",
            "wire inlay",
            "shell inlay"
        ];

        void AddCharacteristicValues(CharacteristicDefinition definition, IEnumerable<string> values)
        {
            var valueIndex = 1;
            foreach (var value in values)
            {
                context.CharacteristicValues.Add(new CharacteristicValue
                {
                    Id = nextId++,
                    Name = value,
                    DefinitionId = definition.Id,
                    Value = valueIndex++.ToString(),
                    Default = false,
                    Pluralisation = 0
                });
            }
        }

        static string BuildCharacteristicProfileDefinition(IEnumerable<string> values)
        {
            return $"<Values> {string.Join(" ", values.Select(x => $"<Value>{x}</Value>"))} </Values>";
        }

        var ancientPigmentFineColours = new[]
        {
            "madder red", "kermes scarlet", "lac crimson", "alkanet purple", "orchil violet",
            "tyrian purple", "woad blue", "egyptian blue", "azurite blue", "lapis blue",
            "malachite green", "verdigris green", "orpiment yellow", "realgar orange",
            "hematite red", "cinnabar red", "red ochre", "yellow ochre", "lamp black",
            "bone black", "lead white", "chalk white", "walnut brown", "oak-gall black",
            "pomegranate yellow", "saffron yellow", "henna orange"
        };

        var ancientPigmentDrabColours = new[]
        {
            "faded madder red", "dull ochre", "dusty red ochre", "dull egyptian blue",
            "faded woad blue", "smoky lamp black", "chalky white", "tarnished lead white",
            "muddy walnut brown", "dull verdigris green", "faded tyrian purple",
            "stained saffron yellow"
        };

        var basicColourProfileValues = new[]
        {
            "black", "white", "grey", "light grey", "dark grey", "red", "dark red", "blue", "dark blue",
            "green", "brown", "dark green", "orange", "light blue", "light green", "yellow", "light red",
            "purple", "pink", "olive"
        };

        var fineColourProfileValues = new[]
        {
            "light grey", "dark grey", "red", "dark red", "blue", "dark blue", "green", "brown",
            "dark green", "pale white", "olive", "caramel", "ebony", "emerald green", "cerulean",
            "violet", "sandy brown", "light brown", "dark brown", "auburn", "onyx", "obsidian",
            "midnight black", "ink black", "jet black", "pitch black", "ivory", "seashell", "snow white",
            "gleaming white", "pure white", "pearl white", "bright white", "bone white", "ghost white",
            "mist grey", "charcoal grey", "thistle grey", "smoky grey", "slate grey", "silver grey",
            "soft grey", "ash grey", "crimson", "scarlet", "ruby red", "blood red", "rose red",
            "wine red", "flame red", "coral", "copper", "fiery orange", "ochre", "sunset orange",
            "amber", "goldenrod", "pale yellow", "golden yellow", "sand yellow", "topaz hued",
            "gold-coloured", "spring green", "sea green", "hunter green", "olive green", "sage green",
            "pine green", "bright green", "rich green", "pale green", "verdant green", "forest green",
            "chartreuse", "slate blue", "bright blue", "powder blue", "sapphire blue", "royal blue",
            "ocean blue", "teal", "cornflour blue", "sky blue", "azure", "beryl", "cobalt",
            "rich indigo", "deep indigo", "vivid indigo", "earthen brown", "deep brown", "rich brown",
            "burnt sienna", "chocolate", "cinnamon", "mahogany", "nut brown", "umber", "amethyst",
            "mauve", "mulbery", "plum", "lavender", "royal purple", "orange", "light blue",
            "light green", "pale blue", "yellow", "cyan", "navy blue", "reddish brown", "beige"
        }.Concat(ancientPigmentFineColours);

        var drabColourProfileValues = new[]
        {
            "faded black", "tattered black", "shabby black", "grimy black", "off-white", "dingy grey",
            "bland yellow", "faded green", "faded blue", "faded indigo", "drab brown", "dim grey",
            "dusky slate grey", "sooty grey", "chalky pale grey", "dull mist grey", "ashen off-white",
            "dirty bone-white", "wan ivory", "spotted white", "stained white", "blotched white",
            "dingy off-white", "stained ivory", "shabby sallow-coloured", "lurid pale yellow",
            "dingy yellow", "gaudy mustard yellow", "sickly pale yellow", "shabby pale yellow",
            "murky brown", "stained brown", "dreary brown", "bland brown", "spotted muddy brown",
            "dismal sand brown", "dreary beige", "grimy beige", "shabby beige", "dirty beige",
            "tattered beige", "bland wheat-coloured", "drab olive", "murky olive", "dim olive",
            "dingy green", "shabby green", "dull green", "sickly greyish-green", "grisly brownish-green",
            "discoloured green", "blotchy green", "grimy rust-red", "blotchy rust-red", "grimy salmon",
            "stained salmon", "blotched red", "dull red", "faded red", "stained red", "dingy red",
            "faded salmon", "well-worn blue", "faded slate blue", "pallid blue", "stained blue",
            "grimy blue", "dim blue-black", "faded blue-black", "dreary blue-black", "dull orange",
            "faded reddish-orange", "tattered reddish-orange", "discoloured orange", "stained orange-red",
            "drab peach-coloured", "lurid peach-coloured", "sickly peach-coloured", "tattered violet",
            "grimy lavender", "spotted lavender", "discoloured purple", "dirty purple", "dingy purple",
            "faded purple", "stained purple", "dusty faded purple"
        }.Concat(ancientPigmentDrabColours);

        var mostColourProfileValues = new[]
        {
            "indian red", "light pink", "pink", "pale violet", "violet red", "lavender pink", "hot pink",
            "deep pink", "maroon red", "orchid pink", "thistle grey", "plum purple", "fuchsia pink",
            "magenta red", "purple", "slate blue", "blue", "navy blue", "midnight blue", "cobalt blue",
            "royal blue", "cornflower blue", "light steel blue", "steel blue", "slate gray", "gray",
            "sky blue", "turquoise blue", "azure blue", "cyan blue", "sea green", "green", "aquamarine",
            "spring green", "spring green", "emerald green", "cobalt green", "mint green", "pale green",
            "forest green", "pale blue", "lime green", "chartreuse green", "olive green", "ivory white",
            "white", "yellow", "light yellow", "goldenrod yellow", "khaki", "dark khaki", "gold",
            "banana yellow", "cornsilk yellow", "orange", "orange red", "off-white", "wheat yellow",
            "moccasin brown", "eggshell white", "tan yellow", "tan brown", "brick brown", "brick red",
            "carrot orange", "dark orange", "peachpuff pink", "seashell gray", "sandy brown",
            "sienna brown", "chocolate brown", "saddle brown", "burnt sienna", "light salmon pink",
            "salmon pink", "coral orange", "sepia brown", "orange brown", "rose red", "snow white",
            "light brown", "dark brown", "brown", "fire brick brown", "beet red", "teal blue",
            "smoky white", "dark gray", "black", "pitch black", "gray black"
        }.Concat(ancientPigmentFineColours);

        AddCharacteristicValues(gemDef, gemColours);
        AddCharacteristicValues(fineGemDef, fineGemColours);
        AddCharacteristicValues(commonStoneDef, commonStones);
        AddCharacteristicValues(jewelleryMotifDef, jewelleryMotifs);
        AddCharacteristicValues(flowerDef, flowers);
        AddCharacteristicValues(metalFinishDef, metalFinishes);
        AddCharacteristicValues(beadPatternDef, beadPatterns);
        AddCharacteristicValues(jewelleryShapeDef, jewelleryShapes);
        AddCharacteristicValues(inlayStyleDef, inlayStyles);

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "All_Colours",
            Type = "all",
            Definition = "<Definition/>",
            TargetDefinitionId = colourDef.Id,
            Description = "All defined colours"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Basic_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(basicColourProfileValues),
            TargetDefinitionId = colourDef.Id,
            Description = "Just basic colours like red, blue, brown etc"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Fine_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(fineColourProfileValues),
            TargetDefinitionId = colourDef.Id,
            Description = "All of the colours from the RPI Engine's $finecolor variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Drab_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(drabColourProfileValues),
            TargetDefinitionId = colourDef.Id,
            Description = "All of the colours from the RPI Engine's $drabcolor variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Gem_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(gemColours),
            TargetDefinitionId = gemDef.Id,
            Description = "All of the values from the RPI Engine's $gemcolor variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Fine_Gem_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(fineGemColours),
            TargetDefinitionId = fineGemDef.Id,
            Description = "All of the values from the RPI Engine's $finegemcolor variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Common_Stones",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(commonStones),
            TargetDefinitionId = commonStoneDef.Id,
            Description = "All of the values from the RPI Engine's $commonstone variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Motifs",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(jewelleryMotifs),
            TargetDefinitionId = jewelleryMotifDef.Id,
            Description = "Common medieval jewellery motifs for the $motif variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Flowers",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(flowers),
            TargetDefinitionId = flowerDef.Id,
            Description = "Common flowers and garland plants for the $flower variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Metal_Finishes",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(metalFinishes),
            TargetDefinitionId = metalFinishDef.Id,
            Description = "Common metal finishes for the $finish variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Bead_Patterns",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(beadPatterns),
            TargetDefinitionId = beadPatternDef.Id,
            Description = "Common bead arrangements for the $beadpattern variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Shapes",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(jewelleryShapes),
            TargetDefinitionId = jewelleryShapeDef.Id,
            Description = "Common jewellery shapes for the $shape variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Jewellery_Inlay_Styles",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(inlayStyles),
            TargetDefinitionId = inlayStyleDef.Id,
            Description = "Common jewellery inlay styles for the $inlay variable"
        });

        context.CharacteristicProfiles.Add(new CharacteristicProfile
        {
            Name = "Most_Colours",
            Type = "Standard",
            Definition = BuildCharacteristicProfileDefinition(mostColourProfileValues),
            TargetDefinitionId = colourDef.Id,
            Description = "Mostly all colours without the $drabcolor inclusions"
        });

        context.SaveChanges();
    }
}
