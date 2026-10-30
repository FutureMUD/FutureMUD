"""Build a private world-content ArmSolo package. Never execute the historical code.

Usage: python scripts/armageddon-worldfile-package.py --source <ArmSolo-root> --output <directory>
The source directory and generated output are private local inputs, never repository assets.
The seeder reads the generated files from FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY.
"""

import argparse
import ast
import gzip
import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path
from collections import Counter


class Reader:
    def __init__(self, path, root):
        self.path = path.relative_to(root).as_posix()
        self.raw = path.read_bytes()
        # Latin-1 preserves every source byte; canonical output is UTF-8 JSON.
        self.text = self.raw.decode("latin-1").replace("\r\n", "\n").replace("\r", "\n")
        self.pos = 0
        self.diagnostics = []

    def location(self):
        return {"Path": self.path, "Line": self.text.count("\n", 0, self.pos) + 1}

    def whitespace(self):
        while self.pos < len(self.text) and self.text[self.pos].isspace():
            self.pos += 1

    def line(self):
        end = self.text.find("\n", self.pos)
        if end < 0:
            end = len(self.text)
        result = self.text[self.pos:end]
        self.pos = min(end + 1, len(self.text))
        return result

    def string(self):
        start = self.pos
        while self.pos < len(self.text):
            line_start = self.pos
            line = self.line()
            if line.rstrip().endswith("~"):
                end = line_start + len(line.rstrip()) - 1
                return self.text[start:end]
        raise ValueError(f"{self.path}: unterminated string at offset {start}")

    def pipe(self):
        end = self.text.find("|", self.pos)
        if end < 0:
            raise ValueError(f"{self.path}: missing pipe at {self.pos}")
        result = self.text[self.pos:end]
        self.pos = end + 1
        return result

    def programs(self):
        result = []
        while True:
            value = self.string()
            if value.strip() == "ep":
                return result
            result.append(value)

    def integers(self, expected=None):
        values = [int(x) for x in self.line().split()]
        if expected is not None and len(values) != expected:
            raise ValueError(f"{self.path}: expected {expected} integers, got {values}")
        return values

    def numbers(self, count, convert=int):
        result = []
        for _ in range(count):
            self.whitespace()
            token = re.match(r"\S+", self.text[self.pos:])
            if token is None:
                raise ValueError(f"{self.location()}: premature end of numeric fields")
            try:
                result.append(convert(token.group()))
            except ValueError as error:
                raise ValueError(f"{self.location()}: invalid number {token.group()!r} at field {len(result) + 1}/{count}") from error
            self.pos += len(token.group())
        self.whitespace()
        return result


def prototypes(reader, npc):
    result = []
    while True:
        reader.whitespace()
        location = reader.location()
        header = reader.line().strip()
        if not header or header.startswith("$"):
            return result
        if not re.fullmatch(r"#\d+\s+-?\d+", header):
            raise ValueError(f"{location}: unexpected prototype header {header!r}")
        vnum, limit = map(int, header[1:].split())
        item = {"Vnum": vnum, "Limit": limit, "Source": location,
                "Name": reader.string(), "ShortDescription": reader.string(),
                "LongDescription": reader.string(), "FullDescription": reader.string()}
        if npc:
            item["Background"] = reader.string()
            flags = reader.line().split()
            if len(flags) != 5 or flags[-1] != "S":
                raise ValueError(f"{location}: invalid NPC flags/format marker {flags}")
            item["Flags"] = list(map(int, flags[:4]))
            item["Combat"] = reader.integers(6)
            item["Money"] = reader.integers(2)
            item["Identity"] = reader.integers(7)
            item["Age"] = -1
            item["Skills"] = []
            item["Clans"] = []
        else:
            flags = reader.integers()
            if len(flags) == 4:
                flags.insert(3, 0)
            if len(flags) != 5:
                raise ValueError(f"{location}: invalid item type/flags/material {flags}")
            item["Flags"] = flags
            item["Values"] = reader.integers(6)
            item["Physical"] = reader.integers(3)
            item["Damage"] = reader.integers(4)
            item["Affects"] = []
        item["Programs"] = reader.programs()
        item["ExtraDescriptions"] = []
        while True:
            reader.whitespace()
            if reader.pos == len(reader.text) or reader.text[reader.pos] in "#$":
                break
            marker = reader.line().split()
            if not marker:
                continue
            if marker[0] == "E":
                item["ExtraDescriptions"].append({"Keyword": reader.string(), "Description": reader.string()})
            elif not npc and marker[0] == "A":
                item["Affects"].append(list(map(int, marker[1:])) if len(marker) > 1 else reader.integers(2))
            elif npc and marker[0] == "S":
                item["Skills"].append(list(map(int, marker[1:])))
            elif npc and marker[0] == "x":
                if len(marker) != 3 or marker[1] != "age":
                    raise ValueError(f"{reader.location()}: unsupported NPC extra field {marker}")
                item["Age"] = int(marker[2])
            elif npc and marker[0] == "C":
                # The marker line has no inline fields in the active writer.
                item["Clans"].append({"Name": reader.string(), "Flags": reader.string(), "Rank": reader.integers(1)[0]})
            else:
                raise ValueError(f"{reader.location()}: unsupported prototype suffix {marker}")
        result.append(item)


def rooms(reader, zone):
    result = []
    while True:
        reader.whitespace()
        location = reader.location()
        marker = reader.pipe().strip()
        if marker == "x":
            return result
        if marker != "n":
            raise ValueError(f"{location}: unexpected room marker {marker!r}")
        item = {"Source": location, "Zone": zone, "Name": reader.pipe(),
                "Vnum": int(reader.pipe()), "Sector": int(reader.pipe()),
                "Flags": int(reader.pipe()), "Background": int(reader.pipe()),
                "Exits": [], "ExtraDescriptions": [], "ObservationLinks": [], "City": 0}
        reader.whitespace()
        item["Programs"] = reader.programs()
        item["Description"] = reader.string()
        while True:
            reader.whitespace()
            if reader.text.startswith(("n|", "x|"), reader.pos):
                break
            marker = reader.pipe().strip()
            if marker == "c":
                item["City"] = int(reader.pipe())
            elif marker == "d":
                item["Exits"].append({"Direction": int(reader.pipe()), "Destination": int(reader.pipe()),
                                      "Keyword": reader.pipe(), "Flags": int(reader.pipe()),
                                      "Key": int(reader.pipe()), "Description": reader.string()})
            elif marker == "e":
                item["ExtraDescriptions"].append({"Keyword": reader.pipe(), "Description": reader.string()})
            elif marker in ("w", "l"):
                item["ObservationLinks"].append({"Type": marker, "Room": int(reader.pipe()), "Text": reader.pipe()})
            else:
                raise ValueError(f"{reader.location()}: unknown room suffix {marker!r}")
        result.append(item)


def resets(reader, zone):
    result = []
    while reader.pos < len(reader.text):
        reader.whitespace()
        location = reader.location()
        line = reader.line().strip()
        if not line or line.startswith("*"):
            continue
        marker = line[0]
        if marker in "S$":
            break
        if marker in "XY":
            # String payloads begin on the next line in the active writer.
            fields = list(map(int, line[1:].split()))
            if len(fields) != 2:
                raise ValueError(f"{location}: unexpected X/Y header")
            result.append({"Source": location, "Zone": zone, "Opcode": marker,
                           "Arguments": fields, "Strings": [reader.string(), reader.string()]})
        else:
            fields = list(map(int, line[1:].split()))
            expected = {"M": 4, "N": 4, "D": 4, "G": 10, "O": 11, "E": 11, "P": 11, "A": 11}
            if marker not in expected or len(fields) != expected[marker]:
                raise ValueError(f"{location}: unexpected reset {line!r}")
            result.append({"Source": location, "Zone": zone, "Opcode": marker, "Arguments": fields, "Strings": []})
    return result


def zones(reader):
    result = []
    while True:
        reader.whitespace()
        if reader.pos == len(reader.text):
            return result
        location = reader.location()
        marker = reader.line().strip()
        if marker.startswith("$"):
            return result
        if not marker.startswith("#"):
            raise ValueError(f"{location}: invalid zone header")
        name = reader.string()
        fields = reader.integers(7)
        privileges = []
        while True:
            value = reader.string()
            if value.startswith("end"):
                break
            privileges.append(value)
        result.append({"Source": location, "Index": len(result), "Header": int(marker[1:]),
                       "Name": name, "Fields": fields, "Privileges": privileges})


def races(reader):
    result = []
    while True:
        reader.whitespace()
        if reader.pos == len(reader.text):
            return result
        location = reader.location()
        name, abbreviation = reader.string(), reader.string()
        stats = re.findall(r"(?:STRENGTH|AGILITY|WISDOM|ENDURANCE):\s*([^\s]+)", reader.line())
        if len(stats) != 4:
            raise ValueError(f"{location}: missing race stat expressions")
        combat, attack, identity = reader.integers(3), reader.line().strip(), reader.integers(3)
        skins = []
        skin_ids = reader.integers(5)
        for vnum in skin_ids:
            if vnum != -1:
                skins.append({"Vnum": vnum, "ActorEcho": reader.string(), "RoomEcho": reader.string()})
        result.append({"Source": location, "Index": len(result) + 1, "Name": name, "Abbreviation": abbreviation,
                       "Stats": stats, "Combat": combat, "Attack": attack, "Identity": identity, "Skins": skins})


def clans(reader):
    result = []
    while True:
        reader.whitespace()
        if reader.pos == len(reader.text):
            return result
        location = reader.location()
        name, fields = reader.string(), reader.integers(4)
        relations = []
        while True:
            marker = reader.line().strip()
            if not marker.startswith("R"):
                if marker != "~":
                    raise ValueError(f"{location}: unknown clan sentinel {marker!r}")
                break
            relations.append(list(map(int, marker[1:].split())))
        result.append({"Source": location, "Index": len(result), "Name": name, "Fields": fields,
                       "Relations": relations, "Ranks": ["a member"]})


def shops(reader):
    count = reader.integers(1)[0]
    result = []
    for index in range(count):
        location = reader.location()
        repairs = []

        def integer_array(size, padding, label):
            reader.whitespace()
            start = reader.location()
            previous = reader.pos
            tokens = reader.line().split()
            if not all(re.fullmatch(r"-?\d+", x) for x in tokens):
                if label == "NewItems" and len(tokens) == 5 and any("." in x for x in tokens) and all(re.fullmatch(r"\d+(?:\.\d+)?", x) for x in tokens):
                    reader.pos = previous
                    repairs.append({"Source": start, "Field": label, "Original": [], "MissingSlots": size, "Padding": padding})
                    return [padding] * size
                raise ValueError(f"{start}: missing/ambiguous {label} array before {tokens}")
            fields = list(map(int, tokens))
            if len(fields) > size and all(x == padding for x in fields[size:]):
                repairs.append({"Source": start, "Field": label, "Original": fields.copy(), "DiscardedEmptySlots": len(fields) - size})
                return fields[:size]
            if not fields or len(fields) > size:
                raise ValueError(f"{start}: ambiguous {label} array")
            while len(fields) < size:
                previous = reader.pos
                extra = reader.line().split()
                if extra and len(extra) <= size - len(fields) and all(re.fullmatch(r"-?\d+", x) for x in extra):
                    fields.extend(map(int, extra))
                    continue
                reader.pos = previous
                missing = size - len(fields)
                repairs.append({"Source": start, "Field": label, "Original": fields.copy(),
                                "MissingSlots": missing, "Padding": padding})
                fields.extend([padding] * missing)
            return fields

        # Preserve explicit row boundaries for malformed short arrays. Without
        # this declared repair C fscanf consumes the following array as this one.
        producing = integer_array(15, -1, "Producing")
        new_items = integer_array(20, 0, "NewItems")
        buy, sell = reader.numbers(5, float), reader.numbers(5, float)
        buy_types = integer_array(5, 0, "BuyTypes")
        sell_types = integer_array(5, 0, "SellTypes")
        messages = [reader.string() for _ in range(8)]
        keeper, opening, closing = reader.numbers(3)
        result.append({"Source": location, "Index": index, "Producing": producing, "NewItems": new_items,
                       "BuyMultipliers": buy, "SellMultipliers": sell, "BuyTypes": buy_types, "SellTypes": sell_types,
                       "Messages": messages, "Keeper": keeper, "OpenHour": opening, "CloseHour": closing, "Repairs": repairs})
    reader.whitespace()
    if reader.pos != len(reader.text):
        reader.diagnostics.append({"Type": "ShopTailOutsideDeclaredCount", "Source": reader.location(),
                                   "DeclaredCount": count, "Disposition": "Archive only, source loader does not read it"})
    return result


def forage(reader):
    result = []
    while True:
        reader.whitespace()
        if reader.pos == len(reader.text):
            return result
        location = reader.location()
        category, sector = reader.string(), reader.string()
        fields = re.fullmatch(r"VNUM:\s*(-?\d+)\s+SHARES:\s*(-?\d+)\s+RARE:\s*(-?\d+)\s*", reader.line())
        if fields is None:
            raise ValueError(f"{location}: invalid forage fields")
        vnum, shares, rarity = map(int, fields.groups())
        result.append({"Source": location, "Category": category, "Sector": sector, "Vnum": vnum,
                       "Shares": shares, "Rarity": rarity})


def guilds(reader, first_index):
    result = []
    while True:
        reader.whitespace()
        if reader.pos == len(reader.text):
            return result
        location = reader.location()
        name, abbreviation = reader.string(), reader.string()
        stats, combat, player = reader.integers(4), reader.integers(4), reader.integers(1)[0]
        skills = []
        while True:
            skill = reader.string()
            if skill.startswith("S"):
                break
            prerequisite = reader.string()
            maximum, starting = reader.integers(2)
            skills.append({"Name": skill, "Prerequisite": prerequisite, "Maximum": maximum, "Starting": starting})
        result.append({"Source": location, "Index": len(result) + first_index, "Name": name,
                       "Abbreviation": abbreviation, "Stats": stats, "Combat": combat, "Player": player, "Skills": skills})


def integer_distribution(expression):
    """Evaluate only the reviewed generator grammar, preserving C integer division."""
    def visit(node):
        if isinstance(node, ast.Constant) and type(node.value) is int:
            return Counter({node.value: 1})
        if isinstance(node, ast.Call) and isinstance(node.func, ast.Name) and node.func.id == "dice" and len(node.args) == 2 and not node.keywords:
            if not all(isinstance(x, ast.Constant) and type(x.value) is int for x in node.args):
                raise ValueError("Dice arguments must be integer constants")
            count, sides = (x.value for x in node.args)
            if not 1 <= count <= 10 or not 1 <= sides <= 1000:
                raise ValueError("Generator dice exceed the reviewed bounds")
            values = Counter({0: 1})
            for _ in range(count):
                following = Counter()
                for current, weight in values.items():
                    for face in range(1, sides + 1):
                        following[current + face] += weight
                values = following
            return values
        if isinstance(node, ast.UnaryOp) and isinstance(node.op, (ast.USub, ast.UAdd)):
            return Counter({(-value if isinstance(node.op, ast.USub) else value): weight for value, weight in visit(node.operand).items()})
        if isinstance(node, ast.BinOp) and isinstance(node.op, (ast.Add, ast.Sub, ast.Mult, ast.Div)):
            result = Counter()
            for left, lw in visit(node.left).items():
                for right, rw in visit(node.right).items():
                    if isinstance(node.op, ast.Add): value = left + right
                    elif isinstance(node.op, ast.Sub): value = left - right
                    elif isinstance(node.op, ast.Mult): value = left * right
                    else:
                        if right == 0: raise ValueError("Division by zero")
                        value = (abs(left) // abs(right)) * (-1 if (left < 0) != (right < 0) else 1)
                    result[value] += lw * rw
            return result
        raise ValueError(f"Unreviewed physical generator syntax: {ast.dump(node)}")
    return visit(ast.parse(expression, mode="eval").body)


def physical_generators(utility, headers, race_ids):
    constants = {name: int(value) for name, value in re.findall(r"^\s*#define\s+(RACE_[A-Z0-9_]+)\s+(\d+)\b", headers, re.M)}
    maps = []
    for function, variable in (("char_init_height", "ht"), ("char_init_weight", "wt")):
        match = re.search(r"\b" + function + r"\(int race\)\s*\{(.*?)\n\}", utility, re.S)
        if match is None: raise ValueError(f"Missing physical generator {function}")
        body = re.sub(r"/\*.*?\*/|//[^\n]*", "", match.group(1), flags=re.S)
        body = body.split("switch (race) {", 1)[1]
        cases, default = {}, None
        for group in body.split("break;"):
            assignment = re.search(r"\b" + variable + r"\s*=\s*([^;]+);", group)
            if assignment is None: continue
            expression = assignment.group(1).strip()
            distribution = integer_distribution(expression)
            entry = {"Expression": expression, "Minimum": min(distribution), "Maximum": max(distribution),
                     "Mean": sum(value * weight for value, weight in distribution.items()) / sum(distribution.values())}
            for name in re.findall(r"case\s+(RACE_[A-Z0-9_]+)\s*:", group):
                if name not in constants: raise ValueError(f"Undefined physical generator constant {name}")
                cases[constants[name]] = entry
            if re.search(r"\bdefault\s*:", group): default = entry
        if default is None: raise ValueError(f"Missing explicit default in {function}")
        maps.append((cases, default))
    return [{"Race": race, "Height": maps[0][0].get(race, maps[0][1]),
             "Weight": maps[1][0].get(race, maps[1][1])} for race in sorted(set(race_ids))]


def first_loaded(records, entity, diagnostics):
    winners = {}
    for item in records:
        if item["Vnum"] in winners:
            diagnostics.append({"Type": "DuplicateFirstWins", "Entity": entity,
                "Vnum": item["Vnum"], "Winner": winners[item["Vnum"]]["Source"], "Discarded": item["Source"]})
        else:
            winners[item["Vnum"]] = item
    return list(winners.values())


def build(source, output):
    root = source / "armExe" / "lib"
    package = {"Version": 1, "Encoding": "latin-1", "Rooms": [], "Items": [], "Npcs": [], "Resets": [], "SupportFiles": []}
    manifest = {"Version": 1, "Files": [], "Diagnostics": [], "Excluded": ["accounts", "player/runtime state", "private messages and boards", "logs", "backups", "unselected scripts"]}
    expected_code = {"db.c": "70cd96804c4c932e08b1d772b1445936a1a06d9b69c1194ff483b7dfea367dc5",
                     "core_structs.h": "c39a953310ce40f2a3d92c1321d106dd57fc66df341b0847e186995fa7b36c88",
                     "barter.c": "77061ff3fd77b122d4fe4d737b471045fc688b929deeb88a9a88e7b54b5682d5",
                     "constants.c": "54b95a366a4b5b9735be2e5a12b7f6e3fe21065efd2db706f7cea99da68bb7e6",
                     "utility.c": "43124504cd27f587ea4397a6d3be53e8c5a074e062e4bb9dae7e20f663fb5609",
                     "guilds.h": "86815424d9beace84bd2379d35935f424407f4eef2435a632e5858b526b46fbe"}
    for path, expected in expected_code.items():
        actual = hashlib.sha256((source / path).read_bytes()).hexdigest()
        if actual != expected:
            raise ValueError(f"Unqualified ArmSolo source version: {path} does not match the reviewed code")
    manifest["SourceCodeSha256"] = expected_code
    families = (("areas", "wld", "Rooms", rooms), ("objects", "obj", "Items", lambda r, z: prototypes(r, False)),
                ("npcs", "npc", "Npcs", lambda r, z: prototypes(r, True)), ("cmds", "cmd", "Resets", resets))
    for directory, extension, key, parse in families:
        paths = sorted((root / directory).glob(f"z*.{extension}"), key=lambda p: int(p.stem[1:]))
        for path in paths:
            reader = Reader(path, root)
            records = parse(reader, int(path.stem[1:]))
            package[key].extend(records)
            manifest["Files"].append({"Path": reader.path, "Sha256": hashlib.sha256(reader.raw).hexdigest(), "Bytes": len(reader.raw), "Records": len(records)})
    support = ("zones", "races", "guilds", "subguilds", "crafting.xml", "forage", "clans", "cities", "shops", "weather")
    paths = [root / "data_files" / name for name in support]
    paths += sorted((root / "background").glob("z*.bdesc"), key=lambda p: int(p.stem[1:]))
    paths += sorted((root / "data_files" / "clan_ranks").glob("clan_*.ranks"))
    for path in paths:
        reader = Reader(path, root)
        package["SupportFiles"].append({"Path": reader.path, "Text": reader.text})
        manifest["Files"].append({"Path": reader.path, "Sha256": hashlib.sha256(reader.raw).hexdigest(), "Bytes": len(reader.raw)})
    for name, key, parse in (("zones", "Zones", zones), ("races", "Races", races), ("clans", "Clans", clans),
                              ("shops", "Shops", shops), ("forage", "Forage", forage)):
        reader = Reader(root / "data_files" / name, root)
        package[key] = parse(reader)
        manifest["Diagnostics"].extend(reader.diagnostics)
    package["Guilds"] = guilds(Reader(root / "data_files" / "guilds", root), 1)
    package["Subguilds"] = guilds(Reader(root / "data_files" / "subguilds", root), 0)
    constants = (source / "constants.c").read_text(encoding="latin-1")
    table = re.search(r"\bskill_name\s*\[[^]]*\]\s*=\s*\{(.*?)\};", constants, re.S)
    if table is None:
        raise ValueError("Cannot locate this version's skill_name array")
    body = re.sub(r"/\*.*?\*/|//[^\n]*", "", table.group(1), flags=re.S)
    package["Skills"] = [ast.literal_eval(x) for x in re.findall(r'"(?:\\.|[^"\\])*"', body)]
    manifest["SourceCodeSha256"]["constants.c"] = hashlib.sha256((source / "constants.c").read_bytes()).hexdigest()
    package["PhysicalGenerators"] = physical_generators((source / "utility.c").read_text(encoding="latin-1"),
        (source / "guilds.h").read_text(encoding="latin-1"), [x["Identity"][6] for x in package["Npcs"]])
    for name in ("utility.c", "guilds.h"):
        manifest["SourceCodeSha256"][name] = hashlib.sha256((source / name).read_bytes()).hexdigest()
    for shop in package["Shops"]:
        for repair in shop["Repairs"]:
            manifest["Diagnostics"].append({"Type": "AuthoredShopArrayRepair", "Shop": shop["Index"], **repair})
    manifest["RawCounts"] = {"Clans": len(package["Clans"])}
    if len(package["Clans"]) > 57:
        manifest["Diagnostics"].append({"Type": "ClansOutsideLoadedRange", "Indices": [x["Index"] for x in package["Clans"][57:]],
                                       "Disposition": "Archive only: MAX_CLAN_FROM_FILE is 57"})
        package["Clans"] = package["Clans"][:57]
    for clan in package["Clans"]:
        ranks = root / "data_files" / "clan_ranks" / f"clan_{clan['Index']}.ranks"
        if ranks.exists():
            reader = Reader(ranks, root)
            count = reader.integers(1)[0]
            clan["Ranks"] = [reader.string() for _ in range(count)] if count > 0 else ["a member"]
    xml = ET.fromstring((root / "data_files" / "crafting.xml").read_bytes())
    package["Crafts"] = [{child.tag: child.text or "" for child in item} for item in xml.findall("ITEM")]
    for key in ("Items", "Npcs"):
        manifest["RawCounts"][key] = len(package[key])
        package[key] = first_loaded(package[key], key, manifest["Diagnostics"])
    room_ids = {x["Vnum"] for x in package["Rooms"]}
    manifest["RawCounts"]["Exits"] = sum(len(x["Exits"]) for x in package["Rooms"])
    for room in package["Rooms"]:
        for link in room["Exits"]:
            if link["Destination"] not in room_ids:
                manifest["Diagnostics"].append({"Type": "MissingExitOmitted", "Room": room["Vnum"], "Destination": link["Destination"], "Source": room["Source"]})
        room["Exits"] = [x for x in room["Exits"] if x["Destination"] in room_ids]
    manifest["EffectiveExits"] = sum(len(x["Exits"]) for x in package["Rooms"])
    item_ids = {x["Vnum"] for x in package["Items"]}
    npc_ids = {x["Vnum"] for x in package["Npcs"]}
    for command in package["Resets"]:
        opcode, arguments = command["Opcode"], command["Arguments"]
        issues = []
        if opcode == "M" and arguments[1] not in npc_ids:
            issues.append(f"NPC Vnum{arguments[1]} does not exist; malformed historical M can index invalid memory")
        if opcode in ("O", "G", "E", "P", "A") and arguments[1] not in item_ids:
            issues.append(f"Item Vnum{arguments[1]} does not exist")
        if opcode in ("M", "O", "D") and arguments[3 if opcode != "D" else 1] not in room_ids:
            issues.append("Reset room does not exist")
        command["ReferenceIssues"] = issues
        if issues:
            manifest["Diagnostics"].append({"Type": "InvalidResetReference", "Source": command["Source"], "Issues": issues})
    for recipe in package["Crafts"]:
        outputs = list(map(int, recipe.get("INTO_SUCCESS", "").split()))
        recipe["ReferenceIssues"] = []
        if not outputs or outputs[0] not in item_ids:
            recipe["ReferenceIssues"].append("First success output is absent; the historical craft aborts and restores inputs")
            manifest["Diagnostics"].append({"Type": "UnusableCraft", "Name": recipe.get("NAME"), "Issues": recipe["ReferenceIssues"]})
    manifest["Counts"] = {key: len(package[key]) for key in ("Zones", "Rooms", "Items", "Npcs", "Resets", "Races", "Clans", "Shops", "Forage", "Crafts")}
    encoded = json.dumps(package, ensure_ascii=True, separators=(",", ":")).encode("utf-8")
    compressed = gzip.compress(encoded, mtime=0)
    manifest["PackageSha256"] = hashlib.sha256(compressed).hexdigest()
    output.mkdir(parents=True, exist_ok=True)
    (output / "ArmageddonWorldfiles.json.gz").write_bytes(compressed)
    (output / "ArmageddonWorldfiles.manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"Counts": manifest["Counts"], "Files": len(manifest["Files"]), "Diagnostics": len(manifest["Diagnostics"]), "Bytes": len(compressed), "Sha256": manifest["PackageSha256"]}))


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    repository = Path(__file__).resolve().parent.parent
    if args.output.resolve() == repository or repository in args.output.resolve().parents:
        parser.error("ArmSolo exports must be written outside the repository; use a private local data directory.")
    build(args.source.resolve(), args.output.resolve())
