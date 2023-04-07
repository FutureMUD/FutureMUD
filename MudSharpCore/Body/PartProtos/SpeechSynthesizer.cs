using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.Body.PartProtos;

public class SpeechSynthesizer : InternalOrganProto
{
	protected SpeechSynthesizer(InternalOrganProto rhs, string newName) : base(rhs, newName)
	{
	}

	public SpeechSynthesizer(BodypartProto proto, IFuturemud game) : base(proto, game)
	{
	}

	public override BodypartTypeEnum BodypartType => BodypartTypeEnum.SpeechSynthesizer;

	public override IBodypart Clone(string newName)
	{
		return new SpeechSynthesizer(this, newName);
	}

	public override string FrameworkItemType => "SpeechSynthesizer";
}