using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.CharacterCreation {
	public interface ICharacterTemplate : IProgVariable, IHaveFuturemud, IHaveTraits, IHaveCharacteristics
	{
		List<IAccent> SelectedAccents { get; }
		List<ITrait> SelectedAttributes { get; }
		MudDate SelectedBirthday { get; }
		List<(ICharacteristicDefinition, ICharacteristicValue)> SelectedCharacteristics { get; }
		ICulture SelectedCulture { get; }
		List<IEntityDescriptionPattern> SelectedEntityDescriptionPatterns { get; }
		IEthnicity SelectedEthnicity { get; }
		string SelectedFullDesc { get; }
		Gender SelectedGender { get; }
		double SelectedHeight { get; }
		IPersonalName SelectedName { get; }
		IRace SelectedRace { get; }
		string SelectedSdesc { get; }
		List<ITraitDefinition> SelectedSkills { get; }
		List<(ITraitDefinition, double)> SkillValues { get; }
		double SelectedWeight { get; }
		ICell SelectedStartingLocation { get; }
		List<IChargenRole> SelectedRoles { get; }
		IAccount Account { get; }
		List<ICharacterMerit> SelectedMerits { get; }
		List<IKnowledge> SelectedKnowledges { get; }
		string NeedsModel { get; }
		Alignment Handedness { get; }
		List<IBodypart> MissingBodyparts { get; }
		List<(IDisfigurementTemplate Disfigurement, IBodypart Bodypart)> SelectedDisfigurements { get; }
		List<IGameItemProto> SelectedProstheses { get; }
		IHealthStrategy? HealthStrategy { get; }

		XElement SaveToXml();

		IEnumerable<ICharacteristicDefinition> IHaveCharacteristics.CharacteristicDefinitions => SelectedCharacteristics.Select(x => x.Item1);
		IEnumerable<ICharacteristicValue> IHaveCharacteristics.RawCharacteristicValues => SelectedCharacteristics.Select(x => x.Item2);
		IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> IHaveCharacteristics.RawCharacteristics => SelectedCharacteristics;

		ICharacteristicValue IHaveCharacteristics.GetCharacteristic(string type, IPerceiver voyeur)
		{
			var definition = GetCharacteristicDefinition(type).Item1;
			if (definition == null)
			{
				if (type.ToLowerInvariant() == "height")
				{
					definition = Gameworld.RelativeHeightDescriptors.Ranges.First().Value.Definition;
				}
			}

			return GetCharacteristic(definition, voyeur);
		}

		ICharacteristicValue IHaveCharacteristics.GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
		{
			if (type.Type == CharacteristicType.RelativeHeight)
			{
				return voyeur is not IHavePhysicalDimensions body
					? type.DefaultValue
					: Gameworld.RelativeHeightDescriptors.Find(SelectedHeight / body.Height);
			}

			return SelectedCharacteristics.FirstOrDefault(x => x.Item1 == type).Item2;
		}

		void IHaveCharacteristics.SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value)
		{
			// Do nothing
		}

		string IHaveCharacteristics.DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
			CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal)
		{
			var characteristic = GetCharacteristic(definition, voyeur);
			switch (type)
			{
				case CharacteristicDescriptionType.Normal:
					return characteristic.GetValue;
				case CharacteristicDescriptionType.Basic:
					return characteristic.GetBasicValue;
				case CharacteristicDescriptionType.Fancy:
					return characteristic.GetFancyValue;
				default:
					throw new NotSupportedException();
			}
		}

		string IHaveCharacteristics.DescribeCharacteristic(string type, IPerceiver voyeur)
		{
			var definition = GetCharacteristicDefinition(type);
			return definition.Item1 == null
				? "--Invalid Characteristic--"
				: DescribeCharacteristic(definition.Item1, voyeur, definition.Item2);
		}

		IObscureCharacteristics IHaveCharacteristics.GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur)
		{
			return null;
		}

		Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> IHaveCharacteristics.GetCharacteristicDefinition(string pattern)
		{
			var descType = CharacteristicDescriptionType.Normal;

			ICharacteristicDefinition type;
			if (IHaveCharacteristicsExtensions.BasicCharacteristicRegex.IsMatch(pattern))
			{
				type =
					CharacteristicDefinitions.FirstOrDefault(
						x => x.Pattern.IsMatch(IHaveCharacteristicsExtensions.BasicCharacteristicRegex.Match(pattern).Groups[1].Value));
				descType = CharacteristicDescriptionType.Basic;
			}
			else if (IHaveCharacteristicsExtensions.FancyCharacteristicRegex.IsMatch(pattern))
			{
				type =
					CharacteristicDefinitions.FirstOrDefault(
						x => x.Pattern.IsMatch(IHaveCharacteristicsExtensions.FancyCharacteristicRegex.Match(pattern).Groups[1].Value));
				descType = CharacteristicDescriptionType.Fancy;
			}
			else
			{
				type = CharacteristicDefinitions.FirstOrDefault(x => x.Pattern.IsMatch(pattern));
			}

			if (type == null)
			{
				return pattern.ToLowerInvariant() == "height"
					? Tuple.Create(Gameworld.RelativeHeightDescriptors.Ranges.First().Value.Definition, descType)
					: Tuple.Create((ICharacteristicDefinition)null, descType);
			}

			return Tuple.Create(type, descType);
		}

		/// <summary>
		///     Signals that the MUD Engine has invalidated this definition and this IHaveCharacteristics should remove it if it
		///     has it
		/// </summary>
		/// <param name="definition"></param>
		void IHaveCharacteristics.ExpireDefinition(ICharacteristicDefinition definition)
		{
			// Do nothing
		}

		void IHaveCharacteristics.RecalculateCharacteristicsDueToExternalChange()
		{
			// Do nothing
		}
	}
}