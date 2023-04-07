using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Commands.Trees;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

public class MagicSpellLockout : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicSpellLockout", (effect, owner) => new MagicSpellLockout(effect, owner));
	}

	private readonly List<IMagicSchool> _magicSchools = new();

	public MagicSpellLockout(IPerceivable owner, IEnumerable<IMagicSchool> schools) : base(owner, null)
	{
		_magicSchools.AddRange(schools);
	}

	protected MagicSpellLockout(XElement root, IPerceivable owner) : base(root, owner)
	{
		foreach (var item in root.Elements())
		{
			_magicSchools.Add(Gameworld.MagicSchools.Get(long.Parse(item.Value)));
		}
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		if (_magicSchools.Count == 0)
		{
			return "Locked out from all spell casting";
		}

		return
			$"Locked out from spell casting in the {_magicSchools.Select(x => x.Name.Colour(x.PowerListColour)).ListToString()} school{(_magicSchools.Count == 1 ? "" : "s")}";
	}

	protected override string SpecificEffectType => "MagicSpellLockout";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			from school in _magicSchools
			select new XElement("School", school.Id)
		);
	}

	public override bool Applies(object target)
	{
		if (target is IMagicSchool school)
		{
			return _magicSchools.Count == 0 || _magicSchools.Contains(school);
		}

		return base.Applies(target);
	}

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		Owner.OutputHandler.Send(
			$"You are no longer locked out from casting {(_magicSchools.Any() ? _magicSchools.Select(x => x.Name.Colour(x.PowerListColour)).ListToString() : "all")} spells.");
	}

	#endregion
}