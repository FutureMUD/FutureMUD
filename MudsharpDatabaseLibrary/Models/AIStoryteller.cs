using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models;

public class AIStoryteller
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Model { get; set; }
	public string SystemPrompt { get; set; }
}
