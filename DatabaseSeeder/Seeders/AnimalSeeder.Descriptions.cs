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
    #region Descriptions
    private void CreateDescription(EntityDescriptionType type, string text, FutureProg prog)
    {
        _context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
        {
            ApplicabilityProg = prog,
            Type = (int)type,
            RelativeWeight = 100,
            Pattern = text
        });
    }

    private void DogLongDescriptions(Race race)
    {
        void AddDogDescription(Ethnicity ethnicity, string description)
        {
            FutureProg prog = new()
            {
                FunctionName = $"IsDog{ethnicity.Name.CollapseString()}",
                FunctionComment =
                    $"Determines whether a character or character template is a dog of the {ethnicity.Name} breed",
                AcceptsAnyParameters = false,
                Category = "Character",
                Subcategory = "Descriptions",
                ReturnType = (long)ProgVariableTypes.Boolean,
                FunctionText =
                    $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Ethnicity == ToEthnicity({ethnicity.Id})"
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);

            CreateDescription(EntityDescriptionType.FullDescription, description, prog);
        }

        Ethnicity Breed(string name)
        {
            return _context.Ethnicities.First(x => x.ParentRaceId == race.Id && x.Name == name);
        }

        AddDogDescription(Breed("Terrier"),
            "This is &a_an[&age], &male terrier; a small, wiry and generally fearless breed of dog with short legs and an eager nature. They are excellent diggers and have a natural instinct to hunt vermin.");
        AddDogDescription(Breed("Setter"),
            "This is &a_an[&age], &male setter; a medium sized hunting dog that was bred to hunt birds. They hold their head high in the air as they move rather than tracking along the ground and they have an instinct to freeze or 'set' when they see their prey. They are readily trainable and very disciplined.");
        AddDogDescription(Breed("Pointer"),
            "This is &a_an[&age], &male pointer; a medium sized hunting dog that was bred to assist hunters using ranged weapons. They stalk prey in dense vegetation and upon sighting it will stand still in a 'point' with their muzzle at their prey.");
        AddDogDescription(Breed("Retriever"),
            "This is &a_an[&age], &male retriever; a medium sized hunting dog that was bred to bring back downed prey. They are loyal and friendly in disposition and relatively soft-mouthed, so make excellent pets.");
        AddDogDescription(Breed("Spaniel"),
            "This is &a_an[&age], &male spaniel; a small hunting dog bred for flushing game out of dense brush. They have distinctive long, silky coats and big droopy ears.");
        AddDogDescription(Breed("Water Dog"),
            "This is &a_an[&age], &male water dog; hunting and companion dogs that are excellent swimmers. They have long and thick hair around their torso to keep them warm in icy-cold water but short-coated limbs to reduce drag while swimming.");
        AddDogDescription(Breed("Sighthound"),
            "This is &a_an[&age], &male sighthound; a large, long-legged and lanky breed of dog with tremendous speed, flexibility and agility. They have long muzzles and flexible backs.");
        AddDogDescription(Breed("Scenthound"),
            "This is &a_an[&age], &male scenthound; a dog with a phenomenal sense of smell, even for their species. These dogs are short-legged and low to the ground, and have big, floppy ears and wet mouths.");
        AddDogDescription(Breed("Bulldog"),
            "This is &a_an[&age], &male bulldog; a stocky, square-bodied dog originally bred for fighting. They have flat faces and tremendous muscle.");
        AddDogDescription(Breed("Mastiff"),
            "This is &a_an[&age], &male mastiff; a large dog originally bred for guarding homes and for war. They have broad, somewhat flat faces and big feet.");
        AddDogDescription(Breed("Herding Dog"),
            "This is &a_an[&age], &male herding dog; a medium sized dog bred to assist in the herding of livestock. They tend to be low to the ground and agile, and have an instinct to nip at the heels of animals and herd small children.");
        AddDogDescription(Breed("Lap Dog"),
            "This is &a_an[&age], &male lap dog; a small, lap-sized dog with little use as a working dog. Purely ornamental, they tend to have a lazy but grumpy disposition and prefer to spend their time in the laps of their owners.");
        AddDogDescription(Breed("Mongrel"),
            "This is &a_an[&age], &male mongrel; a dog of indeterminate parentage and medium size.");
    }

    private void CreateDescriptionsForRace(Race race)
    {
        #region Progs

        FutureProg isRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}",
            FunctionComment = $"Determines whether a character or character template is a {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")"
        };
        isRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isRaceProg);

        FutureProg isAdultMaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}AdultMale",
            FunctionComment = $"Determines whether a character or character template is an adult male {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and In(@ch.AgeCategory, \"YoungAdult\", \"Adult\", \"Elder\", \"Venerable\")"
        };
        isAdultMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isAdultMaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isAdultMaleRaceProg);

        FutureProg isAdultFemaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}AdultFemale",
            FunctionComment =
                $"Determines whether a character or character template is an adult female {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and In(@ch.AgeCategory, \"YoungAdult\", \"Adult\", \"Elder\", \"Venerable\")"
        };
        isAdultFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isAdultFemaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isAdultFemaleRaceProg);

        FutureProg isJuvenileMaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}JuvenileMale",
            FunctionComment =
                $"Determines whether a character or character template is a juvenile male {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and In(@ch.AgeCategory, \"Child\", \"Youth\")"
        };
        isJuvenileMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isJuvenileMaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isJuvenileMaleRaceProg);

        FutureProg isJuvenileFemaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}JuvenileFemale",
            FunctionComment =
                $"Determines whether a character or character template is a juvenile female {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and In(@ch.AgeCategory, \"Child\", \"Youth\")"
        };
        isJuvenileFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isJuvenileFemaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isJuvenileFemaleRaceProg);

        FutureProg isBabyMaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}BabyMale",
            FunctionComment = $"Determines whether a character or character template is a baby male {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Male\") and @ch.AgeCategory == \"Baby\""
        };
        isBabyMaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isBabyMaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isBabyMaleRaceProg);

        FutureProg isBabyFemaleRaceProg = new()
        {
            FunctionName = $"Is{race.Name.CollapseString()}BabyFemale",
            FunctionComment = $"Determines whether a character or character template is a baby female {race.Name}",
            AcceptsAnyParameters = false,
            Category = "Character",
            Subcategory = "Descriptions",
            ReturnType = (long)ProgVariableTypes.Boolean,
            FunctionText =
                $"return @ch.Race == ToRace(\"{race.Name}\") and @ch.Gender == ToGender(\"Female\") and @ch.AgeCategory == \"Baby\""
        };
        isBabyFemaleRaceProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isBabyFemaleRaceProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isBabyFemaleRaceProg);

        #endregion

        void DoLazyDescriptions(string babyName, string juvenileName, string adultMaleName, string adultFemaleName)
        {
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyFemaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {juvenileName}",
                isJuvenileMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {juvenileName}",
                isJuvenileFemaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultMaleName}",
                isAdultMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultFemaleName}",
                isAdultFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
                isBabyMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
                isBabyFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {juvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
                isJuvenileMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {juvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
                isJuvenileFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is &a_an[&age] {adultMaleName}. &he is fully sized and sexually mature.",
                isAdultMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is &a_an[&age] {adultFemaleName}. &he is fully sized and sexually mature.",
                isAdultFemaleRaceProg);
        }

        void DoLazyDescriptionsWithMultipleJuvenile(string babyName, string maleJuvenileName,
            string femaleJuvenileName, string adultMaleName, string adultFemaleName)
        {
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a &male {babyName}", isBabyFemaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a {maleJuvenileName}",
                isJuvenileMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"a {femaleJuvenileName}",
                isJuvenileFemaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultMaleName}",
                isAdultMaleRaceProg);
            CreateDescription(EntityDescriptionType.ShortDescription, $"&a_an[&age] {adultFemaleName}",
                isAdultFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
                isBabyMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a &male {babyName}. &he is young enough to be wholly dependent on &his mother.",
                isBabyFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a {maleJuvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
                isJuvenileMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is a {femaleJuvenileName}, not yet fully sized. &he has characteristics somewhere between that of a baby and an adult, and is not yet sexually mature.",
                isJuvenileFemaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is &a_an[&age] {adultMaleName}. &he is fully sized and sexually mature.",
                isAdultMaleRaceProg);
            CreateDescription(EntityDescriptionType.FullDescription,
                $"This is &a_an[&age] {adultFemaleName}. &he is fully sized and sexually mature.",
                isAdultFemaleRaceProg);
        }

        if (race.Name == "Cat")
        {
            CharacteristicDefinition definition = new()
            {
                Name = "Cat Coat",
                Type = (int)CharacteristicType.Multiform,
                Pattern = "catcoat",
                Description = "Various coats that cats can have",
                ChargenDisplayType = (int)CharacterGenerationDisplayType.DisplayAll,
                Model = "standard",
                Definition = ""
            };
            _context.CharacteristicDefinitions.Add(definition);

            CharacteristicProfile profile = new()
            {
                Name = "All Cat Coats",
                Definition = "<Profile/>",
                Description = "All values of the Cat Coats characteristic definition",
                TargetDefinition = definition,
                Type = "all"
            };
            _context.CharacteristicProfiles.Add(profile);

            race.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
            {
                Race = race,
                CharacteristicDefinition = definition,
                Usage = "base"
            });
            Ethnicity ethnicity = _context.Ethnicities.Local.First(x => x.ParentRace == race);
            ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
            {
                Ethnicity = ethnicity,
                CharacteristicDefinition = definition,
                CharacteristicProfile = profile
            });

            List<CharacteristicValue> solidCats = new();
            List<CharacteristicValue> tabbyCats = new();
            List<CharacteristicValue> orientalCats = new();

            void AddCharacteristicValue(string name, string fancy, bool solid = false, bool tabby = false,
                bool oriental = false)
            {
                CharacteristicValue value = new()
                {
                    Definition = definition,
                    Default = false,
                    Pluralisation = 0,
                    Name = name,
                    Value = $"{name}-coated",
                    AdditionalValue = fancy
                };
                _context.CharacteristicValues.Add(value);

                if (solid)
                {
                    solidCats.Add(value);
                }

                if (tabby)
                {
                    tabbyCats.Add(value);
                }

                if (oriental)
                {
                    orientalCats.Add(value);
                }
            }

            AddCharacteristicValue("black", "&his coat is a solid, evenly distributed black.", true);
            AddCharacteristicValue("blue-grey", "&his coat is a solid, evenly distributed blue-grey.", true);
            AddCharacteristicValue("blue-caramel", "&his coat is a solid, evenly distributed blue-caramel.",
                true);
            AddCharacteristicValue("chocolate", "&his coat is a solid, evenly distributed chocolate.", true);
            AddCharacteristicValue("lilac", "&his coat is a solid, evenly distributed lilac.", true);
            AddCharacteristicValue("taupe", "&his coat is a solid, evenly distributed taupe.", true);
            AddCharacteristicValue("cinnamon", "&his coat is a solid, evenly distributed cinnamon.", true);
            AddCharacteristicValue("fawn", "&his coat is a solid, evenly distributed fawn.", true);
            AddCharacteristicValue("ginger", "&his coat is a solid, evenly distributed ginger.", true);
            AddCharacteristicValue("cream", "&his coat is a solid, evenly distributed cream.", true);
            AddCharacteristicValue("apricot", "&his coat is a solid, evenly distributed apricot.", true);
            AddCharacteristicValue("albino", "&he is an albino cat, with unpigmented fur and pink eyes.",
                true);
            AddCharacteristicValue("white", "&his coat is a solid, evenly distributed white.", true);

            AddCharacteristicValue("brown tabby",
                "&his coat is a tabby pattern of black on coppery-brown, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("blue tabby",
                "&his coat is a tabby pattern of blue-grey on warm brown, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("chocolate tabby",
                "&his coat is a tabby pattern of dark chocolate on light chocolate brown, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("lilac tabby",
                "&his coat is a tabby pattern of frosty grey on pale lavender, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("cinnamon tabby",
                "&his coat is a tabby pattern of cinnamon on honey, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("fawn tabby",
                "&his coat is a tabby pattern of dense fawn on pale ivory, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("red tabby",
                "&his coat is a tabby pattern of deep red on orange, in a pattern almost reminiscent of a tiger.",
                tabby: true);
            AddCharacteristicValue("cream tabby",
                "&his coat is a tabby pattern of peach on pale cream, in a pattern almost reminiscent of a tiger.",
                tabby: true);

            AddCharacteristicValue("seal point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are deep seal and the rest pale fawn.",
                oriental: true);
            AddCharacteristicValue("blue point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are blue grey and the rest bluish white.",
                oriental: true);
            AddCharacteristicValue("chocolate point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are milk chocolate and the rest ivory.",
                oriental: true);
            AddCharacteristicValue("lilac point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are pinkish grey and the rest pearly grey.",
                oriental: true);
            AddCharacteristicValue("cinnamon point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
                oriental: true);
            AddCharacteristicValue("fawn point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
                oriental: true);
            AddCharacteristicValue("flame point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
                oriental: true);
            AddCharacteristicValue("cream point",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
                oriental: true);

            AddCharacteristicValue("seal mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are dark brown and the rest medium brown.",
                oriental: true);
            AddCharacteristicValue("blue mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are slate blue and the rest soft blue-grey.",
                oriental: true);
            AddCharacteristicValue("chocolate mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium brown and the rest buff-cream.",
                oriental: true);
            AddCharacteristicValue("lilac mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are frosty grey and the rest silvery grey.",
                oriental: true);
            AddCharacteristicValue("cinnamon mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
                oriental: true);
            AddCharacteristicValue("fawn mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
                oriental: true);
            AddCharacteristicValue("flame mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
                oriental: true);
            AddCharacteristicValue("cream mink",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
                oriental: true);

            AddCharacteristicValue("seal sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are dark brown and the rest sable.",
                oriental: true);
            AddCharacteristicValue("blue sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium blue and the rest soft lighter blue-grey.",
                oriental: true);
            AddCharacteristicValue("chocolate sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are medium brown and the rest coffee-brown.",
                oriental: true);
            AddCharacteristicValue("lilac sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are frosty grey and the rest dove grey.",
                oriental: true);
            AddCharacteristicValue("cinnamon sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are bright reddish-brown and the rest warm ivory.",
                oriental: true);
            AddCharacteristicValue("fawn sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are warm tan and the rest pale ivory.",
                oriental: true);
            AddCharacteristicValue("flame sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are orange-red and the rest white.",
                oriental: true);
            AddCharacteristicValue("cream sepia",
                "&he has a form of albinism where the 'points' or face, tail and feet are a darker colour than the rest of the body. In this case, &his points are cream and the rest white.",
                oriental: true);

            AddCharacteristicValue("tuxedo",
                "&he has a black coat with white mitts, white belly, white chin and a white tail tip.");
            AddCharacteristicValue("blue tuxedo",
                "&he has a blue-grey coat with white mitts, white belly, white chin and a white tail tip.");
            AddCharacteristicValue("chocolate tuxedo",
                "&he has a chocolate-brown coat with white mitts, white belly, white chin and a white tail tip.");
            AddCharacteristicValue("ginger tuxedo",
                "&he has a ginger coat with white mitts, white belly, white chin and a white tail tip.");

            profile = new CharacteristicProfile
            {
                Name = "Solid Cat Coats",
                Definition =
                    $"<Values>\n{(from value in solidCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
                Description = "All solid colour cat coats",
                TargetDefinition = definition,
                Type = "Standard"
            };
            _context.CharacteristicProfiles.Add(profile);

            profile = new CharacteristicProfile
            {
                Name = "Tabby Cat Coats",
                Definition =
                    $"<Values>\n{(from value in tabbyCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
                Description = "All tabby colour cat coats",
                TargetDefinition = definition,
                Type = "Standard"
            };
            _context.CharacteristicProfiles.Add(profile);

            profile = new CharacteristicProfile
            {
                Name = "Oriental Cat Coats",
                Definition =
                    $"<Values>\n{(from value in orientalCats select $"<Value>{value.Id}</Value>").ListToLines(true)}</Values>",
                Description = "All oriental cat coats (siamese, tonkinese, burmese)",
                TargetDefinition = definition,
                Type = "Standard"
            };
            _context.CharacteristicProfiles.Add(profile);
        }

        if (TryGetRaceTemplate(race.Name, out AnimalRaceTemplate? template))
        {
            CreateDescriptionsFromPack(race, template.DescriptionPack, isAdultMaleRaceProg, isAdultFemaleRaceProg,
                isJuvenileMaleRaceProg, isJuvenileFemaleRaceProg, isBabyMaleRaceProg, isBabyFemaleRaceProg);
            return;
        }

        Dictionary<string, string> extraAdultDescriptions = new(StringComparer.OrdinalIgnoreCase)
{
{"Coyote", "A rangy canine with tawny fur and sharp features. Its wary eyes and lean frame speak of a life spent hunting small game and scavenging."},
{"Hyena", "This hyena has a powerful frame with a sloping back and mottled fur. Its muzzle ends in strong jaws capable of crushing bone."},
{"Rabbit", "A small mammal with long ears, soft fur and a twitching nose. Its powerful hind legs allow it to bound away at the first sign of trouble."},
{"Hare", "Slender and long-legged, this hare sports large ears and a short tail. Its muscles are built for rapid bursts of speed across open ground."},
{"Beaver", "Stout and water-loving, the beaver has dense brown fur and large teeth for gnawing wood. A wide, flat tail aids in swimming."},
{"Otter", "Sleek and playful, this otter has glossy fur and webbed feet. Its streamlined body is built for a life spent swimming and hunting fish."},
{"Lion", "A majestic big cat with a muscular body and, in males, a thick mane. Its amber eyes regard the world with regal confidence."},
{"Cheetah", "This spotted cat has a lean build and long legs made for incredible bursts of speed. Black tear lines run from eyes to mouth."},
{"Leopard", "A powerful feline covered in rosetted spots. Muscular limbs and strong jaws make it an adept climber and ambush predator."},
{"Tiger", "A large striped cat with a massive head and muscular frame. Its orange coat with black stripes helps it stalk through dense foliage."},
{"Panther", "A lithe big cat with a glossy dark coat. Its movements are quiet and precise, well suited to stalking prey."},
{"Jaguar", "Stockier than the leopard, this big cat's rosettes have central spots. It has a broad head and immensely powerful jaws."},
{"Jackal", "A small, cunning canine with a bushy tail and large ears. Its coat is a mix of tawny and grey, perfect for blending into scrubland."},
{"Deer", "This graceful herbivore has slender legs and a sleek coat. Mature males sport antlers that are shed and regrown each year."},
{"Moose", "Towering and long-legged, the moose has a bulbous nose and a heavy dewlap. The males' broad antlers spread wide from their heads."},
{"Boar", "A wild boar with bristly dark hair and sharp tusks protruding from its lower jaw. Its body is thickset and muscular."},
{"Warthog", "Sparse-bristled and wiry, this pig has prominent facial warts and upward curving tusks. It carries itself with surprising speed."},
{"Cow", "A placid bovine with a broad muzzle and gentle eyes. Its large stomach betrays its grass-fed diet."},
{"Ox", "A powerfully built bovine used for labour, with a strong neck and broad shoulders. It bears a patient disposition."},
{"Buffalo", "This buffalo is sturdy and heavyset with sweeping horns. Its hide is tough and it moves with deliberate strength."},
{"Bison", "A hulking creature with a massive head and a shaggy mane of dark fur around its shoulders, tapering to a smaller hindquarters."},
{"Hippopotamus", "Huge and barrel-bodied, the hippo spends much of its time wallowing in water. Its enormous mouth opens to reveal long tusks."},
{"Horse", "A large hoofed animal with a flowing mane and tail. Its powerful legs and strong back make it well suited to riding or hauling."},
{"Bear", "Broad and thickly furred, this bear walks with a lumbering gait. It possesses long claws and a keen sense of smell."},
{"Rhinocerous", "Heavily built with thick grey skin and one or two prominent horns on its snout. Its hide bears folds that look like natural armour."},
{"Giraffe", "An extraordinarily tall creature with a spotted coat and a very long neck. It moves with a slow, loping stride."},
{"Elephant", "An enormous herbivore with a long trunk and large ears. Its ivory tusks curve outward from a massive head."},
{"Fox", "A small, agile canine with reddish fur and a bushy tail. Its pointed ears and muzzle give it a cunning appearance."},
{"Rat", "A small rodent with coarse fur, a scaly tail and beady black eyes. It sniffs the air constantly with twitching whiskers."},
{"Mouse", "Tiny and quick, this mouse has soft fur and large ears. Its long tail helps it balance while scurrying about."},
{"Hamster", "A chubby little rodent with expandable cheek pouches. It has soft fur and tiny paws well adapted for digging."},
{"Ferret", "Slender and inquisitive, the ferret has a long body and short legs. Its coat is silky, and its nose is always searching for scents."},
{"Weasel", "This small predator has a sleek body and sharp teeth. Its fur is smooth and its movements fast and sinuous."},
{"Guinea Pig", "A plump rodent with short legs and no tail. It makes a variety of squeaks and whistles when excited."},
{"Badger", "Stocky and low to the ground, the badger has a distinctive striped face and formidable digging claws."},
{"Wolverine", "Muscular and thick-furred, the wolverine is larger than a badger and known for its ferocity. Its broad paws help it move through snow."},
{"Goat", "A sure-footed ungulate with a short tail and backward curving horns. It happily grazes on rough plants that many animals avoid."},
    {"Llama", "This long-necked camelid has a shaggy coat and an alert expression. It's often used as a pack animal in mountainous regions."},
    {"Alpaca", "Smaller than a llama, this camelid is prized for its soft, luxurious fleece. Large eyes give it a gentle appearance."},

    {"Pigeon", "A plump, short-legged bird with iridescent feathers about its neck. It coos softly while bobbing its head."},
    {"Swallow", "A small bird with long, pointed wings and a forked tail. It darts through the air catching insects."},
    {"Sparrow", "A tiny brown songbird with streaked plumage and a quick, hopping gait."},
    {"Quail", "This squat ground bird sports a topknot of feathers and short rounded wings."},
    {"Grouse", "A chicken-like bird with mottled feathers that blend well with woodland undergrowth."},
    {"Pheasant", "A brightly coloured game bird with a long tail and vivid plumage, especially on the males."},
    {"Seagull", "A coastal bird with grey wings and a raucous cry, often seen scavenging along the shoreline."},
    {"Albatross", "Large and long-winged, this seabird glides effortlessly over the waves for hours at a time."},
    {"Heron", "A tall wading bird with a spear-like beak and long legs for stalking fish in shallow waters."},
    {"Crane", "Elegant and long-legged, the crane moves with slow, deliberate steps and has a bugling call."},
    {"Flamingo", "This pink bird stands on one leg while filtering food from the water with its curved bill."},
    {"Peacock", "A spectacular bird with a metallic blue neck and a fan of brilliant eye-spotted tail feathers."},
    {"Ibis", "A long-legged wader with a down-curved bill, probing mud for small creatures."},
    {"Pelican", "Large and heavy-billed, the pelican scoops up fish with the flexible pouch beneath its beak."},
    {"Crow", "A glossy black bird known for its intelligence and harsh cawing voice."},
    {"Raven", "Bigger than a crow, the raven has shaggy throat feathers and a deep, resonant croak."},
    {"Emu", "A tall, flightless bird with shaggy feathers and powerful legs built for running."},
    {"Ostrich", "The largest of birds, the ostrich has a long neck and strong legs capable of swift kicks."},
    {"Moa", "A massive, extinct flightless bird recreated here with heavy legs and a thick body."},
    {"Vulture", "A bald-headed scavenger with broad wings, circling patiently for carrion."},
    {"Parrot", "A colourful bird with a hooked beak and a squawking voice, often mimicking sounds."},
    {"Woodpecker", "This bird clings to tree trunks, hammering with its beak in search of insects."},
    {"Kingfisher", "A small bird with a large head and vivid plumage, diving swiftly for fish."},
    {"Stork", "Long-legged and long-billed, the stork is often seen wading in wetlands."},
    {"Penguin", "A tuxedo-feathered swimmer that waddles awkwardly on land but soars through the sea."},
    {"Duck", "A waterfowl with a broad bill and webbed feet, quacking softly as it paddles."},
    {"Goose", "Larger than a duck, this bird has a long neck and is quick to honk at intruders."},
    {"Swan", "Graceful and white-feathered, the swan glides serenely across the water."},
    {"Chicken", "A domesticated fowl kept for its eggs and meat, clucking as it scratches at the ground."},
    {"Turkey", "A large game bird with a fan-shaped tail and a fleshy wattle dangling from its beak."},
    {"Hawk", "A sharp-eyed raptor with hooked talons, circling high before diving on prey."},
    {"Eagle", "Powerful and regal, this large bird of prey has a commanding wingspan and piercing gaze."},
    {"Falcon", "A sleek raptor built for speed, with narrow wings and swift, decisive strikes."},
    {"Owl", "A nocturnal hunter with a rounded face and silent wings, staring from huge eyes."}
};

        void AddExtraAdultDescription()
        {
            if (extraAdultDescriptions.TryGetValue(race.Name, out string? text))
            {
                CreateDescription(EntityDescriptionType.FullDescription, text, isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription, text, isAdultFemaleRaceProg);
            }
        }

        switch (race.Name)
        {
            case "Dog":
                CreateDescription(EntityDescriptionType.ShortDescription, "a male &ethnicity pup",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a male &ethnicity pup",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
                    isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] &male &ethnicity",
                    isAdultFemaleRaceProg);
                _context.SaveChanges();
                DogLongDescriptions(race);
                break;
            case "Wolf":
                CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf pup",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf pup",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf whelp",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf whelp",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a male grey wolf", isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a female grey wolf",
                    isAdultFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a pup, too young to survive on its own. Small and somewhat cute, it is covered in grey fur. When fully grown, this will be a male gray wolf.",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a pup, too young to survive on its own. Small and somewhat cute, it is covered in grey fur. When fully grown, this will be a female gray wolf.",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a young &male wolf. Not yet fully mature, &he would nonetheless stand a chance at survival if isolated from &his pack. &his features resemble that of an adult of &his species, but there are still some puppy-like qualities to &his form and behaviour.",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a young &male wolf. Not yet fully mature, &he would nonetheless stand a chance at survival if isolated from &his pack. &his features resemble that of an adult of &his species, but there are still some puppy-like qualities to &his form and behaviour.",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] &male wolf. &he is slender and powerfully built with a deeply descending rib cage, a sloping back and a heavily muscled neck. &he has small, triangular ears and long legs that signal swift movement. &he has large, heavy teeth and a powerful jaw adapted to crushing bone.",
                    isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] &male wolf. &he is slender and powerfully built with a deeply descending rib cage, a sloping back and a heavily muscled neck. &he has small, triangular ears and long legs that signal swift movement. &he has large, heavy teeth and a powerful jaw adapted to crushing bone.",
                    isAdultFemaleRaceProg);
                break;
            case "Coyote":
                DoLazyDescriptions("coyote pup", "coyote whelp", "male coyote", "female coyote");
                AddExtraAdultDescription();
                break;
            case "Hyena":
                DoLazyDescriptions("hyena pup", "hyena whelp", "male hyena", "female hyena");
                AddExtraAdultDescription();
                break;
            case "Rabbit":
                DoLazyDescriptions("bunny", "young rabbit", "buck rabbit", "doe rabbit");
                AddExtraAdultDescription();
                break;
            case "Hare":
                DoLazyDescriptions("hare bunny", "young hare", "buck hare", "doe hare");
                AddExtraAdultDescription();
                break;
            case "Beaver":
                DoLazyDescriptions("beaver cub", "young beaver", "male beaver", "female beaver");
                AddExtraAdultDescription();
                break;
            case "Otter":
                DoLazyDescriptions("otter cub", "young otter", "male otter", "female otter");
                AddExtraAdultDescription();
                break;
            case "Cat":
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] kitten",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a young $catcoat cat",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] tomcat",
                    isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[$catcoat] cat",
                    isAdultFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a &male kitten of the common domestic cat. $catcoatfancy", isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a young &male common domestic cat. $catcoatfancy.", isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This animal is a young &male common domestic cat. $catcoatfancy", isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] &male common domestic cat. $catcoatfancy", isAdultFemaleRaceProg);
                break;
            case "Lion":
                DoLazyDescriptionsWithMultipleJuvenile("lion cub", "juvenile lion", "juvenile lioness", "lion",
                        "lioness");
                AddExtraAdultDescription();
                break;
            case "Cheetah":
                DoLazyDescriptions("cheetah cub", "juvenile cheetah", "cheetah", "she-cheetah");
                AddExtraAdultDescription();
                break;
            case "Leopard":
                DoLazyDescriptionsWithMultipleJuvenile("leopard cub", "juvenile leopard", "juvenile leopardess",
                        "leopard", "leopardess");
                AddExtraAdultDescription();
                break;
            case "Tiger":
                DoLazyDescriptionsWithMultipleJuvenile("tiger cub", "juvenile tiger", "juvenile tigress", "tiger",
                        "tigress");
                AddExtraAdultDescription();
                break;
            case "Panther":
                DoLazyDescriptions("panther cub", "juvenile panther", "male panther", "female panther");
                AddExtraAdultDescription();
                break;
            case "Jaguar":
                DoLazyDescriptions("jaguar cub", "juvenile jaguar", "male jaguar", "female jaguar");
                AddExtraAdultDescription();
                break;
            case "Jackal":
                DoLazyDescriptions("jackal cub", "juvenile jackal", "male jackal", "female jackal");
                AddExtraAdultDescription();
                break;
            case "Deer":
                DoLazyDescriptionsWithMultipleJuvenile("fawn", "juvenile stag", "juvenile doe", "stag", "doe");
                AddExtraAdultDescription();
                break;
            case "Moose":
                DoLazyDescriptionsWithMultipleJuvenile("moose calf", "young bull moose", "moose heifer",
                        "bull moose", "moose cow");
                AddExtraAdultDescription();
                break;
            case "Pig":
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male piglet", isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male piglet", isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] hog",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] sow",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] hog", isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] sow", isAdultFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is an adorable &male piglet with big, curious eyes and a little curly tail. It is still small and likely fully reliant on its mother.",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is an adorable &male piglet with big, curious eyes and a little curly tail. It is still small and likely fully reliant on its mother.",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a juvenile hog, not yet fully sized. He has a large head and a long snout, and feet with four hoofed toes each.",
                    isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a juvenile sow, not yet fully sized. She has a large head and a long snout, and feet with four hoofed toes each.",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] hog. He is very large in size and has a large head and a long snout, and feet with four hoofed toes each.",
                    isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is &a_an[&age] &male wolf. She has a large head and a long snout, and feet with four hoofed toes each.",
                    isAdultFemaleRaceProg);
                break;
            case "Sheep":
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male lamb", isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male lamb", isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male hogget", isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "a &male hogget",
                    isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] ram", isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.ShortDescription, "&a_an[&age] ewe", isAdultFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription, "This is a &male lamb.",
                    isBabyMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription, "This is a &male lamb.",
                    isBabyFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a &male hogget, or yearling sheep.", isJuvenileMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription,
                    "This is a &male hogget, or yearling sheep.", isJuvenileFemaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription, "This is &a_an[&age] &male sheep, or ram.",
                    isAdultMaleRaceProg);
                CreateDescription(EntityDescriptionType.FullDescription, "This is &a_an[&age] &male sheep, or ewe.",
                    isAdultFemaleRaceProg);
                break;
            case "Boar":
                DoLazyDescriptionsWithMultipleJuvenile("boar piglet", "young wild boar", "young wild sow",
                        "wild boar", "wild sow");
                AddExtraAdultDescription();
                break;
            case "Warthog":
                DoLazyDescriptionsWithMultipleJuvenile("warthog piglet", "young warthog boar", "young warthog sow",
                        "warthog boar", "warthog sow");
                AddExtraAdultDescription();
                break;
            case "Cow":
                DoLazyDescriptionsWithMultipleJuvenile("calf", "young bull", "heifer", "bull", "cow");
                AddExtraAdultDescription();
                break;
            case "Ox":
                DoLazyDescriptionsWithMultipleJuvenile("ox calf", "young ox bull", "ox heifer", "ox bull",
                        "ox cow");
                AddExtraAdultDescription();
                break;
            case "Buffalo":
                DoLazyDescriptionsWithMultipleJuvenile("buffalo calf", "young buffalo bull", "buffalo heifer",
                        "buffalo bull", "buffalo cow");
                AddExtraAdultDescription();
                break;
            case "Bison":
                DoLazyDescriptionsWithMultipleJuvenile("bison calf", "young bison bull", "bison heifer",
                        "bison bull", "bison cow");
                AddExtraAdultDescription();
                break;
            case "Hippopotamus":
                DoLazyDescriptionsWithMultipleJuvenile("hippo calf", "young hippo bull", "young hippo cow",
                        "hippo bull", "hippo cow");
                AddExtraAdultDescription();
                break;
            case "Horse":
                DoLazyDescriptionsWithMultipleJuvenile("foal", "colt", "filly", "stallion", "mare");
                AddExtraAdultDescription();
                break;
            case "Bear":
                DoLazyDescriptions("&ethnicity cub", "young &ethnicity", "male &ethnicity", "female &ethnicity");
                AddExtraAdultDescription();
                break;
            case "Rhinocerous":
                DoLazyDescriptions("rhino calf", "young rhino", "rhino bull", "rhino cow");
                AddExtraAdultDescription();
                break;
            case "Giraffe":
                DoLazyDescriptions("giraffe calf", "young giraffe", "male giraffe", "female giraffe");
                AddExtraAdultDescription();
                break;
            case "Elephant":
                DoLazyDescriptionsWithMultipleJuvenile("elephant calf", "juvenile bull elephant",
                        "juvenile elephant", "bull elephant", "elephant");
                AddExtraAdultDescription();
                break;
            case "Fox":
                DoLazyDescriptionsWithMultipleJuvenile("fox kit", "juvenile male fox", "juvenile fox vixen",
                        "male fox", "fox vixen");
                AddExtraAdultDescription();
                break;
            case "Rat":
                DoLazyDescriptions("rat pup", "young rat", "buck rat", "doe rat");
                AddExtraAdultDescription();
                break;
            case "Mouse":
                DoLazyDescriptions("mouse pup", "mouse rat", "buck mouse", "doe mouse");
                AddExtraAdultDescription();
                break;
            case "Hamster":
                DoLazyDescriptions("hamster pup", "young hamster", "buck hamster", "doe hamster");
                AddExtraAdultDescription();
                break;
            case "Ferret":
                DoLazyDescriptions("ferret kit", "young ferret", "jack ferret", "jill ferret");
                AddExtraAdultDescription();
                break;
            case "Weasel":
                DoLazyDescriptions("weasel kit", "young weasel", "jack weasel", "jill weasel");
                AddExtraAdultDescription();
                break;
            case "Guinea Pig":
                DoLazyDescriptions("guinea pig pup", "young guinea pig", "guinea pig boar", "guinea pig sow");
                AddExtraAdultDescription();
                break;
            case "Badger":
                DoLazyDescriptions("badger kit", "young badger", "boar badger", "sow badger");
                AddExtraAdultDescription();
                break;
            case "Wolverine":
                DoLazyDescriptions("wolverine kit", "young wolverine", "boar wolverine", "sow wolverine");
                AddExtraAdultDescription();
                break;
            case "Goat":
                DoLazyDescriptionsWithMultipleJuvenile("kid goat", "buckling goat",
                        "doeling goat", "billy goat", "nanny goat");
                AddExtraAdultDescription();
                break;
            case "Llama":
                DoLazyDescriptions("llama cria", "young llama", "llama macho", "llama hembra");
                AddExtraAdultDescription();
                break;
            case "Alpaca":
                DoLazyDescriptions("alpaca cria", "young alpaca", "alpaca macho", "alpaca hembra");
                AddExtraAdultDescription();
                break;
            case "Pigeon":
            case "Swallow":
            case "Sparrow":
            case "Quail":
            case "Grouse":
            case "Pheasant":
            case "Seagull":
            case "Albatross":
            case "Heron":
            case "Crane":
            case "Flamingo":
            case "Peacock":
            case "Ibis":
            case "Pelican":
            case "Crow":
            case "Raven":
            case "Emu":
            case "Ostrich":
            case "Moa":
            case "Vulture":
            case "Parrot":
            case "Woodpecker":
            case "Kingfisher":
            case "Stork":
            case "Penguin":
                DoLazyDescriptions($"{race.Name.ToLowerInvariant()} chick",
                        $" fledgling {race.Name.ToLowerInvariant()}", $"male {race.Name.ToLowerInvariant()}",
                        $"female {race.Name.ToLowerInvariant()}");
                AddExtraAdultDescription();
                break;

            case "Duck":
                DoLazyDescriptionsWithMultipleJuvenile("duckling", "fledgling drake", "fledgling duck", "drake",
                        "duck");
                AddExtraAdultDescription();
                break;
            case "Goose":
                DoLazyDescriptionsWithMultipleJuvenile("gosling", "fledgling gander", "fledgling goose", "gander",
                        "goose");
                AddExtraAdultDescription();
                break;
            case "Swan":
                DoLazyDescriptionsWithMultipleJuvenile("cygnet", "fledgling swan cob", "fledgling swan pen",
                        "swan cob", "swan pen");
                AddExtraAdultDescription();
                break;
            case "Chicken":
                DoLazyDescriptionsWithMultipleJuvenile("chicklet", "fledgling rooster", "fledgling hen", "rooster",
                        "hen");
                AddExtraAdultDescription();
                break;
            case "Turkey":
                DoLazyDescriptionsWithMultipleJuvenile("poult", "fledgling turkey gobbler", "fledgling turkey hen",
                        "turkey gobbler", "turkey hen");
                AddExtraAdultDescription();
                break;
            case "Hawk":
                DoLazyDescriptions("hawk chick", "fledgling hawk", "male hawk", "female hawk");
                AddExtraAdultDescription();
                break;
            case "Eagle":
                DoLazyDescriptions("eaglet", "fledgling eagle", "male eagle", "female eagle");
                AddExtraAdultDescription();
                break;
            case "Falcon":
                DoLazyDescriptions("falcon chick", "fledgling falcon", "male falcon", "female falcon");
                AddExtraAdultDescription();
                break;
            case "Owl":
                DoLazyDescriptions("owlet", "fledgling owl", "male owl", "female owl");
                AddExtraAdultDescription();
                break;
            default:
                DoLazyDescriptions($"baby {race.Name.ToLowerInvariant()}",
                    $"juvenile {race.Name.ToLowerInvariant()}", race.Name.ToLowerInvariant(),
                    race.Name.ToLowerInvariant());
                break;
        }
    }
    #endregion
}
