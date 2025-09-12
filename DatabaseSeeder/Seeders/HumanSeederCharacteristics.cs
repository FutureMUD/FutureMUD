using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	private void SetupCharacteristics(bool useDistinctive, Race humanRace)
	{
		#region Shapes

		var eyeShape = _context.BodypartShapes.First(x => x.Name == "Eye");
		var noseShape = _context.BodypartShapes.First(x => x.Name == "Nose");
		var earShape = _context.BodypartShapes.First(x => x.Name == "Ear");

		#endregion

		#region Characteristic Definitions

		var colourDef = _context.CharacteristicDefinitions.First(x => x.Name == "Colour");
		var eyeColourDef = new CharacteristicDefinition
		{
			Name = "Eye Colour",
			Type = 2,
			Pattern = "^eyecolou?r",
			Description = "The colour of one's eyes",
			ParentId = colourDef.Id,
			Model = "bodypart",
			ChargenDisplayType = 1,
			Definition =
				$"<Definition><TargetShape>{eyeShape.Id}</TargetShape><OrdinaryCount>2</OrdinaryCount></Definition>"
		};
		_context.CharacteristicDefinitions.Add(eyeColourDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = eyeColourDef, Usage = "base" });

		var eyeShapeDef = new CharacteristicDefinition
		{
			Name = "Eye Shape",
			Type = 3,
			Pattern = "^eyeshape",
			Description = "The shape of one's eyes",
			Model = "bodypart",
			ChargenDisplayType = 2,
			Definition =
				$"<Definition><TargetShape>{eyeShape.Id}</TargetShape><OrdinaryCount>2</OrdinaryCount></Definition>"
		};
		_context.CharacteristicDefinitions.Add(eyeShapeDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = eyeShapeDef, Usage = "base" });

		var noseDef = new CharacteristicDefinition
		{
			Name = "Nose",
			Type = 3,
			Pattern = "^nose",
			Description = "The shape and nature of one's nose",
			Model = "bodypart",
			ChargenDisplayType = 2,
			Definition =
				$"<Definition><TargetShape>{noseShape.Id}</TargetShape><OrdinaryCount>1</OrdinaryCount></Definition>"
		};
		_context.CharacteristicDefinitions.Add(noseDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = noseDef, Usage = "base" });

		var earDef = new CharacteristicDefinition
		{
			Name = "Ears",
			Type = 3,
			Pattern = "^ears",
			Description = "The shape and size of one's ears",
			Model = "bodypart",
			ChargenDisplayType = 2,
			Definition =
				$"<Definition><TargetShape>{earShape.Id}</TargetShape><OrdinaryCount>2</OrdinaryCount></Definition>"
		};
		_context.CharacteristicDefinitions.Add(earDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = earDef, Usage = "base" });


		var hairColourDef = new CharacteristicDefinition
		{
			Name = "Hair Colour",
			Type = 2,
			Pattern = "^haircolou?r",
			Description = "The colour of one's hair",
			ParentId = colourDef.Id,
			Model = "Standard",
			ChargenDisplayType = 1
		};
		_context.CharacteristicDefinitions.Add(hairColourDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = hairColourDef, Usage = "base" });
		_context.SaveChanges();
		var facialHairColourDef = new CharacteristicDefinition
		{
			Name = "Facial Hair Colour",
			Type = 2,
			Pattern = "^facialhaircolou?r",
			Description = "The colour of one's facial hair",
			ParentId = colourDef.Id,
			Model = "Standard",
			ChargenDisplayType = 1,
			Parent = hairColourDef
		};
		_context.CharacteristicDefinitions.Add(facialHairColourDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = facialHairColourDef, Usage = "male" });

		var hairStyleDef = new CharacteristicDefinition
		{
			Name = "Hair Style",
			Type = 4,
			Pattern = "^hairstyle",
			Description = "The style of one's head hair",
			Model = "Standard",
			ChargenDisplayType = 2
		};
		_context.CharacteristicDefinitions.Add(hairStyleDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = hairStyleDef, Usage = "base" });

		var facialHairStyleDef = new CharacteristicDefinition
		{
			Name = "Facial Hair Style",
			Type = 4,
			Pattern = "^facialhairstyle",
			Description = "The style of one's facial hair",
			Model = "Standard",
			ChargenDisplayType = 2
		};
		_context.CharacteristicDefinitions.Add(facialHairStyleDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = facialHairStyleDef, Usage = "male" });

		var skinColourDef = new CharacteristicDefinition
		{
			Name = "Skin Colour",
			Type = 3,
			Pattern = "^skin(colou?r|tone)",
			Description = "The style of one's facial hair",
			Model = "Standard",
			ChargenDisplayType = 0
		};
		_context.CharacteristicDefinitions.Add(skinColourDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = skinColourDef, Usage = "base" });

		var frameDef = new CharacteristicDefinition
		{
			Name = "Frame",
			Type = 3,
			Pattern = "^frame",
			Description = "Frames for humanoids",
			Model = "Standard",
			ChargenDisplayType = 1
		};
		_context.CharacteristicDefinitions.Add(frameDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = frameDef, Usage = "base" });

		var personDef = new CharacteristicDefinition
		{
			Name = "Person Word",
			Type = 3,
			Pattern = "^person",
			Description = "Word Describing Personage, e.g. man, woman, maiden",
			Model = "Standard",
			ChargenDisplayType = 0
		};
		_context.CharacteristicDefinitions.Add(personDef);
		_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
			{ Race = humanRace, CharacteristicDefinition = personDef, Usage = "base" });

		_context.SaveChanges();

		#endregion

		#region Characteristic Profiles

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Eye Colours",
			TargetDefinition = eyeColourDef,
			Type = "all",
			Description = "All Defined Eye Colour Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Eye Shapes",
			TargetDefinition = eyeShapeDef,
			Type = "all",
			Description = "All Defined Eye Shape Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Noses",
			TargetDefinition = noseDef,
			Type = "all",
			Description = "All Defined Nose Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Ears",
			TargetDefinition = earDef,
			Type = "all",
			Description = "All Defined Ear Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Hair Colours",
			TargetDefinition = hairColourDef,
			Type = "all",
			Description = "All Defined Hair Colour Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Facial Hair Colours",
			TargetDefinition = facialHairColourDef,
			Type = "all",
			Description = "All Defined Facial Hair Colour Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Hair Styles",
			TargetDefinition = hairStyleDef,
			Type = "all",
			Description = "All Defined Hair Style Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Facial Hair Styles",
			TargetDefinition = facialHairStyleDef,
			Type = "all",
			Description = "All Defined Facial Hair Style Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Skin Colours",
			TargetDefinition = skinColourDef,
			Type = "all",
			Description = "All Defined Skin Colour Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Frames",
			TargetDefinition = frameDef,
			Type = "all",
			Description = "All Defined Frame Values",
			Definition = "<Definition/>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "All Person Words",
			TargetDefinition = personDef,
			Type = "all",
			Description = "All Defined Person Word Values",
			Definition = "<Definition/>"
		});

		_context.SaveChanges();

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Blue_Eyes",
			TargetDefinition = eyeColourDef,
			Type = "Standard",
			Description = "Valid Blue Eye Colours",
			Definition =
				"<Values> <Value>grey</Value> <Value>light grey</Value> <Value>dark grey</Value> <Value>blue</Value> <Value>dark blue</Value> <Value>cerulean</Value> <Value>mist grey</Value> <Value>thistle grey</Value> <Value>slate grey</Value> <Value>soft grey</Value> <Value>chartreuse</Value> <Value>slate blue</Value> <Value>bright blue</Value> <Value>powder blue</Value> <Value>sapphire blue</Value> <Value>royal blue</Value> <Value>ocean blue</Value> <Value>sky blue</Value> <Value>azure</Value> <Value>beryl</Value> <Value>light blue</Value> <Value>pale blue</Value> <Value>deep blue</Value> <Value>winter blue</Value> <Value>storm blue</Value> <Value>cobalt blue</Value> <Value>light steel blue</Value> <Value>steel blue</Value> <Value>azure blue</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Eyes",
			TargetDefinition = eyeColourDef,
			Type = "Standard",
			Description = "Valid Brown Eye Colours",
			Definition =
				"<Values> <Value>brown</Value> <Value>caramel</Value> <Value>light brown</Value> <Value>dark brown</Value> <Value>copper</Value> <Value>ochre</Value> <Value>amber</Value> <Value>earthen brown</Value> <Value>deep brown</Value> <Value>rich brown</Value> <Value>burnt sienna</Value> <Value>chocolate</Value> <Value>cinnamon</Value> <Value>mahogany</Value> <Value>nut brown</Value> <Value>umber</Value> <Value>reddish brown</Value> <Value>beige</Value> <Value>sienna brown</Value> <Value>saddle brown</Value> <Value>sepia brown</Value> <Value>fire brick brown</Value> <Value>chocolate brown</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Green_Eyes",
			TargetDefinition = eyeColourDef,
			Type = "Standard",
			Description = "Valid Green Eye Colours",
			Definition =
				"<Values> <Value>green</Value> <Value>dark green</Value> <Value>emerald green</Value> <Value>spring green</Value> <Value>sea green</Value> <Value>hunter green</Value> <Value>olive green</Value> <Value>sage green</Value> <Value>pine green</Value> <Value>bright green</Value> <Value>rich green</Value> <Value>pale green</Value> <Value>verdant green</Value> <Value>forest green</Value> <Value>light green</Value> <Value>hazel</Value> <Value>chartreuse green</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Black_Hair",
			TargetDefinition = hairColourDef,
			Type = "Standard",
			Description = "Valid Black Hair Colours",
			Definition =
				"<Values> <Value>black</Value> <Value>onyx</Value> <Value>obsidian</Value> <Value>midnight black</Value> <Value>ink black</Value> <Value>jet black</Value> <Value>pitch black</Value> <Value>salt-and-pepper</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Blonde_Hair",
			TargetDefinition = hairColourDef,
			Type = "Standard",
			Description = "Valid Blonde Hair Colours",
			Definition =
				"<Values> <Value>pale yellow</Value> <Value>golden yellow</Value> <Value>sand yellow</Value> <Value>yellow</Value> <Value>blonde</Value> <Value>dirty blonde</Value> <Value>silver blonde</Value> <Value>ash blonde</Value> <Value>strawberry blonde</Value> <Value>platinum blonde</Value> <Value>light blonde</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Hair",
			TargetDefinition = hairColourDef,
			Type = "Standard",
			Description = "Valid Brown Hair Colours",
			Definition =
				"<Values> <Value>brown</Value> <Value>caramel</Value> <Value>sandy brown</Value> <Value>light brown</Value> <Value>dark brown</Value> <Value>auburn</Value> <Value>earthen brown</Value> <Value>deep brown</Value> <Value>rich brown</Value> <Value>burnt sienna</Value> <Value>chocolate</Value> <Value>cinnamon</Value> <Value>mahogany</Value> <Value>nut brown</Value> <Value>umber</Value> <Value>beige</Value> <Value>dark</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Red_Hair",
			TargetDefinition = hairColourDef,
			Type = "Standard",
			Description = "Valid Red Hair Colours",
			Definition =
				"<Values> <Value>red</Value> <Value>dark red</Value> <Value>auburn</Value> <Value>crimson</Value> <Value>scarlet</Value> <Value>ruby red</Value> <Value>blood red</Value> <Value>rose red</Value> <Value>wine red</Value> <Value>flame red</Value> <Value>coral</Value> <Value>copper</Value> <Value>fiery orange</Value> <Value>ochre</Value> <Value>sunset orange</Value> <Value>orange</Value> <Value>reddish brown</Value> <Value>light red</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Grey_Hair",
			TargetDefinition = hairColourDef,
			Type = "Standard",
			Description = "Valid Grey Hair Colours",
			Definition =
				"<Values> <Value>white</Value> <Value>grey</Value> <Value>light grey</Value> <Value>dark grey</Value> <Value>pale white</Value> <Value>ivory</Value> <Value>seashell</Value> <Value>snow white</Value> <Value>gleaming white</Value> <Value>pure white</Value> <Value>pearl white</Value> <Value>bright white</Value> <Value>bone white</Value> <Value>ghost white</Value> <Value>mist grey</Value> <Value>charcoal grey</Value> <Value>thistle grey</Value> <Value>smoky grey</Value> <Value>slate grey</Value> <Value>silver grey</Value> <Value>soft grey</Value> <Value>ash grey</Value> </Values>"
		});
		_context.SaveChanges();

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "RedBrown_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Reddish Brown Skin Tones",
			Definition =
				"<Values> <Value>russet</Value> <Value>copper</Value> <Value>light copper</Value> <Value>dark copper</Value> <Value>russet</Value> <Value>cinnamon</Value> <Value>mahogany</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Black_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Black Skin Tones",
			Definition =
				"<Values> <Value>dark brown</Value> <Value>ebony</Value> <Value>black</Value> <Value>obsidian</Value> <Value>jet</Value> <Value>cocoa</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Brown Skin Tones",
			Definition =
				"<Values>   <Value>dark olive</Value>   <Value>light brown</Value>   <Value>brown</Value>   <Value>caramel</Value>   <Value>swarthy</Value>   <Value>chestnut</Value>   <Value>mocha</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Fair_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Fair Skin Tones",
			Definition =
				"<Values> <Value>white</Value> <Value>milky white</Value> <Value>pale white</Value> <Value>pasty white</Value> <Value>tanned</Value> <Value>ruddy</Value> <Value>sallow</Value> <Value>pale</Value> <Value>translucent</Value> <Value>pallid</Value> <Value>ashen</Value> <Value>snow white</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Olive_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Olive Skin Tones",
			Definition =
				"<Values> <Value>tanned</Value> <Value>olive</Value> <Value>bronzed</Value> <Value>dark olive</Value> <Value>light brown</Value> <Value>pale olive</Value> <Value>dusky</Value> </Values>"
		});

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Golden_Skin",
			TargetDefinition = skinColourDef,
			Type = "Standard",
			Description = "Golden Skin Tones",
			Definition =
				"<Values> <Value>white</Value> <Value>milky white</Value> <Value>pale white</Value> <Value>pasty white</Value> <Value>olive</Value> <Value>oriental</Value> <Value>dark olive</Value> <Value>light brown</Value> <Value>pale olive</Value> <Value>golden</Value> <Value>light golden</Value> <Value>tawny</Value> <Value>buttermilk</Value> </Values>"
		});
		_context.SaveChanges();

		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Blue_Eyes", Type = "compound", TargetDefinition = eyeColourDef,
			Description = "Blue, Brown and Grey eyes only",
			Definition = "<Definition><Profile>blue_eyes</Profile><Profile>brown_eyes</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Green_Eyes", Type = "compound", TargetDefinition = eyeColourDef,
			Description = "Green, Brown and Hazel eyes only",
			Definition = "<Definition><Profile>brown_eyes</Profile><Profile>green_eyes</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Green_Blue_Eyes", Type = "compound", TargetDefinition = eyeColourDef,
			Description = "Green, Blue, Grey, Brown and Hazel eyes only",
			Definition =
				"<Definition><Profile>blue_eyes</Profile><Profile>brown_eyes</Profile><Profile>green_eyes</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Blue_Green_Eyes", Type = "compound", TargetDefinition = eyeColourDef,
			Description = "Green, Blue, Grey eyes only",
			Definition = "<Definition><Profile>blue_eyes</Profile><Profile>green_eyes</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Black_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Black and Grey hair types only",
			Definition = "<Definition><Profile>grey_hair</Profile><Profile>black_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Black_Brown_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Black, Brown and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>black_hair</Profile><Profile>brown_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Brown and Grey hair types only",
			Definition = "<Definition><Profile>grey_hair</Profile><Profile>brown_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Red_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Brown, Red and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>brown_hair</Profile><Profile>red_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Blonde_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Brown, Blonde and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>brown_hair</Profile><Profile>blonde_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Brown_Blonde_Red_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Brown, Blonde, Red and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>brown_hair</Profile><Profile>blonde_hair</Profile><Profile>red_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Blonde_Red_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Blonde, Red and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>blonde_hair</Profile><Profile>red_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Blonde_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Blonde and Grey hair types only",
			Definition = "<Definition><Profile>grey_hair</Profile><Profile>blonde_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Black_Brown_Blonde_Grey_Hair", Type = "compound", TargetDefinition = hairColourDef,
			Description = "Black, Brown, Blonde and Grey hair types only",
			Definition =
				"<Definition><Profile>grey_hair</Profile><Profile>black_hair</Profile><Profile>brown_hair</Profile><Profile>blonde_hair</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Fair_Olive_Skin", Type = "compound", TargetDefinition = skinColourDef,
			Description = "All fair and olive skin tones",
			Definition = "<Definition><Profile>olive_skin</Profile><Profile>fair_skin</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Dark_Skin", Type = "compound", TargetDefinition = skinColourDef,
			Description = "All black and brown skin",
			Definition = "<Definition><Profile>brown_skin</Profile><Profile>black_skin</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Swarthy_Skin", Type = "compound", TargetDefinition = skinColourDef,
			Description = "All olive and brown skin tones",
			Definition = "<Definition><Profile>brown_skin</Profile><Profile>olive_skin</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Fair_Swarthy_Skin", Type = "compound", TargetDefinition = skinColourDef,
			Description = "All fair, olive and brown skin tones",
			Definition =
				"<Definition><Profile>fair_skin</Profile><Profile>brown_skin</Profile><Profile>olive_skin</Profile></Definition>"
		});
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "No_Facial_Hair", Type = "Standard", TargetDefinition = facialHairStyleDef,
			Description = "For races and whatnot that don't have facial hair",
			Definition = "<Values> <Value>clean shave</Value> </Values>"
		});
		_context.SaveChanges();

		#endregion

		#region Progs

		var useNB = _questionAnswers["nonbinary"].EqualToAny("yes", "y");
		var humanProg = new FutureProg
		{
			FunctionName = "IsHumanoid",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "True if the character is a type of humanoid",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return SameRace(@ch.Race, ToRace(\"Humanoid\"))"
		};
		humanProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = humanProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(humanProg);
		var humanFemaleProg = new FutureProg
		{
			FunctionName = "IsHumanoidFemale",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "True if the character is a type of humanoid and is female",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = useNB
				? "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Gender != ToGender(\"male\")"
				: "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Gender == ToGender(\"female\")"
		};
		humanFemaleProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = humanFemaleProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(humanFemaleProg);

		var humanMaleProg = new FutureProg
		{
			FunctionName = "IsHumanoidMale",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "True if the character is a type of humanoid and is male",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = useNB
				? "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Gender != ToGender(\"female\")"
				: "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Gender == ToGender(\"male\")"
		};
		humanMaleProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = humanMaleProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(humanMaleProg);

		var humanNonFemaleProg = new FutureProg
		{
			FunctionName = "IsHumanoidNonFemale",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment =
				"True if the character is a type of humanoid and is male or non-binary. Used for facial hair characteristics.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Gender != ToGender(\"female\")"
		};
		humanNonFemaleProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = humanNonFemaleProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(humanNonFemaleProg);

		var bmiProg = new FutureProg
		{
			FunctionName = "GetBMI",
			Category = "Character",
			Subcategory = "Physical",
			FunctionComment = "Calculates a BMI for a character.",
			ReturnType = 2,
			StaticType = 0,
			FunctionText = "return (@ch.Weight / 1000) / ((@ch.Height / 100) ^ 2)"
		};
		bmiProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = bmiProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(bmiProg);

		var underweightProg = new FutureProg
		{
			FunctionName = "CanPickUnderweightDescs",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is eligable for \"underweight\" description values.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return @GetBMI(@ch) < 22"
		};
		underweightProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = underweightProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(underweightProg);

		var normalWeightProg = new FutureProg
		{
			FunctionName = "CanPickMidweightDescs",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is eligable for \"middle weight\" description values.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = @"var bmi as number
bmi = @GetBMI(@ch)
return @bmi > 17 and @bmi < 29"
		};
		normalWeightProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = normalWeightProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(normalWeightProg);

		var overweightProg = new FutureProg
		{
			FunctionName = "CanPickOverweightDescs",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is eligable for \"overweight\" description values.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = @"var bmi as number
bmi = @GetBMI(@ch)
return @bmi > 24 and @bmi < 35"
		};
		overweightProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = overweightProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(overweightProg);

		var obeseProg = new FutureProg
		{
			FunctionName = "CanPickObeseDescs",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is eligable for \"obese\" description values.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = @"return @GetBMI(@ch) >= 31"
		};
		obeseProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = obeseProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(obeseProg);

		var isBabyProg = new FutureProg
		{
			FunctionName = "IsHumanoidBaby",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is a baby of a humanoid race.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.Age < 1"
		};
		isBabyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isBabyProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isBabyProg);


		var isToddlerProg = new FutureProg
		{
			FunctionName = "IsHumanoidToddler",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character is a toddler of a humanoid race.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Baby\" and @ch.Age >= 1"
		};
		isToddlerProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isToddlerProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isToddlerProg);

		var isBoyProg = new FutureProg
		{
			FunctionName = "IsHumanoidBoy",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid boy descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Child\" and @ch.Gender != ToGender(\"Female\")"
		};
		isBoyProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isBoyProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isBoyProg);

		var isGirlProg = new FutureProg
		{
			FunctionName = "IsHumanoidGirl",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid girl descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Child\" and @ch.Gender != ToGender(\"Male\")"
		};
		isGirlProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isGirlProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isGirlProg);

		var isChildProg = new FutureProg
		{
			FunctionName = "IsHumanoidChild",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid child descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Child\""
		};
		isChildProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isChildProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isChildProg);

		var isYouthProg = new FutureProg
		{
			FunctionName = "IsHumanoidYouth",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick gender neutral humanoid youth descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText = "return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Youth\""
		};
		isYouthProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isYouthProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isYouthProg);

		var isYoungManProg = new FutureProg
		{
			FunctionName = "IsHumanoidYoungMan",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid young man descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Youth\" or @ch.AgeCategory == \"YoungAdult\") and @ch.Gender != ToGender(\"Female\")"
		};
		isYoungManProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isYoungManProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isYoungManProg);

		var isYoungWomanProg = new FutureProg
		{
			FunctionName = "IsHumanoidYoungWoman",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid young woman descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Youth\" or @ch.AgeCategory == \"YoungAdult\") and @ch.Gender != ToGender(\"Male\")"
		};
		isYoungWomanProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isYoungWomanProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isYoungWomanProg);

		var isYoungAdultProg = new FutureProg
		{
			FunctionName = "IsHumanoidYoungAdult",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid young adult descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Youth\" or @ch.AgeCategory == \"YoungAdult\")"
		};
		isYoungAdultProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isYoungAdultProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isYoungAdultProg);

		var isAdultManProg = new FutureProg
		{
			FunctionName = "IsHumanoidAdultMan",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid adult man descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Adult\" or @ch.AgeCategory == \"YoungAdult\") and @ch.Gender != ToGender(\"Female\")"
		};
		isAdultManProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isAdultManProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isAdultManProg);

		var isAdultWomanProg = new FutureProg
		{
			FunctionName = "IsHumanoidAdultWoman",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid adult woman descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Adult\" or @ch.AgeCategory == \"YoungAdult\") and @ch.Gender != ToGender(\"Male\")"
		};
		isAdultWomanProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isAdultWomanProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isAdultWomanProg);

		var isAdultProg = new FutureProg
		{
			FunctionName = "IsHumanoidAdult",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid adult descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Adult\" or @ch.AgeCategory == \"YoungAdult\")"
		};
		isAdultProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isAdultProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isAdultProg);

		var isOldManProg = new FutureProg
		{
			FunctionName = "IsHumanoidOldMan",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid old man descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Elder\" or @ch.AgeCategory == \"Venerable\") and @ch.Gender != ToGender(\"Female\")"
		};
		isOldManProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isOldManProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isOldManProg);

		var isOldWomanProg = new FutureProg
		{
			FunctionName = "IsHumanoidOldWoman",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid old woman descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Elder\" or @ch.AgeCategory == \"Venerable\") and @ch.Gender != ToGender(\"Male\")"
		};
		isOldWomanProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isOldWomanProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isOldWomanProg);

		var isOldPersonProg = new FutureProg
		{
			FunctionName = "IsHumanoidOldPerson",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = "Determines if the character can pick humanoid old person descriptions.",
			ReturnType = 4,
			StaticType = 0,
			FunctionText =
				"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and (@ch.AgeCategory == \"Elder\" or @ch.AgeCategory == \"Venerable\")"
		};
		isOldPersonProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isOldPersonProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(isOldPersonProg);
		_context.SaveChanges();

		#endregion

		#region Values

		void AddStyleableCharacteristic(long id, CharacteristicDefinition definition, string name, string value, string additionalValue, int growthStage, Difficulty styleDifficulty, long styleToolTag, bool isDefault = false, int pluralisation = 0)
		{
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = id,
				Definition = definition,
				Name = name,
				Value = value,
				AdditionalValue = $"{growthStage} {(int)styleDifficulty} {styleToolTag} {additionalValue}",
				Default = isDefault,
				Pluralisation = pluralisation
			});
		}

		void AddCharacteristicValue(long id, CharacteristicDefinition definition, string name, string value, string additionalValue, FutureProg prog = null, bool isDefault = false, int pluralisation = 0)
		{
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = id,
				Definition = definition,
				Name = name,
				Value = value,
				AdditionalValue = additionalValue,
				Default = isDefault,
				Pluralisation = pluralisation,
				FutureProg = prog
			});
		}

		var nextId = _context.CharacteristicValues.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		AddCharacteristicValue(nextId++, eyeShapeDef, "round", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "almond", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "slanted", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "beady", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "narrow", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "teardrop", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "sunken", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "droopy", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "hooded", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "close-set", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "deep-set", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "hollow", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "puffy", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "doe-like", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "small", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "protruding", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "monolid", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "heavy-lidded", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "prominent", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "upturned", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "asymmetrical", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "downturned", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "wide-set", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "wide", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "large", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "big", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "small", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "tiny", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "rheumy", "", "");
		AddCharacteristicValue(nextId++, eyeShapeDef, "watery", "", "");
		_context.SaveChanges();

		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "clean shave", "clean-shaven", "a clean chin with no hair at all", 0, Difficulty.Automatic, 0, true, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "handlebar moustache", "handlebar-moustached", "a moustache with lengthy and upwardly curved extremities", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "moustache", "moustached", "a naturally-styled moustache", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "goatee", "goateed", "a naturally-styled moustache and adjoining chin beard", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "beard", "bearded", "a full face beard", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "muttonchops", "muttonchopped", "sideburns extending well onto the jawline", 3, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "sideburns", "sideburned", "twin patches of natural hair from the scalp to below the ear, with a clean chin", 2, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "curly sideburns", "curly-sideburned", "twin patches of mid-length curly hair, from the scalp to below the ear, with a clean chin", 2, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "full beard", "full-bearded", "a full, thick beard displaying unmodified growth on the face, neck, chin, cheeks and sideburns.", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "chin curtain", "curtain-bearded", "a short beard running along the jaw line, visibly hanging below the jaw as a curtain hangs from a rod", 3, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "chinstrap", "chinstrapped", "a pair of sideburns, connected by a short, narrow beard along the bottom of the jaw", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "friendly muttonchops", "muttonchopped", "a pair of naturally styled muttonchops, connected by a moustache but with a clean chin", 4, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "fu manchu", "fu-manchued", "a thin, very narrow moustache that grows downward in two very long tendrils from the upper lip.", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "goat patch", "goat-patched", "a narrow strip of hair growing only on the chin, resembling the beard of a goat", 3, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "german goatee", "goateed", "a neatly trimmed goatee which wraps around the mouth, but whose moustache flares outward past the connecting chin lines", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "horseshoe moustache", "horseshoe-moustached", "a full moustache with ends that extend down in parallel straight lines, with a clean shaven chin", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "sidewhiskers", "sidewhiskered", "a pair of muttonchops and connecting moustache, where the portions connecting the sideburns and moustache hang several inches below the chin", 4, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "neckbeard", "neckbearded", "a beard that grows only on the neck and under the jaw", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "pencil moustache", "pencil-moustached", "a very thin, very short moustache sitting just above the lip", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "shenandoah", "shenandoahed", "a combination of a chin curtain and neckbeard, with full, unmodified growth on the neck, lower jawline, sideburns, but a notably hair free lip", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "soul patch", "soul-patched", "a small, single patch of hair just below the lip", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "stubble", "stubbly", "a short growth of stubble all over &his jaw", 1, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "coarse stubble", "coarse-stubbled", "a coarse, prickly growth of stubble all over &his jaw", 2, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "toothbrush moustache", "toothbrush-moustached", "a narrow but tall moustache that does not extend beyond the side of the nose", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "french beard", "french-bearded", "a long, pointy goatee where the chin beard and moustache do not touch", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "walrus moustache", "walrus-moustached", "a thick, bushy growth of whiskers on the lip like the moustache of a walrus", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "beard", "barbate", "a thick and full beard", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "long beard", "long-bearded", "a very long, very thick full face beard", 5, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "patchy beard", "patchy-bearded", "a thin, patchy beard", 3, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "ducktail beard", "ducktail-bearded", "a full-faced beard and moustache, with a mid-length portion hanging below the chin and tapering to a point", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "french fork", "french-forked", "a full-faced beard and moustache, with a mid-length portion hanging down over the chin and styled into two distinct forks", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "chevron moustache", "chevron-moustached", "a neatly trimmed moustache extending little beyond the edges of the nose and cleanly tapered into thick points", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "lampshade moustache", "lampshade-moustached", "a short, trimmed moustache extending only to the edges of the nose and forming a trapezoidal shape, like the profile view of a lampshade", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "petite handlebar", "petite-handlebared", "a short, sharp moustache which curves up at the edges, just past the edge of the nostrils", 2, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "short beard", "short-bearded", "a short full-face beard, in a transitional period between stubble and a real beard", 3, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "pube beard", "pube-bearded", "a nasty, patchy short beard that looks like pubic hair growing on &his chin", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, facialHairStyleDef, "mange beard", "mangey-bearded", "a pathetic bit of facial hair growth. &his beard has bare patches that sort of resemble the early onset of mange", 3, Difficulty.Automatic, 0);
		_context.SaveChanges();
		AddCharacteristicValue(nextId++, frameDef, "emaciated", "emaciated", "abnormally thin and emaciated", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "gaunt", "emaciated", "lean and haggard, looking gaunt", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "lean", "thin", "of a lean, thin build", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "very thin", "emaciated", "very thin frame, with not an ounce of fat", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "thin", "thin", "tends towards the thin side of a healthy frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "skinny", "thin", "of a very skinny build", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "well-proportioned", "normal", "of an extremely fit and healthy build, being well proportioned in all respects", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "muscular", "muscular", "extremely fit looking, with well defined musculature", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "ripped", "muscular", "in every respect ripped, with showy, well-developed muscles", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "bulky", "muscular", "bulky but muscular looking", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "normal", "normal", "of a perfectly ordinary build, with little excess fat or muscle and ordinary proportions", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "flabby", "fat", "flabby, with various pockets of loose, dangling skin", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "chubby", "fat", "a little heavier than normal, with chubby features and a small amount of excess fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "voluptuous", "fat", "large, but well proportioned", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "curvaceous", "fat", "shapely and of ample proportion, with well defined curves", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "curvy", "fat", "shapely and of ample proportion, with well defined curves", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "fat", "fat", "unambiguously fat, being too heavy for their frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "overweight", "fat", "a little overweight, with plenty of excess body fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "pudgy", "fat", "fat in the face, or pudgy", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "obese", "obese", "grossly overweight, with significant extra fat all over the body", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "very fat", "obese", "very fat, in the obese range", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "morbidly obese", "obese", "belarded by great, hanging sacks of fat", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "pot bellied", "obese", "in possession of an enormous, round pot belly", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "barrel chested", "fat", "in possession of a huge barrel chest, on an otherwise large but muscular frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "skeletal", "emaciated", "nothing more than skin and bones - at least by appearances, a walking skeleton", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "ropy", "thin", "whipcord thin, with all the appearance of knotted rope where muscle meets joint and bone", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "rawboned", "thin", "gaunt enough that &his bones are painfully obvious, jutting in sharp angles beneath the skin", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "lanky", "thin", "lean enough to appear long-limbed, with an ungraceful, stork-like appearance", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "bony", "emaciated", "bony, with &his skin stretched tightly over &his ribcage, possessing a jutting collarbone and pronounced cheekbones", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "bantam", "thin", "small, though with a certain chaotic vigor that belies that tiny frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "little", "thin", "undersized and of below average frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "petite", "thin", "rather small, with a thin enough build to be considered dainty", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "scrawny", "emaciated", "thinly-framed enough that &his ribs are clearly defined beneath the skin of &his chest", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "sinewy", "thin", "thin and lean, with sinewy muscle that is sharply pronounced beneath their skin, wound tightly around their bird-like bones", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "slender", "thin", "as slim and thin as a tree sapling, with longish limbs and little body fat", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "scrappy", "thin", "rough and scrawny, with an underfed appearance, bony hands, and knobby knees", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "slim", "thin", "slimly built, with little to no excess fat on &his frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "willowy", "thin", "in possession of a willowy frame that carries no unnecessary fat", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "gangly", "emaciated", "gaunt and underfed, with &his bones protruding from beneath &his skin, all joints and odd angles", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "lithe", "thin", "sleek and slim, with a graceful, cat-like appearance to &his frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "delicate", "thin", "of a seemingly fragile frame: thin, undersized, and delicately boned, with many fine features", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "lissome", "thin", "slim and soft-fleshed, with no hard muscle definition, but no sharp gauntness either", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "spry", "thin", "small and thin, but with a startling vitality to the remaining fleshiness of &his frame", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "supple", "thin", "slim and soft - thin without being gaunt, while retaining no unnecessary body fat", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "lithesome", "thin", "slim and soft-fleshed, with no hard muscle definition, but no sharp gauntness either", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "average", "normal", "of an ordinary build - not particularly muscular, but not overly gaunt", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "athletic", "normal", "fit and trim, with an athletic build, toned calves, and a flat abdomen", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "fit", "normal", "fit and trim, with toned muscles and a flat abdomen", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "strapping", "normal", "quite healthy by all appearances, displaying some muscle definition - but not enough to be classified as 'bulky'", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "vigorous", "normal", "quite healthy by all appearances, with a fit build and a frame that carries no unnecessary body fat", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "typical", "normal", "of a fairly typical build for one of &his race", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "rugged", "muscular", "quite muscular and fit, with a hard ruggedness to &his frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "burly", "muscular", "broad and squat, with a thickly muscular frame that pays special mind to &his arms and shoulders", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "stout", "muscular", "squat but muscular, with very little in the way of curves or softness", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "heavyset", "muscular", "squat but muscular, wearing &his bulkiness well on &his heavyset frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "thick-limbed", "muscular", "in possession of limbs that are thickly corded with muscle, skin and some fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "beefy", "fat", "large, muscular and broad, with wide shoulders and a barrel chest", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "broad-shouldered", "muscular", "in possession of wide shoulders that are thickly corded with muscle", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "bull-necked", "muscular", "muscular and hefty, with a thickly-muscled neck disappearing between &his broad shoulders", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "robust", "muscular", "healthy and vigorous, with a richly muscular frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "brawny", "muscular", "built like a brick shithouse, with very little body fat - mostly muscle over a broad frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "big", "fat", "large and thick-framed, with a build that has gone largely to fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "sturdy", "muscular", "sturdily built, with some muscle, some fat, and some solid bones, sinew and joints holding it all together", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "well-built", "normal", "well-built, with a fit frame and well-proportioned arms and legs", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "thickset", "fat", "thickly-built and possessing a squat, sturdy frame on which sits a good amount of flesh", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "hardy", "muscular", "muscular and squat, with a hardy, fit frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "solid", "muscular", "solidly-built, with a compact frame corded with muscle and padded with fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "wide-hipped", "fat", "in possession of wide hips that are amply padded with soft fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "corpulent", "obese", "unmistakeably large and obese, with an excess of body fat hanging in rolls and fleshy ribbons", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "hefty", "fat", "in possession of a large frame that carries a fair amount of muscle and fat on it, though there's more fat than muscle", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "heavy", "fat", "carrying a good deal of body fat, muscle and other weight on &his heavyset frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "big-boned", "fat", "large, with broad shoulders and a solid amount of fat and some muscle cording their big-boned frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "husky", "fat", "rather plump, though the broadness of &his frame hides the full extent of &his fleshiness", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "stocky", "fat", "squat and compact, with a frame that carries its excess fat well enough", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "plump", "fat", "rather plump - not exactly fat, but soft and at least moderately overweight", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "meaty", "fat", "large and chunky, with a frame that is a meaty, marbled mix of fat and muscle", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "dumpy", "obese", "horrifically large and utterly fat, with all &his excess fat hanging off &his dumpy frame", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "chunky", "fat", "rather chunky, with some excess fat on &his frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "paunchy", "fat", "in possession of a large, protruding abdomen", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "ample", "fat", "quite large and impressive - impressively chubby, at least", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "beer-bellied", "fat", "in possession of a large, prominent abdomen that balloons out - commonly referred to a 'beer belly'", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "podgy", "fat", "soft and fat, with a short frame that encourages loose folds of chunky skin to wrinkle and roll over on themselves", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "portly", "fat", "squat and round, carrying their excess weight awkwardly on their frame", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "rotund", "fat", "quite rotund in frame and build, with a large gut and rounded shoulders", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "muffin-topped", "fat", "shaped like a muffin - with the excess fat spilling out in rolls around the abdomen, much like a muffin's top", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "badonkadonked", "obese", "in possession of a large, round, firm posterior that watermelons out from the back", obeseProg);
		AddCharacteristicValue(nextId++, frameDef, "waifish", "emaciated", "built waifishly - &he is very thin and fragile seeming, with delicate features", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "brutish", "fat", "hulking and large, with a hefty, stooped frame corded thickly with muscle and flesh", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "thick-shouldered", "muscular", "in possession of meaty shoulders that are thickly corded with muscle", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "monstrous", "monstrous", "a monstrously large lump of humanity, of bizarre, malformed proportions, with an oversized head, a barrel of a torso, and stumpy limbs ending in even more oversized hands and somewhat stubby feet, &his massive frame laden thickly with strong muscle and rolls of fat", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "compact", "normal", "a neat, spare look to &his body, compact and well put-together", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "statuesque", "muscular", "in possession of a sculpted, aesthetic look about &his form, as though designed by an artist", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "coltish", "thin", "a lanky look to &him, limbs awkwardly long and gangly", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "hulking", "fat", "a ponderous person of substantial width and volume principally caused by &his excessively robust bone structure and supplemented by ample softer tissues", overweightProg);

		AddCharacteristicValue(nextId++, frameDef, "wiry", "lean", "in possession of a wiry, sinewy frame, built for agility", normalWeightProg);
		AddCharacteristicValue(nextId++, frameDef, "spindly", "thin", "spindly and lanky, with elongated, fragile limbs", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "imposing", "fat", "an imposing, almost unbelievably large figure, built to intimidate", overweightProg);
		AddCharacteristicValue(nextId++, frameDef, "rangy", "thin", "rangy and long-limbed, with a rugged build", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "stooped", "thin", "a thin, stooped posture that lends an air of frailty", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "reed-thin", "thin", "reed-thin, with an elongated and delicate appearance", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "trim", "thin", "trim and sleek, with a fit and elegant figure", underweightProg);
		AddCharacteristicValue(nextId++, frameDef, "chiseled", "muscular", "chiseled and sharply defined, like &he has been carved out of stone", overweightProg);

		_context.SaveChanges();
		AddCharacteristicValue(nextId++, hairColourDef, "blonde", "201", "");
		AddCharacteristicValue(nextId++, hairColourDef, "dirty blonde", "202", "");
		AddCharacteristicValue(nextId++, hairColourDef, "silver blonde", "203", "");
		AddCharacteristicValue(nextId++, hairColourDef, "ash blonde", "204", "");
		AddCharacteristicValue(nextId++, hairColourDef, "strawberry blonde", "205", "");
		AddCharacteristicValue(nextId++, hairColourDef, "platinum blonde", "206", "");
		AddCharacteristicValue(nextId++, hairColourDef, "light blonde", "207", "");
		AddCharacteristicValue(nextId++, hairColourDef, "salt-and-pepper", "208", "");
		AddCharacteristicValue(nextId++, hairColourDef, "dark", "221", "");
		_context.SaveChanges();

		AddStyleableCharacteristic(nextId++, hairStyleDef, "afro", "afro-haired", "a frizzy halo of natural hair", 4, Difficulty.VeryEasy, 0, false, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "beehive", "beehive-haired", "a conical pile of hair in the shape of a beehive", 5, Difficulty.VeryEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bob cut", "bob-cut", "bangs and an even bob cut", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bouffant", "bouffant-styled", "a puffed out, raised section of hair atop the head which hangs down on the sides", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bowl cut", "bowl-cut", "an all-around even haircut", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "braid", "braided", "hair woven into a tight braid", 4, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bun", "bun-haired", "hair tied up into a bun atop the head", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "buzz cut", "buzz-cut", "closely cropped hair", 1, Difficulty.Automatic, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "chignon", "chignon-haired", "hair tied into a chingon bun", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "chonmage", "chonmage-haired", "a shaved pate, with longer back and sides", 4, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "combover", "comb-overed", "thin hair combed over a bald spot", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "cornrows", "corn-rowed", "hair tied into thin, parallel rows of braids", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "crew cut", "crew-cut", "hair closely cropped but tapering longest to shortest from front to back", 1, Difficulty.Automatic, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "cropped cut", "crop-haired", "an all-around even haircut", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "curtain cut", "curtain-haired", "hair parted neatly in the center", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "dreadlocks", "dreadlocked", "combed into locked dreads", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "liberty spikes", "liberty-spiked", "clean, distinct spikes of hair", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bald", "bald", "no hair at all", 0, Difficulty.Automatic, 6, isDefault: true);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "emo cut", "emo-fringed", "medium-length, straight hair combed over one eye", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "fauxhawk", "fauxhawked", "short hair style toward a center spike", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "feathered hair", "feathery-haired", "long, unlayered hair with a center part, brushed back at the sides", 4, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "fishtail hair", "fishtail-haired", "long hair braided into the shape of a fish's tail", 4, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "flattop", "crewcut", "a short, level-topped crewcut", 1, Difficulty.Automatic, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "layered hair", "layered-haired", "long hair cut unevenly to form layers", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long hair", "long-haired", "hair that is cut long and flows freely", 5, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "mop top", "mop-topped", "a mid-length haircut extending to the collar with fringe bangs that brush the forehead", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "mullet", "mulleted", "hair that is short at the front, but long at the back", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "double buns", "double-bunned", "a pair of buns that jut up like ox-tails from the top of the head", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "sidelocks", "sidelocked", "twin locks of curly hair that hang from both sides of the face", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pigtails", "pigtailed", "hair that parts down the middle and is tied into two pony tails on either side", 4, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pixie cut", "pixie-haired", "a short wispy hairstyle with a shaggy fringe", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pompadour", "pompadoured", "hair swept upwards from the face and worn high over the forehead, upswept at the back and sides", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "ponytail", "ponytailed", "medium-length hair pulled back behind the head and tied in place", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long ponytail", "long-ponytailed", "long hair pulled back behind the head and tied in place", 5, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "rattail", "rattailed", "hair that has been shaved short except for a long braid at the back of the neck", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "ringlets", "ringletted", "hair worn in tight curls", 4, Difficulty.ExtremelyEasy, 4, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "shag hair", "shag-haired", "a choppy, layered hairstyle with fullness at the crown and fringes around the edges", 3, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "short hair", "short-haired", "hair that is cut short", 2, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "spiky hair", "spike-haired", "hair that sticks up in spikes on top of the head", 2, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "tonsure", "tonsured", "a narrow ring of short hair surrounding a bald dome", 1, Difficulty.Automatic, 6);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "shoulder-length hair", "shoulder-length-haired", "medium-length hair that falls to the shoulders", 4, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "undercut", "undercut-styled", "hair that is longer directly on top of the head, but shorter everywhere else", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "weaves", "weave-styled", "artificial hair extensions woven into the natural hair to lengthen it", 4, Difficulty.VeryEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "taper cut hair", "taper-cut-styled", "hair of a combable length on the top, a side part, and semi-short back and sides", 2, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "frizzy hair", "frizzy-haired", "medium-length, unmanageable hair with a tendency to frizz", 3, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "wavy hair", "wavy-haired", "thick hair with a natural wave to it", 3, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "thin hair", "thin-haired", "naturally straight, thin hair", 3, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "thick hair", "thick-haired", "naturally full-bodied, thick hair", 3, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "curly hair", "curly-haired", "hair with natural curls", 3, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "norman cut", "norman-cut", "hair that has been shaved at the back of the head and the neck, but allowed to grow on the top front of the head", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "mop hair", "mop-haired", "a mid-length haircut extending to the collar with fringe bangs that brush the forehead", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "stubble hair", "stubble-haired", "a thin, even covering of stubble and regrowth atop &his head", 1, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "short ponytail", "short-ponytailed", "short hair pulled back behind the head into a small ponytail, with barely enough hair to make it work", 3, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "balding", "balding", "a short shaved hairStyleDef with a bald dome, evidently suffering from pattern baldness", 1, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "balding crop", "balding crop-haired", "a mid-length cropped hairstyle with a bald dome, evidently suffering from pattern baldness", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "short afro", "short-afroed", "a short, frizzy halo of hair", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "huge afro", "huge-afroed", "an enormous, frizzy halo of hair, very impractical looking", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "fade cut", "fade-haired", "a short, neatly maintained hairstyle where the hair goes from a combed style on top to a short shave on the back and sides with a seamless transition", 3, Difficulty.ExtremelyEasy, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "mid fade cut", "mid-fade-haired", "a mid-length hairstyle with a wavy combed style on top to a short shaved back and sides in a seamless transition", 3, Difficulty.ExtremelyEasy, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "skin fade", "skin-fade-haired", "a short, neatly maintained hairstyle where the hair goes from a combed style on top to a bare back and sides by way of a seamless transition through a shaved style", 2, Difficulty.ExtremelyEasy, 7);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "short curls", "short-curly-haired", "short hair with natural curls", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "quiff", "quiff-haired", "hair swept forwards from the face, similar to a pompodour but jutting forward from the head", 3, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pompadour mullet", "pompadour-mulleted", "hair that is swept upwards at the front of the face into a pompadour, but long and loose at the back", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "dreadhawk", "dreadhawked", "dreadlocked hair swept back over &his head, and short, shaved hair along the sides", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "fanhawk", "mohawked", "a hairstyle in which both sides of &his head are shaven, leaving a strip of long hair fanned out in the center", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "warhawk", "warhawked", "hair style in a very short mohawk, and shaved along the sides of the skull", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "frohawk", "frohawked", "hair style like a typical afro, only the sides of the skull have been shaved", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "reverse mohawk", "reverse-mohawked", "really, really stupid looking hair: a single strip has been shaved down the middle of &his skull", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "deathhawk", "deathhawked", "voluminous, backcombed hair style into a loose mohawk, with shaved hair along the sides of the skull", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "rathawk", "rathawked", "a typical, fan-style mohawk that ends in a grungy little rattail", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "mohawk", "mohawked", "a hairstyle where only the central strip of hair remains, the sides having been shaved", 4, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "crochet twists", "crochet-twisted", "long coils of hair twisted in rope-like braids", 4, Difficulty.VeryEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "micro braids", "micro-braided", "dozens of uniformly tiny braids covering &his whole head", 4, Difficulty.VeryEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "corkscrew twist curls", "corkscrew-curled", "shoulder length hair which is extremely curly, forming tight corkscrew twists", 4, Difficulty.VeryEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bantu knots", "bantu-knotted", "hair which has been twisted and wrapped to form numerous knobs atop &his head", 4, Difficulty.VeryEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "wedge cut", "wedge-styled", "hair cut short at the nape, layering gradually upward to become longer on top and in the front, creating a wedge-like look", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pixie wedge", "pixie-wedge-cut", "very short hair, shaven at the nape and longer on the top and sides, somewhere between a pixie cut and a wedge", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "pixie bob", "pixie-bob-cut", "hair that is very short in the back, but nearly chin-length in the front, angling to match &his jawline", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "varsity style", "varsity-styled", "vintage style hair that is side-parted and slicked, shorter on the sides and in the back", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "cowlick", "cowlicked", "short, undercut hair slicked into a prominent curl at &his forehead", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "slick  hair", "slick-haired", "short hair that has been slicked back away from &his face", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "side part", "side-parted", "short hair parted along one side and combed sideways across &his head", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "high ponytail", "high-ponytailed", "long hair pulled into a ponytail high on &his head", 4, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "devilock", "devilocked", "a long forelock of hair that rigidly sticks down the front of &his face, sides and back otherwise fairly short", 2, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "wings", "wing-haired", "short, fluffy hair that's been combed to either side of &his face", 2, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "bunches", "bunch-haired", "mid-length hair that's been bunched up into several uneven portions", 3, Difficulty.Automatic, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "jheri curls", "jheri-curled", "a mid-length, dense hairstyle consisting of loose curls, with longer fore-curls", 3, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long jheri curls", "jheri-curled", "a long, dense hairstyle consisting of loose curls, with slightly longer forecurls framing the face", 4, Difficulty.ExtremelyEasy, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "finger wave", "finger-waved", "a short, feminine haircut that has been set in a wedged, wavy style", 3, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "hat hair", "hat-haired", "short, messy hair that looks like &he has had a cap or hat on for an extended period of time", 2, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long braid", "long-braided", "hair woven into a long, tight braid", 5, Difficulty.Trivial, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "large bun", "large-bunned", "hair that has been wrapped up into a large bun", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "twisted bun", "twist-bunned", "hair that has been twisted around into a large bun", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "rope braid", "rope-braided", "a twisted braid that has been styled to look like coiled rope", 5, Difficulty.ExtremelyEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "french braid", "french-braided", "a large, thick braid that starts near the hairline, with a simple alternating overlap pattern", 5, Difficulty.VeryEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "herringbone", "herringbone-braided", "a large, thick braid that starts near the back of the neck and comes together at the back of the head with two flat zones of hair tying it all in", 5, Difficulty.VeryEasy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "crown braid", "crown-braided", "a large, thick braid with a simple alternating overlapping pattern which has been wrapped around &his head like a crown", 5, Difficulty.Easy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "basket weave", "basket-woven", "long hair, natural at the back but woven into a criss-cross basketweave like pattern on top of &his head", 4, Difficulty.Easy, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long wavy hair", "long-wavy-haired", "thick, long hair with a natural wave to it", 4, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long mullet", "long-mulleted", "hair that is short at the front, but very long at the back", 5, Difficulty.Automatic, 0);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "long feathered", "long-feathery-haired", "very long, unlayered hair with a center part, brushed back at the sides", 5, Difficulty.Trivial, 0, pluralisation: 1);
		AddStyleableCharacteristic(nextId++, hairStyleDef, "viking braid", "viking-braided", "a tri-line of braids across &his dome, flowing back to the middle twist intertwined in a low ponytail", 5, Difficulty.ExtremelyEasy, 0);
		_context.SaveChanges();

		var personValues = new List<(CharacteristicValue Value, double Weight)>();

		void AddPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++, Definition = personDef, Name = name, Value = basic, AdditionalValue = "",
				Default = false, Pluralisation = 0, FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			personValues.Add((pw, weight));
		}

		AddPersonWord("baby", "baby", isBabyProg, 3.0);
		AddPersonWord("infant", "baby", isBabyProg, 1.0);
		AddPersonWord("tot", "baby", isBabyProg, 0.5);
		AddPersonWord("newborn", "baby", isBabyProg, 1.0);
		AddPersonWord("babe", "baby", isBabyProg, 0.5);
		AddPersonWord("nursling", "baby", isBabyProg, 0.1);
		AddPersonWord("toddler", "toddler", isToddlerProg, 1.0);
		AddPersonWord("man", "man", isAdultManProg, 10.0);
		AddPersonWord("gent", "man", isAdultManProg, 1.0);
		AddPersonWord("gentleman", "man", isAdultManProg, 1.0);
		AddPersonWord("person", "person", isAdultProg, 1.0);
		AddPersonWord("woman", "woman", isAdultWomanProg, 10.0);
		AddPersonWord("lady", "woman", isAdultWomanProg, 1.0);
		AddPersonWord("lad", "boy", isBoyProg, 4.0);
		AddPersonWord("boy", "boy", isBoyProg, 4.0);
		AddPersonWord("teen boy", "boy", isBoyProg, 1.0);
		AddPersonWord("young boy", "boy", isBoyProg, 1.0);
		AddPersonWord("child", "child", isChildProg, 4.0);
		AddPersonWord("tyke", "child", isChildProg, 0.2);
		AddPersonWord("nipper", "child", isChildProg, 0.2);
		AddPersonWord("urchin", "child", isChildProg, 0.2);
		AddPersonWord("imp", "child", isChildProg, 0.2);
		AddPersonWord("sprout", "child", isChildProg, 0.2);
		AddPersonWord("munchkin", "child", isChildProg, 0.2);
		AddPersonWord("rascal", "child", isChildProg, 0.2);
		AddPersonWord("scamp", "child", isChildProg, 0.2);
		AddPersonWord("youth", "youth", isYouthProg, 5.0);
		AddPersonWord("preteen", "youth", isYouthProg, 1.0);
		AddPersonWord("tween", "youth", isYouthProg, 1.0);
		AddPersonWord("teen", "youth", isYouthProg, 5.0);
		AddPersonWord("tomboy", "youth", isYouthProg, 1.0);
		AddPersonWord("adolescent", "youth", isYouthProg, 2.0);
		AddPersonWord("youngster", "youth", isYouthProg, 1.0);
		AddPersonWord("juvenile", "youth", isYouthProg, 1.0);
		AddPersonWord("waif", "youth", isYouthProg, 1.0);
		AddPersonWord("kid", "youth", isYouthProg, 2.0);
		AddPersonWord("sprite", "youth", isYouthProg, 0.2);
		AddPersonWord("young man", "man", isYoungManProg, 5.0);
		AddPersonWord("youngster", "man", isYoungAdultProg, 1.0);
		AddPersonWord("gaffer", "old man", isOldManProg, 1.0);
		AddPersonWord("lass", "girl", isYoungWomanProg, 1.0);
		AddPersonWord("lassie", "girl", isYoungWomanProg, 0.1);
		AddPersonWord("girl", "girl", isGirlProg, 4.0);
		AddPersonWord("teen girl", "girl", isGirlProg, 1.0);
		AddPersonWord("young girl", "girl", isGirlProg, 5.0);
		AddPersonWord("poppet", "girl", isGirlProg, 0.2);
		AddPersonWord("young woman", "girl", isYoungWomanProg, 5.0);
		AddPersonWord("maiden", "woman", isYoungWomanProg, 1.0);
		AddPersonWord("damsel", "woman", isYoungWomanProg, 1.0);
		AddPersonWord("crone", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("granny", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("old woman", "old woman", isOldWomanProg, 5.0);
		AddPersonWord("elderly woman", "old woman", isOldWomanProg, 5.0);
		AddPersonWord("harridan", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("beldam", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("dowager", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("old man", "old man", isOldManProg, 5.0);
		AddPersonWord("elderly man", "old man", isOldManProg, 3.0);
		AddPersonWord("geezer", "old man", isOldManProg, 1.0);
		AddPersonWord("elder", "old man", isOldManProg, 2.0);
		AddPersonWord("spinster", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("brute", "man", isAdultManProg, 1.0);
		AddPersonWord("guy", "man", isAdultManProg, 1.0);
		AddPersonWord("chap", "man", isAdultManProg, 1.0);
		AddPersonWord("bloke", "man", isAdultManProg, 1.0);
		AddPersonWord("fellow", "man", isAdultManProg, 1.0);
		AddPersonWord("adolescent", "young man", isYoungAdultProg, 5.0);
		AddPersonWord("hag", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("matron", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("codger", "old man", isOldManProg, 1.0);

		if (_questionAnswers["includeextraperson"].EqualToAny("yes", "y"))
		{
			AddPersonWord("beefcake", "man", isAdultManProg, 1.0);
			AddPersonWord("punk", "person", isAdultProg, 1.0);
			AddPersonWord("wretch", "person", isAdultProg, 1.0);
			AddPersonWord("unit", "person", isAdultProg, 1.0);
			AddPersonWord("dude", "person", isAdultProg, 1.0);
			AddPersonWord("frump", "person", isAdultProg, 1.0);
			AddPersonWord("stud", "man", isAdultManProg, 1.0);
			AddPersonWord("hunk", "man", isAdultManProg, 1.0);
			AddPersonWord("specimen", "person", isAdultProg, 1.0);
			AddPersonWord("gal", "woman", isAdultWomanProg, 1.0);
			AddPersonWord("wench", "woman", isAdultWomanProg, 1.0);
			AddPersonWord("diva", "woman", isAdultWomanProg, 1.0);
		}

		_context.SaveChanges();
		_context.CharacteristicProfiles.Add(new CharacteristicProfile
		{
			Name = "Weighted Person Words",
			Definition = new XElement("Values",
				from item in personValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words, weighted so that 'normal' values appear more frequently",
			TargetDefinition = personDef
		});
		_context.SaveChanges();

		AddCharacteristicValue(nextId++, skinColourDef, "white", "fair", "of light, caucasian tone");
		AddCharacteristicValue(nextId++, skinColourDef, "milky-white", "fair", "of milky, pale white tone");
		AddCharacteristicValue(nextId++, skinColourDef, "pale-white", "fair", "of pale white tone");
		AddCharacteristicValue(nextId++, skinColourDef, "pasty-white", "fair", "of a pasty, pale white tone");
		AddCharacteristicValue(nextId++, skinColourDef, "tanned", "fair", "tanned a healthy brown colour");
		AddCharacteristicValue(nextId++, skinColourDef, "olive", "olive", "of an olive complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "oriental", "golden", "of an oriental complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "bronzed", "fair", "of a deep, bronze tone");
		AddCharacteristicValue(nextId++, skinColourDef, "dark-olive", "olive", "of a dark olive complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "light-brown", "brown", "of a light brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "brown", "brown", "of a brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "dark-brown", "brown", "of a dark brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "ebony", "black", "of a deep, ebony complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "black", "black", "of a deep, black complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "pale-olive", "golden", "of a pale olive complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "ruddy", "fair", "of generally white complexion with areas of a ruddy red");
		AddCharacteristicValue(nextId++, skinColourDef, "golden", "golden", "of a golden complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "sallow", "fair", "of an unhealthy pale yellowed complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "copper", "red-brown", "of a coppery brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "caramel", "brown", "of a rich, caramel brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "light-copper", "red-brown", "of a light, coppery brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "dark-copper", "red-brown", "of a dark, coppery brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "light golden", "golden", "of a light golden complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "russet", "red-brown", "of a red-tinged brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "pale", "fair", "that is as pale as milk");
		AddCharacteristicValue(nextId++, skinColourDef, "translucent", "fair", "so pale it's translucent, with a frosted purplish tone");
		AddCharacteristicValue(nextId++, skinColourDef, "dusky", "olive", "that is dark-toned and shaded");
		AddCharacteristicValue(nextId++, skinColourDef, "cinnamon", "red-brown", "that is a warm reddish-brown colour");
		AddCharacteristicValue(nextId++, skinColourDef, "pallid", "fair", "of a pale, unhealthy-looking tone");
		AddCharacteristicValue(nextId++, skinColourDef, "swarthy", "brown", "of a particularly dark hue, with shadowed tones");
		AddCharacteristicValue(nextId++, skinColourDef, "tawny", "golden", "of a deep golden-brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "mahogany", "red-brown", "of a rich red-brown complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "chestnut", "brown", "that is a golden nut-brown colour");
		AddCharacteristicValue(nextId++, skinColourDef, "ashen", "fair", "of a pale ashen-grey complexion");
		AddCharacteristicValue(nextId++, skinColourDef, "obsidian", "black", "that is a deep, rich black in tone");
		AddCharacteristicValue(nextId++, skinColourDef, "jet", "black", "that is a nearly iridescent blue-black colour");
		AddCharacteristicValue(nextId++, skinColourDef, "buttermilk", "golden", "that is a light, delicately yellow-toned shade of pale");
		AddCharacteristicValue(nextId++, skinColourDef, "mocha", "brown", "of a cool-toned brown");
		AddCharacteristicValue(nextId++, skinColourDef, "cocoa", "black", "the rich brown of dark chocolate");
		AddCharacteristicValue(nextId++, skinColourDef, "snow white", "fair", "the colour of white driven snow");
		_context.SaveChanges();

		AddCharacteristicValue(nextId++, noseDef, "small nose", "small-nosed", "a small, refined nose");
		AddCharacteristicValue(nextId++, noseDef, "hook nose", "hook-nosed", "a large, protruding nose that hooks downwards towards the end");
		AddCharacteristicValue(nextId++, noseDef, "straight nose", "straight-nosed", "a medium-sized, straight and symmetrical nose");
		AddCharacteristicValue(nextId++, noseDef, "crooked nose", "crooked-nosed", "a crooked nose that looks like it has been repeatedly broken");
		AddCharacteristicValue(nextId++, noseDef, "button nose", "button-nosed", "a small, cute button nose");
		AddCharacteristicValue(nextId++, noseDef, "aquiline nose", "aquiline-nosed", "a nose with a subtle downwards bend like the beak of an eagle");
		AddCharacteristicValue(nextId++, noseDef, "round nose", "round-nosed", "a medium-sized round nose");
		AddCharacteristicValue(nextId++, noseDef, "large nose", "large-nosed", "a noticably large nose");
		AddCharacteristicValue(nextId++, noseDef, "beak nose", "beak-nosed", "a downward-sloped nose like the beak of a bird");
		AddCharacteristicValue(nextId++, noseDef, "wide nose", "wide-nosed", "a nose with notably wide nostrils");
		AddCharacteristicValue(nextId++, noseDef, "upturned nose", "upturned-nosed", "a nose with a gentle upwards curve");
		AddCharacteristicValue(nextId++, noseDef, "flat nose", "flat-nosed", "a large, flat nose");
		AddCharacteristicValue(nextId++, noseDef, "big nose", "big-nosed", "a very big nose");
		AddCharacteristicValue(nextId++, noseDef, "bent nose", "bent-nosed", "a nose with a notable bend near the top");
		AddCharacteristicValue(nextId++, noseDef, "narrow nose", "narrow-nosed", "a nose with a notably narrow bridge and nostrils");
		AddCharacteristicValue(nextId++, noseDef, "bulbous nose", "bulbous-nosed", "a large, ruddy, bulbous nose");
		AddCharacteristicValue(nextId++, noseDef, "acromegalic nose", "acromegalic-nosed", "a massive lumpy nose resembling a person's fist in proportion");
		AddCharacteristicValue(nextId++, noseDef, "roman nose", "roman-nosed", "a nose with a prominent bridge, slightly convex in profile");
		AddCharacteristicValue(nextId++, noseDef, "snub nose", "snub-nosed", "a nose with a narrow, low bridge, widening and sweeping upward to the tip");
		AddCharacteristicValue(nextId++, noseDef, "long nose", "long-nosed", "a nose which is noticeably long in proportion to &his face");
		AddCharacteristicValue(nextId++, noseDef, "short nose", "short-nosed", "a nose which is noticeably short in proportion to &his face");
		AddCharacteristicValue(nextId++, noseDef, "pig nose", "pig-nosed", "a flattened, up-turned nose with very visible nostrils from any angle");
		AddCharacteristicValue(nextId++, noseDef, "pointed nose", "pointed-nosed", "a sharply pointed nose that tapers to a fine tip");
		AddCharacteristicValue(nextId++, noseDef, "hawk nose", "hawk-nosed", "a sharply curved, angular nose like the beak of a hawk");
		AddCharacteristicValue(nextId++, noseDef, "broad nose", "broad-nosed", "a wide and flat nose with prominent nostrils");
		AddCharacteristicValue(nextId++, noseDef, "dainty nose", "dainty-nosed", "a small, delicate, and finely shaped nose");
		AddCharacteristicValue(nextId++, noseDef, "flared nose", "flared-nosed", "a nose with prominently flared nostrils");
		AddCharacteristicValue(nextId++, noseDef, "recessed nose", "recessed-nosed", "a nose that appears slightly set back into the face");
		AddCharacteristicValue(nextId++, noseDef, "chiseled nose", "chiseled-nosed", "a nose with sharp and well-defined features");
		AddCharacteristicValue(nextId++, noseDef, "asymmetrical nose", "asymmetrical-nosed", "a nose that is noticeably uneven or irregular in shape");
		AddCharacteristicValue(nextId++, noseDef, "stubby nose", "stubby-nosed", "a short, broad nose with a blunt tip");
		AddCharacteristicValue(nextId++, noseDef, "angular nose", "angular-nosed", "a nose with sharp, distinct angles and planes");
		AddCharacteristicValue(nextId++, noseDef, "flattened nose", "flattened-nosed", "a nose that appears pressed down with a broad bridge");
		AddCharacteristicValue(nextId++, noseDef, "drooping nose", "drooping-nosed", "a nose with a tip that droops downwards noticeably");
		AddCharacteristicValue(nextId++, noseDef, "petite nose", "petite-nosed", "a very small and delicate nose, almost doll-like");
		AddCharacteristicValue(nextId++, noseDef, "thick nose", "thick-nosed", "a wide, thickly set nose with a strong presence");
		AddCharacteristicValue(nextId++, noseDef, "ridge nose", "ridge-nosed", "a nose with a pronounced ridge running down its length");
		AddCharacteristicValue(nextId++, noseDef, "cleft nose", "cleft-nosed", "a nose with a distinct groove or indentation down its tip");
		AddCharacteristicValue(nextId++, noseDef, "petite button nose", "petite-button-nosed", "a tiny, delicate, and perfectly round button nose");
		AddCharacteristicValue(nextId++, noseDef, "blunt nose", "blunt-nosed", "a nose with a flat, wide tip that lacks sharp definition");
		AddCharacteristicValue(nextId++, noseDef, "sunken nose", "sunken-nosed", "a nose that appears slightly concave along its bridge");
		AddCharacteristicValue(nextId++, noseDef, "ridge-tipped nose", "ridge-tipped-nosed", "a nose with a slight bump or ridge near its tip");
		AddCharacteristicValue(nextId++, noseDef, "uneven nose", "uneven-nosed", "a nose with subtle irregularities in its shape or symmetry");
		AddCharacteristicValue(nextId++, noseDef, "tapering nose", "tapering-nosed", "a nose that gradually narrows towards its tip");
		AddCharacteristicValue(nextId++, noseDef, "fleshy nose", "fleshy-nosed", "a nose with soft, rounded features and a prominent tip");
		AddCharacteristicValue(nextId++, noseDef, "fine-bridged nose", "fine-bridged-nosed", "a nose with an especially thin and delicate bridge");
		AddCharacteristicValue(nextId++, noseDef, "pronounced-tip nose", "pronounced-tip-nosed", "a nose with a very prominent and distinct tip");

		_context.SaveChanges();

		AddCharacteristicValue(nextId++, earDef, "round", "round-eared", "decidedly round at the top");
		AddCharacteristicValue(nextId++, earDef, "big", "big-eared", "a little bigger than normal");
		AddCharacteristicValue(nextId++, earDef, "small", "small-eared", "a little smaller than normal");
		AddCharacteristicValue(nextId++, earDef, "pointy", "pointy-eared", "shaped like a bit of a point at the top");
		AddCharacteristicValue(nextId++, earDef, "pinched", "pinch-eared", "shaped like something has pinched in the top of the helix");
		AddCharacteristicValue(nextId++, earDef, "cauliflower", "cauliflower-eared", "permanently swollen and engorged from past trauma");
		AddCharacteristicValue(nextId++, earDef, "square", "square-eared", "almost square, with a distinct angular shape");
		AddCharacteristicValue(nextId++, earDef, "long", "long-eared", "a little longer than normal");
		AddCharacteristicValue(nextId++, earDef, "narrow", "narrow-eared", "a little narrower than normal");
		AddCharacteristicValue(nextId++, earDef, "protruding", "protruding-eared", "sticking out from &his head");
		AddCharacteristicValue(nextId++, earDef, "prominent", "prominent-eared", "prominent, sticking out from &his head");
		AddCharacteristicValue(nextId++, earDef, "long-lobed", "long-lobed", "most notable for the long earlobes");
		AddCharacteristicValue(nextId++, earDef, "malformed", "malformed-eared", "malformed, most likely from a birth or childhood defect");
		AddCharacteristicValue(nextId++, earDef, "pointed", "pointed-eared", "ending in a sharper, more pronounced point");
		AddCharacteristicValue(nextId++, earDef, "droopy", "droopy-eared", "hanging downwards slightly from the base");
		AddCharacteristicValue(nextId++, earDef, "curled", "curled-eared", "with the edges of the helix curling inward");
		AddCharacteristicValue(nextId++, earDef, "wide", "wide-eared", "unusually wide, spanning a greater area from base to top");
		AddCharacteristicValue(nextId++, earDef, "asymmetrical", "asymmetrical-eared", "noticeably mismatched in size or shape");
		AddCharacteristicValue(nextId++, earDef, "flat", "flat-eared", "lying unusually close to &his head, with minimal curvature");
		AddCharacteristicValue(nextId++, earDef, "folded", "folded-eared", "folded slightly over at the top");
		AddCharacteristicValue(nextId++, earDef, "flared", "flared-eared", "flaring outward dramatically from &his head");
		AddCharacteristicValue(nextId++, earDef, "thick-lobed", "thick-lobed", "featuring particularly thick and rounded earlobes");
		AddCharacteristicValue(nextId++, earDef, "thin-lobed", "thin-lobed", "with delicate, thin earlobes");
		AddCharacteristicValue(nextId++, earDef, "jagged", "jagged-eared", "with irregular, jagged edges");
		AddCharacteristicValue(nextId++, earDef, "torn", "torn-eared", "showing signs of past damage or tearing");
		AddCharacteristicValue(nextId++, earDef, "clipped", "clipped-eared", "appearing as if the tops have been clipped off");
		AddCharacteristicValue(nextId++, earDef, "folded-lobed", "folded-lobed", "with the lobes folded up towards the ear");
		AddCharacteristicValue(nextId++, earDef, "shell-shaped", "shell-shaped-eared", "resembling the rounded curves of a shell");
		AddCharacteristicValue(nextId++, earDef, "elongated", "elongated-eared", "stretched and elongated further than average");
		AddCharacteristicValue(nextId++, earDef, "angled", "angled-eared", "with a distinctive angle or tilt outward");
		AddCharacteristicValue(nextId++, earDef, "tapered", "tapered-eared", "gradually narrowing towards the tip");
		AddCharacteristicValue(nextId++, earDef, "cupped", "cupped-eared", "deeply curved inward, as if to catch sound");
		AddCharacteristicValue(nextId++, earDef, "uneven", "uneven-eared", "one ear visibly higher or lower than the other");

		_context.SaveChanges();

		#endregion

		#region Distinctive Features

		// Add distinctive features if appropriate
		if (useDistinctive)
		{
			var distinctiveDef = new CharacteristicDefinition
			{
				Name = "Distinctive Feature",
				Type = 3,
				Pattern = "^(distinctive)?feature",
				Description = "A miscellaneous distinctive feature",
				Model = "Standard",
				ChargenDisplayType = 2
			};
			_context.CharacteristicDefinitions.Add(distinctiveDef);
			_context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
				{ Race = humanRace, CharacteristicDefinition = distinctiveDef, Usage = "base" });
			_context.SaveChanges();

			_context.CharacteristicProfiles.Add(new CharacteristicProfile
			{
				Name = "All Distinctive Features",
				TargetDefinition = distinctiveDef,
				Type = "all",
				Description = "All Defined Distinctive Feature Values",
				Definition = "<Definition/>"
			});

			AddCharacteristicValue(nextId++, distinctiveDef, "no lips", "lipless", "no lips at all, only scar tissue and melted skin where lips should be", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "missing left eyebrow", "one-eyebrowed", "only &his left eyebrow remaining");
			AddCharacteristicValue(nextId++, distinctiveDef, "missing right eyebrow", "one-eyebrowed", "only &his right eyebrow remaining");
			AddCharacteristicValue(nextId++, distinctiveDef, "no eyebrows", "eyebrowless", "no eyebrows at all, which is kinda bizarre", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "monobrow", "monobrowed", "a single, thick and bushy monobrow knitting their brows together");
			AddCharacteristicValue(nextId++, distinctiveDef, "beauty mark", "beauty-marked", "a dark beauty spot marring the corner of &his mouth, just below &his nose");
			AddCharacteristicValue(nextId++, distinctiveDef, "hunchback", "hunchbacked", "a hump between &his shoulders hinting at a misshapen spine");
			AddCharacteristicValue(nextId++, distinctiveDef, "clubfoot", "clubfooted", "a deformed foot that appears to have been rotated at the ankle");
			AddCharacteristicValue(nextId++, distinctiveDef, "butt chin", "butt-chinned", "a chin that is so pronounced and jutting that its similarities to a butt cannot be avoided");
			AddCharacteristicValue(nextId++, distinctiveDef, "goiter", "goitered", "an unsightly lump growing on &his neck");
			AddCharacteristicValue(nextId++, distinctiveDef, "buck teeth", "buck-toothed", "two large front teeth that jut out");
			AddCharacteristicValue(nextId++, distinctiveDef, "shattered teeth", "shatter-toothed", "a mouth full of broken, chipped and shattered teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "unblemished complexion", "unblemished", "a body that is seemingly free from blemish or scars");
			AddCharacteristicValue(nextId++, distinctiveDef, "stretch marks", "stretch-marked", "a series of light, off-colour scars that suggest skin that has been stretched", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "pock marks", "pockmarked", "a number of small, textured scars from a pox of some kind", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "acne scars", "acne-scarred", "a number of small, textured scars around the face suggesting a history of acne", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "lantern jaw", "lantern-jawed", "a jutting, strong lower jawline");
			AddCharacteristicValue(nextId++, distinctiveDef, "rough skin", "rough-skinned", "rough, craggy skin", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "smooth skin", "smooth-skinned", "extraordinarily smooth, wrinkle free skin", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "loose skin", "loose-skinned", "loose skin with little elasticity, prone to hanging off &his flesh at the mercy of gravity", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "perfect teeth", "perfect-toothed", "perfect, straight white teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "sausage fingers", "sausage-fingered", "short, fat, chubby little fingers like miniature sausages", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "fat fingers", "fat-fingered", "short, fat fingers", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thin fingers", "thin-fingered", "noticeably gaunt, thin fingers", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "long fingers", "long-fingered", "especially long, skeletally thin fingers", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "harelip", "harelipped", "a cleft lip and palate, leaving a gap at the top of the mouth");
			AddCharacteristicValue(nextId++, distinctiveDef, "unremarkable appearance", "unremarkable", "a distinctly unremarkable appearance, in a way that would be hard to describe and hard to remember");
			AddCharacteristicValue(nextId++, distinctiveDef, "bland appearance", "bland", "a distinctly bland appearance, in a way that would be hard to describe and hard to remember");
			AddCharacteristicValue(nextId++, distinctiveDef, "angular features", "angular-featured", "jutting, angular facial features with high cheekbones and well defined facial lines", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "double chins", "double-chinned", "two chins, one after the other", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thin eyebrows", "thin-browed", "decidedly thin eyebrows", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "cock eyes", "cockeyed", "a cockeyed gaze, with one of the two eyes never quite focusing in the same direction as the other", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "crossed eyes", "cross-eyed", "a gaze that generally causes the eyes to cross over one another", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "facial wrinkles", "wrinkled", "extensive wrinkles in the corners of the eyes and lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "crooked teeth", "crooked-toothed", "crooked, uneven teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "no teeth", "toothless", "no teeth at all, just gums", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "rotten teeth", "rotten-toothed", "horrid-looking rotten black teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "yellow teeth", "yellow-toothed", "extensively stained yellow teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "gold tooth", "gold-toothed", "a tooth made out of gold");
			AddCharacteristicValue(nextId++, distinctiveDef, "jowls", "jowly", "drooping, jowly cheeks", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "weak chin", "weak-chinned", "a weak chin that recedes into the neckline");
			AddCharacteristicValue(nextId++, distinctiveDef, "straight teeth", "straight-teethed", "a set of good, straight teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "saw teeth", "saw-toothed", "a set of angular, jagged teeth like the blades of a saw", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "jagged teeth", "jagged-toothed", "a set of uneven, jagged-looking teeth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "missing tooth", "missing-toothed", "a noticeable gap in &his teeth, where one is missing");
			AddCharacteristicValue(nextId++, distinctiveDef, "chipped tooth", "chip-toothed", "a prominently chipped tooth");
			AddCharacteristicValue(nextId++, distinctiveDef, "off white teeth", "off-white-toothed", "a set of teeth which are decidedly off-white in colour", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thick eyebrows", "thick-browed", "a pair of large, thick eyebrows", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "pointy eyebrows", "pointy-browed", "a pair of pointy eyebrows that almost always look surprised or curious, no matter &his expression", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "chapped lips", "chap-lipped", "lips that are chapped and dry, with noticeable breaks in the skin", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thick lips", "thick-lipped", "a thick, prominent pair of lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "pale lips", "pale-lipped", "a pair of pale lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "dark lips", "dark-lipped", "a pair of lips with a decidedly darker hue than &his skin tone would suggest", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "small lips", "small-lipped", "a pair of small lips, not quite in keeping with the size of &his other facial features", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thin lips", "thin-lipped", "a long pair of thin lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "chiseled jaw", "chisel-jawed", "a strong jaw, as if chiseled out of a block of granite");
			AddCharacteristicValue(nextId++, distinctiveDef, "sultry eyes", "sultry-eyed", "an innately suggestive, sultry look in &his eyes", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "smoldering eyes", "smoldering-eyed", "eyes that smolder with an inner fire and intensity", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "intense gaze", "intense", "a raw intensity that radiates from &his gaze", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "handsome appearance", "handsome", "an innate attractiveness - all &his features just seem to mesh in such a way as to be ridiculously good-looking");
			AddCharacteristicValue(nextId++, distinctiveDef, "attractive features", "attractive", "an overall, undeniable attractiveness to their appearance; an indefinable something", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "pouty lips", "pouty-lipped", "full, lusciously plump lips shaped like a heart", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "rosy cheeks", "rosy-cheeked", "small, cherubic cheeks with a noticeable rosy glow", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "palsy", "palsied", "a face paralyzed on one side, complete with drooping eyelid and slack mouth", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "cataracts", "cloudy-lensed", "one eye blinded by milky white cataracts", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "wall eye", "wall-eyed", "misaligned eyes, one drifting outward, seeming to be watching that side");
			AddCharacteristicValue(nextId++, distinctiveDef, "bull neck", "bull-necked", "an exceedingly thick and muscular neck");
			AddCharacteristicValue(nextId++, distinctiveDef, "no neck", "no-necked", "almost no neck at all, &his head seeming to meld straight into &his shoulders", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "underbite", "underbiting", "a severe underbite, &his lower jaw outthrust beyond the upper");
			AddCharacteristicValue(nextId++, distinctiveDef, "overbite", "parrot-mouthed", "a really noticeable overbite, &his lower jaw set well back from the upper");
			AddCharacteristicValue(nextId++, distinctiveDef, "bushy eyebrows", "bushy-browed", "extremely prominent, bushy eyebrows like two caterpillars sitting above &his eyes", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "resting bitch face", "sneering", "a seemingly permanent sneer, &his upper lip drawn back and brows lowered");
			AddCharacteristicValue(nextId++, distinctiveDef, "haggard appearance", "haggard", "a worn, world-weary look to &him that suggests a life of hardship and deprivation");
			AddCharacteristicValue(nextId++, distinctiveDef, "freckles", "freckled", "a smattering of freckles across &his nose and cheekbones", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "extreme freckles", "heavily-freckled", "copious amounts of freckles over &his whole body", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "chin beauty mark", "beauty-marked", "a prominent mole located on &his chin");
			AddCharacteristicValue(nextId++, distinctiveDef, "gold teeth", "golden-toothed", "several golden teeth, obviously having been replaced at some point, giving &him a shiny smile", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "large forehead", "large-foreheaded", "an unusually large expanse of forehead between &his hairline and brows");
			AddCharacteristicValue(nextId++, distinctiveDef, "cheek beauty mark", "beauty-marked", "a prominent mole located on &his cheek");
			AddCharacteristicValue(nextId++, distinctiveDef, "malformed face", "malformed-faced", "a craggy, bumpy face, a bulging chin and jaw, gapped teeth, and almost no neck, &his head seeming to meld straight into &his shoulders");
			AddCharacteristicValue(nextId++, distinctiveDef, "regal bearing", "regal", "an aura of authority about &him, excellent posture and a lofty gaze contributing to a decidedly regal look");
			AddCharacteristicValue(nextId++, distinctiveDef, "imposing presence", "imposing", "a posture and bearing that suggests power ready to be wielded, imposing and perhaps unnerving");
			AddCharacteristicValue(nextId++, distinctiveDef, "air of elegance", "elegant", "a classically elegant appearance and features combining to impart a sense of refinement and grace");
			AddCharacteristicValue(nextId++, distinctiveDef, "refined features", "refined", "refined, delicately sculpted features");
			AddCharacteristicValue(nextId++, distinctiveDef, "august presence", "august", "a naturally composed, dignified aura about &his bearing and features");
			AddCharacteristicValue(nextId++, distinctiveDef, "striking appearance", "striking", "an intangible something that draws the gaze to &him, unusual and compelling");
			AddCharacteristicValue(nextId++, distinctiveDef, "hawkish look", "hawkish", "a singularly intense look about &him, attention nearly always sharply focused");
			AddCharacteristicValue(nextId++, distinctiveDef, "face like a toad", "toad-faced", "a face that resembles nothing so much as a toad, broad and flat, eyes widely spaced and almost facing opposite directions");
			AddCharacteristicValue(nextId++, distinctiveDef, "pug face", "pug-faced", "a round, heavily flattened face, with bulging eyes and a wide mouth");
			AddCharacteristicValue(nextId++, distinctiveDef, "swan neck", "swan-necked", "a long, graceful neck");
			AddCharacteristicValue(nextId++, distinctiveDef, "face like a hatchet", "hatchet-faced", "a thin, sharp-edged face, angular and protruding like a hatchet");
			AddCharacteristicValue(nextId++, distinctiveDef, "owlish gaze", "owlish", "a wide-eyed appearance, as though &he just woke up from a nap, or needs to open &his eyes extra-wide to take in what &he can see");
			AddCharacteristicValue(nextId++, distinctiveDef, "wolfish look", "wolfish", "something predatory about &him, hungry and feral");
			AddCharacteristicValue(nextId++, distinctiveDef, "austere mein", "austere", "a cold, stern look about &his features");
			AddCharacteristicValue(nextId++, distinctiveDef, "horse face", "horse-faced", "a long, narrow face that evokes the image of a horse");
			AddCharacteristicValue(nextId++, distinctiveDef, "hamburger face", "hamburger-faced", "a face that has suffered a heck of a lot of abuse, and now resembles a bunch of ground up raw meat mashed into a vaguely human shape through all the scars and badly reset bones");
			AddCharacteristicValue(nextId++, distinctiveDef, "low brow", "low-browed", "a low set brow with a noticeable bone ridge reminsicent of a neanderthal");
			AddCharacteristicValue(nextId++, distinctiveDef, "winsome appearance", "winsome", "an undeniably innocent aura of attractiveness about &his person");
			AddCharacteristicValue(nextId++, distinctiveDef, "pretty appearance", "pretty", "a better than average but not world-beating level of attractiveness; &he is definitely pretty");
			AddCharacteristicValue(nextId++, distinctiveDef, "comely appearance", "comely", "an agreeable, pleasant to look upon overall appearance");
			AddCharacteristicValue(nextId++, distinctiveDef, "good appearance", "good-looking", "an overall appearance that is objectively good looking; better than average at least");
			AddCharacteristicValue(nextId++, distinctiveDef, "extensive stretchmarks", "stretchmarked", "has countless light, off-colour scars that suggest skin that has been stretched like it were a size too small for &his body", pluralisation: 1);

			AddCharacteristicValue(nextId++, distinctiveDef, "broad jaw", "broad-jawed", "a strong and broad jawline");
			AddCharacteristicValue(nextId++, distinctiveDef, "birthmarked", "birthmarked", "a striking and obvious birthmark");
			AddCharacteristicValue(nextId++, distinctiveDef, "calloused hands", "calloused-handed", "heavily calloused hands, evidence of a lifetime of manual labour", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "dimples", "dimpled", "prominent dimples when &he smiles", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "elegant fingers", "elegant fingered", "long and finely shaped fingers, elegant in appearance", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "light freckles", "lightly-freckled", "a scattering of light freckles, just enough to cover &his cheeks", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "full lips", "full-lipped", "full and striking lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "thin lips", "thin-lipped", "a mouth delicately framed with thin lips", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "gaunt cheeks", "gaunt-cheeked", "cheeks sunken and gaunt, hinting at hardship", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "high cheekbones", "high-cheekboned", "sharply defined by high and prominent cheekbones", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "knobby knuckles", "knobby-knuckled", "hands defined by prominently knobby knuckles", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "square chin", "square-chinned", "an emphatically square chin, prominently jutting out of &his jaw");
			AddCharacteristicValue(nextId++, distinctiveDef, "square jaw", "square-jawed", "a square jaw, giving &his face a wide appearance");
			AddCharacteristicValue(nextId++, distinctiveDef, "strong hands", "strong-handed", "large and powerful-looking hands, suggesting strength", pluralisation: 1);
			AddCharacteristicValue(nextId++, distinctiveDef, "sun weathered", "sun-weathered", "skin toughened and weathered by long days under the sun");
		}

		#endregion

		_context.SaveChanges();
	}
}