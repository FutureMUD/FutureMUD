#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
    public void SeedMiddleEarthHeritage()
    {
        BodyProto body = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
        Race humanoid = _context.Races.First(x => x.Name == "Organic Humanoid");
        CharacteristicDefinition personDef = _context.CharacteristicDefinitions.First(x => x.Name == "Person Word");

        void SetStockChargenSizeProg(string functionName, string stockText)
        {
            FutureProg prog = _context.FutureProgs.First(x => x.FunctionName == functionName);
            if (!string.IsNullOrWhiteSpace(prog.FunctionText) &&
                !prog.FunctionText.Contains("You will need to expand this as you add new playable races",
                    StringComparison.Ordinal) &&
                !prog.FunctionText.Contains("Stock Middle-Earth chargen size override", StringComparison.Ordinal))
            {
                return;
            }

            prog.FunctionText = $"// Stock Middle-Earth chargen size override\r\n{stockText}";
        }

        SetStockChargenSizeProg("MaximumHeightChargen",
            @"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
	if (@ch.Ethnicity.Group == ""Edain"")
	  if (@ch.Gender == ToGender(""Male""))
		// 7'2""
		return 218.5
	  else
		// 6'11""
		return 211
	  end if
	end if
	if (@ch.Gender == ToGender(""Male""))
	  // 6'9""
	  return 205.75
	else
	  // 6'7""
	  return 200.5
	end if
  case (ToRace(""Elf""))
	if (@ch.Gender == ToGender(""Male""))
	  // 6'9""
	  return 205.75
	else
	  // 6'7""
	  return 200.5
	end if
  case (ToRace(""Dwarf""))
	if (@ch.Gender == ToGender(""Male""))
	  // 4'0""
	  return 121
	else
	  // 3'11""
	  return 119
	end if
  case (ToRace(""Hobbit""))
	if (@ch.Gender == ToGender(""Male""))
	  // 4'0""
	  return 121
	else
	  // 3'11""
	  return 119
	end if
  case (ToRace(""Orc""))
	if (@ch.Gender == ToGender(""Male""))
	  // 6'0""
	  return 181
	else
	  // 6'0""
	  return 181
	end if
  case (ToRace(""Troll""))
	if (@ch.Gender == ToGender(""Male""))
	  // 15'
	  return 510
	else
	  // 15'
	  return 510
	end if
end switch
return 200");
        SetStockChargenSizeProg("MaximumWeightChargen",
            @"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
	bmi = 50
  case (ToRace(""Elf""))
	bmi = 30
  case (ToRace(""Hobbit""))
	bmi = 60
  case (ToRace(""Dwarf""))
	bmi = 55
  case (ToRace(""Orc""))
	bmi = 60
  case (ToRace(""Troll""))
	bmi = 60
  default
	bmi = 50
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000");
        SetStockChargenSizeProg("MinimumHeightChargen",
            @"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
	if (@ch.Ethnicity.Group == ""Edain"")
	  if (@ch.Gender == ToGender(""Male""))
		// 5'10""
		return 178
	  else
		// 5'8""
		return 173
	  end if
	end if
	if (@ch.Gender == ToGender(""Male""))
	  // 5'0""
	  return 152
	else
	  // 4'10""
	  return 147
	end if
  case (ToRace(""Elf""))
	if (@ch.Gender == ToGender(""Male""))
	  // 5'10""
	  return 178
	else
	  // 5'8""
	  return 173
	end if
  case (ToRace(""Dwarf""))
	if (@ch.Gender == ToGender(""Male""))
	  // 3'2""
	  return 97
	else
	  // 3'1""
	  return 94
	end if
  case (ToRace(""Hobbit""))
	if (@ch.Gender == ToGender(""Male""))
	  // 3'0""
	  return 91
	else
	  // 3'0""
	  return 89
	end if
  case (ToRace(""Orc""))
	if (@ch.Gender == ToGender(""Male""))
	  // 4'6""
	  return 137
	else
	  // 4'6""
	  return 137
	end if
  case (ToRace(""Troll""))
	if (@ch.Gender == ToGender(""Male""))
	  // 10'
	  return 300
	else
	  // 10'
	  return 300
	end if
end switch
return 100");
        SetStockChargenSizeProg("MinimumWeightChargen",
            @"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
	bmi = 16
  case (ToRace(""Elf""))
	bmi = 12
  case (ToRace(""Hobbit""))
	bmi = 18
  case (ToRace(""Dwarf""))
	bmi = 20
  case (ToRace(""Orc""))
	bmi = 15
  case (ToRace(""Troll""))
	bmi = 20
  default
	bmi = 16
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000");

        FutureProg CreateVariantAgeProg(FutureProg baseProg,
            Race race)
        {
            FutureProg prog = new()
            {
                FunctionName = baseProg.FunctionName.Replace("Humanoid", race.Name.CollapseString()),
                AcceptsAnyParameters = baseProg.AcceptsAnyParameters,
                StaticType = baseProg.StaticType,
                ReturnType = baseProg.ReturnType,
                Category = baseProg.Category,
                Subcategory = baseProg.Subcategory,
                FunctionComment = baseProg.FunctionComment.Replace("humanoid", race.Name),
                FunctionText = baseProg.FunctionText.Replace("ToRace(\"Humanoid\")", $"ToRace(\"{race.Name}\")")
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterType = 8200,
                ParameterName = "ch"
            });
            _context.FutureProgs.Add(prog);
            return prog;
        }

        FutureProg isBabyProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidBaby");
        FutureProg isToddlerProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidToddler");
        FutureProg isBoyProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidBoy");
        FutureProg isGirlProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidGirl");
        FutureProg isChildProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidChild");
        FutureProg isYouthProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYouth");
        FutureProg isYoungManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYoungMan");
        FutureProg isYoungWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYoungWoman");
        FutureProg isAdultManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdultMan");
        FutureProg isAdultWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdultWoman");
        FutureProg isAdultProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdult");
        FutureProg isOldManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldMan");
        FutureProg isOldWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldWoman");
        FutureProg isOldPersonProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldPerson");

        #region Elves

        FutureProg canSelectElfProg = new()
        {
            FunctionName = "CanSelectElfRace",
            Category = "Character",
            Subcategory = "Race",
            FunctionComment = "Determines if the character select the elf race in chargen.",
            ReturnType = (long)ProgVariableTypes.Boolean,
            StaticType = 0,
            FunctionText = @"// You might consider checking RPP
return true"
        };
        canSelectElfProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectElfProg,
            ParameterIndex = 0,
            ParameterType = 8200,
            ParameterName = "ch"
        });
        _context.FutureProgs.Add(canSelectElfProg);
        _context.SaveChanges();

        (Liquid? elfBlood, Liquid? elfSweat, Material? driedElfBlood, Material? driedElfSweat) = CreateBloodAndSweat("Elven");

        Race elfRace = new()
        {
            Name = "Elf",
            Description = @"Elves are a immortal race of beings who are highly skilled in magic and crafting. They are tall and slender, with pointed ears and an innate connection to the natural world. Elves are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art. They are also skilled warriors, but prefer to use their weapons only as a last resort. 

Elves are divided into several different clans, each with their own distinct culture and way of life. The most well-known of these clans are the Vanyar, the Noldor, and the Teleri. The Vanyar are the fairest and most powerful of all the Elves, and are closely associated with the Valar, the angelic beings who serve the creator god of the universe. The Noldor are the most intellectually curious of the Elves, and are known for their prowess in magic and crafting. The Teleri are the most elusive and reclusive of the clans, and are associated with the sea.",
            AllowedGenders = _humanRace.AllowedGenders,
            ParentRace = humanoid,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 2.0,
            CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
            DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NaturalArmourQuality = 3,
            NeedsToBreathe = true,
            BreathingModel = "simple",
            SweatRateInLitresPerMinute = 0.1,
            SizeStanding = 6,
            SizeProne = 6,
            SizeSitting = 6,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1 3",
            MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
            MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
            RaceUsesStamina = true,
            CanEatCorpses = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            CanEatMaterialsOptIn = false,
            TemperatureRangeFloor = -20,
            TemperatureRangeCeiling = 50,
            BodypartSizeModifier = 0,
            BodypartHealthMultiplier = 1,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
            CanClimb = true,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            ChildAge = 8,
            YouthAge = 20,
            YoungAdultAge = 50,
            AdultAge = 200,
            ElderAge = 1000,
            VenerableAge = 3000,
            AvailabilityProg = canSelectElfProg,
            BaseBody = body,
            BloodLiquid = elfBlood,
            BloodModel = humanoid.BloodModel,
            NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
            NaturalArmourType = humanoid.NaturalArmourType,
            RaceButcheryProfile = null,
            SweatLiquid = elfSweat,
			MaximumFoodSatiatedHours = CultureRaceSatiationLimits["Elf"].MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = CultureRaceSatiationLimits["Elf"].MaximumDrinkSatiatedHours
        };
        _context.Races.Add(elfRace);
        AddRaceAttributeAlterations(elfRace, CultureRaceAttributeProfiles["Elf"]);

        HeightWeightModel elfMaleHWModel = new()
        {
            Name = "Elf Male",
            MeanHeight = 187,
            MeanBmi = 21,
            StddevHeight = 7.6,
            StddevBmi = 1.2,
            Bmimultiplier = 0.1
        };
        _context.Add(elfMaleHWModel);
        HeightWeightModel elfFemaleHWModel = new()
        {
            Name = "Elf Female",
            MeanHeight = 180,
            MeanBmi = 21.5,
            StddevHeight = 5,
            StddevBmi = 1.2,
            Bmimultiplier = 0.1
        };
        _context.Add(elfFemaleHWModel);
        elfRace.DefaultHeightWeightModelMale = elfMaleHWModel;
        elfRace.DefaultHeightWeightModelFemale = elfFemaleHWModel;
        elfRace.DefaultHeightWeightModelNonBinary = elfFemaleHWModel;
        elfRace.DefaultHeightWeightModelNeuter = elfMaleHWModel;
        _races.Add("Elf", elfRace);
        _context.SaveChanges();

        FutureProg isElfProg = new()
        {
            FunctionName = "IsRaceElf",
            FunctionComment = "Determines whether someone is the Elf race",
            FunctionText = $"return @ch.Race == ToRace({elfRace.Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isElfProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isElfProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isElfProg);
        _context.SaveChanges();
        _raceProgs["Elf"] = isElfProg;

        FutureProg isElfBabyProg = CreateVariantAgeProg(isBabyProg, elfRace);
        FutureProg isElfToddlerProg = CreateVariantAgeProg(isToddlerProg, elfRace);
        FutureProg isElfBoyProg = CreateVariantAgeProg(isBoyProg, elfRace);
        FutureProg isElfGirlProg = CreateVariantAgeProg(isGirlProg, elfRace);
        FutureProg isElfChildProg = CreateVariantAgeProg(isChildProg, elfRace);
        FutureProg isElfYouthProg = CreateVariantAgeProg(isYouthProg, elfRace);
        FutureProg isElfYoungManProg = CreateVariantAgeProg(isYoungManProg, elfRace);
        FutureProg isElfYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, elfRace);
        FutureProg isElfAdultManProg = CreateVariantAgeProg(isAdultManProg, elfRace);
        FutureProg isElfAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, elfRace);
        FutureProg isElfAdultPersonProg = CreateVariantAgeProg(isAdultProg, elfRace);
        FutureProg isElfOldManProg = CreateVariantAgeProg(isOldManProg, elfRace);
        FutureProg isElfOldWomanProg = CreateVariantAgeProg(isOldWomanProg, elfRace);
        FutureProg isElfOldPersonProg = CreateVariantAgeProg(isOldPersonProg, elfRace);
        List<(CharacteristicValue Value, double Weight)> elfPersonValues = new();
        long nextId = _context.CharacteristicValues.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

        void AddElfPersonWord(string name, string basic, FutureProg prog, double weight)
        {
            CharacteristicValue pw = new()
            {
                Id = nextId++,
                Definition = personDef,
                Name = name,
                Value = basic,
                AdditionalValue = "",
                Default = false,
                Pluralisation = 0,
                FutureProg = prog
            };
            _context.CharacteristicValues.Add(pw);
            elfPersonValues.Add((pw, weight));
        }

        AddElfPersonWord("elven baby", "elven baby", isElfBabyProg, 1.0);
        AddElfPersonWord("elven infant", "elven baby", isElfBabyProg, 1.0);
        AddElfPersonWord("elven tot", "elven baby", isElfBabyProg, 1.0);
        AddElfPersonWord("elven toddler", "elven toddler", isElfToddlerProg, 5.0);
        AddElfPersonWord("elf", "elf", isElfAdultPersonProg, 5.0);
        AddElfPersonWord("elven person", "elf", isElfAdultPersonProg, 5.0);
        AddElfPersonWord("elf person", "elf", isElfAdultPersonProg, 1.0);
        AddElfPersonWord("elven individual", "elf", isElfAdultPersonProg, 1.0);
        AddElfPersonWord("male elf", "elf", isElfAdultManProg, 1.0);
        AddElfPersonWord("elven man", "elf", isElfAdultManProg, 5.0);
        AddElfPersonWord("elf man", "elf", isElfAdultManProg, 1.0);
        AddElfPersonWord("female elf", "elf", isElfAdultWomanProg, 1.0);
        AddElfPersonWord("elven woman", "elf", isElfAdultWomanProg, 5.0);
        AddElfPersonWord("elf woman", "elf", isElfAdultWomanProg, 1.0);
        AddElfPersonWord("elven lady", "elf", isElfAdultWomanProg, 3.0);
        AddElfPersonWord("elf lady", "elf", isElfAdultWomanProg, 1.0);
        AddElfPersonWord("elven lad", "elven boy", isElfBoyProg, 1.0);
        AddElfPersonWord("elf boy", "elven boy", isElfBoyProg, 3.0);
        AddElfPersonWord("elven boy", "elven boy", isElfBoyProg, 3.0);
        AddElfPersonWord("elven child", "elven child", isElfChildProg, 3.0);
        AddElfPersonWord("elven youth", "elven youth", isElfYouthProg, 3.0);
        AddElfPersonWord("elven adolescent", "elven youth", isElfYouthProg, 1.0);
        AddElfPersonWord("elven youngster", "elven youth", isElfYouthProg, 1.0);
        AddElfPersonWord("elven juvenile", "elven youth", isElfYouthProg, 1.0);
        AddElfPersonWord("elven kid", "elven child", isElfChildProg, 1.0);
        AddElfPersonWord("young elven boy", "elven boy", isElfBoyProg, 1.0);
        AddElfPersonWord("young elf", "elf", isElfYoungManProg, 3.0);
        AddElfPersonWord("elven youngster", "elf", isElfYoungManProg, 1.0);
        AddElfPersonWord("elven lass", "elven girl", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("elf lass", "elven girl", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("elven girl", "elven girl", isElfGirlProg, 3.0);
        AddElfPersonWord("elf girl", "elven girl", isElfGirlProg, 3.0);
        AddElfPersonWord("young elven girl", "elven girl", isElfGirlProg, 1.0);
        AddElfPersonWord("young elf girl", "elven girl", isElfGirlProg, 1.0);
        AddElfPersonWord("young elven woman", "elven girl", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("young elf", "elven girl", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("elf maiden", "elven elf", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("elven maiden", "elven elf", isElfYoungWomanProg, 1.0);
        AddElfPersonWord("old elf", "old elf", isElfOldPersonProg, 1.0);
        AddElfPersonWord("old elven woman", "old elf", isElfOldWomanProg, 1.0);
        AddElfPersonWord("elderly elven woman", "old elf", isElfOldWomanProg, 1.0);
        AddElfPersonWord("old elven man", "old elf", isElfOldManProg, 1.0);
        AddElfPersonWord("elderly elven man", "old elf", isElfOldManProg, 1.0);
        AddElfPersonWord("elderly elf", "old elf", isElfOldPersonProg, 1.0);
        _context.SaveChanges();

        CharacteristicProfile elfPersons = new()
        {
            Name = "Elf Person Words",
            Definition = new XElement("Values",
                from item in elfPersonValues
                select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
            ).ToString(),
            Type = "weighted",
            Description = "All person words applicable to the elven race",
            TargetDefinition = personDef
        };
        _context.CharacteristicProfiles.Add(elfPersons);
        _profiles[elfPersons.Name] = elfPersons;
        _context.SaveChanges();

        void AddElfEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
            string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(elfRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? canSelectElfProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Elf Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddElfEthnicity("Noldor",
            "The Noldor are often called \"High Elves\" ostensibly because they are considered to be the most noble of the Quendi (Elves) in Middle-Earth. In reality, they are so named because they are the only Elves living in Endor who have ever resided in the Blessed Realm of Aman across the sea. This exalted status is accentuated by their close ties with the Valar, a relationship which accounts for their unique cultural and linguistic roots.\r\n\r\nOf all the Elves of Middle-Earth, the Noldor are the most ordered. While their brethren are content to wander or mark time in quiet diffusion, the Noldor seek to build communities and states in beautiful, guarded places. ",
            "Tatyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Black_Brown_Blonde_Grey_Hair", "fair_skin");
        AddElfEthnicity("Vanyar",
            "The Vanyar, also called the Fair Elves and Light-elves, were the first and smallest of the Kindreds of the Eldar. Under the leadership of Ingwë, the Vanyar were the first to set forth on the Great Journey and reach the shores of Belegaer; they sailed to Aman on the first voyage of Tol Eressëa and remained there permanently. Very few, if any, Vanyar remained in or returned to Middle-earth after the Great Journey.\r\n\r\nThe Vanyar were the fairest of all the Elves, hence their name \"Fair Elves\". Unlike other kindreds attracted to the sea or the building and forging of things, the Vanyar culture seemed to revolve about the Valar and Valinor. This is probably the reason why they chose to stay in Valinor proper, centered around Taniquetil, the seat of the rulers of Arda, and loved the light of Valinor best of all the other places in Aman. They greatly valued the wisdom of the Valar and were thus favored by Manwë and Varda, and always distrusted by Melkor. They were known to have had the greatest skill in poetry of all Elves, likened to Manwë, who loved them for it. Their hair was golden, and their banners were white.",
            "Minyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Blonde_Grey_Hair", "fair_skin", false);
        AddElfEthnicity("Teleri",
            "The Teleri (meaning \"Those who come last\") were the third of the Elf clans who came to Aman. Those who came to Aman became known as the Falmari. They were the ancestors of the Valinorean Teleri, and the Sindar, Laiquendi, and Nandor of Middle-earth.\r\n\r\nThe Teleri refused to join the Noldor in leaving Valinor, and many of them were cruelly slain in the Kinslaying at their chief city of Alqualondë, or Swan Harbour, when they refused to supply the exiles with their swan ships. For this reason few or none of the Teleri joined the host of the Valar which set out to capture Morgoth for good. It is recounted that the Teleri eventually forgave the Noldor for the Kinslaying, and the two kindreds were at peace again.",
            "Nelyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Brown_Blonde_Grey_Hair", "fair_skin", false);
        AddElfEthnicity("Sindar",
            "The Sindar or \"Grey-Elves\" are Eldar and were originally part of the great kindred called the Teleri. Unlike the Noldor, Vanyar and the bulk of the Teleri, the Sindar chose not to cross over the sea to Aman; instead they stayed in Middle-Earth. They, like the Silvan Elves, are part of the Moriquendi, the \"Dark Elves\" who never saw the light of Valinor.\r\n\r\nThe Sindar are the most open and cooperative of Middle-Earth's Elves. They are great teachers and borrowers and have an interest in the works of all races. Grey-Elves are a settled people and enjoy the company of others. \r\n\r\nMany of the Sindar feel a kinship to the sea. They build superb ships and are renowned sailors.",
            "Nelyar", "Moriquendi", "Majority O Minor A", "Blue_Eyes", "Brown_Blonde_Grey_Hair", "fair_skin");
        AddElfEthnicity("Silvan",
            "When the Eldar departed from the original Elven homeland during the Elder Days, a number of their brethren remained behind. They decided not to seek the light of the Aman and were labeled as the Avari (meaning \"Unwilling\" or \"Refusers\" in Quenya). These kindreds were left to fend for themselves during the days when Morgoth's Shadow swept over the East. In these dark times they were forced into the secluded safety of the forests of eastern Middle-Earth, where they wandered and hid from the wild Men who dominated most of the lands. They became known as the Silvan or Wood-Elves.\r\n\r\nThe culture of the Silvan Elves is best characterised as unstructured and rustic by Elven standards, but rich and relatively advanced when compared to the ways of Men. They have always been independent, but as of late many have settled in kingdoms ruled by other Elves such as the Noldor or Sindar. Still, all Silvan folk enjoy a good journey or adventure and most look at life much as a game to be played.\r\n\r\nMusic and trickery are their favourite pastimes. The Silvan Elves are also mastered of the wood and know much of wood-craft and wood-lore.",
            "Nelyar", "Moriquendi", "Overwhelmingly O", "Blue_Green_Eyes", "Black_Brown_Blonde_Grey_Hair",
            "Fair_Olive_Skin");

        FutureProg elfSkillProg = new()
        {
            FunctionName = "ElfSkillStartingValue",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine the opening value for a skill for elves at character creation",
            ReturnType = (int)ProgVariableTypes.Number,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
        };
        elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = elfSkillProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (int)ProgVariableTypes.Toon
        });
        elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = elfSkillProg,
            ParameterIndex = 1,
            ParameterName = "trait",
            ParameterType = (int)ProgVariableTypes.Trait
        });
        elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = elfSkillProg,
            ParameterIndex = 2,
            ParameterName = "boosts",
            ParameterType = (int)ProgVariableTypes.Number
        });
        _context.FutureProgs.Add(elfSkillProg);
        _context.SaveChanges();

        Calendar? quenyaCalendar = _context.Calendars.Find(1L);
        Calendar? sindarCalendar = _context.Calendars.Find(2L) ?? quenyaCalendar;

        void AddElfCulture(string name, bool useQuenyaCalendar, string description)
        {
            FutureProg prog = new()
            {
                FunctionName = $"CanSelect{name.CollapseString()}Culture",
                Category = "Chargen",
                Subcategory = "Culture",
                FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
                ReturnType = (int)ProgVariableTypes.Boolean,
                AcceptsAnyParameters = false,
                Public = false,
                StaticType = 0,
                FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Elf"")"
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);
            AddCulture(name, "Eldarin", description, _alwaysTrueProg, elfSkillProg,
                useQuenyaCalendar ? quenyaCalendar : sindarCalendar);
        }

        AddElfCulture("Rivendell", true,
            @"Rivendell, also known as Imladris in Sindarin, was an Elven town and the house of Elrond located in Middle-earth. It is described as ""The Last Homely House East of the Sea"" in reference to Valinor, which was west of the Great Sea in Aman.

The peaceful, sheltered town of Rivendell was located at the edge of a narrow gorge of the river Bruinen, but well hidden in the moorlands and foothills of the Misty Mountains. One of the main approaches to Rivendell came from the nearby Ford of Bruinen.

In Imladris, there was a large hall with a dais and several tables for feasting. Another hall, the Hall of Fire, had a fire in it year-round with carven pillars on either side of the hearth; it was used for singing and storytelling on high days, but stood empty otherwise, and people would come there alone to think and ponder. The eastern side of the house had a porch at which Frodo Baggins found his friends once he had awakened, and was where the Council of Elrond was held.

Rivendell was protected from attack (mainly by the River Bruinen, Elrond, and Elven magic), but Elrond himself said that Rivendell was a place of peace and learning, not a stronghold of battle.");
        AddElfCulture("Lothlórien", true,
            @"Lothlórien, also known as Lórien, was a forest and Elven realm near the lower Misty Mountains. It was first settled by Nandorin Elves, but they were later joined by a small number of Ñoldor and Sindar under Celeborn of Doriath and Galadriel, daughter of Finarfin. It was located on the River Celebrant, southeast of Khazad-dum, and was the only place in Middle-earth where the golden Mallorn trees grew.

Galadriel's magic, later revealed as the power of her ring Nenya, enriched the land and made it a magic forest into which evil could not enter without difficulty. The only way that Lothlórien could have been conquered by the armies of Mordor is if Sauron had come there himself.

After Galadriel left for Valinor, the Elves of Lórien were ruled by their lord Celeborn alone, and the realm was expanded to include a part of southern Mirkwood, but it appears to have quickly been de-populated during the Fourth Age. In ""The Tale of Aragorn and Arwen,"" the timeless Elven kingdom is depicted as being wholly abandoned by the time of King Elessar's passing. Even after the assaults on Lórien by Sauron's forces during the War of the Ring, there must have been several thousand Silvan Elves remaining in the land. Celeborn himself went to dwell in Rivendell, whilst many of the other Elves likely moved to Thranduil's Woodland Realm.

Galadriel bore Nenya on a ship from the Grey Havens into the West, accompanied by the other two Elven Rings and their bearers. With the Ring gone, the magic and beauty of Lórien also faded, along with the extraordinary Mallorn trees that had lived for centuries, and it was gradually depopulated. By the time Arwen came there to die in FO 121, Lothlórien was deserted. She was buried on the hill of Cerin Amroth, where she and Aragorn had been betrothed.");
        AddElfCulture("Mithlond", true,
            @"The Grey Havens, known also as Mithlond, was an Elvish port city on the Gulf of Lune in the Elven realm of Lindon in Middle-earth. 

Because of its cultural and spiritual importance to the Elves, the Grey Havens in time became the primary Elven settlement west of the Misty Mountains prior to the establishment of Eregion and, later, Rivendell. Even after the death of Gil-galad and as the Elves dwindled in numbers by the year, the Grey Havens remained a major Elven settlement and the main departure point of Elven ships to Aman.

Despite being a major port, by the late Third Age the Grey Havens had sparse population, like Rivendell and northeastern Mirkwood.");
        AddElfCulture("Forlindon", true,
            @"Forlindon was the northern part of Lindon, which was separated from Harlindon by the Gulf of Lune. The Elven-haven and harbour of Forlond lay on the southern coast of the region, and was ruled by Gil-galad. Like the rest of Lindon it was a remnant of Beleriand that sank under the Great Sea after the War of Wrath, at the end of the First Age.

Lindon was a region of western Middle-earth. Initially populated by Laiquendi, in the following Ages it became an important Elvish realm, known for its harbors and Elven ships that would embark unto the West");
        AddElfCulture("Hardlindon", false,
            @"Harlindon was a green and fair land on the northwestern shores of Middle-earth. It was located west of the Blue Mountains and south of the Gulf of Lune which divided Lindon into the northern Forlindon and the southern Harlindon. At the head of the Gulf lay the seaport Grey Havens. There were woods at the foot of the Blue Mountains and a haven in a small inlet on the southern shores of the bay of Harlond.

Lindon was a region of western Middle-earth. Initially populated by Laiquendi, in the following Ages it became an important Elvish realm, known for its harbors and Elven ships that would embark unto the West");
        AddElfCulture("Woodland Realm", false,
            @"The Woodland Realm was a kingdom of Silvan Elves located deep in Mirkwood, the great forest of Rhovanion, beginning in the Second Age. Following the War of the Last Alliance, Thranduil of the Sindar ruled over the Silvan Elves. Those of the Woodland Realm were known to be less wise and more dangerous than other Elves, but by the late Third Age were the only remaining Elven realm with a king.");
        _context.SaveChanges();

        #endregion Elves

        #region Men

        FutureProg isHumanProg = new()
        {
            FunctionName = "IsRaceHuman",
            FunctionComment = "Determines whether someone is the Human race",
            FunctionText = $"return @ch.Race == ToRace({_context.Races.First(x => x.Name == "Human").Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isHumanProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isHumanProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isHumanProg);
        _context.SaveChanges();
        _raceProgs["Human"] = isHumanProg;

        void AddHumanEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
            string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(_humanRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? _alwaysTrueProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Weighted Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddHumanEthnicity("Eriadoran", @"Eriadoran men are a hardy and independent people who value self-sufficiency and personal freedom. They are skilled farmers, herders, and hunters, and are known for their practicality and resourcefulness. Eriadoran culture is deeply rooted in tradition, and family ties are highly valued. Eriadoran men are also known for their love of song, poetry, and storytelling, and these art forms play a central role in their cultural identity.

Eriadoran men are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, Eriadoran men are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.", "Middle Man", "Eriadoran", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Arnorian", @"Arnorian men are a proud and noble people with a rich cultural heritage. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. Arnorian culture is deeply rooted in tradition, and the history and legends of their forefathers are an important part of their identity.

Arnorian men are known for their bravery, honor, and loyalty. They are skilled warriors, and have a long tradition of military service. They are also deeply religious, and their faith plays a central role in their lives. Arnorian men are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Middle Man", "Arnorian", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Gondorian", @"Gondorian men are a proud and honorable people with a rich cultural heritage. They are descended from the ancient kingdom of Gondor, which was founded by the brothers Isildur and Anarion during the Second Age. Gondorian culture is deeply rooted in tradition, and the history and legends of their forefathers are an important part of their identity.

Gondorian men are known for their bravery, honor, and loyalty. They are skilled warriors, and have a long tradition of military service. They are also deeply religious, and their faith plays a central role in their lives. Gondorian men are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage. Gondorian men are proud of their civilization and its achievements, and are deeply committed to its defense and preservation.", "Middle Man", "Gondorian", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Rohirrim", @"The Rohirrim are a proud and noble people with a strong sense of honor and loyalty. They are skilled horsemen and warriors, and have a deep connection to the land and their herds. The Rohirrim value independence and freedom, and are fiercely protective of their families and homes. They are known for their kindness and generosity towards their friends and allies, and are deeply loyal to their kin. The Rohirrim are also skilled craftsmen, renowned for their horsemanship and the quality of their weapons and armor. Despite their love of peace, the Rohirrim are not afraid to defend their way of life by force if necessary.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Dunlending", @"The Dunlending men are a fierce and independent people who inhabit the wild, hilly regions of Eriador. They are a hardy and practical people, accustomed to living in a harsh and unforgiving land. The Dunlendings are known for their strength and endurance, and are skilled hunters, farmers, and herders. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their rough exterior, the Dunlendings are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Dunlendings are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their reputation as a rough and barbaric people, the Dunlendings are known for their generosity and hospitality towards their friends and allies.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Dorwinrim", @"The Dorwinrim are a proud and noble people who inhabit the western region of Middle-earth known as Dorwinion. They are a wealthy and cultured people, with a long tradition of trade and commerce. The Dorwinrim are known for their love of luxury and refinement, and are renowned for their fine wines and exotic spices. They are also skilled craftsmen, and are known for their intricate metalwork and jewelery.

Despite their love of luxury and indulgence, the Dorwinrim are a fiercely independent people who value honor and integrity above all else. They are skilled warriors, and have a long tradition of military service. The Dorwinrim are deeply religious, and their faith plays a central role in their lives. They are also deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. The Dorwinrim are known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
        AddHumanEthnicity("Rhovanian", @"The Rhovanion men are a hardy and independent people who inhabit the wild, wooded regions of Middle-earth known as Rhovanion. They are skilled hunters, farmers, and herders, and are known for their practicality and resourcefulness. The Rhovanion men are also skilled craftsmen, and are renowned for their woodworking and leatherworking.

Despite their rough exterior, the Rhovanion men are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Rhovanion men are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their reputation as a rough and barbaric people, the Rhovanion men are known for their generosity and hospitality towards their friends and allies.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Skin");
        AddHumanEthnicity("Easterling", @"The Easterling Men are a diverse and complex people who inhabit the eastern regions of Middle-earth. They are a hardy and resourceful people, accustomed to living in a harsh and unforgiving land. The Easterling Men are skilled warriors, and have a long tradition of military service. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their reputation as a barbaric and warlike people, the Easterling Men are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Easterling Men are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary.", "Fallen Man", "Easterling", "Majority O Minor A", "Brown_Green_Blue_Eyes",
            "Brown_Blonde_Red_Grey_Hair", "Fair_Skin");
        AddHumanEthnicity("Variag", @"The Variags are a civilized and cultured people who inhabit the region of Khand in eastern Middle-earth. They are a wealthy and sophisticated people, with a long tradition of trade and commerce. The Variags are known for their love of luxury and refinement, and are renowned for their fine wines and exotic spices. They are also skilled craftsmen, and are known for their intricate metalwork and jewelery.

Despite their love of luxury and indulgence, the Variags are a fiercely independent people who value honor and integrity above all else. They are skilled warriors, and have a long tradition of military service. The Variags are deeply religious, and their faith plays a central role in their lives. They are also deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. The Variags are known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Fallen Man", "Southron", "B Dominant", "Brown_Eyes", "Black_Brown_Grey_Hair",
            "Swarthy_Skin", false);
        AddHumanEthnicity("Umbaric", @"The men of Umbar are a proud and formidable people who inhabit the southern coastal region of Middle-earth known as Umbar. They are a hardy and resourceful people, accustomed to living in a harsh and unforgiving land. The men of Umbar are skilled sailors and traders, and have a long tradition of maritime commerce. They are also skilled warriors, and have a long tradition of military service.

Despite their reputation as a rough and barbaric people, the men of Umbar are a proud and noble people with a rich cultural heritage. They are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. The men of Umbar are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Fallen Man", "Southron", "B Dominant", "Brown_Eyes", "Black_Brown_Grey_Hair",
            "Swarthy_Skin", false);
        AddHumanEthnicity("Haradrim", @"The Haradrim are a mysterious and reclusive people who inhabit the southern regions of Middle-earth. They are a dark-skinned people, with a culture and way of life that is shrouded in mystery. The Haradrim are skilled warriors, and have a long tradition of military service. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their reputation as a fierce and warlike people, the Haradrim are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Haradrim are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. The Haradrim are known for their strong and fierce personalities, and are often feared and misunderstood by the other peoples of Middle-earth.", "Fallen Man", "Southron", "A Dominant", "Brown_Eyes", "Black_Grey_Hair",
            "Dark_Skin", false);
        AddHumanEthnicity("Gondorian Dunedain", @"The Gondorian Dunedain are a proud and noble people who inhabit the lands of Gondor in Middle-earth. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. The Gondorian Dunedain are a tall and fair-skinned people, with bright blue eyes and golden hair. They are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art.

The Gondorian Dunedain are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, the Gondorian Dunedain are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.

The Gondorian Dunedain are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Elendil, the House of Isildur, and the House of Anarion. The House of Elendil is the most powerful and influential of the clans, and is closely associated with the royal family of Gondor. The House of Isildur is known for its military prowess, and has a long tradition of service to the kingdom. The House of Anarion is known for its artistic and scholarly achievements, and has produced many of Gondor's greatest poets, musicians, and scholars. All of these clans are deeply committed to the defense and preservation of Gondor, and are willing to lay down their lives for their kingdom.", "Edain", "Dunedain", "Overwhelmingly O", "Blue_Green_Eyes",
            "Black_Brown_Grey_Hair", "Fair_Skin");
        AddHumanEthnicity("Arnorian Dunedain", @"The Arnorian Dunedain are a proud and noble people who inhabit the lands of Arnor in Middle-earth. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. The Arnorian Dunedain are a tall and fair-skinned people, with bright blue eyes and golden hair. They are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art.

The Arnorian Dunedain are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, the Arnorian Dunedain are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.

The Arnorian Dunedain are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Elendil, the House of Isildur, and the House of Anarion. The House of Elendil is the most powerful and influential of the clans, and is closely associated with the royal family of Arnor. The House of Isildur is known for its military prowess, and has a long tradition of service to the kingdom. The House of Anarion is known for its artistic and scholarly achievements, and has produced many of Arnor's greatest poets, musicians, and scholars. All of these clans are deeply committed to the defense and preservation of Arnor, and are willing to lay down their lives for their kingdom.", "Edain", "Arnorian", "Overwhelmingly O", "Blue_Green_Eyes",
            "Black_Brown_Grey_Hair", "Fair_Skin");
        AddHumanEthnicity("Black Numenorean", @"The Black Numenoreans, initially named the King's Men, were a fallen group of Numenoreans descended from those who were loyal to the Numenorean Sceptre but in opposition to the Valar and relations with the Elves. After Sauron was brought as a captive to Numenor in fair form, these Numenoreans listened to his words, and were soon corrupted by him. They worshipped the Darkness and its lords (Melkor and later Sauron) and oppressed the Men left in Middle-earth. Ever since, they were the servants of the Enemy and bitter adversaries of the Men of Gondor.

Black Numenoreans are very similar in physical and cultural character to the Dunedain but tend towards darker skin colours due to more intermarriage with Southern peoples. Black Numenoreans usually occupy the upper echelons of whatever society they find themselves in; the model of Black Numenorean ruling class with subject peoples of other cultures or even other races such as orks are common.", "Edain", "Fallen", "Overwhelmingly O", "Blue_Green_Eyes",
            "Black_Brown_Grey_Hair", "Swarthy_Skin");

        void AddHumanCulture(string name, string nameCulture, string description, string canSelectProgText)
        {
            FutureProg prog = new()
            {
                FunctionName = $"CanSelect{name.CollapseString()}Culture",
                Category = "Chargen",
                Subcategory = "Culture",
                FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
                ReturnType = (int)ProgVariableTypes.Boolean,
                AcceptsAnyParameters = false,
                Public = false,
                StaticType = 0,
                FunctionText = canSelectProgText
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);
            _context.SaveChanges();
            AddCulture(name, nameCulture, description, prog);
        }

        AddHumanCulture("Bree Folk", "Eriadoran", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race == ToRace(""Hobbit""))
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
if (@ch.Ethnicity.EthnicGroup == ""Middle Man"" or (@ch.Ethnicity.EthnicGroup == ""Edain"" and @ch.Ethnicity.EthnicSubGroup != ""Fallen""))
  return true
end if
return false");
        AddHumanCulture("Arnorian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
if (@ch.Ethnicity.Name != ""Arnorian"" and @ch.Ethnicity.Name != ""Arnorian Dunedain"")
  return false
end if
return true");
        AddHumanCulture("Gondorian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Gondorian"")
	return true
  case (""Gondorian Dunedain"")
	return true
  case (""Haradrim"")
	return true
  case (""Rohirrim"")
	return true
end switch
return true");
        AddHumanCulture("Rohirrim", "Rohirrim", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Gondorian"")
	return true
  case (""Gondorian Dunedain"")
	return true
  case (""Dunlending"")
	return true
  case (""Rohirrim"")
	return true
end switch
return true");
        AddHumanCulture("Dunlending", "Dunlending", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Eriadoran"")
	return true
  case (""Dunlending"")
	return true
  case (""Rohirrim"")
	return true
end switch
return true");
        AddHumanCulture("Dorwinian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
	return true
  case (""Rhovanion"")
	return true
  case (""Easterling"")
	return true
end switch
return true");
        AddHumanCulture("Dale Folk", "Dalish", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
	return true
  case (""Rhovanion"")
	return true
end switch
return true");
        AddHumanCulture("Easterling", "Easterling", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
	return true
  case (""Easterling"")
	return true
end switch
return true");
        AddHumanCulture("Variag", "Variag", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Variag"")
	return true
end switch
return true");
        AddHumanCulture("Haradrim", "Haradrim", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Variag"")
	return true
  case (""Haradrim"")
	return true
  case (""Gondorian"")
	return true
  case (""Black Numenorean"")
	return true
  case (""Umbaric"")
	return true
end switch
return true");
        AddHumanCulture("Corsair", "Corsair", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Haradrim"")
	return true
  case (""Gondorian"")
	return true
  case (""Black Numenorean"")
	return true
  case (""Umbaric"")
	return true
end switch
return true");

        #endregion Men

        #region Hobbits

        FutureProg canSelectHobbitProg = new()
        {
            FunctionName = "CanSelectHobbitRace",
            Category = "Character",
            Subcategory = "Race",
            FunctionComment = "Determines if the character select the hobbit race in chargen.",
            ReturnType = (long)ProgVariableTypes.Boolean,
            StaticType = 0,
            FunctionText = @"// You might consider checking RPP
return true"
        };
        canSelectHobbitProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectHobbitProg,
            ParameterIndex = 0,
            ParameterType = 8200,
            ParameterName = "ch"
        });
        _context.FutureProgs.Add(canSelectHobbitProg);
        _context.SaveChanges();

        (Liquid? hobbitBlood, Liquid? hobbitSweat, Material _, Material _) = CreateBloodAndSweat("Hobbit");

        Race hobbitRace = new()
        {
            Name = "Hobbit",
            Description = @"The hobbits are a small, curious, and unassuming people who inhabit the land of the Shire in Middle-earth. They are a peaceful and simple folk, with a strong love of home, hearth, and good food. Hobbits are known for their kind and gentle nature, as well as their love of leisure and comfort.

Despite their small size and unassuming appearance, hobbits are tougher and more resilient than they appear. They are skilled farmers, gardeners, and craftsmen, and are able to survive and thrive in even the most difficult of circumstances. Hobbits are also fiercely independent, and are fiercely protective of their families and homes. They are willing to defend themselves and their way of life with determination and resourcefulness.

Hobbits are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the Bagginses, the Tooks, and the Brandybucks. The Bagginses are a wealthy and influential clan, known for their love of comfort and their orderly and well-planned lives. The Tooks are a more adventurous and restless clan, known for their love of travel and exploration. The Brandybucks are a friendly and hospitable clan, known for their love of music and celebration. All of these clans are deeply devoted to the Shire and its way of life, and are fiercely protective of their homes and families.",
            AllowedGenders = _humanRace.AllowedGenders,
            ParentRace = humanoid,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 2.0,
            CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
            DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NaturalArmourQuality = 3,
            NeedsToBreathe = true,
            BreathingModel = "simple",
            SweatRateInLitresPerMinute = 0.5,
            SizeStanding = 5,
            SizeProne = 5,
            SizeSitting = 5,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1 3",
            MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
            MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
            RaceUsesStamina = true,
            CanEatCorpses = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            CanEatMaterialsOptIn = false,
            TemperatureRangeFloor = 0,
            TemperatureRangeCeiling = 40,
            BodypartSizeModifier = 0,
            BodypartHealthMultiplier = 1,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
            CanClimb = true,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            ChildAge = 3,
            YouthAge = 10,
            YoungAdultAge = 16,
            AdultAge = 30,
            ElderAge = 70,
            VenerableAge = 100,
            AvailabilityProg = canSelectHobbitProg,
            BaseBody = body,
            BloodLiquid = hobbitBlood,
            BloodModel = humanoid.BloodModel,
            NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
            NaturalArmourType = humanoid.NaturalArmourType,
            RaceButcheryProfile = null,
            SweatLiquid = hobbitSweat,
			MaximumFoodSatiatedHours = CultureRaceSatiationLimits["Hobbit"].MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = CultureRaceSatiationLimits["Hobbit"].MaximumDrinkSatiatedHours
        };
        _context.Races.Add(hobbitRace);
        AddRaceAttributeAlterations(hobbitRace, CultureRaceAttributeProfiles["Hobbit"]);
        HeightWeightModel hobbitMaleHWModel = new()
        {
            Name = "Hobbit Male",
            MeanHeight = 107,
            MeanBmi = 25.6,
            StddevHeight = 7.6,
            StddevBmi = 3.7,
            Bmimultiplier = 0.1
        };
        _context.Add(hobbitMaleHWModel);
        HeightWeightModel hobbitFemaleHWModel = new()
        {
            Name = "Hobbit Female",
            MeanHeight = 101,
            MeanBmi = 25.6,
            StddevHeight = 5,
            StddevBmi = 4.9,
            Bmimultiplier = 0.1
        };
        _context.Add(hobbitFemaleHWModel);
        hobbitRace.DefaultHeightWeightModelMale = hobbitMaleHWModel;
        hobbitRace.DefaultHeightWeightModelFemale = hobbitFemaleHWModel;
        hobbitRace.DefaultHeightWeightModelNonBinary = hobbitFemaleHWModel;
        hobbitRace.DefaultHeightWeightModelNeuter = hobbitMaleHWModel;
        _races.Add("Hobbit", hobbitRace);
        _context.SaveChanges();

        FutureProg isHobbitProg = new()
        {
            FunctionName = "IsRaceHobbit",
            FunctionComment = "Determines whether someone is the Hobbit race",
            FunctionText = $"return @ch.Race == ToRace({hobbitRace.Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isHobbitProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isHobbitProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isHobbitProg);
        _context.SaveChanges();
        _raceProgs["Hobbit"] = isHobbitProg;

        FutureProg isHobbitBabyProg = CreateVariantAgeProg(isBabyProg, hobbitRace);
        FutureProg isHobbitToddlerProg = CreateVariantAgeProg(isToddlerProg, hobbitRace);
        FutureProg isHobbitBoyProg = CreateVariantAgeProg(isBoyProg, hobbitRace);
        FutureProg isHobbitGirlProg = CreateVariantAgeProg(isGirlProg, hobbitRace);
        FutureProg isHobbitChildProg = CreateVariantAgeProg(isChildProg, hobbitRace);
        FutureProg isHobbitYouthProg = CreateVariantAgeProg(isYouthProg, hobbitRace);
        FutureProg isHobbitYoungManProg = CreateVariantAgeProg(isYoungManProg, hobbitRace);
        FutureProg isHobbitYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, hobbitRace);
        FutureProg isHobbitAdultManProg = CreateVariantAgeProg(isAdultManProg, hobbitRace);
        FutureProg isHobbitAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, hobbitRace);
        FutureProg isHobbitAdultPersonProg = CreateVariantAgeProg(isAdultProg, hobbitRace);
        FutureProg isHobbitOldManProg = CreateVariantAgeProg(isOldManProg, hobbitRace);
        FutureProg isHobbitOldWomanProg = CreateVariantAgeProg(isOldWomanProg, hobbitRace);
        FutureProg isHobbitOldPersonProg = CreateVariantAgeProg(isOldPersonProg, hobbitRace);
        List<(CharacteristicValue Value, double Weight)> hobbitPersonValues = new();

        void AddHobbitPersonWord(string name, string basic, FutureProg prog, double weight)
        {
            CharacteristicValue pw = new()
            {
                Id = nextId++,
                Definition = personDef,
                Name = name,
                Value = basic,
                AdditionalValue = "",
                Default = false,
                Pluralisation = 0,
                FutureProg = prog
            };
            _context.CharacteristicValues.Add(pw);
            hobbitPersonValues.Add((pw, weight));
        }

        AddHobbitPersonWord("hobbit baby", "hobbit baby", isHobbitBabyProg, 1.0);
        AddHobbitPersonWord("hobbit infant", "hobbit baby", isHobbitBabyProg, 1.0);
        AddHobbitPersonWord("hobbit tot", "hobbit baby", isHobbitBabyProg, 1.0);
        AddHobbitPersonWord("hobbit toddler", "hobbit toddler", isHobbitToddlerProg, 5.0);
        AddHobbitPersonWord("hobbit", "hobbit", isHobbitAdultPersonProg, 5.0);
        AddHobbitPersonWord("hobbit person", "hobbit", isHobbitAdultPersonProg, 5.0);
        AddHobbitPersonWord("hobbit individual", "hobbit", isHobbitAdultPersonProg, 1.0);
        AddHobbitPersonWord("male hobbit", "hobbit", isHobbitAdultManProg, 1.0);
        AddHobbitPersonWord("hobbit man", "hobbit", isHobbitAdultManProg, 5.0);
        AddHobbitPersonWord("female hobbit", "hobbit", isHobbitAdultWomanProg, 1.0);
        AddHobbitPersonWord("hobbit woman", "hobbit", isHobbitAdultWomanProg, 5.0);
        AddHobbitPersonWord("hobbit woman", "hobbit", isHobbitAdultWomanProg, 1.0);
        AddHobbitPersonWord("hobbit lady", "hobbit", isHobbitAdultWomanProg, 3.0);
        AddHobbitPersonWord("hobbit lad", "hobbit boy", isHobbitBoyProg, 1.0);
        AddHobbitPersonWord("hobbit boy", "hobbit boy", isHobbitBoyProg, 3.0);
        AddHobbitPersonWord("hobbit child", "hobbit child", isHobbitChildProg, 3.0);
        AddHobbitPersonWord("hobbit youth", "hobbit youth", isHobbitYouthProg, 3.0);
        AddHobbitPersonWord("hobbit adolescent", "hobbit youth", isHobbitYouthProg, 1.0);
        AddHobbitPersonWord("hobbit youngster", "hobbit youth", isHobbitYouthProg, 1.0);
        AddHobbitPersonWord("hobbit juvenile", "hobbit youth", isHobbitYouthProg, 1.0);
        AddHobbitPersonWord("hobbit kid", "hobbit child", isHobbitChildProg, 1.0);
        AddHobbitPersonWord("young hobbit boy", "hobbit boy", isHobbitBoyProg, 1.0);
        AddHobbitPersonWord("young hobbit", "hobbit", isHobbitYoungManProg, 3.0);
        AddHobbitPersonWord("hobbit youngster", "hobbit", isHobbitYoungManProg, 1.0);
        AddHobbitPersonWord("hobbit lass", "hobbit girl", isHobbitYoungWomanProg, 1.0);
        AddHobbitPersonWord("hobbit girl", "hobbit girl", isHobbitGirlProg, 3.0);
        AddHobbitPersonWord("hobbit girl", "hobbit girl", isHobbitGirlProg, 3.0);
        AddHobbitPersonWord("young hobbit girl", "hobbit girl", isHobbitGirlProg, 1.0);
        AddHobbitPersonWord("young hobbit woman", "hobbit girl", isHobbitYoungWomanProg, 1.0);
        AddHobbitPersonWord("young hobbit", "hobbit girl", isHobbitYoungWomanProg, 1.0);
        AddHobbitPersonWord("hobbit maiden", "hobbit hobbit", isHobbitYoungWomanProg, 1.0);
        AddHobbitPersonWord("old hobbit", "old hobbit", isHobbitOldPersonProg, 1.0);
        AddHobbitPersonWord("old hobbit woman", "old hobbit", isHobbitOldWomanProg, 1.0);
        AddHobbitPersonWord("elderly hobbit woman", "old hobbit", isHobbitOldWomanProg, 1.0);
        AddHobbitPersonWord("old hobbit man", "old hobbit", isHobbitOldManProg, 1.0);
        AddHobbitPersonWord("elderly hobbit man", "old hobbit", isHobbitOldManProg, 1.0);
        AddHobbitPersonWord("elderly hobbit", "old hobbit", isHobbitOldPersonProg, 1.0);
        _context.SaveChanges();

        CharacteristicProfile hobbitPersons = new()
        {
            Name = "Hobbit Person Words",
            Definition = new XElement("Values",
                from item in hobbitPersonValues
                select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
            ).ToString(),
            Type = "weighted",
            Description = "All person words applicable to the hobbit race",
            TargetDefinition = personDef
        };
        _context.CharacteristicProfiles.Add(hobbitPersons);
        _profiles[hobbitPersons.Name] = hobbitPersons;
        _context.SaveChanges();

        void AddHobbitEthnicity(string name, string blurb, string facialHair, string ethnicGroup, string ethnicSubGroup,
            string bloodGroup, string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(hobbitRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? canSelectHobbitProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", facialHair);
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Hobbit Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddHobbitEthnicity("Harfoot",
            @"Harfoots were the most common type of hobbit, and in their earliest known history they lived in the lower foothills of the Misty Mountains, in the Vales of Anduin, in an area roughly bounded by the Gladden River in the south and the small forested region where later was the Eagles Eyrie near the High Pass to the north.

They were browner of skin than other hobbits, had no beards, and did not wear any footwear. They lived in holes they called smials, a habit which they long preserved, and were on friendly terms with the Dwarves, who traveled through the High Pass.

The Harfoots were the first to migrate westward into Arnor, and there the Dúnedain named them Periannath or halflings, as recorded in Arnorian records around TA 1050. They tended to settle down for long times, and founded numerous villages as far as Weathertop.

By the 1300s of the Third Age, they had reached Bree, which was the westernmost home of any hobbits for a long while.

The Harfoots were joined between TA 1150 and TA 1300 by the Fallohides and some Stoors. The Harfoots took Fallohides, a bolder breed, as their leaders. The Shire was colonized long after this, in TA 1601, mostly by Harfoots.",
            "No_Facial_Hair", "", "", "B Dominant", "Brown_Green_Blue_Eyes", "Black_Brown_Blonde_Grey_Hair",
            "Fair_Swarthy_Skin");
        AddHobbitEthnicity("Stoor",
            @"The Stoors originally dwelt in the southern vales of the river Anduin. During the hobbits' wandering days, after the Harfoots had migrated westward in TA 1050, and the Fallohides later followed them, the Stoors long remained back in the vale of Anduin, but between TA 1150 and 1300 they too migrated west into Eriador.

They were heavier and broader in build than the other hobbits, and had large hands and feet. Among the hobbits, the Stoors most resembled Men and were most friendly to them. Stoors were the only hobbits who normally grew facial hair.

A habit which set them apart from the Harfoots who lived in the mountain foothills, and the Fallohides who lived in forests far to the north, was that Stoors preferred flat lands and riversides. Only Stoors used boats, fished, and could swim. They also wore boots in muddy weather.

Stoorish characteristics and appearances remained amongst the hobbits of the Eastfarthing, Buckland (such as the Brandybucks) and the Bree-hobbits.",
            "All Facial Hair Styles", "", "", "B Dominant", "Brown_Green_Blue_Eyes", "Brown_Blonde_Grey_Hair",
            "Fair_Swarthy_Skin");
        AddHobbitEthnicity("Fallohide",
            @"The Fallohides were the least common of hobbits, and in their earliest known history they lived in the forested region where later was the Eagles Eyrie near the High Pass to the north, in the Vale of Anduin. To their south lived the far more numerous Harfoots, and far south in the Gladden Fields lived the Stoors.

The Fallohides were fair of skin and hair, and none of them ever grew a beard. They were great lovers of the trees and forests, and skilled hunters. Many of them were friends with the Elves, and because of this they were more learned than the other hobbits. They were the first to later learn Westron, and the only ones to preserve some of their old history. They learned Westron from the Men of Arnor, and it was they who first learned writing.

After the Harfoots had migrated westward in the years following TA 1050, the Fallohides followed them around TA 1150. Unlike the Harfoots they crossed far north of Rivendell, and from there later met up with the Harfoots. The Fallohides were more bold and adventurous than the Harfoots, and many of them became leaders of the Harfoot villages. It was probably under Fallohide rule that the Harfoots migrated westward beyond Weathertop and reached Bree. In TA 1601 two Fallohide brothers, Marcho and Blanco, by permission of Argeleb II, the King in Fornost crossed the Brandywine River and colonized the Shire.

After this, the Fallohides mixed more and more with the Harfoots and later the Stoors, until the three Hobbit breeds became one. The influential Took clan had distinct Fallohide traces both in appearance and character, as did the Oldbuck and later Brandybuck clan. Both Bilbo Baggins and Frodo Baggins were part Fallohide, due to their Took and Brandybuck mothers respectively. Other famous Fallohides included Bandobras Bullroarer Took, who slew an Orc leader, and Peregrin Took.",
            "No_Facial_Hair", "", "", "B Dominant", "Blue_Green_Eyes", "Blonde_Red_Grey_Hair", "Fair_Skin");

        FutureProg hobbitSkillProg = new()
        {
            FunctionName = "HobbitSkillStartingValue",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine the opening value for a skill for hobbits at character creation",
            ReturnType = (int)ProgVariableTypes.Number,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
        };
        hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = hobbitSkillProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (int)ProgVariableTypes.Toon
        });
        hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = hobbitSkillProg,
            ParameterIndex = 1,
            ParameterName = "trait",
            ParameterType = (int)ProgVariableTypes.Trait
        });
        hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = hobbitSkillProg,
            ParameterIndex = 2,
            ParameterName = "boosts",
            ParameterType = (int)ProgVariableTypes.Number
        });
        _context.FutureProgs.Add(hobbitSkillProg);
        _context.SaveChanges();

        Calendar? shireCalendar = _context.Calendars.Find(5L);

        void AddHobbitCulture(string name, string description)
        {
            FutureProg prog = new()
            {
                FunctionName = $"CanSelect{name.CollapseString()}Culture",
                Category = "Chargen",
                Subcategory = "Culture",
                FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
                ReturnType = (int)ProgVariableTypes.Boolean,
                AcceptsAnyParameters = false,
                Public = false,
                StaticType = 0,
                FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Hobbit"")"
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);
            AddCulture(name, "Hobbit", description, prog, hobbitSkillProg, shireCalendar);
        }

        AddHobbitCulture("Shire Hobbit",
            @"The Shire was the homeland of the majority of the hobbits in Middle-earth. It was located in the northwestern portion of Middle-earth, in the northern region of Eriador, within the borders of the Kingdom of Arnor.

The Shire was originally divided in four Farthings (Northfarthing, Southfarthing, Eastfarthing, and Westfarthing), but Buckland and later the Westmarch were added to it in the Fourth Age. Within the Farthings there were some smaller, unofficial divisions such as family lands; nearly all the Tooks lived in the Tookland around Tuckborough, for instance. 

In many cases a hobbit's last name indicates where their family came from: Samwise Gamgee's last name derived from Gamwich, where the family originated. Outside the Farthings, Buckland itself was named for the Oldbucks (later Brandybucks). See further Regions of the Shire.");
        AddHobbitCulture("Bree Hobbit",
            @"Bree was an ancient settlement of men in Eriador by the time of the Third Age of Middle-earth, but after the collapse of the North-kingdom, Bree continued to thrive without any central authority or government for many centuries. Bree had become the most westerly settlement of men in all of Middle-earth by the time of the War of the Ring, and became one of only three or four inhabited settlements in all of Eriador. 

At the time of the War of the Ring, Bree-land was the only part of Middle-earth where Men and hobbits dwelt together. Being located on the most important crossroads in the north, on the crossing of the Great East Road and the Greenway, people would pass through Bree.");
        AddHobbitCulture("Rhovanion Hobbit",
            @"Rhovanion, which is the region of Middle Earth East of Eriador and North of Gondor, is the original homeland of all hobbits, who dwelt along the river Anduin. The hobbits left Rhovanion and wandered West, eventually settling in the Shire. Some hobbits, however, eventually left and returned to their homeland in small numbers.

These Hobbits tend to be more worldly than their kin, and have close ties to the Elves of Lothlorien.");

        _context.SaveChanges();

        #endregion Hobbits

        #region Dwarves

        FutureProg canSelectDwarfProg = new()
        {
            FunctionName = "CanSelectDwarfRace",
            Category = "Character",
            Subcategory = "Race",
            FunctionComment = "Determines if the character select the dwarf race in chargen.",
            ReturnType = (long)ProgVariableTypes.Boolean,
            StaticType = 0,
            FunctionText = @"// You might consider checking RPP
return true"
        };
        canSelectDwarfProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectDwarfProg,
            ParameterIndex = 0,
            ParameterType = 8200,
            ParameterName = "ch"
        });
        _context.FutureProgs.Add(canSelectDwarfProg);
        _context.SaveChanges();

        (Liquid? dwarfBlood, Liquid? dwarfSweat, Material _, Material _) = CreateBloodAndSweat("Dwarf");

        Race dwarfRace = new()
        {
            Name = "Dwarf",
            Description = @"The Dwarves are a proud and ancient race of skilled craftsmen who inhabit the mountains of Middle-earth. They are a short and stocky people, with thick beards and a love of gold and precious gems. Dwarves are known for their skill in metalworking, stonecutting, and engineering, and are renowned for their sturdy and well-made craftsmanship.

Despite their love of gold and material wealth, Dwarves are a fiercely independent and honorable people. They have a strong sense of duty and responsibility, and are deeply loyal to their families and clans. Dwarves are also fiercely protective of their homes and possessions, and are willing to defend them with great determination and ferocity.

Dwarves are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Durin, the House of Thrain, and the House of Bofur. The House of Durin is the most powerful and influential of the clans, and is closely associated with the royal family of the Dwarves. The House of Thrain is known for its military prowess, and has a long tradition of service to the kingdom. The House of Bofur is known for its artistic and scholarly achievements, and has produced many of the Dwarves' greatest poets, musicians, and scholars. All of these clans are deeply devoted to the defense and preservation of the Dwarven kingdom, and are willing to lay down their lives for their people.",
            AllowedGenders = _humanRace.AllowedGenders,
            ParentRace = humanoid,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 1.75,
            CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
            DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NaturalArmourQuality = 3,
            NeedsToBreathe = true,
            BreathingModel = "simple",
            SweatRateInLitresPerMinute = 0.5,
            SizeStanding = 5,
            SizeProne = 5,
            SizeSitting = 5,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1 3",
            MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
            MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
            RaceUsesStamina = true,
            CanEatCorpses = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            CanEatMaterialsOptIn = false,
            TemperatureRangeFloor = -20,
            TemperatureRangeCeiling = 40,
            BodypartSizeModifier = 0,
            BodypartHealthMultiplier = 1.25,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
            CanClimb = true,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            ChildAge = 5,
            YouthAge = 20,
            YoungAdultAge = 45,
            AdultAge = 75,
            ElderAge = 200,
            VenerableAge = 300,
            AvailabilityProg = canSelectDwarfProg,
            BaseBody = body,
            BloodLiquid = dwarfBlood,
            BloodModel = humanoid.BloodModel,
            NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
            NaturalArmourType = humanoid.NaturalArmourType,
            RaceButcheryProfile = null,
            SweatLiquid = dwarfSweat,
			MaximumFoodSatiatedHours = CultureRaceSatiationLimits["Dwarf"].MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = CultureRaceSatiationLimits["Dwarf"].MaximumDrinkSatiatedHours
        };
        _context.Races.Add(dwarfRace);
        AddRaceAttributeAlterations(dwarfRace, CultureRaceAttributeProfiles["Dwarf"]);
        HeightWeightModel dwarfMaleHWModel = new()
        {
            Name = "Dwarf Male",
            MeanHeight = 142,
            MeanBmi = 25.6,
            StddevHeight = 7.6,
            StddevBmi = 3.7,
            Bmimultiplier = 0.1
        };
        _context.Add(dwarfMaleHWModel);
        HeightWeightModel dwarfFemaleHWModel = new()
        {
            Name = "Dwarf Female",
            MeanHeight = 137,
            MeanBmi = 25.6,
            StddevHeight = 5,
            StddevBmi = 4.9,
            Bmimultiplier = 0.1
        };
        _context.Add(dwarfFemaleHWModel);
        dwarfRace.DefaultHeightWeightModelMale = dwarfMaleHWModel;
        dwarfRace.DefaultHeightWeightModelFemale = dwarfFemaleHWModel;
        dwarfRace.DefaultHeightWeightModelNonBinary = dwarfFemaleHWModel;
        dwarfRace.DefaultHeightWeightModelNeuter = dwarfMaleHWModel;
        _races.Add("Dwarf", dwarfRace);
        _context.SaveChanges();

        FutureProg isDwarfProg = new()
        {
            FunctionName = "IsDwarfRace",
            FunctionComment = "Determines whether someone is the Dwarf race",
            FunctionText = $"return @ch.Race == ToRace({dwarfRace.Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isDwarfProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isDwarfProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isDwarfProg);
        _context.SaveChanges();
        _raceProgs["Dwarf"] = isDwarfProg;

        FutureProg isDwarfBabyProg = CreateVariantAgeProg(isBabyProg, dwarfRace);
        FutureProg isDwarfToddlerProg = CreateVariantAgeProg(isToddlerProg, dwarfRace);
        FutureProg isDwarfBoyProg = CreateVariantAgeProg(isBoyProg, dwarfRace);
        FutureProg isDwarfGirlProg = CreateVariantAgeProg(isGirlProg, dwarfRace);
        FutureProg isDwarfChildProg = CreateVariantAgeProg(isChildProg, dwarfRace);
        FutureProg isDwarfYouthProg = CreateVariantAgeProg(isYouthProg, dwarfRace);
        FutureProg isDwarfYoungManProg = CreateVariantAgeProg(isYoungManProg, dwarfRace);
        FutureProg isDwarfYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, dwarfRace);
        FutureProg isDwarfAdultManProg = CreateVariantAgeProg(isAdultManProg, dwarfRace);
        FutureProg isDwarfAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, dwarfRace);
        FutureProg isDwarfAdultPersonProg = CreateVariantAgeProg(isAdultProg, dwarfRace);
        FutureProg isDwarfOldManProg = CreateVariantAgeProg(isOldManProg, dwarfRace);
        FutureProg isDwarfOldWomanProg = CreateVariantAgeProg(isOldWomanProg, dwarfRace);
        FutureProg isDwarfOldPersonProg = CreateVariantAgeProg(isOldPersonProg, dwarfRace);
        List<(CharacteristicValue Value, double Weight)> dwarfPersonValues = new();

        void AddDwarfPersonWord(string name, string basic, FutureProg prog, double weight)
        {
            CharacteristicValue pw = new()
            {
                Id = nextId++,
                Definition = personDef,
                Name = name,
                Value = basic,
                AdditionalValue = "",
                Default = false,
                Pluralisation = 0,
                FutureProg = prog
            };
            _context.CharacteristicValues.Add(pw);
            dwarfPersonValues.Add((pw, weight));
        }

        AddDwarfPersonWord("dwarven baby", "dwarven baby", isDwarfBabyProg, 1.0);
        AddDwarfPersonWord("dwarven infant", "dwarven baby", isDwarfBabyProg, 1.0);
        AddDwarfPersonWord("dwarven tot", "dwarven baby", isDwarfBabyProg, 1.0);
        AddDwarfPersonWord("dwarven toddler", "dwarven toddler", isDwarfToddlerProg, 5.0);
        AddDwarfPersonWord("dwarf", "dwarf", isDwarfAdultPersonProg, 5.0);
        AddDwarfPersonWord("dwarven person", "dwarf", isDwarfAdultPersonProg, 5.0);
        AddDwarfPersonWord("dwarf person", "dwarf", isDwarfAdultPersonProg, 1.0);
        AddDwarfPersonWord("dwarven individual", "dwarf", isDwarfAdultPersonProg, 1.0);
        AddDwarfPersonWord("male dwarf", "dwarf", isDwarfAdultManProg, 1.0);
        AddDwarfPersonWord("dwarven man", "dwarf", isDwarfAdultManProg, 5.0);
        AddDwarfPersonWord("dwarf man", "dwarf", isDwarfAdultManProg, 1.0);
        AddDwarfPersonWord("female dwarf", "dwarf", isDwarfAdultWomanProg, 1.0);
        AddDwarfPersonWord("dwarven woman", "dwarf", isDwarfAdultWomanProg, 5.0);
        AddDwarfPersonWord("dwarf woman", "dwarf", isDwarfAdultWomanProg, 1.0);
        AddDwarfPersonWord("dwarven lady", "dwarf", isDwarfAdultWomanProg, 3.0);
        AddDwarfPersonWord("dwarf lady", "dwarf", isDwarfAdultWomanProg, 1.0);
        AddDwarfPersonWord("dwarven lad", "dwarven boy", isDwarfBoyProg, 1.0);
        AddDwarfPersonWord("dwarf boy", "dwarven boy", isDwarfBoyProg, 3.0);
        AddDwarfPersonWord("dwarven boy", "dwarven boy", isDwarfBoyProg, 3.0);
        AddDwarfPersonWord("dwarven child", "dwarven child", isDwarfChildProg, 3.0);
        AddDwarfPersonWord("dwarven youth", "dwarven youth", isDwarfYouthProg, 3.0);
        AddDwarfPersonWord("dwarven adolescent", "dwarven youth", isDwarfYouthProg, 1.0);
        AddDwarfPersonWord("dwarven youngster", "dwarven youth", isDwarfYouthProg, 1.0);
        AddDwarfPersonWord("dwarven juvenile", "dwarven youth", isDwarfYouthProg, 1.0);
        AddDwarfPersonWord("dwarven kid", "dwarven child", isDwarfChildProg, 1.0);
        AddDwarfPersonWord("young dwarven boy", "dwarven boy", isDwarfBoyProg, 1.0);
        AddDwarfPersonWord("young dwarf", "dwarf", isDwarfYoungManProg, 3.0);
        AddDwarfPersonWord("dwarven youngster", "dwarf", isDwarfYoungManProg, 1.0);
        AddDwarfPersonWord("dwarven lass", "dwarven girl", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("dwarf lass", "dwarven girl", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("dwarven girl", "dwarven girl", isDwarfGirlProg, 3.0);
        AddDwarfPersonWord("dwarf girl", "dwarven girl", isDwarfGirlProg, 3.0);
        AddDwarfPersonWord("young dwarven girl", "dwarven girl", isDwarfGirlProg, 1.0);
        AddDwarfPersonWord("young dwarf girl", "dwarven girl", isDwarfGirlProg, 1.0);
        AddDwarfPersonWord("young dwarven woman", "dwarven girl", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("young dwarf", "dwarven girl", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("dwarf maiden", "dwarven dwarf", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("dwarven maiden", "dwarven dwarf", isDwarfYoungWomanProg, 1.0);
        AddDwarfPersonWord("old dwarf", "old dwarf", isDwarfOldPersonProg, 1.0);
        AddDwarfPersonWord("old dwarven woman", "old dwarf", isDwarfOldWomanProg, 1.0);
        AddDwarfPersonWord("elderly dwarven woman", "old dwarf", isDwarfOldWomanProg, 1.0);
        AddDwarfPersonWord("old dwarven man", "old dwarf", isDwarfOldManProg, 1.0);
        AddDwarfPersonWord("elderly dwarven man", "old dwarf", isDwarfOldManProg, 1.0);
        AddDwarfPersonWord("elderly dwarf", "old dwarf", isDwarfOldPersonProg, 1.0);
        _context.SaveChanges();

        CharacteristicProfile dwarfPersons = new()
        {
            Name = "Dwarf Person Words",
            Definition = new XElement("Values",
                from item in dwarfPersonValues
                select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
            ).ToString(),
            Type = "weighted",
            Description = "All person words applicable to the Dwarf race",
            TargetDefinition = personDef
        };
        _context.CharacteristicProfiles.Add(dwarfPersons);
        _profiles[dwarfPersons.Name] = dwarfPersons;
        _context.SaveChanges();

        void AddDwarfEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
            string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(dwarfRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? canSelectDwarfProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Dwarf Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddDwarfEthnicity("Longbeard",
            @"Durin's Folk were the Longbeards (Sigin-tarâg in Khuzdul), one of the seven kindreds of Dwarves whose leaders were from the House of Durin. Their first king was named Durin, who was one of the seven Fathers of the Dwarves.",
            "Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
        AddDwarfEthnicity("Firebeard",
            @"The Firebeards were one of the seven houses of the Dwarves. They were originally paired with the Broadbeams. The ancestor of the Firebeards was among the oldest (together with the ancestors of the Broadbeams and Longbeards) of the Seven Ancestors of the Dwarves.

The Firebeards (with the Broadbeams) awoke in the Blue Mountains and lived there throughout the history of their people. These two houses built the great Dwarven cities of Nogrod and Belegost in the Blue Mountains, and dwelt in them before their ruining in the War of Wrath.",
            "Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Red_Grey_Hair", "Fair_Swarthy_Skin");
        AddDwarfEthnicity("Broadbeam",
            @"The Broadbeams were one of the seven houses of the Dwarves. They were originally paired with the Firebeards. The ancestor of the Broadbeams was among the oldest of the Seven Fathers of the Dwarves (together with the ancestors of the Firebeards and Longbeards).

The Broadbeams (with the Firebeards) awoke in the Blue Mountains and lived there throughout the history of their people. These two houses built the great Dwarven cities of Nogrod and Belegost in the Blue Mountains, and dwelt in them before their ruining in the War of Wrath. It is not clear whether they shared the two cities or whether each house dwelt in its own. In an earlier version of the legendarium the two cities are clearly inhabited by separate houses; however, Belegost is said to be the home of the Longbeards.",
            "Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
        AddDwarfEthnicity("Ironfist",
            @"The Ironfists were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Stiffbeards.

The locations of the four Dwarven clans, including the Ironfists, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Ironfists and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
            "Eastern", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
        AddDwarfEthnicity("Stiffbeard",
            @"The Stiffbeards were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Ironfists.

The locations of the four Dwarven clans, including the Stiffbeards, who lived in the East, are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Stiffbeards and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
            "Eastern", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Dark_Skin");
        AddDwarfEthnicity("Blacklock",
            @"The Blacklocks were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Stonefoots.

The locations of the four Dwarven clans, including the Blacklocks, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Blacklocks and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
            "Eastern", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin");
        AddDwarfEthnicity("Stonefeet",
            @"The Stonefoots were one of seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Blacklocks.

The locations of the four Dwarven clans, including the Stonefoots, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Stonefoots and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
            "Eastern", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Swarthy_Skin");

        FutureProg dwarfSkillProg = new()
        {
            FunctionName = "DwarfSkillStartingValue",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine the opening value for a skill for dwarves at character creation",
            ReturnType = (int)ProgVariableTypes.Number,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
        };
        dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = dwarfSkillProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (int)ProgVariableTypes.Toon
        });
        dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = dwarfSkillProg,
            ParameterIndex = 1,
            ParameterName = "trait",
            ParameterType = (int)ProgVariableTypes.Trait
        });
        dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = dwarfSkillProg,
            ParameterIndex = 2,
            ParameterName = "boosts",
            ParameterType = (int)ProgVariableTypes.Number
        });
        _context.FutureProgs.Add(dwarfSkillProg);
        _context.SaveChanges();

        Calendar? sindarinCalendar = _context.Calendars.Find(2L);

        void AddDwarfCulture(string name, string description)
        {
            FutureProg prog = new()
            {
                FunctionName = $"CanSelect{name.CollapseString()}Culture",
                Category = "Chargen",
                Subcategory = "Culture",
                FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
                ReturnType = (int)ProgVariableTypes.Boolean,
                AcceptsAnyParameters = false,
                Public = false,
                StaticType = 0,
                FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Dwarf"")"
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);
            AddCulture(name, "Dwarven", description, prog, dwarfSkillProg, sindarinCalendar);
        }

        AddDwarfCulture("Khazad-dum",
            @"Your character grew up in Khazad-dum. Khazad-dum, latterly known as Moria (The Black Chasm, The Black Pit), was the grandest and most famous of the mansions of the Dwarves. There, for many thousands of years, a thriving Dwarvish community created the greatest city ever known.

It lay in the central parts of the Misty Mountains, tunnelled and carved through the living rock of the mountains themselves. By the Second Age a traveller could pass through it from the west of the range to the east.");
        AddDwarfCulture("Nogrod",
            @"Your character grew up in the remnant tribes of Nogrod. Nogrod was one of two Dwarven cities in the Ered Luin, the other being Belegost, that prospered during the First Age. It was home to the Dwarves of Nogrod. 

At the end of the First Age, Nogrod was ruined in the War of Wrath, and around the fortieth year of the Second Age the Dwarves of the Blue Mountains began to migrate to Khazad-dum, abandoning Nogrod and Belegost. However, there always remained some Dwarves on the eastern side of the Blue Mountains in days afterward.");
        AddDwarfCulture("Belegost",
            @"Your character grew up in the remnant tribes of Belegost. Belegost was one of two great Dwarven cities in the Ered Luin, the other being Nogrod. It was home to the Dwarves of Belegost. Belegost lay in the north central part of the Ered Luin, north of Nogrod and northeast of Mount Dolmed, guarding one of the only passes through the mountain range.

At the end of the First Age, Belegost was ruined in the War of Wrath, and around the fortieth year of the Second Age the Dwarves of the Blue Mountains began to migrate to Khazad-dum, abandoning Nogrod and Belegost. However, there always remained some Dwarves on the eastern side of the Blue Mountains in days afterward.");
        AddDwarfCulture("Grey Mountains",
            @"The Dwarves of the Grey Mountains were the Dwarves of Durin's Folk who lived in the Grey Mountains in northern Middle-earth.

For many years, Durin's folk prospered in the Ered Mithrin but, the dragons of the Forodwaith eventually multiplied and became strong, and in T.A. 2570 the Dragons made war on the dwarves, sacking and plundering their halls. The Dwarves held out for around twenty years, but finally in 2589 the Dragons attacked the halls of King Dáin I. King Dáin, and his second son Frór, were killed by a Cold-drake outside his door to his halls.

Following the death of their king, most of Durin's Folk abandoned the Grey Mountains. In 2590, King Thrór and his uncle Borin returned to the Erebor with the Arkenstone to re-establish the Kingdom under the Mountain. However, Thrór's younger brother Grór led others to the Iron Hills.");
        AddDwarfCulture("Erebor",
            @"The Kingdom under the Mountain was the name given to the Dwarf realm of Erebor. It was founded in T.A. 1999 when Thráin I came to the Lonely Mountain and discovered the Arkenstone. The kingdom lasted until 2770, when Smaug the Dragon invaded and either killed the Dwarves or forced them to leave. When Smaug was slain in 2941, Dáin Ironfoot became King Dáin II and the kingdom was restored.

Under Dáin's rule the Dwarves of the Lonely Mountain became very rich and prosperous. They rebuilt the town of Dale, their trade greatly increased with their kinsman in the Iron Hills once again and with Men; and the Lonely Mountain was restored to its original greatness.");
        AddDwarfCulture("Iron Hills",
            @"The Dwarves of the Iron Hills were Dwarves belonging to the house of the Longbeards, otherwise known as Durin's Folk, who lived in the Iron Hills. They became well-known for making a metal mesh that could be used for making flexible items like leg-coverings.

In the Third Age, many Longboard Dwarves lived in the Grey Mountains, but they were greatly troubled by Dragons in that region. After King Dáin I was slain by one of these dragons, his surviving sons led an exodus into the east. Dáin's elder son Thrór recreated the Kingdom under the Mountain at Erebor, while his younger brother Grór led a part of the people further into the east to join their kindred living in the Iron Hills.

Grór settled in the Iron Hills in the year T.A. 2590 and became Lord of the Iron Hills. During his reign, the realm became the strongest in the North, being the only realm standing between Sauron and his plans to destroy Rivendell and taking back the lands of Angmar. Also, following the Sack of Erebor many of Durin's folk fleeing from Smaug and those wandering in exile, except for Thrór and his small company of family and followers, came to the Iron Hills, bolstering their numbers.");
        AddDwarfCulture("Aglarond",
            @"The Glittering Caves of Aglarond were a cave system in the White Mountains behind Helm's Deep. Gimli son of Glóin led a large group of Dwarves of Erebor there after the War of the Ring and became the Lord of the Glittering Caves. 

His Dwarves performed great services for the Rohirrim and the Men of Gondor, of which the most famous was the making of new gates for Minas Tirith, forged out of mithril and steel. 

The dwarves of Aglarond restored the Hornburg following the War of the Ring, and it became a fortress they shared with the Rohirrim. 

The Dwarves of the Glittering Caves carefully tended the stone walls and opened new ways and chambers and hung lamps that filled the caverns with light.");
        AddDwarfCulture("Rhûn",
            @"Rhûn was inhabited by four of the Dwarf clans: the Ironfists, Stiffbeards, Blacklocks and Stonefoots. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

In the Third Age, Dwarves of those kingdoms journeyed out of Rhûn to join all Middle-earth's other Dwarf clans in the War of the Dwarves and Orcs, which was fought in and under the Misty Mountains. After this war, the survivors returned home. 

Late in the Third Age, when war and terror grew in Rhûn itself, considerable numbers of its Dwarves left their ancient homelands. They sought refuge in Middle-earth's western lands.");
        AddDwarfCulture("Dunland",
            @"The Exiled Realm in Dunland was established by Dwarves fleeing from Erebor after it was sacked by Smaug. This is where Thrór departed when he and his companion Nár journeyed to Moria in TA 2790. 

After the Battle of Azanulbizar, provoked by the Orcs' brutal slaying of Thrór, Thráin II and Thorin led the remnants of their followers back to Dunland but soon left (to eventually settle in the Ered Luin).");

        _context.SaveChanges();

        #endregion Dwarves

        #region Orcs

        FutureProg canSelectOrcProg = new()
        {
            FunctionName = "CanSelectOrcRace",
            Category = "Character",
            Subcategory = "Race",
            FunctionComment = "Determines if the character select the orc race in chargen.",
            ReturnType = (long)ProgVariableTypes.Boolean,
            StaticType = 0,
            FunctionText = @"// You might consider checking RPP
return true"
        };
        canSelectOrcProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectOrcProg,
            ParameterIndex = 0,
            ParameterType = 8200,
            ParameterName = "ch"
        });
        _context.FutureProgs.Add(canSelectOrcProg);
        _context.SaveChanges();

        (Liquid? orcBlood, Liquid? orcSweat, Material _, Material _) = CreateBloodAndSweat("Orc");

        Race orcRace = new()
        {
            Name = "Orc",
            Description = @"The Orcs are a cruel and brutal race of creatures who inhabit the dark and shadowy regions of Middle-earth. They are a fecund and fearsome people, with a love of violence and destruction. Orcs are known for their strength and ferocity in battle, and are feared and hated by the other peoples of Middle-earth.

Despite their reputation as mindless beasts, Orcs are a highly organized and hierarchical society. They have a strict system of laws and customs, and are highly disciplined and obedient to their superiors. Orcs value strength and power above all else, and will do whatever it takes to gain and maintain it.

Orcish cultural practices are centered around warfare and domination. Orcs are constantly at war with one another and with the other peoples of Middle-earth. They believe that the strongest and most ruthless will rise to the top, and will do whatever it takes to prove themselves worthy. Orcish society is highly patriarchal, and women are often treated as little more than property. Orcs also have a strong tradition of slavery, and will enslave any creatures that they conquer in battle. Orcs are also known for their love of torture and cruelty, and will often inflict great suffering on their enemies for their own enjoyment.",
            AllowedGenders = _humanRace.AllowedGenders,
            ParentRace = humanoid,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 1.75,
            CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
            DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NaturalArmourQuality = 3,
            NeedsToBreathe = true,
            BreathingModel = "simple",
            SweatRateInLitresPerMinute = 0.5,
            SizeStanding = 5,
            SizeProne = 5,
            SizeSitting = 5,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1 3",
            MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
            MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
            RaceUsesStamina = true,
            CanEatCorpses = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            CanEatMaterialsOptIn = false,
            TemperatureRangeFloor = -10,
            TemperatureRangeCeiling = 50,
            BodypartSizeModifier = 0,
            BodypartHealthMultiplier = 1,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
            CanClimb = true,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            ChildAge = 3,
            YouthAge = 9,
            YoungAdultAge = 14,
            AdultAge = 22,
            ElderAge = 40,
            VenerableAge = 60,
            AvailabilityProg = canSelectOrcProg,
            BaseBody = body,
            BloodLiquid = orcBlood,
            BloodModel = humanoid.BloodModel,
            NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
            NaturalArmourType = humanoid.NaturalArmourType,
            RaceButcheryProfile = null,
            SweatLiquid = orcSweat,
			MaximumFoodSatiatedHours = CultureRaceSatiationLimits["Orc"].MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = CultureRaceSatiationLimits["Orc"].MaximumDrinkSatiatedHours
        };
        _context.Races.Add(orcRace);
        AddRaceAttributeAlterations(orcRace, CultureRaceAttributeProfiles["Orc"]);
        HeightWeightModel orcMaleHWModel = new()
        {
            Name = "Orc Male",
            MeanHeight = 147,
            MeanBmi = 23,
            StddevHeight = 7.6,
            StddevBmi = 3,
            Bmimultiplier = 0.1
        };
        _context.Add(orcMaleHWModel);
        HeightWeightModel orcFemaleHWModel = new()
        {
            Name = "Orc Female",
            MeanHeight = 142,
            MeanBmi = 23.5,
            StddevHeight = 5,
            StddevBmi = 3,
            Bmimultiplier = 0.1
        };
        _context.Add(orcFemaleHWModel);
        orcRace.DefaultHeightWeightModelMale = orcMaleHWModel;
        orcRace.DefaultHeightWeightModelFemale = orcFemaleHWModel;
        orcRace.DefaultHeightWeightModelNonBinary = orcFemaleHWModel;
        orcRace.DefaultHeightWeightModelNeuter = orcMaleHWModel;
        _races.Add("Orc", orcRace);
        _context.SaveChanges();

        FutureProg isOrcProg = new()
        {
            FunctionName = "IsOrcRace",
            FunctionComment = "Determines whether someone is the Orc race",
            FunctionText = $"return @ch.Race == ToRace({orcRace.Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isOrcProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isOrcProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isOrcProg);
        _context.SaveChanges();
        _raceProgs["Orc"] = isOrcProg;

        FutureProg isOrcBabyProg = CreateVariantAgeProg(isBabyProg, orcRace);
        FutureProg isOrcToddlerProg = CreateVariantAgeProg(isToddlerProg, orcRace);
        FutureProg isOrcBoyProg = CreateVariantAgeProg(isBoyProg, orcRace);
        FutureProg isOrcGirlProg = CreateVariantAgeProg(isGirlProg, orcRace);
        FutureProg isOrcChildProg = CreateVariantAgeProg(isChildProg, orcRace);
        FutureProg isOrcYouthProg = CreateVariantAgeProg(isYouthProg, orcRace);
        FutureProg isOrcYoungManProg = CreateVariantAgeProg(isYoungManProg, orcRace);
        FutureProg isOrcYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, orcRace);
        FutureProg isOrcAdultManProg = CreateVariantAgeProg(isAdultManProg, orcRace);
        FutureProg isOrcAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, orcRace);
        FutureProg isOrcAdultPersonProg = CreateVariantAgeProg(isAdultProg, orcRace);
        FutureProg isOrcOldManProg = CreateVariantAgeProg(isOldManProg, orcRace);
        FutureProg isOrcOldWomanProg = CreateVariantAgeProg(isOldWomanProg, orcRace);
        FutureProg isOrcOldPersonProg = CreateVariantAgeProg(isOldPersonProg, orcRace);
        List<(CharacteristicValue Value, double Weight)> orcPersonValues = new();

        void AddOrcPersonWord(string name, string basic, FutureProg prog, double weight)
        {
            CharacteristicValue pw = new()
            {
                Id = nextId++,
                Definition = personDef,
                Name = name,
                Value = basic,
                AdditionalValue = "",
                Default = false,
                Pluralisation = 0,
                FutureProg = prog
            };
            _context.CharacteristicValues.Add(pw);
            orcPersonValues.Add((pw, weight));
        }

        AddOrcPersonWord("orc baby", "orc baby", isOrcBabyProg, 1.0);
        AddOrcPersonWord("orc infant", "orc baby", isOrcBabyProg, 1.0);
        AddOrcPersonWord("orc tot", "orc baby", isOrcBabyProg, 1.0);
        AddOrcPersonWord("orc toddler", "orc toddler", isOrcToddlerProg, 5.0);
        AddOrcPersonWord("orc", "orc", isOrcAdultPersonProg, 5.0);
        AddOrcPersonWord("orkish person", "orc", isOrcAdultPersonProg, 5.0);
        AddOrcPersonWord("orc person", "orc", isOrcAdultPersonProg, 1.0);
        AddOrcPersonWord("orkish individual", "orc", isOrcAdultPersonProg, 1.0);
        AddOrcPersonWord("male orc", "orc", isOrcAdultManProg, 1.0);
        AddOrcPersonWord("orkish man", "orc", isOrcAdultManProg, 5.0);
        AddOrcPersonWord("orc man", "orc", isOrcAdultManProg, 1.0);
        AddOrcPersonWord("female orc", "orc", isOrcAdultWomanProg, 1.0);
        AddOrcPersonWord("orkish woman", "orc", isOrcAdultWomanProg, 5.0);
        AddOrcPersonWord("orc woman", "orc", isOrcAdultWomanProg, 1.0);
        AddOrcPersonWord("orkish lady", "orc", isOrcAdultWomanProg, 3.0);
        AddOrcPersonWord("orc lady", "orc", isOrcAdultWomanProg, 1.0);
        AddOrcPersonWord("orc lad", "orc boy", isOrcBoyProg, 1.0);
        AddOrcPersonWord("orc boy", "orc boy", isOrcBoyProg, 3.0);
        AddOrcPersonWord("orc child", "orc child", isOrcChildProg, 3.0);
        AddOrcPersonWord("orkish youth", "orc youth", isOrcYouthProg, 3.0);
        AddOrcPersonWord("orkish adolescent", "orc youth", isOrcYouthProg, 1.0);
        AddOrcPersonWord("orkish youngster", "orc youth", isOrcYouthProg, 1.0);
        AddOrcPersonWord("orkish juvenile", "orc youth", isOrcYouthProg, 1.0);
        AddOrcPersonWord("orkish kid", "orc child", isOrcChildProg, 1.0);
        AddOrcPersonWord("young orkish boy", "orc boy", isOrcBoyProg, 1.0);
        AddOrcPersonWord("young orc", "orc", isOrcYoungManProg, 3.0);
        AddOrcPersonWord("orkish youngster", "orc", isOrcYoungManProg, 1.0);
        AddOrcPersonWord("orkish lass", "orc girl", isOrcYoungWomanProg, 1.0);
        AddOrcPersonWord("orc lass", "orc girl", isOrcYoungWomanProg, 1.0);
        AddOrcPersonWord("orkish girl", "orc girl", isOrcGirlProg, 3.0);
        AddOrcPersonWord("orc girl", "orc girl", isOrcGirlProg, 3.0);
        AddOrcPersonWord("young orkish girl", "orc girl", isOrcGirlProg, 1.0);
        AddOrcPersonWord("young orc girl", "orc girl", isOrcGirlProg, 1.0);
        AddOrcPersonWord("young orkish woman", "orc girl", isOrcYoungWomanProg, 1.0);
        AddOrcPersonWord("young orc", "orc girl", isOrcYoungWomanProg, 1.0);
        AddOrcPersonWord("old orc", "old orc", isOrcOldPersonProg, 1.0);
        AddOrcPersonWord("old orkish woman", "old orc", isOrcOldWomanProg, 1.0);
        AddOrcPersonWord("elderly orkish woman", "old orc", isOrcOldWomanProg, 1.0);
        AddOrcPersonWord("old orkish man", "old orc", isOrcOldManProg, 1.0);
        AddOrcPersonWord("elderly orkish man", "old orc", isOrcOldManProg, 1.0);
        AddOrcPersonWord("elderly orc", "old orc", isOrcOldPersonProg, 1.0);
        _context.SaveChanges();

        CharacteristicProfile orcPersons = new()
        {
            Name = "Orc Person Words",
            Definition = new XElement("Values",
                from item in orcPersonValues
                select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
            ).ToString(),
            Type = "weighted",
            Description = "All person words applicable to the Orc race",
            TargetDefinition = personDef
        };
        _context.CharacteristicProfiles.Add(orcPersons);
        _profiles[orcPersons.Name] = orcPersons;
        _context.SaveChanges();

        void AddOrcEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
            string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(orcRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? canSelectOrcProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Orc Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddOrcEthnicity("Uruk",
            @"Orcs were the primary foot soldiers of the Dark Lords' armies and sometimes the weakest (but most numerous) of their servants. They were created by the first Dark Lord, Morgoth, before the First Age and served him and later his successor in their quest to dominate Middle-earth. Before Oromë first found the Elves at Cuiviénen, Melkor kidnapped some of them and cruelly tortured them, twisting them into the first Orcs.

Orcs were cruel, sadistic, black-hearted, vicious, and hateful of everybody and everything, particularly the orderly and prosperous. Physically, they were short in stature and humanoid in shape. They were generally squat, broad, flat-nosed, sallow-skinned, bow-legged, with wide mouths and slant eyes, long arms, dark skin, and fangs. They were roughly humanoid in shape with pointed ears, sharpened teeth and grimy skin. Their appearance was considered revolting by most of the other races.",
            "", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin");
        AddOrcEthnicity("Uruk-Hai",
            @"Uruk-hai were brutal warriors of Middle-earth, and the strongest Orcs, who dwelt in Isengard.

The Uruks first appeared out of Mordor in TA 2475, when they assaulted and managed to conquer Ithilien and destroyed the city of Osgiliath. The Uruks in the service of Barad-dûr used the symbol of the red Eye of Sauron, which was also painted on their shields.

Uruk-hai were later bred by the Wizard Saruman the White late in the Third Age, by his dark arts in the pits of Isengard. In the War of the Ring, the Uruk-hai made up a large part of Saruman's army, together with the Dunlendings, man-enemies of Rohan.",
            "", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin", false);

        FutureProg orcSkillProg = new()
        {
            FunctionName = "OrcSkillStartingValue",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine the opening value for a skill for orcs at character creation",
            ReturnType = (int)ProgVariableTypes.Number,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
        };
        orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = orcSkillProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (int)ProgVariableTypes.Toon
        });
        orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = orcSkillProg,
            ParameterIndex = 1,
            ParameterName = "trait",
            ParameterType = (int)ProgVariableTypes.Trait
        });
        orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = orcSkillProg,
            ParameterIndex = 2,
            ParameterName = "boosts",
            ParameterType = (int)ProgVariableTypes.Number
        });
        _context.FutureProgs.Add(orcSkillProg);
        _context.SaveChanges();

        void AddOrcCulture(string name, string description)
        {
            FutureProg prog = new()
            {
                FunctionName = $"CanSelect{name.CollapseString()}Culture",
                Category = "Chargen",
                Subcategory = "Culture",
                FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
                ReturnType = (int)ProgVariableTypes.Boolean,
                AcceptsAnyParameters = false,
                Public = false,
                StaticType = 0,
                FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Orc"") or @ch.Race == ToRace(""Troll"")"
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
            _context.FutureProgs.Add(prog);
            AddCulture(name, "Orc", description, prog, orcSkillProg, sindarinCalendar);
        }

        AddOrcCulture("Misty Mountains Orc",
            @"Mountain Orcs, also known as Orcs of the Mountain, were a sub-race of Orcs that lived in the Misty Mountains.

In the year TA 2790, two tribes of Mountain Orcs; the Gundabad and Guldur Orcs managed to reclaim the ancient dwarf kingdom of Moria.");
        AddOrcCulture("Grey Mountains Orc",
            @"The Grey Mountains, or the Ered Mithrin, was a large mountain range in Middle-earth located to the north of Rhovanion. The Grey Mountains were originally part of the Iron Mountains, the ancient mountain range in the north of Middle-earth, making them one of the oldest mountain ranges in Arda. After the War of Wrath, the Iron Mountains were broken, leaving the Grey Mountains as a separate range and a northern spur of the newer Misty Mountains.

The Dwarves of Durin's Folk considered the Ered Mithrin as part of their land as far back as the reign of Durin I. Because of constant attack by both Orcs of Morgoth and possibly Dragons, they were not heavily explored or settled until the Third Age. By the end of the Third Age all Dwarven strongholds had been abandoned or raided by dragons, and the Grey Mountains served only to divide Forodwaith from Wilderland. It subsequently became home to many Orc tribes.");
        AddOrcCulture("Isengard Orc",
            @"Isengard Orcs also known as Isengard Goblins or Orcs of Isengard and sometimes as Saruman's Orcs were a Goblin tribe originally from the Misty Mountains in the service of the traitorous wizard Saruman");
        AddOrcCulture("Mirkwood Orc",
            @"Many of the Orc tribes of Mirkwood serve under the control of Dol Guldur, serving the Necromancer's wishes. These Orcs are friends of spiders and other foul things, and hate the Elves who hunt them above all else.");
        AddOrcCulture("Mordorian Orc",
            @"Mordorian Orcs live in the mountains and plains of Mordor, as well as several of the surrounding mountain ranges. They are closer to the Dark Lord than their more distant brethren and customarily serve his wishes. This tribe is also most likely to use Black Speech.");

        _context.SaveChanges();

        #endregion Orcs

        #region Trolls

        FutureProg canSelectTrollProg = new()
        {
            FunctionName = "CanSelectTrollRace",
            Category = "Character",
            Subcategory = "Race",
            FunctionComment = "Determines if the character select the troll race in chargen.",
            ReturnType = (long)ProgVariableTypes.Boolean,
            StaticType = 0,
            FunctionText = @"// You might consider checking RPP
return true"
        };
        canSelectTrollProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = canSelectTrollProg,
            ParameterIndex = 0,
            ParameterType = 8200,
            ParameterName = "ch"
        });
        _context.FutureProgs.Add(canSelectTrollProg);
        _context.SaveChanges();

        (Liquid? trollBlood, Liquid? trollSweat, Material _, Material _) = CreateBloodAndSweat("Troll");

        Race trollRace = new()
        {
            Name = "Troll",
            Description = @"Trolls are a large and brutish race of creatures who inhabit the dark and shadowy regions of Middle-earth. They are a primitive and barbaric people, with a love of violence and destruction. Trolls are known for their immense strength and ferocity in battle, and are feared and hated by the other peoples of Middle-earth. In the Black Speech of Sauron, the word for Troll is Olog.

Despite their reputation as mindless beasts, Trolls are highly intelligent and cunning. They are skilled at adapting to their surroundings and are capable of using simple tools and weapons. Trolls are also highly social, and will often live and hunt in packs. They are fiercely territorial, and will fiercely defend their homes and families from any perceived threat.

Trolls are primarily scavengers and predators, and will eat anything they can catch. They are especially fond of the flesh of Elves and Men, and will often raid their settlements in search of food. Trolls are also known for their love of torture and cruelty, and will often inflict great suffering on their enemies for their own enjoyment. They are feared and hated by the other peoples of Middle-earth, and are often hunted down and destroyed on sight.",
            AllowedGenders = _humanRace.AllowedGenders,
            ParentRace = humanoid,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 1.75,
            CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
            DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
            CanUseWeapons = true,
            CanAttack = true,
            CanDefend = true,
            NaturalArmourQuality = 9,
            NeedsToBreathe = true,
            BreathingModel = "simple",
            SweatRateInLitresPerMinute = 3.0,
            SizeStanding = 7,
            SizeProne = 7,
            SizeSitting = 7,
            CommunicationStrategyType = "humanoid",
            DefaultHandedness = 1,
            HandednessOptions = "1 3",
            MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
            MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
            RaceUsesStamina = true,
            CanEatCorpses = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = "",
            CanEatMaterialsOptIn = false,
            TemperatureRangeFloor = -10,
            TemperatureRangeCeiling = 50,
            BodypartSizeModifier = 1,
            BodypartHealthMultiplier = 2.0,
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
            CanClimb = true,
            CanSwim = true,
            MinimumSleepingPosition = 4,
            ChildAge = 3,
            YouthAge = 9,
            YoungAdultAge = 14,
            AdultAge = 22,
            ElderAge = 40,
            VenerableAge = 60,
            AvailabilityProg = canSelectTrollProg,
            BaseBody = body,
            BloodLiquid = trollBlood,
            BloodModel = humanoid.BloodModel,
            NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
            NaturalArmourType = humanoid.NaturalArmourType,
            RaceButcheryProfile = null,
            SweatLiquid = trollSweat,
			MaximumFoodSatiatedHours = CultureRaceSatiationLimits["Troll"].MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = CultureRaceSatiationLimits["Troll"].MaximumDrinkSatiatedHours
        };
        _context.Races.Add(trollRace);
        AddRaceAttributeAlterations(trollRace, CultureRaceAttributeProfiles["Troll"]);
        HeightWeightModel trollMaleHWModel = new()
        {
            Name = "Troll Male",
            MeanHeight = 350,
            MeanBmi = 23,
            StddevHeight = 35,
            StddevBmi = 3,
            Bmimultiplier = 0.1
        };
        _context.Add(trollMaleHWModel);
        HeightWeightModel trollFemaleHWModel = new()
        {
            Name = "Troll Female",
            MeanHeight = 350,
            MeanBmi = 23.5,
            StddevHeight = 35,
            StddevBmi = 3,
            Bmimultiplier = 0.1
        };
        _context.Add(trollFemaleHWModel);
        trollRace.DefaultHeightWeightModelMale = trollFemaleHWModel;
        trollRace.DefaultHeightWeightModelFemale = trollFemaleHWModel;
        trollRace.DefaultHeightWeightModelNonBinary = trollFemaleHWModel;
        trollRace.DefaultHeightWeightModelNeuter = trollFemaleHWModel;
        _races.Add("Troll", trollRace);
        _context.SaveChanges();

        FutureProg isTrollProg = new()
        {
            FunctionName = "IsTrollRace",
            FunctionComment = "Determines whether someone is the Troll race",
            FunctionText = $"return @ch.Race == ToRace({trollRace.Id})",
            ReturnType = (long)ProgVariableTypes.Boolean,
            Category = "Character",
            Subcategory = "Race",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
        isTrollProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = isTrollProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Toon
        });
        _context.FutureProgs.Add(isTrollProg);
        _context.SaveChanges();
        _raceProgs["Troll"] = isTrollProg;

        FutureProg isTrollBabyProg = CreateVariantAgeProg(isBabyProg, trollRace);
        FutureProg isTrollToddlerProg = CreateVariantAgeProg(isToddlerProg, trollRace);
        FutureProg isTrollBoyProg = CreateVariantAgeProg(isBoyProg, trollRace);
        FutureProg isTrollGirlProg = CreateVariantAgeProg(isGirlProg, trollRace);
        FutureProg isTrollChildProg = CreateVariantAgeProg(isChildProg, trollRace);
        FutureProg isTrollYouthProg = CreateVariantAgeProg(isYouthProg, trollRace);
        FutureProg isTrollYoungManProg = CreateVariantAgeProg(isYoungManProg, trollRace);
        FutureProg isTrollYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, trollRace);
        FutureProg isTrollAdultManProg = CreateVariantAgeProg(isAdultManProg, trollRace);
        FutureProg isTrollAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, trollRace);
        FutureProg isTrollAdultPersonProg = CreateVariantAgeProg(isAdultProg, trollRace);
        FutureProg isTrollOldManProg = CreateVariantAgeProg(isOldManProg, trollRace);
        FutureProg isTrollOldWomanProg = CreateVariantAgeProg(isOldWomanProg, trollRace);
        FutureProg isTrollOldPersonProg = CreateVariantAgeProg(isOldPersonProg, trollRace);
        List<(CharacteristicValue Value, double Weight)> trollPersonValues = new();

        void AddTrollPersonWord(string name, string basic, FutureProg prog, double weight)
        {
            CharacteristicValue pw = new()
            {
                Id = nextId++,
                Definition = personDef,
                Name = name,
                Value = basic,
                AdditionalValue = "",
                Default = false,
                Pluralisation = 0,
                FutureProg = prog
            };
            _context.CharacteristicValues.Add(pw);
            trollPersonValues.Add((pw, weight));
        }

        AddTrollPersonWord("troll baby", "troll baby", isTrollBabyProg, 1.0);
        AddTrollPersonWord("troll infant", "troll baby", isTrollBabyProg, 1.0);
        AddTrollPersonWord("troll toddler", "troll toddler", isTrollToddlerProg, 5.0);
        AddTrollPersonWord("troll", "troll", isTrollAdultPersonProg, 5.0);
        AddTrollPersonWord("male troll", "troll", isTrollAdultManProg, 1.0);
        AddTrollPersonWord("female troll", "troll", isTrollAdultWomanProg, 1.0);
        AddTrollPersonWord("troll child", "troll child", isTrollChildProg, 3.0);
        AddTrollPersonWord("troll youth", "troll youth", isTrollYouthProg, 3.0);
        AddTrollPersonWord("young troll", "troll youth", isTrollYouthProg, 3.0);
        AddTrollPersonWord("troll adolescent", "troll youth", isTrollYouthProg, 1.0);
        AddTrollPersonWord("old troll", "old troll", isTrollOldPersonProg, 1.0);
        AddTrollPersonWord("old female troll", "old troll", isTrollOldWomanProg, 1.0);
        AddTrollPersonWord("old male troll", "old troll", isTrollOldManProg, 1.0);
        _context.SaveChanges();

        CharacteristicProfile trollPersons = new()
        {
            Name = "Troll Person Words",
            Definition = new XElement("Values",
                from item in trollPersonValues
                select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
            ).ToString(),
            Type = "weighted",
            Description = "All person words applicable to the Troll race",
            TargetDefinition = personDef
        };
        _context.CharacteristicProfiles.Add(trollPersons);
        _profiles[trollPersons.Name] = trollPersons;
        _context.SaveChanges();

        void AddTrollEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
            string eyeColour, string hairColour, string skinColour, bool selectable = true)
        {
            AddEthnicity(trollRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
                selectable ? canSelectTrollProg : _alwaysFalseProg, blurb);
            AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
            AddEthnicityVariable(name, "Eye Colour", eyeColour);
            AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
            AddEthnicityVariable(name, "Ears", "All Ears");
            AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
            AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
            AddEthnicityVariable(name, "Hair Colour", hairColour);
            AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
            AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
            AddEthnicityVariable(name, "Nose", "All Noses");
            AddEthnicityVariable(name, "Person Word", "Troll Person Words");
            AddEthnicityVariable(name, "Skin Colour", skinColour);
            _context.SaveChanges();
        }

        AddTrollEthnicity("Cave Troll", @"Of the Wild Trolls, Cave Trolls are the largest and most powerful breed. They are extremely solitary and cannibalistic. They are almost blind, but have a superb sense of hearing and smell. Their scale hides are pale, like those of most cave-dwelling creatures.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
            "Fair_Skin");
        AddTrollEthnicity("Forest Troll", @"Forest Trolls are the least brutal of the Wild Trolls. They are more graceful (relatively) and live in loosely organised bands who hunt together. They are rarely cannibalistic, preferring woodland game (or Man or Dwarf if they can manage). They are experts with slings, snares and skinning knives.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
            "Swarthy_Skin");
        AddTrollEthnicity("Hill Troll", @"Hill Trolls live in small groups, but are quite quarrelsome and greedy. They prefer clubs and thrown stones in a fight and are very territorial. They guard their stolen treasures jealously, even those of little use to them such as books.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
            "Swarthy_Skin");
        AddTrollEthnicity("Snow Troll", @"Snow Trolls are rare creatures with grey-white hides and icy blue eyes. When exposed to sunlight they turn into pillars of icy slag. They go for long periods without food, but are virtually unstoppable when they at last sight prey.", "Wild", "", "B Dominant", "Blue_Eyes", "Black_Brown_Grey_Hair",
            "Fair_Skin");
        AddTrollEthnicity("Stone Troll", @"Stone Trolls spend a great deal of time hoarding piles of food and treasure, stealing it from each other, and boasting of their riches. Their fratricidal tendencies are extreme.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
            "Olive_Skin");
        AddTrollEthnicity("Olog-Hai", @"The Olog-hai have been bred by Sauron from lesser Troll stock and have until late been a rare breed. Cunning and organised; yet as big and strong as their lesser brethren. The Olog-hai are superb warriors; they know no fear and thirst for blood and victory.

Olog-hai are also called Black Trolls by some, for they have black scaly hides and black blood. Most carry blank shields and warhammers, although they are adept at using almost any weapon. They differ from older Troll varieties in other ways as well, for example, they do not turn to stone in daylight.", "Greater", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
            "Dark_Skin", false);

        FutureProg trollSkillProg = new()
        {
            FunctionName = "TrollSkillStartingValue",
            Category = "Chargen",
            Subcategory = "Skills",
            FunctionComment = "Used to determine the opening value for a skill for trolls at character creation",
            ReturnType = (int)ProgVariableTypes.Number,
            AcceptsAnyParameters = false,
            Public = false,
            StaticType = 0,
            FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
        };
        trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = trollSkillProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (int)ProgVariableTypes.Toon
        });
        trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = trollSkillProg,
            ParameterIndex = 1,
            ParameterName = "trait",
            ParameterType = (int)ProgVariableTypes.Trait
        });
        trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = trollSkillProg,
            ParameterIndex = 2,
            ParameterName = "boosts",
            ParameterType = (int)ProgVariableTypes.Number
        });
        _context.FutureProgs.Add(trollSkillProg);
        _context.SaveChanges();

        #endregion Trolls
    }
}
