using System;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Health.Corpses;

public class NonDecayingCorpseModel : CorpseModel
{
	public string CorpseShortDescription { get; set; }

	public string CorpseFullDescription { get; set; }

	public string SeveredBodypartDescription { get; set; }

	public static void RegisterTypeLoader()
	{
		CorpseModelFactory.RegisterCorpseModelType("NonDecaying",
			(model, game) => new NonDecayingCorpseModel(model, game));
	}

	#region Constructors

	private NonDecayingCorpseModel(MudSharp.Models.CorpseModel model, IFuturemud gameworld)
	{
		_id = model.Id;
		_name = model.Name;
		Description = model.Description;
		LoadFromXml(XElement.Parse(model.Definition), gameworld);
	}

	private void LoadFromXml(XElement root, IFuturemud gameworld)
	{
		var element = root.Element("SDesc");
		if (element == null)
		{
			throw new XmlException($"NonDecayingCorpseModel {Id} did not have an SDesc element.");
		}

		CorpseShortDescription = element.Value;

		element = root.Element("FDesc");
		if (element == null)
		{
			throw new XmlException($"NonDecayingCorpseModel {Id} did not have an FDesc element.");
		}

		CorpseFullDescription = element.Value;

		element = root.Element("PartDesc");
		if (element == null)
		{
			throw new XmlException($"NonDecayingCorpseModel {Id} did not have a PartDesc element.");
		}

		element = root.Element("CorpseMaterial");
		if (element != null)
		{
			_corpseMaterial = gameworld.Materials.Get(long.Parse(element.Value));
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
	}

	#endregion

	#region ICorpseModel Members

	public override string Describe(DescriptionType type, DecayState state, ICharacter originalCharacter,
		IPerceiver voyeur, double eatenPercentage)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return string.Format(CorpseShortDescription,
					originalCharacter.HowSeen(voyeur, colour: false,
						flags: PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCorpse));
			case DescriptionType.Full:
				return string.Format(CorpseFullDescription,
					             originalCharacter.HowSeen(voyeur, type: DescriptionType.Full, colour: false))
				             .Append(EatenDescription(eatenPercentage).ConcatIfNotEmpty("\n\n"));
			case DescriptionType.Contents:
				return
					$"{Describe(DescriptionType.Full, state, originalCharacter, voyeur, eatenPercentage)}\n\n{EatenDescription(eatenPercentage).ConcatIfNotEmpty("\n\n")}{originalCharacter.Body.GetInventoryString(voyeur)}";
			default:
				throw new NotSupportedException("Invalid DescriptionType in NonDecayingCorpseModel.Describe");
		}
	}


	public override string DescribeSevered(DescriptionType type, DecayState state, ICharacter originalCharacter,
		IPerceiver voyeur, ISeveredBodypart part, double eatenPercentage)
	{
		return string.Format(SeveredBodypartDescription, originalCharacter.Body.DescribeBodypartGroup(part.Parts),
			originalCharacter.Race.Name.ToLowerInvariant());
	}

	private ISolid _corpseMaterial;

	public override double DecayRate(ITerrain terrain)
	{
		return 0;
	}

	public override DecayState GetDecayState(double decayPoints)
	{
		return DecayState.Fresh;
	}

	public override ISolid CorpseMaterial(double decayPoints)
	{
		return _corpseMaterial;
	}

	#endregion
}