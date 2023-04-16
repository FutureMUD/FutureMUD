using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body.PartProtos
{
	public class NonImmobilisingBonyDrapeableBodypartProto : BonyDrapeableBodypartProto
	{
		/// <inheritdoc />
		public NonImmobilisingBonyDrapeableBodypartProto(BodypartProto proto, IFuturemud game) : base(proto, game)
		{
		}

		/// <inheritdoc />
		protected NonImmobilisingBonyDrapeableBodypartProto(BonyDrapeableBodypartProto rhs, string newName) : base(rhs, newName)
		{
		}

		#region Overrides of BonyDrapeableBodypartProto

		/// <inheritdoc />
		public override bool CanBeImmobilised => false;

		public override BodypartTypeEnum BodypartType => BodypartTypeEnum.BonyDrapeable;

		#endregion
	}
}
