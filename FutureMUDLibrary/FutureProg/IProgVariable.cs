using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg {
    [Flags]
    public enum ProgVariableTypes : long {
        /// <summary>
        ///     Used as a return type for programs that do not return anything. Will always evaluate to null.
        /// </summary>
        Void = 0L,

        /// <summary>
        ///     Variable contains textual information. GetObject will return a nullable string.
        /// </summary>
        Text = 1L << 0,

        /// <summary>
        ///     Variable contains a number. GetObject will return a nullable double.
        /// </summary>
        Number = 1L << 1,

        /// <summary>
        ///     Variable contains boolean information. GetObject will return a nullable bool.
        /// </summary>
        Boolean = 1L << 2,

        /// <summary>
        ///     Variable contains a character. GetObject will return an ICharacter.
        /// </summary>
        Character = 1L << 3,

        /// <summary>
        ///     Variable contains a cell. GetObject will return an ICell.
        /// </summary>
        Location = 1L << 4,

        /// <summary>
        ///     Variable contains a game item. GetObject will return an IGameItem.
        /// </summary>
        Item = 1L << 5,

        /// <summary>
        ///     Variable contains an IShard. GetObject will return an IShard.
        /// </summary>
        Shard = 1L << 6,

        /// <summary>
        ///     Used mostly at compile time to indicate an erronous function result
        /// </summary>
        Error = 1L << 7,

        /// <summary>
        ///     Variable contains a gender. GetObject will return a Gender.
        /// </summary>
        Gender = 1L << 8,

        /// <summary>
        ///     Variable contains an IZone. GetObject will return an IZone.
        /// </summary>
        Zone = 1L << 9,

        /// <summary>
        ///     Variable contains a collection of another variable type. GetObject will return an IList of the appropriate type.
        /// </summary>
        Collection = 1L << 10,

        /// <summary>
        ///     Variable contains an IRace. GetObject will return an IRace.
        /// </summary>
        Race = 1L << 11,

        /// <summary>
        ///     Variable contains an ICulture. GetObject will return an ICulture.
        /// </summary>
        Culture = 1L << 12,

        /// <summary>
        ///     Variable contains a Chargen. GetObject will return a Chargen.
        /// </summary>
        Chargen = 1L << 13,

        /// <summary>
        ///     Variable contains a TraitDefinition. GetObject will return a TraitDefinition.
        /// </summary>
        Trait = 1L << 14,

        /// <summary>
        ///     Variables contains an IClan. GetObject will return an IClan
        /// </summary>
        Clan = 1L << 15,

        ClanRank = 1L << 16,

        ClanAppointment = 1L << 17,

        ClanPaygrade = 1L << 18,

        Currency = 1L << 19,

        /// <summary>
        ///     Variable contains an ICellExit. GetObject will return an ICellExit.
        /// </summary>
        Exit = 1L << 20,

        /// <summary>
        ///     A literal is a modifier designed to be used with other types signifying it contains a compile-time constant (e.g. a
        ///     literal)
        /// </summary>
        Literal = 1L << 21,

        /// <summary>
        ///     A DateTime is a FutureProgType representing the built in DateTime type. It refers to real-world DateTime (not in
        ///     game time).
        /// </summary>
        DateTime = 1L << 22,

        /// <summary>
        ///     A TimeSpan is a FutureProgType representing the built in TimeSpan type.
        /// </summary>
        TimeSpan = 1L << 23,

        /// <summary>
        ///     A Language is a FutureProgType representing the Language type. GetObject will return a Language.
        /// </summary>
        Language = 1L << 24,

        /// <summary>
        ///     An Accent is a FutureProgType representing the Accent type. GetObject will return an Accent.
        /// </summary>
        Accent = 1L << 25,

        /// <summary>
        ///     A Merit is a FutureProgType representing an IMerit type. Getobject will return an IMerit.
        /// </summary>
        Merit = 1L << 26,

        /// <summary>
        ///     A MudDateTime is a FutureProgType representing an in-game date time object. GetObject will return a MudDateTime.
        /// </summary>
        MudDateTime = 1L << 27,

        /// <summary>
        ///     A Calendar is a FutureProgType representing an in-game Mud Calendar. GetObject will return a Calendar.
        /// </summary>
        Calendar = 1L << 28,

        /// <summary>
        ///     A Clock is a FutureProgType representing an in-game Mud Clock. GetObject will return a Clock.
        /// </summary>
        Clock = 1L << 29,

        /// <summary>
        ///     An Effect is a FutureProgType representing an in-game Effect. GetObject will return an IEffect
        /// </summary>
        Effect = 1L << 30,

        /// <summary>
        ///     A Knowledge if a FutureProgType representing an in-game Knowledge. GetObject will return an IKnowledge
        /// </summary>
        Knowledge = 1L << 31,

        /// <summary>
        ///     A Role is a FutureProgType representing a chargen role. GetObject will return an IChargenRole
        /// </summary>
        Role = 1L << 32,

        Ethnicity = 1L << 33,

        Drug = 1L << 34,

        /// <summary>
        /// A WeatherEvent is a FutureProgtype representing a current weather situation. GetObject will return an IWeatherEvent
        /// </summary>
        WeatherEvent = 1L << 35,

        Shop = 1L << 36,

        Merchandise = 1L << 37,

        Outfit = 1L << 38,

        OutfitItem = 1L << 39,

        /// <summary>
        /// A Project represents and IActiveProject.
        /// </summary>
        Project = 1L << 40,

        OverlayPackage = 1L << 41,

        Terrain = 1L << 42,

        Solid = 1L << 43,

        Liquid = 1L << 44,

        Gas = 1L << 45,

        /// <summary>
        /// Represents a lookup with a text key and another item as a collection. GetObject returns a Dictionary[string, IFutureProgVariable]
        /// </summary>
        Dictionary = 1L << 46,


        /// <summary>
        /// Represents a lookup with a text key and a collection of the other item. GetObject returns a CollectionDictionary[string,IFutureProgVariable]
        /// </summary>
        CollectionDictionary = 1L << 47,

        MagicSpell = 1L << 48,

        MagicSchool = 1L << 49,

        MagicCapability = 1L << 50,

        Bank = 1L << 51,

        BankAccount = 1L << 52,

        BankAccountType = 1L << 53,

        LegalAuthority = 1L << 54,

        Law = 1L << 55,

        Crime = 1L << 56,

        Market = 1L << 57,

        MarketCategory = 1L << 58,

        LiquidMixture = 1L << 59,

        #region Special Flag Combinations

        /// <summary>
        ///     Any item that is not a collection or an error.
        /// </summary>
        CollectionItem =
            0L | Number | Boolean | Gender | Text | DateTime | TimeSpan | Character | Item | Chargen | Location | Zone |
            Shard | Accent | Language | Race | Culture | Trait | Clan | ClanRank | ClanAppointment | ClanPaygrade |
            Currency | Exit | Merit | MudDateTime | Calendar | Clock | Effect | Knowledge | Role | Ethnicity | Drug | 
            WeatherEvent | Shop | Merchandise | Outfit | OutfitItem | OverlayPackage | Terrain | Project |
            Solid | Liquid | Gas | MagicSchool | MagicCapability | MagicSpell | Bank | BankAccount | BankAccountType | LegalAuthority | Law | Crime | Market | MarketCategory | LiquidMixture
            ,

        /// <summary>
        ///     Any item that implements the IPerceivable interface
        /// </summary>
        Perceivable = Item | Character | Location | Zone | Shard,

        /// <summary>
        ///     Any item that implements the IPerceiver interface
        /// </summary>
        Perceiver = Item | Character,

        /// <summary>
        /// Any item that implements the IHaveMagicResources interface
        /// </summary>
        MagicResourceHaver = Item | Character | Location,

        /// <summary>
        ///     A reference type in FutureProgVariables represents something that maps to a single C# Reference, non-collection
        ///     type, as opposed to a Value-Type wrapper
        /// </summary>
        ReferenceType =
            long.MaxValue ^
            (Text | Number | DateTime | TimeSpan | Literal | Boolean | Error | Gender | Collection | Dictionary | CollectionDictionary | MudDateTime | LiquidMixture),

        ValueType = Text | Number | Boolean | DateTime | TimeSpan | Literal | Gender | MudDateTime | LiquidMixture,

        /// <summary>
        ///     Anything that is not an error
        /// </summary>
        Anything = long.MaxValue ^ Error,

        /// <summary>
        ///     A Toon is either a Character or a Chargen, as they share some of the same properties. Works out to 8200
        /// </summary>
        Toon = Character | Chargen,

        /// <summary>
        /// A Tagged is anything that implements IHaveTags
        /// </summary>
        Tagged = Location | Item | Terrain,

        /// <summary>
        /// A Material is any IMaterial, potentially a liquid solid or gas
        /// </summary>
        Material = Solid | Gas | Liquid,

        #endregion
    }

    public static class FutureProgVariableExtensions {
        /// <summary>
        ///     This function determines whether Type1 is compatible as a variable with Type2. That is to say that Type1 can be assigned to a variable of type Type2.
        /// </summary>
        /// <param name="type1">The variable type whose Compatability is in question</param>
        /// <param name="type2">The variable type to which type1 is desired to be evaluated</param>
        /// <returns>True if the types are compatible</returns>
        public static bool CompatibleWith(this ProgVariableTypes type1, ProgVariableTypes type2) {
            return FutureProgVariableComparer.Instance.Equals(type2, type1);
        }
    }

    public class FutureProgVariableComparer : EqualityComparer<ProgVariableTypes> {
        private FutureProgVariableComparer() {
        }

        public static FutureProgVariableComparer Instance { get; } = new();

        public override bool Equals(ProgVariableTypes x, ProgVariableTypes y) {
            if (x.HasFlag(ProgVariableTypes.Collection) != y.HasFlag(ProgVariableTypes.Collection))
            {
                return false;
            }

            if (x.HasFlag(ProgVariableTypes.Collection))
            {
                return Equals(x & ~ProgVariableTypes.Collection, y & ~ProgVariableTypes.Collection) || x == ProgVariableTypes.Collection;
            }

            if (x.HasFlag(ProgVariableTypes.Dictionary) != y.HasFlag(ProgVariableTypes.Dictionary))
            {
                return false;
            }

            if (x.HasFlag(ProgVariableTypes.Dictionary))
            {
                return Equals(x & ~ProgVariableTypes.Dictionary, y & ~ProgVariableTypes.Dictionary) || x == ProgVariableTypes.Dictionary;
            }

            if (x.HasFlag(ProgVariableTypes.CollectionDictionary) != y.HasFlag(ProgVariableTypes.CollectionDictionary))
            {
                return false;
            }

            if (x.HasFlag(ProgVariableTypes.CollectionDictionary))
            {
                return Equals(x & ~ProgVariableTypes.CollectionDictionary, y & ~ProgVariableTypes.CollectionDictionary) || x == ProgVariableTypes.CollectionDictionary;
            }

            return x.HasFlag(ProgVariableTypes.Literal) ? x.HasFlag(y) : x.HasFlag(y & ~ProgVariableTypes.Literal);
        }

        public override int GetHashCode(ProgVariableTypes obj) {
            return (int) obj;
        }
    }

    /// <summary>
    ///     A class implementing the IFutureProgVariable interface can be used as a variable in a FutureProg. If it is the
    ///     "Owner" class of its FutureProgVariableType, it must also implement a compiler-registerer.
    /// </summary>
    public interface IProgVariable {
        /// <summary>
        ///     The FutureProgVariableType that represents this IFutureProgVariable
        /// </summary>
        ProgVariableTypes Type { get; }

        /// <summary>
        ///     Returns an object representing the underlying variable wrapped in this IFutureProgVariable
        /// </summary>
        object GetObject { get; }

        /// <summary>
        ///     Requests an IFutureProgVariable representing the property referenced by the given string.
        /// </summary>
        /// <param name="property">A string representing the property to be retrieved</param>
        /// <returns>An IFutureProgVariable representing the desired property</returns>
        IProgVariable GetProperty(string property);
    }
}