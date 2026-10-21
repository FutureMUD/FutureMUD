using MudSharp.Framework;
using System.Xml.Linq;

using MudSharp.FutureProg;

namespace MudSharp.Character.Name
{

    public enum NameUsage
    {
        /// <summary>
        ///     A name given at birth
        /// </summary>
        BirthName = 0,

        /// <summary>
        ///     A dimunative form of the primary name
        /// </summary>
        Dimunative = 1,

        /// <summary>
        ///     A nickname given to somebody
        /// </summary>
        Nickname = 2,

        /// <summary>
        ///     Additional middle names that are not primary forms of address
        /// </summary>
        MiddleName = 3,

        /// <summary>
        ///     A name given to somebody upon becoming an adult
        /// </summary>
        AdultName = 4,

        /// <summary>
        ///     A name given to somebody as a child and generally supplanted by another name later in life
        /// </summary>
        ChildName = 5,

        /// <summary>
        ///     A family name
        /// </summary>
        Surname = 6,

        /// <summary>
        ///     A name derived from one's father
        /// </summary>
        Patronym = 7,

        /// <summary>
        ///     A name derived from one's mother
        /// </summary>
        Matronym = 8,

        /// <summary>
        ///     The name of a group of families
        /// </summary>
        FamilyGroupName = 9,

        /// <summary>
        /// The name of all children of a particular generation in a family
        /// </summary>
        GenerationName = 10,

        /// <summary>
        /// A name adopted upon inheriting a feudal title
        /// </summary>
        RegnalName = 11,

        /// <summary>
        /// A name used only in certain sacred contexts
        /// </summary>
        SacredName = 12,

        /// <summary>
        /// A toponym is a name derived from a place
        /// </summary>
        Toponym = 13,

        /// <summary>
        /// An owner name is a name given to you by your owner or master
        /// </summary>
        OwnerName = 14,

		/// <summary>A form of address, without conferring a role or rank.</summary>
		Title = 15,
		/// <summary>A declared association, without conferring membership.</summary>
		Affiliation = 16,
		/// <summary>A clan within a separately recorded family group or tribe.</summary>
		ClanName = 17,
		/// <summary>A name earned by a declared achievement, distinct from adulthood.</summary>
		EarnedName = 18,
    }

    public interface IPersonalName : IFrameworkItem, IHaveFuturemud, IXmlSavable, IProgVariable
    {
        INameCulture Culture { get; }
		XElement EntryProgress => null;
        string GetName(NameStyle style);
    }
}
