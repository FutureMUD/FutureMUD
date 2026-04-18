using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

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
            foreach (IGrouping<string, Models.VariableDefinition> item in FMDB.Context.VariableDefinitions.AsEnumerable().GroupBy(x => x.OwnerTypeDefinition))
            {
                ProgVariableTypes type = ProgVariableTypes.FromStorageString(item.Key);
                Dictionary<string, ProgVariableTypes> newDict = new();
                foreach (Models.VariableDefinition sub in item)
                {
                    newDict[sub.Property] = ProgVariableTypes.FromStorageString(sub.ContainedTypeDefinition);
                }

                _types[type] = newDict;
            }
        }

        _defaultValues = new Dictionary<ProgVariableTypes, Dictionary<string, IVariableValue>>();
        using (new FMDB())
        {
            foreach (IGrouping<string, Models.VariableDefault> item in FMDB.Context.VariableDefaults.AsEnumerable().GroupBy(x => x.OwnerTypeDefinition))
            {
                ProgVariableTypes type = ProgVariableTypes.FromStorageString(item.Key);
                Dictionary<string, IVariableValue> newDict = new();
                foreach (Models.VariableDefault sub in item)
                {
                    IVariableValue newValue;
                    ProgVariableTypes valueType = _types[type][sub.Property];
                    switch (valueType.LegacyCode)
                    {
                        case ProgVariableTypeCode.Boolean:
                        case ProgVariableTypeCode.Gender:
                        case ProgVariableTypeCode.Text:
                        case ProgVariableTypeCode.Number:
                        case ProgVariableTypeCode.TimeSpan:
                        case ProgVariableTypeCode.DateTime:
                        case ProgVariableTypeCode.MudDateTime:
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
            foreach (Models.VariableValue item in FMDB.Context.VariableValues)
            {
                ProgVariableTypes type = ProgVariableTypes.FromStorageString(item.ValueTypeDefinition);
                IVariableValue newValue;
                switch (type.LegacyCode)
                {
                    case ProgVariableTypeCode.Boolean:
                    case ProgVariableTypeCode.Gender:
                    case ProgVariableTypeCode.Text:
                    case ProgVariableTypeCode.Number:
                    case ProgVariableTypeCode.DateTime:
                    case ProgVariableTypeCode.TimeSpan:
                    case ProgVariableTypeCode.MudDateTime:
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
                        Type = ProgVariableTypes.FromStorageString(item.ReferenceTypeDefinition),
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
            foreach (Tuple<ProgVariableTypes, string> item in _changedTypes)
            {
                Models.VariableDefinition dbitem = FMDB.Context.VariableDefinitions.Find(item.Item1.ToStorageString(), item.Item2);
                if (dbitem == null)
                {
                    dbitem = new Models.VariableDefinition
                    {
                        OwnerTypeDefinition = item.Item1.ToStorageString(),
                        Property = item.Item2
                    };
                    FMDB.Context.VariableDefinitions.Add(dbitem);
                }

                dbitem.ContainedTypeDefinition = _types[item.Item1][item.Item2].ToStorageString();
            }

            FMDB.Context.SaveChanges();
            _changedTypes.Clear();

            foreach (Tuple<ProgVariableTypes, string> item in _changedDefaults)
            {
                Models.VariableDefault dbitem = FMDB.Context.VariableDefaults.Find(item.Item1.ToStorageString(), item.Item2);
                if (dbitem == null)
                {
                    dbitem = new Models.VariableDefault
                    {
                        OwnerTypeDefinition = item.Item1.ToStorageString(),
                        Property = item.Item2
                    };
                    FMDB.Context.VariableDefaults.Add(dbitem);
                }

                dbitem.DefaultValue = _defaultValues[item.Item1][item.Item2].SaveToXml().ToString();
            }

            FMDB.Context.SaveChanges();
            _changedDefaults.Clear();

            foreach (VariableReference item in _changedReferences)
            {
                Models.VariableValue dbitem = FMDB.Context.VariableValues.Find(item.Type.ToStorageString(), item.ID, item.VariableName);
                if (dbitem == null)
                {
                    dbitem = new Models.VariableValue
                    {
                        ReferenceId = item.ID,
                        ReferenceProperty = item.VariableName,
                        ReferenceTypeDefinition = item.Type.ToStorageString()
                    };
                    FMDB.Context.VariableValues.Add(dbitem);
                }

                IVariableValue value = _values[item];
                dbitem.ValueDefinition = value.SaveToXml().ToString();
                dbitem.ValueTypeDefinition = value.Type.ToStorageString();
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

            switch (variable.Type.LegacyCode)
            {
                case ProgVariableTypeCode.Gender:
                case ProgVariableTypeCode.Number:
                case ProgVariableTypeCode.Text:
                case ProgVariableTypeCode.Boolean:
                case ProgVariableTypeCode.Error:
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
            foreach (XElement element in root.Elements("var"))
            {
                switch (UnderlyingType.LegacyCode)
                {
                    case ProgVariableTypeCode.Number:
                    case ProgVariableTypeCode.Text:
                    case ProgVariableTypeCode.Boolean:
                    case ProgVariableTypeCode.Gender:
                    case ProgVariableTypeCode.DateTime:
                    case ProgVariableTypeCode.TimeSpan:
                    case ProgVariableTypeCode.MudDateTime:
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
            foreach (XElement element in root.Elements("value"))
            {
                string key = element.Element("key").Value;
                switch (UnderlyingType.LegacyCode)
                {
                    case ProgVariableTypeCode.Number:
                    case ProgVariableTypeCode.Text:
                    case ProgVariableTypeCode.Boolean:
                    case ProgVariableTypeCode.Gender:
                    case ProgVariableTypeCode.DateTime:
                    case ProgVariableTypeCode.TimeSpan:
                    case ProgVariableTypeCode.MudDateTime:
                        foreach (XElement sub in element.Elements("var"))
                        {
                            Dictionary.Add(key, new ValueVariableValue(sub, UnderlyingType, gameworld));
                        }

                        break;
                    default:
                        foreach (XElement sub in element.Elements("var"))
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
                $"CollectionDictionaryVariableValue: Underlying Type [{UnderlyingType.Describe()}] Contents: {Dictionary.Select(x => $"{x.Key} {x.Value}").ListToString()}";
        }

        #endregion

        #region IVariableValue Members

        public IProgVariable GetVariable(IFuturemud game)
        {
            CollectionDictionary<string, IProgVariable> collection = new();
            foreach (KeyValuePair<string, List<IVariableValue>> item in Dictionary)
            {
                foreach (IVariableValue value in item.Value)
                {
                    collection.Add(item.Key, value.GetVariable(game));
                }
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
            foreach (XElement element in root.Elements("value"))
            {
                switch (UnderlyingType.LegacyCode)
                {
                    case ProgVariableTypeCode.Number:
                    case ProgVariableTypeCode.Text:
                    case ProgVariableTypeCode.Boolean:
                    case ProgVariableTypeCode.Gender:
                    case ProgVariableTypeCode.DateTime:
                    case ProgVariableTypeCode.TimeSpan:
                    case ProgVariableTypeCode.MudDateTime:
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
                $"DictionaryVariableValue: Underlying Type [{UnderlyingType.Describe()}] Contents: {Dictionary.Select(x => $"{x.Key} {x.Value}").ListToString()}";
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

            switch (type.LegacyCode)
            {
                case ProgVariableTypeCode.Boolean:
                    Value = bool.Parse(root.Value);
                    break;
                case ProgVariableTypeCode.Gender:
                    Value = (Gender)short.Parse(root.Value);
                    break;
                case ProgVariableTypeCode.Number:
                    Value = decimal.Parse(root.Value);
                    break;
                case ProgVariableTypeCode.Text:
                    Value = root.Value;
                    break;
                case ProgVariableTypeCode.TimeSpan:
                    Value = TimeSpan.Parse(root.Value, CultureInfo.InvariantCulture);
                    break;
                case ProgVariableTypeCode.DateTime:
                    Value = DateTime.Parse(root.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    break;
                case ProgVariableTypeCode.MudDateTime:
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

            switch (Type.LegacyCode)
            {
                case ProgVariableTypeCode.Number:
                    return new NumberVariable((decimal)Value);
                case ProgVariableTypeCode.Text:
                    return new TextVariable((string)Value);
                case ProgVariableTypeCode.Boolean:
                    return new BooleanVariable((bool)Value);
                case ProgVariableTypeCode.Gender:
                    return new GenderVariable((Gender)Value);
                case ProgVariableTypeCode.TimeSpan:
                    return new TimeSpanVariable((TimeSpan)Value);
                case ProgVariableTypeCode.DateTime:
                    return new DateTimeVariable((DateTime)Value);
                case ProgVariableTypeCode.MudDateTime:
                    return (MudDateTime)Value;
                case ProgVariableTypeCode.LiquidMixture:
                    return new LiquidMixture(XElement.Parse(Value.ToString()), game);
            }

            return null;
        }

        public XElement SaveToXml()
        {
            switch (Type.LegacyCode)
            {
                case ProgVariableTypeCode.DateTime:
                    return new XElement("var", ((DateTime)Value).ToString(CultureInfo.InvariantCulture));
                case ProgVariableTypeCode.MudDateTime:
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
			if (Type == ProgVariableTypes.LegalClass)
			{
				return game.LegalClasses.Get(ID);
			}

			switch (Type.LegacyCode)
			{
                case ProgVariableTypeCode.Character:
                    return game.TryGetCharacter(ID, true);
                case ProgVariableTypeCode.Item:
                    return game.Items.Get(ID);
                case ProgVariableTypeCode.Location:
                    return game.Cells.Get(ID);
                case ProgVariableTypeCode.Zone:
                    return game.Zones.Get(ID);
                case ProgVariableTypeCode.Shard:
                    return game.Shards.Get(ID);
                case ProgVariableTypeCode.Race:
                    return game.Races.Get(ID);
                case ProgVariableTypeCode.Culture:
                    return game.Cultures.Get(ID);
                case ProgVariableTypeCode.Trait:
                    return game.Traits.Get(ID);
                case ProgVariableTypeCode.Currency:
                    return game.Currencies.Get(ID);
                case ProgVariableTypeCode.Clan:
                    return game.Clans.Get(ID);
                case ProgVariableTypeCode.ClanRank:
                    return game.Clans.SelectMany(x => x.Ranks).FirstOrDefault(x => x.Id == ID);
                case ProgVariableTypeCode.ClanPaygrade:
                    return game.Clans.SelectMany(x => x.Paygrades).FirstOrDefault(x => x.Id == ID);
                case ProgVariableTypeCode.ClanAppointment:
                    return game.Clans.SelectMany(x => x.Appointments).FirstOrDefault(x => x.Id == ID);
                case ProgVariableTypeCode.Language:
                    return game.Languages.Get(ID);
                case ProgVariableTypeCode.Accent:
                    return game.Accents.Get(ID);
                case ProgVariableTypeCode.Merit:
                    return game.Merits.Get(ID);
                case ProgVariableTypeCode.Calendar:
                    return game.Calendars.Get(ID);
                case ProgVariableTypeCode.Clock:
                    return game.Clocks.Get(ID);
                case ProgVariableTypeCode.Knowledge:
                    return game.Knowledges.Get(ID);
                case ProgVariableTypeCode.Role:
                    return game.Roles.Get(ID);
                case ProgVariableTypeCode.Ethnicity:
                    return game.Ethnicities.Get(ID);
                case ProgVariableTypeCode.Drug:
                    return game.Drugs.Get(ID);
                case ProgVariableTypeCode.WeatherEvent:
                    return game.WeatherEvents.Get(ID);
                case ProgVariableTypeCode.Shop:
                    return game.Shops.Get(ID);
                case ProgVariableTypeCode.Merchandise:
                    return game.Shops.SelectMany(x => x.Merchandises).FirstOrDefault(x => x.Id == ID);
                case ProgVariableTypeCode.Project:
                    return game.ActiveProjects.Get(ID);
                case ProgVariableTypeCode.OverlayPackage:
                    return game.CellOverlayPackages.Get(ID);
                case ProgVariableTypeCode.Terrain:
                    return game.Terrains.Get(ID);
                case ProgVariableTypeCode.Solid:
                    return game.Materials.Get(ID);
                case ProgVariableTypeCode.Liquid:
                    return game.Liquids.Get(ID);
                case ProgVariableTypeCode.Gas:
                    return game.Gases.Get(ID);
                case ProgVariableTypeCode.MagicCapability:
                    return game.MagicCapabilities.Get(ID);
                case ProgVariableTypeCode.MagicSchool:
                    return game.MagicSchools.Get(ID);
                case ProgVariableTypeCode.MagicSpell:
                    return game.MagicSpells.Get(ID);
                case ProgVariableTypeCode.Bank:
                    return game.Banks.Get(ID);
                case ProgVariableTypeCode.BankAccount:
                    return game.BankAccounts.Get(ID);
                case ProgVariableTypeCode.BankAccountType:
                    return game.BankAccountTypes.Get(ID);
                case ProgVariableTypeCode.LegalAuthority:
                    return game.LegalAuthorities.Get(ID);
                case ProgVariableTypeCode.Law:
                    return game.Laws.Get(ID);
                case ProgVariableTypeCode.Crime:
                    return game.Crimes.Get(ID);
                case ProgVariableTypeCode.Script:
                    return game.Scripts.Get(ID);
                case ProgVariableTypeCode.Writing:
                    return game.Writings.Get(ID);
                case ProgVariableTypeCode.Area:
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
        Dictionary<string, IVariableValue> defaultValueDictionary = _defaultValues.ValueOrDefault(item.Type, default);
        if (defaultValueDictionary != null)
        {
            IVariableValue defaultValue = defaultValueDictionary.ValueOrDefault(variable, null);
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
        VariableReference reference = VariableReference.GetReference(item, variable);
        if (reference == null)
        {
            return new NullVariable(item?.Type ?? ProgVariableTypes.Error);
        }

        IVariableValue value = _values.ValueOrDefault(reference, null);
        if (value == null)
        {
            ProgVariableTypes varType = GetTypeForVariable(reference);
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

            Dictionary<string, IVariableValue> defaultValueDictionary = _defaultValues.ValueOrDefault(item.Type, default);
            IVariableValue defaultValue = defaultValueDictionary?.ValueOrDefault(variable, null);
            return defaultValue != null ? defaultValue.GetVariable(Gameworld) : new NullVariable(varType);
        }

        return value.GetVariable(Gameworld);
    }

    public IProgVariable GetDefaultValue(ProgVariableTypes type, string variable)
    {
        variable = variable.ToLowerInvariant();
        Dictionary<string, IVariableValue> lookupType = _defaultValues.ValueOrDefault(type, default);
        if (lookupType == null)
        {
            _defaultValues[type] = new Dictionary<string, IVariableValue>();
        }

        ProgVariableTypes variableType = _types[type][variable];

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

        IVariableValue reference = _defaultValues[type][variable];
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

            CollectionDictionary<string, IVariableValue> collection = new();
            ProgVariableTypes underlying = type ^ ProgVariableTypes.CollectionDictionary;
            foreach (KeyValuePair<string, List<IProgVariable>> item in (CollectionDictionary<string, IProgVariable>)value.GetObject)
            {
                foreach (IProgVariable sub in item.Value)
                {
                    collection.Add(item.Key, GetValue(underlying, sub));
                }
            }

            return new CollectionDictionaryVariableValue { Dictionary = collection };
        }

        switch (type.LegacyCode)
        {
            case ProgVariableTypeCode.Error:
            case ProgVariableTypeCode.Exit:
            case ProgVariableTypeCode.Chargen:
            case ProgVariableTypeCode.Effect:
                return null;
            case ProgVariableTypeCode.Boolean:
            case ProgVariableTypeCode.Gender:
            case ProgVariableTypeCode.Number:
            case ProgVariableTypeCode.Text:
            case ProgVariableTypeCode.TimeSpan:
            case ProgVariableTypeCode.DateTime:
            case ProgVariableTypeCode.MudDateTime:
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
        VariableReference reference = VariableReference.GetReference(item, variable);
        if (reference == null)
        {
            return false;
        }

        ProgVariableTypes type = GetType(item.Type, variable);
        if (value != null && type != value.Type)
        {
            return false;
        }

        IVariableValue newValue = GetValue(type, value);
        _values[reference] = newValue;
        Changed = true;
        _changedReferences.Add(reference);
        return true;
    }

    public void SetDefaultValue(ProgVariableTypes type, string variable, IProgVariable value)
    {
        variable = variable.ToLowerInvariant();
        Dictionary<string, IVariableValue> lookupType = _defaultValues.ValueOrDefault(type, default);
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
        Dictionary<string, ProgVariableTypes> lookupType = _types.ValueOrDefault(type, default);
        if (lookupType == null)
        {
            return false;
        }

        return lookupType.ContainsKey(variable);
    }

    public ProgVariableTypes GetType(ProgVariableTypes type, string variable)
    {
        variable = variable.ToLowerInvariant();
        Dictionary<string, ProgVariableTypes> lookupType = _types.ValueOrDefault(type, default);
        if (lookupType == null)
        {
            return ProgVariableTypes.Error;
        }

        return lookupType.ContainsKey(variable) ? lookupType[variable] : ProgVariableTypes.Error;
    }

    public bool RegisterVariable(ProgVariableTypes ownerType, ProgVariableTypes variableType,
        string variable, object defaultValue = null)
    {
        Dictionary<string, ProgVariableTypes> dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, ProgVariableTypes>();

        if (dictionary.ContainsKey(variable.ToLowerInvariant()))
        {
            return false;
        }

        dictionary[variable.ToLowerInvariant()] = variableType;
        if (!_types.ContainsKey(ownerType))
        {
            _types[ownerType] = dictionary;
        }

        Dictionary<string, IVariableValue> defaultDictionary = _defaultValues.ValueOrDefault(ownerType, default) ??
                                new Dictionary<string, IVariableValue>();
        if (!_defaultValues.ContainsKey(ownerType))
        {
            _defaultValues[ownerType] = defaultDictionary;
        }

        _changedDefaults.Add(new Tuple<ProgVariableTypes, string>(ownerType, variable.ToLowerInvariant()));

        if (ProgVariableTypes.ValueType.HasFlag(variableType))
        {
            switch (variableType.LegacyCode)
            {
                case ProgVariableTypeCode.Boolean:
                    defaultDictionary[variable.ToLowerInvariant()] =
                        new ValueVariableValue { Type = variableType, Value = defaultValue is bool bv && bv };
                    break;
                case ProgVariableTypeCode.Text:
                    defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
                    {
                        Type = variableType,
                        Value = defaultValue is string sv ? sv : ""
                    };
                    break;
                case ProgVariableTypeCode.Number:
                    defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
                    {
                        Type = variableType,
                        Value = defaultValue is decimal dv ? dv :
                            defaultValue is int iv ? (decimal)iv : 0.0M
                    };
                    break;
                case ProgVariableTypeCode.Gender:
                    defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
                    {
                        Type = variableType,
                        Value = defaultValue is Gender gv ? gv : Gender.Indeterminate
                    };
                    break;
                case ProgVariableTypeCode.TimeSpan:
                    defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
                    {
                        Type = variableType,
                        Value = defaultValue is TimeSpan tsv ? tsv : TimeSpan.FromTicks(0)
                    };
                    break;
                case ProgVariableTypeCode.DateTime:
                    defaultDictionary[variable.ToLowerInvariant()] = new ValueVariableValue
                    {
                        Type = variableType,
                        Value = defaultValue is DateTime dtv ? dtv : DateTime.MinValue
                    };
                    break;
                case ProgVariableTypeCode.MudDateTime:
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
        Dictionary<string, ProgVariableTypes> dictionary = _types.ValueOrDefault(ownerType, default) ?? new Dictionary<string, ProgVariableTypes>();

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
        VariableReference reference = VariableReference.GetReference(item, null);
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

        switch ((type ^ ProgVariableTypes.Literal).LegacyCode)
        {
            case ProgVariableTypeCode.Text:
                // All string values are valid
                return true;
            case ProgVariableTypeCode.TimeSpan:
                // Timespans use a regex rather than the built in tryparse
                return FunctionHelper.TimespanRegex.IsMatch(value);
            case ProgVariableTypeCode.DateTime:
                return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                    out DateTime dtValue);
            case ProgVariableTypeCode.Number:
                return double.TryParse(value, out double dValue);
            case ProgVariableTypeCode.Gender:
                return Enum.TryParse(value, true, out Gender gValue);
            case ProgVariableTypeCode.Boolean:
                return bool.TryParse(value, out bool bValue);
            case ProgVariableTypeCode.MudDateTime:
                return MudDateTime.TryParse(value, Gameworld, out MudDateTime mdtValue);
        }

        return false;
    }

    #endregion
}
