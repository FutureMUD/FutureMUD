using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg;

internal class VariableRegister : SaveableItem, IVariableRegister
{
	private readonly HashSet<Tuple<FutureProgVariableTypes, string>> _changedDefaults =
		new();

	private readonly HashSet<VariableReference> _changedReferences = new();

	private readonly HashSet<Tuple<FutureProgVariableTypes, string>> _changedTypes =
		new();

	#region Constructors

	public VariableRegister(IFuturemud gameworld)
	{
		Gameworld = gameworld;

		_types = new Dictionary<FutureProgVariableTypes, Dictionary<string, FutureProgVariableTypes>>();
		using (new FMDB())
		{
			foreach (var item in FMDB.Context.VariableDefinitions.AsEnumerable().GroupBy(x => x.OwnerType))
			{
				var type = (FutureProgVariableTypes)item.Key;
				var newDict = new Dictionary<string, FutureProgVariableTypes>();
				foreach (var sub in item)
				{
					newDict[sub.Property] = (FutureProgVariableTypes)sub.ContainedType;
				}

				_types[type] = newDict;
			}
		}

		_defaultValues = new Dictionary<FutureProgVariableTypes, Dictionary<string, IVariableValue>>();
		using (new FMDB())
		{
			foreach (var item in FMDB.Context.VariableDefaults.AsEnumerable().GroupBy(x => x.OwnerType))
			{
				var type = (FutureProgVariableTypes)item.Key;
				var newDict = new Dictionary<string, IVariableValue>();
				foreach (var sub in item)
				{
					IVariableValue newValue;
					var valueType = _types[type][sub.Property];
					switch (valueType)
					{
						case FutureProgVariableTypes.Boolean:
						case FutureProgVariableTypes.Gender:
						case FutureProgVariableTypes.Text:
						case FutureProgVariableTypes.Number:
						case FutureProgVariableTypes.TimeSpan:
						case FutureProgVariableTypes.DateTime:
						case FutureProgVariableTypes.MudDateTime:
							newValue = new ValueVariableValue(XElement.Parse(sub.DefaultValue), valueType, gameworld);
							break;
						default:
							if (valueType.HasFlag(FutureProgVariableTypes.Collection))
							{
								newValue = new CollectionVariableValue(XElement.Parse(sub.DefaultValue), valueType,
									gameworld);
							}
							else if (valueType.HasFlag(FutureProgVariableTypes.Dictionary))
							{
								newValue = new DictionaryVariableValue(XElement.Parse(sub.DefaultValue), valueType,
									gameworld);
							}
							else if (valueType.HasFlag(FutureProgVariableTypes.CollectionDictionary))
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
				var type = (FutureProgVariableTypes)item.ValueType;
				IVariableValue newValue;
				switch (type)
				{
					case FutureProgVariableTypes.Boolean:
					case FutureProgVariableTypes.Gender:
					case FutureProgVariableTypes.Text:
					case FutureProgVariableTypes.Number:
					case FutureProgVariableTypes.DateTime:
					case FutureProgVariableTypes.TimeSpan:
					case FutureProgVariableTypes.MudDateTime:
						newValue = new ValueVariableValue(XElement.Parse(item.ValueDefinition), type, gameworld);
						break;
					default:
						if (type.HasFlag(FutureProgVariableTypes.Collection))
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
						Type = (FutureProgVariableTypes)item.ReferenceType,
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
		public FutureProgVariableTypes Type { get; init; }
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

		public bool Equals(IFutureProgVariable variable, string property)
		{
			if (!(variable is IFrameworkItem variableAsIItem))
			{
				return false;
			}

			return variableAsIItem.Id == ID && variable.Type == Type &&
			       VariableName.Equals(property, StringComparison.InvariantCultureIgnoreCase);
		}

		public static VariableReference GetReference(IFutureProgVariable variable, string property)
		{
			if (variable?.Type.HasFlag(FutureProgVariableTypes.Collection) != false)
			{
				return null;
			}

			switch (variable.Type)
			{
				case FutureProgVariableTypes.Gender:
				case FutureProgVariableTypes.Number:
				case FutureProgVariableTypes.Text:
				case FutureProgVariableTypes.Boolean:
				case FutureProgVariableTypes.Error:
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
		FutureProgVariableTypes Type { get; }
		IFutureProgVariable GetVariable(IFuturemud game);
		XElement SaveToXml();
	}

	internal class CollectionVariableValue : IVariableValue
	{
		public CollectionVariableValue()
		{
		}

		public CollectionVariableValue(XElement root, FutureProgVariableTypes type, IFuturemud gameworld)
		{
			Collection = new List<IVariableValue>();
			UnderlyingType = type ^ FutureProgVariableTypes.Collection;
			foreach (var element in root.Elements("var"))
			{
				switch (UnderlyingType)
				{
					case FutureProgVariableTypes.Number:
					case FutureProgVariableTypes.Text:
					case FutureProgVariableTypes.Boolean:
					case FutureProgVariableTypes.Gender:
					case FutureProgVariableTypes.DateTime:
					case FutureProgVariableTypes.TimeSpan:
					case FutureProgVariableTypes.MudDateTime:
						Collection.Add(new ValueVariableValue(element, UnderlyingType, gameworld));
						break;
					default:
						Collection.Add(new ReferenceVariableValue(element, UnderlyingType));
						break;
				}
			}
		}

		public List<IVariableValue> Collection { get; init; }
		public FutureProgVariableTypes UnderlyingType { get; init; }

		public FutureProgVariableTypes Type => UnderlyingType | FutureProgVariableTypes.Collection;

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

		public IFutureProgVariable GetVariable(IFuturemud game)
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

		public CollectionDictionaryVariableValue(XElement root, FutureProgVariableTypes type, IFuturemud gameworld)
		{
			Dictionary = new CollectionDictionary<string, IVariableValue>();
			UnderlyingType = type ^ FutureProgVariableTypes.Collection;
			foreach (var element in root.Elements("value"))
			{
				var key = element.Element("key").Value;
				switch (UnderlyingType)
				{
					case FutureProgVariableTypes.Number:
					case FutureProgVariableTypes.Text:
					case FutureProgVariableTypes.Boolean:
					case FutureProgVariableTypes.Gender:
					case FutureProgVariableTypes.DateTime:
					case FutureProgVariableTypes.TimeSpan:
					case FutureProgVariableTypes.MudDateTime:
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
		public FutureProgVariableTypes UnderlyingType { get; init; }

		public FutureProgVariableTypes Type => UnderlyingType | FutureProgVariableTypes.CollectionDictionary;

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

		public IFutureProgVariable GetVariable(IFuturemud game)
		{
			var collection = new CollectionDictionary<string, IFutureProgVariable>();
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

		public DictionaryVariableValue(XElement root, FutureProgVariableTypes type, IFuturemud gameworld)
		{
			Dictionary = new Dictionary<string, IVariableValue>();
			UnderlyingType = type ^ FutureProgVariableTypes.Collection;
			foreach (var element in root.Elements("value"))
			{
				switch (UnderlyingType)
				{
					case FutureProgVariableTypes.Number:
					case FutureProgVariableTypes.Text:
					case FutureProgVariableTypes.Boolean:
					case FutureProgVariableTypes.Gender:
					case FutureProgVariableTypes.DateTime:
					case FutureProgVariableTypes.TimeSpan:
					case FutureProgVariableTypes.MudDateTime:
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
		public FutureProgVariableTypes UnderlyingType { get; init; }

		public FutureProgVariableTypes Type => UnderlyingType | FutureProgVariableTypes.Dictionary;

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

		public IFutureProgVariable GetVariable(IFuturemud game)
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

		public ValueVariableValue(XElement root, FutureProgVariableTypes type, IFuturemud gameworld)
		{
			Type = type;
			if (root.HasElements)
			{
				root = root.Element("var");
			}

			switch (type)
			{
				case FutureProgVariableTypes.Boolean:
					Value = bool.Parse(root.Value);
					break;
				case FutureProgVariableTypes.Gender:
					Value = (Gender)short.Parse(root.Value);
					break;
				case FutureProgVariableTypes.Number:
					Value = decimal.Parse(root.Value);
					break;
				case FutureProgVariableTypes.Text:
					Value = root.Value;
					break;
				case FutureProgVariableTypes.TimeSpan:
					Value = TimeSpan.Parse(root.Value, CultureInfo.InvariantCulture);
					break;
				case FutureProgVariableTypes.DateTime:
					Value = DateTime.Parse(root.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
					break;
				case FutureProgVariableTypes.MudDateTime:
					Value = new MudDateTime(root.Value, gameworld);
					break;
			}
		}

		public object Value { get; init; }
		public FutureProgVariableTypes Type { get; init; }

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"ValueVariableValue: {Value?.ToString() ?? "null"}";
		}

		#endregion

		#region IVariableValue Members

		public IFutureProgVariable GetVariable(IFuturemud game)
		{
			if (Value == null)
			{
				return new NullVariable(Type);
			}

			switch (Type)
			{
				case FutureProgVariableTypes.Number:
					return new NumberVariable((decimal)Value);
				case FutureProgVariableTypes.Text:
					return new TextVariable((string)Value);
				case FutureProgVariableTypes.Boolean:
					return new BooleanVariable((bool)Value);
				case FutureProgVariableTypes.Gender:
					return new GenderVariable((Gender)Value);
				case FutureProgVariableTypes.TimeSpan:
					return new TimeSpanVariable((TimeSpan)Value);
				case FutureProgVariableTypes.DateTime:
					return new DateTimeVariable((DateTime)Value);
				case FutureProgVariableTypes.MudDateTime:
					return (MudDateTime)Value;
			}

			return null;
		}

		public XElement SaveToXml()
		{
			switch (Type)
			{
				case FutureProgVariableTypes.DateTime:
					return new XElement("var", ((DateTime)Value).ToString(CultureInfo.InvariantCulture));
				case FutureProgVariableTypes.MudDateTime:
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

		public ReferenceVariableValue(XElement root, FutureProgVariableTypes type)
		{
			Type = type;
			ID = long.Parse(root.Value);
		}

		public long ID { get; init; }
		public FutureProgVariableTypes Type { get; init; }

		#region Overrides of Object

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"ReferenceVariableValue: Type [{Type.Describe()}] ID [{ID}]";
		}

		#endregion

		#region IVariableValue Members

		public IFutureProgVariable GetVariable(IFuturemud game)
		{
			switch (Type)
			{
				case FutureProgVariableTypes.Character:
					return game.TryGetCharacter(ID, true);
				case FutureProgVariableTypes.Item:
					return game.Items.Get(ID);
				case FutureProgVariableTypes.Location:
					return game.Cells.Get(ID);
				case FutureProgVariableTypes.Zone:
					return game.Zones.Get(ID);
				case FutureProgVariableTypes.Shard:
					return game.Shards.Get(ID);
				case FutureProgVariableTypes.Race:
					return game.Races.Get(ID);
				case FutureProgVariableTypes.Culture:
					return game.Cultures.Get(ID);
				case FutureProgVariableTypes.Trait:
					return game.Traits.Get(ID);
				case FutureProgVariableTypes.Currency:
					return game.Currencies.Get(ID);
				case FutureProgVariableTypes.Clan:
					return game.Clans.Get(ID);
				case FutureProgVariableTypes.ClanRank:
					return game.Clans.SelectMany(x => x.Ranks).FirstOrDefault(x => x.Id == ID);
				case FutureProgVariableTypes.ClanPaygrade:
					return game.Clans.SelectMany(x => x.Paygrades).FirstOrDefault(x => x.Id == ID);
				case FutureProgVariableTypes.ClanAppointment:
					return game.Clans.SelectMany(x => x.Appointments).FirstOrDefault(x => x.Id == ID);
				case FutureProgVariableTypes.Language:
					return game.Languages.Get(ID);
				case FutureProgVariableTypes.Accent:
					return game.Accents.Get(ID);
				case FutureProgVariableTypes.Merit:
					return game.Merits.Get(ID);
				case FutureProgVariableTypes.Calendar:
					return game.Calendars.Get(ID);
				case FutureProgVariableTypes.Clock:
					return game.Clocks.Get(ID);
				case FutureProgVariableTypes.Knowledge:
					return game.Knowledges.Get(ID);
				case FutureProgVariableTypes.Role:
					return game.Roles.Get(ID);
				case FutureProgVariableTypes.Ethnicity:
					return game.Ethnicities.Get(ID);
				case FutureProgVariableTypes.Drug:
					return game.Drugs.Get(ID);
				case FutureProgVariableTypes.WeatherEvent:
					return game.WeatherEvents.Get(ID);
				case FutureProgVariableTypes.Shop:
					return game.Shops.Get(ID);
				case FutureProgVariableTypes.Merchandise:
					return game.Shops.SelectMany(x => x.Merchandises).FirstOrDefault(x => x.Id == ID);
				case FutureProgVariableTypes.Project:
					return game.ActiveProjects.Get(ID);
				case FutureProgVariableTypes.OverlayPackage:
					return game.CellOverlayPackages.Get(ID);
				case FutureProgVariableTypes.Terrain:
					return game.Terrains.Get(ID);
				case FutureProgVariableTypes.Solid:
					return game.Materials.Get(ID);
				case FutureProgVariableTypes.Liquid:
					return game.Liquids.Get(ID);
				case FutureProgVariableTypes.Gas:
					return game.Gases.Get(ID);
				case FutureProgVariableTypes.MagicCapability:
					return game.MagicCapabilities.Get(ID);
				case FutureProgVariableTypes.MagicSchool:
					return game.MagicSchools.Get(ID);
				case FutureProgVariableTypes.MagicSpell:
					return game.MagicSpells.Get(ID);
				case FutureProgVariableTypes.Bank:
					return game.Banks.Get(ID);
				case FutureProgVariableTypes.BankAccount:
					return game.BankAccounts.Get(ID);
				case FutureProgVariableTypes.BankAccountType:
					return game.BankAccountTypes.Get(ID);
				case FutureProgVariableTypes.LegalAuthority:
					return game.LegalAuthorities.Get(ID);
				case FutureProgVariableTypes.Law:
					return game.Laws.Get(ID);
				case FutureProgVariableTypes.Crime:
					return game.Crimes.Get(ID);
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

	private readonly Dictionary<FutureProgVariableTypes, Dictionary<string, FutureProgVariableTypes>> _types;

	private readonly Dictionary<FutureProgVariableTypes, Dictionary<string, IVariableValue>> _defaultValues;

	private readonly Dictionary<VariableReference, IVariableValue> _values;

	private FutureProgVariableTypes GetTypeForVariable(VariableReference reference)
	{
		if (!_types.ContainsKey(reference.Type))
		{
			return FutureProgVariableTypes.Error;
		}

		if (!_types[reference.Type].ContainsKey(reference.VariableName))
		{
			return FutureProgVariableTypes.Error;
		}

		return _types[reference.Type][reference.VariableName];
	}

	#endregion

	#region IVariableRegister Members

	public bool ResetValue(IFutureProgVariable item, string variable)
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

	public IFutureProgVariable GetValue(IFutureProgVariable item, string variable)
	{
		variable = variable.ToLowerInvariant();
		var reference = VariableReference.GetReference(item, variable);
		if (reference == null)
		{
			return new NullVariable(item?.Type ?? FutureProgVariableTypes.Error);
		}

		var value = _values.ValueOrDefault(reference, null);
		if (value == null)
		{
			var varType = GetTypeForVariable(reference);
			// Collections do not have default values, only empty collections
			if (varType.HasFlag(FutureProgVariableTypes.Collection))
			{
				return new CollectionVariable(new List<IFutureProgVariable>(),
					varType ^ FutureProgVariableTypes.Collection);
			}

			if (varType.HasFlag(FutureProgVariableTypes.CollectionDictionary))
			{
				return new CollectionDictionaryVariable(new CollectionDictionary<string, IFutureProgVariable>(),
					varType ^ FutureProgVariableTypes.CollectionDictionary);
			}

			if (varType.HasFlag(FutureProgVariableTypes.Dictionary))
			{
				return new DictionaryVariable(
					new Dictionary<string, IFutureProgVariable>(StringComparer.InvariantCultureIgnoreCase),
					varType ^ FutureProgVariableTypes.Dictionary);
			}

			var defaultValueDictionary = _defaultValues.ValueOrDefault(item.Type, default);
			var defaultValue = defaultValueDictionary?.ValueOrDefault(variable, null);
			return defaultValue != null ? defaultValue.GetVariable(Gameworld) : new NullVariable(varType);
		}

		return value.GetVariable(Gameworld);
	}

	public IFutureProgVariable GetDefaultValue(FutureProgVariableTypes type, string variable)
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

		if (variableType.HasFlag(FutureProgVariableTypes.Collection))
		{
			return new CollectionVariable(new List<IFutureProgVariable>(),
				variableType ^ FutureProgVariableTypes.Collection);
		}

		if (variableType.HasFlag(FutureProgVariableTypes.CollectionDictionary))
		{
			return new CollectionDictionaryVariable(new CollectionDictionary<string, IFutureProgVariable>(),
				variableType ^ FutureProgVariableTypes.CollectionDictionary);
		}

		if (variableType.HasFlag(FutureProgVariableTypes.Dictionary))
		{
			return new DictionaryVariable(
				new Dictionary<string, IFutureProgVariable>(StringComparer.InvariantCultureIgnoreCase),
				variableType ^ FutureProgVariableTypes.Dictionary);
		}

		var reference = _defaultValues[type][variable];
		if (reference is null)
		{
			return new NullVariable(variableType);
		}

		return reference.GetVariable(Gameworld);
	}

	private IVariableValue GetValue(FutureProgVariableTypes type, IFutureProgVariable value)
	{
		if (type.HasFlag(FutureProgVariableTypes.Collection))
		{
			if (value == null)
			{
				return null;
			}

			return new CollectionVariableValue
			{
				Collection =
					((IList<IFutureProgVariable>)value.GetObject).Select(
						x => GetValue(type ^ FutureProgVariableTypes.Collection, x)).ToList(),
				UnderlyingType = type ^ FutureProgVariableTypes.Collection
			};
		}

		if (type.HasFlag(FutureProgVariableTypes.Dictionary))
		{
			if (value == null)
			{
				return null;
			}

			return new DictionaryVariableValue
			{
				Dictionary = ((Dictionary<string, IFutureProgVariable>)value.GetObject).ToDictionary(x => x.Key,
					x => GetValue(type ^ FutureProgVariableTypes.Dictionary, x.Value)),
				UnderlyingType = type ^ FutureProgVariableTypes.Dictionary
			};
		}

		if (type.HasFlag(FutureProgVariableTypes.CollectionDictionary))
		{
			if (value == null)
			{
				return null;
			}

			var collection = new CollectionDictionary<string, IVariableValue>();
			var underlying = type ^ FutureProgVariableTypes.CollectionDictionary;
			foreach (var item in (CollectionDictionary<string, IFutureProgVariable>)value.GetObject)
			foreach (var sub in item.Value)
			{
				collection.Add(item.Key, GetValue(underlying, sub));
			}

			return new CollectionDictionaryVariableValue { Dictionary = collection };
		}

		switch (type)
		{
			case FutureProgVariableTypes.Error:
			case FutureProgVariableTypes.Exit:
			case FutureProgVariableTypes.Chargen:
			case FutureProgVariableTypes.Effect:
				return null;
			case FutureProgVariableTypes.Boolean:
			case FutureProgVariableTypes.Gender:
			case FutureProgVariableTypes.Number:
			case FutureProgVariableTypes.Text:
			case FutureProgVariableTypes.TimeSpan:
			case FutureProgVariableTypes.DateTime:
			case FutureProgVariableTypes.MudDateTime:
				return new ValueVariableValue { Type = type, Value = value.GetObject };
			default:
				return new ReferenceVariableValue
				{
					ID = (value as IFrameworkItem)?.Id ?? 0,
					Type = type
				};
		}
	}

	public bool SetValue(IFutureProgVariable item, string variable, IFutureProgVariable value)
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

	public void SetDefaultValue(FutureProgVariableTypes type, string variable, IFutureProgVariable value)
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

	public bool IsRegistered(FutureProgVariableTypes type, string variable)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _types.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			return false;
		}

		return lookupType.ContainsKey(variable);
	}

	public FutureProgVariableTypes GetType(FutureProgVariableTypes type, string variable)
	{
		variable = variable.ToLowerInvariant();
		var lookupType = _types.ValueOrDefault(type, default);
		if (lookupType == null)
		{
			return FutureProgVariableTypes.Error;
		}

		return lookupType.ContainsKey(variable) ? lookupType[variable] : FutureProgVariableTypes.Error;
	}

	public bool RegisterVariable(FutureProgVariableTypes ownerType, FutureProgVariableTypes variableType,
		string variable, object defaultValue = null)
	{
		var dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, FutureProgVariableTypes>();

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

		_changedDefaults.Add(new Tuple<FutureProgVariableTypes, string>(ownerType, variable.ToLowerInvariant()));

		if (FutureProgVariableTypes.ValueType.HasFlag(variableType))
		{
			switch (variableType)
			{
				case FutureProgVariableTypes.Boolean:
					defaultDictionary[variable.ToLowerInvariant()] =
						new ValueVariableValue { Type = variableType, Value = defaultValue is bool bv && bv };
					break;
				case FutureProgVariableTypes.Text:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is string sv ? sv : ""
					};
					break;
				case FutureProgVariableTypes.Number:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is double dv ? dv : defaultValue is int iv ? iv : 0.0
					};
					break;
				case FutureProgVariableTypes.Gender:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is Gender gv ? gv : Gender.Indeterminate
					};
					break;
				case FutureProgVariableTypes.TimeSpan:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is TimeSpan tsv ? tsv : TimeSpan.FromTicks(0)
					};
					break;
				case FutureProgVariableTypes.DateTime:
					defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
					{
						Type = variableType,
						Value = defaultValue is DateTime dtv ? dtv : DateTime.MinValue
					};
					break;
				case FutureProgVariableTypes.MudDateTime:
					defaultDictionary[variable.ToLowerInvariant()] =
						new ValueVariableValue { Type = variableType, Value = defaultValue };
					break;
			}
		}
		else
		{
			if (variableType.HasFlag(FutureProgVariableTypes.Collection))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new CollectionVariableValue
				{
					Collection = new List<IVariableValue>(),
					UnderlyingType = variableType ^ FutureProgVariableTypes.Collection
				};
			}
			else if (variableType.HasFlag(FutureProgVariableTypes.Dictionary))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new DictionaryVariableValue
				{
					Dictionary = new Dictionary<string, IVariableValue>(),
					UnderlyingType = variableType ^ FutureProgVariableTypes.Dictionary
				};
			}
			else if (variableType.HasFlag(FutureProgVariableTypes.CollectionDictionary))
			{
				defaultDictionary[variable.ToLowerInvariant()] = new CollectionDictionaryVariableValue
				{
					Dictionary = new CollectionDictionary<string, IVariableValue>(),
					UnderlyingType = variableType ^ FutureProgVariableTypes.CollectionDictionary
				};
			}
			else
			{
				defaultDictionary[variable.ToLowerInvariant()] =
					GetValue(variableType, defaultValue as IFutureProgVariable);
			}
		}

		_changedTypes.Add(Tuple.Create(ownerType, variable));
		Changed = true;
		return true;
	}

	public bool DeregisterVariable(FutureProgVariableTypes ownerType, string variable)
	{
		var dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, FutureProgVariableTypes>();

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

	public IEnumerable<Tuple<string, IFutureProgVariable>> AllVariables(IFutureProgVariable item)
	{
		var reference = VariableReference.GetReference(item, null);
		return
			_values.Where(x => VariableEquals(reference, x.Key))
			       .Select(x => Tuple.Create(x.Key.VariableName, x.Value.GetVariable(Gameworld)))
			       .ToList();
	}

	public IEnumerable<Tuple<string, FutureProgVariableTypes>> AllVariables(FutureProgVariableTypes type)
	{
		return
			_types.Where(x => x.Key == type)
			      .SelectMany(x => x.Value)
			      .Select(x => Tuple.Create(x.Key, x.Value))
			      .ToList();
	}

	public bool ValidValueType(FutureProgVariableTypes type, string value)
	{
		if (!type.CompatibleWith(FutureProgVariableTypes.ValueType))
		{
			return false;
		}

		switch (type ^ FutureProgVariableTypes.Literal)
		{
			case FutureProgVariableTypes.Text:
				// All string values are valid
				return true;
			case FutureProgVariableTypes.TimeSpan:
				// Timespans use a regex rather than the built in tryparse
				return FunctionHelper.TimespanRegex.IsMatch(value);
			case FutureProgVariableTypes.DateTime:
				return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
					out var dtValue);
			case FutureProgVariableTypes.Number:
				return double.TryParse(value, out var dValue);
			case FutureProgVariableTypes.Gender:
				return Enum.TryParse(value, true, out Gender gValue);
			case FutureProgVariableTypes.Boolean:
				return bool.TryParse(value, out var bValue);
			case FutureProgVariableTypes.MudDateTime:
				return MudDateTime.TryParse(value, Gameworld, out var mdtValue);
		}

		return false;
	}

	#endregion
}