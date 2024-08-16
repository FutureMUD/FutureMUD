using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
	public interface IArmourType : IEditableItem {
		IDamage AbsorbDamage(IDamage damage, IArmour armour, IHaveWounds owner, ref List<IWound> wounds, bool passive);

		(IDamage partDamage, IDamage organDamge) AbsorbDamage(IDamage damage, ItemQuality quality, IMaterial material, IHaveWounds owner,
			ref List<IWound> wounds);

		(IDamage PassedOn, IDamage Absorbed) AbsorbDamageViaSpell(IDamage damage, ISolid solid, ItemQuality quality,
																  IHaveWounds owner,
																  bool passive);

		double BaseDifficultyBonus { get; }
		double StackedDifficultyBonus { get; }
		OpposedOutcomeDegree MinimumPenetrationDegree { get; }
		IArmourType Clone(string newName);
	}
}