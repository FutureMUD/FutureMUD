using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg
{
    public static class FutureProgVariableExtensions
    {
        /// <summary>
        ///     This function determines whether Type1 is compatible as a variable with Type2. That is to say that Type1 can be assigned to a variable of type Type2.
        /// </summary>
        /// <param name="type1">The variable type whose Compatability is in question</param>
        /// <param name="type2">The variable type to which type1 is desired to be evaluated</param>
        /// <returns>True if the types are compatible</returns>
        public static bool CompatibleWith(this ProgVariableTypes type1, ProgVariableTypes type2)
        {
            return FutureProgVariableComparer.Instance.Equals(type2, type1);
        }
    }

    public class FutureProgVariableComparer : EqualityComparer<ProgVariableTypes>
    {
        private FutureProgVariableComparer()
        {
        }

        public static FutureProgVariableComparer Instance { get; } = new();

        public override bool Equals(ProgVariableTypes x, ProgVariableTypes y)
        {
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

        public override int GetHashCode(ProgVariableTypes obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    ///     A class implementing the IFutureProgVariable interface can be used as a variable in a FutureProg. If it is the
    ///     "Owner" class of its FutureProgVariableType, it must also implement a compiler-registerer.
    /// </summary>
    public interface IProgVariable
    {
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
