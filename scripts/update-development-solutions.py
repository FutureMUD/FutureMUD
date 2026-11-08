"""Refresh IDE filters and serialize memory-heavy helpers without runtime references.

Run from any directory; --check validates generated files without writing them.
Only standard-library Python is required on Windows or POSIX.
"""

import argparse
import json
from pathlib import Path
import re
import uuid
import xml.etree.ElementTree as ET


ROOT = Path(__file__).resolve().parent.parent
SOLUTION = ROOT / "MudSharp.sln"
PROJECT = re.compile(
    r'^Project\("[^"\n]+"\) = "[^"\n]+", "([^"\n]+\.csproj)", "([^"\n]+)"\n.*?^EndProject$',
    re.MULTILINE | re.DOTALL,
)
def generate():
    source = SOLUTION.read_text(encoding="utf-8-sig")
    entries = {m[1].replace("\\", "/"): m for m in PROJECT.finditer(source)}
    dependencies = {}

    def project_dependencies(path):
        if path in dependencies:
            return dependencies[path]
        project = ROOT / path
        if not project.is_file():
            raise ValueError(f"Missing project: {path}")
        refs = set()
        for reference in ET.parse(project).iter("ProjectReference"):
            pattern = reference.attrib["Include"].replace("\\", "/")
            if "$" in pattern:
                raise ValueError(f"Unsupported computed reference in {path}: {pattern}")
            targets = list(project.parent.glob(pattern))
            if not targets:
                raise ValueError(f"Missing project reference in {path}: {pattern}")
            refs.update(target.resolve().relative_to(ROOT).as_posix() for target in targets)
        if path.startswith("MigrationModels/") or path == "MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj":
            refs.add("BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj")
        dependencies[path] = refs
        return refs

    for path in entries:
        if path.startswith(("ItemSeeder/", "ItemSeeder Unit Tests/", "SeederCatalogueTools/")):
            raise ValueError(f"Optional project belongs in ItemSeeder.sln: {path}")
        missing = project_dependencies(path) - entries.keys()
        if missing:
            raise ValueError(f"MudSharp.sln is missing dependencies of {path}: {sorted(missing)}")

    ordered = []
    visiting = set()
    visited = set()

    def visit(path):
        if path in visiting:
            raise ValueError(f"Project dependency cycle at {path}")
        if path in visited:
            return
        visiting.add(path)
        for dependency in sorted(dependencies[path]):
            visit(dependency)
        visiting.remove(path)
        visited.add(path)
        ordered.append(path)

    for path in sorted(entries):
        visit(path)

    # The solution scheduler ignores project-level BuildInParallel. A topologically
    # ordered chain constrains it without creating transitive assembly references.
    heavy = [p for p in ordered if p.startswith("MigrationModels/Batch")]
    previous = {path: entries[heavy[i - 1]][2] for i, path in enumerate(heavy) if i}

    def replace(match):
        path = match[1].replace("\\", "/")
        if path not in heavy:
            return match[0]
        block = re.sub(r'\tProjectSection\(ProjectDependencies\) = postProject\n.*?\tEndProjectSection\n',
                       '', match[0], flags=re.DOTALL)
        if path in previous:
            guid = previous[path]
            block = block.removesuffix("EndProject") + (
                "\tProjectSection(ProjectDependencies) = postProject\n"
                f"\t\t{guid} = {guid}\n\tEndProjectSection\nEndProject"
            )
        return block

    outputs = {SOLUTION: PROJECT.sub(replace, source)}
    for name, root in (("MudSharp.Engine.slnf", "MudSharpCore/MudSharpCore.csproj"),
                       ("MudSharp.Development.slnf", "DatabaseSeeder/DatabaseSeeder.csproj")):
        closure = set()

        def include(path):
            if path in closure:
                return
            closure.add(path)
            for dependency in dependencies[path]:
                include(dependency)

        include(root)
        outputs[ROOT / name] = json.dumps({"solution": {"path": "MudSharp.sln", "projects": [
            p.replace("/", "\\") for p in sorted(closure)
        ]}}, indent=2) + "\n"

    item_roots = [
        "ItemSeeder/ItemSeeder.csproj",
        "ItemSeeder Unit Tests/ItemSeeder Unit Tests.csproj",
        "SeederCatalogueTools/SeederCatalogueTools.csproj",
    ]
    item_projects = set()
    item_ordered = []

    def include_item_project(path):
        if path in item_projects:
            return
        item_projects.add(path)
        for dependency in sorted(project_dependencies(path)):
            include_item_project(dependency)
        item_ordered.append(path)

    for root in item_roots:
        include_item_project(root)

    project_type = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"
    heavy_projects = [path for path in item_ordered if path.startswith((
        "MigrationModels/Batch", "ItemSeeder/Seeders/ItemSeeder/Catalogue/Assemblies/"
    ))]

    def guid(path):
        if path in entries:
            return entries[path][2]
        return "{" + str(uuid.uuid5(uuid.NAMESPACE_URL, "FutureMUD/" + path)).upper() + "}"

    previous_heavy = dict(zip(heavy_projects[1:], heavy_projects))
    lines = ["Microsoft Visual Studio Solution File, Format Version 12.00", "# Visual Studio Version 17",
             "VisualStudioVersion = 17.0.31903.59", "MinimumVisualStudioVersion = 10.0.40219.1"]
    for path in sorted(item_projects, key=lambda path: (path != item_roots[0], path)):
        project_guid = guid(path)
        project_name = (ROOT / path).stem
        windows_path = path.replace("/", "\\")
        block = [f'Project("{project_type}") = "{project_name}", "{windows_path}", "{project_guid}"']
        if path in previous_heavy:
            previous_guid = guid(previous_heavy[path])
            block.extend([
                "\tProjectSection(ProjectDependencies) = postProject",
                f"\t\t{previous_guid} = {previous_guid}",
                "\tEndProjectSection",
            ])
        block.append("EndProject")
        lines.extend(block)
    lines.extend([
        "Global",
        "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution",
        "\t\tDebug|Any CPU = Debug|Any CPU",
        "\t\tRelease|Any CPU = Release|Any CPU",
        "\tEndGlobalSection",
        "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution",
    ])
    for path in sorted(item_projects, key=lambda path: (path != item_roots[0], path)):
        project_guid = guid(path)
        for configuration in ("Debug", "Release"):
            lines.extend([
                f"\t\t{project_guid}.{configuration}|Any CPU.ActiveCfg = {configuration}|Any CPU",
                f"\t\t{project_guid}.{configuration}|Any CPU.Build.0 = {configuration}|Any CPU",
            ])
    lines.extend(["\tEndGlobalSection", "EndGlobal", ""])
    outputs[ROOT / "ItemSeeder.sln"] = "\n".join(lines)
    return outputs


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true")
    args = parser.parse_args()
    stale = []
    for path, content in generate().items():
        if path.exists() and path.read_text(encoding="utf-8-sig") == content:
            continue
        stale.append(path.name)
        if not args.check:
            path.write_text(content, encoding="utf-8", newline="\n")
    if args.check and stale:
        raise SystemExit("Run python scripts/update-development-solutions.py; stale files: " + ", ".join(stale))
    print("Development solution layout is current." if not stale else "Updated: " + ", ".join(stale))
