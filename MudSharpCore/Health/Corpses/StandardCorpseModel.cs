using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Health.Corpses;

public class StandardCorpseModel : CorpseModel
{
	private static readonly Regex DescriptionRegex =
		new("@@(?<which>weight|height|wounds|inv|csdesc|sdesc|desc|he|him|his)", RegexOptions.IgnoreCase);

	private static readonly Regex DescriptionRegex2 =
		new("@@(?<which>eaten|shorteat)\\[(?<addendum>[^\\]]*)\\]", RegexOptions.Multiline);

	protected readonly RankedRange<DecayState> DecayRanges = new();

	protected readonly Dictionary<DescriptionType, Dictionary<DecayState, string>> DecayStringsDictionary =
		new();

	protected readonly Dictionary<DecayState, string> PartDecayStrings = new();

	protected readonly Dictionary<ITerrain, double> TerrainDecayPoints = new();

	protected readonly Dictionary<DecayState, ISolid> DecayMaterials = new();

	protected StandardCorpseModel(MudSharp.Models.CorpseModel model, IFuturemud gameworld)
	{
		_id = model.Id;
		_name = model.Name;
		Description = model.Description;
		LoadFromXml(XElement.Parse(model.Definition), gameworld);
	}

	protected double DefaultDecayPoints { get; set; }

	private void LoadFromXml(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("Ranges");
		if (element == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a Ranges element.");
		}

		foreach (var item in element.Elements())
		{
			try
			{
				DecayRanges.Add((DecayState)int.Parse(item.Attribute("state").Value),
					double.Parse(item.Attribute("lower").Value), double.Parse(item.Attribute("upper").Value));
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The Range element in StandardCorpseModel {Id} element \"{item}\" did not contain one or more of the state, lower, or upper attributes.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The Range element in StandardCorpseModel {Id} element \"{item}\" had a state, lower or upper attribute that did not convert to a number.");
			}
		}

		element = root.Element("EdiblePercentage");
		if (element != null)
		{
			EdiblePercentage = double.Parse(element.Value);
		}
		else
		{
			EdiblePercentage = 1.0;
		}

		element = root.Element("Terrains");
		if (element == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a Terrains element.");
		}

		try
		{
			DefaultDecayPoints = double.Parse(element.Attribute("default").Value);
		}
		catch (NullReferenceException)
		{
			throw new ApplicationException(
				$"The Terrains element in StandardCorpseModel {Id} element \"{element}\" did not contain an attribute called default.");
		}
		catch (FormatException)
		{
			throw new ApplicationException(
				$"The Terrains element in StandardCorpseModel {Id} element \"{element}\" has a value for its default attribute that does not convert to a number.");
		}

		foreach (var item in element.Elements())
		{
			try
			{
				var terrainAttr = item.Attribute("terrain").Value;
				var terrain = long.TryParse(terrainAttr, out var value)
					? gameworld.Terrains.Get(value)
					: gameworld.Terrains.GetByName(terrainAttr);
				if (terrain == null)
				{
					throw new ApplicationException(
						$"The terrain {terrainAttr} was not found in StandardCorpseModel {Id}");
				}

				TerrainDecayPoints[terrain] = double.Parse(item.Attribute("rate").Value);
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The Terrain element in StandardCorpseModel {Id} element \"{item}\" did not contain one or more of the terrain or rate attributes.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The Terrain element in StandardCorpseModel {Id} element \"{item}\" had a rate attribute that did not convert to a number.");
			}
		}

		element = root.Element("Descriptions");
		if (element == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a Descriptions element.");
		}

		var subElement = element.Element("ShortDescriptions");
		if (subElement == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a ShortDescriptions element.");
		}

		DecayStringsDictionary[DescriptionType.Short] = new Dictionary<DecayState, string>();
		foreach (var item in subElement.Elements())
		{
			try
			{
				DecayStringsDictionary[DescriptionType.Short][(DecayState)int.Parse(item.Attribute("state").Value)]
					= item.Value.WindowsLineEndings().SubstituteANSIColour();
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The ShortDescription element in StandardCorpseModel {Id} element \"{item}\" did not contain a state attribute.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The ShortDescription element in StandardCorpseModel {Id} element \"{item}\" had a state attribute that did not convert to a number.");
			}
		}

		subElement = element.Element("FullDescriptions");
		if (subElement == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a FullDescriptions element.");
		}

		DecayStringsDictionary[DescriptionType.Full] = new Dictionary<DecayState, string>();
		foreach (var item in subElement.Elements())
		{
			try
			{
				DecayStringsDictionary[DescriptionType.Full][(DecayState)int.Parse(item.Attribute("state").Value)]
					= item.Value.WindowsLineEndings().SubstituteANSIColour();
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The FullDescription element in StandardCorpseModel {Id} element \"{item}\" did not contain a state attribute.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The FullDescription element in StandardCorpseModel {Id} element \"{item}\" had a state attribute that did not convert to a number.");
			}
		}

		subElement = element.Element("ContentsDescriptions");
		if (subElement == null)
		{
			throw new ApplicationException(
				$"StandardCorpseModel {Id} did not contain a ContentsDescriptions element.");
		}

		DecayStringsDictionary[DescriptionType.Contents] = new Dictionary<DecayState, string>();
		foreach (var item in subElement.Elements())
		{
			try
			{
				DecayStringsDictionary[DescriptionType.Contents][
						(DecayState)int.Parse(item.Attribute("state").Value)]
					= item.Value.WindowsLineEndings().SubstituteANSIColour();
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The ContentsDescription element in StandardCorpseModel {Id} element \"{item}\" did not contain a state attribute.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The ContentsDescription element in StandardCorpseModel {Id} element \"{item}\" had a state attribute that did not convert to a number.");
			}
		}

		subElement = element.Element("PartDescriptions");
		if (subElement == null)
		{
			throw new ApplicationException($"StandardCorpseModel {Id} did not contain a PartDescriptions element.");
		}

		foreach (var item in subElement.Elements())
		{
			try
			{
				PartDecayStrings[(DecayState)int.Parse(item.Attribute("state").Value)] =
					item.Value.WindowsLineEndings().SubstituteANSIColour();
			}
			catch (NullReferenceException)
			{
				throw new ApplicationException(
					$"The PartDescription element in StandardCorpseModel {Id} element \"{item}\" did not contain a state attribute.");
			}
			catch (FormatException)
			{
				throw new ApplicationException(
					$"The PartDescription element in StandardCorpseModel {Id} element \"{item}\" had a state attribute that did not convert to a number.");
			}
		}

		element = root.Element("CorpseMaterials");
		if (element != null)
		{
			foreach (var item in element.Elements("CorpseMaterial"))
			{
				try
				{
					DecayMaterials[(DecayState)int.Parse(item.Attribute("state").Value)] =
						gameworld.Materials.Get(long.Parse(item.Value));
				}
				catch (NullReferenceException)
				{
					throw new ApplicationException(
						$"The CorpseMaterial element in StandardCorpseModel {Id} element \"{item}\" did not contain a state attribute.");
				}
				catch (FormatException)
				{
					throw new ApplicationException(
						$"The CorpseMaterial element in StandardCorpseModel {Id} element \"{item}\" had a state attribute that did not convert to a number.");
				}
			}
		}
	}

	public static void RegisterTypeLoader()
	{
		CorpseModelFactory.RegisterCorpseModelType("Standard", (model, game) => new StandardCorpseModel(model, game));
	}

	protected string ReplaceDescriptionVariables(string description, ICharacter originalCharacter, IPerceiver voyeur,
		DecayState state, double eatenPercentage, IEnumerable<IWound> wounds)
	{
		description = DescriptionRegex.Replace(description, m =>
		{
			switch (m.Groups["which"].Value.ToLowerInvariant())
			{
				case "wounds":
					var genderWord = originalCharacter.ApparentGender(voyeur).Possessive();
					// This workaround with the changing of owners is necessary to account for severed bodyparts not having an owner for a short while
					var oldParent = wounds.FirstOrDefault()?.Parent;
					foreach (var wound in wounds)
					{
						wound.Parent = originalCharacter;
					}

					var woundDescs = wounds.Where(x => x.Bodypart != null).Select(x =>
						                       $"{x.Describe(WoundExaminationType.Look, Outcome.MajorPass)} on {genderWord} {x.Bodypart.FullDescription()}")
					                       .ListToString();
					foreach (var wound in wounds)
					{
						wound.Parent = oldParent;
					}

					return wounds.Any()
						? $"{Describe(DescriptionType.Short, state, originalCharacter, voyeur, eatenPercentage).ColourObject()} has {woundDescs}."
							.Wrap(voyeur.InnerLineFormatLength)
						: $"{Describe(DescriptionType.Short, state, originalCharacter, voyeur, eatenPercentage).ColourObject()} has no visible wounds.";
				case "inv":
					return originalCharacter.Body.GetInventoryString(voyeur);
				case "csdesc":
					return Describe(DescriptionType.Short, state, originalCharacter, voyeur, eatenPercentage)
						.ColourObject();
				case "sdesc":
					return originalCharacter.HowSeen(voyeur, colour: false,
						flags: PerceiveIgnoreFlags.IgnoreSelf |
						       PerceiveIgnoreFlags.IgnoreCorpse);
				case "height":
					return originalCharacter.Gameworld.UnitManager.Describe(originalCharacter.Height,
						UnitType.Length, voyeur);
				case "weight":
					return originalCharacter.Gameworld.UnitManager.Describe(originalCharacter.Weight,
						UnitType.Mass, voyeur);
				case "desc":
					return originalCharacter.HowSeen(voyeur, type: DescriptionType.Full, colour: false);
				case "he":
					return originalCharacter.ApparentGender(voyeur).Subjective();
				case "him":
					return originalCharacter.ApparentGender(voyeur).Objective();
				case "his":
					return originalCharacter.ApparentGender(voyeur).Possessive();
			}

			throw new ApplicationException("Invalid option in StandardCorpseModel regex replace");
		});

		description = DescriptionRegex2.Replace(description, m =>
		{
			switch (m.Groups["which"].Value.ToLowerInvariant())
			{
				case "shorteat":
					return EatenShortDescription(eatenPercentage).ConcatIfNotEmpty(m.Groups["addendum"].Value);
				case "eaten":
					return EatenDescription(eatenPercentage).ConcatIfNotEmpty(m.Groups["addendum"].Value);
			}

			throw new ApplicationException("Invalid option in StandardCorpseModel regex2 replace");
		});
		return originalCharacter.ParseCharacteristics(description, voyeur);
	}

	public override string Describe(DescriptionType type, DecayState state, ICharacter originalCharacter,
		IPerceiver voyeur, double eatenPercentage)
	{
		return
			ReplaceDescriptionVariables(DecayStringsDictionary[type][state], originalCharacter, voyeur, state,
					eatenPercentage, originalCharacter.VisibleWounds(voyeur, WoundExaminationType.Look).ToList())
				.ProperSentences();
	}

	public override string DescribeSevered(DescriptionType type, DecayState state, ICharacter originalCharacter,
		IPerceiver voyeur,
		ISeveredBodypart part, double eatenPercentage)
	{
		return
			ReplaceDescriptionVariables(string.Format(PartDecayStrings[state],
					originalCharacter.Body.DescribeBodypartGroup(part.Parts),
					originalCharacter.Race.Name.ToLowerInvariant()), originalCharacter, voyeur, state, eatenPercentage,
				part.Wounds);
	}

	public override double DecayRate(ITerrain terrain)
	{
		return TerrainDecayPoints.ValueOrDefault(terrain, DefaultDecayPoints);
	}

	public override DecayState GetDecayState(double decayPoints)
	{
		return DecayRanges.Find(decayPoints);
	}

	public override ISolid CorpseMaterial(double decayPoints)
	{
		return DecayMaterials.ValueOrDefault(GetDecayState(decayPoints), null);
	}
}