using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg;

internal class VariableRegister : SaveableItem, IVariableRegister
{
	private readonly HashSet<Tuple<ProgVariableTypes, string>> _changedDefaults =
		new();

	private readonly HashSet<VariableReference> _changedReferences = new();

	private readonly HashSet<Tuple<ProgVariableTypes, string>> _changedTypes =
		new();

	#region Constructors

	public VariableRegister(IFuturemud gameworld)
	{
		Gameworld = gameworld;

		_types = new Dictionary<ProgVariableTypes, Dictionary<string, ProgVariableTypes>>();
		using (new FMDB())
		{
			foreach (var item in FMDB.Context.VariableDefinitions.AsEnumerable().GroupBy(x => x.OwnerType))
			{
				var type = (ProgVariableTypes)item.Key;
				var newDict = new Dictionary<string, ProgVariableTypes>();
				foreach (var sub in item)
				{
					newDict[sub.Property] = (ProgVariableTypes)sub.ContainedType;
				}

				_types[type] = newDict;
			}
		}

		_defaultValues = new Dictionary<ProgVariableTypes, Dictionary<string, IVariableValue>>();
		using (new FMDB())
		{
			foreach (var item in FMDB.Context.VariableDefaults.AsEnumerable().GroupBy(x => x.OwnerType))
			{
				var type = (ProgVariableTypes)item.Key;
				var newDict = new Dictionary<string, IVariableValue>();
				foreach (var sub in item)
				{
					IVariableValue newValue;
					var valueType = _types[type][sub.Property];
					switch (valueType)
					{
						case ProgVariableTypes.Boolean:
						case ProgVariableTypes.Gender:
						case ProgVariableTypes.Text:
						case ProgVariableTypes.Number:
						case ProgVariableTypes.TimeSpan:
						case ProgVariableTypes.DateTime:
						case ProgVariableTypes.MudDateTime:
							newValue = new ValueVariableValue(XElement.Parse(sub.DefaultValue), valueType, gameworld);
							break;
						default:
							if (valueType.HasFlag(ProgVariableTypes.Collection))
							{
								newValue = new CollectionVariableValue(XElement.Parse(sub.DefaultValue), valueType,
									gameworld);
							}
							else if (valueType.HasFlag(ProgVariableTypes.Dictionary))
							{
								newValue = new DictionaryVariableValue(XElement.Parse(sub.DefaultValue), valueType,
									gameworld);
							}
							else if (valueType.HasFlag(ProgVariableTypes.CollectionDictionary))
							{
								newValue = new CollectionDictionaryVariableValue(XElement.Parse(sub.DefaultValue),
									valueType, gameworld);
							}
							else
							{
								newValue = new ReferenceVariableValue(XElement.Parse(sub.DefaultValue), valueType);
							}

							break;
					}

					newDict[sub.Property] = newValue;
				}

				_defaultValues[type] = newDict;
			}
		}

		_values = new Dictionary<VariableReference, IVariableValue>();
		using (new FMDB())
		{
			foreach (var item in FMDB.Context.VariableValues)
			{
				var type = (ProgVariableTypes)item.ValueType;
				IVariableValue newValue;
				switch (type)
				{
					case ProgVariableTypes.Boolean:
					case ProgVariableTypes.Gender:
					case ProgVariableTypes.Text:
					case ProgVariableTypes.Number:
					case ProgVariableTypes.DateTime:
					case ProgVariableTypes.TimeSpan:
					case ProgVariableTypes.MudDateTime:
						newValue = new ValueVariableValue(XElement.Parse(item.ValueDefinition), type, gameworld);
						break;
					default:
						if (type.HasFlag(ProgVariableTypes.Collection))
						{
							newValue = new CollectionVariableValue(XElement.Parse(item.ValueDefinition), type,
								gameworld);
						}
						else
						{
							newValue = new ReferenceVariableValue(XElement.Parse(item.ValueDefinition), type);
						}

						break;
				}

				_values[
					new VariableReference
					{
						ID = item.ReferenceId,
						Type = (ProgVariableTypes)item.ReferenceType,
						VariableName = item.ReferenceProperty
					}] = newValue;
			}
		}
	}

	#endregion

	public override string FrameworkItemType => "VariableRegister";

	public override void Save()
	{
		using (new FMDB())
		{
			foreach (var item in _changedTypes)
			{
				var dbitem = FMDB.Context.VariableDefinitions.Find((long)item.Item1, item.Item2);
				if (dbitem == null)
				{
					dbitem = new Models.VariableDefinition { OwnerType = (long)item.Item1, Property = item.Item2 };
					FMDB.Context.VariableDefinitions.Add(dbitem);
				}

				dbitem.ContainedType = (long)_types[item.Item1][item.Item2];
			}

			FMDB.Context.SaveChanges();
			_changedTypes.Clear();

			foreach (var item in _changedDefaults)
			{
				var dbitem = FMDB.Context.VariableDefaults.Find((long)item.Item1, item.Item2);
				if (dbitem == null)
				{
					dbitem = new Models.VariableDefault { OwnerType = (long)item.Item1, Property = item.Item2 };
					FMDB.Context.VariableDefaults.Add(dbitem);
				}

				dbitem.DefaultValue = _defaultValues[item.Item1][item.Item2].SaveToXml().ToString();
			}

			FMDB.Context.SaveChanges();
			_changedDefaults.Clear();

			foreach (var item in _changedReferences)
			{
				var dbitem = FMDB.Context.VariableValues.Find((long)item.Type, item.ID, item.VariableName);
				if (dbitem == null)
				{
					dbitem = new Models.VariableValue
					{
						ReferenceId = item.ID,
						ReferenceProperty = item.VariableName,
						ReferenceType = (long)item.Type
					};
					FMDB.Context.VariableValues.Add(dbitem);
				}

				var value = _values[item];
				dbitem.ValueDefinition = value.SaveToXml().ToString();
				dbitem.ValueType = (long)value.Type;
			}

			FMDB.Context.SaveChanges();
			_changedReferences.Clear();
		}

		Changed = false;
	}

	internal class VariableReference
	{
		public ProgVariableTypes Type { get; init; }
		public long ID { get; init; }
		public string VariableName { get; init; }

		public override bool Equals(object obj)
		{
			if (!(obj is VariableReference objAsVarRef))
			{
				return false;
			}

			return Type == objAsVarRef.Type && ID == objAsVarRef.ID &&
			       VariableName == objAsVarRef.VariableName;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ID, Type);
		}

		public bool Equals(IProgVariable variable, string property)
		{
			if (!(variable is IFrameworkItem variableAsIItem))
			{
				return false;
			}

			return variableAsIItem.Id == ID && variable.Type == Type &&
			       VariableName.Equals(property, StringComparison.InvariantCultureIgnoreCase);
		}

		public static VariableReference GetReference(IProgVariable variable, string property)
		{
			if (variable?.Type.HasFlag(ProgVariableTypes.Collection) != false)
			{
				return null;
			}

			switch (variable.Type)
			{
				case ProgVariableTypes.Gender:
				case ProgVariableTypes.Number:
				case ProgVariableTypes.Text:
				case ProgVariableTypes.Boolean:
				case ProgVariableTypes.Error:
					return null;
			}

			if (variable is not IFrameworkItem variableAsIItem)
			{
				return null;
			}

			return new VariableReference
			{
				ID = variableAsIItem.Id,
				Type = variable.Type,
				VariableName = property
			};
		}
	}

	internal interface IVariableValue
	{
		ProgVariableTypes Type { get; }
		IProgVariable GetVariable(IFuturemud game);
		XElement SaveToXml();
	}

	internal class CollectionVariableValue : IVariableValue
	{
		public CollectionVariableValue()
		{
		}

		public CollectionVariableValue(XElement root, ProgVariableTypes type, IFuturemud gameworld)
		{
			Collection = new List<IVariableValue>();
			UnderlyingType = type ^ ProgVariableTypes.Collection;
			foreach (var element in root.Elements("var"))
			{
				switch (UnderlyingType)
				{
					case ProgVariableTypes.Number:
					case ProgVariableTypes.Text:
					case ProgVariableTypes.Boolean:
					case ProgVariableTypes.Gender:
					case ProgVariableTypes.DateTime:
					case ProgVariableTypes.TimeSpan:
					case ProgVariableTypes.MudDateTime:
						Collection.Add(new ValueVariableValue(element, UnderlyingType, gameworld));
						break;
					default:
						Collection.Add(new ReferenceVariableValue(element, UnderlyingType));
						break;
				}
			}
		}

		public List<IVariableValue> Collection { get; init; }
		public ProgVariableTypes UnderlyingType { get; init; }

		public ProgVariableTypes Type => UnderlyingType | ProgVariableTypes.Collection;

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return
				$"CollectionVariableValue: Underlying Type [{UnderlyingType.Describe()}] Contents: {Collection.Select(x => x.ToString()).ListToString()}";
		}

		#endregion

		#region IVariableValue Members

		public IProgVariable GetVariable(IFuturemud game)
		{
			return new CollectionVariable(Collection.Select(x => x.GetVariable(game)).ToList(), UnderlyingType);
		}

		public XElement SaveToXml()
		{
			return new XElement("var", new object[]
			{
				from item in Collection
				select item.SaveToXml()
			});
		}

		#endregion
	}

	internal class CollectionDictionaryVariableValue : IVariableValue
	{
		public CollectionDictionaryVariableValue()
		{
		}

		public CollectionDictionaryVariableValue(XElement root, ProgVariableTypes type, IFuturemud gameworld)
		{
			Dictionary = new CollectionDictionary<string, IVariableValue>();
			UnderlyingType = type ^ ProgVariableTypes.Collection;
			foreach (var element in root.Elements("value"))
			{
				var key = element.Element("key").Value;
				switch (UnderlyingType)
				{
					case ProgVariableTypes.Number:
					case ProgVariableTypes.Text:
					case ProgVariableTypes.Boolean:
					case ProgVariableTypes.Gender:
					case ProgVariableTypes.DateTime:
					case ProgVariableTypes.TimeSpan:
					case ProgVariableTypes.MudDateTime:
						foreach (var sub in element.Elements("var"))
						{
							Dictionary.Add(key, new ValueVariableValue(sub, UnderlyingType, gameworld));
						}

						break;
					default:
						foreach (var sub in element.Elements("var"))
						{
							Dictionary.Add(key, new ReferenceVariableValue(sub, UnderlyingType));
						}

						break;
				}
			}
		}

		public CollectionDictionary<string, IVariableValue> Dictionary { get; init; }
		public ProgVariableTypes UnderlyingType { get; init; }

		public ProgVariableTypes Type => UnderlyingType | ProgVariableTypes.CollectionDictionary;

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return
				$"CollectionDictionaryVariableValue: Underlying Type [{UnderlyingType.Describe()}] Contents: {Dictionary.Select(x => $"{x.Key} {x.Value.ToString()}").ListToString()}";
		}

		#endregion

		#region IVariableValue Members

		public IProgVariable GetVariable(IFuturemud game)
		{
			var collection = new CollectionDictionary<string, IProgVariable>();
			foreach (var item in Dictionary)
			foreach (var value in item.Value)
			{
				collection.Add(item.Key, value.GetVariable(game));
			}

			return new CollectionDictionaryVariable(collection, UnderlyingType);
		}

		public XElement SaveToXml()
		{
			return new XElement("var", new object[]
			{
				from item in Dictionary
				select new XElement("value", new XElement("key", item.Key),
					from value in item.Value
					select value.SaveToXml())
			});
		}

		#endregion
	}

	internal class DictionaryVariableValue : IVariableValue
	{
		public DictionaryVariableValue()
		{
		}

		public DictionaryVariableValue(XElement root, ProgVariableTypes type, IFuturemud gameworld)
		{
			Dictionary = new Dictionary<string, IVariableValue>();
			UnderlyingType = type ^ ProgVariableTypes.Collection;
			foreach (var element in root.Elements("value"))
			{
				switch (UnderlyingType)
				{
					case ProgVariableTypes.Number:
					case ProgVariableTypes.Text:
					case ProgVariableTypes.Boolean:
					case ProgVariableTypes.Gender:
					case ProgVariableTypes.DateTime:
					case ProgVariableTypes.TimeSpan:
					case ProgVariableTypes.MudDateTime:
						Dictionary.Add(element.Element("key").Value,
							new ValueVariableValue(element.Element("var"), UnderlyingType, gameworld));
						break;
					default:
						Dictionary.Add(element.Element("key").Value,
							new ReferenceVariableValue(element.Element("var"), UnderlyingType));
						break;
				}
			}
		}

		public Dictionary<string, IVariableValue> Dictionary { get; init; }
		public ProgVariableTypes UnderlyingType { get; init; }

		public ProgVariableTypes Type => UnderlyingType | ProgVariableTypes.Dictionary;

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return
				$"DictionaryVariableValue: Underlying Type [{UnderlyingType.Describe()}] Contents: {Dictionary.Select(x => $"{x.Key} {x.Value.ToString()}").ListToString()}";
		}

		#endregion

		#region IVariableValue Members

		public IProgVariable GetVariable(IFuturemud game)
		{
			return new DictionaryVariable(Dictionary.ToDictionary(x => x.Key, x => x.Value.GetVariable(game)),
				UnderlyingType);
		}

		public XElement SaveToXml()
		{
			return new XElement("var", new object[]
			{
				from item in Dictionary
				select new XElement("value", new XElement("key", item.Key), item.Value.SaveToXml())
			});
		}

		#endregion
	}

	internal class ValueVariableValue : IVariableValue
	{
		public ValueVariableValue()
		{
		}

		public ValueVariableValue(XElement root, ProgVariableTypes type, IFuturemud gameworld)
		{
			Type = type;
			if (root.HasElements)
			{
				root = root.Element("var");
			}

			switch (type)
			{
				case ProgVariableTypes.Boolean:
					Value = bool.Parse(root.Value);
					break;
				case ProgVariableTypes.Gender:
					Value = (Gender)short.Parse(root.Value);
					break;
				case ProgVariableTypes.Number:
					Value = decimal.Parse(root.Value);
					break;
				case ProgVariableTypes.Text:
					Value = root.Value;
					break;
				case ProgVariableTypes.TimeSpan:
					Value = TimeSpan.Parse(root.Value, CultureInfo.InvariantCulture);
					break;
				case ProgVariableTypes.DateTime:
					Value = DateTime.Parse(root.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
					break;
				case ProgVariableTypes.MudDateTime:
					Value = new MudDateTime(root.Value, gameworld);
					break;
			}
		}

		public object Value { get; init; }
		public ProgVariableTypes Type { get; init; }

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"ValueVariableValue: {Value?.ToString() ?? "null"}";
		}

		#endregion

		#region IVariableValue Members

		public IProgVariable GetVariable(IFuturemud game)
		{
			if (Value == null)
			{
				return new NullVariable(Type);
			}

			switch (Type)
			{
				case ProgVariableTypes.Number:
					return new NumberVariable((decimal)Value);
				case ProgVariableTypes.Text:
					return new TextVariable((string)Value);
				case ProgVariableTypes.Boolean:
					return new BooleanVariable((bool)Value);
				case ProgVariableTypes.Gender:
					return new GenderVariable((Gender)Value);
				case ProgVariableTypes.TimeSpan:
					return new TimeSpanVariable((TimeSpan)Value);
				case ProgVariableTypes.DateTime:
					return new DateTimeVariable((DateTime)Value);
				case ProgVariableTypes.MudDateTime:
					return (MudDateTime)Value;
				case ProgVariableTypes.LiquidMixture:
					return new LiquidMixture(XElement.Parse(Value.ToString()), game);
			}

			return null;
		}

		public XElement SaveToXml()
		{
			switch (Type)
			{
				case ProgVariableTypes.DateTime:
					return new XElement("var", ((DateTime)Value).ToString(CultureInfo.InvariantCulture));
				case ProgVariableTypes.MudDateTime:
					return new XElement("var", ((MudDateTime)Value).GetDateTimeString());
			}

			return new XElement("var", Value);
		}

		#endregion
	}

	internal class ReferenceVariableValue : IVariableValue
	{
		public ReferenceVariableValue()
		{
		}

		public ReferenceVariableValue(XElement root, ProgVariableTypes type)
		{
			Type = type;
			ID = long.Parse(root.Value);
		}

		public long ID { get; init; }
		public ProgVariableTypes Type { get; init; }

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"ReferenceVariableValue: Type [{Type.Describe()}] ID [{ID}]";
		}

		#endregion

		#region IVariableValue Members

		public IProgVariable GetVariable(IFuturemud game)
		{
			switch (Type)
			{
				case ProgVariableTypes.Character:
					return game.TryGetCharacter(ID, true);
				case ProgVariableTypes.Item:
					return game.Items.Get(ID);
				case ProgVariableTypes.Location:
					return game.Cells.Get(ID);
				case ProgVariableTypes.Zone:
					return game.Zones.Get(ID);
				case ProgVariableTypes.Shard:
					return game.Shards.Get(ID);
				case ProgVariableTypes.Race:
					return game.Races.Get(ID);
				case ProgVariableTypes.Culture:
					return game.Cultures.Get(ID);
				case ProgVariableTypes.Trait:
					return game.Traits.Get(ID);
				case ProgVariableTypes.Currency:
					return game.Currencies.Get(ID);
				case ProgVariableTypes.Clan:
					return game.Clans.Get(ID);
				case ProgVariableTypes.ClanRank:
					return game.Clans.SelectMany(x => x.Ranks).FirstOrDefault(x => x.Id == ID);
				case ProgVariableTypes.ClanPaygrade:
					return game.Clans.SelectMany(x => x.Paygrades).FirstOrDefault(x => x.Id == ID);
				case ProgVariableTypes.ClanAppointment:
					return game.Clans.SelectMany(x => x.Appointments).FirstOrDefault(x => x.Id == ID);
				case ProgVariableTypes.Language:
					return game.Languages.Get(ID);
				case ProgVariableTypes.Accent:
					return game.Accents.Get(ID);
				case ProgVariableTypes.Merit:
					return game.Merits.Get(ID);
				case ProgVariableTypes.Calendar:
					return game.Calendars.Get(ID);
				case ProgVariableTypes.Clock:
					return game.Clocks.Get(ID);
				case ProgVariableTypes.Knowledge:
					return game.Knowledges.Get(ID);
				case ProgVariableTypes.Role:
					return game.Roles.Get(ID);
				case ProgVariableTypes.Ethnicity:
					return game.Ethnicities.Get(ID);
				case ProgVariableTypes.Drug:
					return game.Drugs.Get(ID);
				case ProgVariableTypes.WeatherEvent:
					return game.WeatherEvents.Get(ID);
				case ProgVariableTypes.Shop:
					return game.Shops.Get(ID);
				case ProgVariableTypes.Merchandise:
					return game.Shops.SelectMany(x => x.Merchandises).FirstOrDefault(x => x.Id == ID);
				case ProgVariableTypes.Project:
					return game.ActiveProjects.Get(ID);
				case ProgVariableTypes.OverlayPackage:
					return game.CellOverlayPackages.Get(ID);
				case ProgVariableTypes.Terrain:
					return game.Terrains.Get(ID);
				case ProgVariableTypes.Solid:
					return game.Materials.Get(ID);
				case ProgVariableTypes.Liquid:
					return game.Liquids.Get(ID);
				case ProgVariableTypes.Gas:
					return game.Gases.Get(ID);
				case ProgVariableTypes.MagicCapability:
					return game.MagicCapabilities.Get(ID);
				case ProgVariableTypes.MagicSchool:
					return game.MagicSchools.Get(ID);
				case ProgVariableTypes.MagicSpell:
					return game.MagicSpells.Get(ID);
				case ProgVariableTypes.Bank:
					return game.Banks.Get(ID);
				case ProgVariableTypes.BankAccount:
					return game.BankAccounts.Get(ID);
				case ProgVariableTypes.BankAccountType:
					return game.BankAccountTypes.Get(ID);
				case ProgVariableTypes.LegalAuthority:
					return game.LegalAuthorities.Get(ID);
				case ProgVariableTypes.Law:
					return game.Laws.Get(ID);
				case ProgVariableTypes.Crime:
					return game.Crimes.Get(ID);
				case ProgVariableTypes.Script:
					return game.Scripts.Get(ID);
				case ProgVariableTypes.Writing:
					return game.Writings.Get(ID);
				case ProgVariableTypes.Area:
					return game.Areas.Get(ID);
				default:
					return null;
			}
		}

		public XElement SaveToXml()
		{
			return new XElement("var", ID);
		}

		#endregion
	}

	#region Core Variables

	private readonly Dictionary<ProgVariableTypes, Dictionary<string, ProgVariableTypes>> _types;

	private readonly Dictionary<ProgVariableTypes, Dictionary<string, IVariableValue>> _defaultValues;

	private readonly Dictionary<VariableReference, IVariableValue> _values;

	private ProgVariableTypes GetTypeForVariable(VariableReference reference)
	{
		if (!_types.ContainsKey(reference.Type))
		{
			return ProgVariableTypes.Error;
		}

		if (!_types[reference.Type].ContainsKey(reference.VariableName))
		{
			return ProgVariableTypes.Error;
		}

		return _types[reference.Type][reference.VariableName];
	}

	#endregion

	#region IVariableRegister Members

	public bool ResetValue(IProgVariable item, string variable)
	{
		variable = variable.ToLowerInvariant();
		var defaultValueDictionary = _defaultValues.ValueOrDefault(item.Type, default);
		if (defaultValueDictionary != null)
		{
			var defaultValue = defaultValueDictionary.ValueOrDefault(variable, null);
			SetValue(item, variable, defaultValue.GetVariable(Gameworld));
		}
		else
		{
			SetValue(item, variable, null);
		}

		return true;
	}

	public IProgVariable GetValue(IProgVariable item, string variable)
	{
		variable = variable.ToLowerInvariant();
		var reference = VariableReference.GetReference(item, variable);
		if (reference == null)
		{
			return new NullVariable(item?.Type ?? ProgVariableTypes.Error);
		}

		var value = _values.ValueOrDefault(reference, null);
		if (value == null)
		{
			var varType = GetTypeForVariable(reference);
			// Collections do not have default values, only empty collections
			if (varType.HasFlag(ProgVariableTypes.Collection))
			{
				return new CollectionVariable(new List<IProgVariable>(),
					varType ^ ProgVariableTypes.Collection);
			}

			if (varType.HasFlag(ProgVariableTypes.CollectionDictionary))
			{
				return new CollectionDictionaryVariable(new CollectionDictionary<string, IProgVariable>(),
					varType ^ ProgVariableTypes.CollectionDictionary);
			}

			if (varType.HasFlag(ProgVariableTypes.Dictionary))
			{
				return new DictionaryVariable(
					new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase),
					varType ^ ProgVariableTypes.Dictionary);
			}

			var defaultValueDictionary = _defaultValues.ValueOrDefault(item.Type, default);
			var defaultValue = defaultValueDictionary?.ValueOrDefault(variable, null);
			return defaultValue != null ? defaultValue.GetVariable(Gameworld) : new NullVariable(varType);
		}

		return value.GetVariable(Gameworld);
	}

	public IProgVariable GetDefaultValue(ProgVariableTypes type, string variable)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _defaultValues.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			_defaultValues[type] = new Dictionary<string, IVariableValue>();
		}

		var variableType = _types[type][variable];

		if (!_defaultValues[type].ContainsKey(variable))
		{
			return new NullVariable(variableType);
		}

		if (variableType.HasFlag(ProgVariableTypes.Collection))
		{
			return new CollectionVariable(new List<IProgVariable>(),
				variableType ^ ProgVariableTypes.Collection);
		}

		if (variableType.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			return new CollectionDictionaryVariable(new CollectionDictionary<string, IProgVariable>(),
				variableType ^ ProgVariableTypes.CollectionDictionary);
		}

		if (variableType.HasFlag(ProgVariableTypes.Dictionary))
		{
			return new DictionaryVariable(
				new Dictionary<string, IProgVariable>(StringComparer.InvariantCultureIgnoreCase),
				variableType ^ ProgVariableTypes.Dictionary);
		}

		var reference = _defaultValues[type][variable];
		if (reference is null)
		{
			return new NullVariable(variableType);
		}

		return reference.GetVariable(Gameworld);
	}

	private IVariableValue GetValue(ProgVariableTypes type, IProgVariable value)
	{
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			if (value == null)
			{
				return null;
			}

			return new CollectionVariableValue
			{
				Collection =
					((IList<IProgVariable>)value.GetObject).Select(
						x => GetValue(type ^ ProgVariableTypes.Collection, x)).ToList(),
				UnderlyingType = type ^ ProgVariableTypes.Collection
			};
		}

		if (type.HasFlag(ProgVariableTypes.Dictionary))
		{
			if (value == null)
			{
				return null;
			}

			return new DictionaryVariableValue
			{
				Dictionary = ((Dictionary<string, IProgVariable>)value.GetObject).ToDictionary(x => x.Key,
					x => GetValue(type ^ ProgVariableTypes.Dictionary, x.Value)),
				UnderlyingType = type ^ ProgVariableTypes.Dictionary
			};
		}

		if (type.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			if (value == null)
			{
				return null;
			}

			var collection = new CollectionDictionary<string, IVariableValue>();
			var underlying = type ^ ProgVariableTypes.CollectionDictionary;
			foreach (var item in (CollectionDictionary<string, IProgVariable>)value.GetObject)
			foreach (var sub in item.Value)
			{
				collection.Add(item.Key, GetValue(underlying, sub));
			}

			return new CollectionDictionaryVariableValue { Dictionary = collection };
		}

		switch (type)
		{
			case ProgVariableTypes.Error:
			case ProgVariableTypes.Exit:
			case ProgVariableTypes.Chargen:
			case ProgVariableTypes.Effect:
				return null;
			case ProgVariableTypes.Boolean:
			case ProgVariableTypes.Gender:
			case ProgVariableTypes.Number:
			case ProgVariableTypes.Text:
			case ProgVariableTypes.TimeSpan:
			case ProgVariableTypes.DateTime:
			case ProgVariableTypes.MudDateTime:
				return new ValueVariableValue { Type = type, Value = value.GetObject };
			default:
				return new ReferenceVariableValue
				{
					ID = (value as IFrameworkItem)?.Id ?? 0,
					Type = type
				};
		}
	}

	public bool SetValue(IProgVariable item, string variable, IProgVariable value)
	{
		variable = variable.ToLowerInvariant();
		var reference = VariableReference.GetReference(item, variable);
		if (reference == null)
		{
			return false;
		}

		var type = GetType(item.Type, variable);
		if (value != null && type != value.Type)
		{
			return false;
		}

		var newValue = GetValue(type, value);
		_values[reference] = newValue;
		Changed = true;
		_changedReferences.Add(reference);
		return true;
	}

	public void SetDefaultValue(ProgVariableTypes type, string variable, IProgVariable value)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _defaultValues.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			_defaultValues[type] = new Dictionary<string, IVariableValue>();
		}

		_defaultValues[type][variable] = GetValue(_types[type][variable], value);
		Changed = true;
		_changedDefaults.Add(Tuple.Create(type, variable));
	}

	public bool IsRegistered(ProgVariableTypes type, string variable)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _types.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			return false;
		}

		return lookupType.ContainsKey(variable);
	}

	public ProgVariableTypes GetType(ProgVariableTypes type, string variable)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _types.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			return ProgVariableTypes.Error;
		}

		return lookupType.ContainsKey(variable) ? lookupType[variable] : ProgVariableTypes.Error;
	}

	public bool RegisterVariable(ProgVariableTypes ownerType, ProgVariableTypes variableType,
		string variable, object defaultValue = null)
	{
		var dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, ProgVariableTypes>();

		if (dictionary.ContainsKey(variable.ToLowerInvariant()))
		{
			return false;
		}

		dictionary[variable.ToLowerInvariant()] = variableType;
		if (!_types.ContainsKey(ownerType))
		{
			_types[ownerType] = dictionary;
		}

		var defaultDictionary = _defaultValues.ValueOrDefault(ownerType, default) ??
		                        new Dictionary<string, IVariableValue>();
		if (!_defaultValues.ContainsKey(ownerType))
		{
			_defaultValues[ownerType] = defaultDictionary;
		}

		_changedDefaults.Add(new Tuple<ProgVariableTypes, string>(ownerType, variable.ToLowerInvariant()));

		if (ProgVariableTypes.ValueType.HasFlag(variableType))
		{
			switch (variableType)
			{
				case ProgVariableTypes.Boolean:
					defaultDictionary[variable.ToLowerInvariant()] =
						new ValueVariableValue { Type = variableType, Value = defaultValue is bool bv && bv };
					break;
				case ProgVariableTypes.Text:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is string sv ? sv : ""
					};
					break;
				case ProgVariableTypes.Number:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is decimal dv ? dv : 
							defaultValue is int iv ? (decimal)iv : 0.0M
					};
					break;
				case ProgVariableTypes.Gender:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is Gender gv ? gv : Gender.Indeterminate
					};
					break;
				case ProgVariableTypes.TimeSpan:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is TimeSpan tsv ? tsv : TimeSpan.FromTicks(0)
					};
					break;
				case ProgVariableTypes.DateTime:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is DateTime dtv ? dtv : DateTime.MinValue
					};
					break;
				case ProgVariableTypes.MudDateTime:
					defaultDictionary[variable.ToLowerInvariant()] =
						new ValueVariableValue { Type = variableType, Value = defaultValue };
					break;
			}
		}
		else
		{
			if (variableType.HasFlag(ProgVariableTypes.Collection))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new CollectionVariableValue
				{
					Collection = new List<IVariableValue>(),
					UnderlyingType = variableType ^ ProgVariableTypes.Collection
				};
			}
			else if (variableType.HasFlag(ProgVariableTypes.Dictionary))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new DictionaryVariableValue
				{
					Dictionary = new Dictionary<string, IVariableValue>(),
					UnderlyingType = variableType ^ ProgVariableTypes.Dictionary
				};
			}
			else if (variableType.HasFlag(ProgVariableTypes.CollectionDictionary))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new CollectionDictionaryVariableValue
				{
					Dictionary = new CollectionDictionary<string, IVariableValue>(),
					UnderlyingType = variableType ^ ProgVariableTypes.CollectionDictionary
				};
			}
			else
			{
				defaultDictionary[variable.ToLowerInvariant()] =
					GetValue(variableType, defaultValue as IProgVariable);
			}
		}

		_changedTypes.Add(Tuple.Create(ownerType, variable));
		Changed = true;
		return true;
	}

	public bool DeregisterVariable(ProgVariableTypes ownerType, string variable)
	{
		var dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, ProgVariableTypes>();

		if (!dictionary.ContainsKey(variable.ToLowerInvariant()))
		{
			return false;
		}

		dictionary.Remove(variable.ToLowerInvariant());
		_changedTypes.Add(Tuple.Create(ownerType, variable));
		Changed = true;
		return true;
	}

	public static bool VariableEquals(VariableReference first, VariableReference second)
	{
		return first != null && second != null && first.ID == second.ID && first.Type == second.Type;
	}

	public IEnumerable<Tuple<string, IProgVariable>> AllVariables(IProgVariable item)
	{
		var reference = VariableReference.GetReference(item, null);
		return
			_values.Where(x => VariableEquals(reference, x.Key))
			       .Select(x => Tuple.Create(x.Key.VariableName, x.Value.GetVariable(Gameworld)))
			       .ToList();
	}

	public IEnumerable<Tuple<string, ProgVariableTypes>> AllVariables(ProgVariableTypes type)
	{
		return
			_types.Where(x => x.Key == type)
			      .SelectMany(x => x.Value)
			      .Select(x => Tuple.Create(x.Key, x.Value))
			      .ToList();
	}

	public bool ValidValueType(ProgVariableTypes type, string value)
	{
		if (!type.CompatibleWith(ProgVariableTypes.ValueType))
		{
			return false;
		}

		switch (type ^ ProgVariableTypes.Literal)
		{
			case ProgVariableTypes.Text:
				// All string values are valid
				return true;
			case ProgVariableTypes.TimeSpan:
				// Timespans use a regex rather than the built in tryparse
				return FunctionHelper.TimespanRegex.IsMatch(value);
			case ProgVariableTypes.DateTime:
				return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
					out var dtValue);
			case ProgVariableTypes.Number:
				return double.TryParse(value, out var dValue);
			case ProgVariableTypes.Gender:
				return Enum.TryParse(value, true, out Gender gValue);
			case ProgVariableTypes.Boolean:
				return bool.TryParse(value, out var bValue);
			case ProgVariableTypes.MudDateTime:
				return MudDateTime.TryParse(value, Gameworld, out var mdtValue);
		}

		return false;
	}

	#endregion
}