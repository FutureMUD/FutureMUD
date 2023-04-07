using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Merits;
using CharacteristicValue = MudSharp.Form.Characteristics.CharacteristicValue;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private static readonly Regex _basicCharacteristicRegex = new(@"(.+)basic", RegexOptions.IgnoreCase);
	private static readonly Regex _fancyCharacteristicRegex = new(@"(.+)fancy", RegexOptions.IgnoreCase);

	protected readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> DefaultCharacteristicValues =
		new();

	public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions => DefaultCharacteristicValues.Keys;

	protected void SaveCharacteristics(MudSharp.Models.Body dbbody)
	{
		FMDB.Context.Characteristics.RemoveRange(dbbody.Characteristics);
		foreach (var item in DefaultCharacteristicValues)
		{
			var dbitem = new Characteristic
			{
				Body = dbbody,
				CharacteristicId = item.Value.Id,
				Type = (int)item.Key.Id
			};
			dbbody.Characteristics.Add(dbitem);
		}

		CharacteristicsChanged = false;
	}

	private bool _characteristicsChanged;

	public bool CharacteristicsChanged
	{
		get => _characteristicsChanged;
		set
		{
			if (value)
			{
				Changed = true;
			}

			_characteristicsChanged = value;
		}
	}

	public IEnumerable<ICharacteristicValue> RawCharacteristicValues => DefaultCharacteristicValues.Values;

	public IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> RawCharacteristics =>
		DefaultCharacteristicValues.Select(x => (x.Key, x.Value));

	public void ExpireDefinition(ICharacteristicDefinition definition)
	{
		DefaultCharacteristicValues.Remove(definition);
		CharacteristicsChanged = true;
	}

	public void RecalculateCharacteristicsDueToExternalChange()
	{
		var raceCharacteristics = Race.Characteristics(Gender.Enum).ToArray();
		foreach (var characteristic in raceCharacteristics)
		{
			if (DefaultCharacteristicValues.ContainsKey(characteristic))
			{
				continue;
			}

			DefaultCharacteristicValues[characteristic] = Gameworld.CharacteristicValues
			                                                       .Where(x => characteristic.IsValue(x))
			                                                       .GetRandomElement();
			CharacteristicsChanged = true;
		}

		foreach (var characteristic in DefaultCharacteristicValues.ToArray())
		{
			if (raceCharacteristics.Contains(characteristic.Key))
			{
				continue;
			}

			DefaultCharacteristicValues.Remove(characteristic.Key);


			CharacteristicsChanged = true;
		}
	}

	public IObscureCharacteristics GetObscurer(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		var obscurers =
			WornItems.Select(x => x.GetItemType<IObscureCharacteristics>())
			         .WhereNotNull(x => x)
			         .Where(x => x.ObscuresCharacteristic(type) && (voyeur == null || voyeur.CanSee(x.Parent)));
		return obscurers.LastOrDefault();
	}

	public void SetCharacteristic(ICharacteristicDefinition type, ICharacteristicValue value)
	{
		DefaultCharacteristicValues[type] = value;
		CharacteristicsChanged = true;
	}

	public Tuple<ICharacteristicDefinition, CharacteristicDescriptionType> GetCharacteristicDefinition(
		string pattern)
	{
		var descType = CharacteristicDescriptionType.Normal;

		ICharacteristicDefinition type;
		if (_basicCharacteristicRegex.IsMatch(pattern))
		{
			type =
				CharacteristicDefinitions.FirstOrDefault(
					x => x.Pattern.IsMatch(_basicCharacteristicRegex.Match(pattern).Groups[1].Value));
			descType = CharacteristicDescriptionType.Basic;
		}
		else if (_fancyCharacteristicRegex.IsMatch(pattern))
		{
			type =
				CharacteristicDefinitions.FirstOrDefault(
					x => x.Pattern.IsMatch(_fancyCharacteristicRegex.Match(pattern).Groups[1].Value));
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

	public ICharacteristicValue GetCharacteristic(string type, IPerceiver voyeur)
	{
		var definition = (this as IHaveCharacteristics).GetCharacteristicDefinition(type).Item1;
		if (definition == null)
		{
			if (type.ToLowerInvariant() == "height")
			{
				definition = Gameworld.RelativeHeightDescriptors.Ranges.First().Value.Definition;
			}
		}

		return GetCharacteristic(definition, voyeur);
	}

	public ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur)
	{
		if (type.Type == CharacteristicType.RelativeHeight)
		{
			return voyeur is not IHavePhysicalDimensions body
				? type.DefaultValue
				: Gameworld.RelativeHeightDescriptors.Find(Height / body.Height);
		}

		var changer =
			WornItems.SelectNotNull(x => x.GetItemType<IChangeCharacteristics>())
			         .LastOrDefault(x => x.ChangesCharacteristic(type));
		return changer?.GetCharacteristic(type, voyeur) ?? DefaultCharacteristicValues[type];
	}

	public string DescribeCharacteristic(string type, IPerceiver voyeur)
	{
		var definition = (this as IHaveCharacteristics).GetCharacteristicDefinition(type);
		return definition.Item1 == null
			? "--Invalid Characteristic--"
			: DescribeCharacteristic(definition.Item1, voyeur, definition.Item2);
	}

	public string DescribeCharacteristic(ICharacteristicDefinition definition, IPerceiver voyeur,
		CharacteristicDescriptionType type = CharacteristicDescriptionType.Normal)
	{
		var obscurers =
			WornItems
				.SelectNotNull(x => x.GetItemType<IChangeCharacteristics>())
				.Where(x => x.ChangesCharacteristic(definition))
				.ToList();

		if (obscurers.Any())
		{
			return obscurers.Last().DescribeCharacteristic(definition, voyeur);
		}

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

	public string GetConsiderString(IPerceiver voyeur)
	{
		var sb = new StringBuilder();
		if (CharacteristicDefinitions.Any())
		{
			sb.AppendLine($"Characteristics:");
			foreach (var characteristic in CharacteristicDefinitions.OrderBy(x => x.Name))
			{
				if (characteristic.Type == CharacteristicType.RelativeHeight)
				{
					continue;
				}

				sb.AppendLine(
					$"\t{characteristic.Name.ColourName()}: {GetCharacteristic(characteristic, voyeur).GetValue.ColourValue()}");
			}
		}

		if (!string.IsNullOrEmpty(Race.ConsiderString))
		{
			sb.AppendLine();
			sb.AppendLine(Race.ConsiderString.SubstituteANSIColour());
		}

		return sb.ToString();
	}

	#region IHaveMerits Members

	private bool _meritsChanged;

	public bool MeritsChanged
	{
		get => _meritsChanged;
		set
		{
			if (!_meritsChanged && value)
			{
				Changed = true;
			}

			_meritsChanged = value;
		}
	}

	private void SaveMerits(MudSharp.Models.Body body)
	{
		var bodyMerits = body.PerceiverMerits;
		var meritsToRemove = bodyMerits.Where(x => _merits.All(y => y.Id != x.MeritId)).ToList();
		FMDB.Context.PerceiverMerits.RemoveRange(meritsToRemove);
		var meritsToAdd = new List<PerceiverMerit>();
		foreach (var merit in _merits.Where(merit => bodyMerits.All(x => x.MeritId != merit.Id)))
		{
			var newMerit = new PerceiverMerit
			{
				Body = body,
				MeritId = merit.Id
			};
			meritsToAdd.Add(newMerit);
		}

		if (meritsToAdd.Any())
		{
			FMDB.Context.PerceiverMerits.AddRange(meritsToAdd);
		}

		FMDB.Context.SaveChanges();
		_meritsChanged = false;
	}

	private readonly List<IMerit> _merits = new();
	public IEnumerable<IMerit> Merits => _merits;

	public bool AddMerit(IMerit merit)
	{
		if (_merits.Contains(merit))
		{
			return false;
		}

		_merits.Add(merit);
		MeritsChanged = true;
		return true;
	}

	public bool RemoveMerit(IMerit merit)
	{
		if (!_merits.Contains(merit))
		{
			return false;
		}

		_merits.Remove(merit);
		MeritsChanged = true;
		return true;
	}

	#endregion
}