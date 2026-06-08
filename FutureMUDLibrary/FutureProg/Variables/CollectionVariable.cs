using MudSharp.Framework;
using MudSharp.Form.Shape;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Variables
{
    public class CollectionVariable : ProgVariable, IEnumerable
    {
        private readonly IList<IProgVariable> _underlyingList;

        private readonly ProgVariableTypes _underlyingType;

        public CollectionVariable(IList underlyingList, ProgVariableTypes underlyingType)
        {
            _underlyingList = underlyingList?.Cast<object>().Select(x => NormaliseCollectionItem(x, underlyingType)).ToList() ?? new List<IProgVariable>();
            _underlyingType = underlyingType;
        }

        public override ProgVariableTypes Type => ProgVariableTypes.Collection | _underlyingType;

        public override object GetObject => _underlyingList;

        private static IProgVariable NormaliseCollectionItem(object item, ProgVariableTypes underlyingType)
        {
            if (item is IProgVariable variable)
            {
                return variable;
            }

            if (item is null)
            {
                return new NullVariable(underlyingType);
            }

            switch (underlyingType.LegacyCode)
            {
                case ProgVariableTypeCode.Boolean:
                    return item is bool boolean ? new BooleanVariable(boolean) : new NullVariable(underlyingType);
                case ProgVariableTypeCode.Gender:
                    return item is Gender gender ? new GenderVariable(gender) : new NullVariable(underlyingType);
                case ProgVariableTypeCode.Number:
                    try
                    {
                        return new NumberVariable(Convert.ToDecimal(item));
                    }
                    catch (Exception)
                    {
                        return new NullVariable(underlyingType);
                    }
                case ProgVariableTypeCode.Text:
                    return new TextVariable(item.ToString() ?? string.Empty);
                case ProgVariableTypeCode.TimeSpan:
                    return item is TimeSpan timeSpan ? new TimeSpanVariable(timeSpan) : new NullVariable(underlyingType);
                case ProgVariableTypeCode.DateTime:
                    return item is DateTime dateTime ? new DateTimeVariable(dateTime) : new NullVariable(underlyingType);
                case ProgVariableTypeCode.MudDateTime:
                    return item is MudDateTime mudDateTime ? mudDateTime : new NullVariable(underlyingType);
                default:
                    return new NullVariable(underlyingType);
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _underlyingList.GetEnumerator();
        }

        #endregion

        private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler(ProgVariableTypes type)
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", ProgVariableTypes.Number},
                {"any", ProgVariableTypes.Boolean},
                {"empty", ProgVariableTypes.Boolean},
                { "first", type},
                { "last", type},
                { "reverse", ProgVariableTypes.Collection | type},
            };
        }

        private static IReadOnlyDictionary<string, string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", "The number of items in the collection"},
                {"any", "True if there are any items in the collection"},
                {"empty", "True if the collection is empty"},
                { "first", "The first item in the collection or null if empty"},
                { "last", "The last item in the collection or null if empty"},
                { "reverse", "The collection in reverse order"},
            };
        }

        public static void RegisterFutureProgCompiler()
        {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Collection, DotReferenceHandler(ProgVariableTypes.Void), DotReferenceHelp());
            foreach (ProgVariableTypes flag in ProgVariableTypes.CollectionItem.GetAllFlags())
            {
                if (flag == ProgVariableTypes.Void)
                {
                    continue;
                }
                ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Collection | flag, DotReferenceHandler(flag), DotReferenceHelp());
            }
        }

        public override IProgVariable GetProperty(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "count":
                    return new NumberVariable(_underlyingList.Count);
                case "any":
                    return new BooleanVariable(_underlyingList.Count > 0);
                case "empty":
                    return new BooleanVariable(_underlyingList.Count == 0);
                case "first":
                    return _underlyingList.Count > 0 ? _underlyingList[0] : new NullVariable(_underlyingType);
                case "last":
                    return _underlyingList.Count > 0 ? _underlyingList[^1] : new NullVariable(_underlyingType);
                case "reverse":
                    List<IProgVariable> list = new();
                    for (int i = _underlyingList.Count - 1; i >= 0; i--)
                    {
                        list.Add(_underlyingList[i]);
                    }

                    return new CollectionVariable(list, _underlyingType);

            }
            throw new NotSupportedException("Invalid property requested in CollectionVariable.GetProperty");
        }
    }
}
