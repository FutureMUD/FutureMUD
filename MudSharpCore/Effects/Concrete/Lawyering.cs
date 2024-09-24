using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;
#nullable enable
public class Lawyering : Effect, IEffect
{
	#region Static Initialisation
	public static void InitialiseEffectType()
	{
		RegisterFactory("Lawyering", (effect, owner) => new Lawyering(effect, owner));
	}
	#endregion

	#region Constructors
	public Lawyering(ICharacter owner, ILegalAuthority authority) : base(owner, null)
	{
		LegalAuthority = authority;
	}

	protected Lawyering(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		_engagedByCharacterId = long.Parse(root.Element("EngagedBy").Value);
	}
	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("EngagedBy", _engagedByCharacterId ?? 0)
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "Lawyering";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Lawyering in the {LegalAuthority.Name.ColourName()} jurisdiction.";
	}

	public override bool SavingEffect => true;
	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	private long? _engagedByCharacterId;
	private ICharacter? _engagedByCharacter;

	public ICharacter? EngagedByCharacter
	{
		get
		{
			return _engagedByCharacter ??= Gameworld.TryGetCharacter(_engagedByCharacterId ?? 0, true);
		}
		set
		{
			_engagedByCharacter = value;
			_engagedByCharacterId = value?.Id;
		}
	}
}
