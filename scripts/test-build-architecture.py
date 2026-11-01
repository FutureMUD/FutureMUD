"""Exercise migration MSBuild integration after building MigrationModelCompiler (Debug)."""

import json
from pathlib import Path
import subprocess
import tempfile
import unittest
import xml.etree.ElementTree as ET


ROOT = Path(__file__).resolve().parent.parent
COMPILER = ROOT / "BuildTools/MigrationModelCompiler/bin/Debug/net10.0/MigrationModelCompiler.dll"


class MigrationBuildTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory(prefix="futuremud-build-")
        self.addCleanup(self.directory.cleanup)
        # MSBuild expands Windows 8.3 aliases when reporting generated source paths.
        self.root = Path(self.directory.name).resolve()
        self.migrations = self.root / "MudsharpDatabaseLibrary/Migrations"
        self.migrations.mkdir(parents=True)
        self.models = self.root / "MigrationModels"
        (self.models / "Batch0001").mkdir(parents=True)
        (self.models / "Batch0001/Batch0001.csproj").write_text("<Project />", encoding="utf-8")
        self.output = self.root / "obj/migration-models"
        self.harness = self.root / "test.proj"
        project = ET.Element("Project")
        ET.SubElement(project, "Import", Project=str(ROOT / "MigrationModels/MigrationModelGeneration.targets"))
        properties = ET.SubElement(project, "PropertyGroup")
        for key, value in {
            "MigrationRepositoryRoot": self.root,
            "MigrationCompilerDll": COMPILER,
            "MigrationModelRole": "Batch0001",
            "IntermediateOutputPath": str(self.root / "obj") + "/",
        }.items():
            ET.SubElement(properties, key).text = str(value)
        # No SDK or project restore is required for this target-level integration probe.
        references = ET.SubElement(project, "Target", Name="ResolveProjectReferences")
        ET.SubElement(references, "Error", Condition="'$(DesignTimeBuild)' == 'true'",
                      Text="Design-time generation must not resolve/build generator dependencies.")
        compile_target = ET.SubElement(project, "Target", Name="CoreCompile")
        ET.SubElement(compile_target, "WriteLinesToFile", File=str(self.root / "compile.txt"),
                      Lines="@(Compile)", Overwrite="true")
        ET.SubElement(compile_target, "Error", Condition="'$(DisableFastUpToDateCheck)' != 'true'",
                      Text="IDE must invoke MSBuild to validate external generation inputs.")
        ET.ElementTree(project).write(self.harness, encoding="unicode")

    def build(self, design_time=False, *properties):
        result = subprocess.run([
            "dotnet", "msbuild", str(self.harness), "-t:CoreCompile", "-m:1", "-v:q", "-nologo",
            "-p:DesignTimeBuild=" + str(design_time).lower(), *properties,
        ], capture_output=True, text=True, timeout=60)
        self.assertEqual(0, result.returncode, result.stdout + result.stderr)
        return (self.root / "compile.txt").read_text(encoding="utf-8-sig").splitlines()

    def designer(self, name, value):
        file = name + ".Designer.cs"
        (self.migrations / file).write_text(
            "using Microsoft.EntityFrameworkCore;\nusing MudSharp.Database;\n"
            "namespace MudSharp.Migrations { [DbContext(typeof(FuturemudDatabaseContext))] "
            f'[Migration("20260101000000_{name}")] partial class {name} {{ '
            "protected override void BuildTargetModel(ModelBuilder modelBuilder) { "
            f'modelBuilder.HasAnnotation("value", "{value}");'
            " } } }", encoding="utf-8")
        (self.models / "partitions.json").write_text(json.dumps({"Files": {
            p.name: "Batch0001" for p in sorted(self.migrations.glob("*.Designer.cs"))
        }}), encoding="utf-8")
        return self.output / (file + ".g.cs")

    def test_design_time_needs_no_generator_manifest_or_existing_outputs(self):
        self.assertEqual([], self.build(True, "-p:MigrationCompilerDll=does-not-exist.dll"))
        self.assertFalse(self.output.exists())

    def test_real_build_tracks_added_changed_deleted_inputs_and_repairs_outputs(self):
        self.assertTrue(COMPILER.exists(), "Build MigrationModelCompiler in Debug before this test.")
        first = self.designer("First", "original")
        self.assertEqual([str(first)], self.build())
        timestamp = first.stat().st_mtime_ns
        self.build()
        self.assertEqual(timestamp, first.stat().st_mtime_ns)
        second = self.designer("Second", "new migration")
        self.assertEqual({str(first), str(second)}, set(self.build()))
        self.designer("First", "changed")
        self.build()
        self.assertIn('"changed"', first.read_text(encoding="utf-8"))
        first.unlink()
        self.build()
        self.assertTrue(first.exists())
        (self.migrations / "Second.Designer.cs").unlink()
        self.designer("First", "changed")
        self.assertEqual([str(first)], self.build())
        self.assertFalse(second.exists())
        # Cached runtime projections must also stay out of the language service.
        self.assertEqual([], self.build(True, "-p:MigrationCompilerDll=does-not-exist.dll"))


if __name__ == "__main__":
    unittest.main()
