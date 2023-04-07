using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class StoredEditorText : Effect, IEffectSubtype
{
	public string Text { get; set; }

	public StoredEditorText(IPerceivable owner, string text, IFutureProg applicabilityProg = null) : base(owner,
		applicabilityProg)
	{
		Text = text;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Has stored editor text from a recent trip to the editor.";
	}

	protected override string SpecificEffectType => "StoredEditorText";
}