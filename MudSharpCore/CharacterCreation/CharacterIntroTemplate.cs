using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.CharacterCreation;

public class CharacterIntroTemplate : FrameworkItem, ICharacterIntroTemplate
{
	public CharacterIntroTemplate(MudSharp.Models.CharacterIntroTemplate template, IFuturemud gameworld)
	{
		_id = template.Id;
		_name = template.Name;
		AppliesToCharacterProg = gameworld.FutureProgs.Get(template.AppliesToCharacterProgId);
		ResolutionPriority = template.ResolutionPriority;
		var root = XElement.Parse(template.Definition);
		foreach (var item in root.Elements("Echo"))
		{
			Echoes.Add(item.Value);
			Delays.Add(TimeSpan.FromSeconds(double.Parse(item.Attribute("delay").Value)));
		}
	}

	public override string FrameworkItemType => "CharacterIntroTemplate";

	public bool AppliesToCharacter(ICharacterTemplate template)
	{
		return (bool?)AppliesToCharacterProg.Execute(template) == true;
	}

	public bool AppliesToCharacter(ICharacter character)
	{
		return (bool?)AppliesToCharacterProg.Execute(character) == true;
	}

	public IFutureProg AppliesToCharacterProg { get; private set; }

	public int ResolutionPriority { get; private set; }

	public List<string> Echoes { get; } = new();

	public List<TimeSpan> Delays { get; } = new();

	public ICharacterIntro GetCharacterIntro()
	{
		return new CharacterIntro
		{
			Echoes = new Queue<string>(Echoes),
			Delays = new Queue<TimeSpan>(Delays)
		};
	}
}