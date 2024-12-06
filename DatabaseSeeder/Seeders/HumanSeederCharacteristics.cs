using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Models;

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

		var nextId = _context.CharacteristicValues.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "round", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "almond", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "slanted", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "beady", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "narrow", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "teardrop", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "sunken", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "droopy", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "hooded", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "close-set", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "deep-set", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "hollow", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "puffy", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "doe-like", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "small", Value = "", AdditionalValue = "", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "protruding", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "monolid", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "heavy-lidded", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "prominent", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "upturned", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "asymmetrical", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "downturned", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = eyeShapeDef, Name = "wide-set", Value = "", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.SaveChanges();
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "clean shave", Value = "clean-shaven",
			AdditionalValue = "0 0 6 a clean chin with no hair at all", Default = true, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "handlebar moustache",
			Value = "handlebar-moustached",
			AdditionalValue = "3 2 0 a moustache with lengthy and upwardly curved extremities", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "moustache", Value = "moustached",
			AdditionalValue = "2 0 0 a naturally-styled moustache", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "goatee", Value = "goateed",
			AdditionalValue = "3 1 0 a naturally-styled moustache and adjoining chin beard", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "beard", Value = "bearded",
			AdditionalValue = "4 0 0 a full face beard", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "muttonchops", Value = "muttonchopped",
			AdditionalValue = "3 0 0 sideburns extending well onto the jawline", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "sideburns", Value = "sideburned",
			AdditionalValue = "2 0 0 twin patches of natural hair from the scalp to below the ear, with a clean chin",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "curly sideburns", Value = "curly-sideburned",
			AdditionalValue =
				"2 0 0 twin patches of mid-length curly hair, from the scalp to below the ear, with a clean chin",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "full beard", Value = "full-bearded",
			AdditionalValue =
				"4 0 0 a full, thick beard displaying unmodified growth on the face, neck, chin, cheeks and sideburns.",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "chin curtain", Value = "curtain-bearded",
			AdditionalValue =
				"3 0 0 a short beard running along the jaw line, visibly hanging below the jaw as a curtain hangs from a rod",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "chinstrap", Value = "chinstrapped",
			AdditionalValue =
				"4 0 0 a pair of sideburns, connected by a short, narrow beard along the bottom of the jaw",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "friendly muttonchops", Value = "muttonchopped",
			AdditionalValue =
				"4 0 0 a pair of naturally styled muttonchops, connected by a moustache but with a clean chin",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "fu manchu", Value = "fu-manchued",
			AdditionalValue =
				"3 2 0 a thin, very narrow moustache that grows downward in two very long tendrils from the upper lip.",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "goat patch", Value = "goat-patched",
			AdditionalValue = "3 0 0 a narrow strip of hair growing only on the chin, resembling the beard of a goat",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "german goatee", Value = "goateed",
			AdditionalValue =
				"3 1 0 a neatly trimmed goatee which wraps around the mouth, but whose moustache flares outward past the connecting chin lines",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "horseshoe moustache",
			Value = "horseshoe-moustached",
			AdditionalValue =
				"3 2 0 a full moustache with ends that extend down in parallel straight lines, with a clean shaven chin",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "sidewhiskers", Value = "sidewhiskered",
			AdditionalValue =
				"4 0 0 a pair of muttonchops and connecting moustache, where the portions connecting the sideburns and moustache hang several inches below the chin",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "neckbeard", Value = "neckbearded",
			AdditionalValue = "4 0 0 a beard that grows only on the neck and under the jaw", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "pencil moustache", Value = "pencil-moustached",
			AdditionalValue = "2 0 0 a very thin, very short moustache sitting just above the lip", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "shenandoah", Value = "shenandoahed",
			AdditionalValue =
				"4 0 0 a combination of a chin curtain and neckbeard, with full, unmodified growth on the neck, lower jawline, sideburns, but a notably hair free lip",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "soul patch", Value = "soul-patched",
			AdditionalValue = "2 0 0 a small, single patch of hair just below the lip", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "stubble", Value = "stubbly",
			AdditionalValue = "1 0 0 a short growth of stubble all over &his jaw", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "coarse stubble", Value = "coarse-stubbled",
			AdditionalValue = "2 0 0 a coarse, prickly growth of stubble all over &his jaw", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "toothbrush moustache",
			Value = "toothbrush-moustached",
			AdditionalValue = "2 0 0 a narrow but tall moustache that does not extend beyond the side of the nose",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "french beard", Value = "french-bearded",
			AdditionalValue = "3 1 0 a long, pointy goatee where the chin beard and moustache do not touch",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "walrus moustache", Value = "walrus-moustached",
			AdditionalValue = "3 1 0 a thick, bushy growth of whiskers on the lip like the moustache of a walrus",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "beard", Value = "barbate",
			AdditionalValue = "4 0 0 a thick and full beard", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "long beard", Value = "long-bearded",
			AdditionalValue = "5 0 0 a very long, very thick full face beard", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "patchy beard", Value = "patchy-bearded",
			AdditionalValue = "3 0 0 a thin, patchy beard", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "ducktail beard", Value = "ducktail-bearded",
			AdditionalValue =
				"5 2 0 a full-faced beard and moustache, with a mid-length portion hanging below the chin and tapering to a point",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "french fork", Value = "french-forked",
			AdditionalValue =
				"5 2 0 a full-faced beard and moustache, with a mid-length portion hanging down over the chin and styled into two distinct forks",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "chevron moustache", Value = "chevron-moustached",
			AdditionalValue =
				"3 1 0 a neatly trimmed moustache extending little beyond the edges of the nose and cleanly tapered into thick points",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "lampshade moustache",
			Value = "lampshade-moustached",
			AdditionalValue =
				"3 2 0 a short, trimmed moustache extending only to the edges of the nose and forming a trapezoidal shape, like the profile view of a lampshade",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "petite handlebar", Value = "petite-handlebared",
			AdditionalValue =
				"2 2 0 a short, sharp moustache which curves up at the edges, just past the edge of the nostrils",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "short beard", Value = "short-bearded",
			AdditionalValue =
				"3 0 0 a short full-face beard, in a transitional period between stubble and a real beard",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "pube beard", Value = "pube-bearded",
			AdditionalValue = "2 0 0 a nasty, patchy short beard that looks like pubic hair growing on &his chin",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = facialHairStyleDef, Name = "mange beard", Value = "mangey-bearded",
			AdditionalValue =
				"3 0 0 a pathetic bit of facial hair growth. &his beard has bare patches that sort of resemble the early onset of mange",
			Default = false, Pluralisation = 0
		});
		_context.SaveChanges();
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "emaciated", Value = "emaciated",
			AdditionalValue = "abnormally thin and emaciated", Default = false, Pluralisation = 0,
			FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "gaunt", Value = "emaciated",
			AdditionalValue = "lean and haggard, looking gaunt", Default = false, Pluralisation = 0,
			FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "lean", Value = "thin",
			AdditionalValue = "of a lean, thin build", Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "very thin", Value = "emaciated",
			AdditionalValue = "very thin frame, with not an ounce of fat", Default = false, Pluralisation = 0,
			FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "thin", Value = "thin",
			AdditionalValue = "tends towards the thin side of a healthy frame", Default = false, Pluralisation = 0,
			FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "skinny", Value = "thin",
			AdditionalValue = "of a very skinny build", Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "well proportioned", Value = "normal",
			AdditionalValue = "of an extremely fit and healthy build, being well proportioned in all respects",
			Default = false, Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "muscular", Value = "muscular",
			AdditionalValue = "extremely fit looking, with well defined musculature", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "ripped", Value = "muscular",
			AdditionalValue = "in every respect ripped, with showy, well-developed muscles", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "bulky", Value = "muscular",
			AdditionalValue = "bulky but muscular looking", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "normal", Value = "normal",
			AdditionalValue =
				"of a perfectly ordinary build, with little excess fat or muscle and ordinary proportions",
			Default = false, Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "flabby", Value = "fat",
			AdditionalValue = "flabby, with various pockets of loose, dangling skin", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "chubby", Value = "fat",
			AdditionalValue = "a little heavier than normal, with chubby features and a small amount of excess fat",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "voluptuous", Value = "fat",
			AdditionalValue = "large, but well proportioned", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "curvaceous", Value = "fat",
			AdditionalValue = "shapely and of ample proportion, with well defined curves", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "curvy", Value = "fat",
			AdditionalValue = "shapely and of ample proportion, with well defined curves", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "fat", Value = "fat",
			AdditionalValue = "unambiguously fat, being too heavy for their frame", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "overweight", Value = "fat",
			AdditionalValue = "a little overweight, with plenty of excess body fat", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "pudgy", Value = "fat",
			AdditionalValue = "fat in the face, or pudgy", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "obese", Value = "obese",
			AdditionalValue = "grossly overweight, with significant extra fat all over the body", Default = false,
			Pluralisation = 0, FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "very fat", Value = "obese",
			AdditionalValue = "very fat, in the obese range", Default = false, Pluralisation = 0, FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "morbidly obese", Value = "obese",
			AdditionalValue = "belarded by great, hanging sacks of fat", Default = false, Pluralisation = 0,
			FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "pot bellied", Value = "obese",
			AdditionalValue = "in possession of an enormous, round pot belly", Default = false, Pluralisation = 0,
			FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "barrel chested", Value = "fat",
			AdditionalValue = "in possession of a huge barrel chest, on an otherwise large but muscular frame",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "skeletal", Value = "emaciated",
			AdditionalValue = "nothing more than skin and bones - at least by appearances, a walking skeleton",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "ropy", Value = "thin",
			AdditionalValue =
				"whipcord thin, with all the appearance of knotted rope where muscle meets joint and bone",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "rawboned", Value = "thin",
			AdditionalValue =
				"gaunt enough that &his bones are painfully obvious, jutting in sharp angles beneath the skin",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "lanky", Value = "thin",
			AdditionalValue = "lean enough to appear long-limbed, with an ungraceful, stork-like appearance",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "bony", Value = "emaciated",
			AdditionalValue =
				"bony, with &his skin stretched tightly over &his ribcage, possessing a jutting collarbone and pronounced cheekbones",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "bantam", Value = "thin",
			AdditionalValue = "small, though with a certain chaotic vigor that belies that tiny frame", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "little", Value = "thin",
			AdditionalValue = "undersized and of below average frame", Default = false, Pluralisation = 0,
			FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "petite", Value = "thin",
			AdditionalValue = "rather small, with a thin enough build to be considered dainty", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "scrawny", Value = "emaciated",
			AdditionalValue = "thinly-framed enough that &his ribs are clearly defined beneath the skin of &his chest",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "sinewy", Value = "thin",
			AdditionalValue =
				"thin and lean, with sinewy muscle that is sharply pronounced beneath their skin, wound tightly around their bird-like bones",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "slender", Value = "thin",
			AdditionalValue = "as slim and thin as a tree sapling, with longish limbs and little body fat",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "scrappy", Value = "thin",
			AdditionalValue = "rough and scrawny, with an underfed appearance, bony hands, and knobby knees",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "slim", Value = "thin",
			AdditionalValue = "slimly built, with little to no excess fat on &his frame", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "willowy", Value = "thin",
			AdditionalValue = "in possession of a willowy frame that carries no unnecessary fat", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "gangly", Value = "emaciated",
			AdditionalValue =
				"gaunt and underfed, with &his bones protruding from beneath &his skin, all joints and odd angles",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "lithe", Value = "thin",
			AdditionalValue = "sleek and slim, with a graceful, cat-like appearance to &his frame", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "delicate", Value = "thin",
			AdditionalValue =
				"of a seemingly fragile frame: thin, undersized, and delicately boned, with many fine features",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "lissome", Value = "thin",
			AdditionalValue = "slim and soft-fleshed, with no hard muscle definition, but no sharp gauntness either",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "spry", Value = "thin",
			AdditionalValue = "small and thin, but with a startling vitality to the remaining fleshiness of &his frame",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "supple", Value = "thin",
			AdditionalValue = "slim and soft - thin without being gaunt, while retaining no unnecessary body fat",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "lithesome", Value = "thin",
			AdditionalValue = "slim and soft-fleshed, with no hard muscle definition, but no sharp gauntness either",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "average", Value = "normal",
			AdditionalValue = "of an ordinary build - not particularly muscular, but not overly gaunt", Default = false,
			Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "athletic", Value = "normal",
			AdditionalValue = "fit and trim, with an athletic build, toned calves, and a flat abdomen", Default = false,
			Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "fit", Value = "normal",
			AdditionalValue = "fit and trim, with toned muscles and a flat abdomen", Default = false, Pluralisation = 0,
			FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "strapping", Value = "normal",
			AdditionalValue =
				"quite healthy by all appearances, displaying some muscle definition - but not enough to be classified as 'bulky'",
			Default = false, Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "vigorous", Value = "normal",
			AdditionalValue =
				"quite healthy by all appearances, with a fit build and a frame that carries no unnecessary body fat",
			Default = false, Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "typical", Value = "normal",
			AdditionalValue = "of a fairly typical build for one of &his race", Default = false, Pluralisation = 0,
			FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "rugged", Value = "muscular",
			AdditionalValue = "quite muscular and fit, with a hard ruggedness to &his frame", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "burly", Value = "muscular",
			AdditionalValue =
				"broad and squat, with a thickly muscular frame that pays special mind to &his arms and shoulders",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "stout", Value = "muscular",
			AdditionalValue = "squat but muscular, with very little in the way of curves or softness", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "heavyset", Value = "muscular",
			AdditionalValue = "squat but muscular, wearing &his bulkiness well on &his heavyset frame", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "thick-limbed", Value = "muscular",
			AdditionalValue = "in possession of limbs that are thickly corded with muscle, skin and some fat",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "beefy", Value = "fat",
			AdditionalValue = "large, muscular and broad, with wide shoulders and a barrel chest", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "broad-shouldered", Value = "muscular",
			AdditionalValue = "in possession of wide shoulders that are thickly corded with muscle", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "bull-necked", Value = "muscular",
			AdditionalValue =
				"muscular and hefty, with a thickly-muscled neck disappearing between &his broad shoulders",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "robust", Value = "muscular",
			AdditionalValue = "healthy and vigorous, with a richly muscular frame", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "brawny", Value = "muscular",
			AdditionalValue =
				"built like a brick shithouse, with very little body fat - mostly muscle over a broad frame",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "big", Value = "fat",
			AdditionalValue = "large and thick-framed, with a build that has gone largely to fat", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "sturdy", Value = "muscular",
			AdditionalValue =
				"sturdily built, with some muscle, some fat, and some solid bones, sinew and joints holding it all together",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "well-built", Value = "normal",
			AdditionalValue = "well-built, with a fit frame and well-proportioned arms and legs", Default = false,
			Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "thickset", Value = "fat",
			AdditionalValue = "thickly-built and possessing a squat, sturdy frame on which sits a good amount of flesh",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "hardy", Value = "muscular",
			AdditionalValue = "muscular and squat, with a hardy, fit frame", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "solid", Value = "muscular",
			AdditionalValue = "solidly-built, with a compact frame corded with muscle and padded with fat",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "wide-hipped", Value = "fat",
			AdditionalValue = "in possession of wide hips that are amply padded with soft fat", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "corpulent", Value = "obese",
			AdditionalValue =
				"unmistakeably large and obese, with an excess of body fat hanging in rolls and fleshy ribbons",
			Default = false, Pluralisation = 0, FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "hefty", Value = "fat",
			AdditionalValue =
				"in possession of a large frame that carries a fair amount of muscle and fat on it, though there's more fat than muscle",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "heavy", Value = "fat",
			AdditionalValue = "carrying a good deal of body fat, muscle and other weight on &his heavyset frame",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "big-boned", Value = "fat",
			AdditionalValue =
				"large, with broad shoulders and a solid amount of fat and some muscle cording their big-boned frame",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "husky", Value = "fat",
			AdditionalValue =
				"rather plump, though the broadness of &his frame hides the full extent of &his fleshiness",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "stocky", Value = "fat",
			AdditionalValue = "squat and compact, with a frame that carries its excess fat well enough",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "plump", Value = "fat",
			AdditionalValue = "rather plump - not exactly fat, but soft and at least moderately overweight",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "meaty", Value = "fat",
			AdditionalValue = "large and chunky, with a frame that is a meaty, marbled mix of fat and muscle",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "dumpy", Value = "obese",
			AdditionalValue =
				"horrifically large and utterly fat, with all &his excess fat hanging off &his dumpy frame",
			Default = false, Pluralisation = 0, FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "chunky", Value = "fat",
			AdditionalValue = "rather chunky, with some excess fat on &his frame", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "paunchy", Value = "fat",
			AdditionalValue = "in possession of a large, protruding abdomen", Default = false, Pluralisation = 0,
			FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "ample", Value = "fat",
			AdditionalValue = "quite large and impressive - impressively chubby, at least", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "beer-bellied", Value = "fat",
			AdditionalValue =
				"in possession of a large, prominent abdomen that balloons out - commonly referred to a 'beer belly'",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "podgy", Value = "fat",
			AdditionalValue =
				"soft and fat, with a short frame that encourages loose folds of chunky skin to wrinkle and roll over on themselves",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "portly", Value = "fat",
			AdditionalValue = "squat and round, carrying their excess weight awkwardly on their frame", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "rotund", Value = "fat",
			AdditionalValue = "quite rotund in frame and build, with a large gut and rounded shoulders",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "muffin-topped", Value = "fat",
			AdditionalValue =
				"shaped like a muffin - with the excess fat spilling out in rolls around the abdomen, much like a muffin's top",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "badonkadonked", Value = "obese",
			AdditionalValue = "in possession of a large, round, firm posterior that watermelons out from the back",
			Default = false, Pluralisation = 0, FutureProg = obeseProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "waifish", Value = "emaciated",
			AdditionalValue = "built waifishly - &he is very thin and fragile seeming, with delicate features",
			Default = false, Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "brutish", Value = "fat",
			AdditionalValue = "hulking and large, with a hefty, stooped frame corded thickly with muscle and flesh",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "thick-shouldered", Value = "muscular",
			AdditionalValue = "in possession of meaty shoulders that are thickly corded with muscle", Default = false,
			Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "monstrous", Value = "monstrous",
			AdditionalValue =
				"a monstrously large lump of humanity, of bizarre, malformed proportions, with an oversized head, a barrel of a torso, and stumpy limbs ending in even more oversized hands and somewhat stubby feet, &his massive frame laden thickly with strong muscle and rolls of fat",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "compact", Value = "normal",
			AdditionalValue = "a neat, spare look to &his body, compact and well put-together", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "statuesque", Value = "muscular",
			AdditionalValue =
				"in possession of a sculpted, aesthetic look about &his form, as though designed by an artist",
			Default = false, Pluralisation = 0, FutureProg = normalWeightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "coltish", Value = "thin",
			AdditionalValue = "a lanky look to &him, limbs awkwardly long and gangly", Default = false,
			Pluralisation = 0, FutureProg = underweightProg
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = frameDef, Name = "hulking", Value = "fat",
			AdditionalValue =
				"a ponderous person of substantial width and volume principally caused by &his excessively robust bone structure and supplemented by ample softer tissues",
			Default = false, Pluralisation = 0, FutureProg = overweightProg
		});
		_context.SaveChanges();
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "blonde", Value = "201", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "dirty blonde", Value = "202", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "silver blonde", Value = "203", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "ash blonde", Value = "204", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "strawberry blonde", Value = "205", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "platinum blonde", Value = "206", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "light blonde", Value = "207", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "salt-and-pepper", Value = "208", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairColourDef, Name = "dark", Value = "221", AdditionalValue = "",
			Default = false, Pluralisation = 0
		});
		_context.SaveChanges();
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "afro", Value = "afro-haired",
			AdditionalValue = "4 3 0 a frizzy halo of hair", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "beehive", Value = "beehive-haired",
			AdditionalValue = "5 3 0 a conical pile of hair in the shape of a beehive", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bob cut", Value = "bob-cut",
			AdditionalValue = "3 1 0 bangs and an even bob cut", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bouffant", Value = "bouffant-styled",
			AdditionalValue = "4 2 0 a puffed out, raised section of hair atop the head which hangs down on the sides",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bowl cut", Value = "bowl-cut",
			AdditionalValue = "2 1 0 an all-around even haircut", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "braid", Value = "braided",
			AdditionalValue = "4 1 0 hair woven into a tight braid", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bun", Value = "bun-haired",
			AdditionalValue = "4 2 0 hair tied up into a bun atop the head", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "buzz cut", Value = "buzz-cut",
			AdditionalValue = "1 0 7 closely cropped hair", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "chignon", Value = "chignon-haired",
			AdditionalValue = "4 2 0 hair tied into a chingon bun", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "chonmage", Value = "chonmage-haired",
			AdditionalValue = "4 1 0 a shaved pate, with longer back and sides", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "combover", Value = "comb-overed",
			AdditionalValue = "2 1 0 thin hair combed over a bald spot", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "cornrows", Value = "corn-rowed",
			AdditionalValue = "4 2 0 hair tied into thin, parallel rows of braids", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "crew cut", Value = "crew-cut",
			AdditionalValue = "1 0 7 hair closely cropped but tapering longest to shortest from front to back",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "cropped cut", Value = "crop-haired",
			AdditionalValue = "2 1 0 an all-around even haircut", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "curtain cut", Value = "curtain-haired",
			AdditionalValue = "3 1 0 hair parted neatly in the center", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "dreadlocks", Value = "dreadlocked",
			AdditionalValue = "4 2 0 combed into locked dreads", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "liberty spikes", Value = "liberty-spiked",
			AdditionalValue = "4 2 0 clean, distinct spikes of hair", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bald", Value = "bald",
			AdditionalValue = "0 0 6 no hair at all", Default = true, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "emo cut", Value = "emo-fringed",
			AdditionalValue = "3 1 0 medium-length, straight hair combed over one eye", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "fauxhawk", Value = "fauxhawked",
			AdditionalValue = "2 1 0 short hair style toward a center spike", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "feathered hair", Value = "feathery-haired",
			AdditionalValue = "4 1 0 long, unlayered hair with a center part, brushed back at the sides",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "fishtail hair", Value = "fishtail-haired",
			AdditionalValue = "4 1 0 long hair braided into the shape of a fish's tail", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "flattop", Value = "crewcut",
			AdditionalValue = "1 0 7 a short, level-topped crewcut", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "layered hair", Value = "layered-haired",
			AdditionalValue = "4 2 0 long hair cut unevenly to form layers", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long hair", Value = "long-haired",
			AdditionalValue = "5 0 0 hair that is cut long and flows freely", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "mop top", Value = "mop-topped",
			AdditionalValue =
				"3 1 0 a mid-length haircut extending to the collar with fringe bangs that brush the forehead",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "mullet", Value = "mulleted",
			AdditionalValue = "4 0 0 hair that is short at the front, but long at the back", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "double buns", Value = "double-bunned",
			AdditionalValue = "4 2 0 a pair of buns that jut up like ox-tails from the top of the head",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "sidelocks", Value = "sidelocked",
			AdditionalValue = "4 2 0 twin locks of curly hair that hang from both sides of the face", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pigtails", Value = "pigtailed",
			AdditionalValue = "4 1 0 hair that parts down the middle and is tied into two pony tails on either side",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pixie cut", Value = "pixie-haired",
			AdditionalValue = "2 1 0 a short wispy hairstyle with a shaggy fringe", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pompadour", Value = "pompadoured",
			AdditionalValue =
				"4 2 0 hair swept upwards from the face and worn high over the forehead, upswept at the back and sides",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "ponytail", Value = "ponytailed",
			AdditionalValue = "4 0 0 medium-length hair pulled back behind the head and tied in place", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long ponytail", Value = "long-ponytailed",
			AdditionalValue = "5 0 0 long hair pulled back behind the head and tied in place", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "rattail", Value = "rattailed",
			AdditionalValue = "3 1 0 hair that has been shaved short except for a long braid at the back of the neck",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "ringlets", Value = "ringletted",
			AdditionalValue = "4 2 4 hair worn in tight curls", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "shag hair", Value = "shag-haired",
			AdditionalValue =
				"3 0 0 a choppy, layered hairstyle with fullness at the crown and fringes around the edges",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "short hair", Value = "short-haired",
			AdditionalValue = "2 1 0 hair that is cut short", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "spiky hair", Value = "spike-haired",
			AdditionalValue = "2 1 0 hair that sticks up in spikes on top of the head", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "tonsure", Value = "tonsured",
			AdditionalValue = "1 0 6 a narrow ring of short hair surrounding a bald dome", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "shoulder-length hair", Value = "shoulder-length-haired",
			AdditionalValue = "4 0 0 medium-length hair that falls to the shoulders", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "undercut", Value = "undercut-styled",
			AdditionalValue = "3 1 0 hair that is longer directly on top of the head, but shorter everywhere else",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "weaves", Value = "weave-styled",
			AdditionalValue = "4 3 0 artificial hair extensions woven into the natural hair to lengthen it",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "taper cut hair", Value = "taper-cut-styled",
			AdditionalValue = "2 2 0 hair of a combable length on the top, a side part, and semi-short back and sides",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "frizzy hair", Value = "frizzy-haired",
			AdditionalValue = "3 1 0 medium-length, unmanageable hair with a tendency to frizz", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "wavy hair", Value = "wavy-haired",
			AdditionalValue = "3 1 0 thick hair with a natural wave to it", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "thin hair", Value = "thin-haired",
			AdditionalValue = "3 1 0 naturally straight, thin hair", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "thick hair", Value = "thick-haired",
			AdditionalValue = "3 1 0 naturally full-bodied, thick hair", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "curly hair", Value = "curly-haired",
			AdditionalValue = "3 1 0 hair with natural curls", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "norman cut", Value = "norman-cut",
			AdditionalValue =
				"3 1 0 hair that has been shaved at the back of the head and the neck, but allowed to grow on the top front of the head",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "mop hair", Value = "mop-haired",
			AdditionalValue =
				"3 1 0 a mid-length haircut extending to the collar with fringe bangs that brush the forehead",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "stubble hair", Value = "stubble-haired",
			AdditionalValue = "1 0 0 a thin, even covering of stubble and regrowth atop &his head", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "short ponytail", Value = "short-ponytailed",
			AdditionalValue =
				"3 0 0 short hair pulled back behind the head into a small ponytail, with barely enough hair to make it work",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "balding", Value = "balding",
			AdditionalValue =
				"1 0 0 a short shaved hairStyleDef with a bald dome, evidently suffering from pattern baldness",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "balding crop", Value = "balding crop-haired",
			AdditionalValue =
				"2 0 0 a mid-length cropped hairstyle with a bald dome, evidently suffering from pattern baldness",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "short afro", Value = "short-afroed",
			AdditionalValue = "3 2 0 a short, frizzy halo of hair", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "huge afro", Value = "huge-afroed",
			AdditionalValue = "5 2 0 an enormous, frizzy halo of hair, very impractical looking", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "fade cut", Value = "fade-haired",
			AdditionalValue =
				"3 2 7 a short, neatly maintained hairstyle where the hair goes from a combed style on top to a short shave on the back and sides with a seamless transition",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "mid fade cut", Value = "mid-fade-haired",
			AdditionalValue =
				"3 2 7 a mid-length hairstyle with a wavy combed style on top to a short shaved back and sides in a seamless transition",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "skin fade", Value = "skin-fade-haired",
			AdditionalValue =
				"2 2 7 a short, neatly maintained hairstyle where the hair goes from a combed style on top to a bare back and sides by way of a seamless transition through a shaved style",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "short curls", Value = "short-curly-haired",
			AdditionalValue = "2 0 0 short hair with natural curls", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "quiff", Value = "quiff-haired",
			AdditionalValue =
				"3 1 0 hair swept forwards from the face, similar to a pompodour but jutting forward from the head",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pompadour mullet", Value = "pompadour-mulleted",
			AdditionalValue =
				"4 2 0 hair that is swept upwards at the front of the face into a pompadour, but long and loose at the back",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "dreadhawk", Value = "dreadhawked",
			AdditionalValue =
				"4 2 0 dreadlocked hair swept back over &his head, and short, shaved hair along the sides",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "fanhawk", Value = "mohawked",
			AdditionalValue =
				"4 2 0 a hairstyle in which both sides of &his head are shaven, leaving a strip of long hair fanned out in the center",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "warhawk", Value = "warhawked",
			AdditionalValue = "4 2 0 hair style in a very short mohawk, and shaved along the sides of the skull",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "frohawk", Value = "frohawked",
			AdditionalValue = "4 2 0 hair style like a typical afro, only the sides of the skull have been shaved",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "reverse mohawk", Value = "reverse-mohawked",
			AdditionalValue =
				"4 2 0 really, really stupid looking hair: a single strip has been shaved down the middle of &his skull",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "deathhawk", Value = "deathhawked",
			AdditionalValue =
				"4 2 0 voluminous, backcombed hair style into a loose mohawk, with shaved hair along the sides of the skull",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "rathawk", Value = "rathawked",
			AdditionalValue = "4 2 0 a typical, fan-style mohawk that ends in a grungy little rattail", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "mohawk", Value = "mohawked",
			AdditionalValue =
				"4 2 0 a hairstyle where only the central strip of hair remains, the sides having been shaved",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "crochet twists", Value = "crochet-twisted",
			AdditionalValue = "4 3 0 long coils of hair twisted in rope-like braids", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "micro braids", Value = "micro-braided",
			AdditionalValue = "4 3 0 dozens of uniformly tiny braids covering &his whole head", Default = false,
			Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "corkscrew twist curls", Value = "corkscrew-curled",
			AdditionalValue = "4 3 0 shoulder length hair which is extremely curly, forming tight corkscrew twists",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bantu knots", Value = "bantu-knotted",
			AdditionalValue = "4 3 0 hair which has been twisted and wrapped to form numerous knobs atop &his head",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "wedge cut", Value = "wedge-styled",
			AdditionalValue =
				"2 0 0 hair cut short at the nape, layering gradually upward to become longer on top and in the front, creating a wedge-like look",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pixie wedge", Value = "pixie-wedge-cut",
			AdditionalValue =
				"2 0 0 very short hair, shaven at the nape and longer on the top and sides, somewhere between a pixie cut and a wedge",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "pixie bob", Value = "pixie-bob-cut",
			AdditionalValue =
				"2 0 0 hair that is very short in the back, but nearly chin-length in the front, angling to match &his jawline",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "varsity style", Value = "varsity-styled",
			AdditionalValue =
				"2 0 0 vintage style hair that is side-parted and slicked, shorter on the sides and in the back",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "cowlick", Value = "cowlicked",
			AdditionalValue = "2 0 0 short, undercut hair slicked into a prominent curl at &his forehead",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "slick  hair", Value = "slick-haired",
			AdditionalValue = "2 0 0 short hair that has been slicked back away from &his face", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "side part", Value = "side-parted",
			AdditionalValue = "2 0 0 short hair parted along one side and combed sideways across &his head",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "high ponytail", Value = "high-ponytailed",
			AdditionalValue = "4 0 0 long hair pulled into a ponytail high on &his head", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "devilock", Value = "devilocked",
			AdditionalValue =
				"2 1 0 a long forelock of hair that rigidly sticks down the front of &his face, sides and back otherwise fairly short",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "wings", Value = "wing-haired",
			AdditionalValue = "2 0 0 short, fluffy hair that's been combed to either side of &his face",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "bunches", Value = "bunch-haired",
			AdditionalValue = "3 0 0 mid-length hair that's been bunched up into several uneven portions",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "jheri curls", Value = "jheri-curled",
			AdditionalValue = "3 2 0 a mid-length, dense hairstyle consisting of loose curls, with longer fore-curls",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long jheri curls", Value = "jheri-curled",
			AdditionalValue =
				"4 2 0 a long, dense hairstyle consisting of loose curls, with slightly longer forecurls framing the face",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "finger wave", Value = "finger-waved",
			AdditionalValue = "3 2 0 a short, feminine haircut that has been set in a wedged, wavy style",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "hat hair", Value = "hat-haired",
			AdditionalValue =
				"2 0 0 short, messy hair that looks like &he has had a cap or hat on for an extended period of time",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long braid", Value = "long-braided",
			AdditionalValue = "5 1 0 hair woven into a long, tight braid", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "large bun", Value = "large-bunned",
			AdditionalValue = "5 2 0 hair that has been wrapped up into a large bun", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "twisted bun", Value = "twist-bunned",
			AdditionalValue = "5 2 0 hair that has been twisted around into a large bun", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "rope braid", Value = "rope-braided",
			AdditionalValue = "5 2 0 a twisted braid that has been styled to look like coiled rope", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "french braid", Value = "french-braided",
			AdditionalValue =
				"5 3 0 a large, thick braid that starts near the hairline, with a simple alternating overlap pattern",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "herringbone", Value = "herringbone-braided",
			AdditionalValue =
				"5 3 0 a large, thick braid that starts near the back of the neck and comes together at the back of the head with two flat zones of hair tying it all in",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "crown braid", Value = "crown-braided",
			AdditionalValue =
				"5 4 0 a large, thick braid with a simple alternating overlapping pattern which has been wrapped around &his head like a crown",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "basket weave", Value = "basket-woven",
			AdditionalValue =
				"4 4 0 long hair, natural at the back but woven into a criss-cross basketweave like pattern on top of &his head",
			Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long wavy hair", Value = "long-wavy-haired",
			AdditionalValue = "4 1 0 thick, long hair with a natural wave to it", Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long mullet", Value = "long-mulleted",
			AdditionalValue = "5 0 0 hair that is short at the front, but very long at the back", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "long feathered", Value = "long-feathery-haired",
			AdditionalValue = "5 1 0 very long, unlayered hair with a center part, brushed back at the sides",
			Default = false, Pluralisation = 1
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = hairStyleDef, Name = "viking braid", Value = "viking-braided",
			AdditionalValue =
				"5 2 0 a tri-line of braids across &his dome, flowing back to the middle twist intertwined in a low ponytail",
			Default = false, Pluralisation = 0
		});
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
		AddPersonWord("tot", "baby", isBabyProg, 1.0);
		AddPersonWord("toddler", "toddler", isToddlerProg, 1.0);
		AddPersonWord("man", "man", isAdultManProg, 10.0);
		AddPersonWord("person", "person", isAdultProg, 1.0);
		AddPersonWord("woman", "woman", isAdultWomanProg, 10.0);
		AddPersonWord("lady", "woman", isAdultWomanProg, 1.0);
		AddPersonWord("lad", "boy", isBoyProg, 4.0);
		AddPersonWord("boy", "boy", isBoyProg, 4.0);
		AddPersonWord("child", "child", isChildProg, 4.0);
		AddPersonWord("tyke", "child", isChildProg, 1.0);
		AddPersonWord("nipper", "child", isChildProg, 1.0);
		AddPersonWord("urchin", "child", isChildProg, 1.0);
		AddPersonWord("imp", "child", isChildProg, 1.0);
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
		AddPersonWord("teen boy", "boy", isBoyProg, 1.0);
		AddPersonWord("young boy", "boy", isBoyProg, 1.0);
		AddPersonWord("young man", "man", isYoungManProg, 5.0);
		AddPersonWord("youngster", "man", isYoungAdultProg, 1.0);
		AddPersonWord("gaffer", "old man", isOldManProg, 1.0);
		AddPersonWord("lass", "girl", isYoungWomanProg, 1.0);
		AddPersonWord("girl", "girl", isGirlProg, 4.0);
		AddPersonWord("teen girl", "girl", isGirlProg, 1.0);
		AddPersonWord("young girl", "girl", isGirlProg, 5.0);
		AddPersonWord("young woman", "girl", isYoungWomanProg, 5.0);
		AddPersonWord("maiden", "woman", isYoungWomanProg, 1.0);
		AddPersonWord("damsel", "woman", isYoungWomanProg, 1.0);
		AddPersonWord("crone", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("old woman", "old woman", isOldWomanProg, 5.0);
		AddPersonWord("elderly woman", "old woman", isOldWomanProg, 5.0);
		AddPersonWord("harridan", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("beldam", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("dowager", "old woman", isOldWomanProg, 1.0);
		AddPersonWord("old man", "old man", isOldManProg, 5.0);
		AddPersonWord("elderly man", "old man", isOldManProg, 5.0);
		AddPersonWord("geezer", "old man", isOldManProg, 1.0);
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
			AddPersonWord("frump", "person", isAdultProg, 1.0);
			AddPersonWord("stud", "man", isAdultManProg, 1.0);
			AddPersonWord("hunk", "man", isAdultManProg, 1.0);
			AddPersonWord("specimen", "person", isAdultProg, 1.0);
			AddPersonWord("gal", "woman", isAdultWomanProg, 1.0);
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

		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "white", Value = "fair",
			AdditionalValue = "of light, caucasian tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "milky-white", Value = "fair",
			AdditionalValue = "of milky, pale white tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "pale-white", Value = "fair",
			AdditionalValue = "of pale white tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "pasty-white", Value = "fair",
			AdditionalValue = "of a pasty, pale white tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "tanned", Value = "fair",
			AdditionalValue = "tanned a healthy brown colour", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "olive", Value = "olive",
			AdditionalValue = "of an olive complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "oriental", Value = "golden",
			AdditionalValue = "of an oriental complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "bronzed", Value = "fair",
			AdditionalValue = "of a deep, bronze tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "dark-olive", Value = "olive",
			AdditionalValue = "of a dark olive complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "light-brown", Value = "brown",
			AdditionalValue = "of a light brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "brown", Value = "brown",
			AdditionalValue = "of a brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "dark-brown", Value = "brown",
			AdditionalValue = "of a dark brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "ebony", Value = "black",
			AdditionalValue = "of a deep, ebony complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "black", Value = "black",
			AdditionalValue = "of a deep, black complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "pale-olive", Value = "golden",
			AdditionalValue = "of a pale olive complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "ruddy", Value = "fair",
			AdditionalValue = "of generally white complexion with areas of a ruddy red", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "golden", Value = "golden",
			AdditionalValue = "of a golden complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "sallow", Value = "fair",
			AdditionalValue = "of an unhealthy pale yellowed complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "copper", Value = "red-brown",
			AdditionalValue = "of a coppery brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "caramel", Value = "brown",
			AdditionalValue = "of a rich, caramel brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "light-copper", Value = "red-brown",
			AdditionalValue = "of a light, coppery brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "dark-copper", Value = "red-brown",
			AdditionalValue = "of a dark, coppery brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "light golden", Value = "golden",
			AdditionalValue = "of a light golden complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "russet", Value = "red-brown",
			AdditionalValue = "of a red-tinged brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "pale", Value = "fair",
			AdditionalValue = "that is as pale as milk", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "translucent", Value = "fair",
			AdditionalValue = "so pale it's translucent, with a frosted purplish tone", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "dusky", Value = "olive",
			AdditionalValue = "that is dark-toned and shaded", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "cinnamon", Value = "red-brown",
			AdditionalValue = "that is a warm reddish-brown colour", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "pallid", Value = "fair",
			AdditionalValue = "of a pale, unhealthy-looking tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "swarthy", Value = "brown",
			AdditionalValue = "of a particularly dark hue, with shadowed tones", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "tawny", Value = "golden",
			AdditionalValue = "of a deep golden-brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "mahogany", Value = "red-brown",
			AdditionalValue = "of a rich red-brown complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "chestnut", Value = "brown",
			AdditionalValue = "that is a golden nut-brown colour", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "ashen", Value = "fair",
			AdditionalValue = "of a pale ashen-grey complexion", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "obsidian", Value = "black",
			AdditionalValue = "that is a deep, rich black in tone", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "jet", Value = "black",
			AdditionalValue = "that is a nearly iridescent blue-black colour", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "buttermilk", Value = "golden",
			AdditionalValue = "that is a light, delicately yellow-toned shade of pale", Default = false,
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "mocha", Value = "brown",
			AdditionalValue = "of a cool-toned brown", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "cocoa", Value = "black",
			AdditionalValue = "the rich brown of dark chocolate", Default = false, Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = skinColourDef, Name = "snow white", Value = "fair",
			AdditionalValue = "the colour of white driven snow", Default = false, Pluralisation = 0
		});
		_context.SaveChanges();

		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "small nose", Value = "small-nosed",
			AdditionalValue = "a small, refined nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "hook nose", Value = "hook-nosed",
			AdditionalValue = "a large, protruding nose that hooks downwards towards the end", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "straight nose", Value = "straight-nosed",
			AdditionalValue = "a medium-sized, straight and symmetrical nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "crooked nose", Value = "crooked-nosed",
			AdditionalValue = "a crooked nose that looks like it has been repeatedly broken", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "button nose", Value = "button-nosed",
			AdditionalValue = "a small, cute button nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "aquiline nose", Value = "aquiline-nosed",
			AdditionalValue = "a nose with a subtle downwards bend like the beak of an eagle", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "round nose", Value = "round-nosed",
			AdditionalValue = "a medium-sized round nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "large nose", Value = "large-nosed",
			AdditionalValue = "a noticably large nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "beak nose", Value = "beak-nosed",
			AdditionalValue = "a downward-sloped nose like the beak of a bird", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "wide nose", Value = "wide-nosed",
			AdditionalValue = "a nose with notably wide nostrils", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "upturned nose", Value = "upturned-nosed",
			AdditionalValue = "a nose with a gentle upwards curve", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "flat nose", Value = "flat-nosed",
			AdditionalValue = "a large, flat nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "big nose", Value = "big-nosed",
			AdditionalValue = "a very big nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "bent nose", Value = "bent-nosed",
			AdditionalValue = "a nose with a notable bend near the top", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "narrow nose", Value = "narrow-nosed",
			AdditionalValue = "a nose with a notably narrow bridge and nostrils", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "bulbous nose", Value = "bulbous-nosed",
			AdditionalValue = "a large, ruddy, bulbous nose", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "acromegalic nose", Value = "acromegalic-nosed",
			AdditionalValue = "a massive lumpy nose resembling a person's fist in proportion", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "roman nose", Value = "roman-nosed",
			AdditionalValue = "a nose with a prominent bridge, slightly convex in profile", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "snub nose", Value = "snub-nosed",
			AdditionalValue = "a nose with a narrow, low bridge, widening and sweeping upward to the tip",
			Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "long nose", Value = "long-nosed",
			AdditionalValue = "a nose which is noticeably long in proportion to &his face", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "short nose", Value = "short-nosed",
			AdditionalValue = "a nose which is noticeably short in proportion to &his face", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = noseDef, Name = "pig nose", Value = "pig-nosed",
			AdditionalValue = "a flattened, up-turned nose with very visible nostrils from any angle", Pluralisation = 0
		});
		_context.SaveChanges();

		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "round", Value = "round-eared",
			AdditionalValue = "decidedly round at the top", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "big", Value = "big-eared",
			AdditionalValue = "a little bigger than normal", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "small", Value = "small-eared",
			AdditionalValue = "a little smaller than normal", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "pointy", Value = "pointy-eared",
			AdditionalValue = "shaped like a bit of a point at the top", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "pinched", Value = "pinch-eared",
			AdditionalValue = "shaped like something has pinched in the top of the helix", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "cauliflower", Value = "cauliflower-eared",
			AdditionalValue = "permanently swolen and engorged from past trauma", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "square", Value = "square-eared",
			AdditionalValue = "almost square, with a distinct angular shape", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "long", Value = "long-eared",
			AdditionalValue = "a little longer than normal", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "narrow", Value = "narrow-eared",
			AdditionalValue = "a little narrower than normal", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "protruding", Value = "protruding-eared",
			AdditionalValue = "sticking out from &his head", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "prominent", Value = "prominent-eared",
			AdditionalValue = "prominent, sticking out from &his head", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "long-lobed", Value = "long-lobed",
			AdditionalValue = "most notable for the long earlobes", Pluralisation = 0
		});
		_context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = nextId++, Definition = earDef, Name = "malformed", Value = "malformed-eared",
			AdditionalValue = "malformed, most likely from a birth or childhood defect", Pluralisation = 0
		});
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

			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "no lips", Value = "lipless",
				AdditionalValue = "no lips at all, only scar tissue and melted skin where lips should be",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "missing left eyebrow", Value = "one-eyebrowed",
				AdditionalValue = "only &his left eyebrow remaining", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "missing right eyebrow", Value = "one-eyebrowed",
				AdditionalValue = "only &his right eyebrow remaining", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "no eyebrows", Value = "eyebrowless",
				AdditionalValue = "no eyebrows at all, which is kinda bizarre", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "monobrow", Value = "monobrowed",
				AdditionalValue = "a single, thick and bushy monobrow knitting their brows together", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "beauty mark", Value = "beauty-marked",
				AdditionalValue = "a dark beauty spot marring the corner of &his mouth, just below &his nose",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "hunchback", Value = "hunchbacked",
				AdditionalValue = "a hump between &his shoulders hinting at a misshapen spine", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "clubfoot", Value = "clubfooted",
				AdditionalValue = "a deformed foot that appears to have been rotated at the ankle", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "butt chin", Value = "butt-chinned",
				AdditionalValue =
					"a chin that is so pronounced and jutting that its similarities to a butt cannot be avoided",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "goiter", Value = "goitered",
				AdditionalValue = "an unsightly lump growing on &his neck", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "buck teeth", Value = "buck-toothed",
				AdditionalValue = "two large front teeth that jut out", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "shattered teeth", Value = "shatter-toothed",
				AdditionalValue = "a mouth full of broken, chipped and shattered teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "unblemished complexion", Value = "unblemished",
				AdditionalValue = "a body that is seemingly free from blemish or scars", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "stretch marks", Value = "stretch-marked",
				AdditionalValue = "a series of light, off-colour scars that suggest skin that has been stretched",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pock marks", Value = "pockmarked",
				AdditionalValue = "a number of small, textured scars from a pox of some kind", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "acne scars", Value = "acne-scarred",
				AdditionalValue = "a number of small, textured scars around the face suggesting a history of acne",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "lantern jaw", Value = "lantern-jawed",
				AdditionalValue = "a jutting, strong lower jawline", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "rough skin", Value = "rough-skinned",
				AdditionalValue = "rough, craggy skin", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "smooth skin", Value = "smooth-skinned",
				AdditionalValue = "extraordinarily smooth, wrinkle free skin", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "loose skin", Value = "loose-skinned",
				AdditionalValue =
					"loose skin with little elasticity, prone to hanging off &his flesh at the mercy of gravity",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "perfect teeth", Value = "perfect-toothed",
				AdditionalValue = "perfect, straight white teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "sausage fingers", Value = "sausage-fingered",
				AdditionalValue = "short, fat, chubby little fingers like miniature sausages", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "fat fingers", Value = "fat-fingered",
				AdditionalValue = "short, fat fingers", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "thin fingers", Value = "thin-fingered",
				AdditionalValue = "noticeably gaunt, thin fingers", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "long fingers", Value = "long-fingered",
				AdditionalValue = "especially long, skeletally thin fingers", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "harelip", Value = "harelipped",
				AdditionalValue = "a cleft lip and palate, leaving a gap at the top of the mouth", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "unremarkable appearance", Value = "unremarkable",
				AdditionalValue =
					"a distinctly unremarkable appearance, in a way that would be hard to describe and hard to remember",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "bland appearance", Value = "bland",
				AdditionalValue =
					"a distinctly bland appearance, in a way that would be hard to describe and hard to remember",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "angular features", Value = "angular-featured",
				AdditionalValue = "jutting, angular facial features with high cheekbones and well defined facial lines",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "double chins", Value = "double-chinned",
				AdditionalValue = "two chins, one after the other", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "thin eyebrows", Value = "thin-browed",
				AdditionalValue = "decidedly thin eyebrows", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "cock eyes", Value = "cockeyed",
				AdditionalValue =
					"a cockeyed gaze, with one of the two eyes never quite focusing in the same direction as the other",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "crossed eyes", Value = "cross-eyed",
				AdditionalValue = "a gaze that generally causes the eyes to cross over one another", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "facial wrinkles", Value = "wrinkled",
				AdditionalValue = "extensive wrinkles in the corners of the eyes and lips", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "crooked teeth", Value = "crooked-toothed",
				AdditionalValue = "crooked, uneven teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "no teeth", Value = "toothless",
				AdditionalValue = "no teeth at all, just gums", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "rotten teeth", Value = "rotten-toothed",
				AdditionalValue = "horrid-looking rotten black teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "yellow teeth", Value = "yellow-toothed",
				AdditionalValue = "extensively stained yellow teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "gold tooth", Value = "gold-toothed",
				AdditionalValue = "a tooth made out of gold", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "jowls", Value = "jowly",
				AdditionalValue = "drooping, jowly cheeks", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "weak chin", Value = "weak-chinned",
				AdditionalValue = "a weak chin that recedes into the neckline", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "straight teeth", Value = "straight-teethed",
				AdditionalValue = "a set of good, straight teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "saw teeth", Value = "saw-toothed",
				AdditionalValue = "a set of angular, jagged teeth like the blades of a saw", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "jagged teeth", Value = "jagged-toothed",
				AdditionalValue = "a set of uneven, jagged-looking teeth", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "missing tooth", Value = "missing-toothed",
				AdditionalValue = "a noticeable gap in &his teeth, where one is missing", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "chipped tooth", Value = "chip-toothed",
				AdditionalValue = "a prominently chipped tooth", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "off white teeth", Value = "off-white-toothed",
				AdditionalValue = "a set of teeth which are decidedly off-white in colour", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "thick eyebrows", Value = "thick-browed",
				AdditionalValue = "a pair of large, thick eyebrows", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pointy eyebrows", Value = "pointy-browed",
				AdditionalValue =
					"a pair of pointy eyebrows that almost always look surprised or curious, no matter &his expression",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "chapped lips", Value = "chap-lipped",
				AdditionalValue = "lips that are chapped and dry, with noticeable breaks in the skin", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "thick lips", Value = "thick-lipped",
				AdditionalValue = "a thick, prominent pair of lips", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pale lips", Value = "pale-lipped",
				AdditionalValue = "a pair of pale lips", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "dark lips", Value = "dark-lipped",
				AdditionalValue = "a pair of lips with a decidedly darker hue than &his skin tone would suggest",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "small lips", Value = "small-lipped",
				AdditionalValue =
					"a pair of small lips, not quite in keeping with the size of &his other facial features",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "thin lips", Value = "thin-lipped",
				AdditionalValue = "a long pair of thin lips", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "chiseled jaw", Value = "chisel-jawed",
				AdditionalValue = "a strong jaw, as if chiseled out of a block of granite", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "sultry eyes", Value = "sultry-eyed",
				AdditionalValue = "an innately suggestive, sultry look in &his eyes", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "smoldering eyes", Value = "smoldering-eyed",
				AdditionalValue = "eyes that smolder with an inner fire and intensity", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "intense gaze", Value = "intense",
				AdditionalValue = "a raw intensity that radiates from &his gaze", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "handsome appearance", Value = "handsome",
				AdditionalValue =
					"an innate attractiveness - all &his features just seem to mesh in such a way as to be ridiculously good-looking",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "attractive features", Value = "attractive",
				AdditionalValue = "an overall, undeniable attractiveness to their appearance; an indefinable something",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pouty lips", Value = "pouty-lipped",
				AdditionalValue = "full, lusciously plump lips shaped like a heart", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "rosy cheeks", Value = "rosy-cheeked",
				AdditionalValue = "small, cherubic cheeks with a noticeable rosy glow", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "palsy", Value = "palsied",
				AdditionalValue = "a face paralyzed on one side, complete with drooping eyelid and slack mouth",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "cataracts", Value = "cloudy-lensed",
				AdditionalValue = "one eye blinded by milky white cataracts", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "wall eye", Value = "wall-eyed",
				AdditionalValue = "misaligned eyes, one drifting outward, seeming to be watching that side",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "bull neck", Value = "bull-necked",
				AdditionalValue = "an exceedingly thick and muscular neck", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "no neck", Value = "no-necked",
				AdditionalValue = "almost no neck at all, &his head seeming to meld straight into &his shoulders",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "underbite", Value = "underbiting",
				AdditionalValue = "a severe underbite, &his lower jaw outthrust beyond the upper", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "overbite", Value = "parrot-mouthed",
				AdditionalValue = "a really noticeable overbite, &his lower jaw set well back from the upper",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "bushy eyebrows", Value = "bushy-browed",
				AdditionalValue = "extremely prominent, bushy eyebrows like two caterpillars sitting above &his eyes",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "resting bitch face", Value = "sneering",
				AdditionalValue = "a seemingly permanent sneer, &his upper lip drawn back and brows lowered",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "haggard appearance", Value = "haggard",
				AdditionalValue = "a worn, world-weary look to &him that suggests a life of hardship and deprivation",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "freckles", Value = "freckled",
				AdditionalValue = "a smattering of freckles across &his nose and cheekbones", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "extreme freckles", Value = "heavily-freckled",
				AdditionalValue = "copious amounts of freckles over &his whole body", Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "chin beauty mark", Value = "beauty-marked",
				AdditionalValue = "a prominent mole located on &his chin", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "gold teeth", Value = "golden-toothed",
				AdditionalValue =
					"several golden teeth, obviously having been replaced at some point, giving &him a shiny smile",
				Pluralisation = 1
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "large forehead", Value = "large-foreheaded",
				AdditionalValue = "an unusually large expanse of forehead between &his hairline and brows",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "cheek beauty mark", Value = "beauty-marked",
				AdditionalValue = "a prominent mole located on &his cheek", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "malformed face", Value = "malformed-faced",
				AdditionalValue =
					"a craggy, bumpy face, a bulging chin and jaw, gapped teeth, and almost no neck, &his head seeming to meld straight into &his shoulders",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "regal bearing", Value = "regal",
				AdditionalValue =
					"an aura of authority about &him, excellent posture and a lofty gaze contributing to a decidedly regal look",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "imposing presence", Value = "imposing",
				AdditionalValue =
					"a posture and bearing that suggests power ready to be wielded, imposing and perhaps unnerving",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "air of elegance", Value = "elegant",
				AdditionalValue =
					"a classically elegant appearance and features combining to impart a sense of refinement and grace",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "refined features", Value = "refined",
				AdditionalValue = "refined, delicately sculpted features", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "august presence", Value = "august",
				AdditionalValue = "a naturally composed, dignified aura about &his bearing and features",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "striking appearance", Value = "striking",
				AdditionalValue = "an intangible something that draws the gaze to &him, unusual and compelling",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "hawkish look", Value = "hawkish",
				AdditionalValue = "a singularly intense look about &him, attention nearly always sharply focused",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "face like a toad", Value = "toad-faced",
				AdditionalValue =
					"a face that resembles nothing so much as a toad, broad and flat, eyes widely spaced and almost facing opposite directions",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pug face", Value = "pug-faced",
				AdditionalValue = "a round, heavily flattened face, with bulging eyes and a wide mouth",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "swan neck", Value = "swan-necked",
				AdditionalValue = "a long, graceful neck", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "face like a hatchet", Value = "hatchet-faced",
				AdditionalValue = "a thin, sharp-edged face, angular and protruding like a hatchet", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "owlish gaze", Value = "owlish",
				AdditionalValue =
					"a wide-eyed appearance, as though &he just woke up from a nap, or needs to open &his eyes extra-wide to take in what &he can see",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "wolfish look", Value = "wolfish",
				AdditionalValue = "something predatory about &him, hungry and feral", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "austere mein", Value = "austere",
				AdditionalValue = "a cold, stern look about &his features", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "horse face", Value = "horse-faced",
				AdditionalValue = "a long, narrow face that evokes the image of a horse", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "hamburger face", Value = "hamburger-faced",
				AdditionalValue =
					"a face that has suffered a heck of a lot of abuse, and now resembles a bunch of ground up raw meat mashed into a vaguely human shape through all the scars and badly reset bones",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "low brow", Value = "low-browed",
				AdditionalValue = "a low set brow with a noticeable bone ridge reminsicent of a neanderthal",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "winsome appearance", Value = "winsome",
				AdditionalValue = "an undeniably innocent aura of attractiveness about &his person", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "pretty appearance", Value = "pretty",
				AdditionalValue =
					"a better than average but not world-beating level of attractiveness; &he is definitely pretty",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "comely appearance", Value = "comely",
				AdditionalValue = "an agreeable, pleasant to look upon overall appearance", Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "good appearance", Value = "good-looking",
				AdditionalValue =
					"an overall appearance that is objectively good looking; better than average at least",
				Pluralisation = 0
			});
			_context.CharacteristicValues.Add(new CharacteristicValue
			{
				Id = nextId++, Definition = distinctiveDef, Name = "extensive stretchmarks", Value = "stretchmarked",
				AdditionalValue =
					"has countless light, off-colour scars that suggest skin that has been stretched like it were a size too small for &his body",
				Pluralisation = 1
			});
		}

		#endregion

		_context.SaveChanges();
	}
}