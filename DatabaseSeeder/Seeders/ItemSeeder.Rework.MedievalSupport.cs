#nullable enable

using MudSharp.Form.Material;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void EnsureMedievalItemMaterialAndTags(string material, MaterialBehaviourType materialType, IEnumerable<string> tags)
	{
		EnsureAntiquityComponentGapMaterial(material, materialType);
		foreach (var tag in tags)
		{
			EnsureAntiquityTagPath(tag);
		}
	}
}