"""Refresh IDE filters and serialize memory-heavy helpers without runtime references.

Run from any directory; --check validates generated files without writing them.
Only standard-library Python is required on Windows or POSIX.
"""

import argparse
import json
from pathlib import Path
import re
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
    for path in entries:
        project = ROOT / path
        refs = set()
        for reference in ET.parse(project).iter("ProjectReference"):
            pattern = reference.attrib["Include"].replace("\\", "/")
            if "$" in pattern:
                raise ValueError(f"Unsupported computed reference in {path}: {pattern}")
            targets = list(project.parent.glob(pattern))
            if not targets:
                raise ValueError(f"Missing project reference in {path}: {pattern}")
            for target in targets:
                refs.add(target.resolve().relative_to(ROOT).as_posix())
        # Imported migration targets add the build-only compiler dependency.
        if path.startswith("MigrationModels/") or path == "MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj":
            refs.add("BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj")
        missing = refs - entries.keys()
        if missing:
            raise ValueError(f"Solution is missing dependencies of {path}: {sorted(missing)}")
        dependencies[path] = refs

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
    heavy = [p for p in ordered if p.startswith((
        "MigrationModels/Batch", "DatabaseSeeder/Seeders/ItemSeeder/Catalogue/Assemblies/"
    ))]
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
