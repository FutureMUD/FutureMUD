#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

/// <summary>Concentration and resource-sustained room psionic ward.</summary>
public sealed class PsionicDomeEffect : PsionicSustainedPowerEffectBase<MudSharp.Magic.Powers.PsionicDomePower>, IPsionicDomeEffect
{
	public static void InitialiseEffectType() => RegisterFactory("PsionicDome", (effect, owner) => new PsionicDomeEffect(effect, owner));
	public PsionicDomeEffect(ICharacter owner, MudSharp.Magic.Powers.PsionicDomePower power) : base(owner, power) => RoomId = owner.Location?.Id ?? 0;
	private PsionicDomeEffect(XElement effect, IPerceivable owner) : base(effect, owner) => RoomId = long.Parse(effect.Element("Effect")?.Element("RoomId")?.Value ?? "0");
	protected override void RegisterEvents()
	{
		base.RegisterEvents();
		CharacterOwner.OnLocationChanged += CharacterOwnerOnLocationChanged;
	}
	public override void ReleaseEvents()
	{
		base.ReleaseEvents();
		CharacterOwner.OnLocationChanged -= CharacterOwnerOnLocationChanged;
	}
	private void CharacterOwnerOnLocationChanged(MudSharp.Form.Shape.ILocateable _, MudSharp.Construction.Boundary.IRoomExit? __)
	{
		if (CharacterOwner.Location?.Id != RoomId) CharacterOwner.RemoveEffect(this, true);
	}
	public ICharacter OwnerCharacter => CharacterOwner;
	public long RoomId { get; }
	public bool AppliesTo(ICharacter character) => RoomId > 0 && character.Location?.Id == RoomId;
	public override void InitialEffect()
	{
		base.InitialEffect();
		if (RoomId <= 0) return;
		foreach (var character in Gameworld.Characters.Where(x => x.Location?.Id == RoomId).ToList())
		{
			if (character.EffectsOfType<ISpellProjectionBoundary>().Any(x => x.Kind == SpellProjectionKind.WalkingShadow))
			{
				Gameworld.SpellOwnedProjections?.RequestRetirement(character, SpellRetirementReason.AnchorSevered);
			}
		}

		// The source dome progressively drove down existing outside contacts. The native
		// mapping deterministically ends links crossing into its warded room.
		foreach (var originator in Gameworld.Characters.ToList())
		{
			foreach (var contact in originator.EffectsOfType<ConnectMindEffect>()
				.Where(x => (x.TargetCharacter.Location?.Id == RoomId) != (originator.Location?.Id == RoomId)).ToList())
			{
				originator.RemoveEffect(contact, true);
			}
		}
	}
	protected override string SpecificEffectType => "PsionicDome";
	public override string Describe(IPerceiver voyeur) => $"A psionic dome is sustained over room #{RoomId.ToString("N0", voyeur)}.";
	protected override XElement SaveDefinition() => SaveToXml(new XElement("Power", Power.Id), new XElement("RoomId", RoomId));
}
