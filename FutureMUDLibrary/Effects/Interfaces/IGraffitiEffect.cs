using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Communication;
using MudSharp.Communication.Language;

#nullable enable
namespace MudSharp.Effects.Interfaces;
public interface IGraffitiEffect : IEffect
{
	IWriting? Writing { get; }
	IDrawing? Drawing { get; }
	string? LocaleDescription { get; }
}
