using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.CharacterCreation;

public class CharacterIntro : ICharacterIntro
{
	public Queue<string> Echoes { get; set; }
	public Queue<TimeSpan> Delays { get; set; }
}