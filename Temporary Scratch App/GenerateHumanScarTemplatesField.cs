using System.Text;
using System.Text.Json;

namespace Temporary_Work_App;

public class GenerateHumanScarTemplatesField
{
    public string Generate(string json)
    {
        using var document = JsonDocument.Parse(json);

        var templates = document.RootElement.GetProperty("Templates");

        static string CsString(string value)
            => "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        static string FormatDouble(double value)
            => value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

        var writer = new StringBuilder();

        writer.AppendLine("private static readonly IReadOnlyList<SeederScarTemplateDefinition> HumanScarTemplates =");
        writer.AppendLine("[");

        var count = templates.GetArrayLength();
        for (var i = 0; i < count; i++)
        {
            var template = templates[i];
            writer.AppendLine("    new SeederScarTemplateDefinition(");
            writer.AppendLine($"        {CsString(template.GetProperty("Name").GetString() ?? string.Empty)},");
            writer.AppendLine($"        {CsString(template.GetProperty("ShortDescription").GetString() ?? string.Empty)},");
            writer.AppendLine($"        {CsString(template.GetProperty("FullDescription").GetString() ?? string.Empty)},");
            writer.AppendLine($"        SizeSteps: {template.GetProperty("SizeSteps").GetInt32()},");
            writer.AppendLine($"        Distinctiveness: {template.GetProperty("Distinctiveness").GetInt32()},");
            writer.AppendLine($"        DamageHealingScarWeight: {FormatDouble(template.GetProperty("DamageHealingScarChance").GetDouble())},");
            writer.AppendLine("        DamageTypes: new Dictionary<DamageType, WoundSeverity>");
            writer.AppendLine("        {");

            foreach (var kvp in template.GetProperty("DamageTypes").EnumerateObject())
            {
                writer.AppendLine($"            [DamageType.{kvp.Name}] = WoundSeverity.{kvp.Value.GetString()},");
            }

            writer.AppendLine("        },");

            if (template.TryGetProperty("BodypartShapeNames", out var shapes) && shapes.ValueKind == JsonValueKind.Array && shapes.GetArrayLength() > 0)
            {
                var shapeValues = string.Join(", ", shapes.EnumerateArray().Select(x => CsString(x.GetString() ?? string.Empty)));
                writer.AppendLine($"        BodypartShapeNames: [{shapeValues}]");
            }
            else if (template.TryGetProperty("BodypartAliases", out var aliases) && aliases.ValueKind == JsonValueKind.Array && aliases.GetArrayLength() > 0)
            {
                var aliasValues = string.Join(", ", aliases.EnumerateArray().Select(x => CsString(x.GetString() ?? string.Empty)));
                writer.AppendLine($"        BodypartAliases: [{aliasValues}]");
            }
            else
            {
                writer.AppendLine("        BodypartShapeNames: []");
            }

            writer.AppendLine(i < count - 1 ? "    )," : "    )");
            writer.AppendLine();
        }

        writer.AppendLine("];\n");
        return writer.ToString();
    }
}