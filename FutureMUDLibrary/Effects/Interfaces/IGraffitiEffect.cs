using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Framework;

#nullable enable
namespace MudSharp.Effects.Interfaces;
public interface IGraffitiEffect : IEffect, IKeyworded
{
	IGraffitiWriting? Writing { get; }
	string? LocaleDescription { get; }
	RoomLayer Layer { get; }
	bool IsJustDrawing { get; }
}
